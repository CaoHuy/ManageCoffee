using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ManageCoffee.DAO;
using ManageCoffee.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace ManageCoffee.Controllers
{

    public class TableController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.IsActive = "table";
            return View();
        }

        public IActionResult Load()
        {

            var tables = TableDAO.Instance.GetTableList();
            var data = tables.Select(t => new
            {
                name = "<a class='btn btn-link text-decoration-none' href='/Table/Edit/" + t?.TableId + "'>" + t?.Name + " </ a > ",
                area = t?.Area,
                note = t?.Note,
                status = t?.GetStatus(),
                action = "<form action='/Table/Delete' method='POST' class='save-form'><input type='hidden' name='id' value='" + t?.TableId + "' data-id='" + t?.TableId + "'/> <button type='submit' class='btn btn-link text-decoration-none btn-remove'><i class='bi bi-trash3'></i></button></form>"
            });
            return Json(new { data = data });
        }

        public IActionResult Create()
        {
            var areas = AreaDAO.Instance.GetAreaList();
            ViewBag.Areas = areas;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Table table)
        {
            if (ModelState.IsValid)
            {
                if (TableDAO.Instance.IsTableExists(table.Name) && TableDAO.Instance.IsAreaExists(table.Area))
                {
                    // Nếu bàn và khu vực đã tồn tại, thêm thông báo lỗi vào ModelState
                    ModelState.AddModelError("Name", "Bàn và khu vực đã tồn tại.");
                    ModelState.AddModelError("Area", "Bàn và khu vực đã tồn tại.");
                    var areas = AreaDAO.Instance.GetAreaList();
                    ViewBag.Areas = areas;
                    // Trả về view với dữ liệu table để người dùng có thể nhập lại
                    return View(table);
                }
                TableDAO.Instance.AddNew(table);
                User user = JsonConvert.DeserializeObject<User>(HttpContext.Session.GetString("user"));
                var dbContext = new ManageCoffeeContext();
                dbContext.SaveChanges();
                ViewBag.IsActive = "table";
                return RedirectToAction(nameof(Index));
            }
            // Nếu ModelState không hợp lệ, trả về view với dữ liệu table để người dùng có thể nhập lại
            var area = AreaDAO.Instance.GetAreaList();
            ViewBag.Areas = area;
            return View(table);
        }


        public ActionResult GetTableFree(Table table)
        {
            var context = new ManageCoffeeContext();
            var tables = context.Tables.Where(t => t.Status == 0).ToList();
            return Json(tables);
        }

        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var table = TableDAO.Instance.GetTableByID(id.Value);
            if (table == null)
            {
                return NotFound();
            }
            var areas = AreaDAO.Instance.GetAreaList();
            ViewBag.Areas = areas;
            return View("Edit", table);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, Table table)
        {
            try
            {
                var existingTable = TableDAO.Instance.GetTableByID(id);
                if (existingTable != null && existingTable.Name == table.Name && existingTable.Area == table.Area && existingTable.Note == table.Note)
                {
                    System.Console.WriteLine(id + " " + Json(table));
                    table.TableId = id;
                    User user = JsonConvert.DeserializeObject<User>(HttpContext.Session.GetString("user"));
                    var dbContext = new ManageCoffeeContext();
                    TableDAO.Instance.Update(table);
                    // LogDAO dao = new LogDAO();
                    // dao.AddNew(new Log
                    // {
                    //     Id = 0,
                    //     UserId = user.Id,
                    //     Action = "Đã cập nhật",
                    //     Object = "Bàn",
                    //     ObjectId = table.Id,
                    // });
                    dbContext.SaveChanges();
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    // Nếu bàn và khu vực đã tồn tại, thêm thông báo lỗi vào ModelState
                    ModelState.AddModelError("Name", "Bàn và khu vực đã tồn tại.");
                    ModelState.AddModelError("Area", "Bàn và khu vực đã tồn tại.");
                    var areas = AreaDAO.Instance.GetAreaList();
                    ViewBag.Areas = areas;
                    // Trả về view với dữ liệu table để người dùng có thể nhập lại
                    return View(table);
                }
            }
            catch (Exception ex)
            {
                var tableList = TableDAO.Instance.GetTableList();
                ViewBag.Message = ex.Message;
                return View("Index", tableList);
            }
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            object response = null;
            try
            {
                if (TableDAO.Instance.GetTableByID(id) == null)
                {
                    response = new
                    {
                        controller = "Table",
                        title = "Đã có lỗi xảy ra trong quá trình xóa! Vui lòng thử lại sau.",
                        status = "danger"
                    };
                    return Json(response);
                }
                else
                {
                    TableDAO.Instance.Remove(id);
                    User user = JsonConvert.DeserializeObject<User>(HttpContext.Session.GetString("user"));
                    var dbContext = new ManageCoffeeContext();
                    // LogDAO dao = new LogDAO();
                    // dao.AddNew(new Log
                    // {
                    //     Id = 0,
                    //     UserId = user.Id,
                    //     Action = "Đã xóa",
                    //     Object = "Bàn",
                    //     ObjectId = id,
                    // });
                    dbContext.SaveChanges();
                    response = new
                    {
                        controller = "Table",
                        title = "Đã xóa thành công.",
                        status = "success"
                    };
                }
                return Json(response);
            }
            catch (Exception ex)
            {
                System.Console.WriteLine(ex);
                response = new
                {
                    controller = "Table",
                    title = "Đã có lỗi xảy ra trong quá trình xóa! Vui lòng thử lại sau.",
                    status = "danger"
                };
                return Json(response);
            }
        }
    }
}