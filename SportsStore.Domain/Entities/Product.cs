using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace SportsStore.Domain.Entities
{
    public enum EGender
    {
        MALE,
        FEMALE,
        NEUTRAL
    }

    public class GenderClass
    {
        static public string[] Genders = new string[]
        {
            @"Boy",
            @"Girl",
            @"Neutral"
        };

        static public string[] GendersCN = new string[]
        {
            @"男孩",
            @"女孩",
            @"中性"
        };
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

    public class CategoryClass
    {
        static public string[] Categories = new string[]
        {
            @"Dresses",
            @"Shoes",
            @"Socks",
            @"Bibs",
            @"T-Shirt",
            @"Others"
        };

        static public string[] CategoriesCN = new string[]
        {
            @"连衣裙",
            @"鞋",
            @"袜子",
            @"围嘴",
            @"T恤",
            @"其他"
        };
    };

    [Serializable]
    public class PriceInfo
    {
        public string Size { get; set; }
        public decimal Price { get; set; }
        public decimal PriceCN { get; set; }
        public string Stock { get; set; }
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
        public PriceInfo[] SizePrices { get; set; }

        [Required]
        [Range(0, 200, ErrorMessage = "Please enter a valid age")]
        public double MinimumAge { get; set; }

        [Required]
        [Range(0, 200, ErrorMessage = "Please enter a valid age")]
        public double MaximumAge { get; set; }

        [Required]
        public string URL { get; set; }

        [Required]
        public string ThumbnailLink { get; set; }

        [Required]
        public string ImageLinks { get; set; }

        [Required]
        public DateTime EntryTime;
    }
}