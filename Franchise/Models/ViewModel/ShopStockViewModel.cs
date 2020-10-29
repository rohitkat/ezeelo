using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Franchise.Models.ViewModel
{
    public class ShopStockViewModel
    {
        public long ID { get; set; }
        public decimal MRP { get; set; }
        public decimal SaleRate { get; set; }
        public Nullable<decimal> BusinessPointPerUnit { get; set; } //Added by Zubair for MLm on 6-01-2018
        public Nullable<long> WarehouseStockID { get; set; }
    }
}