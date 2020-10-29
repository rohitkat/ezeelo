using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ModelLayer.Models;
using System.Data;
using System.Web.Configuration;
using System.Net;
using System.Text;
using Administrator.Models;
using BusinessLogicLayer;

namespace Administrator.Controllers
{
    public class FranchiseMenuController : Controller
    {

        EzeeloDBContext db = new EzeeloDBContext();
        //
        // GET: /FranchiseMenu/
        StringBuilder errStr = new StringBuilder("=====================================================================================" +
         Environment.NewLine
         + "ErrorLog Controller : FranchiseMenuController" + Environment.NewLine);

        //
        // GET: /FranchiseMenu/
        [SessionExpire]
        [CustomAuthorize(Roles = "FranchiseMenu/CanRead")]
        public ActionResult Index()
        {
            try
            {

                List<CategoryDetail> ldata = new List<CategoryDetail>();
                ldata.Add(new CategoryDetail { ID = 0, Name = "Select Category" });
                ViewBag.FranchiseList = new SelectList((from f in db.Franchises where f.ID != 1 select new { f.ID, f.BusinessDetail.Name }).ToList().OrderBy(x => x.Name), "ID", "Name");
               // ViewBag.CategoryList = new SelectList(db.Categories.Where(x => x.Level == 1 && x.IsActive == true).ToList().OrderBy(x => x.Name), "ID", "Name");
                ViewBag.CategoryList = new SelectList(ldata, "ID", "Name");
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

        [HttpPost]
        [SessionExpire]
        [CustomAuthorize(Roles = "FranchiseMenu/CanWrite")]
        public ActionResult Index(Int64 FranchiseList, Int64 CategoryList)
        {
            try
            {
                List<CategoryDetail> ldata = new List<CategoryDetail>();
                ldata.Add(new CategoryDetail { ID = 0, Name = "Select Category" });

                ViewBag.FranchiseList = new SelectList((from f in db.Franchises where f.ID != 1 select new { f.ID, f.BusinessDetail.Name }).ToList().OrderBy(x => x.Name), "ID", "Name", FranchiseList);
               // ViewBag.CategoryList = new SelectList(db.Categories.Where(x => x.Level == 1 && x.IsActive == true).OrderBy(x => x.Name).ToList(), "ID", "Name", CategoryList);
                ViewBag.CategoryList = new SelectList(ldata, "ID", "Name");
                ViewBag.LevelTwoCategoryList = new SelectList(ldata, "ID", "Name");

                return View();
            }
            catch (Exception ex)
            {
                errStr.Append("Method Name[Http Request] :- Index[HttpPost]" + Environment.NewLine +
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

        private static string fConnectionString = WebConfigurationManager.ConnectionStrings["EzeeloDBContext"].ToString();
        public ActionResult LevelOnePriority(ModelLayer.Models.ViewModel.FranchiseMenuList ls, Int64 Franchise)
        {
            try
            {

                ViewBag.FranchiseList = new SelectList((from f in db.Franchises where f.ID != 1 select new { f.ID, f.BusinessDetail.Name }).ToList().OrderBy(x => x.Name), "ID", "Name", Franchise);
                ViewBag.CategoryList = new SelectList(db.Categories.Where(x => x.Level == 1 && x.IsActive == true).OrderBy(x => x.Name).ToList(), "ID", "Name");

                BusinessLogicLayer.UpdateFranchiseCategorySequence obj = new BusinessLogicLayer.UpdateFranchiseCategorySequence();
                Int64 userID = CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(Session["ID"]));

                if (obj.FranchiseMenuListUpdate(ls, Franchise, userID, fConnectionString))
                {
                    TempData["Message"] = "Sequence Order Set Successfully";
                }
                else
                {
                    TempData["Message"] = "Sorry Unable to set Priority ........";
                }

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                errStr.Append("Method Name[Http Request] :- LevelOnePriority[HttpPost]" + Environment.NewLine +
                               "ON Dated" + DateTime.UtcNow.AddHours(5.30).TimeOfDay + Environment.NewLine +
                                   ex.Message.ToString() + Environment.NewLine +
                         "====================================================================================="
                               );
                //ViewBag.Message = "Sorry! Problem in customer registration!!";
                ModelState.AddModelError("Message", "Sorry! Problem in Generate Index view!!");
                ErrorLog.ErrorLogFile("Sorry! Problem in Generate LevelOnePriority view!! " + Environment.NewLine + errStr.ToString()
                    , ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);

                return View();
            }
        }


        //
        // GET: /FranchiseMenu/Details/5
        [SessionExpire]
        [CustomAuthorize(Roles = "FranchiseMenu/CanRead")]
        public ActionResult Details(int id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                FranchiseMenu franchiseMenu = db.FranchiseMenus.Find(id);
                if (franchiseMenu == null)
                {
                    return HttpNotFound();
                }

                BusinessLogicLayer.ReadConfig rcKey = new ReadConfig(System.Web.HttpContext.Current.Server);
                Int64 CityID = db.Franchises.Where(x => x.ID == franchiseMenu.FranchiseID).FirstOrDefault().BusinessDetail.Pincode.CityID;
                Int32 FranchisrID = franchiseMenu.FranchiseID;////added
                franchiseMenu.ImageName = rcKey.CATEGORY_IMAGE_HTTP + "/" + CityID + "/" + FranchisrID + "/" + franchiseMenu.ImageName;////added
               // franchiseMenu.ImageName = rcKey.CATEGORY_IMAGE_HTTP + "/" + CityID + "/" + franchiseMenu.ImageName;////hide
              

                ViewBag.FranchiseName = db.Franchises.Where(x => x.ID == franchiseMenu.FranchiseID).FirstOrDefault().BusinessDetail.Name;
                ViewBag.CategoryName = db.Categories.Where(x => x.ID == franchiseMenu.CategoryID).FirstOrDefault().Name;
                ViewBag.Level = db.Categories.Where(x => x.ID == franchiseMenu.CategoryID).FirstOrDefault().Level;

                return View(franchiseMenu);
            }
            catch (Exception ex)
            {
                errStr.Append("Method Name[Http Request] :- Details[HttpGet]" + Environment.NewLine +
                               "ON Dated" + DateTime.UtcNow.AddHours(5.30).TimeOfDay + Environment.NewLine +
                                   ex.Message.ToString() + Environment.NewLine +
                         "====================================================================================="
                               );
                //ViewBag.Message = "Sorry! Problem in customer registration!!";
                ModelState.AddModelError("Message", "Sorry! Problem in Record Detail!!");
                ErrorLog.ErrorLogFile("Sorry! Problem in Record Detail!!" + Environment.NewLine + errStr.ToString()
                    , ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);

                return View();
            }
        }

        //
        // GET: /FranchiseMenu/Create
        [SessionExpire]
        [CustomAuthorize(Roles = "FranchiseMenu/CanRead")]
        public ActionResult Create()
        {
            try
            {
                List<CategoryDetail> ldata = new List<CategoryDetail>();
                ldata.Add(new CategoryDetail { ID = 0, Name = "Select Category" });
                ViewBag.FranchiseList = new SelectList((from f in db.Franchises where f.ID != 1 select new { f.ID, f.BusinessDetail.Name }).ToList().OrderBy(x => x.Name), "ID", "Name");
                //ViewBag.CategoryList = new SelectList(db.Categories.Where(x => x.Level == 1 && x.IsActive == true).ToList().OrderBy(x => x.Name), "ID", "Name");
                ViewBag.CategoryList = new SelectList(ldata, "ID", "Name");
                ViewBag.LevelTwoCategoryList = new SelectList(ldata, "ID", "Name");
                ViewBag.LevelThreeCategoryList = new SelectList(ldata, "ID", "Name");
                return View();
            }
            catch (Exception ex)
            {
                errStr.Append("Method Name[Http Request] :- Create[HttpGet]" + Environment.NewLine +
                               "ON Dated" + DateTime.UtcNow.AddHours(5.30).TimeOfDay + Environment.NewLine +
                                   ex.Message.ToString() + Environment.NewLine +
                         "====================================================================================="
                               );
                //ViewBag.Message = "Sorry! Problem in customer registration!!";
                ModelState.AddModelError("Message", "Sorry! Problem in Generate Create View!!");
                ErrorLog.ErrorLogFile("Sorry! Problem in Generate Create View " + Environment.NewLine + errStr.ToString()
                    , ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);

                return View();
            }
        }

        //
        // POST: /FranchiseMenu/Create
        [HttpPost]
        [SessionExpire]
        [CustomAuthorize(Roles = "FranchiseMenu/CanWrite")]
        public ActionResult Create(FranchiseMenu franchiseMenu, Int32 CategoryList, Int32? LevelTwoCategoryList, Int32? LevelThreeCategoryList, Int32 FranchiseList, HttpPostedFileBase file)
        {
            try
            {
                // TODO: Add insert logic here
                List<CategoryDetail> ldata = new List<CategoryDetail>();
                ldata.Add(new CategoryDetail { ID = 0, Name = "Select Category" });
                ViewBag.FranchiseList = new SelectList((from f in db.Franchises where f.ID != 1 select new { f.ID, f.BusinessDetail.Name }).ToList().OrderBy(x => x.Name), "ID", "Name");
                //ViewBag.CategoryList = new SelectList(db.Categories.Where(x => x.Level == 1 && x.IsActive == true).OrderBy(x => x.Name).ToList(), "ID", "Name");
                ViewBag.CategoryList = new SelectList(ldata, "ID", "Name");
                ViewBag.LevelTwoCategoryList = new SelectList(ldata, "ID", "Name");
                ViewBag.LevelThreeCategoryList = new SelectList(ldata, "ID", "Name");


                if (LevelThreeCategoryList > 0)
                {
                    franchiseMenu.CategoryID = Convert.ToInt32(LevelThreeCategoryList);
                }
                else if (LevelTwoCategoryList > 0)
                {
                    franchiseMenu.CategoryID = Convert.ToInt32(LevelTwoCategoryList);
                }
                else
                {
                    franchiseMenu.CategoryID = CategoryList;
                }

                franchiseMenu.FranchiseID = FranchiseList;

                Int64 catID = franchiseMenu.CategoryID;
                bool IsUploaded = false;
                if (file != null)
                {
                    Int64 CityID = db.Franchises.Where(x => x.ID == FranchiseList).FirstOrDefault().BusinessDetail.Pincode.CityID;
                    Int32 FranchisrID = franchiseMenu.FranchiseID;////added
                    //upload cat image here
                    IsUploaded = CommonFunctions.UploadCategoryImage(file, CityID,FranchisrID, franchiseMenu.CategoryID + ".png");////added FranchisrID
                }

                if (db.FranchiseMenus.Where(x => x.FranchiseID == FranchiseList && x.CategoryID == catID).Count() < 1)
                {

                    if (franchiseMenu.SequenceOrder <= 0)
                    {
                        franchiseMenu.SequenceOrder = this.priorityLevel(FranchiseList, franchiseMenu.CategoryID) + 1;
                    }

                    string msg;
                    if (this.IsValidated(out msg, franchiseMenu))
                    {
                        List<object> paramValues = new List<object>();
                        paramValues.Add(DBNull.Value);
                        paramValues.Add(franchiseMenu.FranchiseID);
                        paramValues.Add(franchiseMenu.CategoryID);
                        paramValues.Add(franchiseMenu.CategoryName);
                        paramValues.Add(franchiseMenu.SequenceOrder);
                        paramValues.Add(franchiseMenu.IsActive);
                        paramValues.Add(franchiseMenu.CategoryID + ".png");
                        paramValues.Add(DateTime.UtcNow.AddHours(5.5));
                        paramValues.Add(DBNull.Value);
                        paramValues.Add(BusinessLogicLayer.CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(Session["ID"])));
                        paramValues.Add(DBNull.Value);
                        paramValues.Add("Net Browser");
                        paramValues.Add(BusinessLogicLayer.CommonFunctions.GetClientIP());
                        paramValues.Add(franchiseMenu.IsExpire);   //IsExpire
                        paramValues.Add(franchiseMenu.ExpiryDate);   //ExpiryDate
                        paramValues.Add(DataAccessLayer.Enumerators.DB_OPERATIONS.INSERT);
                        paramValues.Add(0);

                        ViewBag.Messaage = BusinessLogicLayer.UpdateFranchiseCategorySequence.Insertupdate_FranchiseMenu(paramValues, DataAccessLayer.Enumerators.DB_OPERATIONS.INSERT, fConnectionString);
                    }
                    else
                    {
                        ViewBag.Messaage = msg;
                    }
                    if (!IsUploaded && file != null)
                    {
                        ViewBag.Messaage = ViewBag.Messaage + " Problem in Uploading Category Image...";
                    }

                    //return View("Create");

                }
                else
                {
                    ViewBag.Messaage = "Franchise Menu already Present....!";
                    //return View("Create");
                }
                TempData["Messaage"] = ViewBag.Messaage;
                return RedirectToAction("Create");
                // return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                errStr.Append("Method Name[Http Request] :- Create[HttpPost]" + Environment.NewLine +
                               "ON Dated" + DateTime.UtcNow.AddHours(5.30).TimeOfDay + Environment.NewLine +
                                   ex.Message.ToString() + Environment.NewLine +
                         "====================================================================================="
                               );
                //ViewBag.Message = "Sorry! Problem in customer registration!!";
                ModelState.AddModelError("Message", "Sorry! Problem in Record Creation!!");
                ErrorLog.ErrorLogFile("Sorry! Problem in Record Creation!!" + Environment.NewLine + errStr.ToString()
                    , ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);

                return View();
            }
        }

        //
        // GET: /FranchiseMenu/Edit/5
        [SessionExpire]
        [CustomAuthorize(Roles = "FranchiseMenu/CanRead")]
        public ActionResult Edit(int id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                FranchiseMenu franchiseMenu = db.FranchiseMenus.Find(id);
                if (franchiseMenu == null)
                {
                    return HttpNotFound();
                }
                this.ViewBagList(id);

                Int64 CityID = db.Franchises.Where(x => x.ID == franchiseMenu.FranchiseID).FirstOrDefault().BusinessDetail.Pincode.CityID;
                Int32 FranchisrID = franchiseMenu.FranchiseID;////added
                
                BusinessLogicLayer.ReadConfig rcKey = new BusinessLogicLayer.ReadConfig(System.Web.HttpContext.Current.Server);
                //franchiseMenu.ImageName = rcKey.CATEGORY_IMAGE_HTTP + "/" + CityID + "/" + franchiseMenu.ImageName;////hide
                franchiseMenu.ImageName = rcKey.CATEGORY_IMAGE_HTTP + "/" + CityID + "/" + FranchisrID + "/" + franchiseMenu.ImageName;////added

                return View(franchiseMenu);
            }
            catch (Exception ex)
            {
                errStr.Append("Method Name[Http Request] :- Edit[HttpGet]" + Environment.NewLine +
                               "ON Dated" + DateTime.UtcNow.AddHours(5.30).TimeOfDay + Environment.NewLine +
                                   ex.Message.ToString() + Environment.NewLine +
                         "====================================================================================="
                               );
                //ViewBag.Message = "Sorry! Problem in customer registration!!";
                ModelState.AddModelError("Message", "Sorry! Problem in Record Update!!");
                ErrorLog.ErrorLogFile("Sorry! Problem in Record Update!!" + Environment.NewLine + errStr.ToString()
                    , ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);

                return View();
            }
        }

        //
        // POST: /FranchiseMenu/Edit/5
        [HttpPost]
        [SessionExpire]
        [CustomAuthorize(Roles = "FranchiseMenu/CanWrite")]
        public ActionResult Edit(FranchiseMenu franchiseMenu, Int32 CategoryList, Int32? LevelTwoCategoryList, Int32? LevelThreeCategoryList, Int32 FranchiseList, string ExpiryDate1, HttpPostedFileBase file)
        {
            try
            {
                this.ViewBagList(franchiseMenu.ID);
                FranchiseMenu lfranchiseMenu = db.FranchiseMenus.Find(franchiseMenu.ID);

                if (LevelThreeCategoryList > 0)
                {
                    franchiseMenu.CategoryID = Convert.ToInt32(LevelThreeCategoryList);
                }
                else if (LevelTwoCategoryList > 0)
                {
                    franchiseMenu.CategoryID = Convert.ToInt32(LevelTwoCategoryList);
                }
                else
                {
                    franchiseMenu.CategoryID = CategoryList;
                }

                //=============== Category Expiration Changes ==============
                DateTime lDate = DateTime.Now;
                if (ExpiryDate1 != "")
                {

                    lDate = CommonFunctions.GetProperDateTime(ExpiryDate1);
                    lfranchiseMenu.ExpiryDate = lDate;
                }
                if (lfranchiseMenu.IsExpire == true && lfranchiseMenu.ExpiryDate == null)
                {
                    ViewBag.Messaage = "Please Provide Expiry date..";
                    return View();
                }
                //=============== Category Expiration Changes ==============
                franchiseMenu.FranchiseID = FranchiseList;

                Int64 catID = franchiseMenu.CategoryID;

                bool IsUploaded = false;
                if (file != null)
                {
                    Int64 CityID = db.Franchises.Where(x => x.ID == FranchiseList).FirstOrDefault().BusinessDetail.Pincode.CityID;
                    Int32 FranchisrID = franchiseMenu.FranchiseID;////added
                    //upload cat image here
                    IsUploaded = CommonFunctions.UploadCategoryImage(file, CityID,FranchisrID, franchiseMenu.CategoryID + ".png");////added
                }

                if (db.FranchiseMenus.Where(x => x.FranchiseID == FranchiseList && x.CategoryID == catID).Count() > 0)
                {

                    if (franchiseMenu.SequenceOrder <= 0)
                    {
                        franchiseMenu.SequenceOrder = this.priorityLevel(franchiseMenu.FranchiseID, franchiseMenu.CategoryID);
                    }

                    List<object> paramValues = new List<object>();
                    paramValues.Add(franchiseMenu.ID);
                    paramValues.Add(franchiseMenu.FranchiseID);
                    paramValues.Add(franchiseMenu.CategoryID);
                    paramValues.Add(franchiseMenu.CategoryName);
                    paramValues.Add(franchiseMenu.SequenceOrder);
                    paramValues.Add(franchiseMenu.IsActive);
                    paramValues.Add(franchiseMenu.CategoryID + ".png");
                    paramValues.Add(lfranchiseMenu.CreateDate);
                    paramValues.Add(DateTime.UtcNow.AddHours(5.5));
                    paramValues.Add(lfranchiseMenu.CreatedBy);
                    paramValues.Add(BusinessLogicLayer.CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(Session["ID"])));
                    paramValues.Add("Net Browser");
                    paramValues.Add(BusinessLogicLayer.CommonFunctions.GetClientIP());
                    paramValues.Add(franchiseMenu.IsExpire);   //IsExpire
                    paramValues.Add(lfranchiseMenu.ExpiryDate);   //ExpiryDate
                    paramValues.Add(DataAccessLayer.Enumerators.DB_OPERATIONS.UPDATE.ToString());
                    paramValues.Add(DBNull.Value);


                    ViewBag.Messaage = BusinessLogicLayer.UpdateFranchiseCategorySequence.Insertupdate_FranchiseMenu(paramValues, DataAccessLayer.Enumerators.DB_OPERATIONS.UPDATE, fConnectionString);
                    if (!IsUploaded && file != null)
                    {
                        ViewBag.Messaage = ViewBag.Messaage + " Problem in Uploading Category Image...";
                    }

                    // this.ViewBagList(franchiseMenu.ID);
                    //return View();
                    TempData["Message"] = ViewBag.Messaage;
                    return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.Messaage = "Franchise Menu Not Present....!";
                    return View();
                }

            }
            catch (Exception ex)
            {
                errStr.Append("Method Name[Http Request] :- Edit[HttpPost]" + Environment.NewLine +
                               "ON Dated" + DateTime.UtcNow.AddHours(5.30).TimeOfDay + Environment.NewLine +
                                   ex.Message.ToString() + Environment.NewLine +
                         "====================================================================================="
                               );
                //ViewBag.Message = "Sorry! Problem in customer registration!!";
                ModelState.AddModelError("Message", "Sorry! Problem in Record Updation!!");
                ErrorLog.ErrorLogFile("Sorry! Problem in Record Updation!!" + Environment.NewLine + errStr.ToString()
                    , ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);

                return View();
            }
        }

        //
        // GET: /FranchiseMenu/Delete/5
        [SessionExpire]
        [CustomAuthorize(Roles = "FranchiseMenu/CanDelete")]
        public ActionResult Delete(int id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                FranchiseMenu franchiseMenu = db.FranchiseMenus.Find(id);
                if (franchiseMenu == null)
                {
                    return HttpNotFound();
                }

                BusinessLogicLayer.ReadConfig rcKey = new ReadConfig(System.Web.HttpContext.Current.Server);
                Int64 CityID = db.Franchises.Where(x => x.ID == franchiseMenu.FranchiseID).FirstOrDefault().BusinessDetail.Pincode.CityID;
                Int32 FranchisrID = franchiseMenu.FranchiseID;////added
                //franchiseMenu.ImageName = rcKey.CATEGORY_IMAGE_HTTP + "/" + CityID + "/" + franchiseMenu.ImageName;////hide
                franchiseMenu.ImageName = rcKey.CATEGORY_IMAGE_HTTP + "/" + CityID + "/" + FranchisrID + "/" + franchiseMenu.ImageName;////added
               

                ViewBag.FranchiseName = db.Franchises.Where(x => x.ID == franchiseMenu.FranchiseID).FirstOrDefault().BusinessDetail.Name;
                ViewBag.CategoryName = db.Categories.Where(x => x.ID == franchiseMenu.CategoryID).FirstOrDefault().Name;
                ViewBag.Level = db.Categories.Where(x => x.ID == franchiseMenu.CategoryID).FirstOrDefault().Level;

                return View(franchiseMenu);
            }
            catch (Exception ex)
            {
                errStr.Append("Method Name[Http Request] :- Delete[HttpGet]" + Environment.NewLine +
                               "ON Dated" + DateTime.UtcNow.AddHours(5.30).TimeOfDay + Environment.NewLine +
                                   ex.Message.ToString() + Environment.NewLine +
                         "====================================================================================="
                               );
                //ViewBag.Message = "Sorry! Problem in customer registration!!";
                ModelState.AddModelError("Message", "Sorry! Problem in Record Deletion!!");
                ErrorLog.ErrorLogFile("Sorry! Problem in Record Deletion!!" + Environment.NewLine + errStr.ToString()
                    , ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);

                return View();
            }
        }

        //
        // POST: /FranchiseMenu/Delete/5
        [SessionExpire]
        [CustomAuthorize(Roles = "FranchiseMenu/CanDelete")]
        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                FranchiseMenu franchiseMenu = db.FranchiseMenus.Find(id);

                List<object> paramValues = new List<object>();
                paramValues.Add(id);
                paramValues.Add(DataAccessLayer.Enumerators.DB_OPERATIONS.DELETE.ToString());
                paramValues.Add(DBNull.Value);

                int resultCode;
                ViewBag.Messaage = BusinessLogicLayer.UpdateFranchiseCategorySequence.Delete_FranchiseMenu(paramValues, DataAccessLayer.Enumerators.DB_OPERATIONS.DELETE, out resultCode, fConnectionString);

                if (resultCode == 3)
                {
                    TempData["Message"] = ViewBag.Messaage;
                    return RedirectToAction("Index");
                }
                else
                    return View(franchiseMenu);
                //return View();
            }
            catch (Exception ex)
            {
                errStr.Append("Method Name[Http Request] :- Delete[HttpPost]" + Environment.NewLine +
                               "ON Dated" + DateTime.UtcNow.AddHours(5.30).TimeOfDay + Environment.NewLine +
                                   ex.Message.ToString() + Environment.NewLine +
                         "====================================================================================="
                               );
                //ViewBag.Message = "Sorry! Problem in customer registration!!";
                ModelState.AddModelError("Message", "Sorry! Problem in Record Deletion!!");
                ErrorLog.ErrorLogFile("Sorry! Problem in Record Deletion!!" + Environment.NewLine + errStr.ToString()
                    , ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
                ViewBag.Messaage = "Unable to Delete Bank Detail :- " + ex.InnerException.ToString();
                return View();
            }

        }

        [HttpPost]
        public JsonResult SelectLevelOneCategoryByFranchise(int id)
        {
            List<CategoryDetail> catList = new List<CategoryDetail>();  
            var newcatList = db.Categories.Where(x => x.Level == 1 && x.IsActive == true)
                                          .Select(x => new CategoryDetail 
                                           {
                                              ID = x.ID, 
                                              Name = x.Name 
                                           }).OrderBy(c => c.ID).ToList();

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
                       }).OrderBy(c=>c.ID).ToList();
            //catList = catList.Union(newcatList).ToList();
            var lst = catList.Select(x => new  { x.ID, x.Name })
                     .Union(newcatList.Select(x => new { x.ID, x.Name })).ToList();

            //lstb.AddRange(catList.Select(a=>new SelectListItem{Text= a.Name,Value=a.ID.ToString()}));
            ////lstb = lstb.Distinct().ToList();
            //var list = lstb.Select(x => new { x.Value, x.Text }).Distinct().ToList();
            //var catgoryList = list.Select(x => new CategoryDetail { ID = Convert.ToInt32(x.Value), Name = x.Text }).ToList();

            return Json(lst, JsonRequestBehavior.AllowGet);
        }

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

        public ActionResult GetFranchiseCategoryList(Int64 FranchiseID, Int64? CategoryID)
        {
            ModelLayer.Models.ViewModel.FranchiseMenuList ls = new ModelLayer.Models.ViewModel.FranchiseMenuList();

            try
            {
                BusinessLogicLayer.ReadConfig rcKey = new BusinessLogicLayer.ReadConfig(System.Web.HttpContext.Current.Server);
                BusinessLogicLayer.FranchiseMenuList obj = new BusinessLogicLayer.FranchiseMenuList();
                DataTable dt = new DataTable();
                dt = obj.Call_Select_Procedure(FranchiseID, CategoryID, System.Web.HttpContext.Current.Server);

                /*Select All the Shop By Franchise */
                ls.franchiseMenuList = (from n in dt.AsEnumerable()
                                        select new ModelLayer.Models.ViewModel.FranchiseMenuViewModel
                                        {
                                            ID = n.Field<Int64>("ID"),
                                            FranchiseID = n.Field<int>("FranchiseID"),
                                            CategoryID = n.Field<int>("CategoryID"),
                                            CategoryName = n.Field<string>("CategoryName"),
                                            FranchiseCategoryName = n.Field<string>("FranchiseCategoryName") == null ? string.Empty : n.Field<string>("FranchiseCategoryName"),
                                            SequenceOrder = n.Field<int?>("SequenceOrder") == null ? 0 : Convert.ToInt32(n.Field<int?>("SequenceOrder")),
                                            ImageName = rcKey.CATEGORY_IMAGE_HTTP + "/" + n.Field<string>("ImageName"),
                                            IsActive = n.Field<bool>("IsActive"),
                                            IsExpire = n.Field<bool>("IsExpire"),
                                            ExpiryDate = n.Field<DateTime?>("ExpiryDate"),
                                        }).OrderBy(x => x.SequenceOrder).ToList();

                //ViewBag.CategoryID = CategoryID;
                ViewBag.Franchise = FranchiseID;
                return PartialView("_EditFranchiseMenu", ls);

            }
            catch
            {
                return PartialView("_EditFranchiseMenu", ls);
            }


        }

        public int priorityLevel(Int32 FranchiseID, Int32 CategoryID)
        {
            BusinessLogicLayer.UpdateFranchiseCategorySequence obj = new BusinessLogicLayer.UpdateFranchiseCategorySequence();
            int level = db.Categories.Where(x => x.ID == CategoryID).Select(x => x.Level).FirstOrDefault();
            int? parentCatId = db.Categories.Where(x => x.ID == CategoryID).FirstOrDefault().ParentCategoryID;
            int PriorityNo = obj.Get_MAX_FMSequenceOrder(FranchiseID, parentCatId, level, fConnectionString);
            //return (PriorityNo == 0 ? 1 : Convert.ToInt32(++PriorityNo));
            return (PriorityNo);
        }

        private void ViewBagList(Int64 id)
        {
            FranchiseMenu franchiseMenu = db.FranchiseMenus.Find(id);

            Int32 lfranchiseID = franchiseMenu.FranchiseID;
            Int32 lCategoryID = franchiseMenu.CategoryID;
            int lCategoyLevel = franchiseMenu.Level;


            List<CategoryDetail> ldata = new List<CategoryDetail>();
            ldata.Add(new CategoryDetail { ID = 0, Name = "Select Category" });
            ViewBag.FranchiseList = new SelectList((from f in db.Franchises where f.ID != 1 select new { f.ID, f.BusinessDetail.Name }).ToList().OrderBy(x => x.Name), "ID", "Name", lfranchiseID);

            if (lCategoyLevel == 1)
            {
                ViewBag.CategoryList = new SelectList(db.Categories.Where(x => x.Level == 1 && x.IsActive == true).ToList().OrderBy(x => x.Name), "ID", "Name", franchiseMenu.CategoryID);
                ViewBag.LevelTwoCategoryList = new SelectList(ldata, "ID", "Name");
                ViewBag.LevelThreeCategoryList = new SelectList(ldata, "ID", "Name");
            }
            else if (lCategoyLevel == 2)
            {
                Int64 lparentOneID = Convert.ToInt64(db.Categories.Where(x => x.ID == lCategoryID).FirstOrDefault().ParentCategoryID);
                ViewBag.CategoryList = new SelectList(db.Categories.Where(x => x.Level == 1 && x.IsActive == true).ToList().OrderBy(x => x.Name), "ID", "Name", lparentOneID);
                ViewBag.LevelTwoCategoryList = new SelectList(db.Categories.Where(x => x.Level == 2 && x.IsActive == true).OrderBy(x => x.Name).ToList(), "ID", "Name", lCategoryID);
                ViewBag.LevelThreeCategoryList = new SelectList(ldata, "ID", "Name");
            }
            else if (lCategoyLevel == 3)
            {
                Int64 lparentTwoID = Convert.ToInt64(db.Categories.Where(x => x.ID == lCategoryID).FirstOrDefault().ParentCategoryID);
                Int64 lparentOneID = Convert.ToInt64(db.Categories.Where(x => x.ID == lparentTwoID).FirstOrDefault().ParentCategoryID);
                ViewBag.CategoryList = new SelectList(db.Categories.Where(x => x.Level == 1 && x.IsActive == true).OrderBy(x => x.Name).ToList(), "ID", "Name", lparentOneID);
                ViewBag.LevelTwoCategoryList = new SelectList(db.Categories.Where(x => x.Level == 2 && x.IsActive == true).OrderBy(x => x.Name).ToList(), "ID", "Name", lparentTwoID);
                ViewBag.LevelThreeCategoryList = new SelectList(db.Categories.Where(x => x.Level == 3 && x.IsActive == true).OrderBy(x => x.Name).ToList(), "ID", "Name", lCategoryID);
            }

        }

        private bool IsValidated(out string msg, FranchiseMenu objSp)
        {
            msg = string.Empty;
            try
            {
                System.Text.StringBuilder str = new System.Text.StringBuilder("Following Errors Are Found" + Environment.NewLine);
                int Count = 0;
                if (objSp.FranchiseID < 0)
                {
                    ++Count;
                    str.Append(Count.ToString() + ") Invalid Franchise" + Environment.NewLine);
                }
                if (objSp.CategoryID < 0)
                {
                    ++Count;
                    str.Append(Count.ToString() + ")  Invalid Category Selection" + Environment.NewLine);
                }
                if (objSp.SequenceOrder < 0)
                {
                    ++Count;
                    str.Append(Count.ToString() + ") Invalid Priority" + Environment.NewLine);
                }
                if (objSp.IsActive == null)
                {
                    ++Count;
                    str.Append(Count.ToString() + ") Invalid IsActive Value" + Environment.NewLine);
                }

                if (Count > 0)
                {
                    msg = str.ToString();
                    return false;
                }
                else
                {

                    return true;
                }
            }
            catch
            {

                return false;
            }
        }


    }

}