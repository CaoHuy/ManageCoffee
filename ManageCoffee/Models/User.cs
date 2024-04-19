using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

#nullable disable

namespace ManageCoffee.Models
{
    public partial class User
    {
        public User()
        {
            Logs = new HashSet<Log>();
            Orders = new HashSet<Order>();
        }

        public int UserId { get; set; }
        [Required(ErrorMessage = "Tên không được bỏ trống!")]
        [StringLength(50, MinimumLength = 5, ErrorMessage = "Tên phải từ 5 đến 50 ký tự!")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Phân quyền không được bỏ trống!")]
        public int Role { get; set; }

        [Required(ErrorMessage = "Email không được bỏ trống!")]
        [DataType(DataType.EmailAddress)]
        [RegularExpression("^[a-zA-Z0-9_+&*-]+(?:\\.[a-zA-Z0-9_+&*-]+)*@" + "(?:[a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,7}$", ErrorMessage = "Email không đúng định dạng!")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Mật khẩu không được trống")]
        [StringLength(20, MinimumLength = 8, ErrorMessage = "Mật khẩu phải từ 8 đến 20 ký tự")]
        public string Password { get; set; }
        [Required(ErrorMessage = "Số điện thoại không được bỏ trống!")]
        [RegularExpression("^(0|\\+84)(\\s|\\.)?((3[2-9])|(5[689])|(7[06-9])|(8[1-689])|(9[0-46-9]))(\\d)(\\s|\\.)?(\\d{3})(\\s|\\.)?(\\d{3})$", ErrorMessage = "Định dạng số điện thoại không hợp lệ")]
        public string Phone { get; set; }

        public virtual ICollection<Log> Logs { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
        public string getRoleName()
        {
            var name = "";
            switch (this.Role)
            {
                case 1:
                    name = "Admin";
                    break;
                case 2:
                    name = "Pha chế";
                    break;
                default:
                    name = "Nhân viên";
                    break;
            }
            return name;
        }
    }
}
