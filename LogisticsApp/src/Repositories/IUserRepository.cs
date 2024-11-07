using LogisticsApp.Models;

namespace LogisticsApp.Repositories;

public interface IUserRepository
{
    public Task<User> AddUserAsync(User newUser);
    public Task<User> GetUserByIdAsync(int id);
    public Task<IEnumerable<User>> GetAllUsersAsync();
}

