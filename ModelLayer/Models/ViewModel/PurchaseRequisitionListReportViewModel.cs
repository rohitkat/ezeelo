using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;


namespace ModelLayer.Models.ViewModel
{
     public class PurchaseRequisitionListReportViewModel
    {

         public long WarehouseID { get; set; }
         public string QuotationCode { get; set; }
         public int Quantity { get; set; }
         public DateTime CreateDate { get; set; }
          public string ReorderLevelStatus { get; set; }
          public string SKUName { get; set; }
          public string SKUUnit { get; set; }
          public long SKUID { get; set; }
            public string Manufacturer { get; set; }
            public string HSNCode { get; set; }
            public DateTime QuotationDate { get; set; }
            public int NoOfVarient { get; set; }
            public string Supplier { get; set; }
            public string FromDate { get; set; }
            public string ToDate { get; set; }
            public double Amount { get; set; }
        public int TotalItems { get; set; }    // Added by Priti on 4-12-2018
        public bool IsSent { get; set; }    // Added by Priti on 4-12-2018
        public DateTime QuotationRequestDate { get; set; } // Added by Priti on 4-12-2018
        public DateTime ExpectedReplyDate { get; set; }//// Added by Priti on 4-12-2018
    
        public string Category3 { get; set; } // Added by Priti on 4-12-2018
        public string Category2 { get; set; } // Added by Priti on 4-12-2018
        public string Category1 { get; set; }// Added by Priti on 4-12-2018
        public  int QuantityRequired { get; set; }   // Added by Priti on 4-12-2018
        public string Remark { get; set; }  // Added by Priti on 4-12-2018
        public bool IsReplied { get; set; }  //added on 7-12-2018
        public string BatchCode { get; set; }//added on 7-12-2018 
         public Double MRP { get; set; } //added on 7-12-2018 
        public double SalePrice { get; set; }   //added on 7-12-2018 
        public DateTime? ReorderHitDate { get; set; }  //added on 7-12-2018 

        public  int LastPurchaseQuantity { get; set; } // Added by Priti on 4-12-2018
        public DateTime? OutOfStockDate { get; set; } // Added by Priti on 4-12-2018

    }
     public class PurchaseRequisitionListReportViewModelList
     {
         public List< PurchaseRequisitionListReportViewModel> lPurchaseRequisitionListReportViewModel= new List< PurchaseRequisitionListReportViewModel>();
         public long SupplierID { get; set; }

         public string SupplierName { get; set; }
         public long WarehouseID { get; set; }
         public string WarehouseName { get; set; }
         public string FromDate { get; set; }
         public string ToDate { get; set; }
         public SelectList WarehouseList { get; set; }







    }
}
