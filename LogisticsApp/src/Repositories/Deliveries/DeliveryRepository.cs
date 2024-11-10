using LogisticsApp.Models;
using LogisticsApp.Data;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace LogisticsApp.Repositories;

public class DeliveryRepository : IDeliveryRepository
{
    private readonly AppDbContext _context;

    public DeliveryRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Delivery> CreateDeliveryAsync(Delivery delivery)
    {
        _context.Deliveries.Add(delivery);
        await _context.SaveChangesAsync();
        return delivery;
    }

    public async Task<Delivery> GetDeliveryByIdAsync(int id)
    {
        return await _context.Deliveries.FindAsync(id);
    }

    public async Task<List<Delivery>> GetAllDeliveriesAsync()
    {
        return await _context.Deliveries.ToListAsync();
    }

    public async Task UpdateDeliveryAsync(Delivery delivery)
    {
        _context.Deliveries.Update(delivery);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteDeliveryAsync(int id)
    {
        var delivery = await _context.Deliveries.FindAsync(id);
        if (delivery != null)
        {
            _context.Deliveries.Remove(delivery);
            await _context.SaveChangesAsync();
        }
    }
}
