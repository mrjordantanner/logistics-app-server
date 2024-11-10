using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LogisticsApp.Models;

public enum DriverStatus
{
    Available,
    Unavailable,
}

public class Driver : User
{
    public int? UserId { get; set; }

    public override int? CurrentPostalCode { get; set; }

    public override DriverStatus Status { get; set; } = DriverStatus.Available;
}
