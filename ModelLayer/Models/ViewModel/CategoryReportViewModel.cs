using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models.ViewModel
{
    public class CategoryReportViewModel
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public int Level { get; set; }
        public DateTime CreateDate { get; set; }
        public Boolean IsActive { get; set; }
        public long ProductID { get; set; }
        public string ProductName { get; set; }
        public string ShopName { get; set; }
        public string MerchantName { get; set; }

    }
}
