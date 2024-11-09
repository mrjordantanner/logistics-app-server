using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;
using LogisticsApp.Models;
using LogisticsApp.Contexts;

namespace LogisticsApp.Repositories;

/// <summary>
/// Reads and Writes User data to and from the database.
/// </summary>
public class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;
    private readonly ILogger<UserRepository> _logger;

    public UserRepository(AppDbContext context, ILogger<UserRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IEnumerable<User>> GetAllUsersAsync()
    {
        return await _context.Users.ToListAsync();
    }

    public async Task<User> GetUserByIdAsync(int id)
    {
        return await _context.Users.Where(u => u.Id == id).FirstOrDefaultAsync();
    }

    public async Task<User> GetUserByEmailAsync(string email)
    {
        try
        {
            return await _context.Users.Where(u => u.Email == email).FirstOrDefaultAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError("UserRepository: Error attempting to find user by email: {ex}", ex);
            return null;
        }
    }

    public async Task<User> AddUserAsync(User newUser)
    {
        try
        {
            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError("UserRepository: Error attempting to add user to database: {ex}", ex);
            return null;
        }

        return newUser;
    }
}
