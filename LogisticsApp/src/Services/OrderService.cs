using LogisticsApp.Models;
using LogisticsApp.Repositories;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace LogisticsApp.Services;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly IOrderItemRepository _orderItemRepository;
    private readonly IItemRepository _itemRepository;
    private readonly ILocationRepository _locationRepository;
    private readonly ILogger<OrderService> _logger;

    public OrderService(IOrderRepository ordersRepository,
                        IOrderItemRepository orderItemsRepository,
                        IItemRepository itemRepository,
                        ILocationRepository locationRepository,
                        ILogger<OrderService> logger)
    {
        _orderRepository = ordersRepository;
        _orderItemRepository = orderItemsRepository;
        _itemRepository = itemRepository;
        _locationRepository = locationRepository;
        _logger = logger;
    }

    public async Task<Order> CreateOrderWithItems(OrderDto orderDto)
    {



        // Validate Origin and Destination location
        var origin = await _locationRepository.GetLocationByNameAsync(orderDto.OriginName);
        var destination = await _locationRepository.GetLocationByNameAsync(orderDto.DestinationName);
        if (origin == null || destination == null)
        {
            _logger.LogError("CreateOrder: Invalid origin or destination");
            return null;
        }

        // Create the Order entity first (without OrderItems)
        var order = new Order
        {
            //CreatedAt = DateTime.UtcNow,
            OrderStatus = OrderStatus.Pending,
            OriginId = origin.Id,
            DestinationId = destination.Id
        };

        // Save the Order first to get the OrderId assigned
        await _orderRepository.CreateOrderAsync(order);

        // Create and associate OrderItems
        var orderItems = new List<OrderItem>();

        foreach (var itemDto in orderDto.Items)
        {
            var item = await _itemRepository.GetItemByIdAsync(itemDto.ItemId);
            if (item == null)
            {
                _logger.LogError("CreateOrder: Item with ID {id} does not exist", itemDto.ItemId);
                return null;
            }

            var orderItem = new OrderItem
            {
                OrderId = order.Id,  // Use the newly assigned OrderId
                ItemId = item.Id,
                Quantity = itemDto.Quantity
            };

            // Add OrderItem to the list
            orderItems.Add(orderItem);
        }

        // Save OrderItems to the OrderItems table after the Order is created
        foreach (var orderItem in orderItems)
        {
            await _orderItemRepository.CreateOrderItemAsync(orderItem);
        }

        return order;
    }


    public async Task<Order> GetOrderByIdAsync(int id)
    {
        var order = await _orderRepository.GetOrderByIdAsync(id);
        if (order == null)
        {
            _logger.LogError("CreateOrder: Order with ID {id} not found.", id);
            return null;
            //throw new KeyNotFoundException($"Order with ID {id} not found.");
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
        var origin = await _locationRepository.GetLocationByNameAsync(orderDto.OriginName);
        var destination = await _locationRepository.GetLocationByNameAsync(orderDto.DestinationName);
        if (origin == null || destination == null)
        {
            //throw new ArgumentException("Invalid origin or destination");
            _logger.LogError("CreateOrder:  Invalid origin or destination");
            return null;
        }

        var order = await _orderRepository.GetOrderByIdAsync(id);
        if (order == null)
        {
            //throw new KeyNotFoundException($"Order with ID {id} not found.");
            _logger.LogError("CreateOrder: Order with ID {id} not found.", id);
            return null;
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
                _logger.LogError("CreateOrder: Item with ID {id} does not exist.", id);
                return null;
                //throw new ArgumentException($"Item with ID {itemDto.ItemId} does not exist");
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
            _logger.LogError("CreateOrder: Order with ID {id} not found.", id);
            return;
           // throw new KeyNotFoundException($"Order with ID {id} not found.");
        }

        // Delete associated order items first
        await _orderItemRepository.DeleteOrderItemsByOrderIdAsync(id);

        // Then delete the order (NOTE: does cascade delete in the db cause this to be deleted anyway?)
        await _orderRepository.DeleteOrderAsync(id);
    }
}


public class OrderDto
{
    //public LocationDto Origin { get; set; }
    //public LocationDto Destination { get; set; }

    public string OriginName { get; set; }
    public string DestinationName { get; set; }
    public List<ItemDto> Items { get; set; }
    
    // Status property so we can update the Order's status after it's created
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public OrderStatus Status { get; set; }
}

//public class LocationDto
//{
//    public string Name { get; set; }
//    public string City { get; set; }
//    public string State { get; set; }
//    public string PostalCode { get; set; }
//    public decimal? Latitude { get; set; }
//    public decimal? Longitude { get; set; }
//}

public class ItemDto
{
    public int ItemId { get; set; }
    public int Quantity { get; set; }
}
