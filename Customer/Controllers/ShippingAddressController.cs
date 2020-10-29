
//-----------------------------------------------------------------------
// <copyright file="ShippingAddressController" company="Ezeelo Consumer Services Pvt. Ltd.">
//     Copyright (c) Ezeelo Consumer Services Pvt. Ltd. All rights reserved.
// </copyright>
// <author>Gaurav Dixit</author>
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
using ModelLayer.Models.ViewModel;
using Gandhibagh.Models;
using System.Data.Entity.Validation;

namespace Gandhibagh.Controllers
{
    [SessionExpire]
    public class ShippingAddressController : Controller
    {
        private EzeeloDBContext db = new EzeeloDBContext();

        // GET: /Test/
        //Yashaswi new parameter IsSaved
        public ActionResult Index(string CurrentAddressID, string IsSaved)
        {
            if (Session["UID"] == null)
            {
                return RedirectToAction("PaymentProcess", "CustomerPaymentProcess");
            }

            long lUserLoginID = 0;
            long.TryParse(Convert.ToString(Session["UID"]), out lUserLoginID);

            var customershippingaddresses = db.CustomerShippingAddresses.Include(c => c.Area).Include(c => c.PersonalDetail).Include(c => c.PersonalDetail1).Include(c => c.Pincode).Include(c => c.UserLogin)
                                             .Where(x => x.UserLoginID == lUserLoginID);

            if (customershippingaddresses != null)
            {
                var list = customershippingaddresses.ToList().OrderByDescending(x => x.CreateDate).ToList().FirstOrDefault();

                if (list != null)
                {
                    if (!string.IsNullOrEmpty(CurrentAddressID))
                    {
                        ViewBag.CurrentAddressID = CurrentAddressID;
                    }
                    else
                    {
                        ViewBag.CurrentAddressID = list.ID;
                    }
                }
            }
            //Yashaswi
            if (IsSaved != null && IsSaved != "")
            {
                ViewBag.IsSaved = "1";
            }
            return View("Index", customershippingaddresses.ToList());
        }

        // GET: /Test/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CustomerShippingAddress customershippingaddress = db.CustomerShippingAddresses.Find(id);
            if (customershippingaddress == null)
            {
                return HttpNotFound();
            }
            return View(customershippingaddress);
        }

        // GET: /Test/Create
        public ActionResult Create()
        {
            if (Session["UID"] == null)
            {
                return RedirectToAction("PaymentProcess", "CustomerPaymentProcess");
            }

            ViewBag.State = new SelectList(db.States, "ID", "Name");

            List<SelectListItem> lData = new List<SelectListItem>();
            lData.Add(new SelectListItem { Text = string.Empty, Value = "0" });

            ViewBag.District = new SelectList(lData, "Value", "Text");

            ViewBag.CityID = new SelectList(lData, "Value", "Text");

            ViewBag.PincodeID = new SelectList(lData, "Value", "Text");

            ViewBag.AreaID = new SelectList(lData, "Value", "Text");

            ViewBag.UserLoginID = new SelectList(db.UserLogins, "ID", "Mobile");

            return View();
        }

        // POST: /Test/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "PrimaryMobile,SecondaryMobile,ShippingAddress, PincodeID,AreaID")] CustomerShippingAddress customershippingaddress, string SelectedAddress, string Pincode)
        {
            try
            {

            if (Session["UID"] == null)
            {
                return RedirectToAction("PaymentProcess", "CustomerPaymentProcess");
            }

            string lPincodeStr = Pincode.Trim();
            if (lPincodeStr == "")
            {
               ViewBag.Message= "Please Enter Pincode";
                return View(customershippingaddress);
            }
            //var fp = lnq.attaches.Where(a => a.sysid == sysid)
            //         .Select(a => a.name)
            //         .First();
            ModelLayer.Models.Pincode lPincode = db.Pincodes.Where(x => x.Name == lPincodeStr).FirstOrDefault();
            //ModelLayer.Models.Pincode lPincode = db.Pincodes.SingleOrDefault(x => x.Name == lPincodeStr);
            if(lPincode == null)
            {
                ViewBag.Message="Invalid Pincode";
                return View(customershippingaddress);
            }
            customershippingaddress.PincodeID = lPincode.ID;


            long userLoginID = 0;
            long.TryParse(Convert.ToString(Session["UID"]), out userLoginID);

            customershippingaddress.CreateDate = DateTime.UtcNow;
            customershippingaddress.CreateBy = 1;
            customershippingaddress.ModifyDate = null;
            customershippingaddress.ModifyBy = null;
            customershippingaddress.NetworkIP = string.Empty;
            customershippingaddress.DeviceType = string.Empty;
            customershippingaddress.DeviceID = string.Empty;
            customershippingaddress.UserLoginID = userLoginID;
            customershippingaddress.IsActive = true;

            TryUpdateModel(customershippingaddress);

            //if (ModelState.IsValid)
            {
                db.CustomerShippingAddresses.Add(customershippingaddress);
                db.SaveChanges();
                //return RedirectToAction("Index");
                //Yashaswi 
                return RedirectToAction("Index", new { IsSaved = "1" });
            }

            List<SelectListItem> lData = new List<SelectListItem>();
            lData.Add(new SelectListItem { Text = string.Empty, Value = "0" });

            ViewBag.District = new SelectList(lData, "Value", "Text");

            ViewBag.CityID = new SelectList(lData, "Value", "Text");

            ViewBag.PincodeID = new SelectList(lData, "Value", "Text");

            ViewBag.AreaID = new SelectList(lData, "Value", "Text");
            ViewBag.State = new SelectList(db.States, "ID", "Name");
            ViewBag.UserLoginID = new SelectList(db.UserLogins, "ID", "Mobile", customershippingaddress.UserLoginID);

            }
            catch (DbEntityValidationException ex)
            {
                // Retrieve the error messages as a list of strings.
                var errorMessages = ex.EntityValidationErrors
                        .SelectMany(x => x.ValidationErrors)
                        .Select(x => new { x.ErrorMessage });

                var fullErrorMessage = string.Join("; ", errorMessages);
                var exceptionMessage = string.Concat(ex.Message, " The validation errors are: ", fullErrorMessage);
                ViewBag.Message = fullErrorMessage.Replace("ErrorMessage = ", " ");
                return View(customershippingaddress);
            }

            catch (Exception)
            {

                throw;
            }
            return View(customershippingaddress);
        }

        // GET: /Test/Edit/5
        public ActionResult Edit(int? id)
        {
            if (Session["UID"] == null)
            {
                return RedirectToAction("PaymentProcess", "CustomerPaymentProcess");
            }
            CustomerShippingAddress customershippingaddress = db.CustomerShippingAddresses.Find(id);
            if (customershippingaddress == null)
            {
                return HttpNotFound();
            }

            ViewBag.Pincode = customershippingaddress.Pincode.Name;

            // ViewBag.State = new SelectList(db.States, "ID", "Name");

            //List<SelectListItem> lData = new List<SelectListItem>();
            //lData.Add(new SelectListItem { Text = string.Empty, Value = "0" });
            //ViewBag.District = new SelectList(lData, "Value", "Text");

            //ViewBag.CityID = new SelectList(lData, "Value", "Text");

            //ViewBag.PincodeID = new SelectList(lData, "Value", "Text");

            // ViewBag.AreaID = new SelectList(lData, "Value", "Text");

            //var shippingAddress = (from p in db.Pincodes
            //                       join a in db.Areas on p.ID equals a.PincodeID
            //                       join c in db.Cities on p.CityID equals c.ID
            //                       join d in db.Districts on c.DistrictID equals d.ID
            //                       join s in db.States on d.StateID equals s.ID
            //                       where p.ID == customershippingaddress.PincodeID
            //                       select new
            //                       {
            //                           StateID = s.ID,
            //                           DistrictID = d.ID,
            //                           CityID = c.ID,
            //                           AreaID = a.ID,
            //                           PincodeID = p.ID
            //                       }).FirstOrDefault();

            //if (shippingAddress != null)
            //{
            //    ViewBag.SelectedState = shippingAddress.StateID;
            //    ViewBag.SelectedDistrict = shippingAddress.DistrictID;
            //    ViewBag.SelectedCity = shippingAddress.CityID;
            //    ViewBag.SelectedPincode = shippingAddress.PincodeID;
            //    ViewBag.SelectedArea = shippingAddress.AreaID;
            //}
            //else
            //{
            //    ViewBag.SelectedState = "0";
            //    ViewBag.SelectedDistrict = "0";
            //    ViewBag.SelectedCity = "0";
            //    ViewBag.SelectedPincode = "0";
            //    ViewBag.SelectedArea = "0";
            //}

            return View(customershippingaddress);
        }

        // POST: /Test/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,PrimaryMobile,SecondaryMobile,ShippingAddress,PincodeID,AreaID,IsActive")] CustomerShippingAddress customershippingaddress, string Pincode)
        {
            if (Session["UID"] == null)
            {
                return RedirectToAction("PaymentProcess", "CustomerPaymentProcess");
            }

            string lPincodeStr = Pincode.Trim();
            if (lPincodeStr == "")
            {
                ModelState.AddModelError("Error", "Invalid Pincode");
                ViewBag.Msg = "Please! Enter Pincode";
            }
            ModelLayer.Models.Pincode lPincode = db.Pincodes.Where(x => x.Name == lPincodeStr).FirstOrDefault();
            if (lPincode == null)
            {
                ModelState.AddModelError("Error", "Invalid Pincode");
                ViewBag.Msg = "Pincode dose not exist. Please try another pincode";

            }
            else
            {
                customershippingaddress.PincodeID = lPincode.ID;

                long userLoginID = 0;
                long.TryParse(Convert.ToString(Session["UID"]), out userLoginID);

                customershippingaddress.CreateDate = DateTime.UtcNow;
                customershippingaddress.CreateBy = 1;
                customershippingaddress.ModifyDate = null;
                customershippingaddress.ModifyBy = null;
                customershippingaddress.NetworkIP = string.Empty;
                customershippingaddress.DeviceType = string.Empty;
                customershippingaddress.DeviceID = string.Empty;
                customershippingaddress.UserLoginID = userLoginID;
            }
            if (ModelState.IsValid)
            {
                db.Entry(customershippingaddress).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            List<SelectListItem> lData = new List<SelectListItem>();
            lData.Add(new SelectListItem { Text = string.Empty, Value = "0" });

            ViewBag.District = new SelectList(lData, "Value", "Text");

            ViewBag.CityID = new SelectList(lData, "Value", "Text");

            ViewBag.PincodeID = new SelectList(lData, "Value", "Text");

            ViewBag.AreaID = new SelectList(lData, "Value", "Text");
            ViewBag.State = new SelectList(db.States, "ID", "Name");
            ViewBag.UserLoginID = new SelectList(db.UserLogins, "ID", "Mobile", customershippingaddress.UserLoginID);

            return View(customershippingaddress);
        }

        // GET: /Test/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CustomerShippingAddress customershippingaddress = db.CustomerShippingAddresses.Find(id);
            if (customershippingaddress == null)
            {
                return HttpNotFound();
            }
            //CustomerShippingAddress customershippingaddress = db.CustomerShippingAddresses.Find(id);
            db.CustomerShippingAddresses.Remove(customershippingaddress);
            db.SaveChanges();
            return RedirectToAction("Index");
            //return View(customershippingaddress);
        }

        // POST: /Test/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            CustomerShippingAddress customershippingaddress = db.CustomerShippingAddresses.Find(id);
            db.CustomerShippingAddresses.Remove(customershippingaddress);
            db.SaveChanges();
            return RedirectToAction("Index");
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

        public JsonResult getArea(int pincodeID)
        {
            List<tempData> objODP = new List<tempData>();

            objODP = db.Areas
                .Where(x => x.PincodeID == pincodeID)
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

        //Yashaswi Add new parameter AreaId
        //Show only selected franchise pincode area
        public ActionResult GetAddress(string Pincode, int? AreaId)
        {
            /*This Action Responces to AJAX Call
             * After entering Pincode returens City, District and State Information
             * */
            ModelLayer.Models.Pincode pincode = db.Pincodes.FirstOrDefault(p => p.Name == Pincode);
            if (!(db.Pincodes.Any(p => p.Name == Pincode)))
            {

                //var errorMsg = "Pincode Dose Not Exist, Please Contact Admin!";
                //return View(new { success = false, Error = errorMsg });

                //***********Added by harshada on 28/12/2016 for error page display on wrong pincode******//
                ViewBag.errorMsg = "Pincode Dose Not Exist, Please Contact Admin!";
                return View("Create");
                //***********End Added by harshada on 28/12/2016 for error page display on wrong pincode******//
            }
            ViewBag.errorMsg = null;
            long CityId = db.Pincodes.FirstOrDefault(p => p.Name == Pincode).CityID;
            ViewBag.City = db.Cities.FirstOrDefault(c => c.ID == CityId).Name.ToString();

            long DistrictId = db.Cities.FirstOrDefault(c => c.ID == CityId).DistrictID;
            ViewBag.District = db.Districts.FirstOrDefault(d => d.ID == DistrictId).Name.ToString();

            long StateId = db.Districts.FirstOrDefault(d => d.ID == DistrictId).StateID;
            ViewBag.State = db.States.FirstOrDefault(d => d.ID == StateId).Name.ToString();

            List<SelectListItem> lData = new List<SelectListItem>();
            //ViewBag.AreaID = new SelectList(db.Areas.Where(x => x.PincodeID == pincode.ID), "ID", "Name");
            //Start Yashaswi
            var lArea = db.Areas.Where(x => x.PincodeID == pincode.ID).Select(x => new { ID = x.ID, Name = x.Name }).ToList();
            if (AreaId != null && AreaId != 0)
            {
                lArea = db.Areas.Where(a => (db.Areas.Where(aa => aa.ID == AreaId).Select(aa => aa.PincodeID)).Contains(a.PincodeID))
                     .Join(db.FranchiseLocations.Where(f => (db.FranchiseLocations.Where(fl => fl.AreaID == AreaId).Select(fl => fl.FranchiseID).Contains(f.FranchiseID)))
                     , a => a.ID, f => f.AreaID, (a, f) => new { ID = a.ID, Name = a.Name }
                     ).ToList();
            }

            ViewBag.AreaID = new SelectList(lArea, "ID", "Name", AreaId);
            //End
            return PartialView("_Address");
        }

    }
}
