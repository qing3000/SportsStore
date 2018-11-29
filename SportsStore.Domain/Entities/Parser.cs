using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

namespace SportsStore.Domain.Entities
{
    public class Parser
    {
        private static void SaveHtmlNode(HtmlNode node)
        {
            HtmlDocument doc = new HtmlDocument();
            doc.DocumentNode.AppendChild(node);
            doc.Save(@"c:\temp\node.html");
        }

        public static Decimal GBP2RMB(Decimal x)
        {
            Decimal PRICE_RMBRATE = 10;
            Decimal PRICE_PROFIT = 50;
            return PRICE_RMBRATE * x + PRICE_PROFIT;
        }

        static public bool WaitAjaxToLoad(IWebDriver driver, IWait<IWebDriver> wait)
        {
            bool ajaxLoaded = false;
            try
            {
                wait.Until(x => (long)((IJavaScriptExecutor)x).ExecuteScript("return jQuery.active") == 1);
                ajaxLoaded = true;
                Console.WriteLine("Ajax loading.");
            }
            catch (WebDriverTimeoutException)
            {
                Console.WriteLine("Timeout, No Ajax loading.");
                // No ajax loading
                ajaxLoaded = false;
            }

            if (ajaxLoaded == true)
            {
                wait.Until(x => (long)((IJavaScriptExecutor)x).ExecuteScript("return jQuery.active") == 0);
                Console.WriteLine("Ajax loading finished.");
            }

            return ajaxLoaded;
        }

        static IWebDriver CreateWebDriver()
        {
            // Create a headless Google Chrome browser (no need to load images).
            ChromeOptions options = new ChromeOptions();
            options.AddArgument(@"--headless");
            options.AddArgument("disable-infobars");
            options.AddArgument("--disable-extensions");
            options.AddArgument("--blink-settings=imagesEnabled=false");

            // Go to the website
            IWebDriver driver = new ChromeDriver(options);
            return driver;
        }

        static public IWebDriver LoadWebPage(string url)
        {
            // Create the web driver.
            IWebDriver driver = CreateWebDriver();

            // Go to the website
            driver.Navigate().GoToUrl(url);

            // Load the webpage and wait for the AJAX and Javascript to finish loading.
            IWait<IWebDriver> wait = new WebDriverWait(driver, TimeSpan.FromSeconds(2));
            wait.PollingInterval = TimeSpan.FromMilliseconds(1);
            WaitAjaxToLoad(driver, wait);
            wait.Until(x => (string)((IJavaScriptExecutor)x).ExecuteScript("return document.readyState") == "complete");

            return driver;
        }

        static public IWebDriver LoadScrollableWebPage(string url)
        {
            // Create the web driver.
            IWebDriver driver = CreateWebDriver();

            // Go to the website
            driver.Navigate().GoToUrl(url);

            // Scroll the page to the bottom and until no more need content can be loaded.
            IWait<IWebDriver> wait = new WebDriverWait(driver, TimeSpan.FromSeconds(2));
            wait.PollingInterval = TimeSpan.FromMilliseconds(1); ;
            do
            {
                ((IJavaScriptExecutor)driver).ExecuteScript(@"window.scrollTo(0, document.body.scrollHeight);");
            } while (WaitAjaxToLoad(driver, wait));

            return driver;
        }


        //public static Product ParseProduct(string url)
        //{
        //    Product product = null;
        //    if (url.Contains(@"next"))
        //    {
        //        product = ParseNEXTProduct(url);
        //        product.URL = url;
        //    }
        //    else if (url.Contains(@"boden"))
        //    {
        //        product = ParseBodenProduct(url);
        //    }

        //    return product;
        //}

        //public static IEnumerable<string> ParsePage(string url)
        //{
        //    IEnumerable<string> productURLs = null;
        //    if (url.Contains(@"next"))
        //    {
        //        productURLs = ParseNEXTProductList(url);
        //    }
        //    else if (url.Contains(@"boden"))
        //    {
        //        productURLs = ParseBODENPage(url);
        //    }

        //    return productURLs;
        //}


        //private static HtmlDocument ReadHtml(string url)
        //{
        //    WebClient client = new WebClient();
        //    // Add a user agent header in case the requested URI contains a query.
        //    // Do the translation
        //    //client.Headers.Add("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");
        //    //Stream data = client.OpenRead(url);

        //    HtmlDocument doc = new HtmlDocument();
        //    //doc.Load(data);

        //    StreamReader sr = new StreamReader(@"c:\temp\1.html");
        //    doc.Load(sr);
        //    sr.Close();
        //    //doc.Save(@"c:\temp\1.html");
        //    return doc;
        //}

        //public static string ReadHtmlByPhantomJS(string url, string jsLoadPage)
        //{
        //    //int urlSplit = url.LastIndexOf('/') + 1;
        //    //string ss1 = url.Substring(0, urlSplit);
        //    //string ss2 = WebUtility.UrlEncode(url.Substring(urlSplit));
        //    //string uri = ss1 + ss2;

        //    string jsFile = @"temp.js";
        //    string outputFile = @"temp.html";

        //    StreamReader sr = new StreamReader(jsLoadPage);
        //    string ss = sr.ReadToEnd();
        //    sr.Close();

        //    StreamWriter sw = new StreamWriter(jsFile);
        //    sw.Write(ss.Replace(@"URL", url).Replace(@"OUTPUT_FILE", outputFile.Replace(@"\", @"/")));
        //    sw.Close();

        //    RunProcess(@"phantomjs.exe", jsFile);

        //    sr = new StreamReader(outputFile);
        //    string pageContent = sr.ReadToEnd();
        //    sr.Close();

        //    return pageContent;
        //}

        //private static Tuple<string, PriceInfo[]> ParseHtmlByPhantomJS(string url, string jsInfoParser)
        //{
        //    string filepath = @"c:\Temp\";
        //    string jsFile = filepath + @"temp.js";

        //    StreamReader sr = new StreamReader(jsInfoParser);
        //    string ss = sr.ReadToEnd();
        //    sr.Close();

        //    StreamWriter sw = new StreamWriter(jsFile);
        //    sw.Write(ss.Replace(@"URL", url));
        //    sw.Close();

        //    string output = RunProcess(filepath + @"phantomjs.exe", jsFile.Replace(@"\", @"/"));

        //    // Extract the page content.

        //    string pageContent = ExtractString(output, @"---START OF PAGE---", @"---END OF PAGE---");
        //    string priceInfoString = ExtractString(output, @"---START OF PRICEINFO---", @"---END OF PRICEINFO---");
        //    PriceInfo[] priceInfo = ParsePriceInfoString(priceInfoString);
        //    return new Tuple<string, PriceInfo[]>(pageContent, priceInfo); ;
        //}

        //private static string ExtractString(string s, string startKey, string endKey)
        //{
        //    int i0 = s.IndexOf(startKey);
        //    int i1 = s.IndexOf(endKey);
        //    string substring = s.Substring(i0 + startKey.Length, i1 - i0 - startKey.Length).Trim();
        //    return substring;
        //}
        //private static PriceInfo[] ParsePriceInfoString(string infoString)
        //{
        //    IList<PriceInfo> priceInfos = new List<PriceInfo>();
        //    string[] lines = infoString.Split('\n');
        //    foreach (string oneline in lines)
        //    {
        //        string[] ss = oneline.Split(',');
        //        string priceNumberString = new string(ss[1].Where(x => Char.IsDigit(x) || x == '.').ToArray());
        //        Decimal price = priceNumberString.Length > 0 ? Convert.ToDecimal(priceNumberString) : -1;
        //        Decimal priceCN = GBP2RMB(price); 
        //        priceInfos.Add(new PriceInfo { Size = ss[0].Trim(), Price= price, PriceCN = priceCN, Stock = ss[2].Trim() });
        //    }

        //    return priceInfos.ToArray();
        //}



        //private static string RunProcess(string command, string arguments)
        //{
        //    System.Diagnostics.Process pProcess = new System.Diagnostics.Process();
        //    pProcess.StartInfo.FileName = command;
        //    pProcess.StartInfo.Arguments = arguments;
        //    pProcess.StartInfo.UseShellExecute = false;
        //    pProcess.StartInfo.RedirectStandardOutput = true;
        //    pProcess.StartInfo.StandardOutputEncoding = System.Text.Encoding.UTF8;
        //    pProcess.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
        //    pProcess.StartInfo.CreateNoWindow = true; //not diplay a windows
        //    pProcess.Start();
        //    string output = pProcess.StandardOutput.ReadToEnd(); //The output result
        //    pProcess.WaitForExit();
        //    return output;
        //}
    }
}