using LogisticsApp.Models;
using LogisticsApp.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LogisticsApp.Services
{
    public interface IDeliveryService
    {
        Task<Delivery> CreateDeliveryAsync(Delivery delivery);
        Task<Delivery> GetDeliveryByIdAsync(int id);
        Task<List<Delivery>> GetAllDeliveriesAsync();
        Task UpdateDeliveryAsync(int id, Delivery delivery);
        Task DeleteDeliveryAsync(int id);
    }

    public class DeliveryService : IDeliveryService
    {
        private readonly IDeliveryRepository _deliveryRepository;

        public DeliveryService(IDeliveryRepository deliveryRepository)
        {
            _deliveryRepository = deliveryRepository;
        }

        public async Task<Delivery> CreateDeliveryAsync(Delivery delivery)
        {
            return await _deliveryRepository.CreateDeliveryAsync(delivery);
        }

        public async Task<Delivery> GetDeliveryByIdAsync(int id)
        {
            return await _deliveryRepository.GetDeliveryByIdAsync(id);
        }

        public async Task<List<Delivery>> GetAllDeliveriesAsync()
        {
            return await _deliveryRepository.GetAllDeliveriesAsync();
        }

        public async Task UpdateDeliveryAsync(int id, Delivery updatedDelivery)
        {
            var existingDelivery = await _deliveryRepository.GetDeliveryByIdAsync(id);
            if (existingDelivery != null)
            {
                existingDelivery.Status = updatedDelivery.Status;
                existingDelivery.DriverId = updatedDelivery.DriverId;
                existingDelivery.Origin = updatedDelivery.Origin;
                existingDelivery.Destination = updatedDelivery.Destination;
                existingDelivery.Orders = updatedDelivery.Orders;
                existingDelivery.TargetDeliveryDate = updatedDelivery.TargetDeliveryDate;
                existingDelivery.ActualDeliveryDate = updatedDelivery.ActualDeliveryDate;
                existingDelivery.UpdatedAt = DateTime.UtcNow;

                await _deliveryRepository.UpdateDeliveryAsync(existingDelivery);
            }
        }

        public async Task DeleteDeliveryAsync(int id)
        {
            await _deliveryRepository.DeleteDeliveryAsync(id);
        }
    }
}
