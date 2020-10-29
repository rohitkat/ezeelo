//-----------------------------------------------------------------------
// <copyright file="SearchProductDetailsViewModel" company="Ezeelo Consumer Services Pvt. Ltd.">
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
    public class PrdVarientViewModel
    {
        public long ShopStockID { get; set; }
        public long ProductID { get; set; }
        public int CategoryID { get; set; }
        public int ShopID { get; set; }
        public int SizeID { get; set; }
        public string Size { get; set; }
        public decimal MRP { get; set; }
        public decimal SaleRate { get; set; }
        public decimal RetailPoint { get; set; } //Yashaswi 10-7-2018
        public int StockStatus { get; set; } //Yashaswi 25-7-2018
        public int StockQty { get; set; } //Yashaswi 25-7-2018
        public long BrandId { get; set; }//Added for Api filter by Sonali_04-01-2019
        public string BrandName { get; set; }//Added for Api filter by Sonali_04-01-2019
        public decimal CashbackPoint { get; set; }
        public int IsDisplayCB { get; set; }
    }


    public class SearchProductDetailsViewModel
    {

        //from default folder
        public String ProductThumbPath { get; set; }
        public long ProductID { get; set; }
        public string Name { get; set; }
        public int CategoryID { get; set; }
        public string CategoryName { get; set; }
        public int StockStatus { get; set; }
        public decimal MRP { get; set; }
        public decimal SaleRate { get; set; }
        public long ShopStockID { get; set; }
        public string Color { get; set; }
        public decimal PackSize { get; set; }
        public string PackUnit { get; set; }
        public string Material { get; set; }
        public string Dimension { get; set; }
        public string Size { get; set; }
        public int StockQty { get; set; }
        public string HtmlColorCode { get; set; }

        public int FirstLevelCatId { get; set; }
        public decimal RetailPoint { get; set; } //Yashaswi 10-7-2018
        public decimal CashbackPoint { get; set; }
        public int IsDisplayCB { get; set; }
        //-- Extra Added on 06-june-2016 for product varients in dropdownlist By Avi Verma.
        public int ShopID { get; set; }
        public List<PrdVarientViewModel> ProductVarientViewModels { get; set; }

        public string URLStructureName { get; set; }//Added for ULR Structure RULE by AShish
        public int OfferType { get; set; }//Added by Sonali for OfferType_17-11-2018
        public long BrandId { get; set; }//Added for Api filter by Sonali_04-01-2019
        public string BrandName { get; set; }//Added for Api filter by Sonali_04-01-2019
    }
}