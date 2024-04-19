using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

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

        [Required(ErrorMessage = "Tên danh mục không được bỏ trống")]
        public int? CatalogueId { get; set; }

        [Required(ErrorMessage = "Tên sản phẩm không được bỏ trống")]
        public string Name { get; set; }
        public string Image { get; set; }

        [Required(ErrorMessage = "Đơn vị không được bỏ trống")]
        public string Unit { get; set; }

        [Required(ErrorMessage = "Giá sản phẩm không được bỏ trống!")]
        [Range(0, double.MaxValue, ErrorMessage = "Giá sản phẩm không hợp lệ.")]
        [RegularExpression("^[1-9][0-9]*$", ErrorMessage = "Nhập giá sản phẩm hợp lệ")]
        public decimal Price { get; set; }
        public DateTime? SoftDelete { get; set; }

        public virtual Catalogue Catalogue { get; set; }
        public virtual ICollection<Detail> Details { get; set; }
    }
}
