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
public class DeliveryController : ControllerBase
{
    private readonly IDeliveryService _deliveryService;
    private readonly ILogger<DeliveryController> _logger;

    public DeliveryController(IDeliveryService deliveryService, ILogger<DeliveryController> logger)
    {
        _deliveryService = deliveryService;
        _logger = logger;
    }

    [HttpPost]
    public async Task<IActionResult> CreateDelivery([FromBody] Delivery delivery)
    {
        try
        {
            var createdDelivery = await _deliveryService.CreateDeliveryAsync(delivery);
            return CreatedAtAction(nameof(GetDeliveryById), new { id = createdDelivery.Id }, createdDelivery);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error creating delivery: {ex.Message}");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetDeliveryById(int id)
    {
        var delivery = await _deliveryService.GetDeliveryByIdAsync(id);
        if (delivery == null)
        {
            return NotFound();
        }
        return Ok(delivery);
    }

    [HttpGet]
    public async Task<IActionResult> GetAllDeliveries()
    {
        var deliveries = await _deliveryService.GetAllDeliveriesAsync();
        return Ok(deliveries);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateDelivery(int id, [FromBody] Delivery delivery)
    {
        try
        {
            await _deliveryService.UpdateDeliveryAsync(id, delivery);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error updating delivery: {ex.Message}");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteDelivery(int id)
    {
        try
        {
            await _deliveryService.DeleteDeliveryAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error deleting delivery: {ex.Message}");
            return StatusCode(500, "Internal server error");
        }
    }
}
