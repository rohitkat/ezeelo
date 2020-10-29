using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models.ViewModel
{
    public class ProductVarientViewModel
    {
       
        public long ShopID { get; set; }
        public long ProductID { get; set; }

        public string ProductName { get; set; }
        public long ProductVarientID { get; set; }
        public int ColorID { get; set; }
        public string ColorName { get; set; }
        public int SizeID { get; set; }
        public string SizeName { get; set; }
        public int DimensionID { get; set; }
        public string DimensionName { get; set; }
        public int MaterialID { get; set; }

        public long ShopStockID { get; set; }
        public string MaterialName { get; set; }
        public decimal Total { get; set; }
        public decimal MRP { get; set; }
        public decimal TotalSaleRate { get; set; }
        public string ShopImage { get; set; }
        public List<ShopProductComponentViewModel> shopProductComponentList = new List<ShopProductComponentViewModel>();
    }
}
