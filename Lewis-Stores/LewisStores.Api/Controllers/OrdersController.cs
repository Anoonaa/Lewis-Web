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
    // [Authorize] - Commented out to allow testing without auth first, can enable if strict auth needed
    public class OrdersController : ControllerBase
    {
        private readonly AppDbContext _context;

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
            return await _context.Orders.ToListAsync();
        }

        /// <summary>
        /// Creates a new order and returns the created resource.
        /// </summary>
        /// <param name="order">Order payload.</param>
        [HttpPost]
        [ProducesResponseType(typeof(Order), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Order>> CreateOrder([FromBody] Order order)
        {
            // Simple mock order creation
            order.Id = "LWS-" + new Random().Next(10000, 99999);
            order.Date = DateTime.UtcNow.ToString("dd MMM yyyy");
            order.Status = "Processing";

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetOrders), new { id = order.Id }, order);
        }
    }
}
