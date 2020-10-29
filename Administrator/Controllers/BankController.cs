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
//-----------------------------------------------------------------------
// <copyright file="ProductList.cs" company="Ezeelo Consumer Services Pvt. Ltd.">
//     Copyright (c) Ezeelo Consumer Services Pvt. Ltd. All rights reserved.
// </copyright>
// <author>Pradnyakar N. Badge</author>
//-----------------------------------------------------------------------
namespace  Administrator.Controllers
{
    /// <summary>
    /// Developed By : Pradnyakar Badge
    /// To Create Bank Name Record as Master Emtry
    /// </summary>
    public class BankController : Controller
    {
        private EzeeloDBContext db = new EzeeloDBContext();

        StringBuilder errStr = new StringBuilder("=====================================================================================" +
          Environment.NewLine
          + "ErrorLog Controller : BankController" + Environment.NewLine);
        // GET: /Bank/
        [SessionExpire]
        [CustomAuthorize(Roles = "Bank/CanRead")]        
        public ActionResult Index()
        {
            try
            {
                return View(db.Banks.OrderBy(x => x.Name).ToList());
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
        
        [SessionExpire]
        [CustomAuthorize(Roles = "Bank/CanRead")]
        // GET: /Bank/Details/5
        public ActionResult Details(int? id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Bank bank = db.Banks.Find(id);
                if (bank == null)
                {
                    return HttpNotFound();
                }
                return View(bank);
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

        [SessionExpire]
        [CustomAuthorize(Roles = "Bank/CanWrite")]
        // GET: /Bank/Create
        public ActionResult Create()
        {
            try
            {
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

        // POST: /Bank/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [SessionExpire]
        [CustomAuthorize(Roles = "Bank/CanWrite")]
        public ActionResult Create([Bind(Include = "ID,Name,IsActive")] Bank bank)
        {
            try
            {

                if (db.Banks.Where(x => x.Name == bank.Name).Count() > 0)
                {
                    ViewBag.lblError = "Bank name is already Exist..!!";
                    return View(bank);
                }


                bank.CreateDate = DateTime.UtcNow.AddHours(5.3);
                bank.CreateBy = CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(Session["ID"]));
                bank.NetworkIP = CommonFunctions.GetClientIP();

                if (ModelState.IsValid)
                {
                    db.Banks.Add(bank);
                    db.SaveChanges();
                   // return RedirectToAction("Index");
                }
                ViewBag.Messaage = "Bank Detail Created Successfully";
                return View(bank);
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
        [CustomAuthorize(Roles = "Bank/CanWrite")]
        // GET: /Bank/Edit/5
        public ActionResult Edit(int? id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Bank bank = db.Banks.Find(id);
                if (bank == null)
                {
                    return HttpNotFound();
                }
                return View(bank);
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

        // POST: /Bank/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [SessionExpire]
        [CustomAuthorize(Roles = "Bank/CanWrite")]
        public ActionResult Edit([Bind(Include = "ID,Name,IsActive")] Bank bank)
        {
            try
            {

                if (db.Banks.Where(x => x.Name == bank.Name && x.ID != bank.ID).Count() > 0)
                {
                    ViewBag.lblError = "Bank name is already Exist..!!";
                    return View(bank);
                }

                Bank lBank = db.Banks.Find(bank.ID);
                bank.CreateDate = lBank.CreateDate;
                bank.CreateBy = lBank.CreateBy;
                bank.ModifyDate = DateTime.UtcNow.AddHours(5.5);
                bank.ModifyBy = CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(Session["ID"]));
                bank.NetworkIP = CommonFunctions.GetClientIP();
                bank.DeviceType = string.Empty;
                bank.DeviceID = string.Empty;

                // TryUpdateModel(bank);

                if (ModelState.IsValid)
                {
                    db.Entry(lBank).CurrentValues.SetValues(bank);
                    db.SaveChanges();

                   // return RedirectToAction("Index");
                }
                ViewBag.Messaage = "Bank Detail Modified Successfully";
                return View(bank);
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

        // GET: /Bank/Delete/5
        [SessionExpire]
        [CustomAuthorize(Roles = "Bank/CanDelete")]
        public ActionResult Delete(int? id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                if (db.OwnerBanks.Where(x => x.BankID == id).Count() > 0)
                {
                    ViewBag.Messaage = "Bank Can't Be deleted:- Bank reference used in other records";
                }
                Bank bank = db.Banks.Find(id);
                if (bank == null)
                {
                    return HttpNotFound();
                }

                return View(bank);
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

        // POST: /Bank/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [SessionExpire]
        [CustomAuthorize(Roles = "Bank/CanDelete")]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                if (db.OwnerBanks.Where(x => x.BankID == id).Count() > 0)
                {
                    ViewBag.Messaage = "Bank Can't Be deleted:- Bank reference used in other records";
                    return View();
                }

                Bank bank = db.Banks.Find(id);
                db.Banks.Remove(bank);
                db.SaveChanges();
                ViewBag.Messaage = "Bank Detail Deleted Successfully";
                return RedirectToAction("Index");
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
