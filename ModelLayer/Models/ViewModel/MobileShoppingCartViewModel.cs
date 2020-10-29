using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models.ViewModel
{
    public class MobileShoppingCartViewModel
    {
        public String StockThumbPath { get; set; }
        public long ProductID { get; set; }
        public string ProductName { get; set; }
        public long ShopStockID { get; set; }
        public long ShopID { get; set; }
        public string ShopName { get; set; }
        public decimal SaleRate { get; set; }
        public decimal MRP { get; set; }
        public int StockQty { get; set; }
        // public bool StockStatus { get; set; }
        public decimal PackSize { get; set; }

        public string SizeName { get; set; }
        public string MaterialName { get; set; }
        public string DimensionName { get; set; }
        public string ColorName { get; set; }
        public int ActualWeight { get; set; }
        public int VolumetricWeight { get; set; }
        public int CategoryID { get; set; }
        public List<CalulatedTaxesRecord> TaxesOnProduct { get; set; }
        public decimal RetailPoint { get; set; }//Sonali_03-11-2018
        public decimal CashbackPoint { get; set; }
        public long? WareHouseStockId { get; set; }//Sonali_03-11-2018
    }

    public class ShopStockIDs
    {
        public long ssID { get; set; }
    }   

}
