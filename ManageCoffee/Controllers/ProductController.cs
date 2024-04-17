using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using MCF.DAO;
using ManageCoffee.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json;

namespace MCF.Controllers
{
    public class ProductController : Controller
    {
        private readonly IWebHostEnvironment hostingEnvironment;

        public ProductController(IWebHostEnvironment environment)
        {
            hostingEnvironment = environment;
        }

        public ActionResult Index()
        {
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
                catalogue = p.getCatalogueName(),
                image = p.Image,
                action = "<form action='/Product/Delete' method='POST' class='save-form'><input type='hidden' name='id' value='" + p.ProductId + "' data-id='" + p.ProductId + "'/> <button type='button' class='btn btn-link text-decoration-none btn-remove-product'><i class='bi bi-trash3'></i></button></form>"
            });

            return Json(new
            {
                data = data,
            });
        }

        public ActionResult getProduct(int id)
        {
            var product = ProductDAO.Instance.GetProductByID(id);
            if (product == null)
            {
                return NotFound();
            }
            var context = new ManageCoffeeContext();
            ViewBag.IsActive = "product";
            return Json(new
            {
                product = product,
            });
        }

        public IActionResult Create()
        {
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
                    var dbContext = new ManageCoffeeContext();
                   
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
            ViewBag.IsActive = "product";
            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, Product product, IFormFile imageFile)
        {
            try
            {
                // Kiểm tra tính hợp lệ của dữ liệu đầu vào
                if (!ModelState.IsValid)
                {
                    return View("Index", product);
                }
                if (imageFile != null && imageFile.Length > 0 )
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
                    var existingProduct = ProductDAO.Instance.GetProductByID(id);
                    if (existingProduct != null)
                    {
                        product.Image = existingProduct.Image;
                    }
                }
                User user = JsonConvert.DeserializeObject<User>(HttpContext.Session.GetString("user"));
                ProductDAO.Instance.Update(product);    
                var dbContext = new ManageCoffeeContext();
                // SettingDAO dao = new SettingDAO();
                // dao.AddNew(new Log
                // {
                //     Id = 0,
                //     UserId = user.Id,
                //     Action = "Đã cập nhật",
                //     Object = "Sản phẩm",
                //     ObjectId = product.Id,
                // });
                dbContext.SaveChanges();
                return RedirectToAction(nameof(Index));
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
                    // SettingDAO dao = new SettingDAO();
                    // dao.AddNew(new Config
                    // {
                    //     Id = 0,
                    //     UserId = user.Id,
                    //     Action = "Đã xóa",
                    //     Object = "Sản phẩm",
                    //     ObjectId = id,
                    // });
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