
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ModelLayer.Models.ViewModel
{
    public class RecentlyViewedCollectionViewModel
    {
        public List<RecentlyViewedViewModel> lRecentlyViewedCollectionViewModel { get; set; }
    }
    public class RecentlyViewedViewModel
    {
        public long ItemID { get; set; }
        public long ShopStockID { get; set; }
        public string Name { get; set; }
        public string ImageUrl { get; set; }
        public string URLStructureName { get; set; } //Added for ULR Structure RULE by Ashish

    }
}