//-----------------------------------------------------------------------
// <copyright file="ProductTechnicalSpecificationViewModel" company="Ezeelo Consumer Services Pvt. Ltd.">
//     Copyright (c) Ezeelo Consumer Services Pvt. Ltd. All rights reserved.
// </copyright>
// <author>Sujata Kullarkar</author>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ModelLayer.Models.ViewModel
{
    public class ProductTechnicalSpecificationViewModel
    {
      
        public Nullable<int> ParentSpecificationID { get; set; }
        public int SpecificationID { get; set; }
        public string ParentSpecificationName { get; set; }
        public string SpecificationName { get; set; }
        public int Level { get; set; }
        public long ProductSpecificationID { get; set; }
        public string Value { get; set; }

    }
}