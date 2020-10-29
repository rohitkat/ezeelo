using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace ModelLayer.Models.ViewModel
{
     public class WarehouseReturnStockDetailsViewModel
    {
        public long ID { get; set; }
        public long WarehouseStockID { get; set; }
        public long ProductID { get; set; }
        public long ProductVarientID { get; set; }
        public string ItemName { get; set; }
        public string HSNCode { get; set; }
        public string StockThumbPath { get; set; }
        [Display(Name= "Buy Rate Per Unit")]
        public decimal BuyRatePerUnit { get; set; }
         [Display(Name = "Return Rate Per Unit")]
        public decimal ReturnRatePerUnit { get; set; }
         [Display(Name = "Received Qty")]
        public int ReceivedQuantity { get; set; }
         [Display(Name = "Return Qty")]
        public int ReturnQuantity { get; set; }
        public int OldReturnQuantity { get; set; }
        public decimal Amount { get; set; }
        
        public long? ReasonId { get; set; }
        public long? SubReasonId { get; set; }
        public int BatchAvlQty { get; set; }
        public string Remark { get; set; }
        public SelectList ReasonList { get; set; }
        public SelectList SubReasonList { get; set; }
        public long WarehouseStockID_Supp { get; set; }
    }
}
