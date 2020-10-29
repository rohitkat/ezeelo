using BusinessLogicLayer;
using ModelLayer.Models;
using ModelLayer.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using ModelLayer.Models;
using System.Web.Services;
using System.IO;
using System.Web.Configuration;
using System.Net;
using System.Data.Entity.SqlServer;
using Gandhibagh.Models;
using System.Data.Entity;
using System.Web.Helpers;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Security.Cryptography;

namespace Customer.Controllers
{
    // [SessionExpire]
    public class MerchantController : Controller
    {
        private EzeeloDBContext db = new EzeeloDBContext();
        MerchantNotification merchantNotification = new MerchantNotification();
        StringBuilder errStr = new StringBuilder("=====================================================================================" +
    Environment.NewLine
    + "ErrorLog Controller : MerchantController" + Environment.NewLine);


        #region Registration
        // GET: Merchant/Create
        [SessionExpire]
        [HttpGet]
        public ActionResult Create()
        {
            try
            {

                ViewBag.State = db.States.Where(c => c.IsActive == true).OrderBy(c => c.Name).ToList();
                ViewBag.Category = db.ServiceMasters.Where(c => c.IsActive == true).OrderBy(c => c.Name).ToList();
                ViewBag.Comission = db.CommissionMasters.Where(c => c.IsActive == true).OrderByDescending(c => c.Commission).ToList().Select(p => new { Id = p.Id, Commission = Convert.ToInt16(p.Commission) + "%" });
                var shopTimingList = db.ShopTimingMasters.Where(c => c.IsActive == true).OrderBy(c => c.FromTime).Select(s => new
                {
                    Id = s.Id,
                    Stime = s.FromTime,
                    Ttime = s.ToTime,
                    Time = "",
                }).ToList();
                shopTimingList = shopTimingList.Select(p => new
                {
                    Id = p.Id,
                    Stime = p.Stime,
                    Ttime = p.Ttime,
                    Time = Convert.ToDateTime(p.Stime).ToString("hh:mm tt") + " - " + Convert.ToDateTime(p.Ttime).ToString("hh:mm tt")
                }).ToList();
                //
                ViewBag.ShopTiming = new SelectList(shopTimingList, "Id", "Time");
                ViewBag.Holiday = db.HolidayMasters.Where(c => c.IsActive == true).OrderBy(c => c.CreateDate).ToList();

                ViewBag.NotApplicableGST = false;
                return View();
            }
            catch (Exception ex)
            {

                errStr.Append("Method Name[Http Request] :- Create[HttpGet]" + Environment.NewLine +
                              "ON Dated" + DateTime.UtcNow.AddHours(5.30).TimeOfDay + Environment.NewLine +
                                  ex.Message.ToString() + Environment.NewLine +
                        "====================================================================================="
                              );
                //ViewBag.Message = "Sorry! Problem in customer registration!!";
                ModelState.AddModelError("Message", "Sorry! Problem in Generate Create View!!");
                ErrorLog.ErrorLogFile("Sorry! Problem in Generate Create View " + Environment.NewLine + errStr.ToString()
                    , ErrorLog.Module.Customer, System.Web.HttpContext.Current.Server);


                return View();

            }
        }

        public JsonResult GetCityList(int stateID)
        {
            List<StateCityFranchiseMerchantViewModel> city = new List<StateCityFranchiseMerchantViewModel>();
            try
            {
                List<long> ldistrict = db.Districts.Where(x => x.StateID == stateID).Select(x => x.ID).ToList();

                city = db.Cities.Where(c => ldistrict.Contains(c.DistrictID) && c.IsActive == true).Select(c => new StateCityFranchiseMerchantViewModel { CityID = c.ID, CityName = c.Name }).ToList();
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Error", "There's Something wrong in filling City dropdown!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[Merchant][POST:GetCityByStateId]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Customer, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Error", "There's Something wrong in filling City dropdown!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[Merchat][POST:GetCityByStateId]",
                    BusinessLogicLayer.ErrorLog.Module.Customer, System.Web.HttpContext.Current.Server);
            }
            return Json(city.Distinct().OrderBy(x => x.CityName).ToList(), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetPincodeByCityId(int cityID)
        {
            List<tempData> objODP = new List<tempData>();

            try
            {
                objODP = db.Pincodes
               .Where(x => x.CityID == cityID)
               .Select(x => new tempData { text = x.Name, value = x.ID }
               ).OrderBy(x => x.text)
               .ToList();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Error", "There's Something wrong in filling Pincode dropdown!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[FranchiseLocation][POST:GetPincodeByCityId]",
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
            }
            return Json(objODP.Distinct().ToList(), JsonRequestBehavior.AllowGet);
        }

        public JsonResult ValidateLeaderContactNo(string contactNo)
        {
            bool result = false;

            try
            {
                //check for leader no
                bool isValidNo = false;
                UserLogin userLogin = db.UserLogins.Where(u => u.Mobile == contactNo).FirstOrDefault();
                if (userLogin != null)
                {
                    MLMUser user = db.MLMUsers.Where(m => m.UserID == userLogin.ID).FirstOrDefault();
                    if (user != null)
                        isValidNo = true;
                }
                if (isValidNo)
                {
                    result = true;
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Error", "There's Something wrong in validating Leader's contact no!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[FranchiseLocation][POST:ValidateLeaderContactNo]",
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        // POST: Merchant/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ModelLayer.Models.Merchant merchant, List<string> Holiday)
        {
            try
            {
                ViewBag.State = db.States.Where(c => c.IsActive == true).OrderBy(c => c.Name).ToList();
                ViewBag.Category = db.ServiceMasters.Where(c => c.IsActive == true).OrderBy(c => c.Name).ToList();
                ViewBag.Comission = db.CommissionMasters.Where(c => c.IsActive == true).OrderBy(c => c.Commission).ToList().Select(p => new { Id = p.Id, Commission = p.Commission + "%" });
                var shopTimingList = db.ShopTimingMasters.Where(c => c.IsActive == true).OrderBy(c => c.FromTime).Select(s => new
                {
                    Id = s.Id,
                    Time = s.FromTime + " - " + s.ToTime
                }).ToList();
                ViewBag.ShopTiming = new SelectList(shopTimingList, "Id", "Time");
                ViewBag.Holiday = db.HolidayMasters.Where(c => c.IsActive == true).OrderBy(c => c.Name).ToList();
                ViewBag.City = new SelectList(new List<City>(), "ID", "Name");
                ViewBag.Pincode = new SelectList(new List<Pincode>(), "ID", "Name");
                ViewBag.NotApplicableGST = false;

                //check for leader no
                bool isValidNo = false;
                UserLogin userLogin = db.UserLogins.Where(u => u.Mobile == merchant.LeaderContactNo).FirstOrDefault();
                if (userLogin != null)
                {
                    MLMUser user = db.MLMUsers.Where(m => m.UserID == userLogin.ID).FirstOrDefault();
                    if (user != null)
                        isValidNo = true;
                }
                if (!isValidNo || db.Merchants.Any(p => p.ContactNumber == merchant.ContactNumber && p.LeaderContactNo == merchant.LeaderContactNo && p.Category == merchant.Category && p.City == merchant.City))
                {
                    if (!isValidNo)
                        ViewBag.Message = "Invalid Leardership Contact No.";
                    else
                        ViewBag.Message = "This service is already register with your contact details by same leader reference.";
                    //bind city
                    List<long> ldistrict = db.Districts.Where(x => x.StateID == merchant.State).Select
                        (x => x.ID).ToList();
                    ViewBag.City = db.Cities.Where(c => ldistrict.Contains(c.DistrictID)).Distinct().OrderBy(c => c.Name).Select(s => new
                    {
                        ID = s.ID,
                        Name = s.Name
                    }).ToList();

                    ViewBag.Pincode = db.Pincodes.Where(c => c.CityID == merchant.City).Distinct().OrderBy(c => c.Name).Select(s => new
                    {
                        ID = s.ID,
                        Name = s.Name
                    }).ToList();

                    if (String.IsNullOrEmpty(merchant.GSTINNo))
                        ViewBag.NotApplicableGST = true;

                    ViewBag.Holidays = String.Join(",", Holiday);
                    ViewBag.WarningMsg = "1";
                    return View(merchant);
                }

                merchant.ValidityPeriod = 1;
                merchant.Status = "Inactive";
                merchant.Country = "India";
                merchant.CreateDate = DateTime.UtcNow.AddHours(5.30);
                merchant.NetworkIP = BusinessLogicLayer.CommonFunctions.GetClientIP();
                merchant.DeviceType = "x";
                merchant.DeviceID = "x";
                if (ModelState.IsValid)
                {
                    db.Merchants.Add(merchant);
                    db.SaveChanges();

                    //add holidays
                    foreach (var obj in Holiday)
                    {
                        MerchantHoliday merchantHoliday = new MerchantHoliday();
                        merchantHoliday.MerchantID = merchant.Id;
                        merchantHoliday.HolidayID = Convert.ToInt32(obj);
                        db.MerchantHolidays.Add(merchantHoliday);
                    }

                    db.SaveChanges();
                    SendSMSonRegistrtion(merchant.ContactNumber, merchant.LeaderContactNo, merchant.FranchiseName);
                    ViewBag.Message = "Thank You for showing interest in joining EZEELO. Your registration is under process, we will get back to you very soon.";
                    CheckRmIsBBPSubscriber(merchant.LeaderContactNo);
                    merchantNotification.SaveNotification(1, merchant.FranchiseName);
                    ModelState.Clear();
                    merchant = new ModelLayer.Models.Merchant();
                }

                return View(merchant);
            }

            catch (Exception ex)
            {
                errStr.Append("Method Name[Http Request] :- Create[HttpPost]" + Environment.NewLine +
                                  "ON Dated" + DateTime.UtcNow.AddHours(5.30).TimeOfDay + Environment.NewLine +
                                      ex.Message.ToString() + Environment.NewLine +
                            "====================================================================================="
                                  );
                //ViewBag.Message = "Sorry! Problem in customer registration!!";
                ModelState.AddModelError("Message", "Sorry! Problem in Record Creation!!");
                ErrorLog.ErrorLogFile("Sorry! Problem in Record Creation!!" + Environment.NewLine + errStr.ToString()
                    , ErrorLog.Module.Customer, System.Web.HttpContext.Current.Server);
                ViewBag.Messaage = "Unable to Insert Merchant Detail ";
                return View(merchant);

            }
        }

        void SendSMSonRegistrtion(string MerchantNO, string LeadersNo, string Shop)
        {
            //SMS to leader
            UserLogin u = db.UserLogins.FirstOrDefault(p => p.Mobile == LeadersNo);
            if (u != null)
            {
                PersonalDetail p = db.PersonalDetails.FirstOrDefault(q => q.UserLoginID == u.ID);
                string name = p.FirstName;
                Dictionary<string, string> dictSMSValues_ = new Dictionary<string, string>();
                BusinessLogicLayer.GateWay gateWay_ = new BusinessLogicLayer.SMS(System.Web.HttpContext.Current.Server);
                dictSMSValues_.Add("#--NAME--#", name);
                dictSMSValues_.Add("#--SHOP--#", Shop);
                gateWay_.SendSMS(BusinessLogicLayer.GateWay.SMSGateWays.SUMIT,
                    BusinessLogicLayer.GateWay.SMSOptions.SINGLE, BusinessLogicLayer.GateWay.SMSTypes.MERCHANT_REG_LEADER,
                    new string[] { LeadersNo }, dictSMSValues_);
            }
            Dictionary<string, string> dictSMSValues = new Dictionary<string, string>();
            BusinessLogicLayer.GateWay gateWay = new BusinessLogicLayer.SMS(System.Web.HttpContext.Current.Server);
            gateWay.SendSMS(BusinessLogicLayer.GateWay.SMSGateWays.SUMIT,
                BusinessLogicLayer.GateWay.SMSOptions.SINGLE, BusinessLogicLayer.GateWay.SMSTypes.MERCHANT_REG,
                new string[] { MerchantNO }, dictSMSValues);
        }
        public class tempData
        {
            public Int64 value;
            public string text;
        }
        #endregion

        #region KYC
        [HttpGet]
        public ActionResult KYC(long? id)
        {
            try
            {
                ModelLayer.Models.Merchant merchant = new ModelLayer.Models.Merchant();
                if (id.HasValue)
                {
                    merchant = db.Merchants.Where(m => m.Id == id).FirstOrDefault();
                    ViewBag.ID = merchant.Id;
                    ViewBag.FranchiseName = merchant.FranchiseName;
                    ViewBag.GST = merchant.GSTINNo ?? "";

                    //check link is alive or not
                    //if (merchant.AcceptDate == null || ((TimeSpan)(DateTime.UtcNow.AddHours(5.30) - merchant.AcceptDate.Value)).TotalHours > 120)
                    //    return RedirectToAction("LinkExpiry");

                    //check if kyc record exists
                    MerchantKYC merchantKYC = db.MerchantKYCs.Where(k => k.MerchantID == merchant.Id && k.IsCompleted == true).FirstOrDefault();
                    if (merchantKYC != null)
                        return RedirectToAction("ThankYou");
                }

                return View();
            }
            catch (Exception ex)
            {
                errStr.Append("Method Name[Http Request] :- KYC[HttpGet]" + Environment.NewLine +
                              "ON Dated" + DateTime.UtcNow.AddHours(5.30).TimeOfDay + Environment.NewLine +
                                  ex.Message.ToString() + Environment.NewLine +
                        "====================================================================================="
                              );
                //ViewBag.Message = "Sorry! Problem in customer registration!!";
                ModelState.AddModelError("Message", "Sorry! Problem in Generate Create View!!");
                ErrorLog.ErrorLogFile("Sorry! Problem in Generate Create View " + Environment.NewLine + errStr.ToString()
                    , ErrorLog.Module.Customer, System.Web.HttpContext.Current.Server);


                return View();

            }
        }

        public JsonResult ValidateMerchantContactNo(string contactNo, long id)
        {
            string result = "";

            try
            {
                ModelLayer.Models.Merchant merchant = db.Merchants.Where(u => u.ContactNumber == contactNo && u.Id == id).FirstOrDefault();
                if (merchant != null)
                {
                    result = merchant.FranchiseName;
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Error", "There's Something wrong in validating Leader's contact no!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[MerchantKYC][POST:ValidateContactNo]",
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ValidateMerchantContactNoSendOTP(string contactNo, long id, int par)
        {
            object obj = new object();
            obj = new { Success = 0, Message = "" };
            try
            {
                ModelLayer.Models.Merchant merchant = db.Merchants.Where(u => u.ContactNumber == contactNo && u.Id == id && u.Status == "Accept").FirstOrDefault();
                if (merchant != null)
                {
                    MerchantsLogin login = db.merchantLogins.FirstOrDefault(p => p.MerchantID == merchant.Id && p.IsActive == true);
                    if (login == null)
                    {
                        if (Session["OTPSent"] == null || par == 2)
                        {
                            if (par == 1)
                            {
                                Session["OTPSent"] = "1";
                            }
                            obj = SendOTP(id);
                        }
                        else
                        {
                            obj = new { Success = 1, Message = "OTP sent on your registered mobile no. Please enter OTP." };
                        }
                    }
                    else
                    {
                        obj = new { Success = 0, Message = "Contact no. is Already Validated." };
                    }
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Error", "There's Something wrong in validating Leader's contact no to send OTP!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[MerchantKYC][POST:ValidateContactNo]",
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
            }
            return Json(obj, JsonRequestBehavior.AllowGet);
        }

        //public JsonResult ValidateOTP(string OTP, long MerchantId, string Password)
        public ActionResult ValidateOTP(string OTP, string Password, long MerchantId)
        {
            object obj = new object();
            obj = new { Success = 0, Message = "" };
            try
            {
                if (OTP == Session["MerchantOTP"].ToString())
                {
                    ModelLayer.Models.Merchant merchant = db.Merchants.FirstOrDefault(p => p.Id == MerchantId);
                    MerchantsLogin login = db.merchantLogins.FirstOrDefault(p => p.MerchantID == MerchantId);
                    if (login == null)
                    {
                        login = new MerchantsLogin();
                        login.MerchantID = MerchantId;
                        login.UserID = merchant.ContactNumber;
                        login.IsActive = true;
                        login.CreateDate = DateTime.Now;
                        login.NetworkIP = CommonFunctions.GetClientIP();
                        login.ModifyBy = "Admin";
                        login.Password = Password;
                        login.ConfirmPassword = login.Password;
                        db.merchantLogins.Add(login);
                    }
                    else
                    {
                        login.IsActive = true;
                        login.ModifyDate = DateTime.Now;
                        login.NetworkIP = CommonFunctions.GetClientIP();
                        login.Password = Password;
                        login.ConfirmPassword = login.Password;
                        login.ModifyBy = "Admin";
                    }
                    db.SaveChanges();
                    SendMailOnValidateNo(MerchantId);
                    string merchantLoginLink = WebConfigurationManager.AppSettings["MERCHANT_DASHBOARD"];
                    obj = new { Success = 1, Message = "Congratulations! Your Contact No. is now verified. We will now redirect you to login page.", DashboardLink = merchantLoginLink };
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Error", "There's Something wrong in validating Leader's contact no to send OTP!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[MerchantKYC][POST:ValidateContactNo]",
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
            }
            return Json(obj, JsonRequestBehavior.AllowGet);
        }

        public void SendMailOnValidateNo(long MerchantId)
        {
            try
            {
                string url = WebConfigurationManager.AppSettings["MERCHANT_DASHBOARD"] + "/Home/SetPassword?id=" + MerchantId;
                ModelLayer.Models.Merchant obj = db.Merchants.FirstOrDefault(p => p.Id == MerchantId);
                Dictionary<string, string> dictEmailValues = new Dictionary<string, string>();
                dictEmailValues.Add("<!--NAME-->", obj.ContactPerson);
                dictEmailValues.Add("<!--SHOP-->", obj.FranchiseName);
                //dictEmailValues.Add("<!--LINK-->", url);
                BusinessLogicLayer.GateWay gateWay = new BusinessLogicLayer.Email(System.Web.HttpContext.Current.Server);
                gateWay.SendEmail(BusinessLogicLayer.GateWay.EmailGateWays.GANDHIBAGH, BusinessLogicLayer.GateWay.SenderMail.INFO, BusinessLogicLayer.GateWay.EMailTypes.VALIDATE_MERCHANT, new string[] { obj.Email }, dictEmailValues, true);
            }
            catch { }
        }
        [HttpPost]
        public ActionResult KYC(MerchantKYC merchant, HttpPostedFileBase photo, HttpPostedFileBase shopCertificate, HttpPostedFileBase pan, HttpPostedFileBase gst, HttpPostedFileBase address, HttpPostedFileBase visitingCard, HttpPostedFileBase cancelledCheque, HttpPostedFileBase Banner)
        {
            long merchantID = merchant.MerchantID;
            try
            {
                MerchantKYC merchantKYC = db.MerchantKYCs.Where(k => k.ID == merchant.ID).FirstOrDefault();
                if (merchantKYC != null)
                    merchant = merchantKYC;

                //bool isNewRecord = true;
                
                ModelLayer.Models.Merchant obj = new ModelLayer.Models.Merchant();
                obj = db.Merchants.Where(m => m.Id == merchant.MerchantID).FirstOrDefault();

                ////check link is alive or not
                //if (obj.AcceptDate == null || ((TimeSpan)(DateTime.UtcNow.AddHours(5.30) - obj.AcceptDate.Value)).TotalHours > 24)
                //    return RedirectToAction("LinkExpiry");

                if (photo == null || shopCertificate == null || pan == null || (!String.IsNullOrEmpty(obj.GSTINNo) ? gst == null : false) || address == null || cancelledCheque == null)
                {
                    string option = string.Empty;
                    if (photo == null)
                        option = "Photo";
                    else if (shopCertificate == null)
                        option = "Shop Certification";
                    else if (pan == null)
                        option = "PAN";
                    else if (gst == null && obj.GSTINNo != null)
                        option = "GST";
                    else if (address == null)
                        option = "Address Proof";
                    else if (cancelledCheque == null)
                        option = "Cancelled Cheque";

                    if (option != string.Empty)
                    {
                        TempData["Messaage"] = "Please select Image file for " + option;
                        return RedirectToAction("KYC", new { id = merchantID });
                    }
                }

                var allowedExtensions = new[] { ".Jpg", ".png", ".jpg", ".jpeg", ".raw" };
                bool IsSaved = false;
                string Filename = "";
                string Ext = "";
                if (photo != null)
                {
                    var fileNamePhoto = Path.GetFileName(photo.FileName);
                    var ExtPhoto = Path.GetExtension(photo.FileName);
                    string namePhoto = Path.GetFileNameWithoutExtension(fileNamePhoto);
                    string photoPath = namePhoto + "_" + merchantID + ExtPhoto;

                    if (allowedExtensions.Contains(ExtPhoto)) //check what type of extension  
                    {
                        IsSaved = UploadImage((int)Document_Type.PHOTO, photo, (long)merchantID, System.Web.HttpContext.Current.Server, out Filename, out Ext);

                        merchant.PhotoImageUrl = Filename;
                    }
                    else
                    {
                        TempData["Messaage"] = "Please choose only Image or PDF file for Shop Photo";
                        return RedirectToAction("KYC", new { id = merchantID });
                    }

                }
                Filename = "";
                if (shopCertificate != null)
                {
                    var fileNameShopCertificate = Path.GetFileName(shopCertificate.FileName);
                    var ExtShopCertificate = Path.GetExtension(shopCertificate.FileName);
                    string nameShopCertificate = Path.GetFileNameWithoutExtension(fileNameShopCertificate);
                    string shopCertificatePath = nameShopCertificate + "_" + merchantID + ExtShopCertificate;

                    if (allowedExtensions.Contains(ExtShopCertificate)) //check what type of extension  
                    {
                        IsSaved = UploadImage((int)Document_Type.SHOPESTABLISHMENT, shopCertificate, (long)merchantID, System.Web.HttpContext.Current.Server, out Filename, out Ext);

                        merchant.ShopEstablishmentCertificateImageUrl = Filename;
                    }
                    else
                    {
                        TempData["Messaage"] = "Please choose only Image or PDF file for Shop Establishment Certificate";
                        return RedirectToAction("KYC", new { id = merchantID });
                    }

                }
                Filename = "";
                if (pan != null)
                {
                    var fileNamePAN = Path.GetFileName(pan.FileName);
                    var ExtPan = Path.GetExtension(pan.FileName);
                    string namePan = Path.GetFileNameWithoutExtension(fileNamePAN);//getting file name without extension  
                    string PANPath = namePan + "_" + merchantID + ExtPan;
                    if (allowedExtensions.Contains(ExtPan)) //check what type of extension  
                    {
                        IsSaved = UploadImage((int)Document_Type.PAN, pan, (long)merchantID, System.Web.HttpContext.Current.Server, out Filename, out Ext);

                        merchant.PanImageUrl = Filename;
                    }
                    else
                    {
                        TempData["Messaage"] = "Please choose only Image or PDF file for PAN Card";
                        return RedirectToAction("KYC", new { id = merchantID });
                    }

                }
                Filename = "";
                if (gst != null)
                {
                    var fileNameGST = Path.GetFileName(gst.FileName);
                    var ExtGST = Path.GetExtension(gst.FileName);
                    string nameGST = Path.GetFileNameWithoutExtension(fileNameGST);
                    string GSTPath = nameGST + "_" + merchantID + ExtGST;

                    if (allowedExtensions.Contains(ExtGST)) //check what type of extension  
                    {
                        IsSaved = UploadImage((int)Document_Type.GST, gst, (long)merchantID, System.Web.HttpContext.Current.Server, out Filename, out Ext);
                        merchant.GSTRegistrationImageUrl = Filename;
                    }
                    else
                    {
                        TempData["Messaage"] = "Please choose only Image or PDF file for GST Registration Certificate";
                        return RedirectToAction("KYC", new { id = merchantID });
                    }


                }
                Filename = "";
                if (address != null)
                {
                    var fileNameAddress = Path.GetFileName(address.FileName);
                    var ExtAddress = Path.GetExtension(address.FileName);
                    string nameAddress = Path.GetFileNameWithoutExtension(fileNameAddress);
                    string AddressPath = nameAddress + "_" + merchantID + ExtAddress;

                    if (allowedExtensions.Contains(ExtAddress)) //check what type of extension  
                    {
                        IsSaved = UploadImage((int)Document_Type.ADDRESS, address, (long)merchantID, System.Web.HttpContext.Current.Server, out Filename, out Ext);
                        merchant.AddressProofUrl = Filename;
                    }
                    else
                    {
                        TempData["Messaage"] = "Please choose only Image or PDF file for Address Proof";
                        return RedirectToAction("KYC", new { id = merchantID });
                    }


                }
                Filename = "";
                if (visitingCard != null)
                {
                    var fileNameVisitingCard = Path.GetFileName(visitingCard.FileName);
                    var ExtVisitingCard = Path.GetExtension(visitingCard.FileName);
                    string nameVisitingCard = Path.GetFileNameWithoutExtension(fileNameVisitingCard);
                    string VisitingCardPath = nameVisitingCard + "_" + merchantID + ExtVisitingCard;

                    if (allowedExtensions.Contains(ExtVisitingCard)) //check what type of extension  
                    {
                        IsSaved = UploadImage((int)Document_Type.VISTINGCARD, visitingCard, (long)merchantID, System.Web.HttpContext.Current.Server, out Filename, out Ext);
                        merchant.VisingCardImageUrl = Filename;
                    }
                    else
                    {
                        TempData["Messaage"] = "Please choose only Image or PDF file for Visiting Card";
                        return RedirectToAction("KYC", new { id = merchantID });
                    }
                }
                Filename = "";
                if (cancelledCheque != null)
                {

                    var fileNameCancelledCheque = Path.GetFileName(cancelledCheque.FileName);
                    var ExtCancelledCheque = Path.GetExtension(cancelledCheque.FileName);
                    string nameCancelledCheque = Path.GetFileNameWithoutExtension(fileNameCancelledCheque);
                    string CancelledChequePath = nameCancelledCheque + "_" + merchantID + ExtCancelledCheque;

                    if (allowedExtensions.Contains(ExtCancelledCheque)) //check what type of extension  
                    {
                        IsSaved = UploadImage((int)Document_Type.CANCELLEDCHEQUE, cancelledCheque, (long)merchantID, System.Web.HttpContext.Current.Server, out Filename, out Ext);
                        merchant.CancelledblankChequeImageUrl = Filename;
                    }
                    else
                    {
                        TempData["Messaage"] = "Please choose only Image or PDF file for Cancelled Blank Cheque";
                        return RedirectToAction("KYC", new { id = merchantID });
                    }
                }
                Filename = "";
                if (Banner != null)
                {
                    var fileNameBanner = Path.GetFileName(Banner.FileName);
                    var ExtBanner = Path.GetExtension(Banner.FileName);
                    string nameBanner = Path.GetFileNameWithoutExtension(fileNameBanner);
                    string BannerPath = nameBanner + "_" + merchantID + ExtBanner;

                    if (allowedExtensions.Contains(ExtBanner)) //check what type of extension  
                    {
                        IsSaved = UploadImage((int)Document_Type.BANNER, Banner, (long)merchantID, System.Web.HttpContext.Current.Server, out Filename, out Ext);
                        merchant.BannerImageUrl = Filename;
                    }
                    else
                    {
                        TempData["Messaage"] = "Please choose only Image or PDF file for Shop Banner";
                        return RedirectToAction("KYC", new { id = merchantID });
                    }
                }

                merchant.CreateDate = DateTime.UtcNow.AddHours(5.30);
                merchant.IsCompleted = true;

                if (merchant.ID == 0)
                    db.MerchantKYCs.Add(merchant);
                else
                    db.Entry(merchant).State = EntityState.Modified;


                MerchantBanner banner = new MerchantBanner();
                banner.IsActive = true;
                banner.CreateDate = DateTime.Now;
                banner.BannerPath = merchant.BannerImageUrl;
                banner.MerchantID = merchant.MerchantID;
                db.merchantBanners.Add(banner);
                db.SaveChanges();

                merchantNotification.SaveNotification(2, obj.FranchiseName);
                SendSMSKYC(merchantID);
                //string merchantLoginLink = WebConfigurationManager.AppSettings["MERCHANT_DASHBOARD"];
                //return Redirect(merchantLoginLink);
                return RedirectToAction("ThankYou");
            }
            catch (Exception ex)
            {
                errStr.Append("Method Name[Http Request] :- Merchant KYC[HttpPost]" + Environment.NewLine +
                                  "ON Dated" + DateTime.UtcNow.AddHours(5.30).TimeOfDay + Environment.NewLine +
                                      ex.Message.ToString() + Environment.NewLine +
                            "====================================================================================="
                                  );
                //ViewBag.Message = "Sorry! Problem in customer registration!!";
                ModelState.AddModelError("Message", "Sorry! Problem in Record Creation!!");
                ErrorLog.ErrorLogFile("Sorry! Problem in Record Creation!!" + Environment.NewLine + errStr.ToString()
                    , ErrorLog.Module.Customer, System.Web.HttpContext.Current.Server);
                ViewBag.Messaage = "Unable to Insert Merchant KYC Detail ";
                TempData["Messaage"] = ex.Message.ToString();
            }
            return RedirectToAction("KYC", new { id = merchantID });
        }
        public void SendSMSKYC(long MerchantID)
        {
            ModelLayer.Models.Merchant obj = db.Merchants.FirstOrDefault(p => p.Id == MerchantID);

            try
            {
                Dictionary<string, string> dictSMSValues_ = new Dictionary<string, string>();
                BusinessLogicLayer.GateWay gateWay_ = new BusinessLogicLayer.SMS(System.Web.HttpContext.Current.Server);
                gateWay_.SendSMS(BusinessLogicLayer.GateWay.SMSGateWays.SUMIT,
                    BusinessLogicLayer.GateWay.SMSOptions.SINGLE, BusinessLogicLayer.GateWay.SMSTypes.MERCHANT_REG_KYC,
                    new string[] { obj.ContactNumber }, dictSMSValues_);
            }
            catch
            {

            }
        }
        public object SendOTP(long MerchantID)
        {
            object obj1 = new object();
            int Attempt = 0;
            if (Request.Cookies["MerchantOTPAttempt"] != null)
            {
                Attempt = Convert.ToInt32(Request.Cookies["MerchantOTPAttempt"].Value);
            }
            ModelLayer.Models.Merchant obj = db.Merchants.FirstOrDefault(p => p.Id == MerchantID);
            string OTP = CommonFunctions.GetRandomNumberOTP(6);
            try
            {
                if (Attempt < 15)
                {
                    Dictionary<string, string> dictSMSValues_ = new Dictionary<string, string>();
                    dictSMSValues_.Add("#--NAME--#", obj.ContactPerson);
                    dictSMSValues_.Add("#--OTP--#", OTP);
                    Session["MerchantOTP"] = OTP;
                    BusinessLogicLayer.GateWay gateWay_ = new BusinessLogicLayer.SMS(System.Web.HttpContext.Current.Server);
                    gateWay_.SendSMS(BusinessLogicLayer.GateWay.SMSGateWays.SUMIT,
                    BusinessLogicLayer.GateWay.SMSOptions.SINGLE, BusinessLogicLayer.GateWay.SMSTypes.OTP_MER_REG,
                    new string[] { obj.ContactNumber }, dictSMSValues_);
                    Response.Cookies["MerchantOTPAttempt"].Value = (Attempt + 1).ToString();
                    Response.Cookies.Add(Response.Cookies["MerchantOTPAttempt"]);
                    Response.Cookies["MerchantOTPAttempt"].Expires = System.DateTime.Now.AddDays(30);
                    obj1 = new { Success = 1, Message = "OTP sent on your registered mobile no. Please enter OTP and set your password." };
                }
                else
                {
                    obj1 = new { Success = 0, Message = "You have exceeds OTP regenerate limit. Please contact customer care on 9172221910 for further assistance" };
                }
            }
            catch (Exception ex)
            {
                obj1 = new { Success = 0, Message = ex.Message };
            }
            return obj1;
        }
        public bool UploadImage(int status, HttpPostedFileBase file, long ID, HttpServerUtility server, out string FileName, out string Ext)
        {
            try
            {
                string UniqueCode = "_" + DateTime.Now.ToString("ddMMyyHHmmss");
                FileName = "";
                string FolderName = "";
                if ((int)Document_Type.SHOPESTABLISHMENT == status)
                {
                    FolderName = WebConfigurationManager.AppSettings["FOLDER_SHOPESTABLISHMENT"];
                }
                else if ((int)Document_Type.PHOTO == status)
                {
                    FolderName = WebConfigurationManager.AppSettings["FOLDER_PHOTO"];
                }
                else if ((int)Document_Type.PAN == status)
                {
                    FolderName = WebConfigurationManager.AppSettings["FOLDER_PAN"];
                }
                else if ((int)Document_Type.GST == status)
                {
                    FolderName = WebConfigurationManager.AppSettings["FOLDER_GST"];
                }
                else if ((int)Document_Type.ADDRESS == status)
                {
                    FolderName = WebConfigurationManager.AppSettings["FOLDER_ADDRESS"];
                }
                else if ((int)Document_Type.VISTINGCARD == status)
                {
                    FolderName = WebConfigurationManager.AppSettings["FOLDER_VISITINGCARD"];
                }
                else if ((int)Document_Type.CANCELLEDCHEQUE == status)
                {
                    FolderName = WebConfigurationManager.AppSettings["FOLDER_CANCELLEDCHEQUE"];
                }
                else
                {
                    FolderName = WebConfigurationManager.AppSettings["FOLDER_BANNER"];
                }
                string FTP_Path = WebConfigurationManager.AppSettings["IMAGE_FTP"] + WebConfigurationManager.AppSettings["DIRECTORY"] + "/" + FolderName + "/";
                string User_name = WebConfigurationManager.AppSettings["USER_NAME"];
                string Password = WebConfigurationManager.AppSettings["PASSWORD"];
                var notAllowedExtensions = new[] { "" };
                var allowedExtensions = new[] { "" };
                var path = "";
                var fileName_ = Path.GetFileName(file.FileName);
                Ext = Path.GetExtension(file.FileName); //getting the extension(ex-.jpg)  

                string name = Path.GetFileNameWithoutExtension(fileName_); //getting file name without extension  
                string myfile = name + "_" + ID + UniqueCode + Ext; //appending the name with InvoiceID  
                path = Path.Combine(server.MapPath("~/Image"), myfile);
                if ((int)Document_Type.PHOTO == status || (int)Document_Type.BANNER == status)
                {
                    if ((int)Document_Type.PHOTO == status)
                    {
                        ResizeImage(file, 300, 200, path);
                    }
                    else
                    {
                        ResizeImage(file, 1200, 400, path);
                    }
                }
                else
                {
                    file.SaveAs(path);
                }
                string source;
                source = server.MapPath("~/Image");
                FileInfo[] lFileInfo = null;

                DirectoryInfo lDrInfo = new DirectoryInfo(server.MapPath("~/Image"));
                if (lDrInfo.Exists)
                {
                    lFileInfo = lDrInfo.GetFiles();
                }

                foreach (FileInfo fl in lFileInfo)
                {
                    FtpWebRequest req = (FtpWebRequest)WebRequest.Create(FTP_Path + fl.Name);
                    req.UseBinary = true;
                    req.KeepAlive = true;
                    req.Method = WebRequestMethods.Ftp.UploadFile;

                    req.Credentials = new NetworkCredential(User_name.Normalize(), Password.Normalize());
                    FileStream fs = System.IO.File.OpenRead(fl.DirectoryName + "\\" + fl.Name);
                    byte[] buffer = new byte[fs.Length];
                    fs.Read(buffer, 0, buffer.Length);

                    fs.Close();

                    Stream ftpstream = req.GetRequestStream();
                    ftpstream.Write(buffer, 0, buffer.Length);
                    ftpstream.Close();

                    string filePath = server.MapPath("~/Image/" + fl.ToString());
                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                    }
                    FileName = myfile;
                }
            }
            catch (Exception ex)
            {
                throw ex;// new BusinessLogicLayer.MyException("[CommonController][GetPersonalDetailID]", "Can't find PersonalDetailID !" + Environment.NewLine + ex.Message);
            }
            return true;

        }
        public void ResizeImage(HttpPostedFileBase fileToUpload, float thumbWidth, float thumbHeight, string path)
        {

            try
            {
                using (Image image = Image.FromStream(fileToUpload.InputStream, true, false))
                {
                    try
                    {
                        int actualthumbWidth = Convert.ToInt32(Math.Floor(thumbWidth));
                        int actualthumbHeight = Convert.ToInt32(Math.Floor(thumbHeight));
                        var thumbnailBitmap = new Bitmap(actualthumbWidth, actualthumbHeight);
                        var thumbnailGraph = Graphics.FromImage(thumbnailBitmap);
                        thumbnailGraph.CompositingQuality = CompositingQuality.HighQuality;
                        thumbnailGraph.SmoothingMode = SmoothingMode.HighQuality;
                        thumbnailGraph.InterpolationMode = InterpolationMode.HighQualityBicubic;
                        var imageRectangle = new Rectangle(0, 0, actualthumbWidth, actualthumbHeight);
                        thumbnailGraph.DrawImage(image, imageRectangle);
                        var ms = new MemoryStream();
                        thumbnailBitmap.Save(path, ImageFormat.Jpeg);
                        ms.Position = 0;
                        GC.Collect();
                        thumbnailGraph.Dispose();
                        thumbnailBitmap.Dispose();
                        image.Dispose();
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public enum Document_Type
        {
            SHOPESTABLISHMENT = 1,
            PAN = 2,
            GST = 3,
            ADDRESS = 4,
            VISTINGCARD = 5,
            CANCELLEDCHEQUE = 6,
            PHOTO = 7,
            BANNER = 8
        }

        [HttpGet]
        public ActionResult ThankYou()
        {
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                errStr.Append("Method Name[Http Request] :- ThankYou[HttpGet]" + Environment.NewLine +
                              "ON Dated" + DateTime.UtcNow.AddHours(5.30).TimeOfDay + Environment.NewLine +
                                  ex.Message.ToString() + Environment.NewLine +
                        "====================================================================================="
                              );
                //ViewBag.Message = "Sorry! Problem in customer registration!!";
                ModelState.AddModelError("Message", "Sorry! Problem in Generate Create View!!");
                ErrorLog.ErrorLogFile("Sorry! Problem in Generate Create View " + Environment.NewLine + errStr.ToString()
                    , ErrorLog.Module.Customer, System.Web.HttpContext.Current.Server);


                return View();

            }
        }

        [HttpGet]
        public ActionResult LinkExpiry()
        {
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                errStr.Append("Method Name[Http Request] :- KYC Link Expiry[HttpGet]" + Environment.NewLine +
                              "ON Dated" + DateTime.UtcNow.AddHours(5.30).TimeOfDay + Environment.NewLine +
                                  ex.Message.ToString() + Environment.NewLine +
                        "====================================================================================="
                              );
                //ViewBag.Message = "Sorry! Problem in customer registration!!";
                ModelState.AddModelError("Message", "Sorry! Problem in Generate Create View!!");
                ErrorLog.ErrorLogFile("Sorry! Problem in Generate Create View " + Environment.NewLine + errStr.ToString()
                    , ErrorLog.Module.Customer, System.Web.HttpContext.Current.Server);


                return View();

            }
        }
        #endregion

        #region MerchantList
        [SessionExpire]
        [HttpGet]
        public ActionResult List(long? id)
        {
            try
            {
                long cityId = 0;

                if (ControllerContext.HttpContext.Request.Cookies["CityCookie"] != null && ControllerContext.HttpContext.Request.Cookies["CityCookie"].Value != string.Empty)
                {
                    cityId = Convert.ToInt32(ControllerContext.HttpContext.Request.Cookies["CityCookie"].Value.Split('$')[0].Trim());
                }

                DateTime today = DateTime.UtcNow.AddHours(5.30);
                List<ModelLayer.Models.Merchant> merchant = new List<ModelLayer.Models.Merchant>();
                string Path = WebConfigurationManager.AppSettings["IMAGE_HTTP"] + WebConfigurationManager.AppSettings["DIRECTORY"] + "/" + WebConfigurationManager.AppSettings["FOLDER_PHOTO"] + "/";

                if (id.HasValue)
                {
                    merchant = db.Merchants.Where(m => m.Category == id && m.Status == "Approve" && (m.ApproveDate != null && (m.ApproveDate != null && SqlFunctions.DateAdd("year", m.ValidityPeriod.Value, m.ApproveDate.Value) >= today)) && m.City == cityId && (db.merchantTopupRecharges.Where(t => t.Amount > 0).Select(t => t.MerchantID)).Contains(m.Id) && (db.MerchantKYCs.Where(MK => MK.IsCompleted == true && MK.IsVerified == true).Select(MK => MK.MerchantID)).Contains(m.Id)).OrderBy(m => m.FranchiseName).ToList();
                    ViewBag.ServiceName = db.ServiceMasters.Where(s => s.Id == id).Select(s => s.Name).FirstOrDefault();
                }
                else
                {
                    merchant = db.Merchants.Where(m => m.Status == "Approve" && (m.ApproveDate != null && (m.ApproveDate != null && SqlFunctions.DateAdd("year", m.ValidityPeriod.Value, m.ApproveDate.Value) >= today)) && m.City == cityId && (db.merchantTopupRecharges.Where(t => t.Amount > 0).Select(t => t.MerchantID)).Contains(m.Id) && (db.MerchantKYCs.Where(MK => MK.IsCompleted == true && MK.IsVerified == true).Select(MK => MK.MerchantID)).Contains(m.Id)).OrderBy(m => m.FranchiseName).ToList();
                }

                //photo path
                merchant.ForEach(m => m.PhotoImageURL = (db.MerchantKYCs.Where(k => k.MerchantID == m.Id).Count() > 0 ? Path + "/" + db.MerchantKYCs.Where(k => k.MerchantID == m.Id).Select(k => k.PhotoImageUrl).FirstOrDefault() : string.Empty));

                //ratings
                merchant.ForEach(m => m.Rating = (db.MerchantRatings.Where(k => k.MerchantID == m.Id && k.IsDisplay == true).Count() > 0 ? db.MerchantRatings.Where(k => k.MerchantID == m.Id && k.IsDisplay == true).Select(k => k.Rating).Average() : 0));

                //total reviews
                merchant.ForEach(m => m.NoOfReviews = db.MerchantRatings.Where(k => k.MerchantID == m.Id && k.IsDisplay == true).Count());
                List<long?> serviceIds = db.Merchants.Where(m => m.City == cityId && m.Status.ToLower() == "approve" && (m.ApproveDate != null && SqlFunctions.DateAdd("year", m.ValidityPeriod.Value, m.ApproveDate.Value) >= today) && (db.merchantTopupRecharges.Where(t => t.Amount > 0).Select(t => t.MerchantID)).Contains(m.Id)).Select(m => m.Category).ToList();

                List<ServiceMaster> serviceMasters = db.ServiceMasters.Where(s => s.IsActive == true && serviceIds.Contains(s.Id)).OrderBy(s => s.Name).ToList();
                ViewBag.Services = serviceMasters;
                if (id != null)
                {
                    ViewBag.SelectedMerchantService = id;
                }

                foreach (var item in merchant)
                {
                    decimal commission = item.CommissionMasterDetail.Commission;
                    decimal BuyerCommssion = 0;
                    MerchantDetails details = db.merchantDetails.FirstOrDefault(p => p.MerchantId == item.Id);
                    if (details != null)
                    {
                        BuyerCommssion = details.Level0;
                    }
                    else
                    {
                        ServiceIncomeMaster incomeMaster = db.ServiceIncomeMasters.FirstOrDefault();
                        BuyerCommssion = incomeMaster.UserLevel0;
                    }
                    item.CasbackToBuyer = Math.Round((commission * BuyerCommssion) / 100, 1);
                }
                return View(merchant);
            }
            catch (Exception ex)
            {
                errStr.Append("Method Name[Http Request] :- List[HttpGet]" + Environment.NewLine +
                              "ON Dated" + DateTime.UtcNow.AddHours(5.30).TimeOfDay + Environment.NewLine +
                                  ex.Message.ToString() + Environment.NewLine +
                        "====================================================================================="
                              );
                //ViewBag.Message = "Sorry! Problem in customer registration!!";
                ModelState.AddModelError("Message", "Sorry! Problem in Generate Create View!!");
                ErrorLog.ErrorLogFile("Sorry! Problem in Generate Create View " + Environment.NewLine + errStr.ToString()
                    , ErrorLog.Module.Customer, System.Web.HttpContext.Current.Server);


                return View();

            }

        }
        #endregion

        #region Merchant Detail

        [HttpGet]
        public ActionResult Details(long id)
        {
            try
            {
                DateTime today = DateTime.UtcNow.AddHours(5.30);
                ModelLayer.Models.Merchant merchant = new ModelLayer.Models.Merchant();
                string Path = WebConfigurationManager.AppSettings["IMAGE_HTTP"] + WebConfigurationManager.AppSettings["DIRECTORY"] + "/" + WebConfigurationManager.AppSettings["FOLDER_PHOTO"] + "/";
                string PathBanner = WebConfigurationManager.AppSettings["IMAGE_HTTP"] + WebConfigurationManager.AppSettings["DIRECTORY"] + "/" + WebConfigurationManager.AppSettings["FOLDER_BANNER"] + "/";
                long cityId = 0;
                if (ControllerContext.HttpContext.Request.Cookies["CityCookie"] != null && ControllerContext.HttpContext.Request.Cookies["CityCookie"].Value != string.Empty)
                {
                    cityId = Convert.ToInt32(ControllerContext.HttpContext.Request.Cookies["CityCookie"].Value.Split('$')[0].Trim());
                }

                merchant = db.Merchants.FirstOrDefault(m => m.Id == id && (cityId == 0 || m.City == cityId));
                if (merchant == null)
                {
                    return RedirectToAction("List");
                }
                if (merchant.Status.ToLower() == "approve" && (merchant.ApproveDate != null && merchant.ApproveDate.Value.AddYears(merchant.ValidityPeriod.Value) >= today))
                {
                    //photo path
                    merchant.PhotoImageURL = db.MerchantKYCs.Where(k => k.MerchantID == id).Count() > 0 ? Path + "/" + db.MerchantKYCs.Where(k => k.MerchantID == id).Select(k => k.PhotoImageUrl).FirstOrDefault() : string.Empty;
                    merchant.BannerImageURL = db.merchantBanners.Where(k => k.MerchantID == id && k.IsActive == true).ToList();

                    //ratings
                    merchant.Rating = db.MerchantRatings.Where(k => k.MerchantID == id && k.IsDisplay == true).Count() > 0 ? db.MerchantRatings.Where(k => k.MerchantID == id && k.IsDisplay == true).Select(k => k.Rating).Average() : 0;

                    //total reviews
                    merchant.NoOfReviews = db.MerchantRatings.Where(k => k.MerchantID == id && k.IsDisplay == true).Count();





                    List<long?> serviceIds = db.Merchants.Where(m => m.City == cityId && m.Status.ToLower() == "approve" && (m.ApproveDate != null && SqlFunctions.DateAdd("year", m.ValidityPeriod.Value, m.ApproveDate.Value) >= today)).Select(m => m.Category).ToList();

                    List<ServiceMaster> serviceMasters = db.ServiceMasters.Where(s => s.IsActive == true && s.Id != id && serviceIds.Contains(s.Id)).OrderBy(s => s.Name).ToList();
                    ViewBag.Services = serviceMasters;

                    ViewBag.HolidayList = String.Join(", ", db.HolidayMasters.Where(h => (db.MerchantHolidays.Where(p => p.MerchantID == id).Select(p => p.HolidayID).ToList().Contains(h.Id))).OrderBy(h => h.CreateDate).Select(h => h.Name).ToList());
                }
                decimal commission = merchant.CommissionMasterDetail.Commission;
                decimal BuyerCommssion = 0;
                MerchantDetails details = db.merchantDetails.FirstOrDefault(p => p.MerchantId == merchant.Id);
                if (details != null)
                {
                    BuyerCommssion = details.Level0;
                }
                else
                {
                    ServiceIncomeMaster incomeMaster = db.ServiceIncomeMasters.FirstOrDefault();
                    BuyerCommssion = incomeMaster.UserLevel0;
                }
                merchant.CasbackToBuyer = Math.Round((commission * BuyerCommssion) / 100, 1);
                return View(merchant);
            }
            catch (Exception ex)
            {
                errStr.Append("Method Name[Http Request] :- List[HttpGet]" + Environment.NewLine +
                              "ON Dated" + DateTime.UtcNow.AddHours(5.30).TimeOfDay + Environment.NewLine +
                                  ex.Message.ToString() + Environment.NewLine +
                        "====================================================================================="
                              );
                //ViewBag.Message = "Sorry! Problem in customer registration!!";
                ModelState.AddModelError("Message", "Sorry! Problem in Generate Create View!!");
                ErrorLog.ErrorLogFile("Sorry! Problem in Generate Create View " + Environment.NewLine + errStr.ToString()
                    , ErrorLog.Module.Customer, System.Web.HttpContext.Current.Server);


                return View();

            }

        }

        [HttpGet]
        public ActionResult Preview(long? id)
        {
            try
            {
                DateTime today = DateTime.UtcNow.AddHours(5.30);
                ModelLayer.Models.Merchant merchant = new ModelLayer.Models.Merchant();
                string Path = WebConfigurationManager.AppSettings["IMAGE_HTTP"] + WebConfigurationManager.AppSettings["DIRECTORY"] + "/" + WebConfigurationManager.AppSettings["FOLDER_PHOTO"] + "/";
                string PathBanner = WebConfigurationManager.AppSettings["IMAGE_HTTP"] + WebConfigurationManager.AppSettings["DIRECTORY"] + "/" + WebConfigurationManager.AppSettings["FOLDER_BANNER"] + "/";

                merchant = db.Merchants.Find(id);
                if (merchant == null)
                {
                    throw new Exception("Not found details");
                }

                //photo path
                merchant.PhotoImageURL = db.MerchantKYCs.Where(k => k.MerchantID == id).Count() > 0 ? Path + "/" + db.MerchantKYCs.Where(k => k.MerchantID == id).Select(k => k.PhotoImageUrl).FirstOrDefault() : string.Empty;
                merchant.BannerImageURL = db.merchantBanners.Where(k => k.MerchantID == id && k.IsActive == true).ToList();

                //ratings
                merchant.Rating = db.MerchantRatings.Where(k => k.MerchantID == id && k.IsDisplay == true).Count() > 0 ? db.MerchantRatings.Where(k => k.MerchantID == id && k.IsDisplay == true).Select(k => k.Rating).Average() : 0;

                //total reviews
                merchant.NoOfReviews = db.MerchantRatings.Where(k => k.MerchantID == id && k.IsDisplay == true).Count();

                long cityId = 0;

                if (ControllerContext.HttpContext.Request.Cookies["CityCookie"] != null && ControllerContext.HttpContext.Request.Cookies["CityCookie"].Value != string.Empty)
                {
                    cityId = Convert.ToInt32(ControllerContext.HttpContext.Request.Cookies["CityCookie"].Value.Split('$')[0].Trim());
                }

                List<long?> serviceIds = db.Merchants.Where(m => m.City == cityId && m.Status.ToLower() == "approve" && (m.ApproveDate != null && SqlFunctions.DateAdd("year", m.ValidityPeriod.Value, m.ApproveDate.Value) >= today)).Select(m => m.Category).ToList();

                List<ServiceMaster> serviceMasters = db.ServiceMasters.Where(s => s.IsActive == true && s.Id != id && serviceIds.Contains(s.Id)).OrderBy(s => s.Name).ToList();
                ViewBag.Services = serviceMasters;

                return View(merchant);
            }
            catch (Exception ex)
            {
                errStr.Append("Method Name[Http Request] :- List[HttpGet]" + Environment.NewLine +
                              "ON Dated" + DateTime.UtcNow.AddHours(5.30).TimeOfDay + Environment.NewLine +
                                  ex.Message.ToString() + Environment.NewLine +
                        "====================================================================================="
                              );
                //ViewBag.Message = "Sorry! Problem in customer registration!!";
                ModelState.AddModelError("Message", "Sorry! Problem in Generate Create View!!");
                ErrorLog.ErrorLogFile("Sorry! Problem in Generate Create View " + Environment.NewLine + errStr.ToString()
                    , ErrorLog.Module.Customer, System.Web.HttpContext.Current.Server);


                return View();

            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SubmitReview(MerchantRating merchantRating)
        {
            try
            {
                if (Session["UID"] == null)
                {
                    throw new Exception("Invalid Login");
                }
                merchantRating.CustomerID = Convert.ToInt64(Session["UID"]);
                MerchantRating obj = db.MerchantRatings.Where(a => a.MerchantID == merchantRating.MerchantID && a.CustomerID == merchantRating.CustomerID).FirstOrDefault();

                if (obj == null)
                {
                    obj = merchantRating;
                    obj.IsDisplay = true;

                    obj.CreateDate = DateTime.Now;
                    db.MerchantRatings.Add(merchantRating);
                }
                else
                {
                    obj.IsDisplay = true;
                    obj.Rating = merchantRating.Rating;
                    obj.Review = merchantRating.Review;
                }
                db.SaveChanges();
                return RedirectToAction("Details", new { id = merchantRating.MerchantID });
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet]
        public ActionResult GetMerchantRatingList(int MerchantID)
        {
            var lst = db.MerchantRatings.Where(a => a.MerchantID == MerchantID && a.IsDisplay == true);
            return PartialView("_ShowAllRatingsList", lst.ToList());
        }
        #endregion

        public ActionResult SendOTPToValidateMo(long ID)
        {
            ModelLayer.Models.Merchant obj = db.Merchants.FirstOrDefault(p => p.Id == ID && p.Status == "Accept");
            if (obj != null)
            {
                MerchantsLogin login = db.merchantLogins.FirstOrDefault(p => p.MerchantID == obj.Id && p.IsActive == true);
                if (login == null)
                {
                    Session["OTPSent"] = null;
                    ViewBag.MerchantName = obj.ContactPerson;
                    ViewBag.ID = obj.Id;
                    return View();
                }
            }
            return RedirectToAction("LinkExpiry");
        }

        public void CheckRmIsBBPSubscriber(string MobileNO)
        {
            UserLogin userLogin = db.UserLogins.FirstOrDefault(P => P.Mobile == MobileNO);
            if (userLogin != null)
            {
                BoosterPlanSubscriber boosterPlanSubscriber = db.BoosterPlanSubscribers.FirstOrDefault(p => p.CreateBy == userLogin.ID && p.IsActive == true);
                if (boosterPlanSubscriber == null)
                {
                    string Text = " Please purchased from the business booster category before this month's payout to get the benefit of the Relationship Manager commission of Market Place Income Plan. Thanks, Team Ezeelo";
                    PersonalDetail personalDetail = db.PersonalDetails.FirstOrDefault(p => p.UserLoginID == userLogin.ID);
                    Dictionary<string, string> dictSMSValues = new Dictionary<string, string>();
                    dictSMSValues.Add("#--NAME--#", personalDetail.FirstName.Trim());
                    dictSMSValues.Add("#--TEXT--#", Text);

                    BusinessLogicLayer.GateWay gateWay = new BusinessLogicLayer.SMS(System.Web.HttpContext.Current.Server);
                    gateWay.SendSMS(BusinessLogicLayer.GateWay.SMSGateWays.SUMIT, BusinessLogicLayer.GateWay.SMSOptions.SINGLE, BusinessLogicLayer.GateWay.SMSTypes.KYC_CMPT_REQUEST, new string[] { MobileNO }, dictSMSValues);

                }
            }
        }

        [HttpGet]
        public ActionResult GoOnline(int merchantId)
        {
            string redirectURL = "";
            try
            {
                long userLoginID = Convert.ToInt64(Session["UID"]);
                var merchant = db.Merchants.Find(merchantId);
                var customer = db.UserLogins.Where(x => x.ID == userLoginID).FirstOrDefault();
                string onlineEncKey = "749HB97HGG68oYuQ";
                if (String.IsNullOrEmpty(customer.OnlineUserHash))
                {
                    string onlineSalt = "JH35KHKP9I9KLO";
                    string hashString = customer.Mobile + onlineSalt;
                    customer.OnlineUserHash = sha256_hash(hashString);
                    db.SaveChanges();
                }
                string customerInfo = merchant.OnlineMerchantHash + "|" + customer.Email + "|" + customer.Mobile + "|" + customer.ID.ToString() + "|" + customer.OnlineUserHash;
                string encryptedCustomerParam = Encrypt(customerInfo, onlineEncKey);
                redirectURL = ("http://marketplace.ezeelo.com/processbridge?info=" + HttpUtility.UrlEncode(encryptedCustomerParam) + "&type=customer");
                return Json(redirectURL, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public String sha256_hash(String value)
        {
            StringBuilder Sb = new StringBuilder();

            using (SHA256 hash = SHA256Managed.Create())
            {
                Encoding enc = Encoding.UTF8;
                Byte[] result = hash.ComputeHash(enc.GetBytes(value));

                foreach (Byte b in result)
                    Sb.Append(b.ToString("x2"));
            }

            return Sb.ToString();
        }

        private const int Keysize = 128;

        // This constant determines the number of iterations for the password bytes generation function.
        private const int DerivationIterations = 1000;

        public static string Encrypt(string plainText, string passPhrase)
        {
            // Salt and IV is randomly generated each time, but is preprended to encrypted cipher text
            // so that the same Salt and IV values can be used when decrypting.  
            var saltStringBytes = Generate256BitsOfRandomEntropy();
            var ivStringBytes = Generate256BitsOfRandomEntropy();
            var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            using (var password = new Rfc2898DeriveBytes(passPhrase, saltStringBytes, DerivationIterations))
            {
                var keyBytes = password.GetBytes(Keysize / 8);
                using (var symmetricKey = new RijndaelManaged())
                {
                    symmetricKey.BlockSize = 128;
                    symmetricKey.Mode = CipherMode.CBC;
                    symmetricKey.Padding = PaddingMode.PKCS7;
                    using (var encryptor = symmetricKey.CreateEncryptor(keyBytes, ivStringBytes))
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                            {
                                cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
                                cryptoStream.FlushFinalBlock();
                                // Create the final bytes as a concatenation of the random salt bytes, the random iv bytes and the cipher bytes.
                                var cipherTextBytes = saltStringBytes;
                                cipherTextBytes = cipherTextBytes.Concat(ivStringBytes).ToArray();
                                cipherTextBytes = cipherTextBytes.Concat(memoryStream.ToArray()).ToArray();
                                memoryStream.Close();
                                cryptoStream.Close();
                                return Convert.ToBase64String(cipherTextBytes);
                            }
                        }
                    }
                }
            }
        }

        public static string Decrypt(string cipherText, string passPhrase)
        {
            // Get the complete stream of bytes that represent:
            // [32 bytes of Salt] + [32 bytes of IV] + [n bytes of CipherText]
            var cipherTextBytesWithSaltAndIv = Convert.FromBase64String(cipherText);
            // Get the saltbytes by extracting the first 32 bytes from the supplied cipherText bytes.
            var saltStringBytes = cipherTextBytesWithSaltAndIv.Take(Keysize / 8).ToArray();
            // Get the IV bytes by extracting the next 32 bytes from the supplied cipherText bytes.
            var ivStringBytes = cipherTextBytesWithSaltAndIv.Skip(Keysize / 8).Take(Keysize / 8).ToArray();
            // Get the actual cipher text bytes by removing the first 64 bytes from the cipherText string.
            var cipherTextBytes = cipherTextBytesWithSaltAndIv.Skip((Keysize / 8) * 2).Take(cipherTextBytesWithSaltAndIv.Length - ((Keysize / 8) * 2)).ToArray();

            using (var password = new Rfc2898DeriveBytes(passPhrase, saltStringBytes, DerivationIterations))
            {
                var keyBytes = password.GetBytes(Keysize / 8);
                using (var symmetricKey = new RijndaelManaged())
                {
                    symmetricKey.BlockSize = 128;
                    symmetricKey.Mode = CipherMode.CBC;
                    symmetricKey.Padding = PaddingMode.PKCS7;
                    using (var decryptor = symmetricKey.CreateDecryptor(keyBytes, ivStringBytes))
                    {
                        using (var memoryStream = new MemoryStream(cipherTextBytes))
                        {
                            using (var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                            {
                                var plainTextBytes = new byte[cipherTextBytes.Length];
                                var decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
                                memoryStream.Close();
                                cryptoStream.Close();
                                return Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount);
                            }
                        }
                    }
                }
            }
        }

        private static byte[] Generate256BitsOfRandomEntropy()
        {
            var randomBytes = new byte[16]; // 32 Bytes will give us 256 bits.
            using (var rngCsp = new RNGCryptoServiceProvider())
            {
                // Fill the array with cryptographically secure random bytes.
                rngCsp.GetBytes(randomBytes);
            }
            return randomBytes;
        }
    }
}

