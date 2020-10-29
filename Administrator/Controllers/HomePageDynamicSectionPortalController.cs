using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using BusinessLogicLayer;
using ModelLayer.Models;
using ModelLayer.Models.ViewModel;

namespace Administrator.Controllers
{
    public class HomePageDynamicSectionPortalController : Controller
    {
        // GET: HomePageDynamicSectionPortal
        EzeeloDBContext db = new EzeeloDBContext();
        StringBuilder errStr = new StringBuilder("=====================================================================================" +
     Environment.NewLine
     + "ErrorLog Controller : HomePageDynamicSectionPortalController" + Environment.NewLine);


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
                if (franchiseList != null)
                {

                    //var franchiseList_ = db.Franchises.Where(f => f.ID == franchiseList).FirstOrDefault();
                    ViewBag.franchiseList = new SelectList(db.Franchises.Where(f => f.ID == franchiseList), "ID", "ContactPerson");
                }
                else
                {
                    ViewBag.franchiseList = new SelectList("ID", "Name");
                }
                //ViewBag.franchiseList = new SelectList("ID", "Name");
                //ViewBag.franchiseList = new SelectList("ID", "Name");
                if (CityList != null && franchiseList != null)
                {
                    DynamicDesingTypelist = (from f in db.HomePageDynamicSection
                                             join p in db.HomePageDynamicSectionsMasters on f.SectionId equals p.SectionID
                                             where f.ShowInApp == false && f.FranchiseId == franchiseList
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

        #region Edit

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }


            HomePageDynamicSection homePageDynamicSections = db.HomePageDynamicSection.Find(id);
            var SectionStyle = Enum.GetNames(typeof(ModelLayer.Models.Enum.Margin_DivisionConstants.Select_Style));
            var list = SectionStyle.ToList().Select(p => new
            {
                ID = p,
                Name = p
            });

            ViewBag.SectionStyle = new SelectList(list, "ID", "Name", homePageDynamicSections.SectionStyle);
            ViewBag.FranchiseID = new SelectList((from f in db.Franchises where f.ID != 1 && f.PincodeID != null select new { f.ID, f.BusinessDetail.Name }).ToList().OrderBy(x => x.Name), "ID", "Name", homePageDynamicSections.FranchiseId);
            //ViewBag.SectionHeader = new SelectList((from h in HomePageDynamicSectionsMasters where h.))
            //ViewBag.SectionHeader = new SelectList(db.HomePageDynamicSectionsMasters.Select(h =>h.SectionHeader).ToList(),"ID", "SectionHeader");
            ViewBag.SectionHeader = new SelectList((from h in db.HomePageDynamicSectionsMasters where  h.SectionHeader != null select new { h.ID, h.SectionHeader }).ToList().OrderBy(x => x.SectionHeader), "ID", "SectionHeader", homePageDynamicSections.SectionId);

            if (homePageDynamicSections == null)
            {
                return HttpNotFound();
            }
            return View(homePageDynamicSections);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(HomePageDynamicSection homePageDynamicSections, string SectionStyle)
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
                    homePageDynamicSections.ModifyDate = DateTime.Now;
                    homePageDynamicSections.ModifyBy = BusinessLogicLayer.CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(Session["ID"]));
                    db.Entry(homePageDynamicSections).State = EntityState.Modified;
                    db.SaveChanges();
                    ViewBag.Message = "Done!  Added Successfully!!";
                    TempData["Message"] = ViewBag.Message;
                    return RedirectToAction("Index", new { CityList = CityId_, franchiseList = FranchiseID });
                }
                else
                {

                    var list = SectionStyle.ToList().Select(p => new
                    {
                        ID = p,
                        Name = p
                    });

                    ViewBag.SectionStyle = new SelectList(list, "ID", "Name", homePageDynamicSections.SectionStyle);
                    ViewBag.FranchiseID = new SelectList((from f in db.Franchises where f.ID != 1 && f.PincodeID != null select new { f.ID, f.BusinessDetail.Name }).ToList().OrderBy(x => x.Name), "ID", "Name", homePageDynamicSections.FranchiseId);
                    ViewBag.Message = "sorry!  failed to save!!";
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
                ErrorLog.ErrorLogFile("Sorry! Problem in Generate Edit view!! " + Environment.NewLine + errStr.ToString()
                    , ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);

                return RedirectToAction("Index", new { CityList = CityId_, franchiseList = FranchiseID });
            }


        }

        #endregion

        #region Add product
        public ActionResult AddProducts(int id)
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
        public ActionResult AddProducts(string ProductList, string RemoveIDList, int HomepageDynamicSectionID, string SDate, string EDate)
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
                        obj.NetworkIp = BusinessLogicLayer.CommonFunctions.GetClientIP();
                        obj.ModifyBy = BusinessLogicLayer.CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(Session["ID"])); //personaldetai id
                        db.Entry(obj).State = EntityState.Modified;
                        db.SaveChanges();

                    }
                    ViewBag.Message = "Done! Product Added Successfully!!";
                    TempData["Message"] = ViewBag.Message;
                    return RedirectToAction("Index", new { CityList = CityId_, franchiseList = FranchiseID });
                }
                else
                {
                    //long FranchiseID = (Int64)db.HomePageDynamicSection.Where(s => s.ID == HomepageDynamicSectionID).Select(a => a.FranchiseId).FirstOrDefault();
                    //HomePageDynamicSections fid = db.HomePageDynamicSection.FirstOrDefault(s => s.ID == FranchiseList);
                    //HomePageDynamicSections fid = db.HomePageDynamicSection.FirstOrDefault(s => s.ID == FranchiseList);
                    //var FranchiseID = fid.FranchiseId;
                    List<ProductDetail> ldata = new List<ProductDetail>();
                    ldata.Add(new ProductDetail { ID = 0, Name = "Select Category" });
                    //ViewBag.FranchiseList = HomepageDynamicSectionID;
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

                    //DataTable lDataTable = new DataTable();
                    //lDataTable.Columns.Add("ID");
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
                                    ShopStock SS = db.ShopStocks.FirstOrDefault(Ss => Ss.ShopProductID == SP.ID);
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
                            obj_.ModifyDate = DateTime.Now;
                            obj_.CreatedBy = BusinessLogicLayer.CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(Session["ID"]));
                            obj_.NetworkIp = BusinessLogicLayer.CommonFunctions.GetClientIP();
                            db.HomePageDynamicSectionProduct.Add(obj_);
                            db.SaveChanges();
                        }

                    }
                    ModelState.Clear();
                    ViewBag.Message = "Done! Product Added Successfully!!";
                    TempData["Message"] = ViewBag.Message;
                    return RedirectToAction("Index", new { CityList = CityId_, franchiseList = FranchiseID });
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
                return RedirectToAction("Index", new { CityList = CityId_, franchiseList = FranchiseID });
            }


        }

        #endregion

        #region Add Banner
        public ActionResult Addbanner(int id)
        {
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
            ViewBag.HomepageDynamicSectionID = id;
            //ViewBag.OfferCategory = new SelectList("ID", "Name");
            return View();
        }

        [HttpPost]
        public ActionResult Addbanner(HomePageDynamicSectionBanner homePageDynamicSectionBanner, int HomepageDynamicSectionID, HttpPostedFileBase file, string SDate, string EDate)
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
               

                DateTime lSDate = CommonFunctions.GetProperDate(SDate);
                DateTime lEDate = CommonFunctions.GetProperDate(EDate);
                ViewBag.SDate = lSDate.ToString("dd/MM/yyyy");
                ViewBag.EDate = lEDate.ToString("dd/MM/yyyy");
                homePageDynamicSectionBanner.IsActive = homePageDynamicSectionBanner.IsActive_;
                homePageDynamicSectionBanner.IsBanner = homePageDynamicSectionBanner.IsBanner_;
               
                homePageDynamicSectionBanner.StartDate = lSDate;
                homePageDynamicSectionBanner.EndDate = lEDate;
                homePageDynamicSectionBanner.ImageName = Path.GetFileName(file.FileName);
                homePageDynamicSectionBanner.NetworkIp = BusinessLogicLayer.CommonFunctions.GetClientIP();
                homePageDynamicSectionBanner.CreateDate = DateTime.Now;
                homePageDynamicSectionBanner.CreatedBy = BusinessLogicLayer.CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(Session["ID"]));
                if (ModelState.IsValid)
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
                            ViewBag.Message = "Done! Banner Added Successfully!!";
                            TempData["Message"] = ViewBag.Message;
                            return RedirectToAction("Index", new { CityList = CityId_, franchiseList = FranchiseID });
                        }
                        else
                        {
                            ViewBag.Messaage = ViewBag.Messaage + " Problem in Uploading  Image...";
                            TempData["Message"] = ViewBag.Messaage;
                            return RedirectToAction("Index", new { CityList = CityId_, franchiseList = FranchiseID });
                        }
                    }
                    if (!IsUploaded && file != null)
                    {
                        ViewBag.Messaage = ViewBag.Messaage + " Problem in Uploading  Image...";

                        TempData["Message"] = ViewBag.Messaage;
                        return RedirectToAction("Index", new { CityList = CityId_, franchiseList = FranchiseID });
                    }
                    else
                    {
                        ViewBag.Message = "Sorry  Something Went wrong!!";
                        TempData["Message"] = ViewBag.Message;
                        return RedirectToAction("Index", new { CityList = CityId_, franchiseList = FranchiseID });
                    }
                }
                else
                {
                    ViewBag.Message = "Sorry  Something Went wrong!!";
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

        #region Update Banner

        public ActionResult Updatebanner(int id)  // it is HomepageDynamicSection id
        {
            int HomepageDynamicsctionID = id;
            BusinessLogicLayer.ReadConfig rcKey = new BusinessLogicLayer.ReadConfig(System.Web.HttpContext.Current.Server);
            var objhomePageDynamicSectionBanner = db.HomePageDynamicSectionBanner.Where(s => s.HomePageDynamicSectionId == id).FirstOrDefault();
            long FranchiseID = (Int64)db.HomePageDynamicSection.Where(s => s.ID == HomepageDynamicsctionID).Select(a => a.FranchiseId).FirstOrDefault();
            //long FranchiseID = (Int64)db.HomePageDynamicSection.Where(s => s.ID == objhomePageDynamicSectionBanner.HomePageDynamicSectionId).Select(a => a.FranchiseId).FirstOrDefault();
            var CityID = (from f in db.Franchises
                          where f.ID == FranchiseID && f.IsActive == true && f.BusinessDetail.UserLogin.IsLocked == false && f.BusinessDetail.Pincode.City.IsActive == true
                          select new { ID = f.BusinessDetail.Pincode.CityID }).Distinct().FirstOrDefault();
            long CityId_ = 0;
            if (CityID != null)
            {
                CityId_ = CityID.ID;
            }
            var biList = db.HomePageDynamicSectionBanner.Where(x => x.HomePageDynamicSectionId == id).ToList();
            int HomePageSectionId = (int)db.HomePageDynamicSection.Where(x => x.ID == HomepageDynamicsctionID).Select(x => x.SectionId).FirstOrDefault();
            if (biList != null && biList.Count > 0)
            {
                foreach(var item in biList)
                {
                    item.ImageName = rcKey.HomePageDynamicSection_IMAGE_HTTP + CityId_ + "/" + FranchiseID + "/" + HomePageSectionId + "/" + item.ImageName;

                  
                }
            }

            return View(biList.OrderBy(x => x.SequenceOrder));
        }

        #endregion

        #region Editbanner

        public ActionResult Editbanner(int id)
        {
           
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BusinessLogicLayer.ReadConfig rcKey = new BusinessLogicLayer.ReadConfig(System.Web.HttpContext.Current.Server);
            HomePageDynamicSectionBanner homePageDynamicSectionsBanner = db.HomePageDynamicSectionBanner.Find(id);
          
            HomePageDynamicSection objHomePageDynamicSection = db.HomePageDynamicSection.FirstOrDefault(p => p.ID == homePageDynamicSectionsBanner.HomePageDynamicSectionId);
            int HomepageDynamicsctionID = Convert.ToInt32( objHomePageDynamicSection.ID);
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

            long FranchiseID = (Int64)db.HomePageDynamicSection.Where(s => s.ID == homePageDynamicSectionsBanner.HomePageDynamicSectionId).Select(a => a.FranchiseId).FirstOrDefault();
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
            homePageDynamicSectionsBanner.ImageName_ = rcKey.HomePageDynamicSection_IMAGE_HTTP + CityId_ + "/" + FranchiseID + "/" + HomePageSectionId + "/" + homePageDynamicSectionsBanner.ImageName;

            ViewBag.HomepageDynamicSectionID = HomepageDynamicsctionID;
            ViewBag.SDate = homePageDynamicSectionsBanner.StartDate.ToString("dd/MM/yyyy");
            ViewBag.EDate = homePageDynamicSectionsBanner.EndDate.ToString("dd/MM/yyyy");

            homePageDynamicSectionsBanner.IsActive_ = homePageDynamicSectionsBanner.IsActive;
            homePageDynamicSectionsBanner.IsBanner_ = homePageDynamicSectionsBanner.IsBanner;

            if (homePageDynamicSectionsBanner == null)
            {
                return HttpNotFound();
            }
            return View(homePageDynamicSectionsBanner);
        }


        [HttpPost]
        public ActionResult Editbanner(HomePageDynamicSectionBanner homePageDynamicSectionBanner, int HomepageDynamicSectionID, HttpPostedFileBase file, string SDate, string EDate)
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
               

                DateTime lSDate = CommonFunctions.GetProperDate(SDate);
                DateTime lEDate = CommonFunctions.GetProperDate(EDate);
                ViewBag.SDate = lSDate.ToString("dd/MM/yyyy");
                ViewBag.EDate = lEDate.ToString("dd/MM/yyyy");
                homePageDynamicSectionBanner.IsActive = homePageDynamicSectionBanner.IsActive_;
                homePageDynamicSectionBanner.IsBanner = homePageDynamicSectionBanner.IsBanner_;
                homePageDynamicSectionBanner.StartDate = lSDate;
                homePageDynamicSectionBanner.EndDate = lEDate;
               if (file != null)
                {
                    homePageDynamicSectionBanner.ImageName = Path.GetFileName(file.FileName);
                }
                homePageDynamicSectionBanner.NetworkIp = BusinessLogicLayer.CommonFunctions.GetClientIP();
                homePageDynamicSectionBanner.CreateDate = DateTime.Now;
                homePageDynamicSectionBanner.CreatedBy = BusinessLogicLayer.CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(Session["ID"]));
                if (ModelState.IsValid)
                {
                    bool IsUploaded = false;
                    if (file != null)
                    {
                        //upload cat image here
                        int HomePageSectionId = (int)db.HomePageDynamicSection.Where(x => x.ID == HomepageDynamicSectionID).Select(x => x.SectionId).FirstOrDefault();

                        IsUploaded = CommonFunctions.UploadHomePageDynamicSectionImage(file, CityId_, FranchiseID, HomePageSectionId, Path.GetFileName(file.FileName));////added FranchiseID
                        if (IsUploaded)
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
                            ViewBag.Messaage = ViewBag.Messaage + " Problem in Uploading  Image...";
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
                        //ViewBag.Messaage = ViewBag.Messaage + " Problem in Uploading  Image...";

                        //TempData["Message"] = ViewBag.Messaage;
                        //return RedirectToAction("Index");
                    }
                    else
                    {
                        ViewBag.Message = "Sorry  Something Went wrong!!";
                        TempData["Message"] = ViewBag.Message;
                        return RedirectToAction("Index", new { CityList = CityId_, franchiseList = FranchiseID });
                    }

                   
                    
                }
                else
                {
                    ViewBag.Message = "Sorry  Something Went wrong!!";
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

        #region HomePageDynamicSectionCreate
        public ActionResult HomePageDynamicSectionCreate()
        {

            var SectionStyle = Enum.GetNames(typeof(ModelLayer.Models.Enum.Margin_DivisionConstants.Select_Style));
            var sectionstyl = db.HomePageDynamicSectionStyle.Select(x => x.SectionStyle).ToList();
            //var SectionStyleValue = Enum.GetNames(typeof(ModelLayer.Models.Enum.Margin_DivisionConstants.Select_Style));
            //ViewBag.SectionStyle = new SelectList(SectionStyle, SectionStyleValue);
            var list = SectionStyle.ToList().Select(p => new
            {
                ID = p,
                Name = p
            });
            ViewBag.SectionStyle = new SelectList(list, "ID", "Name");

            //ViewBag.FranchiseID = new SelectList((from f in db.Franchises where f.ID != 1 && f.PincodeID != null select new { f.ID, f.BusinessDetail.Name }).ToList().OrderBy(x => x.Name), "ID", "Name");

            ViewBag.FranchiseID = new SelectList("ID", "Name");

            var citylist = (from f in db.Franchises
                            where f.ID != 1 && f.IsActive == true && f.BusinessDetail.UserLogin.IsLocked == false && f.BusinessDetail.Pincode.City.IsActive == true
                            select new { ID = f.BusinessDetail.Pincode.CityID, Name = f.BusinessDetail.Pincode.City.Name }).Distinct().ToList().OrderBy(x => x.Name);
            ViewBag.CityList = new SelectList(citylist, "ID", "Name");

            ViewBag.SectionHeader = new SelectList((from h in db.HomePageDynamicSectionsMasters where  h.SectionHeader != null select new { h.ID, h.SectionHeader }).ToList().OrderBy(x => x.SectionHeader), "ID", "SectionHeader");
            return View();
        }


        [HttpPost]
        public ActionResult HomePageDynamicSectionCreate(HomePageDynamicSection homepagedynamicsections, Int64 SectionHeader)
        {
            int FranchiseID = (int)db.HomePageDynamicSection.Where(x => x.ID == homepagedynamicsections.ID).Select(x => x.FranchiseId).FirstOrDefault();
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
                homepagedynamicsections.SectionId = SectionHeader;
                homepagedynamicsections.NetworkIP = BusinessLogicLayer.CommonFunctions.GetClientIP();
                homepagedynamicsections.CreateBy = BusinessLogicLayer.CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(Session["ID"]));
                homepagedynamicsections.ModifyBy = BusinessLogicLayer.CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(Session["ID"]));
                homepagedynamicsections.ModifyDate = DateTime.Now;
                homepagedynamicsections.CreateDate = DateTime.Now;

                homepagedynamicsections.ShowInApp = false;
                if (ModelState.IsValid)
                {

                    db.HomePageDynamicSection.Add(homepagedynamicsections);
                    db.SaveChanges();
                    ViewBag.Message = "Done!  Added Successfully!!";
                    TempData["Message"] = ViewBag.Message;
                    return RedirectToAction("Index", new { CityList = CityId_, franchiseList = FranchiseID });

                }
                else
                {
                    ViewBag.Message = "sorry!  Something Is Wrong !!";
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

        #region Set Sequence
        public ActionResult SetSequence(int id)
        {
            try
            {
                var DisplayName_ = db.HomePageDynamicSection.Where(d => d.ID == id).Select(s => s.SectionDisplayName).FirstOrDefault();
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

        #region DeleteHomePageDynamicSectionCreate

        public ActionResult DeleteHomePageDynamicSection(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }


            HomePageDynamicSection homePageDynamicSections = db.HomePageDynamicSection.Find(id);
            var SectionStyle = Enum.GetNames(typeof(ModelLayer.Models.Enum.Margin_DivisionConstants.Select_Style));
            var list = SectionStyle.ToList().Select(p => new
            {
                ID = p,
                Name = p
            });

            ViewBag.SectionStyle = new SelectList(list, "ID", "Name", homePageDynamicSections.SectionStyle);
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
        public ActionResult DeleteHomePageDynamicSection(int? id , HomePageDynamicSection homePageDynamicSections)
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

        #region Deletebanner

        public ActionResult Deletebanner(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BusinessLogicLayer.ReadConfig rcKey = new BusinessLogicLayer.ReadConfig(System.Web.HttpContext.Current.Server);
            HomePageDynamicSectionBanner homePageDynamicSectionsBanner = db.HomePageDynamicSectionBanner.Find(id);

            HomePageDynamicSection objHomePageDynamicSection = db.HomePageDynamicSection.FirstOrDefault(p => p.ID == homePageDynamicSectionsBanner.HomePageDynamicSectionId);
            int HomepageDynamicsctionID = Convert.ToInt32(objHomePageDynamicSection.ID);
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

            long FranchiseID = (Int64)db.HomePageDynamicSection.Where(s => s.ID == homePageDynamicSectionsBanner.HomePageDynamicSectionId).Select(a => a.FranchiseId).FirstOrDefault();
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
            homePageDynamicSectionsBanner.ImageName_ = rcKey.HomePageDynamicSection_IMAGE_HTTP + CityId_ + "/" + FranchiseID + "/" + HomePageSectionId + "/" + homePageDynamicSectionsBanner.ImageName;

            ViewBag.HomepageDynamicSectionID = HomepageDynamicsctionID;
            ViewBag.SDate = homePageDynamicSectionsBanner.StartDate.ToString("dd/MM/yyyy");
            ViewBag.EDate = homePageDynamicSectionsBanner.EndDate.ToString("dd/MM/yyyy");

            homePageDynamicSectionsBanner.IsActive_ = homePageDynamicSectionsBanner.IsActive;
            homePageDynamicSectionsBanner.IsBanner_ = homePageDynamicSectionsBanner.IsBanner;

            if (homePageDynamicSectionsBanner == null)
            {
                return HttpNotFound();
            }
            return View(homePageDynamicSectionsBanner);
        }

        [HttpPost]
        public ActionResult Deletebanner(int? id,int HomepageDynamicSectionID, HttpPostedFileBase file)
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

               //bool IsDeleted = false;
                HomePageDynamicSectionBanner homePageDynamicSectionBanner = db.HomePageDynamicSectionBanner.Find(id);

                //if(homePageDynamicSectionBanner.ImageName != null)
                //{
                //    string filename = homePageDynamicSectionBanner.ImageName;
                //    int HomePageSectionId = (int)db.HomePageDynamicSection.Where(x => x.ID == HomepageDynamicSectionID).Select(x => x.SectionId).FirstOrDefault();

                //    IsDeleted = CommonFunctions.DeleteHomePageDynamicSectionImage(file, CityId_, FranchiseID, HomePageSectionId, filename);

                //}

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

        #region Method


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
            int HomePageSectionId = (int)db.HomePageDynamicSection.Where(x => x.ID == HomepageDynamicSectionID).Select(x => x.SectionId).FirstOrDefault();

            try
            {
                BusinessLogicLayer.ReadConfig rcKey = new BusinessLogicLayer.ReadConfig(System.Web.HttpContext.Current.Server);
                DataTable dt = new DataTable();
                dt = BusinessLogicLayer.HomePageDynamicSectionPortal.Select_GalleryProducts(FranchiseID, HomepageDynamicSectionID, System.Web.HttpContext.Current.Server);

                /*Select All the Shop By Franchise */
                ls.HomeDynamicBannersetsequence = (from n in dt.AsEnumerable()
                                                   select new ModelLayer.Models.ViewModel.HomePageDynamicBannerSetSequenceViewModel
                                                   {
                                                       ID = n.Field<Int64>("ID"),
                                                       FranchiseID = n.Field<Int64>("FranchiseID"),
                                                       EndDate = n.Field<DateTime>("EndDate"),
                                                       StartDate = n.Field<DateTime>("StartDate"),
                                                       Imagename = rcKey.HomePageDynamicSection_IMAGE_HTTP + CityId_ + "/" + FranchiseID + "/"+ HomePageSectionId + "/" + n.Field<string>("ImageName"),
                                                       LinkUrl = n.Field<string>("LinkUrl"),
                                                       Tooltip = n.Field<string>("Tooltip"),
                                                       SequenceOrder = n.Field<int?>("SequenceOrder") == null ? 0 : Convert.ToInt32(n.Field<int?>("SequenceOrder")),
                                                       IsActive = n.Field<bool>("IsActive")
                                                   }).OrderBy(x => x.SequenceOrder).ToList();

                //ViewBag.CategoryID = CategoryID;
                ViewBag.Franchise = FranchiseID;

                return PartialView("_HomePageDynamicSectionBannerSetSequence", ls);

            }
            catch (Exception ex)
            {
                return PartialView("_HomePageDynamicSectionBannerSetSequence", ls);
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


        [HttpPost]
        public PartialViewResult GetProductList(int FranchiseID,int HomepageDynamicSectionID, int CategoryID)
        {
            var ProductList = (from sp in db.ShopProducts
                               join ss in db.ShopStocks on sp.ID equals ss.ShopProductID
                               where sp.Shop.FranchiseID == FranchiseID
                               && sp.Product.CategoryID == CategoryID
                               && ss.Qty > 0
                               && sp.Product.IsActive == true && sp.IsActive == true && sp.Shop.IsLive == true && sp.Shop.IsActive == true && ss.IsActive == true
                               && !
                                    (from bi in db.HomePageDynamicSectionProduct
                                     where bi.HomePageDynamicSectionId == HomepageDynamicSectionID && bi.IsActive == true
                                     select new
                                     {
                                         bi.ShopStockId
                                     }).ToList().Select(p=>p.ShopStockId).Contains(ss.ID)
                               select new DynamicProductListViewModel { ID = sp.ProductID, Name = sp.Product.Name }).Distinct().ToList();


          

            return PartialView("_ProductListingHomepage", ProductList);
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
                            select new DynamicProductListViewModel { ID = sp.ProductID, Name = sp.Product.Name }).Distinct().Take(10).ToList();

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
                            join cod in db .CustomerOrderDetails on ss.ID equals cod.ShopStockID
                            join  p  in db.Products on sp.ProductID equals p.ID
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



            //return PartialView("_ProductListingHomepage");
            return PartialView("_ProductListingHomepage", list);
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

        public JsonResult getFranchise(int CityID)////added
        {
            List<tempData> objODP = new List<tempData>();

            objODP = db.Franchises
                    .Where(x => x.ID != 1 && x.PincodeID != null && x.IsActive == true && x.BusinessDetail.UserLogin.IsLocked == false && x.BusinessDetail.Pincode.City.IsActive == true
                    && x.BusinessDetail.Pincode.City.ID == CityID)
                     //.Select(x => new tempData { text = x.ID.ToString(), value = x.ID } ////ContactPerson->ID
                     //.Select(x => new tempData { text = x.ContactPerson.ToString(), value = x.ID }
                     .Select(x => new tempData { text = x.ContactPerson, value = x.ID }
                    ).OrderBy(x => x.text)
                    .ToList();

            return Json(objODP, JsonRequestBehavior.AllowGet);
        }



        [HttpPost]
        public JsonResult GetBlocksize(string sectionsty)
        {

            var blocksize = (from dbt in db.HomePageDynamicSectionStyle where dbt.SectionStyle == sectionsty select new { dbt.Portal_Banner_Height, dbt.Portal_Banner_Width, dbt.Portal_Banner_Size }).FirstOrDefault();
            return Json(blocksize, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetBlocksizecategory(string sectionsty)
        {

            var blocksize = (from dbt in db.HomePageDynamicSectionStyle where dbt.SectionStyle == sectionsty select new { dbt.Portal_Category_Height, dbt.Portal_Category_Width, dbt. Portal_Category_Size}).FirstOrDefault();
            return Json(blocksize, JsonRequestBehavior.AllowGet);
        }

        #endregion

        public class ProductDetail
        {
            public Int64 ID { get; set; }
            public string Name { get; set; }
        }
        public class tempData////added
        {
            public Int64 value;
            public string text;
        }
      
    }
}