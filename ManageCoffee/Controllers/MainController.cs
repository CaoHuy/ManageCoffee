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
using ManageCoffee.Models.Authentication;
using Newtonsoft.Json;

namespace ManageCoffee.Controllers
{
    [Authentication]
    public class MainController : Controller
    {
        private ManageCoffeeContext dbContext;

        public MainController()
        {
            dbContext = new ManageCoffeeContext();
        }
        public IActionResult Index()
        {
            var catalogues = CatalogueDAO.Instance.GetCataloguesList(); // Lấy danh sách catalogue từ cơ sở dữ liệu hoặc từ nguồn dữ liệu khác
            ViewBag.Catalogues = catalogues;
            var areas = AreaDAO.Instance.GetAreaList(); // Lấy danh sách khu vực từ cơ sở dữ liệu hoặc từ nguồn dữ liệu khác
            ViewBag.areas = areas;
            return View();
        }

        public object Load()
        {
            return Json(new
            {
                products = ProductDAO.Instance.GetProductList(),
                tables = TableDAO.Instance.GetTableList()
            });
        }

        public object Create(IFormCollection request)
        {
            foreach (var field in request)
            {
                System.Console.WriteLine($"{field.Key}: {field.Value}");
            }
            Order order = new Order();
            User user = JsonConvert.DeserializeObject<User>(HttpContext.Session.GetString("user"));
            order.UserId = 1;
            order.Status = 0;
            order.Note = request["Note"];
            if (request["table_id"] != "")
            {
                order.TableId = int.Parse(request["table_id"]);
            }
            order.TotalPrice = int.Parse(request["total_price"]);
            dbContext.Orders.Add(order);
            dbContext.SaveChanges();
            for (int i = 0; i < request["quantity[]"].Count(); i++)
            {
                Detail detail = new Detail();
                detail.OrderId = order.OrderId;
                detail.ProductId = int.Parse(request["id[]"][i]);
                detail.Quantity = int.Parse(request["quantity[]"][i]);
                detail.Price = int.Parse(request["price[]"][i]);
                dbContext.Add(detail);
                dbContext.SaveChanges();
            }
            var table = dbContext.Tables.FirstOrDefault(t => t.TableId == order.TableId);
            if (table != null)
            {
                table.Status = 1;
                dbContext.SaveChanges();
            }
            return Json(new
            {
                msg = "Đã tạo đơn hàng thành công",
                status = "success",
            });
        }

        [HttpPost]
        public object Edit(IFormCollection request)
        {
            Order order = OrderDAO.Instance.GetOrderByID(int.Parse(request["id"]));
            User user = JsonConvert.DeserializeObject<User>(HttpContext.Session.GetString("user"));
            order.UserId = 1;
            order.TotalPrice = int.Parse(request["total_price"]);
            dbContext.Orders.Update(order);
            dbContext.SaveChanges();
            order.RemoveDetails();
            //Tạo detail
            for (int i = 0; i < request["quantity[]"].Count(); i++)
            {
                Detail detail = new Detail();
                detail.OrderId = order.OrderId;
                detail.ProductId = int.Parse(request["id[]"][i]);
                detail.Quantity = int.Parse(request["quantity[]"][i]);
                detail.Price = int.Parse(request["price[]"][i]);
                detail.Note = request["note[]"][i];
                detail.Status = int.Parse(request["status[]"][i]);
                DetailDAO.Instance.AddNew(detail);
            }

            return Json(new
            {
                msg = "Đã cập nhật đơn hàng thành công",
                status = "success",
            });
        }

        public object GetOrderByTableId(int id)
        {
            Order order = dbContext.Orders.FirstOrDefault(o => o.TableId == id && o.Status == 0);
            if (order != null)
            {
                return Json(new
                {
                    order = order,
                    details = order.GetDetail(),
                    table = order.GetTable(),
                });
            }
            else
            {
                return BadRequest();
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

        public IActionResult GetCatalogueName(int catalogueId)
        {
            string catalogueName = GetCatalogueNameById(catalogueId);
            return Json(new { catalogueName });
        }

        public string GetAreaNameById(int? areaId)
        {
            if (areaId.HasValue)
            {
                using (var context = new ManageCoffeeContext())
                {
                    var area = context.Areas.Find(areaId);
                    return area?.Name ?? "Không có";
                }
            }
            return "Không có";
        }

        public IActionResult GetAreaName(int areaId)
        {
            string areaName = GetAreaNameById(areaId);
            return Json(new { areaName });
        }

        public IActionResult CompleteDetail(int id)
        {
            Detail detail = DetailDAO.Instance.GetDetailByID(id);
            OrderDAO orderDAO = new OrderDAO();
            TableDAO tableDAO = new TableDAO();
            detail.Status = 2;
            DetailDAO.Instance.Update(detail);
            return Json(new { str = "Đã cập nhật trạng thái món" });
        }

    }
}