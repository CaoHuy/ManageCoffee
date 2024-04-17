using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ManageCoffee.Models;


namespace MCF.DAO
{
    public class ProductDAO
    {
        private static ProductDAO instance = null;
        private static readonly object instanceLock = new object();

        public static ProductDAO Instance
        {
            get
            {
                lock (instanceLock)
                {
                    if (instance == null)
                    {
                        instance = new ProductDAO();
                    }
                    return instance;
                }
            }
        }


        public IEnumerable<Product> GetProductList()
        {
            try
            {
                using (var context = new ManageCoffeeContext())
                {
                    return context.Products.ToList();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving product list: " + ex.Message);
            }
        }

        public Product GetProductByID(int product_id)
        {
            try
            {
                using (var context = new ManageCoffeeContext())
                {
                    return context.Products.FirstOrDefault(m => m.ProductId == product_id);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving product by ID: " + ex.Message);
            }
        }


        public void AddNew(Product product)
        {
            try
            {
                var existingProduct = GetProductByID(product.ProductId);
                if (existingProduct == null)
                {
                    using (var context = new ManageCoffeeContext())
                    {
                        System.Console.WriteLine(product.Price);
                        context.Products.Add(product);
                        context.SaveChanges();
                    }
                }
                else
                {
                    throw new Exception("The product already exists.");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error adding new product: " + ex.Message);
            }
        }

        public void Update(Product product)
        {
            try
            {
                var existingProduct = GetProductByID(product.ProductId);
                if (existingProduct != null)
                {
                    using (var context = new ManageCoffeeContext())
                    {
                        context.Products.Update(product);
                        context.SaveChanges();
                    }
                }
                else
                {
                    throw new Exception("The product does not exist.");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error updating product: " + ex.Message);
            }
        }


        public void Remove(int productId)
        {
            try
            {
                var productToRemove = GetProductByID(productId);
                if (productToRemove != null)
                {
                    using (var context = new ManageCoffeeContext())
                    {
                        context.Products.Remove(productToRemove);
                        context.SaveChanges();
                    }
                }
                else
                {
                    throw new Exception("The product does not exist.");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error removing product: " + ex.Message);
            }
        }

        public void RemoveMultiple(List<int> productIds)
        {
            try
            {
                using (var context = new ManageCoffeeContext())
                {
                    foreach (var productId in productIds)
                    {
                        var productToRemove = context.Users.FirstOrDefault(u => u.UserId == productId);
                        if (productToRemove != null)
                        {
                            context.Users.Remove(productToRemove);
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

        public void DeleteDetailsByProductId(int productId)
        {
            try
            {
                using (var context = new ManageCoffeeContext())
                {
                    var detailsToDelete = context.Details.Where(d => d.ProductId == productId).ToList();
                    context.Details.RemoveRange(detailsToDelete);
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error deleting details by product ID: " + ex.Message);
            }
        }
    }
}