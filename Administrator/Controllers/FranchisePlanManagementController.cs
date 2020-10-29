using ModelLayer.Models;
using ModelLayer.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList.Mvc;
using PagedList;
using System.Net;
using System.Transactions;
using BusinessLogicLayer;
using System.Text;
using System.Data;
using System.Web.Configuration;
using Administrator.Models;

namespace Administrator.Controllers
{
    [SessionExpire]
    public class FranchisePlanManagementController : Controller
    {

        StringBuilder errStr = new StringBuilder("=====================================================================================" +
            Environment.NewLine
            + "ErrorLog Controller : FranchisePlanManagement" + Environment.NewLine);


        private EzeeloDBContext db = new EzeeloDBContext();
        //
        // GET: /FranchisePlanManagement/
        public ActionResult Index()
        {
            try
            {
                var ldata = db.PlanBinds.Where(x => x.Level == 1).ToList();
                ViewBag.PlanCode = db.Plans.ToList();
                return View(ldata);
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Message", "Sorry! Problem in Generate Index view!!");
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[FranchisePlanManagement][GET:Index]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
                return View();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Message", "Sorry! Problem in Generate Index view!!");
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[FranchisePlanManagement][GET:Index]",
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
                return View();
            }
           
        }

        public ActionResult Details(int? id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                PlanBind planBind = db.PlanBinds.Find(id);

                ViewBag.MasterPlan = new SelectList(db.Plans.Where(x => x.ID == planBind.PlanID), "ID", "ShortName", planBind.PlanID);
               
                if (planBind == null)
                {
                    return HttpNotFound();
                }

                SelectPlanBindCategoryUsingProcedure objPlanBind = new BusinessLogicLayer.SelectPlanBindCategoryUsingProcedure();
                PlanBindManagement lp = new PlanBindManagement();

                lp = objPlanBind.SelectPlanBindCategoryForUpdate(System.Web.HttpContext.Current.Server, Convert.ToInt16(id));

                lp.type = planBind.Type;

                //lp.planCommission = ls;

                return View(lp);
            }
            catch (Exception ex)
            {
                errStr.Append("Method Name[Http Request] :- Edit[HttpGet]" + Environment.NewLine +
                               "ON Dated" + DateTime.UtcNow.AddHours(5.30) + Environment.NewLine +
                                   ex.Message.ToString() + Environment.NewLine +
                         "====================================================================================="
                               );
                //ViewBag.Message = "Sorry! Problem in customer registration!!";
                ModelState.AddModelError("Message", "Sorry! Problem in customer Plan Bind Management!!");
                ErrorLog.ErrorLogFile("Unable to retrive data " + Environment.NewLine + errStr.ToString()
                    , ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);

                return View();
            }
        }
        
        public ActionResult Create()
        {
            try
            {
                PlanBindManagement pbm = new PlanBindManagement();
                List<SelectListItem> lData = new List<SelectListItem>();
                lData.Add(new SelectListItem { Text = string.Empty, Value = "0" });

                ViewBag.MasterPlan = new SelectList(db.Plans.Where(x => x.PlanCode.Substring(0, 4) == "GBFR" && x.IsActive==true).ToList(), "ID", "ShortName");

                List<PlanCommission> pc = new List<PlanCommission>();
                List<Category> catList = new List<Category>();

                catList = db.Categories.Where(x => x.Level == 1 && x.IsActive == true).ToList();
                pc = (from n in catList
                      select new PlanCommission
                      {
                          CategoryID = n.ID,
                          CategoryName = n.Name,
                          CategoryLevel = n.Level,
                          isSelect = false

                      }).ToList();

                pbm.planCommission = pc;

                return View(pbm);
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Message", "Sorry! Problem in customer Plan Bind Management!!");
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[FranchisePlanManagement][GET:Create]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
                return View();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Message", "Sorry! Problem in customer Plan Bind Management!!");
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[FranchisePlanManagement][GET:Create]",
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
                return View();
            }
        }
        
        [HttpPost]
        public ActionResult Create(PlanBindManagement lpbm, int MasterPlan)
        {
            try
            {
                //included = lpbm.type;

                //List<SelectListItem> lData = new List<SelectListItem>();
                //lData.Add(new SelectListItem { Text = string.Empty, Value = "0" });
                ViewBag.MasterPlan = new SelectList(db.Plans.Where(x => x.IsActive == true).ToList(), "ID", "ShortName", MasterPlan);

                if (lpbm.type < 1 || lpbm.type > 2)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                PlanBind lData = db.PlanBinds.Where(x => x.PlanID == MasterPlan).FirstOrDefault();
                if (lData == null)
                {                    
                    using (TransactionScope ts = new TransactionScope())
                    {
                        try
                        {
                            int identityValue = this.InsertPlanBind(MasterPlan,lpbm.type); //lplanBind.ID;

                            for (int i = 0; i < lpbm.planCommission.Count(); i++)
                            {
                                if (lpbm.planCommission[i].isSelect)
                                {
                                    InsertPlanBindCategory(identityValue, lpbm.planCommission[i].CategoryID);
                                }
                            }
                            // Transaction complete
                            ts.Complete();
                            // Clear model state
                            ModelState.Clear();
                            
                            ModelState.AddModelError("Message", "Done! Plan Bind Successfully Done!!");
                        }
                        catch (Exception exception)
                        {
                            errStr.Append("Method Create[Http Request] :- Create[HttpPost]" + Environment.NewLine +
                                "ON Dated" + DateTime.UtcNow.AddHours(5.30) + Environment.NewLine +
                                    exception.Message.ToString() + Environment.NewLine +
                          "====================================================================================="
                                );
                            //ViewBag.Message = "Sorry! Problem in customer registration!!";
                            ModelState.AddModelError("Message", "Sorry! Problem in Plan Bind!!");
                            // Rollback transaction
                            ts.Dispose();

                            ErrorLog.ErrorLogFile("Unable to Insert Data Complete Transaction RollBack " + Environment.NewLine + errStr.ToString()
                                , ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
                        }

                    }
                }
                else
                {
                    //ViewBag.Message = "Plan Already Exists...!";
                    ModelState.AddModelError("Message", "Plan Already Exists...!");
                    ViewBag.MasterPlan = new SelectList(db.Plans.ToList(), "ID", "PlanCode");
                }

                return View(lpbm);
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Message", "Sorry! Problem in customer Plan Bind Management!!");
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[FranchisePlanManagement][POST:Create]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
                return View();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Message", "Sorry! Problem in customer Plan Bind Management!!");
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[FranchisePlanManagement][POST:Create]",
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
                return View();
            }
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PlanBindCategory planBindCategory = db.PlanBindCategories.Find(id);
            if (planBindCategory == null)
            {
                return HttpNotFound();
            }

            ViewBag.MasterPlan = new SelectList(db.Plans.Where(x => x.ID == planBindCategory.PlanBind.PlanID), "ID", "ShortName", planBindCategory.PlanBind.PlanID);
            ViewBag.CategoryID = new SelectList(db.Categories.Where(x => x.ID == planBindCategory.CategoryID), "ID", "Name", planBindCategory.CategoryID);
            ViewBag.PlanBindID = planBindCategory.PlanBindID;
            ViewBag.PlanID = planBindCategory.PlanBind.PlanID;
            return View(planBindCategory);
        }

        [HttpPost]
        public ActionResult Edit(PlanBindCategory planBindCategory,Int32 PlanID)
        {
            try
            {
                if (db.PlanBindCategories.Where(x => x.ID == planBindCategory.ID).Count() < 1)
                {
                    ViewBag.lblError = "Record Not Found..!!";
                    return View(planBindCategory);
                }
                ViewBag.MasterPlan = new SelectList(db.Plans.Where(x => x.ID == PlanID), "ID", "ShortName", PlanID);
                ViewBag.CategoryID = new SelectList(db.Categories.Where(x => x.ID == planBindCategory.CategoryID), "ID", "Name", planBindCategory.CategoryID);

                PlanBindCategory lplanBindCategory = db.PlanBindCategories.Find(planBindCategory.ID);
                planBindCategory.CreateDate = lplanBindCategory.CreateDate;
                planBindCategory.CreateBy = lplanBindCategory.CreateBy;
                planBindCategory.ModifyDate = DateTime.UtcNow.AddHours(5.5);
                planBindCategory.ModifyBy = CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(Session["ID"]));
                planBindCategory.NetworkIP = CommonFunctions.GetClientIP();
                planBindCategory.DeviceType = string.Empty;
                planBindCategory.DeviceID = string.Empty;

                // TryUpdateModel(bank);

                db.Entry(lplanBindCategory).CurrentValues.SetValues(planBindCategory);
                db.SaveChanges();

                ViewBag.Messaage = "Plan Bind Category Detail Modified Successfully";
                return View(planBindCategory);
            }
            catch
            {
                ViewBag.Messaage = "Plan Bind Category Detail Modified Successfully";
                return View(planBindCategory);
            }
        }

        public ActionResult AddMoreCategory(int id)
        {
            ViewBag.MasterPlan = new SelectList(db.Plans.Where(x => x.ID == id && x.PlanCode.Substring(0, 4).ToUpper() == "GBFR").ToList(), "ID", "ShortName", id);
            ViewBag.Category = new SelectList(NotInPlanCategoryList(id), "ID", "Name");
            ViewBag.PlanID = id;

            return View();
        }

        [HttpPost]
        public ActionResult AddMoreCategory(Int32? MasterPlan, Int32? Category, Int32 PlanID)
        {
            ViewBag.MasterPlan = new SelectList(db.Plans.Where(x => x.IsActive == true && x.PlanCode.Substring(0, 4).ToUpper() == "GBFR").ToList(), "ID", "ShortName");
            ViewBag.Category = new SelectList(NotInPlanCategoryList(PlanID), "ID", "Name");

            if(MasterPlan == null)
                ViewBag.Message = "Please Select Plan";
            else if (Category == null)
                ViewBag.Message = "Please select cataegory";
            else
            {
                var PlanBindID = (from n in db.Plans
                                   join m in db.PlanBinds on n.ID equals m.PlanID
                                   where n.IsActive == true && n.PlanCode.Substring(0, 4).ToUpper() == "GBFR" && n.ID == MasterPlan
                                   && m.IsActive == true
                                   select new
                                   {
                                       ID = m.ID
                                   }).FirstOrDefault();

                if (PlanBindID != null)
                {
                    if (db.PlanBindCategories.Where(x => x.PlanBindID == PlanBindID.ID && x.CategoryID == Category).Count() < 1)
                    {
                        InsertPlanBindCategory(Convert.ToInt32(PlanBindID.ID), Convert.ToInt32(Category));
                        ViewBag.Message = "Category Successfully Bind with Plan";
                    }
                    else
                    {
                        ViewBag.Message = "Category Already Bind with Plan..!!";
                    }
                }
                else
                {
                    ViewBag.Message = "Selected Plan is not Valid...!!";
                }
            }
            return View(); 
        }


        public ActionResult Delete(int? id)
        {
            try
            
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                PlanBind planBind = db.PlanBinds.Find(id);
                ViewBag.MasterPlan = new SelectList(db.Plans.Where(x => x.ID == planBind.PlanID), "ID", "ShortName", planBind.PlanID);

                List<PlanCategoryCharge> lPlanCategoryCharge = new List<PlanCategoryCharge>();
                lPlanCategoryCharge = db.PlanCategoryCharges.Where(x => x.PlanID == planBind.PlanID).ToList();

                if (lPlanCategoryCharge.Count() > 0)
                {
                    ViewBag.pMessage = "Sorry...! This Plan Binded Categories chages has been decided therefor It can not be deleted";
                    //return View();
                }

                if (planBind == null)
                {
                    return HttpNotFound();
                }

                SelectPlanBindCategoryUsingProcedure objPlanBind = new BusinessLogicLayer.SelectPlanBindCategoryUsingProcedure();
                PlanBindManagement lp = new PlanBindManagement();

                lp = objPlanBind.SelectPlanBindCategoryForUpdate(System.Web.HttpContext.Current.Server, Convert.ToInt16(id));
                lp.type = planBind.Type;


                return View(lp);
            }
            catch (Exception ex)
            {
                errStr.Append("Method Name[Http Request] :- Delete[HttpGet]" + Environment.NewLine +
                               "ON Dated" + DateTime.UtcNow.AddHours(5.30) + Environment.NewLine +
                                   ex.Message.ToString() + Environment.NewLine +
                         "====================================================================================="
                               );
                //ViewBag.Message = "Sorry! Problem in customer registration!!";
                ModelState.AddModelError("Message", "Sorry! Problem in customer Plan Bind Management!!");
                ErrorLog.ErrorLogFile("Unable to retrive data " + Environment.NewLine + errStr.ToString()
                    , ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);

                return View();
            }

        }

        [HttpPost, ActionName("Delete")]
        public ActionResult DeletConfirm(int id,int MasterPlan,PlanBindManagement lpbm)
        {
            try
            {

                ViewBag.MasterPlan = new SelectList(db.Plans.ToList(), "ID", "ShortName", MasterPlan);

                List<PlanCategoryCharge> lPlanCategoryCharge = new List<PlanCategoryCharge>();
                lPlanCategoryCharge = db.PlanCategoryCharges.Where(x => x.PlanID == MasterPlan).ToList();

                PlanBind planBind = db.PlanBinds.Find(id);
                ViewBag.MasterPlan = new SelectList(db.Plans.Where(x => x.ID == planBind.PlanID), "ID", "ShortName", planBind.PlanID);


                SelectPlanBindCategoryUsingProcedure objPlanBind = new BusinessLogicLayer.SelectPlanBindCategoryUsingProcedure();
                PlanBindManagement lp = new PlanBindManagement();

                lp = objPlanBind.SelectPlanBindCategoryForUpdate(System.Web.HttpContext.Current.Server, Convert.ToInt16(id));
                lp.type = planBind.Type;

                if (lPlanCategoryCharge.Count() > 0)
                {
                    ViewBag.pMessage = "Sorry...! This Plan Binded Categories chages has been decided therefor It can not be deleted";
                    return View(lpbm);
                }

                using (TransactionScope ts = new TransactionScope())
                {
                    try
                    {
                        PlanBind lData = new PlanBind();
                        lData = db.PlanBinds.Where(x => x.PlanID == MasterPlan).FirstOrDefault();

                        List<PlanBindCategory> planBindCategory = db.PlanBindCategories.Where(x => x.PlanBindID == lData.ID).ToList();
                        db.PlanBindCategories.RemoveRange(planBindCategory);
                        db.SaveChanges();


                        db.PlanBinds.Remove(lData);
                        db.SaveChanges();

                        // Transaction complete
                        ts.Complete();
                        // Clear model state
                        ModelState.Clear();

                        ModelState.AddModelError("Message", "Done! Plan Bind Successfully Done!!");

                    }
                    catch (Exception exception)
                    {
                        errStr.Append("Method Delete[Http Request] :- Delete[HttpPost]" + Environment.NewLine +
                            "ON Dated" + DateTime.UtcNow.AddHours(5.30) + Environment.NewLine +
                                exception.Message.ToString() + Environment.NewLine +
                      "====================================================================================="
                            );
                        //ViewBag.Message = "Sorry! Problem in customer registration!!";
                        ModelState.AddModelError("Message", "Sorry! Problem in Plan Bind!!");
                        // Rollback transaction
                        ts.Dispose();

                        ErrorLog.ErrorLogFile("Unable to Insert Data Complete Transaction RollBack " + Environment.NewLine + errStr.ToString()
                            , ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
                    }
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                errStr.Append("Method Name[Http Request] :- Delete[HttpPost]" + Environment.NewLine +
                               "ON Dated" + DateTime.UtcNow.AddHours(5.30) + Environment.NewLine +
                                   ex.Message.ToString() + Environment.NewLine +
                         "====================================================================================="
                               );
                //ViewBag.Message = "Sorry! Problem in customer registration!!";
                ModelState.AddModelError("Message", "Sorry! Problem in customer Plan Bind Management!!");
                ErrorLog.ErrorLogFile("Unable to retrive data " + Environment.NewLine + errStr.ToString()
                    , ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);

                return View(lpbm);
            }

            return View(lpbm);
        }

        /// <summary>
        /// Insert Record of PlanBind 
        /// </summary>
        /// <param name="MasterPlan">Selected Plan ID</param>
        /// <param name="included">isIncluded/Excluded</param>
        /// <returns>identity Value</returns>
        private int InsertPlanBind(int MasterPlan,int included)
        {
            try
            {
                PlanBind lplanBind = new PlanBind();
                lplanBind.PlanID = Convert.ToInt16(MasterPlan);
                lplanBind.Type = included;
                lplanBind.Level = 1;
                lplanBind.IsActive = true;
                lplanBind.CreateBy = 1;
                lplanBind.CreateDate = DateTime.UtcNow.AddHours(5.5);

                db.PlanBinds.Add(lplanBind);
                db.SaveChanges();

                return lplanBind.ID;
            }
            catch(Exception ex)
            {
                throw new Exception("Methode[InsertPlanBind] Unable to insert data :-" + ex.Message);
            }
        }

        /// <summary>
        /// insert in PlanBindCategory
        /// </summary>
        /// <param name="identityValue">Identity Values</param>
        /// <param name="categoryID">CategoryID</param>
        private void InsertPlanBindCategory(Int32 identityValue,Int32 categoryID)
        {
            try
            {
                PlanBindCategory lplanBindCategory = new PlanBindCategory();
                lplanBindCategory.PlanBindID = identityValue;
                lplanBindCategory.CategoryID = categoryID;
                lplanBindCategory.IsActive = true;
                lplanBindCategory.CreateBy = CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(Session["ID"]));
                lplanBindCategory.CreateDate = DateTime.UtcNow.AddHours(5.5);

                db.PlanBindCategories.Add(lplanBindCategory);
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception("Methode[InsertPlanBindCategory] Unable to insert data :-" + ex.Message);
            }
        }

        public JsonResult PlanExistOrnot(int id)
        {
            return Json(db.PlanBinds.Where(x => x.PlanID == id), JsonRequestBehavior.AllowGet);
        }
        private static string fConnectionString = WebConfigurationManager.ConnectionStrings["EzeeloDBContext"].ToString();
        public List<DropdownList> NotInPlanCategoryList(int planID)
        {
            DataTable dt = new DataTable();
            DataAccessLayer.DbOperations op = new DataAccessLayer.GetData(fConnectionString);
            dt = op.GetRecords("FranchiseCategoryNotBindList", planID);

            List<DropdownList> ls = new List<DropdownList>();
            ls = (from n in dt.AsEnumerable()
                  select new DropdownList
                  {
                      Name = n.Field<string>("Name"),
                      ID = n.Field<Int32>("ID")
                  }).ToList();

            return ls;
        }
	}
}