using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ModelLayer.Models;
using Administrator.Models;
using System.Text;
using BusinessLogicLayer;

namespace Administrator.Controllers
{
    public class DeliveryHolidayController : Controller
    {

        private EzeeloDBContext db = new EzeeloDBContext();

        StringBuilder errStr = new StringBuilder("=====================================================================================" +
          Environment.NewLine
          + "ErrorLog Controller : DeliveryHolidayController" + Environment.NewLine);
        // GET: /Bank/
       
        // GET: /DeliveryHoliday/
        [SessionExpire]
        [CustomAuthorize(Roles = "DeliveryHoliday/CanRead")]  

        public ActionResult Index()
        {
            try
            {
                
               
                return View(db.DeliveryHolidays.ToList());
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
        [CustomAuthorize(Roles = "DeliveryHoliday/CanRead")]  
        // GET: /DeliveryHoliday/Details/5
        public ActionResult Details(int? id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                DeliveryHoliday deliveryholiday = db.DeliveryHolidays.Find(id);
                if (deliveryholiday == null)
                {
                    return HttpNotFound();
                }
                return View(deliveryholiday);
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

        [SessionExpire]
        [CustomAuthorize(Roles = "DeliveryHoliday/CanRead")]  
        // GET: /DeliveryHoliday/Create
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

        // POST: /DeliveryHoliday/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]

         [SessionExpire]
        [CustomAuthorize(Roles = "DeliveryHoliday/CanRead")]
        public ActionResult Create([Bind(Include = "ID,Date,CityID,IsActive,CreateDate,CreateBy,ModifyDate,ModifyBy,NetworkIP,DeviceType,DeviceID")] DeliveryHoliday deliveryholiday, string Date1)
        {
            try
            {

                ViewBag.CityID = new SelectList(db.Cities.OrderBy(x => x.Name).Where(c => c.IsActive == true).ToList(), "ID", "Name");
                ///////////added
                List<SelectListItem> lData = new List<SelectListItem>();
                lData.Add(new SelectListItem { Text = string.Empty, Value = "0" });
                ViewBag.FranchiseID = new SelectList(lData, "Value", "Text");
                ///////////
                var cID = db.DeliveryHolidays.Where(x => x.Date == deliveryholiday.Date).Select(x => x.CityID).FirstOrDefault();
                var fID = db.DeliveryHolidays.Where(x => x.Date == deliveryholiday.Date).Select(x => x.FranchiseID).FirstOrDefault();////added
                if (db.DeliveryHolidays.Where(x => x.Date == deliveryholiday.Date).Count() > 0 && cID == deliveryholiday.CityID && fID == deliveryholiday.FranchiseID)////added && fID == deliveryschedule.FranchiseID
                {
                    ViewBag.lblError = "Delivery holiday is already Exist..!!";
                    ViewBag.FranchiseID = new SelectList(db.Franchises.Where(x => x.ID != 1 && x.IsActive == true && x.BusinessDetail.UserLogin.IsLocked == false && x.BusinessDetail.Pincode.City.IsActive == true
&& x.BusinessDetail.Pincode.City.ID == deliveryholiday.CityID).ToList(), "ID", "ContactPerson", deliveryholiday.FranchiseID);////added
                    return View(deliveryholiday);
                }
                DateTime lDate = DateTime.Now;
                if (Date1 != "")
                {

                    lDate = CommonFunctions.GetProperDateTime(Date1);
                    deliveryholiday.Date = lDate;
                }
                deliveryholiday.CreateDate = DateTime.UtcNow.AddHours(5.3);
                deliveryholiday.CreateBy = CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(Session["ID"]));
                deliveryholiday.NetworkIP = CommonFunctions.GetClientIP();

                if (ModelState.IsValid)
                {
                    db.DeliveryHolidays.Add(deliveryholiday);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }

                return View(deliveryholiday);
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

        [SessionExpire]
        [CustomAuthorize(Roles = "DeliveryHoliday/CanRead")]  
        // GET: /DeliveryHoliday/Edit/5
        public ActionResult Edit(int? id)
        {
            try
            {

                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                DeliveryHoliday deliveryholiday = db.DeliveryHolidays.Find(id);
                deliveryholiday.CityID = db.DeliveryHolidays.Where(x => x.ID == id).Select(y=>y.CityID).FirstOrDefault();
                deliveryholiday.FranchiseID = db.DeliveryHolidays.Where(x => x.ID == id).Select(y => y.FranchiseID).FirstOrDefault();////added
                if (deliveryholiday == null)
                {
                    return HttpNotFound();
                }
                ViewBag.Date1 = deliveryholiday.Date.ToString("dd/MM/yyyy");
                ViewBag.CityID = new SelectList(db.Cities.OrderBy(x => x.Name).Where(c => c.IsActive == true).ToList(), "ID", "Name",deliveryholiday.CityID);
                ViewBag.FranchiseID = new SelectList(db.Franchises.Where(x => x.ID != 1 && x.IsActive == true && x.BusinessDetail.UserLogin.IsLocked == false && x.BusinessDetail.Pincode.City.IsActive == true
           && x.BusinessDetail.Pincode.City.ID == deliveryholiday.CityID).ToList(), "ID", "ContactPerson", deliveryholiday.FranchiseID);////added
                return View(deliveryholiday);
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

        // POST: /DeliveryHoliday/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [SessionExpire]
        [CustomAuthorize(Roles = "DeliveryHoliday/CanRead")]
        public ActionResult Edit([Bind(Include = "ID,Date,CityID,IsActive,CreateDate,CreateBy,ModifyDate,ModifyBy,NetworkIP,DeviceType,DeviceID,FranchiseID")] DeliveryHoliday deliveryholiday, string Date1)////added ,FranchiseID
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
                var cID = db.DeliveryHolidays.Where(x => x.Date == deliveryholiday.Date).Select(x => x.CityID).FirstOrDefault();
                var fID = db.DeliveryHolidays.Where(x => x.Date == deliveryholiday.Date).Select(x => x.FranchiseID).FirstOrDefault();////added
                if (db1.DeliveryHolidays.Where(x => x.Date == deliveryholiday.Date && x.ID != deliveryholiday.ID).Count() > 0 && cID == deliveryholiday.CityID && fID == deliveryholiday.FranchiseID)////added && fID == deliveryschedule.FranchiseID
                {
                    ViewBag.lblError = "Delivery holiday name is already Exist..!!";
                    ViewBag.FranchiseID = new SelectList(db.Franchises.Where(x => x.ID != 1 && x.IsActive == true && x.BusinessDetail.UserLogin.IsLocked == false && x.BusinessDetail.Pincode.City.IsActive == true
&& x.BusinessDetail.Pincode.City.ID == deliveryholiday.CityID).ToList(), "ID", "ContactPerson", deliveryholiday.FranchiseID);////added
                    return View(deliveryholiday);
                }
               
                DeliveryHoliday ldeliveryHoliday = db1.DeliveryHolidays.Find(deliveryholiday.ID);
                deliveryholiday.CreateDate = ldeliveryHoliday.CreateDate;
                deliveryholiday.CreateBy = ldeliveryHoliday.CreateBy;
                deliveryholiday.ModifyDate = DateTime.UtcNow.AddHours(5.5);
                deliveryholiday.ModifyBy = CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(Session["ID"]));
                deliveryholiday.NetworkIP = CommonFunctions.GetClientIP();
                deliveryholiday.DeviceType = string.Empty;
                deliveryholiday.DeviceID = string.Empty;
               
                DateTime lDate = DateTime.Now;

                if (Date1 != "")
                {

                    lDate = CommonFunctions.GetProperDateTime(Date1);
                    deliveryholiday.Date = lDate;
                }
                db1.Dispose();
                if (ModelState.IsValid)
                {
                    db.Entry(deliveryholiday).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                return View(deliveryholiday);
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

        [SessionExpire]
        [CustomAuthorize(Roles = "DeliveryHoliday/CanRead")]  
        // GET: /DeliveryHoliday/Delete/5
        public ActionResult Delete(int? id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                DeliveryHoliday deliveryholiday = db.DeliveryHolidays.Find(id);
                if (deliveryholiday == null)
                {
                    return HttpNotFound();
                }
                return View(deliveryholiday);
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

        [SessionExpire]
        [CustomAuthorize(Roles = "DeliveryHoliday/CanRead")]  
        // POST: /DeliveryHoliday/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                DeliveryHoliday deliveryholiday = db.DeliveryHolidays.Find(id);
                db.DeliveryHolidays.Remove(deliveryholiday);
                db.SaveChanges();
                return RedirectToAction("Index");
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

        public JsonResult getFranchise(int CityID)////added
        {
            List<tempData> objODP = new List<tempData>();

            objODP = db.Franchises
                    .Where(x => x.ID != 1 && x.IsActive == true && x.BusinessDetail.UserLogin.IsLocked == false && x.BusinessDetail.Pincode.City.IsActive == true
                    && x.BusinessDetail.Pincode.City.ID == CityID)
                   // .Select(x => new tempData { text = x.ID.ToString(), value = x.ID } ////ContactPerson->ID
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
