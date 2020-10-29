//-----------------------------------------------------------------------
// <copyright file="ProductList.cs" company="Ezeelo Consumer Services Pvt. Ltd.">
//     Copyright (c) Ezeelo Consumer Services Pvt. Ltd. All rights reserved.
// </copyright>
// <author>Pradnyakar N. Badge</author>
//-----------------------------------------------------------------------
using Administrator.Models;
using BusinessLogicLayer;
using ModelLayer.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Text;
using System.Web.Mvc;

namespace Administrator.Controllers
{
    [SessionExpire]
    public class RootCategoryController : Controller
    {
        private EzeeloDBContext db = new EzeeloDBContext();

        StringBuilder errStr = new StringBuilder("=====================================================================================" +
                                                  Environment.NewLine + "ErrorLog Controller : RootCategoryController" + Environment.NewLine);
        // GET: /RootCategory/
        [CustomAuthorize(Roles = "RootCategory/CanRead")]
        public ActionResult Index()
        {
            try
            {
                var categories = db.Categories.Include(c => c.Category2).Include(c => c.PersonalDetail).Include(c => c.PersonalDetail1).Where(c => c.Level == 1);
                return View(categories.OrderBy(x => x.Name).ToList());
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

        // GET: /RootCategory/Details/5
        [CustomAuthorize(Roles = "RootCategory/CanRead")]
        public ActionResult Details(int? id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Category category = db.Categories.Find(id);
                if (category == null)
                {
                    return HttpNotFound();
                }

                return View(category);
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

        // GET: /RootCategory/Create
        [CustomAuthorize(Roles = "RootCategory/CanWrite")]
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

        // POST: /RootCategory/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(Roles = "RootCategory/CanWrite")]
        public ActionResult Create([Bind(Include = "ID,Name,ParentCategoryID,Description,SearchKeyword,IsActive,IsExpire,ExpiryDate")] Category category)
        {
            try
            {
                category.CreateBy = CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(Session["ID"])); 
                category.Level = 1;
                category.CreateDate = DateTime.UtcNow.AddHours(5.30);
                category.NetworkIP = BusinessLogicLayer.CommonFunctions.GetClientIP();
                category.DeviceID = string.Empty;
                category.DeviceType = string.Empty;

                List<Category> lDuplicate = new List<Category>();

                lDuplicate = db.Categories.Where(x => x.Name == category.Name && x.Level == 1).ToList();

                if (lDuplicate.Count() > 0)
                {
                    ViewBag.Messaage = "Unable to Inserted duplicate Category successfully";
                    return View(category);
                }

                if(category.IsExpire==true && category.ExpiryDate==null)
                {
                    ViewBag.Messaage = "Please Provide Expiry date..";
                    return View(category);
                }

                if (ModelState.IsValid)
                {
                    db.Categories.Add(category);
                    db.SaveChanges();
                    ViewBag.Messaage = "Category Inserted successfully";
                    return View(category);
                }
                return View(category);
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

                ViewBag.Messaage = "Unable to Insert Category Detail ";
                return View(category);
            }
        }

        // GET: /RootCategory/Edit/5
        [CustomAuthorize(Roles = "RootCategory/CanWrite")]
        public ActionResult Edit(int? id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Category category = db.Categories.Find(id);
                if (category == null)
                {
                    return HttpNotFound();
                }

                return View(category);
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

        // POST: /RootCategory/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(Roles = "RootCategory/CanWrite")]
        public ActionResult Edit([Bind(Include = "ID,Name,ParentCategoryID,Description,SearchKeyword,IsActive,IsExpire,ExpiryDate")] Category category)
        {
            try
            {

                List<Category> lDuplicate = new List<Category>();

                lDuplicate = db.Categories.Where(x => x.Name == category.Name && x.Level == 1 && x.ID != category.ID).ToList();

                if (lDuplicate.Count() > 0)
                {
                    ViewBag.Messaage = "Unable to Inserted duplicate Category successfully";
                    return View(category);
                }
                if (category.IsExpire == true && category.ExpiryDate == null)
                {
                    ViewBag.Messaage = "Please Provide Expiry date..";
                    return View(category);
                }
                Category lData = db.Categories.Single(x => x.ID == category.ID);
                category.Level = lData.Level;
                category.CreateBy = lData.CreateBy;
                category.CreateDate = lData.CreateDate;
                category.ModifyBy = CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(Session["ID"])); 
                category.ModifyDate = DateTime.UtcNow.AddHours(5.30);
                category.NetworkIP = BusinessLogicLayer.CommonFunctions.GetClientIP();
                category.DeviceID = string.Empty;
                category.DeviceType = string.Empty;




                if (ModelState.IsValid)
                {
                    db.Entry(lData).CurrentValues.SetValues(category);
                    // db.Entry(category).State = EntityState.Modified;
                    db.SaveChanges();
                    //return RedirectToAction("Index");
                    ViewBag.Messaage = "Category Updated successfully";
                    return View(category);
                }

                return View(category);
            }
            catch (Exception ex)
            {
                errStr.Append("Method Name[Http Request] :- Edit[HttpPost]" + Environment.NewLine +
                               "ON Dated" + DateTime.UtcNow.AddHours(5.30).TimeOfDay + Environment.NewLine +
                                   ex.Message.ToString() + Environment.NewLine +
                         "====================================================================================="
                               );
                //ViewBag.Message = "Sorry! Problem in customer registration!!";
                ModelState.AddModelError("Message", "Sorry! Problem in Record Update!!");
                ErrorLog.ErrorLogFile("Sorry! Problem in Record Update!!" + Environment.NewLine + errStr.ToString()
                    , ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);

                ViewBag.Messaage = "Unable to Update Category Detail ";
                return View(category);
            }
        }

        // GET: /RootCategory/Delete/5
        [CustomAuthorize(Roles = "RootCategory/CanDelete")]
        public ActionResult Delete(int? id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Category category = db.Categories.Find(id);
                if (category == null)
                {
                    return HttpNotFound();
                }
                return View(category);
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

        // POST: /RootCategory/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(Roles = "RootCategory/CanDelete")]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                Category category = db.Categories.Find(id);
                db.Categories.Remove(category);
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

                ViewBag.Messaage = "Unable to Deleted Category Detail ";
                return View(db.Categories.Where(x => x.ID == id).FirstOrDefault());
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
