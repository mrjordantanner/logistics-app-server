using LogisticsApp.Models;

namespace LogisticsApp.Repositories;

public interface IUserRepository
{
    Task<User> AddUserAsync(User newUser);
    Task<User> GetUserByIdAsync(int id);
    Task<IEnumerable<User>> GetAllUsersAsync();
    Task<User> GetUserByEmailAsync(string email);
}

