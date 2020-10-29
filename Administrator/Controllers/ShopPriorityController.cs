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
    public class ShopPriorityController : Controller
    {
        EzeeloDBContext db = new EzeeloDBContext();
        //
        // GET: /ShopPriority/
        StringBuilder errStr = new StringBuilder("=====================================================================================" +
         Environment.NewLine
         + "ErrorLog Controller : ShopPriorityController" + Environment.NewLine);
        // GET: /ShopPriority/
        [SessionExpire]
        [CustomAuthorize(Roles = "ShopPriority/CanRead")]        
        public ActionResult Index()
        {
            try
            { 

            List<CategoryDetail> ldata = new List<CategoryDetail>();
            ldata.Add(new CategoryDetail { ID = 0, Name = "Select Category" });
            ViewBag.FranchiseList = new SelectList(db.Franchises.ToList(), "ID", "ContactPerson");
            ViewBag.CategoryList = new SelectList(db.Categories.Where(x => x.Level == 1).ToList(), "ID", "Name");
            ViewBag.LevelTwoCategoryList = new SelectList(ldata, "ID", "Name");
            ViewBag.LevelThreeCategoryList = new SelectList(ldata, "ID", "Name");

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
        [CustomAuthorize(Roles = "ShopPriority/CanWrite")]
        public ActionResult Index(Int64 FranchiseList, Int64 CategoryList)
        {
            try
            {
                List<CategoryDetail> ldata = new List<CategoryDetail>();
                ldata.Add(new CategoryDetail { ID = 0, Name = "Select Category" });

                ViewBag.FranchiseList = new SelectList(db.Franchises.ToList(), "ID", "ContactPerson", FranchiseList);
                ViewBag.CategoryList = new SelectList(db.Categories.Where(x => x.Level == 1).OrderBy(x => x.Name).ToList(), "ID", "Name", CategoryList);

                ViewBag.LevelTwoCategoryList = new SelectList(ldata, "ID", "Name");
                ViewBag.LevelThreeCategoryList = new SelectList(ldata, "ID", "Name");

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
        public ActionResult LevelOnePriority(ModelLayer.Models.ViewModel.ShopPriorityList ls,Int64 CategoryID)
        {
            try
            {

                ViewBag.FranchiseList = new SelectList(db.Franchises.ToList(), "ID", "ContactPerson");
                ViewBag.CategoryList = new SelectList(db.Categories.Where(x => x.Level == 1).OrderBy(x => x.Name).ToList(), "ID", "Name");

                BusinessLogicLayer.UpdateShopPriority obj = new BusinessLogicLayer.UpdateShopPriority();
                if (obj.ShopPriorityListUpdate(ls, CategoryID, 1, fConnectionString))
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
        // GET: /ShopPriority/Details/5
        [SessionExpire]
        [CustomAuthorize(Roles = "ShopPriority/CanRead")] 
        public ActionResult Details(int id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                ShopPriority shopPriority = db.ShopPriorities.Find(id);
                if (shopPriority == null)
                {
                    return HttpNotFound();
                }

                ViewBag.ShopName = db.Shops.Where(x => x.ID == shopPriority.ShopID).FirstOrDefault().Name;
                ViewBag.CityName = db.Cities.Where(x => x.ID == shopPriority.CityID).FirstOrDefault().Name;
                ViewBag.CategoryName = db.Categories.Where(x => x.ID == shopPriority.CategoryID).FirstOrDefault().Name;
                ViewBag.Level = db.Categories.Where(x => x.ID == shopPriority.CategoryID).FirstOrDefault().Level;

                return View(shopPriority);
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
        // GET: /ShopPriority/Create
        [SessionExpire]
        [CustomAuthorize(Roles = "ShopPriority/CanWrite")] 
        public ActionResult Create()
        {
            try
            {
                List<CategoryDetail> ldata = new List<CategoryDetail>();
                ldata.Add(new CategoryDetail { ID = 0, Name = "Select Category" });
                ViewBag.FranchiseList = new SelectList(db.Franchises.ToList(), "ID", "ContactPerson");
                ViewBag.CategoryList = new SelectList(db.Categories.Where(x => x.Level == 1).OrderBy(x => x.Name).ToList(), "ID", "Name");
                ViewBag.LevelTwoCategoryList = new SelectList(ldata, "ID", "Name");
                ViewBag.LevelThreeCategoryList = new SelectList(ldata, "ID", "Name");
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
        // POST: /ShopPriority/Create
        [HttpPost]
        [SessionExpire]
        [CustomAuthorize(Roles = "ShopPriority/CanWrite")] 
        public ActionResult Create(ShopPriority shopPriority, Int64 CategoryList, Int64? LevelTwoCategoryList, Int64? LevelThreeCategoryList, Int64 ShopID, Int64 FranchiseList)
        {
            try
            {
                // TODO: Add insert logic here
                List<CategoryDetail> ldata = new List<CategoryDetail>();
                ldata.Add(new CategoryDetail { ID = 0, Name = "Select Category" });
                ViewBag.FranchiseList = new SelectList(db.Franchises.ToList(), "ID", "ContactPerson", FranchiseList);
                ViewBag.CategoryList = new SelectList(db.Categories.Where(x => x.Level == 1).OrderBy(x => x.Name).ToList(), "ID", "Name");
                ViewBag.LevelTwoCategoryList = new SelectList(ldata, "ID", "Name");
                ViewBag.LevelThreeCategoryList = new SelectList(ldata, "ID", "Name");
                ViewBag.ShopID = new SelectList(db.Shops.Where(x => x.FranchiseID == FranchiseList).OrderBy(x => x.Name).ToList(), "ID", "Name");



                if (LevelThreeCategoryList > 0)
                {
                    shopPriority.CategoryID = Convert.ToInt64(LevelThreeCategoryList);
                }
                else if (LevelTwoCategoryList > 0)
                {
                    shopPriority.CategoryID = Convert.ToInt64(LevelTwoCategoryList);
                }
                else
                {
                    shopPriority.CategoryID = CategoryList;
                }

                shopPriority.ShopID = ShopID;

                Int64 catID = shopPriority.CategoryID;
                Int64 CityID = db.Shops.Where(x => x.ID == ShopID).FirstOrDefault().Pincode.CityID;
                //Int32 FranchiseID = shopPriority.FranchiseID == null ? 0 :(int)shopPriority.FranchiseID;////added
                Int32 FranchiseID =(int) db.Shops.Where(x => x.ID == ShopID).FirstOrDefault().FranchiseID;

                if (db.ShopPriorities.Where(x => x.ShopID == ShopID && x.CategoryID == catID).Count() < 1)
                {

                    if (shopPriority.PriorityLevel == 0)
                    {
                        shopPriority.PriorityLevel = this.priorityLevel(shopPriority.CategoryID, FranchiseID);////added CityID->
                    }

                    string msg;
                    if (this.IsValidated(out msg, shopPriority, FranchiseList))
                    {
                        List<object> paramValues = new List<object>();
                        paramValues.Add(DBNull.Value);
                        paramValues.Add(CityID);
                        paramValues.Add(shopPriority.CategoryID);
                        paramValues.Add(shopPriority.ShopID);
                        paramValues.Add(shopPriority.PriorityLevel);
                        paramValues.Add(shopPriority.IsActive);
                        paramValues.Add(DateTime.UtcNow.AddHours(5.5));
                        paramValues.Add(DBNull.Value);
                        paramValues.Add(BusinessLogicLayer.CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(Session["ID"])));
                        paramValues.Add(DBNull.Value);
                        paramValues.Add(BusinessLogicLayer.CommonFunctions.GetClientIP());
                        paramValues.Add("Net Browser");
                        paramValues.Add(DBNull.Value);
                        paramValues.Add(FranchiseID);////added
                        paramValues.Add(DataAccessLayer.Enumerators.DB_OPERATIONS.INSERT);
                        paramValues.Add(0);


                        ViewBag.Messaage = BusinessLogicLayer.UpdateShopPriority.Insertupdate_shoppriority(paramValues, DataAccessLayer.Enumerators.DB_OPERATIONS.INSERT, fConnectionString);
                    }
                    else
                    {
                        ViewBag.Messaage = msg;
                    }
                    return View("Create");


                }
                else
                {
                    ViewBag.Messaage = "Shop Priority already Present....!";
                    return RedirectToAction("Create");
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

        //
        // GET: /ShopPriority/Edit/5
        [SessionExpire]
        [CustomAuthorize(Roles = "ShopPriority/CanWrite")] 
        public ActionResult Edit(int id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                ShopPriority shopPriority = db.ShopPriorities.Find(id);
                if (shopPriority == null)
                {
                    return HttpNotFound();
                }
                this.ViewBagList(id);

                return View(shopPriority);
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
        // POST: /ShopPriority/Edit/5
        [HttpPost]
        [SessionExpire]
        [CustomAuthorize(Roles = "ShopPriority/CanWrite")] 
        public ActionResult Edit(ShopPriority shopPriority, Int64 CategoryList, Int64? LevelTwoCategoryList, Int64? LevelThreeCategoryList, Int64 ShopID, Int64 FranchiseList)
        {
            try
            {
                this.ViewBagList(shopPriority.ID);

                if (LevelThreeCategoryList > 0)
                {
                    shopPriority.CategoryID = Convert.ToInt64(LevelThreeCategoryList);
                }
                else if (LevelTwoCategoryList > 0)
                {
                    shopPriority.CategoryID = Convert.ToInt64(LevelTwoCategoryList);
                }
                else
                {
                    shopPriority.CategoryID = CategoryList;
                }

                shopPriority.ShopID = ShopID;

                Int64 catID = shopPriority.CategoryID;
                Int64 CityID = db.Shops.Where(x => x.ID == ShopID).FirstOrDefault().Pincode.CityID;
                //Int32 FranchiseID = shopPriority.FranchiseID == null ? 0 : (int)shopPriority.FranchiseID;////added
                Int32 FranchiseID = (int)db.Shops.Where(x => x.ID == ShopID).FirstOrDefault().FranchiseID;

                if (db.ShopPriorities.Where(x => x.ShopID == ShopID && x.CategoryID == catID).Count() > 0)
                {
                   
                    if (shopPriority.PriorityLevel == 0)
                    {
                        shopPriority.PriorityLevel = this.priorityLevel(shopPriority.CategoryID, FranchiseID);////added CityID->
                    }

                    List<object> paramValues = new List<object>();
                    paramValues.Add(shopPriority.ID);
                    paramValues.Add(CityID);
                    paramValues.Add(shopPriority.CategoryID);
                    paramValues.Add(shopPriority.ShopID);
                    paramValues.Add(shopPriority.PriorityLevel);
                    paramValues.Add(shopPriority.IsActive);
                    paramValues.Add(DBNull.Value);
                    paramValues.Add(DateTime.UtcNow.AddHours(5.5));
                    paramValues.Add(shopPriority.CreateBy);
                    paramValues.Add(BusinessLogicLayer.CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(Session["ID"])));
                    paramValues.Add(BusinessLogicLayer.CommonFunctions.GetClientIP());
                    paramValues.Add("Net Browser");
                    paramValues.Add(DBNull.Value);
                    paramValues.Add(FranchiseID);////added
                    paramValues.Add(DataAccessLayer.Enumerators.DB_OPERATIONS.UPDATE.ToString());
                    paramValues.Add(DBNull.Value);


                    ViewBag.Messaage = BusinessLogicLayer.UpdateShopPriority.Insertupdate_shoppriority(paramValues, DataAccessLayer.Enumerators.DB_OPERATIONS.UPDATE, fConnectionString);

                    this.ViewBagList(shopPriority.ID);

                    return View("Edit");


                }
                else
                {
                    ViewBag.Messaage = "Shop Priority Not Present....!";
                    return View("Edit");
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
        // GET: /ShopPriority/Delete/5
        [SessionExpire]
        [CustomAuthorize(Roles = "ShopPriority/CanDelete")] 
        public ActionResult Delete(int id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                ShopPriority shopPriority = db.ShopPriorities.Find(id);
                if (shopPriority == null)
                {
                    return HttpNotFound();
                }

                ViewBag.ShopName = db.Shops.Where(x => x.ID == shopPriority.ShopID).FirstOrDefault().Name;
                ViewBag.CityName = db.Cities.Where(x => x.ID == shopPriority.CityID).FirstOrDefault().Name;
                ViewBag.CategoryName = db.Categories.Where(x => x.ID == shopPriority.CategoryID).FirstOrDefault().Name;
                ViewBag.Level = db.Categories.Where(x => x.ID == shopPriority.CategoryID).FirstOrDefault().Level;



                return View(shopPriority);
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
        // POST: /ShopPriority/Delete/5
        [SessionExpire]
        [CustomAuthorize(Roles = "ShopPriority/CanDelete")] 
        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                ShopPriority shopPriority = db.ShopPriorities.Find(id);

                List<object> paramValues = new List<object>();
                paramValues.Add(id);
                paramValues.Add(DataAccessLayer.Enumerators.DB_OPERATIONS.DELETE.ToString());
                paramValues.Add(DBNull.Value);

                int resultCode;
                ViewBag.Messaage = BusinessLogicLayer.UpdateShopPriority.Delete_Shoppriority(paramValues, DataAccessLayer.Enumerators.DB_OPERATIONS.UPDATE, out resultCode, fConnectionString);

                if (resultCode == 3)
                    return RedirectToAction("Index");
                else
                    return View(shopPriority);
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

            catList = (from n in db.Categories
                       join pbc in db.PlanBindCategories on n.ID equals pbc.CategoryID
                       join pb in db.PlanBinds on pbc.PlanBindID equals pb.ID
                       join p in db.Plans on pb.PlanID equals p.ID
                       join op in db.OwnerPlans on p.ID equals op.PlanID                       
                       where op.OwnerID == id && p.PlanCode.Substring(0,4) == "GBFR"
                       select new CategoryDetail
                       {
                           ID = n.ID,
                           Name = n.Name
                       }).ToList();

            var lst = catList.Select(x => new { x.ID, x.Name })
                   .Union(newcatList.Select(x => new { x.ID, x.Name })).ToList();

            return Json(lst, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SelectCategory(Int64 ParentCategory, int level)
        {

            List<CategoryDetail> cd = new List<CategoryDetail>();
            cd = (from n in db.Categories
                  where n.ParentCategoryID == ParentCategory && n.Level == level
                  select new CategoryDetail
                  {
                      ID = n.ID,
                      Name = n.Name
                  }).ToList();
         
            return Json(cd, JsonRequestBehavior.AllowGet);

        }

        public ActionResult ShopPriorityList(Int64 FranchiseList, Int64 CategoryList, int level)
        {
            ModelLayer.Models.ViewModel.ShopPriorityList ls = new ModelLayer.Models.ViewModel.ShopPriorityList();

            try
            {
                BusinessLogicLayer.ShopPriorityList obj = new BusinessLogicLayer.ShopPriorityList();
                DataTable dt = new DataTable();
                dt = obj.Call_Select_Procedure(FranchiseList, CategoryList, level, System.Web.HttpContext.Current.Server);

                /*Select All the Shop By Franchise */
                ls.shopListByPriority = (from n in dt.AsEnumerable()
                                         select new ModelLayer.Models.ViewModel.ShopListByPriority
                                         {
                                             Id = n.Field<Int64>("ID"),
                                             Priority = n.Field<int>("PriorityLevel"),
                                             ShopName = n.Field<string>("ShopName"),
                                             ShopID = n.Field<Int64>("ShopID"),
                                             CityID = n.Field<Int64>("CityID")

                                         }).OrderBy(x => x.Priority).ToList();

                ViewBag.CategoryID = CategoryList;
                ViewBag.Franchise = FranchiseList;
                return PartialView("_EditPriorityList", ls);

            }
            catch
            {
                return PartialView("_EditPriorityList", ls);
            }
                

        }

        public JsonResult FranchiseShopList(Int64 franchiseID)
        {
            List<CategoryDetail> shopList = new List<CategoryDetail>();
            shopList = (from n in db.Shops
                        where n.FranchiseID == franchiseID
                        select new CategoryDetail
                        {
                            ID = n.ID,
                            Name = n.Name
                        }).OrderBy(x => x.Name).ToList();

            return Json(shopList, JsonRequestBehavior.AllowGet);

        }

        public int priorityLevel(Int64 CategoryID, Int32 FranchiseID)////added Int64 cityID->Int32 FranchiseID
        {
            BusinessLogicLayer.UpdateShopPriority obj = new BusinessLogicLayer.UpdateShopPriority();
            Int32 PriorityNo = obj.Get_MAX_ShopPriority(FranchiseID, CategoryID, fConnectionString);////added cityID->FranchiseID
            return (PriorityNo == 0 ? 1 : Convert.ToInt32(++PriorityNo));

        }

        private void ViewBagList(Int64 id)
        {
            ShopPriority shopPriority = db.ShopPriorities.Find(id);

            Int64 lshopID = Convert.ToInt64(shopPriority.ShopID);
            Int32 lfranchiseID = Convert.ToInt32(db.Shops.Where(x => x.ID == lshopID).FirstOrDefault().FranchiseID);
            Int64 lCategoryID = shopPriority.CategoryID;
            int lCategoyLevel = db.Categories.Where(x => x.ID == lCategoryID).FirstOrDefault().Level;


            List<CategoryDetail> ldata = new List<CategoryDetail>();
            ldata.Add(new CategoryDetail { ID = 0, Name = "Select Category" });
            ViewBag.FranchiseList = new SelectList(db.Franchises.ToList(), "ID", "ContactPerson", lfranchiseID);
            if (lCategoyLevel == 1)
            {
                ViewBag.CategoryList = new SelectList(db.Categories.Where(x => x.Level == 1).OrderBy(x => x.Name).ToList(), "ID", "Name", lCategoryID);
                ViewBag.LevelTwoCategoryList = new SelectList(ldata, "ID", "Name");
                ViewBag.LevelThreeCategoryList = new SelectList(ldata, "ID", "Name");
            }
            else if (lCategoyLevel == 2)
            {
                Int64 lparentOneID = Convert.ToInt64(db.Categories.Where(x => x.ID == lCategoryID).FirstOrDefault().ParentCategoryID);
                ViewBag.CategoryList = new SelectList(db.Categories.Where(x => x.Level == 1).OrderBy(x => x.Name).ToList(), "ID", "Name", lparentOneID);
                ViewBag.LevelTwoCategoryList = new SelectList(db.Categories.Where(x => x.Level == 2).OrderBy(x => x.Name).ToList(), "ID", "Name", lCategoryID);
                ViewBag.LevelThreeCategoryList = new SelectList(ldata, "ID", "Name");
            }
            else if (lCategoyLevel == 3)
            {
                Int64 lparentTwoID = Convert.ToInt64(db.Categories.Where(x => x.ID == lCategoryID).FirstOrDefault().ParentCategoryID);
                Int64 lparentOneID = Convert.ToInt64(db.Categories.Where(x => x.ID == lparentTwoID).FirstOrDefault().ParentCategoryID);
                ViewBag.CategoryList = new SelectList(db.Categories.Where(x => x.Level == 1).OrderBy(x => x.Name).ToList(), "ID", "Name", lparentOneID);
                ViewBag.LevelTwoCategoryList = new SelectList(db.Categories.Where(x => x.Level == 2).OrderBy(x => x.Name).ToList(), "ID", "Name", lparentTwoID);
                ViewBag.LevelThreeCategoryList = new SelectList(db.Categories.Where(x => x.Level == 3).OrderBy(x => x.Name).ToList(), "ID", "Name", lCategoryID);
            }
            ViewBag.ShopID = new SelectList(db.Shops.Where(x => x.FranchiseID == lfranchiseID).OrderBy(x => x.Name).ToList(), "ID", "Name", lshopID);

        }

        private bool IsValidated(out string msg, ShopPriority objSp, Int64 franchise)
        {
            msg = string.Empty;
            try
            {
                System.Text.StringBuilder str = new System.Text.StringBuilder("Following Errors Are Found" + Environment.NewLine);
                int Count = 0;
                if (franchise < 0)
                {
                    ++Count;
                    str.Append(Count.ToString() + ") Invalid Franchise" + Environment.NewLine);
                }
                if (objSp.ShopID < 0)
                {
                    ++Count;
                    str.Append(Count.ToString() + ") Invalid Isactive Value" + Environment.NewLine);
                }
                if (objSp.CategoryID < 0)
                {
                    ++Count;
                    str.Append(Count.ToString() + ")  Invalid Category Selection" + Environment.NewLine);
                }
                if (objSp.PriorityLevel < 0)
                {
                    ++Count;
                    str.Append(Count.ToString() + ") Invalid Priority" + Environment.NewLine);
                }
                if (objSp.IsActive == null)
                {
                    ++Count;
                    str.Append(Count.ToString() + ") Invalid IsActive Value" + Environment.NewLine);
                }
                if (objSp.CityID < 0)
                {
                    ++Count;
                    str.Append(Count.ToString() + ") Shop Pincode is not Valid Please Update Shop Pincode First" + Environment.NewLine);
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

    public class CategoryDetail
    {
        public Int64 ID { get; set; }
        public string Name { get; set; }
    }


 

}
