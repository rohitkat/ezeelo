using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models.ViewModel
{
    public class ShopPriorityList
    {
     public  List<ShopListByPriority> shopListByPriority { get; set; }
    }

    public class ShopListByPriority
    {
        public Int64 Id { get; set; }
        public int Priority { get; set; }
        public string ShopName { get; set; }
        public Int64 ShopID { get; set; }
        public Int64 CityID { get; set; }

        public string CityName { get; set; }
        public bool? Isactive { get; set; }
        public int FranchiseID { get; set; } ////added
    }


}
