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

namespace  Administrator.Controllers
{
    [SessionExpire]
    public class SizeController : Controller
    {
        private EzeeloDBContext db = new EzeeloDBContext();

        StringBuilder errStr = new StringBuilder("=====================================================================================" +
        Environment.NewLine
        + "ErrorLog Controller : SizeController" + Environment.NewLine);

        // GET: /Size/
        [CustomAuthorize(Roles = "Size/CanRead")]
        public ActionResult Index()
        {
            try
            {
            return View(db.Sizes.ToList());
        }
            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Error", "Sorry! Problem in Generate Index view!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[SizeController][GET:Index]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
                return View();
            }
            catch (Exception ex)
            {

                ModelState.AddModelError("Error", "Sorry! Problem in Generate Index view!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[SizeController][GET:Index]",
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
                return View();
            }
        }

        // GET: /Size/Details/5
        [CustomAuthorize(Roles = "Size/CanRead")]
        public ActionResult Details(int? id)
        {
            try
            {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Size size = db.Sizes.Find(id);
            if (size == null)
            {
                return HttpNotFound();
            }
            return View(size);
        }
            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Error", "Sorry! Problem in Record Detail!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[SizeController][GET:Details]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
                return View();
            }
            catch (Exception ex)
            {

                ModelState.AddModelError("Error", "Sorry! Problem in Record Detail!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[SizeController][GET:Details]",
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
                return View();
            }
        }

        // GET: /Size/Create
        [CustomAuthorize(Roles = "Size/CanWrite")]
        public ActionResult Create()
        {
            try
            {
                return View();
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Error", "Sorry! Problem in Generate Create View!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[SizeController][GET:Create]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
                return View();
            }
            catch (Exception ex)
            {

                ModelState.AddModelError("Error", "Sorry! Problem in Generate Create View!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[SizeController][GET:Create]",
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
            return View();
        }
        }

        // POST: /Size/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(Roles = "Size/CanWrite")]
        public ActionResult Create([Bind(Include="ID,Name,IsActive")] Size size)
        {
            try
            {

                if (db.Sizes.Where(x => x.Name == size.Name).Count() > 0)
                {
                    ViewBag.Messaage = "Can not Save  because size Name Already Exists..!";
                    return View(size);
                }

                size.CreateBy = CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(Session["ID"]));
                size.CreateDate = DateTime.UtcNow.AddHours(5.30);
                size.NetworkIP = BusinessLogicLayer.CommonFunctions.GetClientIP();
                size.DeviceID = string.Empty;
                size.DeviceType = string.Empty;

                if (ModelState.IsValid)
                {
                    db.Sizes.Add(size);
                    db.SaveChanges();
                    //return RedirectToAction("Index");
                    ViewBag.Messaage = "Size Inserted Successfully";
                    return View(size);
                }

                return View(size);
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Error", "Sorry! Problem in Generate Create View!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[SizeController][POST:Create]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);

                ViewBag.Messaage = "Unable to Insert size Detail ";
                return View(size);
            }
            catch (Exception ex)
            {

                ModelState.AddModelError("Error", "Sorry! Problem in Generate Create View!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[SizeController][POST:Create]",
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
                ViewBag.Messaage = "Unable to Insert size Detail ";
                return View(size);
            }
        }

        // GET: /Size/Edit/5
        [CustomAuthorize(Roles = "Size/CanWrite")]
        public ActionResult Edit(int? id)
        {
            try
            {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Size size = db.Sizes.Find(id);
            if (size == null)
            {
                return HttpNotFound();
            }
            return View(size);
        }
            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Error", "Sorry! Problem in Record Update!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[SizeController][GET:Edit]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
                return View();
            }
            catch (Exception ex)
            {

                ModelState.AddModelError("Error", "Sorry! Problem in Record Update!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[SizeController][GET:Edit]",
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
                return View();
            }
        }

        // POST: /Size/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(Roles = "Size/CanWrite")]
        public ActionResult Edit([Bind(Include = "ID,Name,IsActive")] Size size)
        {
            try
            {

                if (db.Sizes.Where(x => x.Name == size.Name && x.ID != size.ID).Count() > 0)
                {
                    ViewBag.Messaage = "Can not Save  because size Name Already Exists..!";
                    return View(size);
                }

                Size lData = db.Sizes.Single(x => x.ID == size.ID);


                size.CreateBy = lData.CreateBy;
                size.CreateDate = lData.CreateDate;
                size.ModifyBy = CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(Session["ID"]));
                size.ModifyDate = DateTime.UtcNow.AddHours(5.30);
                size.NetworkIP = BusinessLogicLayer.CommonFunctions.GetClientIP();
                size.DeviceID = string.Empty;
                size.DeviceType = string.Empty;

                if (ModelState.IsValid)
                {
                    db.Entry(lData).CurrentValues.SetValues(size);
                    //db.Entry(size).State = EntityState.Modified;
                    db.SaveChanges();
                    //return RedirectToAction("Index");
                    ViewBag.Messaage = "Size Updated Successfully";
                }
                return View(size);
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Error", "Sorry! Problem in Record Update!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[SizeController][POST:Edit]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
                ViewBag.Messaage = "Unable to Update Size ";
                return View(size);
            }
            catch (Exception ex)
            {

                ModelState.AddModelError("Error", "Sorry! Problem in Record Update!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[SizeController][POST:Edit]",
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
                ViewBag.Messaage = "Unable to Update Size ";
                return View(size);
            }
        }
                
        // GET: /Size/Delete/5
        [CustomAuthorize(Roles = "Size/CanRead")]
        public ActionResult Delete(int? id)
        {
            try
            {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Size size = db.Sizes.Find(id);
            if (size == null)
            {
                return HttpNotFound();
            }
            return View(size);
        }
            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Error", "Sorry! Problem in Record Deletion!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[SizeController][GET:Delete]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
                return View();
            }
            catch (Exception ex)
            {

                ModelState.AddModelError("Error", "Sorry! Problem in Record Deletion!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[SizeController][GET:Delete]",
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
                return View();
            }
        }

        // POST: /Size/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(Roles = "Size/CanRead")]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
            Size size = db.Sizes.Find(id);
            db.Sizes.Remove(size);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Error", "Sorry! Problem in Record Deletion!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[SizeController][POST:Delete]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
                return View();
            }
            catch (Exception ex)
            {

                ModelState.AddModelError("Error", "Sorry! Problem in Record Deletion!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[SizeController][POST:Delete]",
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
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
