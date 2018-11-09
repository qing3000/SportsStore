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
        [TestMethod]
        public void TestPhantomJSParser()
        {
            string url = "http://www.boden.co.uk/en-gb/girls-dresses";
            string pageContent = Parser.ReadHtmlByPhantomJS(url, @"c:\temp\load_page.js");
            Assert.IsTrue(pageContent.Length > 700000);
        }

        [TestMethod()]
        public void ParseBodenPageTest()
        {
            Console.WriteLine(@"Parsing page");
            IEnumerable<string> links = Parser.ParsePage("http://www.boden.co.uk/en-gb/girls-dresses");
            Assert.AreEqual(90, links.Count());
            IList<Product> products = new List<Product>();
            foreach (string link in links)
            {
                Console.WriteLine(@"Processing link {0}", link);
                products.Add(Parser.ParseProduct(link));
            }
        }

        [TestMethod()]
        public void ParseBodenProductTest()
        {
            string url = "http://www.boden.co.uk/en-gb/girls-dresses/knitted-dresses/g1095-red/girls-polish-red-snowmen-fair-isle-knitted-dress";
            Product product = Parser.ParseProduct(url);
            Assert.IsTrue(product.SizePrices.Length > 0);
        }
    }
}