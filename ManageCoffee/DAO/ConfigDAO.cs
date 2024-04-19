using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ManageCoffee.Models;

namespace ManageCoffee.DAO
{
    public class ConfigDAO
    {
        private static ConfigDAO instance = null;
        private static readonly object instanceLock = new object();

        public static ConfigDAO Instance
        {
            get
            {
                lock (instanceLock)
                {
                    if (instance == null)
                    {
                        instance = new ConfigDAO();
                    }
                    return instance;
                }
            }
        }

        public IEnumerable<Config> GetConfigList()
        {
            try
            {
                using (var context = new ManageCoffeeContext())
                {
                    return context.Configs.ToList();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving config list: " + ex.Message);
            }
        }

        public Config GetConfigByID(int config_id)
        {
            try
            {
                using (var context = new ManageCoffeeContext())
                {
                    return context.Configs.FirstOrDefault(m => m.ConfigId == config_id);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving config by ID: " + ex.Message);
            }
        }


        public void AddNew(Config config)
        {
            try
            {
                var existingConfig = GetConfigByID(config.ConfigId);
                if (existingConfig == null)
                {
                    using (var context = new ManageCoffeeContext())
                    {
                        context.Configs.Add(config);
                        context.SaveChanges();
                    }
                }
                else
                {
                    throw new Exception("The config already exists.");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error adding new config: " + ex.Message);
            }
        }

        public void Update(Config config)
        {
            try
            {
                var existingConfig = GetConfigByID(config.ConfigId);
                if (existingConfig != null)
                {
                    using (var context = new ManageCoffeeContext())
                    {
                        context.Configs.Update(config);
                        context.SaveChanges();
                    }
                }
                else
                {
                    throw new Exception("The config does not exist.");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error updating config: " + ex.Message);
            }
        }
public void Remove(int configId)
        {
            try
            {
                var configToRemove = GetConfigByID(configId);
                if (configToRemove != null)
                {
                    using (var context = new ManageCoffeeContext())
                    {
                        context.Configs.Remove(configToRemove);
                        context.SaveChanges();
                    }
                }
                else
                {
                    throw new Exception("The config does not exist.");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error removing config: " + ex.Message);
            }
        }

        public void RemoveMultiple(List<int> configIds)
        {
            try
            {
                using (var context = new ManageCoffeeContext())
                {
                    foreach (var configId in configIds)
                    {
                        var configToRemove = context.Users.FirstOrDefault(u => u.UserId == configId);
                        if (configToRemove != null)
                        {
                            context.Users.Remove(configToRemove);
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

    }
}