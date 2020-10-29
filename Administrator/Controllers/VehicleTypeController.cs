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

namespace Administrator.Controllers
{
    [SessionExpire]
    public class VehicleTypeController : Controller
    {
        private EzeeloDBContext db = new EzeeloDBContext();
        // GET: /VehicleType/
        [CustomAuthorize(Roles = "VehicleType/CanRead")]
        public ActionResult Index()
        {
            try
            {
                return View(db.VehicleTypes.ToList());
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Message", "Sorry! Problem in Generate Index view!!");
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[VehicleTypeController][GET:Index]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
                return View();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Message", "Sorry! Problem in Generate Index view!!");
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[VehicleTypeController][GET:Index]",
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
                return View();
            }
        }

        // GET: /VehicleType/Details/5
        [CustomAuthorize(Roles = "VehicleType/CanRead")]
        public ActionResult Details(int? id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                VehicleType vehicletype = db.VehicleTypes.Find(id);
                if (vehicletype == null)
                {
                    return HttpNotFound();
                }
                return View(vehicletype);
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Message", "Sorry! Problem in Record Detail!!");
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[VehicleTypeController][GET:Details]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
                return View();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Message", "Sorry! Problem in Record Detail!!");
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[VehicleTypeController][GET:Details]",
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
                return View();
            }
        }

        // GET: /VehicleType/Create
        [CustomAuthorize(Roles = "VehicleType/CanWrite")]
        public ActionResult Create()
        {
            try
            {
                return View();
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Message", "Sorry! Problem in Generate Create View!!");
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[VehicleTypeController][GET:Create]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
                return View();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Message", "Sorry! Problem in Generate Create View!!");
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[VehicleTypeController][GET:Create]",
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
                return View();
            }
        }

        // POST: /VehicleType/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(Roles = "VehicleType/CanWrite")]
        public ActionResult Create([Bind(Include = "ID,Name,IsActive")] VehicleType vehicletype)
        {
            try
            {
                if (db.VehicleTypes.Where(x => x.Name == vehicletype.Name).Count() > 0)
                {
                    ViewBag.Messaage = "Can not save because vehical type already exists...!";
                    return View(vehicletype);
                }

                vehicletype.CreateBy = CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(Session["ID"]));
                vehicletype.CreateDate = DateTime.UtcNow.AddHours(5.30);
                vehicletype.DeviceType = string.Empty;
                vehicletype.NetworkIP = BusinessLogicLayer.CommonFunctions.GetClientIP();

                if (ModelState.IsValid)
                {
                    db.VehicleTypes.Add(vehicletype);
                    db.SaveChanges();
                   // return RedirectToAction("Index");
                    ViewBag.Messaage = "vehicle type Inserted Successfully";
                    return View(vehicletype);
                }

                return View(vehicletype);
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Message", "Sorry! Problem in Generate Create View!!");
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[VehicleTypeController][POST:Create]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
                ViewBag.Messaage = "Unable to Insert vehicle type Detail ";
                return View(vehicletype);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Message", "Sorry! Problem in Generate Create View!!");
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[VehicleTypeController][POST:Create]",
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
                ViewBag.Messaage = "Unable to Insert vehicle type Detail ";
                return View(vehicletype);
            }
        }

        // GET: /VehicleType/Edit/5
        [CustomAuthorize(Roles = "VehicleType/CanWrite")]
        public ActionResult Edit(int? id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                VehicleType vehicletype = db.VehicleTypes.Find(id);
                if (vehicletype == null)
                {
                    return HttpNotFound();
                }
                return View(vehicletype);
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Message", "Sorry! Problem in Generate Create View!!");
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[VehicleTypeController][GET:Edit]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
                return View();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Message", "Sorry! Problem in Generate Create View!!");
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[VehicleTypeController][GET:Edit]",
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
                return View();
            }

        }

        // POST: /VehicleType/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(Roles = "VehicleType/CanWrite")]
        public ActionResult Edit([Bind(Include = "ID,Name,IsActive")] VehicleType vehicletype)
        {
            try
            {
                if (db.VehicleTypes.Where(x => x.Name == vehicletype.Name && x.ID != vehicletype.ID).Count() > 0)
                {
                    ViewBag.Messaage = "Can not save because vehical type already exists...!";
                    return View(vehicletype);
                }

                VehicleType lData = db.VehicleTypes.Single(x => x.ID == vehicletype.ID);

                vehicletype.CreateBy = lData.CreateBy;
                vehicletype.CreateDate = lData.CreateDate;
                vehicletype.DeviceType = string.Empty;
                vehicletype.ModifyDate = DateTime.UtcNow.AddHours(5.30);
                vehicletype.ModifyBy = CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(Session["ID"]));
                vehicletype.NetworkIP = BusinessLogicLayer.CommonFunctions.GetClientIP();
                if (ModelState.IsValid)
                {
                    //db.Entry(vehicletype).State = EntityState.Modified;
                    db.Entry(lData).CurrentValues.SetValues(vehicletype);
                    db.SaveChanges();
                    //return RedirectToAction("Index");
                    ViewBag.Messaage = "vehicle type Updated Successfully";
                    return View(vehicletype);
                }
                return View(vehicletype);
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Message", "Sorry! Problem in Record Update!!");
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[VehicleTypeController][POST:Edit]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
                ViewBag.Messaage = "Unable to Update vehicle type Detail ";
                return View(vehicletype);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Message", "Sorry! Problem in Record Update!!");
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[VehicleTypeController][POST:Edit]",
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);

                ViewBag.Messaage = "Unable to Update vehicle type Detail ";
                return View(vehicletype);
            }
        }

        // GET: /VehicleType/Delete/5
        [CustomAuthorize(Roles = "VehicleType/CanDelete")]
        public ActionResult Delete(int? id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                VehicleType vehicletype = db.VehicleTypes.Find(id);
                if (vehicletype == null)
                {
                    return HttpNotFound();
                }
                return View(vehicletype);
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Message", "Sorry! Problem in Record Deletion!!");
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[VehicleTypeController][GET:Delete]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
                return View();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Message", "Sorry! Problem in Record Deletion!!");
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[VehicleTypeController][GET:Delete]",
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
                return View();
            }
        }

        // POST: /VehicleType/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(Roles = "VehicleType/CanDelete")]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                VehicleType vehicletype = db.VehicleTypes.Find(id);
                db.VehicleTypes.Remove(vehicletype);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Message", "Sorry! Problem in Record Deletion!!");
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[VehicleTypeController][POST:Delete]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
                ViewBag.Messaage = "Unable to Delet vehicle type Detail ";
                return View(db.VehicleTypes.Where(x => x.ID == id).FirstOrDefault());
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Message", "Sorry! Problem in Record Deletion!!");
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[VehicleTypeController][POST:Delete]",
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
                ViewBag.Messaage = "Unable to Delet vehicle type Detail ";
                return View(db.VehicleTypes.Where(x => x.ID == id).FirstOrDefault());
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
