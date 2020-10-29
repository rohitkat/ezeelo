using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ModelLayer.Models.ViewModel;
using ModelLayer.Models;
using BusinessLogicLayer;
using System.Transactions;
using System.Data.Entity.Validation;

namespace Franchise.Controllers
{
    public class ChannelPartnerRegistrationController : Controller
    {
        private EzeeloDBContext db = new EzeeloDBContext();

        private long GetPersonalDetailID()
        {
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

        // GET: /ChannelPartnerRegistration/
        public ActionResult Index()
        {
            try
            {
                long franchiseID = Convert.ToInt64(Session["FRANCHISE_ID"]);//GetFranchiseID();

                var lChanalPartner = (from ul in db.UserLogins
                            join bd in db.BusinessDetails on ul.ID equals bd.UserLoginID
                            join pd in db.PersonalDetails on ul.ID equals pd.UserLoginID
                            join ch in db.ChannelPartners on bd.ID equals ch.BusinessDetailID
                            //where bd.BusinessType.Prefix == "GBMR" && ul.IsLocked == true && s.FranchiseID == franchiseID
                             where bd.BusinessType.Prefix == "GBCP" && ch.FranchiseID == franchiseID
                            select new MerchantPendingApprovalViewModel
                            {
                                ID = pd.ID,
                                UserLoginID = bd.UserLoginID,
                                BusinessTypePrefix = bd.BusinessType.Prefix,
                                Name = pd.Salutation.Name + " " + pd.FirstName + " " + pd.LastName,
                                OwnerId = ch.ID,
                               // ShopName = ch.Name,
                                mobile = ul.Mobile,
                                Email = ul.Email,
                                IsLock = ul.IsLocked,
                            }).Distinct().OrderBy(x => x.Name);

                return View(lChanalPartner);
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[MerchantApprovalController][GET:Index]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Franchise, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[MerchantApprovalController][GET:Index]",
                    BusinessLogicLayer.ErrorLog.Module.Franchise, System.Web.HttpContext.Current.Server);
            }
            return View();

        }

        // GET: /ChannelPartnerRegistration/Details/5
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ChannelPartnerRegistration channelpartnerregistration = db.ChannelPartnerRegistration.Find(id);
            if (channelpartnerregistration == null)
            {
                return HttpNotFound();
            }
            return View(channelpartnerregistration);
        }

        // GET: /ChannelPartnerRegistration/Create
        public ActionResult Create()
        {
            List<GenderTypeViewModel> GenderTypeViewModels = new List<GenderTypeViewModel>();
            GenderTypeViewModels.Add(new GenderTypeViewModel { ID = 1, Name = "Male" });
            GenderTypeViewModels.Add(new GenderTypeViewModel { ID = 2, Name = "Female" });

            ViewBag.Gender = new SelectList(GenderTypeViewModels, "Name", "Name");
            ViewBag.SalutationID = new SelectList(db.Salutations, "ID", "Name");
            ViewBag.BankID = new SelectList(db.Banks.Where(x => x.IsActive == true).OrderBy(x => x.Name).ToList(), "ID", "Name");
            ViewBag.BankAccountTypeID = new SelectList(db.BankAccountTypes, "ID", "Name");

            return View();
        }

        // POST: /ChannelPartnerRegistration/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ChannelPartnerRegistration channelpartnerregistration, string Pincode, string DOB1, string SalutationID, string BankID, string BankAccountTypeID)
        //public ActionResult Create([Bind(Include = "SalutationID,FirstName,MiddleName,LastName,DOB,Gender,PincodeID,Address,Mobile,Email,Name,Landline1,FAX,Website,IsActive,IsCODAllowed,MinimumCODRange,BankID,AccountName,BranchName,IFSCCode,MICRCode,AccountNumber,BankAccountTypeID")] ChannelPartnerRegistration channelpartnerregistration, string Pincode, string DOB1)
        {

            List<GenderTypeViewModel> GenderTypeViewModels = new List<GenderTypeViewModel>();
            GenderTypeViewModels.Add(new GenderTypeViewModel { ID = 1, Name = "Male" });
            GenderTypeViewModels.Add(new GenderTypeViewModel { ID = 2, Name = "Female" });
            ViewBag.Gender = new SelectList(GenderTypeViewModels, "Name", "Name");
            ViewBag.SalutationID = new SelectList(db.Salutations, "ID", "Name");
            ViewBag.BankID = new SelectList(db.Banks.Where(x => x.IsActive == true).OrderBy(x => x.Name).ToList(), "ID", "Name");
            ViewBag.BankAccountTypeID = new SelectList(db.BankAccountTypes, "ID", "Name");

            long PersonalDetailID = GetPersonalDetailID();
            var pin = db.Pincodes.Where(x => x.Name == Pincode).FirstOrDefault();

            if (pin == null)
            {
                throw new Exception("Pincod is not valid or does not exist");
            }
            try
            {

                using (TransactionScope ts = new TransactionScope())
                {
                    UserLogin userLogin = new UserLogin();
                    userLogin.Mobile = channelpartnerregistration.userLogin.Mobile;
                    userLogin.Email = channelpartnerregistration.userLogin.Email;
                    userLogin.Password = GenerateRandomString();
                    userLogin.IsLocked = false;
                    userLogin.CreateDate = DateTime.UtcNow;
                    userLogin.CreateBy = PersonalDetailID;
                    userLogin.NetworkIP = BusinessLogicLayer.CommonFunctions.GetClientIP();
                    db.UserLogins.Add(userLogin);
                    db.SaveChanges();

                    PersonalDetail personalDetail = new PersonalDetail();
                    personalDetail.UserLoginID = userLogin.ID;
                    personalDetail.SalutationID = Convert.ToInt32(SalutationID);// channelpartnerregistration.personalDetail.SalutationID;
                    personalDetail.FirstName = channelpartnerregistration.personalDetail.FirstName;
                    personalDetail.MiddleName = channelpartnerregistration.personalDetail.MiddleName;
                    personalDetail.LastName = channelpartnerregistration.personalDetail.LastName;

                    DateTime? lDOB = CommonFunctions.GetDate(DOB1);
                    if (lDOB != null)
                    { channelpartnerregistration.personalDetail.DOB = lDOB; }
                    ViewBag.DOB1 = channelpartnerregistration.personalDetail.DOB.ToString();
                    if (channelpartnerregistration.personalDetail.DOB > DateTime.Now)
                    {
                        ModelState.AddModelError("CustomError", "Date of birth can't be in future");
                    }
                    personalDetail.DOB = channelpartnerregistration.personalDetail.DOB;
                    personalDetail.Gender = channelpartnerregistration.personalDetail.Gender;
                    personalDetail.PincodeID = pin.ID;
                    personalDetail.Address = channelpartnerregistration.personalDetail.Address;
                    personalDetail.AlternateMobile = channelpartnerregistration.userLogin.Mobile;
                    personalDetail.AlternateEmail = channelpartnerregistration.userLogin.Email;
                    personalDetail.IsActive = true;
                    personalDetail.CreateDate = DateTime.UtcNow;
                    personalDetail.CreateBy = PersonalDetailID;
                    personalDetail.NetworkIP = BusinessLogicLayer.CommonFunctions.GetClientIP();
                    //personalDetail.DeviceType = "x";
                    //personalDetail.DeviceID = "x";
                    db.PersonalDetails.Add(personalDetail);
                    db.SaveChanges();

                    
                    long lbusinessTypeID = db.BusinessTypes.Where(x => x.Prefix == "GBCP").Select(x => x.ID).FirstOrDefault();

                    BusinessDetail bussinesDetails = new BusinessDetail();
                    bussinesDetails.UserLoginID = userLogin.ID;
                    bussinesDetails.Name = channelpartnerregistration.businessDetail.Name;
                    bussinesDetails.BusinessTypeID = Convert.ToInt32(lbusinessTypeID);
                    bussinesDetails.ContactPerson = channelpartnerregistration.personalDetail.FirstName + " " + channelpartnerregistration.personalDetail.LastName; //merchantRegisterViewModel.businessDetail.ContactPerson;
                    bussinesDetails.Mobile = channelpartnerregistration.userLogin.Mobile;
                    bussinesDetails.Email = channelpartnerregistration.userLogin.Email;
                    bussinesDetails.Landline1 = channelpartnerregistration.businessDetail.Landline1;
                    bussinesDetails.FAX = channelpartnerregistration.businessDetail.FAX;
                    bussinesDetails.Address = channelpartnerregistration.personalDetail.Address;
                    bussinesDetails.Website = channelpartnerregistration.businessDetail.Website;
                    bussinesDetails.PincodeID = pin.ID;
                    bussinesDetails.YearOfEstablishment = DateTime.UtcNow;
                    bussinesDetails.IsActive = true;
                    bussinesDetails.CreateBy = PersonalDetailID;
                    bussinesDetails.CreateDate = DateTime.UtcNow;
                    bussinesDetails.NetworkIP = BusinessLogicLayer.CommonFunctions.GetClientIP();
                    //bussinesDetails.DeviceID = "x";
                    //bussinesDetails.DeviceType = "x";
                    db.BusinessDetails.Add(bussinesDetails);
                    db.SaveChanges();

                    long lbusinessDetailID = db.BusinessDetails.Where(x => x.UserLoginID == userLogin.ID).Select(x => x.ID).FirstOrDefault();

                    ChannelPartner channelPartner = new ChannelPartner();
                    channelPartner.BusinessDetailID = Convert.ToInt32(lbusinessDetailID);
                    channelPartner.ContactPerson = channelpartnerregistration.personalDetail.FirstName + " " + channelpartnerregistration.personalDetail.LastName; //merchantRegisterViewModel.businessDetail.ContactPerson;
                    channelPartner.Mobile = channelpartnerregistration.userLogin.Mobile;
                    channelPartner.Email = channelpartnerregistration.userLogin.Email;
                    channelPartner.Landline = channelpartnerregistration.businessDetail.Landline1;
                    channelPartner.FAX = channelpartnerregistration.businessDetail.FAX;
                    channelPartner.Address = channelpartnerregistration.personalDetail.Address;
                    channelPartner.PincodeID = pin.ID;
                    channelPartner.IsActive = true;
                    channelPartner.IsCODAllowed = channelpartnerregistration.channelPartner.IsCODAllowed;
                    channelPartner.MinimumCODRange = channelpartnerregistration.channelPartner.MinimumCODRange;
                    channelPartner.FranchiseID = Convert.ToInt32(Session["FRANCHISE_ID"]);
                    channelPartner.CreateBy = PersonalDetailID;
                    channelPartner.CreateDate = DateTime.UtcNow;
                    channelPartner.NetworkIP = BusinessLogicLayer.CommonFunctions.GetClientIP();
                    //channelPartner.DeviceID = "x";
                    //channelPartner.DeviceType = "x";
                    db.ChannelPartners.Add(channelPartner);
                    db.SaveChanges();

                    OwnerBank ownerBank = new OwnerBank();
                    ownerBank.BusinessTypeID = Convert.ToInt32(lbusinessTypeID);
                    ownerBank.OwnerID = channelPartner.ID;
                    ownerBank.AccountName = channelpartnerregistration.ownerBank.AccountName;
                    ownerBank.BankID = Convert.ToInt32(BankID);//channelpartnerregistration.ownerBank.BankID;
                    ownerBank.BranchName = channelpartnerregistration.ownerBank.BranchName;
                    ownerBank.IFSCCode = channelpartnerregistration.ownerBank.IFSCCode;
                    ownerBank.MICRCode = channelpartnerregistration.ownerBank.MICRCode;
                    ownerBank.AccountNumber = channelpartnerregistration.ownerBank.AccountNumber;
                    ownerBank.BankAccountTypeID = Convert.ToInt32(BankAccountTypeID);// channelpartnerregistration.ownerBank.BankAccountTypeID;
                    ownerBank.IsActive = true;
                    ownerBank.CreateBy = PersonalDetailID;
                    ownerBank.CreateDate = DateTime.UtcNow;
                    ownerBank.NetworkIP = BusinessLogicLayer.CommonFunctions.GetClientIP();
                    //ownerBank.DeviceID = "x";
                    //ownerBank.DeviceType = "x";
                    db.OwnerBanks.Add(ownerBank);
                    db.SaveChanges();

                    ViewBag.Message = "Record Saved Successfully.";
                    ts.Complete();
                }
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
                return View(channelpartnerregistration);
            }

            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Error", "There's Something wrong with the Product upload!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[ProductUploadTemp][POST:Create]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);

            }
            catch (Exception ex)
            {

                ModelState.AddModelError("Error", "There's Something wrong with the Product upload!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[ProductUploadTemp][POST:Create]",
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);

            }

            return RedirectToAction("Index");
        }

        // GET: /ChannelPartnerRegistration/Edit/5
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ChannelPartnerRegistration channelpartnerregistration = new ChannelPartnerRegistration();

            UserLogin UL = db.UserLogins.Find(id);
            channelpartnerregistration.userLogin = UL;

            long personalDetailID = db.PersonalDetails.Where(x => x.UserLoginID == UL.ID).Select(x => x.ID).FirstOrDefault();

            PersonalDetail PD = db.PersonalDetails.Find(personalDetailID);
            channelpartnerregistration.personalDetail = PD;

            long bussinesDetailsID = db.BusinessDetails.Where(x => x.UserLoginID == UL.ID).Select(x => x.ID).FirstOrDefault();

            BusinessDetail BD = db.BusinessDetails.Find(bussinesDetailsID);
            channelpartnerregistration.businessDetail = BD;

            long channelPartnerID = db.ChannelPartners.Where(x => x.BusinessDetailID == BD.ID).Select(x => x.ID).FirstOrDefault();

            ChannelPartner CP = db.ChannelPartners.Find(channelPartnerID);
            channelpartnerregistration.channelPartner = CP;

            long ownerBankID = db.OwnerBanks.Where(x => x.OwnerID == CP.ID && x.BusinessType.Prefix == "GBCP").Select(x => x.ID).FirstOrDefault();

            OwnerBank OB = db.OwnerBanks.Find(ownerBankID);
            channelpartnerregistration.ownerBank = OB;

            //List<GenderTypeViewModel> GenderTypeViewModels = new List<GenderTypeViewModel>();
            //GenderTypeViewModels.Add(new GenderTypeViewModel { ID = 1, Name = "Male" });
            //GenderTypeViewModels.Add(new GenderTypeViewModel { ID = 2, Name = "Female" });
            //ViewBag.Gender = new SelectList(GenderTypeViewModels, "Name", "Name", channelpartnerregistration.personalDetail.Gender);
            ViewBag.SalutationID = new SelectList(db.Salutations, "ID", "Name", channelpartnerregistration.personalDetail.SalutationID);
            ViewBag.BankID = new SelectList(db.Banks.Where(x => x.IsActive == true).OrderBy(x => x.Name).ToList(), "ID", "Name", channelpartnerregistration.ownerBank.BankID);
            ViewBag.BankAccountTypeID = new SelectList(db.BankAccountTypes, "ID", "Name", channelpartnerregistration.ownerBank.BankAccountTypeID);
            Pincode lPincode = db.Pincodes.Find(channelpartnerregistration.personalDetail.PincodeID);
            if (lPincode != null)
            {
                ViewBag.Pincode = lPincode.Name;
            }
            List<GenderTypeViewModel> GenderTypeViewModels = new List<GenderTypeViewModel>();
            GenderTypeViewModels.Add(new GenderTypeViewModel { ID = 1, Name = "Male" });
            GenderTypeViewModels.Add(new GenderTypeViewModel { ID = 2, Name = "Female" });
            if (channelpartnerregistration.personalDetail.Gender != null)
            {
                ViewBag.Gender = new SelectList(GenderTypeViewModels, "Name", "Name", channelpartnerregistration.personalDetail.Gender.Trim());
            }
            else
            {
                ViewBag.Gender = new SelectList(GenderTypeViewModels, "Name", "Name");
            }
            ViewBag.DOB1 = channelpartnerregistration.personalDetail.DOB.ToString();
         
            return View(channelpartnerregistration);
        }

        // POST: /ChannelPartnerRegistration/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
       // public ActionResult Edit([Bind(Include = "ID")] ChannelPartnerRegistration channelpartnerregistration)
        public ActionResult Edit(ChannelPartnerRegistration channelpartnerregistration, string Pincode, string DOB1, string SalutationID, string BankID, string BankAccountTypeID)
        {
            List<GenderTypeViewModel> GenderTypeViewModels = new List<GenderTypeViewModel>();
            GenderTypeViewModels.Add(new GenderTypeViewModel { ID = 1, Name = "Male" });
            GenderTypeViewModels.Add(new GenderTypeViewModel { ID = 2, Name = "Female" });
            ViewBag.Gender = new SelectList(GenderTypeViewModels, "Name", "Name");
            ViewBag.SalutationID = new SelectList(db.Salutations, "ID", "Name");
            ViewBag.BankID = new SelectList(db.Banks.Where(x => x.IsActive == true).OrderBy(x => x.Name).ToList(), "ID", "Name");
            ViewBag.BankAccountTypeID = new SelectList(db.BankAccountTypes, "ID", "Name");

            long PersonalDetailID = GetPersonalDetailID();
            var pin = db.Pincodes.Where(x => x.Name == Pincode).FirstOrDefault();

            if (pin == null)
            {
                throw new Exception("Pincod is not valid or does not exist");
            }
            try
            {

                using (TransactionScope ts = new TransactionScope())
                {
                    UserLogin userLogin = db.UserLogins.Find(channelpartnerregistration.ID);
                    userLogin.Mobile = channelpartnerregistration.userLogin.Mobile;
                    userLogin.Email = channelpartnerregistration.userLogin.Email;
                    userLogin.Password = GenerateRandomString();
                    userLogin.IsLocked = false;
                    userLogin.ModifyDate = DateTime.UtcNow;
                    userLogin.ModifyBy = PersonalDetailID;
                    userLogin.NetworkIP = BusinessLogicLayer.CommonFunctions.GetClientIP();
                    db.SaveChanges();

                    long personalDetailID = db.PersonalDetails.Where(x => x.UserLoginID == channelpartnerregistration.ID).Select(x => x.ID).FirstOrDefault();

                    PersonalDetail personalDetail = db.PersonalDetails.Find(personalDetailID);
                    personalDetail.UserLoginID = userLogin.ID;
                    personalDetail.SalutationID = Convert.ToInt32(SalutationID);// channelpartnerregistration.personalDetail.SalutationID;
                    personalDetail.FirstName = channelpartnerregistration.personalDetail.FirstName;
                    personalDetail.MiddleName = channelpartnerregistration.personalDetail.MiddleName;
                    personalDetail.LastName = channelpartnerregistration.personalDetail.LastName;

                    DateTime? lDOB = CommonFunctions.GetDate(DOB1);
                    if (lDOB != null)
                    { channelpartnerregistration.personalDetail.DOB = lDOB; }
                    ViewBag.DOB1 = channelpartnerregistration.personalDetail.DOB.ToString();
                    if (channelpartnerregistration.personalDetail.DOB > DateTime.Now)
                    {
                        ModelState.AddModelError("CustomError", "Date of birth can't be in future");
                    }
                    personalDetail.DOB = channelpartnerregistration.personalDetail.DOB;
                    personalDetail.Gender = channelpartnerregistration.personalDetail.Gender;
                    personalDetail.PincodeID = pin.ID;
                    personalDetail.Address = channelpartnerregistration.personalDetail.Address;
                    personalDetail.AlternateMobile = channelpartnerregistration.userLogin.Mobile;
                    personalDetail.AlternateEmail = channelpartnerregistration.userLogin.Email;
                    personalDetail.IsActive = true;
                    personalDetail.ModifyDate = DateTime.UtcNow;
                    personalDetail.ModifyBy = PersonalDetailID;
                    personalDetail.NetworkIP = BusinessLogicLayer.CommonFunctions.GetClientIP();
                    db.SaveChanges();

                    long bussinesDetailsID = db.BusinessDetails.Where(x => x.UserLoginID == channelpartnerregistration.ID).Select(x => x.ID).FirstOrDefault();
                    long lbusinessTypeID = db.BusinessTypes.Where(x => x.Prefix == "GBCP").Select(x => x.ID).FirstOrDefault();

                    BusinessDetail bussinesDetails = db.BusinessDetails.Find(bussinesDetailsID);
                    bussinesDetails.UserLoginID = userLogin.ID;
                    bussinesDetails.Name = channelpartnerregistration.businessDetail.Name;
                    bussinesDetails.BusinessTypeID = Convert.ToInt32(lbusinessTypeID);
                    bussinesDetails.ContactPerson = channelpartnerregistration.personalDetail.FirstName + " " + channelpartnerregistration.personalDetail.LastName; //merchantRegisterViewModel.businessDetail.ContactPerson;
                    bussinesDetails.Mobile = channelpartnerregistration.userLogin.Mobile;
                    bussinesDetails.Email = channelpartnerregistration.userLogin.Email;
                    bussinesDetails.Landline1 = channelpartnerregistration.businessDetail.Landline1;
                    bussinesDetails.FAX = channelpartnerregistration.businessDetail.FAX;
                    bussinesDetails.Address = channelpartnerregistration.personalDetail.Address;
                    bussinesDetails.Website = channelpartnerregistration.businessDetail.Website;
                    bussinesDetails.PincodeID = pin.ID;
                    bussinesDetails.YearOfEstablishment = DateTime.UtcNow;
                    bussinesDetails.IsActive = true;
                    bussinesDetails.ModifyBy = PersonalDetailID;
                    bussinesDetails.ModifyDate = DateTime.UtcNow;
                    bussinesDetails.NetworkIP = BusinessLogicLayer.CommonFunctions.GetClientIP();
                    db.SaveChanges();

                    long channelPartnerID = db.ChannelPartners.Where(x => x.BusinessDetailID == bussinesDetails.ID).Select(x => x.ID).FirstOrDefault();
                    long lbusinessDetailID = db.BusinessDetails.Where(x => x.UserLoginID == userLogin.ID && x.BusinessType.Prefix == "GBCP").Select(x => x.ID).FirstOrDefault();

                    ChannelPartner channelPartner = db.ChannelPartners.Find(channelPartnerID);
                    channelPartner.BusinessDetailID = Convert.ToInt32(lbusinessDetailID);
                    channelPartner.ContactPerson = channelpartnerregistration.personalDetail.FirstName + " " + channelpartnerregistration.personalDetail.LastName; //merchantRegisterViewModel.businessDetail.ContactPerson;
                    channelPartner.Mobile = channelpartnerregistration.userLogin.Mobile;
                    channelPartner.Email = channelpartnerregistration.userLogin.Email;
                    channelPartner.Landline = channelpartnerregistration.businessDetail.Landline1;
                    channelPartner.FAX = channelpartnerregistration.businessDetail.FAX;
                    channelPartner.Address = channelpartnerregistration.personalDetail.Address;
                    channelPartner.PincodeID = pin.ID;
                    channelPartner.IsActive = true;
                    channelPartner.IsCODAllowed = channelpartnerregistration.channelPartner.IsCODAllowed;
                    channelPartner.MinimumCODRange = channelpartnerregistration.channelPartner.MinimumCODRange;
                    channelPartner.ModifyBy = PersonalDetailID;
                    channelPartner.ModifyDate = DateTime.UtcNow;
                    channelPartner.NetworkIP = BusinessLogicLayer.CommonFunctions.GetClientIP();
                    db.SaveChanges();

                    long ownerBankID = db.OwnerBanks.Where(x => x.OwnerID == channelPartner.ID && x.BusinessType.Prefix=="GBCP").Select(x => x.ID).FirstOrDefault();

                    OwnerBank ownerBank = db.OwnerBanks.Find(ownerBankID);
                    ownerBank.BusinessTypeID = Convert.ToInt32(lbusinessTypeID);
                    ownerBank.OwnerID = channelPartner.ID;
                    ownerBank.AccountName = channelpartnerregistration.ownerBank.AccountName;
                    ownerBank.BankID = Convert.ToInt32(BankID);//channelpartnerregistration.ownerBank.BankID;
                    ownerBank.BranchName = channelpartnerregistration.ownerBank.BranchName;
                    ownerBank.IFSCCode = channelpartnerregistration.ownerBank.IFSCCode;
                    ownerBank.MICRCode = channelpartnerregistration.ownerBank.MICRCode;
                    ownerBank.AccountNumber = channelpartnerregistration.ownerBank.AccountNumber;
                    ownerBank.BankAccountTypeID = Convert.ToInt32(BankAccountTypeID);// channelpartnerregistration.ownerBank.BankAccountTypeID;
                    ownerBank.IsActive = true;
                    ownerBank.ModifyBy = PersonalDetailID;
                    ownerBank.ModifyDate = DateTime.UtcNow;
                    ownerBank.NetworkIP = BusinessLogicLayer.CommonFunctions.GetClientIP();
                    db.SaveChanges();

                    ViewBag.Message = "Record Updated Successfully.";
                    ts.Complete();
                }
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
                return View(channelpartnerregistration);
            }

            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Error", "There's Something wrong with the Product upload!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[ProductUploadTemp][POST:Create]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);

            }
            catch (Exception ex)
            {

                ModelState.AddModelError("Error", "There's Something wrong with the Product upload!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[ProductUploadTemp][POST:Create]",
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);

            }

            return RedirectToAction("Index");
        }

        // GET: /ChannelPartnerRegistration/Delete/5
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ChannelPartnerRegistration channelpartnerregistration = db.ChannelPartnerRegistration.Find(id);
            if (channelpartnerregistration == null)
            {
                return HttpNotFound();
            }
            return View(channelpartnerregistration);
        }

        // POST: /ChannelPartnerRegistration/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            ChannelPartnerRegistration channelpartnerregistration = db.ChannelPartnerRegistration.Find(id);
            db.ChannelPartnerRegistration.Remove(channelpartnerregistration);
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

                return Json("1", JsonRequestBehavior.AllowGet);
            }

            return Json("0", JsonRequestBehavior.AllowGet);
        }

        private string GenerateRandomString()
        {
            Random rnd = new Random();
            int month = rnd.Next(1, 13); // creates a number between 1 and 12
            int dice = rnd.Next(1, 7);   // creates a number between 1 and 6
            int card = rnd.Next(52);
            return month.ToString("0") + dice.ToString("0") + card.ToString("0");
        }
    }
}
