//-----------------------------------------------------------------------
// <copyright file="ProductRefinementsViewModel" company="Ezeelo Consumer Services Pvt. Ltd.">
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
    public class ProductRefinementsViewModel
    {

        public long ProductID { get; set; }
        public string ProductName { get; set; }
        public long ShopID { get; set; }
        public string ShopName { get; set; }
        public long CategoryID { get; set; }
        public string CategoryName { get; set; }
        public int BrandID { get; set; }
        public string BrandName { get; set; }
        public int ColorID { get; set; }
        public string Color { get; set; }
        public int SizeID { get; set; }
        public string Size { get; set; }
        public int DimensionID { get; set; }
        public string Dimension { get; set; }
        public int MaterialID { get; set; }
        public string Material { get; set; }
        public int ProductSpecificationID { get; set; }
        public int SpecificationID { get; set; }
        public string SpecificationName { get; set; }
        public string SpecificationValue { get; set; }
        public decimal MRP { get; set; }
        public decimal SaleRate { get; set; }
        public int CityID { get; set; }
        public decimal PackSize { get; set; }
        public string PackUnit { get; set; }

        //shop category order sequence added by Pradnyakar sir and Tejaswee 
        public int CategoryOrderSequence { get; set; }

        public int FranchiseID { get; set; }////added by Ashish

        public decimal RetailPoint { get; set; } //Yashaswi 10-7-2018
        public int OfferType { get; set; } //Sonali 19-11-2018
        public decimal CashbackPoint { get; set; }
        public int IsDisplayCB { get; set; }
    }



}