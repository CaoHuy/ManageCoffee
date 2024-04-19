using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using ManageCoffee.DAO;
using ManageCoffee.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ManageCoffee.Controllers
{
    public class ConfigController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.IsActive = "config";
            return View();
        }

        public IActionResult Load()
        {
            var configList = ConfigDAO.Instance.GetConfigList();
            return Json(configList);
        }

        [HttpPost]
        public IActionResult BankUpdate(IFormCollection request)
        {
            foreach (var key in request.Keys)
            {
                UpdateConfig(key, request[key]);
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult ShopUpdate(IFormCollection request)
        {
            foreach (var key in request.Keys)
            {
                UpdateConfig(key, request[key]);
            }
            return RedirectToAction("Index");
        }


        private void UpdateConfig(string key, string value)
        {
            ManageCoffeeContext context = new ManageCoffeeContext();
            var config = context.Configs.SingleOrDefault(s => s.Key == key);
            if (config != null)
            {
                config.Value = value;
                context.SaveChanges();
            }
        }
    }
}