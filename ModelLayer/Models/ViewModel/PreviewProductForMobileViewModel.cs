using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models.ViewModel
{
    public class PreviewProductForMobileViewModel
    {
        public Int64 ShopStockID { get; set; }
        public Int64 ShopID { get; set; }
        public string ShopName { get; set; }
        public Int64 ProductID { get; set; }
        public string ProductName { get; set; }
        public int CategoryID { get; set; }
        public string CategoryName { get; set; }
        public int SizeID { get; set; }
        public string SizeName { get; set; }
        public int ColorID { get; set; }
        public string ColorName { get; set; }
        public int MaterialID { get; set; }
        public string MaterialName { get; set; }
        public int ReorderLevel { get; set; }
        public bool StockStatus { get; set; }
        public int PackUnitID { get; set; }
        public string UnitName { get; set; }
        public decimal PackSize { get; set; }
        public decimal MRP { get; set; }
        public decimal RetailerRate { get; set; }
        public int StockQty { get; set; }
        public bool IsInclusiveOfTax { get; set; }

        public string StockSmallPath { get; set; }
        public decimal BusinessPoint { get; set; }
        public decimal CashbackPoint { get; set; }
        public int IsDisplayCB { get; set; }
        /*Added By Pradnyakar Badge
         * 31-03-2016
         * For Taxtion Detail On Product Purchase at Product Preview Page
        */
        public List<CalulatedTaxesRecord> TaxesOnProduct { get; set; }
    }

        /*Added By Pradnyakar Badge
         * 13-06-2016
         * For New Product List in Mobile APP
         */
    public class ProductListVarientForMobileViewModel
    {
        public Int64 ShopStockID { get; set; }
        public int SizeID { get; set; }
        public string SizeName { get; set; }
        public int ColorID { get; set; }
        public string ColorName { get; set; }
        public decimal MRP { get; set; }
        public decimal RetailerRate { get; set; }
        public bool StockStatus { get; set; }
        public bool IsInclusiveOfTax { get; set; }
        public int StockQty { get; set; }
        public bool isWishList { get; set; }
    }
}
