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
using PagedList;
using PagedList.Mvc;
namespace Administrator.Controllers
{
    [SessionExpire]
    public class CategorySizeController : Controller
    {
        private EzeeloDBContext db = new EzeeloDBContext();

        // GET: /CategorySize/
        [SessionExpire]
        [CustomAuthorize(Roles = "CategorySize/CanRead")]
        public ActionResult Index(int? page)
        {
            try
            {
                var categorysizes = db.CategorySizes.Include(c => c.Category).Include(c => c.PersonalDetail).Include(c => c.PersonalDetail1).Include(c => c.Size);
                return View(categorysizes.OrderBy(x => x.Category.Name).ToList().ToPagedList(page ?? 1, 20));
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Message", "Sorry! Problem in Generate Index view!!");
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[CategorySizeController][GET:Index]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
                return View();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Message", "Sorry! Problem in Generate Index view!!");
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[CategorySizeController][GET:Index]",
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
                return View();
            }

        }

        // GET: /CategorySize/Details/5
        [SessionExpire]
        [CustomAuthorize(Roles = "CategorySize/CanRead")]
        public ActionResult Details(long? id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                CategorySize categorysize = db.CategorySizes.Find(id);
                if (categorysize == null)
                {
                    return HttpNotFound();
                }
                return View(categorysize);
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Message", "Sorry! Problem in Record Detail!!");
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[CategorySizeController][GET:Details]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
                return View();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Message", "Sorry! Problem in Record Detail!!");
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[CategorySizeController][GET:Details]",
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
                return View();
            }


        }


        // GET: /CategorySize/Create
        [SessionExpire]
        [CustomAuthorize(Roles = "CategorySize/CanWrite")]
        public ActionResult Create()
        {
            try
            {
                ViewBag.CategoryID = new SelectList(db.Categories.OrderBy(x => x.Name), "ID", "Name");
                ViewBag.SizeID = new SelectList(db.Sizes.OrderBy(x => x.Name), "ID", "Name");
                return View();
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Message", "Sorry! Problem in Generate Create View!!");
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[CategorySizeController][GET:Create]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
                return View();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Message", "Sorry! Problem in Generate Create View!!");
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[CategorySizeController][GET:Create]",
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
                return View();
            }

        }

        // POST: /CategorySize/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [SessionExpire]
        [CustomAuthorize(Roles = "CategorySize/CanWrite")]
        public ActionResult Create([Bind(Include = "ID,CategoryID,SizeID,IsActive")] CategorySize categorysize)
        {
            try
            {
                ViewBag.CategoryID = new SelectList(db.Categories.OrderBy(x => x.Name), "ID", "Name", categorysize.CategoryID);
                ViewBag.SizeID = new SelectList(db.Sizes.OrderBy(x => x.Name), "ID", "Name", categorysize.SizeID);
                List<CategorySize> lDuplicate = new List<CategorySize>();
                lDuplicate = db.CategorySizes.Where(x => x.CategoryID == categorysize.CategoryID && x.SizeID == categorysize.SizeID).ToList();
                if (lDuplicate.Count() > 0)
                {
                    ViewBag.Messaage = "Unable to Inserted Duplicate Category size Detail ";
                    return View(categorysize);
                }
                categorysize.CreateBy = CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(Session["ID"]));
                categorysize.CreateDate = DateTime.UtcNow.AddHours(5.30);
                categorysize.NetworkIP = BusinessLogicLayer.CommonFunctions.GetClientIP();
                categorysize.DeviceID = string.Empty;
                categorysize.DeviceType = string.Empty;


                if (ModelState.IsValid)
                {
                    db.CategorySizes.Add(categorysize);
                    db.SaveChanges();
                   // return RedirectToAction("Index");
                    ViewBag.Messaage = "Category Size Detail Inserted Successfully";
                }

               
                return View(categorysize);
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Message", "Sorry! Problem in Record Creation!!");
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[CategorySizeController][POST:Create]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
                ViewBag.Messaage = "Unable to Inserted Category size Detail ";
                return View(categorysize);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Message", "Sorry! Problem in Record Creation!!");
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[CategorySizeController][POST:Create]",
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
                ViewBag.Messaage = "Unable to Inserted Category size Detail ";
                return View(categorysize);
            }

        }

        // GET: /CategorySize/Edit/5
        [SessionExpire]
        [CustomAuthorize(Roles = "CategorySize/CanWrite")]
        public ActionResult Edit(long? id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                CategorySize categorysize = db.CategorySizes.Find(id);
                if (categorysize == null)
                {
                    return HttpNotFound();
                }
                ViewBag.CategoryID = new SelectList(db.Categories.OrderBy(x => x.Name), "ID", "Name", categorysize.CategoryID);
                ViewBag.SizeID = new SelectList(db.Sizes.OrderBy(x => x.Name), "ID", "Name", categorysize.SizeID);
                return View(categorysize);
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Message", "Sorry! Problem in Record Update!!");
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[CategorySizeController][GET:Edit]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
                return View();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Message", "Sorry! Problem in Record Update!!");
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[CategorySizeController][GET:Edit]",
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
                return View();
            }

        }

        // POST: /CategorySize/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [SessionExpire]
        [CustomAuthorize(Roles = "CategorySize/CanWrite")]
        public ActionResult Edit([Bind(Include = "ID,CategoryID,SizeID,IsActive")] CategorySize categorysize)
        {
            try
            {
                CategorySize lData = db.CategorySizes.Single(x => x.ID == categorysize.ID);
                categorysize.CreateBy = lData.CreateBy;
                categorysize.CreateDate = lData.CreateDate;
                categorysize.ModifyBy = CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(Session["ID"]));
                categorysize.ModifyDate = DateTime.UtcNow.AddHours(5.30);
                categorysize.NetworkIP = BusinessLogicLayer.CommonFunctions.GetClientIP();
                categorysize.DeviceID = string.Empty;
                categorysize.DeviceType = string.Empty;

                if (ModelState.IsValid)
                {
                    db.Entry(lData).CurrentValues.SetValues(categorysize);
                    //db.Entry(categorysize).State = EntityState.Modified;
                    db.SaveChanges();
                    //return RedirectToAction("Index");
                    ViewBag.Messaage = "Category Size Detail Updated Successfully";
                }
                ViewBag.CategoryID = new SelectList(db.Categories.OrderBy(x => x.Name), "ID", "Name", categorysize.CategoryID);
                ViewBag.SizeID = new SelectList(db.Sizes.OrderBy(x => x.Name), "ID", "Name", categorysize.SizeID);
                return View(categorysize);
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Message", "Sorry! Problem in Record Updation!!");
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[CategorySizeController][POST:Edit]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
                ViewBag.Messaage = "Unable to Update Category size Detail ";
                return View(categorysize);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Message", "Sorry! Problem in Record Updation!!");
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[CategorySizeController][POST:Edit]",
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
                ViewBag.Messaage = "Unable to Update Category size Detail ";
                return View(categorysize);
            }
        }

        // GET: /CategorySize/Delete/5
        [SessionExpire]
        [CustomAuthorize(Roles = "CategorySize/CanDelete")]
        public ActionResult Delete(long? id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                CategorySize categorysize = db.CategorySizes.Find(id);
                if (categorysize == null)
                {
                    return HttpNotFound();
                }
                return View(categorysize);
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Message", "Sorry! Problem in Record Deletion!!");
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[CategorySizeController][GET:Delete]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
                return View();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Message", "Sorry! Problem in Record Deletion!!");
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[CategorySizeController][GET:Delete]",
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
                return View();
            }
        }

        // POST: /CategorySize/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [SessionExpire]
        [CustomAuthorize(Roles = "CategorySize/CanDelete")]
        public ActionResult DeleteConfirmed(long id)
        {
            try
            {
                CategorySize categorysize = db.CategorySizes.Find(id);
                db.CategorySizes.Remove(categorysize);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Message", "Sorry! Problem in Record Deletion!!");
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[CategorySizeController][POST:DeleteConfirmed]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
                ViewBag.Messaage = "Unable to Delete Category size Detail ";
                return View(db.CategorySizes.Where(x => x.ID == id).FirstOrDefault());
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Message", "Sorry! Problem in Record Deletion!!");
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[CategorySizeController][POST:DeleteConfirmed]",
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
                ViewBag.Messaage = "Unable to Delete Category size Detail ";
                return View(db.CategorySizes.Where(x => x.ID == id).FirstOrDefault());
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
