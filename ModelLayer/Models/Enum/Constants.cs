using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models.Enum
{
    public class Margin_DivisionConstants
    {
        //Yashaswi 17/4/2018
        public enum Margin_Division
        {
            LEADERSHIP = 1,
            EZEELO = 2,
            LEADERS_ROYALTY = 3,
            LIFESTYLE_FUND = 4,
            LEADERSHIP_DEVELOPMENT_FUND = 5
        }

        public enum Select_Style
        {
            LOGO_SLIDER = 1,
            MAIN_BANNER = 2,
            BOTTOM_BANNER = 3,
            LEFT_SQUARE_MAIN_BANNER_AND_RIGHT_FOUR_SUBCAT_BANNERS = 4,
            FULL_WIDTH_MAIN_BANNER_AND_BELOW_THREE_SUBCAT_BANNERS = 5,
            TWO_FIRST_ROW_TWO_SECOND_ROW = 6,
            FOUR_SUBCAT_BANNERS = 7,
            FULL_WIDTH_MAIN_BANNER_AND_PRODUCT_BELOW = 8
        }

        public enum HomePageDynamicOfferList
        {
            // lvl1categorylist = 3,
            // lvl2Categorylist = 4,
            OfferProductList = 1,
            //BannerList = 4,
            //LeaderDashboard = 5,
            //LeaderReport = 6,
            //Wishlist = 7,
            Homepage = 2,

        }

        public int GetMarginDivision(int Code)
        {
            EzeeloDBContext db = new EzeeloDBContext();
            MarginDivision obj = db.MarginDivision.FirstOrDefault(m => m.Code == Code);
            if (obj != null)
            {
                return obj.MarginInPercentage;
            }
            return 0;
        }
        //21/6/2018
        public string GetMarginDivisionName(int Code)
        {
            EzeeloDBContext db = new EzeeloDBContext();
            MarginDivision obj = db.MarginDivision.FirstOrDefault(m => m.Code == Code);
            if (obj != null)
            {
                return obj.MarginFor;
            }
            return "";
        }
        public int RoundOfBP(double BP)
        {
            return (int)BP;
        }

        public int BPInPaise()
        {
            return 10;
        }
        public enum Select_StyleMobile
        {
            MAIN_BANNER = 1,
            FULL_WIDTH_MAIN_BANNER_AND_BELOW_TWO_SUBCAT_BANNERS = 2,
            FULL_WIDTH_MAIN_BANNER_AND_PRODUCT_BELOW = 3
        }
    }

    //Yashaswi 10/5/2018
    public class Reason_ParentCategory
    {
        public int ID { get; set; }
        public string Category { get; set; }
        public bool IsChecked { get; set; }
        public enum Reason_Parent
        {
            RETURN = 1,
            WASTAGE = 2
        }
        public List<Reason_ParentCategory> GetListFromEnum()
        {
            List<Reason_ParentCategory> list = new List<Reason_ParentCategory>();
            string[] names = System.Enum.GetNames(typeof(Reason_Parent));
            foreach (string item in names)
            {
                Reason_ParentCategory obj = new Reason_ParentCategory();
                obj.Category = item;
                switch (item)
                {
                    case "RETURN":
                        obj.ID = (int)Reason_Parent.RETURN;
                        break;
                    case "WASTAGE":
                        obj.ID = (int)Reason_Parent.WASTAGE;
                        break;
                }
                obj.IsChecked = false;
                list.Add(obj);
            }

            return list;
        }
    }
    //Added by Sonali_15-11-2018
    public class Offer_Product
    {
        public enum OfferType
        {
            None = 1,
            Percentage = 2,
            Amount = 3
        }
    }
}
