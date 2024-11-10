using LogisticsApp.Models;
using LogisticsApp.Repositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LogisticsApp.Services;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly IOrderItemRepository _orderItemRepository;
    private readonly IItemRepository _itemRepository;
    private readonly ILocationRepository _locationRepository;

    public OrderService(IOrderRepository ordersRepository,
                        IOrderItemRepository orderItemsRepository,
                        IItemRepository itemRepository,
                        ILocationRepository locationRepository)
    {
        _orderRepository = ordersRepository;
        _orderItemRepository = orderItemsRepository;
        _itemRepository = itemRepository;
        _locationRepository = locationRepository;
    }

    public async Task<Order> CreateOrderWithItems(OrderDto orderDto)
    {
        // Validate Origin and Destination location
        var origin = await _locationRepository.GetLocationByNameAsync(orderDto.Origin.Name);
        var destination = await _locationRepository.GetLocationByNameAsync(orderDto.Destination.Name);
        if (origin == null || destination == null)
        {
            throw new ArgumentException("Invalid origin or destination");
        }

        // Create Order
        var order = new Order
        {
            OrderDate = DateTime.Now,
            OrderStatus = OrderStatus.Pending,
            OriginId = origin.Id,
            DestinationId = destination.Id
        };

        await _orderRepository.CreateOrderAsync(order);

        // Create OrderItems from the provided items
        foreach (var itemDto in orderDto.Items)
        {
            var item = await _itemRepository.GetItemByIdAsync(itemDto.ItemId);
            if (item == null)
            {
                throw new ArgumentException($"Item with ID {itemDto.ItemId} does not exist");
            }

            var orderItem = new OrderItem
            {
                OrderId = order.Id,
                ItemId = item.Id,
                Quantity = itemDto.Quantity
            };

            await _orderItemRepository.CreateOrderItemAsync(orderItem);
        }

        return order;
    }

    public async Task<Order> GetOrderByIdAsync(int id)
    {
        var order = await _orderRepository.GetOrderByIdAsync(id);
        if (order == null)
        {
            throw new KeyNotFoundException($"Order with ID {id} not found.");
        }

        return order;
    }

    public async Task<List<Order>> GetAllOrdersAsync()
    {
        return await _orderRepository.GetAllOrdersAsync();
    }

    public async Task<Order> UpdateOrderAsync(int id, OrderDto orderDto)
    {
        // Validate Origin and Destination location
        var origin = await _locationRepository.GetLocationByNameAsync(orderDto.Origin.Name);
        var destination = await _locationRepository.GetLocationByNameAsync(orderDto.Destination.Name);
        if (origin == null || destination == null)
        {
            throw new ArgumentException("Invalid origin or destination");
        }

        var order = await _orderRepository.GetOrderByIdAsync(id);
        if (order == null)
        {
            throw new KeyNotFoundException($"Order with ID {id} not found.");
        }

        // Update the order
        order.OriginId = origin.Id;
        order.DestinationId = destination.Id;
        order.OrderStatus = orderDto.Status;

        await _orderRepository.UpdateOrderAsync(order);

        // Update OrderItems
        foreach (var itemDto in orderDto.Items)
        {
            var item = await _itemRepository.GetItemByIdAsync(itemDto.ItemId);
            if (item == null)
            {
                throw new ArgumentException($"Item with ID {itemDto.ItemId} does not exist");
            }

            var existingOrderItem = await _orderItemRepository.GetOrderItemByOrderAndItemAsync(order.Id, item.Id);
            if (existingOrderItem != null)
            {
                existingOrderItem.Quantity = itemDto.Quantity;
                await _orderItemRepository.UpdateOrderItemAsync(existingOrderItem);
            }
            else
            {
                var orderItem = new OrderItem
                {
                    OrderId = order.Id,
                    ItemId = item.Id,
                    Quantity = itemDto.Quantity
                };
                await _orderItemRepository.CreateOrderItemAsync(orderItem);
            }
        }

        return order;
    }

    public async Task DeleteOrderAsync(int id)
    {
        var order = await _orderRepository.GetOrderByIdAsync(id);
        if (order == null)
        {
            throw new KeyNotFoundException($"Order with ID {id} not found.");
        }

        // Delete associated order items first
        await _orderItemRepository.DeleteOrderItemsByOrderIdAsync(id);

        // Then delete the order
        await _orderRepository.DeleteOrderAsync(id);
    }
}


public class OrderDto
{
    public LocationDto Origin { get; set; }
    public LocationDto Destination { get; set; }
    public List<ItemDto> Items { get; set; }
    public OrderStatus Status { get; set; }
}

public class LocationDto
{
    public string Name { get; set; }
    public string City { get; set; }
    public string State { get; set; }
    public string PostalCode { get; set; }
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
}

public class ItemDto
{
    public int ItemId { get; set; }
    public int Quantity { get; set; }
}
