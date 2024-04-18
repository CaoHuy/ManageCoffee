using System;
using System.Collections.Generic;

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
        public string Name { get; set; }
        public int Role { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
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
                    name = "Chủ quán";
                    break;
                default:
                    name = "Nhân viên";
                    break;
            }
            return name;
        }
    }
}
