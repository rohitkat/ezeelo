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
using CRM.Models.ViewModel;
using CRM.Models;

namespace CRM.Controllers
{
    public class PersonalDetailController : Controller
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

        //[SessionExpire]
        //public ActionResult Index()
        //{
        //    var personaldetails = db.PersonalDetails.Include(p => p.PersonalDetail2).Include(p => p.PersonalDetail3).Include(p => p.Pincode).Include(p => p.Salutation).Include(p => p.UserLogin);
        //    return View(personaldetails.ToList());
        //}

        [SessionExpire]
        // GET: /PersonalDetail/Details/5
        public ActionResult Details(long? id)
        {
            SessionDetails();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PersonalDetail personalDetail = db.PersonalDetails.Find(id);
            if (personalDetail == null)
            {
                return HttpNotFound();
            }
            if (personalDetail.ID != customerCareSessionViewModel.PersonalDetailID)
            {
                return View("AccessDenied");
            }
            return View(personalDetail);
        }

        [SessionExpire]
        // GET: /PersonalDetail/Create
        public ActionResult Create()
        {
            SessionDetails();
            List<GenderTypeViewModel> GenderTypeViewModels = new List<GenderTypeViewModel>();
            GenderTypeViewModels.Add(new GenderTypeViewModel { ID = 1, Name = "Male" });
            GenderTypeViewModels.Add(new GenderTypeViewModel { ID = 1, Name = "Female" });

            ViewBag.Gender = new SelectList(GenderTypeViewModels, "Name", "Name");


            ViewBag.CreateBy = new SelectList(db.PersonalDetails, "ID", "FirstName");
            ViewBag.ModifyBy = new SelectList(db.PersonalDetails, "ID", "FirstName");
            ViewBag.PincodeID = new SelectList(db.Pincodes, "ID", "Name");
            ViewBag.SalutationID = new SelectList(db.Salutations, "ID", "Name");
            ViewBag.UserLoginID = new SelectList(db.UserLogins, "ID", "Mobile");
            return View();
        }

        // POST: /PersonalDetail/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [SessionExpire]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(string Pincode, [Bind(Include = "ID,UserLoginID,SalutationID,FirstName,MiddleName,LastName,DOB,Gender,PincodeID,Address,AlternateMobile,AlternateEmail,IsActive,CreateDate,CreateBy")] PersonalDetail personaldetail)
        {
            SessionDetails();
            try
            {
                personaldetail.CreateDate = DateTime.Now;
                personaldetail.CreateBy = customerCareSessionViewModel.PersonalDetailID;
                personaldetail.UserLoginID = customerCareSessionViewModel.UserLoginID;

                Pincode lPincode = db.Pincodes.Single(x => x.Name == Pincode);
                if (lPincode == null)
                {
                    return View("Error");
                }
                personaldetail.PincodeID = lPincode.ID;

                if (ModelState.IsValid)
                {
                    db.PersonalDetails.Add(personaldetail);
                    db.SaveChanges();
                    return View("Details");
                }

                List<GenderTypeViewModel> GenderTypeViewModels = new List<GenderTypeViewModel>();
                GenderTypeViewModels.Add(new GenderTypeViewModel { ID = 1, Name = "Male" });
                GenderTypeViewModels.Add(new GenderTypeViewModel { ID = 2, Name = "Female" });

                ViewBag.Gender = new SelectList(GenderTypeViewModels, "Name", "Name", personaldetail.Gender.Trim());

                ViewBag.CreateBy = new SelectList(db.PersonalDetails, "ID", "FirstName", personaldetail.CreateBy);
                ViewBag.ModifyBy = new SelectList(db.PersonalDetails, "ID", "FirstName", personaldetail.ModifyBy);
                ViewBag.PincodeID = new SelectList(db.Pincodes, "ID", "Name", personaldetail.PincodeID);
                ViewBag.SalutationID = new SelectList(db.Salutations, "ID", "Name", personaldetail.SalutationID);
                ViewBag.UserLoginID = new SelectList(db.UserLogins, "ID", "Mobile", personaldetail.UserLoginID);
                return View(personaldetail);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Error", "There's something wrong with personal detail values!");

                //Code to write error log
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[PersonalDetail][POST:Create]",
                    BusinessLogicLayer.ErrorLog.Module.CRM, System.Web.HttpContext.Current.Server);

                List<GenderTypeViewModel> GenderTypeViewModels = new List<GenderTypeViewModel>();
                GenderTypeViewModels.Add(new GenderTypeViewModel { ID = 1, Name = "Male" });
                GenderTypeViewModels.Add(new GenderTypeViewModel { ID = 2, Name = "Female" });

                ViewBag.Gender = new SelectList(GenderTypeViewModels, "Name", "Name");

                ViewBag.CreateBy = new SelectList(db.PersonalDetails, "ID", "FirstName", personaldetail.CreateBy);
                ViewBag.ModifyBy = new SelectList(db.PersonalDetails, "ID", "FirstName", personaldetail.ModifyBy);
                ViewBag.PincodeID = new SelectList(db.Pincodes, "ID", "Name", personaldetail.PincodeID);
                ViewBag.SalutationID = new SelectList(db.Salutations, "ID", "Name", personaldetail.SalutationID);
                ViewBag.UserLoginID = new SelectList(db.UserLogins, "ID", "Mobile", personaldetail.UserLoginID);
                return View(personaldetail);
            }
        }

        //[SessionExpire]
        //private ActionResult View(Func<long?, ActionResult> Details)
        //{
        //    throw new NotImplementedException();
        //}

        [SessionExpire]
        // GET: /PersonalDetail/Edit/5
        public ActionResult Edit(long? id)
        {
            SessionDetails();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PersonalDetail personaldetail = db.PersonalDetails.Find(id);
            if (personaldetail == null)
            {
                return HttpNotFound();
            }
            if (personaldetail.ID != customerCareSessionViewModel.PersonalDetailID)
            {
                return View("AccessDenied");
            }

            Pincode lPincode = db.Pincodes.Find(personaldetail.PincodeID);
            if (lPincode != null)
            {
                ViewBag.Pincode = lPincode.Name;
            }

            List<GenderTypeViewModel> GenderTypeViewModels = new List<GenderTypeViewModel>();
            GenderTypeViewModels.Add(new GenderTypeViewModel { ID = 1, Name = "Male" });
            GenderTypeViewModels.Add(new GenderTypeViewModel { ID = 2, Name = "Female" });

            ViewBag.Gender = new SelectList(GenderTypeViewModels, "Name", "Name",personaldetail.Gender.Trim());

            ViewBag.CreateBy = new SelectList(db.PersonalDetails, "ID", "FirstName", personaldetail.CreateBy);
            ViewBag.ModifyBy = new SelectList(db.PersonalDetails, "ID", "FirstName", personaldetail.ModifyBy);
            ViewBag.PincodeID = new SelectList(db.Pincodes, "ID", "Name", personaldetail.PincodeID);
            ViewBag.SalutationID = new SelectList(db.Salutations, "ID", "Name", personaldetail.SalutationID);
            ViewBag.UserLoginID = new SelectList(db.UserLogins, "ID", "Mobile", personaldetail.UserLoginID);
            return View(personaldetail);
        }

        // POST: /PersonalDetail/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [SessionExpire]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(string Pincode, [Bind(Include = "ID,UserLoginID,SalutationID,FirstName,MiddleName,LastName,DOB,Gender,PincodeID,Address,AlternateMobile,AlternateEmail,IsActive,CreateDate,CreateBy,ModifyDate,ModifyBy")] PersonalDetail personaldetail)
        {
            SessionDetails();
            try
            {
                EzeeloDBContext db1 = new EzeeloDBContext();
                PersonalDetail lPersonalDetail = db1.PersonalDetails.Find(personaldetail.ID);
                personaldetail.UserLoginID = customerCareSessionViewModel.UserLoginID;
                personaldetail.CreateBy = lPersonalDetail.CreateBy;
                personaldetail.CreateDate = lPersonalDetail.CreateDate;
                personaldetail.ModifyBy = customerCareSessionViewModel.PersonalDetailID;
                personaldetail.ModifyDate = DateTime.Now;
                db1.Dispose();

                Pincode lPincode = db.Pincodes.Single(x => x.Name == Pincode);
                if (lPincode == null)
                {
                    return View("Error");
                }
                personaldetail.PincodeID = lPincode.ID;

                if (ModelState.IsValid)
                {
                    db.Entry(personaldetail).State = EntityState.Modified;
                    db.SaveChanges();
                    return View("Details", personaldetail);
                }

                List<GenderTypeViewModel> GenderTypeViewModels = new List<GenderTypeViewModel>();
                GenderTypeViewModels.Add(new GenderTypeViewModel { ID = 1, Name = "Male" });
                GenderTypeViewModels.Add(new GenderTypeViewModel { ID = 2, Name = "Female" });

                ViewBag.Gender = new SelectList(GenderTypeViewModels, "Name", "Name",personaldetail.Gender.Trim());

                ViewBag.CreateBy = new SelectList(db.PersonalDetails, "ID", "FirstName", personaldetail.CreateBy);
                ViewBag.ModifyBy = new SelectList(db.PersonalDetails, "ID", "FirstName", personaldetail.ModifyBy);
                ViewBag.PincodeID = new SelectList(db.Pincodes, "ID", "Name", personaldetail.PincodeID);
                ViewBag.SalutationID = new SelectList(db.Salutations, "ID", "Name", personaldetail.SalutationID);
                ViewBag.UserLoginID = new SelectList(db.UserLogins, "ID", "Mobile", personaldetail.UserLoginID);
                return View(personaldetail);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Error", "There's something wrong with personal detail values!");

                //Code to write error log
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[PersonalDetail][POST:Edit]",
                    BusinessLogicLayer.ErrorLog.Module.CRM, System.Web.HttpContext.Current.Server);

                List<GenderTypeViewModel> GenderTypeViewModels = new List<GenderTypeViewModel>();
                GenderTypeViewModels.Add(new GenderTypeViewModel { ID = 1, Name = "Male" });
                GenderTypeViewModels.Add(new GenderTypeViewModel { ID = 1, Name = "Female" });

                ViewBag.Gender = new SelectList(GenderTypeViewModels, "Name", "Name", personaldetail.Gender.Trim());

                ViewBag.CreateBy = new SelectList(db.PersonalDetails, "ID", "FirstName", personaldetail.CreateBy);
                ViewBag.ModifyBy = new SelectList(db.PersonalDetails, "ID", "FirstName", personaldetail.ModifyBy);
                ViewBag.PincodeID = new SelectList(db.Pincodes, "ID", "Name", personaldetail.PincodeID);
                ViewBag.SalutationID = new SelectList(db.Salutations, "ID", "Name", personaldetail.SalutationID);
                ViewBag.UserLoginID = new SelectList(db.UserLogins, "ID", "Mobile", personaldetail.UserLoginID);
                return View(personaldetail);
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
