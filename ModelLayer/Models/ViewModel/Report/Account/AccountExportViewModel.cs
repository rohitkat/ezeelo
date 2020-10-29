using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ModelLayer.Models.ViewModel.Report.Account
{
    public class AccountExportViewModel
    {
        //-- extra added on 27-april-2016, as required by nilavana. ---------------------------------
        //public string CustomerEmail { get; set; }
        //public string CustomerRegMobile { get; set; }
        //public long CustomerPersonalDetailID { get; set; }
        public string CustomerName { get; set; }
        //public string CustomerShippingAddress { get; set; }

        //-- Extra For Display -------------------------------------------------------------------------
        public string OrderCode { get; set; }
        public string ShopOrderCode { get; set; }
        //[DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public string CODCreateDate { get; set; }
        public string PaymentMode { get; set; }

        //-- input -------------------------------------------------------------------------------------
        public string Product { get; set; }
        public string ShopName { get; set; }
        public int Qty { get; set; }
        public string Size { get; set; }
        public decimal MRPPerUnit { get; set; }
        public decimal SaleRatePerUnit { get; set; }
        public Boolean IsInclusiveOfTAX { get; set; }
        public decimal ServiceTAX { get; set; }
        public decimal LandingPriceByShopPerUnit { get; set; }
        public decimal ChargeINPercentByGBPerUnit { get; set; }

        //-- ShopOut -----------------------------------------------------------------------------------
        public int QtyShop { get; set; }
        public decimal TotalMRP { get; set; }
        public decimal TotalSaleRate { get; set; }
        public decimal GBReceivableAmount { get; set; }
        public decimal GBTransactionFee { get; set; }
        public decimal GBServiceTAXOnTransactionFee { get; set; }
        public decimal FinalShopReceivableAfterAllDone { get; set; }
    }
}

