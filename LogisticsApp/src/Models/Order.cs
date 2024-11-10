using LogisticsApp.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace LogisticsApp.Models
{
    public enum OrderStatus
    {
        Pending,
        Shipped,
        Delivered,
        Cancelled
    }

    public class Order
    {
        [Key]
        public int Id { get; set; }

        // Foreign Key for Origin Location
        [ForeignKey("Origin")]
        public int OriginId { get; set; }

        // Foreign Key for Destination Location
        [ForeignKey("Destination")]
        public int DestinationId { get; set; }

        // Foreign Key for the associated Delivery (each order is part of one delivery)
        [Required]
        public int DeliveryId { get; set; } // Made required (non-nullable)

        public DateTime OrderDate { get; set; } = DateTime.Now;

        // Enum for order status
        [Required]
        public OrderStatus OrderStatus { get; set; } = OrderStatus.Pending;

        // Navigation properties
        public virtual Location Origin { get; set; }
        public virtual Location Destination { get; set; }
        public virtual Delivery Delivery { get; set; } // Each order is linked to exactly one delivery
        public virtual ICollection<OrderItem> OrderItems { get; set; } // Relationship with OrderItems
    }
}
