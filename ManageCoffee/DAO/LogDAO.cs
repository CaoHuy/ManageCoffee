using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ManageCoffee.Models;

namespace ManageCoffee.DAO
{
    public class LogDAO
    {
        private static LogDAO instance = null;
        private static readonly object instanceLock = new object();

        public static LogDAO Instance
        {
            get
            {
                lock (instanceLock)
                {
                    if (instance == null)
                    {
                        instance = new LogDAO();
                    }
                    return instance;
                }
            }
        }

        public IEnumerable<Log> GetLogList()
        {
            try
            {
                using (var context = new ManageCoffeeContext())
                {
                    return context.Logs.ToList();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving Log list: " + ex.Message);
            }
        }

        public Log GetLogByID(int Id)
        {
            try
            {
                using (var context = new ManageCoffeeContext())
                {
                    return context.Logs.FirstOrDefault(m => m.LogId == Id);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving Log by ID: " + ex.Message);
            }
        }


        public void AddNew(Log Log)
        {
            try
            {
                var existingLog = GetLogByID(Log.LogId);
                if (existingLog == null)
                {
                    using (var context = new ManageCoffeeContext())
                    {
                        context.Logs.Add(Log);
                        context.SaveChanges();
                    }
                }
                else
                {
                    throw new Exception("The Log already exists.");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error adding new Log: " + ex.Message);
            }
        }
    }
}