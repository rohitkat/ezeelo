using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
namespace ModelLayer.Models.ViewModel
{
     public class NearToExpiryReportViewModel
    {
         public long WarehouseID { get; set; }
         public string Manifecturer { get; set; }
         public string SKUName { get; set; }
         public string HSNCode { get; set; }
         public long SKUID { get; set; }
         public string SKUCode { get; set; }
         public string SKUUnit { get; set; }
         public string SupplierName { get; set; }
         public string BatchCode { get; set; }
         public string InvoiceNO { get; set; }
         public int AvailableQuantity { get; set; }
         public DateTime? ExpiryDate { get; set; }
         public int? DaysLefttoExpire { get; set; }
         public int? LocationID { get; set; }
         public Decimal Amount { get; set; }
         public int? ReorderLevel { get; set; }
         public DateTime PurchaseDate { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime InvoiceDate { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }

       
    }

     public class NearToExpiryReportViewModelList
     {
         public List<NearToExpiryReportViewModel> lNearToExpiryReportViewModel = new List<NearToExpiryReportViewModel>();
         public long SupplierID { get; set; }

         public string SupplierName { get; set; }
         public long WarehouseID { get; set; }
         public string WarehouseName { get; set; }
         public string FromDate { get; set; }
         public string ToDate { get; set; }
         public SelectList WarehouseList { get; set; }
     }
 }

