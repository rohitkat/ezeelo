using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ModelLayer.Models.ViewModel
{
   

    public class BestDealsViewModel
    {
        //public int itemID { get; set; }
        //public int offerID { get; set; }
        //from default folder
        public String ProductThumbPath { get; set; }
        public long ProductID { get; set; }
        public long ShopStockID { get; set; }
        public string Name { get; set; }
        public int CategoryID { get; set; }
        public string CategoryName { get; set; }
        public int StockStatus { get; set; }
        public decimal MRP { get; set; }
        public decimal SaleRate { get; set; }

    }
    public class BestDealProductCollection
    {

        public List<BestDealsViewModel> bestDealsViewModel { get; set; }
    }
  
    public class BestDealHeadings
    {
        public long OfferID { get; set; }
        //public int componentOfferID { get; set; }
        //public int schemeOfferID { get; set; }
        public string OfferName { get; set; }
        //public string componentOfferName { get; set; }
        //public string schemeOfferName { get; set; }
        public string OfferType { get; set; }
        //public int tableName { get; set; }
    }
    public class BestDealHeadingCollection
    {
        public List<BestDealHeadings> bestDealHeadingsCollection = new List<BestDealHeadings>();
    }
}