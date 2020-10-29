using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace ModelLayer.Models.ViewModel
{
     public class CategoryProductListReportViewModel
    {
         public string ShopName { get; set; }
         public long WarehouseID { get; set; }
         public int ID { get; set; }
         public string Name { get; set; }
         public long FrenchisesID { get; set; }
         public DateTime CreateDate { get; set; }
         public long ShopStockID { get; set; }
         public long SKUID { get; set; }
         public string SKUUnit { get; set; }
         public string Level1Name { get; set; }
         public string Level2Name { get; set; }
         public string Level3Name { get; set; }
         public int Level1CategoryID { get; set; }
         public int Level2CategoryID { get; set; }
         public int Level3CategoryID { get; set; }
         public long SKU_ID { get; set; }
         public string ProductName { get; set; }
         public string ColorName { get; set; }
         public string MaterialName { get; set; }
         public string DimensionName { get; set; }
         public string SizeName { get; set; }
         public double CostPrice { get; set; }
         public decimal MRP { get; set; }
         public decimal? SaleRate { get; set; }
         public int Qty { get; set; }
         public decimal? wholesalerate { get; set; }
         public long ProductID { get; set; }
      

         public string FromDate { get; set; }
         public string ToDate { get; set; }


    }
     public class CategoryProductListReportViewModelList
     {
         public List<CategoryProductListReportViewModel> lCategoryProductListReportViewModel = new List<CategoryProductListReportViewModel>();
         public long SupplierID { get; set; }
         public string Name { get; set; }
         public long FrenchisesID { get; set; }
         public string SupplierName { get; set; }
         public long WarehouseID { get; set; }
         public string WarehouseName { get; set; }
         public string FromDate { get; set; }
         public string ToDate { get; set; }
         public SelectList WarehouseList { get; set; }
     }
 }
