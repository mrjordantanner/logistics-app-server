using LogisticsApp.Models;

namespace LogisticsApp.Repositories;

public interface IDeliveryRepository
{
    Task<Delivery> CreateDeliveryAsync(Delivery delivery);
    Task<Delivery> GetDeliveryByIdAsync(int id);
    Task<List<Delivery>> GetAllDeliveriesAsync();
    Task UpdateDeliveryAsync(Delivery delivery);
    Task DeleteDeliveryAsync(int id);
}
