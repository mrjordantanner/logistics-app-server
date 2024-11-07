namespace LogisticsApp.Models;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;

public enum UserRole
{
    Admin,
    Driver
}

public class User
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [MaxLength(255)]
    public string Email { get; set; } = string.Empty;

    [Required]
    public byte[] PasswordHash { get; set; }

    [Required]
    public byte[] PasswordSalt { get; set; }

    [Required]
    public UserRole Role { get; set; } = UserRole.Driver;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// Generates a new PasswordSalt using HMACSHA512’s randomly generated key.
    /// Hashes the password with this salt and stores it in PasswordHash.
    /// </summary>
    public void SetPassword(string password)
    {
        using var hmac = new HMACSHA512();
        PasswordSalt = hmac.Key; // Generate a random salt
        PasswordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password)); // Hash the password
    }

    /// <summary>
    /// Uses the stored PasswordSalt to hash the provided password.
    /// Compares the computed hash with the stored PasswordHash. If any byte differs, the verification fails.
    /// </summary>
    public bool VerifyPassword(string password)
    {
        using var hmac = new HMACSHA512(PasswordSalt); // Use the stored salt
        var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
        for (int i = 0; i < computedHash.Length; i++)
        {
            if (computedHash[i] != PasswordHash[i]) return false;
        }
        return true;
    }
}
