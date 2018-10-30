using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace SportsStore.Domain.Entities {

    public class Product {

        [HiddenInput(DisplayValue = false)]
        public int ProductID { get; set; }

        [Required(ErrorMessage = "Please enter a product name")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Please enter a brand name")]
        public string Brand { get; set; }

        [Required(ErrorMessage = "Please choose a sex")]
        public int Sex { get; set; }

        [DataType(DataType.MultilineText)]
        [Required(ErrorMessage = "Please enter a description")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Please specify a category")]
        public string Category { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Please enter a positive price")] 
        public decimal Price { get; set; }

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Please enter a positive size format")]
        public int SizeFormat { get; set; }

        [HiddenInput(DisplayValue = false)]
        public string ImageLinks { get; set; }

        [HiddenInput(DisplayValue = false)]
        public string Thumbnail { get; set; }

        public string URL { get; set; }
    }
}