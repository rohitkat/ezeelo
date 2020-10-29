using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using ModelLayer.Models;
using System.Linq;
using BusinessLogicLayer;
using ModelLayer.Models.ViewModel;
using System.Configuration;
using MarketPartner.Filter;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.IO;
using System.Web.Configuration;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Net;
using QRCoder;
using System.Text;
using System.Security.Cryptography;

namespace MarketPartner.Controllers
{
    public class HomeController : Controller
    {
        EzeeloDBContext db = new EzeeloDBContext();
        BoosterPlanData BD = new BoosterPlanData();
        MerchantNotification merchantNotification = new MerchantNotification();

        [SessionExpire]
        [Authorize]
        public ActionResult Index()
        {
            long MerchantID = Convert.ToInt64(Session["MerchantID"]);
            MarketPlaceDashboardViewModel obj = new MarketPlaceDashboardViewModel();
            obj = SetDashboardData(MerchantID);
            Session["ShopNAme"] = obj.ShopName;
            Session["ContactPerson"] = obj.ContactPerson;
            Session["ShopImgPath"] = obj.ShopImagePath;
            if(!db.MerchantKYCs.Any(p => p.MerchantID == MerchantID && p.IsVerified == true))
            {
                ViewBag.IsKYCCompleted = 0;
            }
            return View(obj);
        }
        public ActionResult login()
        {
            MerchantsLogin merchants = new MerchantsLogin();
            return View(merchants);
        }
        [HttpPost]
        public ActionResult login(MerchantsLogin merchants, string returnurl)
        {
            //create the authentication ticket
            MerchantsLogin obj_ = db.merchantLogins.FirstOrDefault(p => p.UserID == merchants.UserID);
            if (obj_ != null)
            {
                if (obj_.Password == merchants.Password)
                {
                    if (obj_.IsActive)
                    {
                        SetLoginCookies(obj_.MerchantID.ToString());
                        if (returnurl != null && returnurl != "" && returnurl != "/")
                        {
                            return Redirect(returnurl);
                        }
                        else
                        {
                            return RedirectToAction("Index");
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("Password", "It seems your account is locked. Please contact with admin!!!");
                        return View(merchants);
                    }
                }
                else
                {
                    ModelState.AddModelError("Password", "Incorrect Password!!!");
                    return View(merchants);
                }
            }
            else
            {
                if (db.Merchants.Any(p => p.ContactNumber == merchants.UserID))
                {
                    ModelState.AddModelError("UserID", "It Seems Your Registration Process Is Not Completed Yet. Please Contact with Admin!");
                }
                else
                {
                    ModelState.AddModelError("UserID", "This mobile number is not registered as Merchant!!!");
                }
                return View(merchants);
            }

        }

        public ActionResult ByPassLogin(long MerchantId)
        {
            SetLoginCookies(MerchantId.ToString());
            return RedirectToAction("Index", "Home", new { flag = 0 });
        }
        public MarketPlaceDashboardViewModel SetDashboardData(long ID)
        {
            MarketPlaceDashboardViewModel obj = new MarketPlaceDashboardViewModel();
            Merchant merchant = db.Merchants.FirstOrDefault(p => p.Id == ID);
            if (merchant != null)
            {
                MerchantKYC kYC = db.MerchantKYCs.FirstOrDefault(p => p.MerchantID == ID);
                obj.ShopName = merchant.FranchiseName;
                obj.ContactPerson = merchant.ContactPerson;
                if (kYC != null)
                {
                    obj.ShopImagePath = ConfigurationManager.AppSettings["IMAGE_HTTP"] + ConfigurationManager.AppSettings["MERCHANT_DIRECTORY"] + "/" + ConfigurationManager.AppSettings["MERCHANT_FOLDER_PHOTO"] + "/" + kYC.PhotoImageUrl;
                }
                List<MarketPlaceDashboardViewModel> list = new List<MarketPlaceDashboardViewModel>();
                List<TransReport> transReports = new List<TransReport>();
                List<RatingReport> ratingReports = new List<RatingReport>();
                List<SqlParameter> sp = new List<SqlParameter>()
                {
                    new SqlParameter() {ParameterName = "@MerchantId", SqlDbType = SqlDbType.BigInt, Value= ID},
                };
                DataSet ds = new DataSet();
                ds = BD.GetData("MerchantUserDashboard", sp);
                if (ds.Tables.Count > 0)
                {
                    list = BusinessLogicLayer.Helper.CreateListFromTable<MarketPlaceDashboardViewModel>(ds.Tables[0]);
                }
                if (ds.Tables.Count > 1)
                {
                    transReports = BusinessLogicLayer.Helper.CreateListFromTable<TransReport>(ds.Tables[1]);
                }
                if (ds.Tables.Count > 1)
                {
                    ratingReports = BusinessLogicLayer.Helper.CreateListFromTable<RatingReport>(ds.Tables[2]);
                }

                obj.BillAmount = list.FirstOrDefault().BillAmount;
                obj.Commission = list.FirstOrDefault().Commission;
                obj.DaysLeft = list.FirstOrDefault().DaysLeft;
                obj.PendingRecharge = list.FirstOrDefault().PendingRecharge;
                obj.Rating = list.FirstOrDefault().Rating;
                obj.Recharge = list.FirstOrDefault().Recharge;
                obj.TotalTransaction = list.FirstOrDefault().TotalTransaction;
                obj.TotalUser = list.FirstOrDefault().TotalUser;
                obj.RechargeRemark = list.FirstOrDefault().RechargeRemark;
                obj.RatingRemark = list.FirstOrDefault().RatingRemark;
                obj.DaysLeftRemark = list.FirstOrDefault().DaysLeftRemark;
                obj.PendingTransactionValue = list.FirstOrDefault().PendingTransactionValue;
                obj.TransactionList = transReports;
                obj.RatingList = ratingReports;
                obj.MerchantId = ID;
                obj.month = DateTime.Now.Month;
                obj.year = DateTime.Now.Year;
            }
            return obj;
        }

        public ActionResult GetTrasactionBarChartData(long MerchantId, int Month, int Year)
        {
            List<BarChart> BarChart = new List<BarChart>();
            List<SqlParameter> sp = new List<SqlParameter>()
                {
                    new SqlParameter() {ParameterName = "@MerchantId", SqlDbType = SqlDbType.BigInt, Value= MerchantId},
                    new SqlParameter() {ParameterName = "@Month", SqlDbType = SqlDbType.BigInt, Value= Month},
                    new SqlParameter() {ParameterName = "@Year", SqlDbType = SqlDbType.BigInt, Value= Year},
                };
            DataSet ds = new DataSet();
            ds = BD.GetData("MerchantDashboardBarChartData", sp);
            if (ds.Tables.Count > 0)
            {
                BarChart = BusinessLogicLayer.Helper.CreateListFromTable<BarChart>(ds.Tables[0]);
            }
            return Json(BarChart, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetUsetPieChartData(long MerchantId)
        {
            List<PieChart> PieChart = new List<PieChart>();
            List<SqlParameter> sp = new List<SqlParameter>()
                {
                    new SqlParameter() {ParameterName = "@MerchantId", SqlDbType = SqlDbType.BigInt, Value= MerchantId}
                };
            DataSet ds = new DataSet();
            ds = BD.GetData("MerchantDashbardUsersChartData", sp);
            if (ds.Tables.Count > 0)
            {
                PieChart = BusinessLogicLayer.Helper.CreateListFromTable<PieChart>(ds.Tables[0]);
            }
            return Json(PieChart, JsonRequestBehavior.AllowGet);
        }
        public void SetLoginCookies(string ID)
        {
            Session["MerchantID"] = ID;
            var authTicket = new FormsAuthenticationTicket(
          1,
          ID, //user id
          DateTime.Now,
          DateTime.Now.AddMinutes(20), // expiry
          true, //true to remember
          "", //roles
          "/"
          );
            //encrypt the ticket and add it to a cookie
            HttpCookie cookie = new HttpCookie(FormsAuthentication.FormsCookieName, FormsAuthentication.Encrypt(authTicket));
            Response.Cookies.Add(cookie);
        }

        public ActionResult SetPassword(long id)
        {
            MerchantsLogin merchants = db.merchantLogins.FirstOrDefault(p => p.MerchantID == id);

            Merchant merchant = db.Merchants.FirstOrDefault(p => p.Id == id);
            if (merchants == null || merchant == null)
            {
                return View("~/Views/Shared/404.cshtml");
            }
            else
            {
                if (merchants.Password == null || merchants.Password == "")
                {
                    merchants.UserID = "";
                    ViewBag.MerchantId = id;
                    ViewBag.MerchantShopNAme = merchant.FranchiseName;
                }
                else
                {
                    return View("~/Views/Shared/404.cshtml");
                }
            }
            return View(merchants);
        }

        [HttpPost]
        public ActionResult SetPassword(MerchantsLogin merchants)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    MerchantsLogin merchants_ = db.merchantLogins.FirstOrDefault(p => p.MerchantID == merchants.MerchantID);
                    merchants_.Password = merchants.Password;
                    merchants_.ModifyDate = DateTime.Now;
                    merchants_.ModifyBy = "User";
                    merchants_.NetworkIP = CommonFunctions.GetClientIP();
                    merchants_.IsActive = true;
                    merchants_.ConfirmPassword = merchants_.Password;
                    db.SaveChanges();
                    //SendMailSMSForKYC(merchants.MerchantID);
                    SetLoginCookies(merchants.MerchantID.ToString());
                    return RedirectToAction("Index", "Home", new { flag = 1 });
                }
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException dbEx)
            {
                Exception raise = dbEx;
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        string message = string.Format("{0}:{1}",
                            validationErrors.Entry.Entity.ToString(),
                            validationError.ErrorMessage);
                        raise = new InvalidOperationException(message, raise);
                    }
                }
                throw raise;
            }
            return View();
        }
        public void SendMailSMSForKYC(long MerchantID)
        {
            Merchant obj = db.Merchants.FirstOrDefault(p => p.Id == MerchantID);
            try
            {
                string KYCURL = ConfigurationManager.AppSettings["EZEELO_CUSTOMER_URL"] + "nagpur/1060/merchant/KYC?id=" + MerchantID;

                Dictionary<string, string> dictEmailValues = new Dictionary<string, string>();
                dictEmailValues.Add("<!--NAME-->", obj.ContactPerson);
                dictEmailValues.Add("<!--SHOP-->", obj.FranchiseName);
                dictEmailValues.Add("<!--LINK-->", KYCURL);
                BusinessLogicLayer.GateWay gateWay = new BusinessLogicLayer.Email(System.Web.HttpContext.Current.Server);
                gateWay.SendEmail(BusinessLogicLayer.GateWay.EmailGateWays.GANDHIBAGH, BusinessLogicLayer.GateWay.SenderMail.INFO, BusinessLogicLayer.GateWay.EMailTypes.ACCEPT_MERCHANT, new string[] { obj.Email }, dictEmailValues, true);

            }
            catch(Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                   + Environment.NewLine + ex.Message + Environment.NewLine
                   + "[FranchiseLocation][POST:SendMailSMSForKYC-Mail]",
                   BusinessLogicLayer.ErrorLog.Module.MarketPartner, System.Web.HttpContext.Current.Server);
            }
            try
            {
                Dictionary<string, string> dictSMSValues_ = new Dictionary<string, string>();
                BusinessLogicLayer.GateWay gateWay_ = new BusinessLogicLayer.SMS(System.Web.HttpContext.Current.Server);
                dictSMSValues_.Add("#--NAME--#", obj.ContactPerson);
                gateWay_.SendSMS(BusinessLogicLayer.GateWay.SMSGateWays.SUMIT,
                    BusinessLogicLayer.GateWay.SMSOptions.SINGLE, BusinessLogicLayer.GateWay.SMSTypes.MERCHANT_REG_ACCEPT,
                    new string[] { obj.ContactNumber }, dictSMSValues_);
            }
            catch(Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                   + Environment.NewLine + ex.Message + Environment.NewLine
                   + "[FranchiseLocation][POST:SendMailSMSForKYC-SMS]",
                   BusinessLogicLayer.ErrorLog.Module.MarketPartner, System.Web.HttpContext.Current.Server);
            }
        }
        public JsonResult ValidateContactNo(string contactNo, long MerchantId)
        {
            bool result = false;
            try
            {
                Merchant merchant = db.Merchants.FirstOrDefault(p => p.Id == MerchantId && p.ContactNumber == contactNo);
                if (merchant != null)
                {
                    result = true;
                }
            }
            catch (Exception ex)
            {
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult ValidateContactNoL(string contactNo)
        {
            string result = "";
            try
            {
                Merchant merchant = db.Merchants.FirstOrDefault(p => p.ContactNumber == contactNo);
                MerchantsLogin login = db.merchantLogins.FirstOrDefault(p => p.UserID == merchant.ContactNumber);
                if (merchant != null)
                {
                    if (login == null)
                    {
                        result = "$";
                    }
                    else if (string.IsNullOrEmpty(login.Password))
                    {
                        result = "$$";
                    }
                    else
                    {
                        result = merchant.FranchiseName;
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [SessionExpire]
        [Authorize]
        public ActionResult MyProfile()
        {
            long MerchantId = Convert.ToInt64(Session["MerchantID"]);
            Merchant obj = new Merchant();
            try
            {
                obj = db.Merchants.FirstOrDefault(p => p.Id == MerchantId);
                ViewBag.State = db.States.Where(c => c.IsActive == true).OrderBy(c => c.Name).ToList();
                List<long> ldistrict = db.Districts.Where(x => x.StateID == obj.State).Select(x => x.ID).ToList();
                ViewBag.City = db.Cities.Where(c => ldistrict.Contains(c.DistrictID)).Select(c => new StateCityFranchiseMerchantViewModel { CityID = c.ID, CityName = c.Name }).ToList();
                ViewBag.Pincode = db.Pincodes.Where(x => x.CityID == obj.City).Select(x => new { Name = x.Name, ID = x.ID }).OrderBy(x => x.Name).ToList();
                ViewBag.Category = db.ServiceMasters.Where(c => c.IsActive == true).OrderBy(c => c.Name).ToList();
                ViewBag.Comission = db.CommissionMasters.Where(c => c.IsActive == true).OrderBy(c => c.Commission).ToList();
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
                ViewBag.ShopTiming = new SelectList(shopTimingList, "Id", "Time");
                ViewBag.Holiday = db.HolidayMasters.Where(c => c.IsActive == true).OrderBy(c => c.Name).ToList();
                ViewBag.Holidays = String.Join(",", db.MerchantHolidays.Where(p => p.MerchantID == obj.Id).Select(p => p.HolidayID).ToList());
                if (obj.GSTINNo == null)
                {
                    ViewBag.NotApplicableGST = true;
                }
                else
                {
                    ViewBag.NotApplicableGST = false;
                }
                obj.IsKYCComplete = db.MerchantKYCs.Any(p => p.MerchantID == obj.Id && p.IsCompleted);
                if (obj.Status == "Approve" && obj.ApproveDate != null)
                {
                    DateTime LastDate = obj.ApproveDate.Value.AddYears(obj.ValidityPeriod.Value);
                    if (DateTime.Now <= LastDate)
                    {
                        TimeSpan timeSpan = LastDate - DateTime.Now;
                        ViewBag.RemainingDays = timeSpan.Days.ToString();
                    }
                    else
                    {
                        ViewBag.RemainingDays = "E";
                    }
                }
            }
            catch (Exception ex)
            {

            }

            MerchantProfile merchantProfile = db.merchantProfiles.Where(p => p.MerchantId == MerchantId && p.Status == "Pending").OrderByDescending(p => p.CreateDate).Take(1).FirstOrDefault();
            if (merchantProfile != null)
            {
                obj.Remark = "Profile update request sent to Admin panel, once it is approved by admin data will update to your profile ";
            }
            merchantProfile = db.merchantProfiles.Where(p => p.MerchantId == MerchantId).OrderByDescending(p => p.CreateDate).Take(1).FirstOrDefault();
            if (merchantProfile != null)
            {
                if (merchantProfile.Status == "Rejected")
                {
                    ViewBag.Status = "Your last profile update request is rejected by Admin.";
                }
            }
            return View(obj);
        }

        [SessionExpire]
        [Authorize]
        [HttpPost]
        public ActionResult MyProfile(Merchant obj, List<string> Holiday)
        {
            MerchantProfile merchant = new MerchantProfile();
            merchant.FranchiseName = obj.FranchiseName;
            merchant.GSTINNo = obj.GSTINNo;
            merchant.PANNo = obj.PANNo;
            merchant.Address = obj.Address;
            merchant.State = obj.State;
            merchant.City = obj.City;
            merchant.Pincode = obj.Pincode;
            merchant.ShopTiming = obj.ShopTiming;
            //holiday
            merchant.ContactPerson = obj.ContactPerson;
            merchant.ContactNumber = obj.ContactNumber;
            merchant.Email = obj.Email;
            merchant.Category = obj.Category;
            merchant.Comission = obj.Comission;
            merchant.GoogleMapLink = obj.GoogleMapLink;
            merchant.SpecialRemark = obj.SpecialRemark;
            merchant.MerchantId = obj.Id;
            merchant.Status = "Pending";
            merchant.CreateDate = DateTime.Now;
            db.merchantProfiles.Add(merchant);
            db.SaveChanges();
            if (Holiday != null)
            {
                foreach (var item in Holiday)
                {
                    MerchantHolidayUpdateRequest merchantHoliday = new MerchantHolidayUpdateRequest();
                    merchantHoliday.MerchantProfileID = merchant.Id;
                    merchantHoliday.HolidayID = Convert.ToInt32(item);
                    db.merchantHolidayUpdates.Add(merchantHoliday);
                }
            }
            db.SaveChanges();
            Merchant obj_ = db.Merchants.FirstOrDefault(p => p.Id == merchant.MerchantId);
            merchantNotification.SaveNotification(4, obj_.FranchiseName);
            TempData["SuccessMsg"] = "Profile update request sent to Admin";
            return RedirectToAction("Index");
        }

        [SessionExpire]
        [Authorize]
        public ActionResult BannerList()
        {
            long MerchantId = Convert.ToInt64(Session["MerchantID"]);
            MerchantShopImages obj = new MerchantShopImages();
            MerchantKYC merchant = db.MerchantKYCs.FirstOrDefault(p => p.MerchantID == MerchantId);
            obj.ShopImagePath = ConfigurationManager.AppSettings["IMAGE_HTTP"] + ConfigurationManager.AppSettings["MERCHANT_DIRECTORY"] + "/" + ConfigurationManager.AppSettings["MERCHANT_FOLDER_PHOTO"] + "/" + merchant.PhotoImageUrl;
            List<MerchantBanner> banners = db.merchantBanners.Where(p => p.MerchantID == MerchantId).ToList();
            foreach (var item in banners)
            {
                item.BannerPath = ConfigurationManager.AppSettings["IMAGE_HTTP"] + ConfigurationManager.AppSettings["MERCHANT_DIRECTORY"] + "/" + ConfigurationManager.AppSettings["MERCHANT_FOLDER_BANNER"] + "/" + item.BannerPath;
            }
            List<MerchantBannerUpdateRequest> updateRequests = db.merchantBannerUpdateRequests.Where(p => p.MerchantID == MerchantId && p.IsActive == false).ToList();
            foreach (var item in updateRequests)
            {
                if (item.ShopPath == "True" || item.ShopPath == "False")
                {
                    if (item.ShopPath == "True")
                    {
                        item.Remark = "Existing Banner image set to Display ON";
                    }
                    else
                    {
                        item.Remark = "Existing Banner image set to Display OFF";
                    }
                    item.BannerPath = ConfigurationManager.AppSettings["IMAGE_HTTP"] + ConfigurationManager.AppSettings["MERCHANT_DIRECTORY"] + "/" + ConfigurationManager.AppSettings["MERCHANT_FOLDER_BANNER"] + "/" + item.BannerPath;
                }
                else if (item.ShopPath == null || item.ShopPath == "")
                {
                    if (item.BannerPath != null)
                    {
                        item.Remark = "New Banner image added.";
                        item.BannerPath = ConfigurationManager.AppSettings["IMAGE_HTTP"] + ConfigurationManager.AppSettings["MERCHANT_DIRECTORY"] + "/" + ConfigurationManager.AppSettings["MERCHANT_FOLDER_BANNER"] + "/" + item.BannerPath;
                    }
                }
                else
                {
                    if (item.BannerPath != null)
                    {
                        item.Remark = "New Banner and Shop image added.";
                        item.BannerPath = ConfigurationManager.AppSettings["IMAGE_HTTP"] + ConfigurationManager.AppSettings["MERCHANT_DIRECTORY"] + "/" + ConfigurationManager.AppSettings["MERCHANT_FOLDER_BANNER"] + "/" + item.BannerPath;
                        item.ShopPath = ConfigurationManager.AppSettings["IMAGE_HTTP"] + ConfigurationManager.AppSettings["MERCHANT_DIRECTORY"] + "/" + ConfigurationManager.AppSettings["MERCHANT_FOLDER_PHOTO"] + "/" + item.ShopPath;
                    }
                    else
                    {
                        item.Remark = "New Shop image added.";
                        item.ShopPath = ConfigurationManager.AppSettings["IMAGE_HTTP"] + ConfigurationManager.AppSettings["MERCHANT_DIRECTORY"] + "/" + ConfigurationManager.AppSettings["MERCHANT_FOLDER_PHOTO"] + "/" + item.ShopPath;
                    }
                }
            }
            obj.list = banners.OrderByDescending(p => p.CreateDate).OrderByDescending(p => p.ModifyDate).ToList();
            obj.requestList = updateRequests.OrderByDescending(p => p.CreateDate).ToList();
            return View(obj);
        }

        [SessionExpire]
        [Authorize]
        [HttpPost]
        public ActionResult BannerList(HttpPostedFileBase photo, HttpPostedFileBase Banner)
        {
            try
            {
                long MerchantId = Convert.ToInt64(Session["MerchantID"]);
                if (photo == null && Banner == null)
                {
                    TempData["WarningMsg"] = "No Image to save!!!";
                    return RedirectToAction("BannerList");
                }
                var allowedExtensions = new[] { ".Jpg", ".png", ".jpg", "jpeg" };
                string Filename = "";
                string Ext = "";
                string UniqueCode = "_" + DateTime.Now.ToString("ddMMyyHHmmss");
                MerchantBannerUpdateRequest updateRequest = new MerchantBannerUpdateRequest();
                if (photo != null)
                {
                    var fileNamePhoto = Path.GetFileName(photo.FileName);
                    var ExtPhoto = Path.GetExtension(photo.FileName);
                    string namePhoto = Path.GetFileNameWithoutExtension(fileNamePhoto);
                    string photoPath = namePhoto + "_" + MerchantId + UniqueCode + ExtPhoto;

                    if (allowedExtensions.Contains(ExtPhoto)) //check what type of extension  
                    {
                        UploadImage((int)Document_Type.PHOTO, photo, (long)MerchantId, System.Web.HttpContext.Current.Server, out Filename, out Ext);
                        updateRequest.ShopPath = Filename;
                    }
                    else
                    {
                        TempData["WarningMsg"] = "Please choose only Image file in Shop Photo";
                        return RedirectToAction("BannerList");
                    }
                }
                if (Banner != null)
                {
                    var fileNameBanner = Path.GetFileName(Banner.FileName);
                    var ExtBanner = Path.GetExtension(Banner.FileName);
                    string nameBanner = Path.GetFileNameWithoutExtension(fileNameBanner);
                    string CancelledBanner = nameBanner + "_" + MerchantId + UniqueCode + ExtBanner;

                    if (allowedExtensions.Contains(ExtBanner)) //check what type of extension  
                    {
                        UploadImage((int)Document_Type.BANNER, Banner, (long)MerchantId, System.Web.HttpContext.Current.Server, out Filename, out Ext);
                        updateRequest.BannerPath = Filename;
                    }
                    else
                    {
                        TempData["WarningMsg"] = "Please choose only Image file in Banner";
                        return RedirectToAction("BannerList");
                    }
                }
                updateRequest.MerchantID = MerchantId;
                updateRequest.IsActive = false;
                updateRequest.Status = 0;
                updateRequest.CreateDate = DateTime.Now;
                db.merchantBannerUpdateRequests.Add(updateRequest);
                db.SaveChanges();

                Merchant obj = db.Merchants.FirstOrDefault(p => p.Id == MerchantId);
                merchantNotification.SaveNotification(5, obj.FranchiseName);
                TempData["SuccessMsg"] = "Image update request sent to Admin.";
            }
            catch (Exception ex)
            {
                TempData["WarningMsg"] = ex.Message;
            }
            return RedirectToAction("BannerList");
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

        [SessionExpire]
        [Authorize]
        public ActionResult ViewBanner(long id)
        {
            MerchantBanner obj = db.merchantBanners.FirstOrDefault(p => p.ID == id);
            obj.BannerPath = ConfigurationManager.AppSettings["IMAGE_HTTP"] + ConfigurationManager.AppSettings["MERCHANT_DIRECTORY"] + "/" + ConfigurationManager.AppSettings["MERCHANT_FOLDER_BANNER"] + "/" + obj.BannerPath;
            return View(obj);
        }

        [SessionExpire]
        [Authorize]
        [HttpPost]
        public ActionResult ViewBanner(MerchantBanner obj)
        {
            //MerchantBannerUpdateRequest request = new MerchantBannerUpdateRequest();
            MerchantBanner o = db.merchantBanners.FirstOrDefault(p => p.ID == obj.ID);
            o.IsActive = obj.IsActive;
            o.ModifyDate = DateTime.Now;
            db.SaveChanges();
            TempData["SuccessMsg"] = "Updated On Your Shop Page Successfully!!!";
            //if (o.IsActive != obj.IsActive)
            //{
            //    request.MerchantBannerID = obj.ID;
            //    request.MerchantID = obj.MerchantID;
            //    request.ShopPath = obj.IsActive.ToString();
            //    request.BannerPath = o.BannerPath;
            //    request.CreateDate = DateTime.Now;
            //    request.IsActive = false;
            //    db.merchantBannerUpdateRequests.Add(request);
            //    db.SaveChanges();
            //    TempData["SuccessMsg"] = "Request sent to Admin.";
            //}
            return RedirectToAction("MyProfile");
        }

        [SessionExpire]
        [Authorize]
        [HttpPost]
        public ActionResult EditApprovedBanner(List<MerchantBanner> list)
        {
            bool result = false;
            foreach (var item in list)
            {
                MerchantBanner o = db.merchantBanners.FirstOrDefault(p => p.ID == item.ID);
                if (o.IsActive != item.IsActive)
                {
                    o.IsActive = item.IsActive;
                    o.ModifyDate = DateTime.Now;
                    db.SaveChanges();
                    result = true;
                }
            }
            if (result)
            {
                TempData["SuccessMsg"] = "Updated On Your Shop Page Successfully!!!";
            }
            else
            {
                TempData["WarningMsg"] = "No data found to update!!!";
            }
            return RedirectToAction("BannerList");
        }

        public bool UploadImage(int status, HttpPostedFileBase file, long ID, HttpServerUtility server, out string FileName, out string Ext)
        {
            try
            {
                string UniqueCode = "_" + DateTime.Now.ToString("ddMMyyHHmmss");
                FileName = "";
                string FolderName = "";
                if ((int)Document_Type.PHOTO == status)
                {
                    FolderName = WebConfigurationManager.AppSettings["MERCHANT_FOLDER_PHOTO"];
                }
                else
                {
                    FolderName = WebConfigurationManager.AppSettings["MERCHANT_FOLDER_BANNER"];
                }
                string FTP_Path = WebConfigurationManager.AppSettings["IMAGE_FTP"] + WebConfigurationManager.AppSettings["MERCHANT_DIRECTORY"] + "/" + FolderName + "/";
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
        public JsonResult GetCityList(int stateID)
        {
            List<StateCityFranchiseMerchantViewModel> city = new List<StateCityFranchiseMerchantViewModel>();
            try
            {
                List<long> ldistrict = db.Districts.Where(x => x.StateID == stateID).Select(x => x.ID).ToList();

                city = db.Cities.Where(c => ldistrict.Contains(c.DistrictID)).Select(c => new StateCityFranchiseMerchantViewModel { CityID = c.ID, CityName = c.Name }).ToList();
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
        public class tempData
        {
            public Int64 value;
            public string text;
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

        public ActionResult Error()
        {
            return View("~/Views/Shared/500.cshtml");
        }

        [HttpPost]
        public ActionResult SendSMSOnForgetPass(string contactNo)
        {
            string result = "";
            try
            {
                Merchant merchant = db.Merchants.FirstOrDefault(p => p.ContactNumber == contactNo);
                if (merchant != null)
                {
                    result = merchant.FranchiseName;

                    Merchant obj = db.Merchants.FirstOrDefault(p => p.ContactNumber == merchant.ContactNumber);
                    MerchantsLogin login = db.merchantLogins.FirstOrDefault(p => p.MerchantID == obj.Id && p.IsActive == true);
                    if (Session["MerchantPassSent"] == null)
                    {
                        if (login != null)
                        {
                            Session["MerchantPassSent"] = "1";
                            Dictionary<string, string> dictSMSValues = new Dictionary<string, string>();
                            dictSMSValues.Add("#--NAME--#", obj.ContactPerson);
                            dictSMSValues.Add("#--TEXT--#", "your password for Dashboard is " + login.Password);

                            BusinessLogicLayer.GateWay gateWay = new BusinessLogicLayer.SMS(System.Web.HttpContext.Current.Server);
                            gateWay.SendSMS(BusinessLogicLayer.GateWay.SMSGateWays.SUMIT, BusinessLogicLayer.GateWay.SMSOptions.SINGLE, BusinessLogicLayer.GateWay.SMSTypes.KYC_CMPT_REQUEST, new string[] { obj.ContactNumber }, dictSMSValues);
                        }
                        else
                        {
                            result = "$";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [SessionExpire]
        public ActionResult PrintQrCode()
        {
            long MerchantId = Convert.ToInt64(Session["MerchantID"]);
            Merchant merchant = db.Merchants.FirstOrDefault(p => p.Id == MerchantId);
            ViewBag.shopName = merchant.FranchiseName;
            string qrText = "http://merchant.ezeelo.com/uid=" + MerchantId.ToString();
            using (MemoryStream ms = new MemoryStream())
            {
                QRCodeGenerator qrGenerator = new QRCodeGenerator();
                QRCodeData qrCodeData = qrGenerator.CreateQrCode(qrText, QRCodeGenerator.ECCLevel.Q);
                QRCode qrCode = new QRCode(qrCodeData);
                Bitmap qrCodeImage = qrCode.GetGraphic(20);
                using (Bitmap bitMap = qrCode.GetGraphic(20))
                {
                    bitMap.Save(ms, ImageFormat.Png);
                    ViewBag.QRCodeImage = "data:image/png;base64," + Convert.ToBase64String(ms.ToArray());
                }
            }
            return View();
        }

        [HttpGet]
        public ActionResult GoOnline()
        {
            string redirectURL = "";
            try
            {
                long MerchantId = Convert.ToInt64(Session["MerchantID"]);
                if ((!db.MerchantKYCs.Any(p => p.MerchantID == MerchantId && p.IsVerified == true)) || (!db.merchantTopupRecharges.Any(p => p.MerchantID == MerchantId && p.Amount > 0)))
                {
                    return Json(new { responseCode = -1 }, JsonRequestBehavior.AllowGet);
                }
                var merchant = db.Merchants.Find(MerchantId);
                string onlineEncKey = "749HB97HGG68oYuQ";
                if (String.IsNullOrEmpty(merchant.OnlineMerchantHash))
                {
                    string onlineSalt = "JH35KHKP9I9KLO";
                    string hashString = merchant.ContactNumber + onlineSalt;
                    merchant.OnlineMerchantHash = sha256_hash(hashString);
                    db.SaveChanges();
                }
                string merchantInfo = merchant.Id + "|" + merchant.ContactPerson + "|" + merchant.Email + "|" + merchant.ContactNumber + "|" + merchant.OnlineMerchantHash;
                string encryptedMerchantParam = Encrypt(merchantInfo, onlineEncKey);
                redirectURL = ("http://marketplace.ezeelo.com/processbridge?info=" + HttpUtility.UrlEncode(encryptedMerchantParam) + "&type=merchant");
                return Json(new { responseCode = 1, redirectURL}, JsonRequestBehavior.AllowGet);
            }
            catch(Exception ex)
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