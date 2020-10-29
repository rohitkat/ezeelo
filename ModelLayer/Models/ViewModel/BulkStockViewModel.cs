using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace ModelLayer.Models.ViewModel
{
 public class BulkStockViewModel
    {
        public int ExcelRowID { get; set; }
        public String StockThumbPath { get; set; }
        public String StockSmallImagePath { get; set; }
        public long ProductID { get; set; }
        public string ProductName { get; set; }
        public string ShortDescription { get; set; }
        public string ShopName { get; set; }
        public long CategoryID { get; set; }
        public string CategoryName { get; set; }
        public int BrandID { get; set; }
        public string BrandName { get; set; }
        public int ColorID { get; set; }
        public string ColorName { get; set; }
        public string ColorCode { get; set; }
        public int SizeID { get; set; }
        public string SizeName { get; set; }
        public int DimensionID { get; set; }
        public string DimensionName { get; set; }
        public int MaterialID { get; set; }
        public string MaterialName { get; set; }
        public decimal MRP { get; set; }
        public decimal SaleRate { get; set; }
        public decimal? WholeSaleRate { get; set; }
        public int CityID { get; set; }
        public long ShopID { get; set; }
        public long ShopStockID { get; set; }
        public decimal PackSize { get; set; }
        public string PackUnit { get; set; }
        public int StockQty { get; set; }
        public int ReorderLevel { get; set; }
        public bool StockStatus { get; set; }
        public int BulkLogID { get; set; }
        public Nullable<int> ImageCount { get; set; }

        //To send StockWise images
        public UploadFilesViewModel UploadFiles { get; set; }
       
    }
}
