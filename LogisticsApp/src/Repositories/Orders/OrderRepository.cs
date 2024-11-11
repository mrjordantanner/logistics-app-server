using LogisticsApp.Data;
using LogisticsApp.Models;
using Microsoft.EntityFrameworkCore;

namespace LogisticsApp.Repositories;

public class OrderRepository : IOrderRepository
{
    private readonly AppDbContext _context;
    private readonly ILogger<OrderRepository> _logger;

    public OrderRepository(AppDbContext context, ILogger<OrderRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<List<Order>> GetAllOrdersAsync()
    {
        try
        {
            return await _context.Orders
                                 .Include(o => o.Origin)
                                 .Include(o => o.Destination)
                                 //.Include(o => o.Delivery)
                                 .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error occurred while fetching all orders: {ex.Message}");
            throw new Exception("Error fetching orders", ex);
        }
    }

    public async Task<Order> GetOrderByIdAsync(int id)
    {
        try
        {
            return await _context.Orders
                                 .Include(o => o.Origin)
                                 .Include(o => o.Destination)
                                 //.Include(o => o.Delivery)
                                 .FirstOrDefaultAsync(o => o.Id == id);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error occurred while fetching order with Id {id}: {ex.Message}");
            throw new Exception($"Error fetching order with Id {id}", ex);
        }
    }

    public async Task CreateOrderAsync(Order order)
    {
        _context.ChangeTracker.Clear();

        try
        {
            _context.Orders.Add(order);

            Console.WriteLine("Creating Order...");
            Console.WriteLine($"Id {order.Id}");
            Console.WriteLine($"OriginId {order.OriginId}");
            Console.WriteLine($"DestinationId {order.DestinationId}");
            //Console.WriteLine($"DeliveryId {order.DeliveryId}");
            //Console.WriteLine($"OrderDate {order.CreatedAt}");
            Console.WriteLine($"OrderStatus {order.OrderStatus}");
            Console.WriteLine($"---");
            Console.WriteLine($"Origin {order.Origin}");
            Console.WriteLine($"Destination {order.Destination}");
            //Console.WriteLine($"Delivery {order.Delivery}");
            Console.WriteLine($"OrderItems {order.OrderItems.Count}");

            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error occurred while creating order: {ex.Message}");
            throw new Exception("Error creating order", ex);
        }
    }

    public async Task UpdateOrderAsync(Order order)
    {
        try
        {
            _context.Orders.Update(order);
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error occurred while updating order with Id {order.Id}: {ex.Message}");
            throw new Exception($"Error updating order with Id {order.Id}", ex);
        }
    }

    public async Task DeleteOrderAsync(int id)
    {
        try
        {
            var order = await GetOrderByIdAsync(id);
            if (order != null)
            {
                _context.Orders.Remove(order);
                await _context.SaveChangesAsync();
            }
            else
            {
                _logger.LogWarning($"Attempt to delete non-existing order with Id {id}");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error occurred while deleting order with Id {id}: {ex.Message}");
            throw new Exception($"Error deleting order with Id {id}", ex);
        }
    }
}
