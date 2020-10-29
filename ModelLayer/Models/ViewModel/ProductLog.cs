using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models.ViewModel
{
    public class ProductLogViewModel
    {
        public long RowNumber { get; set; }
        public long ID { get; set; }
        public string Product_Name { get; set; }
        public string Category_Name { get; set; }
        public decimal WholeSaleRate { get; set; }
        public decimal MRP { get; set; }
        public decimal SaleRate { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime ModifyDate { get; set; }


    }
    public class ProductLogViewModelList
    {
        public List<ProductLogViewModel> listProductLogViewModel { get; set; }
    }
}
