using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SportsStore.Domain.Abstract;
using SportsStore.Domain.Entities;
using SportsStore.WebUI.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using SportsStore.WebUI.Models;

namespace SportsStore.UnitTests {
    [TestClass]
    public class AdminTests {

        [TestMethod]
        public void Index_Contains_All_Products() {
            // Arrange - create the mock repository
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[] {
                new Product {ID = 1, Title = "P1"},
                new Product {ID = 2, Title = "P2"},
                new Product {ID = 3, Title = "P3"},
            }.AsQueryable());

            // Arrange - create a controller 
            AdminController target = new AdminController(mock.Object);


            // Action
            Product[] result = ((IEnumerable<Product>)target.Index().ViewData.Model).ToArray();

            // Assert
            Assert.AreEqual(result.Length, 3);
            Assert.AreEqual("P1", result[0].Title);
            Assert.AreEqual("P2", result[1].Title);
            Assert.AreEqual("P3", result[2].Title);
        }


        [TestMethod]
        public void Can_Edit_Product() {

            // Arrange - create the mock repository
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[] {
                new Product {ID = 1, Title = "P1"},
                new Product {ID = 2, Title = "P2"},
                new Product {ID = 3, Title = "P3"},
            }.AsQueryable());

            // Arrange - create the controller
            AdminController target = new AdminController(mock.Object);

            // Act
            Product p1 = target.Edit(1).ViewData.Model as Product;
            Product p2 = target.Edit(2).ViewData.Model as Product;
            Product p3 = target.Edit(3).ViewData.Model as Product;

            // Assert
            Assert.AreEqual(1, p1.ID);
            Assert.AreEqual(2, p2.ID);
            Assert.AreEqual(3, p3.ID);
        }

        [TestMethod]
        public void Cannot_Edit_Nonexistent_Product() {

            // Arrange - create the mock repository
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[] {
                new Product {ID = 1, Title = "P1"},
                new Product {ID = 2, Title = "P2"},
                new Product {ID = 3, Title = "P3"},
            }.AsQueryable());

            // Arrange - create the controller
            AdminController target = new AdminController(mock.Object);

            // Act
            Product result = (Product)target.Edit(4).ViewData.Model;

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void Can_Save_Valid_Changes()
        {
            // Arrange - create mock repository
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            // Arrange - create the controller
            AdminController target = new AdminController(mock.Object);
            // Arrange - create a product
            Product product = new Product { ID = 0, Title = "Test" };
            PriceInfo[] sizePrices = new PriceInfo[3]
                { new PriceInfo { Size = "S", Price = 10, Stock = "In Stock" },
                  new PriceInfo { Size = "M", Price = 20, Stock = "In Stock" },
                  new PriceInfo { Size = "L", Price = 30, Stock = "In Stock" }};

            // Act - try to save the product
            ProductAdminViewModel productViewModel = new ProductAdminViewModel() { product = product, prices = sizePrices };
            ActionResult result = target.Edit(productViewModel);

            // Assert - check that the repository was called
            mock.Verify(m => m.SaveProduct(product));
            // Assert - check the method result type
            Assert.IsNotInstanceOfType(result, typeof(ViewResult));
        }

        [TestMethod]
        public void Cannot_Save_Invalid_Changes() {

            // Arrange - create mock repository
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            // Arrange - create the controller
            AdminController target = new AdminController(mock.Object);
            // Arrange - create a product
            Product product = new Product { Title = "Test" };
            // Arrange - add an error to the model state
            target.ModelState.AddModelError("error", "error");

            PriceInfo[] sizePrices = new PriceInfo[3]
                { new PriceInfo { Size = "S", Price = 10, Stock = "In Stock" },
                  new PriceInfo { Size = "M", Price = 20, Stock = "In Stock" },
                  new PriceInfo { Size = "L", Price = 30, Stock = "In Stock" }};
            // Act - try to save the product
            ProductAdminViewModel productViewModel = new ProductAdminViewModel() { product = product, prices = sizePrices };
            ActionResult result = target.Edit(productViewModel);

            // Assert - check that the repository was not called
            mock.Verify(m => m.SaveProduct(It.IsAny<Product>()), Times.Never());
            // Assert - check the method result type
            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }

        [TestMethod]
        public void Can_Delete_Valid_Products() {

            // Arrange - create a Product
            Product prod = new Product { ID = 2, Title = "Test" };

            // Arrange - create the mock repository
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[] {
                new Product {ID = 1, Title = "P1"},
                prod,
                new Product {ID = 3, Title = "P3"},
            }.AsQueryable());

            // Arrange - create the controller
            AdminController target = new AdminController(mock.Object);

            // Act - delete the product
            target.Delete(prod.ID);

            // Assert - ensure that the repository delete method was 
            // called with the correct Product 
            mock.Verify(m => m.DeleteProduct(prod.ID));
        }
    }
}