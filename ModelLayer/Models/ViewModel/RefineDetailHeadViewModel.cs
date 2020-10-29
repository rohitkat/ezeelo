//-----------------------------------------------------------------------
// <copyright file="RefineDetailHeadViewModel" company="Ezeelo Consumer Services Pvt. Ltd.">
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
    public class RefineDetailHeadViewModel
    {
        public int ProductSpecificationID { get; set; }
        public string SpecificationName { get; set; }
        public int SpecificationID { get; set; }
        public string SpecificationValue { get; set; }
        public bool IsSelected { get; set; }
    }
}