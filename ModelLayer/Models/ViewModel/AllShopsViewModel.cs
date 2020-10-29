using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models.ViewModel
{
    public class AllShopsViewModel
    {
        public long ShopID { get; set; }
        public string ShopName { get; set; }
        public string ShopImageThumbPath { get; set; }
    }

    public class AllShopsCollectionViewModel
    {
        public List<AllShopsViewModel> lAllShopsCollectionViewModel { get; set; }
    }
}
