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
    public class LevelTwoCategoryController : Controller
    {
        private EzeeloDBContext db = new EzeeloDBContext();
        StringBuilder errStr = new StringBuilder("=====================================================================================" +
         Environment.NewLine
         + "ErrorLog Controller : LevelTwoCategoryController" + Environment.NewLine);

        // GET: /LevelTwoCategory/
        [CustomAuthorize(Roles = "LevelTwoCategory/CanRead")]
        public ActionResult Index()
        {
            try
            {
                var categories = db.Categories.Include(c => c.Category2).Include(c => c.PersonalDetail).Include(c => c.PersonalDetail1).Where(c => c.Level == 2);
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

        // GET: /LevelTwoCategory/Details/5
        [CustomAuthorize(Roles = "LevelTwoCategory/CanRead")]
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

        // GET: /LevelTwoCategory/Create
        [CustomAuthorize(Roles = "LevelTwoCategory/CanWrite")]
        public ActionResult Create()
        {
            try
            {
                ViewBag.ParentCategoryID = new SelectList(db.Categories.Where(x => x.Level == 1).OrderBy(x => x.Name), "ID", "Name");
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

        // POST: /LevelTwoCategory/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(Roles = "LevelTwoCategory/CanWrite")]
        public ActionResult Create([Bind(Include = "ID,Name,ParentCategoryID,Description,SearchKeyword,IsActive,IsExpire,ExpiryDate")] Category category)
        {
            try
            {
                ViewBag.ParentCategoryID = new SelectList(db.Categories.Where(x => x.Level == 1).OrderBy(x => x.Name).ToList(), "ID", "Name", category.ParentCategoryID);

                List<Category> lDuplicate = new List<Category>();

                lDuplicate = db.Categories.Where(x => x.Name == category.Name && x.Level == 2 && x.ParentCategoryID == category.ParentCategoryID).OrderBy(x => x.Name).ToList();

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

                category.Level = 2;
                category.CreateBy = CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(Session["ID"])); 
                category.CreateDate = DateTime.UtcNow.AddHours(5.30);
                category.NetworkIP = BusinessLogicLayer.CommonFunctions.GetClientIP();
                category.DeviceID = string.Empty;
                category.DeviceType = string.Empty;

                if (ModelState.IsValid)
                {
                    db.Categories.Add(category);
                    db.SaveChanges();
                    //return RedirectToAction("Index");
                    ViewBag.Messaage = "Level Two Category Detail Inserted Successfully";
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

                ViewBag.Messaage = "Unable to Insert category Detail ";
                return View(category);
            }
        }

        // GET: /LevelTwoCategory/Edit/5
        [CustomAuthorize(Roles = "LevelTwoCategory/CanWrite")]
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
                ViewBag.ParentCategoryID = new SelectList(db.Categories.Where(x => x.Level == 1).OrderBy(x => x.Name).ToList(), "ID", "Name", category.ParentCategoryID);

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

        // POST: /LevelTwoCategory/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(Roles = "LevelTwoCategory/CanWrite")]
        public ActionResult Edit([Bind(Include = "ID,Name,ParentCategoryID,Description,SearchKeyword,IsActive,IsExpire,ExpiryDate")] Category category)
        {
            try
            {
                ViewBag.ParentCategoryID = new SelectList(db.Categories.Where(x => x.Level == 1).ToList(), "ID", "Name", category.ParentCategoryID);

                List<Category> lDuplicate = new List<Category>();

                lDuplicate = db.Categories.Where(x => x.Name == category.Name && x.Level == 2 && x.ParentCategoryID == category.ParentCategoryID && x.ID != category.ID).ToList();

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

                category.Level = 2;

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
                    //db.Entry(category).State = EntityState.Modified;
                    db.SaveChanges();
                    //return RedirectToAction("Index");
                    ViewBag.Messaage = "Level Two Category Detail Updated Successfully";
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

                ViewBag.Messaage = "Unable to Update Level Two Category Detail ";
                return View(category);
            }
        }

        // GET: /LevelTwoCategory/Delete/5
        [CustomAuthorize(Roles = "LevelTwoCategory/CanDelete")]
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

        // POST: /LevelTwoCategory/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(Roles = "LevelTwoCategory/CanDelete")]
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
                errStr.Append("Method Name[Http Request] :- Delete[HttpGet]" + Environment.NewLine +
                               "ON Dated" + DateTime.UtcNow.AddHours(5.30).TimeOfDay + Environment.NewLine +
                                   ex.Message.ToString() + Environment.NewLine +
                         "====================================================================================="
                               );
                //ViewBag.Message = "Sorry! Problem in customer registration!!";
                ModelState.AddModelError("Message", "Sorry! Problem in Record Deletion!!");
                ErrorLog.ErrorLogFile("Sorry! Problem in Record Deletion!!" + Environment.NewLine + errStr.ToString()
                    , ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);

                ViewBag.Messaage = "Unable to Delete Level Two Category Detail ";
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
