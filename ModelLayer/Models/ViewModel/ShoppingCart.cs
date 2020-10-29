using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ModelLayer.Models.ViewModel
{
    /// <summary>
    /// View model for shopping cart
    /// </summary>
    public class ShoppingCartViewModel
    {
        //from stock colorName folder
        public String StockThumbPath { get; set; }
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
        public int StockQty { get; set; }
        public decimal PackSize { get; set; }
        public string PackUnitName { get; set; }
        public int PurchaseQuantity { get; set; }
        public decimal ActualWeight { get; set; }
        public decimal VolumetricWeight { get; set; }

        public decimal DeliveryCharge { get; set; }

        public string Message { get; set; }
        public string CouponCode { get; set; }
        public long CouponValueRs { get; set; }
        public long CouponValuePercent { get; set; }


        //============= Taxation property ================
        public bool IsInclusiveOfTax { get; set; }
        public decimal BusinessPointPerUnit { get; set; } // Added by Zubair for MLM on 04/01/2018
        public decimal BusinessPoints { get; set; } // Added by Zubair for MLM on 04/01/2018
        public decimal CashbackPoints { get; set; } // Added by Yashaswi 3-10-2019
        public decimal CashbackPoitPerUnit { get; set; } // Added by Yashaswi 3-10-2019
        public List<CalulatedTaxesRecord> lCalulatedTaxesRecord { get; set; }


        public string CartVerificationMsg { get; set; } //yashaswi for Cart Verification

        //public decimal TaxPercentage { get; set; }
        //public decimal TaxableAmount { get; set; }
        //public string Remarks { get; set; }
    }

    public class ShoppingCartOrderDetails
    {
        //Product Amount
        public decimal TotalOrderAmount { get; set; }
        public decimal PayableAmount { get; set; }
        public int NoOfPointUsed { get; set; }
        public decimal ValuePerPoint { get; set; }
        public string CoupenCode { get; set; }
        public decimal CoupenAmount { get; set; }

        public decimal EarnAmount { get; set; }
        public decimal UsedEarnAmount { get; set; }

        public decimal RemainEarnAmt { get; set; }

        public bool IsUse { get; set; }

        //Added by Zubair for MLM on 05-01-2018
        public decimal BusinessPointsTotal { get; set; } // Added by Zubair for MLM on 04/01/2018
        public decimal WalletAmountUsed { get; set; } // Added by Zubair for MLM on 04/01/2018
        public decimal DeliveryCharges { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal CashbackPointsTotal { get; set; } //Added by Yashaswi 03-10-2019
    }


    //Name it as CustomerOrderDetailsViewModel 
    /// <summary>
    /// shopping cart view model class define property of "ShoppingCartViewModel" type
    /// </summary>
    public class ShopProductVarientViewModelCollection
    {
        //Master Order 
        public ShoppingCartOrderDetails lShoppingCartOrderDetails { get; set; }
        //Products In Orders

        public MLMWallet lMLMWallets { get; set; }     // Added by Zubair for MLM on 04/01/2018
        public List<ShoppingCartViewModel> lShopProductVarientViewModel { get; set; }
        //Shop Wise Delivery Charges
        public List<ShopWiseDeliveryCharges> lShopWiseDeliveryCharges { get; set; }

        //Subscribed Discount On Category
        public List<SubscribedDiscountOnCategoryViewModel> lSubscribedDiscountOnCategoryViewModel { get; set; }

        public List<CorporateShippingFacility> lCorporateShippingFacility { get; set; }

        public string DeliveryScheduleID { get; set; }

        public DateTime DeliveryDate { get; set; }

        //public List<DeliverySchedule> lDeliverySchedule { get; set; } 
        public List<CalculatedTaxList> lCalculatedTaxList { get; set; }

        //Added by yashaswi
        public double walletAmountUsed { get; set; }
        //Added by Yashaswi
        public string CartMessage { get; set; }
        public bool IsBoosterPlan { get; set; }
    }

    public class TaxList
    {
        //public string TaxName { get; set; }
        public string TaxPrefix { get; set; }
    }



}