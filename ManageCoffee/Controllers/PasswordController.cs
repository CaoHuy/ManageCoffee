using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using ManageCoffee.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ManageCoffee.Controllers
{

    public class PasswordController : Controller
    {
        private readonly ManageCoffeeContext _db;
     
        private DateTime otpEndTime;

        public PasswordController(ManageCoffeeContext db)
        {
            _db = db;
        }

        public string GenerateOTP()
        {
            Random random = new Random();
            int otp = random.Next(100000, 1000000);
            HttpContext.Session.SetString("OTP_Creation_Time", DateTime.Now.ToString("o"));
            return otp.ToString();
        }

        public IActionResult ForgotPassword()
        {
            return View("ForgotPassword");
        }

        [HttpPost]
        public IActionResult ForgotPassword(string email)
        {
            try
            {
                if (!IsValidEmail(email))
                {
                    return BadRequest("Invalid email address");
                }
                var user = _db.Users.FirstOrDefault(u => u.Email == email);
                if (user == null)
                {
                    ViewData["Null"] = "Your email is not registered!!";
                    return View("ForgotPassword");
                }

                string otp = GenerateOTP();

                string fromAddress = "phamcongto1512@gmail.com";
                string fromPassword = "palmvlgqurncodis";

                string toAddress = email;
                string subject = "OTP CODE";
                string body = $"Your OTP code is: {otp}";

                SmtpClient smtp = new SmtpClient
                {
                    Host = "smtp.gmail.com",
                    Port = 587,
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(fromAddress, fromPassword),
                    Timeout = 20000
                };

                using (MailMessage message = new MailMessage(fromAddress, toAddress))
                {
                    message.Subject = subject;
                    message.Body = body;
                    smtp.Send(message);
                }

                TempData["OTP"] = otp;
                TempData["Email"] = email;
                HttpContext.Session.SetString("OtpSentTime", DateTime.Now.ToString());

                return RedirectToAction("VerifyOtp", "Password");
            }
            catch (Exception ex)
            {
                System.Console.WriteLine(ex);
                return RedirectToAction("ForgotPassword", "Password");
            }
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        public IActionResult VerifyOtp()
        {
            return View("VerifyOtp");
        }

        [HttpPost]
        public IActionResult VerifyOtp(string otpInput, string generatedOTP)
        {
            if (HttpContext.Session.GetString("OtpSentTime") == null)
            {
                TempData.Remove("OTP");
                ViewData["Session"] = "Hết phiên đăng nhập! Vui lòng thực hiện lại!";
                return View("/Views/Login/Index.cshtml");
            }
            else
            {
                var otpSentTime = DateTime.Parse(HttpContext.Session.GetString("OtpSentTime"));

                generatedOTP = TempData["OTP"] as string;
                TempData.Keep("OTP");
                otpEndTime = DateTime.Now;

                if (otpEndTime - otpSentTime > TimeSpan.FromSeconds(60))
                {
                    TempData.Remove("OTP");
                    ViewData["EndTime"] = "Mã otp của bạn đã hết hạn sử dụng!";
                    return View("ForgotPassword");
                }

                generatedOTP = TempData["OTP"] as string;
                TempData.Keep("OTP");
                if (otpInput == generatedOTP)
                {
                    TempData.Remove("OTP");
                    return RedirectToAction("UpdatePassword", "Password");
                }
                else
                {
                    ViewData["OTP"] = "Wrong otp code! Please enter again!";
                    return View("VerifyOtp");

                }
            }
        }

        public IActionResult UpdatePassword(string email)
        {
            email = TempData["Email"] as string;
            var user = _db.Users.FirstOrDefault(u => u.Email == email);
            if (user == null)
                return NotFound(); // Or handle accordingly
            ViewData["Message"] = user.Email;
            return View();
        }

        [HttpPost]
        public IActionResult UpdatePassword(string email, string Password, string confirmPassword)
        {

            var user = _db.Users.FirstOrDefault(u => u.Email == email);
            ViewData["Message"] = user.Email;
            if (Password != confirmPassword)
            {
                ViewData["Password"] = "Password and confirm password do not match!";
                return View("UpdatePassword");
            }
            else
            {
                string hashedPassword;
                using (MD5 md5 = MD5.Create())
                {
                    byte[] inputBytes = Encoding.ASCII.GetBytes(Password);
                    byte[] hashBytes = md5.ComputeHash(inputBytes);

                    StringBuilder sb = new StringBuilder();
                    for (int i = 0; i < hashBytes.Length; i++)
                    {
                        sb.Append(hashBytes[i].ToString("x2"));
                    }
                    hashedPassword = sb.ToString();
                }

                user.Password = hashedPassword;
                _db.SaveChanges();
                return RedirectToAction("Index", "Login");
            }
        }
    }
}