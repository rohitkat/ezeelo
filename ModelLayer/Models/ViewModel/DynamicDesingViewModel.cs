using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models.ViewModel
{
    public class HomePageDynamicDesingItemsList
    {
        public List<HomePageDynamicDesingItemsViewModel> DynamicDesingItemsList { get; set; }
    }
    public class HomePageDynamicDesingItemsViewModel
    {
        public Int64 ID { get; set; }
        public int FranchiseID { get; set; }
        public long CityID { get; set; }
        //public Int64 DesignBlockTypeID { get; set; }
        public int SequenceOrder { get; set; }
        public string ImageName { get; set; }
        public string LinkUrl { get; set; }
        public string Tooltip { get; set; }
        public bool IsActive { get; set; }
        public Nullable<Int64> ProductID { get; set; }
        public string ProductName { get; set; }
        public Nullable<Int64> ShopStockID { get; set; }
     
        public string ColorName { get; set; }
        public string URLStructureName { get; set; }
        public string Size { get; set; }
        public decimal RetailPoint { get; set; } 
        public int OfferType { get; set; }
        public string DisplayViewApp { get; set; }
        public Nullable<int> CategoryID { get; set; }
    }

    public class DynamicDesingViewModel
    {
        public Int64 ID { get; set; }
        public string SectionHeader { get; set; }
        public string Section { get; set; }
        public Int64 ItemCount { get; set; }
        public bool IsProduct { get; set; }
        public bool IsActive { get; set; }
        public bool IsCategory { get; set; }
        public bool IsBanner { get; set; }
        public string SectionDisplayName { get; set; }
        public int SequenceOrder { get; set; }
    }

    public class DynamicViewModel
    {
        public Int64 ID { get; set; }
        public string SectionHeader { get; set; }
        public decimal ImageWidth { get; set; }
        public decimal ImageHeight { get; set; }
        public int MaxLimit { get; set; }
        public bool IsActive { get; set; }
        public List<HomePageDynamicDesingItemsViewModel> DynamicDesingItemsList { get; set; }
    }
}
