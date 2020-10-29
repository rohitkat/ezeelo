using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
namespace ModelLayer.Models.ViewModel
{
    public class WastageReportViewModel
    {

        public long WarehouseID { get; set; }
        public long waste_id { get; set; }
        public long id { get; set; }
        public long SKUID { get; set; }
        public long item_id { get; set; } //yashaswi 19/3/2018
        public string item_image_Path { get; set; }
        [Display(Name = "Product Name")]
        public string item_name { get; set; }
        [Display(Name = "Total Product Quantity In Stock")]
        public int total_item_qty_in_stock { get; set; }
        [Display(Name = "Reorder Level")]
        public int reorder_level { get; set; }
        [Display(Name = "Batch Code")]
        public string batch_code { get; set; }
        [Display(Name = "Buy Rate Per Unit")]
        public decimal buy_rate_per_unit { get; set; }
        [Display(Name = "Batch Quantity")]
        public int batch_qty { get; set; }
        [Display(Name = "Batch Available Quantity")]
        public int batch_avl_qty { get; set; }
        [Display(Name = "Expiry Date")]
        public DateTime? expiry_date { get; set; }
        [Display(Name = "Waste Quantity")]
        public int waste_qty { get; set; }
        [Display(Name = "Total Amount")]
        public decimal total_amt { get; set; }
        [Display(Name = "Wastage Reason")]
        public long reason { get; set; }
        [Display(Name = "Sub Reason")]
        public long sub_reason { get; set; }
        [Display(Name = "Item Current Location")]
        public string item_current_location { get; set; }
        [Display(Name = "Do You Want To Relocate")]
        public bool reLocate { get; set; }
        [Display(Name = "New Location")]
        public int new_item_location { get; set; }
        [Display(Name = "Select Image")]
        public string wastage_imgae_path { get; set; }
        public string HSNCode { get; set; }
        public DateTime CreateDate  { get; set; }
        public string Remark { get; set; }

      
        [Required(ErrorMessage = "Please select Supplier name")]
        public long SupplierID { get; set; }

        public int BrandID { get; set; }
        public string ProductVarient { get; set; }

        public decimal WasteAmt { get; set; }

        public string Manufacturer { get; set; }
        public string SupplierName { get; set; }
      
        public string WarehouseName { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
     
        public string SubReason { get; set; }
        public string Reason { get; set; }
    }
      
        public class WastageReportViewModelList
        {
            public List<WastageReportViewModel> lWastageReportViewModel = new List<WastageReportViewModel>();
            public long SupplierID { get; set; }

            public string SupplierName { get; set; }
            public long WarehouseID { get; set; }
            public string WarehouseName { get; set; }
            public string FromDate { get; set; }
            public string ToDate { get; set; }
            public SelectList WarehouseList { get; set; }
        }
}
    

