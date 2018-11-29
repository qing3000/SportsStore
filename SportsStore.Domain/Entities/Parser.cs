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
using System.Diagnostics;

namespace SportsStore.Domain.Entities
{
    public class Parser
    {
        protected IWebDriver driver;
        protected IWait<IWebDriver> wait;
        const double TIMEOUT_SECONDS = 2.0;
        const double POLLING_INTERVAL_MILLISECONDS = 1.0;
        public Parser()
        {

            // Create a headless Google Chrome browser (no need to load images).
            ChromeOptions options = new ChromeOptions();
            options.AddArgument(@"--headless");
            options.AddArgument("disable-infobars");
            options.AddArgument("--disable-extensions");
            options.AddArgument("--blink-settings=imagesEnabled=false");
            //options.AddArgument("--window-size=1920,1080");

            // Go to the website
            this.driver = new ChromeDriver(options);
            this.wait = new WebDriverWait(driver, TimeSpan.FromSeconds(TIMEOUT_SECONDS));
            this.wait.PollingInterval = TimeSpan.FromMilliseconds(POLLING_INTERVAL_MILLISECONDS);
        }

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

        public void LoadWebPage(string url)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            // Go to the website
            this.driver.Navigate().GoToUrl(url);
            stopwatch.Stop();
            Console.WriteLine("Time consumed in goto url = {0}", stopwatch.Elapsed);

            stopwatch.Restart();
            // Load the webpage and wait for the AJAX and Javascript to finish loading.
            WaitAjaxToLoad(this.driver, this.wait);
            this.wait.Until(x => (string)((IJavaScriptExecutor)x).ExecuteScript("return document.readyState") == "complete");
            stopwatch.Stop();
            Console.WriteLine("Time consumed in waiting for page to load = {0}", stopwatch.Elapsed);
        }

        public void LoadScrollableWebPage(string url)
        {
            // Go to the website
            this.driver.Navigate().GoToUrl(url);
            do
            {
                ((IJavaScriptExecutor)this.driver).ExecuteScript(@"window.scrollTo(0, document.body.scrollHeight);");
            } while (WaitAjaxToLoad(driver, wait));
        }
    }
}