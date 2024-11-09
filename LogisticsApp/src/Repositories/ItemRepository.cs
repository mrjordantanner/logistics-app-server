using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;
using LogisticsApp.Models;
using LogisticsApp.Contexts;
using Microsoft.Extensions.Logging;

namespace LogisticsApp.Repositories;

/// <summary>
/// Reads and Writes Item data to and from the database.
/// </summary>
public class ItemRepository : IItemRepository
{
    private readonly AppDbContext _context;
    private readonly ILogger<ItemRepository> _logger;

    public ItemRepository(AppDbContext context, ILogger<ItemRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    // Add Item
    public async Task<Item> AddItemAsync(Item newItem)
    {
        try
        {
            _context.Items.Add(newItem);
            await _context.SaveChangesAsync();
            return newItem;
        }
        catch (Exception ex)
        {
            _logger.LogError("ItemRepository: Error attempting to add item to database: {ex}", ex);
            return null;
        }
    }

    // Add Items
    public async Task<IEnumerable<Item>> AddItemsAsync(List<Item> newItems)
    {
        try
        {
            _context.Items.AddRange(newItems);
            await _context.SaveChangesAsync();
            return newItems;
        }
        catch (Exception ex)
        {
            _logger.LogError("ItemRepository: Error attempting to add items to database: {ex}", ex);
            return null;
        }
    }

    // Get All Items
    public async Task<IEnumerable<Item>> GetAllItemsAsync()
    {
        try
        {
            return await _context.Items.ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError("ItemRepository: Error attempting to retrieve items from database: {ex}", ex);
            return new List<Item>();
        }
    }

    // Get Item by Id
    public async Task<Item> GetItemByIdAsync(string id)
    {
        try
        {
            return await _context.Items.FirstOrDefaultAsync(i => i.Id == id);
        }
        catch (Exception ex)
        {
            _logger.LogError("ItemRepository: Error attempting to retrieve item by ID: {ex}", ex);
            return null;
        }
    }

    // Get Item by Name
    public async Task<Item> GetItemByNameAsync(string itemName)
    {
        try
        {
            return await _context.Items.FirstOrDefaultAsync(i => i.Name.Equals(itemName, StringComparison.OrdinalIgnoreCase));
        }
        catch (Exception ex)
        {
            _logger.LogError("ItemRepository: Error attempting to retrieve item by name: {ex}", ex);
            return null;
        }
    }


}
