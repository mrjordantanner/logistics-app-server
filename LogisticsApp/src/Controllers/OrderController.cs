using LogisticsApp.Models;
using LogisticsApp.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LogisticsApp.Controllers;

[Route("api/[controller]")]
[ApiController]
public class OrderController : ControllerBase
{
    private readonly IOrderService _orderService;
    private readonly ILogger<OrderController> _logger;

    public OrderController(IOrderService orderService, ILogger<OrderController> logger)
    {
        _orderService = orderService;
        _logger = logger;
    }

    // POST: api/Order
    [HttpPost]
    public async Task<IActionResult> CreateOrder([FromBody] OrderDto orderDto)
    {
        try
        {
            if (orderDto == null)
            {
                return BadRequest("Order data is null");
            }

            var order = await _orderService.CreateOrderWithItems(orderDto);

            if (order == null)
            {
                _logger.LogWarning("Failed to create order for provided data.");
                return BadRequest("Failed to create the order.");
            }

            _logger.LogInformation("Order created successfully with ID: {OrderId}", order.Id);
            return CreatedAtAction(nameof(GetOrderById), new { id = order.Id }, order);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error occurred while creating order: {ex.Message}");
            return StatusCode(500, "Internal server error");
        }
    }

    // GET: api/order/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetOrderById(int id)
    {
        try
        {
            var order = await _orderService.GetOrderByIdAsync(id);

            if (order == null)
            {
                _logger.LogWarning("Order with ID {OrderId} not found", id);
                return NotFound();
            }

            return Ok(order);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error occurred while retrieving order with ID {id}: {ex.Message}");
            return StatusCode(500, "Internal server error");
        }
    }

    // GET: api/order
    [HttpGet]
    public async Task<IActionResult> GetAllOrders()
    {
        try
        {
            var orders = await _orderService.GetAllOrdersAsync();

            if (orders == null || orders.Count == 0)
            {
                _logger.LogInformation("No orders found.");
                return NotFound();
            }

            return Ok(orders);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error occurred while retrieving all orders: {ex.Message}");
            return StatusCode(500, "Internal server error");
        }
    }

    // PUT: api/Order/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateOrder(int id, [FromBody] OrderDto orderDto)
    {
        try
        {
            if (orderDto == null)
            {
                return BadRequest("Order data is null");
            }

            var order = await _orderService.GetOrderByIdAsync(id);
            if (order == null)
            {
                _logger.LogWarning("Order with ID {OrderId} not found", id);
                return NotFound();
            }

            var updatedOrder = await _orderService.UpdateOrderAsync(id, orderDto);

            return Ok(updatedOrder);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error occurred while updating order with ID {id}: {ex.Message}");
            return StatusCode(500, "Internal server error");
        }
    }

    // DELETE: api/Order/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteOrder(int id)
    {
        try
        {
            var order = await _orderService.GetOrderByIdAsync(id);
            if (order == null)
            {
                _logger.LogWarning("Order with ID {OrderId} not found", id);
                return NotFound();
            }

            await _orderService.DeleteOrderAsync(id);

            _logger.LogInformation("Order with ID {OrderId} has been deleted.", id);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error occurred while deleting order with ID {id}: {ex.Message}");
            return StatusCode(500, "Internal server error");
        }
    }
}
