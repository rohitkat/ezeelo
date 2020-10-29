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
    public class MPersonalDetailController : Controller
    {
        #region Genral Code
        private EzeeloDBContext db = new EzeeloDBContext();


        private long GetPersonalDetailID()
        {
            //Session["USER_LOGIN_ID"] = 1;
            // long UserLoginID = Convert.ToInt32(Session["USER_LOGIN_ID"]);
            long UserLoginID = Convert.ToInt32(Session["ID"]);
            long PersonalDetailID = 0;
            try
            {
                if (UserLoginID > 0)
                {
                    PersonalDetailID = Convert.ToInt32(db.PersonalDetails.Where(x => x.UserLoginID == UserLoginID).Select(x => x.ID).First());
                }
            }
            catch (Exception ex)
            {
                throw new BusinessLogicLayer.MyException("[PlacedController][GetShopID]", "Can't find PersonalDetailID !" + Environment.NewLine + ex.Message);
            }
            return PersonalDetailID;
        }

        private long GetOwnerBankID()
        {
            long ownerBankID = 0;
            try
            {
                long UserLoginID = Convert.ToInt32(Session["USER_LOGIN_ID"]);
                long shopID = 0, businesstypeID = 0;
                long businessDetailID = 0;
                businessDetailID = db.BusinessDetails.Where(x => x.UserLoginID == UserLoginID).Select(x => x.ID).FirstOrDefault();
                businesstypeID = db.BusinessDetails.Where(x => x.UserLoginID == UserLoginID).Select(x => x.BusinessTypeID).FirstOrDefault();
                shopID = db.Shops.Where(x => x.BusinessDetailID == businessDetailID).Select(x => x.ID).FirstOrDefault();
                ownerBankID = db.OwnerBanks.Where(x => x.BusinessTypeID == businesstypeID && x.OwnerID == shopID).Select(x => x.ID).FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw new BusinessLogicLayer.MyException("[GetOwnerBankID]", "Can't Get Owner Bank ID! in Method !" + Environment.NewLine + ex.Message);
            }
            return ownerBankID;
        }

        #endregion
        //[SessionExpire]
        [CustomAuthorize(Roles = "MPersonalDetail/CanRead")]
        public ActionResult Index()
        {
            var personaldetails = db.PersonalDetails.Include(p => p.PersonalDetail2).Include(p => p.PersonalDetail3).Include(p => p.Pincode).Include(p => p.Salutation).Include(p => p.UserLogin);
            return View(personaldetails.ToList());
        }


        public ActionResult Details()
        {
            long id = GetPersonalDetailID();
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

            ViewBag.CreateBy = new SelectList(db.PersonalDetails, "ID", "FirstName", personaldetail.CreateBy);
            ViewBag.ModifyBy = new SelectList(db.PersonalDetails, "ID", "FirstName", personaldetail.ModifyBy);
            ViewBag.PincodeID = new SelectList(db.Pincodes, "ID", "Name", personaldetail.PincodeID);
            ViewBag.SalutationID = new SelectList(db.Salutations, "ID", "Name", personaldetail.SalutationID);
            ViewBag.UserLoginID = new SelectList(db.UserLogins, "ID", "Mobile", personaldetail.UserLoginID);
            return View(personaldetail);

        }

        // GET: /PersonalDetail/Create
        public ActionResult Create()
        {
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
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,UserLoginID,SalutationID,FirstName,MiddleName,LastName,DOB,Gender,PincodeID,Address,AlternateMobile,AlternateEmail,IsActive,CreateDate,CreateBy,ModifyDate,ModifyBy,NetworkIP,DeviceType,DeviceID")] PersonalDetail personaldetail)
        {
            if (ModelState.IsValid)
            {
                db.PersonalDetails.Add(personaldetail);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CreateBy = new SelectList(db.PersonalDetails, "ID", "FirstName", personaldetail.CreateBy);
            ViewBag.ModifyBy = new SelectList(db.PersonalDetails, "ID", "FirstName", personaldetail.ModifyBy);
            ViewBag.PincodeID = new SelectList(db.Pincodes, "ID", "Name", personaldetail.PincodeID);
            ViewBag.SalutationID = new SelectList(db.Salutations, "ID", "Name", personaldetail.SalutationID);
            ViewBag.UserLoginID = new SelectList(db.UserLogins, "ID", "Mobile", personaldetail.UserLoginID);
            return View(personaldetail);
        }

        // GET: /PersonalDetail/Edit/5
        //[SessionExpire]
        [CustomAuthorize(Roles = "MPersonalDetail/CanRead")]
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                id = Convert.ToInt64(TempData["personalDeailID"]);
            }
            long PersonalDetailID = Convert.ToInt32(db.PersonalDetails.Where(x => x.ID == id).Select(x => x.ID).First());


            PersonalDetail personaldetail = db.PersonalDetails.Find(PersonalDetailID);
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

        // POST: /PersonalDetail/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[SessionExpire]
        [CustomAuthorize(Roles = "MPersonalDetail/CanWrite")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(string Pincode, [Bind(Include = "ID,UserLoginID,SalutationID,FirstName,MiddleName,LastName,DOB,Gender,PincodeID,Address,AlternateMobile,AlternateEmail,IsActive,ModifyDate,ModifyBy,NetworkIP,DeviceType,DeviceID")] PersonalDetail personaldetail, string DOB1)
        {

            try
            {


                long PersonalDetailID = GetPersonalDetailID();
                List<GenderTypeViewModel> GenderTypeViewModels = new List<GenderTypeViewModel>();
                GenderTypeViewModels.Add(new GenderTypeViewModel { ID = 1, Name = "Male" });
                GenderTypeViewModels.Add(new GenderTypeViewModel { ID = 2, Name = "Female" });
                ViewBag.Gender = new SelectList(GenderTypeViewModels, "Name", "Name", personaldetail.Gender);
                ViewBag.CreateBy = new SelectList(db.PersonalDetails, "ID", "FirstName", personaldetail.CreateBy);
                ViewBag.ModifyBy = new SelectList(db.PersonalDetails, "ID", "FirstName", personaldetail.ModifyBy);
                ViewBag.PincodeID = new SelectList(db.Pincodes, "ID", "Name", personaldetail.PincodeID);
                ViewBag.SalutationID = new SelectList(db.Salutations, "ID", "Name", personaldetail.SalutationID);
                ViewBag.UserLoginID = new SelectList(db.UserLogins, "ID", "Mobile", personaldetail.UserLoginID);

                EzeeloDBContext db1 = new EzeeloDBContext();

                PersonalDetail pd = db1.PersonalDetails.Find(personaldetail.ID);

                personaldetail.CreateDate = pd.CreateDate;
                personaldetail.CreateBy = pd.CreateBy;
                personaldetail.UserLoginID = pd.UserLoginID;
                personaldetail.IsActive = pd.IsActive;

                //DateTime lDOB = DateTime.Now;
                //if (DOB1 != "")
                //{
                //    if (DateTime.TryParse(DOB1, out lDOB)) { }
                //    personaldetail.DOB = lDOB;
                //}   
                DateTime? lDOB = CommonFunctions.GetDate(DOB1);
                if (lDOB != null)
                { personaldetail.DOB = lDOB; }
                ViewBag.DOB1 = personaldetail.DOB.ToString();
                if (personaldetail.DOB > DateTime.Now)
                {
                    ModelState.AddModelError("CustomError", "Date of birth can't be in future");
                }
                //pd.UserLoginID = personaldetail.UserLoginID;
                //pd.SalutationID = personaldetail.SalutationID;
                //pd.FirstName = personaldetail.FirstName;
                //pd.MiddleName = personaldetail.MiddleName;
                //pd.LastName = personaldetail.LastName;
                //pd.DOB = personaldetail.DOB;
                //pd.Gender = personaldetail.Gender;

                Pincode lPincode = db.Pincodes.Single(x => x.Name == Pincode);
                if (lPincode == null)
                {
                    return View("Error");
                }
                personaldetail.PincodeID = lPincode.ID;

                personaldetail.ModifyDate = DateTime.UtcNow;
                personaldetail.ModifyBy = PersonalDetailID;
                personaldetail.NetworkIP = BusinessLogicLayer.CommonFunctions.GetClientIP();
                //pd.Address = personaldetail.Address;
                //pd.AlternateMobile = personaldetail.AlternateMobile;
                //pd.AlternateEmail = personaldetail.AlternateEmail;
                //pd.IsActive = true;
                //pd.CreateDate = personaldetail.CreateDate;
                //pd.CreateBy = personaldetail.CreateBy;

                //pd.DeviceType = "x";
                //pd.DeviceID = "x";
                //db.SaveChanges();

               
                if (ModelState.IsValid)
                {
                    db.Entry(personaldetail).State = EntityState.Modified;
                    db.SaveChanges();
                    ViewBag.Message = "Record Saved Successfully.";
                    db1.Dispose();
                    return View(personaldetail);
                }

                return View(personaldetail);
                //return View();
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Error", "There's Something wrong with the Product upload!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[ProductUploadTemp][POST:Edit]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
                List<GenderTypeViewModel> GenderTypeViewModels = new List<GenderTypeViewModel>();
                GenderTypeViewModels.Add(new GenderTypeViewModel { ID = 1, Name = "Male" });
                GenderTypeViewModels.Add(new GenderTypeViewModel { ID = 2, Name = "Female" });
                ViewBag.Gender = new SelectList(GenderTypeViewModels, "Name", "Name", personaldetail.Gender);
                ViewBag.CreateBy = new SelectList(db.PersonalDetails, "ID", "FirstName", personaldetail.CreateBy);
                ViewBag.ModifyBy = new SelectList(db.PersonalDetails, "ID", "FirstName", personaldetail.ModifyBy);
                ViewBag.PincodeID = new SelectList(db.Pincodes, "ID", "Name", personaldetail.PincodeID);
                ViewBag.SalutationID = new SelectList(db.Salutations, "ID", "Name", personaldetail.SalutationID);
                ViewBag.UserLoginID = new SelectList(db.UserLogins, "ID", "Mobile", personaldetail.UserLoginID);
            }
            catch (Exception ex)
            {

                ModelState.AddModelError("Error", "There's Something wrong with the Product upload!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[ProductUploadTemp][POST:Edit]",
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
                List<GenderTypeViewModel> GenderTypeViewModels = new List<GenderTypeViewModel>();
                GenderTypeViewModels.Add(new GenderTypeViewModel { ID = 1, Name = "Male" });
                GenderTypeViewModels.Add(new GenderTypeViewModel { ID = 2, Name = "Female" });
                ViewBag.Gender = new SelectList(GenderTypeViewModels, "Name", "Name", personaldetail.Gender);
                ViewBag.CreateBy = new SelectList(db.PersonalDetails, "ID", "FirstName", personaldetail.CreateBy);
                ViewBag.ModifyBy = new SelectList(db.PersonalDetails, "ID", "FirstName", personaldetail.ModifyBy);
                ViewBag.PincodeID = new SelectList(db.Pincodes, "ID", "Name", personaldetail.PincodeID);
                ViewBag.SalutationID = new SelectList(db.Salutations, "ID", "Name", personaldetail.SalutationID);
                ViewBag.UserLoginID = new SelectList(db.UserLogins, "ID", "Mobile", personaldetail.UserLoginID);
            }
            //return View(personaldetail);
            return View();
        }

        // GET: /PersonalDetail/Delete/5
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PersonalDetail personaldetail = db.PersonalDetails.Find(id);
            if (personaldetail == null)
            {
                return HttpNotFound();
            }
            return View(personaldetail);
        }

        // POST: /PersonalDetail/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            PersonalDetail personaldetail = db.PersonalDetails.Find(id);
            db.PersonalDetails.Remove(personaldetail);
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

        public ActionResult GetAddress(string Pincode)
        {

            if (!(db.Pincodes.Any(p => p.Name == Pincode)))
            {
                //var errorMsg = "Pincode Dose Not Exist, Please Contact Admin!";
                //return View(new { success = false, Error = errorMsg });
                return Json("1", JsonRequestBehavior.AllowGet);
            }


            return Json("0", JsonRequestBehavior.AllowGet);
        }

    }
}
