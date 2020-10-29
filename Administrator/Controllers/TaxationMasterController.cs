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
    public class TaxationMasterController : Controller
    {
        private EzeeloDBContext db = new EzeeloDBContext();

        StringBuilder errStr = new StringBuilder("=====================================================================================" +
         Environment.NewLine
         + "ErrorLog Controller : TaxationMasterController" + Environment.NewLine);

        // GET: /TaxationMaster/
        [SessionExpire]
        [CustomAuthorize(Roles = "TaxationMaster/CanRead")]
        public ActionResult Index()
        {
            try
            {
                return View(db.TaxationMasters.ToList());

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

        // GET: /TaxationMaster/Details/5
        [SessionExpire]
        [CustomAuthorize(Roles = "TaxationMaster/CanRead")]
        public ActionResult Details(int? id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                TaxationMaster taxationmaster = db.TaxationMasters.Find(id);
                if (taxationmaster == null)
                {
                    return HttpNotFound();
                }
                return View(taxationmaster);
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

        // GET: /TaxationMaster/Create
        [SessionExpire]
        [CustomAuthorize(Roles = "TaxationMaster/CanWrite")]
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

        // POST: /TaxationMaster/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [SessionExpire]
        [CustomAuthorize(Roles = "TaxationMaster/CanWrite")]
        public ActionResult Create([Bind(Include = "ID,Prefix,Name,Description,IsActive,CreateDate,CreateBy,ModifyDate,ModifyBy,NetworkIP,DeviceType,DeviceID")] TaxationMaster taxationmaster)
        {
            try
            {

                if (db.TaxationMasters.Where(x => x.Name.Trim().ToUpper() == taxationmaster.Name.Trim().ToUpper()).Count() > 0)
                {
                    ViewBag.Message = "Tax Prefix Already Exists";
                }
                else
                {
                    taxationmaster.Prefix = taxationmaster.Prefix.ToUpper().Trim();
                    taxationmaster.CreateDate = DateTime.UtcNow.AddHours(5.3);
                    taxationmaster.CreateBy = CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(Session["ID"]));
                    taxationmaster.NetworkIP = CommonFunctions.GetClientIP();


                    if (ModelState.IsValid)
                    {
                        db.TaxationMasters.Add(taxationmaster);
                        db.SaveChanges();
                        return RedirectToAction("Index");
                    }
                }
                return View(taxationmaster);
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

        // GET: /TaxationMaster/Edit/5
        [SessionExpire]
        [CustomAuthorize(Roles = "TaxationMaster/CanWrite")]
        public ActionResult Edit(int? id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                TaxationMaster taxationmaster = db.TaxationMasters.Find(id);
                if (taxationmaster == null)
                {
                    return HttpNotFound();
                }
                return View(taxationmaster);
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

        // POST: /TaxationMaster/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [SessionExpire]
        [CustomAuthorize(Roles = "TaxationMaster/CanWrite")]
        public ActionResult Edit([Bind(Include="ID,Prefix,Name,Description,IsActive,CreateDate,CreateBy,ModifyDate,ModifyBy,NetworkIP,DeviceType,DeviceID")] TaxationMaster taxationmaster)
        {
            try
            {
                
                if (db.TaxationMasters.Where(x => x.Name.Trim().ToUpper() == taxationmaster.Name.Trim().ToUpper() && x.ID != taxationmaster.ID).Count() > 0)
                {
                    ViewBag.Message = "Tax Prefix Already Exists";
                }
                else
                {

                    TaxationMaster ltaxationmaster = db.TaxationMasters.Find(taxationmaster.ID);
                    taxationmaster.Prefix = taxationmaster.Prefix.ToUpper().Trim();
                    taxationmaster.CreateDate = ltaxationmaster.CreateDate;
                    taxationmaster.CreateBy = ltaxationmaster.CreateBy;
                    taxationmaster.ModifyDate = DateTime.UtcNow.AddHours(5.5);
                    taxationmaster.ModifyBy = CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(Session["ID"]));
                    taxationmaster.NetworkIP = CommonFunctions.GetClientIP();
                    taxationmaster.DeviceType = string.Empty;
                    taxationmaster.DeviceID = string.Empty;

                    if (ModelState.IsValid)
                    {
                        // db.Entry(taxationmaster).State = EntityState.Modified;
                        db.Entry(ltaxationmaster).CurrentValues.SetValues(taxationmaster);
                        db.SaveChanges();
                        return RedirectToAction("Index");
                    }
                }
                return View(taxationmaster);
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

        // GET: /TaxationMaster/Delete/5
        [SessionExpire]
        [CustomAuthorize(Roles = "TaxationMaster/CanDelete")]
        public ActionResult Delete(int? id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                TaxationMaster taxationmaster = db.TaxationMasters.Find(id);
                if (taxationmaster == null)
                {
                    return HttpNotFound();
                }
                return View(taxationmaster);
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

        // POST: /TaxationMaster/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [SessionExpire]
        [CustomAuthorize(Roles = "TaxationMaster/CanDelete")]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                TaxationMaster taxationmaster = db.TaxationMasters.Find(id);
                db.TaxationMasters.Remove(taxationmaster);
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
