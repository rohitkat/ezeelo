using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models.ViewModel
{
    public class CategoryWiseOffersViewModel
    {
        public List<OfferCategoryList> CatList { get; set; }
        public List<OfferProducts> prodList { get; set; }

        public int flag { get; set; }
    }

    public class OfferProducts
    {
        //public int itemID { get; set; }
        //public int offerID { get; set; }
        public String StockThumbPath { get; set; }
        public long ProductID { get; set; }
        public long ShopStockID { get; set; }
        public string ProductName { get; set; }
        public int CategoryID { get; set; }
        public string CategoryName { get; set; }
        public int StockStatus { get; set; }
        public int StockQty { get; set; }
        public decimal MRP { get; set; }
        public decimal SaleRate { get; set; }

        public long OfferID { get; set; }
        public int OfferPercent { get; set; }
        public int OfferRs { get; set; }
        public DateTime OfferStartTime { get; set; }
        public DateTime OfferEndTime { get; set; }

        public string OfferName { get; set; }

        public long OfferPrice { get; set; }


        //For API
        public string StockSmallImagePath { get; set; }
        public string ShortDescription { get; set; }
        public string BrandName { get; set; }
        public string ColorName { get; set; }
        public string SizeName { get; set; }
        public string DimensionName { get; set; }
        public string MaterialName { get; set; }

        public int ShopID { get; set; }
        public string ShopName { get; set; }
        public decimal RetailPoint { get; set; }  //Yashaswi 9-7-2018
        public int OfferType { get; set; } //Sonali 17-11-2018
        public int BrandId { get; set; }//Sonali 23-02-2019
        public int MaterialId { get; set; }//Sonali 23-02-2019
        public int DimensionId { get; set; }//Sonali 23-02-2019
        public int SizeId { get; set; }//Sonali 23-02-2019
        public string PackedUnit { get; set; }//Sonali 23-02-2019
        public decimal CashbackPoints { get; set; }
        public int IsDisplayCB { get; set; }
    }

    public class OfferCategoryList
    {
        public int FirstLevelCatID { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public string FirstLevelCatName { get; set; }

        public string ImagePath { get; set; }

        public int ShopID { get; set; }
    }


}
