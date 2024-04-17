using System;
using System.Collections.Generic;

#nullable disable

namespace ManageCoffee.Models
{
    public partial class Product
    {
        public Product()
        {
            Details = new HashSet<Detail>();
        }

        public int ProductId { get; set; }
        public int? CatalogueId { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }
        public string Unit { get; set; }
        public decimal Price { get; set; }
        public DateTime? SoftDelete { get; set; }

        public virtual Catalogue Catalogue { get; set; }
        public virtual ICollection<Detail> Details { get; set; }

        public string getCatalogueName()
        {
            var name = "";
            switch (this.CatalogueId)
            {
                case 1:
                    name = "Đồ uống";
                    break;
                case 2:
                    name = "Đồ ăn";
                    break;
                default:
                    name = "Món khác";
                    break;
            }
            return name;
        }
    }
}
