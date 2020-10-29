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
    /// <summary>
    /// To Create Master Entry of Dimension Name
    /// </summary>
    [SessionExpire]
    public class DimensionController : Controller
    {
        private EzeeloDBContext db = new EzeeloDBContext();


        // GET: /Dimension/
        [CustomAuthorize(Roles = "Dimension/CanRead")]
        public ActionResult Index()
        {
            try
            {
                return View(db.Dimensions.ToList());
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Error", "There's Something wrong in loading Index!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[DimensionController][POST:Index]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
                return View();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Error", "There's Something wrong in loading Index!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[DimensionController][POST:Index]",
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
                return View();
            }
        }

        // GET: /Dimension/Details/5
        [CustomAuthorize(Roles = "Dimension/CanRead")]
        public ActionResult Details(int? id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Dimension dimension = db.Dimensions.Find(id);
                if (dimension == null)
                {
                    return HttpNotFound();
                }
                return View(dimension);
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Error", "There's Something wrong in Details view!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[DimensionController][GET:Details]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
                return View();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Error", "There's Something wrong in Details view!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[DimensionController][GET:Details]",
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
                return View();
            }
        }

        // GET: /Dimension/Create
        [CustomAuthorize(Roles = "Dimension/CanWrite")]
        public ActionResult Create()
        {
            try
            {
                return View();
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Error", "There's Something wrong Record Creation!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[DimensionController][GET:Create]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
                return View();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Error", "There's Something wrong in Record Creation!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[DimensionController][GET:Create]",
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
                return View();
            }
        }

        // POST: /Dimension/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(Roles = "Dimension/CanWrite")]
        public ActionResult Create([Bind(Include = "ID,Name,IsActive")] Dimension dimension)
        {
            try
            {
                if (db.Dimensions.Where(x => x.Name == dimension.Name).Count() > 0)
                {
                    ViewBag.Messaage = "Can not Save  because dimension Name Already Exists..!";
                    return View(dimension);
                }

                dimension.CreateBy = CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(Session["ID"]));
                dimension.CreateDate = DateTime.UtcNow.AddHours(5.30);
                dimension.NetworkIP = BusinessLogicLayer.CommonFunctions.GetClientIP();
                dimension.DeviceID = string.Empty;
                dimension.DeviceType = string.Empty;

                if (ModelState.IsValid)
                {
                    db.Dimensions.Add(dimension);
                    db.SaveChanges();
                    //return RedirectToAction("Index");
                    ViewBag.Messaage = "Dimension Detail Inserted Successfully";
                }

                return View(dimension);
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Error", "There's Something wrong Record Creation!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[DimensionController][POST:Create]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
                ViewBag.Messaage = "Unable to Inserted Dimension Detail ";
                return View(dimension);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Error", "There's Something wrong in Record Creation!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[DimensionController][POST:Create]",
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
                ViewBag.Messaage = "Unable to Inserted Dimension Detail ";
                return View(dimension);
            }
        }

        // GET: /Dimension/Edit/5
        [CustomAuthorize(Roles = "Dimension/CanWrite")]
        public ActionResult Edit(int? id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Dimension dimension = db.Dimensions.Find(id);
                if (dimension == null)
                {
                    return HttpNotFound();
                }
                return View(dimension);
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Error", "There's Something wrong Record Updation!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[DimensionController][GET:Edit]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
                return View();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Error", "There's Something wrong in Record Updation!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[DimensionController][GET:Edit]",
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
                return View();
            }
        }

        // POST: /Dimension/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(Roles = "Dimension/CanWrite")]
        public ActionResult Edit([Bind(Include = "ID,Name,IsActive")] Dimension dimension)
        {
            try
            {
                if (db.Dimensions.Where(x => x.Name == dimension.Name && x.ID != dimension.ID).Count() > 0)
                {
                    ViewBag.Messaage = "Can not Save  because dimension Name Already Exists..!";
                    return View(dimension);
                }

                Dimension lData = db.Dimensions.Single(x => x.ID == dimension.ID);


                dimension.CreateBy = lData.CreateBy;
                dimension.CreateDate = lData.CreateDate;
                dimension.ModifyBy = CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(Session["ID"]));
                dimension.ModifyDate = DateTime.UtcNow.AddHours(5.30);
                dimension.NetworkIP = BusinessLogicLayer.CommonFunctions.GetClientIP();
                dimension.DeviceID = string.Empty;
                dimension.DeviceType = string.Empty;


                if (ModelState.IsValid)
                {
                    db.Entry(lData).CurrentValues.SetValues(dimension);
                    //db.Entry(dimension).State = EntityState.Modified;
                    db.SaveChanges();
                    //return RedirectToAction("Index");
                    ViewBag.Messaage = "Dimension Detail Update Successfully";
                }
                return View(dimension);
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Error", "There's Something wrong Record Updation!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[DimensionController][POST:Edit]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
                ViewBag.Messaage = "Unable to Update Dimension Detail ";
                return View(dimension);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Error", "There's Something wrong in Record Updation!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[DimensionController][POST:Edit]",
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
                ViewBag.Messaage = "Unable to Update Dimension Detail ";
                return View(dimension);
            }
        }

        // GET: /Dimension/Delete/5
        [CustomAuthorize(Roles = "Dimension/CanDelete")]
        public ActionResult Delete(int? id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Dimension dimension = db.Dimensions.Find(id);
                if (dimension == null)
                {
                    return HttpNotFound();
                }
                return View(dimension);
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Error", "There's Something wrong Record Deletion!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[DimensionController][GET:Delete]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
                return View();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Error", "There's Something wrong in Record Deletion!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[DimensionController][GET:Delete]",
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
                return View();
            }
        }

        // POST: /Dimension/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(Roles = "Dimension/CanDelete")]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                Dimension dimension = db.Dimensions.Find(id);
                db.Dimensions.Remove(dimension);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Error", "There's Something wrong Record Deletion!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[DimensionController][POST:Delete]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
                ViewBag.Messaage = "Unable to Delete Dimension Detail ";
                return View(db.Dimensions.Where(x => x.ID == id).FirstOrDefault());
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Error", "There's Something wrong in Record Deletion!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[DimensionController][POST:Delete]",
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
                ViewBag.Messaage = "Unable to Delete Dimension Detail ";
                return View(db.Dimensions.Where(x => x.ID == id).FirstOrDefault());
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
