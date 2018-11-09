using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace SportsStore.Domain.Entities {

    public class PriceInfo
    {
        public string Size { get; set; }
        public decimal Price { get; set; }
        public decimal PriceCN { get; set; }
        public string Stock { get; set; }
    }

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
        public byte Sex { get; set; }

        [DataType(DataType.MultilineText)]
        [Required]
        public string Description { get; set; }

        [DataType(DataType.MultilineText)]
        [Required]
        public string DescriptionCN { get; set; }

        [DataType(DataType.MultilineText)]
        [Required(ErrorMessage = "Please enter a description")]
        public string Material { get; set; }

        [DataType(DataType.MultilineText)]
        [Required(ErrorMessage = "Please enter a description")]
        public string MaterialCN { get; set; }

        [Required(ErrorMessage = "Please specify a category")]
        public string Category { get; set; }

        [Required(ErrorMessage = "Please specify a category")]
        public string CategoryCN { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Please enter a valid price")] 
        public PriceInfo[] SizePrices { get; set; }

        [Required]
        [Range(0, 200, ErrorMessage = "Please enter a valid age")]
        public double MinimumAge { get; set; }

        [Required]
        [Range(0, 200, ErrorMessage = "Please enter a valid age")]
        public double MaximumAge { get; set; }

        [Required]
        public string URL { get; set; }

        [HiddenInput(DisplayValue = false)]
        public string ThumbnailLink { get; set; }

        [HiddenInput(DisplayValue = false)]
        public string ImageLinks { get; set; }
    }
}