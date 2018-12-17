using Microsoft.VisualStudio.TestTools.UnitTesting;
using SportsStore.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Web.Mvc;

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
            BodenParser bodenParser = new BodenParser();
            Console.WriteLine(@"Parsing page");
            IEnumerable<string> links = bodenParser.ParseBodenProductList("http://www.boden.co.uk/en-gb/girls-dresses");
            Assert.AreEqual(90, links.Count());
            IList<Product> products = new List<Product>();
            foreach (string link in links)
            {
                Console.WriteLine(@"Processing link {0}", link);
                products.Add(bodenParser.ParseBodenProduct(link));
            }
        }

        [TestMethod()]
        public void ParseBodenProductTest()
        {
            BodenParser bodenParser = new BodenParser();
            string url = "http://www.boden.co.uk/en-gb/girls-dresses/knitted-dresses/g1095-red/girls-polish-red-snowmen-fair-isle-knitted-dress";
            Product product = bodenParser.ParseBodenProduct(url);
            Assert.IsTrue(product.GetPriceInfos().Length > 0);
        }

        [TestMethod()]
        public void ParseNextListTest()
        {
            NextParser nextParser = new NextParser();
            IEnumerable<string> links = nextParser.ParseNextProductList("https://www.next.co.uk/shop/gender-newborngirls-gender-oldergirls-gender-youngergirls-category-dresses");
            Assert.IsTrue(links.Count() > 650);

            IList<Product> products = new List<Product>();
            foreach (string link in links)
            {
                Console.WriteLine(@"Processing link {0}", link);
                products.Add(nextParser.ParseNextProduct(link));
            }
        }

        [TestMethod()]
        public void ParseNextProductTest()
        {
            NextParser nextParser = new NextParser();
            string url = "http://www.next.co.uk/g92236s7#322425";
            Product product = nextParser.ParseNextProduct(url);
            Assert.AreNotEqual(null, product.ThumbnailLink);
        }

        [TestMethod()]
        public void RegularExpressionTest1()
        {
            string ageString = @"1-3 Yrs";
            MatchCollection matches = Regex.Matches(ageString, @"(\d+)");
            Single minAge = Convert.ToSingle(matches[0].Value);
            Single maxAge = Convert.ToSingle(matches[1].Value);
        }

        [TestMethod()]
        public void RegularExpressionTest2()
        {
            // Lets use a regular expression to capture data from a few date strings.
            string pattern = @"([a-zA-Z]+) (\d+)";
            MatchCollection matches = Regex.Matches("June 24, August 9, Dec 12", pattern);

            // This will print the number of matches
            Console.WriteLine("{0} matches", matches.Count);

            // This will print each of the matches and the index in the input string
            // where the match was found:
            //   June 24 at index [0, 7)
            //   August 9 at index [9, 17)
            //   Dec 12 at index [19, 25)
            foreach (Match match in matches)
            {
                Console.WriteLine("Match: {0} at index [{1}, {2})",
                    match.Value,
                    match.Index,
                    match.Index + match.Length);
            }

            // For each match, we can extract the captured information by reading the 
            // captured groups.
            foreach (Match match in matches)
            {
                GroupCollection data = match.Groups;
                // This will print the number of captured groups in this match
                Console.WriteLine("{0} groups captured in {1}", data.Count, match.Value);

                // This will print the month and day of each match.  Remember that the
                // first group is always the whole matched text, so the month starts at
                // index 1 instead.
                Console.WriteLine("Month: " + data[1] + ", Day: " + data[2]);

                // Each Group in the collection also has an Index and Length member,
                // which stores where in the input string that the group was found.
                Console.WriteLine("Month found at[{0}, {1})",
                    data[1].Index,
                    data[1].Index + data[1].Length);
            }
        }

        [TestMethod()]
        public void TestTemp()
        {
            IList<string> ss = new List<string>();
            foreach (EGender oneKey in GenderClass.Genders.Keys)
            {
                ss.Add(GenderClass.Genders[oneKey].Item1);
            }

            IEnumerable<SelectListItem> genderSelectList = GenderClass.Genders.Select(x => new SelectListItem() { Text = x.Value.Item2, Value = x.Key.ToString() });
        }
    }
}