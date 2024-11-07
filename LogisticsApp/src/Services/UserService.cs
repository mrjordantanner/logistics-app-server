namespace LogisticsApp.Services;

using LogisticsApp.Models;
using LogisticsApp.Repositories;

/// <summary>
/// Creates, Updates, and Deletes Users.
/// </summary>
public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly ILogger<UserService> _logger;

    public UserService(IUserRepository userRepository, ILogger<UserService> logger)
    {
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task<User> CreateNewUser(UserDto userDto)
    {
        if (userDto == null || string.IsNullOrEmpty(userDto.Name) || string.IsNullOrEmpty(userDto.Email) || string.IsNullOrEmpty(userDto.Role))
        {
            _logger.LogError("UserService: Unable to create User. UserDto was null or contained invalid data.");
            throw new ArgumentException("Invalid UserDto data provided.", nameof(userDto));
        }

        var newUser = new User
        {
            Name = userDto.Name,
            Email = userDto.Email,
            CreatedAt = DateTime.UtcNow,
            Role = userDto.Role.ToLower() switch
            {
                "admin" => UserRole.Admin,
                "driver" => UserRole.Driver,
                _ => throw new ArgumentException("Invalid role specified.", nameof(userDto.Role))
            }
        };

        if (string.IsNullOrEmpty(userDto.Password))
        {
            _logger.LogError("UserService: User creation failed due to empty password.");
            throw new ArgumentException("Password cannot be empty.", nameof(userDto.Password));
        }

        newUser.SetPassword(userDto.Password);

        var createdUser = await _userRepository.AddUserAsync(newUser);

        if (createdUser != null)
        {
            _logger.LogInformation("UserService: Created and added new user: {Id} | {Name}", newUser.Id, newUser.Name);
            return createdUser;
        }

        _logger.LogError("UserService: Failed to add new user to repository.");
        throw new Exception("User could not be created.");
    }
}
