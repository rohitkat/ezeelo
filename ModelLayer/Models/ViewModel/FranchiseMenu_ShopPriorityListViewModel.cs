using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models.ViewModel
{
    public class FranchiseMenu_ShopPriorityListViewModel
    {
        public string ShopName { get; set; }
        public Nullable<Int64> ShopID { get; set; }
        public string FirstLevel_Name { get; set; }
        public Nullable<Int64> FirstLevel_ID { get; set; }
        public string SecondLevel_Name { get; set; }
        public Nullable<Int64> SecondLevel_ID { get; set; }
        public string ThirdLevel_Name { get; set; }
        public Nullable<Int64> ThirdLevel_ID { get; set; }
        public string FirstLevel_Image { get; set; }
        public Nullable<int> PriorityLevel { get; set; }
        public Nullable<int> ShopPriorityLevel { get; set; }

        public string ShopLogoPath { get; set; }

    }

    public class AllShops
    {
        public List<FranchiseMenu_ShopPriorityListViewModel> CategoryList { get; set; }

        public List<FranchiseMenu_ShopPriorityListViewModel> ShopList { get; set; }
    }
}
