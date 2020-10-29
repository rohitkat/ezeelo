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
using PagedList;
using PagedList.Mvc;
using System.Text;
using BusinessLogicLayer;
using Administrator.Models;

namespace Administrator.Controllers
{

    public class PincodeController : Controller
    {
        private EzeeloDBContext db = new EzeeloDBContext();

        // GET: /Pincode/
        [SessionExpire]
        [CustomAuthorize(Roles = "Pincode/CanRead")]
        public ActionResult Index(int? page, int? district, int? state, Int64? city)
        {
            try
            {

                if (state > 0)
                {
                    ViewBag.district = new SelectList(db.Districts.Where(x => x.StateID == state).ToList(), "ID", "Name");
                    ViewBag.State = new SelectList(db.States.ToList(), "ID", "Name", state);
                }
                else
                {
                    List<SelectListItem> lData = new List<SelectListItem>();
                    lData.Add(new SelectListItem { Text = string.Empty, Value = "0" });
                    ViewBag.district = new SelectList(lData, "Value", "Text");
                    ViewBag.State = new SelectList(db.States.ToList(), "ID", "Name");

                }

                List<Pincode> pincodelist = new List<Pincode>();

                if (district > 0 && city > 0)
                {
                    ViewBag.city = new SelectList(db.Cities.Where(x => x.DistrictID == district).ToList(), "ID", "Name", city);
                    pincodelist = db.Pincodes.Where(x => x.CityID == city).ToList();

                }
                else
                {
                    List<SelectListItem> lData = new List<SelectListItem>();
                    lData.Add(new SelectListItem { Text = string.Empty, Value = "0" });
                    ViewBag.city = new SelectList(lData, "Value", "Text");

                    pincodelist = db.Pincodes.Where(x => x.City.District.State.ID == 1).Include(p => p.City).ToList();
                }



                return View(pincodelist.ToList());
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Message", "Sorry! Problem in Generate Index view!!");
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[PincodeController][GET:Index]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
                return View();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Message", "Sorry! Problem in Generate Index view!!");
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[PincodeController][GET:Index]",
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
                return View();
            }

        }

        // GET: /Pincode/Details/5
        [SessionExpire]
        [CustomAuthorize(Roles = "Pincode/CanRead")]
        public ActionResult Details(int? id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Pincode pincode = db.Pincodes.Find(id);
                if (pincode == null)
                {
                    return HttpNotFound();
                }
                return View(pincode);
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Message", "Sorry! Problem in Record Detail!!");
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[PincodeController][GET:Details]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
                return View();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Message", "Sorry! Problem in Record Detail!!");
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[PincodeController][GET:Details]",
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
                return View();
            }

        }

        // GET: /Pincode/Create
        [SessionExpire]
        [CustomAuthorize(Roles = "Pincode/CanWrite")]
        public ActionResult Create()
        {
            try
            {
                ViewBag.State = new SelectList(db.States, "ID", "Name");

                List<SelectListItem> lData = new List<SelectListItem>();
                lData.Add(new SelectListItem { Text = string.Empty, Value = "0" });
                ViewBag.District = new SelectList(lData, "Value", "Text");

                ViewBag.CityID = new SelectList(lData, "Value", "Text");

                //ViewBag.CityID = new SelectList(db.Cities, "ID", "Name");
                return View();
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Message", "Sorry! Problem in Generate Create View!!");
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[PincodeController][GET:Create]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
                return View();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Message", "Sorry! Problem in Generate Create View!!");
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[PincodeController][GET:Create]",
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
                return View();
            }

        }

        // POST: /Pincode/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [SessionExpire]
        [CustomAuthorize(Roles = "Pincode/CanWrite")]
        public ActionResult Create([Bind(Include = "ID,Name,CityID,IsActive,IsDeliverablePincode")] Pincode pincode)
        {
            try
            {

                ViewBag.State = new SelectList(db.States, "ID", "Name");

                List<SelectListItem> lData = new List<SelectListItem>();
                lData.Add(new SelectListItem { Text = string.Empty, Value = "0" });
                ViewBag.District = new SelectList(lData, "Value", "Text");

                ViewBag.CityID = new SelectList(lData, "Value", "Text");

                if (db.Pincodes.Where(x => x.Name == pincode.Name).Count() > 0)
                {
                    ViewBag.Messaage = "unable insert duplicate pincode.";
                    return View(pincode);
                }

                pincode.CreateDate = DateTime.UtcNow.AddHours(5.30);
                pincode.CreateBy = CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(Session["ID"]));
                pincode.ModifyDate = null;
                pincode.ModifyBy = null;
                pincode.NetworkIP = string.Empty;
                pincode.DeviceType = string.Empty;
                pincode.DeviceID = string.Empty;

                TryUpdateModel(pincode);

                if (ModelState.IsValid)
                {
                    db.Pincodes.Add(pincode);
                    db.SaveChanges();
                    //return RedirectToAction("Index");
                    ViewBag.Messaage = "Pincode Inserted successfully";
                }

                //Start Yashaswi 12-2-19 To add newly created pincode in delivery Pincode tabel
                if (pincode.IsDeliverablePincode)
                {
                    //Insert Row for current pincode
                    DeliveryPincode DP_ = new DeliveryPincode();
                    DP_.DeliveryPartnerID = 1;
                    DP_.PincodeID = pincode.ID;
                    DP_.IsActive = true;
                    DP_.CreateDate = DateTime.Now;
                    DP_.CreateBy = CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(Session["ID"]));
                    DP_.NetworkIP = CommonFunctions.GetClientIP();
                    pincode.DeviceType = string.Empty;
                    pincode.DeviceID = string.Empty;
                    db.DeliveryPincodes.Add(DP_);
                    db.SaveChanges();
                }
                //End Yashaswi 12-2-19 To add newly created pincode in delivery Pincode tabel

                // ViewBag.CityID = new SelectList(db.Cities, "ID", "Name", pincode.CityID);
                return View(pincode);
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Message", "Sorry! Problem in Generate Create View!!");
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[PincodeController][POST:Create]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);

                ViewBag.Messaage = "Unable to Insert Pincode Detail ";
                return View(pincode);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Message", "Sorry! Problem in Generate Create View!!");
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[PincodeController][POST:Create]",
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);

                ViewBag.Messaage = "Unable to Insert Pincode Detail ";
                return View(pincode);
            }

        }

        // GET: /Pincode/Edit/5
        [CustomAuthorize(Roles = "Pincode/CanWrite")]
        public ActionResult Edit(int? id)
        {
            try
            {

                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Pincode pincode = db.Pincodes.Find(id);
                if (pincode == null)
                {
                    return HttpNotFound();
                }


                //Start Yashaswi 12-2-19 To add newly created pincode in delivery Pincode tabel

                //ViewBag.State = new SelectList(db.States, "ID", "Name");
                //List<SelectListItem> lData = new List<SelectListItem>();
                //lData.Add(new SelectListItem { Text = string.Empty, Value = "0" });
                //ViewBag.District = new SelectList(lData, "Value", "Text");
                //ViewBag.CityID = new SelectList(lData, "Value", "Text");

                long PincodeId = pincode.ID;
                long CityId = pincode.CityID;
                long DistrictId = db.Cities.FirstOrDefault(p => p.ID == CityId).DistrictID;
                long StateId = db.Districts.FirstOrDefault(p => p.ID == DistrictId).StateID;
                ViewBag.State = new SelectList(db.States, "ID", "Name", StateId);
                ViewBag.District = new SelectList(db.Districts.Where(p => p.StateID == StateId).ToList(), "ID", "Name", DistrictId);
                ViewBag.CityID = new SelectList(db.Cities.Where(p => p.DistrictID == DistrictId).ToList(), "ID", "Name", CityId);

                DeliveryPincode DP_ = db.DeliveryPincodes.FirstOrDefault(p => p.PincodeID == id && p.IsActive == true);
                if (DP_ != null)
                {
                    pincode.IsDeliverablePincode = true;
                }
                else
                {
                    pincode.IsDeliverablePincode = false;
                }
                //End Yashaswi 12-2-19 To add newly created pincode in delivery Pincode tabel

                //ViewBag.CityID = new SelectList(db.Cities, "ID", "Name", pincode.CityID);
                return View(pincode);
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Message", "Sorry! Problem in Record Update!!");
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[PincodeController][GET:Edit]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
                return View();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Message", "Sorry! Problem in Record Update!!");
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[PincodeController][GET:Edit]",
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
                return View();
            }


        }


        // POST: /Pincode/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [SessionExpire]
        [CustomAuthorize(Roles = "Pincode/CanWrite")]
        public ActionResult Edit([Bind(Include = "ID,Name,CityID,IsActive,IsDeliverablePincode")] Pincode pincode)
        {
            try
            {
                //ViewBag.CityID = new SelectList(db.Cities, "ID", "Name", pincode.CityID);

                ViewBag.State = new SelectList(db.States, "ID", "Name");

                List<SelectListItem> lData = new List<SelectListItem>();
                lData.Add(new SelectListItem { Text = string.Empty, Value = "0" });
                ViewBag.District = new SelectList(lData, "Value", "Text");

                ViewBag.CityID = new SelectList(lData, "Value", "Text");

                if (db.Pincodes.Where(x => x.Name == pincode.Name && x.ID != pincode.ID).Count() > 0)
                {
                    ViewBag.Messaage = "unable insert duplicate pincode.";
                    return View(pincode);
                }


                Pincode lPincode = db.Pincodes.Find(pincode.ID);

                pincode.CreateDate = lPincode.CreateDate;
                pincode.CreateBy = lPincode.CreateBy;
                pincode.ModifyDate = DateTime.UtcNow.AddHours(5.30);
                pincode.ModifyBy = CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(Session["ID"]));
                pincode.NetworkIP = string.Empty;
                pincode.DeviceType = string.Empty;
                pincode.DeviceID = string.Empty;

                TryUpdateModel(pincode);

                if (ModelState.IsValid)
                {
                    //db.Entry(pincode).State = EntityState.Modified;
                    db.Entry(lPincode).CurrentValues.SetValues(pincode);
                    db.SaveChanges();
                    //return RedirectToAction("Index");
                    ViewBag.Messaage = "Pincode Updated successfully";
                }
                //Start Yashaswi 12-2-19 To add newly created pincode in delivery Pincode tabel

                DeliveryPincode DP = db.DeliveryPincodes.FirstOrDefault(p => p.PincodeID == pincode.ID);
                if (DP == null)
                {
                    //Insert Row for current pincode
                    DeliveryPincode DP_ = new DeliveryPincode();
                    DP_.DeliveryPartnerID = 1;
                    DP_.PincodeID = pincode.ID;
                    DP_.IsActive = pincode.IsDeliverablePincode;
                    DP_.CreateDate = DateTime.Now;
                    DP_.CreateBy = CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(Session["ID"]));
                    DP_.NetworkIP = CommonFunctions.GetClientIP();
                    pincode.DeviceType = string.Empty;
                    pincode.DeviceID = string.Empty;
                    db.DeliveryPincodes.Add(DP_);
                    db.SaveChanges();
                }
                else
                {
                    DP.DeliveryPartnerID = 1;
                    DP.PincodeID = pincode.ID;
                    DP.IsActive = pincode.IsDeliverablePincode;
                    DP.ModifyDate = DateTime.Now;
                    DP.ModifyBy = CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(Session["ID"]));
                    DP.NetworkIP = CommonFunctions.GetClientIP();
                    pincode.DeviceType = string.Empty;
                    pincode.DeviceID = string.Empty;
                    db.SaveChanges();
                }

                //End Yashaswi 12-2-19 To add newly created pincode in delivery Pincode tabel

                return View(pincode);
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Message", "Sorry! Problem in Record Update!!");
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[PincodeController][POST:Edit]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);

                ViewBag.Messaage = "Unable to Update Pincode Detail ";
                return View(pincode);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Message", "Sorry! Problem in Record Update!!");
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[PincodeController][POST:Edit]",
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);

                ViewBag.Messaage = "Unable to Update Pincode Detail ";
                return View(pincode);
            }

        }

        // GET: /Pincode/Delete/5
        [SessionExpire]
        [CustomAuthorize(Roles = "Pincode/CanDelete")]
        public ActionResult Delete(int? id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Pincode pincode = db.Pincodes.Find(id);
                if (pincode == null)
                {
                    return HttpNotFound();
                }
                return View(pincode);
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Message", "Sorry! Problem in Record Deletion!!");
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[PincodeController][GET:Delete]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
                return View();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Message", "Sorry! Problem in Record Deletion!!");
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[PincodeController][GET:Delete]",
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
                return View();
            }

        }

        // POST: /Pincode/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [SessionExpire]
        [CustomAuthorize(Roles = "Pincode/CanDelete")]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                Pincode pincode = db.Pincodes.Find(id);
                db.Pincodes.Remove(pincode);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Message", "Sorry! Problem in Record Deletion!!");
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[PincodeController][POST:DeleteConfirmed]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);

                ViewBag.Messaage = "Unable to Update Pincode Detail ";
                return View(db.Pincodes.Where(x => x.ID == id).FirstOrDefault());
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Message", "Sorry! Problem in Record Deletion!!");
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[PincodeController][POST:DeleteConfirmed]",
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);

                ViewBag.Messaage = "Unable to Update Pincode Detail ";
                return View(db.Pincodes.Where(x => x.ID == id).FirstOrDefault());
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

        public JsonResult getDistrict(int StateID)
        {
            List<tempData> objODP = new List<tempData>();

            objODP = db.Districts
                .Where(x => x.StateID == StateID)
                .Select(x => new tempData { text = x.Name, value = x.ID }
                ).OrderBy(x => x.text)
                .ToList();

            return Json(objODP, JsonRequestBehavior.AllowGet);
        }

        public JsonResult getCity(int DistrictID)
        {
            List<tempData> objODP = new List<tempData>();

            objODP = db.Cities
                .Where(x => x.DistrictID == DistrictID)
                .Select(x => new tempData { text = x.Name, value = x.ID }
                ).OrderBy(x => x.text)
                .ToList();

            return Json(objODP, JsonRequestBehavior.AllowGet);
        }

        public JsonResult getPincode(int CityID)
        {
            List<tempData> objODP = new List<tempData>();

            objODP = db.Pincodes
                .Where(x => x.CityID == CityID)
                .Select(x => new tempData { text = x.Name, value = x.ID }
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
