using BusinessLogicLayer;
using ModelLayer.Models;
using ModelLayer.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace API.Controllers
{
    public class GetCategoryListController : ApiController
    {
        private EzeeloDBContext db = new EzeeloDBContext();
        public object Get(int cityId, int areaId)
        {
            object obj1 = new object();
            CategoryViewModel model = new CategoryViewModel();
            List<MenuViewModel> levelOneMenu = null;
            try
            {
                if (cityId != null && cityId != 0 && areaId != null && areaId != 0)
                {
                    //var franch = db.FranchiseLocations.FirstOrDefault();
                    int franchiseId = FranchiseID.GetFranchiseID(cityId, areaId).Select(x => x.FranchiseIDs).FirstOrDefault();
                    BusinessLogicLayer.ReadConfig rcKey = new BusinessLogicLayer.ReadConfig(System.Web.HttpContext.Current.Server);
                    BusinessLogicLayer.FranchiseMenuList obj = new BusinessLogicLayer.FranchiseMenuList();
                    DataTable dt = new DataTable();
                    // dt = obj.Select_FranchiseMenu(cityId, System.Web.HttpContext.Current.Server);////hide
                    dt = obj.Select_FranchiseMenu(cityId, franchiseId, System.Web.HttpContext.Current.Server);////added

                    /*Select All Menu By Franchise */
                    List<MenuViewModel> FMenu = new List<MenuViewModel>();
                    FMenu = (from n in dt.AsEnumerable()
                             select new MenuViewModel
                             {
                                 ID = n.Field<Int32>("ID"),
                                 CategoryName = n.Field<string>("CategoryName"),
                                 CategoryRouteName = n.Field<string>("CategoryRouteName"),
                                 ImagePath = rcKey.CATEGORY_IMAGE_HTTP + "/" + n.Field<string>("ImagePath"),
                                 SequenceOrder = n.Field<int?>("SequenceOrder"),
                                 Level = n.Field<int>("Level"),
                                 ParentCategoryID = n.Field<int?>("ParentCategoryID"),
                                 IsManaged = n.Field<bool>("IsManaged")
                             }).OrderBy(x => x.SequenceOrder).ToList();

                    levelOneMenu = FMenu.Where(x => x.Level == 1).OrderBy(x => x.SequenceOrder).ToList();
                    if (levelOneMenu != null && levelOneMenu.Count > 0)
                    {
                        foreach (MenuViewModel M in levelOneMenu)
                        {
                            M.LevelTwoListing = FMenu.Where(x => x.Level == 2 && x.ParentCategoryID == M.ID).ToList();
                            string img = rcKey.LOCALIMG_PATH + "Content/img/" + "Grocery_banner.png";
                            foreach (MenuViewModel M2 in M.LevelTwoListing)
                            {
                                M2.BannerImageList = new List<string>();
                                string Img1 = string.Empty;
                                if (M2.ID == 1684)
                                {
                                    Img1 = rcKey.LOCALIMG_PATH + "Content/img/" + "wheat_rice_and_pulses.png";
                                    M2.BannerImageList.Add(Img1);
                                }
                                else if (M2.ID == 1683)
                                {
                                    Img1 = rcKey.LOCALIMG_PATH + "Content/img/" + "Spices.png";
                                    M2.BannerImageList.Add(Img1);
                                }
                                else if (M2.ID == 1075)
                                {
                                    Img1 = rcKey.LOCALIMG_PATH + "Content/img/" + "Grocery_banner.png";
                                    M2.BannerImageList.Add(Img1);
                                }
                                else if (M2.ID == 1077)
                                {
                                    Img1 = rcKey.LOCALIMG_PATH + "Content/img/" + "Beverages_and_ Syrups.png";
                                    M2.BannerImageList.Add(Img1);
                                }
                                else if (M2.ID == 1670)
                                {
                                    Img1 = rcKey.LOCALIMG_PATH + "Content/img/" + "oil_and_Ghee.png";
                                    M2.BannerImageList.Add(Img1);
                                }
                                else if (M2.ID == 1318)
                                {
                                    Img1 = rcKey.LOCALIMG_PATH + "Content/img/" + "Deo_and_perfume.png";
                                    M2.BannerImageList.Add(Img1);
                                }
                                else if (M2.ID == 1315)
                                {
                                    Img1 = rcKey.LOCALIMG_PATH + "Content/img/" + "Hair_care.png";
                                    M2.BannerImageList.Add(Img1);
                                }
                                else if (M2.ID == 2686)
                                {
                                    Img1 = rcKey.LOCALIMG_PATH + "Content/img/" + "personal_Hygine.png";
                                    M2.BannerImageList.Add(Img1);
                                }
                                else if (M2.ID == 1314)
                                {
                                    Img1 = rcKey.LOCALIMG_PATH + "Content/img/" + "Bathing_and_Body_care.png";
                                    M2.BannerImageList.Add(Img1);
                                }
                                else if (M2.ID == 1319)
                                {
                                    Img1 = rcKey.LOCALIMG_PATH + "Content/img/" + "personal-care-banner.png";
                                    M2.BannerImageList.Add(Img1);

                                }
                                else if (M2.ID == 1316)
                                {
                                    Img1 = rcKey.LOCALIMG_PATH + "Content/img/" + "Oral_Care.png";
                                    M2.BannerImageList.Add(Img1);
                                }
                                else if (M2.ID == 1320)
                                {
                                    Img1 = rcKey.LOCALIMG_PATH + "Content/img/" + "Makeup.png";
                                    M2.BannerImageList.Add(Img1);
                                }
                                else if (M2.ID == 1070)
                                {
                                    Img1 = rcKey.LOCALIMG_PATH + "Content/img/" + "dry.png";
                                    M2.BannerImageList.Add(Img1);
                                }
                                else if (M2.ID == 1073)
                                {
                                    Img1 = rcKey.LOCALIMG_PATH + "Content/img/" + "Bottels_can_and_packets.png";
                                    M2.BannerImageList.Add(Img1);
                                }
                                else if (M2.ID == 1072)
                                {
                                    Img1 = rcKey.LOCALIMG_PATH + "Content/img/" + "Bakery.png";
                                    M2.BannerImageList.Add(Img1);
                                }
                                else if (M2.ID == 1078)
                                {
                                    Img1 = rcKey.LOCALIMG_PATH + "Content/img/" + "Dairy_products.png";
                                    M2.BannerImageList.Add(Img1);
                                }
                                else if (M2.ID == 1310)
                                {
                                    Img1 = rcKey.LOCALIMG_PATH + "Content/img/" + "Household.png";
                                    M2.BannerImageList.Add(Img1);
                                }
                                else if (M2.ID == 1637)
                                {
                                    Img1 = rcKey.LOCALIMG_PATH + "Content/img/" + "BabyCare.png";
                                    M2.BannerImageList.Add(Img1);
                                }
                                else if(M2.ID == 2795)
                                {
                                    Img1 = rcKey.LOCALIMG_PATH + "Content/img/" + "Health_wellnes.png";
                                    M2.BannerImageList.Add(Img1);
                                }
                                else if (M2.ID == 3828)
                                {
                                    Img1 = rcKey.LOCALIMG_PATH + "Content/img/" + "Booster_Plan.png";
                                    M2.BannerImageList.Add(Img1);
                                }
                                else if (M2.ID == 3835)
                                {
                                    Img1 = rcKey.LOCALIMG_PATH + "Content/img/" + "dry.png";
                                    M2.BannerImageList.Add(Img1);
                                }
                                else if (M2.ID == 3833)
                                {
                                    Img1 = rcKey.LOCALIMG_PATH + "Content/img/" + "Grocery_banner.png";
                                    M2.BannerImageList.Add(Img1);
                                }
                                else if (M2.ID == 3839)
                                {
                                    Img1 = rcKey.LOCALIMG_PATH + "Content/img/" + "Household.png";
                                    M2.BannerImageList.Add(Img1);
                                }
                                else if (M2.ID == 3837)
                                {
                                    Img1 = rcKey.LOCALIMG_PATH + "Content/img/" + "Spices.png";
                                    M2.BannerImageList.Add(Img1);
                                }
                                else if (M2.ID == 3830)
                                {
                                    Img1 = rcKey.LOCALIMG_PATH + "Content/img/" + "personal-care-banner.png";
                                    M2.BannerImageList.Add(Img1);
                                }
                                else if (M2.ID == 3859)
                                {
                                    Img1 = rcKey.LOCALIMG_PATH + "Content/img/" + "Grocery_banner.png";
                                    M2.BannerImageList.Add(Img1);
                                }
                                M2.LevelThreeListing = FMenu.Where(x => x.Level == 3 && x.ParentCategoryID == M2.ID).ToList();
                            }
                            if (M.ID == 2)
                            {
                                img = rcKey.LOCALIMG_PATH + "Content/img/" + "Grocery_banner.png";
                            }
                            else if (M.ID == 1313)
                            {
                                img = rcKey.LOCALIMG_PATH + "Content/img/" + "personal-care-banner.png";
                            }
                            else if (M.ID == 2683)
                            {
                                img = rcKey.LOCALIMG_PATH + "Content/img/" + "Branded_food_banner.png";
                            }
                            else if (M.ID == 2684)
                            {
                                img = rcKey.LOCALIMG_PATH + "Content/img/" + "dry.png";
                            }
                            else if (M.ID == 2685)
                            {
                                img = rcKey.LOCALIMG_PATH + "Content/img/" + "Household.png";
                            }
                            else if (M.ID == 1627)
                            {
                                img = rcKey.LOCALIMG_PATH + "Content/img/" + "BabyCare.png";
                            }
                            else if (M.ID == 3827)
                            {
                                img = rcKey.LOCALIMG_PATH + "Content/img/" + "Booster_Plan.png";
                            }
                            M.BannerImageList = new List<string>();
                            M.BannerImageList.Add(img);
                        }
                        model.CategoryList = levelOneMenu;
                    }
                    List<HomePageDynamicSection> objHomePageBynamicList = db.HomePageDynamicSection.Where(x => x.IsActive && x.FranchiseId == franchiseId && x.ShowInApp).ToList();
                    if (objHomePageBynamicList != null && objHomePageBynamicList.Count > 0)
                    {
                        var objCategorySection = (from li in objHomePageBynamicList
                                                  join sec in db.HomePageDynamicSectionsMasters on li.SectionId equals sec.ID
                                                  where sec.Section.Contains("Category Section")
                                                  select new { li.ID, li.IsActive, li.IsBanner, li.IsCategory, li.IsProduct, li.SectionDisplayName, li.SectionId, li.SectionStyle, li.SequenceOrder, li.ShowInApp, li.FranchiseId, li.CreateBy, li.CreateDate, sec.Section, sec.SectionHeader }
                                        ).ToList();
                        if (objCategorySection != null && objCategorySection.Count > 0)
                        {
                            model.SectionList = new List<Section>();
                            List<long> HomePageIds = objCategorySection.Select(x => x.ID).ToList();
                            List<HomePageDynamicSectionBanner> BannerList = db.HomePageDynamicSectionBanner.Where(x => HomePageIds.Contains(x.HomePageDynamicSectionId) && x.IsActive == true && EntityFunctions.TruncateTime(x.StartDate) <= EntityFunctions.TruncateTime(DateTime.Now) && EntityFunctions.TruncateTime(x.EndDate) >= EntityFunctions.TruncateTime(DateTime.Now)).ToList();
                            foreach (var sectionItem in objCategorySection)
                            {
                                Section objSection = new Section();
                                objSection.Name = sectionItem.SectionDisplayName;
                                var MainBannerList = BannerList.Where(x => x.HomePageDynamicSectionId == sectionItem.ID && x.IsBanner == true).ToList();
                                objSection.BannerList = new List<DealBanner>();
                                foreach (var item in MainBannerList)
                                {
                                    DealBanner objDealBanner = new DealBanner();
                                    objDealBanner.DealBannerId = item.ID;
                                    objDealBanner.Image = rcKey.HomePageDynamicSection_IMAGE_HTTP + cityId + "/" + franchiseId + "/" + sectionItem.SectionId + "/" + item.ImageName;
                                    objDealBanner.Type = item.DisplayViewApp;
                                    if (objDealBanner.Type == "1stlvlcategorylist")
                                    {
                                        objDealBanner.Id = item.CategoryID;
                                    }
                                    else if (objDealBanner.Type == "2ndlvlCategorylist")
                                    {
                                        objDealBanner.Id = item.CategoryID;
                                    }
                                    else if (objDealBanner.Type == "productlist" || objDealBanner.Type == "OfferProductList")
                                    {
                                        objDealBanner.Id = item.ID;
                                    }
                                    objSection.BannerList.Add(objDealBanner);
                                }
                                if (objSection.BannerList != null && objSection.BannerList.Count <= 0)
                                {
                                    objSection.BannerList = null;
                                }
                                var SubBannerList = BannerList.Where(x => x.HomePageDynamicSectionId == sectionItem.ID && x.IsBanner == false).ToList();
                                objSection.SubBannerList = new List<DealSubBanner>();
                                foreach (var item in SubBannerList)
                                {
                                    DealSubBanner objDealBanner = new DealSubBanner();
                                    objDealBanner.DealCategoryId = item.ID;
                                    objDealBanner.Image = rcKey.HomePageDynamicSection_IMAGE_HTTP + cityId + "/" + franchiseId + "/" + sectionItem.SectionId + "/" + item.ImageName;
                                    objDealBanner.Type = item.DisplayViewApp;
                                    if (objDealBanner.Type == "1stlvlcategorylist")
                                    {
                                        objDealBanner.Id = item.CategoryID;
                                    }
                                    else if (objDealBanner.Type == "2ndlvlCategorylist")
                                    {
                                        objDealBanner.Id = item.CategoryID;
                                    }
                                    else if (objDealBanner.Type == "productlist" || objDealBanner.Type == "OfferProductList")
                                    {
                                        objDealBanner.Id = item.ID;
                                    }
                                    objSection.SubBannerList.Add(objDealBanner);
                                }
                                if (objSection.SubBannerList != null && objSection.SubBannerList.Count <= 0)
                                {
                                    objSection.SubBannerList = null;
                                }
                                model.SectionList.Add(objSection);
                            }
                        }
                    }

                    //Deals deal = new Deals();
                    //List<Deal> DealList = db.Deals.Where(x => x.IdSectionDisplay && x.IsActive && EntityFunctions.TruncateTime(x.StartDateTime) <= EntityFunctions.TruncateTime(DateTime.Now) && EntityFunctions.TruncateTime(x.EndDateTime) >= EntityFunctions.TruncateTime(DateTime.Now)).Take(10).ToList();
                    //if (DealList != null && DealList.Count > 0)
                    //{
                    //    model.SectionList = new List<Section>();
                    //    foreach (var item in DealList)
                    //    {
                    //        Section objSection = new Section();
                    //        objSection.Name = item.ShortName;
                    //        //objSection.BannerList =
                    //        var bannerlist = deal.GetDealsBanners(franchiseId, item.ID);
                    //        if (bannerlist != null && bannerlist.Count > 0)
                    //        {
                    //            objSection.BannerList = (from bl in bannerlist
                    //                                     select new DealBanner
                    //                                     {
                    //                                         DealBannerId = bl.Id,
                    //                                         Image = rcKey.DealBanner_IMAGE_HTTP + bl.ImageName,
                    //                                         Type = bl.DisplayViewApp,
                    //                                         Id = deal.GetBannerIdByDealBanner(bl)
                    //                                     }).ToList();
                    //        }
                    //        var subBannerList = deal.GetDealsCategory(franchiseId, item.ID);
                    //        if (subBannerList != null && subBannerList.Count > 0)
                    //        {
                    //            objSection.SubBannerList = (from bl in subBannerList
                    //                                        select new DealSubBanner
                    //                                        {
                    //                                            DealCategoryId = bl.Id,
                    //                                            Image = rcKey.DealBanner_IMAGE_HTTP + bl.ImageName,
                    //                                            Type = bl.DisplayViewApp,
                    //                                            Id = deal.GetBannerIdByDealSubBanner(bl)
                    //                                        }).ToList();
                    //        }
                    //        model.SectionList.Add(objSection);
                    //    }
                    //}

                    obj1 = new { Success = 1, Message = "SuccessFull", data = model };
                    //}
                    //else
                    //{
                    //    obj1 = new { Success = 0, Message = "Category list not found.", data = string.Empty };
                    //}

                }
                else
                {
                    obj1 = new { Success = 0, Message = "Please enter valid cityid or areaid", data = string.Empty };
                }
            }
            catch (Exception ex)
            {
                obj1 = new { Success = 0, Message = ex.Message, data = string.Empty };
            }
            return obj1;
        }

        public object GetCategory_ProductList(int cityId, int FranchiseId, long CategoryId)
        {
            object obj = new object();
            try
            {
                if (cityId == null || cityId <= 0 || FranchiseId == null || FranchiseId <= 0 || CategoryId == null || CategoryId <= 0)
                {
                    obj = new { Success = 0, Message = "Enter valid parameter", data = string.Empty };
                }
                List<CategoryProductViewModel> model = new List<CategoryProductViewModel>();
                BusinessLogicLayer.ReadConfig rcKey = new BusinessLogicLayer.ReadConfig(System.Web.HttpContext.Current.Server);
                BusinessLogicLayer.FranchiseMenuList objFranchiseMenu = new BusinessLogicLayer.FranchiseMenuList();
                DataTable dt = new DataTable();
                // dt = obj.Select_FranchiseMenu(cityId, System.Web.HttpContext.Current.Server);////hide
                dt = objFranchiseMenu.Select_FranchiseMenu(cityId, FranchiseId, System.Web.HttpContext.Current.Server);////added

                /*Select All Menu By Franchise */
                List<CategoryProductViewModel> FMenu = new List<CategoryProductViewModel>();
                FMenu = (from n in dt.AsEnumerable()
                         select new CategoryProductViewModel
                         {
                             ID = n.Field<Int32>("ID"),
                             CategoryName = n.Field<string>("CategoryName"),
                             CategoryRouteName = n.Field<string>("CategoryRouteName"),
                             ImagePath = rcKey.CATEGORY_IMAGE_HTTP + "/" + n.Field<string>("ImagePath"),
                             SequenceOrder = n.Field<int?>("SequenceOrder"),
                             Level = n.Field<int>("Level"),
                             ParentCategoryID = n.Field<int?>("ParentCategoryID"),
                             IsManaged = n.Field<bool>("IsManaged")
                         }).OrderBy(x => x.SequenceOrder).ToList();

                model = FMenu.Where(x => x.Level == 2 && x.ID == CategoryId).OrderBy(x => x.SequenceOrder).ToList();
                if (model != null && model.Count > 0)
                {
                    foreach (CategoryProductViewModel M in model)
                    {
                        M.LevelThreeListing = FMenu.Where(x => x.Level == 3 && x.ParentCategoryID == M.ID).ToList();
                        foreach (var item in M.LevelThreeListing)
                        {
                            ProductSearchViewModel productSearch = new ProductSearchViewModel();
                            productSearch.PageIndex = 1;
                            productSearch.Keyword = string.Empty;
                            productSearch.CategoryID = item.ID;
                            productSearch.ProductID = 0;
                            productSearch.BrandIDs = string.Empty;
                            productSearch.ShopID = 0;
                            //productSearch.ShopStockIDList = blockItems.ShopStockID.HasValue ? blockItems.ShopStockID.Value : 0;
                            productSearch.CityID = (int)cityId;
                            productSearch.FranchiseID = FranchiseId;
                            productSearch.PageSize = 50;//Sonali_03-11-2018_For display all product
                            productSearch.Version = 1;
                            ProductWithRefinementViewModel productWithRefinementViewModel = new ProductWithRefinementViewModel();
                            ProductList productList = new ProductList(System.Web.HttpContext.Current.Server);
                            productWithRefinementViewModel = productList.GetProductList(productSearch, false);
                            item.productList = productWithRefinementViewModel.productList;
                        }
                        M.BannerImageList = new List<string>();
                        string Img1 = string.Empty;
                        if (M.ID == 1684)
                        {
                            Img1 = rcKey.LOCALIMG_PATH + "Content/img/" + "wheat_rice_and_pulses.png";
                            M.BannerImageList.Add(Img1);
                        }
                        else if (M.ID == 1683)
                        {
                            Img1 = rcKey.LOCALIMG_PATH + "Content/img/" + "Spices.png";
                            M.BannerImageList.Add(Img1);
                        }
                        else if (M.ID == 1075)
                        {
                            Img1 = rcKey.LOCALIMG_PATH + "Content/img/" + "Grocery_banner.png";
                            M.BannerImageList.Add(Img1);
                        }
                        else if (M.ID == 1077)
                        {
                            Img1 = rcKey.LOCALIMG_PATH + "Content/img/" + "Beverages_and_ Syrups.png";
                            M.BannerImageList.Add(Img1);
                        }
                        else if (M.ID == 1670)
                        {
                            Img1 = rcKey.LOCALIMG_PATH + "Content/img/" + "oil_and_Ghee.png";
                            M.BannerImageList.Add(Img1);
                        }
                        else if (M.ID == 1318)
                        {
                            Img1 = rcKey.LOCALIMG_PATH + "Content/img/" + "Deo_and_perfume.png";
                            M.BannerImageList.Add(Img1);
                        }
                        else if (M.ID == 1315)
                        {
                            Img1 = rcKey.LOCALIMG_PATH + "Content/img/" + "Hair_care.png";
                            M.BannerImageList.Add(Img1);
                        }
                        else if (M.ID == 2686)
                        {
                            Img1 = rcKey.LOCALIMG_PATH + "Content/img/" + "personal_Hygine.png";
                            M.BannerImageList.Add(Img1);
                        }
                        else if (M.ID == 1314)
                        {
                            Img1 = rcKey.LOCALIMG_PATH + "Content/img/" + "Bathing_and_Body_care.png";
                            M.BannerImageList.Add(Img1);
                        }
                        else if (M.ID == 1319)
                        {
                            Img1 = rcKey.LOCALIMG_PATH + "Content/img/" + "personal-care-banner.png";
                            M.BannerImageList.Add(Img1);

                        }
                        else if (M.ID == 1316)
                        {
                            Img1 = rcKey.LOCALIMG_PATH + "Content/img/" + "Oral_Care.png";
                            M.BannerImageList.Add(Img1);
                        }
                        else if (M.ID == 1320)
                        {
                            Img1 = rcKey.LOCALIMG_PATH + "Content/img/" + "Makeup.png";
                            M.BannerImageList.Add(Img1);
                        }
                        else if (M.ID == 1070)
                        {
                            Img1 = rcKey.LOCALIMG_PATH + "Content/img/" + "dry.png";
                            M.BannerImageList.Add(Img1);
                        }
                        else if (M.ID == 1073)
                        {
                            Img1 = rcKey.LOCALIMG_PATH + "Content/img/" + "Bottels_can_and_packets.png";
                            M.BannerImageList.Add(Img1);
                        }
                        else if (M.ID == 1072)
                        {
                            Img1 = rcKey.LOCALIMG_PATH + "Content/img/" + "Bakery.png";
                            M.BannerImageList.Add(Img1);
                        }
                        else if (M.ID == 1078)
                        {
                            Img1 = rcKey.LOCALIMG_PATH + "Content/img/" + "Dairy_products.png";
                            M.BannerImageList.Add(Img1);
                        }
                        else if (M.ID == 1310)
                        {
                            Img1 = rcKey.LOCALIMG_PATH + "Content/img/" + "Household.png";
                            M.BannerImageList.Add(Img1);
                        }
                        else if (M.ID == 1637)
                        {
                            Img1 = rcKey.LOCALIMG_PATH + "Content/img/" + "BabyCare.png";
                            M.BannerImageList.Add(Img1);
                        }
                    }
                    obj = new { Success = 1, Message = "SuccessFull", data = model };
                }
                else
                {
                    obj = new { Success = 0, Message = "Category list not found.", data = string.Empty };
                }

            }
            catch (Exception ex)
            {
                obj = new
                {
                    Success = 0,
                    Message = ex.Message,
                    data = string.Empty
                };
            }
            return obj;
        }

        public class CategoryProductViewModel
        {
            public int ID { get; set; }
            public string CategoryName { get; set; }
            public string CategoryRouteName { get; set; }
            public int? ParentCategoryID { get; set; }
            public int Level { get; set; }
            public string ImagePath { get; set; }
            public int? SequenceOrder { get; set; }
            public List<CategoryProductViewModel> LevelThreeListing { get; set; }
            public bool IsManaged { get; set; }
            public List<string> BannerImageList { get; set; }
            public List<SearchProductDetailsViewModel> productList { get; set; }
        }


        public class CategoryViewModel
        {
            public List<MenuViewModel> CategoryList { get; set; }
            public List<Section> SectionList { get; set; }
            //public Grocery_Staples GroceryStaple { get; set; }
            //public Personal_care PersonalCare { get; set; }
            //public Household HouseHold { get; set; }
            //public BrandedFood BrandedFood { get; set; }
            //public BabyCare BabyCare { get; set; }
        }
        public class DealSubBanner
        {
            public long DealCategoryId { get; set; }
            public string Image { get; set; }
            public long? Id { get; set; }
            public string Type { get; set; }
        }
        public class Section
        {
            public string Name { get; set; }
            public List<DealBanner> BannerList { get; set; }
            public List<DealSubBanner> SubBannerList { get; set; }
        }
    }
}
//public class Grocery_Staples
//{
//    public List<DealBanner> BannerList { get; set; }
//    public List<DealCategory> CategoryList { get; set; }
//}
//public class Personal_care
//{
//    public List<DealBanner> BannerList { get; set; }
//    public List<DealCategory> CategoryList { get; set; }
//}
//public class Household
//{
//    public List<DealBanner> BannerList { get; set; }
//    public List<DealCategory> CategoryList { get; set; }
//}
//public class BrandedFood
//{
//    public List<DealBanner> BannerList { get; set; }
//    public List<DealCategory> CategoryList { get; set; }
//}
//public class BabyCare
//{
//    public List<DealBanner> BannerList { get; set; }
//    public List<DealCategory> CategoryList { get; set; }
//}

