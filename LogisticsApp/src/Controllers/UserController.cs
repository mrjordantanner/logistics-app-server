using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Collections.Generic;
using LogisticsApp.Services;
using LogisticsApp.Models;
using LogisticsApp.Repositories;
using Microsoft.AspNetCore.Identity;

namespace LogisticsApp.Controllers;

/// <summary>
/// API Controller for Users.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly IUserRepository _userRepository;
    private readonly IUserService _userService;

    public UserController(IUserRepository userRepository, IUserService userService)
    {
        _userRepository = userRepository;
        _userService = userService;
    }

    // GET: api/user
    [HttpGet]
    public async Task<ActionResult<IEnumerable<User>>> GetAllUsers()
    {
        var users = await _userRepository.GetAllUsersAsync();

        if (users == null || !users.Any())
        {
            return NotFound();
        }

        return Ok(users);
    }

    // GET: api/user/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<User>> GetUserById(int id)
    {
        var user = await _userRepository.GetUserByIdAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        return Ok(user);
    }

    // POST: api/user
    [HttpPost]
    public async Task<ActionResult<User>> CreateUser([FromBody] UserDto userDto)
    {
        if (userDto == null || string.IsNullOrEmpty(userDto.Name) || string.IsNullOrEmpty(userDto.Email))
        {
            return BadRequest("CreateUser: Invalid User data.");
        }

        var newUser = await _userService.CreateNewUser(userDto);

        return CreatedAtAction(nameof(GetUserById), new { id = newUser.Id }, newUser);
    }

}

