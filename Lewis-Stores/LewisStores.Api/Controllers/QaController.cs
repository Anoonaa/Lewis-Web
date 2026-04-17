using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LewisStores.Api.Data;

namespace LewisStores.Api.Controllers
{
    /// <summary>
    /// QA training endpoints for inspecting scenario flags and audit events.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class QaController : ControllerBase
    {
        private readonly AppDbContext _context;

        public QaController(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Returns all QA feature flags with current state.
        /// </summary>
        [HttpGet("flags")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetFlags()
        {
            var flags = await _context.QaFeatureFlags
                .OrderBy(f => f.Key)
                .Select(f => new
                {
                    f.Key,
                    f.Description,
                    f.IsEnabled,
                    f.UpdatedAtUtc
                })
                .ToListAsync();

            return Ok(flags);
        }

        /// <summary>
        /// Toggle a feature flag for QA scenario control.
        /// </summary>
        /// <param name="key">Feature flag key.</param>
        /// <param name="request">Toggle payload.</param>
        [HttpPut("flags/{key}")]
        [Authorize(Roles = "Admin,Manager,QaTester")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateFlag(string key, [FromBody] UpdateFlagRequest request)
        {
            var flag = await _context.QaFeatureFlags.FirstOrDefaultAsync(f => f.Key == key);
            if (flag == null)
            {
                return NotFound(new { Message = $"Feature flag '{key}' was not found." });
            }

            flag.IsEnabled = request.IsEnabled;
            flag.UpdatedAtUtc = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return Ok(new
            {
                flag.Key,
                flag.Description,
                flag.IsEnabled,
                flag.UpdatedAtUtc
            });
        }

        /// <summary>
        /// Returns recent audit events for student investigations.
        /// </summary>
        /// <param name="take">Maximum number of events (default 100).</param>
        /// <param name="eventType">Optional event type filter.</param>
        [HttpGet("audit")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAuditLogs([FromQuery] int take = 100, [FromQuery] string? eventType = null)
        {
            take = Math.Clamp(take, 1, 500);

            var query = _context.AuditLogs.AsQueryable();
            if (!string.IsNullOrWhiteSpace(eventType))
            {
                query = query.Where(a => a.EventType == eventType);
            }

            var logs = await query
                .OrderByDescending(a => a.TimestampUtc)
                .Take(take)
                .Select(a => new
                {
                    a.Id,
                    a.TimestampUtc,
                    a.EventType,
                    a.UserId,
                    a.Severity,
                    a.Details
                })
                .ToListAsync();

            return Ok(logs);
        }

        /// <summary>
        /// Returns pre-defined scenario packs for instructor-led training.
        /// </summary>
        [HttpGet("scenario-packs")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetScenarioPacks()
        {
            var packs = new[]
            {
                new
                {
                    Key = "happy_path_baseline",
                    Title = "Happy Path Baseline",
                    Description = "Disables intentional defects for baseline validation and regression checks.",
                    FlagSettings = new Dictionary<string, bool>
                    {
                        ["product_duplicate_in_list"] = false,
                        ["order_total_mismatch"] = false,
                        ["auth_email_case_sensitive"] = false,
                        ["returns_refund_delay"] = false,
                        ["support_assignment_conflict"] = false,
                        ["audit_verbose_events"] = true
                    },
                    FocusAreas = new[] { "smoke", "regression", "core checkout" }
                },
                new
                {
                    Key = "data_integrity_hunt",
                    Title = "Data Integrity Hunt",
                    Description = "Enables dataset and totals inconsistencies for investigation exercises.",
                    FlagSettings = new Dictionary<string, bool>
                    {
                        ["product_duplicate_in_list"] = true,
                        ["order_total_mismatch"] = true,
                        ["auth_email_case_sensitive"] = true,
                        ["returns_refund_delay"] = true,
                        ["support_assignment_conflict"] = false,
                        ["audit_verbose_events"] = true
                    },
                    FocusAreas = new[] { "data quality", "api contract", "consistency checks" }
                },
                new
                {
                    Key = "support_ops_breakdown",
                    Title = "Support Ops Breakdown",
                    Description = "Simulates support workflow friction in assignment and refund processes.",
                    FlagSettings = new Dictionary<string, bool>
                    {
                        ["product_duplicate_in_list"] = false,
                        ["order_total_mismatch"] = false,
                        ["auth_email_case_sensitive"] = false,
                        ["returns_refund_delay"] = true,
                        ["support_assignment_conflict"] = true,
                        ["audit_verbose_events"] = true
                    },
                    FocusAreas = new[] { "support triage", "returns", "ops escalation" }
                }
            };

            return Ok(packs);
        }

        /// <summary>
        /// Applies a scenario pack by updating grouped feature flags.
        /// </summary>
        [HttpPost("scenario-packs/{key}/apply")]
        [Authorize(Roles = "Admin,Manager,QaTester")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ApplyScenarioPack(string key)
        {
            var packs = new Dictionary<string, Dictionary<string, bool>>(StringComparer.OrdinalIgnoreCase)
            {
                ["happy_path_baseline"] = new()
                {
                    ["product_duplicate_in_list"] = false,
                    ["order_total_mismatch"] = false,
                    ["auth_email_case_sensitive"] = false,
                    ["returns_refund_delay"] = false,
                    ["support_assignment_conflict"] = false,
                    ["audit_verbose_events"] = true
                },
                ["data_integrity_hunt"] = new()
                {
                    ["product_duplicate_in_list"] = true,
                    ["order_total_mismatch"] = true,
                    ["auth_email_case_sensitive"] = true,
                    ["returns_refund_delay"] = true,
                    ["support_assignment_conflict"] = false,
                    ["audit_verbose_events"] = true
                },
                ["support_ops_breakdown"] = new()
                {
                    ["product_duplicate_in_list"] = false,
                    ["order_total_mismatch"] = false,
                    ["auth_email_case_sensitive"] = false,
                    ["returns_refund_delay"] = true,
                    ["support_assignment_conflict"] = true,
                    ["audit_verbose_events"] = true
                }
            };

            if (!packs.TryGetValue(key, out var settings))
            {
                return NotFound(new { Message = $"Scenario pack '{key}' was not found." });
            }

            var flags = await _context.QaFeatureFlags.Where(f => settings.Keys.Contains(f.Key)).ToListAsync();
            foreach (var flag in flags)
            {
                flag.IsEnabled = settings[flag.Key];
                flag.UpdatedAtUtc = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();

            return Ok(new
            {
                AppliedPack = key,
                UpdatedFlags = flags.Select(f => new
                {
                    f.Key,
                    f.IsEnabled,
                    f.UpdatedAtUtc
                })
            });
        }

        public class UpdateFlagRequest
        {
            public bool IsEnabled { get; set; }
        }
    }
}
