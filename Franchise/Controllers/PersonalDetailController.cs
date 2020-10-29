using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ModelLayer.Models;
using System.Collections;
using ModelLayer.Models.ViewModel;
using BusinessLogicLayer;
using Franchise.Models;

namespace Franchise.Controllers
{
    [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
    [SessionExpire]
    public class PersonalDetailController : Controller
    {
        #region Genral Code
        private EzeeloDBContext db = new EzeeloDBContext();

        private long GetPersonalDetailID()
        {
            long PID = 0;
            try
            {
                int franchiseID = Convert.ToInt32(Session["FRANCHISE_ID"]);
                long UID = 0, businessDetailID = 0;
                businessDetailID = db.Franchises.Where(x => x.ID == franchiseID).Select(x => x.BusinessDetailID).FirstOrDefault();
                UID = db.BusinessDetails.Where(x => x.ID == businessDetailID).Select(x => x.UserLoginID).FirstOrDefault();
                PID = db.PersonalDetails.Where(x => x.UserLoginID == UID).Select(x => x.ID).FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw new BusinessLogicLayer.MyException("[GetFranchiseID]", "Can't Get Franchise ID! in Method !" + Environment.NewLine + ex.Message);
            }
            return PID;
        }
        #endregion
        //public ActionResult Index()
        //{
        //    var personaldetails = db.PersonalDetails.Include(p => p.PersonalDetail2).Include(p => p.PersonalDetail3).Include(p => p.Pincode).Include(p => p.Salutation).Include(p => p.UserLogin);
        //    return View(personaldetails.ToList());
        //}

        [SessionExpire]
        [CustomAuthorize(Roles = "PersonalDetail/CanRead")]
        public ActionResult Details()
        {
            try
            {
                long id = CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(Session["ID"]));
                //if (id == null)
                // {
                //     return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                // }
                PersonalDetail personaldetail = db.PersonalDetails.Find(id);
                if (personaldetail == null)
                {
                    return HttpNotFound();
                }

                Pincode lPincode = db.Pincodes.Find(personaldetail.PincodeID);
                if (lPincode != null)
                {
                    ViewBag.Pincode = lPincode.Name;
                }
                List<GenderTypeViewModel> GenderTypeViewModels = new List<GenderTypeViewModel>();
                GenderTypeViewModels.Add(new GenderTypeViewModel { ID = 1, Name = "Male" });
                GenderTypeViewModels.Add(new GenderTypeViewModel { ID = 2, Name = "Female" });
                if (personaldetail.Gender != null)
                {
                    ViewBag.Gender = new SelectList(GenderTypeViewModels, "Name", "Name", personaldetail.Gender.Trim());
                }
                else
                {
                    ViewBag.Gender = new SelectList(GenderTypeViewModels, "Name", "Name");
                }

                ViewBag.CreateBy = new SelectList(db.PersonalDetails, "ID", "FirstName", personaldetail.CreateBy);
                ViewBag.ModifyBy = new SelectList(db.PersonalDetails, "ID", "FirstName", personaldetail.ModifyBy);
                ViewBag.PincodeID = new SelectList(db.Pincodes, "ID", "Name", personaldetail.PincodeID);
                ViewBag.SalutationID = new SelectList(db.Salutations, "ID", "Name", personaldetail.SalutationID);
                ViewBag.UserLoginID = new SelectList(db.UserLogins, "ID", "Mobile", personaldetail.UserLoginID);
                return View(personaldetail);
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[PersonalDetailController][GET:Details]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Franchise, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[PersonalDetailController][GET:Details]",
                    BusinessLogicLayer.ErrorLog.Module.Franchise, System.Web.HttpContext.Current.Server);
            }
            return View();
        }


        // GET: /PersonalDetail/Edit/5
        [SessionExpire]
        [CustomAuthorize(Roles = "PersonalDetail/CanRead")]
        public ActionResult Edit()
        {
            try
            {
                long id = GetPersonalDetailID(); // CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(Session["ID"]));
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                PersonalDetail personaldetail = db.PersonalDetails.Find(id);
                if (personaldetail == null)
                {
                    return HttpNotFound();
                }

                Pincode lPincode = db.Pincodes.Find(personaldetail.PincodeID);
                if (lPincode != null)
                {
                    ViewBag.Pincode = lPincode.Name;
                }
                List<GenderTypeViewModel> GenderTypeViewModels = new List<GenderTypeViewModel>();
                GenderTypeViewModels.Add(new GenderTypeViewModel { ID = 1, Name = "Male" });
                GenderTypeViewModels.Add(new GenderTypeViewModel { ID = 2, Name = "Female" });

                if (personaldetail.Gender != null)
                {
                    ViewBag.Gender = new SelectList(GenderTypeViewModels, "Name", "Name", personaldetail.Gender.Trim());
                }
                else
                {
                    ViewBag.Gender = new SelectList(GenderTypeViewModels, "Name", "Name");
                }

                ViewBag.DOB1 = personaldetail.DOB.ToString();
                ViewBag.CreateBy = new SelectList(db.PersonalDetails, "ID", "FirstName", personaldetail.CreateBy);
                ViewBag.ModifyBy = new SelectList(db.PersonalDetails, "ID", "FirstName", personaldetail.ModifyBy);
                ViewBag.PincodeID = new SelectList(db.Pincodes, "ID", "Name", personaldetail.PincodeID);
                ViewBag.SalutationID = new SelectList(db.Salutations, "ID", "Name", personaldetail.SalutationID);
                ViewBag.UserLoginID = new SelectList(db.UserLogins, "ID", "Mobile", personaldetail.UserLoginID);
                return View(personaldetail);
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[PersonalDetailController][GET:Edit]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Franchise, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[PersonalDetailController][GET:Edit]",
                    BusinessLogicLayer.ErrorLog.Module.Franchise, System.Web.HttpContext.Current.Server);
            }
            return View();
        }

        // POST: /PersonalDetail/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [SessionExpire]
        [CustomAuthorize(Roles = "PersonalDetail/CanWrite")]
        public ActionResult Edit(string Pincode, [Bind(Include = "ID,UserLoginID,SalutationID,FirstName,MiddleName,LastName,DOB,Gender,PincodeID,Address,AlternateMobile,AlternateEmail,IsActive,CreateDate,CreateBy,ModifyDate,ModifyBy,NetworkIP,DeviceType,DeviceID")] PersonalDetail personaldetail, string DOB1)
        {

            try
            {
                long PersonalDetailID = GetPersonalDetailID(); //CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(Session["ID"]));

                ViewBag.CreateBy = new SelectList(db.PersonalDetails, "ID", "FirstName", personaldetail.CreateBy);
                ViewBag.ModifyBy = new SelectList(db.PersonalDetails, "ID", "FirstName", personaldetail.ModifyBy);
                ViewBag.PincodeID = new SelectList(db.Pincodes, "ID", "Name", personaldetail.PincodeID);
                ViewBag.SalutationID = new SelectList(db.Salutations, "ID", "Name", personaldetail.SalutationID);
                ViewBag.UserLoginID = new SelectList(db.UserLogins, "ID", "Mobile", personaldetail.UserLoginID);

                List<GenderTypeViewModel> GenderTypeViewModels = new List<GenderTypeViewModel>();
                GenderTypeViewModels.Add(new GenderTypeViewModel { ID = 1, Name = "Male" });
                GenderTypeViewModels.Add(new GenderTypeViewModel { ID = 2, Name = "Female" });

                if (personaldetail.Gender != null)
                {
                    ViewBag.Gender = new SelectList(GenderTypeViewModels, "Name", "Name", personaldetail.Gender.Trim());
                }
                else
                {
                    ViewBag.Gender = new SelectList(GenderTypeViewModels, "Name", "Name");
                }

                PersonalDetail pd = db.PersonalDetails.Find(personaldetail.ID);

                //DateTime lDOB = DateTime.Now;
                //if (DOB1 != "")
                //{
                //    if (DateTime.TryParse(DOB1, out lDOB)) { }
                //    personaldetail.DOB = lDOB;
                //}

                DateTime? lDOB = CommonFunctions.GetDate(DOB1);
                if (lDOB != null)
                { pd.DOB = lDOB; }
                ViewBag.DOB1 = lDOB;
                if (pd.DOB > DateTime.Now)
                {
                    ModelState.AddModelError("CustomError", "Date of birth can't be in future");
                }
                pd.UserLoginID = personaldetail.UserLoginID;
                pd.SalutationID = personaldetail.SalutationID;
                pd.FirstName = personaldetail.FirstName;
                pd.MiddleName = personaldetail.MiddleName;
                pd.LastName = personaldetail.LastName;

                pd.Gender = personaldetail.Gender;

                Pincode lPincode = db.Pincodes.Single(x => x.Name == Pincode);
                if (lPincode == null)
                {
                    return View("Error");
                }
                personaldetail.PincodeID = lPincode.ID;

                pd.PincodeID = personaldetail.PincodeID;
                pd.Address = personaldetail.Address;
                pd.AlternateMobile = personaldetail.AlternateMobile;
                pd.AlternateEmail = personaldetail.AlternateEmail;
                pd.IsActive = true;
                pd.ModifyDate = DateTime.UtcNow;
                pd.ModifyBy = PersonalDetailID;
                pd.NetworkIP = BusinessLogicLayer.CommonFunctions.GetClientIP();
                pd.DeviceType = "x";
                pd.DeviceID = "x";
                if (ModelState.IsValid)
                {
                    db.SaveChanges();
                    ViewBag.Message = "Personal Detail Save Successfully.";
                }
                return View(personaldetail);
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[PersonalDetailController][POST:Edit]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Franchise, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[PersonalDetailController][POST:Edit]",
                    BusinessLogicLayer.ErrorLog.Module.Franchise, System.Web.HttpContext.Current.Server);
            }
            return View(personaldetail);
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
        public ActionResult GetAddress(string Pincode)
        {
            try
            {
                if (!(db.Pincodes.Any(p => p.Name == Pincode)))
                {
                    //var errorMsg = "Pincode Dose Not Exist, Please Contact Admin!";
                    //return View(new { success = false, Error = errorMsg });
                    return Json("1", JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                throw new BusinessLogicLayer.MyException("[GetAddress]", "Can't Get Address! in Method !" + Environment.NewLine + ex.Message);
            }
            return Json("0", JsonRequestBehavior.AllowGet);
        }




    }
}
