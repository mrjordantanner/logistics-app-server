using Microsoft.AspNetCore.Mvc;
using LogisticsApp.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LogisticsApp.Repositories;

namespace Logistics.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ItemController : ControllerBase
{
    private readonly IItemRepository _itemRepository;

    public ItemController(IItemRepository itemRepository)
    {
        _itemRepository = itemRepository;
    }

    // POST: api/item/create
    [HttpPost("create")]
    public async Task<IActionResult> CreateItems([FromBody] List<Item> newItems)
    {
        try
        {
            await _itemRepository.AddItemsAsync(newItems);
            return Ok(new { message = $"{newItems.Count} items created successfully." });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = "Error adding items to the database", error = ex.Message });
        }
    }

    // GET: api/item/all
    [HttpGet("all")]
    public async Task<IActionResult> GetAllItems()
    {
        var items = await _itemRepository.GetAllItemsAsync();
        return Ok(items);
    }

    // GET: api/item
    [HttpGet]
    public async Task<IActionResult> GetItemByName([FromQuery] string itemName)
    {
        var item = await _itemRepository.GetItemByNameAsync(itemName);

        if (item == null)
        {
            return NotFound(new { message = "Item not found." });
        }

        return Ok(item);
    }
}
