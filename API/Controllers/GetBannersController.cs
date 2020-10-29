using BusinessLogicLayer;
using ModelLayer.Models;
using ModelLayer.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity.Core.Objects;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace API.Controllers
{
    public class GetBannersController : ApiController
    {

        EzeeloDBContext db = new EzeeloDBContext();

        public object Get(int areaid, int cityId)
        {
            object obj = new object();
            HomePage homePage = new HomePage();
            Deals deal = new Deals();
            try
            {
                if (areaid == null || areaid <= 0 || cityId == null || cityId <= 0)
                {
                    return obj = new { Success = 0, Message = "Invalid Parameter.", data = string.Empty };
                }
                int franchiseId = FranchiseID.GetFranchiseID(cityId, areaid).Select(x => x.FranchiseIDs).FirstOrDefault();
                BusinessLogicLayer.ReadConfig rcKey = new BusinessLogicLayer.ReadConfig(System.Web.HttpContext.Current.Server);
                List<HomePageDynamicSection> objHomePageBynamicList = db.HomePageDynamicSection.Where(x => x.IsActive && x.FranchiseId == franchiseId && x.ShowInApp).ToList();
                if (objHomePageBynamicList != null && objHomePageBynamicList.Count > 0)
                {
                    /*Started Main Banner*/
                    homePage.BannerList = new List<BannerImage>();
                    var objHomePageSection = (from li in objHomePageBynamicList
                                              join sec in db.HomePageDynamicSectionsMasters on li.SectionId equals sec.ID
                                              where sec.SectionID == 1
                                              select new { li.ID, li.IsActive, li.IsBanner, li.IsCategory, li.IsProduct, li.SectionDisplayName, li.SectionId, li.SectionStyle, li.SequenceOrder, li.ShowInApp, li.FranchiseId, li.CreateBy, li.CreateDate }
                                             ).ToList();
                    if (objHomePageSection != null && objHomePageSection.Count() > 0)
                    {
                        List<long> Ids = objHomePageSection.Where(y => y.IsBanner).Select(y => y.ID).ToList();
                        List<HomePageDynamicSectionBanner> objBannerList = db.HomePageDynamicSectionBanner.Where(x => Ids.Contains(x.HomePageDynamicSectionId) && x.IsActive == true && x.IsBanner == true && EntityFunctions.TruncateTime(x.StartDate) <= EntityFunctions.TruncateTime(DateTime.Now) && EntityFunctions.TruncateTime(x.EndDate) >= EntityFunctions.TruncateTime(DateTime.Now)).OrderBy(x => x.SequenceOrder).ToList();
                        foreach (var item in objBannerList)
                        {
                            BannerImage objMainBanner = new BannerImage();
                            objMainBanner.BlockItemId = item.ID;
                            objMainBanner.Type = item.DisplayViewApp;
                            objMainBanner.Image = rcKey.HomePageDynamicSection_IMAGE_HTTP + cityId + "/" + franchiseId + "/" + objHomePageSection.FirstOrDefault().SectionId + "/" + item.ImageName;
                            if (objMainBanner.Type == "1stlvlcategorylist")
                            {
                                objMainBanner.Id = item.CategoryID;
                            }
                            else if (objMainBanner.Type == "2ndlvlCategorylist")
                            {
                                objMainBanner.Id = item.CategoryID;
                            }
                            else if (objMainBanner.Type == "productlist" || objMainBanner.Type == "OfferProductList")
                            {
                                objMainBanner.Id = item.ID;
                            }
                            homePage.BannerList.Add(objMainBanner);
                        }
                        if (homePage.BannerList != null && homePage.BannerList.Count == 0)
                        {
                            homePage.BannerList = null;
                        }
                    }
                    /*Ended Main Banner*/

                    /*Started Season special Banner*/
                    homePage.SeasonSpecial = new SeasonSpecial();
                    homePage.SeasonSpecial.BannerList = new List<DealBanner>();
                    var objSpecialSection = (from li in objHomePageBynamicList
                                             join sec in db.HomePageDynamicSectionsMasters on li.SectionId equals sec.ID
                                             where sec.SectionID == 5
                                             select new { li.ID, li.IsActive, li.IsBanner, li.IsCategory, li.IsProduct, li.SectionDisplayName, li.SectionId, li.SectionStyle, li.SequenceOrder, li.ShowInApp, li.FranchiseId, li.CreateBy, li.CreateDate }
                                             ).ToList();
                    if (objSpecialSection != null && objSpecialSection.Count > 0)
                    {
                        homePage.SeasonSpecial.BannerList = new List<DealBanner>();
                        List<long> SeasonIds = objSpecialSection.Where(y => y.IsBanner).Select(y => y.ID).ToList();
                        List<HomePageDynamicSectionBanner> objBannerList = db.HomePageDynamicSectionBanner.Where(x => SeasonIds.Contains(x.HomePageDynamicSectionId) && x.IsActive == true && x.IsBanner == true && EntityFunctions.TruncateTime(x.StartDate) <= EntityFunctions.TruncateTime(DateTime.Now) && EntityFunctions.TruncateTime(x.EndDate) >= EntityFunctions.TruncateTime(DateTime.Now)).OrderBy(x => x.SequenceOrder).ToList();
                        foreach (var item in objBannerList)
                        {
                            DealBanner objDealBanner = new DealBanner();
                            objDealBanner.DealBannerId = item.ID;
                            objDealBanner.Type = item.DisplayViewApp;
                            objDealBanner.Image = rcKey.HomePageDynamicSection_IMAGE_HTTP + cityId + "/" + franchiseId + "/" + objSpecialSection.FirstOrDefault().SectionId + "/" + item.ImageName;
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
                            homePage.SeasonSpecial.BannerList.Add(objDealBanner);
                        }
                        if (homePage.SeasonSpecial.BannerList != null && homePage.SeasonSpecial.BannerList.Count == 0)
                        {
                            homePage.SeasonSpecial.BannerList = null;
                        }

                    }
                    /*Ended Season special Banner*/

                    /*Started Hot deals Banner*/
                    homePage.HotDeals = new HotDeals();

                    var objHotDealSection = (from li in objHomePageBynamicList
                                             join sec in db.HomePageDynamicSectionsMasters on li.SectionId equals sec.ID
                                             where sec.SectionID == 6
                                             select new { li.ID, li.IsActive, li.IsBanner, li.IsCategory, li.IsProduct, li.SectionDisplayName, li.SectionId, li.SectionStyle, li.SequenceOrder, li.ShowInApp, li.FranchiseId, li.CreateBy, li.CreateDate }
                                             ).ToList();
                    if (objHotDealSection != null && objHotDealSection.Count > 0)
                    {
                        homePage.HotDeals.BannerList = new List<DealBanner>();
                        List<long> HotDealIds = objHotDealSection.Where(y => y.IsBanner).Select(y => y.ID).ToList();
                        List<HomePageDynamicSectionBanner> objBannerList = db.HomePageDynamicSectionBanner.Where(x => HotDealIds.Contains(x.HomePageDynamicSectionId) && x.IsActive == true && x.IsBanner == true && EntityFunctions.TruncateTime(x.StartDate) <= EntityFunctions.TruncateTime(DateTime.Now) && EntityFunctions.TruncateTime(x.EndDate) >= EntityFunctions.TruncateTime(DateTime.Now)).OrderBy(x => x.SequenceOrder).ToList();
                        foreach (var item in objBannerList)
                        {
                            DealBanner objDealBanner = new DealBanner();
                            objDealBanner.DealBannerId = item.ID;
                            objDealBanner.Type = item.DisplayViewApp;
                            objDealBanner.Image = rcKey.HomePageDynamicSection_IMAGE_HTTP + cityId + "/" + franchiseId + "/" + objHotDealSection.FirstOrDefault().SectionId + "/" + item.ImageName;
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
                            homePage.HotDeals.BannerList.Add(objDealBanner);
                        }
                        if (homePage.HotDeals.BannerList != null && homePage.HotDeals.BannerList.Count == 0)
                        {
                            homePage.HotDeals.BannerList = null;
                        }
                    }
                    if (objHotDealSection != null && objHotDealSection.Count > 0)
                    {
                        homePage.HotDeals.ProductList = new List<OfferProducts>();
                        foreach (var item in objHotDealSection.Where(x => x.IsProduct))
                        {
                            List<OfferProducts> ProductList = deal.GetHomeProductList(cityId, OfferStatus.AVAILABLEDEALS, 0, 1, 12, item.ID, franchiseId);////added FranchiseID for Mutiple MCO
                            if (ProductList != null && ProductList.Count > 0)
                            {
                                foreach (var productitem in ProductList)
                                {
                                    homePage.HotDeals.ProductList.Add(productitem);
                                }
                            }
                        }
                        if (homePage.HotDeals.ProductList != null && homePage.HotDeals.ProductList.Count <= 0)
                        {
                            homePage.HotDeals.ProductList = null;
                        }
                    }

                    /*Ended Hot deals Banner*/

                    /*Started Deals Of Day Banner*/
                    homePage.DealsOfDay = new DealOfDay();

                    var objDealofDaySection = (from li in objHomePageBynamicList
                                               join sec in db.HomePageDynamicSectionsMasters on li.SectionId equals sec.ID
                                               where sec.SectionID == 7
                                               select new { li.ID, li.IsActive, li.IsBanner, li.IsCategory, li.IsProduct, li.SectionDisplayName, li.SectionId, li.SectionStyle, li.SequenceOrder, li.ShowInApp, li.FranchiseId, li.CreateBy, li.CreateDate }
                                             ).ToList();
                    if (objDealofDaySection != null && objDealofDaySection.Count > 0)
                    {
                        homePage.DealsOfDay.BannerList = new List<DealBanner>();
                        List<long> DealOfDayIds = objDealofDaySection.Where(y => y.IsBanner).Select(y => y.ID).ToList();
                        List<HomePageDynamicSectionBanner> objBannerList = db.HomePageDynamicSectionBanner.Where(x => DealOfDayIds.Contains(x.HomePageDynamicSectionId) && x.IsActive == true && x.IsBanner == true && EntityFunctions.TruncateTime(x.StartDate) <= EntityFunctions.TruncateTime(DateTime.Now) && EntityFunctions.TruncateTime(x.EndDate) >= EntityFunctions.TruncateTime(DateTime.Now)).OrderBy(x => x.SequenceOrder).ToList();
                        foreach (var item in objBannerList)
                        {
                            DealBanner objDealBanner = new DealBanner();
                            objDealBanner.DealBannerId = item.ID;
                            objDealBanner.Type = item.DisplayViewApp;
                            objDealBanner.Image = rcKey.HomePageDynamicSection_IMAGE_HTTP + cityId + "/" + franchiseId + "/" + objDealofDaySection.FirstOrDefault().SectionId + "/" + item.ImageName;
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
                            homePage.DealsOfDay.BannerList.Add(objDealBanner);
                        }
                        if (homePage.DealsOfDay.BannerList != null && homePage.DealsOfDay.BannerList.Count == 0)
                        {
                            homePage.DealsOfDay.BannerList = null;
                        }
                    }
                    if (objDealofDaySection != null && objDealofDaySection.Count > 0)
                    {
                        homePage.DealsOfDay.ProductList = new List<OfferProducts>();
                        foreach (var item in objDealofDaySection.Where(x => x.IsProduct))
                        {
                            List<OfferProducts> ProductList = deal.GetHomeProductList(cityId, OfferStatus.AVAILABLEDEALS, 0, 1, 12, item.ID, franchiseId);////added FranchiseID for Mutiple MCO
                            if (ProductList != null && ProductList.Count > 0)
                            {
                                foreach (var productitem in ProductList)
                                {
                                    homePage.DealsOfDay.ProductList.Add(productitem);
                                }
                            }
                        }
                        if (homePage.DealsOfDay.ProductList != null && homePage.DealsOfDay.ProductList.Count <= 0)
                        {
                            homePage.DealsOfDay.ProductList = null;
                        }
                        //  homePage.DealsOfDay.ProductList = deal.GetHomeProductList(cityId, OfferStatus.AVAILABLEDEALS, 0, 1, 12, objDealofDaySection.ID, franchiseId);////added FranchiseID for Mutiple MCO
                    }

                    /*Ended Deals Of Day Banner*/

                    /*Started Major Retail Points Banner*/
                    homePage.MajorRetailpoint = new MajorRetailPoint();

                    var objMajorRetailSection = (from li in objHomePageBynamicList
                                                 join sec in db.HomePageDynamicSectionsMasters on li.SectionId equals sec.ID
                                                 where sec.SectionID == 8
                                                 select new { li.ID, li.IsActive, li.IsBanner, li.IsCategory, li.IsProduct, li.SectionDisplayName, li.SectionId, li.SectionStyle, li.SequenceOrder, li.ShowInApp, li.FranchiseId, li.CreateBy, li.CreateDate }
                                             ).ToList();
                    if (objMajorRetailSection != null && objMajorRetailSection.Count > 0)
                    {
                        homePage.MajorRetailpoint.BannerList = new List<DealBanner>();
                        List<long> MajorIds = objMajorRetailSection.Where(y => y.IsBanner).Select(y => y.ID).ToList();
                        List<HomePageDynamicSectionBanner> objBannerList = db.HomePageDynamicSectionBanner.Where(x => MajorIds.Contains(x.HomePageDynamicSectionId) && x.IsActive == true && x.IsBanner == true && EntityFunctions.TruncateTime(x.StartDate) <= EntityFunctions.TruncateTime(DateTime.Now) && EntityFunctions.TruncateTime(x.EndDate) >= EntityFunctions.TruncateTime(DateTime.Now)).OrderBy(x => x.SequenceOrder).ToList();
                        foreach (var item in objBannerList)
                        {
                            DealBanner objDealBanner = new DealBanner();
                            objDealBanner.DealBannerId = item.ID;
                            objDealBanner.Type = item.DisplayViewApp;
                            objDealBanner.Image = rcKey.HomePageDynamicSection_IMAGE_HTTP + cityId + "/" + franchiseId + "/" + objMajorRetailSection.FirstOrDefault().SectionId + "/" + item.ImageName;
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
                            homePage.MajorRetailpoint.BannerList.Add(objDealBanner);
                        }
                        if (homePage.MajorRetailpoint.BannerList != null && homePage.MajorRetailpoint.BannerList.Count == 0)
                        {
                            homePage.MajorRetailpoint.BannerList = null;
                        }
                    }
                    if (objMajorRetailSection != null && objMajorRetailSection.Count > 0)
                    {
                        homePage.MajorRetailpoint.ProductList = new List<OfferProducts>();
                        foreach (var item in objMajorRetailSection.Where(x => x.IsProduct))
                        {
                            List<OfferProducts> ProductList = deal.GetHomeProductList(cityId, OfferStatus.AVAILABLEDEALS, 0, 1, 12, item.ID, franchiseId);////added FranchiseID for Mutiple MCO
                            if (ProductList != null && ProductList.Count > 0)
                            {
                                foreach (var productitem in ProductList)
                                {
                                    homePage.MajorRetailpoint.ProductList.Add(productitem);
                                }
                            }
                        }
                        if (homePage.MajorRetailpoint.ProductList != null && homePage.MajorRetailpoint.ProductList.Count <= 0)
                        {
                            homePage.MajorRetailpoint.ProductList = null;
                        }
                        //homePage.MajorRetailpoint.ProductList = deal.GetHomeProductList(cityId, OfferStatus.AVAILABLEDEALS, 0, 1, 12, objMajorRetailSection.ID, franchiseId);////added FranchiseID for Mutiple MCO
                    }

                    /*Ended Major Retail Points Banner*/
                    /*Started 48Hrs Deals Banner*/
                    homePage.Deals_48 = new Deals_48Hrs();

                    var objDeals48Section = (from li in objHomePageBynamicList
                                             join sec in db.HomePageDynamicSectionsMasters on li.SectionId equals sec.ID
                                             where sec.SectionID == 9
                                             select new { li.ID, li.IsActive, li.IsBanner, li.IsCategory, li.IsProduct, li.SectionDisplayName, li.SectionId, li.SectionStyle, li.SequenceOrder, li.ShowInApp, li.FranchiseId, li.CreateBy, li.CreateDate }
                                             ).ToList();
                    if (objDeals48Section != null && objDeals48Section.Count > 0)
                    {
                        homePage.Deals_48.BannerList = new List<DealBanner>();
                        List<long> Deal48Ids = objDeals48Section.Where(y => y.IsBanner).Select(y => y.ID).ToList();
                        List<HomePageDynamicSectionBanner> objBannerList = db.HomePageDynamicSectionBanner.Where(x => Deal48Ids.Contains(x.HomePageDynamicSectionId) && x.IsActive == true && x.IsBanner == true && EntityFunctions.TruncateTime(x.StartDate) <= EntityFunctions.TruncateTime(DateTime.Now) && EntityFunctions.TruncateTime(x.EndDate) >= EntityFunctions.TruncateTime(DateTime.Now)).OrderBy(x => x.SequenceOrder).ToList();
                        foreach (var item in objBannerList)
                        {
                            DealBanner objDealBanner = new DealBanner();
                            objDealBanner.DealBannerId = item.ID;
                            objDealBanner.Type = item.DisplayViewApp;
                            objDealBanner.Image = rcKey.HomePageDynamicSection_IMAGE_HTTP + cityId + "/" + franchiseId + "/" + objDeals48Section.FirstOrDefault().SectionId + "/" + item.ImageName;
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
                            homePage.Deals_48.BannerList.Add(objDealBanner);
                        }
                        if (homePage.Deals_48.BannerList != null && homePage.Deals_48.BannerList.Count == 0)
                        {
                            homePage.Deals_48.BannerList = null;
                        }
                    }
                    if (objDeals48Section != null && objDeals48Section.Count > 0)
                    {
                        homePage.Deals_48.ProductList = new List<OfferProducts>();
                        foreach (var item in objDeals48Section.Where(x => x.IsProduct))
                        {
                            List<OfferProducts> ProductList = deal.GetHomeProductList(cityId, OfferStatus.AVAILABLEDEALS, 0, 1, 12, item.ID, franchiseId);////added FranchiseID for Mutiple MCO
                            if (ProductList != null && ProductList.Count > 0)
                            {
                                foreach (var productitem in ProductList)
                                {
                                    homePage.Deals_48.ProductList.Add(productitem);
                                }
                            }
                        }
                        if (homePage.Deals_48.ProductList != null && homePage.Deals_48.ProductList.Count <= 0)
                        {
                            homePage.Deals_48.ProductList = null;
                        }
                        // homePage.Deals_48.ProductList = deal.GetHomeProductList(cityId, OfferStatus.AVAILABLEDEALS, 0, 1, 12, objDeals48Section.ID, franchiseId);////added FranchiseID for Mutiple MCO
                    }

                    /*Ended 48Hrs Deals Banner*/
                    /*Started Newly launched Banner*/
                    homePage.NewlyLaunch = new NewlyLaunch();

                    var objNewlyLaunchSection = (from li in objHomePageBynamicList
                                                 join sec in db.HomePageDynamicSectionsMasters on li.SectionId equals sec.ID
                                                 where sec.SectionID == 10
                                                 select new { li.ID, li.IsActive, li.IsBanner, li.IsCategory, li.IsProduct, li.SectionDisplayName, li.SectionId, li.SectionStyle, li.SequenceOrder, li.ShowInApp, li.FranchiseId, li.CreateBy, li.CreateDate }
                                             ).ToList();
                    if (objNewlyLaunchSection != null && objNewlyLaunchSection.Count > 0)
                    {
                        homePage.NewlyLaunch.BannerList = new List<DealBanner>();
                        List<long> NewlyLaunchIds = objNewlyLaunchSection.Where(y => y.IsBanner).Select(y => y.ID).ToList();
                        List<HomePageDynamicSectionBanner> objBannerList = db.HomePageDynamicSectionBanner.Where(x => NewlyLaunchIds.Contains(x.HomePageDynamicSectionId) && x.IsActive == true && x.IsBanner == true && EntityFunctions.TruncateTime(x.StartDate) <= EntityFunctions.TruncateTime(DateTime.Now) && EntityFunctions.TruncateTime(x.EndDate) >= EntityFunctions.TruncateTime(DateTime.Now)).OrderBy(x => x.SequenceOrder).ToList();
                        foreach (var item in objBannerList)
                        {
                            DealBanner objDealBanner = new DealBanner();
                            objDealBanner.DealBannerId = item.ID;
                            objDealBanner.Type = item.DisplayViewApp;
                            objDealBanner.Image = rcKey.HomePageDynamicSection_IMAGE_HTTP + cityId + "/" + franchiseId + "/" + objNewlyLaunchSection.FirstOrDefault().SectionId + "/" + item.ImageName;
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
                            homePage.NewlyLaunch.BannerList.Add(objDealBanner);
                        }
                        if (homePage.NewlyLaunch.BannerList != null && homePage.NewlyLaunch.BannerList.Count == 0)
                        {
                            homePage.NewlyLaunch.BannerList = null;
                        }
                    }
                    if (objNewlyLaunchSection != null && objNewlyLaunchSection.Count > 0)
                    {
                        homePage.NewlyLaunch.ProductList = new List<OfferProducts>();
                        foreach (var item in objNewlyLaunchSection.Where(x => x.IsProduct))
                        {
                            List<OfferProducts> ProductList = deal.GetHomeProductList(cityId, OfferStatus.AVAILABLEDEALS, 0, 1, 12, item.ID, franchiseId);////added FranchiseID for Mutiple MCO
                            if (ProductList != null && ProductList.Count > 0)
                            {
                                foreach (var productitem in ProductList)
                                {
                                    homePage.NewlyLaunch.ProductList.Add(productitem);
                                }
                            }
                        }
                        if (homePage.NewlyLaunch.ProductList != null && homePage.NewlyLaunch.ProductList.Count <= 0)
                        {
                            homePage.NewlyLaunch.ProductList = null;
                        }
                        //homePage.NewlyLaunch.ProductList = deal.GetHomeProductList(cityId, OfferStatus.AVAILABLEDEALS, 0, 1, 12, objNewlyLaunchSection.ID, franchiseId);////added FranchiseID for Mutiple MCO
                    }

                    /*Ended Newly launched Banner*/
                    /*Started Trending deal Banner*/
                    homePage.TrendingDeals = new TrendingDeals();

                    var objTrendingDealSection = (from li in objHomePageBynamicList
                                                  join sec in db.HomePageDynamicSectionsMasters on li.SectionId equals sec.ID
                                                  where sec.SectionID == 11
                                                  select new { li.ID, li.IsActive, li.IsBanner, li.IsCategory, li.IsProduct, li.SectionDisplayName, li.SectionId, li.SectionStyle, li.SequenceOrder, li.ShowInApp, li.FranchiseId, li.CreateBy, li.CreateDate }
                                             ).ToList();
                    if (objTrendingDealSection != null && objTrendingDealSection.Count > 0)
                    {
                        homePage.TrendingDeals.BannerList = new List<DealBanner>();
                        List<long> TrendingIds = objTrendingDealSection.Where(y => y.IsBanner).Select(y => y.ID).ToList();
                        List<HomePageDynamicSectionBanner> objBannerList = db.HomePageDynamicSectionBanner.Where(x => TrendingIds.Contains(x.HomePageDynamicSectionId) && x.IsActive == true && x.IsBanner == true && EntityFunctions.TruncateTime(x.StartDate) <= EntityFunctions.TruncateTime(DateTime.Now) && EntityFunctions.TruncateTime(x.EndDate) >= EntityFunctions.TruncateTime(DateTime.Now)).OrderBy(x => x.SequenceOrder).ToList();
                        foreach (var item in objBannerList)
                        {
                            DealBanner objDealBanner = new DealBanner();
                            objDealBanner.DealBannerId = item.ID;
                            objDealBanner.Type = item.DisplayViewApp;
                            objDealBanner.Image = rcKey.HomePageDynamicSection_IMAGE_HTTP + cityId + "/" + franchiseId + "/" + objTrendingDealSection.FirstOrDefault().SectionId + "/" + item.ImageName;
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
                            homePage.TrendingDeals.BannerList.Add(objDealBanner);
                        }
                        if (homePage.TrendingDeals.BannerList != null && homePage.TrendingDeals.BannerList.Count == 0)
                        {
                            homePage.TrendingDeals.BannerList = null;
                        }
                    }
                    if (objTrendingDealSection != null && objTrendingDealSection.Count > 0)
                    {
                        homePage.TrendingDeals.ProductList = new List<OfferProducts>();
                        foreach (var item in objTrendingDealSection.Where(x => x.IsProduct))
                        {
                            List<OfferProducts> ProductList = deal.GetHomeProductList(cityId, OfferStatus.AVAILABLEDEALS, 0, 1, 12, item.ID, franchiseId);////added FranchiseID for Mutiple MCO
                            if (ProductList != null && ProductList.Count > 0)
                            {
                                foreach (var productitem in ProductList)
                                {
                                    homePage.TrendingDeals.ProductList.Add(productitem);
                                }
                            }
                        }
                        if (homePage.TrendingDeals.ProductList != null && homePage.TrendingDeals.ProductList.Count <= 0)
                        {
                            homePage.TrendingDeals.ProductList = null;
                        }
                        // homePage.TrendingDeals.ProductList = deal.GetHomeProductList(cityId, OfferStatus.AVAILABLEDEALS, 0, 1, 12, objTrendingDealSection.ID, franchiseId);////added FranchiseID for Mutiple MCO
                    }
                    /*Ended Trending deal Banner*/
                }
                /*Ended Main Banner*/


                /*Banner Image List*/


                //DataSet ds = BusinessLogicLayer.HomePageBlockItemsList.GetHomeIndexItemList(franchiseId, System.Web.HttpContext.Current.Server);////added

                ///*Select All Menu By Franchise */
                //DataTable dt = new DataTable();
                //dt = ds.Tables[0];
                //List<BlockViewModel> lBlockTypes = new List<BlockViewModel>();
                //lBlockTypes = (from n in dt.AsEnumerable()
                //               select new BlockViewModel
                //               {
                //                   ID = n.Field<Int64>("ID"),
                //                   Name = n.Field<string>("Name"),
                //                   ImageWidth = n.Field<decimal>("ImageWidth"),
                //                   ImageHeight = n.Field<decimal>("ImageHeight"),
                //                   MaxLimit = n.Field<int>("MaxLimit"),
                //                   IsActive = n.Field<bool>("IsActive")
                //               }).OrderBy(x => x.Name).ToList();


                //DataTable dt1 = new DataTable();
                //dt1 = ds.Tables[1];

                //DataView dv = new DataView(dt1);
                //if (lBlockTypes != null && lBlockTypes.Count > 0)
                //{
                //    foreach (BlockViewModel B in lBlockTypes)
                //    {
                //        //int max = 0;
                //        //int.TryParse(Convert.ToString(B.MaxLimit), out max);

                //        dv.RowFilter = "DesignBlockTypeID=" + B.ID;
                //        // var test = dv.ToTable().AsEnumerable();
                //        B.blockItemsList = (from n in dv.ToTable().AsEnumerable()
                //                            select new HomePageBlockItemsViewModel
                //                            {
                //                                ID = n.Field<Int64>("ID"),
                //                                ImageName = rcKey.HOME_IMAGE_HTTP + n.Field<string>("ImageName"),
                //                                LinkUrl = n.Field<string>("LinkUrl"),
                //                                Tooltip = n.Field<string>("Tooltip"),
                //                                SequenceOrder = n.Field<int>("SequenceOrder"),
                //                                IsActive = n.Field<bool>("IsActive"),
                //                                DisplayViewApp = n.Field<string>("DisplayViewApp"), //Added for App banner replication by Sonali on 16-01-2019
                //                                CategoryID = n.Field<int?>("CategoryID")
                //                            }).OrderBy(x => x.SequenceOrder).ToList();
                //    }
                //}

                //if (ds.Tables[2].Rows.Count > 0)
                //{

                //    DataTable dt2 = new DataTable();
                //    dt2 = ds.Tables[2];
                //    lBlockTypes.Where(x => x.Name.ToLower() == "product gallery").FirstOrDefault().blockItemsList = (from n in dt2.AsEnumerable()
                //                                                                                                     select new HomePageBlockItemsViewModel
                //                                                                                                     {
                //                                                                                                         ID = n.Field<Int64>("ID"),
                //                                                                                                         LinkUrl = n.Field<string>("LinkUrl"),
                //                                                                                                         Tooltip = n.Field<string>("Tooltip"),
                //                                                                                                         SequenceOrder = n.Field<int>("SequenceOrder"),
                //                                                                                                         IsActive = n.Field<bool>("IsActive"),
                //                                                                                                         ProductID = n.Field<Int64?>("ProductID"),
                //                                                                                                         // ProductName = n.Field<string>("Name"),
                //                                                                                                         ProductName = n.Field<string>("Name").Replace("+", " "),//Added for SEO URL Structure RULE by AShish
                //                                                                                                         ShopStockID = n.Field<Int64?>("ShopStockID"),
                //                                                                                                         RetailerRate = n.Field<decimal?>("RetailerRate"),
                //                                                                                                         MRP = n.Field<decimal?>("MRP"),
                //                                                                                                         ImageName = rcKey.HOME_IMAGE_HTTP + n.Field<string>("ImageName"),
                //                                                                                                         URLStructureName = GetURLStructureName(n.Field<string>("Name")),//Added for SEO URL Structure RULE by AShish
                //                                                                                                         Size = db.ShopStocks.Find(n.Field<Int64?>("ShopStockID")).ProductVarient.Size.Name, //Added Size on Home page 
                //                                                                                                         RetailPoint = n.Field<decimal>("RetailPoint"),  //Yashaswi 7-7-2018
                //                                                                                                         DisplayViewApp = n.Field<string>("DisplayViewApp") //Added for App banner replication by Sonali on 16-01-2019
                //                                                                                                     }).OrderBy(x => x.SequenceOrder).ToList();
                //}
                //lBlockTypes = lBlockTypes.Where(x => x.Name == "Banner").ToList();
                //homePage.BannerList = new List<BannerImage>();
                //if (lBlockTypes != null && lBlockTypes.Count > 0)
                //{
                //    //homePage.BannerList.BlockItemId = lBlockTypes.Select(x => x.blockItemsList[0].ID).FirstOrDefault();
                //    //homePage.BannerList.Image = lBlockTypes.Select(x => x.blockItemsList[0].ImageName).FirstOrDefault();
                //    //homePage.BannerList.ImageList = new List<string>();
                //    foreach (var item in lBlockTypes)
                //    {
                //        if (item.blockItemsList != null && item.blockItemsList.Count > 0)
                //        {
                //            foreach (var blockitem in item.blockItemsList)
                //            {
                //                BannerImage bannerImg = new BannerImage();
                //                bannerImg.BlockItemId = blockitem.ID;
                //                bannerImg.Image = AddSuffix(blockitem.ImageName, String.Format("{0}", 'm'));
                //                bannerImg.Type = blockitem.DisplayViewApp;
                //                bannerImg.Id = deal.GetBannerIdByBlockItemList(blockitem);
                //                // bannerImg.Image = bannerImg.Image.Replace('/',' ');
                //                //bannerImg.Image = blockitem.ImageName;
                //                // bannerImg.LinkUrl = blockitem.LinkUrl;
                //                homePage.BannerList.Add(bannerImg);
                //                //homePage.BannerList.ImageList.Add(blockitem.ImageName);
                //            }
                //        }
                //    }
                //}

                /*Banner Image List Completed.*/

                ///* Dynamic section banner*/














                ///*Started Season special */
                //SeasonSpecial objSeasonSpecial = new SeasonSpecial();
                ////objSeasonSpecial.IsBanner = true;
                ////objSeasonSpecial.IsCategory = false;
                ////objSeasonSpecial.IsProduct = false;
                //long SeasonDealId = db.Deals.Where(x => x.ShortName == "Season special").Select(x => x.ID).FirstOrDefault();
                ////if (objSeasonSpecial.IsBanner)
                ////{

                //var bannerlist = deal.GetDealsBanners(franchiseId, SeasonDealId);
                //if (bannerlist != null && bannerlist.Count > 0)
                //{
                //    objSeasonSpecial.BannerList = (from bl in bannerlist
                //                                   select new DealBanner
                //                                   {
                //                                       DealBannerId = bl.Id,
                //                                       Image = rcKey.DealBanner_IMAGE_HTTP + bl.ImageName,
                //                                       Type = bl.DisplayViewApp,
                //                                       Id = deal.GetBannerIdByDealBanner(bl)
                //                                   }).ToList();
                //}

                ////}
                ////else if (objSeasonSpecial.IsProduct)
                ////{
                //// objSeasonSpecial.ProductList = deal.GetDealsProducts(cityId, OfferStatus.AVAILABLEDEALS, 0, 1, 12, SeasonDealId, franchiseId);////added FranchiseID for Mutiple MCO
                ////}
                ////else
                ////{

                ////}
                //homePage.SeasonSpecial = objSeasonSpecial;
                ///*End Season special */
                ///*Hot Deals and Offer*/
                //HotDeals hotdeals = new HotDeals();
                //Offers lOffers = new Offers();
                ////hotdeals.IsProduct = true;
                ////hotdeals.IsBanner = false;
                ////hotdeals.IsCategory = false;
                ////if (hotdeals.IsCategory)
                ////{
                ////    List<OfferCategoryList> lCategoryList = new List<OfferCategoryList>();
                ////    lCategoryList = lOffers.GetOfferCategory(cityId, franchiseId);////added franchiseId for Multiple MCO

                ////    if (franchiseId != null)////For New APP
                ////    {
                ////        if (ConfigurationManager.AppSettings["GB_DEALS_" + cityId + "_" + franchiseId] != null) ////added + "_" + franchiseId
                ////        {

                ////            //gb banners
                ////            string[] mykey = ConfigurationManager.AppSettings["GB_DEALS_" + cityId + "_" + franchiseId].Split(','); ////added + "_" + franchiseId
                ////            OfferCategoryList lGBDealsViewModel = new OfferCategoryList();
                ////            foreach (var item in mykey)
                ////            {
                ////                if (item != string.Empty)
                ////                {
                ////                    string[] val = item.Split('/');
                ////                    lGBDealsViewModel.FirstLevelCatID = Convert.ToInt32(val[0]);
                ////                    lGBDealsViewModel.ShopID = Convert.ToInt32(val[1]);

                ////                    lGBDealsViewModel.ImagePath = rcKey.GB_DEALS_IMAGE + "/" + cityId + "/" + franchiseId + "/" + Convert.ToInt32(val[0]) + ".png"; ////added  "/" + franchiseId +
                ////                    lCategoryList.Add(lGBDealsViewModel);
                ////                }
                ////            }


                ////        }
                ////    }
                ////    else
                ////    {
                ////        if (ConfigurationManager.AppSettings["GB_DEALS_" + cityId] != null) ////For Old APP
                ////        {

                ////            //gb banners
                ////            string[] mykey = ConfigurationManager.AppSettings["GB_DEALS_" + cityId].Split(',');
                ////            OfferCategoryList lGBDealsViewModel = new OfferCategoryList();
                ////            foreach (var item in mykey)
                ////            {
                ////                if (item != string.Empty)
                ////                {
                ////                    string[] val = item.Split('/');
                ////                    lGBDealsViewModel.FirstLevelCatID = Convert.ToInt32(val[0]);
                ////                    lGBDealsViewModel.ShopID = Convert.ToInt32(val[1]);

                ////                    lGBDealsViewModel.ImagePath = rcKey.GB_DEALS_IMAGE + "/" + cityId + "/" + Convert.ToInt32(val[0]) + ".png";
                ////                    lCategoryList.Add(lGBDealsViewModel);
                ////                }
                ////            }

                ////        }
                ////    }
                ////    hotdeals.CategoryList = lCategoryList;
                ////}
                ////else if (hotdeals.IsProduct)
                ////{
                //List<OfferProducts> lOfferProducts = lOffers.GetOfferProducts(cityId, OfferStatus.AVAILABLEDEALS, 0, 1, 12, franchiseId);////added FranchiseID for Mutiple MCO
                //hotdeals.ProductList = lOfferProducts;
                //long HotDealId = db.Deals.Where(x => x.ShortName == "Hot Deals").Select(x => x.ID).FirstOrDefault();
                //var bannerlist_HotDeals = deal.GetDealsBanners(franchiseId, HotDealId);
                //if (bannerlist_HotDeals != null && bannerlist_HotDeals.Count > 0)
                //{
                //    hotdeals.BannerList = (from bl in bannerlist_HotDeals
                //                           select new DealBanner
                //                           {
                //                               DealBannerId = bl.Id,
                //                               Image = rcKey.DealBanner_IMAGE_HTTP + bl.ImageName,
                //                               Type = bl.DisplayViewApp,
                //                               Id = deal.GetBannerIdByDealBanner(bl)
                //                           }).ToList();
                //}
                ////}
                //homePage.HotDeals = hotdeals;
                ///*Hot Deals and Offer Completed*/

                ///*Deals of Day*/
                //// string folderPath = @"G:\Sonali\Working Code20-8-2018\HH\HH\API\Content\img";
                //DealOfDay dealsOfDay = new DealOfDay();
                ////dealsOfDay.IsBanner = false;
                ////dealsOfDay.IsCategory = false;
                ////dealsOfDay.IsProduct = true;
                ////if (dealsOfDay.IsBanner)
                ////{
                ////dealsOfDay.BannerList = new List<DealBanner>();
                //// DealBanner homepagemodel_deals = new DealBanner();
                ////homepagemodel_deals.Image = rcKey.LOCALIMG_PATH + "Content/img/" + "DealsOfDay.png";
                ////dealsOfDay.BannerList.Add(homepagemodel_deals);
                //long DealId = db.Deals.Where(x => x.ShortName == "Deals of Day").Select(x => x.ID).FirstOrDefault();
                //var bannerlist_DealOfDay = deal.GetDealsBanners(franchiseId, DealId);
                //if (bannerlist_DealOfDay != null && bannerlist_DealOfDay.Count > 0)
                //{
                //    dealsOfDay.BannerList = (from bl in bannerlist_DealOfDay
                //                             select new DealBanner
                //                             {
                //                                 DealBannerId = bl.Id,
                //                                 Image = rcKey.DealBanner_IMAGE_HTTP + bl.ImageName,
                //                                 Type = bl.DisplayViewApp,
                //                                 Id = deal.GetBannerIdByDealBanner(bl)
                //                             }).ToList();
                //}

                ////}
                ////else if (dealsOfDay.IsProduct)
                ////{
                //dealsOfDay.ProductList = deal.GetDealsProducts(cityId, OfferStatus.AVAILABLEDEALS, 0, 1, 12, DealId, franchiseId);////added FranchiseID for Mutiple MCO
                ////}
                //homePage.DealsOfDay = dealsOfDay;
                ///*Deals of Day Completed*/

                ///*Major Retail Point*/
                //homePage.MajorRetailpoint = new MajorRetailPoint();

                ////homePage.MajorRetailpoint.ProductList = new List<SearchProductDetailsViewModel>();
                ////homePage.MajorRetailpoint.ProductList = (from sh in db.Shops
                ////                                         join sp in db.ShopProducts on sh.ID equals sp.ShopID
                ////                                         join ss in db.ShopStocks on sp.ID equals ss.ShopProductID
                ////                                         where sh.FranchiseID == franchiseId && sp.IsActive && sh.IsLive && ss.RetailerRate > 0 && ss.IsActive
                ////                                         select new SearchProductDetailsViewModel
                ////                                         {
                ////                                             //ProductThumbPath = ImageDisplay.SetProductThumbPath(sp.ProductID, "default", string.Empty, ProductUpload.IMAGE_TYPE.Approved),
                ////                                             ProductID = sp.ProductID,
                ////                                             Name = sp.Product.Name,
                ////                                             CategoryID = sp.Product.CategoryID,
                ////                                             CategoryName = sp.Product.Category.Name,
                ////                                             //StockStatus = (Int32)ss.StockStatus,
                ////                                             MRP = ss.MRP,
                ////                                             SaleRate = ss.RetailerRate,
                ////                                             ShopStockID = ss.ID,
                ////                                             Color = null,
                ////                                             PackSize = ss.PackSize,
                ////                                             PackUnit = ss.Unit.Name,
                ////                                             StockQty = ss.Qty,
                ////                                             RetailPoint = ss.BusinessPoints,   //Yashaswi 10-7-
                ////                                             ShopID = (Int32)sp.ShopID
                ////                                         }).OrderByDescending(x => x.RetailPoint).Take(8).ToList();
                ////// homePage.MajorRetailPointList = homePage.MajorRetailPointList.OrderByDescending(x => x.RetailPoint).Take(8).ToList();
                ////foreach (var item in homePage.MajorRetailpoint.ProductList)
                ////{
                ////    item.ProductThumbPath = ImageDisplay.SetProductThumbPath(item.ProductID, "default", string.Empty, ProductUpload.IMAGE_TYPE.Approved);
                ////    item.Name = item.Name.Replace("+", " ");
                ////}




                //homePage.MajorRetailpoint.ProductList = new List<OfferProducts>();
                //homePage.MajorRetailpoint.ProductList = (from sh in db.Shops
                //                                         join sp in db.ShopProducts on sh.ID equals sp.ShopID
                //                                         join ss in db.ShopStocks on sp.ID equals ss.ShopProductID
                //                                         where sh.FranchiseID == franchiseId && sp.IsActive && sh.IsLive && ss.RetailerRate > 0 && ss.IsActive
                //                                         select new OfferProducts
                //                                         {
                //                                             ProductID = sp.ProductID,
                //                                             ShopStockID = ss.ID,
                //                                             ProductName = sp.Product.Name,
                //                                             CategoryID = sp.Product.CategoryID,
                //                                             CategoryName = sp.Product.Category.Name,
                //                                             StockStatus = ss.StockStatus ? 1 : 0,
                //                                             StockQty = ss.Qty,
                //                                             MRP = ss.MRP,
                //                                             SaleRate = ss.RetailerRate,
                //                                             OfferID = 0,
                //                                             OfferPercent = 0,
                //                                             OfferRs = 0,
                //                                             OfferStartTime = DateTime.Now,
                //                                             OfferEndTime = DateTime.Now,
                //                                             OfferName = "MajorRetail Point",
                //                                             OfferPrice = 0,
                //                                             ShortDescription = sp.Product.Description,
                //                                             MaterialName = ss.ProductVarient.Material.Name,
                //                                             ColorName = ss.ProductVarient.Color.Name,
                //                                             SizeName = ss.ProductVarient.Size.Name,
                //                                             DimensionName = ss.ProductVarient.Dimension.Name,
                //                                             ShopID = (Int32)sp.ShopID,
                //                                             ShopName = sh.Name,
                //                                             RetailPoint = ss.BusinessPoints,
                //                                         }).OrderByDescending(x => x.RetailPoint).Take(8).ToList();
                //// homePage.MajorRetailPointList = homePage.MajorRetailPointList.OrderByDescending(x => x.RetailPoint).Take(8).ToList();
                //foreach (var item in homePage.MajorRetailpoint.ProductList)
                //{
                //    item.StockThumbPath = ImageDisplay.SetProductThumbPath(item.ProductID, "default", string.Empty, ProductUpload.IMAGE_TYPE.Approved);
                //    item.ProductName = item.ProductName.Replace("+", " ");
                //}

                //long RPDealId = db.Deals.Where(x => x.ShortName == "Major RetailPoint").Select(x => x.ID).FirstOrDefault();
                //var bannerlist_RP = deal.GetDealsBanners(franchiseId, RPDealId);
                //if (bannerlist_RP != null && bannerlist_RP.Count > 0)
                //{
                //    homePage.MajorRetailpoint.BannerList = (from bl in bannerlist_RP
                //                                            select new DealBanner
                //                                            {
                //                                                DealBannerId = bl.Id,
                //                                                Image = rcKey.DealBanner_IMAGE_HTTP + bl.ImageName,
                //                                                Type = bl.DisplayViewApp,
                //                                                Id = deal.GetBannerIdByDealBanner(bl)
                //                                            }).ToList();
                //}

                ///*Major Retail Point Completed*/

                ///*48 hrs deals*/
                //// string folderPath = @"G:\Sonali\Working Code20-8-2018\HH\HH\API\Content\img";
                //Deals_48Hrs deals_48 = new Deals_48Hrs();
                //long Deal48Id = db.Deals.Where(x => x.ShortName == "24Hr deal").Select(x => x.ID).FirstOrDefault();
                //deals_48.ProductList = deal.GetDealsProducts(cityId, OfferStatus.AVAILABLEDEALS, 0, 1, 12, Deal48Id, franchiseId);////added FranchiseID for Mutiple MCO
                ////deals_48.IsBanner = false;
                ////deals_48.IsCategory = false;
                ////deals_48.IsProduct = true;
                ////if (deals_48.IsBanner)
                ////{
                //var bannerlist_deals48 = deal.GetDealsBanners(franchiseId, Deal48Id);
                //if (bannerlist_deals48 != null && bannerlist_deals48.Count > 0)
                //{
                //    deals_48.BannerList = (from bl in bannerlist_deals48
                //                           select new DealBanner
                //                           {
                //                               DealBannerId = bl.Id,
                //                               Image = rcKey.DealBanner_IMAGE_HTTP + bl.ImageName,
                //                               Type = bl.DisplayViewApp,
                //                               Id = deal.GetBannerIdByDealBanner(bl)
                //                           }).ToList();
                //}
                ////deals_48.BannerList = new List<DealBanner>();
                ////DealBanner homepagemodel = new DealBanner();
                ////homepagemodel.Image = rcKey.LOCALIMG_PATH + "Content/img/" + "24_hr_deal_copy.png";
                ////deals_48.BannerList.Add(homepagemodel);
                ////}
                ////else if (deals_48.IsProduct)
                ////{

                ////}
                //homePage.Deals_48 = deals_48;
                ////Path.Combine(folderPath, "24 hr deal copy.png");

                ////  homePage.Deals_48 = Directory.GetFiles("G:/Sonali/Working Code20-8-2018/HH/HH/API/Content/img/24 hr deal copy.png");
                ////+  Server.MapPath("~/content/img")+ Directory.GetFiles(@Content+"\img\24 hr deal copy.png");
                ///*48 hrs dealsCompleted*/





                /*Shop by Brand*/
                homePage.BrandList = new List<ShopByBrand>();
                if (franchiseId == 1052)
                {
                    ShopByBrand brand1 = new ShopByBrand();
                    brand1.ImageName = rcKey.LOCALIMG_PATH + "Content/img/" + "Nivea.png";
                    brand1.BrandId = 966;
                    brand1.Name = "Nivea";
                    homePage.BrandList.Add(brand1);
                    ShopByBrand brand2 = new ShopByBrand();
                    brand2.ImageName = rcKey.LOCALIMG_PATH + "Content/img/" + "Park_Avenue.png";
                    brand2.BrandId = 762;
                    brand2.Name = "PARK AVENUE";
                    homePage.BrandList.Add(brand2);
                    ShopByBrand brand3 = new ShopByBrand();
                    brand3.ImageName = rcKey.LOCALIMG_PATH + "Content/img/" + "Everest.png";
                    brand3.BrandId = 1138;
                    brand3.Name = "EVEREST";
                    homePage.BrandList.Add(brand3);
                    ShopByBrand brand = new ShopByBrand();
                    brand.ImageName = rcKey.LOCALIMG_PATH + "Content/img/" + "himalaya.png";
                    brand.BrandId = 502;
                    brand.Name = "Himalaya";
                    homePage.BrandList.Add(brand);
                }
                else if (franchiseId == 1054 || franchiseId == 1056)
                {
                    ShopByBrand brand1 = new ShopByBrand();
                    brand1.ImageName = rcKey.LOCALIMG_PATH + "Content/img/" + "Patanjali-logo.png";
                    brand1.BrandId = 1132;
                    brand1.Name = "PATANJALI";
                    homePage.BrandList.Add(brand1);
                    ShopByBrand brand2 = new ShopByBrand();
                    brand2.ImageName = rcKey.LOCALIMG_PATH + "Content/img/" + "Nivea.png";
                    brand2.BrandId = 966;
                    brand2.Name = "NIVEA";
                    homePage.BrandList.Add(brand2);
                    ShopByBrand brand3 = new ShopByBrand();
                    brand3.ImageName = rcKey.LOCALIMG_PATH + "Content/img/" + "Typhoo.png";
                    brand3.BrandId = 6860;
                    brand3.Name = "TYPHOO";
                    homePage.BrandList.Add(brand3);
                    ShopByBrand brand = new ShopByBrand();
                    brand.ImageName = rcKey.LOCALIMG_PATH + "Content/img/" + "Park_Avenue.png";
                    brand.BrandId = 762;
                    brand.Name = "PARK AVENUE";
                    homePage.BrandList.Add(brand);
                }
                else if (franchiseId == 1063)
                {
                    ShopByBrand brand1 = new ShopByBrand();
                    brand1.ImageName = rcKey.LOCALIMG_PATH + "Content/img/" + "Patanjali-logo.png";
                    brand1.BrandId = 1132;
                    brand1.Name = "PATANJALI";
                    homePage.BrandList.Add(brand1);
                    ShopByBrand brand2 = new ShopByBrand();
                    brand2.ImageName = rcKey.LOCALIMG_PATH + "Content/img/" + "Nivea.png";
                    brand2.BrandId = 966;
                    brand2.Name = "NIVEA";
                    homePage.BrandList.Add(brand2);
                    ShopByBrand brand = new ShopByBrand();
                    brand.ImageName = rcKey.LOCALIMG_PATH + "Content/img/" + "Park_Avenue.png";
                    brand.BrandId = 762;
                    brand.Name = "PARK AVENUE";
                    homePage.BrandList.Add(brand);
                }
                else
                {
                    ShopByBrand brand1 = new ShopByBrand();
                    brand1.ImageName = rcKey.LOCALIMG_PATH + "Content/img/" + "Patanjali-logo.png";
                    brand1.BrandId = 1132;
                    brand1.Name = "PATANJALI";
                    homePage.BrandList.Add(brand1);
                    ShopByBrand brand2 = new ShopByBrand();
                    brand2.ImageName = rcKey.LOCALIMG_PATH + "Content/img/" + "Nivea.png";
                    brand2.BrandId = 966;
                    brand2.Name = "NIVEA";
                    homePage.BrandList.Add(brand2);
                    ShopByBrand brand = new ShopByBrand();
                    brand.ImageName = rcKey.LOCALIMG_PATH + "Content/img/" + "Park_Avenue.png";
                    brand.BrandId = 762;
                    brand.Name = "PARK AVENUE";
                    homePage.BrandList.Add(brand);
                }
                /*Shop by Brand Completed*/



                ///*Newly Launch*/
                //homePage.NewlyLaunch = new NewlyLaunch();
                ////homePage.NewlyLaunch.IsBanner = false;
                ////homePage.NewlyLaunch.IsCategory = false;
                ////homePage.NewlyLaunch.IsProduct = true;
                ////if (homePage.NewlyLaunch.IsProduct)
                ////{
                //long NewlyLaunchDealID = db.Deals.Where(x => x.ShortName == "Newly Launch").Select(x => x.ID).FirstOrDefault();
                //homePage.NewlyLaunch.ProductList = deal.GetDealsProducts(cityId, OfferStatus.AVAILABLEDEALS, 0, 1, 12, NewlyLaunchDealID, franchiseId);////added FranchiseID for Mutiple MCO
                //var bannerlist_newlyLaunch = deal.GetDealsBanners(franchiseId, NewlyLaunchDealID);
                //if (bannerlist_newlyLaunch != null && bannerlist_newlyLaunch.Count > 0)
                //{
                //    homePage.NewlyLaunch.BannerList = (from bl in bannerlist_newlyLaunch
                //                                       select new DealBanner
                //                                       {
                //                                           DealBannerId = bl.Id,
                //                                           Image = rcKey.DealBanner_IMAGE_HTTP + bl.ImageName,
                //                                           Type = bl.DisplayViewApp,
                //                                           Id = deal.GetBannerIdByDealBanner(bl)
                //                                       }).ToList();
                //}                                                                                                                                      //}
                //                                                                                                                                       //else if (homePage.NewlyLaunch.IsBanner)
                //                                                                                                                                       //{
                //                                                                                                                                       //homePage.NewlyLaunch.BannerList = new List<DealBanner>();
                //                                                                                                                                       //DealBanner blockModel = new DealBanner();
                //                                                                                                                                       //blockModel.Image = rcKey.LOCALIMG_PATH + "Content/img/" + "Newly_Launch(1).png";
                //                                                                                                                                       //homePage.NewlyLaunch.BannerList.Add(blockModel);
                //                                                                                                                                       //}

                ////List<SearchProductDetailsViewModel> productDeatilList = new List<SearchProductDetailsViewModel>();
                ////productDeatilList = (from sh in db.Shops
                ////                     join sp in db.ShopProducts on sh.ID equals sp.ShopID
                ////                     join ss in db.ShopStocks on sp.ID equals ss.ShopProductID
                ////                     where sh.FranchiseID == franchiseId && sp.IsActive && sh.IsLive && ss.RetailerRate > 0 && ss.IsActive
                ////                     select new SearchProductDetailsViewModel
                ////                     {
                ////                         // ProductThumbPath = ImageDisplay.SetProductThumbPath(sp.ProductID, "default", string.Empty, ProductUpload.IMAGE_TYPE.Approved),
                ////                         ProductID = sp.ProductID,
                ////                         Name = sp.Product.Name,
                ////                         CategoryID = sp.Product.CategoryID,
                ////                         CategoryName = sp.Product.Category.Name,
                ////                         //StockStatus = (Int32)ss.StockStatus,
                ////                         MRP = ss.MRP,
                ////                         SaleRate = ss.RetailerRate,
                ////                         ShopStockID = ss.ID,
                ////                         Color = null,
                ////                         PackSize = ss.PackSize,
                ////                         PackUnit = ss.Unit.Name,
                ////                         StockQty = ss.Qty,
                ////                         RetailPoint = ss.BusinessPoints,   //Yashaswi 10-7-
                ////                         ShopID = (Int32)sp.ShopID
                ////                     }).Take(8).ToList();
                ////foreach (var item in productDeatilList)
                ////{
                ////    item.ProductThumbPath = ImageDisplay.SetProductThumbPath(item.ProductID, "default", string.Empty, ProductUpload.IMAGE_TYPE.Approved);
                ////}
                ///*Newly Launch dealsCompleted*/

                ///*Trending Deals*/
                //homePage.TrendingDeals = new TrendingDeals();
                //List<SearchProductDetailsViewModel> productDetailView = new List<SearchProductDetailsViewModel>();
                //long TrandingDealID = db.Deals.Where(x => x.ShortName == "Trending Deal").Select(x => x.ID).FirstOrDefault();
                //List<OfferProducts> ProductList = deal.GetDealsProducts(cityId, OfferStatus.AVAILABLEDEALS, 0, 1, 12, TrandingDealID, franchiseId);////added FranchiseID for Mutiple MCO
                //homePage.TrendingDeals.ProductList = ProductList;
                ////productDetailView = (from pl in ProductList
                ////                     select new SearchProductDetailsViewModel
                ////                     {
                ////                         ProductID = pl.ProductID,
                ////                         Name = pl.ProductName,
                ////                         CategoryID = pl.CategoryID,
                ////                         CategoryName = pl.CategoryName,
                ////                         //StockStatus = (Int32)ss.StockStatus,
                ////                         MRP = pl.MRP,
                ////                         SaleRate = pl.SaleRate,
                ////                         ShopStockID = pl.ShopStockID,
                ////                         Color = null,
                ////                         PackSize = 0,
                ////                         Size = pl.SizeName,
                ////                         Dimension = pl.DimensionName,
                ////                         PackUnit = null,
                ////                         StockQty = pl.StockQty,
                ////                         RetailPoint = pl.RetailPoint,   //Yashaswi 10-7-
                ////                         ShopID = pl.ShopID
                ////                     }).OrderBy(x => x.SaleRate).ToList();
                //var bannerlist_TrandingDeal = deal.GetDealsBanners(franchiseId, TrandingDealID);
                //if (bannerlist_TrandingDeal != null && bannerlist_TrandingDeal.Count > 0)
                //{
                //    homePage.TrendingDeals.BannerList = (from bl in bannerlist_TrandingDeal
                //                                         select new DealBanner
                //                                         {
                //                                             DealBannerId = bl.Id,
                //                                             Image = rcKey.DealBanner_IMAGE_HTTP + bl.ImageName,
                //                                             Type = bl.DisplayViewApp,
                //                                             Id = deal.GetBannerIdByDealBanner(bl)
                //                                         }).ToList();
                //}
                ////productDetailView = (from sh in db.Shops
                ////                     join sp in db.ShopProducts on sh.ID equals sp.ShopID
                ////                     join ss in db.ShopStocks on sp.ID equals ss.ShopProductID
                ////                     where sh.FranchiseID == franchiseId && sp.IsActive && sh.IsLive && ss.RetailerRate > 0 && ss.IsActive
                ////                     select new SearchProductDetailsViewModel
                ////                     {
                ////                         // ProductThumbPath = ImageDisplay.SetProductThumbPath(sp.ProductID, "default", string.Empty, ProductUpload.IMAGE_TYPE.Approved),
                ////                         ProductID = sp.ProductID,
                ////                         Name = sp.Product.Name,
                ////                         CategoryID = sp.Product.CategoryID,
                ////                         CategoryName = sp.Product.Category.Name,
                ////                         //StockStatus = (Int32)ss.StockStatus,
                ////                         MRP = ss.MRP,
                ////                         SaleRate = ss.RetailerRate,
                ////                         ShopStockID = ss.ID,
                ////                         Color = null,
                ////                         PackSize = ss.PackSize,
                ////                         PackUnit = ss.Unit.Name,
                ////                         StockQty = ss.Qty,
                ////                         RetailPoint = ss.BusinessPoints,   //Yashaswi 10-7-
                ////                         ShopID = (Int32)sp.ShopID
                ////                     }).OrderBy(x => x.SaleRate).Take(8).ToList();
                ////foreach (var item in productDetailView)
                ////{
                ////    item.ProductThumbPath = ImageDisplay.SetProductThumbPath(item.ProductID, "default", string.Empty, ProductUpload.IMAGE_TYPE.Approved);
                ////    //item.Name = item.Name.Replace("+", " ");
                ////}

                ////homePage.TrendingDeals.IsProduct = true;
                ////homePage.TrendingDeals.IsBanner = false;
                ////homePage.TrendingDeals.IsCategory = false;

                ////long TrendingDealID = db.Deals.Where(x => x.ShortName == "Trending Deal").Select(x => x.ID).FirstOrDefault();
                ////homePage.TrendingDeals.ProductList = deal.GetDealsProducts(cityId, OfferStatus.AVAILABLEDEALS, 0, 1, 12, TrendingDealID, franchiseId);////added FranchiseID for Mutiple MCO

                ///*Trending Deals Completed*/

                /*Refer and Earn*/
                homePage.ReferEarn = rcKey.LOCALIMG_PATH + "Content/img/" + "Refer_Earn.png";
                /*Refer and Earn Completed*/

                /*Logo*/
                db.Configuration.ProxyCreationEnabled = false;
                //  BusinessLogicLayer.ReadConfig rcKey = new BusinessLogicLayer.ReadConfig(System.Web.HttpContext.Current.Server);
                // int franchiseId = db.FranchiseLocations.Where(x => x.AreaID == areaid && x.IsActive).Select(x => x.FranchiseID ?? 0).FirstOrDefault();
                DesignBlockType blocktype = new DesignBlockType();
                blocktype = db.DesignBlockTypes.Where(x => x.Name == "Logo").FirstOrDefault();
                BlockItemsList blockitemlist = db.BlockItemsLists.Where(x => x.DesignBlockTypeID == blocktype.ID && x.FranchiseID == franchiseId).FirstOrDefault();
                if (blockitemlist != null)
                {
                    string imagename = rcKey.HOME_IMAGE_HTTP + blockitemlist.ImageName;
                    homePage.Logo = imagename;
                    //ImageDisplay.SetProductThumbPath((Int64)blockitemlist.ProductID, "default", string.Empty, ProductUpload.IMAGE_TYPE.Approved);
                    //obj = new { Success = 1, Message = "Success.", data = imagename };
                }
                /*Logo Completed.*/
                obj = new { Success = 1, Message = "List is found.", data = homePage };
            }
            catch (Exception ex)
            {
                obj = new { Success = 0, Message = ex.Message, data = string.Empty };
            }
            return obj;
        }


        public string GetURLStructureName(string Name)
        {
            string str = Name;
            str = System.Text.RegularExpressions.Regex.Replace(str, @"[\\\#$~%.':*?<>{} ]", " ").Replace("&", "and");
            str = System.Text.RegularExpressions.Regex.Replace(str, @"\s+/g", " ");
            string concat = "";
            str = System.Text.RegularExpressions.Regex.Replace(str, @"[\/\+\-\,()]", "|");
            string[] strSplit = str.Split('|');
            for (int i = 0; i < strSplit.Length; i++)
            {
                if (concat.Length <= 30)
                {
                    concat = concat.Length == 0 ? strSplit[i].Trim() : concat + ' ' + strSplit[i].Trim();
                }
            }
            concat = System.Text.RegularExpressions.Regex.Replace(concat, @"\s+", " ");
            concat = System.Text.RegularExpressions.Regex.Replace(concat, @"[\/\\#,+()$~%.':*?<>{} ]", "-").Replace("&", "and");
            concat = concat.Trim(new[] { '-' });
            return concat;
        }

        [Route("api/GetBanners/GetBannerData")]
        public object GetBannerData(long BlockItemId)
        {
            object obj = new object();
            try
            {
                if (BlockItemId == null || BlockItemId <= 0)
                {
                    return obj = new { Success = 0, Message = "Invalid Parameter.", data = string.Empty };
                }
                BlockItemsList blockItems = db.BlockItemsLists.Where(x => x.ID == BlockItemId && EntityFunctions.TruncateTime(x.StartDate) <= EntityFunctions.TruncateTime(DateTime.Now) && EntityFunctions.TruncateTime(x.EndDate) >= EntityFunctions.TruncateTime(DateTime.Now)).FirstOrDefault();
                if (blockItems == null)
                    return obj = new { Success = 0, Message = "Invalid BlockItemId.", data = string.Empty };
                long CityId = (from fr in db.Franchises
                               join pin in db.Pincodes on fr.PincodeID equals pin.ID
                               where fr.ID == blockItems.FranchiseID
                               select new { pin.CityID }
                              ).FirstOrDefault().CityID;
                if (blockItems.OfferID != null && blockItems.OfferID > 0)
                {
                    Offers lOffers = new Offers();
                    List<OfferProducts> lOfferProducts = lOffers.GetOfferProducts(CityId, OfferStatus.AVAILABLEDEALS, 0, 1, 12, blockItems.FranchiseID).Where(x => x.OfferID == blockItems.OfferID).ToList(); ;////added FranchiseID for Mutiple MCO
                    if (lOfferProducts != null && lOfferProducts.Count > 0)
                    {
                        ProductWithRefinementViewModel objProductWithRefinement = ConversionOfferProductsToProductView(lOfferProducts, blockItems.FranchiseID);
                        obj = new { Success = 1, Message = "Record Found.", data = objProductWithRefinement };
                    }
                    else
                        obj = new { Success = 0, Message = "Record Not Found.", data = string.Empty };
                }
                else if (!string.IsNullOrEmpty(blockItems.Keyword) || (blockItems.CategoryID != null && blockItems.CategoryID > 0) || (blockItems.ShopID != null && blockItems.ShopID > 0))
                {
                    Category category = db.Categories.Where(x => x.IsActive && x.ID == blockItems.CategoryID && x.IsExpire == false).FirstOrDefault();
                    if (category != null && (category.Level == 1 || category.Level == 2))
                    {
                        List<MenuViewModel> levelOneMenu = null;
                        List<MenuViewModel> levelTwoMenu = null;
                        List<int> levelThreeIds = new List<int>();
                        BusinessLogicLayer.FranchiseMenuList objMenuList = new BusinessLogicLayer.FranchiseMenuList();
                        DataTable dt = new DataTable();
                        // dt = obj.Select_FranchiseMenu(cityId, System.Web.HttpContext.Current.Server);////hide
                        dt = objMenuList.Select_FranchiseMenu(CityId, blockItems.FranchiseID, System.Web.HttpContext.Current.Server);////added

                        /*Select All Menu By Franchise */
                        List<MenuViewModel> FMenu = new List<MenuViewModel>();
                        FMenu = (from n in dt.AsEnumerable()
                                 select new MenuViewModel
                                 {
                                     ID = n.Field<Int32>("ID"),
                                     CategoryName = n.Field<string>("CategoryName"),
                                     CategoryRouteName = n.Field<string>("CategoryRouteName"),
                                     //ImagePath = rcKey.CATEGORY_IMAGE_HTTP + "/" + n.Field<string>("ImagePath"),
                                     SequenceOrder = n.Field<int?>("SequenceOrder"),
                                     Level = n.Field<int>("Level"),
                                     ParentCategoryID = n.Field<int?>("ParentCategoryID"),
                                     IsManaged = n.Field<bool>("IsManaged")
                                 }).OrderBy(x => x.SequenceOrder).ToList();

                        if (category.Level == 1)
                        {
                            levelOneMenu = FMenu.Where(x => x.Level == 1 && x.ID == category.ID).OrderBy(x => x.SequenceOrder).ToList();
                            if (levelOneMenu != null && levelOneMenu.Count > 0)
                            {
                                foreach (MenuViewModel M in levelOneMenu)
                                {
                                    M.LevelTwoListing = FMenu.Where(x => x.Level == 2 && x.ParentCategoryID == M.ID).ToList();
                                    foreach (MenuViewModel M2 in M.LevelTwoListing)
                                    {
                                        M2.BannerImageList = new List<string>();
                                        string Img1 = string.Empty;
                                        M2.LevelThreeListing = FMenu.Where(x => x.Level == 3 && x.ParentCategoryID == M2.ID).ToList();
                                        foreach (var item in M2.LevelThreeListing)
                                        {
                                            levelThreeIds.Add(item.ID);
                                        }
                                    }
                                }
                            }
                        }
                        else if (category.Level == 2)
                        {
                            levelTwoMenu = FMenu.Where(x => x.Level == 2 && x.ID == category.ID).OrderBy(x => x.SequenceOrder).ToList();
                            if (levelTwoMenu != null && levelTwoMenu.Count > 0)
                            {
                                foreach (MenuViewModel M2 in levelTwoMenu)
                                {
                                    M2.BannerImageList = new List<string>();
                                    string Img1 = string.Empty;
                                    M2.LevelThreeListing = FMenu.Where(x => x.Level == 3 && x.ParentCategoryID == M2.ID).ToList();
                                    foreach (var item in M2.LevelThreeListing)
                                    {
                                        levelThreeIds.Add(item.ID);
                                    }
                                }
                            }
                        }
                        ProductWithRefinementViewModel productWithRefinementViewModel = new ProductWithRefinementViewModel();
                        if (levelThreeIds != null && levelThreeIds.Count > 0)
                        {
                            ProductListByCategoryViewModel productSearch = new ProductListByCategoryViewModel();
                            productSearch.PageIndex = 1;
                            productSearch.Keyword = string.Empty;
                            productSearch.ProductID = 0;
                            productSearch.BrandIDs = string.Empty;
                            productSearch.ShopID = 0;
                            //productSearch.ShopStockIDList = blockItems.ShopStockID.HasValue ? blockItems.ShopStockID.Value : 0;
                            productSearch.CityID = (int)CityId;
                            productSearch.FranchiseID = blockItems.FranchiseID;
                            productSearch.PageSize = 50;//Sonali_03-11-2018_For display all product
                            productSearch.Version = 1;

                            DataTable dtCategoryId = new DataTable();
                            dtCategoryId.Columns.Add("CategoryID");
                            foreach (var item in levelThreeIds)
                            {
                                dtCategoryId.Rows.Add(item);
                            }
                            productSearch.CategoryIDList = dtCategoryId;
                            ProductList productList = new ProductList(System.Web.HttpContext.Current.Server);
                            productWithRefinementViewModel = productList.GetProductListBy3rdLevelCategory(productSearch);
                        }

                        // var output = aSearch.GetSearchMetaData(pre, (AutoSearch.SEARCHBY)searchBy, cityID, franchiseID);
                        if (productWithRefinementViewModel != null && productWithRefinementViewModel.productList.Count > 0 && productWithRefinementViewModel.productRefinements.Count > 0)
                            obj = new { Success = 1, Message = "Record Found.", data = productWithRefinementViewModel };
                        else
                            obj = new { Success = 0, Message = "Record Not Found.", data = string.Empty };

                        //BusinessLogicLayer.ReadConfig rcKey = new BusinessLogicLayer.ReadConfig(System.Web.HttpContext.Current.Server);
                        //BusinessLogicLayer.DynamicProductList dpObj = new BusinessLogicLayer.DynamicProductList();
                        //DataSet ds = new DataSet();
                        //ds = dpObj.Select_DynamicProducts(franchiseId, (int)blockItems.CategoryID.Value, System.Web.HttpContext.Current.Server);////added cityId->franchiseID
                        ///*Select All Menu By Franchise */
                        //DataTable dt = new DataTable();
                        //dt = ds.Tables[0];

                        //List<CategoryPageViewModel> FMenu = new List<CategoryPageViewModel>();

                        //FMenu = (from n in dt.AsEnumerable()
                        //         select new CategoryPageViewModel
                        //         {
                        //             ID = n.Field<Int32>("ID"),
                        //             CategoryName = n.Field<string>("CategoryName"),
                        //             SequenceOrder = n.Field<int?>("SequenceOrder"),
                        //             Level = n.Field<int>("Level"),
                        //             ParentCategoryID = n.Field<int?>("ParentCategoryID"),
                        //             IsManaged = n.Field<bool>("IsManaged")
                        //         }).OrderBy(x => x.SequenceOrder).ToList();


                        //DataTable dt1 = new DataTable();
                        //dt1 = ds.Tables[1];

                        //List<DynamicProductViewModel> DProducts = new List<DynamicProductViewModel>();

                        //DProducts = (from n in dt1.AsEnumerable()
                        //             select new DynamicProductViewModel
                        //             {
                        //                 ID = n.Field<Int64>("ProductID"),
                        //                 Name = n.Field<string>("Name"),
                        //                 LevelTwoCatID = n.Field<int>("LevelTwoCatID"),
                        //                 ShopStockID = n.Field<Int64>("ShopStockID"),
                        //                 SequenceOrder = n.Field<int?>("SequenceOrder") == null ? 0 : n.Field<int?>("SequenceOrder"),
                        //                 RetailerRate = n.Field<decimal>("RetailerRate"),
                        //                 MRP = n.Field<decimal>("MRP"),
                        //                 RetailPoint = n.Field<decimal>("RetailPoint") //Yashaswi 9-7-2018
                        //             }).ToList();
                        //// }

                        //foreach (DynamicProductViewModel DP in DProducts)
                        //{
                        //    Color color = db.ShopStocks.Where(x => x.ID == DP.ShopStockID).FirstOrDefault().ProductVarient.Color;
                        //    DP.ImagePath = BusinessLogicLayer.ImageDisplay.SetProductThumbPath(DP.ID,
                        //                 ((color.ID == 1 || color.Name == "N/A") ? "default" : color.Name.Trim()),
                        //                 string.Empty, BusinessLogicLayer.ProductUpload.IMAGE_TYPE.Approved);
                        //    string sizeName = db.ShopStocks.Find(DP.ShopStockID).ProductVarient.Size.Name;
                        //    DP.Size = (sizeName.ToUpper().Trim() == "N/A") ? string.Empty : sizeName;
                        //    //Added for SEO URL Structure RULE by AShish
                        //    DP.Name = DP.Name.Replace("+", " ");
                        //    DP.URLStructureName = GetURLStructureName(DP.Name);
                        //}

                        //if (DProducts != null && DProducts.Count > 0)
                        //{
                        //    obj = new { Success = 1, Message = "Record Found.", data = new { CategoryList = FMenu, ProductList = DProducts } };
                        //}
                        //else
                        //{
                        //    obj = new { Success = 0, Message = "Record Not Found.", data = string.Empty };
                        //}

                    }
                    else
                    {
                        ProductSearchViewModel productSearch = new ProductSearchViewModel();
                        productSearch.PageIndex = 1;
                        productSearch.Keyword = blockItems.Keyword != null ? blockItems.Keyword : string.Empty;
                        productSearch.CategoryID = (long)(blockItems.CategoryID.HasValue ? blockItems.CategoryID.Value : 0);
                        productSearch.ProductID = blockItems.ProductID.HasValue ? blockItems.ProductID.Value : 0;
                        productSearch.BrandIDs = blockItems.BrandId.HasValue ? blockItems.BrandId.Value.ToString() : string.Empty;
                        productSearch.ShopID = blockItems.ShopID.HasValue ? blockItems.ShopID.Value : 0;
                        //productSearch.ShopStockIDList = blockItems.ShopStockID.HasValue ? blockItems.ShopStockID.Value : 0;
                        productSearch.CityID = (int)CityId;
                        productSearch.FranchiseID = blockItems.FranchiseID;
                        productSearch.PageSize = 50;//Sonali_03-11-2018_For display all product
                        productSearch.Version = 1;
                        ProductWithRefinementViewModel productWithRefinementViewModel = new ProductWithRefinementViewModel();
                        ProductList productList = new ProductList(System.Web.HttpContext.Current.Server);
                        productWithRefinementViewModel = productList.GetProductList(productSearch, false);
                        // var output = aSearch.GetSearchMetaData(pre, (AutoSearch.SEARCHBY)searchBy, cityID, franchiseID);
                        if (productWithRefinementViewModel != null && productWithRefinementViewModel.productList.Count > 0 && productWithRefinementViewModel.productRefinements.Count > 0)
                            obj = new { Success = 1, Message = "Record Found.", data = productWithRefinementViewModel };
                        else
                            obj = new { Success = 0, Message = "Record Not Found.", data = string.Empty };
                    }
                }
                else if (blockItems.BrandId != null && blockItems.BrandId > 0)
                {
                    //This data fetch is temprorely by Sonali
                    ProductSearchViewModel productSearch = new ProductSearchViewModel();
                    productSearch.CityID = (Int32)CityId;
                    productSearch.FranchiseID = blockItems.FranchiseID;
                    productSearch.ShopID = 0;
                    productSearch.Keyword = db.Brands.Where(x => x.ID == blockItems.BrandId).Select(x => x.Name).FirstOrDefault();
                    productSearch.CategoryID = 0;
                    productSearch.BrandIDs = blockItems.BrandId.ToString();
                    productSearch.ProductID = 0;
                    productSearch.ColorIDs = string.Empty;
                    productSearch.SizeIDs = string.Empty;
                    productSearch.DimensionIDs = string.Empty;
                    productSearch.MaterialIDs = string.Empty;
                    productSearch.SpecificationIDs = string.Empty;
                    productSearch.SpecificationValues = string.Empty;
                    productSearch.MinPrice = 0;
                    productSearch.MaxPrice = 0;
                    productSearch.PageIndex = 1;
                    productSearch.PageSize = 50;
                    productSearch.Version = 1;
                    productSearch.IsScroll = false;
                    productSearch.CustLoginID = 0;
                    productSearch.SearchInCategoryOnly = false;
                    productSearch.ImageType = string.Empty;
                    productSearch.IsVarientRestricted = false;
                    productSearch.isListVarient = false;
                    productSearch.SortVal = 0;
                    ProductWithRefinementViewModel productWithRefinementViewModel = new ProductWithRefinementViewModel();
                    ProductList productList = new ProductList(System.Web.HttpContext.Current.Server);
                    productWithRefinementViewModel = productList.GetProductList(productSearch, false);
                    if (productWithRefinementViewModel != null)
                    {
                        obj = new { Success = 1, Message = "Product list are found.", data = productWithRefinementViewModel };
                    }

                }
                else if (blockItems.ProductID != null && blockItems.ProductID > 0)
                {
                    string clName = "default";
                    ProductDetails p = new ProductDetails(System.Web.HttpContext.Current.Server);
                    ProductDetailview detailview = new ProductDetailview();
                    detailview.ProductbasicDetail = p.GetBasicDetails(Convert.ToInt64(blockItems.ProductID));
                    detailview.ProductVarient = p.GetStockVarients(Convert.ToInt64(blockItems.ProductID), 0, null, blockItems.FranchiseID);
                    detailview.Images = ImageDisplay.GetStockImages(Convert.ToInt64(blockItems.ProductID), string.IsNullOrEmpty(clName) || clName == "N/A" ? string.Empty : clName);
                    detailview.ProductGeneralSpecification = p.GetGeneralDescription(Convert.ToInt64(blockItems.ProductID));
                    if (!string.IsNullOrEmpty(detailview.ProductGeneralSpecification))
                    {
                        var httpWebRequest = (HttpWebRequest)WebRequest.Create(detailview.ProductGeneralSpecification);
                        //httpWebRequest.ContentType = "application/json";
                        httpWebRequest.Method = "GET";
                        var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                        using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                        {
                            var result = streamReader.ReadToEnd();
                            detailview.ProductGeneralSpecification = result;
                        };
                    }
                    if (detailview.ProductbasicDetail != null && detailview.ProductVarient != null)
                    {
                        obj = new { Success = 1, Message = "Record Found.", data = detailview };
                    }
                    else
                    {
                        obj = new { Success = 0, Message = "Record Not Found.", data = string.Empty };
                    }
                }
            }
            catch (Exception ex)
            {
                obj = new { Success = 0, Message = ex.Message, data = string.Empty };
            }
            return obj;
        }

        [Route("api/GetBanners/GetOfferData")]
        public object GetOfferData(long Id)
        {
            object obj = new object();
            try
            {
                if (Id == null || Id <= 0)
                {
                    return obj = new { Success = 0, Message = "Invalid Parameter.", data = string.Empty };
                }
                HomePageDynamicSectionBanner blockItems = db.HomePageDynamicSectionBanner.Where(x => x.ID == Id && x.IsActive == true && EntityFunctions.TruncateTime(x.StartDate) <= EntityFunctions.TruncateTime(DateTime.Now) && EntityFunctions.TruncateTime(x.EndDate) >= EntityFunctions.TruncateTime(DateTime.Now)).FirstOrDefault();
                if (blockItems == null)
                    return obj = new { Success = 0, Message = "Invalid Id.", data = string.Empty };
                int franchiseId = (int)db.HomePageDynamicSection.Where(x => x.ID == blockItems.HomePageDynamicSectionId && x.IsActive).Select(x => x.FranchiseId).FirstOrDefault();
                long CityId = (from fr in db.Franchises
                               join pin in db.Pincodes on fr.PincodeID equals pin.ID
                               where fr.ID == franchiseId
                               select new { pin.CityID }
                              ).FirstOrDefault().CityID;
                if (blockItems.OfferId != null && blockItems.OfferId > 0)
                {
                    Offers lOffers = new Offers();
                    List<OfferProducts> lOfferProducts = lOffers.GetOfferProducts(CityId, OfferStatus.AVAILABLEDEALS, 0, 1, 12, franchiseId).Where(x => x.OfferID == blockItems.OfferId).ToList();////added FranchiseID for Mutiple MCO
                    if (lOfferProducts != null && lOfferProducts.Count > 0)
                    {
                        ProductWithRefinementViewModel productWithRefinementViewModel = ConversionOfferProductsToProductView(lOfferProducts, franchiseId);
                        obj = new { Success = 1, Message = "Record Found.", data = productWithRefinementViewModel };
                    }
                    else
                        obj = new { Success = 0, Message = "Record Not Found.", data = string.Empty };
                }
                else if (!string.IsNullOrEmpty(blockItems.Keyword) || (blockItems.CategoryID != null && blockItems.CategoryID > 0) || (blockItems.ShopId != null && blockItems.ShopId > 0))
                {
                    Category category = db.Categories.Where(x => x.IsActive && x.ID == blockItems.CategoryID && x.IsExpire == false).FirstOrDefault();
                    if (category != null && (category.Level == 1 || category.Level == 2))
                    {
                        List<MenuViewModel> levelOneMenu = null;
                        List<MenuViewModel> levelTwoMenu = null;
                        List<int> levelThreeIds = new List<int>();
                        BusinessLogicLayer.FranchiseMenuList objMenuList = new BusinessLogicLayer.FranchiseMenuList();
                        DataTable dt = new DataTable();
                        // dt = obj.Select_FranchiseMenu(cityId, System.Web.HttpContext.Current.Server);////hide
                        dt = objMenuList.Select_FranchiseMenu(CityId, franchiseId, System.Web.HttpContext.Current.Server);////added

                        /*Select All Menu By Franchise */
                        List<MenuViewModel> FMenu = new List<MenuViewModel>();
                        FMenu = (from n in dt.AsEnumerable()
                                 select new MenuViewModel
                                 {
                                     ID = n.Field<Int32>("ID"),
                                     CategoryName = n.Field<string>("CategoryName"),
                                     CategoryRouteName = n.Field<string>("CategoryRouteName"),
                                     //ImagePath = rcKey.CATEGORY_IMAGE_HTTP + "/" + n.Field<string>("ImagePath"),
                                     SequenceOrder = n.Field<int?>("SequenceOrder"),
                                     Level = n.Field<int>("Level"),
                                     ParentCategoryID = n.Field<int?>("ParentCategoryID"),
                                     IsManaged = n.Field<bool>("IsManaged")
                                 }).OrderBy(x => x.SequenceOrder).ToList();

                        if (category.Level == 1)
                        {
                            levelOneMenu = FMenu.Where(x => x.Level == 1 && x.ID == category.ID).OrderBy(x => x.SequenceOrder).ToList();
                            if (levelOneMenu != null && levelOneMenu.Count > 0)
                            {
                                foreach (MenuViewModel M in levelOneMenu)
                                {
                                    M.LevelTwoListing = FMenu.Where(x => x.Level == 2 && x.ParentCategoryID == M.ID).ToList();
                                    foreach (MenuViewModel M2 in M.LevelTwoListing)
                                    {
                                        M2.BannerImageList = new List<string>();
                                        string Img1 = string.Empty;
                                        M2.LevelThreeListing = FMenu.Where(x => x.Level == 3 && x.ParentCategoryID == M2.ID).ToList();
                                        foreach (var item in M2.LevelThreeListing)
                                        {
                                            levelThreeIds.Add(item.ID);
                                        }
                                    }
                                }
                            }
                        }
                        else if (category.Level == 2)
                        {
                            levelTwoMenu = FMenu.Where(x => x.Level == 2 && x.ID == category.ID).OrderBy(x => x.SequenceOrder).ToList();
                            if (levelTwoMenu != null && levelTwoMenu.Count > 0)
                            {
                                foreach (MenuViewModel M2 in levelTwoMenu)
                                {
                                    M2.BannerImageList = new List<string>();
                                    string Img1 = string.Empty;
                                    M2.LevelThreeListing = FMenu.Where(x => x.Level == 3 && x.ParentCategoryID == M2.ID).ToList();
                                    foreach (var item in M2.LevelThreeListing)
                                    {
                                        levelThreeIds.Add(item.ID);
                                    }
                                }
                            }
                        }
                        ProductWithRefinementViewModel productWithRefinementViewModel = new ProductWithRefinementViewModel();
                        if (levelThreeIds != null && levelThreeIds.Count > 0)
                        {
                            ProductListByCategoryViewModel productSearch = new ProductListByCategoryViewModel();
                            productSearch.PageIndex = 1;
                            productSearch.Keyword = string.Empty;
                            productSearch.ProductID = 0;
                            productSearch.BrandIDs = string.Empty;
                            productSearch.ShopID = 0;
                            //productSearch.ShopStockIDList = blockItems.ShopStockID.HasValue ? blockItems.ShopStockID.Value : 0;
                            productSearch.CityID = (int)CityId;
                            productSearch.FranchiseID = franchiseId;
                            productSearch.PageSize = 50;//Sonali_03-11-2018_For display all product
                            productSearch.Version = 1;

                            DataTable dtCategoryId = new DataTable();
                            dtCategoryId.Columns.Add("CategoryID");
                            foreach (var item in levelThreeIds)
                            {
                                dtCategoryId.Rows.Add(item);
                            }
                            productSearch.CategoryIDList = dtCategoryId;
                            ProductList productList = new ProductList(System.Web.HttpContext.Current.Server);
                            productWithRefinementViewModel = productList.GetProductListBy3rdLevelCategory(productSearch);
                        }

                        // var output = aSearch.GetSearchMetaData(pre, (AutoSearch.SEARCHBY)searchBy, cityID, franchiseID);
                        if (productWithRefinementViewModel != null && productWithRefinementViewModel.productList.Count > 0 && productWithRefinementViewModel.productRefinements.Count > 0)
                            obj = new { Success = 1, Message = "Record Found.", data = productWithRefinementViewModel };
                        else
                            obj = new { Success = 0, Message = "Record Not Found.", data = string.Empty };

                        //BusinessLogicLayer.ReadConfig rcKey = new BusinessLogicLayer.ReadConfig(System.Web.HttpContext.Current.Server);
                        //BusinessLogicLayer.DynamicProductList dpObj = new BusinessLogicLayer.DynamicProductList();
                        //DataSet ds = new DataSet();
                        //ds = dpObj.Select_DynamicProducts(franchiseId, (int)blockItems.CategoryID.Value, System.Web.HttpContext.Current.Server);////added cityId->franchiseID
                        ///*Select All Menu By Franchise */
                        //DataTable dt = new DataTable();
                        //dt = ds.Tables[0];

                        //List<CategoryPageViewModel> FMenu = new List<CategoryPageViewModel>();

                        //FMenu = (from n in dt.AsEnumerable()
                        //         select new CategoryPageViewModel
                        //         {
                        //             ID = n.Field<Int32>("ID"),
                        //             CategoryName = n.Field<string>("CategoryName"),
                        //             SequenceOrder = n.Field<int?>("SequenceOrder"),
                        //             Level = n.Field<int>("Level"),
                        //             ParentCategoryID = n.Field<int?>("ParentCategoryID"),
                        //             IsManaged = n.Field<bool>("IsManaged")
                        //         }).OrderBy(x => x.SequenceOrder).ToList();


                        //DataTable dt1 = new DataTable();
                        //dt1 = ds.Tables[1];

                        //List<DynamicProductViewModel> DProducts = new List<DynamicProductViewModel>();

                        //DProducts = (from n in dt1.AsEnumerable()
                        //             select new DynamicProductViewModel
                        //             {
                        //                 ID = n.Field<Int64>("ProductID"),
                        //                 Name = n.Field<string>("Name"),
                        //                 LevelTwoCatID = n.Field<int>("LevelTwoCatID"),
                        //                 ShopStockID = n.Field<Int64>("ShopStockID"),
                        //                 SequenceOrder = n.Field<int?>("SequenceOrder") == null ? 0 : n.Field<int?>("SequenceOrder"),
                        //                 RetailerRate = n.Field<decimal>("RetailerRate"),
                        //                 MRP = n.Field<decimal>("MRP"),
                        //                 RetailPoint = n.Field<decimal>("RetailPoint") //Yashaswi 9-7-2018
                        //             }).ToList();
                        //// }

                        //foreach (DynamicProductViewModel DP in DProducts)
                        //{
                        //    Color color = db.ShopStocks.Where(x => x.ID == DP.ShopStockID).FirstOrDefault().ProductVarient.Color;
                        //    DP.ImagePath = BusinessLogicLayer.ImageDisplay.SetProductThumbPath(DP.ID,
                        //                 ((color.ID == 1 || color.Name == "N/A") ? "default" : color.Name.Trim()),
                        //                 string.Empty, BusinessLogicLayer.ProductUpload.IMAGE_TYPE.Approved);
                        //    string sizeName = db.ShopStocks.Find(DP.ShopStockID).ProductVarient.Size.Name;
                        //    DP.Size = (sizeName.ToUpper().Trim() == "N/A") ? string.Empty : sizeName;
                        //    //Added for SEO URL Structure RULE by AShish
                        //    DP.Name = DP.Name.Replace("+", " ");
                        //    DP.URLStructureName = GetURLStructureName(DP.Name);
                        //}

                        //if (DProducts != null && DProducts.Count > 0)
                        //{
                        //    obj = new { Success = 1, Message = "Record Found.", data = new { CategoryList = FMenu, ProductList = DProducts } };
                        //}
                        //else
                        //{
                        //    obj = new { Success = 0, Message = "Record Not Found.", data = string.Empty };
                        //}

                    }
                    else
                    {
                        ProductSearchViewModel productSearch = new ProductSearchViewModel();
                        productSearch.PageIndex = 1;
                        productSearch.Keyword = blockItems.Keyword != null ? blockItems.Keyword : string.Empty;
                        productSearch.CategoryID = (long)(blockItems.CategoryID.HasValue ? blockItems.CategoryID.Value : 0);
                        productSearch.ProductID = 0;
                        productSearch.BrandIDs = blockItems.BrandID.HasValue ? blockItems.BrandID.Value.ToString() : string.Empty;
                        productSearch.ShopID = blockItems.ShopId.HasValue ? blockItems.ShopId.Value : 0;
                        //productSearch.ShopStockIDList = blockItems.ShopStockID.HasValue ? blockItems.ShopStockID.Value : 0;
                        productSearch.CityID = (int)CityId;
                        productSearch.FranchiseID = franchiseId;
                        productSearch.PageSize = 50;//Sonali_03-11-2018_For display all product
                        productSearch.Version = 1;
                        ProductWithRefinementViewModel productWithRefinementViewModel = new ProductWithRefinementViewModel();
                        ProductList productList = new ProductList(System.Web.HttpContext.Current.Server);
                        productWithRefinementViewModel = productList.GetProductList(productSearch, false);
                        // var output = aSearch.GetSearchMetaData(pre, (AutoSearch.SEARCHBY)searchBy, cityID, franchiseID);
                        if (productWithRefinementViewModel != null && productWithRefinementViewModel.productList.Count > 0 && productWithRefinementViewModel.productRefinements.Count > 0)
                            obj = new { Success = 1, Message = "Record Found.", data = productWithRefinementViewModel };
                        else
                            obj = new { Success = 0, Message = "Record Not Found.", data = string.Empty };
                    }
                }
                else if (blockItems.BrandID != null && blockItems.BrandID > 0)
                {
                    //This data fetch is temprorely by Sonali
                    ProductSearchViewModel productSearch = new ProductSearchViewModel();
                    productSearch.CityID = (Int32)CityId;
                    productSearch.FranchiseID = franchiseId;
                    productSearch.ShopID = 0;
                    productSearch.Keyword = db.Brands.Where(x => x.ID == blockItems.BrandID).Select(x => x.Name).FirstOrDefault();
                    productSearch.CategoryID = 0;
                    productSearch.BrandIDs = blockItems.BrandID.ToString();
                    productSearch.ProductID = 0;
                    productSearch.ColorIDs = string.Empty;
                    productSearch.SizeIDs = string.Empty;
                    productSearch.DimensionIDs = string.Empty;
                    productSearch.MaterialIDs = string.Empty;
                    productSearch.SpecificationIDs = string.Empty;
                    productSearch.SpecificationValues = string.Empty;
                    productSearch.MinPrice = 0;
                    productSearch.MaxPrice = 0;
                    productSearch.PageIndex = 1;
                    productSearch.PageSize = 50;
                    productSearch.Version = 1;
                    productSearch.IsScroll = false;
                    productSearch.CustLoginID = 0;
                    productSearch.SearchInCategoryOnly = false;
                    productSearch.ImageType = string.Empty;
                    productSearch.IsVarientRestricted = false;
                    productSearch.isListVarient = false;
                    productSearch.SortVal = 0;
                    ProductWithRefinementViewModel productWithRefinementViewModel = new ProductWithRefinementViewModel();
                    ProductList productList = new ProductList(System.Web.HttpContext.Current.Server);
                    productWithRefinementViewModel = productList.GetProductList(productSearch, false);
                    if (productWithRefinementViewModel != null)
                    {
                        obj = new { Success = 1, Message = "Product list are found.", data = productWithRefinementViewModel };
                    }
                }
                //else if (blockItems.ProductID != null && blockItems.ProductID > 0)
                //{
                //    string clName = "default";
                //    ProductDetails p = new ProductDetails(System.Web.HttpContext.Current.Server);
                //    ProductDetailview detailview = new ProductDetailview();
                //    detailview.ProductbasicDetail = p.GetBasicDetails(Convert.ToInt64(blockItems.ProductID));
                //    detailview.ProductVarient = p.GetStockVarients(Convert.ToInt64(blockItems.ProductID), 0, null, blockItems.FranchiseId);
                //    detailview.Images = ImageDisplay.GetStockImages(Convert.ToInt64(blockItems.ProductID), string.IsNullOrEmpty(clName) || clName == "N/A" ? string.Empty : clName);
                //    detailview.ProductGeneralSpecification = p.GetGeneralDescription(Convert.ToInt64(blockItems.ProductID));
                //    if (!string.IsNullOrEmpty(detailview.ProductGeneralSpecification))
                //    {
                //        var httpWebRequest = (HttpWebRequest)WebRequest.Create(detailview.ProductGeneralSpecification);
                //        //httpWebRequest.ContentType = "application/json";
                //        httpWebRequest.Method = "GET";
                //        var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                //        using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                //        {
                //            var result = streamReader.ReadToEnd();
                //            detailview.ProductGeneralSpecification = result;
                //        };
                //    }
                //    if (detailview.ProductbasicDetail != null && detailview.ProductVarient != null)
                //    {
                //        obj = new { Success = 1, Message = "Record Found.", data = detailview };
                //    }
                //    else
                //    {
                //        obj = new { Success = 0, Message = "Record Not Found.", data = string.Empty };
                //    }
                //}
            }
            catch (Exception ex)
            {
                obj = new { Success = 0, Message = ex.Message, data = string.Empty };
            }
            return obj;
        }

        [Route("api/GetBanners/GetSubBannerProductData")]
        public object GetSubBannerData(long Id)
        {
            object obj = new object();
            try
            {
                if (Id == null || Id <= 0)
                {
                    return obj = new { Success = 0, Message = "Invalid Parameter.", data = string.Empty };
                }
                DealCategoryList blockItems = db.DealCategoryLists.Where(x => x.Id == Id && EntityFunctions.TruncateTime(x.StartDate) <= EntityFunctions.TruncateTime(DateTime.Now) && EntityFunctions.TruncateTime(x.EndDate) >= EntityFunctions.TruncateTime(DateTime.Now)).FirstOrDefault();
                if (blockItems == null)
                    return obj = new { Success = 0, Message = "Invalid Id.", data = string.Empty };
                long CityId = (from fr in db.Franchises
                               join pin in db.Pincodes on fr.PincodeID equals pin.ID
                               where fr.ID == blockItems.FranchiseId
                               select new { pin.CityID }
                              ).FirstOrDefault().CityID;
                if (blockItems.OfferID != null && blockItems.OfferID > 0)
                {
                    Offers lOffers = new Offers();
                    List<OfferProducts> lOfferProducts = lOffers.GetOfferProducts(CityId, OfferStatus.AVAILABLEDEALS, 0, 1, 12, blockItems.FranchiseId).Where(x => x.OfferID == blockItems.OfferID).ToList(); ;////added FranchiseID for Mutiple MCO
                    if (lOfferProducts != null && lOfferProducts.Count > 0)
                    {
                        ProductWithRefinementViewModel productWithRefinementViewModel = ConversionOfferProductsToProductView(lOfferProducts, blockItems.FranchiseId);
                        obj = new { Success = 1, Message = "Record Found.", data = productWithRefinementViewModel };
                    }
                    else
                        obj = new { Success = 0, Message = "Record Not Found.", data = string.Empty };
                }
                else if (!string.IsNullOrEmpty(blockItems.Keyword) || (blockItems.CategoryID != null && blockItems.CategoryID > 0) || (blockItems.ShopID != null && blockItems.ShopID > 0))
                {
                    Category category = db.Categories.Where(x => x.IsActive && x.ID == blockItems.CategoryID && x.IsExpire == false).FirstOrDefault();
                    if (category != null && (category.Level == 1 || category.Level == 2))
                    {
                        List<MenuViewModel> levelOneMenu = null;
                        List<MenuViewModel> levelTwoMenu = null;
                        List<int> levelThreeIds = new List<int>();
                        BusinessLogicLayer.FranchiseMenuList objMenuList = new BusinessLogicLayer.FranchiseMenuList();
                        DataTable dt = new DataTable();
                        // dt = obj.Select_FranchiseMenu(cityId, System.Web.HttpContext.Current.Server);////hide
                        dt = objMenuList.Select_FranchiseMenu(CityId, blockItems.FranchiseId, System.Web.HttpContext.Current.Server);////added

                        /*Select All Menu By Franchise */
                        List<MenuViewModel> FMenu = new List<MenuViewModel>();
                        FMenu = (from n in dt.AsEnumerable()
                                 select new MenuViewModel
                                 {
                                     ID = n.Field<Int32>("ID"),
                                     CategoryName = n.Field<string>("CategoryName"),
                                     CategoryRouteName = n.Field<string>("CategoryRouteName"),
                                     //ImagePath = rcKey.CATEGORY_IMAGE_HTTP + "/" + n.Field<string>("ImagePath"),
                                     SequenceOrder = n.Field<int?>("SequenceOrder"),
                                     Level = n.Field<int>("Level"),
                                     ParentCategoryID = n.Field<int?>("ParentCategoryID"),
                                     IsManaged = n.Field<bool>("IsManaged")
                                 }).OrderBy(x => x.SequenceOrder).ToList();

                        if (category.Level == 1)
                        {
                            levelOneMenu = FMenu.Where(x => x.Level == 1 && x.ID == category.ID).OrderBy(x => x.SequenceOrder).ToList();
                            if (levelOneMenu != null && levelOneMenu.Count > 0)
                            {
                                foreach (MenuViewModel M in levelOneMenu)
                                {
                                    M.LevelTwoListing = FMenu.Where(x => x.Level == 2 && x.ParentCategoryID == M.ID).ToList();
                                    foreach (MenuViewModel M2 in M.LevelTwoListing)
                                    {
                                        M2.BannerImageList = new List<string>();
                                        string Img1 = string.Empty;
                                        M2.LevelThreeListing = FMenu.Where(x => x.Level == 3 && x.ParentCategoryID == M2.ID).ToList();
                                        foreach (var item in M2.LevelThreeListing)
                                        {
                                            levelThreeIds.Add(item.ID);
                                        }
                                    }
                                }
                            }
                        }
                        else if (category.Level == 2)
                        {
                            levelTwoMenu = FMenu.Where(x => x.Level == 2 && x.ID == category.ID).OrderBy(x => x.SequenceOrder).ToList();
                            if (levelTwoMenu != null && levelTwoMenu.Count > 0)
                            {
                                foreach (MenuViewModel M2 in levelTwoMenu)
                                {
                                    M2.BannerImageList = new List<string>();
                                    string Img1 = string.Empty;
                                    M2.LevelThreeListing = FMenu.Where(x => x.Level == 3 && x.ParentCategoryID == M2.ID).ToList();
                                    foreach (var item in M2.LevelThreeListing)
                                    {
                                        levelThreeIds.Add(item.ID);
                                    }
                                }
                            }
                        }
                        ProductWithRefinementViewModel productWithRefinementViewModel = new ProductWithRefinementViewModel();
                        if (levelThreeIds != null && levelThreeIds.Count > 0)
                        {
                            ProductListByCategoryViewModel productSearch = new ProductListByCategoryViewModel();
                            productSearch.PageIndex = 1;
                            productSearch.Keyword = string.Empty;
                            productSearch.ProductID = 0;
                            productSearch.BrandIDs = string.Empty;
                            productSearch.ShopID = 0;
                            //productSearch.ShopStockIDList = blockItems.ShopStockID.HasValue ? blockItems.ShopStockID.Value : 0;
                            productSearch.CityID = (int)CityId;
                            productSearch.FranchiseID = blockItems.FranchiseId;
                            productSearch.PageSize = 50;//Sonali_03-11-2018_For display all product
                            productSearch.Version = 1;

                            DataTable dtCategoryId = new DataTable();
                            dtCategoryId.Columns.Add("CategoryID");
                            foreach (var item in levelThreeIds)
                            {
                                dtCategoryId.Rows.Add(item);
                            }
                            productSearch.CategoryIDList = dtCategoryId;
                            ProductList productList = new ProductList(System.Web.HttpContext.Current.Server);
                            productWithRefinementViewModel = productList.GetProductListBy3rdLevelCategory(productSearch);
                        }

                        // var output = aSearch.GetSearchMetaData(pre, (AutoSearch.SEARCHBY)searchBy, cityID, franchiseID);
                        if (productWithRefinementViewModel != null && productWithRefinementViewModel.productList.Count > 0 && productWithRefinementViewModel.productRefinements.Count > 0)
                            obj = new { Success = 1, Message = "Record Found.", data = productWithRefinementViewModel };
                        else
                            obj = new { Success = 0, Message = "Record Not Found.", data = string.Empty };

                        //BusinessLogicLayer.ReadConfig rcKey = new BusinessLogicLayer.ReadConfig(System.Web.HttpContext.Current.Server);
                        //BusinessLogicLayer.DynamicProductList dpObj = new BusinessLogicLayer.DynamicProductList();
                        //DataSet ds = new DataSet();
                        //ds = dpObj.Select_DynamicProducts(franchiseId, (int)blockItems.CategoryID.Value, System.Web.HttpContext.Current.Server);////added cityId->franchiseID
                        ///*Select All Menu By Franchise */
                        //DataTable dt = new DataTable();
                        //dt = ds.Tables[0];

                        //List<CategoryPageViewModel> FMenu = new List<CategoryPageViewModel>();

                        //FMenu = (from n in dt.AsEnumerable()
                        //         select new CategoryPageViewModel
                        //         {
                        //             ID = n.Field<Int32>("ID"),
                        //             CategoryName = n.Field<string>("CategoryName"),
                        //             SequenceOrder = n.Field<int?>("SequenceOrder"),
                        //             Level = n.Field<int>("Level"),
                        //             ParentCategoryID = n.Field<int?>("ParentCategoryID"),
                        //             IsManaged = n.Field<bool>("IsManaged")
                        //         }).OrderBy(x => x.SequenceOrder).ToList();


                        //DataTable dt1 = new DataTable();
                        //dt1 = ds.Tables[1];

                        //List<DynamicProductViewModel> DProducts = new List<DynamicProductViewModel>();

                        //DProducts = (from n in dt1.AsEnumerable()
                        //             select new DynamicProductViewModel
                        //             {
                        //                 ID = n.Field<Int64>("ProductID"),
                        //                 Name = n.Field<string>("Name"),
                        //                 LevelTwoCatID = n.Field<int>("LevelTwoCatID"),
                        //                 ShopStockID = n.Field<Int64>("ShopStockID"),
                        //                 SequenceOrder = n.Field<int?>("SequenceOrder") == null ? 0 : n.Field<int?>("SequenceOrder"),
                        //                 RetailerRate = n.Field<decimal>("RetailerRate"),
                        //                 MRP = n.Field<decimal>("MRP"),
                        //                 RetailPoint = n.Field<decimal>("RetailPoint") //Yashaswi 9-7-2018
                        //             }).ToList();
                        //// }

                        //foreach (DynamicProductViewModel DP in DProducts)
                        //{
                        //    Color color = db.ShopStocks.Where(x => x.ID == DP.ShopStockID).FirstOrDefault().ProductVarient.Color;
                        //    DP.ImagePath = BusinessLogicLayer.ImageDisplay.SetProductThumbPath(DP.ID,
                        //                 ((color.ID == 1 || color.Name == "N/A") ? "default" : color.Name.Trim()),
                        //                 string.Empty, BusinessLogicLayer.ProductUpload.IMAGE_TYPE.Approved);
                        //    string sizeName = db.ShopStocks.Find(DP.ShopStockID).ProductVarient.Size.Name;
                        //    DP.Size = (sizeName.ToUpper().Trim() == "N/A") ? string.Empty : sizeName;
                        //    //Added for SEO URL Structure RULE by AShish
                        //    DP.Name = DP.Name.Replace("+", " ");
                        //    DP.URLStructureName = GetURLStructureName(DP.Name);
                        //}

                        //if (DProducts != null && DProducts.Count > 0)
                        //{
                        //    obj = new { Success = 1, Message = "Record Found.", data = new { CategoryList = FMenu, ProductList = DProducts } };
                        //}
                        //else
                        //{
                        //    obj = new { Success = 0, Message = "Record Not Found.", data = string.Empty };
                        //}

                    }
                    else
                    {
                        ProductSearchViewModel productSearch = new ProductSearchViewModel();
                        productSearch.PageIndex = 1;
                        productSearch.Keyword = blockItems.Keyword != null ? blockItems.Keyword : string.Empty;
                        productSearch.CategoryID = (long)(blockItems.CategoryID.HasValue ? blockItems.CategoryID.Value : 0);
                        productSearch.ProductID = blockItems.ProductID.HasValue ? blockItems.ProductID.Value : 0;
                        productSearch.BrandIDs = blockItems.BrandId.HasValue ? blockItems.BrandId.Value.ToString() : string.Empty;
                        productSearch.ShopID = blockItems.ShopID.HasValue ? blockItems.ShopID.Value : 0;
                        //productSearch.ShopStockIDList = blockItems.ShopStockID.HasValue ? blockItems.ShopStockID.Value : 0;
                        productSearch.CityID = (int)CityId;
                        productSearch.FranchiseID = blockItems.FranchiseId;
                        productSearch.PageSize = 50;//Sonali_03-11-2018_For display all product
                        productSearch.Version = 1;
                        ProductWithRefinementViewModel productWithRefinementViewModel = new ProductWithRefinementViewModel();
                        ProductList productList = new ProductList(System.Web.HttpContext.Current.Server);
                        productWithRefinementViewModel = productList.GetProductList(productSearch, false);
                        // var output = aSearch.GetSearchMetaData(pre, (AutoSearch.SEARCHBY)searchBy, cityID, franchiseID);
                        if (productWithRefinementViewModel != null && productWithRefinementViewModel.productList.Count > 0 && productWithRefinementViewModel.productRefinements.Count > 0)
                            obj = new { Success = 1, Message = "Record Found.", data = productWithRefinementViewModel };
                        else
                            obj = new { Success = 0, Message = "Record Not Found.", data = string.Empty };
                    }
                }
                else if (blockItems.BrandId != null && blockItems.BrandId > 0)
                {
                    //This data fetch is temprorely by Sonali
                    ProductSearchViewModel productSearch = new ProductSearchViewModel();
                    productSearch.CityID = (Int32)CityId;
                    productSearch.FranchiseID = blockItems.FranchiseId;
                    productSearch.ShopID = 0;
                    productSearch.Keyword = db.Brands.Where(x => x.ID == blockItems.BrandId).Select(x => x.Name).FirstOrDefault();
                    productSearch.CategoryID = 0;
                    productSearch.BrandIDs = blockItems.BrandId.ToString();
                    productSearch.ProductID = 0;
                    productSearch.ColorIDs = string.Empty;
                    productSearch.SizeIDs = string.Empty;
                    productSearch.DimensionIDs = string.Empty;
                    productSearch.MaterialIDs = string.Empty;
                    productSearch.SpecificationIDs = string.Empty;
                    productSearch.SpecificationValues = string.Empty;
                    productSearch.MinPrice = 0;
                    productSearch.MaxPrice = 0;
                    productSearch.PageIndex = 1;
                    productSearch.PageSize = 50;
                    productSearch.Version = 1;
                    productSearch.IsScroll = false;
                    productSearch.CustLoginID = 0;
                    productSearch.SearchInCategoryOnly = false;
                    productSearch.ImageType = string.Empty;
                    productSearch.IsVarientRestricted = false;
                    productSearch.isListVarient = false;
                    productSearch.SortVal = 0;
                    ProductWithRefinementViewModel productWithRefinementViewModel = new ProductWithRefinementViewModel();
                    ProductList productList = new ProductList(System.Web.HttpContext.Current.Server);
                    productWithRefinementViewModel = productList.GetProductList(productSearch, false);
                    if (productWithRefinementViewModel != null)
                    {
                        obj = new { Success = 1, Message = "Product list are found.", data = productWithRefinementViewModel };
                    }
                }
                //else if (blockItems.ProductID != null && blockItems.ProductID > 0)
                //{
                //    string clName = "default";
                //    ProductDetails p = new ProductDetails(System.Web.HttpContext.Current.Server);
                //    ProductDetailview detailview = new ProductDetailview();
                //    detailview.ProductbasicDetail = p.GetBasicDetails(Convert.ToInt64(blockItems.ProductID));
                //    detailview.ProductVarient = p.GetStockVarients(Convert.ToInt64(blockItems.ProductID), 0, null, blockItems.FranchiseId);
                //    detailview.Images = ImageDisplay.GetStockImages(Convert.ToInt64(blockItems.ProductID), string.IsNullOrEmpty(clName) || clName == "N/A" ? string.Empty : clName);
                //    detailview.ProductGeneralSpecification = p.GetGeneralDescription(Convert.ToInt64(blockItems.ProductID));
                //    if (!string.IsNullOrEmpty(detailview.ProductGeneralSpecification))
                //    {
                //        var httpWebRequest = (HttpWebRequest)WebRequest.Create(detailview.ProductGeneralSpecification);
                //        //httpWebRequest.ContentType = "application/json";
                //        httpWebRequest.Method = "GET";
                //        var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                //        using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                //        {
                //            var result = streamReader.ReadToEnd();
                //            detailview.ProductGeneralSpecification = result;
                //        };
                //    }
                //    if (detailview.ProductbasicDetail != null && detailview.ProductVarient != null)
                //    {
                //        obj = new { Success = 1, Message = "Record Found.", data = detailview };
                //    }
                //    else
                //    {
                //        obj = new { Success = 0, Message = "Record Not Found.", data = string.Empty };
                //    }
                //}
            }
            catch (Exception ex)
            {
                obj = new { Success = 0, Message = ex.Message, data = string.Empty };
            }
            return obj;
        }

        string AddSuffix(string filename, string suffix)
        {
            string[] test = filename.Split('/');
            if (test != null && test.Count() > 0)
            {
                string newFileName = test[test.Length - 1];
                newFileName = "m" + newFileName;
                filename = filename.Replace(test[test.Length - 1], newFileName);
            }
            return filename;
            //string fDir = Path.GetDirectoryName(filename);
            //string fName = Path.GetFileNameWithoutExtension(filename);
            //string fExt = Path.GetExtension(filename);
            //return Path.Combine(fDir, String.Concat(fName, suffix, fExt));
        }

        ProductWithRefinementViewModel ConversionOfferProductsToProductView(List<OfferProducts> lOfferProducts, int FranchiseId)
        {
            ProductWithRefinementViewModel productWithRefinementViewModel = new ProductWithRefinementViewModel();
            productWithRefinementViewModel.productList = new List<SearchProductDetailsViewModel>();
            foreach (var item in lOfferProducts)
            {
                var brand = (from p in db.Products
                             join b in db.Brands on p.BrandID equals b.ID
                             where p.ID == item.ProductID
                             select new
                             {
                                 ID = b.ID,
                                 name = b.Name
                             }).FirstOrDefault();
                SearchProductDetailsViewModel objSearchProduct = new SearchProductDetailsViewModel();
                objSearchProduct.ProductThumbPath = item.StockSmallImagePath;
                objSearchProduct.ProductID = item.ProductID;
                objSearchProduct.Name = item.ProductName;
                objSearchProduct.CategoryID = item.CategoryID;
                objSearchProduct.CategoryName = item.CategoryName;
                objSearchProduct.MRP = item.MRP;
                objSearchProduct.SaleRate = item.SaleRate;
                objSearchProduct.StockStatus = item.StockStatus;
                objSearchProduct.ShopStockID = item.ShopStockID;
                objSearchProduct.Color = item.ColorName;
                objSearchProduct.PackSize = 0;
                objSearchProduct.PackUnit = string.Empty;
                objSearchProduct.Dimension = item.DimensionName;
                objSearchProduct.Size = item.SizeName;
                objSearchProduct.Material = item.MaterialName;
                objSearchProduct.StockQty = item.StockQty;
                objSearchProduct.HtmlColorCode = string.Empty;
                objSearchProduct.RetailPoint = item.RetailPoint;
                objSearchProduct.CashbackPoint = item.CashbackPoints;
                objSearchProduct.IsDisplayCB = item.IsDisplayCB;
                objSearchProduct.ShopID = item.ShopID;
                objSearchProduct.BrandId = brand.ID;//Added for api sort by Sonali_04-01-2018
                objSearchProduct.BrandName = brand.name;//Added for api sort by Sonali_04-01-2018
                                                        //productuploadtempviewmodel.CategoryID = TP.CategoryID;
                var obj1 = (from n in db.Categories
                            join m in db.Categories on n.ID equals m.ParentCategoryID
                            join p in db.Categories on m.ID equals p.ParentCategoryID
                            where p.ID == item.CategoryID
                            select new
                            {
                                LevelOne = n.ID
                            }).FirstOrDefault();
                if (obj1 != null)
                    objSearchProduct.FirstLevelCatId = Convert.ToInt32(obj1.LevelOne);
                productWithRefinementViewModel.productList.Add(objSearchProduct);
            }

            productWithRefinementViewModel.productRefinements = new List<ProductRefinementsViewModel>();
            foreach (var item in lOfferProducts)
            {
                var brand = (from p in db.Products
                             join b in db.Brands on p.BrandID equals b.ID
                             where p.ID == item.ProductID
                             select new
                             {
                                 ID = b.ID,
                                 name = b.Name
                             }).FirstOrDefault();
                ProductRefinementsViewModel objProductRefinements = new ProductRefinementsViewModel();
                objProductRefinements.BrandID = brand.ID;
                objProductRefinements.BrandName = brand.name;
                objProductRefinements.CategoryID = item.CategoryID;
                objProductRefinements.CategoryName = item.CategoryName;
                // CityID = Convert.To
                objProductRefinements.ColorID = 0;
                objProductRefinements.Color = item.ColorName;
                objProductRefinements.DimensionID = item.DimensionId;
                objProductRefinements.Dimension = item.DimensionName;
                objProductRefinements.MaterialID = item.MaterialId;
                objProductRefinements.Material = item.MaterialName;
                objProductRefinements.MRP = item.MRP;
                objProductRefinements.ProductID = item.ProductID;
                objProductRefinements.ProductName = item.ProductName;
                objProductRefinements.SaleRate = item.SaleRate;
                objProductRefinements.ShopID = item.ShopID;
                objProductRefinements.ShopName = item.ShopName;
                objProductRefinements.SizeID = item.SizeId;
                objProductRefinements.Size = item.SizeName;
                objProductRefinements.SpecificationID = 0;
                objProductRefinements.SpecificationValue = string.Empty;
                objProductRefinements.ProductSpecificationID = 0;
                objProductRefinements.SpecificationName = string.Empty;
                objProductRefinements.PackSize = 0;
                objProductRefinements.PackUnit = item.PackedUnit;
                objProductRefinements.CategoryOrderSequence = 0;
                objProductRefinements.FranchiseID = FranchiseId;
                objProductRefinements.RetailPoint = item.RetailPoint;
                objProductRefinements.CashbackPoint = item.CashbackPoints;
                objProductRefinements.IsDisplayCB = item.IsDisplayCB;
                productWithRefinementViewModel.productRefinements.Add(objProductRefinements);
            }
            productWithRefinementViewModel.searchCount = new SearchCountViewModel();
            List<long> ProductIds = lOfferProducts.Select(x => x.ProductID).ToList();
            long ShopId = db.Shops.Where(x => x.FranchiseID == FranchiseId).Select(x => x.ID).FirstOrDefault();
            List<PrdVarientViewModel> ProdVarientList = (from p in db.Products
                                                         join SP in db.ShopProducts on p.ID equals SP.ProductID
                                                         join S in db.Shops on SP.ShopID equals S.ID
                                                         join pin in db.Pincodes on S.PincodeID equals pin.ID
                                                         join SS in db.ShopStocks on SP.ID equals SS.ShopProductID
                                                         join U in db.Units on SS.PackUnitID equals U.ID
                                                         join PV in db.ProductVarients on new { ProductId = p.ID, VarientId = SS.ProductVarientID } equals new { ProductId = PV.ProductID, VarientId = PV.ID }
                                                         join BD in db.Brands on p.BrandID equals BD.ID
                                                         join C in db.Colors on PV.ColorID equals C.ID
                                                         join SI in db.Sizes on PV.SizeID equals SI.ID
                                                         join D in db.Dimensions on PV.DimensionID equals D.ID
                                                         join M in db.Materials on PV.MaterialID equals M.ID
                                                         where S.FranchiseID == FranchiseId && PV.IsActive == true && S.IsLive == true &&
                                                         SP.IsActive == true && pin.IsActive == true && SS.RetailerRate > 0 && SS.IsActive == true &&
                                                         p.IsActive == true && SS.WarehouseStockID != null && SS.Qty > 0 && ProductIds.Contains(PV.ProductID) && S.ID == ShopId
                                                         select new PrdVarientViewModel
                                                         {
                                                             BrandId = BD.ID,
                                                             CategoryID = p.CategoryID,
                                                             BrandName = BD.Name,
                                                             ShopStockID = SS.ID,
                                                             ShopID = (Int32)S.ID,
                                                             MRP = SS.MRP,
                                                             SaleRate = SS.RetailerRate,
                                                             ProductID = p.ID,
                                                             RetailPoint = SS.BusinessPoints,
                                                             CashbackPoint = SS.CashbackPoints,
                                                             Size = PV.Size.Name,
                                                             SizeID = PV.SizeID,
                                                             StockQty = SS.Qty,
                                                             IsDisplayCB = 1,
                                                             StockStatus = SS.StockStatus ? 1 : 0
                                                         }).ToList();

            if (productWithRefinementViewModel.productList != null && productWithRefinementViewModel.productList.Count > 0)
            {
                foreach (var item in productWithRefinementViewModel.productList)
                {
                    if (lOfferProducts.Where(x => x.ProductID == item.ProductID && x.ShopID == item.ShopID).ToList().Count > 0)
                    {
                        item.ProductVarientViewModels = ProdVarientList.Where(x => x.ProductID == item.ProductID && x.ShopID == item.ShopID).ToList();
                    }
                }
                productWithRefinementViewModel.searchCount.PageCount = 1;
                productWithRefinementViewModel.searchCount.ProductCount = productWithRefinementViewModel.productList.Count();
            }
            return productWithRefinementViewModel;
        }


    }
}


public class HomePage
{
    public List<BannerImage> BannerList { get; set; }
    public HotDeals HotDeals { get; set; }
    public SeasonSpecial SeasonSpecial { get; set; }
    public MajorRetailPoint MajorRetailpoint { get; set; }
    //public List<SearchProductDetailsViewModel> MajorRetailPointList { get; set; }
    public DealOfDay DealsOfDay { get; set; }
    public Deals_48Hrs Deals_48 { get; set; }
    public List<ShopByBrand> BrandList { get; set; }
    public NewlyLaunch NewlyLaunch { get; set; }
    public TrendingDeals TrendingDeals { get; set; }
    public string Logo { get; set; }
    public string ReferEarn { get; set; }
}

public class BannerImage
{
    public long BlockItemId { get; set; }
    public string Image { get; set; }
    //public string LinkUrl { get; set; }
    public long? Id { get; set; }
    public string Type { get; set; }
}
public class DealBanner
{
    public long DealBannerId { get; set; }
    public string Image { get; set; }
    public long? Id { get; set; }
    public string Type { get; set; }
}

public class SeasonSpecial
{
    //public List<OfferProducts> ProductList { get; set; }
    //public List<OfferCategoryList> CategoryList { get; set; }
    //public bool IsCategory { get; set; }
    //public bool IsProduct { get; set; }
    //public bool IsBanner { get; set; }
    public List<DealBanner> BannerList { get; set; }
}
public class HotDeals
{
    public List<OfferProducts> ProductList { get; set; }
    // public List<OfferCategoryList> CategoryList { get; set; }
    //public bool IsCategory { get; set; }
    //public bool IsProduct { get; set; }
    //public bool IsBanner { get; set; }
    public List<DealBanner> BannerList { get; set; }
}

public class MajorRetailPoint
{
    public List<DealBanner> BannerList { get; set; }
    public List<OfferProducts> ProductList { get; set; }
}

public class DealOfDay
{
    public List<OfferProducts> ProductList { get; set; }
    //public List<OfferCategoryList> OfferCategoryList { get; set; }
    //public bool IsCategory { get; set; }
    //public bool IsProduct { get; set; }
    //public bool IsBanner { get; set; }
    public List<DealBanner> BannerList { get; set; }
}

public class Deals_48Hrs
{
    //public List<Category> CategoryList { get; set; }
    public List<OfferProducts> ProductList { get; set; }
    public List<DealBanner> BannerList { get; set; }
    //public bool IsCategory { get; set; }
    //public bool IsProduct { get; set; }
    //public bool IsBanner { get; set; }
}

public class ShopByBrand
{
    public int BrandId { get; set; }
    public string ImageName { get; set; }
    public string Name { get; set; }
}

public class NewlyLaunch
{
    //public List<Category> CategoryList { get; set; }
    public List<OfferProducts> ProductList { get; set; }
    public List<DealBanner> BannerList { get; set; }
    //public bool IsCategory { get; set; }
    //public bool IsProduct { get; set; }
    //public bool IsBanner { get; set; }
}

public class TrendingDeals
{
    //public List<Category> CategoryList { get; set; }
    public List<OfferProducts> ProductList { get; set; }
    public List<DealBanner> BannerList { get; set; }
    //public bool IsCategory { get; set; }
    //public bool IsProduct { get; set; }
    //public bool IsBanner { get; set; }
}