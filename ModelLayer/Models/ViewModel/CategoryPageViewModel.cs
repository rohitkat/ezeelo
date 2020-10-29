using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models.ViewModel
{
    public class CategoryPageViewModel
    {
        public int ID { get; set; }
        public string CategoryName { get; set; }
        public int? ParentCategoryID { get; set; }
        public int Level { get; set; }
        public int? SequenceOrder { get; set; }
        public List<CategoryPageViewModel> LevelThreeListing { get; set; }
        public List<DynamicProductViewModel> ProductListing { get; set; }
        public bool IsManaged { get; set; }


    }

    public class DynamicProductViewModel
    {
        public long ID { get; set; }
        public string Name { get; set; }
        public int LevelTwoCatID { get; set; }
        public long ShopStockID { get; set; }
        public string ImagePath { get; set; }
        public int? SequenceOrder { get; set; }
        public decimal RetailerRate { get; set; }
        public decimal MRP { get; set; }
        public string Size { get; set; }
        public decimal RetailPoint { get; set; }   //Yashaswi 9-7-2018
        public string URLStructureName { get; set; }//Added for URL Structure RULE by AShish
        public decimal CashbackPoint { get; set; }
        public List<SearchProductDetailsViewModel> productList { get; set; }

    }
}
