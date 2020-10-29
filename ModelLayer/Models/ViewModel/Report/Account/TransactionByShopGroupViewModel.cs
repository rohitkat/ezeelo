using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models.ViewModel.Report.Account
{
    public class TransactionByShopGroupViewModel
    {
        public long ShopID { get; set; }
        public string ShopName { get; set; }
        //[DisplayFormat(DataFormatString = "{0:n2}", ApplyFormatInEditMode = true)]

        //public string Product { get; set; }
        //public long ShopStockID { get; set; }
        public int QtyAskedByCustomer { get; set; }
        //public decimal MRPPerUnit { get; set; }
        //public decimal SaleRatePerUnit { get; set; }
        //public decimal OfferInPercentByShopPerUnit { get; set; }
        //public decimal OfferInRsByShopPerUnit { get; set; }
        //public Boolean IsInclusiveOfTAX { get; set; }
        //public decimal ServiceTAX { get; set; }
        public Boolean IsShopHandleOtherTAX { get; set; }
        public decimal SumOfOtherTAX { get; set; }
        //public decimal LandingPriceByShopPerUnit { get; set; }
        //public decimal ChargeINPercentByGBPerUnit { get; set; }

        //-- ShopOut -----------------------------------------------------------------------------------
        public int QtyShopOut { get; set; }
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
        public string ProcessRemark { get; set; }
    }
}
