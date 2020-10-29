//-----------------------------------------------------------------------
// <copyright file="OwnerPlanController.cs" company="Ezeelo Consumer Services Pvt. Ltd.">
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
using Administrator.Models;
using System.Text;

namespace Administrator.Controllers
{

    public class OwnerPlanController : Controller
    {
        private EzeeloDBContext db = new EzeeloDBContext();

        // GET: /OwnerPlan/
        [SessionExpire]
        [CustomAuthorize(Roles = "OwnerPlan/CanRead")]
        public ActionResult Index(long? ownerId)
        {
            try
            {
                if (ownerId == null)
                {
                    var franchiseList = (from bd in db.BusinessDetails
                                         join f in db.Franchises on bd.ID equals f.BusinessDetailID
                                         join o in db.OwnerPlans on f.ID equals o.OwnerID
                                         join p in db.Plans on o.PlanID equals p.ID
                                         join pd in db.PersonalDetails on bd.UserLoginID equals pd.UserLoginID
                                         join ul in db.UserLogins on bd.UserLoginID equals ul.ID
                                         where p.PlanCode.StartsWith("GBFR") && ul.IsLocked == true
                                         select new OwnerPlanViewModel
                                         {
                                             ID = o.ID,
                                             PlanID = o.PlanID,
                                             OwnerID = o.OwnerID,
                                             OwnerName = pd.Salutation.Name + " " + pd.FirstName + " " + pd.LastName,
                                             StartDate = o.StartDate,
                                             EndDate = o.EndDate,
                                             IsActive = o.IsActive,
                                             CreateDate = o.CreateDate,
                                             CreateBy = o.CreateBy,
                                             ModifyDate = o.ModifyDate,
                                             ModifyBy = o.ModifyBy,
                                             NetworkIP = o.NetworkIP,
                                             DeviceType = o.DeviceType,
                                             DeviceID = o.DeviceID,
                                             PersonalDetail = o.PersonalDetail,
                                             PersonalDetail1 = o.PersonalDetail1,
                                             Plan = o.Plan
                                         });
                    return View(franchiseList.ToList());
                }

                List<OwnerPlan> ownerplans = new List<OwnerPlan>();
                if (ownerId != null)
                {
                    GetOwnerName(ownerId);
                    //db.Plans.Include(p=>p.PlanCode.StartsWith("GBFR")).

                    ownerplans = db.OwnerPlans.Where(e => e.OwnerID == ownerId && e.Plan.PlanCode.StartsWith("GBFR") && e.IsActive == true).ToList();

                    if (ownerplans.Count() > 0)
                    {
                        int ID = Convert.ToInt32((from o in db.OwnerPlans where o.OwnerID == ownerId && o.Plan.PlanCode.StartsWith("GBFR") && o.IsActive == true select o.ID).First());
                        return RedirectToAction("Details", new { id = ID });
                    }
                    else
                    {
                        FillData(ownerId);
                        return RedirectToAction("Create", new { ownerId = ownerId });
                    }
                }
                return View(ownerplans);
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[OwnerPlanController][GET:Index]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[OwnerPlanController][GET:Index]",
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
            }
            return View();
        }

        // GET: /OwnerPlan/Details/5
        [SessionExpire]
        [CustomAuthorize(Roles = "OwnerPlan/CanRead")]
        public ActionResult Details(int? id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                OwnerPlan ownerplan = db.OwnerPlans.Find(id);
                if (ownerplan == null)
                {
                    return HttpNotFound();
                }
                GetOwnerName(ownerplan.OwnerID);

                OwnerPlanViewModel lOwnerPlan = new OwnerPlanViewModel();
                lOwnerPlan.ID = ownerplan.ID;
                lOwnerPlan.PlanID = ownerplan.PlanID;
                lOwnerPlan.OwnerID = ownerplan.OwnerID;
                lOwnerPlan.OwnerName = @ViewBag.OwnerName;
                lOwnerPlan.StartDate = ownerplan.StartDate;
                lOwnerPlan.EndDate = ownerplan.EndDate;
                lOwnerPlan.IsActive = ownerplan.IsActive;
                lOwnerPlan.Plan = ownerplan.Plan;
                ViewBag.StartDate1 = ownerplan.StartDate.ToString("dd/MM/yyyy hh:mm:ss tt");
                ViewBag.EndDate1 = ownerplan.EndDate.ToString("dd/MM/yyyy hh:mm:ss tt");
                return View(lOwnerPlan);
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[OwnerPlanController][GET:Details]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[OwnerPlanController][GET:Details]",
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
            }
            return View();
        }

        // GET: /OwnerPlan/Create
        [SessionExpire]
        [CustomAuthorize(Roles = "OwnerPlan/CanRead")]
        public ActionResult Create(long? ownerId)
        {
            try
            {
                if (ownerId != null)
                {
                    FillData(ownerId);
                }
                ViewBag.CreateBy = new SelectList(db.PersonalDetails, "ID", "FirstName");
                ViewBag.ModifyBy = new SelectList(db.PersonalDetails, "ID", "FirstName");
                return View();
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[OwnerPlanController][GET:Create]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[OwnerPlanController][GET:Create]",
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
            }
            return View();
        }

        // POST: /OwnerPlan/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [SessionExpire]
        [CustomAuthorize(Roles = "OwnerPlan/CanWrite")]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,PlanID,OwnerID,StartDate,EndDate,IsActive,CreateDate,CreateBy,ModifyDate,ModifyBy,NetworkIP,DeviceType,DeviceID")] OwnerPlan ownerplan, string StartDate1)
        {
            try
            {
                if (ownerplan.OwnerID != null)
                {
                    FillData(ownerplan.OwnerID);
                }
                if (ownerplan.PlanID > 0 && ownerplan.OwnerID > 0)
                {
                    CheckIsPreviousPlanExist(ownerplan.OwnerID);

                    Plan lPlan = db.Plans.Find(ownerplan.PlanID);
                    if (lPlan == null)
                    {
                        return HttpNotFound();
                    }

                    DateTime? lStartDate = CommonFunctions.GetDateTime(StartDate1);
                    if (lStartDate != null)
                    {
                        ownerplan.StartDate = (DateTime)lStartDate;
                    }
                    ViewBag.StartDate = ownerplan.StartDate.ToString();
                    if (ownerplan.StartDate.Date >= DateTime.Now.Date)
                    {
                        ownerplan.IsActive = true;
                        ownerplan.CreateDate = DateTime.UtcNow.AddHours(5.30);
                        ownerplan.NetworkIP = CommonFunctions.GetClientIP();
                        ownerplan.CreateBy = CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(Session["ID"]));
                        ownerplan.EndDate = ownerplan.StartDate.AddDays(lPlan.Day).AddMonths(lPlan.Month).AddYears(lPlan.Year);

                        using (var dbContextTransaction = db.Database.BeginTransaction())
                        {
                            if (ModelState.IsValid)
                            {
                                db.OwnerPlans.Add(ownerplan);
                                db.SaveChanges();
                                dbContextTransaction.Commit();
                                return RedirectToAction("Details", new { id = ownerplan.ID });
                            }
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("CustomError", "Please Select Today or Future Date in Start Date...");
                    }
                }
                else
                {
                    ModelState.AddModelError("CustomError", "Please Select Proper Details...");
                }
                ViewBag.CreateBy = new SelectList(db.PersonalDetails, "ID", "FirstName", ownerplan.CreateBy);
                ViewBag.ModifyBy = new SelectList(db.PersonalDetails, "ID", "FirstName", ownerplan.ModifyBy);
                ViewBag.PlanID = new SelectList(db.Plans.Where(x => x.PlanCode.StartsWith("GBFR")), "ID", "ShortName", ownerplan.PlanID);
                return View(ownerplan);
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[OwnerPlanController][POST:Create]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[OwnerPlanController][POST:Create]",
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
            }
            return View();
        }

        // GET: /OwnerPlan/Edit/5
        [SessionExpire]
        [CustomAuthorize(Roles = "OwnerPlan/CanRead")]
        public ActionResult Edit(int? id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                OwnerPlan ownerplan = db.OwnerPlans.Find(id);
                if (ownerplan == null)
                {
                    return HttpNotFound();
                }
                ViewBag.StartDate1 = ownerplan.StartDate.ToString("dd/MM/yyyy hh:mm:ss tt");
                ViewBag.EndDate1 = ownerplan.EndDate.ToString("dd/MM/yyyy hh:mm:ss tt");
                FillOwner(ownerplan.OwnerID);
                var planType = (from bd in db.BusinessDetails
                                join f in db.Franchises on bd.ID equals f.BusinessDetailID
                                where f.ID == ownerplan.OwnerID
                                select bd.BusinessType.Prefix).FirstOrDefault();

                ViewBag.PlanID = new SelectList(db.Plans.Where(x => x.PlanCode.StartsWith(planType)), "ID", "ShortName", ownerplan.PlanID);
                ViewBag.CreateBy = new SelectList(db.PersonalDetails, "ID", "FirstName", ownerplan.CreateBy);
                ViewBag.ModifyBy = new SelectList(db.PersonalDetails, "ID", "FirstName", ownerplan.ModifyBy);
                return View(ownerplan);
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[OwnerPlanController][GET:Edit]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[OwnerPlanController][GET:Edit]",
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
            }
            return View();
        }

        // POST: /OwnerPlan/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [SessionExpire]
        [CustomAuthorize(Roles = "OwnerPlan/CanWrite")]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,PlanID,OwnerID,StartDate,EndDate,IsActive,CreateDate,CreateBy,ModifyDate,ModifyBy,NetworkIP,DeviceType,DeviceID")] OwnerPlan ownerplan, string StartDate1, string EndDate1)
        {
            using (var dbContextTransaction = db.Database.BeginTransaction())
            {
                try
                {
                    OwnerPlan lOwnerPlan = db.OwnerPlans.Find(ownerplan.ID);
                    if (lOwnerPlan == null)
                    {
                        return View("Error");
                    }
                    FillOwner(ownerplan.OwnerID);
                    //WriteToLogTable(lOwnerPlan, ModelLayer.Models.Enum.COMMAND.UPDATE);
                    DateTime? lStartDate = CommonFunctions.GetDateTime(StartDate1);
                    if (lStartDate != null)
                    {
                        ownerplan.StartDate = (DateTime)lStartDate;
                    }
                    DateTime? lEndDate = CommonFunctions.GetDateTime(EndDate1);
                    if (lEndDate != null)
                    {
                        ownerplan.EndDate = (DateTime)lEndDate;
                    }
                    ViewBag.StartDate1 = ownerplan.StartDate;
                    ViewBag.EndDate1 = ownerplan.EndDate;
                    if ((DateTime.Now > ownerplan.EndDate) && ownerplan.EndDate.Date.Equals(DateTime.Now.Date))
                    {
                        ownerplan.IsActive = false;
                        ownerplan.CreateDate = Convert.ToDateTime(lOwnerPlan.CreateDate);
                        ownerplan.CreateBy = Convert.ToInt64(lOwnerPlan.CreateBy);
                        ownerplan.ModifyDate = DateTime.UtcNow;
                        ownerplan.ModifyBy = CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(Session["ID"]));
                        TryUpdateModel(ownerplan);

                        if (ModelState.IsValid)
                        {
                            db.Entry(lOwnerPlan).CurrentValues.SetValues(ownerplan);
                            db.SaveChanges();
                            dbContextTransaction.Commit();
                            return RedirectToAction("Create", new { ownerId = ownerplan.OwnerID });
                            //return RedirectToAction("Index");
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("CustomError", "Set Today's date in End date for de-activation of this Plan!!");
                    }
                    ViewBag.CreateBy = new SelectList(db.PersonalDetails, "ID", "FirstName", ownerplan.CreateBy);
                    ViewBag.ModifyBy = new SelectList(db.PersonalDetails, "ID", "FirstName", ownerplan.ModifyBy);
                    ViewBag.PlanID = new SelectList(db.Plans, "ID", "ShortName", ownerplan.PlanID);
                    return View(ownerplan);
                }
                catch (BusinessLogicLayer.MyException myEx)
                {
                    BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                        + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                        + "[OwnerPlanController][POST:Edit]" + myEx.EXCEPTION_PATH,
                        BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
                }
                catch (Exception ex)
                {
                    BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                        + Environment.NewLine + ex.Message + Environment.NewLine
                        + "[OwnerPlanController][POST:Edit]",
                        BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
                }
            }
            return View();
        }

        // GET: /OwnerPlan/Delete/5
        [SessionExpire]
        [CustomAuthorize(Roles = "OwnerPlan/CanRead")]
        public ActionResult Delete(int? id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                OwnerPlan ownerplan = db.OwnerPlans.Find(id);
                if (ownerplan == null)
                {
                    return HttpNotFound();
                }
                FillOwner(ownerplan.OwnerID);
                GetOwnerName(ownerplan.OwnerID);
                var planType = (from bd in db.BusinessDetails
                                join f in db.Franchises on bd.ID equals f.BusinessDetailID
                                where f.ID == ownerplan.OwnerID
                                select bd.BusinessType.Prefix).FirstOrDefault();

                ViewBag.PlanID = new SelectList(db.Plans.Where(x => x.PlanCode.StartsWith(planType)), "ID", "ShortName", ownerplan.PlanID);

                OwnerPlanViewModel lOwnerPlan = new OwnerPlanViewModel();
                lOwnerPlan.ID = ownerplan.ID;
                lOwnerPlan.OwnerID = ownerplan.OwnerID;
                lOwnerPlan.OwnerName = @ViewBag.OwnerName;
                lOwnerPlan.StartDate = ownerplan.StartDate;
                lOwnerPlan.EndDate = ownerplan.EndDate;
                lOwnerPlan.IsActive = ownerplan.IsActive;
                lOwnerPlan.Plan = ownerplan.Plan;
                return View(lOwnerPlan);
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[OwnerPlanController][GET:Delete]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[OwnerPlanController][GET:Delete]",
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
            }
            return View();
        }

        // POST: /OwnerPlan/Delete/5
        [HttpPost, ActionName("Delete")]
        [SessionExpire]
        [CustomAuthorize(Roles = "OwnerPlan/CanWrite")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            using (var dbContextTransaction = db.Database.BeginTransaction())
            {
                try
                {
                    OwnerPlan ownerplan = db.OwnerPlans.Find(id);

                    WriteToLogTable(ownerplan, ModelLayer.Models.Enum.COMMAND.DELETE);

                    List<OwnerPlanCategoryCharge> listOpcc = db.OwnerPlanCategoryCharges.Where(x => x.OwnerPlanID == id).ToList();
                    foreach (OwnerPlanCategoryCharge opcc in listOpcc)
                    {
                        db.OwnerPlanCategoryCharges.Remove(opcc);
                        db.SaveChanges();
                    }

                    db.OwnerPlans.Remove(ownerplan);
                    db.SaveChanges();

                    dbContextTransaction.Commit();
                    return RedirectToAction("Index");
                }
                catch (BusinessLogicLayer.MyException myEx)
                {
                    BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                        + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                        + "[OwnerPlanController][POST:Delete]" + myEx.EXCEPTION_PATH,
                        BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
                }
                catch (Exception ex)
                {
                    BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                        + Environment.NewLine + ex.Message + Environment.NewLine
                        + "[OwnerPlanController][POST:Delete]",
                        BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
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

        private void CheckIsPreviousPlanExist(long ownerID)
        {
            try
            {
                List<OwnerPlan> opList = db.OwnerPlans.Where(x => x.OwnerID == ownerID && x.Plan.PlanCode.StartsWith("GBFR")).ToList();
                foreach (OwnerPlan op in opList)
                {
                    op.IsActive = false;
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw new BusinessLogicLayer.MyException("[CheckIsPreviousPlanExist]", "Can't Check Is Previous Plan Exist" + Environment.NewLine + ex.Message);
            }
        }
        protected void FillData(long? ownerId)
        {
            try
            {
                var plantype = (from bd in db.BusinessDetails
                                join f in db.Franchises on bd.ID equals f.BusinessDetailID
                                where f.ID == ownerId
                                select bd.BusinessType.Prefix).FirstOrDefault();
                ViewBag.PlanType = plantype;
                FillPlan(plantype);
                FillOwner(ownerId);
            }
            catch (Exception ex)
            {
                throw new BusinessLogicLayer.MyException("[FillData]", "Can't Fill Data! in Method !" + Environment.NewLine + ex.Message);
            }
        }

        protected void FillPlan(string planType)
        {
            try
            {
                ViewBag.PlanID = new SelectList(db.Plans.Where(x => x.PlanCode.StartsWith(planType)), "ID", "ShortName");
            }
            catch (Exception ex)
            {
                throw new BusinessLogicLayer.MyException("[FillPlan]", "Can't Fill Plan! in Method !" + Environment.NewLine + ex.Message);
            }
        }
        protected void FillOwner(long? ownerId)
        {
            try
            {
                if (ownerId != null)
                {
                    var lOwnerID = from p in db.PersonalDetails
                                   join bd in db.BusinessDetails on p.UserLoginID equals bd.UserLoginID
                                   join f in db.Franchises on bd.ID equals f.BusinessDetailID
                                   where f.ID == ownerId
                                   select new
                                   {
                                       Id = f.ID,
                                       Name = p.Salutation.Name + " " + p.FirstName + " " + p.LastName
                                   };
                    ViewBag.OwnerID = new SelectList(lOwnerID, "Id", "Name", ownerId);
                }
            }
            catch (Exception ex)
            {
                throw new BusinessLogicLayer.MyException("[FillOwner]", "Can't Fill Owner! in Method !" + Environment.NewLine + ex.Message);
            }
        }
        protected void GetOwnerName(long? ownerId)
        {
            try
            {
                ViewBag.OwnerName = (from p in db.PersonalDetails
                                     join bd in db.BusinessDetails on p.UserLoginID equals bd.UserLoginID
                                     join f in db.Franchises on bd.ID equals f.BusinessDetailID
                                     where f.ID == ownerId
                                     select p.Salutation.Name + " " + p.FirstName + " " + p.LastName).FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw new BusinessLogicLayer.MyException("[GetOwnerName]", "Can't Get Owner Name! in Method !" + Environment.NewLine + ex.Message);
            }
        }

        public void WriteToLogTable(OwnerPlan obj, ModelLayer.Models.Enum.COMMAND mode)
        {
            try
            {
                //Log Table Insertion
                //LogTable logTable = new LogTable();
                //logTable.TableName = "OwnerPlan";//table Name(Model Name)
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
                     + "[OwnerPlanController][WriteToLogTable]" + myEx.EXCEPTION_PATH,
                     BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[OwnerPlanController][WriteToLogTable]",
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
            }
        }
    }
}
