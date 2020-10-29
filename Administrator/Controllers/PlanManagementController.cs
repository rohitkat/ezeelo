//-----------------------------------------------------------------------
// <copyright file="ProductList.cs" company="Ezeelo Consumer Services Pvt. Ltd.">
//     Copyright (c) Ezeelo Consumer Services Pvt. Ltd. All rights reserved.
// </copyright>
// <author>Pradnyakar N. Badge</author>
//-----------------------------------------------------------------------
using ModelLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ModelLayer.Models.ViewModel;
using System.Transactions;
using System.Net;
using BusinessLogicLayer;
using System.Text;
using Administrator.Models;
using PagedList.Mvc;
using PagedList;
using System.Data.Entity;
using System.Collections;
using System.Data;
using System.Data.SqlClient;
using System.Web.Configuration;

namespace Administrator.Controllers
{

    public class PlanManagementController : Controller
    {
        StringBuilder errStr = new StringBuilder("=====================================================================================" +
            Environment.NewLine
            + "ErrorLog Controller : FranchisePlanManagement" + Environment.NewLine);
        private EzeeloDBContext db = new EzeeloDBContext();
        //
        // GET: /PlanManagement/
        [SessionExpire]
        [CustomAuthorize(Roles = "PlanManagement/CanRead")]
        public ActionResult Index()
        {
            try
            {
                var ldata = db.PlanBinds.Where(x => x.Level == 3 && x.IsActive == true).ToList();
                ViewBag.PlanCode = db.Plans.Where(x => x.IsActive == true).ToList();
                return View(ldata);
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Message", "Sorry! Problem in Generate Index view!!");
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[PlanManagementController][GET:Index]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
                return View();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Message", "Sorry! Problem in Generate Index view!!");
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[PlanManagementController][GET:Index]",
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

        [SessionExpire]
        [CustomAuthorize(Roles = "PlanManagement/CanWrite")]
        public ActionResult Create(int? page)
        {
            try
            {
                ViewBag.Levelone = new SelectList(db.Categories.Where(x => x.Level == 1 && x.IsActive== true).ToList(), "ID", "Name");

                List<SelectListItem> lData = new List<SelectListItem>();
                lData.Add(new SelectListItem { Text = string.Empty, Value = "0" });
                ViewBag.LevelTwo = new SelectList(lData, "Value", "Text");
                ViewBag.MasterPlan = new SelectList(db.Plans.Where(x => x.PlanCode.Substring(0, 4).ToUpper() == "GBMR" && x.IsActive == true).ToList(), "ID", "ShortName");

                var categories = db.Categories.Where(c => c.Level == 0 && c.IsActive == true);
                return View(categories.Where(x => x.IsActive == true).ToList().ToPagedList(page ?? 1, 20));


            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Message", "Sorry! Problem in customer Plan Bind Management!!");
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[PlanManagementController][GET:Create]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
                return View();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Message", "Sorry! Problem in customer Plan Bind Management!!");
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[PlanManagementController][GET:Create]",
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
                return View();
            }

        }

        [HttpPost]
        [SessionExpire]
        [CustomAuthorize(Roles = "PlanManagement/CanWrite")]
        public ActionResult Create(string CaltegoryList, int MasterPlan, int included, List<Category> lCategory)
        {
            try
            {
                this.fillViewBag();
                string[] strCategory;

                strCategory = CaltegoryList.Split(',');

                if (included < 1 || included > 2)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                if (isPlanBindExist(MasterPlan))
                {
                    ViewBag.pMessage = "Plan is already Exist";
                    var catList = db.Categories.Where(c => c.Level == 0 && c.IsActive == true);
                    return View(catList.ToList().ToPagedList(1, 20));
                }

                int level = getLevel(Convert.ToInt16(MasterPlan));


                int identityValue = this.InsertUpdatePlanBind(Convert.ToInt16(MasterPlan), included, level); // lplanBind.ID;


                this.UpdatePlanBindCategory(strCategory, identityValue, CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(Session["ID"])), BusinessLogicLayer.CommonFunctions.GetClientIP());


                ModelState.Clear();
                ModelState.AddModelError("Message", "Done! Plan Bind Successfully Done!!");
                ViewBag.pMessage = "Plan created for Marchant";


            }

            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Message", "Sorry! Problem in Plan Bind registration!!");
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[PlanManagementController][POST:Create]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
                var catList = db.Categories.Where(c => c.Level == 0);
                return View(catList.ToList().ToPagedList(1, 20));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Message", "Sorry! Problem in Plan Bind registration!!");
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[PlanManagementController][POST:Create]",
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
                var catList = db.Categories.Where(c => c.Level == 0);
                return View(catList.ToList().ToPagedList(1, 20));
            }

            var categoryList = db.Categories.Where(c => c.Level == 0);
            return View(categoryList.ToList().ToPagedList(1, 20));

        }

        [SessionExpire]
        [CustomAuthorize(Roles = "PlanManagement/CanWrite")]
        public ActionResult Edit(int? id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                PlanBind planBind = db.PlanBinds.Find(id);
                this.fillViewBag();
                ViewBag.MasterPlan = new SelectList(db.Plans.Where(x => x.ID == planBind.PlanID && x.IsActive == true), "ID", "ShortName", planBind.PlanID);


                if (planBind == null)
                {
                    return HttpNotFound();
                }

                SelectPlanBindCategoryUsingProcedure objPlanBind = new BusinessLogicLayer.SelectPlanBindCategoryUsingProcedure();
                PlanBindManagement lp = new PlanBindManagement();

                lp = objPlanBind.SelectPlanBindCategoryForUpdate(System.Web.HttpContext.Current.Server, Convert.ToInt16(id));

                List<PlanCommission> pc = new List<PlanCommission>();
                pc = lp.planCommission;

                ViewBag.PlanBind = pc;

                var categories = db.Categories.Where(c => c.Level == 0 && c.IsActive == true);
                return View(categories.ToList().ToPagedList(1, 20));
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Message", "Sorry! Problem in customer Plan Bind Management!!");
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[PlanManagementController][GET:Edit]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
                return View();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Message", "Sorry! Problem in customer Plan Bind Management!!");
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[PlanManagementController][GET:Edit]",
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
                return View();
            }


        }

        private static string fConnectionString = WebConfigurationManager.ConnectionStrings["EzeeloDBContext"].ToString();
        /// <summary>
        /// To Insert and Update PlanBind Category
        /// </summary>
        /// <param name="strCategory">List Of Category</param>
        /// <param name="identityValue">Plan ID</param>
        /// <param name="createdBY">Created By</param>
        /// <param name="IP">Network IP</param>
        private void UpdatePlanBindCategory(string[] strCategory, int identityValue, Int64 createdBY, string IP)
        {

            DataTable lDataTable = new DataTable();
            lDataTable.Columns.Add("CategoryID");
            foreach (string val in strCategory)
            {
                int v = Convert.ToInt32(val);
                DataRow dr = lDataTable.NewRow();
                dr[0] = v;
                lDataTable.Rows.Add(dr);
            }

            using (SqlConnection conn = new SqlConnection(fConnectionString))
            {
                SqlCommand sqlComm = new SqlCommand("InsertupdatePlanBindCategory", conn);
                sqlComm.CommandType = CommandType.StoredProcedure;
                sqlComm.Parameters.AddWithValue("@PlanBindID", SqlDbType.BigInt).Value = identityValue;
                sqlComm.Parameters.AddWithValue("@CreateBy", SqlDbType.DateTime2).Value = createdBY;
                sqlComm.Parameters.AddWithValue("@NetworkIP", SqlDbType.Structured).Value = IP;
                sqlComm.Parameters.AddWithValue("@CategoryList", SqlDbType.Structured).Value = lDataTable;
                conn.Open();
                sqlComm.ExecuteNonQuery();

                conn.Close();
                ViewBag.pMessage = "Done! Plan Bind Updated Successfully Done!!";
            }
        }

        [HttpPost]
        [SessionExpire]
        [CustomAuthorize(Roles = "PlanManagement/CanWrite")]
        public ActionResult Edit(string CaltegoryList, int MasterPlan, int included)
        {
            try
            {
                string[] strCategory;

                strCategory = CaltegoryList.Split(',');

                if (MasterPlan < 1 || MasterPlan == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                else if (included < 1 || included > 2)
                {
                    ViewBag.pMessage = "Please Select Plan Type [Included/ Excluded]...!";

                    ModelState.AddModelError("Message", "Please Select Plan Type [Included/ Excluded]...!");
                    EditViewData(MasterPlan);
                    var categories = db.Categories.Where(c => c.Level == 0 && c.IsActive == true);
                    return View(categories.ToList().ToPagedList(1, 20));

                }
                else if (MasterPlan < 1)
                {
                    ViewBag.pMessage = "Please Select Plan ...!";

                    ModelState.AddModelError("Message", "Please Select Plan...!");
                    EditViewData(MasterPlan);
                    var categories = db.Categories.Where(c => c.Level == 0 && c.IsActive == true);
                    return View(categories.ToList().ToPagedList(1, 20));

                }


                PlanBind lData = db.PlanBinds.Where(x => x.PlanID == MasterPlan && x.IsActive == true).FirstOrDefault();

                if (lData != null)
                {

                    ViewBag.MasterPlan = new SelectList(db.Plans.Where(x => x.ID == lData.PlanID && x.IsActive == true), "ID", "ShortName", lData.PlanID);
                    this.fillViewBag();

                    this.UpdatePlanBind(MasterPlan, included);

                    int identityValue = lData.ID;

                    this.UpdatePlanBindCategory(strCategory, identityValue, CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(Session["ID"])), BusinessLogicLayer.CommonFunctions.GetClientIP());

                    ViewBag.pMessage = "Done! Plan Bind Updated Successfully Done!!";

                }
                else
                {

                    ViewBag.pMessage = "Plan Commission is Already Exists...! Therefore this plan can not be Updated";
                    ModelState.AddModelError("Message", "Plan Commission is Already Exists...! Therefore this plan can not be Updated");
                    ViewBag.MasterPlan = new SelectList(db.Plans.Where(x => x.IsActive == true).ToList(), "ID", "ShortName");
                    EditViewData(MasterPlan);
                    var categories = db.Categories.Where(c => c.Level == 0 && c.IsActive == true);
                    return View(categories.ToList().ToPagedList(1, 20));
                }
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                ViewBag.pMessage = "Sorry! Problem in Plan Bind registration!!";

                ModelState.AddModelError("Message", "Sorry! Problem in Plan Bind registration!!");
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[PlanManagementController][POST:Edit]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);

                var categories = db.Categories.Where(c => c.Level == 0);
                return View(categories.ToList().ToPagedList(1, 20));
            }
            catch (Exception ex)
            {
                ViewBag.pMessage = "Sorry! Problem in Plan Bind registration!!";

                ModelState.AddModelError("Message", "Sorry! Problem in Plan Bind registration!!");
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[PlanManagementController][POST:Edit]",
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);

                var categories = db.Categories.Where(c => c.Level == 0);
                return View(categories.ToList().ToPagedList(1, 20));
            }

            var catList = db.Categories.Where(c => c.Level == 0);
            return View(catList.ToList().ToPagedList(1, 20));

        }

        [SessionExpire]
        [CustomAuthorize(Roles = "PlanManagement/CanDelete")]
        public ActionResult Delete(int id)
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


        /// <summary>
        /// Insert into PlanBind Table
        /// </summary>
        /// <param name="MasterPlan">PlanID</param>
        /// <param name="included">isIncluded(1)/Excluded(2)</param>
        /// <param name="level">Level(3)</param>
        /// <returns></returns>
        private int InsertUpdatePlanBind(int MasterPlan, int included, int level)
        {
            try
            {
                PlanBind lplanBind = new PlanBind();
                lplanBind.PlanID = MasterPlan;
                lplanBind.Type = included;
                lplanBind.Level = level;
                lplanBind.IsActive = true;
                lplanBind.CreateBy = 1; //CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(Session["ID"]));
                lplanBind.CreateDate = DateTime.UtcNow.AddHours(5.5);

                db.PlanBinds.Add(lplanBind);
                db.SaveChanges();

                return lplanBind.ID;
            }
            catch (Exception ex)
            {
                throw new System.ArgumentException("Problem in data insert in PlanBind ");
            }
        }

        private int UpdatePlanBind(int MasterPlan, int included)
        {
            try
            {
                PlanBind lData = db.PlanBinds.Where(x => x.PlanID == MasterPlan).FirstOrDefault();

                PlanBind lplanBind = new PlanBind();
                lplanBind.ID = lData.ID;
                lplanBind.PlanID = lData.PlanID;
                lplanBind.Type = included;
                lplanBind.Level = 3;
                lplanBind.IsActive = lData.IsActive;
                lplanBind.CreateBy = lData.CreateBy;
                lplanBind.CreateDate = lData.CreateDate;
                lplanBind.ModifyBy = CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(Session["ID"]));
                lplanBind.ModifyDate = DateTime.UtcNow.AddHours(5.5);
                lplanBind.NetworkIP = BusinessLogicLayer.CommonFunctions.GetClientIP();
                lplanBind.DeviceID = string.Empty;
                lplanBind.DeviceType = string.Empty;

                db.Entry(lData).CurrentValues.SetValues(lplanBind);
                db.SaveChanges();

                return lplanBind.ID;
            }
            catch (Exception ex)
            {
                throw new System.ArgumentException("Problem in data insert in PlanBind ");
            }
        }

        private void EditViewData(int id)
        {
            ViewBag.MasterPlan = new SelectList(db.Plans.Where(x => x.IsActive == true).ToList(), "ID", "ShortName");
            this.fillViewBag();

            SelectPlanBindCategoryUsingProcedure objPlanBind = new BusinessLogicLayer.SelectPlanBindCategoryUsingProcedure();
            PlanBindManagement lp = new PlanBindManagement();

            lp = objPlanBind.SelectPlanBindCategoryForUpdate(System.Web.HttpContext.Current.Server, Convert.ToInt16(id));

            List<PlanCommission> pc = new List<PlanCommission>();
            pc = lp.planCommission;

            ViewBag.PlanBind = pc;

        }

        private void fillViewBag()
        {
            ViewBag.Levelone = new SelectList(db.Categories.Where(x => x.Level == 1 && x.IsActive == true).ToList(), "ID", "Name");

            List<SelectListItem> lData = new List<SelectListItem>();
            lData.Add(new SelectListItem { Text = string.Empty, Value = "0" });
            ViewBag.LevelTwo = new SelectList(lData, "Value", "Text");
            ViewBag.MasterPlan = new SelectList(db.Plans.Where(x => x.PlanCode.Substring(0, 4).ToUpper() == "GBMR" && x.IsActive== true).ToList(), "ID", "ShortName");
        }

        /// <summary>
        /// Insert in PlanBindCategory
        /// </summary>
        /// <param name="identityValue">Identity of Plan Bind Table</param>
        /// <param name="strCategory">Category ID</param>
        //private void InsertUpdatePlanBindCategory(int identityValue, int strCategory)
        //{
        //    try
        //    {
        //        PlanBindCategory lplanBindCategory = new PlanBindCategory();
        //        lplanBindCategory.PlanBindID = identityValue;
        //        lplanBindCategory.CategoryID = Convert.ToInt32(strCategory);
        //        lplanBindCategory.IsActive = true;
        //        lplanBindCategory.CreateBy =  CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(Session["ID"]));
        //        lplanBindCategory.CreateDate = DateTime.UtcNow.AddHours(5.5);

        //        db.PlanBindCategories.Add(lplanBindCategory);
        //        db.SaveChanges();

        //    }
        //    catch (Exception ex)
        //    {
        //        //throw new System.ArgumentException("Problem in data insert in PlanBindCategory ");
        //        ModelState.AddModelError("Message", "Sorry! Problem in data insert in PlanBindCategory!!");
        //        BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
        //            + Environment.NewLine + ex.Message + Environment.NewLine
        //            + "[PlanManagementController][POST:Edit][InsertUpdatePlanBindCategory]",
        //            BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
        //    }
        //}

        //private void UpdatePlanBindCategory(int identityValue, int strCategory)
        //{
        //    try
        //    {
        //        //PlanBindCategory lplanBindCategory = new PlanBindCategory();

        //        //lplanBindCategory = db.PlanBindCategories.Where(x => x.CategoryID == strCategory && x.PlanBindID == identityValue).FirstOrDefault();

        //        //lplanBindCategory.PlanBindID = identityValue;
        //        //lplanBindCategory.CategoryID = Convert.ToInt32(strCategory);
        //        //lplanBindCategory.IsActive = true;
        //        //lplanBindCategory.CreateBy =  CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(Session["ID"]));
        //        //lplanBindCategory.ModifyDate = DateTime.UtcNow.AddHours(5.5);

        //        //db.Entry(lplanBindCategory).State = EntityState.Modified;
        //        //db.SaveChanges();



        //    }
        //    catch (Exception ex)
        //    {
        //       // throw new System.ArgumentException("Problem in data insert in PlanBindCategory ");
        //        ModelState.AddModelError("Message", "Sorry! Problem in data Update in PlanBindCategory!!");
        //        BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
        //            + Environment.NewLine + ex.Message + Environment.NewLine
        //            + "[PlanManagementController][POST:Edit][UpdatePlanBindCategory]",
        //            BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
        //    }
        //}

        public PlanBindManagement FillData()
        {
            try
            {
                List<Category> lcategory = new List<Category>();
                List<PlanCommission> planls = new List<PlanCommission>();
                PlanBindManagement ls = new PlanBindManagement();
                planls = (from d in db.Categories
                          where d.IsActive == true
                          select new PlanCommission
                             {
                                 CategoryID = d.ID,
                                 parentCategoryID = d.ParentCategoryID,
                                 CategoryLevel = d.Level,
                                 CategoryName = d.Name
                             }).ToList();

                ls.planCommission = planls;

                return ls;
            }
            catch
            {
                throw new System.ArgumentException("Problem in data retriving from PlanCommission ");
            }
        }

        public List<PlanCommission> FillDataForEdit(int PlanBindID)
        {
            try
            {
                List<Category> lcategory = new List<Category>();
                List<PlanCommission> ls = new List<PlanCommission>();

                lcategory = db.Categories.ToList();
                foreach (Category dd in lcategory)
                {
                    PlanCommission lpc = new PlanCommission();
                    lpc.CategoryID = dd.ID;
                    lpc.parentCategoryID = Convert.ToInt16(dd.ParentCategoryID);
                    lpc.CategoryLevel = dd.Level;
                    lpc.CategoryName = dd.Name;
                    ls.Add(lpc);
                }

                var lplanBind = db.PlanBinds.ToList();


                return ls;
            }
            catch
            {
                throw new System.ArgumentException("Problem in data retriving from PlanCommission for Edit", "Orignal Error");
            }
        }


        public class CategoryList
        {
            public Int64 ID { get; set; }
            public int level { get; set; }
            public string Name { get; set; }
            public bool Iselected { get; set; }

        }

        public class CatID
        {
            public Int16 id { get; set; }
        }

        public bool isPlanBindExist(int MasterPlan)
        {
            if (db.PlanBinds.Where(x => x.PlanID == MasterPlan).Count() > 0)
                return true;
            else
                return false;
        }

        public JsonResult GetSecondLevelCategory(int pcatID)
        {
            List<CategoryList> lst = new List<CategoryList>();

            lst = (from n in db.Categories
                   where n.ParentCategoryID == pcatID && n.IsActive == true
                   select new CategoryList
                   {
                       ID = n.ID,
                       Name = n.Name,
                       level = n.Level
                   }).ToList();

            return Json(lst, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Check the Owner Type
        /// if Owner is Marchant(GBMR) then he will deal with Level 3 category
        /// if Owner is Franchise(GBFR) then he will deal with Level 1 category
        /// </summary>
        /// <param name="plaID">Plan ID</param>
        /// <returns>level (1 or 3)</returns>
        public int getLevel(int plaID)
        {
            try
            {
                Plan lData = db.Plans.Find(plaID);


                string planCode = lData.PlanCode;
                string lCode = planCode.Remove(4);

                switch (lCode)
                {
                    case "GBMR":
                        return 3;
                    case "GBFR":
                        return 1;
                }
                return 0;
            }
            catch
            {

                throw new System.ArgumentException("Unable to get Level", "Orignal Error");
            }
        }

        [SessionExpire]
        
        public JsonResult DeletePlanCategory(Int32 PlanID, Int32 categoryID)
        {
            int result;
            PlanBindCategory pbl = new PlanBindCategory();
            int planBindId = db.PlanBinds.Where(x => x.PlanID == PlanID).FirstOrDefault().ID;
            int id = db.PlanBindCategories.Where(x => x.PlanBindID == planBindId && x.CategoryID == categoryID).FirstOrDefault().ID;

            if (db.PlanCategoryCharges.Where(x => x.PlanID == PlanID && x.CategoryID == categoryID).Count() > 0)
            {
                result = 0;
            }
            else
            {
                pbl = db.PlanBindCategories.Find(id);
                db.PlanBindCategories.Remove(pbl);
                db.SaveChanges();
                result = 1;
            }


            return Json(result, JsonRequestBehavior.AllowGet);
        }

    }
}