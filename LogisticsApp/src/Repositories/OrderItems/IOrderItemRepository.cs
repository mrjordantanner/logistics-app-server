using LogisticsApp.Models;
using System.Threading.Tasks;

namespace LogisticsApp.Repositories;

public interface IOrderItemRepository
{
    Task CreateOrderItemAsync(OrderItem orderItem);
    Task UpdateOrderItemAsync(OrderItem orderItem);
    Task<OrderItem> GetOrderItemByIdAsync(int id);
    Task<OrderItem> GetOrderItemByOrderAndItemAsync(int orderId, int itemId);
    Task DeleteOrderItemsByOrderIdAsync(int orderId);
}
