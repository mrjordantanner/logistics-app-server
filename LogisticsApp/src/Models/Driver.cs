using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

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

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public DriverStatus Status { get; set; } = DriverStatus.Available;
}
