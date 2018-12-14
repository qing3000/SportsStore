using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
using OpenQA.Selenium;

namespace SportsStore.Domain.Entities
{
    public class NextParser : Parser
    {
        public IList<string> ParseNextProductList(string url)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            this.LoadScrollableWebPage(url);
            stopwatch.Stop();
            Console.WriteLine("Time consumed in loading webpage = {0}", stopwatch.Elapsed);

            stopwatch.Restart();
            IList<string> productURLs = new List<string>();
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(this.driver.PageSource);
            HtmlNode rootNode = doc.DocumentNode;
            HtmlNode mainNode = rootNode.SelectSingleNode(@"//form[@id=""result""]");
            HtmlNodeCollection productNodes = mainNode.SelectNodes(@"//article[contains(@class, ""Item"") and contains(@class, ""Fashion"")]");
            foreach (HtmlNode node in productNodes)
            {
                string productURL = node.SelectSingleNode(@".//a[@class=""TitleText""]").Attributes["href"].Value;
                productURLs.Add(@"http:" + productURL);
            }
            stopwatch.Stop();
            Console.WriteLine("Time consumed in parsing the content {0}", stopwatch.Elapsed);

            return productURLs;
        }

        public Product ParseNextProduct(string url)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            this.LoadWebPage(url);
            stopwatch.Stop();
            Console.WriteLine("Time consumed in loading webpage = {0}", stopwatch.Elapsed);

            StreamWriter sw = new StreamWriter("c:\\temp\\b.html");
            sw.Write(this.driver.PageSource);
            sw.Close();

            // Prepare the product object.
            stopwatch.Restart();
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(this.driver.PageSource);
            Product product = new Product();
            product.URL = url;
            HtmlNode rootNode = doc.DocumentNode;
            product.Brand = @"Next";

            // Parse the product name
            HtmlNode currentNode = rootNode.SelectSingleNode(@"//div[@class=""Title""]");
            if (currentNode != null)
            {
                product.Title = currentNode.InnerText.Trim();
                product.TitleCN = Translator.Translate(product.Title);
            }

            currentNode = rootNode.SelectSingleNode(@"//div[@class=""ItemNumber""]");
            product.ProductID = currentNode.InnerText;

            // Parse the gender.
            HtmlNodeCollection currentNodes = rootNode.SelectNodes(@"//li[contains(@class,""Breadcrumb"")]");
            if (currentNodes != null)
            {
                IEnumerable<string> breadCrumbs = currentNodes.Select(x => x.InnerText.Trim());
                string fullCrumbString = string.Join(@"\", breadCrumbs);
                product.Gender = ParseNextGenderString(fullCrumbString);

                // Parse the category
                product.Category = ParseNextCategoryString(fullCrumbString);
            }

            // Parse the description
            currentNode = rootNode.SelectSingleNode(@"//div[@id='ToneOfVoice']");
            if (currentNode != null)
            {
                product.Description = currentNode.InnerText.Trim();
            }
            else
            {
                currentNodes = rootNode.SelectNodes(@"//div[@class='description']");
                if (currentNodes != null)
                {
                    product.Description = WebUtility.HtmlDecode(string.Join(@" ", currentNodes.Select(x => x.InnerText)));
                }
                else
                {
                    product.Description = "";
                }
            }

            product.DescriptionCN = Translator.Translate(product.Description);

            // Parse the material.
            currentNode = rootNode.SelectSingleNode(@"//div[@id='Composition']");
            if (currentNode != null)
            {
                product.Material = currentNode.InnerText.Trim();
                product.MaterialCN = Translator.Translate(product.Material);
            }

            // Parse the price
            currentNode = rootNode.SelectSingleNode(@"//div[contains(@class, 'SizeSelector')]");
            if (currentNode != null)
            {
                PriceInfo[] priceInfos = ParseNextPriceInfos(currentNode);
                // Parse the age info
                if (priceInfos.Length > 0)
                {
                    Tuple<Single, Single> firstAge = ParseNextAgeInfo(priceInfos.First().Size);
                    Tuple<Single, Single> lastAge = ParseNextAgeInfo(priceInfos.Last().Size);
                    product.MinimumAge = firstAge.Item1;
                    product.MaximumAge = lastAge.Item2;
                }

                product.SetPriceInfos(priceInfos);
            }

            // Parse the image links
            HtmlNode imgLinksNode = rootNode.SelectSingleNode(@"//div[@class=""ThumbNailNavClip""]");
            IList<string> imgLinks = new List<string>();
            if (imgLinksNode != null)
            {
                currentNode = imgLinksNode.ChildNodes["ul"];
                if (currentNode != null)
                {
                    foreach (HtmlNode node in currentNode.ChildNodes)
                    {
                        HtmlNode aNode = node.SelectSingleNode(@"a");
                        imgLinks.Add(aNode.Attributes["rel"].Value);
                        if (product.ThumbnailLink == null)
                        {
                            HtmlNode thumbnailNode = aNode.ChildNodes.FirstOrDefault();
                            if (thumbnailNode != null)
                            {
                                product.ThumbnailLink = thumbnailNode.Attributes["src"].Value;
                            }
                        }
                    }
                }
            }

            product.ImageLinks = string.Join(@";", imgLinks);
            product.InsertTime = DateTime.Now;
            product.UpdateTime = DateTime.Now;

            stopwatch.Stop();
            Console.WriteLine("Time consumed in parsing the content {0}", stopwatch.Elapsed);
            return product;
        }

        private static Tuple<Single, Single> ParseNextAgeInfo(string ageString)
        {
            Single minAge;
            Single maxAge;
            if (Regex.IsMatch(ageString, @"^\d+$"))
            {
                minAge = Convert.ToSingle(ageString);
                maxAge = minAge;
            }
            else if (Regex.IsMatch(ageString, @"^\d+ Yrs$"))
            {
                minAge = Convert.ToSingle(ageString.Split(' ')[0]);
                maxAge = minAge;
            }
            else if (Regex.IsMatch(ageString, @"^\d+-\d+ Mths$"))
            {
                string dString = ageString.Split(' ')[0];
                string[] dd = dString.Split('-');
                minAge = Convert.ToSingle(dd[0]) / 12;
                maxAge = Convert.ToSingle(dd[1]) / 12;
            }
            else if (Regex.IsMatch(ageString, @"^\d+-\d+ Yrs$"))
            {
                string dString = ageString.Split(' ')[0];
                string[] dd = dString.Split('-');
                minAge = Convert.ToSingle(dd[0]);
                maxAge = Convert.ToSingle(dd[1]);
            }
            else
            {
                minAge = 0;
                maxAge = 0;
            }

            return new Tuple<Single, Single>(minAge, maxAge);
        }

        private static PriceInfo[] ParseNextPriceInfos(HtmlNode node)
        {
            HtmlNodeCollection nodes = node.SelectNodes(@".//li");
            IList<PriceInfo> priceInfos = new List<PriceInfo>();
            foreach (HtmlNode oneNode in nodes.Skip(1))
            {
                // MatchCollection matches = Regex.Matches(oneNode.InnerText.Trim(), @"(\d+)");
                // Single minAge = Convert.ToSingle(matches[0].Value);
                // Single maxAge = Convert.ToSingle(matches[1].Value);
                string[] ss = oneNode.InnerText.Split(new[] { @" - " }, StringSplitOptions.None);
                PriceInfo priceInfo = new PriceInfo();
                if (ss.Length > 0)
                {
                    priceInfo.Size = ss[0];
                    if (ss.Length >= 3)
                    {
                        priceInfo.Price = Convert.ToDecimal(ss[1]);
                        priceInfo.Stock = ss[2];
                    }
                    else
                    {

                        if (ss.Length == 2)
                        {
                            decimal price = 0;
                            if (!Decimal.TryParse(ss[1], out price))
                            {
                                // Get the price from another html node.
                                HtmlNode priceNode = node.SelectSingleNode(@"//div[@class=""nowPrice""]");
                                string priceString = priceNode.InnerText.Trim();
                                priceInfo.Price = Convert.ToDecimal(new string(priceString.Where(x => Char.IsDigit(x)).ToArray()));
                                priceInfo.Stock = ss[1];
                            }
                            else
                            {
                                priceInfo.Price = price;
                                priceInfo.Stock = @"In stock";
                            }
                        }
                    }

                    priceInfo.PriceCN = GBP2RMB(priceInfo.Price);
                    priceInfos.Add(priceInfo);
                }
            }

            return priceInfos.ToArray();
        }

        private static EGender ParseNextGenderString(string genderString)
        {
            string ls = genderString.ToLower();
            return ls.Contains("girl") || ls.Contains("dress") ? EGender.FEMALE : EGender.MALE;
        }

        private static ECategory ParseNextCategoryString(string categoryString)
        {
            ECategory category = ECategory.OTHERS;
            if (categoryString.ToLower().Contains(@"dress"))
            {
                category = ECategory.DRESSES;
            }

            return category;
        }
    }
}
