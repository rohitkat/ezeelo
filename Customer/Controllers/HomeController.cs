using BusinessLogicLayer;
using Gandhibagh.Models;
using ModelLayer.Models;
using ModelLayer.Models.ViewModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.SqlServer;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Gandhibagh.Controllers
{
    public class HomeController : Controller
    {
        EzeeloDBContext db = new EzeeloDBContext();
        Int64 franchiseIDMsg = 0; //// Initialise for Dynamic Messages

        //Added by Zubair on 16-09-2017 for filling CityCookie for SEO Schema
        //if link is directly hitted from google search
        public class GetCityIdFranchiseIDContact4 ////added GetCityIdFranchiseIDContact
        {
            public long cityId { get; set; }
            public int franchiseId { get; set; }
            public string contact { get; set; }
        }
        //End by Zubair

        
        public ActionResult Index()
        {
            //Set Cookie for Url saving & Use in Continue shopping
            URLCookie.SetCookies();

            try
            {

                Int64 cityVal = 0;
                Int64 franchiseVal = 0;////added
                Int64 helpLineNoVal = 0;////added



                if (ControllerContext.HttpContext.Request.Cookies["CityCookie"] == null || ControllerContext.HttpContext.Request.Cookies["CityCookie"].Value == string.Empty)
                {
                    //Yashaswi 01/12/2018 Default City Change 
                    cityVal = Convert.ToInt64(URLsFromConfig.GetDefaultData("CITY_ID"));
                    franchiseVal = Convert.ToInt64(URLsFromConfig.GetDefaultData("FRANCHISE_ID"));
                    helpLineNoVal = Convert.ToInt64(URLsFromConfig.GetDefaultData("HELPLINE_NO"));
                    franchiseIDMsg = franchiseVal;

                    //ControllerContext.HttpContext.Response.Cookies["CityCookie"].Value = "4968$Nagpur";
                    //ControllerContext.HttpContext.Response.Cookies["CityCookie"].Expires = System.DateTime.Now.AddDays(30);
                    //ControllerContext.HttpContext.Response.Cookies.Add(ControllerContext.HttpContext.Response.Cookies["CityCookie"]);
                }
                else
                {
                    //-- Differentiate between Old and New Cookie --//
                    Int64 length = ControllerContext.HttpContext.Request.Cookies["CityCookie"].Value.Split('$').Length;
                    if (length != 4)
                    {
                        ControllerContext.HttpContext.Request.Cookies["CityCookie"].Value = "";
                        ControllerContext.HttpContext.Request.Cookies["CityCookie"].Value = URLsFromConfig.GetDefaultData("CITY_ID") + "$" + URLsFromConfig.GetDefaultData("CITY_NAME") + "$" + URLsFromConfig.GetDefaultData("FRANCHISE_ID") + "$" + URLsFromConfig.GetDefaultData("HELPLINE_NO"); //Yashaswi 01/12/2018 Default City Change
                    }
                    else
                    {
                        Int64.TryParse(ControllerContext.HttpContext.Request.Cookies["CityCookie"].Value.Split('$')[0], out cityVal);
                        Int64.TryParse(ControllerContext.HttpContext.Request.Cookies["CityCookie"].Value.Split('$')[2], out franchiseVal);////added
                        Int64.TryParse(ControllerContext.HttpContext.Request.Cookies["CityCookie"].Value.Split('$')[3], out helpLineNoVal);////added
                    }
                }
                //-- Add by Ashish for Dynamic Weekly Holiday Message --//

                List<WeeklySeasonalFestivalPageMessage> WSFMsg1 = new List<WeeklySeasonalFestivalPageMessage>();
                WSFMsg1 = CallGetPageMessageAPI();
                ViewBag.FestivalMessage = WSFMsg1.Where(x => x.MessageType == "Festival").Select(i => i.Message).ToList();
                ViewBag.FestivalFrmDate = WSFMsg1.Where(x => x.MessageType == "Festival").Select(i => i.FestivalMsgFrmDate).ToList();
                ViewBag.FestivalToDate = WSFMsg1.Where(x => x.MessageType == "Festival").Select(i => i.FestivalMsgToDate).ToList();
                ViewBag.SeasonalMessage = WSFMsg1.Where(x => x.MessageType == "Seasonal").Select(i => i.Message).ToList();
                ViewBag.SeasonalFrmMonth = WSFMsg1.Where(x => x.MessageType == "Seasonal").Select(i => i.SeasonalMsgFrmMonth).ToList();
                ViewBag.SeasonalToMonth = WSFMsg1.Where(x => x.MessageType == "Seasonal").Select(i => i.SeasonalMsgToMonth).ToList();
                ViewBag.WeeklyMessage = WSFMsg1.Where(x => x.MessageType == "Weekly").Select(i => i.Message).ToList();
                ViewBag.WeeklyDay = WSFMsg1.Where(x => x.MessageType == "Weekly").Select(i => i.WeeklyHoliday).ToList();
                //-- End Add by Ashish for Dynamic Weekly Holiday Message --//
                if (franchiseVal > 1) //cityVal
                {

                    // DataSet ds = BusinessLogicLayer.HomePageBlockItemsList.GetHomeIndexItemList(cityVal, System.Web.HttpContext.Current.Server);////hide
                    DataSet ds = BusinessLogicLayer.HomePageBlockItemsList.GetHomeIndexItemList(franchiseVal, System.Web.HttpContext.Current.Server);////added

                    /*Select All Menu By Franchise */
                    DataTable dt = new DataTable();
                    dt = ds.Tables[0];


                    List<BlockViewModel> lBlockTypes = new List<BlockViewModel>();
                    lBlockTypes = (from n in dt.AsEnumerable()
                                   select new BlockViewModel
                                   {
                                       ID = n.Field<Int64>("ID"),
                                       Name = n.Field<string>("Name"),
                                       ImageWidth = n.Field<decimal>("ImageWidth"),
                                       ImageHeight = n.Field<decimal>("ImageHeight"),
                                       MaxLimit = n.Field<int>("MaxLimit"),
                                       IsActive = n.Field<bool>("IsActive")
                                   }).OrderBy(x => x.Name).ToList();

                    BusinessLogicLayer.ReadConfig rcKey = new BusinessLogicLayer.ReadConfig(System.Web.HttpContext.Current.Server);
                    DataTable dt1 = new DataTable();
                    dt1 = ds.Tables[1];

                    DataView dv = new DataView(dt1);
                    foreach (BlockViewModel B in lBlockTypes)
                    {
                        //int max = 0;
                        //int.TryParse(Convert.ToString(B.MaxLimit), out max);

                        dv.RowFilter = "DesignBlockTypeID=" + B.ID;
                        B.blockItemsList = (from n in dv.ToTable().AsEnumerable()
                                            select new HomePageBlockItemsViewModel
                                            {
                                                ID = n.Field<Int64>("ID"),
                                                ImageName = rcKey.HOME_IMAGE_HTTP + n.Field<string>("ImageName"),
                                                LinkUrl = n.Field<string>("LinkUrl"),
                                                Tooltip = n.Field<string>("Tooltip"),
                                                SequenceOrder = n.Field<int>("SequenceOrder"),
                                                IsActive = n.Field<bool>("IsActive")
                                            }).OrderBy(x => x.SequenceOrder).ToList();
                    }

                    TempData.Remove("DynamicBlocks");
                    TempData.Add("DynamicBlocks", lBlockTypes.Where(x => x.Name.ToLower().Trim() == "logo" || x.Name.ToLower().Trim() == "navigation bar").ToList());


                    if (ds.Tables[2].Rows.Count > 0)
                    {

                        DataTable dt2 = new DataTable();
                        dt2 = ds.Tables[2];
                        lBlockTypes.Where(x => x.Name.ToLower() == "product gallery").FirstOrDefault().blockItemsList = (from n in dt2.AsEnumerable()
                                                                                                                         select new HomePageBlockItemsViewModel
                                                                                                                         {
                                                                                                                             ID = n.Field<Int64>("ID"),
                                                                                                                             LinkUrl = n.Field<string>("LinkUrl"),
                                                                                                                             Tooltip = n.Field<string>("Tooltip"),
                                                                                                                             SequenceOrder = n.Field<int>("SequenceOrder"),
                                                                                                                             IsActive = n.Field<bool>("IsActive"),
                                                                                                                             ProductID = n.Field<Int64?>("ProductID"),
                                                                                                                             // ProductName = n.Field<string>("Name"),
                                                                                                                             ProductName = n.Field<string>("Name").Replace("+", " "),//Added for SEO URL Structure RULE by AShish
                                                                                                                             ShopStockID = n.Field<Int64?>("ShopStockID"),
                                                                                                                             RetailerRate = n.Field<decimal?>("RetailerRate"),
                                                                                                                             MRP = n.Field<decimal?>("MRP"),
                                                                                                                             ImageName = (n.Field<string>("ColorName").ToLower().Trim() == "n/a") ? ImageDisplay.SetProductThumbPath((Int64)n.Field<Int64?>("ProductID"), "default", string.Empty, ProductUpload.IMAGE_TYPE.Approved) : ImageDisplay.SetProductThumbPath((Int64)n.Field<Int64?>("ProductID"), n.Field<string>("ColorName").ToLower().Trim(), string.Empty, ProductUpload.IMAGE_TYPE.Approved),
                                                                                                                             URLStructureName = GetURLStructureName(n.Field<string>("Name")),//Added for SEO URL Structure RULE by AShish
                                                                                                                             Size = db.ShopStocks.Find(n.Field<Int64?>("ShopStockID")).ProductVarient.Size.Name, //Added Size on Home page 
                                                                                                                             RetailPoint = n.Field<decimal>("RetailPoint"),  //Yashaswi 7-7-2018
                                                                                                                             CashbackPoint = n.Field<decimal>("CashbackPoint")
                                                                                                                         }).OrderBy(x => x.SequenceOrder).ToList();
                    }
                    // Added by Yashaswi 20-2-2019 For Home Page Dynamic Sections
                    // Execute stored procedure 'SP_HomePageDynamicSections' and return HomePageDynamicSection
                    // Stored procedure return dataset contain 3 Table
                    // 1. HomePageDynamicSection
                    // 2. HomePageDynamicSectionBanners
                    // 3. HomePageDynamicSectionProduct

                    DataSet ds_HomeDynamicSections = BusinessLogicLayer.HomePageBlockItemsList.GetHomeDynamicSections(franchiseVal, System.Web.HttpContext.Current.Server);////added
                    List<HomePageDynamicViewModel> HomePageDynamicViewModels = new List<HomePageDynamicViewModel>();
                    List<HomePageDynamicBannerViewModel> HomePageDynamicBannerViewModel = new List<HomePageDynamicBannerViewModel>();
                    List<HomePageBlockItemsViewModel> HomePageBlockItemsViewModel = new List<HomePageBlockItemsViewModel>();
                    if (ds_HomeDynamicSections.Tables.Count >= 1)
                    {
                        //Select Home Page Dynamic Sections
                        DataTable dt_DynamicSections = new DataTable();

                        dt_DynamicSections = ds_HomeDynamicSections.Tables[0];
                        HomePageDynamicViewModels = (from n in dt_DynamicSections.AsEnumerable()
                                                     select new HomePageDynamicViewModel
                                                     {
                                                         SectionId = n.Field<Int32>("SectionId"),
                                                         HomePageDynamicSectionsId = n.Field<Int64>("HomePageDynamicSectionsId"),
                                                         SectionDisplayName = n.Field<string>("SectionDisplayName"),
                                                         IsBanner = n.Field<bool>("IsBanner"),
                                                         IsCategory = n.Field<bool>("IsCategory"),
                                                         IsProduct = n.Field<bool>("IsProduct"),
                                                         SequenceOrder = n.Field<int>("SequenceOrder"),
                                                         SectionStyle = n.Field<string>("SectionStyle")
                                                     }).OrderBy(x => x.SectionId).ToList();
                    }
                    if (ds_HomeDynamicSections.Tables.Count >= 2)
                    {
                        //Select Home Page Dynamic Sections Banners and categories Image
                        DataTable dt_DynamicSectionBanner = new DataTable();
                        dt_DynamicSectionBanner = ds_HomeDynamicSections.Tables[1];

                        HomePageDynamicBannerViewModel = (from n in dt_DynamicSectionBanner.AsEnumerable()
                                                          select new HomePageDynamicBannerViewModel
                                                          {
                                                              SectionId = n.Field<int>("SectionId"),
                                                              HomePageDynamicSectionsId = n.Field<Int64>("HomePageDynamicSectionsId"),
                                                              ImageName = n.Field<string>("ImageName"),
                                                              SequenceOrder = n.Field<int>("SequenceOrder"),
                                                              ToolTip = n.Field<string>("ToolTip"),
                                                              LinkURL = n.Field<string>("LinkURL"),
                                                              IsBanner = n.Field<bool>("IsBanner"),
                                                              CreateDate = n.Field<DateTime>("CreateDate"),
                                                          }).OrderBy(x => x.SectionId).ToList();
                    }
                    if (ds_HomeDynamicSections.Tables.Count >= 3)
                    {
                        //Select Home Page Dynamic Sections wise product list
                        DataTable dt_DynamicSectionProduct = new DataTable();
                        dt_DynamicSectionProduct = ds_HomeDynamicSections.Tables[2];

                        HomePageBlockItemsViewModel = (from n in dt_DynamicSectionProduct.AsEnumerable()
                                                       select new HomePageBlockItemsViewModel
                                                       {
                                                           SectionId = n.Field<int>("SectionId"),
                                                           HomePageDynamicSectionsId = n.Field<Int64>("HomePageDynamicSectionsId"),
                                                           SequenceOrder = n.Field<int>("SequenceOrder"),
                                                           ProductID = n.Field<Int64?>("ProductID"),
                                                           ProductName = n.Field<string>("Name").Replace("+", " "),
                                                           ShopStockID = n.Field<Int64?>("ShopStockId"),
                                                           RetailerRate = n.Field<decimal?>("RetailerRate"),
                                                           MRP = n.Field<decimal?>("MRP"),
                                                           ImageName = (n.Field<string>("ColorName").ToLower().Trim() == "n/a") ? ImageDisplay.SetProductThumbPath((Int64)n.Field<Int64?>("ProductID"), "default", string.Empty, ProductUpload.IMAGE_TYPE.Approved) : ImageDisplay.SetProductThumbPath((Int64)n.Field<Int64?>("ProductID"), n.Field<string>("ColorName").ToLower().Trim(), string.Empty, ProductUpload.IMAGE_TYPE.Approved),
                                                           URLStructureName = GetURLStructureName(n.Field<string>("Name")),
                                                           Size = db.ShopStocks.Find(n.Field<Int64?>("ShopStockId")).ProductVarient.Size.Name,
                                                           RetailPoint = n.Field<decimal>("RetailPoint"),
                                                           CashbackPoint = n.Field<decimal>("CashbackPoint"),
                                                           LinkUrl = "",
                                                           Tooltip = "",
                                                           IsActive = true
                                                       }).OrderBy(x => x.SequenceOrder).ToList();

                    }
                    BlockViewModel obj = new BlockViewModel();
                    obj.Name = "DynamicSections";
                    obj.IsActive = true;
                    lBlockTypes.Add(obj);

                    lBlockTypes.Where(x => x.Name.ToLower() == "dynamicsections").FirstOrDefault().listHomePageDynamic = HomePageDynamicViewModels;
                    lBlockTypes.Where(x => x.Name.ToLower() == "dynamicsections").FirstOrDefault().listHomePageDynamicBanners = HomePageDynamicBannerViewModel;
                    lBlockTypes.Where(x => x.Name.ToLower() == "dynamicsections").FirstOrDefault().listHomePageDynamicProducts = HomePageBlockItemsViewModel;

                    //End by Yashaswi 20-2-2019 For Home Page Dynamic Sections

                    return View(lBlockTypes);
                }
                else
                {
                    string CityName = db.Cities.FirstOrDefault(p => p.ID == cityVal).Name.ToLower();
                    return Redirect(System.Configuration.ConfigurationManager.AppSettings["EZEELO_CUSTOMER_URL"] + CityName +"/"+ franchiseVal + "/Merchant/List");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            
        }
        public ActionResult Index_varanasi()
        {
            return View();
        }
        public ActionResult Index_kanpur()
        {
            return View();
        }

        public ActionResult GetMenuListing()
        {
            long cityId = 0;

            int franchiseId = 0;////added
            if (ControllerContext.HttpContext.Request.Cookies["CityCookie"] != null && ControllerContext.HttpContext.Request.Cookies["CityCookie"].Value != string.Empty)
            {
                cityId = Convert.ToInt32(ControllerContext.HttpContext.Request.Cookies["CityCookie"].Value.Split('$')[0].Trim());
                franchiseId = Convert.ToInt32(ControllerContext.HttpContext.Request.Cookies["CityCookie"].Value.Split('$')[2].Trim());////added
            }

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

            var levelOneMenu = FMenu.Where(x => x.Level == 1).ToList().OrderBy(x => x.SequenceOrder);
            foreach (MenuViewModel M in levelOneMenu)
            {
                M.LevelTwoListing = FMenu.Where(x => x.Level == 2 && x.ParentCategoryID == M.ID).ToList();
                foreach (MenuViewModel M2 in M.LevelTwoListing)
                {
                    M2.LevelThreeListing = FMenu.Where(x => x.Level == 3 && x.ParentCategoryID == M2.ID).ToList();
                }
            }
            TempData.Remove("Menu");
            TempData.Add("Menu", levelOneMenu.OrderBy(x => x.SequenceOrder));
            return PartialView("_Menu", levelOneMenu.OrderBy(x => x.SequenceOrder));
        }

        //Added by Shaili Khatri on 09-07-2019
        public ActionResult GetServiceMenu()
        {
            EzeeloDBContext db = new EzeeloDBContext();
            //get list of merchant in selected 
            long cityId = 0;

            if (ControllerContext.HttpContext.Request.Cookies["CityCookie"] != null && ControllerContext.HttpContext.Request.Cookies["CityCookie"].Value != string.Empty)
            {
                cityId = Convert.ToInt32(ControllerContext.HttpContext.Request.Cookies["CityCookie"].Value.Split('$')[0].Trim());
            }
            DateTime today = DateTime.UtcNow.AddHours(5.30);
            List<long?> serviceIds = db.Merchants.Where(m => m.City == cityId && m.Status.ToLower() == "approve" && (m.ApproveDate != null && SqlFunctions.DateAdd("year", m.ValidityPeriod.Value, m.ApproveDate.Value) >= today)).Select(m => m.Category).ToList();

            List<ServiceMaster> serviceMasters = db.ServiceMasters.Where(s => s.IsActive == true && serviceIds.Contains(s.Id)).OrderBy(s => s.Name).ToList();
            return PartialView("_ServiceMenu", serviceMasters);
        }

        public ActionResult GetFooterMenuListing(decimal Width)
        {
            if (Width > 800)
            {
                var levelOneMenu = TempData["Menu"];
                return PartialView("_FooterCategoryMenu", levelOneMenu);
            }
            else
            {
                var levelOneMenu = TempData["Menu"];
                return PartialView("_FooterCategoryForMobile", levelOneMenu);
            }
        }

        [SessionExpire]
        [DynamicMetaTag]
        public ActionResult CategoryPage(int CatID, int? secLvlCatID)//doubt
        {
            try
            {
                URLCookie.SetCookies();

                long cityId = 0;
                int franchiseID = 0;////added

                if (ControllerContext.HttpContext.Request.Cookies["CityCookie"] != null && ControllerContext.HttpContext.Request.Cookies["CityCookie"].Value != string.Empty)
                {
                    cityId = Convert.ToInt32(ControllerContext.HttpContext.Request.Cookies["CityCookie"].Value.Split('$')[0].Trim());
                    franchiseID = Convert.ToInt32(ControllerContext.HttpContext.Request.Cookies["CityCookie"].Value.Split('$')[2].Trim());////added
                }
                else
                {
                    //Added by Zubair on 16-09-2017 for filling CityCookie for SEO Schema
                    //if link is directly hitted from google search
                    //string cname = Request.Url.ToString().Replace("http://localhost:5555/", "");
                    string cname = Request.Url.AbsoluteUri.ToString().Replace("" + (new URLsFromConfig()).GetURL("CUSTOMER") + "", "");
                    string cityName = cname.Substring(0, cname.IndexOf('/'));
                    ArrayList al = new ArrayList(cname.Split('/'));
                    franchiseID = Convert.ToInt32(al[1]);

                    var CityIDFranchiseIdContact = (from f in db.Franchises
                                                    join bd in db.BusinessDetails on f.BusinessDetailID equals bd.ID
                                                    join ul in db.UserLogins on bd.UserLoginID equals ul.ID
                                                    join pc in db.Pincodes on bd.PincodeID equals pc.ID
                                                    join c in db.Cities on pc.CityID equals c.ID
                                                    join hd in db.HelpDeskDetails on f.ID equals hd.FranchiseID into LOJ
                                                    from hd in LOJ.DefaultIfEmpty()//-- For Left Outer Join --//
                                                    where f.ID != 1 && f.IsActive == true && c.Name.ToLower().Trim() == cityName.ToLower().Trim()
                                                          && f.ID == franchiseID && ul.IsLocked == false && c.IsActive == true
                                                    select new GetCityIdFranchiseIDContact4
                                                    {
                                                        cityId = c.ID,
                                                        franchiseId = f.ID,
                                                        contact = hd.HelpLineNumber
                                                    }).FirstOrDefault();

                    if (CityIDFranchiseIdContact != null)
                    {
                        cityId = Convert.ToInt32(CityIDFranchiseIdContact.cityId);
                        string contact = CityIDFranchiseIdContact.contact;
                        string City = cityId + "$" + cityName + "$" + franchiseID + "$" + contact;
                        //HttpCookie cookie = Request.Cookies["CityCookie"];//Get the existing cookie by cookie name.
                        HttpCookie cookie = new HttpCookie("CityCookie");
                        cookie.Value = Convert.ToString(City);
                        cookie.Expires = System.DateTime.Now.AddDays(7);
                        Response.SetCookie(cookie); //SetCookie() is used for update the cookie.
                        string cookieValue = ControllerContext.HttpContext.Request.Cookies["CityCookie"].Value;

                        //cityId = Convert.ToInt32(ControllerContext.HttpContext.Request.Cookies["CityCookie"].Value.Split('$')[0].Trim());
                        franchiseID = Convert.ToInt32(ControllerContext.HttpContext.Request.Cookies["CityCookie"].Value.Split('$')[2].Trim());////added
                    }
                }
                //End by Zubair

                //to set title of a page
                // int franchiseID = db.Franchises.Where(x => x.BusinessDetail.Pincode.CityID == cityId && x.ID != 1).FirstOrDefault().ID;////hide
                ViewBag.CatName = db.FranchiseMenus.Where(x => x.CategoryID == CatID && x.FranchiseID == franchiseID).FirstOrDefault().CategoryName;

                BusinessLogicLayer.ReadConfig rcKey = new BusinessLogicLayer.ReadConfig(System.Web.HttpContext.Current.Server);
                BusinessLogicLayer.DynamicProductList obj = new BusinessLogicLayer.DynamicProductList();
                DataSet ds = new DataSet();
                ds = obj.Select_DynamicProducts(franchiseID, CatID, System.Web.HttpContext.Current.Server);////added cityId->franchiseID

                /*Select All Menu By Franchise */
                DataTable dt = new DataTable();
                dt = ds.Tables[0];

                List<CategoryPageViewModel> FMenu = new List<CategoryPageViewModel>();
                //If Condition Added by Tejaswee for second level category page -URL structure
                if (secLvlCatID > 0)
                {
                    FMenu = (from n in dt.AsEnumerable()
                             select new CategoryPageViewModel
                             {
                                 ID = n.Field<Int32>("ID"),
                                 CategoryName = n.Field<string>("CategoryName"),
                                 SequenceOrder = n.Field<int?>("SequenceOrder"),
                                 Level = n.Field<int>("Level"),
                                 ParentCategoryID = n.Field<int?>("ParentCategoryID"),
                                 IsManaged = n.Field<bool>("IsManaged")
                             }).Where(x => x.ID == secLvlCatID || x.ParentCategoryID == secLvlCatID).OrderBy(x => x.SequenceOrder).ToList();

                }
                else
                {
                    FMenu = (from n in dt.AsEnumerable()
                             select new CategoryPageViewModel
                             {
                                 ID = n.Field<Int32>("ID"),
                                 CategoryName = n.Field<string>("CategoryName"),
                                 SequenceOrder = n.Field<int?>("SequenceOrder"),
                                 Level = n.Field<int>("Level"),
                                 ParentCategoryID = n.Field<int?>("ParentCategoryID"),
                                 IsManaged = n.Field<bool>("IsManaged")
                             }).OrderBy(x => x.SequenceOrder).ToList();

                }
                DataTable dt1 = new DataTable();
                dt1 = ds.Tables[1];

                List<DynamicProductViewModel> DProducts = new List<DynamicProductViewModel>();
                //If Condition Added by Tejaswee for second level category page - URL structure
                if (secLvlCatID > 0)
                {
                    DProducts = (from n in dt1.AsEnumerable()
                                 select new DynamicProductViewModel
                                 {
                                     ID = n.Field<Int64>("ProductID"),
                                     Name = n.Field<string>("Name"),
                                     LevelTwoCatID = n.Field<int>("LevelTwoCatID"),
                                     ShopStockID = n.Field<Int64>("ShopStockID"),
                                     SequenceOrder = n.Field<int?>("SequenceOrder") == null ? 0 : n.Field<int?>("SequenceOrder"),
                                     RetailerRate = n.Field<decimal>("RetailerRate"),
                                     MRP = n.Field<decimal>("MRP"),
                                     RetailPoint = n.Field<decimal>("RetailPoint"), //Yashaswi 9-7-2018
                                     CashbackPoint = n.Field<decimal>("CashbackPoint")
                                 }).Where(x => x.LevelTwoCatID == secLvlCatID).ToList();
                }
                else
                {
                    DProducts = (from n in dt1.AsEnumerable()
                                 select new DynamicProductViewModel
                                 {
                                     ID = n.Field<Int64>("ProductID"),
                                     Name = n.Field<string>("Name"),
                                     LevelTwoCatID = n.Field<int>("LevelTwoCatID"),
                                     ShopStockID = n.Field<Int64>("ShopStockID"),
                                     SequenceOrder = n.Field<int?>("SequenceOrder") == null ? 0 : n.Field<int?>("SequenceOrder"),
                                     RetailerRate = n.Field<decimal>("RetailerRate"),
                                     MRP = n.Field<decimal>("MRP"),
                                     RetailPoint = n.Field<decimal>("RetailPoint"), //Yashaswi 9-7-2018
                                     CashbackPoint = n.Field<decimal>("CashbackPoint")
                                 }).ToList();
                }

                foreach (DynamicProductViewModel DP in DProducts)
                {
                    Color color = db.ShopStocks.Where(x => x.ID == DP.ShopStockID).FirstOrDefault().ProductVarient.Color;
                    DP.ImagePath = BusinessLogicLayer.ImageDisplay.SetProductThumbPath(DP.ID,
                                 ((color.ID == 1 || color.Name == "N/A") ? "default" : color.Name.Trim()),
                                 string.Empty, BusinessLogicLayer.ProductUpload.IMAGE_TYPE.Approved);
                    string sizeName = db.ShopStocks.Find(DP.ShopStockID).ProductVarient.Size.Name;
                    DP.Size = (sizeName.ToUpper().Trim() == "N/A") ? string.Empty : sizeName;
                    //Added for SEO URL Structure RULE by AShish
                    DP.Name = DP.Name.Replace("+", " ");
                    DP.URLStructureName = GetURLStructureName(DP.Name);
                }


                var levelTwoMenu = FMenu.Where(x => x.Level == 2).ToList();
                foreach (CategoryPageViewModel M2 in levelTwoMenu)
                {
                    M2.LevelThreeListing = FMenu.Where(x => x.Level == 3 && x.ParentCategoryID == M2.ID).ToList();

                    if (DProducts.Where(x => x.LevelTwoCatID == M2.ID).Count() > 0)
                    {
                        M2.ProductListing = DProducts.Where(x => x.LevelTwoCatID == M2.ID).OrderBy(x => x.SequenceOrder).ToList();
                    }
                }

                ViewBag.CatDescription = CommonFunctions.GetCategoryDescription(db.Categories.Find(CatID).Name, SEOManagement.CATEGEORY_LEVEL.LEVEL1);
                //============== Manoj ======================
                ViewBag.SessionValue = Convert.ToInt64(Session["UID"]);
                return View(levelTwoMenu.OrderBy(x => x.SequenceOrder));
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[HomeController][GET:CategoryPage]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Gandhibagh, System.Web.HttpContext.Current.Server);
                return View("SomethingWrong");
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[HomeController][GET:CategoryPage]",
                    BusinessLogicLayer.ErrorLog.Module.Gandhibagh, System.Web.HttpContext.Current.Server);
                return View("SomethingWrong");
            }
        }

        /// <summary>
        /// Added for URL Structure RULE by AShish
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        public string GetURLStructureName(string Name)
        {
            string str = Name;
            str = System.Text.RegularExpressions.Regex.Replace(str, @"[\\\#$~%.':*?<>{} ]", " ").Replace("&", "and");
            str = System.Text.RegularExpressions.Regex.Replace(str, @"\s+/g", " ");
            // string[] parts2 = Regex.Split(str, @"\+\/\-\,\(\)");
            ///////////////////
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

            //////////////////
            /* string remaining = "";
             string concat = "";
             string[] strSplit = str.Split('+');
             if (!String.IsNullOrEmpty(strSplit[0]) && strSplit.Length >= 2)
             {
                 if (concat.Length + strSplit[0].Length + 1 <= 30)
                 { concat = concat.Length == 0 ? strSplit[0].Trim() : concat + ' ' + strSplit[0].Trim(); }
             }


             if (strSplit.Length >= 2)
             { remaining = strSplit[1]; }
             else { remaining = strSplit[0].Trim(); }
             strSplit = null;
             strSplit = remaining.Split('/');
             if (!String.IsNullOrEmpty(strSplit[0]) && strSplit.Length >= 2)
             {
                 if (concat.Length + strSplit[0].Length + 1 <= 30)
                 { concat = concat.Length == 0 ? strSplit[0].Trim() : concat + ' ' + strSplit[0].Trim(); }
             }
             if (concat.Length <= 30)
             {
                 remaining = "";
                 if (strSplit.Length >= 2)
                 { remaining = strSplit[1]; }
                 else { remaining = strSplit[0].Trim(); }

                 strSplit = null;
                 strSplit = remaining.Split('-');
                 if (!String.IsNullOrEmpty(strSplit[0]) && strSplit.Length >= 2)
                 {
                     if (concat.Length + strSplit[0].Length + 1 <= 30)
                     { concat = concat.Length == 0 ? strSplit[0].Trim() : concat + ' ' + strSplit[0].Trim(); }

                 }
                 if (concat.Length <= 30)
                 {
                     remaining = "";
                     if (strSplit.Length >= 2)
                     { remaining = strSplit[1]; }
                     else { remaining = strSplit[0].Trim(); }

                     strSplit = null;
                     strSplit = remaining.Split(',');
                     if (!String.IsNullOrEmpty(strSplit[0]) && strSplit.Length >= 2)
                     {
                         if (concat.Length + strSplit[0].Length + 1 <= 30)
                         { concat = concat.Length == 0 ? strSplit[0].Trim() : concat + ' ' + strSplit[0].Trim(); }

                     }
                     if (concat.Length <= 30)
                     {
                         remaining = "";
                         if (strSplit.Length >= 2)
                         { remaining = strSplit[1]; }
                         else { remaining = strSplit[0].Trim(); }

                         strSplit = null;
                         strSplit = remaining.Split('(');
                         if (!String.IsNullOrEmpty(strSplit[0]) && strSplit.Length >= 2)
                         {
                             if (concat.Length + strSplit[0].Length + 1 <= 30)
                             { concat = concat.Length == 0 ? strSplit[0].Trim() : concat + ' ' + strSplit[0].Trim(); }

                         }
                         if (concat.Length <= 30)
                         {
                             remaining = "";
                             if (strSplit.Length >= 2)
                             { remaining = strSplit[1].Trim(); }
                             else { remaining = strSplit[0]; }

                             strSplit = null;
                             strSplit = remaining.Split(')');
                             if (!String.IsNullOrEmpty(strSplit[0]) && strSplit.Length >= 2)
                             {
                                 if (concat.Length + strSplit[0].Length + 1 <= 30)
                                 { concat = concat.Length == 0 ? strSplit[0].Trim() : concat + ' ' + strSplit[0].Trim(); }
                             }
                             if (concat.Length <= 30)
                             {
                                 ///////////
                                 if (strSplit.Length >= 2)
                                 { remaining = strSplit[1]; }
                                 else { remaining = strSplit[0].Trim(); }
                                 ///////////////
                                 if (concat == "" || remaining != "")
                                 {
                                     if (concat == "")
                                     { concat = remaining.Trim(); }
                                     else
                                     {
                                         remaining = System.Text.RegularExpressions.Regex.Replace(remaining, @"\s+/g", " ").Trim();
                                         concat = concat.Trim() + ' ' + System.Text.RegularExpressions.Regex.Replace(remaining, @"[\/\\#,+()$~%.':*?<>{} ]", " ").Replace("&", "and").Trim(); //remaining.Trim(); 
                                     }
                                 }
                             }

                         }
                     }
                 }

             }*/


            // concat = concat.Replace(" ", "-");//working
            concat = System.Text.RegularExpressions.Regex.Replace(concat, @"\s+", " ");
            concat = System.Text.RegularExpressions.Regex.Replace(concat, @"[\/\\#,+()$~%.':*?<>{} ]", "-").Replace("&", "and");
            concat = concat.Trim(new[] { '-' });
            ////////////
            //string test = concat.Substring(0, 1);

            //if (test == "-")
            //{ concat = concat.Substring(1, concat.Length); }
            //var test2 = concat[concat.Length - 1];
            //if (test2 == '-')
            //{ concat = concat.Substring(0, concat.Length - 1); }
            ////////////
            return concat;
        }
        public JsonResult GetCities()
        {
            var Cities = (from f in db.Franchises
                          where f.ID != 1 && f.IsActive == true && f.BusinessDetail.UserLogin.IsLocked == false && f.BusinessDetail.Pincode.City.IsActive == true
                          select new { ID = f.BusinessDetail.Pincode.CityID, Name = f.BusinessDetail.Pincode.City.Name, IsFranchise = 1 }).Distinct().ToList().OrderBy(x => x.Name).ToList();
            //Added City list where merchant is registered but Franchise is not
            DateTime today = DateTime.Now;
            Cities.AddRange(db.Merchants.Where(m => m.Status == "Approve" && (m.ApproveDate != null && (m.ApproveDate != null && SqlFunctions.DateAdd("year", m.ValidityPeriod.Value, m.ApproveDate.Value) >= today)) && (db.merchantTopupRecharges.Where(t => t.Amount > 0).Select(t => t.MerchantID)).Contains(m.Id) && (db.MerchantKYCs.Where(MK => MK.IsCompleted == true && MK.IsVerified == true).Select(MK => MK.MerchantID)).Contains(m.Id)).ToList()
               .AsEnumerable().Where(m => !(Cities.Select(c => c.ID).Contains(m.City)))
               .Select(p => new
               {
                   ID = p.City,
                   Name = p.CityDetail.Name,
                   IsFranchise = 0
               }).ToList());

            return Json(Cities.OrderBy(x=>x.Name), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetPincode(int cityid, string SearchBy)
        {
            List<FranchiseAreaPincode> FranchAreaPin = new List<FranchiseAreaPincode>();
            FranchAreaPin = FranchiseAllotedLocation.GetFranchiseAreaPincode(cityid);
            try
            {
                if (!string.IsNullOrEmpty(SearchBy))
                {
                    string[] searchSplit = SearchBy.Split('=');
                    if (searchSplit[0] == "Pincode")
                    {
                        FranchAreaPin = FranchAreaPin.Where(p => p.Pincode.Contains(searchSplit[1])).ToList();
                    }
                    else
                    {
                        FranchAreaPin = FranchAreaPin.Where(p => p.Area.ToLower().Contains(searchSplit[1].ToLower())).ToList();
                    }
                }
            }
            catch
            {

            }
            var Pincodelist = (from f in FranchAreaPin
                               select new { ID = f.AreaID, Name = f.Pincode + " - " + f.Area }).Distinct().ToList().OrderBy(x => x.Name);
            return Json(Pincodelist, JsonRequestBehavior.AllowGet);
        }
        // public JsonResult GetCity(string city)////hide
        public JsonResult GetCityFranchise(string city, int franchiseid)
        {
            /*long CityID = (from f in db.Franchises
                           where f.ID != 1 && f.IsActive == true && f.BusinessDetail.UserLogin.IsLocked == false && f.BusinessDetail.Pincode.City.IsActive == true
                           && f.BusinessDetail.Pincode.City.Name == city
                           select f.BusinessDetail.Pincode.CityID).FirstOrDefault();*/ ////hide
            /////added && f.ID == franchiseid
            //--added by Ashish for multiple franchise in same city--//
            var CityIDFranchiseIdContact = (from f in db.Franchises
                                            join bd in db.BusinessDetails on f.BusinessDetailID equals bd.ID
                                            join ul in db.UserLogins on bd.UserLoginID equals ul.ID
                                            join pc in db.Pincodes on bd.PincodeID equals pc.ID
                                            join c in db.Cities on pc.CityID equals c.ID
                                            join hd in db.HelpDeskDetails on f.ID equals hd.FranchiseID into LOJ
                                            from hd in LOJ.DefaultIfEmpty()//-- For Left Outer Join --//
                                            where f.ID != 1 && f.IsActive == true && c.Name.ToLower().Trim() == city.ToLower().Trim()
                                                  && f.ID == franchiseid && ul.IsLocked == false && c.IsActive == true
                                            select new GetCityIdFranchiseIDContact
                                            {
                                                cityId = c.ID,
                                                franchiseId = f.ID,
                                                contact = hd.HelpLineNumber
                                            }).FirstOrDefault();


            if (CityIDFranchiseIdContact != null)//CityID -> CityIDFranchiseIdContact
            {
                ShoppingCartInitialization sci = new ShoppingCartInitialization();
                sci.DeleteShoppingCartCookie();
            }
            return Json(CityIDFranchiseIdContact, JsonRequestBehavior.AllowGet);//CityID->CityIDFranchiseIdContact
        }

        public JsonResult GetLayoutDynamicBlocks()
        {
            try
            {
                if (ControllerContext.HttpContext.Request.Cookies["CityCookie"] != null && ControllerContext.HttpContext.Request.Cookies["CityCookie"].Value != string.Empty)
                {
                    Int64 cityVal = 0;
                    Int64 franchiseVal = 0;////added
                    Int64 helpLineNoVal = 0;////added
                    Int64.TryParse(ControllerContext.HttpContext.Request.Cookies["CityCookie"].Value.Split('$')[0], out cityVal);
                    Int64.TryParse(ControllerContext.HttpContext.Request.Cookies["CityCookie"].Value.Split('$')[2], out franchiseVal);////added
                    Int64.TryParse(ControllerContext.HttpContext.Request.Cookies["CityCookie"].Value.Split('$')[3], out helpLineNoVal);////added

                    DataSet ds = BusinessLogicLayer.HomePageBlockItemsList.GetHomeIndexItemList(franchiseVal, System.Web.HttpContext.Current.Server);////added cityVal->franchiseVal

                    /*Select All Menu By Franchise */
                    DataTable dt = new DataTable();
                    dt = ds.Tables[0];

                    List<BlockViewModel> lBlockTypes = new List<BlockViewModel>();
                    lBlockTypes = (from n in dt.AsEnumerable()
                                   where n.Field<string>("Name").ToLower().Trim() == "logo"
                                   select new BlockViewModel
                                   {
                                       ID = n.Field<Int64>("ID"),
                                       Name = n.Field<string>("Name"),
                                       ImageWidth = n.Field<decimal>("ImageWidth"),
                                       ImageHeight = n.Field<decimal>("ImageHeight"),
                                       MaxLimit = n.Field<int>("MaxLimit"),
                                       IsActive = n.Field<bool>("IsActive")
                                   }).OrderBy(x => x.Name).ToList();

                    BusinessLogicLayer.ReadConfig rcKey = new BusinessLogicLayer.ReadConfig(System.Web.HttpContext.Current.Server);
                    DataTable dt1 = new DataTable();
                    dt1 = ds.Tables[1];

                    DataView dv = new DataView(dt1);
                    foreach (BlockViewModel B in lBlockTypes)
                    {
                        int max = 0;
                        int.TryParse(Convert.ToString(B.MaxLimit), out max);

                        dv.RowFilter = "DesignBlockTypeID=" + B.ID;
                        B.blockItemsList = (from n in dv.ToTable().AsEnumerable()
                                            select new HomePageBlockItemsViewModel
                                            {
                                                ID = n.Field<Int64>("ID"),
                                                ImageName = rcKey.HOME_IMAGE_HTTP + n.Field<string>("ImageName"),
                                                LinkUrl = n.Field<string>("LinkUrl"),
                                                Tooltip = n.Field<string>("Tooltip"),
                                                SequenceOrder = n.Field<int>("SequenceOrder"),
                                                IsActive = n.Field<bool>("IsActive")
                                            }).OrderBy(x => x.SequenceOrder).ToList();
                    }

                    return Json(lBlockTypes, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Json("", JsonRequestBehavior.AllowGet);
        }


        public ActionResult OrderDetailCart()
        {
            return View();
        }
        public ActionResult Submenu()
        {
            return View();
        }
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }
        public ActionResult DownloadApp()
        {
            return View();
        }
        public ActionResult Career()
        {
            ViewBag.Message = "Your Career page.";

            //var lsCareer = db.Careers.ToList();
            return View(db.Careers.Where(x => x.ExpiryDate >= DateTime.Now && x.IsActive == true && x.NoOfOpening > 0).OrderBy(x => x.CreateDate).ToList());
        }
        public ActionResult FAQ()
        {
            ViewBag.Message = "Your FAQ page.";

            return View();
        }
        public ActionResult Terms()
        {
            return View();
        }
        public ActionResult Feedback()
        {
            try
            {
                ViewBag.FeedbackCategaryID = new SelectList(db.FeedbackCategaries, "ID", "Name");
                if (Session["UID"] != null)
                {
                    long uid = Convert.ToInt64(Session["UID"]);
                    var userDetail = (from userlog in db.UserLogins
                                      where userlog.ID == uid
                                      select userlog).FirstOrDefault();
                    ViewBag.email = userDetail.Email;
                    ViewBag.mobile = userDetail.Mobile;
                }
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Error", "There's something wrong with loading value in feedback dropdown list!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[FeedbackController][GET:Create]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {

                ModelState.AddModelError("Error", "There's something wrong with loading value in feedback dropdown list!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[FeedbackController][GET:Create]",
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
            }
            return View();
        }
        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        public ActionResult CustomerGuide()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        public ActionResult PaymentGuide()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        public ActionResult MarchantGuide()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        public ActionResult FranchiseGuide()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        public ActionResult DeliveryGuide()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        public ActionResult ShopperGuide()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        public ActionResult NewIndex()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        public ActionResult NewIndex11()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        public ActionResult IndexNew()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        public ActionResult IndexHome()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        public ActionResult Mail()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        public ActionResult Termandcondition()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        public ActionResult IndexNew1()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult Festival(string city)
        {
            ViewBag.Message = "Your contact page.";
            switch (city.Trim().ToLower())
            {
                //case "nagpur": return View("FastivalIndex");
                //case "varanasi": return View("FastivalIndexVaranasi");
                case "kanpur": return View("FastivalIndexKanpur");//Yashaswi 01/12/2018 Default City Change
                                                                  //case "wardha": return View("FastivalIndexWardha");
            }
            return View("SomethingWrong");
        }

        public ActionResult Festiv(string city)
        {
            URLCookie.SetCookies();
            ViewBag.Message = "Your contact page.";
            switch (city.Trim().ToLower())
            {
                //case "nagpur": return View("Fastiv");
                //case "varanasi": return View("FastivVaranasi");
                case "kanpur": return View("FastivKanpur");
                    //Yashaswi 01/12/2018 Default City Change
                    //case "wardha": return View("FastivWardha");
            }
            return View("SomethingWrong");
        }
        public ActionResult ComingSoon()
        {


            return View("Coming-soon");
        }
        public ActionResult blankPage()
        {


            return View("blankPage");
        }
        public ActionResult OfferPage()
        {


            return View("OfferPage");
        }



        public ActionResult Exclusive(string city)
        {
            switch (city.Trim().ToLower())
            {
                //Yashaswi 01/12/2018 Default City Change
                case "kanpur": return View("Exclusive");
                    //case "varanasi": return View("");
                    //case "kanpur": return View("");
                    //case "wardha": return View("");
            }
            return View("SomethingWrong");
        }
        public ActionResult Nursery(string city)
        {

            switch (city.Trim().ToLower())
            {
                //Yashaswi 01/12/2018 Default City Change
                case "kanpur": return View("Nursery");
                    //case "varanasi": return View("");
                    //case "kanpur": return View("");
                    //case "wardha": return View("");
            }
            return View("SomethingWrong");
        }
        public ActionResult Holioffers(string city)
        {

            switch (city.Trim().ToLower())
            {
                //Yashaswi 01/12/2018 Default City Change
                case "kanpur": return View("Holioffers");
                    //case "varanasi": return View("");
                    //case "kanpur": return View("");
                    //case "wardha": return View("");
            }
            return View("SomethingWrong");
        }


        public JsonResult SendSMSForAppLink(string mob)
        {
            try
            {

                CommonFunctions cf = new CommonFunctions();

                Dictionary<string, string> dictSMSValues = new Dictionary<string, string>();

                //dictSMSValues.Add("#--SUBSCRIPTION_AMT--#", subscriptionAmount.ToString());


                BusinessLogicLayer.GateWay gateWay = new BusinessLogicLayer.SMS(System.Web.HttpContext.Current.Server);

                gateWay.SendSMS(BusinessLogicLayer.GateWay.SMSGateWays.SUMIT, BusinessLogicLayer.GateWay.SMSOptions.SINGLE, BusinessLogicLayer.GateWay.SMSTypes.CUST_APP_DOWNLOAD_LINK, new string[] { mob }, dictSMSValues);

            }
            catch (Exception)
            {

                throw;
            }
            return Json("1");
        }

        public ActionResult DealOfDay(string city)
        {
            switch (city.Trim().ToLower())
            {
                //Yashaswi 01/12/2018 Default City Change
                case "kanpur": return View("DealOfDay");
                    //case "varanasi": return View("");
                    //case "kanpur": return View("");
                    //case "wardha": return View("EngineeringAndMachinery");
            }
            return View("SomethingWrong");
        }

        public ActionResult FourtyEightHour(string city)
        {
            switch (city.Trim().ToLower())
            {
                //Yashaswi 01/12/2018 Default City Change
                case "kanpur": return View("FourtyEightHour");
                    //case "varanasi": return View("");
                    //case "kanpur": return View("");
                    // case "wardha": return View("EngineeringAndMachinery");
            }
            return View("SomethingWrong");
        }

        public class GandhibaghCities
        {
            public long cityID { get; set; }
            public string cityName { get; set; }
            public string ImgPath { get; set; }
        }

        public JsonResult GetGBWalletAmt(long userID)
        {
            ReferAndEarn lReferAndEarn = new ReferAndEarn();
            decimal val = lReferAndEarn.GetTotalEarnAmount(userID);
            //Yashaswi 7-9-2018

            MLMWallet obj = db.MLMWallets.FirstOrDefault(p => p.UserLoginID == userID);
            if (obj != null)
            {
                val = val + obj.Amount;
            }

            CashbackWallet wallet = db.cashbackWallets.FirstOrDefault(p => p.UserLoginID == userID);
            if (wallet != null)
            {
                val = val + wallet.Amount;
            }
            return Json(val);
        }

        public class GetCityIdFranchiseIDContact ////added GetCityIdFranchiseIDContact
        {
            public long cityId { get; set; }
            public int franchiseId { get; set; }
            public string contact { get; set; }
        }
        /// <summary>
        /// Geting value from API BussinessLayer for Dynamic Message
        /// By Ashish
        /// </summary>
        /// <returns></returns>
        private List<WeeklySeasonalFestivalPageMessage> CallGetPageMessageAPI()
        {
            int FranchiseID = 0;
            if (ControllerContext.HttpContext.Request.Cookies["CityCookie"] != null && ControllerContext.HttpContext.Request.Cookies["CityCookie"].Value != string.Empty)
            {

                FranchiseID = Convert.ToInt32(ControllerContext.HttpContext.Request.Cookies["CityCookie"].Value.Split('$')[2].Trim());
            }
            else
            {
                FranchiseID = Convert.ToInt32(franchiseIDMsg);
            }
            List<WeeklySeasonalFestivalPageMessage> WSFMsg = new List<WeeklySeasonalFestivalPageMessage>();
            WSFMsg = FranchisePageMessages.GetFranchisePageMessage(FranchiseID);

            return WSFMsg;
        }

    }
}