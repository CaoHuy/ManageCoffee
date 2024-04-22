using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ManageCoffee.DAO;
using ManageCoffee.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using ManageCoffee.Models.Authentication;

namespace ManageCoffee.Controllers
{
    [Authentication]
    public class ProductController : Controller
    {

        private readonly IWebHostEnvironment hostingEnvironment;

        public ProductController(IWebHostEnvironment environment)
        {
            hostingEnvironment = environment;
        }


        public ActionResult Index()
        {
            var catalogues = CatalogueDAO.Instance.GetCataloguesList();
            ViewBag.IsActive = "product";
            return View();
        }

        public IActionResult Load()
        {
            var products = ProductDAO.Instance.GetProductList();
            var data = products.Select(p => new
            {
                name = "<a class='btn btn-link text-decoration-none' href='/Product/Edit/" + p.ProductId + "'>" + p.Name + " </ a >",
                unit = p.Unit,
                price = p.Price,
                catalogue = GetCatalogueNameById(p?.CatalogueId),
                image = p.Image,
                action = "<form action='/Product/Delete' method='POST' class='save-form'><input type='hidden' name='id' value='" + p.ProductId + "' data-id='" + p.ProductId + "'/> <button type='button' class='btn btn-link text-decoration-none btn-remove-product' data-id='" + p.ProductId + "'><i class='bi bi-trash3'></i></button></form>"
            });

            return Json(new
            {
                data = data,
                products = products,
            });
        }

        public ActionResult GetProduct(int id)
        {
            var context = new ManageCoffeeContext();
            var product = ProductDAO.Instance.GetProductByID(id);
            var details = context.Details.Where(d => d.ProductId == id).ToList();
            if (product == null)
            {
                return NotFound();
            }
            ViewBag.IsActive = "product";
            return Json(new
            {
                product = product,
                details = details
            });
        }

        public IActionResult Create()
        {
            var catalogues = CatalogueDAO.Instance.GetCataloguesList();
            ViewBag.Catalogues = catalogues;
            ViewBag.IsActive = "product";
            return View();
        }

        [HttpPost]
        public ActionResult Create(Product product, IFormFile imageFile)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var catalogue = CatalogueDAO.Instance.GetCataloguesList();
                    ViewBag.Catalogues = catalogue;
                    return View(product);
                }
                else
                {
                    if (imageFile != null && imageFile.Length > 0)
                    {
                        // Lưu ảnh vào thư mục wwwroot/images
                        var uploadsFolder = Path.Combine(hostingEnvironment.WebRootPath, "img");
                        var uniqueFileName = Guid.NewGuid().ToString() + "_" + imageFile.FileName;
                        var filePath = Path.Combine(uploadsFolder, uniqueFileName);
                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            imageFile.CopyTo(fileStream);
                        }
                        // Lưu đường dẫn của ảnh vào trường Image của đối tượng Product
                        product.Image = "/img/" + uniqueFileName;
                    }
                    else
                    {
                        product.Image = "/img/placeholder.jpg";
                    }
                    ProductDAO.Instance.AddNew(product);
                    User user = JsonConvert.DeserializeObject<User>(HttpContext.Session.GetString("user"));
                    var dbContext = new ManageCoffeeContext();
                    LogDAO dao = new LogDAO();
                    dao.AddNew(new Log
                    {
                        LogId = 0,
                        UserId = user.UserId,
                        Action = "Đã tạo",
                        Object = "Sản phẩm",
                        ObjectId = product.ProductId,
                    });
                    dbContext.SaveChanges();
                }
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                System.Console.WriteLine(ex);
                ViewBag.Message = ex.Message;
                return View(product);
            }
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = ProductDAO.Instance.GetProductByID(id.Value);
            if (product == null)
            {
                return NotFound();
            }

            var catalogues = CatalogueDAO.Instance.GetCataloguesList();
            ViewBag.Catalogues = catalogues;
            ViewBag.IsActive = "product";

            return View("Edit", product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, Product product, IFormFile imageFile)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var existingProduct = ProductDAO.Instance.GetProductByID(id);

                    // Kiểm tra xem sản phẩm cần cập nhật có tồn tại không
                    if (existingProduct != null)
                    {
                        // Kiểm tra xem có tải lên tệp hình ảnh mới không
                        if (imageFile != null && imageFile.Length > 0)
                        {
                            // Lưu ảnh vào thư mục wwwroot/images
                            var uploadsFolder = Path.Combine(hostingEnvironment.WebRootPath, "img");
                            var uniqueFileName = Guid.NewGuid().ToString() + "_" + imageFile.FileName;
                            var filePath = Path.Combine(uploadsFolder, uniqueFileName);
                            using (var fileStream = new FileStream(filePath, FileMode.Create))
                            {
                                imageFile.CopyTo(fileStream);
                            }

                            // Cập nhật đường dẫn hình ảnh mới cho sản phẩm
                            product.Image = "/img/" + uniqueFileName;
                        }
                        else
                        {
                            // Nếu không có hình ảnh mới được tải lên, giữ nguyên đường dẫn hình ảnh cũ
                            product.Image = existingProduct.Image;
                        }

                        // Cập nhật thông tin sản phẩm
                        User user = JsonConvert.DeserializeObject<User>(HttpContext.Session.GetString("user"));
                        product.ProductId = id; // Đảm bảo rằng Id của sản phẩm không thay đổi
                        ProductDAO.Instance.Update(product);
                        var dbContext = new ManageCoffeeContext(); 
                        LogDAO dao = new LogDAO();
                        dao.AddNew(new Log
                        {
                            LogId = 0,
                            UserId = user.UserId,
                            Action = "Đã cập nhật",
                            Object = "Sản phẩm",
                            ObjectId = product.ProductId,
                        });

                        dbContext.SaveChanges();

                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        return NotFound(); // Trả về NotFound nếu sản phẩm không tồn tại
                    }
                }
                else
                {
                    // ModelState không hợp lệ, trả về view với dữ liệu sản phẩm để người dùng có thể sửa lại
                    var catalogues = CatalogueDAO.Instance.GetCataloguesList();
                    ViewBag.Catalogues = catalogues;
                    ViewBag.IsActive = "product";
                    return View(product);
                }
            }
            catch (Exception ex)
            {
                System.Console.WriteLine(ex);
                ViewBag.Message = ex.Message;
                return View(product);
            }
        }



        [HttpPost]
        public ActionResult Delete(int id)
        {
            object response = null;
            try
            {
                if (ProductDAO.Instance.GetProductByID(id) == null)
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
                    ProductDAO DAO = new ProductDAO();
                    DAO.DeleteDetailsByProductId(id);
                    ProductDAO.Instance.Remove(id);
                    User user = JsonConvert.DeserializeObject<User>(HttpContext.Session.GetString("user"));
                    response = new
                    {
                        controller = "Product",
                        title = "Đã xóa thành công.",
                        status = "success"
                    };
                    var dbContext = new ManageCoffeeContext();
                    LogDAO dao = new LogDAO();
                    dao.AddNew(new Log
                    {
                        LogId = 0,
                        UserId = user.UserId,
                        Action = "Đã xóa",
                        Object = "Sản phẩm",
                        ObjectId = id,
                    });
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
        public string GetCatalogueNameById(int? catalogueId)
        {
            if (catalogueId.HasValue)
            {
                using (var context = new ManageCoffeeContext())
                {
                    var catalogue = context.Catalogues.Find(catalogueId);
                    return catalogue?.Name ?? "Không có";
                }
            }
            return "Không có";
        }
    }
}