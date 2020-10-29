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
    public class SEOpageController : Controller
    {
        private EzeeloDBContext db = new EzeeloDBContext();

        StringBuilder errStr = new StringBuilder("=====================================================================================" +
        Environment.NewLine
        + "ErrorLog Controller : SEOpageController" + Environment.NewLine);


        // GET: /SEOpage/
        [CustomAuthorize(Roles = "SEOpage/CanRead")]
        public ActionResult Index()
        {
            try
            {

                return View(db.SEOs.ToList());
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Error", "Sorry! Problem in Generate Index view!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[SEOpageController][GET:Index]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
                return View();
            }
            catch (Exception ex)
            {

                ModelState.AddModelError("Error", "Sorry! Problem in Generate Index view!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[SEOpageController][GET:Index]",
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
                return View();
            }

        }

        // GET: /SEOpage/Details/5
        [CustomAuthorize(Roles = "SEOpage/CanRead")]
        public ActionResult Details(long? id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                SEO seo = db.SEOs.Find(id);
                if (seo == null)
                {
                    return HttpNotFound();
                }
                if (seo.BusinessType.Prefix.Equals("MTDT"))
                {
                    ViewBag.EntityName = db.Products.Where(x => x.ID == seo.EntityID).FirstOrDefault().Name;
                }
                else
                {
                    ViewBag.EntityName = db.Categories.Where(x => x.ID == seo.EntityID).FirstOrDefault().Name;
                }

                return View(seo);
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Error", "Sorry! Problem in Record Detail!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[SEOpageController][GET:Details]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
                return View();
            }
            catch (Exception ex)
            {

                ModelState.AddModelError("Error", "Sorry! Problem in Record Detail!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[SEOpageController][GET:Details]",
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
                return View();
            }

        }

        // GET: /SEOpage/Create
        [CustomAuthorize(Roles = "SEOpage/CanWrite")]
        public ActionResult Create()
        {
            try
            {
                ViewBag.BusinessTypeID = new SelectList(db.BusinessTypes.Where(x => x.Name == "ProductMetadata" || x.Name == "CategoryMetadata").ToList(), "ID", "Name");
                this.ViewBagList(0, 0, 0, 0);
                return View();
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Error", "Sorry! Problem in Generate Create View!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[SEOpageController][GET:Create]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
                return View();
            }
            catch (Exception ex)
            {

                ModelState.AddModelError("Error", "Sorry! Problem in Generate Create View!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[SEOpageController][GET:Create]",
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
                return View();
            }

        }

        // POST: /SEOpage/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(Roles = "SEOpage/CanWrite")]
        public ActionResult Create([Bind(Include = "H1,Metatag,BusinessTypeID,Description,MetaKeyword,URL,PageName,EntityID,IsActive")] SEO seo, Int64? ProductID, Int64? Levelone, Int64? levelTwo, Int64? levelthree, string ProductDisription)
        {
            try
            {
                ViewBag.BusinessTypeID = new SelectList(db.BusinessTypes.Where(x => x.Name == "ProductMetadata" || x.Name == "CategoryMetadata").ToList(), "ID", "Name");
                this.ViewBagList(Levelone, levelTwo, levelthree, ProductID);

                if (db.SEOs.Where(x => x.BusinessTypeID == seo.BusinessTypeID && x.EntityID == seo.EntityID).Count() < 1)
                {
                    seo.CreateBy = CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(Session["ID"]));
                    seo.CreateDate = DateTime.UtcNow.AddHours(5.30);
                    seo.NetworkIP = BusinessLogicLayer.CommonFunctions.GetClientIP();
                    seo.DeviceID = string.Empty;
                    seo.DeviceType = string.Empty;

                    if (ModelState.IsValid)
                    {
                        db.SEOs.Add(seo);
                        db.SaveChanges();
                        //return RedirectToAction("Index");
                        if (ProductID > 0)
                        {
                            this.UpdateProductDescription(Convert.ToInt64(ProductID), ProductDisription);
                            ViewBag.ProductDisription = ProductDisription;
                        }

                        ViewBag.ErrorMessage = "SEO Inserted successfully";
                        ViewBag.OperationResult = "1";
                        //return View();
                        TempData["ErrorMessage"] = "SEO Inserted successfully";

                        SEO obj = new SEO();
                        return View("Create", obj);
                    }
                }
                else
                {
                    ViewBag.ErrorMessage = "SEO Can't Insert because entry is already Exist";
                }

                return View(seo);
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Error", "Sorry! Problem in Generate Create View!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[SEOpageController][POST:Create]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);

                ViewBag.Messaage = "Unable to Insert SEO Detail ";
                return View(seo);
            }
            catch (Exception ex)
            {

                ModelState.AddModelError("Error", "Sorry! Problem in Generate Create View!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[SEOpageController][POST:Create]",
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);

                ViewBag.Messaage = "Unable to Insert SEO Detail ";
                return View(seo);
            }

        }

        private void UpdateProductDescription(Int64? ProductID, string ProductDisription)
        {
            try
            {
                Product lProduct = new Product();
                lProduct = db.Products.Where(x => x.ID == ProductID).FirstOrDefault();

                lProduct.Description = ProductDisription;
                lProduct.ModifyBy = CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(Session["ID"]));
                lProduct.ModifyDate = DateTime.UtcNow.AddHours(5.30);
                lProduct.NetworkIP = BusinessLogicLayer.CommonFunctions.GetClientIP();
                lProduct.DeviceID = string.Empty;
                lProduct.DeviceType = string.Empty;

                if (ModelState.IsValid)
                {
                    db.Entry(lProduct).State = EntityState.Modified;
                    db.SaveChanges();
                }

            }
            catch (Exception ex)
            {

                ModelState.AddModelError("Error", "Sorry! Problem in Generate Create View!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[SEOpageController][POST:Create]",
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);

                ViewBag.Messaage = "Unable to update Product Description";

            }
            
        }

        // GET: /SEOpage/Edit/5
        [CustomAuthorize(Roles = "SEOpage/CanWrite")]
        public ActionResult Edit(long? id)
        {
            try
            {

                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                SEO seo = db.SEOs.Find(id);
                if (seo == null)
                {
                    return HttpNotFound();
                }


                this.ViewBagListForit(id);

                return View(seo);
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Error", "Sorry! Problem in Record Update!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[SEOpageController][GET:Edit]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
                return View();
            }
            catch (Exception ex)
            {

                ModelState.AddModelError("Error", "Sorry! Problem in Record Update!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[SEOpageController][GET:Edit]",
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
                return View();
            }

        }

        // POST: /SEOpage/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(Roles = "SEOpage/CanWrite")]
        public ActionResult Edit([Bind(Include = "ID,H1,Metatag,BusinessTypeID,Description,MetaKeyword,URL,PageName,EntityID,IsActive")] SEO seo)
        {
            try
            {

                //ViewBag.BusinessTypeID = new SelectList(db.BusinessTypes.ToList(), "ID", "Name");

                this.ViewBagListForit(seo.ID);
                if (db.SEOs.Where(x => x.BusinessTypeID == seo.BusinessTypeID && x.EntityID == seo.EntityID && x.ID != seo.ID).Count() < 1)
                {
                    SEO lData = db.SEOs.Single(x => x.ID == seo.ID);

                    seo.CreateBy = lData.CreateBy;
                    seo.CreateDate = lData.CreateDate;
                    seo.ModifyBy = CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(Session["ID"]));
                    seo.ModifyDate = DateTime.UtcNow.AddHours(5.30);
                    seo.NetworkIP = BusinessLogicLayer.CommonFunctions.GetClientIP();
                    seo.DeviceID = string.Empty;
                    seo.DeviceType = string.Empty;

                    if (ModelState.IsValid)
                    {
                        db.Entry(lData).CurrentValues.SetValues(seo);
                        //db.Entry(seo).State = EntityState.Modified;
                        db.SaveChanges();
                        //return RedirectToAction("Index");

                        ViewBag.ErrorMessage = "SEO Updated successfully";
                        return View();
                    }
                }
                else
                {
                    ViewBag.ErrorMessage = "SEO Can't Update because same entry is already Exist";
                }
                return View(seo);
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Error", "Sorry! Problem in Record Update!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[SEOpageController][POST:Edit]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);

                ViewBag.Messaage = "Unable to Update SEO Detail ";
                return View(seo);
            }
            catch (Exception ex)
            {

                ModelState.AddModelError("Error", "Sorry! Problem in Record Update!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[SEOpageController][POST:Edit]",
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);

                ViewBag.Messaage = "Unable to Update SEO Detail ";
                return View(seo);
            }

        }

        // GET: /SEOpage/Delete/5
        [CustomAuthorize(Roles = "SEOpage/CanDelete")]
        public ActionResult Delete(long? id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                SEO seo = db.SEOs.Find(id);
                if (seo == null)
                {
                    return HttpNotFound();
                }
                if (seo.BusinessType.Prefix.Equals("MTDT"))
                {
                    ViewBag.EntityName = db.Products.Where(x => x.ID == seo.EntityID).FirstOrDefault().Name;
                } 
                else
                {
                    ViewBag.EntityName = db.Categories.Where(x => x.ID == seo.EntityID).FirstOrDefault().Name;
                }
                return View(seo);
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Error", "Sorry! Problem in Record Deletion!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[SEOpageController][GET:Delete]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
                return View();
            }
            catch (Exception ex)
            {

                ModelState.AddModelError("Error", "Sorry! Problem in Record Deletion!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[SEOpageController][GET:Delete]",
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
                return View();
            }

        }

        // POST: /SEOpage/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(Roles = "SEOpage/CanDelete")]
        public ActionResult DeleteConfirmed(long id)
        {
            try
            {
                SEO seo = db.SEOs.Find(id);
                db.SEOs.Remove(seo);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Error", "Sorry! Problem in Record Deletion!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[SEOpageController][POST:Delete]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);

                ViewBag.Messaage = "Unable to Delete SEO Detail ";
                return View(db.SEOs.Where(x => x.ID == id).FirstOrDefault());
            }
            catch (Exception ex)
            {

                ModelState.AddModelError("Error", "Sorry! Problem in Record Deletion!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[SEOpageController][POST:Delete]",
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);

                ViewBag.Messaage = "Unable to Delete SEO Detail ";
                return View(db.SEOs.Where(x => x.ID == id).FirstOrDefault());
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

        public JsonResult PrefixVerification(int id)
        {
            string prefix = db.BusinessTypes.Where(x => x.ID == id).FirstOrDefault().Prefix;

            return Json(prefix, JsonRequestBehavior.AllowGet);
        }

        private void ViewBagListForit(long? id)
        {
            SEO lseo = new SEO();
            lseo = db.SEOs.Where(x => x.ID == id).FirstOrDefault();


            ViewBag.BusinessTypeID = new SelectList(db.BusinessTypes.Where(x => x.Name == "ProductMetadata" || x.Name == "CategoryMetadata").ToList(), "ID", "Name", lseo.BusinessTypeID);

            ViewBagDetail productDetail = new ViewBagDetail();
            ViewBagDetail CategoryDetail = new ViewBagDetail();
            long categoryID = 0;

            if (db.SEOs.Where(x => x.BusinessType.Name == "ProductMetadata" && x.ID == id).Count() > 0)
            {
                productDetail = (from n in db.Products
                                 join s in db.SEOs on n.ID equals s.EntityID
                                 join bt in db.BusinessTypes on s.BusinessTypeID equals bt.ID
                                 where bt.Name == "ProductMetadata" && s.ID == id
                                 select new ViewBagDetail
                                 {
                                     ProductID = n.ID,
                                     LevelThreeCat = n.CategoryID
                                 }).FirstOrDefault();
            }

            if (db.SEOs.Where(x => x.BusinessType.Name == "CategoryMetadata" && x.ID == id).Count() > 0)
            {
                categoryID = (from n in db.Categories
                              join s in db.SEOs on n.ID equals s.EntityID
                              join bt in db.BusinessTypes on s.BusinessTypeID equals bt.ID
                              where bt.Name == "CategoryMetadata" && s.ID == id
                              select new { n.ID }).FirstOrDefault().ID;
            }

            if (productDetail.ProductID > 0)
            {
                CategoryDetail = (from c in db.Categories
                                  join c1 in db.Categories on c.ID equals c1.ParentCategoryID
                                  join c2 in db.Categories on c1.ID equals c2.ParentCategoryID
                                  where c2.ID == productDetail.LevelThreeCat
                                  select new ViewBagDetail
                                  {
                                      LevelOneCat = c.ID,
                                      LevelTwoCat = c1.ID
                                  }).FirstOrDefault();

                ViewBag.levelOne = new SelectList(db.Categories.Where(x => x.Level == 1).ToList(), "ID", "Name", CategoryDetail.LevelOneCat);
                ViewBag.levelTwo = new SelectList(db.Categories.Where(x => x.ParentCategoryID == CategoryDetail.LevelOneCat).ToList(), "ID", "Name", CategoryDetail.LevelTwoCat);
                ViewBag.levelThree = new SelectList(db.Categories.Where(x => x.ParentCategoryID == CategoryDetail.LevelTwoCat && x.Level == 3).ToList(), "ID", "Name", productDetail.LevelThreeCat);
                ViewBag.ProductID = new SelectList(db.Products.Where(x => x.CategoryID == productDetail.LevelThreeCat), "ID", "Name", productDetail.ProductID);


                //this.UpdateProductDescription(Convert.ToInt64(productDetail.ProductID), ProductDisription);
                //ViewBag.ProductDisription = ProductDisription;

                //ViewBag.ProductDisription = db.Products.Where(x => x.ID == productDetail.ProductID).FirstOrDefault().Description;
            }
            else
            {
                List<SelectListItem> ldata = new List<SelectListItem>();
                ldata.Add(new SelectListItem { Text = "Select Category", Value = "0" });

                if (db.Categories.Where(x => x.ID == categoryID && x.Level == 1).Count() > 0)
                {
                    ViewBag.levelOne = new SelectList(db.Categories.Where(x => x.Level == 1).ToList(), "ID", "Name", categoryID);
                    ViewBag.levelTwo = new SelectList(ldata, "Value", "Text");
                    ViewBag.levelThree = new SelectList(ldata, "Value", "Text");
                    ViewBag.ProductID = new SelectList(ldata, "Value", "Text");
                }
                else if (db.Categories.Where(x => x.ID == categoryID && x.Level == 2).Count() > 0)
                {
                    var LevelTwoCategoryDetail = (from c in db.Categories
                                                  join c1 in db.Categories on c.ID equals c1.ParentCategoryID
                                                  where c1.ID == categoryID
                                                  select new ViewBagDetail
                                                  {
                                                      LevelOneCat = c.ID,
                                                      LevelTwoCat = c1.ID
                                                  }).FirstOrDefault();

                    ViewBag.levelOne = new SelectList(db.Categories.Where(x => x.Level == 1).ToList(), "ID", "Name", LevelTwoCategoryDetail.LevelOneCat);
                    ViewBag.levelTwo = new SelectList(db.Categories.Where(x => x.Level == 2 && x.ParentCategoryID == LevelTwoCategoryDetail.LevelOneCat).ToList(), "ID", "Name", LevelTwoCategoryDetail.LevelTwoCat);
                    ViewBag.levelThree = new SelectList(ldata, "Value", "Text");
                    ViewBag.ProductID = new SelectList(ldata, "Value", "Text");
                }
                else if (db.Categories.Where(x => x.ID == categoryID && x.Level == 3).Count() > 0)
                {
                    var LevelTwoCategoryDetail = (from c in db.Categories
                                                  join c1 in db.Categories on c.ID equals c1.ParentCategoryID
                                                  join c2 in db.Categories on c1.ID equals c2.ParentCategoryID
                                                  where c2.ID == categoryID
                                                  select new ViewBagDetail
                                                  {
                                                      LevelOneCat = c.ID,
                                                      LevelTwoCat = c1.ID
                                                  }).FirstOrDefault();

                    ViewBag.levelOne = new SelectList(db.Categories.Where(x => x.Level == 1).ToList(), "ID", "Name", LevelTwoCategoryDetail.LevelOneCat);
                    ViewBag.levelTwo = new SelectList(db.Categories.Where(x => x.Level == 2 && x.ParentCategoryID == LevelTwoCategoryDetail.LevelOneCat).ToList(), "ID", "Name", LevelTwoCategoryDetail.LevelTwoCat);
                    ViewBag.levelThree = new SelectList(db.Categories.Where(x => x.Level == 3 && x.ParentCategoryID == LevelTwoCategoryDetail.LevelTwoCat).ToList(), "ID", "Name", categoryID);
                    ViewBag.ProductID = new SelectList(ldata, "Value", "Text");
                }
            }

        }

        private void ViewBagList(Int64? levelOne, Int64? levelTwo, Int64? levelThree, Int64? ProductID)
        {

            List<SelectListItem> ldata = new List<SelectListItem>();
            ldata.Add(new SelectListItem { Text = "Select Category", Value = "0" });


            if (levelOne > 0)
            {
                ViewBag.levelOne = new SelectList(db.Categories.Where(x => x.Level == 1).ToList(), "ID", "Name", levelOne);
                ViewBag.levelTwo = new SelectList(db.Categories.Where(x => x.ParentCategoryID == levelOne).ToList(), "ID", "Name");
            }
            else
            {
                ViewBag.levelOne = new SelectList(db.Categories.Where(x => x.Level == 1).ToList(), "ID", "Name", levelOne);
                ViewBag.levelTwo = new SelectList(ldata, "Text", "Value");
            }

            if (levelTwo > 0)
            {
                ViewBag.levelTwo = new SelectList(db.Categories.Where(x => x.ParentCategoryID == levelOne && x.Level == 2).ToList(), "ID", "Name");
                ViewBag.levelThree = new SelectList(db.Categories.Where(x => x.ParentCategoryID == levelThree && x.Level == 3).ToList(), "ID", "Name");
            }
            else
            {
                ViewBag.levelThree = new SelectList(ldata, "Text", "Value");
            }

            if (levelThree > 0)
            {
                ViewBag.levelThree = new SelectList(db.Categories.Where(x => x.ParentCategoryID == levelThree && x.Level == 3).ToList(), "ID", "Name", levelThree);
                ViewBag.ProductID = new SelectList(db.Products.Where(x => x.CategoryID == levelThree), "ID", "Name");
            }
            else
            {
                ViewBag.ProductID = new SelectList(db.Products.Where(x => x.ID == ProductID), "ID", "Name");
            }

            if (ProductID > 0)
            {
                ViewBag.ProductID = new SelectList(db.Products.Where(x => x.ID == ProductID), "ID", "Name");

                ViewBag.ProductDisription = db.Products.Where(x => x.ID == ProductID).FirstOrDefault().Description;
            }

        }

        public JsonResult CaltegoryList(long id)
        {
            List<CatList> ls = new List<CatList>();
            ls = (from n in db.Categories
                  where n.ParentCategoryID == id
                  select new CatList
                  {
                      ID = n.ID,
                      Name = n.Name
                  }).ToList();

            return Json(ls, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ProductList(long id)
        {
            List<CatList> ls = new List<CatList>();
            ls = (from n in db.Products
                  where n.CategoryID == id
                  select new CatList
                  {
                      ID = n.ID,
                      Name = n.Name
                  }
                  ).ToList();

            return Json(ls, JsonRequestBehavior.AllowGet);

        }

        public JsonResult SelectedCategory(long id)
        {
            List<CatList> catList = new List<CatList>();
            catList = this.CategoryListByID(id);
            return Json(catList.ToList().OrderBy(x => x.Name), JsonRequestBehavior.AllowGet);
        }

        public JsonResult SelectedProduct(long id)
        {
            List<CatList> CompleteList = new List<CatList>();
            long productCategoryID = db.Products.Where(x => x.ID == id).FirstOrDefault().CategoryID;

            CompleteList = this.CategoryListByID(productCategoryID).Concat(ProductListByID(id, 0)).ToList();

            return Json(CompleteList, JsonRequestBehavior.AllowGet);

        }

        private List<CatList> ProductListByID(long ProductId, long ThreeLevelCategoryID)
        {
            List<CatList> productList = new List<CatList>();
            if (ProductId > 0)
            {
                long productCategoryID = db.Products.Where(x => x.ID == ProductId).FirstOrDefault().CategoryID;
             
                productList = (from n in db.Products
                               where n.CategoryID == productCategoryID && n.IsActive == true
                               select new CatList
                               {
                                   ID = n.ID,
                                   Name = n.Name,
                                   selectedID = n.ID == ProductId ? true : false
                               }).ToList();
            }
            else if (ThreeLevelCategoryID > 0)
            {
                productList = (from n in db.Products
                               where n.CategoryID == ThreeLevelCategoryID && n.IsActive == true
                               select new CatList
                               {
                                   ID = n.ID,
                                   Name = n.Name,
                                   selectedID = n.ID == ProductId ? true : false
                               }).ToList();
            }
            return productList;
        }
        private List<CatList> CategoryListByID(long id)
        {
            List<CatList> catList = new List<CatList>();

            if (db.Categories.Where(x => x.ID == id).Count() > 0)
            {
                long SelectedparentID = db.Categories.Where(x => x.ID == id).FirstOrDefault().ParentCategoryID == null
                        ? 0 : Convert.ToInt64(db.Categories.Where(x => x.ID == id).FirstOrDefault().ParentCategoryID);

                if (db.Categories.Where(x => x.ID == id).FirstOrDefault().Level == 3)
                {
                    List<CatList> catListThreeLevel = new List<CatList>();
                    catListThreeLevel = (from c3 in db.Categories
                                         where c3.Level == 3 && c3.ParentCategoryID == SelectedparentID
                                         select new CatList
                                         {
                                             ID = c3.ID,
                                             Name = c3.Name,
                                             ParentID = c3.ParentCategoryID,
                                             selectedID = c3.ID == id ? true : false,
                                             level = 3
                                         }).ToList();

                    List<CatList> catListTwoLevel = new List<CatList>();
                    long SelectedparentIDLevelTwo = db.Categories.Where(x => x.ID == SelectedparentID).FirstOrDefault().ParentCategoryID == null
                       ? 0 : Convert.ToInt64(db.Categories.Where(x => x.ID == SelectedparentID).FirstOrDefault().ParentCategoryID);
                    catListTwoLevel = (from c2 in db.Categories
                                       where c2.Level == 2 && c2.ParentCategoryID == SelectedparentIDLevelTwo
                                       select new CatList
                                       {
                                           ID = c2.ID,
                                           Name = c2.Name,
                                           ParentID = c2.ParentCategoryID,
                                           selectedID = c2.ID == SelectedparentID ? true : false,
                                           level = 2
                                       }).ToList();

                    List<CatList> catListOneLevel = new List<CatList>();
                    long SelectedparentIDLevelOne = db.Categories.Where(x => x.ID == SelectedparentIDLevelTwo).FirstOrDefault().ID == null
                       ? 0 : Convert.ToInt64(db.Categories.Where(x => x.ID == SelectedparentIDLevelTwo).FirstOrDefault().ID);
                    catListOneLevel = (from c1 in db.Categories
                                       where c1.Level == 1
                                       select new CatList
                                       {
                                           ID = c1.ID,
                                           Name = c1.Name,
                                           ParentID = c1.ParentCategoryID,
                                           selectedID = c1.ID == SelectedparentIDLevelOne ? true : false,
                                           level = 1
                                       }).ToList();

                    catList = catListThreeLevel.Concat(catListTwoLevel).Concat(catListOneLevel).Concat(ProductListByID(0,Convert.ToInt64(catListThreeLevel.Where(x => x.selectedID== true).FirstOrDefault().ID))).ToList();
                }
                else if (db.Categories.Where(x => x.ID == id).FirstOrDefault().Level == 2)
                {
                    List<CatList> catListThreeLevel = new List<CatList>();
                    catListThreeLevel = (from c3 in db.Categories
                                         where c3.Level == 3 && c3.ParentCategoryID == id
                                         select new CatList
                                         {
                                             ID = c3.ID,
                                             Name = c3.Name,
                                             ParentID = c3.ParentCategoryID,
                                             selectedID = c3.ID == id ? true : false,
                                             level = 3
                                         }).ToList();

                    List<CatList> catListTwoLevel = new List<CatList>();
                    catListTwoLevel = (from c2 in db.Categories
                                       where c2.Level == 2 && c2.ParentCategoryID == SelectedparentID
                                       select new CatList
                                       {
                                           ID = c2.ID,
                                           Name = c2.Name,
                                           ParentID = c2.ParentCategoryID,
                                           selectedID = c2.ID == SelectedparentID ? true : false,
                                           level = 2
                                       }).ToList();


                    List<CatList> catListOneLevel = new List<CatList>();
                    long SelectedparentIDLevelOne = db.Categories.Where(x => x.ID == SelectedparentID).FirstOrDefault().ParentCategoryID == null
                       ? 0 : Convert.ToInt64(db.Categories.Where(x => x.ID == SelectedparentID).FirstOrDefault().ParentCategoryID);
                    catListOneLevel = (from c1 in db.Categories
                                       where c1.Level == 1 && c1.ParentCategoryID == SelectedparentIDLevelOne
                                       select new CatList
                                       {
                                           ID = c1.ID,
                                           Name = c1.Name,
                                           ParentID = c1.ParentCategoryID,
                                           selectedID = c1.ID == SelectedparentID ? true : false,
                                           level = 1
                                       }).ToList();

                    catList = catListOneLevel.Concat(catListTwoLevel).ToList();
                }
                else if (db.Categories.Where(x => x.ID == id).FirstOrDefault().Level == 1)
                {
                    List<CatList> catListTwoLevel = new List<CatList>();
                    catListTwoLevel = (from c2 in db.Categories
                                       where c2.Level == 2 && c2.ParentCategoryID == id
                                       select new CatList
                                       {
                                           ID = c2.ID,
                                           Name = c2.Name,
                                           ParentID = c2.ParentCategoryID,
                                           selectedID = c2.ID == SelectedparentID ? true : false,
                                           level = 2
                                       }).ToList();

                    List<CatList> catListOneLevel = new List<CatList>();
                    catListOneLevel = (from c1 in db.Categories
                                       where c1.Level == 1 && c1.ID == id
                                       select new CatList
                                       {
                                           ID = c1.ID,
                                           Name = c1.Name,
                                           ParentID = c1.ParentCategoryID,
                                           selectedID = c1.ID == SelectedparentID ? true : false,
                                           level = 1
                                       }).ToList();

                    catList = catListOneLevel.Concat(catListTwoLevel).ToList();
                }

            }

            return catList;
        }
        

        class CatList
        {
            public long ID { get; set; }
            public string Name { get; set; }

            public int level { get; set; }
            public long? ParentID { get; set; }
            public bool selectedID{get; set;}
        }

        class ViewBagDetail
        {
            public int LevelOneCat { get; set; }
            public int LevelTwoCat { get; set; }
            public int LevelThreeCat { get; set; }
            public long ProductID { get; set; }
        }

    }
}
