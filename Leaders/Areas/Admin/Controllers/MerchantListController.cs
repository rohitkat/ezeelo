using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using Leaders.Filter;
using ModelLayer.Models;
using ModelLayer.Models.ViewModel;
using QRCoder;

namespace Leaders.Areas.Admin.Controllers
{
    [AdminSessionExpire]
    public class MerchantListController : Controller
    {
        EzeeloDBContext db = new EzeeloDBContext();
                
        public ActionResult Registered(string status)
        {
            List<Merchant> list = new List<Merchant>();
            if (status == null)
            {
                list = db.Merchants.Where(p => p.Status != "NotAccepted").OrderByDescending(p => p.CreateDate).ToList();
            }
            else
            {
                list = db.Merchants.Where(p => p.Status == "Approve").OrderByDescending(p => p.CreateDate).ToList();
            }
            foreach (var item in list)
            {
                item.IsKYCComplete = db.MerchantKYCs.Any(p => p.MerchantID == item.Id && p.IsCompleted);
                if (db.Merchants.Any(p => p.Id != item.Id && p.GSTINNo != null && p.GSTINNo == item.GSTINNo))
                {
                    List<Merchant> merchants = db.Merchants.Where(p => p.Id != item.Id && p.GSTINNo != null && p.GSTINNo == item.GSTINNo).ToList();
                    item.Remark = "Other shop found with same GST no. Shop Name :- " + String.Join(",", merchants.Select(p => p.FranchiseName).ToList()) + ".";
                }
                if (db.Merchants.Any(p => p.Id != item.Id && p.PANNo == item.PANNo))
                {
                    List<Merchant> merchants = db.Merchants.Where(p => p.Id != item.Id && p.PANNo == item.PANNo).ToList();
                    item.Remark = (string.IsNullOrEmpty(item.Remark) ? "" : item.Remark + "\n") + "Other shop found with same PAN no. Shop Name :- " + String.Join(",", merchants.Select(p => p.FranchiseName).ToList()) + ".";
                }
                if (db.Merchants.Any(p => p.Id != item.Id && p.FranchiseName == item.FranchiseName && p.Category == item.Category))
                {
                    List<Merchant> merchants = db.Merchants.Where(p => p.Id != item.Id && p.FranchiseName == item.FranchiseName && p.Category == item.Category).ToList();
                    item.Remark = (string.IsNullOrEmpty(item.Remark) ? "" : item.Remark + "\n") + "Other shop found with same shop name having same service.";
                }
                if (db.Merchants.Any(p => p.Id != item.Id && p.Email == item.Email && p.ContactNumber == item.ContactNumber))
                {
                    List<Merchant> merchants = db.Merchants.Where(p => p.Id != item.Id && p.Email == item.Email && p.ContactNumber == item.ContactNumber).ToList();
                    item.Remark = (string.IsNullOrEmpty(item.Remark) ? "" : item.Remark + "\n") + "Other shop found with same emailId and contact no. Shop Name :- " + String.Join(",", merchants.Select(p => p.FranchiseName).ToList()) + ".";
                }
                else
                {
                    if (db.Merchants.Any(p => p.Id != item.Id && p.ContactNumber == item.ContactNumber))
                    {
                        List<Merchant> merchants = db.Merchants.Where(p => p.Id != item.Id && p.ContactNumber == item.ContactNumber).ToList();
                        item.Remark = (string.IsNullOrEmpty(item.Remark) ? "" : item.Remark + "\n") + "Other shop found with same contact no. Shop Name :- " + String.Join(",", merchants.Select(p => p.FranchiseName).ToList()) + ".";
                    }
                    if (db.Merchants.Any(p => p.Id != item.Id && p.Email == item.Email))
                    {
                        List<Merchant> merchants = db.Merchants.Where(p => p.Id != item.Id && p.Email == item.Email).ToList();
                        item.Remark = (string.IsNullOrEmpty(item.Remark) ? "" : item.Remark + "\n") + "Other shop found with same emailId. Shop Name :- " + String.Join(",", merchants.Select(p => p.FranchiseName).ToList()) + ".";
                    }
                }

                if (item.Status.ToLower() == "inactive")
                {
                    List<MerchantApprovalLog> approvalLogs = db.merchantApprovalLogs.Where(p => p.MerchantID == item.Id).ToList();
                    if (approvalLogs.Count != 0)
                    {
                        string status_ = approvalLogs.OrderByDescending(p => p.ID).Take(1).FirstOrDefault().Status;
                        if(status_.ToLower() == "reject")
                        {
                            item.Status_ = status_;
                        }
                    }
                }
            }
            return View(list);
        }

        public ActionResult NotAcceptedList()
        {
            List<Merchant> list = new List<Merchant>();
            list = db.Merchants.Where(p => p.Status == "NotAccepted").OrderByDescending(p => p.CreateDate).ToList();
            return View(list);
        }
        public ActionResult Details(long id)
        {
            TempData["PreviousUrl"] = System.Web.HttpContext.Current.Request.UrlReferrer;
            if (System.Web.HttpContext.Current.Request.UrlReferrer != null)
            {
                TempData["PreviousUrlPath"] = System.Web.HttpContext.Current.Request.UrlReferrer.PathAndQuery;
            }
            Merchant obj = new Merchant();
            MerchantCommonValues values = db.MerchantCommonValues.FirstOrDefault();
            
            try
            {
                obj = db.Merchants.FirstOrDefault(p => p.Id == id);
                PersonalDetail pp = db.PersonalDetails.FirstOrDefault(p => p.UserLoginID == (db.UserLogins.FirstOrDefault(u => u.Mobile == obj.LeaderContactNo).ID));
                ViewBag.LeadersName = pp.FirstName + ((pp.LastName == null) ? "" : pp.LastName);
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
                if (db.Merchants.Any(p => p.Id != obj.Id && p.GSTINNo != null && p.GSTINNo == obj.GSTINNo))
                {
                    List<Merchant> merchants = db.Merchants.Where(p => p.Id != obj.Id && p.GSTINNo != null && p.GSTINNo == obj.GSTINNo).ToList();
                    obj.Remark = "Other shop found with same GST no. Shop Name :- " + String.Join(",", merchants.Select(p => p.FranchiseName).ToList());
                }
                if (db.Merchants.Any(p => p.Id != obj.Id && p.PANNo == obj.PANNo))
                {
                    List<Merchant> merchants = db.Merchants.Where(p => p.Id != obj.Id && p.PANNo == obj.PANNo).ToList();
                    obj.Remark = (string.IsNullOrEmpty(obj.Remark) ? "" : obj.Remark + "\n") + "Other shop found with same PAN no. Shop Name :- " + String.Join(",", merchants.Select(p => p.FranchiseName).ToList());
                }
                if (db.Merchants.Any(p => p.Id != obj.Id && p.FranchiseName == obj.FranchiseName && p.Category == obj.Category))
                {
                    List<Merchant> merchants = db.Merchants.Where(p => p.Id != obj.Id && p.FranchiseName == obj.FranchiseName && p.Category == obj.Category).ToList();
                    obj.Remark = (string.IsNullOrEmpty(obj.Remark) ? "" : obj.Remark + "\n") + "Other shop found with same shop name having same service.";
                }
                if (db.Merchants.Any(p => p.Id != obj.Id && p.Email == obj.Email && p.ContactNumber == obj.ContactNumber))
                {
                    List<Merchant> merchants = db.Merchants.Where(p => p.Id != obj.Id && p.Email == obj.Email && p.ContactNumber == obj.ContactNumber).ToList();
                    obj.Remark = (string.IsNullOrEmpty(obj.Remark) ? "" : obj.Remark + "\n") + "Other shop found with same emailId and contact no. Shop Name :- " + String.Join(",", merchants.Select(p => p.FranchiseName).ToList());
                }
                else
                {
                    if (db.Merchants.Any(p => p.Id != obj.Id && p.ContactNumber == obj.ContactNumber))
                    {
                        List<Merchant> merchants = db.Merchants.Where(p => p.Id != obj.Id && p.ContactNumber == obj.ContactNumber).ToList();
                        obj.Remark = (string.IsNullOrEmpty(obj.Remark) ? "" : obj.Remark + "\n") + "Other shop found with same contact no. Shop Name :- " + String.Join(",", merchants.Select(p => p.FranchiseName).ToList());
                    }
                    if (db.Merchants.Any(p => p.Id != obj.Id && p.Email == obj.Email))
                    {
                        List<Merchant> merchants = db.Merchants.Where(p => p.Id != obj.Id && p.Email == obj.Email).ToList();
                        obj.Remark = (string.IsNullOrEmpty(obj.Remark) ? "" : obj.Remark + "\n") + "Other shop found with same emailId. Shop Name :- " + String.Join(",", merchants.Select(p => p.FranchiseName).ToList());
                    }
                }
                MerchantsLogin login = db.merchantLogins.FirstOrDefault(p => p.UserID == obj.ContactNumber && p.IsActive == true && p.Password != null);
                if (login != null)
                {
                    obj.DashboardLink = ConfigurationManager.AppSettings["MERCHANT_DASHBOARD"] + "/Home/ByPassLogin?MerchantId=" + obj.Id;
                }

                List<int?> pincodes = db.Pincodes.Where(p => p.CityID == obj.City).Select(p => (int?)p.ID).ToList();
                List<Franchise> fr = db.Franchises.Where(q => pincodes.Contains(q.PincodeID) && q.IsActive == true).ToList();
                if (fr.Count != 0)
                {
                    ViewBag.FranchiseId = fr.FirstOrDefault().ID;
                    ViewBag.CityName = obj.CityDetail.Name.ToLower();
                }
                else
                {
                    ViewBag.FranchiseId = "1085";
                    ViewBag.CityName = "nagpur";
                }
                MerchantDetails details = db.merchantDetails.FirstOrDefault(p => p.MerchantId == id);
                if (details != null)
                {
                    obj.LeaderCommision = details.LeaderCommisiion;
                    obj.RegistrationFee = details.RegistrationFee;
                }
                else
                {
                    obj.LeaderCommision = values.LeaderCommission;
                    obj.RegistrationFee = values.MerchantRegistrationFee;
                }
            }
            catch (Exception ex)
            {

            }
            string qrText = "http://merchant.ezeelo.com/uid="+obj.Id.ToString();
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
            return View(obj);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Details(Merchant merchant, List<string> Holiday)
        {
            Merchant obj = db.Merchants.FirstOrDefault(p => p.Id == merchant.Id);
            obj.ModifyDate = DateTime.Now;
            obj.NetworkIP = BusinessLogicLayer.CommonFunctions.GetClientIP();
            obj.LeaderContactNo = merchant.LeaderContactNo;
            obj.FranchiseName = merchant.FranchiseName;
            obj.GSTINNo = merchant.GSTINNo;
            obj.PANNo = merchant.PANNo;
            obj.Address = merchant.Address;
            obj.State = merchant.State;
            obj.City = merchant.City;
            obj.Pincode = merchant.Pincode;
            obj.ShopTiming = merchant.ShopTiming;
            obj.ContactPerson = merchant.ContactPerson;
            obj.ContactNumber = merchant.ContactNumber;
            obj.Email = merchant.Email;
            obj.Category = merchant.Category;
            obj.Comission = merchant.Comission;
            obj.GoogleMapLink = merchant.GoogleMapLink;
            obj.SpecialRemark = merchant.SpecialRemark;

            db.MerchantHolidays.RemoveRange(db.MerchantHolidays.Where(p => p.MerchantID == obj.Id).ToList());

            if (Holiday != null)
            {
                foreach (var item in Holiday)
                {
                    MerchantHoliday merchantHoliday = new MerchantHoliday();
                    merchantHoliday.MerchantID = merchant.Id;
                    merchantHoliday.HolidayID = Convert.ToInt32(item);
                    db.MerchantHolidays.Add(merchantHoliday);
                }
            }

            MerchantsLogin login = db.merchantLogins.FirstOrDefault(p => p.MerchantID == obj.Id);
            if (login != null)
            {
                if (login.UserID != obj.ContactNumber)
                {
                    login.UserID = obj.ContactNumber;
                    login.ConfirmPassword = login.Password;
                    login.ModifyDate = DateTime.Now;
                    login.ModifyBy = "Admin";
                }
            }


            MerchantCommonValues values = db.MerchantCommonValues.FirstOrDefault();
            if(values.LeaderCommission != merchant.LeaderCommision || values.MerchantRegistrationFee != merchant.RegistrationFee)
            {
                MerchantDetails details = db.merchantDetails.FirstOrDefault(p => p.MerchantId == merchant.Id);
                if(details == null)
                {
                    ServiceIncomeMaster commission = db.ServiceIncomeMasters.FirstOrDefault();
                    details = new MerchantDetails();
                    details.GST = 0;
                    details.IsGSTApply = false;
                    details.LeaderCommisiion = merchant.LeaderCommision;
                    details.Level0 = commission.UserLevel0;
                    details.MerchantId = merchant.Id;
                    details.Part5th = commission.Part5th;
                    details.RegistrationFee = merchant.RegistrationFee;
                    details.RMCommission = commission.RelationshipManager;
                    details.UptoLevel6 = commission.UptoLevel6;
                    details.Company = commission.Company;
                    details.CreateDate = DateTime.Now;
                    db.merchantDetails.Add(details);
                }
                else
                {
                    details.RegistrationFee = merchant.RegistrationFee;
                    details.LeaderCommisiion = merchant.LeaderCommision;
                }
            }
            db.SaveChanges();
            TempData["Result"] = "Update successfully!";
            if (TempData["PreviousUrlPath"] != null)
            {
                return Redirect(TempData["PreviousUrlPath"].ToString());
            }
            else
            {
                if (obj.Status == "Approve")
                {
                    return RedirectToAction("Registered", new { status = "1" });
                }
                else
                {
                    return RedirectToAction("Registered");
                }
            }
        }

        [HttpPost]
        public ActionResult Upgrade(long MerchantID)
        {
            string result = "";
            try
            {
                Merchant obj = db.Merchants.FirstOrDefault(p => p.Id == MerchantID);
                if (obj.Status == "Approve")
                {
                    obj.ApproveDate = DateTime.Now;
                    db.SaveChanges();

                    MerchantApprovalLog log = new MerchantApprovalLog();
                    log.MerchantID = MerchantID;
                    log.Status = "Upgrade";
                    log.Date = DateTime.Now;
                    log.NetworkIP = BusinessLogicLayer.CommonFunctions.GetClientIP();
                    db.merchantApprovalLogs.Add(log);
                    db.SaveChanges();
                    TempData["Alert"] = "Shop Validity Period Upgraded Upto " + DateTime.Now.AddYears(1).ToString("dd/MM/yyyy") + " !!!";
                }
                else
                {
                    TempData["Messaage"] = "Problem in getting Shop current status. Please contact with admin!!!";
                }
            }
            catch (Exception ex)
            {
                TempData["Messaage"] = ex.Message.ToString();
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Accept(long MerchantID)
        {
            string result = "";
            try
            {
                Merchant obj = db.Merchants.FirstOrDefault(p => p.Id == MerchantID);
                obj.Status = "Accept";
                obj.AcceptDate = DateTime.Now;

                MerchantApprovalLog log = new MerchantApprovalLog();
                log.MerchantID = MerchantID;
                log.Status = "Accept";
                log.Date = DateTime.Now;
                log.NetworkIP = BusinessLogicLayer.CommonFunctions.GetClientIP();
                db.merchantApprovalLogs.Add(log);

                db.SaveChanges();

                //Changes in Flow send KYC form link at the END
                //SendMailSMSOnAccept(MerchantID);
                SendMailSMSForValidateMo(MerchantID);
                TempData["Alert"] = "Shop Accepted Successfully!!!";
            }
            catch (Exception ex)
            {
                TempData["Messaage"] = ex.Message.ToString();
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        //public void SendMailSMSOnAccept(long MerchantID)
        //{
        //    Merchant obj = db.Merchants.FirstOrDefault(p => p.Id == MerchantID);
        //    try
        //    {
        //        string KYCURL = ConfigurationManager.AppSettings["EZEELO_CUSTOMER_URL"] + "nagpur/1060/merchant/KYC?id=" + MerchantID;

        //        Dictionary<string, string> dictEmailValues = new Dictionary<string, string>();
        //        dictEmailValues.Add("<!--NAME-->", obj.ContactPerson);
        //        dictEmailValues.Add("<!--SHOP-->", obj.FranchiseName);
        //        dictEmailValues.Add("<!--LINK-->", KYCURL);
        //        BusinessLogicLayer.GateWay gateWay = new BusinessLogicLayer.Email(System.Web.HttpContext.Current.Server);
        //        gateWay.SendEmail(BusinessLogicLayer.GateWay.EmailGateWays.GANDHIBAGH, BusinessLogicLayer.GateWay.SenderMail.INFO, BusinessLogicLayer.GateWay.EMailTypes.ACCEPT_MERCHANT, new string[] { obj.Email }, dictEmailValues, true);

        //    }
        //    catch
        //    {

        //    }
        //    try
        //    {
        //        Dictionary<string, string> dictSMSValues_ = new Dictionary<string, string>();
        //        BusinessLogicLayer.GateWay gateWay_ = new BusinessLogicLayer.SMS(System.Web.HttpContext.Current.Server);
        //        dictSMSValues_.Add("#--NAME--#", obj.ContactPerson);
        //        gateWay_.SendSMS(BusinessLogicLayer.GateWay.SMSGateWays.SUMIT,
        //            BusinessLogicLayer.GateWay.SMSOptions.SINGLE, BusinessLogicLayer.GateWay.SMSTypes.MERCHANT_REG_ACCEPT,
        //            new string[] { obj.ContactNumber }, dictSMSValues_);
        //    }
        //    catch
        //    {

        //    }
        //}

        public void SendMailSMSForValidateMo(long MerchantID)
        {
            Merchant obj = db.Merchants.FirstOrDefault(p => p.Id == MerchantID);
            bool result = false;
            try
            {
                List<Merchant> merchants = db.Merchants.Where(p => p.Id != MerchantID && p.Email == obj.Email).ToList();
                foreach (var item in merchants)
                {
                    if (item.Status == "Approve")
                    {
                        if (db.merchantLogins.Any(p => p.MerchantID == item.Id && p.Password != null && p.IsActive == true))
                        {
                            result = true;
                        }
                    }
                }
                string OTPVerificationURL = ConfigurationManager.AppSettings["EZEELO_CUSTOMER_URL"] + "nagpur/1060/merchant/SendOTPToValidateMo?ID=" + MerchantID;
                Dictionary<string, string> dictEmailValues = new Dictionary<string, string>();
                dictEmailValues.Add("<!--NAME-->", obj.ContactPerson);
                dictEmailValues.Add("<!--SHOP-->", obj.FranchiseName);
                dictEmailValues.Add("<!--LINK-->", OTPVerificationURL);
                BusinessLogicLayer.GateWay gateWay = new BusinessLogicLayer.Email(System.Web.HttpContext.Current.Server);

                if (result)
                {
                    gateWay.SendEmail(BusinessLogicLayer.GateWay.EmailGateWays.GANDHIBAGH, BusinessLogicLayer.GateWay.SenderMail.INFO, BusinessLogicLayer.GateWay.EMailTypes.APPROVE_MERCHANT_SECOND_ACCOUNT, new string[] { obj.Email }, dictEmailValues, true);
                }
                else
                {
                    gateWay.SendEmail(BusinessLogicLayer.GateWay.EmailGateWays.GANDHIBAGH, BusinessLogicLayer.GateWay.SenderMail.INFO, BusinessLogicLayer.GateWay.EMailTypes.APPROVE_MERCHANT, new string[] { obj.Email }, dictEmailValues, true);
                }
            }
            catch
            {

            }           
        }



        public void SendSMSOnReject(long MerchantID)
        {
            Merchant obj = db.Merchants.FirstOrDefault(p => p.Id == MerchantID);
            try
            {
                //SMS to merchant on Rejection
                Dictionary<string, string> dictSMSValues_ = new Dictionary<string, string>();
                dictSMSValues_.Add("#--SHOP--#", obj.FranchiseName);
                dictSMSValues_.Add("#--NAME--#", obj.ContactPerson);
                BusinessLogicLayer.GateWay gateWay_ = new BusinessLogicLayer.SMS(System.Web.HttpContext.Current.Server);
                gateWay_.SendSMS(BusinessLogicLayer.GateWay.SMSGateWays.SUMIT,
                    BusinessLogicLayer.GateWay.SMSOptions.SINGLE, BusinessLogicLayer.GateWay.SMSTypes.MERCHANT_REG_REJECT,
                    new string[] { obj.ContactNumber }, dictSMSValues_);
            }
            catch
            {

            }
            try
            {
                //SMS to Leaders on REjection 
                Dictionary<string, string> dictSMSValues_ = new Dictionary<string, string>();
                dictSMSValues_.Add("#--SHOP--#", obj.FranchiseName);
                BusinessLogicLayer.GateWay gateWay_ = new BusinessLogicLayer.SMS(System.Web.HttpContext.Current.Server);
                gateWay_.SendSMS(BusinessLogicLayer.GateWay.SMSGateWays.SUMIT,
                    BusinessLogicLayer.GateWay.SMSOptions.SINGLE, BusinessLogicLayer.GateWay.SMSTypes.MERCHANT_REG_REJECT_LEADER,
                    new string[] { obj.ContactNumber }, dictSMSValues_);
            }
            catch
            {

            }
        }
        [HttpPost]
        public ActionResult SendSMS(long MerchantID, string Text)
        {
            string result = "";
            try
            {
                Merchant obj = db.Merchants.FirstOrDefault(p => p.Id == MerchantID);

                string Mobile = obj.ContactNumber;
                Dictionary<string, string> dictSMSValues = new Dictionary<string, string>();
                dictSMSValues.Add("#--NAME--#", obj.ContactPerson);
                dictSMSValues.Add("#--TEXT--#", Text);

                BusinessLogicLayer.GateWay gateWay = new BusinessLogicLayer.SMS(System.Web.HttpContext.Current.Server);
                gateWay.SendSMS(BusinessLogicLayer.GateWay.SMSGateWays.SUMIT, BusinessLogicLayer.GateWay.SMSOptions.SINGLE, BusinessLogicLayer.GateWay.SMSTypes.KYC_CMPT_REQUEST, new string[] { Mobile }, dictSMSValues);

            }
            catch (Exception ex)
            {
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Approve(long MerchantID)
        {
            string result = "";
            try
            {
                Merchant obj = db.Merchants.FirstOrDefault(p => p.Id == MerchantID);
                obj.Status = "Approve";
                obj.ApproveDate = DateTime.Now;

                MerchantApprovalLog log = new MerchantApprovalLog();
                log.MerchantID = MerchantID;
                log.Status = "Approve";
                log.Date = DateTime.Now;
                log.NetworkIP = BusinessLogicLayer.CommonFunctions.GetClientIP();
                db.merchantApprovalLogs.Add(log);

                MerchantKYC kYC = db.MerchantKYCs.FirstOrDefault(p => p.MerchantID == MerchantID);
                kYC.IsVerified = true;
                kYC.IsCompleted = true;
                kYC.ModifyDate = DateTime.Now;

                db.SaveChanges();
                SendSMSOnApprove(MerchantID);
                TempData["Alert"] = "Shop Approved Successfully!!!";
                //result = "Shop Approved Successfully!!!";
            }
            catch (Exception ex)
            {
                TempData["Messaage"] = ex.Message.ToString();
                //result = ex.Message.ToString();
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public void SendSMSOnApprove(long MerchantId)
        {
            Merchant obj = db.Merchants.FirstOrDefault(p => p.Id == MerchantId);
            try
            {
                //SMS to merchant on approval
                Dictionary<string, string> dictSMSValues_ = new Dictionary<string, string>();
                BusinessLogicLayer.GateWay gateWay_ = new BusinessLogicLayer.SMS(System.Web.HttpContext.Current.Server);
                gateWay_.SendSMS(BusinessLogicLayer.GateWay.SMSGateWays.SUMIT,
                    BusinessLogicLayer.GateWay.SMSOptions.SINGLE, BusinessLogicLayer.GateWay.SMSTypes.MERCHANT_REG_APPROVE,
                    new string[] { obj.ContactNumber }, dictSMSValues_);
            }
            catch
            {

            }
            try
            {
                //SMS to Leaders on approval 
                Dictionary<string, string> dictSMSValues_ = new Dictionary<string, string>();
                dictSMSValues_.Add("#--SHOP--#", obj.FranchiseName);
                BusinessLogicLayer.GateWay gateWay_ = new BusinessLogicLayer.SMS(System.Web.HttpContext.Current.Server);
                gateWay_.SendSMS(BusinessLogicLayer.GateWay.SMSGateWays.SUMIT,
                    BusinessLogicLayer.GateWay.SMSOptions.SINGLE, BusinessLogicLayer.GateWay.SMSTypes.MERCHANT_REG_APPROVE_LEADER,
                    new string[] { obj.LeaderContactNo }, dictSMSValues_);
            }
            catch
            {

            }
        }

        [HttpPost]
        public ActionResult Reject(long MerchantID,string Remark)
        {
            string result = "";
            try
            {
                Merchant obj = db.Merchants.FirstOrDefault(p => p.Id == MerchantID);
                obj.Status = "Inactive";
                obj.ModifyDate = DateTime.Now;

                MerchantApprovalLog log = new MerchantApprovalLog();
                log.MerchantID = MerchantID;
                log.Status = "Reject";
                log.Date = DateTime.Now;
                log.NetworkIP = BusinessLogicLayer.CommonFunctions.GetClientIP();
                log.Remark = Remark;
                db.merchantApprovalLogs.Add(log);

                MerchantKYC kYC = db.MerchantKYCs.FirstOrDefault(p => p.MerchantID == MerchantID);
                if (kYC != null)
                {
                    kYC.IsVerified = false;
                    kYC.IsCompleted = false;
                    kYC.ModifyDate = DateTime.Now;
                }

                MerchantsLogin login = db.merchantLogins.FirstOrDefault(p => p.MerchantID == obj.Id);
                if (login != null)
                {
                    login.IsActive = false;
                    login.ModifyBy = "Admin";
                    login.ModifyDate = DateTime.Now;
                    login.ConfirmPassword = login.Password;
                }
                db.SaveChanges();
                SendSMSOnReject(MerchantID);
                TempData["Alert"] = "Shop Deactivated Successfully!!!";
            }
            catch (Exception ex)
            {
                TempData["Messaage"] = ex.Message.ToString();
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Reject1(long MerchantID, string Remark)
        {
            string result = "";
            try
            {
                Merchant obj = db.Merchants.FirstOrDefault(p => p.Id == MerchantID);
                obj.Status = "NotAccepted";
                obj.ModifyDate = DateTime.Now;

                MerchantApprovalLog log = new MerchantApprovalLog();
                log.MerchantID = MerchantID;
                log.Status = "NotAccepted";
                log.Date = DateTime.Now;
                log.Remark = Remark;
                log.NetworkIP = BusinessLogicLayer.CommonFunctions.GetClientIP();
                db.merchantApprovalLogs.Add(log);

                MerchantKYC kYC = db.MerchantKYCs.FirstOrDefault(p => p.MerchantID == MerchantID);
                if (kYC != null)
                {
                    kYC.IsVerified = false;
                    kYC.IsCompleted = false;
                    kYC.ModifyDate = DateTime.Now;
                }

                MerchantsLogin login = db.merchantLogins.FirstOrDefault(p => p.MerchantID == obj.Id);
                if (login != null)
                {
                    login.IsActive = false;
                    login.ModifyBy = "Admin";
                    login.ModifyDate = DateTime.Now;
                    login.ConfirmPassword = login.Password;
                }
                db.SaveChanges();
                SendSMSOnReject(MerchantID);
                TempData["Alert"] = "Shop Deactivated Successfully!!!";
            }
            catch (Exception ex)
            {
                TempData["Messaage"] = ex.Message.ToString();
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult KYC(long Id)
        {
            try
            {
                TempData["PreviousUrl"] = System.Web.HttpContext.Current.Request.UrlReferrer;
                Merchant merchant = db.Merchants.FirstOrDefault(p => p.Id == Id);
                MerchantKYC kYC = db.MerchantKYCs.FirstOrDefault(p => p.MerchantID == Id);
                if (kYC != null)
                {
                    ViewBag.MerchantName = merchant.FranchiseName;
                    if (kYC.PhotoImageUrl != null)
                    {
                        kYC.PhotoImageUrl = ConfigurationManager.AppSettings["IMAGE_HTTP"] + ConfigurationManager.AppSettings["MERCHANT_DIRECTORY"] + "/" + ConfigurationManager.AppSettings["MERCHANT_FOLDER_PHOTO"] + "/" + kYC.PhotoImageUrl;
                    }
                    if (kYC.ShopEstablishmentCertificateImageUrl != null)
                    {
                        kYC.ShopEstablishmentCertificateImageUrl = ConfigurationManager.AppSettings["IMAGE_HTTP"] + ConfigurationManager.AppSettings["MERCHANT_DIRECTORY"] + "/" + ConfigurationManager.AppSettings["MERCHANT_FOLDER_SHOPESTABLISHMENT"] + "/" + kYC.ShopEstablishmentCertificateImageUrl;
                    }
                    if (kYC.PanImageUrl != null)
                    {
                        kYC.PanImageUrl = ConfigurationManager.AppSettings["IMAGE_HTTP"] + ConfigurationManager.AppSettings["MERCHANT_DIRECTORY"] + "/" + ConfigurationManager.AppSettings["MERCHANT_FOLDER_PAN"] + "/" + kYC.PanImageUrl;
                    }
                    if (kYC.GSTRegistrationImageUrl != null)
                    {
                        kYC.GSTRegistrationImageUrl = ConfigurationManager.AppSettings["IMAGE_HTTP"] + ConfigurationManager.AppSettings["MERCHANT_DIRECTORY"] + "/" + ConfigurationManager.AppSettings["MERCHANT_FOLDER_GST"] + "/" + kYC.GSTRegistrationImageUrl;
                    }
                    if (kYC.VisingCardImageUrl != null)
                    {
                        kYC.VisingCardImageUrl = ConfigurationManager.AppSettings["IMAGE_HTTP"] + ConfigurationManager.AppSettings["MERCHANT_DIRECTORY"] + "/" + ConfigurationManager.AppSettings["MERCHANT_FOLDER_VISITINGCARD"] + "/" + kYC.VisingCardImageUrl;
                    }
                    if (kYC.CancelledblankChequeImageUrl != null)
                    {
                        kYC.CancelledblankChequeImageUrl = ConfigurationManager.AppSettings["IMAGE_HTTP"] + ConfigurationManager.AppSettings["MERCHANT_DIRECTORY"] + "/" + ConfigurationManager.AppSettings["MERCHANT_FOLDER_CANCELLEDCHEQUE"] + "/" + kYC.CancelledblankChequeImageUrl;
                    }
                    if (kYC.AddressProofUrl != null)
                    {
                        kYC.AddressProofUrl = ConfigurationManager.AppSettings["IMAGE_HTTP"] + ConfigurationManager.AppSettings["MERCHANT_DIRECTORY"] + "/" + ConfigurationManager.AppSettings["MERCHANT_FOLDER_ADDRESS"] + "/" + kYC.AddressProofUrl;
                    }
                    if (kYC.BannerImageUrl != null)
                    {
                        MerchantBanner MB = db.merchantBanners.Where(p => p.MerchantID == Id).OrderByDescending(p => p.CreateDate).ToList().FirstOrDefault();
                        if (MB != null)
                        {
                            kYC.BannerImageUrl = ConfigurationManager.AppSettings["IMAGE_HTTP"] + ConfigurationManager.AppSettings["MERCHANT_DIRECTORY"] + "/" + ConfigurationManager.AppSettings["MERCHANT_FOLDER_BANNER"] + "/" + MB.BannerPath;
                        }
                        else
                        {
                            kYC.BannerImageUrl = ConfigurationManager.AppSettings["IMAGE_HTTP"] + ConfigurationManager.AppSettings["MERCHANT_DIRECTORY"] + "/" + ConfigurationManager.AppSettings["MERCHANT_FOLDER_BANNER"] + "/" + kYC.BannerImageUrl;
                        }
                    }
                    List<int?> pincodes = db.Pincodes.Where(p => p.CityID == merchant.City).Select(p => (int?)p.ID).ToList();
                    List<Franchise> fr = db.Franchises.Where(q => pincodes.Contains(q.PincodeID) && q.IsActive == true).ToList();
                    if (fr.Count != 0)
                    {
                        ViewBag.FranchiseId = fr.FirstOrDefault().ID;
                        ViewBag.CityName = merchant.CityDetail.Name.ToLower();
                    }
                    else
                    {
                        ViewBag.FranchiseId = "1085";
                        ViewBag.CityName = "nagpur";
                    }
                    return View(kYC);
                }
            }
            catch
            {

            }
            return View();
        }

        [HttpPost]
        public ActionResult KYC(MerchantKYC merchant, HttpPostedFileBase photo, HttpPostedFileBase shopCertificate, HttpPostedFileBase pan, HttpPostedFileBase gst, HttpPostedFileBase address, HttpPostedFileBase visitingCard, HttpPostedFileBase cancelledCheque, HttpPostedFileBase Banner)
        {
            long merchantID = merchant.MerchantID;
            try
            {
                MerchantKYC merchantKYC = db.MerchantKYCs.FirstOrDefault(p => p.MerchantID == merchantID);

                if (photo == null && shopCertificate == null && pan == null && gst == null && address == null && cancelledCheque == null && visitingCard == null && Banner == null)
                {
                    TempData["Messaage"] = "No Attachment to save!!!";
                    return RedirectToAction("KYC", new { Id = merchantID });
                }
                var allowedExtensions = new[] { ".Jpg", ".png", ".jpg", ".jpeg", ".pdf" };
                bool IsSaved = false;
                string Filename = "";
                string Ext = "";
                string UniqueCode = "_" + DateTime.Now.ToString("ddMMyyHHmmss");
                if (photo != null)
                {
                    var fileNamePhoto = Path.GetFileName(photo.FileName);
                    var ExtPhoto = Path.GetExtension(photo.FileName);
                    string namePhoto = Path.GetFileNameWithoutExtension(fileNamePhoto);
                    string photoPath = namePhoto + "_" + merchantID + UniqueCode + ExtPhoto;

                    if (allowedExtensions.Contains(ExtPhoto)) //check what type of extension  
                    {
                        IsSaved = UploadImage((int)Document_Type.PHOTO, photo, (long)merchantID, System.Web.HttpContext.Current.Server, out Filename, out Ext);

                        merchantKYC.PhotoImageUrl = Filename;
                    }
                    else
                    {
                        TempData["Messaage"] = "Please choose only Image file in Photo";
                        return RedirectToAction("KYC", new { Id = merchantID });
                    }

                }
                if (shopCertificate != null)
                {
                    var fileNameShopCertificate = Path.GetFileName(shopCertificate.FileName);
                    var ExtShopCertificate = Path.GetExtension(shopCertificate.FileName);
                    string nameShopCertificate = Path.GetFileNameWithoutExtension(fileNameShopCertificate);
                    string shopCertificatePath = nameShopCertificate + "_" + merchantID + UniqueCode + ExtShopCertificate;

                    if (allowedExtensions.Contains(ExtShopCertificate)) //check what type of extension  
                    {
                        IsSaved = UploadImage((int)Document_Type.SHOPESTABLISHMENT, shopCertificate, (long)merchantID, System.Web.HttpContext.Current.Server, out Filename, out Ext);

                        merchantKYC.ShopEstablishmentCertificateImageUrl = Filename;
                    }
                    else
                    {
                        TempData["Messaage"] = "Please choose only Image file in Shop Establishment Certificate";
                        return RedirectToAction("KYC", new { Id = merchantID });
                    }

                }
                if (pan != null)
                {
                    var fileNamePAN = Path.GetFileName(pan.FileName);
                    var ExtPan = Path.GetExtension(pan.FileName);
                    string namePan = Path.GetFileNameWithoutExtension(fileNamePAN);//getting file name without extension  
                    string PANPath = namePan + "_" + merchantID + UniqueCode + ExtPan;
                    if (allowedExtensions.Contains(ExtPan)) //check what type of extension  
                    {
                        IsSaved = UploadImage((int)Document_Type.PAN, pan, (long)merchantID, System.Web.HttpContext.Current.Server, out Filename, out Ext);

                        merchantKYC.PanImageUrl = Filename;
                    }
                    else
                    {
                        TempData["Messaage"] = "Please choose only Image file in PAN";
                        return RedirectToAction("KYC", new { Id = merchantID });
                    }

                }
                if (gst != null)
                {
                    var fileNameGST = Path.GetFileName(gst.FileName);
                    var ExtGST = Path.GetExtension(gst.FileName);
                    string nameGST = Path.GetFileNameWithoutExtension(fileNameGST);
                    string GSTPath = nameGST + "_" + merchantID + UniqueCode + ExtGST;

                    if (allowedExtensions.Contains(ExtGST)) //check what type of extension  
                    {
                        IsSaved = UploadImage((int)Document_Type.GST, gst, (long)merchantID, System.Web.HttpContext.Current.Server, out Filename, out Ext);
                        merchantKYC.GSTRegistrationImageUrl = Filename;
                    }
                    else
                    {
                        TempData["Messaage"] = "Please choose only Image file in GST";
                        return RedirectToAction("KYC", new { Id = merchantID });
                    }


                }
                if (address != null)
                {
                    var fileNameAddress = Path.GetFileName(address.FileName);
                    var ExtAddress = Path.GetExtension(address.FileName);
                    string nameAddress = Path.GetFileNameWithoutExtension(fileNameAddress);
                    string AddressPath = nameAddress + "_" + merchantID + UniqueCode + ExtAddress;

                    if (allowedExtensions.Contains(ExtAddress)) //check what type of extension  
                    {
                        IsSaved = UploadImage((int)Document_Type.ADDRESS, address, (long)merchantID, System.Web.HttpContext.Current.Server, out Filename, out Ext);
                        merchantKYC.AddressProofUrl = Filename;
                    }
                    else
                    {
                        TempData["Messaage"] = "Please choose only Image file in Address proof";
                        return RedirectToAction("KYC", new { Id = merchantID });
                    }


                }
                if (visitingCard != null)
                {
                    var fileNameVisitingCard = Path.GetFileName(visitingCard.FileName);
                    var ExtVisitingCard = Path.GetExtension(visitingCard.FileName);
                    string nameVisitingCard = Path.GetFileNameWithoutExtension(fileNameVisitingCard);
                    string VisitingCardPath = nameVisitingCard + "_" + merchantID + UniqueCode + ExtVisitingCard;

                    if (allowedExtensions.Contains(ExtVisitingCard)) //check what type of extension  
                    {
                        IsSaved = UploadImage((int)Document_Type.VISTINGCARD, visitingCard, (long)merchantID, System.Web.HttpContext.Current.Server, out Filename, out Ext);
                        merchantKYC.VisingCardImageUrl = Filename;
                    }
                    else
                    {
                        TempData["Messaage"] = "Please choose only Image file in Visting Card";
                        return RedirectToAction("KYC", new { Id = merchantID });
                    }
                }
                if (cancelledCheque != null)
                {

                    var fileNameCancelledCheque = Path.GetFileName(cancelledCheque.FileName);
                    var ExtCancelledCheque = Path.GetExtension(cancelledCheque.FileName);
                    string nameCancelledCheque = Path.GetFileNameWithoutExtension(fileNameCancelledCheque);
                    string CancelledChequePath = nameCancelledCheque + "_" + merchantID + UniqueCode + ExtCancelledCheque;

                    if (allowedExtensions.Contains(ExtCancelledCheque)) //check what type of extension  
                    {
                        IsSaved = UploadImage((int)Document_Type.CANCELLEDCHEQUE, cancelledCheque, (long)merchantID, System.Web.HttpContext.Current.Server, out Filename, out Ext);
                        merchantKYC.CancelledblankChequeImageUrl = Filename;
                    }
                    else
                    {
                        TempData["Messaage"] = "Please choose only Image file in Cancelled Cheque";
                        return RedirectToAction("KYC", new { Id = merchantID });
                    }
                }
                if (Banner != null)
                {
                    var fileNameBanner = Path.GetFileName(Banner.FileName);
                    var ExtBanner = Path.GetExtension(Banner.FileName);
                    string nameBanner = Path.GetFileNameWithoutExtension(fileNameBanner);
                    string CancelledBanner = nameBanner + "_" + merchantID + UniqueCode + ExtBanner;

                    if (allowedExtensions.Contains(ExtBanner)) //check what type of extension  
                    {
                        IsSaved = UploadImage((int)Document_Type.BANNER, Banner, (long)merchantID, System.Web.HttpContext.Current.Server, out Filename, out Ext);
                        // merchantKYC.BannerImageUrl = Filename;

                        MerchantBanner banner_ = new MerchantBanner();
                        banner_.IsActive = true;
                        banner_.CreateDate = DateTime.Now;
                        banner_.BannerPath = Filename;
                        banner_.MerchantID = merchantKYC.MerchantID;
                        db.merchantBanners.Add(banner_);
                    }
                    else
                    {
                        TempData["Messaage"] = "Please choose only Image file in Banner";
                        return RedirectToAction("KYC", new { Id = merchantID });
                    }
                }
                merchantKYC.ModifyDate = DateTime.Now;

                db.SaveChanges();

                TempData["Alert"] = "Attachment Save Successfully!!!";
                return RedirectToAction("KYC", new { Id = merchantID });
            }
            catch (Exception ex)
            {
                TempData["Messaage"] = ex.Message;
                return RedirectToAction("KYC", new { Id = merchantID });
            }
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
                    FolderName = WebConfigurationManager.AppSettings["MERCHANT_FOLDER_SHOPESTABLISHMENT"];
                }
                else if ((int)Document_Type.PHOTO == status)
                {
                    FolderName = WebConfigurationManager.AppSettings["MERCHANT_FOLDER_PHOTO"];
                }
                else if ((int)Document_Type.PAN == status)
                {
                    FolderName = WebConfigurationManager.AppSettings["MERCHANT_FOLDER_PAN"];
                }
                else if ((int)Document_Type.GST == status)
                {
                    FolderName = WebConfigurationManager.AppSettings["MERCHANT_FOLDER_GST"];
                }
                else if ((int)Document_Type.ADDRESS == status)
                {
                    FolderName = WebConfigurationManager.AppSettings["MERCHANT_FOLDER_ADDRESS"];
                }
                else if ((int)Document_Type.VISTINGCARD == status)
                {
                    FolderName = WebConfigurationManager.AppSettings["MERCHANT_FOLDER_VISITINGCARD"];
                }
                else if ((int)Document_Type.CANCELLEDCHEQUE == status)
                {
                    FolderName = WebConfigurationManager.AppSettings["MERCHANT_FOLDER_CANCELLEDCHEQUE"];
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

        public JsonResult ValidateContactNo(string contactNo, long Id)
        {
            bool result = false;

            try
            {
                Merchant merchant = db.Merchants.Where(u => u.ContactNumber == contactNo && u.Id != Id).FirstOrDefault();
                if (merchant == null)
                {
                    result = true;
                }

            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Error", "There's Something wrong in validating Leader's contact no!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[FranchiseLocation][POST:ValidateContactNo]",
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult MerchantDetails(long MerchantId)
        {
            Merchant merchant = db.Merchants.FirstOrDefault(p => p.Id == MerchantId);
            MerchantDetails details = new MerchantDetails();
            MerchantDetails obj = db.merchantDetails.FirstOrDefault(p => p.MerchantId == MerchantId);
            ServiceIncomeMaster incomeMaster = db.ServiceIncomeMasters.FirstOrDefault();
            if (obj == null)
            {
                details.Company = incomeMaster.Company;
                details.RMCommission = incomeMaster.RelationshipManager;
                details.Level0 = incomeMaster.UserLevel0;
                details.UptoLevel6 = incomeMaster.UptoLevel6;
                details.Part5th = incomeMaster.Part5th;
            }
            else
            {
                details = obj;
            }
            details.MerchantId = MerchantId;
            details.ShopName = merchant.FranchiseName;
            return View(details);
        }

        [HttpPost]
        public ActionResult MerchantDetails(MerchantDetails details)
        {
            MerchantDetails obj = db.merchantDetails.FirstOrDefault(p => p.MerchantId == details.MerchantId);
            if (obj != null)
            {
                obj.Company = details.Company;
                obj.RMCommission = details.RMCommission;
                obj.Level0 = details.Level0;
                obj.UptoLevel6 = details.UptoLevel6;
                obj.Part5th = details.Part5th;
                obj.GST = details.GST;
                obj.IsGSTApply = details.IsGSTApply;
                obj.ModifyDate = DateTime.Now;
            }
            else
            {
                details.CreateDate = DateTime.Now;
                db.merchantDetails.Add(details);
            }
            db.SaveChanges();
            TempData["Result"] = "Saved Successfully";
            return View(details);
        }


        public ActionResult ProfileUpdateList()
        {
            List<MerchantProfile> List = db.merchantProfiles.ToList();
            foreach (var item in List)
            {
                Merchant obj = db.Merchants.FirstOrDefault(p => p.Id == item.MerchantId);
                item.FranchiseName = obj.FranchiseName;
                item.ContactNumber = obj.ContactNumber;
                item.ContactPerson = obj.ContactPerson;
                item.CityName = obj.CityDetail.Name;
            }
            return View(List.OrderByDescending(p => p.CreateDate).ToList());
        }

        public ActionResult ProfileUpdateRequest(long ProfileId)
        {
            Session["ProfileId"] = ProfileId;
            TempData["PreviousUrl"] = System.Web.HttpContext.Current.Request.UrlReferrer;
            if (System.Web.HttpContext.Current.Request.UrlReferrer != null)
            {
                TempData["PreviousUrlPath"] = System.Web.HttpContext.Current.Request.UrlReferrer.PathAndQuery;
            }
            Merchant obj = new Merchant();
            MerchantProfile profile = db.merchantProfiles.FirstOrDefault(p => p.Id == ProfileId);
            try
            {
                obj = db.Merchants.FirstOrDefault(p => p.Id == profile.MerchantId);
                obj.Address = profile.Address;
                obj.Category = profile.Category;
                obj.City = profile.City;
                obj.Comission = profile.Comission;
                obj.ContactNumber = profile.ContactNumber;
                obj.ContactPerson = profile.ContactPerson;
                obj.Email = profile.Email;
                obj.FranchiseName = profile.FranchiseName;
                obj.GoogleMapLink = profile.GoogleMapLink;
                obj.GSTINNo = profile.GSTINNo;
                obj.PANNo = profile.PANNo;
                obj.Pincode = profile.Pincode;
                obj.ShopTiming = profile.ShopTiming;
                obj.SpecialRemark = profile.SpecialRemark;
                obj.State = profile.State;
                obj.Id = profile.MerchantId;
                ViewBag.State = db.States.Where(c => c.IsActive == true).OrderBy(c => c.Name).ToList();
                List<long> ldistrict = db.Districts.Where(x => x.StateID == obj.State).Select(x => x.ID).ToList();
                ViewBag.City = db.Cities.Where(c => ldistrict.Contains(c.DistrictID)).Select(c => new StateCityFranchiseMerchantViewModel { CityID = c.ID, CityName = c.Name }).ToList();
                ViewBag.Pincode = db.Pincodes.Where(x => x.CityID == obj.City).Select(x => new { Name = x.Name, ID = x.ID }).OrderBy(x => x.Name).ToList();
                ViewBag.Category = db.ServiceMasters.Where(c => c.IsActive == true).OrderBy(c => c.Name).ToList();
                ViewBag.Comission = db.CommissionMasters.Where(c => c.IsActive == true).OrderBy(c => c.Commission).ToList();
                var shopTimingList = db.ShopTimingMasters.Where(c => c.IsActive == true).OrderBy(c => c.FromTime).Select(s => new
                {
                    Id = s.Id,
                    Time = s.FromTime + " - " + s.ToTime
                }).ToList();
                ViewBag.ShopTiming = new SelectList(shopTimingList, "Id", "Time");
                ViewBag.Holiday = db.HolidayMasters.Where(c => c.IsActive == true).OrderBy(c => c.Name).ToList();
                ViewBag.Holidays = String.Join(",", db.merchantHolidayUpdates.Where(p => p.MerchantProfileID == profile.Id).Select(p => p.HolidayID).ToList());
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
            return View(obj);
        }

        [HttpPost]
        public ActionResult ProfileUpdateRequest(Merchant profile, string Save, string Reject)
        {
            try
            {
                long ProfileId = Convert.ToInt64(Session["ProfileId"]);
                MerchantProfile Mprofile = db.merchantProfiles.FirstOrDefault(p => p.Id == ProfileId);
                if (Reject == null)
                {
                    Merchant obj = db.Merchants.FirstOrDefault(p => p.Id == profile.Id);
                    obj.Address = profile.Address;
                    obj.Category = profile.Category;
                    obj.City = profile.City;
                    obj.Comission = profile.Comission;
                    obj.ContactNumber = profile.ContactNumber;
                    obj.ContactPerson = profile.ContactPerson;
                    obj.Email = profile.Email;
                    obj.FranchiseName = profile.FranchiseName;
                    obj.GoogleMapLink = profile.GoogleMapLink;
                    obj.GSTINNo = profile.GSTINNo;
                    obj.PANNo = profile.PANNo;
                    obj.Pincode = profile.Pincode;
                    obj.ShopTiming = profile.ShopTiming;
                    obj.SpecialRemark = profile.SpecialRemark;
                    obj.State = profile.State;
                    obj.ModifyDate = DateTime.Now;
                    Mprofile.Status = "Updated";
                }
                else
                {
                    Mprofile.Status = "Rejected";
                }
                db.SaveChanges();
            }
            catch (Exception ex)
            {

            }
            TempData["Result"] = "Saved Successfully";
            return RedirectToAction("ProfileUpdateList");
        }

        public ActionResult BannerImageList()
        {
            List<MerchantBannerUpdateRequest> list = db.merchantBannerUpdateRequests.ToList();
            foreach (var item in list)
            {
                Merchant merchant = db.Merchants.FirstOrDefault(p => p.Id == item.MerchantID);
                item.ShopName = merchant.FranchiseName;
                string result = "";
                item.Remark = GetImageUpdateText(item, out result);
                item.Cityname = merchant.CityDetail.Name;
            }
            return View(list.OrderByDescending(p => p.CreateDate).ToList());
        }

        public ActionResult BannerImageUpdate(long ProfileId)
        {
            MerchantBannerUpdateRequest item = db.merchantBannerUpdateRequests.FirstOrDefault(p => p.ID == ProfileId);
            item.ShopName = db.Merchants.FirstOrDefault(p => p.Id == item.MerchantID).FranchiseName;
            string result = "";
            item.Remark = GetImageUpdateText(item, out result);
            ViewBag.Result = result;
            return View(item);
        }

        public string GetImageUpdateText(MerchantBannerUpdateRequest item, out string result)
        {
            result = "";
            if (item.ShopPath == "True" || item.ShopPath == "False")
            {
                if (item.ShopPath == "True")
                {
                    item.Remark = "Existing Banner image set to Display ON";
                    result = "1";

                }
                else
                {
                    item.Remark = "Existing Banner image set to Display OFF";
                    result = "2";
                }
                item.BannerPath = ConfigurationManager.AppSettings["IMAGE_HTTP"] + ConfigurationManager.AppSettings["MERCHANT_DIRECTORY"] + "/" + ConfigurationManager.AppSettings["MERCHANT_FOLDER_BANNER"] + "/" + item.BannerPath;

            }
            else if (item.ShopPath == null || item.ShopPath == "")
            {
                if (item.BannerPath != null)
                {
                    item.Remark = "New Banner image added.";
                    item.BannerPath = ConfigurationManager.AppSettings["IMAGE_HTTP"] + ConfigurationManager.AppSettings["MERCHANT_DIRECTORY"] + "/" + ConfigurationManager.AppSettings["MERCHANT_FOLDER_BANNER"] + "/" + item.BannerPath;
                    result = "3";
                }
            }
            else
            {
                if (item.BannerPath != null)
                {
                    item.Remark = "New Banner and Shop image added.";
                    result = "4";
                    item.BannerPath = ConfigurationManager.AppSettings["IMAGE_HTTP"] + ConfigurationManager.AppSettings["MERCHANT_DIRECTORY"] + "/" + ConfigurationManager.AppSettings["MERCHANT_FOLDER_BANNER"] + "/" + item.BannerPath;
                    item.ShopPath = ConfigurationManager.AppSettings["IMAGE_HTTP"] + ConfigurationManager.AppSettings["MERCHANT_DIRECTORY"] + "/" + ConfigurationManager.AppSettings["MERCHANT_FOLDER_PHOTO"] + "/" + item.ShopPath;
                }
                else
                {
                    item.Remark = "New Shop image added.";
                    result = "5";
                    item.ShopPath = ConfigurationManager.AppSettings["IMAGE_HTTP"] + ConfigurationManager.AppSettings["MERCHANT_DIRECTORY"] + "/" + ConfigurationManager.AppSettings["MERCHANT_FOLDER_PHOTO"] + "/" + item.ShopPath;
                }
            }
            return item.Remark;
        }

        [HttpPost]
        public ActionResult BannerImageUpdate(MerchantBannerUpdateRequest obj, string Accept, string Reject)
        {

            MerchantBannerUpdateRequest item = db.merchantBannerUpdateRequests.FirstOrDefault(p => p.ID == obj.ID);
            obj.BannerPath = item.BannerPath;
            obj.ShopPath = item.ShopPath;
            MerchantKYC kYC = db.MerchantKYCs.FirstOrDefault(p => p.MerchantID == obj.MerchantID);
            //List<MerchantBanner> list = db.merchantBanners.Where(p => p.MerchantID == obj.MerchantID).ToList();
            string result = "";
            item.Remark = GetImageUpdateText(item, out result);
            if (Accept != null)
            {
                if (result == "1")
                {
                    MerchantBanner banner = db.merchantBanners.FirstOrDefault(p => p.MerchantID == obj.MerchantID && p.BannerPath == obj.BannerPath);
                    banner.IsActive = true;
                    banner.ModifyDate = DateTime.Now;
                }
                else if (result == "2")
                {
                    MerchantBanner banner = db.merchantBanners.FirstOrDefault(p => p.MerchantID == obj.MerchantID && p.BannerPath == obj.BannerPath);
                    banner.IsActive = false;
                    banner.ModifyDate = DateTime.Now;
                }
                else if (result == "3" || result == "4")
                {
                    MerchantBanner banner = new MerchantBanner();
                    banner.BannerPath = obj.BannerPath;
                    banner.CreateDate = DateTime.Now;
                    banner.IsActive = true;
                    banner.MerchantID = item.MerchantID;
                    db.merchantBanners.Add(banner);
                }
                if (result == "4" || result == "5")
                {
                    kYC.PhotoImageUrl = obj.ShopPath;
                    kYC.ModifyDate = DateTime.Now;
                }
                item.IsActive = true;
                item.Status = 1;
            }
            else
            {
                item.IsActive = false;
                item.Status = 2;
            }
            item.BannerPath = obj.BannerPath;
            item.ShopPath = obj.ShopPath;
            item.ModifyDate = DateTime.Now;
            db.SaveChanges();


            TempData["Result"] = "Saved Successfully";
            return RedirectToAction("BannerImageList");
        }

        public ActionResult MerchantApprovedBannerList(long MerchantId)
        {
            List<MerchantBanner> merchantBanners = db.merchantBanners.Where(p => p.MerchantID == MerchantId).OrderByDescending(p => p.CreateDate).ToList();
            foreach (var item in merchantBanners)
            {
                item.BannerPath = ConfigurationManager.AppSettings["IMAGE_HTTP"] + ConfigurationManager.AppSettings["MERCHANT_DIRECTORY"] + "/" + ConfigurationManager.AppSettings["MERCHANT_FOLDER_BANNER"] + "/" + item.BannerPath;
            }
            ViewBag.Merchantname = db.Merchants.FirstOrDefault(p => p.Id == MerchantId).FranchiseName;
            ViewBag.MerchantId = MerchantId;
            return View(merchantBanners);
        }

        public ActionResult EditApprovedBanner(long id)
        {
            MerchantBanner banner = db.merchantBanners.FirstOrDefault(p => p.ID == id);
            banner.BannerPath = ConfigurationManager.AppSettings["IMAGE_HTTP"] + ConfigurationManager.AppSettings["MERCHANT_DIRECTORY"] + "/" + ConfigurationManager.AppSettings["MERCHANT_FOLDER_BANNER"] + "/" + banner.BannerPath;
            ViewBag.Merchantname = db.Merchants.FirstOrDefault(p => p.Id == banner.MerchantID).FranchiseName;
            return View(banner);
        }

        [HttpPost]
        public ActionResult EditApprovedBanner(MerchantBanner obj)
        {
            MerchantBanner banner = db.merchantBanners.FirstOrDefault(p => p.ID == obj.ID);
            banner.IsActive = obj.IsActive;
            banner.ModifyDate = DateTime.Now;
            db.SaveChanges();
            TempData["Result"] = "Update Successfully!!!";
            return RedirectToAction("MerchantApprovedBannerList", new { MerchantId = obj.MerchantID });
        }

       
    }
}