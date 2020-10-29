using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models.ViewModel
{
   public class MenuViewModel
    {
        public int ID { get; set; }
        public string CategoryName { get; set; }
        public string CategoryRouteName { get; set; }
        public int? ParentCategoryID { get; set; }
        public int Level { get; set; }
        public string ImagePath { get; set; }
        public int? SequenceOrder { get; set; }
        public List<MenuViewModel> LevelTwoListing { get; set; }
        public List<MenuViewModel> LevelThreeListing { get; set; }
        public bool IsManaged { get; set; }
        public List<string> BannerImageList { get; set; }
    }
    public class menu
    {
        public List<MenuViewModel> levelOneMenu { get; set; }
        public List<MenuViewModel> LevelTwoListing { get; set; }
        public List<MenuViewModel> LevelThreeListing { get; set; }
        public List<BrandViewModel> BrandList { get; set; }
        public List<ShopViewModellist> ShopList { get; set; }
        public List<OfferIDList> offerLists { get; set; }
    }
    public class BrandViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
    public class ShopViewModellist
    {
        public long Id { get; set; }
        public string Name { get; set; }
    }
    public class OfferIDList
    {
        public long Id { get; set; }
        public string Name { get; set; }
    }
}
