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

namespace ManageCoffee.Controllers
{
    public class LoginController : Controller
    {
        public ActionResult Index()
        {
            return View("~/Views/Login/Index.cshtml");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(User user)
        {

            if (!IsValid(user))
            {
                ViewData["Message"] = "Mật khẩu và email không được trống!";
                return View("~/Views/Login/Index.cshtml");
            }
            else if (!IsValidEmail(user.Email))
            {
                ViewData["Message"] = "Email phải đúng định dạng!";
                return View("~/Views/Login/Index.cshtml");
            }
            else
            {
                System.Console.WriteLine("Kiem tra ");
                var userList = UserDAO.Instance.GetUserList();

                string hashedPassword = GetMd5Hash(user.Password);

                var authenticatedUser = userList.FirstOrDefault(u => u.Email == user.Email && u.Password == hashedPassword);

                if (authenticatedUser != null)
                {
                    System.Console.WriteLine("thanh cong ");
                    // Lấy ISession
                    HttpContext context = HttpContext;
                    var session = context.Session;
                    string key_access = "user";

                    // Convert accessInfo thành chuỗi Json và lưu lại vào Session
                    string userJson = Newtonsoft.Json.JsonConvert.SerializeObject(authenticatedUser);
                    session.SetString(key_access, userJson);

                    return RedirectToAction("Index", "Home");
                }
                System.Console.WriteLine("That bai");
                ViewData["Message"] = "Đăng nhập thất bại! Mật khẩu hoặc email không đúng!";
                return View("~/Views/Login/Index.cshtml");
            }
        }

        private string GetMd5Hash(string input)
        {
            using (MD5 md5Hash = MD5.Create())
            {
                byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));
                StringBuilder stringBuilder = new StringBuilder();

                for (int i = 0; i < data.Length; i++)
                {
                    stringBuilder.Append(data[i].ToString("x2"));
                }
                return stringBuilder.ToString();
            }
        }

        public ActionResult Logout()
        {
            HttpContext.Session.Clear();
            HttpContext.Session.Remove("user");
            return RedirectToAction("Index", "Login");
        }

        private bool IsValid(User user)
        {
            if (user == null || string.IsNullOrEmpty(user.Email) || string.IsNullOrEmpty(user.Password))
            {
                return false;
            }
            return true;
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
    }
}