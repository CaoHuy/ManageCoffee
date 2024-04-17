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
using Newtonsoft.Json;

namespace ManageCoffee.Controllers
{
    public class AreaController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public IActionResult Load()
        {
            var areas = AreaDAO.Instance.GetAreaList();
            var data = areas.Select(u => new
            {
                id = u.AreaId,
                name = "<a class='btn btn-link text-decoration-none' href='/Area/Edit/" + u.AreaId + "'>" + u.Name + " </ a > ",
                action = "<form action='/Area/Delete' method='POST' class='save-form'><input type='hidden' name='id' value='" + u.AreaId + "' data-id='" + u.AreaId + "'/> <button type='button' class='btn btn-link text-decoration-none btn-remove'><i class='bi bi-trash3'></i></button></form>"
            });

            return Json(new
            {
                data = data,
            });
        }

        public IActionResult Get(int id)
        {
            Area user = AreaDAO.Instance.GetAreaByID(id);
            ViewBag.IsActive = "user";
            return Json(user);
        }

        public IActionResult Create()
        {
            ViewBag.IsActive = "user";
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Area area)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(area);
                }
                else
                {
                    if (AreaDAO.Instance.IsAreaExist(area.Name))
                    {
                        ModelState.AddModelError("Name", "Khu vực này đã tồn tại");
                        return View(area);
                    }
                    AreaDAO.Instance.AddNew(area);
                    var dbContext = new ManageCoffeeContext();

                    dbContext.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return View(area);
            }
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var area = AreaDAO.Instance.GetAreaByID(id.Value);
            if (area == null)
            {
                return NotFound();
            }
            return View(area);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, Area area)
        {
            try
            {

                // Lấy khu vực hiện tại từ ID
                var existingArea = AreaDAO.Instance.GetAreaByID(id);

                // Kiểm tra xem khu vực mới đã tồn tại hay chưa (trừ khu vực hiện tại của bản ghi đang được chỉnh sửa)
                if (AreaDAO.Instance.IsAreaExist(area.Name) && area.Name != existingArea.Name)
                {
                    ModelState.AddModelError("Name", "Khu vực này đã tồn tại");
                    return View(area);
                }
                AreaDAO.Instance.Update(area);
                var dbContext = new ManageCoffeeContext();
                dbContext.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                // Xử lý lỗi và trả về phản hồi phù hợp
                ViewBag.Message = ex.Message;
                return View(area);
            }
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            object response = null;
            try
            {
                if (AreaDAO.Instance.GetAreaByID(id) == null)
                {
                    response = new
                    {
                        title = "Đã có lỗi xảy ra trong quá trình xóa! Vui lòng thử lại sau.",
                        status = "danger"
                    };
                    return Json(response);
                }
                else
                {
                    response = new
                    {
                        controller = "Area",
                        title = "Đã xóa thành công.",
                        status = "success"
                    };
                    AreaDAO.Instance.Remove(id);
                    var dbContext = new ManageCoffeeContext();
                    dbContext.SaveChanges();
                }
                return Json(response);
            }
            catch (System.Exception)
            {
                response = new
                {
                    title = "Đã có lỗi xảy ra trong quá trình xóa! Vui lòng thử lại sau.",
                    status = "danger"
                };
                return Json(response);
            }
        }

    }
}