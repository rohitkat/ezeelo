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
using BusinessLogicLayer;
using System.Text;
using Administrator.Models;
namespace  Administrator.Controllers
{
    /// <summary>
    /// To Create Master Entry of Color Name
    /// </summary>
    [SessionExpire]
    public class ColorController : Controller
    {
        private EzeeloDBContext db = new EzeeloDBContext();
      
        // GET: /Color/
        [SessionExpire]
        [CustomAuthorize(Roles = "Color/CanRead")]
        public ActionResult Index()
        {
            try
            {
                return View(db.Colors.OrderBy(x => x.Name).ToList());
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Error", "Sorry! Problem in Generate Index view!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[ColorController][GET:Index]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
                return View();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Error", "Sorry! Problem in Generate Index view!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[ColorController][GET:Index]",
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
                return View();
            }
        }

        // GET: /Color/Details/5
        [SessionExpire]
        [CustomAuthorize(Roles = "Color/CanRead")]
        public ActionResult Details(int? id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Color color = db.Colors.Find(id);
                if (color == null)
                {
                    return HttpNotFound();
                }
                return View(color);
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Error", "Sorry! Problem in Generate Details view!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[ColorController][GET:Details]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
                return View();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Error", "Sorry! Problem in Generate Details view!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[ColorController][GET:Details]",
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
                return View();
            }
        }

        // GET: /Color/Create
        [SessionExpire]
        [CustomAuthorize(Roles = "Color/CanWrite")]
        public ActionResult Create()
        {
            try
            {
                return View();
            }

            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Error", "Sorry! Problem in Generate Create view!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[ColorController][GET:Create]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
                return View();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Error", "Sorry! Problem in Generate Create view!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[ColorController][GET:Create]",
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
                return View();
            }
          
        }

        // POST: /Color/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [SessionExpire]
        [CustomAuthorize(Roles = "Color/CanWrite")]
        public ActionResult Create([Bind(Include="ID,HtmlCode,Name,IsActive")] Color color)
        {
            try
            {
                if (db.Colors.Where(x => x.Name == color.Name).Count() > 0)
                {
                    ViewBag.Messaage = "Can not Save  because Color  Name Already Exists..!";
                    return View(color);
                }

                color.CreateBy = CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(Session["ID"])); ;
                color.CreateDate = DateTime.UtcNow.AddHours(5.30);
                color.NetworkIP = BusinessLogicLayer.CommonFunctions.GetClientIP();
                color.DeviceID = string.Empty;
                color.DeviceType = string.Empty;

                if (ModelState.IsValid)
                {
                    db.Colors.Add(color);
                    db.SaveChanges();
                    //return RedirectToAction("Index");
                    ViewBag.Messaage = "Color Detail Inserted Successfully";
                }

                return View(color);
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Error", "Sorry! Problem in Generate Create view!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[ColorController][POST:Create]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
                ViewBag.Messaage = "Unable to Inserted Color Detail ";
                return View(color);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Error", "Sorry! Problem in Generate Create view!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[ColorController][POST:Create]",
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);

                ViewBag.Messaage = "Unable to Inserted Color Detail ";
                return View(color);
            }
          
        }

        // GET: /Color/Edit/5
        [SessionExpire]
        [CustomAuthorize(Roles = "Color/CanWrite")]
        public ActionResult Edit(int? id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Color color = db.Colors.Find(id);
                if (color == null)
                {
                    return HttpNotFound();
                }
                return View(color);
            
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Error", "Sorry! Problem in Record Update!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[ColorController][GET:Edit]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
                return View();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Error", "Sorry! Problem in Record Update!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[ColorController][GET:Edit]",
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
                return View();
            }
        }

        // POST: /Color/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [SessionExpire]
        [CustomAuthorize(Roles = "Color/CanWrite")]
        public ActionResult Edit([Bind(Include="ID,HtmlCode,Name,IsActive")] Color color)
        {
            try
            {
                if (db.Colors.Where(x => x.Name == color.Name && x.ID != color.ID).Count() > 0)
                {
                    ViewBag.Messaage = "Can not Save because Color Name Already Exists..!";
                    return View(color);
                }

                Color lData = db.Colors.Single(x => x.ID == color.ID);


                color.CreateBy = lData.CreateBy;
                color.CreateDate = lData.CreateDate;
                color.ModifyBy = 1;
                color.ModifyDate = DateTime.UtcNow.AddHours(5.30);
                color.NetworkIP = BusinessLogicLayer.CommonFunctions.GetClientIP();
                color.DeviceID = string.Empty;
                color.DeviceType = string.Empty;

                if (ModelState.IsValid)
                {
                    db.Entry(lData).CurrentValues.SetValues(color);
                    //db.Entry(color).State = EntityState.Modified;
                    db.SaveChanges();
                    //return RedirectToAction("Index");
                    ViewBag.Messaage = "Color Detail Updated Successfully";
                }
                return View(color);
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Error", "Sorry! Problem in Record Update!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[ColorController][POST:Edit]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
                ViewBag.Messaage = "Unable to Update Color Detail ";
                return View(color);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Error", "Sorry! Problem in Record Update!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[ColorController][POST:Edit]",
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);

                ViewBag.Messaage = "Unable to Update Color Detail ";
                return View(color);
            }
        }

        // GET: /Color/Delete/5
        public ActionResult Delete(int? id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Color color = db.Colors.Find(id);
                if (color == null)
                {
                    return HttpNotFound();
                }
                return View(color);
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Error", "Sorry! Problem in Record Deletion!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[ColorController][GET:Delete]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
                return View();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Error", "Sorry! Problem in Record Deletion!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[ColorController][GET:Delete]",
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
                return View();
            }
        }

        // POST: /Color/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                Color color = db.Colors.Find(id);
                db.Colors.Remove(color);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Error", "Sorry! Problem in Record Deletion!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[ColorController][POST:Edit]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
                ViewBag.Messaage = "Unable to Delete Color Detail ";
                return View(db.Colors.Where(x => x.ID == id).FirstOrDefault());
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Error", "Sorry! Problem in Record Deletion!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[ColorController][POST:Edit]",
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
                ViewBag.Messaage = "Unable to Delete Color Detail ";
                return View(db.Colors.Where(x => x.ID == id).FirstOrDefault());
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
