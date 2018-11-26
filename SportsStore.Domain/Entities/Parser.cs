using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;

namespace SportsStore.Domain.Entities
{
    public class Parser
    {
        public static Decimal GBP2RMB(Decimal x)
        {
            Decimal PRICE_RMBRATE = 10;
            Decimal PRICE_PROFIT = 50;
            return PRICE_RMBRATE * x + PRICE_PROFIT;
        }

        public static Product ParseProduct(string url)
        {
            Product product = null;
            if (url.Contains(@"next"))
            {
                product = ParseNEXTProduct(url);
                product.URL = url;
            }
            else if (url.Contains(@"boden"))
            {
                product = ParseBodenProduct(url);
            }

            return product;
        }

        public static IEnumerable<string> ParsePage(string url)
        {
            IEnumerable<string> productURLs = null;
            if (url.Contains(@"next"))
            {
                productURLs = ParseNEXTPage(url);
            }
            else if (url.Contains(@"boden"))
            {
                productURLs = ParseBODENPage(url);
            }

            return productURLs;
        }

        public static IEnumerable<string> ParseNEXTPage(string url)
        {
            IList<string> productURLs = new List<string>();
            HtmlNode rootNode = ReadHtml(url).DocumentNode;
            HtmlNode mainNode = rootNode.SelectSingleNode(@"//form[@id=""result""]");
            HtmlNodeCollection productNodes = mainNode.SelectNodes(@"//article[contains(@class, ""Item"") and contains(@class, ""Fashion"")]");
            foreach (HtmlNode node in productNodes)
            {
                string productURL = node.SelectSingleNode(@".//a[@class=""TitleText""]").Attributes["href"].Value;
                productURLs.Add(@"http:" + productURL);
            }

            return productURLs;
        }

        public static IEnumerable<string> ParseBODENPage(string url)
        {
            string rootUrl = @"http://www.boden.co.uk";
            IList<string> productURLs = new List<string>();

            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(ReadHtmlByPhantomJS(url, @"c:\temp\load_page.js"));
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

        private static Product ParseNEXTProduct(string url)
        {
            string html = ReadHtmlByPhantomJS(url, @"load_next_product.js");

            // Prepare the product object.
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(html);
            Product product = new Product();
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
                product.DescriptionCN = Translator.Translate(product.Description);
            }

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
            return product;
        }

        private static Tuple<Single, Single> ParseNextAgeInfo(string ageString)
        {
            //Single minAge;
            //Single maxAge;
            //if (Regex.IsMatch(ageString, @"\d*-\d* Yrs"))

            //int index = ageString.IndexOf(@"up to");
            //if (index >= 0)
            //{
            //    minAge = 0;
            //    string rightString = ageString.Substring(index + 6);
            //    index = rightString.IndexOf('m');
            //    if (index >= 0)
            //    {
            //        numbericString = rightString.
            //    }

            //    maxAge = AgeStringToDecimal(rightString);
            //}

            return new Tuple<Single, Single>(0, 0);
        }

        private static PriceInfo[] ParseNextPriceInfos(HtmlNode node)
        {
            HtmlNodeCollection nodes = node.SelectNodes(@".//li");
            IList<PriceInfo> priceInfos = new List<PriceInfo>();
            foreach (HtmlNode oneNode in nodes.Skip(1))
            {
                string[] ss = oneNode.InnerText.Split('-');
                if (ss.Length >= 2)
                {
                    PriceInfo priceInfo = new PriceInfo();
                    priceInfo.Size = ss[0];
                    priceInfo.Price = Convert.ToDecimal(ss[1]);
                    priceInfo.PriceCN = GBP2RMB(priceInfo.Price);
                    if (ss.Length == 3)
                    {
                        priceInfo.Stock = ss[2];
                    }
                    else
                    {
                        priceInfo.Stock = @"In stock";
                    }
                    priceInfos.Add(priceInfo);
                }
            }

            return priceInfos.ToArray();
        }

        private static Product ParseBodenProduct(string url)
        {
            Tuple<string, PriceInfo[]> output = ParseHtmlByPhantomJS(url, @"c:\temp\parse_boden_page.js");

            // Write out to file for viewing.
            StreamWriter sw = new StreamWriter(@"c:\temp\tmp.html");
            sw.Write(output.Item1);
            sw.Close();

            // Prepare the product object.
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(output.Item1);
            Product product = new Product();
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
                HtmlNodeCollection categoryNodes  = breadNode.SelectNodes(@".//li");

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
                    product.Material = String.Join(@"", desStrings.Skip(1)).Trim().Replace(@"\n", @" ").Replace(@"\r",@"");
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
            product.SetPriceInfos(output.Item2);

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

        private static HtmlDocument ReadHtml(string url)
        {
            WebClient client = new WebClient();
            // Add a user agent header in case the requested URI contains a query.
            // Do the translation
            //client.Headers.Add("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");
            //Stream data = client.OpenRead(url);

            HtmlDocument doc = new HtmlDocument();
            //doc.Load(data);

            StreamReader sr = new StreamReader(@"c:\temp\1.html");
            doc.Load(sr);
            sr.Close();
            //doc.Save(@"c:\temp\1.html");
            return doc;
        }

        public static string ReadHtmlByPhantomJS(string url, string jsLoadPage)
        {
            //int urlSplit = url.LastIndexOf('/') + 1;
            //string ss1 = url.Substring(0, urlSplit);
            //string ss2 = WebUtility.UrlEncode(url.Substring(urlSplit));
            //string uri = ss1 + ss2;

            string jsFile = @"temp.js";
            string outputFile = @"temp.html";

            StreamReader sr = new StreamReader(jsLoadPage);
            string ss = sr.ReadToEnd();
            sr.Close();

            StreamWriter sw = new StreamWriter(jsFile);
            sw.Write(ss.Replace(@"URL", url).Replace(@"OUTPUT_FILE", outputFile.Replace(@"\", @"/")));
            sw.Close();

            RunProcess(@"phantomjs.exe", jsFile);

            sr = new StreamReader(outputFile);
            string pageContent = sr.ReadToEnd();
            sr.Close();

            return pageContent;
        }

        private static Tuple<string, PriceInfo[]> ParseHtmlByPhantomJS(string url, string jsInfoParser)
        {
            string filepath = @"c:\Temp\";
            string jsFile = filepath + @"temp.js";

            StreamReader sr = new StreamReader(jsInfoParser);
            string ss = sr.ReadToEnd();
            sr.Close();

            StreamWriter sw = new StreamWriter(jsFile);
            sw.Write(ss.Replace(@"URL", url));
            sw.Close();

            string output = RunProcess(filepath + @"phantomjs.exe", jsFile.Replace(@"\", @"/"));

            // Extract the page content.

            string pageContent = ExtractString(output, @"---START OF PAGE---", @"---END OF PAGE---");
            string priceInfoString = ExtractString(output, @"---START OF PRICEINFO---", @"---END OF PRICEINFO---");
            PriceInfo[] priceInfo = ParsePriceInfoString(priceInfoString);
            return new Tuple<string, PriceInfo[]>(pageContent, priceInfo); ;
        }

        private static string ExtractString(string s, string startKey, string endKey)
        {
            int i0 = s.IndexOf(startKey);
            int i1 = s.IndexOf(endKey);
            string substring = s.Substring(i0 + startKey.Length, i1 - i0 - startKey.Length).Trim();
            return substring;
        }
        private static PriceInfo[] ParsePriceInfoString(string infoString)
        {
            IList<PriceInfo> priceInfos = new List<PriceInfo>();
            string[] lines = infoString.Split('\n');
            foreach (string oneline in lines)
            {
                string[] ss = oneline.Split(',');
                string priceNumberString = new string(ss[1].Where(x => Char.IsDigit(x) || x == '.').ToArray());
                Decimal price = priceNumberString.Length > 0 ? Convert.ToDecimal(priceNumberString) : -1;
                Decimal priceCN = GBP2RMB(price); 
                priceInfos.Add(new PriceInfo { Size = ss[0].Trim(), Price= price, PriceCN = priceCN, Stock = ss[2].Trim() });
            }

            return priceInfos.ToArray();
        }

        private static EGender ParseBodenGenderString(string genderString)
        {
            return genderString.ToLower().Contains("girl") ? EGender.FEMALE : EGender.MALE;
        }

        private static EGender ParseNextGenderString(string genderString)
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

        private static ECategory ParseNextCategoryString(string categoryString)
        {
            ECategory category = ECategory.OTHERS;
            if (categoryString.ToLower().Contains(@"dress"))
            {
                category = ECategory.DRESSES;
            }

            return category;
        }

        private static string RunProcess(string command, string arguments)
        {
            System.Diagnostics.Process pProcess = new System.Diagnostics.Process();
            pProcess.StartInfo.FileName = command;
            pProcess.StartInfo.Arguments = arguments;
            pProcess.StartInfo.UseShellExecute = false;
            pProcess.StartInfo.RedirectStandardOutput = true;
            pProcess.StartInfo.StandardOutputEncoding = System.Text.Encoding.UTF8;
            pProcess.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            pProcess.StartInfo.CreateNoWindow = true; //not diplay a windows
            pProcess.Start();
            string output = pProcess.StandardOutput.ReadToEnd(); //The output result
            pProcess.WaitForExit();
            return output;
        }

        private static void SaveHtmlNode(HtmlNode node)
        {
            HtmlDocument doc = new HtmlDocument();
            doc.DocumentNode.AppendChild(node);
            doc.Save(@"c:\temp\node.html");
        }
    }
}