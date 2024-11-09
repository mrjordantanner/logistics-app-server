using System.Collections.Generic;
using System.Threading.Tasks;
using LogisticsApp.Models;

namespace LogisticsApp.Repositories;

public interface IItemRepository
{
    Task<Item> AddItemAsync(Item newItem);
    Task<IEnumerable<Item>> AddItemsAsync(List<Item> newItems);
    Task<IEnumerable<Item>> GetAllItemsAsync();
    Task<Item> GetItemByIdAsync(string id);
    Task<Item> GetItemByNameAsync(string itemName);
}
