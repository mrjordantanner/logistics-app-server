using LogisticsApp.Models;

namespace LogisticsApp.Services;

public interface IOrderService
{
    Task<Order> CreateOrderWithItems(OrderDto orderDto);
    Task<Order> GetOrderByIdAsync(int id);
    Task<List<Order>> GetAllOrdersAsync();
    Task<Order> UpdateOrderAsync(int id, OrderDto orderDto);
    Task DeleteOrderAsync(int id);
}
