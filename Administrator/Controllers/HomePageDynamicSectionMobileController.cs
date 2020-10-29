using BusinessLogicLayer;
using ModelLayer.Models;
using ModelLayer.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Data;
using System.Data.Entity;
using System.Net;
using System.Data.Entity.Core.Objects;

namespace Administrator.Controllers
{
    public class HomePageDynamicSectionMobileController : Controller
    {
        // GET: HomePageDynamicSectionMobile

        EzeeloDBContext db = new EzeeloDBContext();

        StringBuilder errStr = new StringBuilder("=====================================================================================" +
     Environment.NewLine
     + "ErrorLog Controller : HomePageDynamicSectionMobileController" + Environment.NewLine);


        #region Index

        public ActionResult Index(long? CityList, int? franchiseList)
        {
            List<HomePageDynamicSectionsMaster> obj = db.HomePageDynamicSectionsMasters.ToList();

            List<DynamicDesingViewModel> DynamicDesingTypelist = new List<DynamicDesingViewModel>();
            try
            {


                var citylist = (from f in db.Franchises
                                where f.ID != 1 && f.IsActive == true && f.BusinessDetail.UserLogin.IsLocked == false && f.BusinessDetail.Pincode.City.IsActive == true
                                select new { ID = f.BusinessDetail.Pincode.CityID, Name = f.BusinessDetail.Pincode.City.Name }).Distinct().ToList().OrderBy(x => x.Name);
                ViewBag.CityList = new SelectList(citylist, "ID", "Name");
                //ViewBag.franchiseList = new SelectList((from f in db.Franchises where f.ID != 1 && f.PincodeID != null select new { f.ID, f.BusinessDetail.Name }).ToList().OrderBy(x => x.Name), "ID", "Name");
                ViewBag.franchiseList = new SelectList("ID", "Name");
                if (franchiseList != null)
                {
                    var FranchiseSelectList = new SelectList((from f in db.Franchises where f.ID == franchiseList && f.PincodeID != null select new { f.ID, f.BusinessDetail.Name }).ToList().OrderBy(x => x.Name), "ID", "Name");
                    foreach (var item in FranchiseSelectList)
                    {
                        item.Selected = true;
                        break;
                    }
                    ViewBag.franchiseList = FranchiseSelectList;
                }
                if (CityList != null && franchiseList != null)
                {
                    DynamicDesingTypelist = (from f in db.HomePageDynamicSection
                                             join p in db.HomePageDynamicSectionsMasters on f.SectionId equals p.SectionID
                                             where f.ShowInApp == true && f.FranchiseId == franchiseList
                                             select new DynamicDesingViewModel { ID = f.ID, IsActive = f.IsActive, SequenceOrder = f.SequenceOrder, IsProduct = f.IsProduct, IsBanner = f.IsBanner, IsCategory = f.IsCategory, SectionDisplayName = f.SectionDisplayName, SectionHeader = p.SectionHeader, ItemCount = db.HomePageDynamicSection.Where(x => x.SectionId == p.ID && x.FranchiseId == franchiseList).Select(x => x.ID).Count() }).ToList();
                    //select new DynamicDesingViewModel { ID = dbt.ID, SectionHeader = dbt.SectionHeader, ItemCount = db.HomePageDynamicSection.Where(x => x.SectionId == dbt.ID && x.FranchiseId == franchiseList && x.HomePageDynamicSectionsMaster.SectionHeader.ToLower().Trim() != "product gallery").Select(x => x.ID).Count() }).ToList();

                }

                return View(DynamicDesingTypelist.OrderBy(x => x.SequenceOrder));

            }
            catch (Exception ex)
            {
                errStr.Append("Method Name[Http Request] :- Index[HttpGet]" + Environment.NewLine +
                              "ON Dated" + DateTime.UtcNow.AddHours(5.30).TimeOfDay + Environment.NewLine +
                                  ex.Message.ToString() + Environment.NewLine +
                        "====================================================================================="
                              );
                //ViewBag.Message = "Sorry! Problem in customer registration!!";
                ModelState.AddModelError("Message", "Sorry! Problem in Generate Index view!!");
                ErrorLog.ErrorLogFile("Sorry! Problem in Generate Index view!! " + Environment.NewLine + errStr.ToString()
                    , ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);

                return View();
            }

        }




        #endregion

        #region HomePageSectionCreate
        public ActionResult HomePageDynamicSectionCreate()
        {
            var SectionStyle = Enum.GetNames(typeof(ModelLayer.Models.Enum.Margin_DivisionConstants.Select_StyleMobile));
            //var SectionStyleValue = Enum.GetNames(typeof(ModelLayer.Models.Enum.Margin_DivisionConstants.Select_StyleMobile));
            //var list = SectionStyle.ToList().Select(p => new
            //{
            //    ID = p,
            //    Name = p
            //});
            //ViewBag.SectionStyle = new SelectList(list, "ID", "Name");
            // ViewBag.SectionStyle = new SelectList(SectionStyle, SectionStyleValue);
            ViewBag.SectionStyle = new SelectList("ID", "Name");

            ViewBag.FranchiseID = new SelectList("ID", "Name");

            var citylist = (from f in db.Franchises
                            where f.ID != 1 && f.IsActive == true && f.BusinessDetail.UserLogin.IsLocked == false && f.BusinessDetail.Pincode.City.IsActive == true
                            select new { ID = f.BusinessDetail.Pincode.CityID, Name = f.BusinessDetail.Pincode.City.Name }).Distinct().ToList().OrderBy(x => x.Name);
            ViewBag.CityList = new SelectList(citylist, "ID", "Name");

            ViewBag.SectionHeader = new SelectList((from h in db.HomePageDynamicSectionsMasters where h.SectionHeader != null select new { h.ID, h.SectionHeader }).ToList().OrderBy(x => x.SectionHeader), "ID", "SectionHeader");

            return View();
        }


        [HttpPost]
        public ActionResult HomePageDynamicSectionCreate(HomePageDynamicSection homePageDynamicSection, Int64 SectionHeader, string SectionStyle)
        {
            long FranchiseID = (Int64)db.HomePageDynamicSection.Where(s => s.ID == homePageDynamicSection.ID).Select(a => a.FranchiseId).FirstOrDefault();
            //long CityID = (from c in db.)
            var CityID = (from f in db.Franchises
                          where f.ID == FranchiseID && f.IsActive == true && f.BusinessDetail.UserLogin.IsLocked == false && f.BusinessDetail.Pincode.City.IsActive == true
                          select new { ID = f.BusinessDetail.Pincode.CityID }).Distinct().FirstOrDefault();
            long CityId_ = 0;
            if (CityID != null)
            {
                CityId_ = CityID.ID;
            }
            try
            {
                homePageDynamicSection.SectionId = SectionHeader;
                homePageDynamicSection.NetworkIP = BusinessLogicLayer.CommonFunctions.GetClientIP();
                homePageDynamicSection.CreateBy = BusinessLogicLayer.CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(Session["ID"]));
                homePageDynamicSection.CreateDate = DateTime.Now;
                homePageDynamicSection.ShowInApp = true;
                if (ModelState.IsValid)
                {
                    db.HomePageDynamicSection.Add(homePageDynamicSection);
                    db.SaveChanges();
                    ViewBag.Message = "Done!  Added Successfully!!";
                    TempData["Message"] = ViewBag.Message;
                    return RedirectToAction("Index", new { CityList = CityId_, franchiseList = FranchiseID });
                }
                else
                {
                    ViewBag.Message = "Sorry Something is wrong!!";
                    TempData["Message"] = ViewBag.Message;
                    return RedirectToAction("Index", new { CityList = CityId_, franchiseList = FranchiseID });
                }
            }
            catch (Exception ex)
            {
                errStr.Append("Method Name[Http Request] :- Index[HttpGet]" + Environment.NewLine +
                              "ON Dated" + DateTime.UtcNow.AddHours(5.30).TimeOfDay + Environment.NewLine +
                                  ex.Message.ToString() + Environment.NewLine +
                        "====================================================================================="
                              );
                //ViewBag.Message = "Sorry! Problem in customer registration!!";
                ModelState.AddModelError("Message", "Sorry! Problem in Generate Index view!!");
                ErrorLog.ErrorLogFile("Sorry! Problem in Generate HomePageDynamicSectionCreate view!! " + Environment.NewLine + errStr.ToString()
                    , ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);

                return RedirectToAction("Index", new { CityList = CityId_, franchiseList = FranchiseID });
            }

        }
        #endregion

        #region update banner

        public ActionResult Updatebanner(int id)  // it is HomepageDynamicSection id
        {
            int HomepageDynamicsectionID = id;
            //ViewBag.ListName = db.HomePageDynamicSection.Where(x => x.ID == HomepageDynamicsectionID).Select(x => x.SectionDisplayName).FirstOrDefault();
            BusinessLogicLayer.ReadConfig rcKey = new BusinessLogicLayer.ReadConfig(System.Web.HttpContext.Current.Server);
            var objhomePageDynamicSectionBanner = db.HomePageDynamicSectionBanner.Where(s => s.HomePageDynamicSectionId == id).FirstOrDefault();
            long FranchiseID = (Int64)db.HomePageDynamicSection.Where(s => s.ID == HomepageDynamicsectionID).Select(a => a.FranchiseId).FirstOrDefault();
            //long FranchiseID = (Int64)db.HomePageDynamicSection.Where(s => s.ID == objhomePageDynamicSectionBanner.HomePageDynamicSectionId).Select(a => a.FranchiseId).FirstOrDefault();
            var CityID = (from f in db.Franchises
                          where f.ID == FranchiseID && f.IsActive == true && f.BusinessDetail.UserLogin.IsLocked == false && f.BusinessDetail.Pincode.City.IsActive == true
                          select new { ID = f.BusinessDetail.Pincode.CityID }).Distinct().FirstOrDefault();
            long CityId_ = 0;
            if (CityID != null)
            {
                CityId_ = CityID.ID;
            }

            int HomePageSectionId = (int)db.HomePageDynamicSection.Where(x => x.ID == HomepageDynamicsectionID).Select(x => x.SectionId).FirstOrDefault();
            var biList = db.HomePageDynamicSectionBanner.Where(x => x.HomePageDynamicSectionId == id).ToList();
            if (biList != null && biList.Count > 0)
            {
                foreach (var item in biList)
                {
                    item.ImageName = rcKey.HomePageDynamicSection_IMAGE_HTTP + CityId_ + "/" + FranchiseID + "/" + HomePageSectionId + "/" + item.ImageName;

                }
            }
            return View(biList.OrderBy(x => x.SequenceOrder));
        }

        #endregion

        #region Edit Banner

        public ActionResult EditBanner(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            BusinessLogicLayer.ReadConfig rcKey = new BusinessLogicLayer.ReadConfig(System.Web.HttpContext.Current.Server);

            HomePageDynamicSectionBanner homePageDynamicSectionBanner = db.HomePageDynamicSectionBanner.Find(id);

            //--- comment is use for show category like level oneid two and three id
            //var LevelThree = db.Categories.Where(x => x.Level == 3 && x.IsActive == true).ToList().OrderBy(x => x.ID);
            //ViewBag.CategoryID = new SelectList(LevelThree, "ID", "Name", homePageDynamicSectionBanner.CategoryID);

            //ViewBag.leveloneId = new SelectList("ID", "Name");
            //ViewBag.leveltwoId = new SelectList("ID", "Name");
            //ViewBag.LevelThree = new SelectList("ID", "Name");
            if (homePageDynamicSectionBanner.CategoryID != null)
            {
                if (homePageDynamicSectionBanner.DisplayViewApp == "lvl1categorylist")
                {
                    var Levelone = db.Categories.Where(x => x.Level == 1 && x.IsActive == true).ToList().OrderBy(x => x.ID);
                    ViewBag.leveloneId = new SelectList(Levelone, "ID", "Name", homePageDynamicSectionBanner.CategoryID);
                    ViewBag.leveltwoId = new SelectList("ID", "Name");
                    ViewBag.LevelThree = new SelectList("ID", "Name");
                }

                if (homePageDynamicSectionBanner.DisplayViewApp == "lvl2Categorylist")
                {
                    var Leveltwo = db.Categories.Where(x => x.Level == 1 && x.IsActive == true).ToList().OrderBy(x => x.ID);
                    ViewBag.leveltwoId = new SelectList(Leveltwo, "ID", "Name", homePageDynamicSectionBanner.CategoryID);
                    ViewBag.leveloneId = new SelectList("ID", "Name");
                    ViewBag.LevelThree = new SelectList("ID", "Name");
                }
                if (homePageDynamicSectionBanner.DisplayViewApp == "OfferProductList")
                {
                    var LevelThree = db.Categories.Where(x => x.Level == 3 && x.IsActive == true).ToList().OrderBy(x => x.ID);
                    ViewBag.LevelThree = new SelectList(LevelThree, "ID", "Name", homePageDynamicSectionBanner.CategoryID);
                    ViewBag.leveloneId = new SelectList("ID", "Name");
                    ViewBag.leveltwoId = new SelectList("ID", "Name");
                }

            }
            else
            {
                ViewBag.leveloneId = new SelectList("ID", "Name");
                ViewBag.leveltwoId = new SelectList("ID", "Name");
                ViewBag.LevelThree = new SelectList("ID", "Name");
            }



            var BrandName = db.Brands.OrderBy(x => x.Name).Where(b => b.IsActive == true).ToList();
            ViewBag.BrandName = new SelectList(BrandName, "ID", "Name", homePageDynamicSectionBanner.BrandID);
            //var OfferList = (from s in db.Shops
            //                 join o in db.Offers on s.ID equals o.OwnerID
            //                 join od in db.OfferDurations on o.ID equals od.OfferID
            //                 where o.IsActive == true && od.IsActive == true && s.IsActive == true && od.StartDateTime <= EntityFunctions.TruncateTime(DateTime.Now) && od.EndDateTime >= EntityFunctions.TruncateTime(DateTime.Now) && s.FranchiseID == franchiseId
            //                 select new OfferIDList
            //                 {
            //                     Id = o.ID,
            //                     Name = o.Description
            //                 }).ToList();
            ViewBag.Offername = new SelectList("ID", "Name");

            HomePageDynamicSection objHomePageDynamicSection = db.HomePageDynamicSection.FirstOrDefault(p => p.ID == homePageDynamicSectionBanner.HomePageDynamicSectionId);
            int HomepageDynamicsctionID = Convert.ToInt32(objHomePageDynamicSection.ID);
            if (objHomePageDynamicSection != null)
            {
                Franchise f = db.Franchises.FirstOrDefault(p => p.ID == objHomePageDynamicSection.FranchiseId);
                ViewBag.franchiseId = f.ID;
                if (f != null)
                {
                    ViewBag.FranchiseName = db.BusinessDetails.FirstOrDefault(p => p.ID == f.BusinessDetailID).Name;
                }
                else
                {
                    ViewBag.FranchiseName = "";
                }
                ViewBag.SectionStyle = objHomePageDynamicSection.SectionStyle;
                ViewBag.SectionDisplayName = objHomePageDynamicSection.SectionDisplayName;
            }

            long FranchiseID = (Int64)db.HomePageDynamicSection.Where(s => s.ID == homePageDynamicSectionBanner.HomePageDynamicSectionId).Select(a => a.FranchiseId).FirstOrDefault();
            //long CityID = (from c in db.)
            var CityID = (from f in db.Franchises
                          where f.ID == FranchiseID && f.IsActive == true && f.BusinessDetail.UserLogin.IsLocked == false && f.BusinessDetail.Pincode.City.IsActive == true
                          select new { ID = f.BusinessDetail.Pincode.CityID }).Distinct().FirstOrDefault();
            long CityId_ = 0;
            if (CityID != null)
            {
                CityId_ = CityID.ID;
            }
            int HomePageSectionId = (int)db.HomePageDynamicSection.Where(x => x.ID == HomepageDynamicsctionID).Select(x => x.SectionId).FirstOrDefault();
            homePageDynamicSectionBanner.ImageName_ = rcKey.HomePageDynamicSection_IMAGE_HTTP + CityId_ + "/" + FranchiseID + "/" + HomePageSectionId + "/" + homePageDynamicSectionBanner.ImageName;

            var SectionStyle = Enum.GetNames(typeof(ModelLayer.Models.Enum.Margin_DivisionConstants.HomePageDynamicOfferList));

            var list = SectionStyle.ToList().Select(p => new
            {
                ID = p,
                Name = p
            });

            ViewBag.Category = new SelectList(list, "ID", "Name", homePageDynamicSectionBanner.DisplayViewApp);
            List<OfferIDList> OfferList = (from s in db.Shops
                                           join o in db.Offers on s.ID equals o.OwnerID
                                           join od in db.OfferDurations on o.ID equals od.OfferID
                                           where o.IsActive == true && od.IsActive == true && s.IsActive == true && od.StartDateTime <= EntityFunctions.TruncateTime(DateTime.Now) && od.EndDateTime >= EntityFunctions.TruncateTime(DateTime.Now) && s.FranchiseID == FranchiseID
                                           select new OfferIDList
                                           {
                                               Id = o.ID,
                                               Name = o.Description
                                           }
                                     ).ToList();
            ViewBag.Offername = new SelectList(OfferList, "ID", "Name", homePageDynamicSectionBanner.OfferId);
            //var Category = Enum.GetNames(typeof(ModelLayer.Models.Enum.Margin_DivisionConstants.HomePageDynamicOfferList));
            //var list = Category.ToList().Select(p => new
            //{
            //    ID = p,
            //    Name = p
            //});
            //ViewBag.Category = new SelectList(list, "ID", "Name", objHomePageDynamicSection.SectionDisplayName);
            //var ShopName = db.Shops.Where(s => s.FranchiseID == franchiseId).Select(s => s.Name).ToList();
            //var ShopID = db.Shops.Where(s => s.Name == ShopName).Select(s => s.ID).FirstOrDefault();
            var ShopName = db.Shops.Where(s => s.FranchiseID == FranchiseID && s.IsActive).ToList();


            var ShopList = (from n in ShopName
                            select new
                            {
                                ID = n.ID,
                                Name = n.Name
                            }
                            ).ToList();
            //var SelectlistShop = new SelectList(ShopList, "ID", "Name", ShopList.FirstOrDefault().ID);
            //foreach (var item in SelectlistShop)
            //{
            var SelectedShop = new SelectList(ShopList, "ID", "Name", homePageDynamicSectionBanner.ShopId);
            //    //foreach (var item in SelectedShop)
            //    //{
            //    //    item.Selected = true;
            //    //    break;
            //    //}
            //}
            ViewBag.ShopName = SelectedShop;
            //ViewBag.ShopName.ClearSelection(); //making sure the previous selection has been cleared
            //ViewBag.ShopName.FindByValue(homePageDynamicSectionBanner.ShopId).Selected = true;


            ViewBag.HomepageDynamicSectionID = HomepageDynamicsctionID;
            ViewBag.SDate = homePageDynamicSectionBanner.StartDate.ToString("dd/MM/yyyy");
            ViewBag.EDate = homePageDynamicSectionBanner.EndDate.ToString("dd/MM/yyyy");

            homePageDynamicSectionBanner.IsActive_ = homePageDynamicSectionBanner.IsActive;
            homePageDynamicSectionBanner.IsBanner_ = homePageDynamicSectionBanner.IsBanner;
            if (homePageDynamicSectionBanner == null)
            {
                return HttpNotFound();
            }
            return View(homePageDynamicSectionBanner);
        }
        [HttpPost]
        public ActionResult EditBanner(HomePageDynamicSectionBanner homePageDynamicSectionBanner, int HomepageDynamicSectionID, int? leveloneId, int? leveltwoId, int? Offername, string Category, int? LevelThree, int? BrandName, int? ShopName, HttpPostedFileBase file, string SDate, string EDate, int? tempcategory)
        {
            long FranchiseID = (Int64)db.HomePageDynamicSection.Where(s => s.ID == HomepageDynamicSectionID).Select(a => a.FranchiseId).FirstOrDefault();
            //long CityID = (from c in db.)
            var CityID = (from f in db.Franchises
                          where f.ID == FranchiseID && f.IsActive == true && f.BusinessDetail.UserLogin.IsLocked == false && f.BusinessDetail.Pincode.City.IsActive == true
                          select new { ID = f.BusinessDetail.Pincode.CityID }).Distinct().FirstOrDefault();
            long CityId_ = 0;
            if (CityID != null)
            {
                CityId_ = CityID.ID;
            }
            try
            {

                //-------comment for level one and level two 
                if (tempcategory != null && leveloneId == null && Category == "lvl1categorylist")
                {
                    leveloneId = Convert.ToInt32(tempcategory);
                }
                if (tempcategory != null && leveltwoId == null && Category == "lvl2categorylist")
                {
                    leveltwoId = Convert.ToInt32(tempcategory);
                }
                if (tempcategory != null && BrandName == null && ShopName == null && Offername == null && homePageDynamicSectionBanner.Keyword == null && LevelThree == null && Category == "OfferProductList")
                {
                    LevelThree = Convert.ToInt32(tempcategory);
                }

                string Categoryname = Category;

                switch (Categoryname)
                {
                    case ("lvl1categorylist"):
                        homePageDynamicSectionBanner.CategoryID = leveloneId;
                        homePageDynamicSectionBanner.ShopId = null;
                        homePageDynamicSectionBanner.Keyword = null;
                        homePageDynamicSectionBanner.BrandID = null;
                        homePageDynamicSectionBanner.OfferId = null;
                        break;

                    case ("lvl2Categorylist"):
                        homePageDynamicSectionBanner.CategoryID = leveltwoId;
                        homePageDynamicSectionBanner.ShopId = null;
                        homePageDynamicSectionBanner.Keyword = null;
                        homePageDynamicSectionBanner.BrandID = null;
                        homePageDynamicSectionBanner.OfferId = null;

                        break;

                    case ("OfferProductList"):
                        if (LevelThree != null)
                        {
                            homePageDynamicSectionBanner.CategoryID = LevelThree;
                            homePageDynamicSectionBanner.ShopId = null;
                            homePageDynamicSectionBanner.Keyword = null;
                            homePageDynamicSectionBanner.BrandID = null;
                            homePageDynamicSectionBanner.OfferId = null;
                            break;
                        }
                        if (BrandName != null)
                        {
                            homePageDynamicSectionBanner.BrandID = BrandName;
                            homePageDynamicSectionBanner.CategoryID = null;
                            homePageDynamicSectionBanner.ShopId = null;
                            homePageDynamicSectionBanner.Keyword = null;
                            homePageDynamicSectionBanner.OfferId = null;
                            break;
                        }
                        if (ShopName != null)
                        {
                            //var ShopID = db.Shops.Where(s => s.Name == ShopName).Select(s => s.ID).FirstOrDefault();
                            homePageDynamicSectionBanner.ShopId = ShopName;
                            homePageDynamicSectionBanner.CategoryID = null;
                            homePageDynamicSectionBanner.Keyword = null;
                            homePageDynamicSectionBanner.BrandID = null;
                            homePageDynamicSectionBanner.OfferId = null;
                            break;
                        }
                        if (homePageDynamicSectionBanner.Keyword != null)
                        {
                            homePageDynamicSectionBanner.ShopId = null;
                            homePageDynamicSectionBanner.OfferId = null;
                            homePageDynamicSectionBanner.CategoryID = null;
                            homePageDynamicSectionBanner.BrandID = null;
                            break;
                        }
                        if (Offername != null)
                        {
                            homePageDynamicSectionBanner.OfferId = Offername;
                            homePageDynamicSectionBanner.ShopId = null;
                            homePageDynamicSectionBanner.Keyword = null;
                            homePageDynamicSectionBanner.CategoryID = null;
                            homePageDynamicSectionBanner.BrandID = null;
                            break;
                        }
                        break;

                    default:

                        break;

                }





                //long FranchiseID = (Int64)db.HomePageDynamicSection.Where(s => s.ID == HomepageDynamicSectionID).Select(a => a.FranchiseId).FirstOrDefault();
                ////long CityID = (from c in db.)
                //var CityID = (from f in db.Franchises
                //              where f.ID == FranchiseID && f.IsActive == true && f.BusinessDetail.UserLogin.IsLocked == false && f.BusinessDetail.Pincode.City.IsActive == true
                //              select new { ID = f.BusinessDetail.Pincode.CityID }).Distinct().FirstOrDefault();
                //long CityId_ = 0;
                //if (CityID != null)
                //{
                //    CityId_ = CityID.ID;
                //}

                DateTime lSDate = CommonFunctions.GetProperDate(SDate);
                DateTime lEDate = CommonFunctions.GetProperDate(EDate);
                ViewBag.SDate = lSDate.ToString("dd/MM/yyyy");
                ViewBag.EDate = lEDate.ToString("dd/MM/yyyy");

                homePageDynamicSectionBanner.StartDate = lSDate;
                homePageDynamicSectionBanner.EndDate = lEDate;
                homePageDynamicSectionBanner.DisplayViewApp = Category;

                //homePageDynamicSectionBanner.BrandID = BrandName;
                //homePageDynamicSectionBanner.OfferId = Offername;
                if (file != null)
                {
                    homePageDynamicSectionBanner.ImageName = Path.GetFileName(file.FileName);
                }
                //homePageDynamicSectionBanner.ImageName = Path.GetFileName(file.FileName);
                homePageDynamicSectionBanner.NetworkIp = BusinessLogicLayer.CommonFunctions.GetClientIP();
                homePageDynamicSectionBanner.CreateDate = DateTime.Now;
                homePageDynamicSectionBanner.CreatedBy = BusinessLogicLayer.CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(Session["ID"]));
                homePageDynamicSectionBanner.IsActive = homePageDynamicSectionBanner.IsActive_;
                homePageDynamicSectionBanner.IsBanner = homePageDynamicSectionBanner.IsBanner_;

                if (homePageDynamicSectionBanner.ImageName != null && homePageDynamicSectionBanner.StartDate != null && homePageDynamicSectionBanner.EndDate != null)
                {
                    bool IsUploaded = false;
                    if (file != null)
                    {
                        //upload cat image here
                        int HomePageSectionId = (int)db.HomePageDynamicSection.Where(x => x.ID == HomepageDynamicSectionID).Select(x => x.SectionId).FirstOrDefault();
                        IsUploaded = CommonFunctions.UploadHomePageDynamicSectionImage(file, CityId_, FranchiseID, HomePageSectionId, Path.GetFileName(file.FileName));////added FranchiseID
                        if (IsUploaded)
                        {
                            db.Entry(homePageDynamicSectionBanner).State = EntityState.Modified;
                            db.SaveChanges();
                            ViewBag.Message = "Done!  Added Successfully!!";
                            TempData["Message"] = ViewBag.Message;
                            return RedirectToAction("Index", new { CityList = CityId_, franchiseList = FranchiseID });
                        }
                        else
                        {
                            ViewBag.Messaage = ViewBag.Messaage + " Problem in Uploading Image...";
                            TempData["Message"] = ViewBag.Messaage;
                            return RedirectToAction("Index", new { CityList = CityId_, franchiseList = FranchiseID });
                        }


                    }
                    if (!IsUploaded && file == null)
                    {
                        if (homePageDynamicSectionBanner.ImageName != null)
                        {
                            homePageDynamicSectionBanner.ModifyDate = DateTime.Now;
                            homePageDynamicSectionBanner.ModifyBy = BusinessLogicLayer.CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(Session["ID"]));
                            db.Entry(homePageDynamicSectionBanner).State = EntityState.Modified;
                            db.SaveChanges();
                            ViewBag.Message = "Done!  Added Successfully!!";
                            TempData["Message"] = ViewBag.Message;
                            return RedirectToAction("Index", new { CityList = CityId_, franchiseList = FranchiseID });
                        }
                        else
                        {
                            ViewBag.Message = "Sorry  Something Went wrong!!";
                            TempData["Message"] = ViewBag.Message;
                            return RedirectToAction("Index", new { CityList = CityId_, franchiseList = FranchiseID });
                        }
                        //ViewBag.Messaage = ViewBag.Messaage + " Problem in Uploading Image...";
                        //TempData["Message"] = ViewBag.Messaage;
                        //return RedirectToAction("Index");
                    }
                    else
                    {
                        ViewBag.Message = "Sorry Something is wrong!!";
                        TempData["Message"] = ViewBag.Message;
                        return RedirectToAction("Index", new { CityList = CityId_, franchiseList = FranchiseID });
                    }
                }
                else
                {
                    ViewBag.Message = "Sorry Something is wrong!!";
                    TempData["Message"] = ViewBag.Message;
                    return RedirectToAction("Index", new { CityList = CityId_, franchiseList = FranchiseID });
                }
            }
            catch (Exception ex)
            {
                errStr.Append("Method Name[Http Request] :- Index[HttpGet]" + Environment.NewLine +
                              "ON Dated" + DateTime.UtcNow.AddHours(5.30).TimeOfDay + Environment.NewLine +
                                  ex.Message.ToString() + Environment.NewLine +
                        "====================================================================================="
                              );
                //ViewBag.Message = "Sorry! Problem in customer registration!!";
                ModelState.AddModelError("Message", "Sorry! Problem in Generate Index view!!");
                ErrorLog.ErrorLogFile("Sorry! Problem in Generate Addbanner view!! " + Environment.NewLine + errStr.ToString()
                    , ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);

                return RedirectToAction("Index", new { CityList = CityId_, franchiseList = FranchiseID });
            }
        }




        #endregion

        #region AddBanner
        public ActionResult Addbanner(int id)
        {
            ViewBag.HomepageDynamicSectionID = id;
            HomePageDynamicSection objhomedynamic = db.HomePageDynamicSection.FirstOrDefault(x => x.ID == id);
            if (objhomedynamic != null)
            {
                Franchise f = db.Franchises.FirstOrDefault(a => a.ID == objhomedynamic.FranchiseId);
                ViewBag.franchiseId = f.ID;
                if (f != null)
                {

                    ViewBag.FranchiseName = db.BusinessDetails.FirstOrDefault(c => c.ID == f.BusinessDetailID).Name;
                }
                else
                {
                    ViewBag.FranchiseName = "";
                }

                var Category = Enum.GetNames(typeof(ModelLayer.Models.Enum.Margin_DivisionConstants.HomePageDynamicOfferList));
                var CategoryValue = Enum.GetNames(typeof(ModelLayer.Models.Enum.Margin_DivisionConstants.HomePageDynamicOfferList));

                ViewBag.Category = new SelectList(Category, CategoryValue);
                ViewBag.SectionStyle = objhomedynamic.SectionStyle;
                ViewBag.SectionDisplayName = objhomedynamic.SectionDisplayName;
                ViewBag.leveloneId = new SelectList("ID", "Name");
                ViewBag.leveltwoId = new SelectList("ID", "Name");
                ViewBag.LevelThree = new SelectList("ID", "Name");
                ViewBag.ShopName = new SelectList("ID", "Name");
                ViewBag.BrandName = new SelectList("ID", "Name");
                ViewBag.Offername = new SelectList("ID", "Name");
            }


            return View();
        }



        [HttpPost]
        public ActionResult Addbanner(HomePageDynamicSectionBanner homePageDynamicSectionBanner, int HomepageDynamicSectionID, int? leveloneId, int? leveltwoId, int? Offername, string Category, int? LevelThree, int? BrandName, int? ShopName, HttpPostedFileBase file, string SDate, string EDate)
        {
            int FranchiseID = (int)db.HomePageDynamicSection.Where(x => x.ID == homePageDynamicSectionBanner.HomePageDynamicSectionId).Select(x => x.FranchiseId).FirstOrDefault();
            var CityID = (from f in db.Franchises
                          where f.ID == FranchiseID && f.IsActive == true && f.BusinessDetail.UserLogin.IsLocked == false && f.BusinessDetail.Pincode.City.IsActive == true
                          select new { ID = f.BusinessDetail.Pincode.CityID }).Distinct().FirstOrDefault();
            long CityId_ = 0;
            if (CityID != null)
            {
                CityId_ = CityID.ID;
            }

            try
            {
                if (Category == "lvl1categorylist" || Category == "lvl2categorylist" || Category == "OfferProductList")
                {
                    if (Category == "lvl1categorylist" || Category == "lvl2Categorylist")
                    {

                        if (Category == "lvl1categorylist")
                        {

                            homePageDynamicSectionBanner.CategoryID = leveloneId;

                        }
                        if (Category == "lvl2categorylist")
                        {
                            homePageDynamicSectionBanner.CategoryID = leveltwoId;

                        }
                    }
                    else
                    {
                        homePageDynamicSectionBanner.CategoryID = LevelThree;
                    }
                }
                //if (ShopName != null)
                //{
                //    var ShopID = db.Shops.Where(s => s.Name == ShopName).Select(s => s.ID).FirstOrDefault();
                //    homePageDynamicSectionBanner.ShopId = ShopID;
                //}
                //else
                //{
                //    homePageDynamicSectionBanner.ShopId = null;
                //}

                homePageDynamicSectionBanner.ShopId = ShopName;
                //homePageDynamicSectionBanner.CategoryID = LevelThree;
                DateTime lSDate = CommonFunctions.GetProperDate(SDate);
                DateTime lEDate = CommonFunctions.GetProperDate(EDate);
                ViewBag.SDate = lSDate.ToString("dd/MM/yyyy");
                ViewBag.EDate = lEDate.ToString("dd/MM/yyyy");

                homePageDynamicSectionBanner.StartDate = lSDate;
                homePageDynamicSectionBanner.EndDate = lEDate;
                homePageDynamicSectionBanner.DisplayViewApp = Category;
                homePageDynamicSectionBanner.BrandID = BrandName;
                homePageDynamicSectionBanner.OfferId = Offername;

                homePageDynamicSectionBanner.ImageName = Path.GetFileName(file.FileName);
                homePageDynamicSectionBanner.NetworkIp = BusinessLogicLayer.CommonFunctions.GetClientIP();
                homePageDynamicSectionBanner.CreateDate = DateTime.Now;
                homePageDynamicSectionBanner.CreatedBy = BusinessLogicLayer.CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(Session["ID"]));
                homePageDynamicSectionBanner.IsActive = homePageDynamicSectionBanner.IsActive_;
                homePageDynamicSectionBanner.IsBanner = homePageDynamicSectionBanner.IsBanner_;


                if (homePageDynamicSectionBanner.ImageName != null && homePageDynamicSectionBanner.StartDate != null && homePageDynamicSectionBanner.EndDate != null)
                {
                    bool IsUploaded = false;
                    if (file != null)
                    {
                        //upload cat image here

                        int HomePageSectionId = (int)db.HomePageDynamicSection.Where(x => x.ID == HomepageDynamicSectionID).Select(x => x.SectionId).FirstOrDefault();

                        IsUploaded = CommonFunctions.UploadHomePageDynamicSectionImage(file, CityId_, FranchiseID, HomePageSectionId, Path.GetFileName(file.FileName));////added FranchiseID
                        if (IsUploaded)
                        {
                            db.HomePageDynamicSectionBanner.Add(homePageDynamicSectionBanner);
                            db.SaveChanges();
                            ViewBag.Message = "Done! Banner  Added Successfully!!";
                            TempData["Message"] = ViewBag.Message;
                            return RedirectToAction("Index", new { CityList = CityId_, franchiseList = FranchiseID });
                        }
                        else
                        {
                            ViewBag.Messaage = ViewBag.Messaage + " Problem in Uploading Image...";
                            TempData["Message"] = ViewBag.Messaage;
                            return RedirectToAction("Index", new { CityList = CityId_, franchiseList = FranchiseID });
                        }


                    }
                    if (!IsUploaded && file != null)
                    {
                        ViewBag.Messaage = ViewBag.Messaage + " Problem in Uploading Image...";
                        TempData["Message"] = ViewBag.Messaage;
                        return RedirectToAction("Index", new { CityList = CityId_, franchiseList = FranchiseID });

                    }
                    else
                    {
                        ViewBag.Message = "Sorry Something is wrong!!";
                        TempData["Message"] = ViewBag.Message;
                        return RedirectToAction("Index", new { CityList = CityId_, franchiseList = FranchiseID });

                    }
                }
                else
                {
                    ViewBag.Message = "Sorry Something is wrong!!";
                    TempData["Message"] = ViewBag.Message;
                    return RedirectToAction("Index", new { CityList = CityId_, franchiseList = FranchiseID });

                }
            }
            catch (Exception ex)
            {

                errStr.Append("Method Name[Http Request] :- Index[HttpGet]" + Environment.NewLine +
                              "ON Dated" + DateTime.UtcNow.AddHours(5.30).TimeOfDay + Environment.NewLine +
                                  ex.Message.ToString() + Environment.NewLine +
                        "====================================================================================="
                              );
                //ViewBag.Message = "Sorry! Problem in customer registration!!";
                ModelState.AddModelError("Message", "Sorry! Problem in Generate Index view!!");
                ErrorLog.ErrorLogFile("Sorry! Problem in Generate Addbanner view!! " + Environment.NewLine + errStr.ToString()
                    , ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);

                return RedirectToAction("Index", new { CityList = CityId_, franchiseList = FranchiseID });
            }

        }

        #endregion

        #region Edit HomePage Section

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }


            HomePageDynamicSection homePageDynamicSections = db.HomePageDynamicSection.Find(id);
            var SectionStyle = Enum.GetNames(typeof(ModelLayer.Models.Enum.Margin_DivisionConstants.Select_StyleMobile));

            var list = SectionStyle.ToList().Select(p => new
            {
                ID = p,
                Name = p
            });
            ViewBag.SectionStyle = new SelectList(list, "ID", "Name", homePageDynamicSections.SectionStyle);
            //ViewBag.SectionStyle = new SelectList("ID", "Name");

            ViewBag.FranchiseID = new SelectList((from f in db.Franchises where f.ID != 1 && f.PincodeID != null select new { f.ID, f.BusinessDetail.Name }).ToList().OrderBy(x => x.Name), "ID", "Name", homePageDynamicSections.FranchiseId);
            //ViewBag.SectionHeader = new SelectList((from h in HomePageDynamicSectionsMasters where h.))
            //ViewBag.SectionHeader = new SelectList(db.HomePageDynamicSectionsMasters.Select(h =>h.SectionHeader).ToList(),"ID", "SectionHeader");
            ViewBag.SectionHeader = new SelectList((from h in db.HomePageDynamicSectionsMasters where h.SectionHeader != null select new { h.ID, h.SectionHeader }).ToList().OrderBy(x => x.SectionHeader), "ID", "SectionHeader", homePageDynamicSections.SectionId);

            if (homePageDynamicSections == null)
            {
                return HttpNotFound();
            }
            return View(homePageDynamicSections);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(HomePageDynamicSection homePageDynamicSections, Int64 SectionHeader)
        {
            int FranchiseID = (int)db.HomePageDynamicSection.Where(x => x.ID == homePageDynamicSections.ID).Select(x => x.FranchiseId).FirstOrDefault();
            var CityID = (from f in db.Franchises
                          where f.ID == FranchiseID && f.IsActive == true && f.BusinessDetail.UserLogin.IsLocked == false && f.BusinessDetail.Pincode.City.IsActive == true
                          select new { ID = f.BusinessDetail.Pincode.CityID }).Distinct().FirstOrDefault();
            long CityId_ = 0;
            if (CityID != null)
            {
                CityId_ = CityID.ID;
            }

            try
            {


                if (ModelState.IsValid)
                {
                    homePageDynamicSections.SectionId = SectionHeader;
                    homePageDynamicSections.ModifyBy = BusinessLogicLayer.CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(Session["ID"]));
                    homePageDynamicSections.ModifyDate = DateTime.Now;
                    homePageDynamicSections.NetworkIP = BusinessLogicLayer.CommonFunctions.GetClientIP();
                    db.Entry(homePageDynamicSections).State = EntityState.Modified;
                    db.SaveChanges();
                    ViewBag.Message = "Done!  Added Successfully!!";
                    TempData["Message"] = ViewBag.Message;
                    return RedirectToAction("Index", new { CityList = CityId_, franchiseList = FranchiseID });
                }
                else
                {
                    return View(homePageDynamicSections);
                }
            }
            catch (Exception ex)
            {
                errStr.Append("Method Name[Http Request] :- Index[HttpGet]" + Environment.NewLine +
                              "ON Dated" + DateTime.UtcNow.AddHours(5.30).TimeOfDay + Environment.NewLine +
                                  ex.Message.ToString() + Environment.NewLine +
                        "====================================================================================="
                              );
                //ViewBag.Message = "Sorry! Problem in customer registration!!";
                ModelState.AddModelError("Message", "Sorry! Problem in Generate Index view!!");
                ErrorLog.ErrorLogFile("Sorry! Problem in Generate Edit view!! " + Environment.NewLine + errStr.ToString()
                    , ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);

                return RedirectToAction("Index", new { CityList = CityId_, franchiseList = FranchiseID });
            }


        }

        #endregion


        #region AddProduct
        public ActionResult AddProduct(int id)
        {
            long FranchiseID = (Int64)db.HomePageDynamicSection.Where(s => s.ID == id).Select(a => a.FranchiseId).FirstOrDefault();
            ViewBag.FranchiseID = FranchiseID;
            HomePageDynamicSection objHomePageDynamicSection = db.HomePageDynamicSection.FirstOrDefault(p => p.ID == id);
            if (objHomePageDynamicSection != null)
            {
                Franchise f = db.Franchises.FirstOrDefault(p => p.ID == objHomePageDynamicSection.FranchiseId);
                if (f != null)
                {

                    ViewBag.FranchiseName = db.BusinessDetails.FirstOrDefault(p => p.ID == f.BusinessDetailID).Name;
                }
                else
                {
                    ViewBag.FranchiseName = "";
                }
                ViewBag.SectionStyle = objHomePageDynamicSection.SectionStyle;
                ViewBag.SectionDisplayName = objHomePageDynamicSection.SectionDisplayName;
            }
            List<ProductDetail> ldata = new List<ProductDetail>();
            ldata.Add(new ProductDetail { ID = 0, Name = "Select Category" });
            //ViewBag.FranchiseList = new SelectList( db.HomePageDynamicSection.Where(s=> s.ID == id),"FranchiseId");
            ViewBag.HomepageDynamicSectionID = id;
            List<ProductDetail> proList = new List<ProductDetail>();
            proList = (from c in db.Categories
                       join pbc in db.PlanCategoryCharges on c.ID equals pbc.CategoryID
                       join p in db.Plans on pbc.PlanID equals p.ID
                       join op in db.OwnerPlans on p.ID equals op.PlanID
                       where op.OwnerID == FranchiseID && p.PlanCode.Substring(0, 4) == "GBFR"
                       && op.IsActive == true && pbc.IsActive == true && c.IsActive == true
                       select new ProductDetail
                       {
                           ID = c.ID,
                           Name = c.Name
                       }).ToList();
            ViewBag.CategoryList = new SelectList(proList.OrderBy(x => x.Name), "ID", "Name");
            ViewBag.LevelTwoCategoryList = new SelectList(ldata, "ID", "Name");
            ViewBag.LevelThreeCategoryList = new SelectList(ldata, "ID", "Name");
            ViewBag.Offername = new SelectList("ID", "Name");
            return View();
        }

        [HttpPost]
        public ActionResult AddProduct(string ProductList, string RemoveIDList, int HomepageDynamicSectionID, string SDate, string EDate)
        {
            int Franchiseid = (int)db.HomePageDynamicSection.Where(x => x.ID == HomepageDynamicSectionID).Select(x => x.FranchiseId).FirstOrDefault();
            var CityID = (from f in db.Franchises
                          where f.ID == Franchiseid && f.IsActive == true && f.BusinessDetail.UserLogin.IsLocked == false && f.BusinessDetail.Pincode.City.IsActive == true
                          select new { ID = f.BusinessDetail.Pincode.CityID }).Distinct().FirstOrDefault();
            long CityId_ = 0;
            if (CityID != null)
            {
                CityId_ = CityID.ID;
            }

            try
            {

                if (RemoveIDList != "")
                {
                    ViewBag.CategoryList = new SelectList("ID", "Name");
                    ViewBag.LevelTwoCategoryList = new SelectList("ID", "Name");
                    ViewBag.LevelThreeCategoryList = new SelectList("ID", "Name");

                    string[] strRemoveidlist;
                    strRemoveidlist = RemoveIDList.Split(',');
                    foreach (string val in strRemoveidlist)
                    {
                        long homepageproductId = Convert.ToInt32(val);

                        HomePageDynamicSectionProduct obj = db.HomePageDynamicSectionProduct.FirstOrDefault(p => p.HomePageDynamicSectionId == HomepageDynamicSectionID && p.ID == homepageproductId);

                        obj.IsActive = false;
                        obj.ModifyDate = DateTime.Now;
                        obj.ModifyBy = BusinessLogicLayer.CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(Session["ID"]));
                        db.Entry(obj).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    ViewBag.Message = "Done! Product Added Successfully!!";
                    TempData["Message"] = ViewBag.Message;
                    return RedirectToAction("Index", new { CityList = CityId_, franchiseList = Franchiseid });
                }
                else
                {
                    long FranchiseID = (Int64)db.HomePageDynamicSection.Where(s => s.ID == HomepageDynamicSectionID).Select(a => a.FranchiseId).FirstOrDefault();

                    List<ProductDetail> ldata = new List<ProductDetail>();
                    ldata.Add(new ProductDetail { ID = 0, Name = "Select Category" });

                    List<ProductDetail> proList = new List<ProductDetail>();
                    proList = (from c in db.Categories
                               join pbc in db.PlanBindCategories on c.ID equals pbc.CategoryID
                               join pb in db.PlanBinds on pbc.PlanBindID equals pb.ID
                               join p in db.Plans on pb.PlanID equals p.ID
                               join op in db.OwnerPlans on p.ID equals op.PlanID
                               where op.OwnerID == HomepageDynamicSectionID && p.PlanCode.Substring(0, 4) == "GBFR"
                               && op.IsActive == true && pbc.IsActive == true && c.IsActive == true
                               select new ProductDetail
                               {
                                   ID = c.ID,
                                   Name = c.Name
                               }).ToList();


                    ViewBag.CategoryList = new SelectList(proList.OrderBy(x => x.Name), "ID", "Name");
                    ViewBag.LevelTwoCategoryList = new SelectList(ldata, "ID", "Name");
                    ViewBag.LevelThreeCategoryList = new SelectList(ldata, "ID", "Name");

                    string[] strProducts;
                    strProducts = ProductList.Split(',');
                    DateTime lSDate = CommonFunctions.GetProperDate(SDate);
                    DateTime lEDate = CommonFunctions.GetProperDate(EDate);


                    foreach (string val in strProducts)
                    {
                        long ProductId = Convert.ToInt32(val);
                        long ShopStockId = 0;
                        try
                        {
                            Shop S = db.Shops.FirstOrDefault(s => s.FranchiseID == FranchiseID);
                            if (S != null)
                            {
                                ShopProduct SP = db.ShopProducts.FirstOrDefault(Sp => Sp.ShopID == S.ID && Sp.ProductID == ProductId);
                                if (SP != null)
                                {
                                    ShopStock SS = db.ShopStocks.FirstOrDefault(Ss => Ss.ShopProductID == SP.ID && Ss.Qty > 0);
                                    if (SS != null)
                                    {
                                        ShopStockId = SS.ID;


                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            ShopStockId = 0;
                        }

                        if (ShopStockId == 0)
                        {
                            continue;
                        }

                        HomePageDynamicSectionProduct obj = db.HomePageDynamicSectionProduct.FirstOrDefault(p => p.HomePageDynamicSectionId == HomepageDynamicSectionID && p.ShopStockId == ShopStockId);
                        if (obj != null)
                        {
                            if (obj.IsActive)
                            {
                                continue;
                            }
                            else
                            {
                                obj.StartDate = lSDate;
                                obj.EndDate = lEDate;
                                obj.IsActive = true;
                                obj.ModifyBy = BusinessLogicLayer.CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(Session["ID"])); //personaldetai id
                                obj.ModifyDate = DateTime.Now;
                                db.SaveChanges();

                            }
                        }
                        else
                        {
                            HomePageDynamicSectionProduct obj_ = new HomePageDynamicSectionProduct();
                            obj_.HomePageDynamicSectionId = HomepageDynamicSectionID;
                            obj_.ShopStockId = ShopStockId;
                            obj_.IsActive = true;
                            obj_.StartDate = lSDate;
                            obj_.EndDate = lEDate;
                            obj_.CreateDate = DateTime.Now;
                            obj_.CreatedBy = BusinessLogicLayer.CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(Session["ID"])); //personaldetai id
                            obj_.ModifyBy = BusinessLogicLayer.CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(Session["ID"]));                                                                                                        /// obj_.ModifyBy = BusinessLogicLayer.CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(Session["ID"])); //personaldetai id
                            obj_.ModifyDate = DateTime.Now;
                            obj_.NetworkIp = BusinessLogicLayer.CommonFunctions.GetClientIP();
                            db.HomePageDynamicSectionProduct.Add(obj_);
                            db.SaveChanges();
                        }

                    }


                    ModelState.Clear();
                    ViewBag.Message = "Done! Product Added Successfully!!";
                    TempData["Message"] = ViewBag.Message;
                    return RedirectToAction("Index", new { CityList = CityId_, franchiseList = Franchiseid });
                }


            }


            catch (Exception ex)
            {
                ModelState.AddModelError("Message", "Sorry! Problem in Plan Bind registration!!");
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[HomePageDynamicSectionPortal][POST:Create]",
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
                var catList = db.Categories.Where(c => c.Level == 0);
                return RedirectToAction("Index", new { CityList = CityId_, franchiseList = Franchiseid });
            }

        }


        #endregion

        #region Method

        [HttpPost]
        public JsonResult SelectCategory(Int64 ParentCategory, int level)
        {

            List<CategoryDetail> cd = new List<CategoryDetail>();
            cd = (from n in db.Categories
                  where n.ParentCategoryID == ParentCategory && n.Level == level
                  && n.IsActive == true
                  select new CategoryDetail
                  {
                      ID = n.ID,
                      Name = n.Name
                  }).OrderBy(x => x.Name).ToList();

            return Json(cd, JsonRequestBehavior.AllowGet);

        }


        public JsonResult GetAddedProducts(int HomepageDynamicSectionID, string SDate, string EDate)
        {
            DateTime lSDate = CommonFunctions.GetProperDate(SDate);
            DateTime lEDate = CommonFunctions.GetProperDate(EDate);

            List<DynamicProductListViewModel> ProductList = new List<DynamicProductListViewModel>();
            ProductList = GetProductList(HomepageDynamicSectionID, lSDate, lEDate);

            //var ProductList = (from dcp in db.DynamicCategoryProducts
            //                   where dcp.FranchiseID == FranchiseID && dcp.StartDate >= lSDate && dcp.EndDate <= lEDate
            //                   select new DynamicProductListViewModel { ID = dcp.ID, Name = dcp.Product.Name }).ToList().OrderBy(x => x.Name);
            return Json(ProductList, JsonRequestBehavior.AllowGet);
        }

        public List<DynamicProductListViewModel> GetProductList(int HomepageDynamicSectionID, DateTime lSDate, DateTime lEDate)
        {
            List<DynamicProductListViewModel> ProductList = new List<DynamicProductListViewModel>();
            DataTable dt = new DataTable();
            ReadConfig config = new ReadConfig(System.Web.HttpContext.Current.Server);
            DataAccessLayer.DbOperations dbOpr = new DataAccessLayer.GetData(config.DB_CONNECTION);
            List<object> paramValues = new List<object>();
            paramValues.Add(HomepageDynamicSectionID);
            paramValues.Add(lSDate);
            paramValues.Add(lEDate);

            dt = dbOpr.GetRecords("Select_HomeProductForDynamicList", paramValues);

            ProductList = (from n in dt.AsEnumerable()
                           select new DynamicProductListViewModel
                           {
                               ID = n.Field<Int64>("id"),
                               Name = n.Field<string>("Name")
                           }).ToList();
            return ProductList;
        }


        [HttpPost]
        public PartialViewResult GetProductList(int FranchiseID, int HomepageDynamicSectionID, int CategoryID)
        {
            var ProductList = (from sp in db.ShopProducts
                               join ss in db.ShopStocks on sp.ID equals ss.ShopProductID
                               where sp.Shop.FranchiseID == FranchiseID
                               && sp.Product.CategoryID == CategoryID
                               && sp.Product.IsActive == true && sp.IsActive == true && sp.Shop.IsLive == true && sp.Shop.IsActive == true && ss.IsActive == true
                               && ss.Qty > 0
                               && !
                                    (from bi in db.HomePageDynamicSectionProduct
                                     where bi.HomePageDynamicSectionId == HomepageDynamicSectionID && bi.IsActive == true
                                     select new
                                     {
                                         bi.ShopStockId
                                     }).ToList().Select(p => p.ShopStockId).Contains(ss.ID)
                               select new DynamicProductListViewModel { ID = sp.ProductID, Name = sp.Product.Name }).Distinct().ToList();

            return PartialView("_ProductListingHomepage", ProductList);
        }




        public JsonResult GetDropDetails(string EnumList, int franchiseId)
        {

            if (EnumList == "lvl1categorylist" || EnumList == "lvl2Categorylist" || EnumList == "OfferProductList")
            {
                if (EnumList == "lvl1categorylist" || EnumList == "lvl2Categorylist")
                {
                    long cityId = 0;

                    if (franchiseId > 0)
                    {
                        var result = (from f in db.Franchises
                                      join p in db.Pincodes on f.PincodeID equals p.ID
                                      where f.ID == franchiseId
                                      select new
                                      {
                                          CityId = p.CityID
                                      }).FirstOrDefault();
                        if (result != null)
                        {
                            cityId = result.CityId;
                        }
                    }
                    BusinessLogicLayer.ReadConfig rcKey = new BusinessLogicLayer.ReadConfig(System.Web.HttpContext.Current.Server);
                    BusinessLogicLayer.FranchiseMenuList obj = new BusinessLogicLayer.FranchiseMenuList();
                    DataTable dt = new DataTable();
                    // dt = obj.Select_FranchiseMenu(cityId, System.Web.HttpContext.Current.Server);////hide
                    dt = obj.Select_FranchiseMenu(cityId, franchiseId, System.Web.HttpContext.Current.Server);////added
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
                    ViewBag.categorylist = new SelectList(FMenu, "ID", "CategoryName");
                    foreach (MenuViewModel M in levelOneMenu)
                    {
                        M.LevelTwoListing = FMenu.Where(x => x.Level == 2 && x.ParentCategoryID == M.ID).ToList();

                    }
                    var leveltwomenu = FMenu.Where(x => x.Level == 2).ToList().OrderBy(x => x.SequenceOrder);
                    menu objmenu = new menu();
                    if (EnumList == "lvl1categorylist")
                    {
                        objmenu.levelOneMenu = FMenu.Where(x => x.Level == 1).ToList().OrderBy(x => x.SequenceOrder).Select(p => new MenuViewModel { ID = p.ID, CategoryName = p.CategoryName }).ToList();
                    }
                    else
                    {
                        objmenu.LevelTwoListing = FMenu.Where(x => x.Level == 2).ToList().ToList().OrderBy(x => x.SequenceOrder).Select(p => new MenuViewModel { ID = p.ID, CategoryName = p.CategoryName }).ToList();

                    }

                    return Json(objmenu, JsonRequestBehavior.AllowGet);
                }
                else
                {

                    menu objmenu = new menu();
                    var LevelThree = db.Categories.Where(x => x.Level == 3 && x.IsActive == true).ToList().OrderBy(x => x.ID);
                    objmenu.LevelThreeListing = new List<MenuViewModel>();
                    foreach (var item in LevelThree)
                    {
                        MenuViewModel objmenuViewModel = new MenuViewModel();
                        objmenuViewModel.ID = item.ID;
                        objmenuViewModel.CategoryName = item.Name;
                        objmenu.LevelThreeListing.Add(objmenuViewModel);
                    }

                    //var ShopName = db.Shops.Where(s => s.FranchiseID == franchiseId).Select(s => s.Name).ToList();
                    //objmenu.ShopList = new List<ShopViewModellist>();
                    //foreach (var item in ShopName)
                    //{
                    //    ShopViewModellist objShopViewModel = new ShopViewModellist();

                    //    objShopViewModel.Name = item;
                    //    objmenu.ShopList.Add(objShopViewModel);
                    //}

                    var ShopName = db.Shops.Where(s => s.FranchiseID == franchiseId).ToList();
                    objmenu.ShopList = new List<ShopViewModellist>();
                    foreach (var item in ShopName)
                    {
                        ShopViewModellist objShopViewModel = new ShopViewModellist();
                        objShopViewModel.Id = item.ID;
                        objShopViewModel.Name = item.Name;
                        objmenu.ShopList.Add(objShopViewModel);
                    }

                    objmenu.offerLists = new List<OfferIDList>();
                    List<OfferIDList> OfferList = (from s in db.Shops
                                                   join o in db.Offers on s.ID equals o.OwnerID
                                                   join od in db.OfferDurations on o.ID equals od.OfferID
                                                   where o.IsActive == true && od.IsActive == true && s.IsActive == true && od.StartDateTime <= EntityFunctions.TruncateTime(DateTime.Now) && od.EndDateTime >= EntityFunctions.TruncateTime(DateTime.Now) && s.FranchiseID == franchiseId
                                                   select new OfferIDList
                                                   {
                                                       Id = o.ID,
                                                       Name = o.Description
                                                   }
                                      ).ToList();
                    objmenu.offerLists = OfferList;


                    var BrandName = db.Brands.OrderBy(x => x.Name).Where(b => b.IsActive == true).ToList();
                    objmenu.BrandList = new List<BrandViewModel>();
                    foreach (var item in BrandName)
                    {
                        BrandViewModel objbrandViewModel = new BrandViewModel();
                        objbrandViewModel.Id = item.ID;
                        objbrandViewModel.Name = item.Name;
                        objmenu.BrandList.Add(objbrandViewModel);
                    }

                    return Json(objmenu, JsonRequestBehavior.AllowGet);

                }

            }
            else
            {
                return Json(EnumList);
            }

        }




        [HttpGet]
        public ActionResult GetProductListInGallery(Int64 FranchiseID, Int64 HomepageDynamicSectionID)
        {
            var CityID = (from f in db.Franchises
                          where f.ID == FranchiseID && f.IsActive == true && f.BusinessDetail.UserLogin.IsLocked == false && f.BusinessDetail.Pincode.City.IsActive == true
                          select new { ID = f.BusinessDetail.Pincode.CityID }).Distinct().FirstOrDefault();
            long CityId_ = 0;
            if (CityID != null)
            {
                CityId_ = CityID.ID;
            }
            ModelLayer.Models.ViewModel.HomePageDynamicBannerSetSequence ls = new ModelLayer.Models.ViewModel.HomePageDynamicBannerSetSequence();

            try
            {
                BusinessLogicLayer.ReadConfig rcKey = new BusinessLogicLayer.ReadConfig(System.Web.HttpContext.Current.Server);
                DataTable dt = new DataTable();
                dt = BusinessLogicLayer.HomePageDynamicSectionPortal.Select_GalleryProducts(FranchiseID, HomepageDynamicSectionID, System.Web.HttpContext.Current.Server);
                int HomePageSectionId = (int)db.HomePageDynamicSection.Where(x => x.ID == HomepageDynamicSectionID).Select(x => x.SectionId).FirstOrDefault();
                /*Select All the Shop By Franchise */
                ls.HomeDynamicBannersetsequence = (from n in dt.AsEnumerable()
                                                   select new ModelLayer.Models.ViewModel.HomePageDynamicBannerSetSequenceViewModel
                                                   {
                                                       ID = n.Field<Int64>("ID"),
                                                       FranchiseID = n.Field<Int64>("FranchiseID"),
                                                       EndDate = n.Field<DateTime>("EndDate"),
                                                       StartDate = n.Field<DateTime>("StartDate"),
                                                       Imagename = rcKey.HomePageDynamicSection_IMAGE_HTTP + CityId_ + "/" + FranchiseID + "/" + HomePageSectionId + "/" + n.Field<string>("ImageName"),
                                                       //LinkUrl = n.Field<string>("LinkUrl"),
                                                       //Tooltip = n.Field<string>("Tooltip"),
                                                       SequenceOrder = n.Field<int?>("SequenceOrder") == null ? 0 : Convert.ToInt32(n.Field<int?>("SequenceOrder")),
                                                       IsActive = n.Field<bool>("IsActive")
                                                   }).OrderBy(x => x.SequenceOrder).ToList();

                //ViewBag.CategoryID = CategoryID;
                ViewBag.Franchise = FranchiseID;

                return PartialView("_HomePageDynamicSectionBannerMobileSetSequence", ls);

            }
            catch (Exception ex)
            {
                return PartialView("_HomePageDynamicSectionBannerMobileSetSequence", ls);
            }
        }


        public ActionResult UpdateBannerSequence(ModelLayer.Models.ViewModel.HomePageDynamicBannerSetSequence ls, Int64 Franchise)
        {

            BusinessLogicLayer.HomePageDynamicSectionPortal obj = new BusinessLogicLayer.HomePageDynamicSectionPortal();
            Int64 userID = CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(Session["ID"]));

            TempData["Message"] = obj.HomePageBannerSequenceUpdate(ls, Franchise, userID, System.Web.HttpContext.Current.Server);

            return RedirectToAction("Index");
        }



        [HttpPost]
        public PartialViewResult AutomateProductList(Int32 FranchiseID, Int32 HomepageDynamicSectionID)
        {
            HomePageDynamicSection obj = db.HomePageDynamicSection.FirstOrDefault(p => p.IsActive == true && p.ID == HomepageDynamicSectionID);
            long SectionId = obj.SectionId;


            List<DynamicProductListViewModel> list = new List<DynamicProductListViewModel>();
            switch (SectionId)
            {
                case 6: //Hot Deals and offers
                    //list = (from sp in db.ShopProducts
                    //        join ss in db.ShopStocks on sp.ID equals ss.ShopProductID
                    //        where sp.Shop.FranchiseID == FranchiseID
                    //        //&& sp.Product.CategoryID == CategoryID
                    //        && sp.Product.IsActive == true && sp.IsActive == true && sp.Shop.IsLive == true && sp.Shop.IsActive == true && ss.IsActive == true
                    //        && ss.Qty > 0
                    //        && !
                    //             (from bi in db.HomePageDynamicSectionProduct
                    //              where bi.HomePageDynamicSectionId == HomepageDynamicSectionID && bi.IsActive == true
                    //              select new
                    //              {
                    //                  bi.ShopStockId
                    //              }).ToList().Select(p => p.ShopStockId).Contains(ss.ID)
                    //        orderby ss.BusinessPoints, (ss.MRP - ss.RetailerRate) descending
                    //        select new DynamicProductListViewModel { ID = sp.ProductID, Name = sp.Product.Name }).Distinct().Take(10).ToList();


                    //list = (from s in db.Shops 
                    //        join o in db.Offers on s.ID equals o.OwnerID
                    //        join ozp in db.OfferZoneProducts  on o.ID equals ozp.OfferID
                    //        where s.FranchiseID == FranchiseID 
                    //        && !
                    //            (from bi in db.HomePageDynamicSectionProduct
                    //             where bi.HomePageDynamicSectionId == HomepageDynamicSectionID && bi.IsActive == true
                    //             select new
                    //             {
                    //                 bi.ShopStockId
                    //             }).ToList().Select(p => p.ShopStockId).Contains(ss.ID)
                    //        orderby ss.BusinessPoints, (ss.MRP - ss.RetailerRate) descending
                    //        select new DynamicProductListViewModel { ID = sp.ProductID, Name = sp.Product.Name }).Distinct().Take(10).ToList()
                    //        )

                    break;
                case 7: //Deals of the day
                    var random = new Random().Next();
                    //Random random = new Random();
                    list = (from sp in db.ShopProducts
                            join ss in db.ShopStocks on sp.ID equals ss.ShopProductID
                            where sp.Shop.FranchiseID == FranchiseID
                            //&& sp.Product.CategoryID == CategoryID
                            && sp.Product.IsActive == true && sp.IsActive == true && sp.Shop.IsLive == true && sp.Shop.IsActive == true && ss.IsActive == true
                            && ss.Qty > 0
                            && !
                                 (from bi in db.HomePageDynamicSectionProduct
                                  where bi.HomePageDynamicSectionId == HomepageDynamicSectionID && bi.IsActive == true
                                  select new
                                  {
                                      bi.ShopStockId

                                  }).ToList().Select(p => p.ShopStockId).Contains(ss.ID)
                            orderby ss.BusinessPoints, (ss.MRP - ss.RetailerRate) descending
                            select new DynamicProductListViewModel { ID = sp.ProductID, Name = sp.Product.Name }).Distinct().OrderBy(x => random).Take(10).ToList();
                    // select new DynamicProductListViewModel { ID = sp.ProductID, Name = sp.Product.Name }).Distinct().Take(10).ToList();

                    break;
                case 8: //Major Retail points
                    list = (from sp in db.ShopProducts
                            join ss in db.ShopStocks on sp.ID equals ss.ShopProductID
                            where sp.Shop.FranchiseID == FranchiseID
                            //&& sp.Product.CategoryID == CategoryID
                            && sp.Product.IsActive == true && sp.IsActive == true && sp.Shop.IsLive == true && sp.Shop.IsActive == true && ss.IsActive == true
                            && ss.Qty > 0
                            && !
                                 (from bi in db.HomePageDynamicSectionProduct
                                  where bi.HomePageDynamicSectionId == HomepageDynamicSectionID && bi.IsActive == true
                                  select new
                                  {
                                      bi.ShopStockId
                                  }).ToList().Select(p => p.ShopStockId).Contains(ss.ID)
                            orderby ss.BusinessPoints descending
                            select new DynamicProductListViewModel { ID = sp.ProductID, Name = sp.Product.Name }).Distinct().Take(10).ToList();

                    break;
                case 9: //48 Hours Deal
                    list = (from sp in db.ShopProducts
                            join ss in db.ShopStocks on sp.ID equals ss.ShopProductID
                            join ws in db.WarehouseStocks on ss.WarehouseStockID equals ws.ID
                            //join id in db.InvoiceDetails on 
                            where sp.Shop.FranchiseID == FranchiseID
                            // && sp.Product.CategoryID == CategoryID
                            && sp.Product.IsActive == true && sp.IsActive == true && sp.Shop.IsLive == true && sp.Shop.IsActive == true && ss.IsActive == true
                            && ss.Qty > 0
                            && !
                                 (from bi in db.HomePageDynamicSectionProduct
                                  where bi.HomePageDynamicSectionId == HomepageDynamicSectionID && bi.IsActive == true
                                  select new
                                  {
                                      bi.ShopStockId
                                  }).ToList().Select(p => p.ShopStockId).Contains(ss.ID)
                            orderby ss.BusinessPoints, (ss.MRP - ss.RetailerRate) descending
                            select new DynamicProductListViewModel { ID = sp.ProductID, Name = sp.Product.Name }).Distinct().Take(10).ToList();

                    break;
                case 10: //Newly Launched
                    list = (from sp in db.ShopProducts
                            join ss in db.ShopStocks on sp.ID equals ss.ShopProductID
                            join ws in db.WarehouseStocks on ss.WarehouseStockID equals ws.ID
                            //join id in db.InvoiceDetails on 
                            where sp.Shop.FranchiseID == FranchiseID
                            // && sp.Product.CategoryID == CategoryID
                            && sp.Product.IsActive == true && sp.IsActive == true && sp.Shop.IsLive == true && sp.Shop.IsActive == true && ss.IsActive == true
                            && ss.Qty > 0
                            && !
                                 (from bi in db.HomePageDynamicSectionProduct
                                  where bi.HomePageDynamicSectionId == HomepageDynamicSectionID && bi.IsActive == true
                                  select new
                                  {
                                      bi.ShopStockId
                                  }).ToList().Select(p => p.ShopStockId).Contains(ss.ID)
                            orderby ws.CreateDate descending
                            select new DynamicProductListViewModel { ID = sp.ProductID, Name = sp.Product.Name }).Distinct().Take(10).ToList();

                    break;
                case 11: //Trending Deals
                    list = (from sp in db.ShopProducts
                            join ss in db.ShopStocks on sp.ID equals ss.ShopProductID
                            join cod in db.CustomerOrderDetails on ss.ID equals cod.ShopStockID
                            join p in db.Products on sp.ProductID equals p.ID
                            where sp.Shop.FranchiseID == FranchiseID
                            && sp.Product.IsActive == true && sp.IsActive == true && sp.Shop.IsLive == true && sp.Shop.IsActive == true && ss.IsActive == true
                            && ss.Qty > 0
                            && !
                                 (from bi in db.HomePageDynamicSectionProduct
                                  where bi.HomePageDynamicSectionId == HomepageDynamicSectionID && bi.IsActive == true
                                  select new
                                  {
                                      bi.ShopStockId
                                  }).ToList().Select(p => p.ShopStockId).Contains(ss.ID)

                            orderby cod.CreateDate descending
                            select new DynamicProductListViewModel { ID = sp.ID, Name = sp.Product.Name }).Distinct().Take(10).ToList();


                    //list = list.GroupBy(x => x.ID).Select(g => new { Name =   });
                    break;
                default:

                    break;
            }
            return PartialView("_ProductListingHomepage", list);
        }


        [HttpPost]
        public JsonResult GetSectionStyle(int EnumList)
        {

            if (EnumList == 1 || EnumList == 5)
            {
                var SectionStyle = Enum.GetNames(typeof(ModelLayer.Models.Enum.Margin_DivisionConstants.Select_StyleMobile));

                var list = SectionStyle.ToList().Select(p => new
                {
                    ID = p,
                    Name = p
                });
                //var checkId = MAIN_BANNER";
                list = list.Where(x => x.ID == "MAIN_BANNER").ToList();
                return Json(list);
            }
            if (EnumList == 12 || EnumList == 13 || EnumList == 14 || EnumList == 15 || EnumList == 16)
            {
                var SectionStyle = Enum.GetNames(typeof(ModelLayer.Models.Enum.Margin_DivisionConstants.Select_StyleMobile));

                var list = SectionStyle.ToList().Select(p => new
                {
                    ID = p,
                    Name = p
                });
                //var checkId = MAIN_BANNER";
                list = list.Where(x => x.ID == "FULL_WIDTH_MAIN_BANNER_AND_BELOW_TWO_SUBCAT_BANNERS").ToList();
                return Json(list);
            }
            if (EnumList == 6 || EnumList == 7 || EnumList == 8 || EnumList == 9 || EnumList == 10 || EnumList == 11)
            {
                var SectionStyle = Enum.GetNames(typeof(ModelLayer.Models.Enum.Margin_DivisionConstants.Select_StyleMobile));

                var list = SectionStyle.ToList().Select(p => new
                {
                    ID = p,
                    Name = p
                });
                //var checkId = MAIN_BANNER";
                list = list.Where(x => x.ID == "FULL_WIDTH_MAIN_BANNER_AND_PRODUCT_BELOW").ToList();
                return Json(list);
            }
            else
            {
                return Json(EnumList);
            }



        }


        //public JsonResult GetBlocksize(Int64 ID)
        //{

        //    long sectionid = db.HomePageDynamicSection.Where(x => x.ID == ID).Select(a => a.SectionId).FirstOrDefault();
        //    var blocksize = (from dbt in db.HomePageDynamicSectionsMasters where dbt.ID == sectionid select new { dbt.MobileImgHeight, dbt.MobileImgWidth ,dbt.MobileImgSize}).FirstOrDefault();
        //   return Json(blocksize, JsonRequestBehavior.AllowGet);
        //}


        [HttpPost]
        public JsonResult GetBlocksize(string sectionsty)
        {

            var blocksize = (from dbt in db.HomePageDynamicSectionStyle where dbt.SectionStyle == sectionsty select new { dbt.Mobile_Banner_Height, dbt.Mobile_Banner_Width, dbt.Mobile_Banner_Size }).FirstOrDefault();
            return Json(blocksize, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetBlocksizecategory(string sectionsty)
        {

            var blocksize = (from dbt in db.HomePageDynamicSectionStyle where dbt.SectionStyle == sectionsty select new { dbt.Mobile_Category_Height, dbt.Mobile_Category_Width, dbt.Mobile_Category_Size }).FirstOrDefault();
            return Json(blocksize, JsonRequestBehavior.AllowGet);
        }



        public JsonResult getsequencedeatail(int id, int sequenceOrder)
        {
            string msg = "";
            if (db.HomePageDynamicSectionBanner.Any(p => p.HomePageDynamicSectionId == id && p.SequenceOrder == sequenceOrder))
            {
                msg = " Sequence Order is already exist. Last value is " + db.HomePageDynamicSectionBanner.Where(p => p.HomePageDynamicSectionId == id).Max(p => p.SequenceOrder).ToString();
            }
            return Json(msg, JsonRequestBehavior.AllowGet);
        }


        //[HttpPost]
        public JsonResult GetsectionDetail(int id, int FranchiseID)
        {
            string msg = "";
            if (db.HomePageDynamicSection.Any(p => p.SectionId == id && p.FranchiseId == FranchiseID && p.ShowInApp))
            {
                msg = "This Section Is Already Exist.";
            }

            return Json(msg, JsonRequestBehavior.AllowGet);
        }




        public JsonResult Getofferlist(int FranchiseID)
        {
            var OfferList = (from s in db.Shops
                             join o in db.Offers on s.ID equals o.OwnerID
                             join od in db.OfferDurations on o.ID equals od.OfferID
                             where o.IsActive == true && od.IsActive == true && s.IsActive == true && EntityFunctions.TruncateTime(od.StartDateTime) <= EntityFunctions.TruncateTime(DateTime.Now) && EntityFunctions.TruncateTime(od.EndDateTime) >= EntityFunctions.TruncateTime(DateTime.Now) && s.FranchiseID == FranchiseID
                             select new OfferIDList
                             {
                                 Id = o.ID,
                                 Name = o.Description
                             }
                               ).ToList();


            return Json(OfferList, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public PartialViewResult Autolist(Int32 franchiseID, Int32 offerID, Int32 HomepageDynamicSectionID)
        {
            var CityID = (from f in db.Franchises
                          where f.ID == franchiseID && f.IsActive == true && f.BusinessDetail.UserLogin.IsLocked == false && f.BusinessDetail.Pincode.City.IsActive == true
                          select new { ID = f.BusinessDetail.Pincode.CityID }).Distinct().FirstOrDefault();
            long CityId_ = 0;
            if (CityID != null)
            {
                CityId_ = CityID.ID;
            }
            Offers lOffers = new Offers();

            List<OfferProducts> lOfferProducts = lOffers.GetOfferProducts(CityId_, OfferStatus.AVAILABLEDEALS, 0, 1, 12, franchiseID).Where(x => x.OfferID == offerID).ToList();////added FranchiseID for Mutiple MCO

            var list = (from n in lOfferProducts
                        where !(from bi in db.HomePageDynamicSectionProduct
                                where bi.HomePageDynamicSectionId == HomepageDynamicSectionID && bi.IsActive == true
                                select new
                                {
                                    bi.ShopStockId
                                }).ToList().Select(p => p.ShopStockId).Contains(n.ShopStockID)

                        select new DynamicProductListViewModel { ID = n.ProductID, Name = n.ProductName }
                        ).ToList();
            // select new DynamicProductListViewModel { ID = sp.ID, Name = sp.Product.Name }).ToList();


            return PartialView("_ProductListingHomepage", list);
        }



        [HttpPost]
        public PartialViewResult searchproduct(Int32 franchiseID, string product, Int32 HomepageDynamicSectionID)
        {
            var CityID = (from f in db.Franchises
                          where f.ID == franchiseID && f.IsActive == true && f.BusinessDetail.UserLogin.IsLocked == false && f.BusinessDetail.Pincode.City.IsActive == true
                          select new { ID = f.BusinessDetail.Pincode.CityID }).Distinct().FirstOrDefault();
            int CityId_ = 0;
            if (CityID != null)
            {
                CityId_ = Convert.ToInt32(CityID.ID);
                //int City_ = Convert.ToInt32(CityId_);
            }
            ProductSearchViewModel productSearch = new ProductSearchViewModel();
            productSearch.PageIndex = 1;
            productSearch.Keyword = product;
            productSearch.CityID = CityId_;
            productSearch.FranchiseID = franchiseID;
            productSearch.PageSize = 50;//Sonali_03-11-2018_For display all product
            productSearch.Version = 0;
            ProductWithRefinementViewModel productWithRefinementViewModel = new ProductWithRefinementViewModel();
            ProductList productList = new ProductList(System.Web.HttpContext.Current.Server);
            productWithRefinementViewModel = productList.GetProductList(productSearch, false);
            List<long> ShopStockIds = productWithRefinementViewModel.productList.Select(x => x.ShopStockID).ToList();

            var list = (from n in productWithRefinementViewModel.productList
                        where !(from bi in db.HomePageDynamicSectionProduct
                                where bi.HomePageDynamicSectionId == HomepageDynamicSectionID && bi.IsActive == true
                                select new
                                {
                                    bi.ShopStockId
                                }).ToList().Select(p => p.ShopStockId).Contains(n.ShopStockID)

                        select new DynamicProductListViewModel { ID = n.ProductID, Name = n.Name }
                        ).ToList();
            // select new DynamicProductListViewModel { ID = sp.ID, Name = sp.Product.Name }).ToList();


            return PartialView("_ProductListingHomepage", list);
        }
        #endregion

        #region DeleteHomePageDyanmicSection

        public ActionResult DeleteHomePageDyanmicSection(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }


            HomePageDynamicSection homePageDynamicSections = db.HomePageDynamicSection.Find(id);
            var SectionStyle = Enum.GetNames(typeof(ModelLayer.Models.Enum.Margin_DivisionConstants.Select_StyleMobile));

            var list = SectionStyle.ToList().Select(p => new
            {
                ID = p,
                Name = p
            });
            ViewBag.SectionStyle = new SelectList(list, "ID", "Name", homePageDynamicSections.SectionStyle);
            //ViewBag.SectionStyle = new SelectList("ID", "Name");

            ViewBag.FranchiseID = new SelectList((from f in db.Franchises where f.ID != 1 && f.PincodeID != null select new { f.ID, f.BusinessDetail.Name }).ToList().OrderBy(x => x.Name), "ID", "Name", homePageDynamicSections.FranchiseId);
            //ViewBag.SectionHeader = new SelectList((from h in HomePageDynamicSectionsMasters where h.))
            //ViewBag.SectionHeader = new SelectList(db.HomePageDynamicSectionsMasters.Select(h =>h.SectionHeader).ToList(),"ID", "SectionHeader");
            ViewBag.SectionHeader = new SelectList((from h in db.HomePageDynamicSectionsMasters where h.SectionHeader != null select new { h.ID, h.SectionHeader }).ToList().OrderBy(x => x.SectionHeader), "ID", "SectionHeader", homePageDynamicSections.SectionId);

            if (homePageDynamicSections == null)
            {
                return HttpNotFound();
            }
            return View(homePageDynamicSections);
        }

        [HttpPost]
        public ActionResult DeleteHomePageDyanmicSection(long id, HomePageDynamicSection homePageDynamicSections)
        {
            int FranchiseID = (int)db.HomePageDynamicSection.Where(x => x.ID == homePageDynamicSections.ID).Select(x => x.FranchiseId).FirstOrDefault();
            var CityID = (from f in db.Franchises
                          where f.ID == FranchiseID && f.IsActive == true && f.BusinessDetail.UserLogin.IsLocked == false && f.BusinessDetail.Pincode.City.IsActive == true
                          select new { ID = f.BusinessDetail.Pincode.CityID }).Distinct().FirstOrDefault();
            long CityId_ = 0;
            if (CityID != null)
            {
                CityId_ = CityID.ID;
            }

            try
            {


                HomePageDynamicSection homePageDynamic = db.HomePageDynamicSection.Find(id);

                db.HomePageDynamicSection.Remove(homePageDynamic);
                db.SaveChanges();
                ViewBag.Message = "Done! Deleted Successfully!!";
                TempData["Message"] = ViewBag.Message;
                return RedirectToAction("Index", new { CityList = CityId_, franchiseList = FranchiseID });

            }
            catch
            {
                ViewBag.Message = "Sorry Section Is Not Deleted !!";
                TempData["Message"] = ViewBag.Message;
                return RedirectToAction("Index", new { CityList = CityId_, franchiseList = FranchiseID });
            }



        }




        #endregion

        #region DeleteBanner

        public ActionResult DeleteBanner(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            BusinessLogicLayer.ReadConfig rcKey = new BusinessLogicLayer.ReadConfig(System.Web.HttpContext.Current.Server);

            HomePageDynamicSectionBanner homePageDynamicSectionBanner = db.HomePageDynamicSectionBanner.Find(id);

            //--- comment is use for show category like level oneid two and three id
            //var LevelThree = db.Categories.Where(x => x.Level == 3 && x.IsActive == true).ToList().OrderBy(x => x.ID);
            //ViewBag.CategoryID = new SelectList(LevelThree, "ID", "Name", homePageDynamicSectionBanner.CategoryID);

            var BrandName = db.Brands.OrderBy(x => x.Name).Where(b => b.IsActive == true).ToList();
            ViewBag.BrandName = new SelectList(BrandName, "ID", "Name", homePageDynamicSectionBanner.BrandID);
            //var OfferList = (from s in db.Shops
            //                 join o in db.Offers on s.ID equals o.OwnerID
            //                 join od in db.OfferDurations on o.ID equals od.OfferID
            //                 where o.IsActive == true && od.IsActive == true && s.IsActive == true && od.StartDateTime <= EntityFunctions.TruncateTime(DateTime.Now) && od.EndDateTime >= EntityFunctions.TruncateTime(DateTime.Now) && s.FranchiseID == franchiseId
            //                 select new OfferIDList
            //                 {
            //                     Id = o.ID,
            //                     Name = o.Description
            //                 }).ToList();
            ViewBag.Offername = new SelectList("ID", "Name");

            HomePageDynamicSection objHomePageDynamicSection = db.HomePageDynamicSection.FirstOrDefault(p => p.ID == homePageDynamicSectionBanner.HomePageDynamicSectionId);
            int HomepageDynamicsctionID = Convert.ToInt32(objHomePageDynamicSection.ID);
            if (objHomePageDynamicSection != null)
            {
                Franchise f = db.Franchises.FirstOrDefault(p => p.ID == objHomePageDynamicSection.FranchiseId);
                ViewBag.franchiseId = f.ID;
                if (f != null)
                {
                    ViewBag.FranchiseName = db.BusinessDetails.FirstOrDefault(p => p.ID == f.BusinessDetailID).Name;
                }
                else
                {
                    ViewBag.FranchiseName = "";
                }
                ViewBag.SectionStyle = objHomePageDynamicSection.SectionStyle;
                ViewBag.SectionDisplayName = objHomePageDynamicSection.SectionDisplayName;
            }

            long FranchiseID = (Int64)db.HomePageDynamicSection.Where(s => s.ID == homePageDynamicSectionBanner.HomePageDynamicSectionId).Select(a => a.FranchiseId).FirstOrDefault();
            //long CityID = (from c in db.)
            var CityID = (from f in db.Franchises
                          where f.ID == FranchiseID && f.IsActive == true && f.BusinessDetail.UserLogin.IsLocked == false && f.BusinessDetail.Pincode.City.IsActive == true
                          select new { ID = f.BusinessDetail.Pincode.CityID }).Distinct().FirstOrDefault();
            long CityId_ = 0;
            if (CityID != null)
            {
                CityId_ = CityID.ID;
            }
            int HomePageSectionId = (int)db.HomePageDynamicSection.Where(x => x.ID == HomepageDynamicsctionID).Select(x => x.SectionId).FirstOrDefault();
            homePageDynamicSectionBanner.ImageName_ = rcKey.HomePageDynamicSection_IMAGE_HTTP + CityId_ + "/" + FranchiseID + "/" + HomePageSectionId + "/" + homePageDynamicSectionBanner.ImageName;

            var SectionStyle = Enum.GetNames(typeof(ModelLayer.Models.Enum.Margin_DivisionConstants.HomePageDynamicOfferList));

            var list = SectionStyle.ToList().Select(p => new
            {
                ID = p,
                Name = p
            });

            ViewBag.Category = new SelectList(list, "ID", "Name", homePageDynamicSectionBanner.DisplayViewApp);
            List<OfferIDList> OfferList = (from s in db.Shops
                                           join o in db.Offers on s.ID equals o.OwnerID
                                           join od in db.OfferDurations on o.ID equals od.OfferID
                                           where o.IsActive == true && od.IsActive == true && s.IsActive == true && od.StartDateTime <= EntityFunctions.TruncateTime(DateTime.Now) && od.EndDateTime >= EntityFunctions.TruncateTime(DateTime.Now) && s.FranchiseID == FranchiseID
                                           select new OfferIDList
                                           {
                                               Id = o.ID,
                                               Name = o.Description
                                           }
                                     ).ToList();
            ViewBag.Offername = new SelectList(OfferList, "ID", "Name", homePageDynamicSectionBanner.OfferId);
            //var Category = Enum.GetNames(typeof(ModelLayer.Models.Enum.Margin_DivisionConstants.HomePageDynamicOfferList));
            //var list = Category.ToList().Select(p => new
            //{
            //    ID = p,
            //    Name = p
            //});
            //ViewBag.Category = new SelectList(list, "ID", "Name", objHomePageDynamicSection.SectionDisplayName);
            //var ShopName = db.Shops.Where(s => s.FranchiseID == franchiseId).Select(s => s.Name).ToList();
            //var ShopID = db.Shops.Where(s => s.Name == ShopName).Select(s => s.ID).FirstOrDefault();
            var ShopName = db.Shops.Where(s => s.FranchiseID == FranchiseID && s.IsActive).ToList();


            var ShopList = (from n in ShopName
                            select new
                            {
                                ID = n.ID,
                                Name = n.Name
                            }
                            ).ToList();
            //var SelectlistShop = new SelectList(ShopList, "ID", "Name", ShopList.FirstOrDefault().ID);
            //foreach (var item in SelectlistShop)
            //{
            var SelectedShop = new SelectList(ShopList, "ID", "Name", homePageDynamicSectionBanner.ShopId);
            //    //foreach (var item in SelectedShop)
            //    //{
            //    //    item.Selected = true;
            //    //    break;
            //    //}
            //}
            ViewBag.ShopName = SelectedShop;
            //ViewBag.ShopName.ClearSelection(); //making sure the previous selection has been cleared
            //ViewBag.ShopName.FindByValue(homePageDynamicSectionBanner.ShopId).Selected = true;
            ViewBag.leveloneId = new SelectList("ID", "Name");
            ViewBag.leveltwoId = new SelectList("ID", "Name");
            ViewBag.LevelThree = new SelectList("ID", "Name");
            ViewBag.HomepageDynamicSectionID = HomepageDynamicsctionID;
            ViewBag.SDate = homePageDynamicSectionBanner.StartDate.ToString("dd/MM/yyyy");
            ViewBag.EDate = homePageDynamicSectionBanner.EndDate.ToString("dd/MM/yyyy");

            homePageDynamicSectionBanner.IsActive_ = homePageDynamicSectionBanner.IsActive;
            homePageDynamicSectionBanner.IsBanner_ = homePageDynamicSectionBanner.IsBanner;
            if (homePageDynamicSectionBanner == null)
            {
                return HttpNotFound();
            }
            return View(homePageDynamicSectionBanner);
        }

        [HttpPost]
        public ActionResult DeleteBanner(int? id, int HomepageDynamicSectionID, HttpPostedFileBase file)
        {
            int FranchiseID = (int)db.HomePageDynamicSection.Where(x => x.ID == HomepageDynamicSectionID).Select(x => x.FranchiseId).FirstOrDefault();
            var CityID = (from f in db.Franchises
                          where f.ID == FranchiseID && f.IsActive == true && f.BusinessDetail.UserLogin.IsLocked == false && f.BusinessDetail.Pincode.City.IsActive == true
                          select new { ID = f.BusinessDetail.Pincode.CityID }).Distinct().FirstOrDefault();
            long CityId_ = 0;
            if (CityID != null)
            {
                CityId_ = CityID.ID;
            }

            try
            {
                //bool IsUploaded = false;
                //if (file != null)
                //{
                //    int HomePageSectionId = (int)db.HomePageDynamicSection.Where(x => x.ID == HomepageDynamicSectionID).Select(x => x.SectionId).FirstOrDefault();

                //    IsUploaded = CommonFunctions.DeleteHomePageDynamicSectionImage(file, CityId_, FranchiseID, HomePageSectionId, Path.GetFileName(file.FileName));

                //}
                HomePageDynamicSectionBanner homePageDynamicSectionBanner = db.HomePageDynamicSectionBanner.Find(id);
                db.HomePageDynamicSectionBanner.Remove(homePageDynamicSectionBanner);
                db.SaveChanges();
                ViewBag.Message = "Done! Product Added Successfully!!";
                TempData["Message"] = ViewBag.Message;
                return RedirectToAction("Index", new { CityList = CityId_, franchiseList = FranchiseID });

            }
            catch
            {
                ViewBag.Message = "Sorry Section Is Not Delete !!";
                TempData["Message"] = ViewBag.Message;
                return RedirectToAction("Index", new { CityList = CityId_, franchiseList = FranchiseID });
            }
        }

        #endregion

        #region Setsequence
        public ActionResult SetSequence(int id)
        {
            try
            {
                var DisplayName_ = db.HomePageDynamicSection.Where(h => h.ID == id).Select(d => d.SectionDisplayName).FirstOrDefault();
                ViewBag.DisplayName = DisplayName_;
                ViewBag.HomepageDynamicSectionid = id;
                long FranchiseID = (Int64)db.HomePageDynamicSection.Where(s => s.ID == id).Select(a => a.FranchiseId).FirstOrDefault();
                ViewBag.FranchiseID = FranchiseID;
                ViewBag.FranchiseList = id;
                List<CategoryDetail> catList = new List<CategoryDetail>();
                catList = (from c in db.Categories
                           join pbc in db.PlanCategoryCharges on c.ID equals pbc.CategoryID
                           join p in db.Plans on pbc.PlanID equals p.ID
                           join op in db.OwnerPlans on p.ID equals op.PlanID
                           where op.OwnerID == id && p.PlanCode.Substring(0, 4) == "GBFR"
                           && op.IsActive == true && pbc.IsActive == true && c.IsActive == true
                           select new CategoryDetail
                           {
                               ID = c.ID,
                               Name = c.Name
                           }).ToList();
                ViewBag.CategoryList = new SelectList(catList.OrderBy(x => x.Name), "ID", "Name");
                List<CategoryDetail> ldata = new List<CategoryDetail>();
                ldata.Add(new CategoryDetail { ID = 0, Name = "Select Category" });
                ViewBag.LevelTwoCategoryList = new SelectList(ldata, "ID", "Name");

                return View();
            }
            catch (Exception ex)
            {
                errStr.Append("Method Name[Http Request] :- Index[HttpGet]" + Environment.NewLine +
                               "ON Dated" + DateTime.UtcNow.AddHours(5.30).TimeOfDay + Environment.NewLine +
                                   ex.Message.ToString() + Environment.NewLine +
                         "====================================================================================="
                               );
                //ViewBag.Message = "Sorry! Problem in customer registration!!";
                ModelState.AddModelError("Message", "Sorry! Problem in Generate Index view!!");
                ErrorLog.ErrorLogFile("Sorry! Problem in Generate Index view!! " + Environment.NewLine + errStr.ToString()
                    , ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);

                return View();
            }
        }
        #endregion

        public class ProductDetail
        {
            public Int64 ID { get; set; }
            public string Name { get; set; }
        }
    }

}