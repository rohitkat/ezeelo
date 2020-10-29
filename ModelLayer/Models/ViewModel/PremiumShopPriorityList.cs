using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models.ViewModel
{
   public class PremiumShopPriorityList
    {
        public List<PremiumShopPriorityViewModel> premiumShopPriorityList { get; set; }
    }
    public class PremiumShopPriorityViewModel
    {
        public Int64 ID { get; set; }
        public int Priority { get; set; }
        public string CategoryName { get; set; }
        public int CategoryID { get; set; }
        public Int64 ShopID { get; set; }
        public string ShopName { get; set; }
        public int  FranchiseID { get; set; }      
        public bool IsActive { get; set; }
    }
}
