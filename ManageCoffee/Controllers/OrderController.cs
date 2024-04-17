using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ManageCoffee.DAO;
using ManageCoffee.Models;

namespace ManageCoffee.Controllers
{
    // [Authentication]
    public class OrderController : Controller
    {
        private ManageCoffeeContext dbContext = null;

        public OrderController()
        {
            dbContext = new ManageCoffeeContext();
        }


        public ActionResult Index()
        {
            ViewBag.IsActive = "order";
            return View();
        }

        public IActionResult Load()
        {
            var orders = OrderDAO.Instance.GetOrderList();
            var data = orders.Select(o => new
            {
                id = o?.OrderId,
                table_id = "<a class='btn btn-link text-decoration-none' href='/Order/Edit/" + o?.OrderId + "'>" + o?.getTableName() + " </ a > ",
                user_id = o.getUserName(),
                total_price = "<span class='price'>" + (int)o?.TotalPrice + "</span>",
                note = o?.Note,
                created_at = o?.CreatedAt.Value.ToString("HH:mm:ss dd/MM/yyyy"),
                status = "<span class='status " + (o.Status == 0 ? "text-danger" : "text-success") + "'>" + o?.getStatus() + "</span>",
                action = "<div class='d-flex'><a class='btn text-dark btn-print-bill me-2' data-id='" + o?.OrderId + "'><i class='bi bi-printer'></i></a><a class='btn text-success btn-pay me-2' data-id='" + o?.OrderId + "'><i class='bi bi-currency-dollar'></i></a><form action='/Order/Delete' method='POST' class='save-form'><input type='hidden' name='id' value='" + o?.OrderId + "' data-id='" + o?.OrderId + "'/> <button type='submit' class='btn btn-link text-decoration-none btn-remove'><i class='bi bi-trash3'></i></button></form></div>"
            });
            return Json(new { data = data });
        }

        public ActionResult Create()
        {
            ViewBag.IsActive = "order";
            return View();
        }

        [HttpPost]
        public ActionResult Create(Order Order, List<Detail> details)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    User user = JsonConvert.DeserializeObject<User>(HttpContext.Session.GetString("user"));
                    Order.UserId = user.UserId;
                    OrderDAO.Instance.AddNew(Order);
                    var table = dbContext.Tables.FirstOrDefault(t => t.TableId == Order.TableId);
                    if (table != null)
                    {
                        table.Status = 1;
                        dbContext.SaveChanges();
                    }
                    // IDetailRepository detailRepository = new DetailRepository();
                    // LogDAO dao = new LogDAO();
                    // dao.AddNew(new Log
                    // {
                    //     Id = 0,
                    //     UserId = user.Id,
                    //     Action = "Đã tạo",
                    //     Object = "Đơn hàng",
                    //     ObjectId = Order.Id,
                    // });
                    // dbContext.SaveChanges();
                }
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return View(Order);
            }
        }

        public ActionResult getOrder(int? id)
        {
            var order = OrderDAO.Instance.GetOrderByID(id.Value);
            if (order == null)
            {
                return NotFound();
            }
            ViewBag.IsActive = "order";
            return Json(new
            {
                order = order,
                table = order.GetTable(),
                user = order.GetUser(),
                details = order.GetDetail(),
                products = dbContext.Products.ToList(),
            });
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var order = OrderDAO.Instance.GetOrderByID(id.Value);
            if (order == null)
            {
                return NotFound();
            }

            ViewBag.IsActive = "order";
            return View(order);
        }

        [HttpPost]
        public ActionResult Edit(int id, Order order)
        {
            try
            {
                // Kiểm tra tính hợp lệ của dữ liệu đầu vào
                if (!ModelState.IsValid)
                {
                    foreach (var modelState in ModelState.Values)
                    {
                        foreach (var error in modelState.Errors)
                        {
                            System.Console.WriteLine("Error: " + error.ErrorMessage);
                        }
                    }
                    return View("Index", order);
                }
                Order ord = OrderDAO.Instance.GetOrderByID(id);
                order.CreatedAt = ord.CreatedAt;
                order.RemoveDetails();
                var dbContext = new ManageCoffeeContext();
                if (ord.TableId != order.TableId)
                {
                    dbContext.Tables.FirstOrDefault(t => t.TableId == ord.TableId).Status = 0;
                    dbContext.Tables.FirstOrDefault(t => t.TableId == order.TableId).Status = 1;
                }
                User user = JsonConvert.DeserializeObject<User>(HttpContext.Session.GetString("user"));
                order.UserId = user.UserId;
                OrderDAO.Instance.Update(order);
                // LogDAO dao = new LogDAO();
                // dao.AddNew(new Log
                // {
                //     Id = 0,
                //     UserId = user.Id,
                //     Action = "Đã cập nhật",
                //     Object = "Sản phẩm",
                //     ObjectId = order.Id,
                // });
                dbContext.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                // Xử lý lỗi và trả về phản hồi phù hợp
                ViewBag.Message = ex.Message;
                return View(order);
            }
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            object response = null;
            try
            {
                if (OrderDAO.Instance.GetOrderByID(id) == null)
                {
                    response = new
                    {
                        controller = "Order",
                        title = "Đã có lỗi xảy ra trong quá trình xóa! Vui lòng thử lại sau.",
                        status = "danger"
                    };
                }
                else
                {
                    OrderDAO.Instance.Remove(id);
                    response = new
                    {
                        controller = "Order",
                        title = "Đã xóa thành công.",
                        status = "success"
                    };
                    User user = JsonConvert.DeserializeObject<User>(HttpContext.Session.GetString("user"));
                    var dbContext = new ManageCoffeeContext();
                    // LogDAO dao = new LogDAO();
                    // dao.AddNew(new Log
                    // {
                    //     Id = 0,
                    //     UserId = user.Id,
                    //     Action = "Đã xóa",
                    //     Object = "Đơn hàng",
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
                    controller = "Table",
                    title = "Đã có lỗi xảy ra trong quá trình xóa! Vui lòng thử lại sau.",
                    status = "danger"
                };
                return Json(response);
            }
        }

        [HttpPost]
        public ActionResult Pay(int id, int payment)
        {
            try
            {
                Order order = OrderDAO.Instance.GetOrderByID(id);
                order.Status = payment;
                OrderDAO.Instance.Update(order);
                User user = JsonConvert.DeserializeObject<User>(HttpContext.Session.GetString("user"));
                order.UserId = user.UserId;
                OrderDAO.Instance.Update(order);
                var dbContext = new ManageCoffeeContext();
                if (order.TableId != null)
                {
                    var table = dbContext.Tables.FirstOrDefault(t => t.TableId == order.TableId);
                    table.Status = 0;
                    dbContext.SaveChanges();
                }
                // LogDAO dao = new LogDAO();
                // dao.AddNew(new Log
                // {
                //     Id = 0,
                //     UserId = user.Id,
                //     Action = "Đã thanh toán",
                //     Object = "Đơn hàng",
                //     ObjectId = order.Id,
                // });

                return Ok();
            }
            catch (Exception ex)
            {
                System.Console.WriteLine(ex);
                return StatusCode(500, "Lỗi server");
            }
        }

        public ActionResult Bill(int order_id)
        {
            ViewBag.IsActive = order_id + "";
            return View("Bill");
        }
    }
}