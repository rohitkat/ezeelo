using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace ModelLayer.Models.ViewModel
{
     public class QuotationListReportViewModel
    {
         public long WarehouseID { get; set; }
         public string SKUName { get; set; }
         public string HSNCode { get; set; }
         public string SupplierName  { get; set; }
         public long SKUID { get; set; }
         public string SKUUnit { get; set; }
         public string QuotationCode { get; set; }
         public int QuantityRequired { get; set; }
         public DateTime CreateDate { get; set; }
         public DateTime QuotationRequestDate { get; set; }
         public DateTime ExpectedReplyDate { get; set; }
         public string Category3 { get; set; }
         public string Category2{ get; set; }
         public string Category1{ get; set; }
         public bool STATUS { get; set; }
         public string Remark { get; set; }
         public string ReorderLevelStatus { get; set; }
         public decimal EstimatedTotalCost { get; set; }
         public decimal PurchasePrice { get; set; }
         public decimal MRP { get; set; }
         public decimal Amount  {get;set;}
        public long QuotationID { get; set; }    //added on 1-12-2018
        public string WarehouseName { get; set; }   //added on 1-12-2018
        public bool IsReplied { get; set; }  //added on 1-12-2018
        public int TotalItems { get; set; }//added on 1-12-2018
        public int ReplyItemCount { get; set; }  //added on 1-12-2018
        public long QuotationSupplierListID { get; set; }  //added on 1-12-2018
        public long SupplierID { get; set; }//added on 1-12-2018
        public long ProductVarientID { get; set; }//added on 1-12-2018
      
        public DateTime PriceValidTillDate{get;set;}
         public string FromDate { get; set; }
         public string ToDate { get; set; }

     }

     public class QuotationListReportViewModelList
     {
         public List<QuotationListReportViewModel> lQuotationListReportViewModel = new List<QuotationListReportViewModel>();
         public long SupplierID { get; set; }

         public string SupplierName { get; set; }
         public long WarehouseID { get; set; }
         public string WarehouseName { get; set; }
         public string FromDate { get; set; }
         public string ToDate { get; set; }
         public SelectList WarehouseList { get; set; }
     }

        


    }

