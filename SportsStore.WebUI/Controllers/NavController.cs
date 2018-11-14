using SportsStore.Domain.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SportsStore.Domain.Entities;

namespace SportsStore.WebUI.Controllers {
    public class NavController : Controller
    {
        private IProductRepository repository;

        public NavController(IProductRepository repo)
        {
            repository = repo;
        }

        public PartialViewResult Menu(bool categoryOn = false, ECategory category = ECategory.OTHERS)
        {
            ViewBag.CategoryOn = categoryOn;
            ViewBag.SelectedCategory = category;

            IEnumerable<ECategory> categories = repository.Products.Select(x => x.Category).Distinct().OrderBy(x => x);
            IEnumerable<string> categoryStrings = categories.Select(x => CategoryClass.CategoriesCN[(int)x]);

            return PartialView(categoryStrings.ToList());
        }
    }
}
