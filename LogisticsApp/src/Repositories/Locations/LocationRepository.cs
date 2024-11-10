using LogisticsApp.Data;
using LogisticsApp.Models;
using Microsoft.EntityFrameworkCore;

namespace LogisticsApp.Repositories;

public class LocationRepository : ILocationRepository
{
    private readonly AppDbContext _context;
    private readonly ILogger<LocationRepository> _logger;

    public LocationRepository(AppDbContext context, ILogger<LocationRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Location> CreateLocationAsync(Location location)
    {
        try
        {
            _context.Locations.Add(location);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Created new location with Id: {LocationId}", location.Id);
            return location;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while creating a new location");
            throw;
        }
    }

    public async Task<Location> GetLocationByIdAsync(int id)
    {
        try
        {
            var location = await _context.Locations
                .FirstOrDefaultAsync(l => l.Id == id);

            if (location == null)
            {
                _logger.LogWarning("Location with Id: {LocationId} not found", id);
            }

            return location;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while fetching location with Id: {LocationId}", id);
            throw;
        }
    }

    public async Task<Location> GetLocationByNameAsync(string name)
    {
        try
        {
            var location = await _context.Locations
                .FirstOrDefaultAsync(l => l.Name == name);

            if (location == null)
            {
                _logger.LogWarning("Location with Name: {name} not found", name);
            }

            return location;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while fetching location with Name: {name}", name);
            throw;
        }
    }

    public async Task<IEnumerable<Location>> GetAllLocationsAsync()
    {
        try
        {
            var locations = await _context.Locations.ToListAsync();
            _logger.LogInformation("Fetched {Count} locations", locations.Count);
            return locations;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while fetching all locations");
            throw;
        }
    }

    public async Task<bool> UpdateLocationAsync(Location location)
    {
        try
        {
            _context.Locations.Update(location);
            var affectedRows = await _context.SaveChangesAsync();
            var isUpdated = affectedRows > 0;
            _logger.LogInformation(isUpdated ? "Location updated successfully" : "No location updated");
            return isUpdated;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while updating location with Id: {LocationId}", location.Id);
            throw;
        }
    }

    public async Task<bool> DeleteLocationAsync(int id)
    {
        try
        {
            var location = await _context.Locations.FindAsync(id);
            if (location == null)
            {
                _logger.LogWarning("Location with Id: {LocationId} not found for deletion", id);
                return false;
            }

            _context.Locations.Remove(location);
            var affectedRows = await _context.SaveChangesAsync();
            var isDeleted = affectedRows > 0;
            _logger.LogInformation(isDeleted ? "Location deleted successfully" : "No location deleted");
            return isDeleted;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while deleting location with Id: {LocationId}", id);
            throw;
        }
    }
}
