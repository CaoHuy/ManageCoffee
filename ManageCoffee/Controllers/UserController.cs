using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using ManageCoffee.DAO;
using ManageCoffee.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using ManageCoffee.Models.Authentication;

namespace ManageCoffee.Controllers
{
    [Authentication]
    public class UserController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.IsActive = "user";
            return View();
        }

        public IActionResult Load()
        {
            var users = UserDAO.Instance.GetUserList();
            var data = users.Select(u => new
            {
                name = "<a class='btn btn-link text-decoration-none' href='/User/Edit/" + u.UserId + "'>" + u.Name + " </ a > ",
                role = u.getRoleName(),
                email = u.Email,
                phone = "" + u.Phone,
                action = "<form action='/User/Delete' method='POST' class='save-form'><input type='hidden' name='id' value='" + u.UserId + "' data-id='" + u.UserId + "'/> <button type='button' class='btn btn-link text-decoration-none btn-remove'><i class='bi bi-trash3'></i></button></form>"
            });

            return Json(new
            {
                data = data,
            });
        }

        public IActionResult Get(int id)
        {
            User user = UserDAO.Instance.GetUserByID(id);
            ViewBag.IsActive = "user";
            return Json(user);
        }

        public IActionResult Create()
        {
            ViewBag.IsActive = "user";
            return View();
        }

        [HttpPost]
        public ActionResult Create(User user)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(user);
                }
                else
                {
                    if (UserDAO.Instance.IsUserEmailExists(user.Email))
                    {
                        ModelState.AddModelError("Email", "Email đã tồn tại");
                    }

                    if (UserDAO.Instance.IsUserPhoneExists(user.Phone))
                    {
                        ModelState.AddModelError("Phone", "Số điện thoại đã tồn tại");
                    }

                    if (UserDAO.Instance.IsUserEmailExists(user.Email) || UserDAO.Instance.IsUserPhoneExists(user.Phone))
                    {
                        return View(user);
                    }
                    user.Password = HashPassword(user.Password);
                    UserDAO.Instance.AddNew(user);
                    var dbContext = new ManageCoffeeContext();
                    User auth = JsonConvert.DeserializeObject<User>(HttpContext.Session.GetString("user"));
                    LogDAO dao = new LogDAO();
                    dao.AddNew(new Log
                    {
                        LogId = 0,
                        UserId = user.UserId,
                        Action = "Đã tạo",
                        Object = "Tài khoản mới",
                        ObjectId = user.UserId,
                    });
                    dbContext.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return View(user);
            }
        }
        public static string HashPassword(string password)
        {
            // Hash the password using MD5
            using (MD5 md5 = MD5.Create())
            {
                byte[] inputBytes = Encoding.ASCII.GetBytes(password);
                byte[] hashBytes = md5.ComputeHash(inputBytes);
                // Convert the byte array to hexadecimal string
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("x2"));
                }
                return sb.ToString();
            }
        }


        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var user = UserDAO.Instance.GetUserByID(id.Value);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, User user)
        {
            try
            {
                var existingUser = UserDAO.Instance.GetUserByID(id);

                // Kiểm tra xem người dùng đã thay đổi số điện thoại hoặc email hay không
                if (existingUser.Email != user.Email && UserDAO.Instance.IsUserEmailExists(user.Email))
                {
                    ModelState.AddModelError("Email", "Email đã tồn tại");
                    return View(user);
                }
                if (existingUser.Phone != user.Phone && UserDAO.Instance.IsUserPhoneExists(user.Phone))
                {
                    ModelState.AddModelError("Phone", "Số điện thoại đã tồn tại");
                    return View(user);
                }

                // Cập nhật thông tin người dùng
                user.Password = existingUser.Password;
                UserDAO.Instance.Update(user);
                user.Password = UserDAO.Instance.GetUserByID(id).Password;
                // Tiến hành cập nhật thông tin vai trò
                UserDAO.Instance.Update(user);
                User auth = JsonConvert.DeserializeObject<User>(HttpContext.Session.GetString("user"));
                var dbContext = new ManageCoffeeContext();
                LogDAO dao = new LogDAO();
                dao.AddNew(new Log
                {
                    LogId = 0,
                    UserId = user.UserId,
                    Action = "Đã cập nhật",
                    Object = "tài khoản",
                    ObjectId = user.UserId,
                });
                dbContext.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                // Xử lý lỗi và trả về phản hồi phù hợp
                ViewBag.Message = ex.Message;
                return View(user);
            }
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            object response = null;
            try
            {
                if (UserDAO.Instance.GetUserByID(id) == null)
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
                        controller = "User",
                        title = "Đã xóa thành công.",
                        status = "success"
                    };
                    User user = JsonConvert.DeserializeObject<User>(HttpContext.Session.GetString("user"));
                    UserDAO.Instance.Remove(id);
                    var dbContext = new ManageCoffeeContext();
                    LogDAO dao = new LogDAO();
                    dao.AddNew(new Log
                    {
                        LogId = 0,
                        UserId = user.UserId,
                        Action = "Đã xóa",
                        Object = "tài khoản",
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

        public User GetUserInfo()
        {
            return Helper.UserInfo(HttpContext);
        }
    }
}