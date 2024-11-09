using System;
using System.ComponentModel.DataAnnotations;

namespace LogisticsApp.Models;

public class Item
{
    [Required]
    public string Id { get; set; } = Guid.NewGuid().ToString("N").Substring(0, 8);

    [Required]
    public string Name { get; set; } = string.Empty;

    [Required]
    public string Description { get; set; } = string.Empty;

    [Required]
    public int Weight { get; set; } = 0;

    [Required]
    public int Value { get; set; } = 0;

    [Required]
    public string Size { get; set; } = "small";
}
