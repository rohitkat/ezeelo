using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models.ViewModel
{ 
    public class ShopMenuPriorityList
    {
        public List<ShopMenuPriorityViewModel> shopMenuPriorityList { get; set; }
    }
    public class ShopMenuPriorityViewModel
    {
        public Int64 ID { get; set; }
        public int SequenceOrder { get; set; }
        public string ShopCategoryName { get; set; }
        public int CategoryID { get; set; }
        public Int64 ShopID { get; set; }
        public string CategoryName { get; set; }
        public string ImageName { get; set; }
        public bool IsActive { get; set; }
    }
}
