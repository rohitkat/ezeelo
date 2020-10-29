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
using Inventory.Models;
using BusinessLogicLayer;

namespace Inventory.Controllers
{
    public class PersonalDetailController : Controller
    {
        //
        // GET: /PersonalDetail/
        #region Genral Code
        private EzeeloDBContext db = new EzeeloDBContext();


        private long GetPersonalDetailID()
        {
            //Session["USER_LOGIN_ID"] = 1;
            long UserLoginID = Convert.ToInt32(Session["USER_LOGIN_ID"]);
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
        [SessionExpire]
        public ActionResult Edit1()
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
            ViewBag.DOB1 = Convert.ToDateTime(personaldetail.DOB).ToString("dd/MM/yyyy");
            //ViewBag.DOB1 = "";
            //if (personaldetail.DOB != null)
            //{
            //    ViewBag.DOB1 = Convert.ToDateTime(personaldetail.DOB).ToString("dd/MM/yyyy");
            //}
            return View(personaldetail);
        }

        [HttpPost]
        [SessionExpire]
        [ValidateAntiForgeryToken]
        public ActionResult Edit1(string Pincode, [Bind(Include = "ID,UserLoginID,SalutationID,FirstName,MiddleName,LastName,DOB,Gender,PincodeID,Address,AlternateMobile,AlternateEmail,IsActive,CreateDate,CreateBy,ModifyDate,ModifyBy,NetworkIP,DeviceType,DeviceID")] PersonalDetail personaldetail, string DOB1)
        {

            try
            {

                DateTime lDisplayProductFromDate = DateTime.Now;
                if (DOB1 != "")
                {
                    //if (DateTime.TryParse(DOB1, out lDisplayProductFromDate)) { }
                    //personaldetail.DOB = lDisplayProductFromDate;
                    lDisplayProductFromDate = CommonFunctions.GetProperDateTime(DOB1);
                    personaldetail.DOB = lDisplayProductFromDate;
                }

                ViewBag.DOB1 = Convert.ToDateTime(personaldetail.DOB).ToString("dd/MM/yyyy");

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

                PersonalDetail pd = db.PersonalDetails.Find(personaldetail.ID);

                pd.UserLoginID = personaldetail.UserLoginID;
                pd.SalutationID = personaldetail.SalutationID;
                pd.FirstName = personaldetail.FirstName;
                pd.MiddleName = personaldetail.MiddleName;
                pd.LastName = personaldetail.LastName;
                pd.DOB = personaldetail.DOB;
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
                pd.CreateDate = personaldetail.CreateDate;
                pd.CreateBy = personaldetail.CreateBy;
                pd.ModifyDate = DateTime.UtcNow;
                pd.ModifyBy = PersonalDetailID;
                pd.NetworkIP = BusinessLogicLayer.CommonFunctions.GetClientIP();
                pd.DeviceType = "x";
                pd.DeviceID = "x";
                db.SaveChanges();
                ViewBag.Message = "Record Saved Successfully.";
                //return View(personaldetail);
                return View();
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
        [SessionExpire]
        [Authorize(Roles = "PersonalDetail/CanRead")]
        public ActionResult Edit()
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
            ViewBag.DOB1 = Convert.ToDateTime(personaldetail.DOB).ToString("dd/MM/yyyy");
            //ViewBag.DOB1 = "";
            //if (personaldetail.DOB != null)
            //{
            //    ViewBag.DOB1 =Convert.ToDateTime( personaldetail.DOB).ToString("dd/MM/yyyy");
            //}
            return View(personaldetail);
        }

        // POST: /PersonalDetail/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [SessionExpire]
        [Authorize(Roles = "PersonalDetail/CanWrite")]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(string Pincode, [Bind(Include = "ID,UserLoginID,SalutationID,FirstName,MiddleName,LastName,DOB,Gender,PincodeID,Address,AlternateMobile,AlternateEmail,IsActive,CreateDate,CreateBy,ModifyDate,ModifyBy,NetworkIP,DeviceType,DeviceID")] PersonalDetail personaldetail, string DOB1)
        {

            try
            {

                DateTime lDisplayProductFromDate = DateTime.Now;
                if (DOB1 != "")
                {
                    //if (DateTime.TryParse(DOB1, out lDisplayProductFromDate)) { }
                    //personaldetail.DOB = lDisplayProductFromDate;
                    lDisplayProductFromDate = CommonFunctions.GetProperDateTime(DOB1);
                    personaldetail.DOB = lDisplayProductFromDate;
                }
                ViewBag.DOB1 = Convert.ToDateTime(personaldetail.DOB).ToString("dd/MM/yyyy");

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

                PersonalDetail pd = db.PersonalDetails.Find(personaldetail.ID);

                pd.UserLoginID = personaldetail.UserLoginID;
                pd.SalutationID = personaldetail.SalutationID;
                pd.FirstName = personaldetail.FirstName;
                pd.MiddleName = personaldetail.MiddleName;
                pd.LastName = personaldetail.LastName;
                pd.DOB = personaldetail.DOB;
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
                pd.CreateDate = personaldetail.CreateDate;
                pd.CreateBy = personaldetail.CreateBy;
                pd.ModifyDate = DateTime.UtcNow;
                pd.ModifyBy = PersonalDetailID;
                pd.NetworkIP = BusinessLogicLayer.CommonFunctions.GetClientIP();
                pd.DeviceType = "x";
                pd.DeviceID = "x";
                db.SaveChanges();
                ViewBag.Message = "Record Saved Successfully.";
                //return View(personaldetail);
                return View();
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