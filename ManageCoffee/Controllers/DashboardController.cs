using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using ManageCoffee.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ManageCoffee.Controllers
{
    public class DashboardController : Controller
    {
        private readonly ILogger<DashboardController> _logger;
        ManageCoffeeContext _dbConnect;

        public DashboardController(ILogger<DashboardController> logger)
        {
            _dbConnect = new ManageCoffeeContext();
            _logger = logger;
        }

        public IActionResult Index()
        {
            ViewBag.IsActive = "dashboard";
            return View();
        }

        public IActionResult Load(string range)
        {
            try
            {
                List<int> list = new List<int> { };
                var dateRange = range.Split(" - ");
                var start = DateTime.ParseExact(dateRange[0], "d/M/yyyy", CultureInfo.InvariantCulture);
                var end = DateTime.ParseExact(dateRange[1], "d/M/yyyy", CultureInfo.InvariantCulture);

                var orders = _dbConnect.Orders
                .Where(o => o.CreatedAt >= start && o.CreatedAt <= end)
                .ToList();
                
                _logger.LogInformation($" {orders} ");
                foreach (Order item in orders)
                {
                    list.Add(item.OrderId);
                }
                return Json(new
                {
                    numberProducts = new OrderDAO().getNumberProduct(list),
                    DailiRevenues = orders,
                });
            }
            catch (Exception ex)
            {
                return Json(new { error = "An error occurred: " + ex.Message });
            }
        }
    }
}