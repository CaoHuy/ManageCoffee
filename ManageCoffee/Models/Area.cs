using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

#nullable disable

namespace ManageCoffee.Models
{
    public partial class Area
    {
        public Area()
        {
            Tables = new HashSet<Table>();
        }

        public int AreaId { get; set; }

        [Required(ErrorMessage = "Tên khu vực không được bỏ trống")]
        public string Name { get; set; }
        public DateTime? SoftDelete { get; set; }

        public virtual ICollection<Table> Tables { get; set; }
    }
}
