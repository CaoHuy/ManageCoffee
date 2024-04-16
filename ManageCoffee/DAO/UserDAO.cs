using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ManageCoffee.Models;

namespace ManageCoffee.DAO
{
    public class UserDAO
    {
           private static UserDAO instance = null;
        private static readonly object instanceLock = new object();

        public static UserDAO Instance
        {
            get
            {
                lock (instanceLock)
                {
                    if (instance == null)
                    {
                        instance = new UserDAO();
                    }
                    return instance;
                }
            }
        }

        public IEnumerable<User> GetUserList(int role = 0)
        {
            try
            {
                using (var context = new ManageCoffeeContext())
                {
                    return context.Users.Where(u => u.Role != role).ToList();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving user list: " + ex.Message);
            }
        }

        public User GetUserByID(int userId)
        {
            try
            {
                using (var context = new ManageCoffeeContext())
                {
                    return context.Users.FirstOrDefault(m => m.UserId == userId);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving user by ID: " + ex.Message);
            }
        }


        public void AddNew(User user)
        {
            try
            {
                var existingUser = GetUserByID(user.UserId);
                if (existingUser == null)
                {
                    using (var context = new ManageCoffeeContext())
                    {
                        context.Users.Add(user);
                        context.SaveChanges();
                    }
                }
                else
                {
                    throw new Exception("The user already exists.");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error adding new user: " + ex.Message);
            }
        }

        public void Update(User user)
        {
            try
            {
                var existingUser = GetUserByID(user.UserId);
                if (existingUser != null)
                {
                    using (var context = new ManageCoffeeContext())
                    {
                        context.Users.Update(user);
                        context.SaveChanges();
                    }
                }
                else
                {
                    throw new Exception("The user does not exist.");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error updating user: " + ex.Message);
            }
        }


        public void Remove(int userId)
        {
            try
            {
                var userToRemove = GetUserByID(userId);
                if (userToRemove != null)
                {
                    using (var context = new ManageCoffeeContext())
                    {
                        userToRemove.Role = 0;
                        context.Users.Update(userToRemove);
                        context.SaveChanges();
                    }
                }
                else
                {
                    throw new Exception("The user does not exist.");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error removing user: " + ex.Message);
            }
        }

        public void RemoveMultiple(List<int> userIds)
        {
            try
            {
                using (var context = new ManageCoffeeContext())
                {
                    foreach (var userId in userIds)
                    {
                        var userToRemove = context.Users.FirstOrDefault(u => u.UserId == userId);
                        if (userToRemove != null)
                        {
                            context.Users.Remove(userToRemove);
                        }
                    }
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error removing users: " + ex.Message);
            }
        }

        public bool IsUserEmailExists(string email)
        {
            try
            {
                using (var context = new ManageCoffeeContext())
                {
                    return context.Users.Any(u => u.Email == email);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error checking for existing email: " + ex.Message);
            }
        }

        public bool IsUserPhoneExists(string phone)
        {
            try
            {
                using (var context = new ManageCoffeeContext())
                {
                    return context.Users.Any(u => u.Phone == phone);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error checking for existing phone: " + ex.Message);
            }
        }
    }
}