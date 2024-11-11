using LogisticsApp.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

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

        [ForeignKey("OriginId")]
        public int OriginId { get; set; }

        [ForeignKey("DestinationId")]
        public int DestinationId { get; set; }

       // public int? DeliveryId { get; set; }

        //public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        //public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public OrderStatus OrderStatus { get; set; } = OrderStatus.Pending;

        // Navigation properties
        public virtual Location Origin { get; set; }
        public virtual Location Destination { get; set; }
        //public virtual Delivery? Delivery { get; set; } = null;
        public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}
