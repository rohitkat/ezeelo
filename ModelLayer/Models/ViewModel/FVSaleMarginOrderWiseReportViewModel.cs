using System;
using System.Collections.Generic;

using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace ModelLayer.Models.ViewModel
{
     public class FVSaleMarginOrderWiseReportViewModel
    {

        public DateTime? OrderDate { get; set; }
        public string SKUName { get; set; }
        public long SKUID { get; set; }
        public string SKUUnit { get; set; }
        public string OrderCode { get; set; }
        public DateTime? OrderDeliveryDate { get; set; }
        public string Customer { get; set; }
        public string PaymentMode { get; set; }
        public decimal? TotalAmount { get; set; }
        public decimal? DeliveryCharge { get; set; }
        public decimal? WalletAmountUsed { get; set; }
        public decimal MarginAmount { get; set; }
        public decimal RetailPointsonOrder { get; set; }
        public DateTime OrderPlacedDate { get; set; }
        public DateTime ? DeliveryDate { get; set; }
        public string OrderStatus { get; set; }
        public decimal ? PayableAmount { get; set; }

    }




    public class FVSaleMarginOrderWiseReportViewModelList
    {
        public List<FVSaleMarginOrderWiseReportViewModel> lFVSaleMarginOrderWiseReportViewModel = new List<FVSaleMarginOrderWiseReportViewModel>();
        public int FrenchiseID { get; set; }
        public string FrenchiseName { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
    }
}
