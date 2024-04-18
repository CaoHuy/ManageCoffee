using System;
using System.Collections.Generic;

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
        public string Name { get; set; }

        public virtual ICollection<Product> Products { get; set; }
    }
}
