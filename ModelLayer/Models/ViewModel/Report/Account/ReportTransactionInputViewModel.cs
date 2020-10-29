using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models.ViewModel.Report.Account
{
    public class ReportTransactionInputViewModel
    {
        //-- input -------------------------------------------------------------------------------------
        public long ID { get; set; }
        public string Product { get; set; }
        public long ShopStockID { get; set; }
        public long ShopID { get; set; }
        public string ShopName { get; set; }
        public long ChannelPartnerID { get; set; }
        public long MCOCustomerID { get; set; }
        public long MCOShopID { get; set; }
        public long MCODeliveryID { get; set; }
        public long GBID { get; set; }
        public int Qty { get; set; }
        public string Size { get; set; }
        public decimal MRPPerUnit { get; set; }
        public decimal SaleRatePerUnit { get; set; }
        public decimal OfferInPercentByShopPerUnit { get; set; }
        public decimal OfferInRsByShopPerUnit { get; set; }
        public Boolean IsInclusiveOfTAX { get; set; }
        public decimal ServiceTAX { get; set; }
        public Boolean IsShopHandleOtherTAX { get; set; }
        public decimal SumOfOtherTAX { get; set; }
        public decimal LandingPriceByShopPerUnit { get; set; }
        public decimal ChargeINPercentByGBPerUnit { get; set; }
        public decimal CommisionInPercentMCOCustomer { get; set; }
        public decimal CommisionInRsMCOCustomer { get; set; }
        public decimal CommisionInPercentMCOShop { get; set; }
        public decimal CommisionInRsMCOShop { get; set; }
        public decimal CommisionInPercentMCODelivery { get; set; }
        public decimal CommisionInRsMCODelivery { get; set; }
        public decimal CommisionInPercentGB { get; set; }
        public decimal CommisionInRsGB { get; set; }

        //-- ShopOut -----------------------------------------------------------------------------------
        public int QtyShop { get; set; }
        public decimal TotalMRP { get; set; }
        public decimal TotalSaleRate { get; set; }
        public decimal ShopTotalOffer { get; set; }
        public decimal NewSaleRateAfterOffer { get; set; }
        public decimal TotalShopFinalPrice { get; set; }
        public decimal ShopReceivable { get; set; }
        public Boolean IsShopHandleOtherTAX1 { get; set; }
        public decimal OtherTAXPayableReceivableFromMerchant { get; set; }
        public decimal SumOfAmountShopReceivableAfterOtherTAX { get; set; }
        public decimal GBReceivableAmount { get; set; }
        public decimal GBTransactionFee { get; set; }
        public decimal GBServiceTAXOnTransactionFee { get; set; }
        public decimal FinalShopReceivableAfterAllDone { get; set; }

        //-- MCO ---------------------------------------------------------------------------------------
        public int QtyMCO { get; set; }
        public decimal MCOTotalMRP { get; set; }
        public decimal MCOTotalSaleRate { get; set; }
        public decimal MCOCustomerReceivable { get; set; }
        public decimal MCOShopReceivable { get; set; }
        public decimal MCODeliveryReceivable { get; set; }
        public decimal AmountRemaining { get; set; }


        //-- Extra For Display -------------------------------------------------------------------------
        public long CustomerOrderDetailID { get; set; }
        public string ShopOrderCode { get; set; }
        public DateTime CODCreateDate { get; set; }
        public long CODCreateBy { get; set; }
        public DateTime CODModifyDate { get; set; }
        public long CODModifyBy { get; set; }
        public long CustomerOrderID { get; set; }
        public string OrderCode { get; set; }
        public string PaymentMode { get; set; }
        public long CustomerUserLoginID { get; set; }

        //-- extra added on 27-april-2016, as required by nilavana. ---------------------------------
		public string CustomerEmail { get; set; }
        public string CustomerRegMobile { get; set; }
		public long CustomerPersonalDetailID { get; set; }
        public string CustomerName { get; set; }
        public string CustomerShippingAddress { get; set; }
    }
}




