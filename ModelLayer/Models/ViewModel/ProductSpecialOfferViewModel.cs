//-----------------------------------------------------------------------
// <copyright file="ProductSpecialOfferViewModel" company="Ezeelo Consumer Services Pvt. Ltd.">
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
    public class ProductSpecialOfferViewModel
    {
        public long OfferID { get; set; }
        public string Title { get; set; }
        public bool IsFree { get; set; }
        public int MinPurchaseQty { get; set; }
        public int FreeQty { get; set; }
        public decimal DiscountInRs { get; set; }
        public decimal DiscountInPercent { get; set; }
        public string Description { get; set; }
        public List<long> FreeStockList { get; set; }


    }
}