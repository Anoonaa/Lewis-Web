using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using LewisStores.Api.Data;
using LewisStores.Api.Models;

namespace LewisStores.Api.Controllers
{
    /// <summary>
    /// Handles order retrieval and creation endpoints.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class OrdersController : ControllerBase
    {
        private readonly AppDbContext _context;

        public class CreateOrderRequest
        {
            public decimal Total { get; set; }
            public string Items { get; set; } = string.Empty;
        }

        public OrdersController(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Returns all orders.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Order>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<Order>>> GetOrders()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId))
            {
                return Unauthorized();
            }

            var orders = await _context.Orders
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.Date)
                .ToListAsync();

            var mismatchDefectEnabled = await _context.QaFeatureFlags
                .Where(f => f.Key == "order_total_mismatch")
                .Select(f => f.IsEnabled)
                .FirstOrDefaultAsync();

            if (mismatchDefectEnabled && orders.Count > 0)
            {
                orders[0].Total += 1.11m;
            }

            return orders;
        }

        /// <summary>
        /// Creates a new order and returns the created resource.
        /// </summary>
        /// <param name="request">Order payload.</param>
        [HttpPost]
        [ProducesResponseType(typeof(Order), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Order>> CreateOrder([FromBody] CreateOrderRequest request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId))
            {
                return Unauthorized();
            }

            if (request.Total <= 0)
            {
                return BadRequest(new { Message = "Order total must be greater than zero." });
            }

            var order = new Order
            {
                Id = "LWS-" + Random.Shared.Next(10000, 99999),
                Date = DateTime.UtcNow.ToString("dd MMM yyyy"),
                Status = "Processing",
                Total = request.Total,
                UserId = userId,
                Items = request.Items
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            var verboseAudit = await _context.QaFeatureFlags
                .Where(f => f.Key == "audit_verbose_events")
                .Select(f => f.IsEnabled)
                .FirstOrDefaultAsync();

            if (verboseAudit)
            {
                _context.AuditLogs.Add(new AuditLog
                {
                    TimestampUtc = DateTime.UtcNow,
                    EventType = "order.created",
                    UserId = userId,
                    Severity = "Info",
                    Details = $"{{\"orderId\":\"{order.Id}\",\"total\":{order.Total}}}"
                });
                await _context.SaveChangesAsync();
            }

            return CreatedAtAction(nameof(GetOrders), new { id = order.Id }, order);
        }
    }
}
