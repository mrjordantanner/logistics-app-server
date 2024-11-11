using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace LogisticsApp.Models;

public enum DeliveryStatus
{
    Scheduled, 
    InTransit, 
    Delivered, 
    Delayed, 
    Failed
}

public class Delivery
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public int DriverId { get; set; }
    [Required]
    public int Origin { get; set; }
    [Required]
    public int Destination { get; set; }
    //public DateTime? TargetDeliveryDate { get; set; } = DateTime.UtcNow.AddDays(2);
    //public DateTime? ActualDeliveryDate { get; set; } = null;

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public DeliveryStatus Status { get; set; } = DeliveryStatus.Scheduled;
    //public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    //public DateTime? UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public User Driver { get; set; }
    public ICollection<Order> Orders { get; set; } 
    public Location OriginLocation { get; set; }
    public Location DestinationLocation { get; set; }
}



