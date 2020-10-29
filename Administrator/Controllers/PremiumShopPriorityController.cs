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
    public class PremiumShopPriorityController : Controller
    {
        //
        // GET: /ShopMenuPriority/
        StringBuilder errStr = new StringBuilder("=====================================================================================" +
          Environment.NewLine
          + "ErrorLog Controller : PremiumShopPriorityController" + Environment.NewLine);
        EzeeloDBContext db = new EzeeloDBContext();

        //
        // GET: /FranchiseMenu/
        
        /// <summary>
        /// Index Listing according to the Selection
        /// On Franchise Selection :- Generate All the Categories of First Level category As per their Order Sequence & drop Down Fill With All First Level Category
        /// On First Level Category Selection :- Generate All the Shops under First Level category As per their Order Sequence & drop Down Fill With All Shop
        /// On Shop Selection :- Generate all third level category under the shop according to their order sequence
        /// </summary>
        /// <returns></returns>
        
        [SessionExpire]
        [CustomAuthorize(Roles = "PremiumShopPriority/CanRead")]
        
        public ActionResult Index()
        {
            try
            {

                List<CategoryDetail> ldata = new List<CategoryDetail>();
                ldata.Add(new CategoryDetail { ID = 0, Name = "Select Category" });
                /* List Of Franchise for Dropdown*/
                ViewBag.FranchiseList = new SelectList((from f in db.Franchises where f.ID != 1 select new { f.ID, f.BusinessDetail.Name }).ToList().OrderBy(x => x.Name), "ID", "Name");
                /*Empty List of First Level Category for Dropdown */                
                ViewBag.LevelOneCategoryList = new SelectList(ldata, "ID", "Name");
                /*Empty Shop List for Dropdown*/
                ViewBag.ShopID = new SelectList(ldata, "ID", "Name");
                /*Empty Shop List for Dropdown*/
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
               
                ModelState.AddModelError("Message", "Sorry! Problem in Generate Index view!!");
                ErrorLog.ErrorLogFile("Sorry! Problem in Generate Index view!! " + Environment.NewLine + errStr.ToString()
                    , ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);

                return View();
            }
        }

        /// <summary>
        /// On Post of Index
        /// </summary>
        /// <param name="FranchiseList">Franchise ID</param>
        /// <param name="shopID">Shop ID</param>
        /// <returns></returns>
        [HttpPost]
        [SessionExpire]
        [CustomAuthorize(Roles = "PremiumShopPriority/CanWrite")]      
        public ActionResult Index(Int64 FranchiseList, Int64 shopID)
        {
            try
            {
                List<CategoryDetail> ldata = new List<CategoryDetail>();
                ldata.Add(new CategoryDetail { ID = 0, Name = "Select Category" });
                /* List Of Franchise for Dropdown*/                
                ViewBag.FranchiseList = new SelectList((from f in db.Franchises where f.ID != 1 select new { f.ID, f.BusinessDetail.Name }).ToList().OrderBy(x => x.Name), "ID", "Name", FranchiseList);
                /*Empty Shop List for Dropdown*/
                ViewBag.ShopID = new SelectList(ldata, "ID", "Name");
                /*Empty List of First Level Category for Dropdown */ 
                ViewBag.LevelOneCategoryList = new SelectList(ldata, "ID", "Name");
                  /*Empty List of Second Level Category for Dropdown */
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
        
        /*Database Connection String */
        private static string fConnectionString = WebConfigurationManager.ConnectionStrings["EzeeloDBContext"].ToString();

        /// <summary>
        /// Save List of First Level Category sequence posted by from friend end partial view i.e. _EditPremiumLevelOneCategory
        /// </summary>
        /// <param name="ls">
        /// CategoryID 
        /// Order Sqeuence
        /// </param>
        /// <param name="Shop">ShopID</param>
        /// <returns></returns>
        public ActionResult LevelOnePriority(ModelLayer.Models.ViewModel.PremiumShopPriorityList ls, Int64 Shop)
        {
            try
            {
                /*List of Franchise for dropdown*/
                ViewBag.FranchiseList = new SelectList((from f in db.Franchises where f.ID != 1 select new { f.ID, f.BusinessDetail.Name }).ToList().OrderBy(x => x.Name), "ID", "Name");

                List<CategoryDetail> ldata = new List<CategoryDetail>();
                ldata.Add(new CategoryDetail { ID = 0, Name = "Select Category" });
                /*Empty Shop List for dropdown*/
                ViewBag.ShopID = new SelectList(ldata, "ID", "Name");

                BusinessLogicLayer.PremiumShopPriority obj = new BusinessLogicLayer.PremiumShopPriority();
                /*To get Login user ID and fetch personal detail*/
                Int64 userID = CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(Session["ID"]));
                /**/
                if (obj.UpdatePremiumShopPriority(ls,Shop, userID, fConnectionString))
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
        /// <summary>
        /// Detail about selected Categoruy or shop 
        /// </summary>
        /// <param name="id">selected row ID</param>
        /// <returns></returns>
        [SessionExpire]
        [CustomAuthorize(Roles = "PremiumShopPriority/CanRead")]
        public ActionResult Details(int id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                // find the record of pass premium shop id
                PremiumShopsPriority premiumShopsPriority = db.PremiumShopsPriorities.Find(id);
                if (premiumShopsPriority == null)
                {
                    return HttpNotFound();
                }

                long shopid = premiumShopsPriority.ShopID;
                //List of Shop for drop down
                int franchiseID = db.Shops.Where(x => x.ID == shopid).FirstOrDefault().FranchiseID == null ? 0 : Convert.ToInt32(db.Shops.Where(x => x.ID == shopid).FirstOrDefault().FranchiseID);
                int categoryID = premiumShopsPriority.CategoryID;
                List<CategoryDetail> ldata = new List<CategoryDetail>();
                ldata.Add(new CategoryDetail { ID = 0, Name = "Select Category" });
                //select franchise Contact Name
                ViewBag.FranchiseID = db.Franchises.Where(x => x.ID == franchiseID).FirstOrDefault().ContactPerson;
                //Select Shop Name
                ViewBag.ShopID = db.Shops.Where(x => x.ID == shopid).FirstOrDefault().Name;
                //select Category Name
                ViewBag.CategoryID = db.Categories.Where(x => x.ID == categoryID).FirstOrDefault().Name;

                return View(premiumShopsPriority);
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
        /// <summary>
        /// Initial Request for the Create on get request
        /// </summary>
        /// <returns></returns>
        [SessionExpire]
        [CustomAuthorize(Roles = "PremiumShopPriority/CanWrite")]
        public ActionResult Create()
        {
            try
            {
                List<CategoryDetail> ldata = new List<CategoryDetail>();
                ldata.Add(new CategoryDetail { ID = 0, Name = "Select Category" });
                // List of Franchise for dropdown
                ViewBag.FranchiseID = new SelectList((from f in db.Franchises where f.ID != 1 select new { f.ID, f.BusinessDetail.Name }).ToList().OrderBy(x => x.Name), "ID", "Name");
                // Empty Category DropDown
                //List of Category of Shop according to the assign Plan
               
                    ViewBag.CategoryID = new SelectList(ldata, "ID", "Name");               
                // Empty Shop DropDown
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
        /// <summary>
        /// To Add New third level category of PremiumShop in Priority Order
        /// </summary>
        /// <param name="PremiumShop">Premium shop Detail</param>
        /// <param name="FranchiseID">Franchise ID</param>
        /// <returns></returns>
         [HttpPost]
         [SessionExpire]
         [CustomAuthorize(Roles = "PremiumShopPriority/CanWrite")]
        public ActionResult Create(ModelLayer.Models.PremiumShopsPriority PremiumShop, int FranchiseID)
        {
            try
            {
                // TODO: Add insert logic here
                List<CategoryDetail> ldata = new List<CategoryDetail>();
                ldata.Add(new CategoryDetail { ID = 0, Name = "Select Category" });
                //List of Franchise For dropdown
                ViewBag.FranchiseID = new SelectList((from f in db.Franchises where f.ID != 1 select new { f.ID, f.BusinessDetail.Name }).ToList().OrderBy(x => x.Name), "ID", "Name", FranchiseID);
                
                Int64 shopID = PremiumShop.ShopID;
                Int64 catID = PremiumShop.CategoryID;

                //List of Active Shop  of Selected Franchise for dropdown

                List<ShopList> ls = new List<ShopList>();
                BusinessLogicLayer.ReadConfig rcKey = new BusinessLogicLayer.ReadConfig(System.Web.HttpContext.Current.Server);
                BusinessLogicLayer.PremiumShopPriority obj = new BusinessLogicLayer.PremiumShopPriority();
                DataTable dt = new DataTable();
                dt = obj.SelectShopFrom_PremiumShop(FranchiseID, 0, System.Web.HttpContext.Current.Server);
                ls = (from n in dt.AsEnumerable()
                      select new ShopList
                      {
                          Name = n.Field<string>("Name"),
                          ID = n.Field<Int64>("ID"),
                      }).ToList();

                ViewBag.ShopID = new SelectList(ls.ToList(), "ID", "Name", shopID);

                //List of Category of Shop according to the assign Plan

                

                ViewBag.CategoryID = new SelectList((from op in db.OwnerPlans
                                                     join p in db.Plans on op.PlanID equals p.ID
                                                     join pcc in db.PlanCategoryCharges on op.PlanID equals pcc.PlanID
                                                     join c in db.Categories on pcc.CategoryID equals c.ID
                                                     where pcc.IsActive == true && op.IsActive == true && c.IsActive == true
                                                     && c.Level == 3 && p.IsActive == true && op.OwnerID == shopID
                                                     && op.Plan.PlanCode.StartsWith("GBMR") && c.IsActive == true
                                                     select new ShopList { Name = c.Name, ID = c.ID }).ToList(), "ID", "Name", catID);


                
                // Check weather the category entry is present with shop or not 
                // To avoid duplicate entry of shop and category
                if (db.PremiumShopsPriorities.Where(x => x.CategoryID == catID && x.ShopID == shopID ).Count() < 1)
                {
                    //BusinessLogicLayer.PremiumShopPriority obj = new PremiumShopPriority();

                    if (PremiumShop.PriorityLevel == 0)
                    {
                        //to get Max priority according to the shop Category List (third level Category)
                        PremiumShop.PriorityLevel = obj.Get_MAX_PremiumShopsPriorityOrder(PremiumShop.ShopID, PremiumShop.CategoryID, fConnectionString);
                        //increase priority by 1 for next highest priority 
                        PremiumShop.PriorityLevel += 1;
                    }

                    string msg;
                    //check validation of data for new insert record
                    if (this.IsValidated(out msg, PremiumShop))
                    {
                        List<object> paramValues = new List<object>();
                        paramValues.Add(DBNull.Value);
                        paramValues.Add(FranchiseID);
                        paramValues.Add(PremiumShop.ShopID);
                        paramValues.Add(PremiumShop.CategoryID);                        
                        paramValues.Add(PremiumShop.PriorityLevel);
                        paramValues.Add(PremiumShop.IsActive);                        
                        paramValues.Add(DateTime.UtcNow.AddHours(5.5));
                        paramValues.Add(DBNull.Value);
                        paramValues.Add(BusinessLogicLayer.CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(Session["ID"])));
                        paramValues.Add(DBNull.Value);
                        paramValues.Add(BusinessLogicLayer.CommonFunctions.GetClientIP());
                        paramValues.Add("Net Browser");
                        paramValues.Add(DBNull.Value);
                        paramValues.Add(DataAccessLayer.Enumerators.DB_OPERATIONS.INSERT);
                        paramValues.Add(0);
                        
                        //call businesslogic layer for insert given parameter values
                        //get messages according to the operation perform
                        ViewBag.Messaage = obj.Insertupdate_PremiumShopsPriority(paramValues, DataAccessLayer.Enumerators.DB_OPERATIONS.INSERT, fConnectionString);
                    }
                    else
                    {
                        ViewBag.Messaage = msg;
                    }

                    return View("Create");


                }
                else
                {
                    ViewBag.Messaage = "Shop Menu already Present....!";
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

        /// <summary>
         /// To Correction in third level category of PremiumShop in Priority Order or else
         /// </summary>         
         /// <returns></returns>
        [SessionExpire]
        [CustomAuthorize(Roles = "PremiumShopPriority/CanWrite")]
        public ActionResult Edit(int id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                ModelLayer.Models.PremiumShopsPriority premiumShopPriority = db.PremiumShopsPriorities.Find(id);
                if (premiumShopPriority == null)
                {
                    return HttpNotFound();
                }


                List<CategoryDetail> ldata = new List<CategoryDetail>();
                ldata.Add(new CategoryDetail { ID = 0, Name = "Select Category" });
                //List of Franchise For dropdown
                ViewBag.FranchiseID = new SelectList((from f in db.Franchises where f.ID != 1 select new { f.ID, f.BusinessDetail.Name }).ToList().OrderBy(x => x.Name),
                                                    "ID", "Name", premiumShopPriority.FranchiseID);
                //List of Active Shop  of Selected Franchise for dropdown
                ViewBag.ShopID = new SelectList((from n in db.Shops
                                                 where n.FranchiseID == premiumShopPriority.FranchiseID
                                                 select new ShopList
                                                 {
                                                     Name = n.Name,
                                                     ID = n.ID
                                                 }).ToList(), "ID", "Name", premiumShopPriority.ShopID);
                //List of Category of Shop according to the assign Plan
                ViewBag.CategoryID = new SelectList((from op in db.OwnerPlans
                                                     join p in db.Plans on op.PlanID equals p.ID
                                                     join pcc in db.PlanCategoryCharges on op.PlanID equals pcc.PlanID
                                                     join c in db.Categories on pcc.CategoryID equals c.ID
                                                     where pcc.IsActive == true && op.IsActive == true && op.OwnerID == premiumShopPriority.ShopID
                                                     && op.Plan.PlanCode.StartsWith("GBMR") && c.IsActive == true
                                                     select new ShopList { Name = c.Name, ID = c.ID }).ToList(), "ID", "Name", premiumShopPriority.CategoryID);

                return View(premiumShopPriority);
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

        /// <summary>
        /// To Correction in third level category of PremiumShop in Priority Order or else
        /// </summary>
        /// <param name="PremiumShop">Premium shop Detail</param>
        /// <param name="FranchiseID">Franchise ID</param>
        /// <returns></returns>
        [HttpPost]
        [SessionExpire]
        [CustomAuthorize(Roles = "PremiumShopPriority/CanWrite")]
        public ActionResult Edit(ModelLayer.Models.PremiumShopsPriority PremiumShop, Int32 FranchiseID, HttpPostedFileBase file)
        {
            try
            {

                ModelLayer.Models.PremiumShopsPriority lPremiumShop = db.PremiumShopsPriorities.Find(PremiumShop.ID);

                Int64 catID = PremiumShop.CategoryID;

                //List of Franchise For dropdown  
                ViewBag.FranchiseID = new SelectList((from f in db.Franchises where f.ID != 1 select new { f.ID, f.BusinessDetail.Name }).ToList().OrderBy(x => x.Name),
                                                        "ID", "Name", FranchiseID);

                Int64 shopID = PremiumShop.ShopID;
                //List of Active Shop  of Selected Franchise for dropdown
                ViewBag.ShopID = new SelectList((from n in db.Shops
                                                 where n.FranchiseID == FranchiseID
                                                 select new ShopList
                                                 {
                                                     Name = n.Name,
                                                     ID = n.ID
                                                 }).ToList(), "ID", "Name", shopID);
                //List of Category of Shop according to the assign Plan
                ViewBag.CategoryID = new SelectList((from op in db.OwnerPlans
                                                     join p in db.Plans on op.PlanID equals p.ID
                                                     join pcc in db.PlanCategoryCharges on op.PlanID equals pcc.PlanID
                                                     join c in db.Categories on pcc.CategoryID equals c.ID
                                                     where pcc.IsActive == true && op.IsActive == true && op.OwnerID == shopID
                                                     && op.Plan.PlanCode.StartsWith("GBMR") && c.IsActive == true
                                                     select new ShopList { Name = c.Name, ID = c.ID }).ToList(), "ID", "Name", FranchiseID);


                // Check weather the category entry is present with shop or not 
                // To avoid duplicate entry of shop and category
                if (db.PremiumShopsPriorities.Where(x => x.ShopID == PremiumShop.ShopID && x.CategoryID == catID).Count() > 0)
                {

                    BusinessLogicLayer.PremiumShopPriority obj = new PremiumShopPriority();
                    //to get Max priority according to the shop Category List (third level Category)
                    int maxPriority = obj.Get_MAX_PremiumShopsPriorityOrder(PremiumShop.ShopID, 3, fConnectionString);
                    if (PremiumShop.PriorityLevel == 0)
                    {
                        PremiumShop.PriorityLevel = maxPriority;
                    }
                    else if (PremiumShop.PriorityLevel > maxPriority)
                    {
                        PremiumShop.PriorityLevel = maxPriority;
                    }

                    string msg;
                    //check validation of data for new insert record
                    if (this.IsValidated(out msg, PremiumShop))
                    {
                        List<object> paramValues = new List<object>();
                        paramValues.Add(PremiumShop.ID);
                        paramValues.Add(FranchiseID);
                        paramValues.Add(PremiumShop.ShopID);
                        paramValues.Add(PremiumShop.CategoryID);
                        paramValues.Add(PremiumShop.PriorityLevel);
                        paramValues.Add(PremiumShop.IsActive);
                        paramValues.Add(DateTime.UtcNow.AddHours(5.5));
                        paramValues.Add(lPremiumShop.CreateDate);
                        paramValues.Add(lPremiumShop.CreateBy);
                        paramValues.Add(BusinessLogicLayer.CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(Session["ID"])));
                        paramValues.Add(BusinessLogicLayer.CommonFunctions.GetClientIP());
                        paramValues.Add("Net Browser");
                        paramValues.Add(DBNull.Value);
                        paramValues.Add(DataAccessLayer.Enumerators.DB_OPERATIONS.UPDATE);
                        paramValues.Add(0);

                        //call businesslogic layer for insert given parameter values
                        //get messages according to the operation perform
                        ViewBag.Messaage = obj.Insertupdate_PremiumShopsPriority(paramValues, DataAccessLayer.Enumerators.DB_OPERATIONS.UPDATE, fConnectionString);
                    }

                }
                else
                {
                    ViewBag.Messaage = "Premium Shop Not Present....!";
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
        /// <summary>
        /// to delete third level category of premium shop
        /// </summary>
        /// <param name="id">record ID</param>
        /// <returns></returns>
        [SessionExpire]
        [CustomAuthorize(Roles = "PremiumShopPriority/CanDelete")]
        public ActionResult Delete(int id)
        {
            try
            {
                //check is provided record id is null or not
                if (id == null)
                {
                    //if record id is null redirec to the Bad request page
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                //find the record in respect to provided id
                PremiumShopsPriority shopMenuPriority = db.PremiumShopsPriorities.Find(id);
                // is record presenet or not
                if (shopMenuPriority == null)
                {
                    // no record is found redirect to the not found page
                    return HttpNotFound();
                }

                long shopid = shopMenuPriority.ShopID;
                // Franchise Id of Given shop
                int franchiseID = db.Shops.Where(x => x.ID == shopid).FirstOrDefault().FranchiseID == null ? 0 : Convert.ToInt32(db.Shops.Where(x => x.ID == shopid).FirstOrDefault().FranchiseID);
                // third level Category ID of provided argument
                int categoryID = shopMenuPriority.CategoryID;

                List<CategoryDetail> ldata = new List<CategoryDetail>();                
                ldata.Add(new CategoryDetail { ID = 0, Name = "Select Category" });
                // select Franchise Name by FranchiseID
                ViewBag.FranchiseID = db.Franchises.Where(x => x.ID == franchiseID).FirstOrDefault().ContactPerson;
                // Select Shop Name by ShopID
                ViewBag.ShopID = db.Shops.Where(x => x.ID == shopid).FirstOrDefault().Name;
                // Select CategoryName by CategoryID
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

        /// <summary>
        /// Delete Specific Record from Premium Shop
        /// </summary>
        /// <param name="id">Record ID</param>
        /// <returns></returns>
        [SessionExpire]
        [CustomAuthorize(Roles = "PremiumShopPriority/CanDelete")]
        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                //find the record in respect to provided id
                PremiumShopsPriority PremiumShopsPriority = db.PremiumShopsPriorities.Find(id);
                if (PremiumShopsPriority == null)
                {
                    //record not found redirect to norecord found page
                    return HttpNotFound();
                }

                long shopid = PremiumShopsPriority.ShopID;

                // Franchise ID of Given Shop
                int franchiseID = db.Shops.Where(x => x.ID == shopid).FirstOrDefault().FranchiseID == null ? 0 : Convert.ToInt32(db.Shops.Where(x => x.ID == shopid).FirstOrDefault().FranchiseID);
                // Category ID from given category
                int categoryID = PremiumShopsPriority.CategoryID;
                List<CategoryDetail> ldata = new List<CategoryDetail>();
                ldata.Add(new CategoryDetail { ID = 0, Name = "Select Category" });
                // select Franchsie Name 
                ViewBag.FranchiseID = db.Franchises.Where(x => x.ID == franchiseID).FirstOrDefault().ContactPerson;
                // Selec Shop Name
                ViewBag.ShopID = db.Shops.Where(x => x.ID == shopid).FirstOrDefault().Name;
                // Select Category Name 
                ViewBag.CategoryID = db.Categories.Where(x => x.ID == categoryID).FirstOrDefault().Name;


                BusinessLogicLayer.PremiumShopPriority obj = new PremiumShopPriority();
                // Declare to Collect all parameter in paramValues variable to perform delete opration at backend (at database storeprocedure)
                List<object> paramValues = new List<object>();
                //Record id to delete
                paramValues.Add(id);
                //Operation to perform
                paramValues.Add(DataAccessLayer.Enumerators.DB_OPERATIONS.DELETE.ToString());
                //Output operation
                paramValues.Add(DBNull.Value);

                int resultCode =0;
                int catID = PremiumShopsPriority.CategoryID;
                // Check level of the provided category
                if (db.Categories.Where(x => x.ID == catID).FirstOrDefault().Level == 1)
                {
                    //generate message to delete level one/ primery category of shop
                    ViewBag.Messaage = obj.LevelOneWise_Delete_PremiumShopsPriority(paramValues, DataAccessLayer.Enumerators.DB_OPERATIONS.UPDATE, out resultCode, fConnectionString);
                }
                else if (db.Categories.Where(x => x.ID == catID).FirstOrDefault().Level == 2)
                {
                    //generate message to delete level one/ primery category of shop
                    ViewBag.Messaage = obj.LevelTwoWise_Delete_PremiumShopsPriority(paramValues, DataAccessLayer.Enumerators.DB_OPERATIONS.UPDATE, out resultCode, fConnectionString);
                }
                else if (db.Categories.Where(x => x.ID == catID).FirstOrDefault().Level == 3)
                {
                    //generate message to delete level three category of shop
                    ViewBag.Messaage = obj.Delete_PremiumShopsPriority(paramValues, DataAccessLayer.Enumerators.DB_OPERATIONS.UPDATE, out resultCode, fConnectionString);
                }
                if (resultCode == 3)
                    return RedirectToAction("Index");
                else
                    return View(PremiumShopsPriority);
              
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


        /// <summary>
        /// Select Category By their Parents and level
        /// </summary>
        /// <param name="ParentCategory">ParentCatID</param>
        /// <param name="level">Level</param>
        /// <returns>CategoryDetail list</returns>
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

        /// <summary>
        /// CategoryList for partial page i.e. _EditPremiumShopPriority
        /// </summary>
        /// <param name="ShopID">ShopID</param>
        /// <param name="CategoryID">CategoryID</param>
        /// <returns></returns>
        public ActionResult GetFranchiseCategoryList(Int64 ShopID, Int64 CategoryID)
        {
            ModelLayer.Models.ViewModel.PremiumShopPriorityList ls = new ModelLayer.Models.ViewModel.PremiumShopPriorityList();

            try
            {
                ViewBag.Shop = ShopID;
                BusinessLogicLayer.ReadConfig rcKey = new BusinessLogicLayer.ReadConfig(System.Web.HttpContext.Current.Server);
                BusinessLogicLayer.PremiumShopPriority obj = new BusinessLogicLayer.PremiumShopPriority();
                DataTable dt = new DataTable();
                dt = obj.Call_Select_Procedure(ShopID, CategoryID, System.Web.HttpContext.Current.Server);

                /*Select All the Shop By Franchise */
                ls.premiumShopPriorityList = (from n in dt.AsEnumerable()
                                           select new ModelLayer.Models.ViewModel.PremiumShopPriorityViewModel
                                           {
                                               ID = n.Field<Int64>("ID"),
                                               ShopID = n.Field<Int64>("ShopID"),
                                               CategoryID = n.Field<int>("CategoryID"),
                                               CategoryName = n.Field<string>("CategoryName"),
                                               ShopName = n.Field<string>("ShopName"),

                                               Priority = n.Field<int?>("PriorityLevel") == null ? 0 : Convert.ToInt32(n.Field<int?>("PriorityLevel")),                                               
                                               IsActive = n.Field<bool>("IsActive")
                                           }).OrderBy(x => x.Priority).ToList();

              
                return PartialView("_EditPremiumShopPriority", ls);

            }
            catch
            {
                return PartialView("_EditPremiumShopPriority", ls);
            }
        }

        

        /// <summary>
        /// Premium Shop List for Sequence Manipulation and return partial view i.e. _EditPremiumLevelOneShop
        /// </summary>
        /// <param name="FranchiseID"></param>
        /// <param name="CategoryID"></param>
        /// <returns></returns>
        public ActionResult GetFranchiseFirstLevel_ShopList(int FranchiseID, Int64 CategoryID)
        {
            ModelLayer.Models.ViewModel.PremiumShopPriorityList ls = new ModelLayer.Models.ViewModel.PremiumShopPriorityList();

            try
            {
                ViewBag.FranchiseID = FranchiseID;
                ViewBag.CategoryID = CategoryID;
                BusinessLogicLayer.ReadConfig rcKey = new BusinessLogicLayer.ReadConfig(System.Web.HttpContext.Current.Server);
                BusinessLogicLayer.PremiumShopPriority obj = new BusinessLogicLayer.PremiumShopPriority();
                DataTable dt = new DataTable();
                dt = obj.LevelOneWise_Call_Select_Procedure(FranchiseID, CategoryID, System.Web.HttpContext.Current.Server);

                /*Select All the Shop By Franchise */
                ls.premiumShopPriorityList = (from n in dt.AsEnumerable()
                                              select new ModelLayer.Models.ViewModel.PremiumShopPriorityViewModel
                                              {
                                                  ID = n.Field<Int64>("ID"),
                                                  ShopID = n.Field<Int64>("ShopID"),
                                                  CategoryID = n.Field<int>("CategoryID"),
                                                  CategoryName = n.Field<string>("CategoryName"),
                                                  ShopName = n.Field<string>("ShopName"),
                                                  Priority = n.Field<int?>("PriorityLevel") == null ? 0 : Convert.ToInt32(n.Field<int?>("PriorityLevel")),
                                                  IsActive = n.Field<bool>("IsActive")
                                              }).OrderBy(x => x.Priority).ToList();

                //ViewBag.CategoryID = CategoryID;
                //ViewBag.Franchise = FranchiseID;
                return PartialView("_EditPremiumLevelOneShop", ls);

            }
            catch
            {
                return PartialView("_EditPremiumLevelOneShop", ls);
            }
        }

        /// <summary>
        /// Generate Shop List according to the Francshie and Level One Category as primary category in premium shop
        /// to manupulate shop sequence
        /// retrurn json result
        /// </summary>
        /// <param name="FranchiseID">FranchiseID</param>
        /// <param name="CategoryID">Category ID</param>
        /// <returns></returns>
        public JsonResult GetShopByFranchiseAndCategory(int FranchiseID, Int64 CategoryID)
        {
            List<ModelLayer.Models.ViewModel.PremiumShopPriorityViewModel> ls = new List<ModelLayer.Models.ViewModel.PremiumShopPriorityViewModel>();

            try
            {
                ViewBag.FranchiseID = FranchiseID;
                ViewBag.CategoryID = CategoryID;
                BusinessLogicLayer.ReadConfig rcKey = new BusinessLogicLayer.ReadConfig(System.Web.HttpContext.Current.Server);
                BusinessLogicLayer.PremiumShopPriority obj = new BusinessLogicLayer.PremiumShopPriority();
                DataTable dt = new DataTable();
                dt = obj.LevelOneWise_Call_Select_Procedure(FranchiseID, CategoryID, System.Web.HttpContext.Current.Server);

                /*Select All the Shop By Franchise */
                ls = (from n in dt.AsEnumerable()
                                              select new ModelLayer.Models.ViewModel.PremiumShopPriorityViewModel
                                              {
                                                  ID = n.Field<Int64>("ID"),
                                                  ShopID = n.Field<Int64>("ShopID"),
                                                  CategoryID = n.Field<int>("CategoryID"),
                                                  CategoryName = n.Field<string>("CategoryName"),
                                                  ShopName = n.Field<string>("ShopName"),
                                                  Priority = n.Field<int?>("PriorityLevel") == null ? 0 : Convert.ToInt32(n.Field<int?>("PriorityLevel")),
                                                  IsActive = n.Field<bool>("IsActive")
                                              }).OrderBy(x => x.Priority).ToList();

                return Json(ls, JsonRequestBehavior.AllowGet);
              
            }
            catch
            {
                return Json(ls, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// List of shop for drop down on selection of franchise 
        /// </summary>
        /// <param name="franchiseID"></param>
        /// <returns></returns>
        public JsonResult ShopListByFranchise(Int32 franchiseID)
        {
            List<ShopList> ls = new List<ShopList>();
            BusinessLogicLayer.ReadConfig rcKey = new BusinessLogicLayer.ReadConfig(System.Web.HttpContext.Current.Server);
            BusinessLogicLayer.PremiumShopPriority obj = new BusinessLogicLayer.PremiumShopPriority();
            DataTable dt = new DataTable();
            dt = obj.SelectShopFrom_PremiumShop(franchiseID, 0, System.Web.HttpContext.Current.Server);
            ls = (from n in dt.AsEnumerable()
                  select new ShopList
                  {
                      Name = n.Field<string>("Name"),
                      ID = n.Field<Int64>("ID"),
                  }).ToList();
            return Json(ls, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Validate given data
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="objSp"></param>
        /// <returns></returns>
        private bool IsValidated(out string msg, PremiumShopsPriority objSp)
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

        /// <summary>
        /// Validate Given Data
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="objSp"></param>
        /// <returns></returns>
        private bool IsValidatedPrimeryMarket(out string msg, ModelLayer.Models.ViewModel.PremiumShopPrimaryMarket_ViewModel objSp)
        {
            msg = string.Empty;
            try
            {
                System.Text.StringBuilder str = new System.Text.StringBuilder("Following Errors Are Found" + Environment.NewLine);
                int Count = 0;
                
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
            //List<ShopList> ls = new List<ShopList>();

            //ls = (from n in db.Shops     
            //      join ps in db.PremiumShopsPriorities on n.ID equals ps.ShopID                 
            //      where n.FranchiseID == franchiseID && n.IsActive == true
            //      select new ShopList
            //      {
            //          Name = n.Name,
            //          ID = n.ID
            //      }).ToList();
            List<ShopList> ls = new List<ShopList>();
            BusinessLogicLayer.ReadConfig rcKey = new BusinessLogicLayer.ReadConfig(System.Web.HttpContext.Current.Server);
            BusinessLogicLayer.PremiumShopPriority obj = new BusinessLogicLayer.PremiumShopPriority();
            DataTable dt = new DataTable();
            dt = obj.SelectShopFrom_PremiumShop(franchiseID, 0, System.Web.HttpContext.Current.Server);
            ls = (from n in dt.AsEnumerable()
                  select new ShopList
                  {
                      Name = n.Field<string>("Name"),
                      ID = n.Field<Int64>("ID"),
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
                  where pcc.IsActive == true && op.IsActive == true && c.IsActive == true
                  && c.Level == 3 && p.IsActive == true && op.OwnerID == ShopID
                  && op.Plan.PlanCode.StartsWith("GBMR") && c.IsActive == true
                  select new ShopList { Name = c.Name, ID = c.ID }).ToList();

            return Json(ls.ToList().OrderBy(x => x.Name), JsonRequestBehavior.AllowGet);
        }


        /* Pradnyakar Badge
         * First Level Category Wise  Premium Shop
         * Started on Date : 02-03-2016
         */
        #region ----- Shop Entry For Premium Shop -----

        [SessionExpire]
        [CustomAuthorize(Roles = "PremiumShopPriority/CanWrite")]
        public ActionResult Shop_Create()
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

        [HttpPost]
        [SessionExpire]
        [CustomAuthorize(Roles = "PremiumShopPriority/CanWrite")]
        public ActionResult Shop_Create(ModelLayer.Models.PremiumShopsPriority PremiumShop, int FranchiseID)
        {
            try
            {
                // TODO: Add insert logic here
                ViewBag.FranchiseID = new SelectList((from f in db.Franchises where f.ID != 1 select new { f.ID, f.BusinessDetail.Name }).ToList().OrderBy(x => x.Name), "ID", "Name", FranchiseID);
                Int64 shopID = PremiumShop.ShopID;
                Int32 CategoryID = PremiumShop.CategoryID;
                /**/
                List<ShopList> ls = new List<ShopList>();
                BusinessLogicLayer.PremiumShopPriority objPremiumShopPriority = new BusinessLogicLayer.PremiumShopPriority();
                DataTable dt = new DataTable();
                dt = objPremiumShopPriority.LevelOneWise_SelectShopFrom_PremiumShop(FranchiseID, CategoryID, System.Web.HttpContext.Current.Server);
                ls = (from n in dt.AsEnumerable()
                      select new ShopList
                      {
                          Name = n.Field<string>("Name"),
                          ID = n.Field<Int64>("ID"),
                      }).ToList();
                ViewBag.ShopID = new SelectList(ls, "ID", "Name", shopID);
               /*End OF Viewbag Shop*/

                DataTable dt1 = new DataTable();
                List<ShopList> ls1 = new List<ShopList>();
                dt1 = objPremiumShopPriority.PremiumShopPrimaryMarket_Call_Select_Procedure(FranchiseID, System.Web.HttpContext.Current.Server);

                /*Select All the Shop By Franchise */
                ls1 = (from n in dt1.AsEnumerable()
                       select new ShopList
                      {
                          ID = n.Field<int>("CategoryID"),
                          Name = n.Field<string>("CategoryName"),
                      }).ToList();

                ViewBag.CategoryID = new SelectList(ls1, "ID", "Name", CategoryID);
                /*End Of Viewbang Category*/

                Int64 catID = PremiumShop.CategoryID;

                if (db.PremiumShopsPriorities.Where(x => x.CategoryID == catID && x.ShopID == shopID).Count() < 1)
                {
                    BusinessLogicLayer.PremiumShopPriority obj = new PremiumShopPriority();

                    if (PremiumShop.PriorityLevel == 0)
                    {
                        PremiumShop.PriorityLevel = obj.LevelOneWise_Get_MAX_PremiumShopsPriorityOrder(PremiumShop.CategoryID, FranchiseID, fConnectionString);
                        PremiumShop.PriorityLevel += 1;
                    }

                    string msg;
                    if (this.IsValidated(out msg, PremiumShop))
                    {
                        List<object> paramValues = new List<object>();
                        paramValues.Add(DBNull.Value);
                        paramValues.Add(FranchiseID);
                        paramValues.Add(PremiumShop.ShopID);
                        paramValues.Add(PremiumShop.CategoryID);
                        paramValues.Add(PremiumShop.PriorityLevel);
                        paramValues.Add(PremiumShop.IsActive);
                        paramValues.Add(DateTime.UtcNow.AddHours(5.5));
                        paramValues.Add(DBNull.Value);
                        paramValues.Add(BusinessLogicLayer.CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(Session["ID"])));
                        paramValues.Add(DBNull.Value);
                        paramValues.Add(BusinessLogicLayer.CommonFunctions.GetClientIP());
                        paramValues.Add("Net Browser");
                        paramValues.Add(DBNull.Value);
                        paramValues.Add(DataAccessLayer.Enumerators.DB_OPERATIONS.INSERT);
                        paramValues.Add(0);

                        ViewBag.Messaage = obj.LevelOneWise_Insertupdate_PremiumShopsPriority(paramValues, DataAccessLayer.Enumerators.DB_OPERATIONS.INSERT, fConnectionString);
                    }
                    else
                    {
                        ViewBag.Messaage = msg;
                    }

                    return View("Shop_Create");


                }
                else
                {
                    ViewBag.Messaage = "Shop Entry is already Present with Parent Category....!";
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

       
        [SessionExpire]
        [CustomAuthorize(Roles = "PremiumShopPriority/CanWrite")]
        public ActionResult Shop_Edit(int id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                ModelLayer.Models.PremiumShopsPriority premiumShopPriority = db.PremiumShopsPriorities.Find(id);
                if (premiumShopPriority == null)
                {
                    return HttpNotFound();
                }


                List<CategoryDetail> ldata = new List<CategoryDetail>();
                ldata.Add(new CategoryDetail { ID = 0, Name = "Select Category" });
                ViewBag.FranchiseID = new SelectList((from f in db.Franchises where f.ID != 1 select new { f.ID, f.BusinessDetail.Name }).ToList().OrderBy(x => x.Name),
                                                    "ID", "Name", premiumShopPriority.FranchiseID);

                //ViewBag.ShopID = new SelectList((from n in db.Shops
                //                                 where n.FranchiseID == premiumShopPriority.FranchiseID
                //                                 select new ShopList
                //                                 {
                //                                     Name = n.Name,
                //                                     ID = n.ID
                //                                 }).ToList(), "ID", "Name", premiumShopPriority.ShopID);

                //ViewBag.CategoryID = new SelectList((from op in db.OwnerPlans
                //                                     join p in db.Plans on op.PlanID equals p.ID
                //                                     join pcc in db.PlanCategoryCharges on op.PlanID equals pcc.PlanID
                //                                     join c in db.Categories on pcc.CategoryID equals c.ID
                //                                     where pcc.IsActive == true && op.IsActive == true && op.OwnerID == premiumShopPriority.ShopID
                //                                     && op.Plan.PlanCode.StartsWith("GBMR") && c.IsActive == true
                //                                     select new ShopList { Name = c.Name, ID = c.ID }).ToList(), "ID", "Name", premiumShopPriority.CategoryID);

                /**/
                List<CategoryForPremiumShopList> ls = new List<CategoryForPremiumShopList>();
                BusinessLogicLayer.PremiumShopPriority objPremiumShopPriority = new BusinessLogicLayer.PremiumShopPriority();
                DataTable dt = new DataTable();
                dt = objPremiumShopPriority.LevelOneWise_SelectFirstLevelCategoryFrom_PremiumShop(premiumShopPriority.FranchiseID, System.Web.HttpContext.Current.Server);
                ls = (from n in dt.AsEnumerable()
                      select new CategoryForPremiumShopList
                      {
                          Name = n.Field<string>("Name"),
                          ID = n.Field<Int32>("ID"),
                      }).ToList();

                ViewBag.CategoryID = new SelectList(ls.ToList(), "ID", "Name", premiumShopPriority.CategoryID);
                /*End OF Viewbag Shop*/

                DataTable dt1 = new DataTable();
                List<ShopList> ls1 = new List<ShopList>();
                dt1 = objPremiumShopPriority.LevelOneWise_SelectShopFrom_PremiumShop(premiumShopPriority.FranchiseID, Convert.ToInt64(premiumShopPriority.CategoryID), System.Web.HttpContext.Current.Server);
                ls1 = (from n in dt1.AsEnumerable()
                       select new ShopList
                       {
                           Name = n.Field<string>("Name"),
                           ID = n.Field<Int64>("ID"),
                       }).ToList();
                ViewBag.ShopID = new SelectList(ls1.ToList(), "ID", "Name", premiumShopPriority.ShopID);
              
                /*End Of Viewbang Category*/


                return View(premiumShopPriority);
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

        [HttpPost]
        [SessionExpire]
        [CustomAuthorize(Roles = "PremiumShopPriority/CanWrite")]
        public ActionResult Shop_Edit(ModelLayer.Models.PremiumShopsPriority PremiumShop, Int32 FranchiseID)
        {
            try
            {

                ModelLayer.Models.PremiumShopsPriority lPremiumShop = db.PremiumShopsPriorities.Find(PremiumShop.ID);

                Int64 catID = PremiumShop.CategoryID;


                ViewBag.FranchiseID = new SelectList((from f in db.Franchises where f.ID != 1 select new { f.ID, f.BusinessDetail.Name }).ToList().OrderBy(x => x.Name),
                                                        "ID", "Name", FranchiseID);
                Int64 shopID = PremiumShop.ShopID;
                /**/
                List<CategoryForPremiumShopList> ls = new List<CategoryForPremiumShopList>();
                BusinessLogicLayer.PremiumShopPriority objPremiumShopPriority = new BusinessLogicLayer.PremiumShopPriority();
                DataTable dt = new DataTable();
                dt = objPremiumShopPriority.LevelOneWise_SelectFirstLevelCategoryFrom_PremiumShop(PremiumShop.FranchiseID, System.Web.HttpContext.Current.Server);
                ls = (from n in dt.AsEnumerable()
                      select new CategoryForPremiumShopList
                      {
                          Name = n.Field<string>("Name"),
                          ID = n.Field<Int32>("ID"),
                      }).ToList();

                ViewBag.CategoryID = new SelectList(ls.ToList(), "ID", "Name", PremiumShop.CategoryID);
                /*End OF Viewbag Shop*/

                DataTable dt1 = new DataTable();
                List<ShopList> ls1 = new List<ShopList>();
                dt1 = objPremiumShopPriority.LevelOneWise_SelectShopFrom_PremiumShop(PremiumShop.FranchiseID, Convert.ToInt64(PremiumShop.CategoryID), System.Web.HttpContext.Current.Server);
                ls1 = (from n in dt1.AsEnumerable()
                       select new ShopList
                       {
                           Name = n.Field<string>("Name"),
                           ID = n.Field<Int64>("ID"),
                       }).ToList();
                ViewBag.ShopID = new SelectList(ls1.ToList(), "ID", "Name", PremiumShop.ShopID);

                /*End Of Viewbang Category*/


                if (db.PremiumShopsPriorities.Where(x => x.ShopID == PremiumShop.ShopID && x.CategoryID == catID).Count() > 0)
                {

                    BusinessLogicLayer.PremiumShopPriority obj = new PremiumShopPriority();
                    int maxPriority = obj.LevelOneWise_Get_MAX_PremiumShopsPriorityOrder(PremiumShop.CategoryID,PremiumShop.FranchiseID , fConnectionString);
                    if (PremiumShop.PriorityLevel == 0)
                    {
                        PremiumShop.PriorityLevel = maxPriority;
                    }
                    else if (PremiumShop.PriorityLevel > maxPriority)
                    {
                        PremiumShop.PriorityLevel = maxPriority;
                    }

                    string msg;
                    if (this.IsValidated(out msg, PremiumShop))
                    {
                        List<object> paramValues = new List<object>();
                        paramValues.Add(PremiumShop.ID);
                        paramValues.Add(FranchiseID);
                        paramValues.Add(PremiumShop.ShopID);
                        paramValues.Add(PremiumShop.CategoryID);
                        paramValues.Add(PremiumShop.PriorityLevel);
                        paramValues.Add(PremiumShop.IsActive);
                        paramValues.Add(DateTime.UtcNow.AddHours(5.5));
                        paramValues.Add(DBNull.Value);
                        paramValues.Add(BusinessLogicLayer.CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(Session["ID"])));
                        paramValues.Add(DBNull.Value);
                        paramValues.Add(BusinessLogicLayer.CommonFunctions.GetClientIP());
                        paramValues.Add("Net Browser");
                        paramValues.Add(DBNull.Value);
                        paramValues.Add(DataAccessLayer.Enumerators.DB_OPERATIONS.UPDATE);
                        paramValues.Add(0);

                        ViewBag.Messaage = obj.LevelOneWise_Insertupdate_PremiumShopsPriority(paramValues, DataAccessLayer.Enumerators.DB_OPERATIONS.UPDATE, fConnectionString);
                    }
                    else
                    {
                        ViewBag.Messaage = msg;
                    }

                }
                else
                {
                    ViewBag.Messaage = "Premium Shop Not Present....!";
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
        
        public JsonResult FranchiseCategoryList(Int32 FranchiseID)
        {
            //ModelLayer.Models.ViewModel.PremiumShopPriorityList ls = new ModelLayer.Models.ViewModel.PremiumShopPriorityList();
            List<CategoryForPremiumShopList> ls = new List<CategoryForPremiumShopList>();
            BusinessLogicLayer.PremiumShopPriority obj = new BusinessLogicLayer.PremiumShopPriority();
            DataTable dt = new DataTable();
            dt = obj.LevelOneWise_SelectFirstLevelCategoryFrom_PremiumShop(FranchiseID, System.Web.HttpContext.Current.Server);
            ls = (from n in dt.AsEnumerable()
                  select new CategoryForPremiumShopList
                  {
                      Name = n.Field<string>("Name"),
                      ID = n.Field<Int32>("ID"),
                  }).ToList();
            //BusinessLogicLayer.PremiumShopPriority obj = new BusinessLogicLayer.PremiumShopPriority();
            //DataTable dt = new DataTable();
            //dt = obj.PremiumShopPrimaryMarket_Call_Select_Procedure(FranchiseID, System.Web.HttpContext.Current.Server);

            ///*Select All the Shop By Franchise */
            //ls = (from n in dt.AsEnumerable()
            //                              select new CategoryForPremiumShopList
            //                              {
            //                                  ID = n.Field<int>("CategoryID"),
            //                                  Name = n.Field<string>("CategoryName"),                                              
            //                              }).OrderBy(x => x.Name).ToList();

           // return Json(ls, JsonRequestBehavior.AllowGet);
            return Json(ls.ToList().OrderBy(x => x.Name), JsonRequestBehavior.AllowGet);
        }

        public JsonResult FranchiseCategoryList_ForIndex(Int32 FranchiseID)
        {
           
            List<CategoryForPremiumShopList> ls = new List<CategoryForPremiumShopList>();           
            BusinessLogicLayer.PremiumShopPriority obj = new BusinessLogicLayer.PremiumShopPriority();
            DataTable dt = new DataTable();
            dt = obj.PremiumShopPrimaryMarket_Call_Select_Procedure(FranchiseID, System.Web.HttpContext.Current.Server);

            /*Select All the Shop By Franchise */
            ls = (from n in dt.AsEnumerable()
                  select new CategoryForPremiumShopList
                  {
                      ID = n.Field<int>("CategoryID"),
                      Name = n.Field<string>("CategoryName"),
                  }).ToList();

            
            return Json(ls.ToList().OrderBy(x => x.Name), JsonRequestBehavior.AllowGet);
        }
        
        public JsonResult CategoryShop(Int32 FranchiseID, Int64 CategoryID)
        {
            List<ShopList> ls = new List<ShopList>();
          
            BusinessLogicLayer.PremiumShopPriority obj = new BusinessLogicLayer.PremiumShopPriority();
            DataTable dt = new DataTable();
            dt = obj.LevelOneWise_SelectShopFrom_PremiumShop(FranchiseID, CategoryID, System.Web.HttpContext.Current.Server);
            ls = (from n in dt.AsEnumerable()
                  select new ShopList
                  {
                      Name = n.Field<string>("Name"),
                      ID = n.Field<Int64>("ID"),
                  }).ToList();
            // return Json(ls, JsonRequestBehavior.AllowGet);
            return Json(ls.ToList().OrderBy(x => x.Name), JsonRequestBehavior.AllowGet);
        }

        public ActionResult LevelOneWise_UpdatePremiumShopPriority(ModelLayer.Models.ViewModel.PremiumShopPriorityList ls, int FranchiseID, long CategoryID)
        {
            try
            {

                ViewBag.FranchiseList = new SelectList((from f in db.Franchises where f.ID != 1 select new { f.ID, f.BusinessDetail.Name }).ToList().OrderBy(x => x.Name), "ID", "Name");

                List<CategoryDetail> ldata = new List<CategoryDetail>();
                ldata.Add(new CategoryDetail { ID = 0, Name = "Select Category" });
                ViewBag.ShopID = new SelectList(ldata, "ID", "Name");

                BusinessLogicLayer.PremiumShopPriority obj = new BusinessLogicLayer.PremiumShopPriority();
                Int64 userID = CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(Session["ID"]));

                if (obj.LevelOneWise_UpdatePremiumShopPriority(ls, FranchiseID, CategoryID, userID, fConnectionString))
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

        #endregion

        /*Pradnyakar Badge
         * Premium Shop Market Primary Market
         * 07-03-2016         
         */
        #region ----- Primery First Level Category Entry
        public ActionResult GetFranchisePremiumShopMarket(int FranchiseID)
        {
            ModelLayer.Models.ViewModel.PremiumShopPriorityList ls = new ModelLayer.Models.ViewModel.PremiumShopPriorityList();

            try
            {
                ViewBag.FranchiseID = FranchiseID;
               // ViewBag.CategoryID = CategoryID;
                
                BusinessLogicLayer.PremiumShopPriority obj = new BusinessLogicLayer.PremiumShopPriority();
                DataTable dt = new DataTable();
                dt = obj.PremiumShopPrimaryMarket_Call_Select_Procedure(FranchiseID, System.Web.HttpContext.Current.Server);

                /*Select All the Shop By Franchise */
                ls.premiumShopPriorityList = (from n in dt.AsEnumerable()
                                              select new ModelLayer.Models.ViewModel.PremiumShopPriorityViewModel
                                              {
                                                  ID = n.Field<Int64>("ID"),
                                                 
                                                  CategoryID = n.Field<int>("CategoryID"),
                                                  CategoryName = n.Field<string>("CategoryName"),                                                
                                                  Priority = n.Field<int?>("PriorityLevel") == null ? 0 : Convert.ToInt32(n.Field<int?>("PriorityLevel")),
                                                  IsActive = n.Field<bool>("IsActive")
                                              }).OrderBy(x => x.Priority).ToList();

                //ViewBag.CategoryID = CategoryID;
                //ViewBag.Franchise = FranchiseID;
                return PartialView("_EditPremiumLevelOneCategory", ls);

            }
            catch
            {
                return PartialView("_EditPremiumLevelOneCategory", ls);
            }
        }

        [SessionExpire]
        [CustomAuthorize(Roles = "PremiumShopPriority/CanWrite")]
        public ActionResult PremiumShopPrimaryMarket_Create()
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

        [HttpPost]
        [SessionExpire]
        [CustomAuthorize(Roles = "PremiumShopPriority/CanWrite")]
        public ActionResult PremiumShopPrimaryMarket_Create(ModelLayer.Models.PremiumShopsPriority PremiumShop, int FranchiseID)
        {
            try
            {
                // TODO: Add insert logic here
                ViewBag.FranchiseID = new SelectList((from f in db.Franchises where f.ID != 1 select new { f.ID, f.BusinessDetail.Name }).ToList().OrderBy(x => x.Name), "ID", "Name", FranchiseID);
                Int64 shopID = PremiumShop.ShopID;
                Int64 catID = PremiumShop.CategoryID;
                PremiumShopPriority obj = new PremiumShopPriority();

                DataTable dt = new DataTable();
                List<ShopList> ls = new List<ShopList>();
                //dt1 = obj.PremiumShopPrimaryMarket_Call_Select_Procedure(FranchiseID, System.Web.HttpContext.Current.Server);
                //ls1 = (from n in dt1.AsEnumerable()
                //       select new ShopList
                //       {
                //           Name = n.Field<string>("CategoryName"),
                //           ID = n.Field<Int32>("CategoryID"),
                //       }).ToList();
                dt = obj.LevelOneWise_SelectFirstLevelCategoryFrom_PremiumShop(FranchiseID, System.Web.HttpContext.Current.Server);
                ls = (from n in dt.AsEnumerable()
                      select new ShopList
                      {
                          Name = n.Field<string>("Name"),
                          ID = n.Field<Int32>("ID"),
                      }).ToList();

                ViewBag.CategoryID = new SelectList(ls, "ID", "Name", catID);
                /*End Of Viewbang Category*/

              

                if (db.PremiumShopsPriorities.Where(x => x.CategoryID == catID && x.ShopID == shopID).Count() < 1)
                {
                    //BusinessLogicLayer.PremiumShopPriority objPremiumShopPriority = new PremiumShopPriority();

                    if (PremiumShop.PriorityLevel == 0)
                    {
                        PremiumShop.PriorityLevel = obj.Get_MAX_PremiumShopPrimaryMarketOrder(FranchiseID, fConnectionString);
                        PremiumShop.PriorityLevel += 1;
                    }

                    string msg;
                    if (this.IsValidated(out msg, PremiumShop))
                    {
                        List<object> paramValues = new List<object>();
                        paramValues.Add(DBNull.Value);
                        paramValues.Add(FranchiseID);                        
                        paramValues.Add(PremiumShop.CategoryID);
                        paramValues.Add(PremiumShop.PriorityLevel);
                        paramValues.Add(PremiumShop.IsActive);
                        paramValues.Add(DateTime.UtcNow.AddHours(5.5));
                        paramValues.Add(DBNull.Value);
                        paramValues.Add(BusinessLogicLayer.CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(Session["ID"])));
                        paramValues.Add(DBNull.Value);
                        paramValues.Add(BusinessLogicLayer.CommonFunctions.GetClientIP());
                        paramValues.Add("Net Browser");
                        paramValues.Add(DBNull.Value);
                        paramValues.Add(DataAccessLayer.Enumerators.DB_OPERATIONS.INSERT);
                        paramValues.Add(0);

                        ViewBag.Messaage = obj.Insertupdate_PremiumShopPrimaryMarket(paramValues, DataAccessLayer.Enumerators.DB_OPERATIONS.INSERT, fConnectionString);
                    }
                    else
                    {
                        ViewBag.Messaage = msg;
                    }

                    return View("PremiumShopPrimaryMarket_Create");


                }
                else
                {
                    ViewBag.Messaage = "Category is already Present with Parent Category....!";
                    return View("PremiumShopPrimaryMarket_Create");
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

        /// <summary>
        /// Edit Selected record get Request
        /// </summary>
        /// <param name="PremiumShop">Premium Shop Market Detail</param>
        /// <param name="FranchiseID">Franchsie ID</param>
        /// <returns></returns>
        [SessionExpire]
        [CustomAuthorize(Roles = "PremiumShopPriority/CanWrite")]
        public ActionResult PremiumShopPrimaryMarket_Edit(int id)
        {
            try
            {
                BusinessLogicLayer.PremiumShopPriority obj = new PremiumShopPriority();

                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                List<ModelLayer.Models.ViewModel.PremiumShopPrimaryMarket_ViewModel> premiumShopPrimaryMarket = new List<ModelLayer.Models.ViewModel.PremiumShopPrimaryMarket_ViewModel>();
                premiumShopPrimaryMarket = obj.SelectPremiumShopPrimaryMarket(id, System.Web.HttpContext.Current.Server);
                //Check only on record is selected or not
                if (premiumShopPrimaryMarket.Count < 1 || premiumShopPrimaryMarket.Count > 1)
                {
                    // no or more than one record record not found page
                    return HttpNotFound();
                }

                int FranchiseID = premiumShopPrimaryMarket.FirstOrDefault().FranchiseID;
                int CategoryID = premiumShopPrimaryMarket.FirstOrDefault().CategoryID;

                ViewBag.FranchiseID = new SelectList((from f in db.Franchises where f.ID != 1 select new { f.ID, f.BusinessDetail.Name }).ToList().OrderBy(x => x.Name),
                                                    "ID", "Name", FranchiseID);

                List<CategoryForPremiumShopList> ls = new List<CategoryForPremiumShopList>();
                BusinessLogicLayer.PremiumShopPriority objPremiumShopPriority = new BusinessLogicLayer.PremiumShopPriority();
                DataTable dt = new DataTable();
                dt = objPremiumShopPriority.LevelOneWise_SelectFirstLevelCategoryFrom_PremiumShop(FranchiseID, System.Web.HttpContext.Current.Server);
                ls = (from n in dt.AsEnumerable()
                      select new CategoryForPremiumShopList
                      {
                          Name = n.Field<string>("Name"),
                          ID = n.Field<Int32>("ID"),
                      }).ToList();

                ViewBag.CategoryID = new SelectList(ls.ToList(), "ID", "Name", CategoryID);

                return View(premiumShopPrimaryMarket.FirstOrDefault());
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

        /// <summary>
        /// Edit Selected record Post Request
        /// </summary>
        /// <param name="PremiumShop">Premium Shop Market Detail</param>
        /// <param name="FranchiseID">Franchsie ID</param>
        /// <returns></returns>
        [HttpPost]
        [SessionExpire]
        [CustomAuthorize(Roles = "PremiumShopPriority/CanWrite")]
        public ActionResult PremiumShopPrimaryMarket_Edit(ModelLayer.Models.ViewModel.PremiumShopPrimaryMarket_ViewModel PremiumShop, Int32 FranchiseID)
        {
            try
            {
                Int64 id = PremiumShop.ID;

                BusinessLogicLayer.PremiumShopPriority obj = new PremiumShopPriority();
                List<ModelLayer.Models.ViewModel.PremiumShopPrimaryMarket_ViewModel> premiumShopPrimaryMarket = new List<ModelLayer.Models.ViewModel.PremiumShopPrimaryMarket_ViewModel>();
                premiumShopPrimaryMarket = obj.SelectPremiumShopPrimaryMarket(id, System.Web.HttpContext.Current.Server);
                //Check only on record is selected or not
                if (premiumShopPrimaryMarket.Count < 1 || premiumShopPrimaryMarket.Count > 1)
                {
                    return HttpNotFound();
                }

                int CategoryID = premiumShopPrimaryMarket.FirstOrDefault().CategoryID;
                // Franchise List for DropDown
                ViewBag.FranchiseID = new SelectList((from f in db.Franchises where f.ID != 1 select new { f.ID, f.BusinessDetail.Name }).ToList().OrderBy(x => x.Name),
                                                    "ID", "Name", FranchiseID);

                List<CategoryForPremiumShopList> ls = new List<CategoryForPremiumShopList>();
                BusinessLogicLayer.PremiumShopPriority objPremiumShopPriority = new BusinessLogicLayer.PremiumShopPriority();
                DataTable dt = new DataTable();

                dt = objPremiumShopPriority.LevelOneWise_SelectFirstLevelCategoryFrom_PremiumShop(FranchiseID, System.Web.HttpContext.Current.Server);
                ls = (from n in dt.AsEnumerable()
                      select new CategoryForPremiumShopList
                      {
                          Name = n.Field<string>("Name"),
                          ID = n.Field<Int32>("ID"),
                      }).ToList();
                // Category List  for DropDown
                ViewBag.CategoryID = new SelectList(ls.ToList(), "ID", "Name", CategoryID);

                //BusinessLogicLayer.PremiumShopPriority obj = new PremiumShopPriority();
                //to get Max priority according to the shop Category List (third level Category)
                int maxPriority = obj.Get_MAX_PremiumShopPrimaryMarketOrder(FranchiseID, fConnectionString);
                if (PremiumShop.PriorityLevel == 0)
                {
                    PremiumShop.PriorityLevel = maxPriority;
                }
                else if (PremiumShop.PriorityLevel > maxPriority)
                {
                    PremiumShop.PriorityLevel = maxPriority;
                }

                string msg;
                // Validate given data
                if (this.IsValidatedPrimeryMarket(out msg, PremiumShop))
                {
                    List<object> paramValues = new List<object>();
                    paramValues.Add(PremiumShop.ID);
                    paramValues.Add(FranchiseID);
                    paramValues.Add(PremiumShop.CategoryID);
                    paramValues.Add(PremiumShop.PriorityLevel);
                    paramValues.Add(PremiumShop.IsActive);
                    paramValues.Add(DateTime.UtcNow.AddHours(5.5));
                    paramValues.Add(DBNull.Value);
                    paramValues.Add(BusinessLogicLayer.CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(Session["ID"])));
                    paramValues.Add(DBNull.Value);
                    paramValues.Add(BusinessLogicLayer.CommonFunctions.GetClientIP());
                    paramValues.Add("Net Browser");
                    paramValues.Add(DBNull.Value);
                    paramValues.Add(DataAccessLayer.Enumerators.DB_OPERATIONS.UPDATE);
                    paramValues.Add(0);
                    // Update operation and generate message accordingly
                    ViewBag.Messaage = obj.Insertupdate_PremiumShopPrimaryMarket(paramValues, DataAccessLayer.Enumerators.DB_OPERATIONS.UPDATE, fConnectionString);
                }
                else
                {
                    //generate message of validation
                    ViewBag.Messaage = msg;
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

        /// <summary>
        /// Save List of First Level Category sequence posted by from front end (Primary Market)
        /// </summary>
        /// <param name="ls">
        /// CategoryID 
        /// Order Sqeuence
        /// </param>
        /// <param name="Shop">Franchise</param>
        /// <returns></returns>
        public ActionResult UpdatePremiumShopPrimaryMarket(ModelLayer.Models.ViewModel.PremiumShopPriorityList ls, int FranchiseID)
        {
            try
            {

                ViewBag.FranchiseList = new SelectList((from f in db.Franchises where f.ID != 1 select new { f.ID, f.BusinessDetail.Name }).ToList().OrderBy(x => x.Name), "ID", "Name");
                
                BusinessLogicLayer.PremiumShopPriority obj = new BusinessLogicLayer.PremiumShopPriority();
                Int64 userID = CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(Session["ID"]));

                // Request to update list of first level category order of franchise
                if (obj.UpdatePremiumShopPrimaryMarket(ls,FranchiseID, userID, fConnectionString))
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

        /// <summary>
        /// Detail about selected Shop Primary Market (Franchise Level One Category)
        /// </summary>
        /// <param name="id">selected row ID</param>
        /// <returns></returns>
        [SessionExpire]
        [CustomAuthorize(Roles = "PremiumShopPriority/CanRead")]
        public ActionResult PremiumShopPrimaryMarket_Details(int id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                BusinessLogicLayer.PremiumShopPriority obj = new PremiumShopPriority();               
                List<ModelLayer.Models.ViewModel.PremiumShopPrimaryMarket_ViewModel> premiumShopPrimaryMarket = new List<ModelLayer.Models.ViewModel.PremiumShopPrimaryMarket_ViewModel>();
                // find the record of pass franchise Level one Category
                premiumShopPrimaryMarket = obj.SelectPremiumShopPrimaryMarket(id, System.Web.HttpContext.Current.Server);
                //Check only on record is selected or not
                if (premiumShopPrimaryMarket.Count < 1 || premiumShopPrimaryMarket.Count > 1)
                {
                    //no record or moret then one record redirect to not found page
                    return HttpNotFound();
                }

                int FranchiseID = premiumShopPrimaryMarket.FirstOrDefault().FranchiseID;
                int CategoryID = premiumShopPrimaryMarket.FirstOrDefault().CategoryID;

               
                  List<CategoryDetail> ldata = new List<CategoryDetail>();
                ldata.Add(new CategoryDetail { ID = 0, Name = "Select Category" });
                ViewBag.FranchiseID = db.Franchises.Where(x => x.ID == FranchiseID).FirstOrDefault().ContactPerson;
               
                ViewBag.CategoryID = db.Categories.Where(x => x.ID == CategoryID).FirstOrDefault().Name;

                return View(premiumShopPrimaryMarket.FirstOrDefault());
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

        /// <summary>
        /// Get Method to To Delete Premium Shop Primary Category
        /// </summary>
        /// <param name="id">Record ID</param>
        /// <returns></returns>
        [SessionExpire]
        [CustomAuthorize(Roles = "PremiumShopPriority/CanDelete")]
        public ActionResult premiumShopPrimaryMarket_Delete(int id)
        {
            try
            {
                //Check is id null
                if (id == null)
                {
                    //redirect to bad request page
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                BusinessLogicLayer.PremiumShopPriority obj = new PremiumShopPriority();
                
                List<ModelLayer.Models.ViewModel.PremiumShopPrimaryMarket_ViewModel> premiumShopPrimaryMarket = new List<ModelLayer.Models.ViewModel.PremiumShopPrimaryMarket_ViewModel>();
                premiumShopPrimaryMarket = obj.SelectPremiumShopPrimaryMarket(id, System.Web.HttpContext.Current.Server);
                //Check only on record is selected or not
                if (premiumShopPrimaryMarket.Count < 1 || premiumShopPrimaryMarket.Count > 1)
                {
                    //no record or moret then one record redirect to not found page
                    return HttpNotFound();
                }

                int FranchiseID = premiumShopPrimaryMarket.FirstOrDefault().FranchiseID;
                int CategoryID = premiumShopPrimaryMarket.FirstOrDefault().CategoryID;


                List<CategoryDetail> ldata = new List<CategoryDetail>();
                ldata.Add(new CategoryDetail { ID = 0, Name = "Select Category" });
                // select Franchise Name 
                ViewBag.FranchiseID = db.Franchises.Where(x => x.ID == FranchiseID).FirstOrDefault().ContactPerson;
                // select Category Name 
                ViewBag.CategoryID = db.Categories.Where(x => x.ID == CategoryID).FirstOrDefault().Name;

                return View(premiumShopPrimaryMarket.FirstOrDefault());
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

        /// <summary>
        /// Post Method to To Delete Premium Shop Primary Category
        /// </summary>
        /// <param name="id">Record ID</param>
        /// <returns></returns>
        [SessionExpire]
        [CustomAuthorize(Roles = "PremiumShopPriority/CanDelete")]
        [HttpPost, ActionName("premiumShopPrimaryMarket_Delete")]
        public ActionResult premiumShopPrimaryMarket_DeleteConfirmed(int id)
        {
            try
            {
                //PremiumShopsPriority PremiumShopsPriority = db.PremiumShopsPriorities.Find(id);
                //if (PremiumShopsPriority == null)
                //{
                //    return HttpNotFound();
                //}
                BusinessLogicLayer.PremiumShopPriority obj = new PremiumShopPriority();

                List<ModelLayer.Models.ViewModel.PremiumShopPrimaryMarket_ViewModel> premiumShopPrimaryMarket = new List<ModelLayer.Models.ViewModel.PremiumShopPrimaryMarket_ViewModel>();
                premiumShopPrimaryMarket = obj.SelectPremiumShopPrimaryMarket(id, System.Web.HttpContext.Current.Server);
                //Check only on record is selected or not
                if (premiumShopPrimaryMarket.Count < 1 || premiumShopPrimaryMarket.Count > 1)
                {
                    //no record or moret then one record redirect to not found page
                    return HttpNotFound();
                }

                int FranchiseID = premiumShopPrimaryMarket.FirstOrDefault().FranchiseID;
                int CategoryID = premiumShopPrimaryMarket.FirstOrDefault().CategoryID;


                List<CategoryDetail> ldata = new List<CategoryDetail>();
                ldata.Add(new CategoryDetail { ID = 0, Name = "Select Category" });
                // select Franchise Name 
                ViewBag.FranchiseID = db.Franchises.Where(x => x.ID == FranchiseID).FirstOrDefault().ContactPerson;
                // select Category Name 
                ViewBag.CategoryID = db.Categories.Where(x => x.ID == CategoryID).FirstOrDefault().Name;

                // Declare to Collect all parameter in paramValues variable to perform delete opration at backend (at database storeprocedure)
                List<object> paramValues = new List<object>();
                //Record id to delete
                paramValues.Add(id);
                //Operation to perform
                paramValues.Add(DataAccessLayer.Enumerators.DB_OPERATIONS.DELETE.ToString());
                //Output operation
                paramValues.Add(DBNull.Value);

                int resultCode = 0;
       
             
               
                    //generate message to delete level one/ primery category of shop
                    ViewBag.Messaage = obj.Delete_PremiumShopPrimaryMarket(paramValues, DataAccessLayer.Enumerators.DB_OPERATIONS.UPDATE, out resultCode, fConnectionString);
               
                
                if (resultCode == 3)
                    return RedirectToAction("Index");
                else
                    return View(premiumShopPrimaryMarket.FirstOrDefault());


                
               

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

        #endregion

        #region ----- Second Level Entry For Premium Shop -----


        [SessionExpire]
        [CustomAuthorize(Roles = "PremiumShopPriority/CanWrite")]
        public ActionResult PremiumShopSecondLevelCategory_Create()
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

        [HttpPost]
        [SessionExpire]
        [CustomAuthorize(Roles = "PremiumShopPriority/CanWrite")]
        public ActionResult PremiumShopSecondLevelCategory_Create(ModelLayer.Models.PremiumShopsPriority PremiumShop, int FranchiseID)
        {
            try
            {
                // TODO: Add insert logic here
                List<CategoryDetail> ldata = new List<CategoryDetail>();
                ldata.Add(new CategoryDetail { ID = 0, Name = "Select Category" });
                //List of Franchise For dropdown
                ViewBag.FranchiseID = new SelectList((from f in db.Franchises where f.ID != 1 select new { f.ID, f.BusinessDetail.Name }).ToList().OrderBy(x => x.Name), "ID", "Name", FranchiseID);

                Int64 shopID = PremiumShop.ShopID;
                Int64 catID = PremiumShop.CategoryID;

                //List of Active Shop  of Selected Franchise for dropdown

                List<ShopList> ls = new List<ShopList>();
                BusinessLogicLayer.ReadConfig rcKey = new BusinessLogicLayer.ReadConfig(System.Web.HttpContext.Current.Server);
                BusinessLogicLayer.PremiumShopPriority obj = new BusinessLogicLayer.PremiumShopPriority();
                DataTable dt = new DataTable();
                dt = obj.SelectShopFrom_PremiumShop(FranchiseID, 0, System.Web.HttpContext.Current.Server);
                ls = (from n in dt.AsEnumerable()
                      select new ShopList
                      {
                          Name = n.Field<string>("Name"),
                          ID = n.Field<Int64>("ID"),
                      }).ToList();

                ViewBag.ShopID = new SelectList(ls.ToList(), "ID", "Name", shopID);


                //List of Category of Shop according to the assign Plan
                List<ShopList> catals = new List<ShopList>();              
                DataTable catdt = new DataTable();
                catdt = obj.Select_SecondLevelCategoryBySchop(FranchiseID, shopID, System.Web.HttpContext.Current.Server);
                catals = (from n in catdt.AsEnumerable()
                      select new ShopList
                      {
                          Name = n.Field<string>("Name"),
                          ID = n.Field<int>("ID")
                      }).ToList();

                ViewBag.CategoryID = new SelectList(catals.ToList(), "ID", "Name", catID);



                // Check weather the category entry is present with shop or not 
                // To avoid duplicate entry of shop and category
                if (db.PremiumShopsPriorities.Where(x => x.CategoryID == catID && x.ShopID == shopID).Count() < 1)
                {
                    //BusinessLogicLayer.PremiumShopPriority obj = new PremiumShopPriority();

                    if (PremiumShop.PriorityLevel == 0)
                    {
                        //to get Max priority according to the shop Category List (third level Category)
                        PremiumShop.PriorityLevel = obj.LevelTwoWise_Get_MAX_PremiumShopsPriorityOrder(PremiumShop.FranchiseID, PremiumShop.ShopID, 2, fConnectionString);
                        //increase priority by 1 for next highest priority 
                        PremiumShop.PriorityLevel += 1;
                    }

                    string msg;
                    //check validation of data for new insert record
                    if (this.IsValidated(out msg, PremiumShop))
                    {
                        List<object> paramValues = new List<object>();
                        paramValues.Add(DBNull.Value);
                        paramValues.Add(FranchiseID);
                        paramValues.Add(PremiumShop.ShopID);
                        paramValues.Add(PremiumShop.CategoryID);
                        paramValues.Add(PremiumShop.PriorityLevel);
                        paramValues.Add(PremiumShop.IsActive);
                        paramValues.Add(DateTime.UtcNow.AddHours(5.5));
                        paramValues.Add(DBNull.Value);
                        paramValues.Add(BusinessLogicLayer.CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(Session["ID"])));
                        paramValues.Add(DBNull.Value);
                        paramValues.Add(BusinessLogicLayer.CommonFunctions.GetClientIP());
                        paramValues.Add("Net Browser");
                        paramValues.Add(DBNull.Value);
                        paramValues.Add(DataAccessLayer.Enumerators.DB_OPERATIONS.INSERT);
                        paramValues.Add(0);

                        //call businesslogic layer for insert given parameter values
                        //get messages according to the operation perform
                        ViewBag.Messaage = obj.LevelTwoWise_Insertupdate_PremiumShopsPriority(paramValues, DataAccessLayer.Enumerators.DB_OPERATIONS.INSERT, fConnectionString);
                    }
                    else
                    {
                        ViewBag.Messaage = msg;
                    }

                    return View(PremiumShop);


                }
                else
                {
                    ViewBag.Messaage = "Shop Menu already Present....!";
                    return View(PremiumShop);
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

        /// <summary>
        /// To Correction in third level category of PremiumShop in Priority Order or else
        /// </summary>         
        /// <returns></returns>
        [SessionExpire]
        [CustomAuthorize(Roles = "PremiumShopPriority/CanWrite")]
        public ActionResult PremiumShopSecondLevelCategory_Edit(int id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                ModelLayer.Models.PremiumShopsPriority premiumShopPriority = db.PremiumShopsPriorities.Find(id);
                if (premiumShopPriority == null)
                {
                    return HttpNotFound();
                }


                List<CategoryDetail> ldata = new List<CategoryDetail>();
                ldata.Add(new CategoryDetail { ID = 0, Name = "Select Category" });
                //List of Franchise For dropdown
                ViewBag.FranchiseID = new SelectList((from f in db.Franchises where f.ID != 1 select new { f.ID, f.BusinessDetail.Name }).ToList().OrderBy(x => x.Name),
                                                    "ID", "Name", premiumShopPriority.FranchiseID);
                //List of Active Shop  of Selected Franchise for dropdown
                List<ShopList> ls = new List<ShopList>();
                BusinessLogicLayer.ReadConfig rcKey = new BusinessLogicLayer.ReadConfig(System.Web.HttpContext.Current.Server);
                BusinessLogicLayer.PremiumShopPriority objPremiumShopPriority = new BusinessLogicLayer.PremiumShopPriority();
                DataTable dt = new DataTable();
                dt = objPremiumShopPriority.SelectShopFrom_PremiumShop(premiumShopPriority.FranchiseID, premiumShopPriority.ShopID, System.Web.HttpContext.Current.Server);
                ls = (from n in dt.AsEnumerable()
                      select new ShopList
                      {
                          Name = n.Field<string>("Name"),
                          ID = n.Field<Int64>("ID"),
                      }).ToList();

                ViewBag.ShopID = new SelectList(ls.ToList(), "ID", "Name", premiumShopPriority.ShopID);


                //List of Category of Shop according to the assign Plan
                List<ShopList> catals = new List<ShopList>();
                DataTable catdt = new DataTable();
                catdt = objPremiumShopPriority.Select_SecondLevelCategoryBySchop(premiumShopPriority.FranchiseID, premiumShopPriority.ShopID, System.Web.HttpContext.Current.Server);
                catals = (from n in catdt.AsEnumerable()
                          select new ShopList
                          {
                              Name = n.Field<string>("Name"),
                              ID = n.Field<int>("ID")
                          }).ToList();

                ViewBag.CategoryID = new SelectList(catals.ToList(), "ID", "Name", premiumShopPriority.CategoryID);
                return View(premiumShopPriority);
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

        /// <summary>
        /// To Correction in third level category of PremiumShop in Priority Order or else
        /// </summary>
        /// <param name="PremiumShop">Premium shop Detail</param>
        /// <param name="FranchiseID">Franchise ID</param>
        /// <returns></returns>
        [HttpPost]
        [SessionExpire]
        [CustomAuthorize(Roles = "PremiumShopPriority/CanWrite")]
        public ActionResult PremiumShopSecondLevelCategory_Edit(ModelLayer.Models.PremiumShopsPriority PremiumShop, Int32 FranchiseID, HttpPostedFileBase file)
        {
            try
            {

                ModelLayer.Models.PremiumShopsPriority lPremiumShop = db.PremiumShopsPriorities.Find(PremiumShop.ID);

                Int64 catID = PremiumShop.CategoryID;

                //List of Franchise For dropdown  
                ViewBag.FranchiseID = new SelectList((from f in db.Franchises where f.ID != 1 select new { f.ID, f.BusinessDetail.Name }).ToList().OrderBy(x => x.Name),
                                                        "ID", "Name", FranchiseID);

                Int64 shopID = PremiumShop.ShopID;
                //List of Active Shop  of Selected Franchise for dropdown
                List<ShopList> ls = new List<ShopList>();
                BusinessLogicLayer.ReadConfig rcKey = new BusinessLogicLayer.ReadConfig(System.Web.HttpContext.Current.Server);
                BusinessLogicLayer.PremiumShopPriority objPremiumShopPriority = new BusinessLogicLayer.PremiumShopPriority();
                DataTable dt = new DataTable();
                dt = objPremiumShopPriority.SelectShopFrom_PremiumShop(FranchiseID, 0, System.Web.HttpContext.Current.Server);
                ls = (from n in dt.AsEnumerable()
                      select new ShopList
                      {
                          Name = n.Field<string>("Name"),
                          ID = n.Field<Int64>("ID"),
                      }).ToList();

                ViewBag.ShopID = new SelectList(ls.ToList(), "ID", "Name", shopID);


                //List of Category of Shop according to the assign Plan
                List<ShopList> catals = new List<ShopList>();
                DataTable catdt = new DataTable();
                catdt = objPremiumShopPriority.Select_SecondLevelCategoryBySchop(FranchiseID, shopID, System.Web.HttpContext.Current.Server);
                catals = (from n in catdt.AsEnumerable()
                          select new ShopList
                          {
                              Name = n.Field<string>("Name"),
                              ID = n.Field<int>("ID")
                          }).ToList();

                ViewBag.CategoryID = new SelectList(catals.ToList(), "ID", "Name", catID);


                // Check weather the category entry is present with shop or not 
                // To avoid duplicate entry of shop and category
                if (db.PremiumShopsPriorities.Where(x => x.ShopID == PremiumShop.ShopID && x.CategoryID == catID).Count() > 0)
                {

                    BusinessLogicLayer.PremiumShopPriority obj = new PremiumShopPriority();
                    //to get Max priority according to the shop Category List (third level Category)
                    int maxPriority = obj.LevelTwoWise_Get_MAX_PremiumShopsPriorityOrder(PremiumShop.FranchiseID, PremiumShop.ShopID, 2, fConnectionString);
                    if (PremiumShop.PriorityLevel == 0)
                    {
                        PremiumShop.PriorityLevel = maxPriority;
                    }
                    else if (PremiumShop.PriorityLevel > maxPriority)
                    {
                        PremiumShop.PriorityLevel = maxPriority;
                    }

                    string msg;
                    //check validation of data for new insert record
                    if (this.IsValidated(out msg, PremiumShop))
                    {
                        List<object> paramValues = new List<object>();
                        paramValues.Add(PremiumShop.ID);
                        paramValues.Add(FranchiseID);
                        paramValues.Add(PremiumShop.ShopID);
                        paramValues.Add(PremiumShop.CategoryID);
                        paramValues.Add(PremiumShop.PriorityLevel);
                        paramValues.Add(PremiumShop.IsActive);
                        paramValues.Add(DateTime.UtcNow.AddHours(5.5));
                        paramValues.Add(lPremiumShop.CreateDate);
                        paramValues.Add(lPremiumShop.CreateBy);
                        paramValues.Add(BusinessLogicLayer.CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(Session["ID"])));
                        paramValues.Add(BusinessLogicLayer.CommonFunctions.GetClientIP());
                        paramValues.Add("Net Browser");
                        paramValues.Add(DBNull.Value);
                        paramValues.Add(DataAccessLayer.Enumerators.DB_OPERATIONS.UPDATE);
                        paramValues.Add(0);

                        //call businesslogic layer for insert given parameter values
                        //get messages according to the operation perform
                        ViewBag.Messaage = obj.LevelTwoWise_Insertupdate_PremiumShopsPriority(paramValues, DataAccessLayer.Enumerators.DB_OPERATIONS.UPDATE, fConnectionString);
                    }

                }
                else
                {
                    ViewBag.Messaage = "Premium Shop Not Present....!";
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


        public JsonResult ShopSecondCategoryList(int FranchiseID, Int64 ShopID)
        {
            List<ShopList> ls = new List<ShopList>();
            PremiumShopPriority bll = new PremiumShopPriority();
            DataTable dt = new DataTable();
            dt = bll.Select_SecondLevelCategoryBySchop(FranchiseID, ShopID, System.Web.HttpContext.Current.Server);
            ls = (from n in dt.AsEnumerable()
                  select new ShopList
                  {
                      Name = n.Field<string>("Name"),
                      ID = n.Field<int>("ID")
                  }).ToList();

            return Json(ls.ToList().OrderBy(x => x.Name), JsonRequestBehavior.AllowGet);
        }

        public JsonResult ShopSecondCategoryList_DDL(int FranchiseID, Int64 ShopID)
        {
            List<ShopList> ls = new List<ShopList>();
            PremiumShopPriority bll = new PremiumShopPriority();
            DataTable dt = new DataTable();
            dt = bll.LevelTwoWise_Call_Select_Procedure(ShopID, 0, System.Web.HttpContext.Current.Server);
            ls = (from n in dt.AsEnumerable()
                  select new ShopList
                  {
                      Name = n.Field<string>("CategoryName"),
                      ID = n.Field<int>("CategoryID")
                  }).ToList();

            return Json(ls.ToList().OrderBy(x => x.Name), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// CategoryList for partial page i.e. _EditPremiumShopPriority
        /// </summary>
        /// <param name="ShopID">ShopID</param>
        /// <param name="CategoryID">CategoryID</param>
        /// <returns></returns>
        public ActionResult GetShopSecondLevelCategoryList(Int64 ShopID, Int64 CategoryID)
        {
            ModelLayer.Models.ViewModel.PremiumShopPriorityList ls = new ModelLayer.Models.ViewModel.PremiumShopPriorityList();

            try
            {
                ViewBag.Shop = ShopID;
                BusinessLogicLayer.ReadConfig rcKey = new BusinessLogicLayer.ReadConfig(System.Web.HttpContext.Current.Server);
                BusinessLogicLayer.PremiumShopPriority obj = new BusinessLogicLayer.PremiumShopPriority();
                DataTable dt = new DataTable();
                dt = obj.LevelTwoWise_Call_Select_Procedure(ShopID, CategoryID, System.Web.HttpContext.Current.Server);

                /*Select All the Shop By Franchise */
                ls.premiumShopPriorityList = (from n in dt.AsEnumerable()
                                              select new ModelLayer.Models.ViewModel.PremiumShopPriorityViewModel
                                              {
                                                  ID = n.Field<Int64>("ID"),
                                                  ShopID = n.Field<Int64>("ShopID"),
                                                  CategoryID = n.Field<int>("CategoryID"),
                                                  CategoryName = n.Field<string>("CategoryName"),
                                                  ShopName = n.Field<string>("ShopName"),

                                                  Priority = n.Field<int?>("PriorityLevel") == null ? 0 : Convert.ToInt32(n.Field<int?>("PriorityLevel")),
                                                  IsActive = n.Field<bool>("IsActive")
                                              }).OrderBy(x => x.Priority).ToList();


                return PartialView("_EditShopSecondLevelCategoryPriority", ls);

            }
            catch
            {
                return PartialView("_EditPremiumShopPriority", ls);
            }
        }


        public ActionResult LevelTwoPriority(ModelLayer.Models.ViewModel.PremiumShopPriorityList ls, Int64 Shop)
        {
            try
            {
                /*List of Franchise for dropdown*/
                ViewBag.FranchiseList = new SelectList((from f in db.Franchises where f.ID != 1 select new { f.ID, f.BusinessDetail.Name }).ToList().OrderBy(x => x.Name), "ID", "Name");

                List<CategoryDetail> ldata = new List<CategoryDetail>();
                ldata.Add(new CategoryDetail { ID = 0, Name = "Select Category" });
                /*Empty Shop List for dropdown*/
                ViewBag.ShopID = new SelectList(ldata, "ID", "Name");

                BusinessLogicLayer.PremiumShopPriority obj = new BusinessLogicLayer.PremiumShopPriority();
                /*To get Login user ID and fetch personal detail*/
                Int64 userID = CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(Session["ID"]));
                /**/
                if (obj.LevelTwoWise_UpdatePremiumShopPriority(ls, Shop, userID, fConnectionString))
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

        #endregion

    }


    public class CategoryForPremiumShopList
    {

        public int ID { get; set; }
        public string Name { get; set; }

    }

}