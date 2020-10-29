//-----------------------------------------------------------------------
// <copyright file="ShopProductVarientViewModel" company="Ezeelo Consumer Services Pvt. Ltd.">
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
    public class ShopProductVarientViewModel
    {
        //from stock colorName folder
        public String  StockThumbPath { get; set; }
        public long ProductVarientID { get; set; }
        public long ProductID { get; set; }
        public long CategoryID { get; set; }
        public string CategoryName { get; set; }
        public string ProductName { get; set; }
        public string ColorCode { get; set; }
        public string ColorName { get; set; }
        public string SizeName { get; set; }
        public string DimensionName { get; set; }
        public string MaterialName { get; set; }
        public long ColorID { get; set; }
        public long SizeID { get; set; }
        public long DimensionID { get; set; }
        public long MaterialID { get; set; }
        public long ShopStockID { get; set; }
        public Nullable<long> WarehouseStockID { get; set; }  //Added by Zubair for Inventory on 27-03-2018
        public long ShopID { get; set; }
        public string ShopName { get; set; }
        public decimal MRP { get; set; }
        public decimal SaleRate { get; set; }
        public int StockQty{ get; set; }
        public bool StockStatus { get; set; }
        public decimal PackSize { get; set; }
        public string PackUnitName { get; set; }
        public decimal ActualWeight { get; set; }
        public decimal VolumetricWeight { get; set;}        
        public bool IsAddedInWishlist { get; set;}

        //Added by Tejaswee for taxation
        public bool IsInclusiveOfTax { get; set; }

        public decimal BusinessPointPerUnit { get; set; } // Added by Zubair for BusinessPoint on 04/01/2018
        public decimal BusinessPoints { get; set; } // Added by Zubair for BusinessPoint on 04/01/2018
        public decimal CashbackPointPerUnit { get; set; } // Added by Zubair for BusinessPoint on 04/01/2018
       //public decimal CashbackPoints { get; set; }
        public int IsDisplayCB { get; set; }
        public List<CalulatedTaxesRecord> lCalulatedTaxesRecord { get; set; }

        //public string fcon { get; set; }

        public Nullable<int> IsPriority { get; set; }
        public List<ShopPaymentModeViewModel> PayMode { get; set;}
        public List<ImageListViewModel> StockImageList { get; set; }

        public int OfferType { get; set; }
    }
    
}