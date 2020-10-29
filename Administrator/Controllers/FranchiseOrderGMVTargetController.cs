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
using Administrator.Models;
using BusinessLogicLayer;
using System.Globalization;

namespace Administrator.Controllers
{
    public class FranchiseOrderGMVTargetController : Controller
    {
        private EzeeloDBContext db = new EzeeloDBContext();

        StringBuilder errStr = new StringBuilder("=====================================================================================" +
        Environment.NewLine
        + "ErrorLog Controller : FranchiseOrderGMVTargetController" + Environment.NewLine);


        // GET: /FranchiseOrderGMVTarget/
        [SessionExpire]
        [CustomAuthorize(Roles = "FranchiseOrderGMVTarget/CanRead")]
        public ActionResult Index()
        {

            try
            {
                var franchiseordergmvtarget = db.FranchiseOrderGMVTargets.Include(f => f.Franchise);
                return View(franchiseordergmvtarget.ToList());
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

        // GET: /FranchiseOrderGMVTarget/Details/5
        [SessionExpire]
        [CustomAuthorize(Roles = "FranchiseOrderGMVTarget/CanRead")]
        public ActionResult Details(long? id)
        {
            try
            { 
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FranchiseOrderGMVTarget franchiseordergmvtarget = db.FranchiseOrderGMVTargets.Find(id);
            if (franchiseordergmvtarget == null)
            {
                return HttpNotFound();
            }
            return View(franchiseordergmvtarget);
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

        // GET: /FranchiseOrderGMVTarget/Create
        [SessionExpire]
        [CustomAuthorize(Roles = "FranchiseOrderGMVTarget/CanRead")]
        public ActionResult Create()
        {
            try
            {
                Dictionary<int, string> myDict = new Dictionary<int, string>();
                for (int i = 1; i <= 12; i++)
                    myDict.Add(i, CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(i));

                ViewBag.CityID = new SelectList(db.Cities.OrderBy(x => x.Name).Where(c => c.IsActive == true).ToList(), "ID", "Name");

            List<SelectListItem> lData = new List<SelectListItem>();
            lData.Add(new SelectListItem { Text = string.Empty, Value = "0" });
            ViewBag.FranchiseID = new SelectList(lData, "Value", "Text");
            ViewBag.SeasonalStartMonth = myDict;
            return View();
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

        // POST: /FranchiseOrderGMVTarget/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [SessionExpire]
        [CustomAuthorize(Roles = "FranchiseOrderGMVTarget/CanRead")]
        public ActionResult Create([Bind(Include = "ID,FranchiseID,CityID,MonthlyOrderTarget,MonthlyGMVTarget,ForYear,FromMonth,ToMonth,IsActive,CreateDate,CreateBy,ModifyDate,ModifyBy,NetworkIP,DeviceType,DeviceID")] FranchiseOrderGMVTarget franchiseordergmvtarget)
        {
              try
            {
                ViewBag.CityID = new SelectList(db.Cities.OrderBy(x => x.Name).Where(c => c.IsActive == true).ToList(), "ID", "Name");

                List<SelectListItem> lData = new List<SelectListItem>();
                lData.Add(new SelectListItem { Text = string.Empty, Value = "0" });
                ViewBag.FranchiseID = new SelectList(lData, "Value", "Text");
            if (ModelState.IsValid)
            {
                db.FranchiseOrderGMVTargets.Add(franchiseordergmvtarget);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            franchiseordergmvtarget.CreateDate = DateTime.UtcNow.AddHours(5.3);
            franchiseordergmvtarget.CreateBy = CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(Session["ID"]));
            franchiseordergmvtarget.NetworkIP = CommonFunctions.GetClientIP();
           //// ViewBag.FranchiseID = new SelectList(db.Franchises, "ID", "ServiceNumber", franchiseordergmvtarget.FranchiseID);
            return View(franchiseordergmvtarget);
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

        // GET: /FranchiseOrderGMVTarget/Edit/5
        [SessionExpire]
        [CustomAuthorize(Roles = "FranchiseOrderGMVTarget/CanRead")]
        public ActionResult Edit(long? id)
        {
            try
            {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FranchiseOrderGMVTarget franchiseordergmvtarget = db.FranchiseOrderGMVTargets.Find(id);
            franchiseordergmvtarget.CityID = db.FranchiseOrderGMVTargets.Where(x => x.ID == id).Select(y => y.CityID).FirstOrDefault();
            franchiseordergmvtarget.FranchiseID = db.FranchiseOrderGMVTargets.Where(x => x.ID == id).Select(y => y.FranchiseID).FirstOrDefault();

            Dictionary<int, string> myDict = new Dictionary<int, string>();
            for (int i = 1; i <= 12; i++)
                myDict.Add(i, CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(i));
            ViewBag.SeasonalStartMonth = myDict;
            if (franchiseordergmvtarget == null)
            {
                return HttpNotFound();
            }
            ViewBag.CityID = new SelectList(db.Cities.OrderBy(x => x.Name).Where(c => c.IsActive == true).ToList(), "ID", "Name", franchiseordergmvtarget.CityID);
            ViewBag.FranchiseID = new SelectList(db.Franchises.Where(x => x.ID != 1 && x.IsActive == true && x.BusinessDetail.UserLogin.IsLocked == false && x.BusinessDetail.Pincode.City.IsActive == true
         && x.BusinessDetail.Pincode.City.ID == franchiseordergmvtarget.CityID).ToList(), "ID", "ContactPerson", franchiseordergmvtarget.FranchiseID);
            //ViewBag.FranchiseID = new SelectList(db.Franchises, "ID", "ServiceNumber", franchiseordergmvtarget.FranchiseID);
            return View(franchiseordergmvtarget);
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

        // POST: /FranchiseOrderGMVTarget/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [SessionExpire]
        [CustomAuthorize(Roles = "FranchiseOrderGMVTarget/CanRead")]
        public ActionResult Edit([Bind(Include = "ID,FranchiseID,CityID,MonthlyOrderTarget,MonthlyGMVTarget,ForYear,FromMonth,ToMonth,IsActive,CreateDate,CreateBy,ModifyDate,ModifyBy,NetworkIP,DeviceType,DeviceID")] FranchiseOrderGMVTarget franchiseordergmvtarget)
        {
          try
            {
                EzeeloDBContext db1 = new EzeeloDBContext();
                ViewBag.CityID = new SelectList(db.Cities.OrderBy(x => x.Name).Where(c => c.IsActive == true).ToList(), "ID", "Name");
                List<SelectListItem> lData = new List<SelectListItem>();
                lData.Add(new SelectListItem { Text = string.Empty, Value = "0" });
                ViewBag.FranchiseID = new SelectList(lData, "Value", "Text");
                FranchiseOrderGMVTarget lfranchiseordergmvtarget = db1.FranchiseOrderGMVTargets.Find(franchiseordergmvtarget.ID);
                franchiseordergmvtarget.CreateDate = lfranchiseordergmvtarget.CreateDate;
                franchiseordergmvtarget.CreateBy = lfranchiseordergmvtarget.CreateBy;
                franchiseordergmvtarget.ModifyDate = DateTime.UtcNow.AddHours(5.5);
                franchiseordergmvtarget.ModifyBy = CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(Session["ID"]));
                franchiseordergmvtarget.NetworkIP = CommonFunctions.GetClientIP();
                franchiseordergmvtarget.DeviceType = string.Empty;
                franchiseordergmvtarget.DeviceID = string.Empty;
                db1.Dispose();
            if (ModelState.IsValid)
            {
                db.Entry(franchiseordergmvtarget).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.FranchiseID = new SelectList(db.Franchises, "ID", "ServiceNumber", franchiseordergmvtarget.FranchiseID);
            return View(franchiseordergmvtarget);
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

        // GET: /FranchiseOrderGMVTarget/Delete/5
     /*   public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FranchiseOrderGMVTarget franchiseordergmvtarget = db.FranchiseOrderGMVTarget.Find(id);
            if (franchiseordergmvtarget == null)
            {
                return HttpNotFound();
            }
            return View(franchiseordergmvtarget);
        }

        // POST: /FranchiseOrderGMVTarget/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            FranchiseOrderGMVTarget franchiseordergmvtarget = db.FranchiseOrderGMVTarget.Find(id);
            db.FranchiseOrderGMVTarget.Remove(franchiseordergmvtarget);
            db.SaveChanges();
            return RedirectToAction("Index");
        }*/

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
        public JsonResult getFranchise(int CityID)
        {
            List<tempData> objODP = new List<tempData>();

            objODP = db.Franchises
                    .Where(x => x.ID != 1 && x.IsActive == true && x.BusinessDetail.UserLogin.IsLocked == false && x.BusinessDetail.Pincode.City.IsActive == true
                    && x.BusinessDetail.Pincode.City.ID == CityID)
                    .Select(x => new tempData { text = x.ContactPerson, value = x.ID }
                    ).OrderBy(x => x.text)
                    .ToList();

            return Json(objODP, JsonRequestBehavior.AllowGet);
        }
        public class tempData
        {
            public Int64 value;
            public string text;
        }

    }
}
