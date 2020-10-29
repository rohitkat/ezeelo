using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace ModelLayer.Models.ViewModel
{
    //class StockMisMatchReportViewModel
    //{
    //}
    public class StockMisMatchReportViewModel
    {
        public List<StockMisMatchReportViewModelList> lStockMisMatchReportViewModelList = new List<StockMisMatchReportViewModelList>();
        public long SupplierID { get; set; }
        public string SupplierName { get; set; }
        public long WarehouseID { get; set; }
        //public string WarehouseName { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public SelectList WarehouseList { get; set; }
        [NotMapped]
        public bool? IsChecked { get; set; }
        public List<OutOfStocKReportViewModelOnPlaced> lOutofStockReportViewModel = new List<OutOfStocKReportViewModelOnPlaced>();//Added by Rumana

       

    }
    public class StockMisMatchReportViewModelList
    {
        public int? ActualShopQty { get; set; }
        public string InvoiceCode { get; set; }
        public DateTime InvoiceConfirmationDate { get; set; }
        public string SKUName { get; set; }
        public string SKUUnit { get; set; }
        public long SKUID { get; set; }
        public Nullable<decimal> MRP { get; set; }
        public Nullable<decimal> PurchaseRate { get; set; }
        public Nullable<decimal> SaleRate { get; set; }
        public decimal? RetailPoints { get; set; }
        public string ActiveBatch { get; set; }
        public string BatchCode { get; set; }
        public string BrandName { get; set; }
        public long WarehouseID { get; set; }
        public int InitialQty { get; set; }
        public int WarehouseQty { get; set; }
        public int ShopQty { get; set; }
        public int ReservedQty { get; set; }
        public int DeliveredQty { get; set; }
        public int ReturnToDvQty { get; set; }
        public int ReturnFromCustomerQty { get; set; }
        public int CanceledQty { get; set; }
        public int AbandonedQty { get; set; }
        public decimal PortalMRP { get; set; }
        public decimal PortalSaleRate { get; set; }
        public decimal PortalRetailPoint { get; set; }
        public int PendingQty { get; set; }
        public int PlacedQty { get; set; }
        public int ConfirmQty { get; set; }
        public int PackedQty { get; set; }
        public int Dispatched_from_shopQty { get; set; }
        public int In_godownQty { get; set; }
        public int Dispatched_from_godownQty { get; set; }
        public long WarehouseStockId { get; set; }
        public long ShopStockId { get; set; }
        public double EzeeloPurchasePrice { get; set; }

    }
    public class ExporttoExcelStockMisMatchReportViewModelList
    {
        public int SrNo { get; set; }
        public long SKUID { get; set; }
        public string SKUName { get; set; }
        public string SKUUnit { get; set; }
        public string BrandName { get; set; }
        public string BatchCode { get; set; }
        public string InvoiceCode { get; set; }
        public Nullable<decimal> MRP { get; set; }
        public int? ActualShopQty { get; set; }
        public DateTime InvoiceConfirmationDate { get; set; }
        public Nullable<decimal> PurchaseRate { get; set; }
        public Nullable<decimal> SaleRate { get; set; }
        public decimal? RetailPoints { get; set; }
        public string ActiveBatch { get; set; }
        public int InitialQty { get; set; }
        public int WarehouseQty { get; set; }
        public int ShopQty { get; set; }
        public int ReservedQty { get; set; }
        public int DeliveredQty { get; set; }
        public int ReturnToDvQty { get; set; }
        public int ReturnFromCustomerQty { get; set; }
        public int CanceledQty { get; set; }
        public int AbandonedQty { get; set; }
        public decimal PortalMRP { get; set; }
        public decimal PortalSaleRate { get; set; }
        public decimal PortalRetailPoint { get; set; }
        public int PendingQty { get; set; }
        public int PlacedQty { get; set; }
        public int ConfirmQty { get; set; }
        public int PackedQty { get; set; }
        public int Dispatched_from_shopQty { get; set; }
        public int In_godownQty { get; set; }
        public int Dispatched_from_godownQty { get; set; }
        public long WarehouseStockId { get; set; }
        public long ShopStockId { get; set; }
        public double EzeeloPurchasePrice { get; set; }
    }
}
