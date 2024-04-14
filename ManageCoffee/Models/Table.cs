using System;
using System.Collections.Generic;

#nullable disable

namespace ManageCoffee.Models
{
    public partial class Table
    {
        public Table()
        {
            Orders = new HashSet<Order>();
        }

        public int TableId { get; set; }
        public int? AreaId { get; set; }
        public string Name { get; set; }
        public string Note { get; set; }
        public int? Status { get; set; }

        public virtual Area Area { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
    }
}
