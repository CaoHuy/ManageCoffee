using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ManageCoffee.DAO
{
    public class TableDAO
    {
        private static TableDAO instance = null;
        private static readonly object instanceLock = new object();

        public static TableDAO Instance
        {
            get
            {
                lock (instanceLock)
                {
                    if (instance == null)
                    {
                        instance = new TableDAO();
                    }
                    return instance;
                }
            }
        }

        public IEnumerable<Table> GetTableList()
        {
            try
            {
                using (var context = new MCFContext())
                {
                    return context.Tables.Where(t => t.Name != "Deleted").ToList();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving table list: " + ex.Message);
            }
        }

        public Table GetTableByID(int? tableId)
        {
            try
            {
                using (var context = new MCFContext())
                {
                    return context.Tables.FirstOrDefault(m => m.Id == tableId);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving table by ID: " + ex.Message);
            }
        }

        public void AddNew(Table table)
        {
            try
            {
                var existingTable = GetTableByID(table.Id);
                if (existingTable == null)
                {
                    using (var context = new MCFContext())
                    {
                        context.Tables.Add(table);
                        context.SaveChanges();
                    }
                }
                else
                {
                    throw new Exception("The table already exists.");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error adding new table: " + ex.Message);
            }
        }

        public void Update(Table table)
        {
            try
            {
                var existingTable = GetTableByID(table.Id);
                if (existingTable != null)
                {
                    using (var context = new MCFContext())
                    {
                        context.Tables.Update(table);
                        context.SaveChanges();
                    }
                }
                else
                {
                    throw new Exception("The table does not exist.");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error updating table: " + ex.Message);
            }
        }

        public void Remove(int tableId)
        {
            System.Console.WriteLine("DAO " + tableId);
            try
            {
                var tableToRemove = GetTableByID(tableId);
                if (tableToRemove != null)
                {
                    using (var context = new MCFContext())
                    {
                        var orders = context.Orders.Where(o => o.TableId == tableId);
                        foreach (var order in orders)
                        {
                            order.TableId = null;
                        }
                        context.Tables.Remove(tableToRemove);
                        context.SaveChanges();
                    }
                }
                else
                {
                    throw new Exception("The table does not exist.");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error removing table: " + ex.Message);
            }
        }

        public void RemoveMultiple(List<int> tableIds)
        {
            try
            {
                using (var context = new MCFContext())
                {
                    foreach (var tableId in tableIds)
                    {
                        var tableToRemove = context.Tables.FirstOrDefault(t => t.Id == tableId);
                        if (tableToRemove != null)
                        {
                            context.Tables.Remove(tableToRemove);
                        }
                    }
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error removing tables: " + ex.Message);
            }
        }
        public bool IsTableExists(string tableName)
        {
            try
            {
                using (var context = new MCFContext())
                {
                    return context.Tables.Any(t => t.Name == tableName);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error checking for existing email: " + ex.Message);
            }
        }

        public bool IsAreaExists(string areaName)
        {
            try
            {
                using (var context = new MCFContext())
                {
                    return context.Tables.Any(t => t.Area == areaName);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error checking for existing area: " + ex.Message);
            }
        }

    }
}