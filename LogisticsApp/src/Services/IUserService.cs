using LogisticsApp.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LogisticsApp.Services;

public interface IUserService
{
    Task<User> CreateNewUser(CreateUserRequest request);
    Task<User> GetUserById(int id);
    Task<IEnumerable<User>> GetAllUsers();
    Task<User> UpdateUser(int id, User user);
    Task<bool> DeleteUser(int id);
}
