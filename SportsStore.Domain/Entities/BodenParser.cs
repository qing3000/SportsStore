using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using HtmlAgilityPack;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;

namespace SportsStore.Domain.Entities
{
    public class BodenParser : Parser
    {
        public IEnumerable<string> ParseBodenProductList(string url)
        {
            this.LoadWebPage(url);

            string rootUrl = @"http://www.boden.co.uk";
            IList<string> productURLs = new List<string>();

            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(this.driver.PageSource);
            HtmlNode rootNode = doc.DocumentNode;

            HtmlNode mainNode = rootNode.SelectSingleNode(@"//div[contains(@class,""product-items"")]");
            HtmlNodeCollection productNodes = mainNode.SelectNodes(@".//div[@class=""product-item""]");
            foreach (HtmlNode node in productNodes)
            {

                HtmlNode imageNode = node.SelectSingleNode(@".//div[@class=""product-image""]");
                productURLs.Add(rootUrl + imageNode.ChildNodes["a"].Attributes["href"].Value);
            }

            return productURLs;
        }

        public Product ParseBodenProduct(string url)
        {
            this.LoadWebPage(url);

            // Prepare the product object.
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(this.driver.PageSource);
            Product product = new Product();
            product.URL = url;
            HtmlNode rootNode = doc.DocumentNode;
            product.Brand = @"Boden";
            product.URL = url;

            // Parse the product id from the url.
            string[] ss = url.Split('/');
            product.ProductID = ss[ss.Length - 2];

            // Parse the product name
            HtmlNode currentNode = rootNode.SelectSingleNode(@"//h1[@class=""pdpProductTitle""]");
            if (currentNode != null)
            {
                string productTitle = currentNode.InnerText.Trim();
                product.Title = productTitle;
                product.TitleCN = Translator.Translate(productTitle);
            }

            // Parse the sex and category.
            HtmlNode breadNode = rootNode.SelectSingleNode(@"//div[@class=""breadcrumb""]");
            if (breadNode != null)
            {
                HtmlNodeCollection categoryNodes = breadNode.SelectNodes(@".//li");

                // Parse the gender.
                product.Gender = ParseBodenGenderString(categoryNodes[1].InnerText);

                int num = categoryNodes.Count;
                // Parse the category.
                if (num > 2)
                {
                    string categoryStr = categoryNodes[num - 2].InnerText.Trim();
                    product.Category = ParseBodenCategoryString(categoryStr);
                }
            }

            // Parse the description and material.
            currentNode = rootNode.SelectSingleNode(@"//div[@class=""tabContent pdpProductPnl a-slide""]");
            if (currentNode != null)
            {
                string productDescription = WebUtility.HtmlDecode(currentNode.InnerText.Trim());
                string[] desStrings = productDescription.Split('\n');
                product.Description = desStrings[0].Trim();
                product.DescriptionCN = Translator.Translate(product.Description);
                if (ss.Length > 1)
                {
                    product.Material = String.Join(@"", desStrings.Skip(1)).Trim().Replace(@"\n", @" ").Replace(@"\r", @"");
                    string sentence = product.Material.Replace(@"&", @"and");
                    product.MaterialCN = Translator.Translate(sentence);
                }
                else
                {
                    product.Material = @"";
                    product.MaterialCN = @"";
                }
            }

            // Parse the image links
            HtmlNode imgContainerNode = rootNode.SelectSingleNode(@"//div[@class=""imageryImagesContainer""]");
            if (imgContainerNode != null)
            {
                IList<string> imgLinks = new List<string>();
                HtmlNodeCollection imgNodes = imgContainerNode.SelectNodes(@".//img[@class=""cloudzoom""]");
                foreach (HtmlNode imgNode in imgNodes)
                {
                    imgLinks.Add(imgNode.Attributes["src"].Value);
                }

                product.ImageLinks = string.Join(@";", imgLinks);
                product.ThumbnailLink = imgLinks.First();
            }

            product.InsertTime = DateTime.Now;
            product.UpdateTime = DateTime.Now;

            PriceInfo[] priceInfos = ParseBodenPrices(this.driver, this.wait);
            product.SetPriceInfos(priceInfos);

            // Parse the age info
            if (priceInfos.Length > 0)
            {
                Tuple<Single, Single> firstAge = ParseBodenAgeInfo(priceInfos.First().Size);
                Tuple<Single, Single> lastAge = ParseBodenAgeInfo(priceInfos.Last().Size);
                product.MinimumAge = firstAge.Item1;
                product.MaximumAge = lastAge.Item2;
            }


            return product;
        }

        private static PriceInfo[] ParseBodenPrices(IWebDriver driver, IWait<IWebDriver> wait)
        {
            IList<PriceInfo> priceInfos = new List<PriceInfo>();
            ReadOnlyCollection<IWebElement> sizeList = driver.FindElement(By.Id("pdpBuyingPanel_SizeChart")).FindElements(By.CssSelector("*"));
            if (sizeList.Count > 0)
            {
                foreach (IWebElement sizeItem in sizeList)
                {
                    ReadOnlyCollection<IWebElement> anchors = sizeItem.FindElements(By.TagName("a"));
                    if (anchors.Count > 0)
                    {
                        IWebElement sizeButton = anchors[0];
                        string sizeString = sizeButton.Text;

                        try
                        {
                            // With a small window, the element might not come into view. In theory we should move to the element then click.
                            // However, Actions does not seem to work in this case.
                            //Actions actions = new Actions(driver);
                            //actions.MoveToElement(sizeButton).Click().Perform();

                            // Altenatively, we start with a large enough screen and then Click() function should work. But this is not ideal.
                            //sizeButton.Click();

                            // Executing a javascript seems to work well regardless of screen size.
                            IJavaScriptExecutor ex = (IJavaScriptExecutor)driver;
                            ex.ExecuteScript("arguments[0].click();", sizeButton);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("exception: {0}", e);
                            continue;
                        }

                        wait.Until(x => (string)((IJavaScriptExecutor)x).ExecuteScript("return document.readyState") == "complete");
                        IWebElement priceElement = driver.FindElements(By.ClassName("pdpAddToBagPrice")).First();
                        string priceString = priceElement.Text;
                        ReadOnlyCollection<IWebElement> stockElements = driver.FindElements(By.ClassName("pdpStockAvailability")).First().FindElements(By.CssSelector("*"));
                        string stockString = "Unknown";
                        foreach (IWebElement stockElement in stockElements)
                        {
                            string className = stockElement.GetAttribute("class");
                            if (!className.Contains("ng-hide") || className.Contains("ng-hide-remove"))
                            {
                                stockString = stockElement.Text;
                            }
                        }

                        string priceNumberString = new string(priceString.Where(x => Char.IsDigit(x) || x == '.').ToArray());
                        Decimal price = priceNumberString.Length > 0 ? Convert.ToDecimal(priceNumberString) : -1;
                        priceInfos.Add(new PriceInfo { Size = sizeString.Trim(), Price = price, PriceCN = GBP2RMB(price), Stock = stockString.Trim() });
                    }
                }
            }

            return priceInfos.ToArray();
        }

        private static Tuple<Single, Single> ParseBodenAgeInfo(string ageString)
        {
            Single minAge;
            Single maxAge;
            if (Regex.IsMatch(ageString, @"\d+-\d+y"))
            {
                MatchCollection matches = Regex.Matches(ageString, @"(\d+)");
                minAge = Convert.ToSingle(matches[0].Value);
                maxAge = Convert.ToSingle(matches[1].Value);
            }
            else
            {
                minAge = 0;
                maxAge = 0;
            }
            return new Tuple<Single, Single>(minAge, maxAge);
        }


        private static EGender ParseBodenGenderString(string genderString)
        {
            return genderString.ToLower().Contains("girl") ? EGender.FEMALE : EGender.MALE;
        }

        private static ECategory ParseBodenCategoryString(string categoryString)
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
