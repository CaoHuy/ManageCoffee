using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ManageCoffee.Models;

namespace ManageCoffee.DAO
{
    public class CatalogueDAO
    {
        private static CatalogueDAO instance = null;
        private static readonly object instanceLock = new object();

        public static CatalogueDAO Instance
        {
            get
            {
                lock (instanceLock)
                {
                    if (instance == null)
                    {
                        instance = new CatalogueDAO();
                    }
                    return instance;
                }
            }
        }


        public IEnumerable<Catalogue> GetCataloguesList()
        {
            try
            {
                using (var context = new ManageCoffeeContext())
                {
                    return context.Catalogues.ToList();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving catalogue list: " + ex.Message);
            }
        }

        public Catalogue GetCatalogueById(int catalogue_id)
        {
            try
            {
                using (var context = new ManageCoffeeContext())
                {
                    return context.Catalogues.FirstOrDefault(m => m.CatalogueId == catalogue_id);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving catalogue by ID: " + ex.Message);
            }
        }


        public void AddNew(Catalogue catalogue)
        {
            try
            {
                var existingCatalogue = GetCatalogueById(catalogue.CatalogueId);
                if (existingCatalogue == null)
                {
                    using (var context = new ManageCoffeeContext())
                    {
                        context.Catalogues.Add(catalogue);
                        context.SaveChanges();
                    }
                }
                else
                {
                    throw new Exception("The catalogue already exists.");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error adding new catalogue: " + ex.Message);
            }
        }

        public void Update(Catalogue catalogue)
        {
            try
            {
                var existingCatalogue = GetCatalogueById(catalogue.CatalogueId);
                if (existingCatalogue != null)
                {
                    using (var context = new ManageCoffeeContext())
                    {
                        context.Catalogues.Update(catalogue);
                        context.SaveChanges();
                    }
                }
                else
                {
                    throw new Exception("The catalogue does not exist.");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error updating catalogue: " + ex.Message);
            }
        }


        public void Remove(int catalogueId)
        {
            try
            {
                var catalogueremove = GetCatalogueById(catalogueId);
                if (catalogueremove != null)
                {
                    using (var context = new ManageCoffeeContext())
                    {
                        context.Catalogues.Remove(catalogueremove);
                        context.SaveChanges();
                    }
                }
                else
                {
                    throw new Exception("The catalogue does not exist.");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error removing catalogue: " + ex.Message);
            }
        }

        public void RemoveMultiple(List<int> catalogueIds)
        {
            try
            {
                using (var context = new ManageCoffeeContext())
                {
                    foreach (var catalogueId in catalogueIds)
                    {
                        var catalogueremove = context.Users.FirstOrDefault(u => u.UserId == catalogueId);
                        if (catalogueremove != null)
                        {
                            context.Users.Remove(catalogueremove);
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

        public bool IsCatalogueExist(string cataloguename)
        {
            try
            {
                using (var context = new ManageCoffeeContext())
                {
                    return context.Catalogues.Any(a => a.Name == cataloguename);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error checking for existing catalogue: " + ex.Message);
            }
        }

    }
}