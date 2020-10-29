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
    /// Developed By :- Pradnyakar Badge
    /// Purpose:- To Create Dimension of category
    /// </summary>
    [SessionExpire]
    public class CategoryDimentionController : Controller
    {
        private EzeeloDBContext db = new EzeeloDBContext();
        StringBuilder errStr = new StringBuilder("=====================================================================================" +
          Environment.NewLine
          + "ErrorLog Controller : CategoryDimentionController" + Environment.NewLine);

        // GET: /CategoryDimention/
        [SessionExpire]
        [CustomAuthorize(Roles = "CategoryDimension/CanRead")]
        public ActionResult Index()
        {
            try
            {
                var categorydimensions = db.CategoryDimensions.Include(c => c.Category).Include(c => c.Dimension).Include(c => c.PersonalDetail).Include(c => c.PersonalDetail1);
                return View(categorydimensions.OrderBy(x => x.Category.Name).ToList());
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

        // GET: /CategoryDimention/Details/5
        [SessionExpire]
        [CustomAuthorize(Roles = "CategoryDimension/CanRead")]
        public ActionResult Details(long? id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                CategoryDimension categorydimension = db.CategoryDimensions.Find(id);
                if (categorydimension == null)
                {
                    return HttpNotFound();
                }
                return View(categorydimension);
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

        // GET: /CategoryDimention/Create
        [SessionExpire]
        [CustomAuthorize(Roles = "CategoryDimension/CanWrite")]
        public ActionResult Create()
        {
            try
            {
                ViewBag.CategoryID = new SelectList(db.Categories.OrderBy(x => x.Name), "ID", "Name");
                ViewBag.DimensionID = new SelectList(db.Dimensions.OrderBy(x => x.Name), "ID", "Name");

                return View();
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

        // POST: /CategoryDimention/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [SessionExpire]
        [CustomAuthorize(Roles = "CategoryDimension/CanWrite")]
        public ActionResult Create([Bind(Include="ID,CategoryID,DimensionID,IsActive")] CategoryDimension categorydimension)
        {
            try
            {
                ViewBag.CategoryID = new SelectList(db.Categories.OrderBy(x => x.Name), "ID", "Name", categorydimension.CategoryID);
                ViewBag.DimensionID = new SelectList(db.Dimensions.OrderBy(x => x.Name), "ID", "Name", categorydimension.DimensionID);

                List<CategoryDimension> lDuplicate = new List<CategoryDimension>();
                lDuplicate = db.CategoryDimensions.Where(x => x.CategoryID == categorydimension.CategoryID && x.DimensionID == categorydimension.DimensionID).ToList();

                if (lDuplicate.Count() > 0)
                {
                    ViewBag.Messaage = "Unable to Inserted Duplicate Category Dimension Detail";
                    return View(categorydimension);
                }

                categorydimension.CreateBy = CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(Session["ID"])); 
                categorydimension.CreateDate = DateTime.UtcNow.AddHours(5.30);
                categorydimension.NetworkIP = BusinessLogicLayer.CommonFunctions.GetClientIP();
                categorydimension.DeviceID = string.Empty;
                categorydimension.DeviceType = string.Empty;


                if (ModelState.IsValid)
                {
                    db.CategoryDimensions.Add(categorydimension);
                    db.SaveChanges();
                    //return RedirectToAction("Index");
                    ViewBag.Messaage = "Category Dimension Detail Inserted Successfully";
                }

                
                
                return View(categorydimension);
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

                ViewBag.Messaage = "Unable to Inserted Category Dimension Detail ";
                return View(categorydimension);
            }
        }

        // GET: /CategoryDimention/Edit/5
        [SessionExpire]
        [CustomAuthorize(Roles = "CategoryDimension/CanWrite")]
        public ActionResult Edit(long? id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                CategoryDimension categorydimension = db.CategoryDimensions.Find(id);
                if (categorydimension == null)
                {
                    return HttpNotFound();
                }
                ViewBag.CategoryID = new SelectList(db.Categories.OrderBy(x => x.Name), "ID", "Name", categorydimension.CategoryID);
                ViewBag.DimensionID = new SelectList(db.Dimensions.OrderBy(x => x.Name), "ID", "Name", categorydimension.DimensionID);

                return View(categorydimension);
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

        // POST: /CategoryDimention/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [SessionExpire]
        [CustomAuthorize(Roles = "CategoryDimension/CanWrite")]
        public ActionResult Edit([Bind(Include="ID,CategoryID,DimensionID,IsActive")] CategoryDimension categorydimension)
        {
            try
            {

                List<CategoryDimension> lDuplicate = new List<CategoryDimension>();
                lDuplicate = db.CategoryDimensions.Where(x => x.CategoryID == categorydimension.CategoryID && x.DimensionID == categorydimension.DimensionID && x.ID != categorydimension.ID).ToList();

                if (lDuplicate.Count() > 0)
                {
                    ViewBag.Messaage = "Unable to Update Duplicate Category Dimension Detail";
                    return View(categorydimension);
                }

                CategoryDimension lData = db.CategoryDimensions.Single(x => x.ID == categorydimension.ID);
                categorydimension.CreateBy = lData.CreateBy;
                categorydimension.CreateDate = lData.CreateDate;
                categorydimension.ModifyBy = CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(Session["ID"])); 
                categorydimension.ModifyDate = DateTime.UtcNow.AddHours(5.30);
                categorydimension.NetworkIP = BusinessLogicLayer.CommonFunctions.GetClientIP();
                categorydimension.DeviceID = string.Empty;
                categorydimension.DeviceType = string.Empty;


                if (ModelState.IsValid)
                {
                    db.Entry(lData).CurrentValues.SetValues(categorydimension);
                    //db.Entry(categorydimension).State = EntityState.Modified;
                    db.SaveChanges();
                    //return RedirectToAction("Index");
                    ViewBag.Messaage = "Category Dimension Detail Updated Successfully";
                }
                ViewBag.CategoryID = new SelectList(db.Categories.OrderBy(x => x.Name), "ID", "Name", categorydimension.CategoryID);
                ViewBag.DimensionID = new SelectList(db.Dimensions.OrderBy(x => x.Name), "ID", "Name", categorydimension.DimensionID);

                return View(categorydimension);
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

                ViewBag.Messaage = "Unable to Inserted Category Dimension Detail ";
                return View(categorydimension);
            }
        }

        // GET: /CategoryDimention/Delete/5
        [SessionExpire]
        [CustomAuthorize(Roles = "CategoryDimension/CanDelete")]
        public ActionResult Delete(long? id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                CategoryDimension categorydimension = db.CategoryDimensions.Find(id);
                if (categorydimension == null)
                {
                    return HttpNotFound();
                }
                return View(categorydimension);

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

        // POST: /CategoryDimention/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [SessionExpire]
        [CustomAuthorize(Roles = "CategoryDimension/CanDelete")]
        public ActionResult DeleteConfirmed(long id)
        {
            try
            {
                CategoryDimension categorydimension = db.CategoryDimensions.Find(id);
                db.CategoryDimensions.Remove(categorydimension);
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

                ViewBag.Messaage = "Unable to Delete Category Dimension Detail ";
                return View(db.CategoryDimensions.Where(x => x.ID == id).FirstOrDefault());
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
