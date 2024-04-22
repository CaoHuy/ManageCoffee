using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ManageCoffee.Models.Authentication;
using ManageCoffee.Models;
using ManageCoffee.DAO;

namespace ManageCoffee.Controllers
{
    [Authentication]
    public class BartenderController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.IsActive = "bartender";
            return View();
        }

        public IActionResult Load()
        {
            var dbContext = new ManageCoffeeContext();
            OrderDAO orderDAO = new OrderDAO();
            ProductDAO productDAO = new ProductDAO();
            TableDAO tableDAO = new TableDAO();
            List<Detail> details = dbContext.Details.Where(d => d.Status == 0).OrderBy(d => d.OrderId).ToList();
            List<object> resultList = new List<object>();
            foreach (var detail in details)
            {
                var obj = new
                {
                    detailId = detail.DetailId,
                    tableName = (orderDAO.GetOrderByID(detail.OrderId).TableId != null) ? tableDAO.GetTableByID(orderDAO.GetOrderByID(detail.OrderId).TableId).Name : ("Đơn " + detail.OrderId + "(Mang đi)"),
                    productName = productDAO.GetProductByID(detail.ProductId).Name,
                    quantity = detail.Quantity
                };
                resultList.Add(obj);
            }
            return Json(resultList);
        }
    }
}