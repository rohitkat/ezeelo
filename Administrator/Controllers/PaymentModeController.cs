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
using System.Text;
using BusinessLogicLayer;
using Administrator.Models;

namespace User.Models
{
    public class PaymentModeController : Controller
    {
        private EzeeloDBContext db = new EzeeloDBContext();

        // GET: /PaymentMode/
        [SessionExpire]
        [CustomAuthorize(Roles = "PaymentMode/CanRead")]
        public ActionResult Index()
        {
            try
            {
                return View(db.PaymentModes.ToList());
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[PaymentModeController][GET:Index]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[PaymentModeController][GET:Index]",
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
            }
            return View();
        }

        // GET: /PaymentMode/Details/5
        [SessionExpire]
       [CustomAuthorize(Roles = "PaymentMode/CanRead")]
        public ActionResult Details(int? id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                PaymentMode paymentmode = db.PaymentModes.Find(id);
                if (paymentmode == null)
                {
                    return HttpNotFound();
                }
                return View(paymentmode);
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[PaymentModeController][GET:Details]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[PaymentModeController][GET:Details]",
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
            }
            return View();
        }

        // GET: /PaymentMode/Create
        [SessionExpire]
       [CustomAuthorize(Roles = "PaymentMode/CanRead")]
        public ActionResult Create()
        {
            try
            {
                return View();
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[PaymentModeController][GET:Create]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[PaymentModeController][GET:Create]",
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
            }
            return View();
        }

        // POST: /PaymentMode/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [SessionExpire]
        [CustomAuthorize(Roles = "PaymentMode/CanWrite")]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="ID,Name,Description,IsActive,CreateDate,CreateBy,ModifyDate,ModifyBy,NetworkIP,DeviceType,DeviceID")] PaymentMode paymentmode)
        {
            try
            {
                if (db.PaymentModes.Count(x => x.Name == paymentmode.Name) > 0)
                {
                    ViewBag.ErrorMessage = "Duplication of Payment Mode";
                    return View(paymentmode);
                }

                paymentmode.CreateDate = DateTime.UtcNow;
                paymentmode.CreateBy = CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(Session["ID"]));
                if (ModelState.IsValid)
                {
                    db.PaymentModes.Add(paymentmode);
                    db.SaveChanges();
                    //return RedirectToAction("Index");
                    ViewBag.ErrorMessage = "Payment Mode Inserted successfully";
                    return View(paymentmode);
                }

                return View(paymentmode);
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[PaymentModeController][POST:Create]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);

                ViewBag.Messaage = "Unable to Insert payment mode Detail ";
                return View(paymentmode);
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[PaymentModeController][POST:Create]",
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);

                ViewBag.Messaage = "Unable to Insert Payment Mode Detail ";
                return View(paymentmode);
            }
            //return View();
        }

        // GET: /PaymentMode/Edit/5
        [SessionExpire]
        [CustomAuthorize(Roles = "PaymentMode/CanRead")]
        public ActionResult Edit(int? id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                PaymentMode paymentmode = db.PaymentModes.Find(id);
                if (paymentmode == null)
                {
                    return HttpNotFound();
                }
                return View(paymentmode);
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[PaymentModeController][GET:Edit]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[PaymentModeController][GET:Edit]",
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
            }
            return View();
        }

        // POST: /PaymentMode/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [SessionExpire]
        [CustomAuthorize(Roles = "PaymentMode/CanWrite")]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="ID,Name,Description,IsActive")] PaymentMode paymentmode)
        {
            using (var dbContextTransaction = db.Database.BeginTransaction())
            {
                try
                {
                    PaymentMode payment = db.PaymentModes.Find(paymentmode.ID);

                    //Log Table Insertion
                    //LogTable logTable = new LogTable();
                    //logTable.TableName = "PaymentMode";//table Name(Model Name)
                    //logTable.RecordXML = ModelLayer.Models.ObjectToXml.GetXMLFromObject(payment);
                    //logTable.TableRowID = payment.ID;
                    //logTable.Command = ModelLayer.Models.Enum.COMMAND.UPDATE.ToString();
                    //long? rowOwnerID = (payment.ModifyBy >= 0 ? payment.ModifyBy : payment.CreateBy);
                    //logTable.RowOwnerID = (long)rowOwnerID;
                    //logTable.CreateDate = DateTime.UtcNow;
                    //logTable.CreateBy = CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(Session["ID"]));//Session ID
                    //db.LogTables.Add(logTable);
                    /**************************************/
                    if (db.PaymentModes.Count(x => x.Name == paymentmode.Name && x.ID != paymentmode.ID) > 0)
                    {
                        ViewBag.ErrorMessage = "Duplication of Payment Mode";
                        return View(paymentmode);
                    }

                    paymentmode.CreateDate = payment.CreateDate;
                    paymentmode.CreateBy = payment.CreateBy;
                    paymentmode.ModifyDate = DateTime.UtcNow;
                    payment.ModifyBy = CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(Session["ID"]));
                    TryUpdateModel(paymentmode);
                    if (ModelState.IsValid)
                    {
                        // db.Entry(paymentmode).State = EntityState.Modified;
                        db.Entry(payment).CurrentValues.SetValues(paymentmode);
                        db.SaveChanges();
                        dbContextTransaction.Commit();
                        //return RedirectToAction("Index");
                        ViewBag.ErrorMessage = "Payment Mode Updated Successfully";
                        return View(paymentmode);
                    }
                    return View(paymentmode);
                }
                catch (BusinessLogicLayer.MyException myEx)
                {
                    BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                        + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                        + "[PaymentModeController][POST:Edit]" + myEx.EXCEPTION_PATH,
                        BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);

                    ViewBag.Messaage = "Unable to Update Payment Mode Detail ";
                    return View(paymentmode);
                }
                catch (Exception ex)
                {
                    BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                        + Environment.NewLine + ex.Message + Environment.NewLine
                        + "[PaymentModeController][POST:Edit]",
                        BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);

                    ViewBag.Messaage = "Unable to Update Payment Mode Detail ";
                    return View(paymentmode);
                }
            }
           // return View();
        }

        // GET: /PaymentMode/Delete/5
        public ActionResult Delete(int? id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                PaymentMode paymentmode = db.PaymentModes.Find(id);
                if (paymentmode == null)
                {
                    return HttpNotFound();

                }
                return View(paymentmode);
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[PaymentModeController][GET:Delete]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[PaymentModeController][GET:Delete]",
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
            }
            return View();
        }

        // POST: /PaymentMode/Delete/5
        [HttpPost, ActionName("Delete")]
        [SessionExpire]
        [CustomAuthorize(Roles = "PaymentMode/CanDelete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            using (var dbContextTransaction = db.Database.BeginTransaction())
            {
                try
                {
                    PaymentMode paymentmode = db.PaymentModes.Find(id);

                    //Log Table Insertion
                    //LogTable logTable = new LogTable();
                    //logTable.TableName = "PaymentMode";//table Name(Model Name)
                    //logTable.RecordXML = ModelLayer.Models.ObjectToXml.GetXMLFromObject(paymentmode);
                    //logTable.TableRowID = paymentmode.ID;
                    //logTable.Command = ModelLayer.Models.Enum.COMMAND.UPDATE.ToString();
                    //long? rowOwnerID = (paymentmode.ModifyBy >= 0 ? paymentmode.ModifyBy : paymentmode.CreateBy);
                    //logTable.RowOwnerID = (long)rowOwnerID;
                    //logTable.CreateDate = DateTime.UtcNow;
                    //logTable.CreateBy = CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(Session["ID"]));//Session ID
                    //db.LogTables.Add(logTable);
                    /**************************************/

                    db.PaymentModes.Remove(paymentmode);
                    db.SaveChanges();
                    dbContextTransaction.Commit();
                    //ViewBag.ErrorMessage = "Payment Mode Deleted Successfully";
                    //return View(paymentmode);
                    return RedirectToAction("Index");
                }
                catch (BusinessLogicLayer.MyException myEx)
                {
                    BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                        + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                        + "[PaymentModeController][POST:Delete]" + myEx.EXCEPTION_PATH,
                        BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);

                    ViewBag.Messaage = "Unable to Delete Payment Mode Detail ";
                    return View(db.PaymentModes.Where(x => x.ID == id).FirstOrDefault());
                }
                catch (Exception ex)
                {
                    BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                        + Environment.NewLine + ex.Message + Environment.NewLine
                        + "[PaymentModeController][POST:Delete]",
                        BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);

                    ViewBag.Messaage = "Unable to Delete Payment Mode Detail ";
                    return View(db.PaymentModes.Where(x => x.ID == id).FirstOrDefault());
                }
            }
           // return View();
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
