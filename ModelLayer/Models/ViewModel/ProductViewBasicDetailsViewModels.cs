//-----------------------------------------------------------------------
// <copyright file="ProductViewBasicDetailsViewModels" company="Ezeelo Consumer Services Pvt. Ltd.">
//     Copyright (c) Ezeelo Consumer Services Pvt. Ltd. All rights reserved.
// </copyright>
// <author>Sujata Kullarkar</author>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models.ViewModel
{
    public class ProductViewBasicDetailsViewModels
    {
        public string  ThumbPath { get; set; }
        public long ProductID { get; set; }
        public string ProductName { get; set; }
        public int CategoryID { get; set; }
        public int WeightInGram { get; set; }
        public int LengthInCm { get; set; }
        public int BreadthInCm { get; set; }
        public int HeightInCm { get; set; }
        public string Description { get; set; }
        public int BrandID { get; set; }
        public string BrandName { get; set; }
        public string CategoryName { get; set; }
        public string BrandThumbPath { get; set; }


    }
}
