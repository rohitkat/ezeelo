using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ModelLayer.Models;
using ModelLayer.Models.ViewModel;
using Leaders.Filter;
using System.Text.RegularExpressions;
using BusinessLogicLayer;
using System.Web.UI.WebControls;
using System.IO;
using System.Web.UI;
using System.Data.SqlClient;

namespace Leaders.Areas.Admin.Controllers
{
    [AdminSessionExpire]
    public class PayoutRequestController : Controller
    {
        private EzeeloDBContext db = new EzeeloDBContext();
        [SessionExpire]
        public ActionResult Index()
        {
            return View(GetPayoutList().Where(x => x.IsActive == true || x.IsActive == null));
        }

        public ActionResult AccountsIndex()
        {
            return View(GetPayoutList().Where(x => x.RequestedStatus != 2));
        }

        public List<LeadersPayoutRequestViewModel> GetPayoutList()
        {
            List<LeadersPayoutRequestViewModel> payoutList = db.PersonalDetails.Join(db.LeadersPayoutRequests, pd => pd.UserLoginID, lpay => lpay.UserLoginID,
                (pd, lpay) => new LeadersPayoutRequestViewModel
                {
                    AccountHolderName = pd.FirstName + "" + pd.LastName,
                    BenificiaryName = db.KYCModels.Where(x => x.UserLoginID == lpay.UserLoginID).Select(y => y.BenificiaryName).FirstOrDefault(),
                    Email = db.UserLogins.Where(x => x.ID == lpay.UserLoginID).Select(y => y.Email).FirstOrDefault(),
                    UserLoginID = lpay.UserLoginID,
                    RedeamableCash = lpay.RedeamableAmount,
                    RequestedAmount = lpay.RequestedAmount,
                    GSTAmount = lpay.GSTAmount,
                    GST = lpay.GST,
                    TDS = lpay.TDS,
                    TDSAmount = lpay.TDSAmount,
                    MinReservedAmount = lpay.MinReservedAmount,
                    MinReserved = lpay.MinReserved,
                    Penalty = lpay.Penalty,
                    PenaltyAmount = lpay.PenaltyAmount,
                    ProcessingFees = lpay.ProcessingFees,
                    TotalAmount = lpay.TotalAmount,
                    CreateDate = lpay.Create_Date,
                    RequestedStatus = lpay.RequestStatus,
                    IsActive = lpay.IsActive ?? false,
                    RequestID = lpay.RequestID ?? 0

                }).OrderByDescending(y => y.CreateDate).ToList();

            foreach (var item in payoutList)
            {
                if (item.RequestedStatus == 0)
                {
                    item.Status = "Pending";

                }
                else if (item.RequestedStatus == 1)
                {
                    item.Status = "Accepted";
                }
                else if (item.RequestedStatus == 2)
                {
                    item.Status = "Approved";
                }
                else if (item.RequestedStatus == 3)
                {
                    item.Status = "Cancelled";
                }
            }
            return payoutList;
        }
        [SessionExpire]
        public ActionResult Approval(int id, int status)
        {
            LeadersPayoutRequest objPayoutRequest = db.LeadersPayoutRequests.Where(x => x.UserLoginID == id && x.IsActive == true).FirstOrDefault();
            if (objPayoutRequest != null)

            {
                MLMWallet objWallet = db.MLMWallets.Where(x => x.UserLoginID == id).FirstOrDefault();
                if (objPayoutRequest != null)
                {
                    if (status == 1)
                    {
                        //objWallet.Amount = (Decimal)(objWallet.Amount - objPayoutRequest.RequestedAmount);
                        // var requestedPoints = objPayoutRequest.RequestedAmount * 10;
                        //objWallet.Points = (Decimal)(objWallet.Points - requestedPoints);

                        objPayoutRequest.IsActive = true;
                    }
                    if (status == 3)
                    {
                        objWallet.Amount = (Decimal)(objWallet.Amount + objPayoutRequest.RequestedAmount);
                        var requestedPoints = objPayoutRequest.RequestedAmount * 10;
                        objWallet.Points = (Decimal)(objWallet.Points + requestedPoints);

                        objPayoutRequest.IsActive = false;

                        objPayoutRequest.Remark = "Cancelled By Admin";
                        objPayoutRequest.RequestStatus_Date = System.DateTime.Now;

                    }
                    // LeadersPayoutRequest objRequest = db.LeadersPayoutRequests.Where(x => x.UserLoginID == id).FirstOrDefault();
                    objPayoutRequest.RequestStatus = status;
                    objPayoutRequest.RequestStatus_Date = System.DateTime.Now;
                    objPayoutRequest.PaymentStatus = false;
                    objPayoutRequest.TransactionID = "xxxxxxxxxxx";
                    objPayoutRequest.TransactionDate = System.DateTime.Now;


                }

                db.SaveChanges();

                if (status == 1)
                {
                    SendEmail_OnAccept(id);
                    SendSMSToCustomer(id, status);
                    (new SendFCMNotification()).SendNotification("withdrawn_accept", id);
                }
                if (status == 3)
                {
                    SendSMSToCustomer(id, status);
                    SendEmail_OnCancel(id);
                    (new SendFCMNotification()).SendNotification("withdrawn_cancel", id);
                }
                if (status == 2)
                {
                    (new SendFCMNotification()).SendNotification("withdrawn_approve", id);
                }
            }
            return RedirectToAction("Index");
        }

        public void SendEmail_OnCancel(long userLoginID)
        {
            try
            {
                string mobile = db.UserLogins.Where(x => x.ID == userLoginID).Select(x => x.Mobile).FirstOrDefault();
                string name = db.PersonalDetails.Where(u => u.UserLoginID == userLoginID).Select(y => y.FirstName + "" + y.LastName).FirstOrDefault();
                string Email = db.UserLogins.Where(x => x.ID == userLoginID).Select(x => x.Email).FirstOrDefault();
                string Password = db.UserLogins.Where(x => x.ID == userLoginID).Select(y => y.Password).FirstOrDefault();
                string TotalAmount = db.LeadersPayoutRequests.Where(x => x.UserLoginID == userLoginID).Select(y => y.RequestedAmount).FirstOrDefault().ToString();
                string RequestedDate = db.LeadersPayoutRequests.Where(x => x.UserLoginID == userLoginID).OrderByDescending(y => y.Create_Date).Select(y => y.Create_Date).FirstOrDefault().ToString();
                string AccountNo = db.KYCModels.Where(y => y.UserLoginID == userLoginID).Select(y => y.AccountNo).FirstOrDefault().ToString();
                string reasonCancel = db.LeadersPayoutRequests.Where(y => y.UserLoginID == userLoginID && y.RequestStatus == 3).OrderByDescending(y => y.Create_Date).Select(y => y.Remark).FirstOrDefault();

                string requestedAmount = db.LeadersPayoutRequests.Where(x => x.UserLoginID == userLoginID && x.RequestStatus == 3).OrderByDescending(y => y.Create_Date).Select(y => y.RequestedAmount).FirstOrDefault().ToString();

                string requesteID = db.LeadersPayoutRequests.Where(x => x.UserLoginID == userLoginID && x.RequestStatus == 3).OrderByDescending(y => y.Create_Date).Select(y => y.RequestID).FirstOrDefault().ToString();

                var UserLogin = db.PersonalDetails.Where(u => u.UserLoginID == userLoginID)
                    .Join(db.MLMUsers, u => u.UserLoginID, m => m.UserID, (u, m) => new
                    {
                        RefferalCode = m.Ref_Id,
                        Name = u.FirstName
                    }).ToList();

                var last4 = Regex.Match(AccountNo, @"(.{4})\s*$");
                AccountNo = "xxxxxxxxxx" + last4;


                var last2Mobile = Regex.Match(mobile, @"(.{2})\s*$");
                mobile = "xxxxxxxx" + last2Mobile;

                if (UserLogin != null && UserLogin.Count() > 0)
                {
                    string Name = UserLogin.First().Name;
                    // string RefferalCode = UserLogin.First().RefferalCode;
                    string URL = "http://leaders.ezeelo.com/LeadersLogin/?Email=" + Email + "&Password=" + Password;
                    // string URL = "" + (new URLsFromConfig()).GetURL("LEADERS") + "LeadersLogin/Login/?UserName=" + Email + "&Password=" + Password;
                    Dictionary<string, string> dictEmailValues = new Dictionary<string, string>();


                    dictEmailValues.Add("<!--Requested_Date-->", RequestedDate);
                    dictEmailValues.Add("<!--Customer_Name-->", Name);
                    dictEmailValues.Add("<!--Requested_Amount-->", requestedAmount);
                    dictEmailValues.Add("<!--Account_Number-->", AccountNo);
                    dictEmailValues.Add("<!--Reason_Cancel-->", reasonCancel);

                    dictEmailValues.Add("<!--Mobile-->", mobile);
                    dictEmailValues.Add("<!--Requeste_ID-->", requesteID);

                    dictEmailValues.Add("<!--URL-->", URL);
                    BusinessLogicLayer.GateWay gateWay = new BusinessLogicLayer.Email(System.Web.HttpContext.Current.Server);
                    //string EmailID = "tech@ezeelo.com";

                    // gateWay.SendEmail(BusinessLogicLayer.GateWay.EmailGateWays.GANDHIBAGH, BusinessLogicLayer.GateWay.SenderMail.INFO, BusinessLogicLayer.GateWay.EMailTypes.ON_CANCEL_PAYOUT_REQUEST, new string[] { Email, "support@ezeelo.com" }, dictEmailValues, true);
                    gateWay.SendEmail(BusinessLogicLayer.GateWay.EmailGateWays.GANDHIBAGH, BusinessLogicLayer.GateWay.SenderMail.INFO, BusinessLogicLayer.GateWay.EMailTypes.ON_CANCEL_PAYOUT_REQUEST, new string[] { "gaurav.shrote@ezeelo.com" }, dictEmailValues, true);

                }
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[DashboardController][M:SendEmail_InviteUser]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.BussinessLogicLayer, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[DashboardController][M:SendEmail_InviteUser]",
                    BusinessLogicLayer.ErrorLog.Module.BussinessLogicLayer, System.Web.HttpContext.Current.Server);
            }
        }

        public void SendEmail_OnAccept(long userLoginID)
        {
            try
            {
                string requestedStatus = "";
                string mobile = db.UserLogins.Where(x => x.ID == userLoginID).Select(x => x.Mobile).FirstOrDefault();
                string name = db.PersonalDetails.Where(u => u.UserLoginID == userLoginID).Select(y => y.FirstName + "" + y.LastName).FirstOrDefault();
                string Email = db.UserLogins.Where(x => x.ID == userLoginID).Select(x => x.Email).FirstOrDefault();
                string Password = db.UserLogins.Where(x => x.ID == userLoginID).Select(y => y.Password).FirstOrDefault();
                String TotalAmount = db.LeadersPayoutRequests.Where(x => x.UserLoginID == userLoginID).Select(y => y.RequestedAmount).FirstOrDefault().ToString();
                string RequestedDate = db.LeadersPayoutRequests.Where(x => x.UserLoginID == userLoginID).Select(y => y.Create_Date).ToString();
                string AccountNo = db.KYCModels.Where(y => y.UserLoginID == userLoginID).Select(y => y.AccountNo).ToString();

                LeadersPayoutRequest objLeadersPayoutRequest = db.LeadersPayoutRequests.Where(x => x.UserLoginID == userLoginID && x.IsActive == true).FirstOrDefault();


                var UserLogin = db.PersonalDetails.Where(u => u.UserLoginID == userLoginID)
                    .Join(db.MLMUsers, u => u.UserLoginID, m => m.UserID, (u, m) => new
                    {
                        RefferalCode = m.Ref_Id,
                        Name = u.FirstName
                    }).ToList();


                if (UserLogin != null && UserLogin.Count() > 0)
                {
                    string gst = objLeadersPayoutRequest.GST.ToString() ?? "0";
                    string gstAmount = objLeadersPayoutRequest.GSTAmount.ToString() ?? "0";
                    string tds = objLeadersPayoutRequest.TDS.ToString() ?? "0";
                    string tdsAmount = objLeadersPayoutRequest.TDSAmount.ToString() ?? "0";
                    string processingFees = objLeadersPayoutRequest.ProcessingFees.ToString() ?? "0";
                    string minReserved = objLeadersPayoutRequest.MinReserved.ToString() ?? "0";
                    string minReservedAmount = objLeadersPayoutRequest.MinReservedAmount.ToString() ?? "0";
                    string requestedAmount = objLeadersPayoutRequest.RequestedAmount.ToString() ?? "0";
                    string penalty = objLeadersPayoutRequest.Penalty.ToString() ?? "0";
                    string penaltyAmount = objLeadersPayoutRequest.PenaltyAmount.ToString() ?? "0";
                    string totalAmount = objLeadersPayoutRequest.TotalAmount.ToString() ?? "0";
                    string requestedDate = objLeadersPayoutRequest.Create_Date.ToString();
                    string RequestID = objLeadersPayoutRequest.RequestID.ToString();
                    string redeamableAmount = objLeadersPayoutRequest.RedeamableAmount.ToString();
                    if (objLeadersPayoutRequest.RequestStatus == 1)
                    {
                        requestedStatus = "Accepted";
                    }

                    string AccountNumber = (db.KYCModels.Where(x => x.UserLoginID == userLoginID).Select(y => y.AccountNo).FirstOrDefault());
                    //var XXNo = "XXXXXXXXXXXX";

                    var last4 = Regex.Match(AccountNumber, @"(.{4})\s*$");
                    AccountNo = "xxxxxxxxxx" + last4;

                    var last4Mobile = Regex.Match(mobile, @"(.{2})\s*$");
                    mobile = "xxxxxxxx" + last4Mobile;
                    string refferalID = db.MLMUsers.Where(x => x.UserID == userLoginID).Select(y => y.Ref_Id).FirstOrDefault(); ;


                    string Name = UserLogin.First().Name;
                    string RefferalCode = UserLogin.First().RefferalCode;
                    string URL = "http://leaders.ezeelo.com/LeadersLogin/?Email=" + Email + "&Password=" + Password;
                    // string URL = "" + (new URLsFromConfig()).GetURL("LEADERS") + "LeadersLogin/Login/?UserName=" + Email + "&Password=" + Password;

                    Dictionary<string, string> dictEmailValues = new Dictionary<string, string>();
                    dictEmailValues.Add("<!--RequestedDate-->", requestedDate);
                    dictEmailValues.Add("<!--Customer_Name-->", name);
                    dictEmailValues.Add("<!--Referral_ID-->", refferalID);
                    dictEmailValues.Add("<!--Redeemable_Cash-->", redeamableAmount);
                    dictEmailValues.Add("<!--Requested_Amount-->", requestedAmount);
                    dictEmailValues.Add("<!--GST-->", gst);
                    dictEmailValues.Add("<!--GST_Amount-->", gstAmount);
                    dictEmailValues.Add("<!--TDS_Amount-->", tdsAmount);
                    dictEmailValues.Add("<!--TDS-->", tds);
                    dictEmailValues.Add("<!--Min_Reserved-->", minReserved);

                    dictEmailValues.Add("<!--Min_Reserved_Amount-->", minReservedAmount);
                    dictEmailValues.Add("<!--Penalty-->", penalty);
                    dictEmailValues.Add("<!--Penalty_Amount-->", penaltyAmount);
                    dictEmailValues.Add("<!--Processing_Fees-->", processingFees);
                    dictEmailValues.Add("<!--Total_Amount-->", totalAmount);
                    dictEmailValues.Add("<!--Request_Date-->", requestedDate);
                    dictEmailValues.Add("<!--Request_Status-->", requestedStatus);
                    dictEmailValues.Add("<!--Account_Number-->", AccountNo);
                    dictEmailValues.Add("<!--Request_ID-->", RequestID);
                    dictEmailValues.Add("<!--URL-->", URL);
                    BusinessLogicLayer.GateWay gateWay = new BusinessLogicLayer.Email(System.Web.HttpContext.Current.Server);
                    string EmailID = "tech@ezeelo.com";

                    //gateWay.SendEmail(BusinessLogicLayer.GateWay.EmailGateWays.GANDHIBAGH, BusinessLogicLayer.GateWay.SenderMail.INFO, BusinessLogicLayer.GateWay.EMailTypes.ON_APPROVAL_ACCOUNT_TEAM_PAYOUT_REQUEST, new string[] { "disha.modak@ezeelo.com" }, dictEmailValues, true);
                    //gateWay.SendEmail(BusinessLogicLayer.GateWay.EmailGateWays.GANDHIBAGH, BusinessLogicLayer.GateWay.SenderMail.INFO, BusinessLogicLayer.GateWay.EMailTypes.ON_APPROVAL_ACCOUNT_TEAM_PAYOUT_REQUEST, new string[] { "gaurav.shrote@ezeelo.com  " }, dictEmailValues, true);
                }
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[DashboardController][M:SendEmail_InviteUser]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.BussinessLogicLayer, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[DashboardController][M:SendEmail_InviteUser]",
                    BusinessLogicLayer.ErrorLog.Module.BussinessLogicLayer, System.Web.HttpContext.Current.Server);
            }
        }

        public void SendSMSToCustomer(long userLoginID, int status)
        {
            string requestID = "";
            string requestedAmount = "";

            try
            {
                // var lOrder = db.CustomerOrders.Where(x => x.ID == orderId).ToList();

                if (status == 0)
                {
                    requestID = db.LeadersPayoutRequests.Where(x => x.UserLoginID == userLoginID && x.RequestStatus == 0).OrderByDescending(y => y.RequestStatus_Date).Select(y => y.RequestID).FirstOrDefault().ToString();
                    requestedAmount = db.LeadersPayoutRequests.Where(y => y.UserLoginID == userLoginID && y.RequestStatus == 0).OrderByDescending(x => x.RequestStatus_Date).Select(y => y.RequestedAmount).FirstOrDefault().ToString();
                }
                if (status == 1)
                {
                    requestID = db.LeadersPayoutRequests.Where(x => x.UserLoginID == userLoginID && x.RequestStatus == 1).OrderByDescending(y => y.RequestStatus_Date).Select(y => y.RequestID).FirstOrDefault().ToString();
                    requestedAmount = db.LeadersPayoutRequests.Where(y => y.UserLoginID == userLoginID && y.RequestStatus == 1).OrderByDescending(x => x.RequestStatus_Date).Select(y => y.RequestedAmount).FirstOrDefault().ToString();
                }
                if (status == 3)
                {
                    requestID = db.LeadersPayoutRequests.Where(x => x.UserLoginID == userLoginID && x.RequestStatus == 3).OrderByDescending(y => y.RequestStatus_Date).Select(y => y.RequestID).FirstOrDefault().ToString();
                    requestedAmount = db.LeadersPayoutRequests.Where(y => y.UserLoginID == userLoginID && y.RequestStatus == 3).OrderByDescending(y => y.RequestStatus_Date).Select(y => y.RequestedAmount).FirstOrDefault().ToString();

                }

                string mobileNo = db.UserLogins.Where(x => x.ID == userLoginID).Select(y => y.Mobile).FirstOrDefault();
                var lPersonalDetails = db.PersonalDetails.Where(x => x.UserLoginID == userLoginID).ToList();
                string accountNo = db.KYCModels.Where(x => x.UserLoginID == userLoginID).Select(y => y.AccountNo).FirstOrDefault();


                Dictionary<string, string> dictSMSValues = new Dictionary<string, string>();

                // Dictionary<string, string> dictSMSValues = new Dictionary<string, string>();
                dictSMSValues.Add("#--NAME--#", Convert.ToString(lPersonalDetails.FirstOrDefault().FirstName));
                dictSMSValues.Add("#--REQUEST_ID--#", requestID);
                dictSMSValues.Add("#--REQUEST_AMOUNT--#", requestedAmount);
                dictSMSValues.Add("#--ACCOUNT_NUMBER--#", accountNo);

                BusinessLogicLayer.GateWay gateWay = new BusinessLogicLayer.SMS(System.Web.HttpContext.Current.Server);
                if (status == 3)
                {
                    //gateWay.SendSMS(BusinessLogicLayer.GateWay.SMSGateWays.SUMIT, BusinessLogicLayer.GateWay.SMSOptions.SINGLE, BusinessLogicLayer.GateWay.SMSTypes.CANCEL_PAYMENT_PAYOUT_REQUEST, new string[] { "7020176511" }, dictSMSValues);

                }
                if (status == 1)
                {
                    //gateWay.SendSMS(BusinessLogicLayer.GateWay.SMSGateWays.SUMIT, BusinessLogicLayer.GateWay.SMSOptions.SINGLE, BusinessLogicLayer.GateWay.SMSTypes.ACCEPT_PAYOUT_REQUEST, new string[] { "7020176511" }, dictSMSValues);

                }

                if (status == 0)
                {
                    //gateWay.SendSMS(BusinessLogicLayer.GateWay.SMSGateWays.SUMIT, BusinessLogicLayer.GateWay.SMSOptions.SINGLE, BusinessLogicLayer.GateWay.SMSTypes.CUST_PAYOUT_REQUEST, new string[] { "7020176511" }, dictSMSValues);

                }
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[PaymentProcessController][M:SendOTPToCustomer]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Gandhibagh, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[PaymentProcessController][M:SendOTPToCustomer]",
                    BusinessLogicLayer.ErrorLog.Module.Gandhibagh, System.Web.HttpContext.Current.Server);
            }

        }

        public ActionResult GetUserist()
        {
            return View();
        }

        public ActionResult GetReport(string AllData)
        {
            List<LeadersPayoutRequestViewModel> list = GetData(AllData,"0");
            return PartialView("_GetAllData", list);
        }

        public List<LeadersPayoutRequestViewModel> GetData(string AllData,string flag)
        {
            List<LeadersPayoutRequestViewModel> list = new List<LeadersPayoutRequestViewModel>();
            var flag_ = new SqlParameter
            {
                ParameterName = "flag",
                Value = flag
            };
            list = db.Database.SqlQuery<LeadersPayoutRequestViewModel>("exec SendPayoutRequest @flag", flag_).ToList();
            //list = db.Database.SqlQuery<LeadersPayoutRequestViewModel>("exec SendPayoutRequest").ToList();
            if (AllData != "true" && flag == "0")
            {
                list = list.Where(p => p.isAllowed == "Yes" && p.RedeamableCash > 1).ToList();
            }
            return list;
        }

        public ActionResult SendPayoutRequest(string AllData)
        {
            try
            {
                List<LeadersPayoutRequestViewModel> list = GetData(AllData, "1");
                if (list.Count() == 0)
                {
                    return Json("No Record found for Payout Request!!!", JsonRequestBehavior.AllowGet);
                }
                else
                {
                    foreach (var item in list)
                    {
                        try
                        {
                            SendEmail_BeforePayment(item.UserLoginID, item.TotalAmount.ToString(), "0");
                            SendSMSToCustomer(item.UserLoginID, 0);
                        }
                        catch
                        {

                        }
                    }
                    return Json("Payout Request send successfully for "+ list.Count()+ " users!!!", JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }
        public void SendEmail_BeforePayment(long userLoginID, string TotalAmount, string WalletAmount)
        {
            try
            {
                string mobile = db.UserLogins.Where(x => x.ID == userLoginID).Select(x => x.Mobile).FirstOrDefault();
                string name = db.PersonalDetails.Where(u => u.UserLoginID == userLoginID).Select(y => y.FirstName + "" + y.LastName).FirstOrDefault();
                string Email = db.UserLogins.Where(x => x.ID == userLoginID).Select(y => y.Email).FirstOrDefault();

                string refferalID = db.MLMUsers.Where(x => x.UserID == userLoginID).Select(y => y.Ref_Id).FirstOrDefault();


                LeadersPayoutRequest objLeadersPayout = db.LeadersPayoutRequests.Where(x => x.UserLoginID == userLoginID && x.IsActive == true).FirstOrDefault();
                string requested_ID = objLeadersPayout.RequestID.ToString();

                string gstAmount = objLeadersPayout.GSTAmount.ToString() ?? "0";
                string tdsAmount = objLeadersPayout.TDSAmount.ToString() ?? "0";
                string processingFees = objLeadersPayout.ProcessingFees.ToString() ?? "0";
               string  Password = db.UserLogins.Where(x => x.ID == userLoginID).Select(y => y.Password).FirstOrDefault();

                string AccountNo = db.KYCModels.Where(x => x.UserLoginID == userLoginID).Select(y => y.AccountNo).FirstOrDefault();

                string requestedAmount = objLeadersPayout.RequestedAmount.ToString() ?? "0";
                string requestedDate = System.DateTime.Now.ToString();

                var last4 = Regex.Match(AccountNo, @"(.{4})\s*$");


                var last4Mobile = Regex.Match(mobile, @"(.{2})\s*$");
                mobile = "xxxxxxxx" + last4Mobile;
                AccountNo = "xxxxxxxxx" + last4;


                var UserLogin = db.PersonalDetails.Where(u => u.UserLoginID == userLoginID)
                    .Join(db.MLMUsers, u => u.UserLoginID, m => m.UserID, (u, m) => new
                    {
                        RefferalCode = m.Ref_Id,
                        Name = u.FirstName
                    }).ToList();

                if (UserLogin != null && UserLogin.Count() > 0)
                {
                    string Name = UserLogin.First().Name;
                    string RefferalCode = UserLogin.First().RefferalCode;
                    //string URL = "http://leaders.ezeelo.com/LeadersLogin/?Email=" + Email + "&Password=" + Password;
                    string URL = "" + (new URLsFromConfig()).GetURL("LEADERS") + "LeadersLogin/Login/?UserName=" + Email + "&Password=" + "zoya2388";
                    Dictionary<string, string> dictEmailValues = new Dictionary<string, string>();
                    dictEmailValues.Add("<!--Total_Amount-->", TotalAmount);
                    dictEmailValues.Add("<!--Customer_Name-->", name);
                    dictEmailValues.Add("<!--Request_ID-->", requested_ID);
                    dictEmailValues.Add("<!--URL-->", URL);
                    dictEmailValues.Add("<!--Requested_Amount-->", requestedAmount);
                    dictEmailValues.Add("<!--GST_Amount-->", gstAmount);
                    dictEmailValues.Add("<!--TDS_Amount-->", tdsAmount);
                    dictEmailValues.Add("<!--Processing_Fees-->", processingFees);
                    dictEmailValues.Add("<!--Account_Number-->", AccountNo);

                    dictEmailValues.Add("<!--Request_Date-->", requestedDate);

                    dictEmailValues.Add("<!--Mobile-->", mobile);

                    BusinessLogicLayer.GateWay gateWay = new BusinessLogicLayer.Email(System.Web.HttpContext.Current.Server);
                    //string EmailID = "tech@ezeelo.com";

                    // gateWay.SendEmail(BusinessLogicLayer.GateWay.EmailGateWays.GANDHIBAGH, BusinessLogicLayer.GateWay.SenderMail.INFO, BusinessLogicLayer.GateWay.EMailTypes.CUST_PAYOUT_REQUEST, new string[] { "amit.pantawne@ezeelo.com", "sales@ezeelo.com" }, dictEmailValues, true);

                    ReadConfig readConfig = new ReadConfig(System.Web.HttpContext.Current.Server);//Added by Sonali on 18-04-2019

                    string leaderAdminEmail = readConfig.LEADERS_ADMIN_EMAILID;
                    gateWay.SendEmail(BusinessLogicLayer.GateWay.EmailGateWays.GANDHIBAGH, BusinessLogicLayer.GateWay.SenderMail.INFO, BusinessLogicLayer.GateWay.EMailTypes.ON_CUST_PAYOUT_REQUEST, new string[] { leaderAdminEmail }, dictEmailValues, true);
                }
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[PayoutRequest][M:SendEmail_BeforePayment]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.BussinessLogicLayer, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                 + Environment.NewLine + ex.Message + Environment.NewLine
                 + "[PayoutRequest][M:SendEmail_BeforePayment]",
                 BusinessLogicLayer.ErrorLog.Module.BussinessLogicLayer, System.Web.HttpContext.Current.Server);
            }
        }

        public ActionResult ExportToExcel(string AllData)
        {
            List<LeadersPayoutRequestViewModel> list = GetData(AllData,"0");
            var gv = new GridView();
            string FileName = "User List For Payout Request";
            
            gv.DataSource = list.Select(p => new { p.UserLoginID, p.Name, p.Email, p.Mobile, p.EzeeloWalletCash, p.RedeamableCash, p.GSTAmount, p.TDSAmount, p.MinReservedAmount, p.PenaltyAmount, p.ProcessingFeesAmount, p.TotalAmount,p.isAllowed });
            gv.DataBind();


            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename=" + FileName + ".xls");
            Response.ContentType = "application/ms-excel";
            Response.Charset = "";
            StringWriter objStringWriter = new StringWriter();
            HtmlTextWriter objHtmlTextWriter = new HtmlTextWriter(objStringWriter);
            gv.RenderControl(objHtmlTextWriter);

            Response.Output.Write(objStringWriter.ToString());
            Response.Flush();
            Response.End();
            return RedirectToAction("GetUserist");
        }
    }
}