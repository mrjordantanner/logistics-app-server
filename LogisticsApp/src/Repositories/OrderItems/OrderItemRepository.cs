using LogisticsApp.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using LogisticsApp.Data;

namespace LogisticsApp.Repositories;

public class OrderItemRepository : IOrderItemRepository
{
    private readonly AppDbContext _context;
    private readonly ILogger<OrderItemRepository> _logger;

    public OrderItemRepository(AppDbContext context, ILogger<OrderItemRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task CreateOrderItemAsync(OrderItem orderItem)
    {
        try
        {
            _context.OrderItems.Add(orderItem);
            await _context.SaveChangesAsync();
            _logger.LogInformation($"OrderItem {orderItem.Id} created successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error occurred while creating OrderItem: {ex.Message}");
            throw new Exception("An error occurred while creating the OrderItem.");
        }
    }

    public async Task UpdateOrderItemAsync(OrderItem orderItem)
    {
        try
        {
            _context.OrderItems.Update(orderItem);
            await _context.SaveChangesAsync();
            _logger.LogInformation($"OrderItem {orderItem.Id} updated successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error occurred while updating OrderItem: {ex.Message}");
            throw new Exception("An error occurred while updating and saving the OrderItem.");
        }
    }

    public async Task<OrderItem> GetOrderItemByIdAsync(int id)
    {
        try
        {
            return await _context.OrderItems
                .Include(oi => oi.Item)
                .FirstOrDefaultAsync(oi => oi.Id == id);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error occurred while fetching OrderItem with ID {id}: {ex.Message}");
            throw new Exception("An error occurred while retrieving the OrderItem.");
        }
    }

    public async Task<OrderItem> GetOrderItemByOrderAndItemAsync(int orderId, int itemId)
    {
        // Search for an OrderItem that matches both OrderId and ItemId
        return await _context.OrderItems
                             .Where(oi => oi.OrderId == orderId && oi.ItemId == itemId)
                             .FirstOrDefaultAsync();
    }

    public async Task DeleteOrderItemsByOrderIdAsync(int orderId)
    {
        // Retrieve the OrderItems associated with the provided OrderId
        var orderItems = await _context.OrderItems
                                        .Where(oi => oi.OrderId == orderId)
                                        .ToListAsync();

        // If OrderItems are found, remove them
        if (orderItems.Any())
        {
            _context.OrderItems.RemoveRange(orderItems);
            await _context.SaveChangesAsync();
        }
    }


}
