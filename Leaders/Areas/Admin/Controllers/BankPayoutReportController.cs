using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;
using ModelLayer.Models;
using ModelLayer.Models.ViewModel;
using BusinessLogicLayer;
using System.Text.RegularExpressions;
using Leaders.Filter;

namespace Leaders.Areas.Admin.Controllers
{
    [AdminSessionExpire]
    public class BankPayoutReportController : Controller
    {
        EzeeloDBContext db = new EzeeloDBContext();
        PayoutRequestController objPayoutController = new PayoutRequestController();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult AfterPayment()
        {
            List<BankPayoutRequestViewModel> list = GetBankPayoutRequestList().Where(x => x.PaymentStatus == false && x.RequestedStatus == 1).ToList();

            foreach (var item in list)
            {
                if (item.PaymentStatus == true)
                {
                    item.StatusPayment = "Done";
                }
                else
                {
                    item.StatusPayment = "Pending";
                }
            }
            return View(list);
        }

        public ActionResult EditAfterPayment(long id)
        {
            LeadersPayoutRequest model = db.LeadersPayoutRequests.Where(x => x.UserLoginID == id).FirstOrDefault();
            return View(model);
        }

        [HttpPost]
        public ActionResult EditAfterPayment(LeadersPayoutRequest collection, int PaymentStatus)
        {
            LeadersPayoutRequest objPayoutRequest = db.LeadersPayoutRequests.Where(x => x.UserLoginID == collection.UserLoginID && (x.IsActive == true || x.IsActive == null)).FirstOrDefault();

            if (PaymentStatus != 0)
            {
                MLMWallet objWallet = db.MLMWallets.Where(x => x.UserLoginID == collection.UserLoginID).FirstOrDefault();
                objPayoutRequest.Remark = collection.Remark;
                if (PaymentStatus == 2)
                {
                    objPayoutRequest.PaymentStatus = true;
                    objPayoutRequest.RequestStatus = 2;
                    objPayoutRequest.IsActive = false;
                    objPayoutRequest.RequestStatus_Date = System.DateTime.Now;

                    //objWallet.Amount = (Decimal)(objWallet.Amount - objPayoutRequest.RequestedAmount);
                    //var requestedPoints = objPayoutRequest.RequestedAmount * 10;
                    //objWallet.Points = (Decimal)(objWallet.Points - requestedPoints);
                }
                else if (PaymentStatus == 4)
                {
                    objPayoutRequest.PaymentStatus = false;
                    objPayoutRequest.RequestStatus = 4;
                    objPayoutRequest.IsActive = false;
                    objPayoutRequest.RequestStatus_Date = System.DateTime.Now;


                    objWallet.Amount = (Decimal)(objWallet.Amount + objPayoutRequest.RequestedAmount);
                    var requestedPoints = objPayoutRequest.RequestedAmount * 10;
                    objWallet.Points = (Decimal)(objWallet.Points + requestedPoints);

                    LogMlmWalletPayout logMlmWalletPayout = new LogMlmWalletPayout();
                    logMlmWalletPayout.UserLoginID = collection.UserLoginID;
                    logMlmWalletPayout.Request_Amount = objPayoutRequest.RequestedAmount;
                    logMlmWalletPayout.LeadersPayoutRequestID = objPayoutRequest.ID;
                    logMlmWalletPayout.TransactionPoints = objWallet.Points;
                    logMlmWalletPayout.Create_Date = System.DateTime.Now;
                    db.LogMlmWalletPayouts.Add(logMlmWalletPayout);



                }

                else
                {
                    objPayoutRequest.PaymentStatus = false;
                    objPayoutRequest.RequestStatus = 1;
                    objPayoutRequest.IsActive = true;
                    objPayoutRequest.RequestStatus_Date = System.DateTime.Now;
                }

                objPayoutRequest.TransactionID = collection.TransactionID;

                objPayoutRequest.TransactionDate = collection.TransactionDate;

                // objPayoutRequest.RequestStatus_Date = System.DateTime.Now;

                var mobileNo = db.UserLogins.Where(x => x.ID == collection.UserLoginID).Select(y => y.Mobile).FirstOrDefault();


                db.SaveChanges();

                if (PaymentStatus == 2)
                {
                    SendEmail_AfterPayment(collection.UserLoginID, collection.TransactionID);

                    SendSMSToCustomer(collection.UserLoginID, collection.TransactionID, collection.RequestedAmount);

                }

                if (PaymentStatus == 4)
                {
                    objPayoutController.SendEmail_OnCancel(collection.UserLoginID);

                    // SendSMSToCustomer(collection.UserLoginID, collection.TransactionID, collection.RequestedAmount);
                    objPayoutController.SendSMSToCustomer(collection.UserLoginID, 3);
                }

                TempData["Message"] = 1;
            }
            else
            {
                TempData["Alert"] = 1;
                return RedirectToAction("EditAfterPayment", new { id = collection.UserLoginID });
            }

            return RedirectToAction("AfterPayment");
        }

        public ActionResult partialPayoutDetailsToBank()
        {
            var list = GetBankPayoutRequestList().Where(x => x.RequestedStatus == 1 && x.PaymentStatus != true).OrderBy(x => x.RequestStatus_Date);

            return PartialView("partialPayoutDetailsInBank", list);

        }

        public List<BankPayoutRequestViewModel> GetBankPayoutRequestList()
        {
            BankPayoutRequestViewModel hdfcIntegration = new BankPayoutRequestViewModel();

            List<KYCModel> bankDetailList = db.KYCModels.ToList();

            List<LeadersPayoutRequest> bankPayoutRequestList = db.LeadersPayoutRequests.ToList();

            List<BankPayoutRequestViewModel> bankPayoutList = new List<BankPayoutRequestViewModel>();


            foreach (var item in bankPayoutRequestList)
            {
                var bankID = db.KYCModels.Where(x => x.UserLoginID == item.UserLoginID).Select(y => y.BankID).FirstOrDefault();
                var BenificiaryName = db.KYCModels.Where(x => x.UserLoginID == item.UserLoginID).Select(y => y.BenificiaryName).FirstOrDefault();
                if (BenificiaryName == "")
                {
                    BenificiaryName = db.PersonalDetails.Where(x => x.UserLoginID == item.UserLoginID).Select(y => y.FirstName + " " + y.LastName).FirstOrDefault();
                }


                bankPayoutList.Add(new BankPayoutRequestViewModel
                {
                    UserLoginID = item.UserLoginID,
                    FullName = db.PersonalDetails.Where(x => x.UserLoginID == item.UserLoginID).Select(y => y.FirstName + " " + y.LastName).FirstOrDefault(),
                    Email = db.UserLogins.Where(x => x.ID == item.UserLoginID).Select(y => y.Email).FirstOrDefault(),
                    Mobile = db.UserLogins.Where(x => x.ID == item.UserLoginID).Select(y => y.Mobile).FirstOrDefault(),
                    BenificiaryAccountNumber = db.KYCModels.Where(x => x.UserLoginID == item.UserLoginID).Select(y => y.AccountNo).FirstOrDefault(),
                    BenificiaryAccountName = BenificiaryName,

                    IFSC = db.KYCModels.Where(x => x.UserLoginID == item.UserLoginID).Select(y => y.BankIFSC).FirstOrDefault(),
                    TransactionType = db.KYCModels.Where(x => x.UserLoginID == item.UserLoginID).Select(y => y.AccountType).FirstOrDefault(),
                    InstrumentAmount = item.TotalAmount ?? 0,
                    BenificiaryEmail = db.KYCModels.Where(x => x.UserLoginID == item.UserLoginID).Select(y => y.BenificiaryEmail).FirstOrDefault(),
                    CreateDate = item.Create_Date,
                    PaymentStatus = item.PaymentStatus ?? false,
                    BenificiaryBankName = db.Banks.Where(x => x.ID == bankID).Select(y => y.Name).FirstOrDefault(),
                    PaymentRemark = item.Remark,
                    TransactionID = item.TransactionID,
                    RequestedStatus = item.RequestStatus,
                    TransactionDate = System.DateTime.Now.ToString("dd/MM/yy"),
                    RequestStatus_Date = item.RequestStatus_Date,
                    RequestID = item.RequestID,


                });

            }

            // return bankPayoutList.OrderByDescending(Y=>Y.CreateDate).ToList();
            return bankPayoutList;

        }


        public ActionResult ListBetweenDate(string startDate, string endDate)
        {
            DateTime sDate = Convert.ToDateTime(startDate);
            DateTime eDate = Convert.ToDateTime(endDate);


            var SDate = new DateTime(sDate.Year, sDate.Month, sDate.Day, sDate.Hour, sDate.Minute, 1);
            var EDate = SDate.AddDays(1).AddMinutes(-1);

            if (startDate == endDate)
            {
                List<BankPayoutRequestViewModel> listOrder1 = GetBankPayoutRequestList().Where(x => x.CreateDate >= SDate && x.CreateDate <= EDate && x.PaymentStatus == true).ToList();

                return PartialView("partialPayoutDetailsInBank", listOrder1);
            }

            List<BankPayoutRequestViewModel> listOrder = GetBankPayoutRequestList().Where(x => x.CreateDate >= sDate && x.CreateDate <= eDate && x.PaymentStatus == true).ToList();
            return PartialView("partialPayoutDetailsInBank", listOrder);

        }

        public ActionResult BankPayoutReport()
        {
            List<BankPayoutRequestViewModel> PayoutList = new List<BankPayoutRequestViewModel>();
            PayoutList = GetBankPayoutRequestList().Where(y => y.PaymentStatus == true || y.RequestedStatus == 3 || y.RequestedStatus == 4).ToList();
            List<LeadersPayoutRequest> payoutRequestList = db.LeadersPayoutRequests.Where(x => x.PaymentStatus == true || x.RequestStatus == 3 || x.RequestStatus == 4).ToList();

            foreach (var item in PayoutList)
            {
                LeadersPayoutRequest objPayout = db.LeadersPayoutRequests.Where(x => x.UserLoginID == item.UserLoginID && (x.PaymentStatus == true || x.RequestStatus == 3 || x.RequestStatus == 4)).OrderByDescending(x => x.TransactionDate).FirstOrDefault();

                item.PaymentRemark = db.LeadersPayoutRequests.Where(x => x.UserLoginID == item.UserLoginID).Select(y => y.Remark).FirstOrDefault();
                if (objPayout.TransactionDate != null)
                {
                    item.TransactionDate = objPayout.TransactionDate.ToString();
                }
                if (item.RequestedStatus == 2)
                {
                    item.StatusPayment = "Done";
                }
                else if (item.RequestedStatus == 3)
                {
                    item.StatusPayment = "Cancelled";
                }
                else if (item.RequestedStatus == 4)
                {
                    item.StatusPayment = "Decline";
                }
            }

            return View(PayoutList);
        }

        public ActionResult ExportToExcel(string toDate, string fromDate)
        {
            // List<ExcelLeadersOrderViewModel> listLeadersOrder = new List<ExcelLeadersOrderViewModel>();
            List<BankPayoutRequestViewModel> listOrder = GetBankPayoutRequestList().Where(x => x.PaymentStatus == true || x.RequestedStatus == 3 || x.RequestedStatus == 4).ToList();

            List<ExcelBankReportViewModel> excelList = new List<ExcelBankReportViewModel>();
            foreach (var item in listOrder)
            {
                if (item.RequestedStatus == 1)
                {
                    item.StatusPayment = "Pending";
                }
                if (item.RequestedStatus == 2)
                {
                    item.StatusPayment = "Done";
                }
                if (item.RequestedStatus == 3)
                {
                    item.StatusPayment = "Cancelled";
                }
                if (item.RequestedStatus == 4)
                {
                    item.StatusPayment = "Declined";
                }
                excelList.Add(new ExcelBankReportViewModel
                {
                    BenificiaryAccountName = item.BenificiaryAccountName,
                    BenificiaryBankName = item.BenificiaryBankName,
                    IFSC = item.IFSC,
                    PaidAmount = item.InstrumentAmount,
                    PaymentRemark = item.PaymentRemark,
                    StatusPayment = item.StatusPayment,
                    PaymentStatus = item.PaymentStatus,
                    BenificiaryAccountNumber = item.BenificiaryAccountNumber,
                    BenificiaryEmail = item.BenificiaryEmail,
                    CreateDate = item.CreateDate,
                    TransactionDate = item.TransactionDate,


                    TransactionID = item.TransactionID,
                    Name = item.FullName,
                    Mobile = item.Mobile,
                    EmailID = item.Email,
                    RequestID = item.RequestID ?? 0



                });
            }

            if (toDate != "" && fromDate != "")
            {
                DateTime sDate = Convert.ToDateTime(fromDate);
                DateTime eDate = Convert.ToDateTime(toDate);

                if (fromDate == toDate)
                {
                    var SDate = new DateTime(sDate.Year, sDate.Month, sDate.Day, sDate.Hour, sDate.Minute, 1);
                    var EDate = SDate.AddDays(1).AddMinutes(-1);
                    excelList = excelList.Where(x => x.CreateDate >= SDate && x.CreateDate <= EDate).ToList();
                }


                excelList = excelList.Where(x => x.CreateDate >= sDate && x.CreateDate <= eDate).ToList();

            }

            var gv = new GridView();

            gv.DataSource = excelList;
            gv.DataBind();
            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename=BankPayoutReport.xls");
            Response.ContentType = "application/ms-excel";
            Response.Charset = "";
            StringWriter objStringWriter = new StringWriter();
            HtmlTextWriter objHtmlTextWriter = new HtmlTextWriter(objStringWriter);
            gv.RenderControl(objHtmlTextWriter);
            Response.Output.Write(objStringWriter.ToString());
            Response.Flush();
            Response.End();
            return RedirectToAction("Select");
        }

        public void SendEmail_AfterPayment(long userLoginID, string transactionID)
        {
            string TransactionID = "null";
            try
            {
                string RequestedAmount = GetBankPayoutRequestList().Where(x => x.UserLoginID == userLoginID).Select(k => k.InstrumentAmount).FirstOrDefault().ToString();
                string Mobile = db.UserLogins.Where(x => x.ID == userLoginID).Select(y => y.Mobile).FirstOrDefault();
                string RequestID = GetBankPayoutRequestList().Where(p => p.UserLoginID == userLoginID).Select(y => y.RequestID).FirstOrDefault().ToString();
                string RequestDate = GetBankPayoutRequestList().Where(p => p.UserLoginID == userLoginID).Select(x => x.CreateDate).FirstOrDefault().ToString();
                string AccountNo = db.KYCModels.Where(x => x.UserLoginID == userLoginID).Select(y => y.AccountNo).FirstOrDefault();
                string TransactionDate = GetBankPayoutRequestList().Where(x => x.UserLoginID == userLoginID).Select(y => y.TransactionDate).FirstOrDefault();
                TransactionID = transactionID;
                // var accountNotext = String.Right(AccountNo.GetLast(4));

                var last4 = Regex.Match(AccountNo, @"(.{4})\s*$");


                var last4Mobile = Regex.Match(Mobile, @"(.{2})\s*$");
                Mobile = "xxxxxxxx" + last4Mobile;
                AccountNo = "xxxxxxxxx" + last4;

                string Email = GetBankPayoutRequestList().Where(x => x.UserLoginID == userLoginID).Select(y => y.BenificiaryEmail).FirstOrDefault();
                string password = db.UserLogins.Where(x => x.ID == userLoginID).Select(y => y.Password).FirstOrDefault();

                if (string.IsNullOrEmpty(Email))
                {
                    Email = db.UserLogins.Where(x => x.ID == userLoginID).Select(y => y.Email).FirstOrDefault();

                };


                string CustomerName = db.PersonalDetails.Where(u => u.UserLoginID == userLoginID).Select(y => y.FirstName + "" + y.LastName).FirstOrDefault();

                if (string.IsNullOrEmpty(CustomerName))
                {
                    CustomerName = GetBankPayoutRequestList().Where(x => x.UserLoginID == userLoginID).Select(y => y.BenificiaryAccountName).FirstOrDefault();

                }


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
                    string URL = "http://leaders.ezeelo.com/LeadersLogin/?Email=" + Email + "&Password=" + password;

                    // string URL= "" + (new URLsFromConfig()).GetURL("LEADERS") + "LeadersLogin/Login/?UserName=" + Email + "&Password=" + password;
                    Dictionary<string, string> dictEmailValues = new Dictionary<string, string>();
                    dictEmailValues.Add("<!--CUSTOMER_NAME-->", CustomerName);
                    dictEmailValues.Add("<!--REQUEST_AMOUNT-->", RequestedAmount);
                    dictEmailValues.Add("<!--REQUEST_ID-->", RequestID);
                    dictEmailValues.Add("<!--REQUEST_DATE-->", RequestDate);
                    dictEmailValues.Add("<!--TRANSACTION_ID-->", TransactionID);
                    dictEmailValues.Add("<!--MOBILE-->", Mobile);
                    dictEmailValues.Add("<!--ACCOUNT_NO-->", AccountNo);
                    dictEmailValues.Add("<!--CREDITED_DATE-->", TransactionDate);
                    dictEmailValues.Add("<!--URL-->", URL);

                    BusinessLogicLayer.GateWay gateWay = new BusinessLogicLayer.Email(System.Web.HttpContext.Current.Server);

                    string EmailID = Email;
                    //gateWay.SendEmail(BusinessLogicLayer.GateWay.EmailGateWays.GANDHIBAGH, BusinessLogicLayer.GateWay.SenderMail.INFO, BusinessLogicLayer.GateWay.EMailTypes.ON_SUCCESS_CUST_PAYMENT, new string[] { Email }, dictEmailValues, true);
                    gateWay.SendEmail(BusinessLogicLayer.GateWay.EmailGateWays.GANDHIBAGH, BusinessLogicLayer.GateWay.SenderMail.INFO, BusinessLogicLayer.GateWay.EMailTypes.ON_SUCCESS_CUST_PAYMENT, new string[] { "gaurav.shrote@ezeelo.com" }, dictEmailValues, true);
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



        public void SendSMSToCustomer(long userLoginID, string TransactionID, decimal? RequestedAmount)
        {
            try
            {
                // var lOrder = db.CustomerOrders.Where(x => x.ID == orderId).ToList();

                string mobileNo = db.UserLogins.Where(x => x.ID == userLoginID).Select(y => y.Mobile).FirstOrDefault();
                var lPersonalDetails = db.PersonalDetails.Where(x => x.UserLoginID == userLoginID).ToList();

                Dictionary<string, string> dictSMSValues = new Dictionary<string, string>();

                dictSMSValues.Add("#--NAME--#", Convert.ToString(lPersonalDetails.FirstOrDefault().FirstName));
                dictSMSValues.Add("#--TRANS_ID--#", TransactionID);
                dictSMSValues.Add("#--REQUEST_AMOUNT--#", RequestedAmount.ToString());

                BusinessLogicLayer.GateWay gateWay = new BusinessLogicLayer.SMS(System.Web.HttpContext.Current.Server);
                //gateWay.SendSMS(BusinessLogicLayer.GateWay.SMSGateWays.SUMIT, BusinessLogicLayer.GateWay.SMSOptions.SINGLE, BusinessLogicLayer.GateWay.SMSTypes.AFTER_PAYMENT_DONE_PAYOUT_REQUEST, new string[] { mobileNo }, dictSMSValues);
                gateWay.SendSMS(BusinessLogicLayer.GateWay.SMSGateWays.SUMIT, BusinessLogicLayer.GateWay.SMSOptions.SINGLE, BusinessLogicLayer.GateWay.SMSTypes.AFTER_PAYMENT_DONE_PAYOUT_REQUEST, new string[] { "7020176511" }, dictSMSValues);

            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[OrderCancelSmsAndEmail][M:SendSMSToCustomer]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Gandhibagh, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[OrderCancelSmsAndEmail][M:SendSMSToCustomer]",
                    BusinessLogicLayer.ErrorLog.Module.Gandhibagh, System.Web.HttpContext.Current.Server);
            }
        }
        /// <summary>
        /// Send SMS to all Merchants for order Cancelled
        /// </summary>
        /// <param name="userLoginID">Customer Login ID</param>
        /// <param name="orderId">Order ID</param>
    }
}