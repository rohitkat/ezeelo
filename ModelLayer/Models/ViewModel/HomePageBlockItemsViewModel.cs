using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models.ViewModel
{
    public class HomePageBlockItemsList
    {
        public List<HomePageBlockItemsViewModel> blockItemsList { get; set; }
    }
    public class HomePageBlockItemsViewModel
    {
        public Int64 ID { get; set; }
        public int FranchiseID { get; set; }
        //public Int64 DesignBlockTypeID { get; set; }
        public int SequenceOrder { get; set; }
        public string ImageName { get; set; }
        public string LinkUrl { get; set; }
        public string Tooltip { get; set; }
        public bool IsActive { get; set; }
        public Nullable<Int64> ProductID { get; set; }
        public string ProductName { get; set; }
        public Nullable<Int64> ShopStockID { get; set; }
        public Nullable<decimal> RetailerRate { get; set; }
        public Nullable<decimal> MRP { get; set; }
        public string ColorName { get; set; }
        public string URLStructureName { get; set; }//Added for URL Structure RULE by AShish
        public string Size { get; set; }//Added for Size on Home page 
        public decimal RetailPoint { get; set; }  //Yashaswi 7-7-2018
        public decimal CashbackPoint { get; set; }
        public int OfferType { get; set; }//Added by Sonali_15-11-2018
        public string DisplayViewApp { get; set; }//For Api Banner replication added by Sonali_16-01-2019
        public Nullable<int> CategoryID { get; set; }//For Api Banner replication added by Sonali_05-12-2018
        public int SectionId { get; set; } //By yashaswi 20-02-2019 For HomePageDynamicSection
        public long HomePageDynamicSectionsId { get; set; }//By yashaswi 20-02-2019 For HomePageDynamicSection
    }

    public class BlockTypeViewModel
    {
        public Int64 ID { get; set; }
        public string Name { get; set; }
        public Int64 ItemCount { get; set; }
    }


    public class BlockViewModel
    {
        public Int64 ID { get; set; }
        public string Name { get; set; }
        public decimal ImageWidth { get; set; }
        public decimal ImageHeight { get; set; }
        public int MaxLimit { get; set; }
        public bool IsActive { get; set; }
        public List<HomePageBlockItemsViewModel> blockItemsList { get; set; }
        public List<HomePageDynamicViewModel> listHomePageDynamic { get; set; }//By yashaswi 20-02-2019 For HomePageDynamicSection
        public List<HomePageDynamicBannerViewModel> listHomePageDynamicBanners { get; set; }//By yashaswi 20-02-2019 For HomePageDynamicSection
        public List<HomePageBlockItemsViewModel> listHomePageDynamicProducts { get; set; }//By yashaswi 20-02-2019 For HomePageDynamicSection
    }

    //Start By yashaswi 20-02-2019 For HomePageDynamicSection
    public class HomePageDynamicViewModel
    {
        public int SectionId { get; set; }
        public long HomePageDynamicSectionsId { get; set; }
        public string SectionDisplayName { get; set; }
        public bool IsBanner { get; set; }
        public bool IsCategory { get; set; }
        public bool IsProduct { get; set; }
        public int SequenceOrder { get; set; }
        public string SectionStyle { get; set; }
    }

    public class HomePageDynamicBannerViewModel
    {
        public int SectionId { get; set; }
        public long HomePageDynamicSectionsId { get; set; }
        public string ImageName { get; set; }
        public int SequenceOrder { get; set; }
        public string ToolTip { get; set; }
        public string LinkURL { get; set; }
        public bool IsBanner { get; set; }
        public DateTime CreateDate { get; set; }
    }
    //ByEnd yashaswi 20-02-2019 For HomePageDynamicSection
}
