using SportsStore.Domain.Abstract;
using SportsStore.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.SignalR;
using SportsStore.Helpers;
using System.Threading;
using SportsStore.WebUI.Models;

namespace SportsStore.WebUI.Controllers {

    [System.Web.Mvc.Authorize]
    public class AdminController : Controller
    {
        private IProductRepository repository;

        public AdminController(IProductRepository repo)
        {
            repository = repo;
        }

        public ViewResult Index()
        {
            return View(repository.Products);
        }

        public ViewResult Create()
        {
            return View("Edit", new ProductAdminViewModel());
        }

        public ViewResult Edit(int ID)
        {
            Product product = repository.Products
                .FirstOrDefault(p => p.ID == ID);
            ProductAdminViewModel productViewModel = new ProductAdminViewModel() { product = product, prices = product.GetPriceInfos() };
            return View(productViewModel);
        }

        [HttpPost]
        public ActionResult Edit(ProductAdminViewModel productViewModel)
        {
            if (ModelState.IsValid)
            {
                Product product = productViewModel.product;
                product.SetPriceInfos(productViewModel.prices);
                repository.SaveProduct(product);
                TempData["message"] = string.Format("{0} has been saved", product.Title);
                return RedirectToAction("Index");
            }
            else
            {
                // there is something wrong with the data values
                return View(productViewModel.product);
            }
        }

        public ActionResult ParseProduct(string productURL)
        {
            Product product = new Product();
            product.ID = 0;
            product.URL = productURL;
            if (productURL != null)
            {
                Product tempProduct = Parser.ParseProduct(productURL);
                if (tempProduct != null)
                {
                    product = tempProduct;
                }
            }
            else
            {
                TempData["message"] = @"Invalid product link";
            }

            ProductAdminViewModel productViewModel = new ProductAdminViewModel() { product = product, prices = product.GetPriceInfos() };
            return View("Edit", productViewModel);
        }



        [HttpPost]
        public ActionResult Delete(int productId)
        {
            Product deletedProduct = repository.DeleteProduct(productId);
            if (deletedProduct != null)
            {
                TempData["message"] = string.Format("{0} was deleted", deletedProduct.Title);
            }
            return RedirectToAction("Index");
        }

        public ViewResult CreateMultiple()
        {
            return View("EditProducts", new List<Product>());
        }

        public ActionResult AddMultiple(IEnumerable<Product> products)
        {
            if (ModelState.IsValid)
            {
                int counter = 0;
                foreach (Product product in products)
                {
                    if (repository.SaveProduct(product))
                    {
                        counter++;
                    }
                }

                string msg1 = string.Format("{0} new products have been saved.", counter);
                string msg2 = "";
                if (counter < products.Count())
                {
                    msg2 = string.Format(" {0} products already exist in the database.", products.Count() - counter);
                }

                TempData["message"] = msg1 + msg2;
            }

            return RedirectToAction("Index");
        }

        public ViewResult ParseMultiple(string url)
        {
            IList<Product> products = new List<Product>();
            IEnumerable<string> productURLs = Parser.ParsePage(url);
            // Utilities.WriteToBinaryFile(@"c:\temp\urls.dat", productURLs);
            // IEnumerable<string> productURLs = Utilities.ReadFromBinaryFile<IEnumerable<string>>(@"c:\temp\urls.dat");
            for (int i = 0; i < productURLs.Count(); i++)
            {
                string productURL = productURLs.ElementAt(i);
                Product product = Parser.ParseProduct(productURL);
                if (product != null)
                {
                    product.URL = productURL;
                    products.Add(product);
                }

                ProgressBarFunctions.SendProgress("Process in progress...", i, productURLs.Count());
            }

            // Utilities.WriteToBinaryFile(@"c:\temp\products.dat", products);
            // products = Utilities.ReadFromBinaryFile<IList<Product>>(@"c:\temp\products.dat");

            // Prepare the gender dropdown list. 
            ViewBag.genderSelectList = new List<SelectListItem>();
            for (int i = 0; i < GenderClass.GendersCN.Length; i++)
            {
                ViewBag.genderSelectList.Add(new SelectListItem() { Text = GenderClass.GendersCN[i], Value = i.ToString() });
            }

            // Prepare the category dropdown list.
            ViewBag.categorySelectList = new List<SelectListItem>();
            for (int i = 0; i < CategoryClass.CategoriesCN.Length; i++)
            {
                ViewBag.categorySelectList.Add(new SelectListItem() { Text = CategoryClass.CategoriesCN[i], Value = i.ToString() });
            }

            return View("EditProducts", products);
        }
    }
}
