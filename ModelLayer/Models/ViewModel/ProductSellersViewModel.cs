//-----------------------------------------------------------------------
// <copyright file="ProductSellersViewModel" company="Ezeelo Consumer Services Pvt. Ltd.">
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
    public class ProductSellersViewModel
    {
        public string  LogoPath { get; set; }
        public  long ShopID { get; set; }
        public long ProductID { get; set; }
        public long ShopProductID { get; set; }  
        public long ShopStockID { get; set; }        
        public string ShopName { get; set; }
        public int OfferCount { get; set; }
        public decimal MRP { get; set; }
        public decimal SaleRate { get; set; }
        public string Color { get; set; }
        public decimal PackSize { get; set; }
        public string PackUnit { get; set; }
        public string Material { get; set; }
        public string Dimension { get; set; }
        public string Size { get; set; }
        public int StockQty { get; set; }
        public string HtmlColorCode { get; set; }
        public ProductOffersViewModel ProductOffer { get; set; }        

    }
}