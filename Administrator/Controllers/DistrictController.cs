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
using PagedList;
using PagedList.Mvc;
using System.Text;
using BusinessLogicLayer;
using Administrator.Models;

namespace  Administrator.Controllers
{

    /// <summary>
    /// Developed By :- Pradnyakar Badge
    /// To Create Master Entry of District Name
    /// </summary>
    [SessionExpire]
    public class DistrictController : Controller
    {
        private EzeeloDBContext db = new EzeeloDBContext();
        StringBuilder errStr = new StringBuilder("=====================================================================================" +
        Environment.NewLine
        + "ErrorLog Controller : DistrictController" + Environment.NewLine);

        // GET: /District/
        [CustomAuthorize(Roles = "District/CanRead")]
        public ActionResult Index(int? page)
        {
            try
            {
                //var cities = db.Districts.Include(c => c.Cities);
                //return View(cities.ToList().ToPagedList(page ?? 1, 10));
                var districts = db.Districts.Include(d => d.State);
                return View(districts.ToList());
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

        // GET: /District/Details/5
        [CustomAuthorize(Roles = "District/CanRead")]
        public ActionResult Details(long? id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                District district = db.Districts.Find(id);
                if (district == null)
                {
                    return HttpNotFound();
                }
                return View(district);
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

        // GET: /District/Create
        [CustomAuthorize(Roles = "District/CanWrite")]
        public ActionResult Create()
        {
            try
            {
                ViewBag.StateID = new SelectList(db.States, "ID", "Name");
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

        // POST: /District/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(Roles = "District/CanWrite")]
        public ActionResult Create([Bind(Include="ID,Name,StateID,IsActive")] District district)
        {
            try
            {
                district.CreateDate = DateTime.UtcNow.AddHours(5.30);
                district.CreateBy = CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(Session["ID"])); 
                district.ModifyDate = null;
                district.ModifyBy = null;
                district.NetworkIP = string.Empty;
                district.DeviceType = string.Empty;
                district.DeviceID = string.Empty;

                TryUpdateModel(district);

                if (ModelState.IsValid)
                {
                    db.Districts.Add(district);
                    db.SaveChanges();
                    //return RedirectToAction("Index");
                    ViewBag.Messaage = "District Detail Inserted Successfully";
                }

                ViewBag.StateID = new SelectList(db.States, "ID", "Name", district.StateID);
                return View(district);
            }
            catch (Exception ex)
            {
                errStr.Append("Method Name[Http Request] :- Create[HttpPost]" + Environment.NewLine +
                               "ON Dated" + DateTime.UtcNow.AddHours(5.30).TimeOfDay + Environment.NewLine +
                                   ex.Message.ToString() + Environment.NewLine +
                         "====================================================================================="
                               );
                //ViewBag.Message = "Sorry! Problem in customer registration!!";
                ModelState.AddModelError("Message", "Sorry! Problem in Generate Create View!!");
                ErrorLog.ErrorLogFile("Sorry! Problem in Generate Create View " + Environment.NewLine + errStr.ToString()
                    , ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);

                ViewBag.Messaage = "Unable to Insert District Detail ";
                return View(district);
            }
        }

        // GET: /District/Edit/5
        [CustomAuthorize(Roles = "District/CanWrite")]
        public ActionResult Edit(long? id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                District district = db.Districts.Find(id);
                if (district == null)
                {
                    return HttpNotFound();
                }
                ViewBag.StateID = new SelectList(db.States, "ID", "Name", district.StateID);
                return View(district);
            }
            catch (Exception ex)
            {
                errStr.Append("Method Name[Http Request] :- Edit[HttpGet]" + Environment.NewLine +
                               "ON Dated" + DateTime.UtcNow.AddHours(5.30).TimeOfDay + Environment.NewLine +
                                   ex.Message.ToString() + Environment.NewLine +
                         "====================================================================================="
                               );
                //ViewBag.Message = "Sorry! Problem in customer registration!!";
                ModelState.AddModelError("Message", "Sorry! Problem in Updating Record!!");
                ErrorLog.ErrorLogFile("Sorry! Problem in Updating Record!!" + Environment.NewLine + errStr.ToString()
                    , ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);

                return View();
            }
        }

        // POST: /District/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(Roles = "District/CanWrite")]
        public ActionResult Edit([Bind(Include="ID,Name,StateID,IsActive,CreateDate,CreateBy,ModifyDate,ModifyBy,NetworkIP,DeviceType,DeviceID")] District district)
        {
            try
            {
                District lDistrict = db.Districts.Find(district.ID);

                district.CreateDate = lDistrict.CreateDate;
                district.CreateBy = lDistrict.CreateBy;
                district.ModifyDate = DateTime.UtcNow.AddHours(5.30);
                district.ModifyBy = CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(Session["ID"])); 
                district.NetworkIP = string.Empty;
                district.DeviceType = string.Empty;
                district.DeviceID = string.Empty;

                TryUpdateModel(district);

                if (ModelState.IsValid)
                {
                    //db.Entry(district).State = EntityState.Modified;
                    db.Entry(lDistrict).CurrentValues.SetValues(district);
                    db.SaveChanges();
                    //return RedirectToAction("Index");
                    ViewBag.Messaage = "District Detail Updated Successfully";
                }
                ViewBag.StateID = new SelectList(db.States, "ID", "Name", district.StateID);
                return View(district);
            }
            catch (Exception ex)
            {
                errStr.Append("Method Name[Http Request] :- Edit[HttpPost]" + Environment.NewLine +
                               "ON Dated" + DateTime.UtcNow.AddHours(5.30).TimeOfDay + Environment.NewLine +
                                   ex.Message.ToString() + Environment.NewLine +
                         "====================================================================================="
                               );
                //ViewBag.Message = "Sorry! Problem in customer registration!!";
                ModelState.AddModelError("Message", "Sorry! Problem in Updating Record!!");
                ErrorLog.ErrorLogFile("Sorry! Problem in Updating Record!!" + Environment.NewLine + errStr.ToString()
                    , ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);

                ViewBag.Messaage = "Unable to Updated District Detail ";
                return View(district);
            }
        }

        // GET: /District/Delete/5
        [CustomAuthorize(Roles = "District/CanDelete")]
        public ActionResult Delete(long? id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                District district = db.Districts.Find(id);
                if (district == null)
                {
                    return HttpNotFound();
                }
                return View(district);
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

        // POST: /District/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(Roles = "District/CanDelete")]
        public ActionResult DeleteConfirmed(long id)
        {
            try
            {
                District district = db.Districts.Find(id);
                db.Districts.Remove(district);
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

                ViewBag.Messaage = "Unable to Delete District Detail ";
                return View(db.Districts.Where(x => x.ID == id).FirstOrDefault());
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
