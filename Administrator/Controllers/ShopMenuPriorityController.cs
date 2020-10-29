using Administrator.Models;
using BusinessLogicLayer;
using ModelLayer.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;

namespace Administrator.Controllers
{
    public class ShopMenuPriorityController : Controller
    {
        //
        // GET: /ShopMenuPriority/
        StringBuilder errStr = new StringBuilder("=====================================================================================" +
          Environment.NewLine
          + "ErrorLog Controller : ShopMenuPriorityController" + Environment.NewLine);
        EzeeloDBContext db = new EzeeloDBContext();

        //
        // GET: /FranchiseMenu/
        [SessionExpire]
        [CustomAuthorize(Roles = "ShopMenuPriority/CanRead")]
        public ActionResult Index()
        {
            try
            {

                List<CategoryDetail> ldata = new List<CategoryDetail>();
                ldata.Add(new CategoryDetail { ID = 0, Name = "Select Category" });
                ViewBag.FranchiseList = new SelectList((from f in db.Franchises where f.ID != 1 select new { f.ID, f.BusinessDetail.Name }).ToList().OrderBy(x => x.Name), "ID", "Name");
                ViewBag.CategoryList = new SelectList(db.Categories.Where(x => x.Level == 1 && x.IsActive == true).ToList().OrderBy(x => x.Name), "ID", "Name");
                ViewBag.LevelTwoCategoryList = new SelectList(ldata, "ID", "Name");
                ViewBag.ShopID = new SelectList(ldata, "ID", "Name");

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
        [CustomAuthorize(Roles = "ShopMenuPriority/CanWrite")]
        public ActionResult Index(Int64 FranchiseList, Int64 shopID)
        {
            try
            {
                List<CategoryDetail> ldata = new List<CategoryDetail>();
                ldata.Add(new CategoryDetail { ID = 0, Name = "Select Category" });

                ViewBag.FranchiseList = new SelectList((from f in db.Franchises where f.ID != 1 select new { f.ID, f.BusinessDetail.Name }).ToList().OrderBy(x => x.Name), "ID", "Name", FranchiseList);
                ViewBag.ShopID = new SelectList(ldata, "ID", "Name");

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
        public ActionResult LevelOnePriority(ModelLayer.Models.ViewModel.ShopMenuPriorityList ls, Int64 Shop)
        {
            try
            {

                ViewBag.FranchiseList = new SelectList((from f in db.Franchises where f.ID != 1 select new { f.ID, f.BusinessDetail.Name }).ToList().OrderBy(x => x.Name), "ID", "Name");

                List<CategoryDetail> ldata = new List<CategoryDetail>();
                ldata.Add(new CategoryDetail { ID = 0, Name = "Select Category" });
                ViewBag.ShopID = new SelectList(ldata, "ID", "Name");

                BusinessLogicLayer.ShopMenuPriority_Operation obj = new BusinessLogicLayer.ShopMenuPriority_Operation();
                Int64 userID = CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(Session["ID"]));

                if (obj.ShopMenuListUpdate(ls, Shop, userID, fConnectionString))
                {
                    TempData["Message"] = "Priority Set Successfully";
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
        [CustomAuthorize(Roles = "ShopMenuPriority/CanRead")]
        public ActionResult Details(int id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                ShopMenuPriority shopMenuPriority = db.ShopMenuPriorities.Find(id);
                if (shopMenuPriority == null)
                {
                    return HttpNotFound();
                }

                long shopid = shopMenuPriority.ShopID;
                int franchiseID = db.Shops.Where(x => x.ID == shopid).FirstOrDefault().FranchiseID == null ? 0 : Convert.ToInt32(db.Shops.Where(x => x.ID == shopid).FirstOrDefault().FranchiseID);
                int categoryID = shopMenuPriority.CategoryID;
                List<CategoryDetail> ldata = new List<CategoryDetail>();
                ldata.Add(new CategoryDetail { ID = 0, Name = "Select Category" });
                ViewBag.FranchiseID = db.Franchises.Where(x => x.ID == franchiseID).FirstOrDefault().ContactPerson;
                ViewBag.ShopID = db.Shops.Where(x => x.ID == shopid).FirstOrDefault().Name;
                ViewBag.CategoryID = db.Categories.Where(x => x.ID == categoryID).FirstOrDefault().Name;

                return View(shopMenuPriority);
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
        [CustomAuthorize(Roles = "ShopMenuPriority/CanWrite")]
        public ActionResult Create()
        {
            try
            {
                List<CategoryDetail> ldata = new List<CategoryDetail>();
                ldata.Add(new CategoryDetail { ID = 0, Name = "Select Category" });
                ViewBag.FranchiseID = new SelectList((from f in db.Franchises where f.ID != 1 select new { f.ID, f.BusinessDetail.Name }).ToList().OrderBy(x => x.Name), "ID", "Name");
                ViewBag.CategoryID = new SelectList(ldata, "ID", "Name");
                ViewBag.ShopID = new SelectList(ldata, "ID", "Name");
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
         [CustomAuthorize(Roles = "ShopMenuPriority/CanWrite")]

        public ActionResult Create(ModelLayer.Models.ShopMenuPriority ShopMenu, int FranchiseID)
        {
            try
            {
                // TODO: Add insert logic here
                List<CategoryDetail> ldata = new List<CategoryDetail>();
                ldata.Add(new CategoryDetail { ID = 0, Name = "Select Category" });
                ViewBag.FranchiseID = new SelectList((from f in db.Franchises where f.ID != 1 select new { f.ID, f.BusinessDetail.Name }).ToList().OrderBy(x => x.Name), "ID", "Name", FranchiseID);
                Int64 shopID = ShopMenu.ShopID;
                ViewBag.ShopID = new SelectList((from n in db.Shops
                                                 where n.FranchiseID == FranchiseID
                                                 select new ShopList
                                                 {
                                                     Name = n.Name,
                                                     ID = n.ID
                                                 }).ToList(), "ID", "Name", shopID);

                ViewBag.CategoryID = new SelectList((from op in db.OwnerPlans
                                                     join p in db.Plans on op.PlanID equals p.ID
                                                     join pcc in db.PlanCategoryCharges on op.PlanID equals pcc.PlanID
                                                     join c in db.Categories on pcc.CategoryID equals c.ID
                                                     where pcc.IsActive == true && op.IsActive == true && op.OwnerID == shopID
                                                     && op.Plan.PlanCode.StartsWith("GBMR") && c.IsActive == true
                                                     select new ShopList { Name = c.Name, ID = c.ID }).ToList(), "ID", "Name", FranchiseID);


                Int64 catID = ShopMenu.CategoryID;
                
                if (db.ShopMenuPriorities.Where(x => x.CategoryID == catID && x.ShopID == shopID ).Count() < 1)
                {
                    BusinessLogicLayer.ShopMenuPriority_Operation obj = new ShopMenuPriority_Operation();

                    if (ShopMenu.SequenceOrder == 0)
                    {
                        ShopMenu.SequenceOrder = obj.Get_MAX_ShopMenuPrioritySequenceOrder(ShopMenu.ShopID, 3, fConnectionString);
                        ShopMenu.SequenceOrder += 1;
                    }

                    string msg;
                    if (this.IsValidated(out msg, ShopMenu))
                    {
                        List<object> paramValues = new List<object>();
                        paramValues.Add(DBNull.Value);
                        paramValues.Add(ShopMenu.ShopID);
                        paramValues.Add(ShopMenu.CategoryID);
                        paramValues.Add(ShopMenu.CategoryName);
                        paramValues.Add(ShopMenu.SequenceOrder);
                        paramValues.Add(ShopMenu.IsActive);
                        paramValues.Add(ShopMenu.CategoryID + ".png");
                        paramValues.Add(DateTime.UtcNow.AddHours(5.5));
                        paramValues.Add(DBNull.Value);
                        paramValues.Add(BusinessLogicLayer.CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(Session["ID"])));
                        paramValues.Add(DBNull.Value);
                        paramValues.Add("Net Browser");
                        paramValues.Add(BusinessLogicLayer.CommonFunctions.GetClientIP());
                        paramValues.Add(DataAccessLayer.Enumerators.DB_OPERATIONS.INSERT);
                        paramValues.Add(0);

                        ViewBag.Messaage = obj.Insertupdate_ShopMenuPriority(paramValues, DataAccessLayer.Enumerators.DB_OPERATIONS.INSERT, fConnectionString);
                    }
                    else
                    {
                        ViewBag.Messaage = msg;
                    }

                    return View("Create");


                }
                else
                {
                    ViewBag.Messaage = "Franchise Menu already Present....!";
                    return View("Create");
                }
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




        // GET: /FranchiseMenu/Edit/5
         [SessionExpire]
         [CustomAuthorize(Roles = "ShopMenuPriority/CanWrite")]
        public ActionResult Edit(int id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                ShopMenuPriority shopMenuPriority = db.ShopMenuPriorities.Find(id);
                if (shopMenuPriority == null)
                {
                    return HttpNotFound();
                }


                List<CategoryDetail> ldata = new List<CategoryDetail>();
                ldata.Add(new CategoryDetail { ID = 0, Name = "Select Category" });
                ViewBag.FranchiseID = new SelectList((from f in db.Franchises where f.ID != 1 select new { f.ID, f.BusinessDetail.Name }).ToList().OrderBy(x => x.Name),
                                                    "ID", "Name", shopMenuPriority.Shop.FranchiseID);

                ViewBag.ShopID = new SelectList((from n in db.Shops
                                                 where n.FranchiseID == shopMenuPriority.Shop.FranchiseID
                                                 select new ShopList
                                                 {
                                                     Name = n.Name,
                                                     ID = n.ID
                                                 }).ToList(), "ID", "Name", shopMenuPriority.ShopID);

                ViewBag.CategoryID = new SelectList((from op in db.OwnerPlans
                                                     join p in db.Plans on op.PlanID equals p.ID
                                                     join pcc in db.PlanCategoryCharges on op.PlanID equals pcc.PlanID
                                                     join c in db.Categories on pcc.CategoryID equals c.ID
                                                     where pcc.IsActive == true && op.IsActive == true && op.OwnerID == shopMenuPriority.ShopID
                                                     && op.Plan.PlanCode.StartsWith("GBMR") && c.IsActive == true
                                                     select new ShopList { Name = c.Name, ID = c.ID }).ToList(), "ID", "Name", shopMenuPriority.CategoryID);

                return View(shopMenuPriority);
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
         [CustomAuthorize(Roles = "ShopMenuPriority/CanWrite")]
        public ActionResult Edit(ModelLayer.Models.ShopMenuPriority ShopMenu, Int32 FranchiseID, HttpPostedFileBase file)
        {
            try
            {

                ShopMenuPriority lShopMenu = db.ShopMenuPriorities.Find(ShopMenu.ID);

                Int64 catID = ShopMenu.CategoryID;


                ViewBag.FranchiseID = new SelectList((from f in db.Franchises where f.ID != 1 select new { f.ID, f.BusinessDetail.Name }).ToList().OrderBy(x => x.Name),
                                                        "ID", "Name", FranchiseID);
                Int64 shopID = ShopMenu.ShopID;
                ViewBag.ShopID = new SelectList((from n in db.Shops
                                                 where n.FranchiseID == FranchiseID
                                                 select new ShopList
                                                 {
                                                     Name = n.Name,
                                                     ID = n.ID
                                                 }).ToList(), "ID", "Name", shopID);

                ViewBag.CategoryID = new SelectList((from op in db.OwnerPlans
                                                     join p in db.Plans on op.PlanID equals p.ID
                                                     join pcc in db.PlanCategoryCharges on op.PlanID equals pcc.PlanID
                                                     join c in db.Categories on pcc.CategoryID equals c.ID
                                                     where pcc.IsActive == true && op.IsActive == true && op.OwnerID == shopID
                                                     && op.Plan.PlanCode.StartsWith("GBMR") && c.IsActive == true
                                                     select new ShopList { Name = c.Name, ID = c.ID }).ToList(), "ID", "Name", FranchiseID);


                if (db.ShopMenuPriorities.Where(x => x.ShopID == ShopMenu.ShopID && x.CategoryID == catID).Count() > 0)
                {

                    BusinessLogicLayer.ShopMenuPriority_Operation obj = new ShopMenuPriority_Operation();
                    int maxPriority = obj.Get_MAX_ShopMenuPrioritySequenceOrder(ShopMenu.ShopID, 3, fConnectionString);
                    if (ShopMenu.SequenceOrder == 0)
                    {
                        ShopMenu.SequenceOrder = maxPriority;
                    }
                    else if (ShopMenu.SequenceOrder > maxPriority)
                    {
                        ShopMenu.SequenceOrder = maxPriority;
                    }

                    string msg;
                    if (this.IsValidated(out msg, ShopMenu))
                    {
                        List<object> paramValues = new List<object>();
                        paramValues.Add(ShopMenu.ID);
                        paramValues.Add(ShopMenu.ShopID);
                        paramValues.Add(ShopMenu.CategoryID);
                        paramValues.Add(ShopMenu.CategoryName);
                        paramValues.Add(ShopMenu.SequenceOrder);
                        paramValues.Add(ShopMenu.IsActive);
                        paramValues.Add(ShopMenu.CategoryID + ".png");
                        paramValues.Add(lShopMenu.CreateDate);
                        paramValues.Add(DateTime.UtcNow.AddHours(5.5));
                        paramValues.Add(lShopMenu.CreatedBy);
                        paramValues.Add(BusinessLogicLayer.CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(Session["ID"])));
                        paramValues.Add("Net Browser");
                        paramValues.Add(BusinessLogicLayer.CommonFunctions.GetClientIP());
                        paramValues.Add(DataAccessLayer.Enumerators.DB_OPERATIONS.UPDATE);
                        paramValues.Add(0);

                        ViewBag.Messaage = obj.Insertupdate_ShopMenuPriority(paramValues, DataAccessLayer.Enumerators.DB_OPERATIONS.UPDATE, fConnectionString);
                    }

                }
                else
                {
                    ViewBag.Messaage = "Franchise Menu Not Present....!";
                    return View();
                }
                return View();
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
        [CustomAuthorize(Roles = "ShopMenuPriority/CanDelete")]
        public ActionResult Delete(int id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                ShopMenuPriority shopMenuPriority = db.ShopMenuPriorities.Find(id);
                if (shopMenuPriority == null)
                {
                    return HttpNotFound();
                }

                long shopid =  shopMenuPriority.ShopID;
                int franchiseID = db.Shops.Where(x => x.ID == shopid).FirstOrDefault().FranchiseID == null ? 0 : Convert.ToInt32(db.Shops.Where(x => x.ID == shopid).FirstOrDefault().FranchiseID);
                int categoryID = shopMenuPriority.CategoryID;
                List<CategoryDetail> ldata = new List<CategoryDetail>();
                ldata.Add(new CategoryDetail { ID = 0, Name = "Select Category" });
                ViewBag.FranchiseID = db.Franchises.Where(x => x.ID == franchiseID).FirstOrDefault().ContactPerson;
                ViewBag.ShopID = db.Shops.Where(x => x.ID == shopid).FirstOrDefault().Name;
                ViewBag.CategoryID = db.Categories.Where(x => x.ID == categoryID).FirstOrDefault().Name;

                return View(shopMenuPriority);
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
        [CustomAuthorize(Roles = "ShopMenuPriority/CanDelete")]
        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                ShopMenuPriority shopMenuPriority = db.ShopMenuPriorities.Find(id);
                if (shopMenuPriority == null)
                {
                    return HttpNotFound();
                }

                long shopid = shopMenuPriority.ShopID;
                int franchiseID = db.Shops.Where(x => x.ID == shopid).FirstOrDefault().FranchiseID == null ? 0 : Convert.ToInt32(db.Shops.Where(x => x.ID == shopid).FirstOrDefault().FranchiseID);
                int categoryID = shopMenuPriority.CategoryID;
                List<CategoryDetail> ldata = new List<CategoryDetail>();
                ldata.Add(new CategoryDetail { ID = 0, Name = "Select Category" });
                ViewBag.FranchiseID = db.Franchises.Where(x => x.ID == franchiseID).FirstOrDefault().ContactPerson;
                ViewBag.ShopID = db.Shops.Where(x => x.ID == shopid).FirstOrDefault().Name;
                ViewBag.CategoryID = db.Categories.Where(x => x.ID == categoryID).FirstOrDefault().Name;

                BusinessLogicLayer.ShopMenuPriority_Operation obj = new ShopMenuPriority_Operation();
                List<object> paramValues = new List<object>();
                paramValues.Add(id);
                paramValues.Add(DataAccessLayer.Enumerators.DB_OPERATIONS.DELETE.ToString());
                paramValues.Add(DBNull.Value);

                int resultCode;
                ViewBag.Messaage = obj.Delete_ShopMenuPriority(paramValues, DataAccessLayer.Enumerators.DB_OPERATIONS.UPDATE, out resultCode, fConnectionString);

                if (resultCode == 3)
                    return RedirectToAction("Index");
                else
                    return View(shopMenuPriority);
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

        // [HttpPost]
        //public JsonResult SelectCategoryByShop(int id)
        //{
        //    List<CategoryDetail> catList = new List<CategoryDetail>();
        //    catList = (from c in db.Categories
        //               join pbc in db.PlanBindCategories on c.ID equals pbc.CategoryID
        //               join pb in db.PlanBinds on pbc.PlanBindID equals pb.ID
        //               join p in db.Plans on pb.PlanID equals p.ID
        //               join op in db.OwnerPlans on p.ID equals op.PlanID

        //               where op.OwnerID == id && p.PlanCode.Substring(0, 4) == "GBMR"
        //               && op.IsActive == true && pbc.IsActive == true && c.IsActive == true
        //               select new CategoryDetail
        //               {
        //                   ID = c.ID,
        //                   Name = c.Name
        //               }).ToList();

        //    return Json(catList, JsonRequestBehavior.AllowGet);


        //}

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

        public ActionResult GetFranchiseCategoryList(Int64 ShopID, Int64 CategoryID)
        {
            ModelLayer.Models.ViewModel.ShopMenuPriorityList ls = new ModelLayer.Models.ViewModel.ShopMenuPriorityList();

            try
            {
                ViewBag.Shop = ShopID;
                BusinessLogicLayer.ReadConfig rcKey = new BusinessLogicLayer.ReadConfig(System.Web.HttpContext.Current.Server);
                BusinessLogicLayer.ShopMenuPriority_Operation obj = new BusinessLogicLayer.ShopMenuPriority_Operation();
                DataTable dt = new DataTable();
                dt = obj.Call_Select_Procedure(ShopID, CategoryID, System.Web.HttpContext.Current.Server);

                /*Select All the Shop By Franchise */
                ls.shopMenuPriorityList = (from n in dt.AsEnumerable()
                                           select new ModelLayer.Models.ViewModel.ShopMenuPriorityViewModel
                                           {
                                               ID = n.Field<Int64>("ID"),
                                               ShopID = n.Field<Int64>("ShopID"),
                                               CategoryID = n.Field<int>("CategoryID"),
                                               CategoryName = n.Field<string>("CategoryName"),
                                               ShopCategoryName = n.Field<string>("ShopCategoryName") == null ? string.Empty : n.Field<string>("ShopCategoryName"),
                                               SequenceOrder = n.Field<int?>("SequenceOrder") == null ? 0 : Convert.ToInt32(n.Field<int?>("SequenceOrder")),
                                               ImageName = n.Field<string>("ImageName") == null ? string.Empty : rcKey.CATEGORY_IMAGE_HTTP + "/" + n.Field<string>("ImageName"),
                                               IsActive = n.Field<bool>("IsActive")
                                           }).OrderBy(x => x.SequenceOrder).ToList();

                //ViewBag.CategoryID = CategoryID;
                //ViewBag.Franchise = FranchiseID;
                return PartialView("_EditShopMenuPriority", ls);

            }
            catch
            {
                return PartialView("_EditShopMenuPriority", ls);
            }
        }


        public JsonResult ShopListByFranchise(Int32 franchiseID)
        {
            List<ShopList> ls = new List<ShopList>();
            BusinessLogicLayer.ReadConfig rcKey = new BusinessLogicLayer.ReadConfig(System.Web.HttpContext.Current.Server);
            BusinessLogicLayer.ShopMenuPriority_Operation obj = new BusinessLogicLayer.ShopMenuPriority_Operation();
            DataTable dt = new DataTable();
            dt = obj.SelectShopFrom_ShopMenuPriority(franchiseID, 0, System.Web.HttpContext.Current.Server);
            ls = (from n in dt.AsEnumerable()
                  select new ShopList
                  {
                      Name = n.Field<string>("Name"),
                      ID = n.Field<Int64>("ID"),
                  }).ToList();
            return Json(ls, JsonRequestBehavior.AllowGet);
        }
        
        private bool IsValidated(out string msg, ShopMenuPriority objSp)
        {
            msg = string.Empty;
            try
            {
                System.Text.StringBuilder str = new System.Text.StringBuilder("Following Errors Are Found" + Environment.NewLine);
                int Count = 0;
                if (objSp.ShopID < 0)
                {
                    ++Count;
                    str.Append(Count.ToString() + ") Invalid Shop Selection" + Environment.NewLine);
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

        public JsonResult FranchiseShop(int franchiseID)
        {
            List<ShopList> ls = new List<ShopList>();

            ls = (from n in db.Shops
                  where n.FranchiseID == franchiseID
                  select new ShopList
                  {
                      Name = n.Name,
                      ID = n.ID
                  }).ToList();
            return Json(ls.ToList().OrderBy(x => x.Name), JsonRequestBehavior.AllowGet);

        }


        public JsonResult ShopCategoryList(Int64 ShopID)
        {
            List<ShopList> ls = new List<ShopList>();

            ls = (from op in db.OwnerPlans
                  join p in db.Plans on op.PlanID equals p.ID
                  join pcc in db.PlanCategoryCharges on op.PlanID equals pcc.PlanID
                  join c in db.Categories on pcc.CategoryID equals c.ID
                  where pcc.IsActive == true && op.IsActive == true && op.OwnerID == ShopID
                  && op.Plan.PlanCode.StartsWith("GBMR") && c.IsActive == true
                  select new ShopList { Name = c.Name, ID = c.ID }).ToList();

            return Json(ls.ToList().OrderBy(x => x.Name), JsonRequestBehavior.AllowGet);
        }

    }

    public class ShopList
    {

        public  Int64 ID {get; set;}
        public string Name { get; set; }
    
    }




}