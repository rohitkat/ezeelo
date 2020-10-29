using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ModelLayer.Models.ViewModel
{
    public class RefinePriceViewModel
    {
        public decimal Max { get; set; }
        public decimal Min { get; set; }
        public string Slab { get; set; }
        public bool IsSelected { get; set; }
        public bool IsProductAvailable { get; set; }
    }
}