using LogisticsApp.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LogisticsApp.Repositories
{
    public interface ILocationRepository
    {
        Task<Location> CreateLocationAsync(Location location);
        Task<Location> GetLocationByIdAsync(int id);
        Task<Location> GetLocationByNameAsync(string name);
        Task<IEnumerable<Location>> GetAllLocationsAsync();
        Task<bool> UpdateLocationAsync(Location location);
        Task<bool> DeleteLocationAsync(int id);
    }
}
