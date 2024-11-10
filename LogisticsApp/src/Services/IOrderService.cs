using LogisticsApp.Models;

namespace LogisticsApp.Services;

public interface IOrderService
{
    Task<Order> CreateOrderWithItems(OrderDto orderDto);
}
