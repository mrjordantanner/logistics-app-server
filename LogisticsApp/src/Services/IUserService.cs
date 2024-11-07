using LogisticsApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace LogisticsApp.Services;

public interface IUserService
{
    public Task<User> CreateNewUser(UserDto userDto);
}
