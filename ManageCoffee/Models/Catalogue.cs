using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

#nullable disable

namespace ManageCoffee.Models
{
    public partial class Catalogue
    {
        public Catalogue()
        {
            Products = new HashSet<Product>();
        }

        public int CatalogueId { get; set; }

        [Required(ErrorMessage = "Tên danh mục không được bỏ trống")]
        public string Name { get; set; }

        public virtual ICollection<Product> Products { get; set; }
    }
}
