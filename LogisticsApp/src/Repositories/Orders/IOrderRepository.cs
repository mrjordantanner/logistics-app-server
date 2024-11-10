using LogisticsApp.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LogisticsApp.Repositories;

public interface IOrderRepository
{
    Task<List<Order>> GetAllOrdersAsync();
    Task<Order> GetOrderByIdAsync(int id);
    Task CreateOrderAsync(Order order);
    Task UpdateOrderAsync(Order order);
    Task DeleteOrderAsync(int id);
}
