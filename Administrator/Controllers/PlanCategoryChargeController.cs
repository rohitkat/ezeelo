//-----------------------------------------------------------------------
// <copyright file="PlanCategoryChargeController.cs" company="Ezeelo Consumer Services Pvt. Ltd.">
//     Copyright (c) Ezeelo Consumer Services Pvt. Ltd. All rights reserved.
// </copyright>
// <author>Snehal Shende</author>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ModelLayer.Models;
using ModelLayer.Models.ViewModel;
using BusinessLogicLayer;
using System.Data.Entity.SqlServer;
using System.Data.SqlClient;
using System.Text;
using Administrator.Models;

namespace Administrator.Controllers
{
    public class PlanCategoryChargeController : Controller
    {
        private EzeeloDBContext db = new EzeeloDBContext();


        // GET: /PlanCategoryCharge/
        [SessionExpire]
        [CustomAuthorize(Roles = "PlanCategoryCharge/CanRead")]
        public ActionResult Index()
        {
            try
            {
                return View(db.Plans.ToList());
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[PlanCategoryChargeController][GET:Index]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[PlanCategoryChargeController][GET:Index]",
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
            }
            return View();
        }

        [SessionExpire]
        [CustomAuthorize(Roles = "PlanCategoryCharge/CanRead")]
        public ActionResult ShowList(int planId, int? Category_2)
        {
            try
            {
                TempData["PlanID"] = planId;
                List<PlanCategoryChargeViewModel> categorylist = null;
                BusinessLogicLayer.GetPlanCatCharge obj = new GetPlanCatCharge();
                categorylist = obj.GetPlanCategoryCharges(planId);

                bool isMerchantPlan = (db.Plans.Where(x => x.ID == planId && x.PlanCode.StartsWith("GBMR")).Count() > 0) ? true : false;
                ViewBag.IsMerchantPlan = isMerchantPlan;
                if (isMerchantPlan)
                {
                    List<int> category_3 = categorylist.Select(x => x.CategoryID).ToList();
                    // var category_2 = db.Categories.Where(x => categorylist.Contains(x.ParentCategoryID));
                    var category_2 = (from c in db.Categories
                                      where category_3.Contains(c.ID) && c.Level == 3
                                      select new { ID = c.ParentCategoryID, Name = c.Category2.Name }).Distinct().OrderBy(x => x.Name);
                    ViewBag.Category_2 = new SelectList(category_2, "ID", "Name", Category_2);

                    if (Category_2 != null)
                    {
                        ViewBag.PCatId = Category_2;
                        List<PlanCategoryChargeViewModel> categorylist1 = (from cl in categorylist
                                                                           join c in db.Categories on cl.CategoryID equals c.ID
                                                                           where c.ParentCategoryID == Convert.ToInt32(Category_2)
                                                                           select cl
                                                                          ).ToList();
                        TempData.Remove("CatList");
                        TempData.Add("CatList", categorylist1);
                        return View(categorylist1);
                    }
                }
                return View(categorylist);
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[PlanCategoryChargeController][GET:ShowList]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[PlanCategoryChargeController][GET:ShowList]",
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
            }
            return View();
        }

        //[SessionExpire]
        //[CustomAuthorize(Roles = "PlanCategoryCharge/CanRead")]
        //public ActionResult ApplyChargesToMany(int? pCatID)
        //{
        //    //TempData.Keep("CatList");
        //    //TempData.Keep("PlanID");
        //    //List<PlanCategoryChargeViewModel>  catList = (List<PlanCategoryChargeViewModel>)TempData["CatList"];
        //    ViewBag.ChargeID = new SelectList(db.Charges, "ID", "Name");
        //    return PartialView("_ApplyChargesToMany");
        //}

        //[HttpPost]
        //[SessionExpire]
        //[CustomAuthorize(Roles = "PlanCategoryCharge/CanWrite")]
        //[ValidateAntiForgeryToken]
        //public ActionResult ApplyChargesToMany(FormCollection frm, string submit)
        //{
        //    try
        //    {
        //        int pCatId = 0;
        //        int.TryParse(frm.GetValue("Category_2").AttemptedValue, out pCatId);

        //        if (submit != null)
        //        {
        //            decimal chargeInPercent = 0;
        //            decimal chargeInRupee = 0;
        //            int chargeId = 0;

        //            decimal.TryParse(frm.GetValue("ChargeInPercent").AttemptedValue, out chargeInPercent);
        //            decimal.TryParse(frm.GetValue("ChargeInRupee").AttemptedValue, out chargeInRupee);
        //            int.TryParse(frm.GetValue("ChargeID").AttemptedValue, out chargeId);

        //            if (chargeId == 0)
        //            {
        //                ModelState.AddModelError("CustomError", "Please Select Charge Name");
        //                ViewBag.ChargeID = new SelectList(db.Charges, "ID", "Name");
        //               // return PartialView("_ApplyChargesToMany");
        //            }
        //            else if ((chargeInPercent > 0 && chargeInRupee > 0) || (chargeInPercent == 0 && chargeInRupee == 0))
        //            {
        //                ModelState.AddModelError("CustomError", "Please Enter Charge either in % or in Rs...");
        //                ViewBag.ChargeID = new SelectList(db.Charges, "ID", "Name", chargeId);
        //                //return PartialView("_ApplyChargesToMany");
        //            }
        //            else
        //            {
        //                insertUpdatePlanCategoryCharegs(chargeId, chargeInPercent, chargeInRupee);
        //            }
        //        }
        //        else if (frm.GetValue("Category_2").AttemptedValue != null)
        //        {
        //            return RedirectToAction("ShowList", new { planId = (int)TempData.Peek("PlanID"), Category_2 = pCatId });
        //        }
        //        return RedirectToAction("ShowList", new { planId = (int)TempData.Peek("PlanID") });
        //    }
        //    catch (BusinessLogicLayer.MyException myEx)
        //    {
        //        BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
        //            + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
        //            + "[PlanCategoryChargeController][POST:ApplyChargesToMany]" + myEx.EXCEPTION_PATH,
        //            BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
        //    }
        //    catch (Exception ex)
        //    {
        //        BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
        //            + Environment.NewLine + ex.Message + Environment.NewLine
        //            + "[PlanCategoryChargeController][POST:ApplyChargesToMany]",
        //            BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
        //    }

        //    return View();
        //}

        //private void insertUpdatePlanCategoryCharegs(int chargeId, decimal chargeInPercent, decimal chargeInRupee)
        //{
        //    try
        //    {
        //        using (var dbContextTransaction = db.Database.BeginTransaction())
        //        {
        //            List<PlanCategoryChargeViewModel> catList = (List<PlanCategoryChargeViewModel>)TempData.Peek("CatList");

        //            foreach (PlanCategoryChargeViewModel pccv in catList)
        //            {
        //                string mode = "edit";
        //                PlanCategoryCharge pcc = db.PlanCategoryCharges.Find(pccv.ID);
        //                if (pcc == null)
        //                {
        //                    mode = "insert";
        //                    pcc = new PlanCategoryCharge();
        //                }
        //                if (mode == "insert")
        //                {
        //                    pcc.PlanID = pccv.PlanID;
        //                    pcc.CategoryID = pccv.CategoryID;
        //                    pcc.ChargeID = chargeId;
        //                    pcc.ChargeInPercent = chargeInPercent;
        //                    pcc.ChargeInRupee = chargeInRupee;
        //                    pcc.IsActive = pccv.IsActive;
        //                    pcc.CreateBy = CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(Session["ID"]));
        //                    pcc.CreateDate = DateTime.UtcNow;
        //                    pcc.NetworkIP = CommonFunctions.GetClientIP();
        //                    db.PlanCategoryCharges.Add(pcc);
        //                    db.SaveChanges();
        //                }
        //                else
        //                {
        //                    //pcc.PlanID = pccv.PlanID;
        //                    //pcc.CategoryID = pccv.CategoryID;
        //                    pcc.ChargeID = chargeId;
        //                    pcc.ChargeInPercent = chargeInPercent;
        //                    pcc.ChargeInRupee = chargeInRupee;
        //                    pcc.ModifyBy = CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(Session["ID"]));
        //                    pcc.ModifyDate = DateTime.UtcNow;
        //                    pcc.NetworkIP = CommonFunctions.GetClientIP();
        //                    db.SaveChanges();
        //                }
        //            }
        //            dbContextTransaction.Commit();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new BusinessLogicLayer.MyException("[insertUpdatePlanCategoryCharegs]", "Can't Insert or Update PlanCategoryCharegs!" + Environment.NewLine + ex.Message);
        //    }            
        //}


        [SessionExpire]
        [CustomAuthorize(Roles = "PlanCategoryCharge/CanRead")]
        public ActionResult ApplyChargesToMany(long? ownerId, int? CatID)
        {
            try
            {
                ViewBag.ChargeID = new SelectList(db.Charges, "ID", "Name");
                ViewBag.ownerId = ownerId;
                int planId = 0;
                int.TryParse(TempData.Peek("PlanID").ToString(), out planId);
                if (planId == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                ViewBag.PlanID = planId;
                List<PlanCategoryChargeViewModel> categorylist = null;
                BusinessLogicLayer.GetPlanCatCharge obj = new GetPlanCatCharge();
                categorylist = obj.GetPlanCategoryCharges(planId);
                TempData.Remove("CatList");
                TempData.Add("CatList", categorylist);

                GetCatChargeList(planId, categorylist, 0);

            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[MerchantPlanCategoryChargeController][GET:ApplyChargesToMany]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[MerchantPlanCategoryChargeController][GET:ApplyChargesToMany]",
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
            }
            return View();
        }

        
        [HttpPost]
        [SessionExpire]
        [CustomAuthorize(Roles = "PlanCategoryCharge/CanWrite")]
        [ValidateAntiForgeryToken]
        public ActionResult ApplyChargesToMany(FormCollection frm, long? ownerId, bool IsActive)
        {
            try
            {
                ViewBag.Message = null;  
                decimal chargeInPercent = 0;
                decimal chargeInRupee = 0;
                int catId = 0;
                int planId = 0;
                int chargeId = 0;                

                int.TryParse(TempData.Peek("PlanID").ToString(), out planId);
                if (planId == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                ViewBag.PlanID = planId;
                decimal.TryParse(frm.GetValue("ChargeInPercent").AttemptedValue, out chargeInPercent);
                decimal.TryParse(frm.GetValue("ChargeInRupee").AttemptedValue, out chargeInRupee);
                int.TryParse(frm.GetValue("CatId").AttemptedValue, out catId);
                int.TryParse(TempData.Peek("PlanID").ToString(), out planId);
                int.TryParse(frm.GetValue("ChargeID").AttemptedValue, out chargeId);
                ViewBag.ChargeID = new SelectList(db.Charges, "ID", "Name", chargeId);

                List<PlanCategoryChargeViewModel> categorylist = (List<PlanCategoryChargeViewModel>)TempData.Peek("CatList");

                categorylist=new List<PlanCategoryChargeViewModel>(GetCatChargeList(planId, categorylist, catId));

                if ((chargeInPercent > 0 && chargeInRupee > 0) || (chargeInPercent == 0 && chargeInRupee == 0))
                {
                    ModelState.AddModelError("CustomError", "Please Enter Charge either in % or in Rs...");
                    return View();
                }
                else
                {
                    InsertUpdatePlanCategoryCharge(chargeId, chargeInPercent, chargeInRupee, IsActive, categorylist);
                    //return RedirectToAction("ShowList", new { planId = planId });
                    ViewBag.Message = "Chages is Apply to the Category";
                    return View();
                }

            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[MerchantPlanCategoryChargeController][POST:ApplyChargesToMany]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[MerchantPlanCategoryChargeController][POST:ApplyChargesToMany]",
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
            }
            return View();
        }

        private List<PlanCategoryChargeViewModel> GetCatChargeList(int planId, List<PlanCategoryChargeViewModel> categorylist, int catId)
        {
            try
            {
                //bool isMerchantPlan = (db.Plans.Where(x => x.ID == planId && x.PlanCode.StartsWith("GBMR")).Count() > 0) ? true : false;
                //if (isMerchantPlan)
                //{
                //    List<int> category_3 = categorylist.Select(x => x.CategoryID).ToList();
                //    // var category_2 = db.Categories.Where(x => categorylist.Contains(x.ParentCategoryID));
                //    var category_2 = (from c in db.Categories
                //                      where category_3.Contains(c.ID) && c.Level == 3
                //                      select new { ID = c.ParentCategoryID, Name = c.Category2.Name }).Distinct().OrderBy(x => x.Name);
                //    ViewBag.CatID = new SelectList(category_2, "ID", "Name", CatID);
                //}
                //else
                //{
                //    List<int> catlist = categorylist.Select(x => x.CategoryID).ToList();
                //    var category_1 = (from c in db.Categories
                //                      where catlist.Contains(c.ID) && c.Level == 1
                //                      select new { ID = c.ID, Name = c.Name }).Distinct().OrderBy(x => x.Name);
                //    ViewBag.CatID = new SelectList(category_1, "ID", "Name", CatID);
                //}

                bool isMerchantPlan = (db.Plans.Where(x => x.ID == planId && x.PlanCode.StartsWith("GBMR")).Count() > 0) ? true : false;
                if (isMerchantPlan)
                {
                    List<int> category_3 = categorylist.Select(x => x.CategoryID).ToList();
                    // var category_2 = db.Categories.Where(x => categorylist.Contains(x.ParentCategoryID));
                    var category_2 = (from c in db.Categories
                                      where category_3.Contains(c.ID) && c.Level == 3
                                      select new { ID = c.ParentCategoryID, Name = c.Category2.Name }).Distinct().OrderBy(x => x.Name);
                    ViewBag.CatID = new SelectList(category_2, "ID", "Name", catId);
                    if (catId > 0)
                    {
                        ViewBag.PCatId = catId;
                        List<PlanCategoryChargeViewModel> catOpccList = (from cl in categorylist
                                                                         join c in db.Categories on cl.CategoryID equals c.ID
                                                                         where c.ParentCategoryID == Convert.ToInt32(catId)
                                                                         select cl
                                                                          ).ToList();

                        categorylist = new List<PlanCategoryChargeViewModel>(catOpccList);
                    }
                }
                else
                {
                    List<int> catlist = categorylist.Select(x => x.CategoryID).ToList();
                    var category_1 = (from c in db.Categories
                                      where catlist.Contains(c.ID) && c.Level == 1
                                      select new { ID = c.ID, Name = c.Name }).Distinct().OrderBy(x => x.Name);
                    ViewBag.CatID = new SelectList(category_1, "ID", "Name", catId);

                    if (catId > 0)
                    {
                        ViewBag.PCatId = catId;
                        List<PlanCategoryChargeViewModel> catOpccList = (from cl in categorylist
                                                                         join c in db.Categories on cl.CategoryID equals c.ID
                                                                         where c.ID == Convert.ToInt32(catId)
                                                                         select cl
                                                                          ).ToList();

                        categorylist = new List<PlanCategoryChargeViewModel>(catOpccList);
                    }
                }
                return categorylist;
            }
            catch (Exception ex)
            {
                throw new BusinessLogicLayer.MyException("[GetCatChargeList]", "Can't Get Category Charge List!" + Environment.NewLine + ex.Message);
            }
        }
        
        private void InsertUpdatePlanCategoryCharge(int chargeId, decimal chargeInPercent, decimal chargeInRupee,bool isactive, List<PlanCategoryChargeViewModel> catList)
        {
            try
            {
                using (var dbContextTransaction = db.Database.BeginTransaction())
                {
                    foreach (PlanCategoryChargeViewModel pccv in catList)
                    {
                        string mode = "edit";
                        PlanCategoryCharge pcc = db.PlanCategoryCharges.Find(pccv.ID);
                        if (pcc == null)
                        {
                            mode = "insert";
                            pcc = new PlanCategoryCharge();
                        }
                        if (mode == "insert")
                        {
                            pcc.PlanID = pccv.PlanID;
                            pcc.CategoryID = pccv.CategoryID;
                            pcc.ChargeID = chargeId;
                            pcc.ChargeInPercent = chargeInPercent;
                            pcc.ChargeInRupee = chargeInRupee;
                            pcc.IsActive = isactive;
                            pcc.CreateBy = CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(Session["ID"]));
                            pcc.CreateDate = DateTime.UtcNow;
                            pcc.NetworkIP = CommonFunctions.GetClientIP();
                            db.PlanCategoryCharges.Add(pcc);
                            db.SaveChanges();
                        }
                        else
                        {
                            //pcc.PlanID = pccv.PlanID;
                            //pcc.CategoryID = pccv.CategoryID;
                            pcc.ChargeID = chargeId;
                            pcc.ChargeInPercent = chargeInPercent;
                            pcc.ChargeInRupee = chargeInRupee;
                            pcc.IsActive = isactive;
                            pcc.ModifyBy = CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(Session["ID"]));
                            pcc.ModifyDate = DateTime.UtcNow;
                            pcc.NetworkIP = CommonFunctions.GetClientIP();
                            db.SaveChanges();
                        }
                    }
                    dbContextTransaction.Commit();
                }
            }
            catch (Exception ex)
            {
                throw new BusinessLogicLayer.MyException("[insertUpdatePlanCategoryCharegs]", "Can't Insert or Update PlanCategoryCharegs!" + Environment.NewLine + ex.Message);
            }
        }

        [SessionExpire]
        [CustomAuthorize(Roles = "PlanCategoryCharge/CanRead")]
        // GET: /PlanCategoryCharge/Details/5
        public ActionResult Details(int? id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                PlanCategoryCharge plancategorycharge = db.PlanCategoryCharges.Find(id);
                if (plancategorycharge == null)
                {
                    return HttpNotFound();
                }
                return View(plancategorycharge);
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[PlanCategoryChargeController][GET:Details]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[PlanCategoryChargeController][GET:Details]",
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
            }
            return View();
        }

        [SessionExpire]
        [CustomAuthorize(Roles = "PlanCategoryCharge/CanRead")]
        // GET: /PlanCategoryCharge/Create
        public ActionResult Create(int catID, int planID)
        {
            try
            {
                ViewBag.PlanID = new SelectList(db.Plans.Where(a => a.ID.Equals(planID)), "ID", "ShortName", planID);
                ViewBag.CategoryID = new SelectList(db.Categories.Where(a => a.ID.Equals(catID)), "ID", "Name", catID);
                ViewBag.ChargeID = new SelectList(db.Charges, "ID", "Name");
                ViewBag.CreateBy = new SelectList(db.PersonalDetails, "ID", "FirstName");
                ViewBag.ModifyBy = new SelectList(db.PersonalDetails, "ID", "FirstName");
                return View();
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[PlanCategoryChargeController][GET:Create]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[PlanCategoryChargeController][GET:Create]",
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
            }
            return View();
        }

        // POST: /PlanCategoryCharge/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [SessionExpire]
        [CustomAuthorize(Roles = "PlanCategoryCharge/CanWrite")]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,PlanID,CategoryID,ChargeID,ChargeInPercent,ChargeInRupee,IsActive")] PlanCategoryCharge plancategorycharge)
        {
            try
            {
                if ((plancategorycharge.ChargeInPercent > 0 && plancategorycharge.ChargeInRupee > 0) ||
                    ((plancategorycharge.ChargeInPercent == 0 || plancategorycharge.ChargeInPercent == null) &&
                        (plancategorycharge.ChargeInRupee == 0 || plancategorycharge.ChargeInRupee == null)))
                {
                    ModelState.AddModelError("CustomError", "Please Enter Charge either in % or in Rs...");
                }
                else if (plancategorycharge.ChargeInPercent > 0)
                {
                    plancategorycharge.ChargeInRupee = 0;
                }
                else
                {
                    plancategorycharge.ChargeInPercent = 0;
                }

                plancategorycharge.CreateBy = CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(Session["ID"]));
                plancategorycharge.CreateDate = DateTime.UtcNow;
                plancategorycharge.NetworkIP = CommonFunctions.GetClientIP();

                if (ModelState.IsValid)
                {
                    db.PlanCategoryCharges.Add(plancategorycharge);
                    db.SaveChanges();
                    return RedirectToAction("ShowList", new { planId = plancategorycharge.PlanID });
                }

                ViewBag.PlanID = new SelectList(db.Plans, "ID", "ShortName", plancategorycharge.PlanID);
                ViewBag.CategoryID = new SelectList(db.Categories, "ID", "Name", plancategorycharge.CategoryID);
                ViewBag.ChargeID = new SelectList(db.Charges, "ID", "Name", plancategorycharge.ChargeID);
                ViewBag.CreateBy = new SelectList(db.PersonalDetails, "ID", "FirstName", plancategorycharge.CreateBy);
                ViewBag.ModifyBy = new SelectList(db.PersonalDetails, "ID", "FirstName", plancategorycharge.ModifyBy);
                return View(plancategorycharge);
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[PlanCategoryChargeController][POST:Create]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[PlanCategoryChargeController][POST:Create]",
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
            }
            return View();
        }

        // GET: /PlanCategoryCharge/Edit/5
        [SessionExpire]
        [CustomAuthorize(Roles = "PlanCategoryCharge/CanRead")]
        public ActionResult Edit(int? id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                PlanCategoryCharge plancategorycharge = db.PlanCategoryCharges.Find(id);
                if (plancategorycharge == null)
                {
                    return HttpNotFound();
                }
                ViewBag.CategoryID = new SelectList(db.Categories, "ID", "Name", plancategorycharge.CategoryID);
                ViewBag.ChargeID = new SelectList(db.Charges, "ID", "Name", plancategorycharge.ChargeID);
                ViewBag.CreateBy = new SelectList(db.PersonalDetails, "ID", "FirstName", plancategorycharge.CreateBy);
                ViewBag.ModifyBy = new SelectList(db.PersonalDetails, "ID", "FirstName", plancategorycharge.ModifyBy);
                return View(plancategorycharge);
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[PlanCategoryChargeController][GET:Edit]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[PlanCategoryChargeController][GET:Edit]",
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
            }
            return View();
        }

        // POST: /PlanCategoryCharge/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [SessionExpire]
        [CustomAuthorize(Roles = "PlanCategoryCharge/CanWrite")]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,PlanID,CategoryID,ChargeID,ChargeInPercent,ChargeInRupee,IsActive,CreateDate,CreateBy,ModifyDate,ModifyBy,NetworkIP,DeviceType,DeviceID")] PlanCategoryCharge plancategorycharge)
        {
            using (var dbContextTransaction = db.Database.BeginTransaction())
            {
                try
                {
                    PlanCategoryCharge lPlanCategoryCharge = db.PlanCategoryCharges.Find(plancategorycharge.ID);

                    if (lPlanCategoryCharge == null)
                    {
                        return View("Error");
                    }
                    if ((plancategorycharge.ChargeInPercent > 0 && plancategorycharge.ChargeInRupee > 0) ||
                    ((plancategorycharge.ChargeInPercent == 0 || plancategorycharge.ChargeInPercent == null) &&
                        (plancategorycharge.ChargeInRupee == 0 || plancategorycharge.ChargeInRupee == null)))
                    {
                        ModelState.AddModelError("CustomError", "Please Enter Charge either in % or in Rs...");
                    }
                    else if (plancategorycharge.ChargeInPercent > 0)
                    {
                        plancategorycharge.ChargeInRupee = 0;
                    }
                    else
                    {
                        plancategorycharge.ChargeInPercent = 0;
                    }
                    WriteToLogTable(lPlanCategoryCharge, ModelLayer.Models.Enum.COMMAND.UPDATE);

                    plancategorycharge.CreateDate = Convert.ToDateTime(lPlanCategoryCharge.CreateDate);
                    plancategorycharge.CreateBy = Convert.ToInt64(lPlanCategoryCharge.CreateBy);
                    plancategorycharge.ModifyDate = DateTime.UtcNow;
                    plancategorycharge.ModifyBy = CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(Session["ID"]));
                    //TryUpdateModel(plancategorycharge);


                    if (ModelState.IsValid)
                    {
                        db.Entry(lPlanCategoryCharge).CurrentValues.SetValues(plancategorycharge);
                        db.SaveChanges();
                        dbContextTransaction.Commit();
                        return RedirectToAction("ShowList", new { planId = plancategorycharge.PlanID });
                    }
                    ViewBag.CategoryID = new SelectList(db.Categories, "ID", "Name", plancategorycharge.CategoryID);
                    ViewBag.ChargeID = new SelectList(db.Charges, "ID", "Name", plancategorycharge.ChargeID);
                    ViewBag.CreateBy = new SelectList(db.PersonalDetails, "ID", "FirstName", plancategorycharge.CreateBy);
                    ViewBag.ModifyBy = new SelectList(db.PersonalDetails, "ID", "FirstName", plancategorycharge.ModifyBy);
                    return View(plancategorycharge);
                }
                catch (BusinessLogicLayer.MyException myEx)
                {
                    BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                        + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                        + "[PlanCategoryChargeController][POST:Edit]" + myEx.EXCEPTION_PATH,
                        BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
                }
                catch (Exception ex)
                {
                    BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                        + Environment.NewLine + ex.Message + Environment.NewLine
                        + "[PlanCategoryChargeController][POST:Edit]",
                        BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
                }
            }
            return View();
        }

        // GET: /PlanCategoryCharge/Delete/5
        [SessionExpire]
        [CustomAuthorize(Roles = "PlanCategoryCharge/CanRead")]
        public ActionResult Delete(int? id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                PlanCategoryCharge plancategorycharge = db.PlanCategoryCharges.Find(id);
                if (plancategorycharge == null)
                {
                    return HttpNotFound();
                }
                return View(plancategorycharge);
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[PlanCategoryChargeController][GET:Delete]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[PlanCategoryChargeController][GET:Delete]",
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
            }
            return View();
        }

        // POST: /PlanCategoryCharge/Delete/5
        [HttpPost, ActionName("Delete")]
        [SessionExpire]
        [CustomAuthorize(Roles = "PlanCategoryCharge/CanWrite")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            using (var dbContextTransaction = db.Database.BeginTransaction())
            {
                try
                {
                    PlanCategoryCharge plancategorycharge = db.PlanCategoryCharges.Find(id);

                    WriteToLogTable(plancategorycharge, ModelLayer.Models.Enum.COMMAND.DELETE);

                    db.PlanCategoryCharges.Remove(plancategorycharge);
                    db.SaveChanges();
                    dbContextTransaction.Commit();
                    return RedirectToAction("Index");
                }
                catch (BusinessLogicLayer.MyException myEx)
                {
                    BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                        + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                        + "[PlanCategoryChargeController][POST:Delete]" + myEx.EXCEPTION_PATH,
                        BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
                }
                catch (Exception ex)
                {
                    BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                        + Environment.NewLine + ex.Message + Environment.NewLine
                        + "[PlanCategoryChargeController][POST:Delete]",
                        BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
                }
            }
            return View();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        public void WriteToLogTable(PlanCategoryCharge obj, ModelLayer.Models.Enum.COMMAND mode)
        {
            try
            {
                //Log Table Insertion
                //LogTable logTable = new LogTable();
                //logTable.TableName = "PlanCategoryCharge";//table Name(Model Name)
                //logTable.RecordXML = ModelLayer.Models.ObjectToXml.GetXMLFromObject(obj);
                //logTable.TableRowID = obj.ID;
                //logTable.Command = mode.ToString();
                //long? rowOwnerID = (obj.ModifyBy >= 0 ? obj.ModifyBy : obj.CreateBy);
                //logTable.RowOwnerID = (long)rowOwnerID;
                //logTable.CreateDate = DateTime.UtcNow;
                //logTable.CreateBy = CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(Session["ID"]));//Session ID
                //db.LogTables.Add(logTable);
                /**************************************/
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                     + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                     + "[PlanCategoryChargeController][WriteToLogTable]" + myEx.EXCEPTION_PATH,
                     BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[PlanCategoryChargeController][WriteToLogTable]",
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
            }
        }
    }
}
