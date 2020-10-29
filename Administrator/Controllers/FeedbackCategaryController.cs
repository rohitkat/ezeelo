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
    public class FeedbackCategaryController : Controller
    {
        private EzeeloDBContext db = new EzeeloDBContext();

        StringBuilder errStr = new StringBuilder("=====================================================================================" +
         Environment.NewLine
         + "ErrorLog Controller : FeedbackCategaryController" + Environment.NewLine);

        // GET: /FeedbackCategary/
        [CustomAuthorize(Roles = "FeedbackCategary/CanRead")]
        public ActionResult Index()
        {
            try
            {
                var feedbackcategaries = db.FeedbackCategaries.Include(f => f.PersonalDetail).Include(f => f.PersonalDetail1);
                return View(feedbackcategaries.ToList());
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

        // GET: /FeedbackCategary/Details/5
        [CustomAuthorize(Roles = "FeedbackCategary/CanRead")]
        public ActionResult Details(int? id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                FeedbackCategary feedbackcategary = db.FeedbackCategaries.Find(id);
                if (feedbackcategary == null)
                {
                    return HttpNotFound();
                }
                return View(feedbackcategary);
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

        // GET: /FeedbackCategary/Create
        [CustomAuthorize(Roles = "FeedbackCategary/CanWrite")]
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

        // POST: /FeedbackCategary/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(Roles = "FeedbackCategary/CanWrite")]
        public ActionResult Create([Bind(Include="ID,Name,IsActive")] FeedbackCategary feedbackcategary)
        {
            try
            {
                feedbackcategary.CreateBy = CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(Session["ID"])); 
                feedbackcategary.CreateDate = DateTime.UtcNow.AddHours(5.30);
                feedbackcategary.NetworkIP = BusinessLogicLayer.CommonFunctions.GetClientIP();
                feedbackcategary.DeviceID = string.Empty;
                feedbackcategary.DeviceType = string.Empty;

                if (ModelState.IsValid)
                {
                    db.FeedbackCategaries.Add(feedbackcategary);
                    db.SaveChanges();
                    //return RedirectToAction("Index");
                    ViewBag.Messaage = "FeedbackCategary Detail Inserted Successfully";
                }

                ViewBag.CreateBy = new SelectList(db.PersonalDetails, "ID", "FirstName", feedbackcategary.CreateBy);
                ViewBag.ModifyBy = new SelectList(db.PersonalDetails, "ID", "FirstName", feedbackcategary.ModifyBy);
                return View(feedbackcategary);
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

                ViewBag.Messaage = "Unable to Insert feedbackcategary Detail ";
                return View(feedbackcategary);
            }
        }

        // GET: /FeedbackCategary/Edit/5
        [CustomAuthorize(Roles = "FeedbackCategary/CanWrite")]
        public ActionResult Edit(int? id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                FeedbackCategary feedbackcategary = db.FeedbackCategaries.Find(id);
                if (feedbackcategary == null)
                {
                    return HttpNotFound();
                }
                ViewBag.CreateBy = new SelectList(db.PersonalDetails, "ID", "FirstName", feedbackcategary.CreateBy);
                ViewBag.ModifyBy = new SelectList(db.PersonalDetails, "ID", "FirstName", feedbackcategary.ModifyBy);
                return View(feedbackcategary);
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

        // POST: /FeedbackCategary/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="ID,Name,IsActive")] FeedbackCategary feedbackcategary)
        {
            try
            {
                FeedbackCategary lfeedbackcategary = db.FeedbackCategaries.Find(feedbackcategary.ID);


                feedbackcategary.CreateBy = lfeedbackcategary.CreateBy;
                feedbackcategary.CreateDate = lfeedbackcategary.CreateDate;
                feedbackcategary.ModifyBy = CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(Session["ID"])); 
                feedbackcategary.ModifyDate = DateTime.UtcNow.AddHours(5.30);
                feedbackcategary.NetworkIP = BusinessLogicLayer.CommonFunctions.GetClientIP();
                feedbackcategary.DeviceID = string.Empty;
                feedbackcategary.DeviceType = string.Empty;

                if (ModelState.IsValid)
                {
                    db.Entry(lfeedbackcategary).CurrentValues.SetValues(feedbackcategary);
                   // db.Entry(feedbackcategary).State = EntityState.Modified;
                    db.SaveChanges();
                    //return RedirectToAction("Index");
                    ViewBag.Messaage = "feedbackcategary Detail Updated Successfully";
                }
                ViewBag.CreateBy = new SelectList(db.PersonalDetails, "ID", "FirstName", feedbackcategary.CreateBy);
                ViewBag.ModifyBy = new SelectList(db.PersonalDetails, "ID", "FirstName", feedbackcategary.ModifyBy);
                return View(feedbackcategary);
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

                ViewBag.Messaage = "Unable to Updated feedbackcategary Detail ";
                return View(feedbackcategary);
            }
        }

        // GET: /FeedbackCategary/Delete/5
        [CustomAuthorize(Roles = "FeedbackCategary/CanDelete")]
        public ActionResult Delete(int? id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                FeedbackCategary feedbackcategary = db.FeedbackCategaries.Find(id);
                if (feedbackcategary == null)
                {
                    return HttpNotFound();
                }
                return View(feedbackcategary);
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

        // POST: /FeedbackCategary/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(Roles = "FeedbackCategary/CanDelete")]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                FeedbackCategary feedbackcategary = db.FeedbackCategaries.Find(id);
                db.FeedbackCategaries.Remove(feedbackcategary);
                db.SaveChanges();
                return RedirectToAction("Index");
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

                ViewBag.Messaage = "Unable to Delete FeedbackCategaries Detail ";
                return View(db.FeedbackCategaries.Where(x => x.ID == id).FirstOrDefault());
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
