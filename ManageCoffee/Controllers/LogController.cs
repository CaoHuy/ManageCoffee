using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using ManageCoffee.DAO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ManageCoffee.Controllers
{
    public class LogController : Controller
    {
        LogDAO dao = new LogDAO();

        public ActionResult Index()
        { 
            ViewBag.IsActive = "log";
            return View();
        }
        public IActionResult Load()
        {
            var logs = dao.GetLogList();
            var data = logs.Select(o => new
            {
                id = o?.LogId,
                user_id = o.getUserName(),
                action = o.Action,
                object_name = o.Object,
                object_id = o.ObjectId,
                created_at = o?.CreatedAt.Value.ToString("HH:mm:ss dd/MM/yyyy"),
            });
            return Json(new { data = data });
        }
    }
}