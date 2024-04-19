using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

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

        [Required(ErrorMessage = "Khu vực không được bỏ trống!")]
        public int? AreaId { get; set; }
        
        [Required(ErrorMessage = "Bàn không được bỏ trống!")]
        public string Name { get; set; }
        public string Note { get; set; }
        public int? Status { get; set; }

        public virtual Area Area { get; set; }
        public virtual ICollection<Order> Orders { get; set; }

        public string GetStatus()
        {
            var stt = (this.Status == 0) ? "<p class='text-success status'>Trống</p>" : "<p class='text-danger status'>Có khách</p>";
            return stt;

        }
    }
}
