using System;
using System.Collections.Generic;
using System.Linq;
using ManageCoffee.DAO;

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
        public DateTime? UpdatedAt { get; set; }

        public virtual Table Table { get; set; }
        public virtual User User { get; set; }
        public virtual ICollection<Detail> Details { get; set; }

        public string getStatus()
        {
            return (this.Status == 0) ? "Chưa thanh toán" : "Đã thanh toán";
        }

        public string getTableName()
        {
            if (this.TableId != null)
            {
                TableDAO table = new TableDAO();
                var name = table.GetTableByID(this.TableId).Name;
                return name;
            }
            return "Mang đi";
        }

        public string getUserName()
        {
            UserDAO dao = new UserDAO();
            var name = dao.GetUserByID(this.UserId).Name;
            return name;
        }

        public Table GetTable()
        {
            if (this.TableId != null)
            {
                TableDAO dao = new TableDAO();
                var table = dao.GetTableByID(this.TableId);
                return table;
            }
            else
            {
                return null;
            }
        }

        public IEnumerable<Detail> GetDetail()
        {
            var context = new ManageCoffeeContext();
            var details = context.Details.Where(d => d.OrderId == this.OrderId).ToList();
            return details;
        }

        public User GetUser()
        {
            UserDAO dao = new UserDAO();
            var user = dao.GetUserByID(this.UserId);
            return user;
        }
        public void RemoveDetails()
        {
            var context = new ManageCoffeeContext();
            var details = context.Details.Where(d => d.OrderId == this.OrderId).Select(d => d.OrderId).ToList();
            DetailDAO dao = new DetailDAO();
            dao.RemoveMultiple(details);
        }
    }
}
