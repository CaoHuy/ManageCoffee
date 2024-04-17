using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ManageCoffee.Models;


namespace ManageCoffee.DAO
{
    public class AreaDAO
    {
        private static AreaDAO instance = null;
        private static readonly object instanceLock = new object();

        public static AreaDAO Instance
        {
            get
            {
                lock (instanceLock)
                {
                    if (instance == null)
                    {
                        instance = new AreaDAO();
                    }
                    return instance;
                }
            }
        }


        public IEnumerable<Area> GetAreaList()
        {
            try
            {
                using (var context = new ManageCoffeeContext())
                {
                    return context.Areas.ToList();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving area list: " + ex.Message);
            }
        }

        public Area GetAreaByID(int area_id)
        {
            try
            {
                using (var context = new ManageCoffeeContext())
                {
                    return context.Areas.FirstOrDefault(m => m.AreaId == area_id);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving area by ID: " + ex.Message);
            }
        }


        public void AddNew(Area area)
        {
            try
            {
                var existingArea = GetAreaByID(area.AreaId);
                if (existingArea == null)
                {
                    using (var context = new ManageCoffeeContext())
                    {
                        context.Areas.Add(area);
                        context.SaveChanges();
                    }
                }
                else
                {
                    throw new Exception("The area already exists.");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error adding new area: " + ex.Message);
            }
        }

        public void Update(Area area)
        {
            try
            {
                var existingArea = GetAreaByID(area.AreaId);
                if (existingArea != null)
                {
                    using (var context = new ManageCoffeeContext())
                    {
                        context.Areas.Update(area);
                        context.SaveChanges();
                    }
                }
                else
                {
                    throw new Exception("The area does not exist.");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error updating area: " + ex.Message);
            }
        }


        public void Remove(int areaId)
        {
            try
            {
                var areaToRemove = GetAreaByID(areaId);
                if (areaToRemove != null)
                {
                    using (var context = new ManageCoffeeContext())
                    {
                        context.Areas.Remove(areaToRemove);
                        context.SaveChanges();
                    }
                }
                else
                {
                    throw new Exception("The area does not exist.");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error removing area: " + ex.Message);
            }
        }

        public void RemoveMultiple(List<int> areaIds)
        {
            try
            {
                using (var context = new ManageCoffeeContext())
                {
                    foreach (var areaId in areaIds)
                    {
                        var areaToRemove = context.Users.FirstOrDefault(u => u.UserId == areaId);
                        if (areaToRemove != null)
                        {
                            context.Users.Remove(areaToRemove);
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

        public bool IsAreaExist(string areaName)
        {
            try
            {
                using (var context = new ManageCoffeeContext())
                {
                    return context.Areas.Any(a => a.Name == areaName);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error checking for existing area: " + ex.Message);
            }
        }


    }
}