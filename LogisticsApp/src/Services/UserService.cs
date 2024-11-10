using LogisticsApp.Models;
using LogisticsApp.Repositories;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LogisticsApp.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly ILogger<UserService> _logger;

    public UserService(IUserRepository userRepository, ILogger<UserService> logger)
    {
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task<User> CreateNewUser(CreateUserRequest request)
    {
        if (request == null || string.IsNullOrEmpty(request.Name) || string.IsNullOrEmpty(request.Email) || request.Phone == 0)
        {
            _logger.LogError("UserService: Unable to create User. user was null or contained invalid data.");
            throw new ArgumentException("Invalid user data provided.", nameof(request));
        }

        if (await _userRepository.GetUserByEmailAsync(request.Email) != null)
        {
            _logger.LogError("UserService: User with email {Email} already exists.", request.Email);
            throw new ArgumentException("User with the same email already exists.", nameof(request.Email));
        }
        var newUser = new User
        {
            Name = request.Name,
            Phone = request.Phone,
            Email = request.Email,
            Role = request.Role,
            CreatedAt = DateTime.UtcNow,
        };

        if (string.IsNullOrEmpty(request.Password))
        {
            _logger.LogError("UserService: User creation failed due to empty password.");
            throw new ArgumentException("Password cannot be empty.", nameof(request.Password));
        }

        newUser.SetPassword(request.Password);

        var createdUser = await _userRepository.AddUserAsync(newUser);

        if (createdUser != null)
        {
            _logger.LogInformation("UserService: Created and added new user: {Id} | {Name}", createdUser.Id, createdUser.Name);
            return createdUser;
        }

        _logger.LogError("UserService: Failed to add new user to repository.");
        throw new Exception("User could not be created.");
    }

    public async Task<User> GetUserById(int id)
    {
        var user = await _userRepository.GetUserByIdAsync(id);
        if (user == null)
        {
            _logger.LogError("UserService: User with id {Id} not found.", id);
            throw new KeyNotFoundException($"User with id {id} not found.");
        }
        return user;
    }

    public async Task<IEnumerable<User>> GetAllUsers()
    {
        return await _userRepository.GetAllUsersAsync();
    }

    public async Task<User> UpdateUser(int id, User user)
    {
        var existingUser = await _userRepository.GetUserByIdAsync(id);
        if (existingUser == null)
        {
            _logger.LogError("UserService: User with id {Id} not found for update.", id);
            throw new KeyNotFoundException($"User with id {id} not found.");
        }

        existingUser.Name = user.Name ?? existingUser.Name;
        existingUser.Phone = user.Phone != 0 ? user.Phone : existingUser.Phone;
        existingUser.Email = user.Email ?? existingUser.Email;
        existingUser.UpdatedAt = DateTime.UtcNow;

        var updatedUser = await _userRepository.UpdateUserAsync(existingUser);
        return updatedUser;
    }

    public async Task<bool> DeleteUser(int id)
    {
        var result = await _userRepository.DeleteUserAsync(id);
        if (!result)
        {
            _logger.LogError("UserService: Failed to delete user with id {Id}.", id);
        }
        return result;
    }
}
