using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SportsStore.Domain.Entities;

namespace SportsStore.WebUI.Models
{
    public class NavFilterViewModel
    {
        public EGender Gender { get; set; }
        public Decimal Age { get; set; }

        public string[] CategoryStrings { get; set; }
        public bool CategoryOn { get; set; }
        public ECategory Category { get; set; }

        public string[] BrandStrings { get; set; }
        public bool BrandOn { get; set; }
        public EBrand Brand { get; set; }

        public EColorSeries ColorSeries { get; set; }
        public Decimal MinimumPrice { get; set; }
        public Decimal MaximumPrice { get; set; }
        public string KeyWord { get; set; }
    }
}