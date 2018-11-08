using Microsoft.VisualStudio.TestTools.UnitTesting;
using SportsStore.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportsStore.Domain.Entities.Tests
{
    [TestClass()]
    public class ParserTests
    {
        [TestMethod()]
        public void ParseBodenPageTest()
        {
            IEnumerable<string> links = Parser.ParsePage("http://www.boden.co.uk/en-gb/girls-dresses");
            Assert.AreEqual(90, links.Count());
        }

        [TestMethod()]
        public void ParseBodenProductTest()
        {
            string url = "http://www.boden.co.uk/en-gb/girls-dresses/jersey-dresses/g0884-red/girls-polish-red-festive-bunnies-festive-big-appliqué-dress";
            Product product = Parser.ParseProduct(url);
            Assert.AreEqual(9, product.SizePrices.Length);
        }
    }
}