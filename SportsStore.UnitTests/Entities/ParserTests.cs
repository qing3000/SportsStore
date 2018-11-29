﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
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
        //[TestMethod]
        //public void TestPhantomJSParser()
        //{
        //    string url = "http://www.boden.co.uk/en-gb/girls-dresses";
        //    string pageContent = Parser.ReadHtmlByPhantomJS(url, @"c:\temp\load_page.js");
        //    Assert.IsTrue(pageContent.Length > 700000);
        //}

        [TestMethod()]
        public void ParseBodenPageTest()
        {
            Console.WriteLine(@"Parsing page");
            IEnumerable<string> links = BodenParser.ParseBodenProductList("http://www.boden.co.uk/en-gb/girls-dresses");
            Assert.AreEqual(90, links.Count());
            IList<Product> products = new List<Product>();
            foreach (string link in links)
            {
                Console.WriteLine(@"Processing link {0}", link);
                products.Add(BodenParser.ParseBodenProduct(link));
            }
        }

        [TestMethod()]
        public void ParseBodenProductTest()
        {
            string url = "http://www.boden.co.uk/en-gb/girls-dresses/knitted-dresses/g1095-red/girls-polish-red-snowmen-fair-isle-knitted-dress";
            Product product = BodenParser.ParseBodenProduct(url);
            Assert.IsTrue(product.GetPriceInfos().Length > 0);
        }

        [TestMethod()]
        public void ParseNextListTest()
        {
            IEnumerable<string> links = NextParser.ParseNextProductList("https://www.next.co.uk/shop/gender-newborngirls-gender-oldergirls-gender-youngergirls-category-dresses");
            Assert.IsTrue(links.Count() > 650);
        }

        [TestMethod()]
        public void ParseNextProductTest()
        {
            string url = "http://www.next.co.uk/g92236s7#322425";
            Product product = NextParser.ParseNextProduct(url);
            Assert.AreNotEqual(null, product.ThumbnailLink);
        }
    }
}