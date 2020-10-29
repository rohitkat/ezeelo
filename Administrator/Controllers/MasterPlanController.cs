//-----------------------------------------------------------------------
// <copyright file="ProductList.cs" company="Ezeelo Consumer Services Pvt. Ltd.">
//     Copyright (c) Ezeelo Consumer Services Pvt. Ltd. All rights reserved.
// </copyright>
// <author>Pradnyakar N. Badge</author>
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
using BusinessLogicLayer;
using System.Text;
using Administrator.Models;

namespace  Administrator.Controllers
{
    [SessionExpire]
    public class MasterPlanController : Controller
    {
        private EzeeloDBContext db = new EzeeloDBContext();
        
        // GET: /MasterPlan/
         [CustomAuthorize(Roles = "MasterPlan/CanRead")]
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
                    + "[MasterPlanController][GET:Index]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[MasterPlanController][GET:Index]",
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
            }
            return View();
        }

        // GET: /MasterPlan/Details/5
        [CustomAuthorize(Roles = "MasterPlan/CanRead")]
        public ActionResult Details(int? id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Plan plan = db.Plans.Find(id);
                if (plan == null)
                {
                    return HttpNotFound();
                }
                return View(plan);
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[MasterPlanController][GET:Details]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[MasterPlanController][GET:Details]",
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
            }
            return View();
        }

        // GET: /MasterPlan/Create
        [CustomAuthorize(Roles = "MasterPlan/CanRead")]
        public ActionResult Create()
        {
            try
            {
                ViewBag.PlanCode = new SelectList(db.BusinessTypes, "Prefix", "Name");
                return View();
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[MasterPlanController][GET:Create]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[MasterPlanController][GET:Create]",
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
            }
            return View();
        }

        // POST: /MasterPlan/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [CustomAuthorize(Roles = "MasterPlan/CanWrite")]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="ID,PlanCode,ShortName,NoOfEntitiesAllowed,Year,Month,Day,Description,Fees,IsActive")] Plan plan)
        {
            try
            {
                ViewBag.PlanCode = new SelectList(db.BusinessTypes, "Prefix", "Name");

                Int64 countCode = db.Plans.Count();
                plan.PlanCode = plan.PlanCode + DateTime.Now.Day.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Year.ToString() + countCode.ToString();

                plan.CreateBy = CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(Session["ID"])); 
                plan.CreateDate = DateTime.UtcNow.AddHours(5.30);

                if (ModelState.IsValid)
                {
                    db.Plans.Add(plan);
                    db.SaveChanges();
                    ViewBag.Messaage = "Plan Detail Inserted Successfully";
                }

                return View(plan);
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[MasterPlanController][POST:Create]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
                ViewBag.Messaage = "Unable to Insert Plan Detail ";
                return View(plan);
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[MasterPlanController][POST:Create]",
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);

                ViewBag.Messaage = "Unable to Insert Plan Detail ";
                return View(plan);
            }
            return View();
        }

        // GET: /MasterPlan/Edit/5
        [CustomAuthorize(Roles = "MasterPlan/CanRead")]
        public ActionResult Edit(int? id)
        {
            try
            {

                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Plan plan = db.Plans.Find(id);
                if (plan == null)
                {
                    return HttpNotFound();
                }


                return View(plan);
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[MasterPlanController][GET:Edit]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[MasterPlanController][GET:Edit]",
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
            }
            return View();
        }

        // POST: /MasterPlan/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [CustomAuthorize(Roles = "MasterPlan/CanWrite")]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="ID,PlanCode,ShortName,NoOfEntitiesAllowed,Year,Month,Day,Description,Fees,IsActive")] Plan plan)
        {
            using (var dbContextTransaction = db.Database.BeginTransaction())
            {
                try
                {
                    if (ModelState.IsValid)
                    {
                        Plan lData = db.Plans.Single(x => x.ID == plan.ID);

                        //Log Table Insertion
                        //LogTable logTable = new LogTable();
                        //logTable.TableName = "Plan";//table Name(Model Name)
                        //logTable.RecordXML = ModelLayer.Models.ObjectToXml.GetXMLFromObject(lData);
                        //logTable.TableRowID = lData.ID;
                        //logTable.Command = ModelLayer.Models.Enum.COMMAND.UPDATE.ToString();
                        //long? rowOwnerID = (lData.ModifyBy >= 0 ? lData.ModifyBy : lData.CreateBy);
                        //logTable.RowOwnerID = (long)rowOwnerID;
                        //logTable.CreateDate = DateTime.UtcNow;
                        //logTable.CreateBy = CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(Session["ID"]));//Session ID
                        //db.LogTables.Add(logTable);
                        /**************************************/

                        plan.CreateDate = lData.CreateDate;
                        plan.CreateBy = lData.CreateBy;
                        plan.ModifyDate = DateTime.UtcNow.AddHours(5.30);
                        plan.ModifyBy = CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(Session["ID"]));
                        db.Entry(lData).CurrentValues.SetValues(plan);
                        //db.Entry(plan).State = EntityState.Modified;
                        db.SaveChanges();
                        dbContextTransaction.Commit();
                        //return RedirectToAction("Index");
                        ViewBag.Messaage = "Plan Detail Updated Successfully";
                    }
                    return View(plan);
                }
                catch (BusinessLogicLayer.MyException myEx)
                {
                    BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                        + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                        + "[MasterPlanController][POST:Edit]" + myEx.EXCEPTION_PATH,
                        BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);

                    ViewBag.Messaage = "Unable to Insert Plan Detail ";
                    return View(plan);
                }
                catch (Exception ex)
                {
                    BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                        + Environment.NewLine + ex.Message + Environment.NewLine
                        + "[MasterPlanController][POST:Edit]",
                        BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);

                    ViewBag.Messaage = "Unable to Insert Plan Detail ";
                    return View(plan);

                }
            }
            return View();
        }

        // GET: /MasterPlan/Delete/5
        [CustomAuthorize(Roles = "MasterPlan/CanRead")]
        public ActionResult Delete(int? id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Plan plan = db.Plans.Find(id);
                if (plan == null)
                {
                    return HttpNotFound();
                }
                return View(plan);
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[MasterPlanController][GET:Delete]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[MasterPlanController][GET:Delete]",
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
            }
            return View();
        }

        // POST: /MasterPlan/Delete/5
        [HttpPost, ActionName("Delete")]
        [CustomAuthorize(Roles = "MasterPlan/CanWrite")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            using (var dbContextTransaction = db.Database.BeginTransaction())
            {
                try
                {
                    Plan plan = db.Plans.Find(id);

                    //Log Table Insertion
                    //LogTable logTable = new LogTable();
                    //logTable.TableName = "Plan";//table Name(Model Name)
                    //logTable.RecordXML = ModelLayer.Models.ObjectToXml.GetXMLFromObject(plan);
                    //logTable.TableRowID = plan.ID;
                    //logTable.Command = ModelLayer.Models.Enum.COMMAND.UPDATE.ToString();
                    //long? rowOwnerID = (plan.ModifyBy >= 0 ? plan.ModifyBy : plan.CreateBy);
                    //logTable.RowOwnerID = (long)rowOwnerID;
                    //logTable.CreateDate = DateTime.UtcNow;
                    //logTable.CreateBy = CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(Session["ID"]));//Session ID
                    //db.LogTables.Add(logTable);
                    /**************************************/
                    db.Plans.Remove(plan);
                    db.SaveChanges();
                    dbContextTransaction.Commit();
                    return RedirectToAction("Index");
                }
                catch (BusinessLogicLayer.MyException myEx)
                {
                    BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                        + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                        + "[MasterPlanController][POST:Delete]" + myEx.EXCEPTION_PATH,
                        BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);

                    ViewBag.Messaage = "Unable to Delete Plan Detail ";
                    return View(db.Plans.Where( x=> x.ID == id).FirstOrDefault());

                }
                catch (Exception ex)
                {
                    BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                        + Environment.NewLine + ex.Message + Environment.NewLine
                        + "[MasterPlanController][POST:Delete]",
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
    }
}
