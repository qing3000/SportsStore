using System;
using System.Collections.Generic;
using System.Linq;
using SportsStore.Domain.Entities;
using System.Web;

namespace ConsoleTesting
{
    class Program
    {
        static void Main(string[] args)
        {
            string url = "https://www.next.co.uk/shop/gender-newborngirls-gender-oldergirls-gender-youngergirls-category-dresses";
            NextParser nextParser = new NextParser();
            // IEnumerable<string> links = nextParser.ParseNextProductList(url);
            // Utilities.WriteToBinaryFile(@"temp.bin", links);
            IEnumerable<string> links = (Utilities.ReadFromBinaryFile<IEnumerable<string>>(@"temp.bin"));
            Console.WriteLine(@"Total of {0} products found in the page", links.Count());
            IList<Product> products = new List<Product>();
            foreach (string link in links.Take(10))
            {
                Console.WriteLine(@"Processing link {0}", link);
                products.Add(nextParser.ParseNextProduct(link));
             }
        }
    }
}
