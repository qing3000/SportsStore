using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SportsStore.Domain.Abstract;
using SportsStore.Domain.Entities;
using SportsStore.WebUI.Models;

namespace SportsStore.WebUI.Controllers {
    public class ProductController : Controller
    {
        private IProductRepository repository;
        public int PageSize = 4;

        public ProductController(IProductRepository productRepository)
        {
            this.repository = productRepository;
        }

        public ViewResult List(bool categoryOn = false, ECategory category = ECategory.OTHERS, int page = 1)
        {
            ProductsListViewModel viewModel = new ProductsListViewModel {
                Products = repository.Products
                    .Where(p => categoryOn == false || p.Category == category)
                    .OrderBy(p => p.ProductID)
                    .Skip((page - 1) * PageSize)
                    .Take(PageSize),
                PagingInfo = new PagingInfo {
                    CurrentPage = page,
                    ItemsPerPage = PageSize,
                    TotalItems = categoryOn == false ?
                        repository.Products.Count() :
                        repository.Products.Where(e => e.Category == category).Count()
                },
                CategoryOn = categoryOn,
                CurrentCategory = category
            };
            return View(viewModel);
        }

        public ViewResult ProductDetails(int ID)
        {
            Product product = repository.Products.FirstOrDefault(p => p.ID == ID);
            // Prepare the gender dropdown list. 
            ViewBag.sizeSelectList = new List<SelectListItem>();
            PriceInfo[] prices = product.GetPriceInfos();
            for (int i = 0; i < prices.Length; i++)
            {
                string sizeStringCN = Translator.ManualTranslate(prices[i].Size);
                string stockStringCN = Translator.ManualTranslate(prices[i].Stock);
                string text = string.Format(@"{0,5}{1,6}{2,10}", sizeStringCN, prices[i].PriceCN, stockStringCN);
                ViewBag.sizeSelectList.Add(new SelectListItem() { Text = text, Value = i.ToString() });
            }

            return View(product);
        }

        public ActionResult CheckDatabase()
        {
            string connString = GetConnectionString();
            ViewBag.Title = DBConnectionStatus(connString) == true ? @"Connection alright" : @"Connection failed";
            ViewBag.ConnString = connString;
            return View("Error");
        }

        private static string GetConnectionString()
        {
            System.Configuration.Configuration rootWebConfig = System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration("/TechHead");
            System.Configuration.ConnectionStringSettings connString = null;
            if (rootWebConfig.ConnectionStrings.ConnectionStrings.Count > 0)
            {
                connString = rootWebConfig.ConnectionStrings.ConnectionStrings["EFDbContext"];
            }

            return connString.ConnectionString;
        }

        private static bool DBConnectionStatus(string connString)
        {
            try
            {
                using (SqlConnection sqlConn = new SqlConnection(connString))
                {
                    sqlConn.Open();
                    return (sqlConn.State == ConnectionState.Open);
                }
            }
            catch (SqlException)
            {
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
