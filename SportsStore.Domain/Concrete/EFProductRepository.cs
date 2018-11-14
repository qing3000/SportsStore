using SportsStore.Domain.Abstract;
using SportsStore.Domain.Entities;
using System.Linq;
using System;

namespace SportsStore.Domain.Concrete
{
    public class EFProductRepository : IProductRepository
    {
        private EFDbContext context = new EFDbContext();

        public IQueryable<Product> Products
        {
            get { return context.Products; }
        }

        public bool SaveProduct(Product product)
        {
            bool ret = false;
            if (product.ID == 0)
            {
                Product dbEntry = context.Products.First(x => x.ProductID == product.ProductID && x.Brand == product.Brand);
                if (dbEntry == null)
                {
                    context.Products.Add(product);
                    ret = true;
                }
            }
            else
            {
                Product dbEntry = context.Products.Find(product.ID);
                if (dbEntry != null)
                {
                    dbEntry.TitleCN = product.TitleCN;
                    dbEntry.DescriptionCN = product.DescriptionCN;
                    dbEntry.Category = product.Category;
                    dbEntry.MaterialCN = product.MaterialCN;
                    dbEntry.MinimumAge = product.MinimumAge;
                    dbEntry.MaximumAge = product.MaximumAge;
                    dbEntry.SizePricesBinary = product.SizePricesBinary;
                    dbEntry.Gender = product.Gender;
                    dbEntry.UpdateTime = DateTime.Now;
                    ret = true;
                }
            }

            context.SaveChanges();
            return ret; 
        }

        public Product DeleteProduct(int productID)
        {
            Product dbEntry = context.Products.Find(productID);
            if (dbEntry != null)
            {
                context.Products.Remove(dbEntry);
                context.SaveChanges();
            }
            return dbEntry;
        }
    }
}