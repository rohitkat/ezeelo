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
using CRM.Models;
using CRM.Models.ViewModel;

namespace CRM.Controllers
{
    public class ForLoopClass //----------------use this class for loop purpose in below functions----------------
    {
        public long ID { get; set; }
        public string Name { get; set; }
    }


    public class WishListController : Controller
    {
        private EzeeloDBContext db = new EzeeloDBContext();
        private CustomerCareSessionViewModel customerCareSessionViewModel = new CustomerCareSessionViewModel();
        private int pageSize = 10;

        public void SessionDetails()
        {
            customerCareSessionViewModel.UserLoginID = Convert.ToInt64(Session["ID"]);
            customerCareSessionViewModel.Username = Session["UserName"].ToString();
            Common.Common.GetAllLoginDetailFromSession(ref customerCareSessionViewModel);
        }

        [SessionExpire]
        public ActionResult Index(string FromDate, string ToDate, int? page, string SearchString = "", string CategoryLevel0 = "", string CategoryLevel1 = "", string ddlProduct = "")
        {
            SessionDetails();
            int pageNumber = (page ?? 1);
            ViewBag.PageNumber = pageNumber;
            ViewBag.PageSize = pageSize;


            ViewBag.CategoryLevel0 = new SelectList(db.Categories.Where(c => c.Level == 1), "ID", "Name");
            //ViewBag.CategoryLevel1 = new SelectList(db.Categories.Where(c => c.Level == 2), "ID", "Name");
            //ViewBag.ProductID = new SelectList(db.Products, "ID", "Name");


            var wishlists = db.WishLists.Include(w => w.PersonalDetail).Include(w => w.PersonalDetail1).Include(w => w.ShopStock).Include(w => w.UserLogin).ToList();

            List<ProductByShopStockViewModel> ProductByShopStockViewModels = (from PR in db.Products
                                                                              join ShopPr in db.ShopProducts on PR.ID equals ShopPr.ProductID
                                                                              join ShopSt in db.ShopStocks on ShopPr.ID equals ShopSt.ShopProductID
                                                                              join CuOrDt in db.CustomerOrderDetails on ShopSt.ID equals CuOrDt.ShopStockID
                                                                              select new ProductByShopStockViewModel
                                                                              {
                                                                                  ShopStockID = ShopSt.ID,
                                                                                  ShopProductID = ShopPr.ID,
                                                                                  ProductID = PR.ID,
                                                                                  ProductName = PR.Name
                                                                              }).ToList();

            ViewBag.ProductByShopStockViewModels = ProductByShopStockViewModels;


            if ((FromDate != null && FromDate != "") || (ToDate != null && ToDate != ""))
            {
                DateTime lFromDate = DateTime.Now;
                if (DateTime.TryParse(FromDate, out lFromDate)) { }

                DateTime lToDate = DateTime.Now;
                if (DateTime.TryParse(ToDate, out lToDate)) { }

                wishlists = wishlists.Where(x => x.CreateDate.Date >= lFromDate.Date && x.CreateDate.Date <= lToDate.Date).ToList();
            }
            if (ddlProduct != null && ddlProduct != "")
            {
                wishlists = wishlists.Where(x => x.ShopStock.ShopProduct.ProductID.ToString() == ddlProduct).ToList();
            }

            return View(wishlists.ToPagedList(pageNumber, pageSize));
            // return View(wishlists.ToList());
        }


        public JsonResult GetCategoryLevel1ByParentCategory(int categoryID)
        {
            //var district = (List<District>)db.Districts.Where(u => u.StateID == stateID).ToList();
            //var district = from cust in db.States 
            //                        select cust;
            List<Category> lCategory = new List<Category>();
            //List<City> lcity = new List<City>();
            List<ForLoopClass> forloopclasses = new List<ForLoopClass>();
            lCategory = db.Categories.Where(x => x.ParentCategoryID == categoryID).ToList();
            foreach (var c in lCategory)
            {
                ForLoopClass av = new ForLoopClass();
                av.ID = c.ID;
                av.Name = c.Name;
                forloopclasses.Add(av);
            }

            return Json(forloopclasses.Distinct().OrderBy(x => x.Name).ToList(), JsonRequestBehavior.AllowGet);
        }


        public JsonResult GetCategoryLevel2ByCategoryLevel1(int categoryID)
        {
            //var district = (List<District>)db.Districts.Where(u => u.StateID == stateID).ToList();
            //var district = from cust in db.States 
            //                        select cust;
            List<Category> lCategory = new List<Category>();
            //List<City> lcity = new List<City>();
            List<ForLoopClass> forloopclasses = new List<ForLoopClass>();
            lCategory = db.Categories.Where(x => x.ParentCategoryID == categoryID).ToList();
            foreach (var c in lCategory)
            {
                ForLoopClass av = new ForLoopClass();
                av.ID = c.ID;
                av.Name = c.Name;
                forloopclasses.Add(av);
            }

            return Json(forloopclasses.Distinct().OrderBy(x => x.Name).ToList(), JsonRequestBehavior.AllowGet);
        }


        public JsonResult GetPoductByCategoryId(int categoryID)
        {
            //var district = (List<District>)db.Districts.Where(u => u.StateID == stateID).ToList();
            //var district = from cust in db.States 
            //                        select cust;
            List<Product> lProduct = new List<Product>();
            //List<City> lcity = new List<City>();
            List<ForLoopClass> forloopclasses = new List<ForLoopClass>();
            lProduct = db.Products.Where(x => x.CategoryID == categoryID).ToList();
            foreach (var p in lProduct)
            {
                ForLoopClass av = new ForLoopClass();
                av.ID = p.ID;
                av.Name = p.Name;
                forloopclasses.Add(av);
            }

            return Json(forloopclasses.Distinct().OrderBy(x => x.Name).ToList(), JsonRequestBehavior.AllowGet);
        }


        // GET: /WishList/Details/5
        [SessionExpire]
        public ActionResult Details(int? id)
        {
            SessionDetails();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            WishList wishlist = db.WishLists.Find(id);
            if (wishlist == null)
            {
                return HttpNotFound();
            }
            return View(wishlist);
        }



        //// GET: /WishList/Edit/5
        //[SessionExpire]
        //public ActionResult Edit(int? id)
        //{
        //    SessionDetails();
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    WishList wishlist = db.WishLists.Find(id);
        //    if (wishlist == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    ViewBag.CreateBy = new SelectList(db.PersonalDetails, "ID", "FirstName", wishlist.CreateBy);
        //    ViewBag.ModifyBy = new SelectList(db.PersonalDetails, "ID", "FirstName", wishlist.ModifyBy);
        //    ViewBag.ShopStockID = new SelectList(db.ShopStocks, "ID", "NetworkIP", wishlist.ShopStockID);
        //    ViewBag.UserLoginID = new SelectList(db.UserLogins, "ID", "Mobile", wishlist.UserLoginID);
        //    return View(wishlist);
        //}

        //// POST: /WishList/Edit/5
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Edit([Bind(Include = "ID,UserLoginID,ShopStockID,CreateDate,CreateBy,ModifyDate,ModifyBy,NetworkIP,DeviceType,DeviceID")] WishList wishlist)
        //{
        //    try
        //    {
        //        if (ModelState.IsValid)
        //        {
        //            db.Entry(wishlist).State = EntityState.Modified;
        //            db.SaveChanges();
        //            return RedirectToAction("Index");
        //        }
        //        ViewBag.CreateBy = new SelectList(db.PersonalDetails, "ID", "FirstName", wishlist.CreateBy);
        //        ViewBag.ModifyBy = new SelectList(db.PersonalDetails, "ID", "FirstName", wishlist.ModifyBy);
        //        ViewBag.ShopStockID = new SelectList(db.ShopStocks, "ID", "NetworkIP", wishlist.ShopStockID);
        //        ViewBag.UserLoginID = new SelectList(db.UserLogins, "ID", "Mobile", wishlist.UserLoginID);
        //        return View(wishlist);
        //    }
        //    catch (Exception ex)
        //    {
        //        ModelState.AddModelError("Error", "There's something wrong with wish list values!");

        //        //Code to write error log
        //        BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
        //            + Environment.NewLine + ex.Message + Environment.NewLine
        //            + "[WishList][POST:Edit]",
        //            BusinessLogicLayer.ErrorLog.Module.CRM, System.Web.HttpContext.Current.Server);

        //        ViewBag.CreateBy = new SelectList(db.PersonalDetails, "ID", "FirstName", wishlist.CreateBy);
        //        ViewBag.ModifyBy = new SelectList(db.PersonalDetails, "ID", "FirstName", wishlist.ModifyBy);
        //        ViewBag.ShopStockID = new SelectList(db.ShopStocks, "ID", "NetworkIP", wishlist.ShopStockID);
        //        ViewBag.UserLoginID = new SelectList(db.UserLogins, "ID", "Mobile", wishlist.UserLoginID);
        //        return View(wishlist);
        //    }
        //}
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
