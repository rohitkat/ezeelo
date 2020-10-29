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

namespace Administrator.Controllers
{
     [SessionExpire]
    public class PreviewFeaturesController : Controller
    {
        private EzeeloDBContext db = new EzeeloDBContext();
        StringBuilder errStr = new StringBuilder("=====================================================================================" +
         Environment.NewLine
         + "ErrorLog Controller : PreviewFeaturesController" + Environment.NewLine);

        [SessionExpire]
        [CustomAuthorize(Roles = "PreviewFeatures/CanRead")]
        // GET: /PreviewFeatures/
        public ActionResult Index()
        {
            return View(db.PreviewFeatures.ToList());
        }

        [SessionExpire]
        [CustomAuthorize(Roles = "PreviewFeatures/CanRead")]
        // GET: /PreviewFeatures/Details/5
        public ActionResult Details(int? id)
        {
            PreviewFeature previewfeature = new PreviewFeature();
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                previewfeature = db.PreviewFeatures.Find(id);
                if (previewfeature == null)
                {
                    return HttpNotFound();
                }
                return View(previewfeature);
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
                ViewBag.Messaage = "Unable to Inserted Brand Detail ";
                return View(previewfeature);

            }
        }

        [SessionExpire]
        [CustomAuthorize(Roles = "PreviewFeatures/CanWrite")]
        // GET: /PreviewFeatures/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: /PreviewFeatures/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [SessionExpire]
        [CustomAuthorize(Roles = "PreviewFeatures/CanWrite")]
        public ActionResult Create([Bind(Include="ID,PreviewFeatureName,IsActive,CreateDate,CreateBy,ModifyDate,ModifyBy,NetworkIP,DeviceType,DeviceID")] PreviewFeature previewfeature)
        {
            try
            {
                previewfeature.CreateBy = CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(Session["ID"]));
                previewfeature.CreateDate = DateTime.UtcNow.AddHours(5.30);
                previewfeature.NetworkIP = BusinessLogicLayer.CommonFunctions.GetClientIP();
                if (db.PreviewFeatures.Where(x => x.PreviewFeatureName == previewfeature.PreviewFeatureName).Count() > 0)
                {
                    ViewBag.Messaage = "Can not Save because Preview Feature Name Already Exists..!";
                    return View(previewfeature);
                }
                if (ModelState.IsValid)
                {
                    db.PreviewFeatures.Add(previewfeature);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }

                return View(previewfeature);
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
                ViewBag.Messaage = "Unable to Inserted Brand Detail ";
                return View(previewfeature);

            }
        }

        [SessionExpire]
        [CustomAuthorize(Roles = "PreviewFeatures/CanWrite")]
        // GET: /PreviewFeatures/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PreviewFeature previewfeature = db.PreviewFeatures.Find(id);
            if (previewfeature == null)
            {
                return HttpNotFound();
            }
            return View(previewfeature);
        }

        // POST: /PreviewFeatures/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [SessionExpire]
        [CustomAuthorize(Roles = "PreviewFeatures/CanWrite")]
        public ActionResult Edit([Bind(Include="ID,PreviewFeatureName,IsActive,CreateDate,CreateBy,ModifyDate,ModifyBy,NetworkIP,DeviceType,DeviceID")] PreviewFeature previewfeature)
        {
            if (ModelState.IsValid)
            {
                db.Entry(previewfeature).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(previewfeature);
        }

        [SessionExpire]
        [CustomAuthorize(Roles = "PreviewFeatures/CanDelete")]
        // GET: /PreviewFeatures/Delete/5
        public ActionResult Delete(int? id)
        {
            PreviewFeature previewfeature = new PreviewFeature();
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                previewfeature = db.PreviewFeatures.Find(id);
                if (previewfeature == null)
                {
                    return HttpNotFound();
                }
                return View(previewfeature);
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
                ViewBag.Messaage = "Unable to Inserted Brand Detail ";
                return View(previewfeature);

            }
        }

        // POST: /PreviewFeatures/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [SessionExpire]
        [CustomAuthorize(Roles = "PreviewFeatures/CanDelete")]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                PreviewFeature previewfeature = db.PreviewFeatures.Find(id);
                db.PreviewFeatures.Remove(previewfeature);
                db.SaveChanges();
                return RedirectToAction("Index");
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
                ViewBag.Messaage = "Unable to Inserted Brand Detail ";
                return RedirectToAction("Index");

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
