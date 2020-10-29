using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ModelLayer.Models;

namespace Administrator.Controllers
{
    public class CoupenListController : Controller
    {
        private EzeeloDBContext db = new EzeeloDBContext();

        private long GetPersonalDetailID()
        {
            //Session["USER_LOGIN_ID"] = 1;
            long UserLoginID = Convert.ToInt32(Session["ID"]);
            long PersonalDetailID = 0;
            try
            {
                if (UserLoginID > 0)
                {
                    PersonalDetailID = Convert.ToInt32(db.PersonalDetails.Where(x => x.UserLoginID == UserLoginID).Select(x => x.ID).First());
                }
            }
            catch (Exception ex)
            {
                throw new BusinessLogicLayer.MyException("[PlacedController][GetShopID]", "Can't find PersonalDetailID !" + Environment.NewLine + ex.Message);
            }
            return PersonalDetailID;
        }
        // GET: /CoupenList/
        [Authorize(Roles = "CoupenList/CanRead")]
        public ActionResult Index()
        {
            try
            {
                var coupenlists = db.CoupenLists.Include(c => c.City).Include(c => c.PersonalDetail).Include(c => c.PersonalDetail1).Include(c => c.SchemeType);
                return View(coupenlists.OrderByDescending(x=>x.ID).Take(30).ToList());
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Error", "There's Something wrong in CoupenList Index page !!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[CoupenListController][POST:GetReport]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Error", "There's Something wrong in in CoupenList Index page!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[CoupenListController][POST:GetReport]",
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
            }
            return View();
            
        }

        // GET: /CoupenList/Details/5
        [Authorize(Roles = "CoupenList/CanRead")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CoupenList coupenlist = db.CoupenLists.Find(id);
            if (coupenlist == null)
            {
                return HttpNotFound();
            }
            return View(coupenlist);
        }

        // GET: /CoupenList/Create
       [Authorize(Roles = "CoupenList/CanRead")]
        public ActionResult Create()
        {
            ViewBag.CityID = new SelectList(db.Cities, "ID", "Name");
            ViewBag.SchemeTypeID = new SelectList(db.SchemeTypes, "ID", "Name");
            ////////////added
            List<SelectListItem> lData = new List<SelectListItem>();
            lData.Add(new SelectListItem { Text = string.Empty, Value = "0" });
            ViewBag.FranchiseID = new SelectList(lData, "Value", "Text");
            /////////////////
            return View();
        }

        // POST: /CoupenList/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "CoupenList/CanWrite")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,SchemeTypeID,CoupenCode,CoupenQty,UsedQty,CityID,IsActive,CreateDate,CreateBy,ModifyDate,ModifyBy,NetworkIP,DeviceType,DeviceID,FranchiseID")] CoupenList coupenlist)////added ,FranchiseID
        {
            try
            {
                ///////////added
                ViewBag.CityID = new SelectList(db.Cities.OrderBy(x => x.Name).Where(c => c.IsActive == true).ToList(), "ID", "Name");

                List<SelectListItem> lData = new List<SelectListItem>();
                lData.Add(new SelectListItem { Text = string.Empty, Value = "0" });
                ViewBag.FranchiseID = new SelectList(lData, "Value", "Text");
                ///////////

                if (ModelState.IsValid)
                {
                    ViewBag.CityID = new SelectList(db.Cities, "ID", "Name", coupenlist.CityID);
                    ViewBag.SchemeTypeID = new SelectList(db.SchemeTypes, "ID", "Name", coupenlist.SchemeTypeID);

                    if (!db.CoupenLists.Any(m => m.CoupenCode == coupenlist.CoupenCode))
                    {
                        long PersonalDetailID = this.GetPersonalDetailID();

                        CoupenList lCoupenList = new CoupenList();
                        lCoupenList.SchemeTypeID = coupenlist.SchemeTypeID;
                        lCoupenList.CoupenCode = coupenlist.CoupenCode;
                        lCoupenList.CoupenQty = coupenlist.CoupenQty;
                        lCoupenList.UsedQty = coupenlist.UsedQty;
                        lCoupenList.CityID = coupenlist.CityID;
                        lCoupenList.IsActive = coupenlist.IsActive;
                        lCoupenList.CreateDate = DateTime.UtcNow;
                        lCoupenList.CreateBy = PersonalDetailID;
                        lCoupenList.NetworkIP = BusinessLogicLayer.CommonFunctions.GetClientIP();
                        lCoupenList.DeviceID = "x";
                        lCoupenList.DeviceType = "x";
                        lCoupenList.FranchiseID = coupenlist.FranchiseID;////added
                        db.CoupenLists.Add(lCoupenList);
                        db.SaveChanges();
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        ViewBag.Message = "Coupen Code is already Exists";
                        return View(coupenlist);
                    }
                    
                }
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Error", "There's Something wrong in CoupenList Create page !!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[CoupenListController][POST:GetReport]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Error", "There's Something wrong in in CoupenList Create page!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[CoupenListController][POST:GetReport]",
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
            }
           

            return View(coupenlist);
        }

        // GET: /CoupenList/Edit/5
        [Authorize(Roles = "CoupenList/CanRead")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CoupenList coupenlist = db.CoupenLists.Find(id);
            ///////////added
            coupenlist.CityID = db.CoupenLists.Where(x => x.ID == id).Select(y => y.CityID).FirstOrDefault();
            coupenlist.FranchiseID = db.CoupenLists.Where(x => x.ID == id).Select(y => y.FranchiseID).FirstOrDefault();
            if (coupenlist == null)
            {
                return HttpNotFound();
            }
            ////////////
            if (coupenlist == null)
            {
                return HttpNotFound();
            }
            ViewBag.CityID = new SelectList(db.Cities, "ID", "Name", coupenlist.CityID);
            ViewBag.CreateBy = new SelectList(db.PersonalDetails, "ID", "FirstName", coupenlist.CreateBy);
            ViewBag.ModifyBy = new SelectList(db.PersonalDetails, "ID", "FirstName", coupenlist.ModifyBy);
            ViewBag.SchemeTypeID = new SelectList(db.SchemeTypes, "ID", "Name", coupenlist.SchemeTypeID);
            ViewBag.FranchiseID = new SelectList(db.Franchises.Where(x => x.ID != 1 && x.IsActive == true && x.BusinessDetail.UserLogin.IsLocked == false && x.BusinessDetail.Pincode.City.IsActive == true
               && x.BusinessDetail.Pincode.City.ID == coupenlist.CityID).ToList(), "ID", "ContactPerson", coupenlist.FranchiseID);////added
            //testing
            //ViewBag.FranchiseID = new SelectList((from f in db.Franchises
            //                                      join bd in db.BusinessDetails on f.BusinessDetailID equals bd.ID
            //                                      join bt in db.BusinessTypes on bd.BusinessTypeID equals bt.ID
            //                                      join ul in db.UserLogins on bd.UserLoginID equals ul.ID
            //                                      join pd in db.PersonalDetails on ul.ID equals pd.UserLoginID
            //                                      where ul.IsLocked == false && f.IsActive == true && bt.Prefix == "GBFR" && f.ID != 1
            //                                      select new { ID = f.ID, Name = pd.Salutation.Name + " " + pd.FirstName + " " + pd.LastName + " (" + bd.Name + ")", }).ToList(), "ID", "Name", coupenlist.FranchiseID);
            return View(coupenlist);
        }

        // POST: /CoupenList/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "CoupenList/CanWrite")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,SchemeTypeID,CoupenCode,CoupenQty,UsedQty,CityID,IsActive,CreateDate,CreateBy,ModifyDate,ModifyBy,NetworkIP,DeviceType,DeviceID,FranchiseID")] CoupenList coupenlist)////added ,FranchiseID
        {
            try
            {
                //////////////added
                EzeeloDBContext db1 = new EzeeloDBContext();
                ViewBag.CityID = new SelectList(db.Cities.OrderBy(x => x.Name).Where(c => c.IsActive == true).ToList(), "ID", "Name");

                List<SelectListItem> lData = new List<SelectListItem>();
                lData.Add(new SelectListItem { Text = string.Empty, Value = "0" });
                ViewBag.FranchiseID = new SelectList(lData, "Value", "Text");
                //////////////

                if (ModelState.IsValid)
                {
                    long PersonalDetailID = this.GetPersonalDetailID();

                    CoupenList lCoupenList = db.CoupenLists.Find(coupenlist.ID);
                    lCoupenList.SchemeTypeID = coupenlist.SchemeTypeID;
                    lCoupenList.CoupenCode = coupenlist.CoupenCode;
                    lCoupenList.CoupenQty = coupenlist.CoupenQty;
                    lCoupenList.UsedQty = coupenlist.UsedQty;
                    lCoupenList.CityID = coupenlist.CityID;
                    lCoupenList.FranchiseID = coupenlist.FranchiseID;////added
                    lCoupenList.IsActive = coupenlist.IsActive;
                    lCoupenList.ModifyDate = DateTime.UtcNow;
                    lCoupenList.ModifyBy = PersonalDetailID;
                    lCoupenList.NetworkIP = BusinessLogicLayer.CommonFunctions.GetClientIP();
                    lCoupenList.DeviceID = "x";
                    lCoupenList.DeviceType = "x";
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Error", "There's Something wrong in CoupenList Edit page !!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[CoupenListController][POST:GetReport]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Error", "There's Something wrong in in CoupenList Edit page!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[CoupenListController][POST:GetReport]",
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
            }
           
            ViewBag.CityID = new SelectList(db.Cities, "ID", "Name", coupenlist.CityID);
            ViewBag.SchemeTypeID = new SelectList(db.SchemeTypes, "ID", "Name", coupenlist.SchemeTypeID);
            ViewBag.FranchiseID = new SelectList(db.Franchises.Where(x => x.ID != 1 && x.IsActive == true && x.BusinessDetail.UserLogin.IsLocked == false && x.BusinessDetail.Pincode.City.IsActive == true
   && x.BusinessDetail.Pincode.City.ID == coupenlist.CityID).ToList(), "ID", "ContactPerson", coupenlist.FranchiseID);////added

            //testing
           //ViewBag.FranchiseID= new SelectList((from f in db.Franchises
           //                 join bd in db.BusinessDetails on f.BusinessDetailID equals bd.ID
           //                 join bt in db.BusinessTypes on bd.BusinessTypeID equals bt.ID
           //                 join ul in db.UserLogins on bd.UserLoginID equals ul.ID
           //                 join pd in db.PersonalDetails on ul.ID equals pd.UserLoginID
           //                 where ul.IsLocked == false && f.IsActive == true && bt.Prefix == "GBFR" && f.ID != 1
           //                                     select new { ID = f.ID, Name = pd.Salutation.Name + " " + pd.FirstName + " " + pd.LastName + " (" + bd.Name + ")", }).ToList(), "ID", "Name", coupenlist.FranchiseID);
            return View(coupenlist);
        }

        // GET: /CoupenList/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CoupenList coupenlist = db.CoupenLists.Find(id);
            if (coupenlist == null)
            {
                return HttpNotFound();
            }
            return View(coupenlist);
        }

        // POST: /CoupenList/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            CoupenList coupenlist = db.CoupenLists.Find(id);
            db.CoupenLists.Remove(coupenlist);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public JsonResult getFranchise(int CityID)////added
        {
            List<tempData> objODP = new List<tempData>();

            objODP = db.Franchises
                    .Where(x => x.ID != 1 && x.IsActive == true && x.BusinessDetail.UserLogin.IsLocked == false && x.BusinessDetail.Pincode.City.IsActive == true
                    && x.BusinessDetail.Pincode.City.ID == CityID)
                    //.Select(x => new tempData { text = x.ID.ToString(), value = x.ID } ////ContactPerson->ID
                    .Select(x => new tempData { text = x.ContactPerson, value = x.ID }
                    ).OrderBy(x => x.text)
                    .ToList();

            return Json(objODP, JsonRequestBehavior.AllowGet);
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        public class tempData////added
        {
            public Int64 value;
            public string text;
        }
    }
}
