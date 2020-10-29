//-----------------------------------------------------------------------
// <copyright file="ProductDescriptionViewModel" company="Ezeelo Consumer Services Pvt. Ltd.">
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
   public class ProductDescriptionViewModel
    {
        public long ProductID { get; set; }
        public int ProductSpecificationID { get; set; }
        public int SpecificationID { get; set; }
        public string SpecificationName { get; set; }
        public string SpecificationValue { get; set; }
    }
}
