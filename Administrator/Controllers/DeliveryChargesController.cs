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
using System.Web;
using System.Web.Mvc;

namespace Administrator.Controllers
{
    public class DeliveryChargesController : Controller
    {
        private EzeeloDBContext db = new EzeeloDBContext();
        StringBuilder errStr = new StringBuilder("=====================================================================================" +
        Environment.NewLine
        + "ErrorLog Controller : DeliveryScheduleController" + Environment.NewLine);

        // GET: DeliveryCharges
        [SessionExpire]
        [CustomAuthorize(Roles = "DeliveryCharges/CanRead")]
        public ActionResult Index()
        {
            try
            {
                return View(db.DeliveryCharges.ToList());
                //var deliveryCharges = db.DeliveryCharges.Include(d => d.City).Include(d => d.Franchise);
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
            //return View(deliveryCharges.ToList());
        }

        // GET: DeliveryCharges/Details/5
        [SessionExpire]
        [CustomAuthorize(Roles = "DeliveryCharges/CanRead")]
        public ActionResult Details(long? id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                DeliveryCharge deliveryCharge = db.DeliveryCharges.Find(id);
                if (deliveryCharge == null)
                {
                    return HttpNotFound();
                }
                return View(deliveryCharge);
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

        // GET: DeliveryCharges/Create
        [SessionExpire]
        [CustomAuthorize(Roles = "DeliveryCharges/CanRead")]
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
                // ViewBag.FranchiseID = new SelectList(db.Franchises, "ID", "ServiceNumber");
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
                ViewBag.CityID = new SelectList(db.Cities.OrderBy(x => x.Name).Where(c => c.IsActive == true).ToList(), "ID", "Name");

                ///////////added
                List<SelectListItem> lData = new List<SelectListItem>();
                lData.Add(new SelectListItem { Text = string.Empty, Value = "0" });
                ViewBag.FranchiseID = new SelectList(lData, "Value", "Text");
                ///////////
                return View();
            }
        }

        // POST: DeliveryCharges/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [SessionExpire]
        [CustomAuthorize(Roles = "DeliveryCharges/CanRead")]
        public ActionResult Create(DeliveryCharge deliveryCharge)//Added OrderAmount by Sonali_19-01-2019//Added MinOrderAmount by Rumana_22-03-2019
        {
            try
            {


                ViewBag.CityID = new SelectList(db.Cities.OrderBy(x => x.Name).Where(c => c.IsActive == true).ToList(), "ID", "Name");

                ///////////added
                List<SelectListItem> lData = new List<SelectListItem>();
                lData.Add(new SelectListItem { Text = string.Empty, Value = "0" });
                ViewBag.FranchiseID = new SelectList(lData, "Value", "Text");
                ///////////
                ///
                var cID = db.DeliveryCharges.Where(x => x.Charges == deliveryCharge.Charges).Select(x => x.CityID).FirstOrDefault();
                var fID = db.DeliveryCharges.Where(x => x.Charges == deliveryCharge.Charges).Select(x => x.FranchiseID).FirstOrDefault();
                if (db.DeliveryCharges.Where(x => x.Charges == deliveryCharge.Charges).Count() > 0 && cID == deliveryCharge.CityID && fID == deliveryCharge.FranchiseID)////added && fID == deliveryschedule.FranchiseID
                {
                    ViewBag.lblError = "Delivery charges is already Exist..!!";
                    ViewBag.FranchiseID = new SelectList(db.Franchises.Where(x => x.ID != 1 && x.IsActive == true && x.BusinessDetail.UserLogin.IsLocked == false && x.BusinessDetail.Pincode.City.IsActive == true
                    && x.BusinessDetail.Pincode.City.ID == deliveryCharge.CityID).ToList(), "ID", "ContactPerson", deliveryCharge.FranchiseID);////added
                    return View(deliveryCharge);
                }

                deliveryCharge.CreateDate = DateTime.UtcNow.AddHours(5.3);
                deliveryCharge.CreateBy = CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(Session["ID"]));
                deliveryCharge.NetworkIP = CommonFunctions.GetClientIP();
                if (ModelState.IsValid)
                {
                    db.DeliveryCharges.Add(deliveryCharge);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                return View(deliveryCharge);
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
                ViewBag.CityID = new SelectList(db.Cities.OrderBy(x => x.Name).Where(c => c.IsActive == true).ToList(), "ID", "Name");

                ///////////added
                List<SelectListItem> lData = new List<SelectListItem>();
                lData.Add(new SelectListItem { Text = string.Empty, Value = "0" });
                ViewBag.FranchiseID = new SelectList(lData, "Value", "Text");
                ///////////
                return View();
            }
        }

        // GET: DeliveryCharges/Edit/5
        [SessionExpire]
        [CustomAuthorize(Roles = "DeliveryCharges/CanRead")]
        public ActionResult Edit(long? id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                DeliveryCharge deliveryCharge = db.DeliveryCharges.Find(id);
                deliveryCharge.CityID = db.DeliveryCharges.Where(x => x.ID == id).Select(y => y.CityID).FirstOrDefault();
                deliveryCharge.FranchiseID = db.DeliveryCharges.Where(x => x.ID == id).Select(y => y.FranchiseID).FirstOrDefault();////added

                if (deliveryCharge == null)
                {
                    return HttpNotFound();
                }
                ViewBag.CityID = new SelectList(db.Cities.OrderBy(x => x.Name).Where(c => c.IsActive == true).ToList(), "ID", "Name", deliveryCharge.CityID);
                ViewBag.FranchiseID = new SelectList(db.Franchises.Where(x => x.ID != 1 && x.IsActive == true && x.BusinessDetail.UserLogin.IsLocked == false && x.BusinessDetail.Pincode.City.IsActive == true
             && x.BusinessDetail.Pincode.City.ID == deliveryCharge.CityID).ToList(), "ID", "ContactPerson", deliveryCharge.FranchiseID);////added
                //ViewBag.CityID = new SelectList(db.Cities, "ID", "Name", deliveryCharge.CityID);
                //ViewBag.FranchiseID = new SelectList(db.Franchises, "ID", "ServiceNumber", deliveryCharge.FranchiseID);
                return View(deliveryCharge);
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
                ViewBag.CityID = new SelectList(db.Cities.OrderBy(x => x.Name).Where(c => c.IsActive == true).ToList(), "ID", "Name");

                ///////////added
                List<SelectListItem> lData = new List<SelectListItem>();
                lData.Add(new SelectListItem { Text = string.Empty, Value = "0" });
                ViewBag.FranchiseID = new SelectList(lData, "Value", "Text");
                ///////////
                return View();
            }
        }

        // POST: DeliveryCharges/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [SessionExpire]
        [CustomAuthorize(Roles = "DeliveryCharges/CanRead")]
        public ActionResult Edit(DeliveryCharge deliveryCharge)//Added OrderAmount by Sonali_19-01-2019
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

                var cID = db.DeliveryCharges.Where(x => x.Charges == deliveryCharge.Charges).Select(x => x.CityID).FirstOrDefault();
                var fID = db.DeliveryCharges.Where(x => x.Charges == deliveryCharge.Charges).Select(x => x.FranchiseID).FirstOrDefault();////added
                //if (db1.DeliveryCharges.Where(x => x.Charges == deliveryCharge.Charges && x.ID != deliveryCharge.ID).Count() > 0 && cID == deliveryCharge.CityID && fID == deliveryCharge.FranchiseID)////added && fID == deliveryschedule.FranchiseID
                //{
                //    ViewBag.lblError = "Delivery charge is already Exist..!!";
                //    ViewBag.FranchiseID = new SelectList(db.Franchises.Where(x => x.ID != 1 && x.IsActive == true && x.BusinessDetail.UserLogin.IsLocked == false && x.BusinessDetail.Pincode.City.IsActive == true
                //    && x.BusinessDetail.Pincode.City.ID == deliveryCharge.CityID).ToList(), "ID", "ContactPerson", deliveryCharge.FranchiseID);////added
                //    return View(deliveryCharge);
                //}
                DeliveryCharge ldeliverycharge = db1.DeliveryCharges.Find(deliveryCharge.ID);
                deliveryCharge.CreateDate = ldeliverycharge.CreateDate;
                deliveryCharge.CreateBy = ldeliverycharge.CreateBy;
                deliveryCharge.ModifyDate = DateTime.UtcNow.AddHours(5.5);
                deliveryCharge.ModifyBy = CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(Session["ID"]));
                deliveryCharge.NetworkIP = CommonFunctions.GetClientIP();
                deliveryCharge.DeviceType = string.Empty;
                deliveryCharge.DeviceID = string.Empty;
                db1.Dispose();

                if (ModelState.IsValid)
                {
                    db.Entry(deliveryCharge).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                //ViewBag.CityID = new SelectList(db.Cities, "ID", "Name", deliveryCharge.CityID);
                //ViewBag.FranchiseID = new SelectList(db.Franchises, "ID", "ServiceNumber", deliveryCharge.FranchiseID);
                return View(deliveryCharge);
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
                ViewBag.CityID = new SelectList(db.Cities.OrderBy(x => x.Name).Where(c => c.IsActive == true).ToList(), "ID", "Name");

                ///////////added
                List<SelectListItem> lData = new List<SelectListItem>();
                lData.Add(new SelectListItem { Text = string.Empty, Value = "0" });
                ViewBag.FranchiseID = new SelectList(lData, "Value", "Text");
                ///////////
                return View();
            }
        }

        // GET: DeliveryCharges/Delete/5
        [SessionExpire]
        [CustomAuthorize(Roles = "DeliveryCharges/CanRead")]
        public ActionResult Delete(long? id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                DeliveryCharge deliveryCharge = db.DeliveryCharges.Find(id);
                if (deliveryCharge == null)
                {
                    return HttpNotFound();
                }
                return View(deliveryCharge);
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

        // POST: DeliveryCharges/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [SessionExpire]
        [CustomAuthorize(Roles = "DeliveryCharges/CanRead")]
        public ActionResult DeleteConfirmed(long id)
        {
            try
            {
                DeliveryCharge deliveryCharge = db.DeliveryCharges.Find(id);
                db.DeliveryCharges.Remove(deliveryCharge);
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
