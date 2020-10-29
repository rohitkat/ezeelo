//-----------------------------------------------------------------------
// <copyright file="ProductComponentOfferViewModel" company="Ezeelo Consumer Services Pvt. Ltd.">
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
    public class ProductComponentOfferViewModel
    {
        public int OfferID { get; set; }
        public string Title{ get; set; }
        public int ComponentID { get; set; }
        public string ComponentName { get; set; }
        public decimal OfferInRs { get; set; }
        public decimal OfferInPercent { get; set; }
        public string Description { get; set; } 
    }
}