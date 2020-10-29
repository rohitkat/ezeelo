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
using DeliveryPartner.Models.ViewModel;
using System.Collections;
using DeliveryPartner.Models;

namespace DeliveryPartner.Controllers
{
    public class TimeHourSlabing
    {
        public string HourText { get; set; }
        public string HourValue { get; set; }
        //public string MinuteText { get; set; }
        //public string MinuteValue { get; set; }

    }
    public class DeliveryPartnerController : Controller
    {
        private EzeeloDBContext db = new EzeeloDBContext();
        private DeliveryPartnerSessionViewModel deliveryPartnerSessionViewModel = new DeliveryPartnerSessionViewModel();
        private int pageSize = 10;

        public void SessionDetails()
        {
            deliveryPartnerSessionViewModel.UserLoginID = Convert.ToInt64(Session["ID"]);
            deliveryPartnerSessionViewModel.Username = Session["UserName"].ToString();
            Common.Common.GetAllLoginDetailFromSession(ref deliveryPartnerSessionViewModel);
        }
        //// GET: /DeliveryPartner/
        //[SessionExpire]
        //public ActionResult Index()
        //{
        //    SessionDetails();
        //    var deliverypartners = db.DeliveryPartners.Include(d => d.BusinessDetail).Include(d => d.PersonalDetail).Include(d => d.PersonalDetail1).Include(d => d.Pincode).Include(d => d.VehicleType);
        //    return View(deliverypartners.ToList());
        //}

        // GET: /DeliveryPartner/Details/5
        [SessionExpire]
        public ActionResult Details(int? id)
        {
            SessionDetails();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ModelLayer.Models.DeliveryPartner deliverypartner = db.DeliveryPartners.Find(id);
            if (deliverypartner == null)
            {
                return HttpNotFound();
            }
            if (deliverypartner.ID != deliveryPartnerSessionViewModel.DeliveryPartnerID)
            {
                return View("AccessDenied");
            }
            return View(deliverypartner);
        }

        // GET: /DeliveryPartner/Create
        [SessionExpire]
        public ActionResult Create()
        {
            SessionDetails();
            var ServiceLevel = from DeliveryPartner.Common.Constant.DELIVERY_PARTNER_SERVICE_LEVEL d in Enum.GetValues(typeof(DeliveryPartner.Common.Constant.DELIVERY_PARTNER_SERVICE_LEVEL))
                               select new { ID = (int)d, Name = d.ToString() };
            ViewBag.ServiceLevel = new SelectList(ServiceLevel, "ID", "Name");
            ViewBag.BusinessDetailID = new SelectList(db.BusinessDetails, "ID", "Name");
            ViewBag.CreateBy = new SelectList(db.PersonalDetails, "ID", "FirstName");
            ViewBag.ModifyBy = new SelectList(db.PersonalDetails, "ID", "FirstName");
            ViewBag.PincodeID = new SelectList(db.Pincodes, "ID", "Name");
            ViewBag.VehicleTypeID = new SelectList(db.VehicleTypes, "ID", "Name");

            var days = from DeliveryPartner.Common.Constant.DAYS_TYPE d in Enum.GetValues(typeof(DeliveryPartner.Common.Constant.DAYS_TYPE)) select new { Name = d.ToString() };
            ViewBag.WeeklyOff = new SelectList(days, "Name", "Name");
            return View();
        }

        // POST: /DeliveryPartner/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [SessionExpire]
        public ActionResult Create(string Pincode, [Bind(Include = "ID,BusinessDetailID,GodownAddress,PincodeID,ServiceNumber,ServiceLevel,ContactPerson,Mobile,Email,Landline,FAX,VehicleTypeID,OpeningTime,ClosingTime,WeeklyOff,IsLive,IsActive,CreateDate,CreateBy,ModifyDate,ModifyBy,NetworkIP,DeviceType,DeviceID")] ModelLayer.Models.DeliveryPartner deliverypartner)
        {
            SessionDetails();
            try
            {
                BusinessDetail lBusinessDetail = db.BusinessDetails.SingleOrDefault(x => x.UserLoginID == deliveryPartnerSessionViewModel.UserLoginID);
                deliverypartner.BusinessDetailID = lBusinessDetail.ID;
                deliverypartner.CreateDate = DateTime.Now;
                deliverypartner.CreateBy = deliveryPartnerSessionViewModel.PersonalDetailID;

                Pincode lPincode = db.Pincodes.Single(x => x.Name == Pincode);
                if (lPincode == null)
                {
                    return View("Error");
                }
                deliverypartner.PincodeID = lPincode.ID;

                if (ModelState.IsValid)
                {
                    db.DeliveryPartners.Add(deliverypartner);
                    db.SaveChanges();
                    return View("Details");
                }
                var ServiceLevel = from DeliveryPartner.Common.Constant.DELIVERY_PARTNER_SERVICE_LEVEL d in Enum.GetValues(typeof(DeliveryPartner.Common.Constant.DELIVERY_PARTNER_SERVICE_LEVEL))
                                   select new { ID = (int)d, Name = d.ToString() };
                ViewBag.ServiceLevel = new SelectList(ServiceLevel, "ID", "Name", deliverypartner.ServiceLevel);
                ViewBag.BusinessDetailID = new SelectList(db.BusinessDetails, "ID", "Name", deliverypartner.BusinessDetailID);
                ViewBag.CreateBy = new SelectList(db.PersonalDetails, "ID", "FirstName", deliverypartner.CreateBy);
                ViewBag.ModifyBy = new SelectList(db.PersonalDetails, "ID", "FirstName", deliverypartner.ModifyBy);
                ViewBag.PincodeID = new SelectList(db.Pincodes, "ID", "Name", deliverypartner.PincodeID);
                ViewBag.VehicleTypeID = new SelectList(db.VehicleTypes, "ID", "Name", deliverypartner.VehicleTypeID);

                var days = from DeliveryPartner.Common.Constant.DAYS_TYPE d in Enum.GetValues(typeof(DeliveryPartner.Common.Constant.DAYS_TYPE)) select new { Name = d.ToString() };
                ViewBag.WeeklyOff = new SelectList(days, "Name", "Name");

                return View(deliverypartner);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Error", "There's something wrong with the delivery partner values!");

                //Code to write error log
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[DeliveryPartner][POST:Create]",
                    BusinessLogicLayer.ErrorLog.Module.DeliveryPartner, System.Web.HttpContext.Current.Server);

                var ServiceLevel = from DeliveryPartner.Common.Constant.DELIVERY_PARTNER_SERVICE_LEVEL d in Enum.GetValues(typeof(DeliveryPartner.Common.Constant.DELIVERY_PARTNER_SERVICE_LEVEL))
                                   select new { ID = (int)d, Name = d.ToString() };
                ViewBag.ServiceLevel = new SelectList(ServiceLevel, "ID", "Name", deliverypartner.ServiceLevel);
                ViewBag.BusinessDetailID = new SelectList(db.BusinessDetails, "ID", "Name", deliverypartner.BusinessDetailID);
                ViewBag.CreateBy = new SelectList(db.PersonalDetails, "ID", "FirstName", deliverypartner.CreateBy);
                ViewBag.ModifyBy = new SelectList(db.PersonalDetails, "ID", "FirstName", deliverypartner.ModifyBy);
                ViewBag.PincodeID = new SelectList(db.Pincodes, "ID", "Name", deliverypartner.PincodeID);
                ViewBag.VehicleTypeID = new SelectList(db.VehicleTypes, "ID", "Name", deliverypartner.VehicleTypeID);
                
                var days = from DeliveryPartner.Common.Constant.DAYS_TYPE d in Enum.GetValues(typeof(DeliveryPartner.Common.Constant.DAYS_TYPE)) select new { Name = d.ToString() };
                ViewBag.WeeklyOff = new SelectList(days, "Name", "Name");

                return View(deliverypartner);
            }

        }

        // GET: /DeliveryPartner/Edit/5
        [SessionExpire]
        public ActionResult Edit(int? id)
        {
            SessionDetails();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ModelLayer.Models.DeliveryPartner deliverypartner = db.DeliveryPartners.Find(id);
            if (deliverypartner == null)
            {
                return HttpNotFound();
            }

            if (deliverypartner.ID != deliveryPartnerSessionViewModel.DeliveryPartnerID)
            {
                return View("AccessDenied");
            }

            Pincode lPincode = db.Pincodes.Find(deliverypartner.PincodeID);
            if (lPincode == null)
            {
                return View("Error");
            }
            ViewBag.Pincode = lPincode.Name;
        
            var ServiceLevel = from DeliveryPartner.Common.Constant.DELIVERY_PARTNER_SERVICE_LEVEL d in Enum.GetValues(typeof(DeliveryPartner.Common.Constant.DELIVERY_PARTNER_SERVICE_LEVEL))
                               select new { ID=(int)d,Name=d.ToString()};
            ViewBag.ServiceLevel = new SelectList(ServiceLevel.Where(x => x.ID == deliverypartner.ServiceLevel), "ID", "Name",deliverypartner.ServiceLevel);

            ViewBag.BusinessDetailID = new SelectList(db.BusinessDetails, "ID", "Name", deliverypartner.BusinessDetailID);
            ViewBag.CreateBy = new SelectList(db.PersonalDetails, "ID", "FirstName", deliverypartner.CreateBy);
            ViewBag.ModifyBy = new SelectList(db.PersonalDetails, "ID", "FirstName", deliverypartner.ModifyBy);
            ViewBag.PincodeID = new SelectList(db.Pincodes, "ID", "Name", deliverypartner.PincodeID);
            ViewBag.VehicleTypeID = new SelectList(db.VehicleTypes, "ID", "Name", deliverypartner.VehicleTypeID);

            // var Status = from DeliveryPartner.Common.Constant.ORDER_STATUS d in Enum.GetValues(typeof(DeliveryPartner.Common.Constant.ORDER_STATUS))
            //             select new { ID = (int)d, Name = d.ToString() };

            //ViewBag.DeliveryStatus = new SelectList(Status.Where(x => x.ID >= (int)Common.Constant.ORDER_STATUS.PACKED), "ID", "Name");

            var days = from DeliveryPartner.Common.Constant.DAYS_TYPE d in Enum.GetValues(typeof(DeliveryPartner.Common.Constant.DAYS_TYPE)) select new { Name = d.ToString() };
            ViewBag.WeeklyOff = new SelectList(days, "Name", "Name", deliverypartner.WeeklyOff);
           // ViewBag.WeeklyOff = new SelectList(days, "Name", "Name");
            LoadTimeSlab(deliverypartner);
            return View(deliverypartner);
        }

        // POST: /DeliveryPartner/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [SessionExpire]
        public ActionResult Edit(string Pincode, [Bind(Include = "ID,BusinessDetailID,GodownAddress,PinCodeName,ServiceNumber,ServiceLevel,ContactPerson,Mobile,Email,Landline,FAX,VehicleTypeID,OpeningTime,ClosingTime,WeeklyOff,IsLive,IsActive,CreateDate,CreateBy,ModifyDate,ModifyBy")] ModelLayer.Models.DeliveryPartner deliverypartner)
        {
            SessionDetails();
            try
            {
                EzeeloDBContext db1 = new EzeeloDBContext();
                ModelLayer.Models.DeliveryPartner lDeliveryPartner = db1.DeliveryPartners.SingleOrDefault(x => x.BusinessDetail.UserLoginID == deliveryPartnerSessionViewModel.UserLoginID);
                BusinessDetail lBusinessDetail = db.BusinessDetails.SingleOrDefault(x => x.UserLoginID == deliveryPartnerSessionViewModel.UserLoginID && x.BusinessTypeID == (int)Common.Constant.BUSINESS_TYPE.DELIVERY_PARTNER);
                deliverypartner.CreateBy = lDeliveryPartner.CreateBy;
                deliverypartner.CreateDate = lDeliveryPartner.CreateDate;
                deliverypartner.ModifyBy = deliveryPartnerSessionViewModel.PersonalDetailID;
                deliverypartner.ModifyDate = DateTime.Now;
                deliverypartner.IsLive = lDeliveryPartner.IsLive;
                deliverypartner.IsActive = lDeliveryPartner.IsActive;
                deliverypartner.ID = lDeliveryPartner.ID;
                deliverypartner.BusinessDetailID = lDeliveryPartner.BusinessDetailID;
                db1.Dispose();

                Pincode lPincode = db.Pincodes.Single(x => x.Name == Pincode);
                if (lPincode == null)
                {
                    return View("Error");
                }
                deliverypartner.PincodeID = lPincode.ID;

                if (ModelState.IsValid)
                {
                    db.Entry(deliverypartner).State = EntityState.Modified;
                    db.SaveChanges();
                    TempData["Msg"] = "Data Saved Successfully";
                    return View("Details", deliverypartner);
                }
                var ServiceLevel = from DeliveryPartner.Common.Constant.DELIVERY_PARTNER_SERVICE_LEVEL d in Enum.GetValues(typeof(DeliveryPartner.Common.Constant.DELIVERY_PARTNER_SERVICE_LEVEL))
                                   select new { ID = (int)d, Name = d.ToString() };
                ViewBag.ServiceLevel = new SelectList(ServiceLevel.Where(x => x.ID == deliverypartner.ServiceLevel), "ID", "Name", deliverypartner.ServiceLevel);


                ViewBag.BusinessDetailID = new SelectList(db.BusinessDetails, "ID", "Name", deliverypartner.BusinessDetailID);
                ViewBag.CreateBy = new SelectList(db.PersonalDetails, "ID", "FirstName", deliverypartner.CreateBy);
                ViewBag.ModifyBy = new SelectList(db.PersonalDetails, "ID", "FirstName", deliverypartner.ModifyBy);
                ViewBag.PincodeID = new SelectList(db.Pincodes, "ID", "Name", deliverypartner.PincodeID);
                ViewBag.VehicleTypeID = new SelectList(db.VehicleTypes, "ID", "Name", deliverypartner.VehicleTypeID);
                var days = from DeliveryPartner.Common.Constant.DAYS_TYPE d in Enum.GetValues(typeof(DeliveryPartner.Common.Constant.DAYS_TYPE)) select new { Name = d.ToString() };
                ViewBag.WeeklyOff = new SelectList(days, "Name", "Name", deliverypartner.WeeklyOff);
                return View(deliverypartner);
            }
            catch (Exception ex)
            {

                ModelState.AddModelError("Error", "There's something wrong with the delivery partner values!");

                //Code to write error log
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[DeliveryPartner][POST:Edit]",
                    BusinessLogicLayer.ErrorLog.Module.DeliveryPartner, System.Web.HttpContext.Current.Server);


                var ServiceLevel = from DeliveryPartner.Common.Constant.DELIVERY_PARTNER_SERVICE_LEVEL d in Enum.GetValues(typeof(DeliveryPartner.Common.Constant.DELIVERY_PARTNER_SERVICE_LEVEL))
                                   select new { ID = (int)d, Name = d.ToString() };
                ViewBag.ServiceLevel = new SelectList(ServiceLevel.Where(x => x.ID == deliverypartner.ServiceLevel), "ID", "Name", deliverypartner.ServiceLevel);


                ViewBag.BusinessDetailID = new SelectList(db.BusinessDetails, "ID", "Name", deliverypartner.BusinessDetailID);
                ViewBag.CreateBy = new SelectList(db.PersonalDetails, "ID", "FirstName", deliverypartner.CreateBy);
                ViewBag.ModifyBy = new SelectList(db.PersonalDetails, "ID", "FirstName", deliverypartner.ModifyBy);
                ViewBag.PincodeID = new SelectList(db.Pincodes, "ID", "Name", deliverypartner.PincodeID);
                ViewBag.VehicleTypeID = new SelectList(db.VehicleTypes, "ID", "Name", deliverypartner.VehicleTypeID);

                var days = from DeliveryPartner.Common.Constant.DAYS_TYPE d in Enum.GetValues(typeof(DeliveryPartner.Common.Constant.DAYS_TYPE)) select new { Name = d.ToString() };
                ViewBag.WeeklyOff = new SelectList(days, "Name", "Name", deliverypartner.WeeklyOff);

                LoadTimeSlab(deliverypartner);
                return View(deliverypartner);
            }
        }

        public ActionResult GetAddress(string Pincode)
        {
            /*This Action Responces to AJAX Call
             * After entering Pincode returens City, District and State Information
             * */
            if (!(db.Pincodes.Any(p => p.Name == Pincode)))
            {
                var errorMsg = "Pincode Dose Not Exist, Please Contact Admin!";
                return View(new { success = false, Error = errorMsg });
            }

            long CityId = db.Pincodes.Single(p => p.Name == Pincode).CityID;
            ViewBag.City = db.Cities.Single(c => c.ID == CityId).Name.ToString();

            long DistrictId = db.Cities.Single(c => c.ID == CityId).DistrictID;
            ViewBag.District = db.Districts.Single(d => d.ID == DistrictId).Name.ToString();

            long StateId = db.Districts.Single(d => d.ID == DistrictId).StateID;
            ViewBag.State = db.States.Single(d => d.ID == StateId).Name.ToString();

            return PartialView("_Address");
        }

        private void LoadTimeSlab(ModelLayer.Models.DeliveryPartner deliverypartner)
        {
            List<TimeHourSlabing> lTimeHourSlabing = new List<TimeHourSlabing>();
            for (int hr = 0; hr <= 23; hr++)
            {
                for (int mn = 0; mn <= 59; mn += 15)
                {
                    TimeHourSlabing thr = new TimeHourSlabing();
                    thr.HourValue = hr.ToString("00") + ":" + mn.ToString("00");
                    if (hr < 12)
                    {
                        thr.HourText = hr.ToString("00") + ":" + mn.ToString("00") + " AM";
                        if (hr == 0)
                        {
                            thr.HourText = "12" + ":" + mn.ToString("00") + " AM";
                        }
                    }
                    else
                    {
                        thr.HourText = (hr - 12).ToString("00") + ":" + mn.ToString("00") + " PM";
                        if (hr == 12)
                        {
                            thr.HourText = "12" + ":" + mn.ToString("00") + " PM";
                        }
                    }
                    lTimeHourSlabing.Add(thr);
                }
            }

            if (deliverypartner == null)
            {
                ViewBag.OpeningTime = new SelectList(lTimeHourSlabing, "HourValue", "HourText", "10:00");
                ViewBag.ClosingTime = new SelectList(lTimeHourSlabing, "HourValue", "HourText", "10:00");
            }
            else
            {
                string lSelectedOpeningTime = deliverypartner.OpeningTime.ToString().Substring(0, 5);
                ViewBag.OpeningTime = new SelectList(lTimeHourSlabing, "HourValue", "HourText", lSelectedOpeningTime);
                string lSelectedClosingTime = deliverypartner.ClosingTime.ToString().Substring(0, 5);
                ViewBag.ClosingTime = new SelectList(lTimeHourSlabing, "HourValue", "HourText", lSelectedClosingTime);
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
