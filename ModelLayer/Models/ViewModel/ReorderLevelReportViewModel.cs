using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace ModelLayer.Models.ViewModel
{
    public class ReorderLevelReportViewModel
    {

        public string Warehouse { get; set; }
        public long WarehouseID { get; set; }
        public string Manifecturer { get; set; }
        public string Supplier { get; set; }
        public string SKUUnit { get; set; }
        public long SKUID { get; set; }
        public string SKUName { get; set; }
        public int AvailableQuantity { get; set; }
        public int InitialQuantity { get; set; }
        public int ReorderLevel { get; set; }
        public long ProductVarient { get; set; }
        public long SupplierID { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime OrderDate { get; set; }
        public int OutOfStock { get; set; }
        public Double Average { get; set; }
        public Double? RemainingdaysofStock { get; set; }
        public int RemaningQuantity { get; set; }
        public Double Averagepertotal { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public DateTime ReorderLevelHitDate { get; set; } //Added by Rumana on 26-04-2019
        public string BatchCode { get; set; } //Added by Rumana on 30-04-2019
        public int ShopQty { get; set; }


    }
    public class ReorderLevelReportViewModelList
    {
        public List<ReorderLevelReportViewModel> lReorderLevelReportViewModel = new List<ReorderLevelReportViewModel>();
        public long SupplierID { get; set; }

        public string SupplierName { get; set; }
        public long WarehouseID { get; set; }
        public string WarehouseName { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public SelectList WarehouseList { get; set; }
        public List<ReorderLevelReportViewModelOnPlaced> lReorderLevelViewModelOnPlaced = new List<ReorderLevelReportViewModelOnPlaced>();//Added by Rumana
    }
}

