using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using ManageCoffee.Models;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace ManageCoffee
{
    public class Helper
    {
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

        public static User UserInfo(HttpContext context)
        {
            var session = context.Session;
            string key_access = "user";
            string jsonUser = session.GetString(key_access);
            User user = null;
            if (jsonUser != null)
            {
                user = JsonConvert.DeserializeObject<User>(jsonUser);
                return user;
            }
            else
            {
                return null;
            }
        }
    }
}