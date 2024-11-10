using LogisticsApp.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace LogisticsApp.Models
{
    public class OrderItem
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Order")]
        public int OrderId { get; set; }

        [ForeignKey("Item")]
        public int ItemId { get; set; }

        public int Quantity { get; set; }

        // Navigation Properties
        public virtual Order Order { get; set; }
        public virtual Item Item { get; set; }
    }
}
