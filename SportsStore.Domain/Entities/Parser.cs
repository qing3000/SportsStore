using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;

namespace SportsStore.Domain.Entities
{
    public class Parser
    {
        public static Product ParseProduct(string url)
        {
            Product product = null;
            if (url.Contains(@"next"))
            {
                product = ParseNextProduct(url);
                product.URL = url;
            }

            return product;
        }

        public static IEnumerable<string> ParsePage(string url)
        {
            IList<string> productURLs = new List<string>();
            HtmlNode rootNode = ReadHtml(url).DocumentNode;
            HtmlNode mainNode = rootNode.SelectSingleNode(@"//div[contains(@class,""defaultView"")]");
            HtmlNodeCollection productNodes = mainNode.SelectNodes(@"//div[@class=""Price""]");
            foreach (HtmlNode node in productNodes)
            {
                string productURL = node.ChildNodes["a"].Attributes["href"].Value;
                productURLs.Add(productURL);
            }

            return productURLs;
        }

        private static Product ParseNextProduct(string url)
        {
            Product product = new Product();
            HtmlNode rootNode = ReadHtmlByPhantomJS(url).DocumentNode;
            product.Brand = @"next";

            // Parse the product name
            HtmlNode currentNode = rootNode.SelectSingleNode(@"//div[@class=""Title""]");
            if (currentNode != null)
            {
                string productName = currentNode.InnerText.Trim();
                product.Name = Translator.Translate(productName);
            }


            // Parse the sex
            HtmlNodeCollection currentNodes = rootNode.SelectNodes(@"//li[contains(@class,""Breadcrumb"")]");
            if (currentNodes != null)
            {
                IEnumerable<string> breadCrumbs = currentNodes.Select(x => x.InnerText.Trim());
                string sexString = string.Join(@"\", breadCrumbs);
                product.Sex = sexString.ToLower().Contains(@"girl") ? 0 : 1;

                // Parse the category
                product.Category = Translator.Translate(breadCrumbs.ElementAt(1));
            }

            // Parse the description
            currentNode = rootNode.SelectSingleNode(@"//div[contains(@class,""StyleContent"")]");
            if (currentNode != null)
            {
                string productDescription = currentNode.InnerText.Trim();
                product.Description = Translator.Translate(productDescription);
            }

            // Parse the price
            currentNode = rootNode.SelectSingleNode(@"//div[@class=""Price""]");
            if (currentNode != null)
            {
                string priceString = currentNode.FirstChild.InnerText;
                string singlePriceString = priceString.Contains(@"-") ? priceString.Split('-').LastOrDefault() : priceString;
                string priceNumberString = new string(singlePriceString.Where(x => Char.IsDigit(x)).ToArray());
                product.Price = Convert.ToDecimal(priceNumberString);
            }

            // Parse the image links
            HtmlNode imgLinksNode = rootNode.SelectSingleNode(@"//div[@class=""ThumbNailNavClip""]");
            if (imgLinksNode != null)
            {
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

                        }
                    }
                }

                product.ImageLinks = string.Join(@";", imgLinks);
                product.Thumbnail = imgLinksNode.ChildNodes["ul"].FirstChild.SelectSingleNode(@"a").Attributes[@"href"].Value;
            }

            return product;
        }

        private static HtmlDocument ReadHtml(string url)
        {
            WebClient client = new WebClient();
            // Add a user agent header in case the requested URI contains a query.
            // Do the translation
            client.Headers.Add("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");
            Stream data = client.OpenRead(url);

            HtmlDocument doc = new HtmlDocument();
            doc.Load(data);
            doc.Save(@"c:\temp\file.htm");
            return doc;
        }

        private static HtmlDocument ReadHtmlByPhantomJS(string url)
        {
            string output = RunPhantomJS(url);

            //string tmpFile = @"c:\temp\tmp.html";
            //StreamWriter sw = new StreamWriter(tmpFile);
            //sw.Write(output);
            //sw.Close();

            //StreamReader sr = new StreamReader(tmpFile);
            //string output = sr.ReadToEnd();
            //sr.Close();

            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(output);
            return doc;
        }

        private static string RunPhantomJS(string url)
        {
            string filepath = @"c:\Temp\";
            string jsFile = filepath + @"loadpage.js";
            StreamWriter sw = new StreamWriter(jsFile);
            sw.WriteLine(@"var page = require('webpage').create();");
            sw.WriteLine("var page = require('webpage').create();");
            sw.WriteLine("page.open('" + url + "');");
            sw.WriteLine("page.settings.javascriptEnabled=true;");
            sw.WriteLine("page.onLoadFinished=function(status){");
            sw.WriteLine("setTimeout(function(){console.log(page.content);phantom.exit()},2000);");
            sw.WriteLine("};");
            sw.Close();

            string output = RunProcess(filepath + @"phantomjs.exe", jsFile);
            return output;
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