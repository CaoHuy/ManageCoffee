using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ManageCoffee.Models;

namespace ManageCoffee.DAO
{
    public class DetailDAO
    {
        private static DetailDAO instance = null;
        private static readonly object instanceLock = new object();

        public static DetailDAO Instance
        {
            get
            {
                lock (instanceLock)
                {
                    if (instance == null)
                    {
                        instance = new DetailDAO();
                    }
                    return instance;
                }
            }
        }

        public IEnumerable<Detail> GetDetailList()
        {
            try
            {
                using (var context = new ManageCoffeeContext())
                {
                    return context.Details.ToList();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving Detail list: " + ex.Message);
            }
        }

        public Detail GetDetailByID(int Id)
        {
            try
            {
                using (var context = new ManageCoffeeContext())
                {
                    return context.Details.FirstOrDefault(d => d.DetailId == Id);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving Detail by ID: " + ex.Message);
            }
        }


        public void AddNew(Detail Detail)
        {
            try
            {
                var existingDetail = GetDetailByID(Detail.DetailId);
                if (existingDetail == null)
                {
                    using (var context = new ManageCoffeeContext())
                    {
                        context.Details.Add(Detail);
                        context.SaveChanges();
                    }
                }
                else
                {
                    throw new Exception("The Detail already exists.");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error adding new Detail: " + ex.Message);
            }
        }

        public void Update(Detail Detail)
        {
            try
            {
                var existingDetail = GetDetailByID(Detail.DetailId);
                if (existingDetail != null)
                {
                    using (var context = new ManageCoffeeContext())
                    {
                        context.Details.Update(Detail);
                        context.SaveChanges();
                    }
                }
                else
                {
                    throw new Exception("The Detail does not exist.");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error updating Detail: " + ex.Message);
            }
        }


        public void Remove(int Id)
        {
            try
            {
                var DetailToRemove = GetDetailByID(Id);
                if (DetailToRemove != null)
                {
                    using (var context = new ManageCoffeeContext())
                    {
                        context.Details.Remove(DetailToRemove);
                        context.SaveChanges();
                    }
                }
                else
                {
                    throw new Exception("The Detail does not exist.");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error removing Detail: " + ex.Message);
            }
        }

        public void RemoveMultiple(List<int> Ids)
        {
            try
            {
                using (var context = new ManageCoffeeContext())
                {
                    foreach (var Id in Ids)
                    {
                        var DetailToRemove = context.Details.FirstOrDefault(u => u.DetailId == Id);
                        if (DetailToRemove != null)
                        {
                            context.Details.Remove(DetailToRemove);
                        }
                    }
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error removing Details: " + ex.Message);
            }
        }
    }
}