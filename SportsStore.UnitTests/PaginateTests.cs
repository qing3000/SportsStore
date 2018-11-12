using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SportsStore.Domain.Abstract;
using SportsStore.Domain.Entities;
using SportsStore.WebUI.Controllers;
using SportsStore.WebUI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using SportsStore.WebUI.HtmlHelpers;

namespace SportsStore.UnitTests
{
    [TestClass]
    public class PaginateTests
    {
        [TestMethod]
        public void Can_Paginate()
        {
            // Arrange
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[] {
                new Product {ID = 1, Title = "P1"},
                new Product {ID = 2, Title = "P2"},
                new Product {ID = 3, Title = "P3"},
                new Product {ID = 4, Title = "P4"},
                new Product {ID = 5, Title = "P5"}
            }.AsQueryable());

            // create a controller and make the page size 3 items
            ProductController controller = new ProductController(mock.Object);
            controller.PageSize = 3;

            // Act
            ProductsListViewModel result = (ProductsListViewModel)controller.List(true, ECategory.OTHERS, 2).Model;

            // Assert
            Product[] prodArray = result.Products.ToArray();
            Assert.IsTrue(prodArray.Length == 2);
            Assert.AreEqual(prodArray[0].Title, "P4");
            Assert.AreEqual(prodArray[1].Title, "P5");
        }

        [TestMethod]
        public void Can_Generate_Page_Links() {

            // Arrange - define an HTML helper - we need to do this
            // in order to apply the extension method
            HtmlHelper myHelper = null;

            // Arrange - create PagingInfo data
            PagingInfo pagingInfo = new PagingInfo {
                CurrentPage = 2,
                TotalItems = 28,
                ItemsPerPage = 10
            };

            // Arrange - set up the delegate using a lambda expression
            Func<int, string> pageUrlDelegate = i => "Page" + i;

            // Act
            MvcHtmlString result = myHelper.PageLinks(pagingInfo, pageUrlDelegate);

            // Assert
            Assert.AreEqual(result.ToString(), @"<a href=""Page1"">1</a>" 
                + @"<a class=""selected"" href=""Page2"">2</a>" 
                + @"<a href=""Page3"">3</a>");
        }

        [TestMethod]
        public void Can_Send_Pagination_View_Model() {

            // Arrange
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[] {
                new Product {ID = 1, Title = "P1"},
                new Product {ID = 2, Title = "P2"},
                new Product {ID = 3, Title = "P3"},
                new Product {ID = 4, Title = "P4"},
                new Product {ID = 5, Title = "P5"}
            }.AsQueryable());

            // Arrange - create a controller and make the page size 3 items
            ProductController controller = new ProductController(mock.Object);
            controller.PageSize = 3;

            // Act
            ProductsListViewModel result = (ProductsListViewModel)controller.List(true, ECategory.OTHERS, 2).Model;

            // Assert
            PagingInfo pageInfo = result.PagingInfo;
            Assert.AreEqual(pageInfo.CurrentPage, 2);
            Assert.AreEqual(pageInfo.ItemsPerPage, 3);
            Assert.AreEqual(pageInfo.TotalItems, 5);
            Assert.AreEqual(pageInfo.TotalPages, 2);
        }

        [TestMethod]
        public void Can_Filter_Products() {

            // Arrange
            // - create the mock repository
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[] {
                new Product {ID = 1, Title = "P1", Category = ECategory.BIBS},
                new Product {ID = 2, Title = "P2", Category = ECategory.BIBS},
                new Product {ID = 3, Title = "P3", Category = ECategory.BIBS},
                new Product {ID = 4, Title = "P4", Category = ECategory.BIBS},
                new Product {ID = 5, Title = "P5", Category = ECategory.BIBS}
            }.AsQueryable());

            // Arrange - create a controller and make the page size 3 items
            ProductController controller = new ProductController(mock.Object);
            controller.PageSize = 3;

            // Action
            Product[] result = ((ProductsListViewModel)controller.List(false, ECategory.DRESSES, 1).Model)
                .Products.ToArray();

            // Assert
            Assert.AreEqual(result.Length, 2);
            Assert.IsTrue(result[0].Title == "P2" && result[0].Category == ECategory.DRESSES);
            Assert.IsTrue(result[1].Title == "P4" && result[1].Category == ECategory.DRESSES);
        }

        [TestMethod]
        public void Can_Create_Categories() {

            // Arrange
            // - create the mock repository
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[] {
                new Product {ID = 1, Title = "P1", Category = ECategory.BIBS},
                new Product {ID = 2, Title = "P2", Category = ECategory.BIBS},
                new Product {ID = 3, Title = "P3", Category = ECategory.DRESSES},
                new Product {ID = 4, Title = "P4", Category = ECategory.SHOES},
            }.AsQueryable());

            // Arrange - create the controller
            NavController target = new NavController(mock.Object);

            // Act = get the set of categories 
            ECategory[] results = ((IEnumerable<ECategory>)target.Menu(ECategory.BIBS).Model).ToArray();

            // Assert
            Assert.AreEqual(results.Length, 3);
            Assert.AreEqual(results[0], ECategory.BIBS);
            Assert.AreEqual(results[1], ECategory.DRESSES);
            Assert.AreEqual(results[2], ECategory.SHOES);
        }

        [TestMethod]
        public void Indicates_Selected_Category() {

            // Arrange
            // - create the mock repository
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[] {
                new Product {ID = 1, Title = "P1", Category = ECategory.BIBS},
                new Product {ID = 4, Title = "P2", Category = ECategory.DRESSES},
            }.AsQueryable());

            // Arrange - create the controller 
            NavController target = new NavController(mock.Object);

            // Arrange - define the category to selected
            ECategory categoryToSelect = ECategory.BIBS;

            // Action
            string result = target.Menu(categoryToSelect).ViewBag.SelectedCategory;

            // Assert
            Assert.AreEqual(categoryToSelect, result);
        }

        [TestMethod]
        public void Generate_Category_Specific_Product_Count() {
            // Arrange
            // - create the mock repository
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[] {
                new Product {ID = 1, Title = "P1", Category = ECategory.BIBS},
                new Product {ID = 2, Title = "P2", Category = ECategory.DRESSES},
                new Product {ID = 3, Title = "P3", Category = ECategory.BIBS},
                new Product {ID = 4, Title = "P4", Category = ECategory.DRESSES},
                new Product {ID = 5, Title = "P5", Category = ECategory.OTHERS}
            }.AsQueryable());

            // Arrange - create a controller and make the page size 3 items
            ProductController target = new ProductController(mock.Object);
            target.PageSize = 3;

            // Action - test the product counts for different categories
            int res1 
                = ((ProductsListViewModel)target.List(false, ECategory.BIBS).Model).PagingInfo.TotalItems;
            int res2 
                = ((ProductsListViewModel)target.List(false, ECategory.DRESSES).Model).PagingInfo.TotalItems;
            int res3
                = ((ProductsListViewModel)target.List(false, ECategory.OTHERS).Model).PagingInfo.TotalItems;
            int resAll 
                = ((ProductsListViewModel)target.List(false, ECategory.SHOES).Model).PagingInfo.TotalItems;

            // Assert
            Assert.AreEqual(res1, 2);
            Assert.AreEqual(res2, 2);
            Assert.AreEqual(res3, 1);
            Assert.AreEqual(resAll, 5);
        }
    }
}
