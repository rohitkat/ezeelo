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

namespace Administrator.Controllers
{
    public class DeliveryScheduleController : Controller
    {
        private EzeeloDBContext db = new EzeeloDBContext();

        StringBuilder errStr = new StringBuilder("=====================================================================================" +
         Environment.NewLine
         + "ErrorLog Controller : DeliveryScheduleController" + Environment.NewLine);

        // GET: /DeliverySchedule/
        [SessionExpire]
        [CustomAuthorize(Roles = "DeliverySchedule/CanRead")]
        public ActionResult Index()
        {
            try
            {
                return View(db.DeliverySchedules.ToList());
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

        [SessionExpire]
        [CustomAuthorize(Roles = "DeliverySchedule/CanRead")]

        // GET: /DeliverySchedule/Details/5
        public ActionResult Details(long? id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                DeliverySchedule deliveryschedule = db.DeliverySchedules.Find(id);
                if (deliveryschedule == null)
                {
                    return HttpNotFound();
                }
                return View(deliveryschedule);
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

        [SessionExpire]
        [CustomAuthorize(Roles = "DeliverySchedule/CanRead")]
        // GET: /DeliverySchedule/Create
        public ActionResult Create()
        {
            try
            {
                ViewBag.CityID = new SelectList(db.Cities.OrderBy(x => x.Name).Where(c => c.IsActive == true).ToList(), "ID", "Name");
                ////////////added
                List<SelectListItem> lData = new List<SelectListItem>();
                lData.Add(new SelectListItem { Text = string.Empty, Value = "0" });
                ViewBag.FranchiseID = new SelectList(lData, "Value", "Text");
                /////////////////
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

        // POST: /DeliverySchedule/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [SessionExpire]
        [CustomAuthorize(Roles = "DeliverySchedule/CanRead")]
        public ActionResult Create([Bind(Include = "ID,DisplayName,ActualTimeFrom,ActualTimeTo,CityID,NoOfDelivery,IsActive,CreateDate,CreateBy,ModifyDate,ModifyBy,NetworkIP,DeviceType,DeviceID,FranchiseID")] DeliverySchedule deliveryschedule)////added ,FranchiseID
        {
            try
            {
                ViewBag.CityID = new SelectList(db.Cities.OrderBy(x => x.Name).Where(c => c.IsActive == true).ToList(), "ID", "Name");

                ///////////added
                List<SelectListItem> lData = new List<SelectListItem>();
                lData.Add(new SelectListItem { Text = string.Empty, Value = "0" });
                ViewBag.FranchiseID = new SelectList(lData, "Value", "Text");
                ///////////

                var cID = db.DeliverySchedules.Where(x => x.DisplayName == deliveryschedule.DisplayName).Select(x => x.CityID).FirstOrDefault();
                var fID = db.DeliverySchedules.Where(x => x.DisplayName == deliveryschedule.DisplayName).Select(x => x.FranchiseID).FirstOrDefault();////added
                if (db.DeliverySchedules.Where(x => x.DisplayName == deliveryschedule.DisplayName).Count() > 0 && cID == deliveryschedule.CityID && fID == deliveryschedule.FranchiseID)////added && fID == deliveryschedule.FranchiseID
                {
                    ViewBag.lblError = "Delivery schedule name is already Exist..!!";
                    ViewBag.FranchiseID = new SelectList(db.Franchises.Where(x => x.ID != 1 && x.IsActive == true && x.BusinessDetail.UserLogin.IsLocked == false && x.BusinessDetail.Pincode.City.IsActive == true
&& x.BusinessDetail.Pincode.City.ID == deliveryschedule.CityID).ToList(), "ID", "ContactPerson", deliveryschedule.FranchiseID);////added
                    return View(deliveryschedule);
                }

                deliveryschedule.CreateDate = DateTime.UtcNow.AddHours(5.3);
                deliveryschedule.CreateBy = CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(Session["ID"]));
                deliveryschedule.NetworkIP = CommonFunctions.GetClientIP();

                if (ModelState.IsValid)
                {
                    db.DeliverySchedules.Add(deliveryschedule);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }

                return View(deliveryschedule);
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

        [SessionExpire]
        [CustomAuthorize(Roles = "DeliverySchedule/CanRead")]
        // GET: /DeliverySchedule/Edit/5
        public ActionResult Edit(long? id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                DeliverySchedule deliveryschedule = db.DeliverySchedules.Find(id);

                deliveryschedule.CityID = db.DeliverySchedules.Where(x => x.ID == id).Select(y => y.CityID).FirstOrDefault();
                deliveryschedule.FranchiseID = db.DeliverySchedules.Where(x => x.ID == id).Select(y => y.FranchiseID).FirstOrDefault();////added
                if (deliveryschedule == null)
                {
                    return HttpNotFound();
                }
                ViewBag.CityID = new SelectList(db.Cities.OrderBy(x => x.Name).Where(c => c.IsActive == true).ToList(), "ID", "Name", deliveryschedule.CityID);
                ViewBag.FranchiseID = new SelectList(db.Franchises.Where(x => x.ID != 1 && x.IsActive == true && x.BusinessDetail.UserLogin.IsLocked == false && x.BusinessDetail.Pincode.City.IsActive == true
             && x.BusinessDetail.Pincode.City.ID == deliveryschedule.CityID).ToList(), "ID", "ContactPerson", deliveryschedule.FranchiseID);////added
                return View(deliveryschedule);
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

        // POST: /DeliverySchedule/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [SessionExpire]
        [CustomAuthorize(Roles = "DeliverySchedule/CanRead")]
        public ActionResult Edit([Bind(Include = "ID,DisplayName,ActualTimeFrom,ActualTimeTo,CityID,NoOfDelivery,IsActive,CreateDate,CreateBy,ModifyDate,ModifyBy,NetworkIP,DeviceType,DeviceID,FranchiseID")] DeliverySchedule deliveryschedule)////added ,FranchiseID
        {
            try
            {

                EzeeloDBContext db1 = new EzeeloDBContext();
                ViewBag.CityID = new SelectList(db.Cities.OrderBy(x => x.Name).Where(c => c.IsActive == true).ToList(), "ID", "Name");
                //////////////added
                List<SelectListItem> lData = new List<SelectListItem>();
                lData.Add(new SelectListItem { Text = string.Empty, Value = "0" });
                ViewBag.FranchiseID = new SelectList(lData, "Value", "Text");
                //////////////

                var cID = db.DeliverySchedules.Where(x => x.DisplayName == deliveryschedule.DisplayName).Select(x => x.CityID).FirstOrDefault();
                var fID = db.DeliverySchedules.Where(x => x.DisplayName == deliveryschedule.DisplayName).Select(x => x.FranchiseID).FirstOrDefault();////added
                if (db1.DeliverySchedules.Where(x => x.DisplayName == deliveryschedule.DisplayName && x.ID != deliveryschedule.ID).Count() > 0 && cID == deliveryschedule.CityID && fID == deliveryschedule.FranchiseID)////added && fID == deliveryschedule.FranchiseID
                {
                    ViewBag.lblError = "Delivery schedule name is already Exist..!!";
                    ViewBag.FranchiseID = new SelectList(db.Franchises.Where(x => x.ID != 1 && x.IsActive == true && x.BusinessDetail.UserLogin.IsLocked == false && x.BusinessDetail.Pincode.City.IsActive == true
&& x.BusinessDetail.Pincode.City.ID == deliveryschedule.CityID).ToList(), "ID", "ContactPerson", deliveryschedule.FranchiseID);////added
                    return View(deliveryschedule);
                }
                DeliverySchedule ldeliveryschedule = db1.DeliverySchedules.Find(deliveryschedule.ID);
                deliveryschedule.CreateDate = ldeliveryschedule.CreateDate;
                deliveryschedule.CreateBy = ldeliveryschedule.CreateBy;
                deliveryschedule.ModifyDate = DateTime.UtcNow.AddHours(5.5);
                deliveryschedule.ModifyBy = CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(Session["ID"]));
                deliveryschedule.NetworkIP = CommonFunctions.GetClientIP();
                deliveryschedule.DeviceType = string.Empty;
                deliveryschedule.DeviceID = string.Empty;
                db1.Dispose();
                if (ModelState.IsValid)
                {
                    db.Entry(deliveryschedule).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }

                return View(deliveryschedule);
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

        [SessionExpire]
        [CustomAuthorize(Roles = "DeliverySchedule/CanRead")]
        // GET: /DeliverySchedule/Delete/5
        public ActionResult Delete(long? id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                DeliverySchedule deliveryschedule = db.DeliverySchedules.Find(id);
                if (deliveryschedule == null)
                {
                    return HttpNotFound();
                }
                return View(deliveryschedule);
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

        // POST: /DeliverySchedule/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [SessionExpire]
        [CustomAuthorize(Roles = "DeliverySchedule/CanRead")]
        public ActionResult DeleteConfirmed(long id)
        {
            try
            {
                // EzeeloDBContext db1 = new EzeeloDBContext();
                DeliverySchedule deliveryschedule = db.DeliverySchedules.Find(id);
                db.DeliverySchedules.Remove(deliveryschedule);
                db.SaveChanges();
                return RedirectToAction("Index");
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
                ViewBag.referenceMsg = "Entry you wish to delete is already used by customer while placing order";
                return View();
            }
        }

        public JsonResult getFranchise(int CityID)////added
        {
            List<tempData> objODP = new List<tempData>();

            objODP = db.Franchises
                    .Where(x => x.ID != 1 && x.IsActive == true && x.BusinessDetail.UserLogin.IsLocked == false && x.BusinessDetail.Pincode.City.IsActive == true
                    && x.BusinessDetail.Pincode.City.ID == CityID)
                    //.Select(x => new tempData { text = x.ID.ToString(), value = x.ID } ////ContactPerson->ID
                     .Select(x => new tempData { text = x.ContactPerson.ToString(), value = x.ID }
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
