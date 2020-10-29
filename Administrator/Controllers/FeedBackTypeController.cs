//-----------------------------------------------------------------------
// <copyright file="FeedBackTypeController.cs" company="Ezeelo Consumer Services Pvt. Ltd.">
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

namespace Administrator.Controllers
{
    [SessionExpire]
    public class FeedBackTypeController : Controller
    {
        private EzeeloDBContext db = new EzeeloDBContext();
        
        StringBuilder errStr = new StringBuilder("=====================================================================================" +
          Environment.NewLine
          + "ErrorLog Controller : FeedBackTypeController" + Environment.NewLine);

        // GET: /FeedBackType/
        [CustomAuthorize(Roles = "FeedBackType/CanRead")]
        public ActionResult Index()
        {
            try
            {
                var feedbacktypes = db.FeedBackTypes.Include(f => f.PersonalDetail).Include(f => f.PersonalDetail1);
                return View(feedbacktypes.ToList());
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

        // GET: /FeedBackType/Details/5
        [CustomAuthorize(Roles = "FeedBackType/CanRead")]
        public ActionResult Details(int? id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                FeedBackType feedbacktype = db.FeedBackTypes.Find(id);
                if (feedbacktype == null)
                {
                    return HttpNotFound();
                }
                return View(feedbacktype);
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
        
        // GET: /FeedBackType/Create
        [CustomAuthorize(Roles = "FeedBackType/CanWrite")]
        public ActionResult Create()
        {
            try
            {
                ViewBag.CreateBy = new SelectList(db.PersonalDetails, "ID", "FirstName");
                ViewBag.ModifyBy = new SelectList(db.PersonalDetails, "ID", "FirstName");
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

        // POST: /FeedBackType/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(Roles = "FeedBackType/CanWrite")]
        public ActionResult Create([Bind(Include="ID,Name,IsActive")] FeedBackType feedbacktype)
        {
            try
            {
                feedbacktype.CreateBy = CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(Session["ID"])); 
                feedbacktype.CreateDate = DateTime.UtcNow.AddHours(5.30);
                feedbacktype.NetworkIP = BusinessLogicLayer.CommonFunctions.GetClientIP();
                feedbacktype.DeviceID = string.Empty;
                feedbacktype.DeviceType = string.Empty;

                if (ModelState.IsValid)
                {
                    db.FeedBackTypes.Add(feedbacktype);
                    db.SaveChanges();
                    //return RedirectToAction("Index");
                    ViewBag.Messaage = "feedbacktype Detail Inserted Successfully";
                }

                ViewBag.CreateBy = new SelectList(db.PersonalDetails, "ID", "FirstName", feedbacktype.CreateBy);
                ViewBag.ModifyBy = new SelectList(db.PersonalDetails, "ID", "FirstName", feedbacktype.ModifyBy);
                return View(feedbacktype);
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

                ViewBag.Messaage = "Unable to Insert feedbacktype Detail ";
                return View(feedbacktype);
            }
        }

        // GET: /FeedBackType/Edit/5
        [CustomAuthorize(Roles = "FeedBackType/CanWrite")]
        public ActionResult Edit(int? id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                FeedBackType feedbacktype = db.FeedBackTypes.Find(id);
                if (feedbacktype == null)
                {
                    return HttpNotFound();
                }
                ViewBag.CreateBy = new SelectList(db.PersonalDetails, "ID", "FirstName", feedbacktype.CreateBy);
                ViewBag.ModifyBy = new SelectList(db.PersonalDetails, "ID", "FirstName", feedbacktype.ModifyBy);
                return View(feedbacktype);
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

        // POST: /FeedBackType/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(Roles = "FeedBackType/CanWrite")]
        public ActionResult Edit([Bind(Include="ID,Name,IsActive")] FeedBackType feedbacktype)
        {
            try
            {
                FeedBackType lfeedbacktype = db.FeedBackTypes.Find(feedbacktype.ID);


                feedbacktype.CreateBy = lfeedbacktype.CreateBy;
                feedbacktype.CreateDate = lfeedbacktype.CreateDate;
                feedbacktype.ModifyBy = CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(Session["ID"])); 
                feedbacktype.ModifyDate = DateTime.UtcNow.AddHours(5.30);
                feedbacktype.NetworkIP = BusinessLogicLayer.CommonFunctions.GetClientIP();
                feedbacktype.DeviceID = string.Empty;
                feedbacktype.DeviceType = string.Empty;

                if (ModelState.IsValid)
                {
                    db.Entry(lfeedbacktype).CurrentValues.SetValues(feedbacktype);
                    //db.Entry(feedbacktype).State = EntityState.Modified;
                    db.SaveChanges();
                    //return RedirectToAction("Index");
                    ViewBag.Messaage = "feedbacktype Detail Updated Successfully";
                }
                ViewBag.CreateBy = new SelectList(db.PersonalDetails, "ID", "FirstName", feedbacktype.CreateBy);
                ViewBag.ModifyBy = new SelectList(db.PersonalDetails, "ID", "FirstName", feedbacktype.ModifyBy);
                return View(feedbacktype);
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

                ViewBag.Messaage = "Unable to Updated feedbacktype Detail ";
                return View(feedbacktype);
            }
        }

        // GET: /FeedBackType/Delete/5
        [CustomAuthorize(Roles = "FeedBackType/CanDelete")]
        public ActionResult Delete(int? id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                FeedBackType feedbacktype = db.FeedBackTypes.Find(id);
                if (feedbacktype == null)
                {
                    return HttpNotFound();
                }
                return View(feedbacktype);
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

        // POST: /FeedBackType/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(Roles = "FeedBackType/CanDelete")]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                FeedBackType feedbacktype = db.FeedBackTypes.Find(id);
                db.FeedBackTypes.Remove(feedbacktype);
                db.SaveChanges();
                return RedirectToAction("Index");
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

                ViewBag.Messaage = "Unable to Delete feedbacktype Detail ";
                return View(db.FeedBackTypes.Where(x => x.ID == id).FirstOrDefault());
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
