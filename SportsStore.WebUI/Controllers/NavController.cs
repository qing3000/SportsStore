using SportsStore.Domain.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SportsStore.Domain.Entities;
using SportsStore.WebUI.Models;

namespace SportsStore.WebUI.Controllers {
    public class NavController : Controller
    {
        private IProductRepository repository;

        public NavController(IProductRepository repo)
        {
            repository = repo;
        }

        public PartialViewResult Menu(EGender gender = EGender.UNISEX, Decimal age = 0, 
                                      bool categoryOn = false, ECategory category = ECategory.OTHERS,
                                      bool brandOn = false, EBrand brand = EBrand.UNKNOWN, 
                                      Decimal minimumPrice = 0, Decimal maximumPrice = 0,
                                      EColorSeries colorSeries = EColorSeries.ANY, string keyword = "")
        {
            ViewBag.CategoryOn = categoryOn;
            ViewBag.SelectedCategory = category;

            IEnumerable<ECategory> categories = repository.Products.Select(x => x.Category).Distinct().OrderBy(x => x);
            IEnumerable<string> categoryStrings = CategoryClass.Categories.Select(x => x.Value.Item2);
            NavFilterViewModel navViewModel = new NavFilterViewModel()
            {
                Gender = gender,
                Age = age,
                CategoryOn = categoryOn,
                Category = category,
                BrandOn = brandOn,
                Brand = brand,
                MinimumPrice = minimumPrice,
                MaximumPrice = maximumPrice,
                ColorSeries = colorSeries,
                KeyWord = keyword,
                CategoryStrings = categoryStrings.ToArray()
            };

            return PartialView(categoryStrings.ToList());
        }
    }
}
