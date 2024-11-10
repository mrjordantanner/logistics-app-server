using System;
using System.ComponentModel.DataAnnotations;

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
    public int Id { get; set; }
    public int DriverId { get; set; }
    public int Origin { get; set; }
    public int Destination { get; set; }
    public DateTime? TargetDeliveryDate { get; set; }
    public DateTime? ActualDeliveryDate { get; set; }
    public string Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    // Navigation properties
    public User Driver { get; set; }
    public ICollection<Order> Orders { get; set; }  // One delivery can have multiple orders
    public Location OriginLocation { get; set; }
    public Location DestinationLocation { get; set; }
}



