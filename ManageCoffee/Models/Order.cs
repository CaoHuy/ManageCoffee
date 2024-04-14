using System;
using System.Collections.Generic;

#nullable disable

namespace ManageCoffee.Models
{
    public partial class Order
    {
        public Order()
        {
            Details = new HashSet<Detail>();
        }

        public int OrderId { get; set; }
        public int UserId { get; set; }
        public int? TableId { get; set; }
        public int Status { get; set; }
        public string Note { get; set; }
        public decimal TotalPrice { get; set; }
        public int? Revision { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatetedAt { get; set; }

        public virtual Table Table { get; set; }
        public virtual User User { get; set; }
        public virtual ICollection<Detail> Details { get; set; }
    }
}
