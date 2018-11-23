using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Web.Mvc;

namespace SportsStore.Domain.Entities
{
    public enum EGender
    {
        MALE,
        FEMALE,
        UNISEX
    }

    public enum EBrand
    {
        BODEN,
        NEXT,
        UNKNOWN
    }

    public enum EColorSeries
    {
        RED,
        GREEN,
        BLUE,
        OTHERS,
        ANY
    }

    public enum ECategory
    {
        DRESSES,
        SHOES,
        SOCKES,
        BIBS,
        TSHIRTS,
        OTHERS
    };

    public class BrandClass
    {
        static public Dictionary<EBrand, string> Brands = new Dictionary<EBrand, string>
        {
            { EBrand.BODEN, @"Boden" },
            { EBrand.NEXT, @"Next" },
            { EBrand.UNKNOWN, @"Unknown" }
        };
    }

    public class GenderClass
    {
        static public Dictionary<EGender, Tuple<string, string>> Genders = new Dictionary <EGender, Tuple<string, string>>
        {
            { EGender.MALE, Tuple.Create(@"Boy", @"男孩") },
            { EGender.MALE, Tuple.Create(@"Girl", @"女孩") },
            { EGender.UNISEX, Tuple.Create(@"Unisex", @"中性") }
        };
    }

    public class CategoryClass
    {
        static public Dictionary<ECategory, Tuple<string, string>> Categories = new Dictionary<ECategory, Tuple<string, string>>
        {
            { ECategory.DRESSES, Tuple.Create(@"Dresses", @"连衣裙") },
            { ECategory.SHOES, Tuple.Create(@"Dresses", @"鞋") },
            { ECategory.SOCKES, Tuple.Create(@"Dresses", @"袜子") },
            { ECategory.BIBS, Tuple.Create(@"Dresses", @"围嘴") },
            { ECategory.TSHIRTS, Tuple.Create(@"Dresses", @"T恤") },
            { ECategory.OTHERS, Tuple.Create(@"Dresses", @"其他") },
        };
    };

    public class ColorSeriesClass
    {
        static public Dictionary<EColorSeries, Tuple<string, string>> ColorSeries = new Dictionary<EColorSeries, Tuple<string, string>>
        {
            { EColorSeries.RED, Tuple.Create(@"Red color series", @"红色系") },
            { EColorSeries.GREEN, Tuple.Create(@"Dresses", @"绿色系") },
            { EColorSeries.BLUE, Tuple.Create(@"Dresses", @"蓝色系") },
            { EColorSeries.OTHERS, Tuple.Create(@"Dresses", @"其他色系") },
            { EColorSeries.ANY, Tuple.Create(@"Dresses", @"任意色系") },
        };
    };

    [Serializable]
    public class PriceInfo
    {
        public string Size { get; set; }
        public decimal Price { get; set; }
        public decimal PriceCN { get; set; }
        public string Stock { get; set; }
        public string TranslateSize()
        {
            return Translator.ManualTranslate(this.Size);
        }
        public string TranslateStock()
        {
            return Translator.ManualTranslate(this.Stock);
        }
    }

    [Serializable]
    public class Product {

        [HiddenInput(DisplayValue = false)]
        public int ID { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string TitleCN { get; set; }

        [Required]
        public string Brand { get; set; }

        [Required]
        public string ProductID { get; set; }

        [Required]
        public EGender Gender { get; set; }

        [DataType(DataType.MultilineText)]
        [Required]
        public string Description { get; set; }

        [DataType(DataType.MultilineText)]
        [Required]
        public string DescriptionCN { get; set; }

        [DataType(DataType.MultilineText)]
        [Required]
        public string Material { get; set; }

        [DataType(DataType.MultilineText)]
        [Required]
        public string MaterialCN { get; set; }

        [Required]
        public ECategory Category { get; set; }

        [Required]
        public byte[] SizePricesBinary { get; set; }

        [Required]
        [Range(0, 200, ErrorMessage = "Please enter a valid age")]
        public Single MinimumAge { get; set; }

        [Required]
        [Range(0, 200, ErrorMessage = "Please enter a valid age")]
        public Single MaximumAge { get; set; }

        [Required]
        public string URL { get; set; }

        [Required]
        public string ThumbnailLink { get; set; }

        [Required]
        public string ImageLinks { get; set; }

        [Required]
        public DateTime InsertTime { get; set; }

        [Required]
        public DateTime UpdateTime { get; set; }

        public void SetPriceInfos(PriceInfo[] priceInfos)
        {
            var binaryFormatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
            MemoryStream priceStream = new MemoryStream();
            binaryFormatter.Serialize(priceStream, priceInfos);
            this.SizePricesBinary = priceStream.ToArray();
        }

        public PriceInfo[] GetPriceInfos()
        {
            var binaryFormatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
            MemoryStream priceStream = new MemoryStream(this.SizePricesBinary);
            PriceInfo[] prices = (PriceInfo[])binaryFormatter.Deserialize(priceStream);
            return prices;
        }

        public Tuple<decimal, decimal> GetMinMaxPriceCN()
        {
            PriceInfo[] priceInfos = GetPriceInfos();
            IEnumerable<decimal> prices = priceInfos.Select(x => x.PriceCN);
            return new Tuple<decimal, decimal>(prices.Min(), prices.Max());
        } 
    }
}