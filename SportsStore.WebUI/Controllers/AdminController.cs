using SportsStore.Domain.Abstract;
using SportsStore.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.SignalR;
using SportsStore.Utilities;
using System.Threading;

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
            return View("Edit", new Product());
        }

        public ViewResult Edit(int productId)
        {
            Product product = repository.Products
                .FirstOrDefault(p => p.ProductID == productId);
            return View(product);
        }

        [HttpPost]
        public ActionResult Edit(Product product)
        {
            if (ModelState.IsValid)
            {
                repository.SaveProduct(product);
                TempData["message"] = string.Format("{0} has been saved", product.Name);
                return RedirectToAction("Index");
            }
            else
            {
                // there is something wrong with the data values
                return View(product);
            }
        }

        public ActionResult ParseProduct(int productId, string productURL)
        {
            Product product = new Product();
            product.ProductID = productId;
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

            return View("Edit", product);
        }

        [HttpPost]
        public ActionResult Delete(int productId)
        {
            Product deletedProduct = repository.DeleteProduct(productId);
            if (deletedProduct != null)
            {
                TempData["message"] = string.Format("{0} was deleted", deletedProduct.Name);
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
                foreach (Product product in products)
                {
                    repository.SaveProduct(product);
                }

                TempData["message"] = string.Format("{0} new products have been saved", products.Count());
            }

            return RedirectToAction("Index");
        }

        public ViewResult ParseMultiple(string url)
        {
            IList<Product> products = new List<Product>();
            IEnumerable<string> productURLs = Parser.ParsePage(url);
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

            return View("EditProducts", products);
        }
    }
}
