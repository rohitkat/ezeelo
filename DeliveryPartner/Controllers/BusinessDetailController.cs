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
    public class BusinessDetailController : Controller
    {
        private EzeeloDBContext db = new EzeeloDBContext();
        private DeliveryPartnerSessionViewModel deliveryPartnerSessionViewModel = new DeliveryPartnerSessionViewModel();
        private int pageSize = 10;

        public void SessionDetails()
        {
            deliveryPartnerSessionViewModel.UserLoginID = Convert.ToInt64(Session["ID"]);
            deliveryPartnerSessionViewModel.Username = Session["UserName"].ToString();
            if (!Common.Common.GetAllLoginDetailFromSession(ref deliveryPartnerSessionViewModel))
            {
                new RedirectToRouteResult(new System.Web.Routing.RouteValueDictionary   
                   {  
                    { "action", "Login" },  
                    { "controller", "Login" }
                 });
            }
        }
        //// GET: /BusinessDetail/
        //[SessionExpire]
        //public ActionResult Index()
        //{
        //    SessionDetails();
        //    var businessdetails = db.BusinessDetails.Include(b => b.BusinessType).Include(b => b.PersonalDetail).Include(b => b.PersonalDetail1).Include(b => b.Pincode).Include(b => b.SourceOfInfo).Include(b => b.UserLogin);
        //    return View(businessdetails.ToList());
        //}

        // GET: /BusinessDetail/Details/5
        [SessionExpire]
        public ActionResult Details(long? id)
        {
            SessionDetails();
            if (id == null || id != deliveryPartnerSessionViewModel.BusinessDetailID)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BusinessDetail businessdetail = db.BusinessDetails.Find(id);
            if (businessdetail == null)
            {
                return HttpNotFound();
            }
            return View(businessdetail);
        }

        // GET: /BusinessDetail/Create
        [SessionExpire]
        public ActionResult Create()
        {
            SessionDetails();
            ViewBag.BusinessTypeID = new SelectList(db.BusinessTypes, "ID", "Name");
            ViewBag.CreateBy = new SelectList(db.PersonalDetails, "ID", "FirstName");
            ViewBag.ModifyBy = new SelectList(db.PersonalDetails, "ID", "FirstName");
            ViewBag.PincodeID = new SelectList(db.Pincodes, "ID", "Name");
            ViewBag.SourceOfInfoID = new SelectList(db.SourceOfInfoes, "ID", "Name");
            ViewBag.UserLoginID = new SelectList(db.UserLogins, "ID", "Mobile");
            return View();
        }

        // POST: /BusinessDetail/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [SessionExpire]
        public ActionResult Create(string Pincode, [Bind(Include = "ID,UserLoginID,Name,BusinessTypeID,ContactPerson,Mobile,Email,Landline1,Landline2,FAX,Address,PincodeID,Website,YearOfEstablishment,SourceOfInfoID,SourcesInfoDescription,IsActive,CreateDate,CreateBy,ModifyDate,ModifyBy,NetworkIP,DeviceType,DeviceID")] BusinessDetail businessdetail, string YearOfEstablishment1)
        {
            SessionDetails();
            try
            {
                businessdetail.CreateDate = DateTime.Now;
                businessdetail.CreateBy = deliveryPartnerSessionViewModel.PersonalDetailID;
                businessdetail.UserLoginID = deliveryPartnerSessionViewModel.UserLoginID;

                DateTime lYearOfEstablishment = DateTime.Now;
                if (DateTime.TryParse(YearOfEstablishment1, out lYearOfEstablishment)) { }
                businessdetail.YearOfEstablishment = lYearOfEstablishment;


                Pincode lPincode = db.Pincodes.Single(x => x.Name == Pincode);
                if (lPincode == null)
                {
                    return View("Error");
                }
                businessdetail.PincodeID = lPincode.ID;

                if (ModelState.IsValid)
                {
                    db.BusinessDetails.Add(businessdetail);
                    db.SaveChanges();
                    return View("Details");
                }

                ViewBag.BusinessTypeID = new SelectList(db.BusinessTypes, "ID", "Name", businessdetail.BusinessTypeID);
                ViewBag.CreateBy = new SelectList(db.PersonalDetails, "ID", "FirstName", businessdetail.CreateBy);
                ViewBag.ModifyBy = new SelectList(db.PersonalDetails, "ID", "FirstName", businessdetail.ModifyBy);
                ViewBag.PincodeID = new SelectList(db.Pincodes, "ID", "Name", businessdetail.PincodeID);
                ViewBag.SourceOfInfoID = new SelectList(db.SourceOfInfoes, "ID", "Name", businessdetail.SourceOfInfoID);
                ViewBag.UserLoginID = new SelectList(db.UserLogins, "ID", "Mobile", businessdetail.UserLoginID);
                return View(businessdetail);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Error" ,"There's Something wrong with the business detail values!");
               
                //Code to write error log
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[BusinessDetail][POST:Create]",
                    BusinessLogicLayer.ErrorLog.Module.DeliveryPartner, System.Web.HttpContext.Current.Server);

                ViewBag.BusinessTypeID = new SelectList(db.BusinessTypes, "ID", "Name", businessdetail.BusinessTypeID);
                ViewBag.CreateBy = new SelectList(db.PersonalDetails, "ID", "FirstName", businessdetail.CreateBy);
                ViewBag.ModifyBy = new SelectList(db.PersonalDetails, "ID", "FirstName", businessdetail.ModifyBy);
                ViewBag.PincodeID = new SelectList(db.Pincodes, "ID", "Name", businessdetail.PincodeID);
                ViewBag.SourceOfInfoID = new SelectList(db.SourceOfInfoes, "ID", "Name", businessdetail.SourceOfInfoID);
                ViewBag.UserLoginID = new SelectList(db.UserLogins, "ID", "Mobile", businessdetail.UserLoginID);
                return View(businessdetail);
            }
        }

        // GET: /BusinessDetail/Edit/5
        [SessionExpire]
        public ActionResult Edit(long? id)
        {
            SessionDetails();
            if (id == null || id != deliveryPartnerSessionViewModel.BusinessDetailID)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BusinessDetail businessdetail = db.BusinessDetails.Find(id);
            if (businessdetail == null)
            {
                return HttpNotFound();
            }

            Pincode lPincode = db.Pincodes.Find(businessdetail.PincodeID);
            if (lPincode == null)
            {
                return View("Error");
            }
            ViewBag.Pincode = lPincode.Name;

            ViewBag.YearOfEstablishment1 = "";
            if (businessdetail.YearOfEstablishment != null)
            {
                DateTime lYearOfEstablishment = Convert.ToDateTime(businessdetail.YearOfEstablishment);
                ViewBag.YearOfEstablishment1 = lYearOfEstablishment.ToString("dd/MM/yyyy");
            }

            ViewBag.BusinessTypeID = new SelectList(db.BusinessTypes, "ID", "Name", businessdetail.BusinessTypeID);
            ViewBag.CreateBy = new SelectList(db.PersonalDetails, "ID", "FirstName", businessdetail.CreateBy);
            ViewBag.ModifyBy = new SelectList(db.PersonalDetails, "ID", "FirstName", businessdetail.ModifyBy);
            ViewBag.PincodeID = new SelectList(db.Pincodes, "ID", "Name", businessdetail.PincodeID);
            ViewBag.SourceOfInfoID = new SelectList(db.SourceOfInfoes, "ID", "Name", businessdetail.SourceOfInfoID);
            ViewBag.UserLoginID = new SelectList(db.UserLogins, "ID", "Mobile", businessdetail.UserLoginID);
            return View(businessdetail);
        }

        // POST: /BusinessDetail/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [SessionExpire]
        public ActionResult Edit(string Pincode, [Bind(Include = "ID,UserLoginID,Name,BusinessTypeID,ContactPerson,Mobile,Email,Landline1,Landline2,FAX,Address,PincodeID,Website,YearOfEstablishment,SourceOfInfoID,SourcesInfoDescription,IsActive,CreateDate,CreateBy,ModifyDate,ModifyBy")] BusinessDetail businessdetail, string YearOfEstablishment1)
        {
            SessionDetails();
            EzeeloDBContext db1 = new EzeeloDBContext();
            BusinessDetail lBusinessDetail = db1.BusinessDetails.SingleOrDefault(x => x.UserLoginID == deliveryPartnerSessionViewModel.UserLoginID && x.BusinessTypeID == (int)Common.Constant.BUSINESS_TYPE.DELIVERY_PARTNER);
            DateTime lYearOfEstablishment = DateTime.Now;
            if (DateTime.TryParse(YearOfEstablishment1, out lYearOfEstablishment)) { }
            try
            {
                businessdetail.UserLoginID = lBusinessDetail.UserLoginID;
                businessdetail.BusinessTypeID = lBusinessDetail.BusinessTypeID;
                businessdetail.YearOfEstablishment = lYearOfEstablishment;
                businessdetail.CreateBy = lBusinessDetail.CreateBy;
                businessdetail.CreateDate = lBusinessDetail.CreateDate;
                businessdetail.ModifyBy = deliveryPartnerSessionViewModel.PersonalDetailID;
                businessdetail.ModifyDate = DateTime.Now;
                businessdetail.ID = lBusinessDetail.ID;
                db1.Dispose();

                Pincode lPincode = db.Pincodes.Single(x => x.Name == Pincode);
                if (lPincode == null)
                {
                    return View("Error");
                }
                businessdetail.PincodeID = lPincode.ID;

                if (ModelState.IsValid)
                {
                    db.Entry(businessdetail).State = EntityState.Modified;
                    db.SaveChanges();
                    TempData["Msg"] = "Data Saved Successfully";
                    return View("Details", businessdetail);
                }
                ViewBag.BusinessTypeID = new SelectList(db.BusinessTypes, "ID", "Name", businessdetail.BusinessTypeID);
                ViewBag.CreateBy = new SelectList(db.PersonalDetails, "ID", "FirstName", businessdetail.CreateBy);
                ViewBag.ModifyBy = new SelectList(db.PersonalDetails, "ID", "FirstName", businessdetail.ModifyBy);
                ViewBag.PincodeID = new SelectList(db.Pincodes, "ID", "Name", businessdetail.PincodeID);
                ViewBag.SourceOfInfoID = new SelectList(db.SourceOfInfoes, "ID", "Name", businessdetail.SourceOfInfoID);
                ViewBag.UserLoginID = new SelectList(db.UserLogins, "ID", "Mobile", businessdetail.UserLoginID);
                return View(businessdetail);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Error", "There's Something wrong with the business detail values!");

                //Code to write error log
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[BusinessDetail][POST:Edit]",
                    BusinessLogicLayer.ErrorLog.Module.DeliveryPartner, System.Web.HttpContext.Current.Server);

                ViewBag.BusinessTypeID = new SelectList(db.BusinessTypes, "ID", "Name", businessdetail.BusinessTypeID);
                ViewBag.CreateBy = new SelectList(db.PersonalDetails, "ID", "FirstName", businessdetail.CreateBy);
                ViewBag.ModifyBy = new SelectList(db.PersonalDetails, "ID", "FirstName", businessdetail.ModifyBy);
                ViewBag.PincodeID = new SelectList(db.Pincodes, "ID", "Name", businessdetail.PincodeID);
                ViewBag.SourceOfInfoID = new SelectList(db.SourceOfInfoes, "ID", "Name", businessdetail.SourceOfInfoID);
                ViewBag.UserLoginID = new SelectList(db.UserLogins, "ID", "Mobile", businessdetail.UserLoginID);
                return View(businessdetail);
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
