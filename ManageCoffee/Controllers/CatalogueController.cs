using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using ManageCoffee.DAO;
using ManageCoffee.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ManageCoffee.Controllers
{

    public class CatalogueController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public IActionResult Load()
        {
            var catalogues = CatalogueDAO.Instance.GetCataloguesList();
            var data = catalogues.Select(u => new
            {
                id = u.CatalogueId,
                name = "<a class='btn btn-link text-decoration-none' href='/Catalogue/Edit/" + u.CatalogueId + "'>" + u.Name + " </ a > ",
                action = "<form action='/Catalogue/Delete' method='POST' class='save-form'><input type='hidden' name='id' value='" + u.CatalogueId + "' data-id='" + u.CatalogueId + "'/> <button type='button' class='btn btn-link text-decoration-none btn-remove'><i class='bi bi-trash3'></i></button></form>"
            });

            return Json(new
            {
                data = data,
            });
        }
        public IActionResult Get(int id)
        {
            Catalogue user = CatalogueDAO.Instance.GetCatalogueById(id);
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
        public ActionResult Create(Catalogue catalogue)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(catalogue);
                }
                else
                {
                    if (CatalogueDAO.Instance.IsCatalogueExist(catalogue.Name))
                    {
                        ModelState.AddModelError("Name", "Danh mục này đã tồn tại");
                        return View(catalogue);
                    }
                    CatalogueDAO.Instance.AddNew(catalogue);
                    var dbContext = new ManageCoffeeContext();

                    dbContext.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return View(catalogue);
            }
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var catalogue = CatalogueDAO.Instance.GetCatalogueById(id.Value);
            if (catalogue == null)
            {
                return NotFound();
            }
            return View(catalogue);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, Catalogue catalogue)
        {
            try
            {

                // Lấy Danh mục hiện tại từ ID
                var existingCatalogue = CatalogueDAO.Instance.GetCatalogueById(id);

                // Kiểm tra xem Danh mục mới đã tồn tại hay chưa (trừ Danh mục hiện tại của bản ghi đang được chỉnh sửa)
                if (CatalogueDAO.Instance.IsCatalogueExist(catalogue.Name) && catalogue.Name != existingCatalogue.Name)
                {
                    ModelState.AddModelError("Name", "Danh mục này đã tồn tại");
                    return View(catalogue);
                }
                CatalogueDAO.Instance.Update(catalogue);
                var dbContext = new ManageCoffeeContext();
                dbContext.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                // Xử lý lỗi và trả về phản hồi phù hợp
                ViewBag.Message = ex.Message;
                return View(catalogue);
            }
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            object response = null;
            try
            {
                if (CatalogueDAO.Instance.GetCatalogueById(id) == null)
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
                        controller = "Catalogue",
                        title = "Đã xóa thành công.",
                        status = "success"
                    };
                    CatalogueDAO.Instance.Remove(id);
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