using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using OpenQA.Selenium;

namespace SportsStore.Domain.Entities
{
    public class BodenParser : Parser
    {
        public IEnumerable<string> ParseBodenProductList(string url)
        {
            IWebDriver driver = this.LoadWebPage(url);

            string rootUrl = @"http://www.boden.co.uk";
            IList<string> productURLs = new List<string>();

            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(driver.PageSource);
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
            IWebDriver driver = this.LoadWebPage(url);
            // Tuple<string, PriceInfo[]> output = ParseHtmlByPhantomJS(url, @"c:\temp\parse_boden_page.js");

            // Prepare the product object.
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(driver.PageSource);
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

            // Get the prices.
            //product.SetPriceInfos(output.Item2);

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
            return product;
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
