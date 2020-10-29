using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ModelLayer.Models.ViewModel;
using ModelLayer.Models;
using System.Text.RegularExpressions;
using System.Web.Security;
using BusinessLogicLayer;
using System.Data;


namespace Franchise.Controllers
{
    public class FranchiseRegisterController : Controller
    {
        //
        // GET: /FranchiseRegister/
        private EzeeloDBContext db = new EzeeloDBContext();

        // GET: /FranchiseRegister/
        public ActionResult Index()
        {
            try
            {
                ViewBag.SalutationID = new SelectList(db.Salutations, "ID", "Name", 1);
                ViewBag.SecurityQuestionID = new SelectList(db.SecurityQuestions, "ID", "Question");
                TempData["ReturnUrl"] = Request.QueryString["returnurl"];
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[FranchiseRegisterController][GET:Index]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Franchise, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[FranchiseRegisterController][GET:Index]",
                    BusinessLogicLayer.ErrorLog.Module.Franchise, System.Web.HttpContext.Current.Server);
            }
            return View();
        }

        [HttpPost]
        public ActionResult Index(FranchiseRegisterViewModel franchiseRegisterViewModel, string Mobile)
        {
            try
            {
                var pin = db.Pincodes.Where(x => x.Name == franchiseRegisterViewModel.MyPincode).FirstOrDefault();

                if (pin == null)
                {
                    ModelState.AddModelError("CustomError", "Pincod is not valid or does not exist");
                }
                else if (Mobile == null)
                {
                    ModelState.AddModelError("CustomError", "Mobile No. is not valid");
                }
                else
                {
                    franchiseRegisterViewModel.pincode = pin;

                    ViewBag.SalutationID = new SelectList(db.Salutations, "ID", "Name", franchiseRegisterViewModel.SalutationID);
                    ViewBag.SecurityQuestionID = new SelectList(db.SecurityQuestions, "ID", "Question", franchiseRegisterViewModel.SecurityQuestionID);
                    if (ModelState.IsValid)
                    {
                        using (var dbContextTransaction = db.Database.BeginTransaction())
                        {
                            UserLogin userLogin = new UserLogin();
                            userLogin.Mobile = Mobile;// merchantRegisterViewModel.userLogin.Mobile;
                            userLogin.Email = franchiseRegisterViewModel.userLogin.Email;
                            userLogin.Password = franchiseRegisterViewModel.userLogin.Password;
                            userLogin.IsLocked = true;
                            userLogin.CreateDate = DateTime.UtcNow;
                            userLogin.CreateBy = 1;
                            userLogin.NetworkIP = BusinessLogicLayer.CommonFunctions.GetClientIP();
                            userLogin.DeviceType = "x";
                            userLogin.DeviceID = "x";
                            db.UserLogins.Add(userLogin);
                            //db.SaveChanges();

                            //ModelLayer.Models.Pincode pin = new ModelLayer.Models.Pincode();

                            PersonalDetail personalDetail = new PersonalDetail();
                            personalDetail.UserLoginID = userLogin.ID;
                            personalDetail.SalutationID = franchiseRegisterViewModel.SalutationID;
                            personalDetail.FirstName = franchiseRegisterViewModel.personalDetail.FirstName;
                            personalDetail.MiddleName = franchiseRegisterViewModel.personalDetail.MiddleName;
                            personalDetail.LastName = franchiseRegisterViewModel.personalDetail.LastName;
                            personalDetail.DOB = franchiseRegisterViewModel.personalDetail.DOB;
                            personalDetail.Gender = franchiseRegisterViewModel.personalDetail.Gender;
                            personalDetail.PincodeID = pin.ID; // merchantRegisterViewModel.personalDetail.PincodeID;
                            personalDetail.Address = franchiseRegisterViewModel.personalDetail.Address;
                            personalDetail.AlternateMobile = franchiseRegisterViewModel.personalDetail.AlternateMobile;
                            personalDetail.AlternateEmail = franchiseRegisterViewModel.personalDetail.AlternateEmail;
                            personalDetail.IsActive = true;
                            personalDetail.CreateDate = DateTime.UtcNow;
                            personalDetail.CreateBy = 1;
                            personalDetail.NetworkIP = BusinessLogicLayer.CommonFunctions.GetClientIP();
                            personalDetail.DeviceType = "x";
                            personalDetail.DeviceID = "x";
                            db.PersonalDetails.Add(personalDetail);
                            //db.SaveChanges();

                            BusinessDetail bussinesDetails = new BusinessDetail();
                            bussinesDetails.UserLoginID = userLogin.ID;
                            bussinesDetails.Name = string.Empty;// merchantRegisterViewModel.businessDetail.Name;
                            bussinesDetails.BusinessTypeID = db.BusinessTypes.Where(x => x.Prefix == "GBFR").Select(x => x.ID).FirstOrDefault(); //merchantRegisterViewModel.businessDetail.BusinessTypeID;
                            bussinesDetails.ContactPerson = franchiseRegisterViewModel.personalDetail.FirstName + " " + franchiseRegisterViewModel.personalDetail.LastName; //merchantRegisterViewModel.businessDetail.ContactPerson;
                            bussinesDetails.Mobile = Mobile;// merchantRegisterViewModel.businessDetail.Mobile;
                            bussinesDetails.Email = franchiseRegisterViewModel.userLogin.Email; //merchantRegisterViewModel.businessDetail.Email;
                            bussinesDetails.PincodeID = pin.ID; //merchantRegisterViewModel.businessDetail.PincodeID;
                            bussinesDetails.IsActive = true;
                            bussinesDetails.CreateBy = 1;
                            bussinesDetails.CreateDate = DateTime.UtcNow;
                            bussinesDetails.NetworkIP = BusinessLogicLayer.CommonFunctions.GetClientIP();
                            bussinesDetails.DeviceID = "x";
                            bussinesDetails.DeviceType = "x";
                            db.BusinessDetails.Add(bussinesDetails);
                            //db.SaveChanges();

                            ModelLayer.Models.Franchise franchise = new ModelLayer.Models.Franchise();
                            franchise.BusinessDetailID = bussinesDetails.ID;
                            franchise.ServiceNumber = franchiseRegisterViewModel.franchise.ServiceNumber;
                            franchise.ContactPerson = string.Empty;
                            franchise.Mobile = string.Empty;
                            franchise.Email = string.Empty;
                            franchise.IsActive = true;
                            franchise.CreateBy = 1;
                            franchise.CreateDate = DateTime.UtcNow;
                            franchise.NetworkIP = BusinessLogicLayer.CommonFunctions.GetClientIP();
                            franchise.DeviceID = string.Empty;
                            franchise.DeviceType = string.Empty;
                            db.Franchises.Add(franchise);

                            db.SaveChanges();
                            dbContextTransaction.Commit();
                            //sneding mail to franchise after registration
                            sendEmail(userLogin.ID);
                            //sneding sms to franchise after registration
                            sendSMS(userLogin.ID);
                            ViewBag.Msg = "Congratulations! You have successfully Registered… We will contact you soon !!";
                            TempData["Msg"] = "Congratulations! You have successfully Registered… We will contact you soon !!";
                            return RedirectToAction("Index", "FranchiseRegister");
                        }
                    }
                    else
                    {
                        var errors = ModelState.Select(x => x.Value.Errors).Where(y => y.Count > 0).ToList();
                        foreach (var e in errors)
                        {
                            ModelState.AddModelError("CustomError", e.ToString());
                        }
                    }
                }
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[FranchiseRegisterController][POST:Index]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Franchise, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[FranchiseRegisterController][POST:Index]",
                    BusinessLogicLayer.ErrorLog.Module.Franchise, System.Web.HttpContext.Current.Server);
            }
            return View();
            //return RedirectToAction("View1", "FranchiseRegister");
            //closing of case save                
        }


        //[HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            Session.Clear();
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "FranchiseRegister");
        }

        [HttpPost]
        public ActionResult Login(FranchiseRegisterViewModel franchiseRegisterViewModel)
        {
            try
            {
                ViewBag.Salutation = new SelectList(db.Salutations.ToList(), "ID", "Name", 1);
                ViewBag.SecurityQuestion = new SelectList(db.SecurityQuestions, "ID", "Question");

                if (franchiseRegisterViewModel.loginViewModel.UserName == null)
                {
                    ViewBag.Message = "Plz fill username!";
                    TempData["Message"] = "Plz fill username!";
                    return RedirectToAction("Index", "FranchiseRegister");
                }

                if (franchiseRegisterViewModel.loginViewModel.Password == null)
                {
                    ViewBag.Message = "Plz fill Password!";
                    TempData["Message"] = "Plz fill Password!";
                    return RedirectToAction("Index", "FranchiseRegister");
                }


                bool IsEmailValid = false, IsMobileValid = false;

                IsEmailValid = this.IsValidEmailId(franchiseRegisterViewModel.loginViewModel.UserName);

                if (IsEmailValid == false)
                    IsMobileValid = this.IsValidMobile(franchiseRegisterViewModel.loginViewModel.UserName);

                if (IsEmailValid == false && IsMobileValid == false)
                {
                    ViewBag.Message = "Invalid UserName/Password!!";
                    TempData["Message"] = "Invalid UserName/Password!!";

                    //return View("Create");
                    return RedirectToAction("Index", "FranchiseRegister");
                }
                else
                {
                    Dictionary<string, string> lDictLoginDetails = this.CheckLogin(franchiseRegisterViewModel.loginViewModel.UserName, franchiseRegisterViewModel.loginViewModel.Password);

                    if (lDictLoginDetails.Count() <= 0)
                    {
                        ViewBag.Message = "Invalid UserName/Password!!";
                        TempData["Message"] = "Invalid UserName/Password!!";

                        //return View("Create");
                        return RedirectToAction("Index", "FranchiseRegister");
                    }
                    else
                    {
                        long UserLoginID = Convert.ToInt64(lDictLoginDetails["ID"].ToString());
                        long businessDetailID = db.BusinessDetails.Where(x => x.UserLoginID == UserLoginID && x.BusinessType.Prefix == "GBFR").Select(x => x.ID).FirstOrDefault();
                        var IsExist = db.Franchises.Where(x => x.BusinessDetailID == businessDetailID && x.IsActive == true).FirstOrDefault();
                        var IsEmployee = db.Employees.Where(x => x.EmployeeCode.StartsWith("GBFR") && x.UserLoginID == UserLoginID && x.IsActive == true).FirstOrDefault();
                        if (IsExist != null)
                        {
                            if (IsPlanValid(UserLoginID, businessDetailID, IsExist))
                            {
                                Session["FRANCHISE_ID"] = IsExist.ID;
                                Session["ID"] = lDictLoginDetails["ID"];
                                Session["USER_LOGIN_ID"] = lDictLoginDetails["ID"];


                                ModelLayer.Models.PersonalDetail pd = new ModelLayer.Models.PersonalDetail();
                                long LoginID = Convert.ToInt64(lDictLoginDetails["ID"].ToString());
                                pd = db.PersonalDetails.Where(x => x.UserLoginID == LoginID).FirstOrDefault();
                                Session["PERSONAL_ID"] = pd == null ? 0 : pd.ID;

                                Session["USER_NAME"] = lDictLoginDetails["UserName"];
                                FormsAuthentication.SetAuthCookie(franchiseRegisterViewModel.loginViewModel.UserName, true);

                                string businessname = db.BusinessDetails.Where(x => x.UserLoginID == LoginID).Select(x => x.Name).FirstOrDefault();
                                int fId = Convert.ToInt32(Session["FRANCHISE_ID"]);
                                string contactPersonName = db.Franchises.Where(x => x.ID == fId).Select(x => x.ContactPerson).FirstOrDefault();

                                //=============== call method to get no. of days remaining for plan expiration =================
                                this.GetRemainingDaysByPlan();


                                string Rurl = Convert.ToString(TempData.Peek("ReturnUrl"));
                                string decodedUrl = "";
                                if (!string.IsNullOrEmpty(Rurl))
                                {
                                    decodedUrl = Server.UrlDecode(Rurl);
                                }

                                if (Url.IsLocalUrl(decodedUrl))
                                {
                                    TempData.Remove("ReturnUrl");
                                    return Redirect(decodedUrl);
                                }
                                else
                                {
                                    if (string.IsNullOrEmpty(businessname) || string.IsNullOrEmpty(contactPersonName))
                                    {
                                        return RedirectToAction("Edit", "PersonalDetail");
                                    }
                                    else
                                    {
                                        return RedirectToAction("Index", "Home");
                                    }
                                }
                                //if (string.IsNullOrEmpty(businessname) || string.IsNullOrEmpty(contactPersonName))
                                //{
                                //    return RedirectToAction("Edit", "PersonalDetail");
                                //}
                                //else
                                //{
                                //    return RedirectToAction("Index", "Home");
                                //}

                              
                                

                            }
                            else
                            {
                                UserLogin ul = db.UserLogins.Find(UserLoginID);
                                ul.IsLocked = true;
                                ul.ModifyDate = DateTime.Now;
                                db.SaveChanges();

                                ViewBag.Message = "Can't Login!! Your Plan is Expired. Please, Contact Administrator...";
                                TempData["Message"] = "Can't Login!! Your Plan is Expired. Please, Contact Administrator...";
                                return RedirectToAction("Index", "FranchiseRegister");
                            }
                        }
                        else if (IsEmployee != null)
                        {
                            long? FranchiseId = db.Employees.Where(x => x.UserLoginID == UserLoginID).Select(x => x.OwnerID).FirstOrDefault();
                            long BID = db.Franchises.Where(x => x.ID == FranchiseId).Select(x => x.BusinessDetailID).FirstOrDefault();
                            long Fran_UID = db.BusinessDetails.Where(x => x.ID == BID).Select(x => x.UserLoginID).FirstOrDefault();
                            if (IsPlanValid(Fran_UID, BID, db.Franchises.Find(FranchiseId)))
                            {
                                Session["FRANCHISE_ID"] = FranchiseId;
                                Session["ID"] = lDictLoginDetails["ID"];
                                Session["USER_LOGIN_ID"] = lDictLoginDetails["ID"];


                                ModelLayer.Models.PersonalDetail pd = new ModelLayer.Models.PersonalDetail();
                                long LoginID = Fran_UID;
                                pd = db.PersonalDetails.Where(x => x.UserLoginID == LoginID).FirstOrDefault();
                                Session["PERSONAL_ID"] = pd == null ? 0 : pd.ID;

                                Session["USER_NAME"] = lDictLoginDetails["UserName"];
                                FormsAuthentication.SetAuthCookie(franchiseRegisterViewModel.loginViewModel.UserName, true);


                                string Rurl = ViewBag.ReturnUrl;
                                string decodedUrl = "";
                                if (!string.IsNullOrEmpty(Rurl))
                                {
                                    decodedUrl = Server.UrlDecode(Rurl);
                                }

                                if (Url.IsLocalUrl(decodedUrl))
                                {
                                    TempData.Remove("ReturnUrl");
                                    return Redirect(decodedUrl);
                                }
                                else
                                {
                                    return RedirectToAction("Index", "Home");
                                }
                                //return RedirectToAction("Index", "Home");
                            }
                            else
                            {
                                UserLogin ul = db.UserLogins.Find(Fran_UID);
                                ul.IsLocked = true;
                                ul.ModifyDate = DateTime.Now;
                                db.SaveChanges();

                                ViewBag.Message = "Can't Login!! Your Plan is Expired. Please, Contact Administrator...";
                                TempData["Message"] = "Can't Login!! Your Plan is Expired. Please, Contact Administrator...";
                                return RedirectToAction("Index", "FranchiseRegister");
                            }
                        }
                        else
                        {
                            ViewBag.Message = "You are not a Partner";
                            TempData["Message"] = "You are not a Partner";

                            //return View("Create");
                            return RedirectToAction("Index", "FranchiseRegister");
                        }


                    }
                }
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[FranchiseRegisterController][POST:Index]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Franchise, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[FranchiseRegisterController][POST:Index]",
                    BusinessLogicLayer.ErrorLog.Module.Franchise, System.Web.HttpContext.Current.Server);
            }
            return View();
            //return RedirectToAction("Index", "MerchantRegister");

        }

        //Added by Priti on Partner direct login  from Inventory on button click on 6-12-2018
        public ActionResult LoginURL(string pUserName, string pPassword)
        {
            try
            {
                ViewBag.Salutation = new SelectList(db.Salutations.ToList(), "ID", "Name", 1);
                ViewBag.SecurityQuestion = new SelectList(db.SecurityQuestions, "ID", "Question");

                if (pUserName == null)
                {
                    ViewBag.Message = "Plz fill username!";
                    TempData["Message"] = "Plz fill username!";
                    return RedirectToAction("Index", "FranchiseRegister");
                }

                if (pPassword == null)
                {
                    ViewBag.Message = "Plz fill Password!";
                    TempData["Message"] = "Plz fill Password!";
                    return RedirectToAction("Index", "FranchiseRegister");
                }

                bool IsEmailValid = false, IsMobileValid = false;

                IsEmailValid = this.IsValidEmailId(pUserName);

                if (IsEmailValid == false)
                    IsMobileValid = this.IsValidMobile(pUserName);

                if (IsEmailValid == false && IsMobileValid == false)
                {
                    ViewBag.Message = "Invalid UserName/Password!!";
                    TempData["Message"] = "Invalid UserName/Password!!";

                    //return View("Create");
                    return RedirectToAction("Index", "FranchiseRegister");
                }
                else
                {
                    Dictionary<string, string> lDictLoginDetails = this.CheckLogin(pUserName,pPassword);

                    if (lDictLoginDetails.Count() <= 0)
                    {
                        ViewBag.Message = "Invalid UserName/Password!!";
                        TempData["Message"] = "Invalid UserName/Password!!";

                        //return View("Create");
                        return RedirectToAction("Index", "FranchiseRegister");
                    }
                    else
                    {
                        long UserLoginID = Convert.ToInt64(lDictLoginDetails["ID"].ToString());
                        long businessDetailID = db.BusinessDetails.Where(x => x.UserLoginID == UserLoginID && x.BusinessType.Prefix == "GBFR").Select(x => x.ID).FirstOrDefault();
                        var IsExist = db.Franchises.Where(x => x.BusinessDetailID == businessDetailID && x.IsActive == true).FirstOrDefault();
                        var IsEmployee = db.Employees.Where(x => x.EmployeeCode.StartsWith("GBFR") && x.UserLoginID == UserLoginID && x.IsActive == true).FirstOrDefault();
                        if (IsExist != null)
                        {
                            if (IsPlanValid(UserLoginID, businessDetailID, IsExist))
                            {
                                Session["FRANCHISE_ID"] = IsExist.ID;
                                Session["ID"] = lDictLoginDetails["ID"];
                                Session["USER_LOGIN_ID"] = lDictLoginDetails["ID"];


                                ModelLayer.Models.PersonalDetail pd = new ModelLayer.Models.PersonalDetail();
                                long LoginID = Convert.ToInt64(lDictLoginDetails["ID"].ToString());
                                pd = db.PersonalDetails.Where(x => x.UserLoginID == LoginID).FirstOrDefault();
                                Session["PERSONAL_ID"] = pd == null ? 0 : pd.ID;

                                Session["USER_NAME"] = lDictLoginDetails["UserName"];
                                FormsAuthentication.SetAuthCookie(pUserName, true);

                                string businessname = db.BusinessDetails.Where(x => x.UserLoginID == LoginID).Select(x => x.Name).FirstOrDefault();
                                int fId = Convert.ToInt32(Session["FRANCHISE_ID"]);
                                string contactPersonName = db.Franchises.Where(x => x.ID == fId).Select(x => x.ContactPerson).FirstOrDefault();

                                //=============== call method to get no. of days remaining for plan expiration =================
                                this.GetRemainingDaysByPlan();


                                string Rurl = Convert.ToString(TempData.Peek("ReturnUrl"));
                                string decodedUrl = "";
                                if (!string.IsNullOrEmpty(Rurl))
                                {
                                    decodedUrl = Server.UrlDecode(Rurl);
                                }

                                if (Url.IsLocalUrl(decodedUrl))
                                {
                                    TempData.Remove("ReturnUrl");
                                    return Redirect(decodedUrl);
                                }
                                else
                                {
                                    if (string.IsNullOrEmpty(businessname) || string.IsNullOrEmpty(contactPersonName))
                                    {
                                        return RedirectToAction("Edit", "PersonalDetail");
                                    }
                                    else
                                    {
                                        return RedirectToAction("Index", "Home");
                                    }
                                }

                            }
                            else
                            {
                                UserLogin ul = db.UserLogins.Find(UserLoginID);
                                ul.IsLocked = true;
                                ul.ModifyDate = DateTime.Now;
                                db.SaveChanges();

                                ViewBag.Message = "Can't Login!! Your Plan is Expired. Please, Contact Administrator...";
                                TempData["Message"] = "Can't Login!! Your Plan is Expired. Please, Contact Administrator...";
                                return RedirectToAction("Index", "FranchiseRegister");
                            }
                        }
                        else if (IsEmployee != null)
                        {
                            long? FranchiseId = db.Employees.Where(x => x.UserLoginID == UserLoginID).Select(x => x.OwnerID).FirstOrDefault();
                            long BID = db.Franchises.Where(x => x.ID == FranchiseId).Select(x => x.BusinessDetailID).FirstOrDefault();
                            long Fran_UID = db.BusinessDetails.Where(x => x.ID == BID).Select(x => x.UserLoginID).FirstOrDefault();
                            if (IsPlanValid(Fran_UID, BID, db.Franchises.Find(FranchiseId)))
                            {
                                Session["FRANCHISE_ID"] = FranchiseId;
                                Session["ID"] = lDictLoginDetails["ID"];
                                Session["USER_LOGIN_ID"] = lDictLoginDetails["ID"];


                                ModelLayer.Models.PersonalDetail pd = new ModelLayer.Models.PersonalDetail();
                                long LoginID = Fran_UID;
                                pd = db.PersonalDetails.Where(x => x.UserLoginID == LoginID).FirstOrDefault();
                                Session["PERSONAL_ID"] = pd == null ? 0 : pd.ID;

                                Session["USER_NAME"] = lDictLoginDetails["UserName"];
                                FormsAuthentication.SetAuthCookie(pUserName, true);


                                string Rurl = ViewBag.ReturnUrl;
                                string decodedUrl = "";
                                if (!string.IsNullOrEmpty(Rurl))
                                {
                                    decodedUrl = Server.UrlDecode(Rurl);
                                }

                                if (Url.IsLocalUrl(decodedUrl))
                                {
                                    TempData.Remove("ReturnUrl");
                                    return Redirect(decodedUrl);
                                }
                                else
                                {
                                    return RedirectToAction("Index", "Home");
                                }
                                //return RedirectToAction("Index", "Home");
                            }
                            else
                            {
                                UserLogin ul = db.UserLogins.Find(Fran_UID);
                                ul.IsLocked = true;
                                ul.ModifyDate = DateTime.Now;
                                db.SaveChanges();

                                ViewBag.Message = "Can't Login!! Your Plan is Expired. Please, Contact Administrator...";
                                TempData["Message"] = "Can't Login!! Your Plan is Expired. Please, Contact Administrator...";
                                return RedirectToAction("Index", "FranchiseRegister");
                            }
                        }
                        else
                        {
                            ViewBag.Message = "You are not a Partner";
                            TempData["Message"] = "You are not a Partner";

                            //return View("Create");
                            return RedirectToAction("Index", "FranchiseRegister");
                        }
                    }
                }
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[FranchiseRegisterController][POST:Index]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Franchise, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[FranchiseRegisterController][POST:Index]",
                    BusinessLogicLayer.ErrorLog.Module.Franchise, System.Web.HttpContext.Current.Server);
            }
            return RedirectToAction("Index", "FranchiseRegister");
        }
        //End by Priti in 6-12-2018
        private bool IsPlanValid(long UserLoginID, long businessDetailID, ModelLayer.Models.Franchise frn)
        {
            bool flag = false;
            try
            {
                DateTime PlanEndDate = (from op in db.OwnerPlans
                                        join p in db.Plans on op.PlanID equals p.ID
                                        where op.OwnerID == frn.ID && p.PlanCode.StartsWith("GBFR") && op.IsActive == true
                                        select op.EndDate).FirstOrDefault();
                if (PlanEndDate.Date.CompareTo(DateTime.Now.Date) >= 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                throw new BusinessLogicLayer.MyException("[IsPlanValid]", "Can't Log in! in Method !" + Environment.NewLine + ex.Message);
            }
            return flag;
        }

        public ActionResult View1()
        {
            return View();
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
                /*This Action Responces to AJAX Call
                 * After entering Pincode returens City, District and State Information
                 * */
                if (!(db.Pincodes.Any(p => p.Name == Pincode)))
                {
                    //var errorMsg = "Pincode Dose Not Exist, Please Contact Admin!";
                    //return View(new { success = false, Error = errorMsg });
                    return Json("1", JsonRequestBehavior.AllowGet);
                }
                return Json("0", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw new BusinessLogicLayer.MyException("[GetAddress]", "Can't Get Address! in Method !" + Environment.NewLine + ex.Message);
            }
        }

        public JsonResult IsMobileAvailable(string Mobile)
        {
            if (db.UserLogins.Any(x => x.Mobile == Mobile))
            {
                return Json("1", JsonRequestBehavior.AllowGet);
            }
            return Json("0", JsonRequestBehavior.AllowGet);
        }

        public JsonResult IsEmailAvailable(string Email)
        {
            if (db.UserLogins.Any(x => x.Email == Email))
            {
                return Json("1", JsonRequestBehavior.AllowGet);
            }
            return Json("0", JsonRequestBehavior.AllowGet);
        }

        public JsonResult IsServiceNumAvailable(string ServiceNum)
        {
            try
            {
                if (db.Franchises.Any(x => x.ServiceNumber == ServiceNum))
                {
                    return Json("1", JsonRequestBehavior.AllowGet);
                }
                return Json("0", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw new BusinessLogicLayer.MyException("[IsServiceNumAvailable]", "Can't Get Service Number! in Method !" + Environment.NewLine + ex.Message);
            }
        }

        //funcn for login
        private bool IsValidEmailId(string pInputEmail)
        {
            //Regex To validate Email Address
            Regex regex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
            Match match = regex.Match(pInputEmail);
            if (match.Success)
                return true;
            else
                return false;
        }
        private bool IsValidMobile(string pInputMobile)
        {
            //Regex To validate Email Address
            Regex regex = new Regex(@"^[7-9]{1}[0-9]{9}$");
            Match match = regex.Match(pInputMobile);
            if (match.Success)
                return true;
            else
                return false;
        }
        private Dictionary<string, string> CheckLogin(string pUserName, string pPassword)
        {
            try
            {
                Dictionary<string, string> lDictUserDetails = new Dictionary<string, string>();

                var userExist = db.UserLogins.Select(x => new { x.Email, x.Mobile, x.Password, x.ID, x.IsLocked })
                                             .Where(x => (x.Email == pUserName || x.Mobile == pUserName) && x.Password == pPassword && x.IsLocked == false).ToList();
                if (userExist.Count() > 0)
                {
                    foreach (var item in userExist)
                    {
                        lDictUserDetails.Add("ID", item.ID.ToString());
                        lDictUserDetails.Add("UserName", item.Email.ToString());
                    }
                }

                return lDictUserDetails;
            }
            catch (Exception ex)
            {
                throw new BusinessLogicLayer.MyException("[CheckLogin]", "Can't Log in! in Method !" + Environment.NewLine + ex.Message);
            }
        }

        public void sendEmail(long uid)
        {
            try
            {
                PersonalDetail lPD = db.PersonalDetails.Find(CommonFunctions.GetPersonalDetailsID(uid));
                string email = db.UserLogins.Find(uid).Email;
                // var merchantDetail= db.UserLogins.Find(uid);

                //long franchiseId = db.BusinessDetails.Where(x => x.UserLoginID == uid).FirstOrDefault().ID;

                //string shopName = db.Shops.Where(x => x.BusinessDetailID == merchantId).FirstOrDefault().Name;

                // Sending email to the user
                BusinessLogicLayer.ReadConfig rcKey = new BusinessLogicLayer.ReadConfig(System.Web.HttpContext.Current.Server);
                BusinessLogicLayer.GateWay gateWay = new BusinessLogicLayer.Email(System.Web.HttpContext.Current.Server);
                Dictionary<string, string> emailParaMetres = new Dictionary<string, string>();
                emailParaMetres.Add("<!--ACCOUNT_URL-->", "" + (new URLsFromConfig()).GetURL("PARTNER") + "");
                emailParaMetres.Add("<!--NAME-->", lPD.FirstName);
                //emailParaMetres.Add("<!--SHOP_NAME-->", shopName);
                //emailParaMetres.Add("<!--FRANCHISE_ID-->", franchiseId.ToString());
                


                gateWay.SendEmail(BusinessLogicLayer.GateWay.EmailGateWays.GANDHIBAGH, BusinessLogicLayer.GateWay.SenderMail.INFO, BusinessLogicLayer.GateWay.EMailTypes.FRN_REGISTRATION, new string[] { email, rcKey.DEFAULT_ALL_EMAIL }, emailParaMetres, true);

            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Message", "Merchant Approved Succesfully, there might be problem sending email, please check your email or contact administrator!");
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[MerchantApprovalController][sendEmail]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Franchise, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[MerchantApprovalController][sendEmail]",
                    BusinessLogicLayer.ErrorLog.Module.Franchise, System.Web.HttpContext.Current.Server);
            }
        }

        public void sendSMS(long uid)
        {
            try
            {
                PersonalDetail lPD = db.PersonalDetails.Find(CommonFunctions.GetPersonalDetailsID(uid));
                string mbno = db.UserLogins.Find(uid).Mobile;

                // Sending sms to the user
                BusinessLogicLayer.ReadConfig rcKey = new BusinessLogicLayer.ReadConfig(System.Web.HttpContext.Current.Server);
                BusinessLogicLayer.GateWay gateWay = new BusinessLogicLayer.SMS(System.Web.HttpContext.Current.Server);

                Dictionary<string, string> smsValues = new Dictionary<string, string>();
                smsValues.Add("#--NAME--#", lPD.FirstName);

                gateWay.SendSMS(BusinessLogicLayer.GateWay.SMSGateWays.SUMIT, BusinessLogicLayer.GateWay.SMSOptions.MULTIPLE, BusinessLogicLayer.GateWay.SMSTypes.FRN_REG, new string[] { mbno, rcKey.DEFAULT_ALL_SMS }, smsValues);

            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Message", "Merchant Approved Succesfully, there might be problem sending sms, please check your email or contact administrator!");
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[MerchantApprovalController][sendSMS]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Franchise, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[MerchantApprovalController][sendSMS]",
                    BusinessLogicLayer.ErrorLog.Module.Franchise, System.Web.HttpContext.Current.Server);
            }
        }

        public void GetRemainingDaysByPlan()
        {
            DataTable dt = new DataTable();
            if (Session["USER_LOGIN_ID"] != null)
            {
                ReadConfig config = new ReadConfig(System.Web.HttpContext.Current.Server);
                DataAccessLayer.DbOperations dbOpr = new DataAccessLayer.GetData(config.DB_CONNECTION);
                List<object> paramValues = new List<object>();
                paramValues.Add(Convert.ToInt64(Session["USER_LOGIN_ID"]));
                paramValues.Add("GBFR");
                dt = dbOpr.GetRecords("SelectRemainingPlanDays", paramValues);
                if (dt != null && dt.Rows.Count > 0 && Convert.ToInt32(dt.Rows[0]["DiffDate"].ToString())<=30)
                {
                    Session["PlanExpDaysRem"] = dt.Rows[0]["DiffDate"].ToString();
                }
            }
            //return dt;
        }
    }
}