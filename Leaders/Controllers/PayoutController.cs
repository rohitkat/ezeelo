using BusinessLogicLayer;
using Leaders.Areas.Admin.Controllers;
using ModelLayer.Models;
using ModelLayer.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace Leaders.Controllers
{
    public class PayoutController : Controller
    {

        Decimal Amount = 0;
        string Password = "";

        private EzeeloDBContext db = new EzeeloDBContext();

        PayoutRequestController payoutRequestController = new PayoutRequestController();

        public ActionResult PayOutRequest()
        {
            long userID = Convert.ToInt64(Session["ID"]);
            PersonalDetail objPersonel = db.PersonalDetails.Where(x => x.UserLoginID == userID).FirstOrDefault();
            KYCModel objKYC = db.KYCModels.Where(y => y.UserLoginID == userID).FirstOrDefault();
            LeadersPayoutRequestViewModel objPayout = new LeadersPayoutRequestViewModel();

            MLMWallet objWalletTransaction = db.MLMWallets.Where(x => x.UserLoginID == userID).FirstOrDefault();
            LeadersPayoutMaster objPayoutMaster = db.LeadersPayoutMasters.FirstOrDefault();


            if (objWalletTransaction != null)
            {

                objPayout.EzeeloWalletCash = objWalletTransaction.Amount;
                Decimal reservedAmount = Convert.ToDecimal(objWalletTransaction.Amount * objPayoutMaster.Min_Resereved * Convert.ToDecimal(0.01));
                Decimal RedeamableCash = objWalletTransaction.Amount - reservedAmount;

                // objPayout.RedeamableCash = RedeamableCash;
                objPayout.RedeamableCash = Math.Round(RedeamableCash, 2);


            }
            else
            {
                objPayout.EzeeloWalletCash = 0;
                objPayout.RedeamableCash = 0;
            }

            if (objKYC != null)
            {

                objPayout.AccountNo = objKYC.AccountNo;
                objPayout.AccountType = objKYC.AccountType;
                objPayout.AdhaarNo = objKYC.AdhaarNo;
                objPayout.BankID = objKYC.BankID;
                objPayout.BankIFSC = objKYC.BankIFSC;
                objPayout.BranchName = objKYC.BranchName;
                objPayout.UserLoginID = objKYC.UserLoginID;
                objPayout.AccountHolderName = objKYC.BenificiaryName;  //amit on 18-1-19
                objPayout.BeneficiaryEmail = objKYC.BenificiaryEmail;  // amit on 13-2-19

                objPayout.BankName = db.Banks.Where(y => y.ID == objKYC.BankID).Select(y => y.Name).FirstOrDefault();


                return View(objPayout);
            }
            else
            {
                return View(objPayout);
            }
                       
        }
        [HttpPost]
        public ActionResult PayOutRequest(string gstAmount, string requestedAmount, string tdsAmount, string totalAmount)
        {
            string TotalAmount;
            string WalleteAmount;
            int? RequestedID = 100;
            string CurrentWalletAmount;


            // randome no generation //

            Random rnd = new Random();

            int rndnumber = rnd.Next();
            int randomvalue = 0;
            using (RNGCryptoServiceProvider rg = new RNGCryptoServiceProvider())
            {
                byte[] rno = new byte[5];
                rg.GetBytes(rno);
                randomvalue = BitConverter.ToInt32(rno, 0);
            }

            //--end--//

            try
            {
                long userID = Convert.ToInt64(Session["ID"]);

                LeadersPayoutRequest objPayoutRequest = new LeadersPayoutRequest();

                LeadersPayoutMaster objMaster = db.LeadersPayoutMasters.FirstOrDefault();
                MLMWallet objWalletTransaction = db.MLMWallets.Where(x => x.UserLoginID == userID).FirstOrDefault();

                objPayoutRequest.GSTAmount = Convert.ToDecimal(gstAmount);
                objPayoutRequest.GST = objMaster.GST;
                objPayoutRequest.TDSAmount = Convert.ToDecimal(tdsAmount);
                objPayoutRequest.TDS = objMaster.TDS;
                //objPayoutRequest.ProcessingFees = objMaster.Processing_Fees;

                Decimal ProcessingFee = Convert.ToDecimal(requestedAmount) * objMaster.Processing_Fees * Convert.ToDecimal(0.01) ?? 0;
                objPayoutRequest.ProcessingFees = Math.Round(ProcessingFee, 2);

                objPayoutRequest.MinReserved = objMaster.Min_Resereved;
                //objPayoutRequest.MinReservedAmount = objWalletTransaction.Amount * Convert.ToDecimal(0.1);
                objPayoutRequest.UserLoginID = userID;
                objPayoutRequest.TotalAmount = Convert.ToDecimal(totalAmount);
                objPayoutRequest.Penalty = objMaster.Penalty;


                objPayoutRequest.RequestedAmount = Convert.ToDecimal(requestedAmount);
                objPayoutRequest.Remark = "Requested";

                if (objWalletTransaction != null)
                {
                    Decimal minReservedAmnt = Convert.ToDecimal(objWalletTransaction.Amount * objMaster.Min_Resereved * Convert.ToDecimal(0.01));
                    Decimal penaltyAmount = Convert.ToDecimal(objWalletTransaction.Amount * objMaster.Penalty * Convert.ToDecimal(0.01));

                    objPayoutRequest.MinReservedAmount = minReservedAmnt;
                    objPayoutRequest.PenaltyAmount = penaltyAmount;

                    Decimal RedeamableCash = objWalletTransaction.Amount - minReservedAmnt;

                    // objPayoutRequest.RedeamableAmount = RedeamableCash;
                    // objPayout.RedeamableCash = RedeamableCash;
                    objPayoutRequest.RedeamableAmount = Math.Round(RedeamableCash, 2);

                }
                else
                {
                    objPayoutRequest.RedeamableAmount = 0;
                    objPayoutRequest.MinReservedAmount = 0;
                    objPayoutRequest.PenaltyAmount = 0;
                }
                objPayoutRequest.RequestStatus = 0;

                MLMWallet objWallet = db.MLMWallets.Where(x => x.UserLoginID == userID).FirstOrDefault();

                objWallet.Amount = (Decimal)(objWallet.Amount - objPayoutRequest.RequestedAmount);
                var requestedPoints = objPayoutRequest.RequestedAmount * 10;
                objWallet.Points = (Decimal)(objWallet.Points - requestedPoints);

                Session["EzzMoney"] = objWallet.Amount.ToString();

                objPayoutRequest.IsActive = true;

                TotalAmount = objPayoutRequest.TotalAmount.ToString();  // added on 11-2-19

                WalleteAmount = objWalletTransaction.Amount.ToString(); // added on 11-2-19

                TempData["CurrentWalletAmount"] = objWalletTransaction.Amount.ToString();
                //CurrentWalletAmount= objWalletTransaction

                //objPayoutRequest.RequestID = randomvalue;  // added on 13-2-19
                objPayoutRequest.Create_Date = System.DateTime.Now;
                objPayoutRequest.PaymentStatus = false;
                objPayoutRequest.TransactionID = "NULL";
                objPayoutRequest.TransactionDate = System.DateTime.Now;

                RequestedID = db.LeadersPayoutRequests.OrderByDescending(y => y.RequestID).Select(y => y.RequestID).FirstOrDefault();

                if (RequestedID == 0 || RequestedID == null)
                {
                    RequestedID = 100;

                }
                objPayoutRequest.RequestID = RequestedID + 1;


                db.LeadersPayoutRequests.Add(objPayoutRequest);

                db.SaveChanges();
                // added for maintain log of Ezeemoney wallet
                LeadersPayoutRequest oPayoutRequest = db.LeadersPayoutRequests.Where(x => x.UserLoginID == userID && (x.IsActive == true || x.IsActive == null)).FirstOrDefault();
                LogMlmWalletPayout logWalletPayout = new LogMlmWalletPayout();
                logWalletPayout.UserLoginID = userID;
                logWalletPayout.Current_WalletAmount = Convert.ToDecimal(TempData["CurrentWalletAmount"]);
                logWalletPayout.AddOrSub = false;   // for substract amount wallet
                logWalletPayout.Request_Amount = Convert.ToDecimal(requestedAmount);
                logWalletPayout.TransactionPoints = objWallet.Points;
                logWalletPayout.Create_Date = System.DateTime.Now;
                logWalletPayout.Create_By = "User";
                logWalletPayout.LeadersPayoutRequestID = oPayoutRequest.ID;

                db.LogMlmWalletPayouts.Add(logWalletPayout);
                db.SaveChanges();


                //---send email to account section---//

                try
                {
                    string Email = db.UserLogins.Where(x => x.ID == userID).Select(y => y.Email).FirstOrDefault();
                    string mobile = db.UserLogins.Where(x => x.ID == userID).Select(x => x.Mobile).FirstOrDefault();

                    // string name = db.PersonalDetails.Where(u => u.UserLoginID == userID).Select(y => y.FirstName + "" + y.LastName).FirstOrDefault();
                    // string Email = "amit.pantawne@ezeelo.com";
                    string CustomerName = db.PersonalDetails.Where(x => x.UserLoginID == userID).Select(y => y.FirstName + "" + y.LastName).FirstOrDefault().ToString();
                    var RefferalID = db.MLMUsers.Where(x => x.UserID == userID).Select(y => y.Ref_Id).FirstOrDefault();
                    string RedeamableCash = objPayoutRequest.RedeamableAmount.ToString() ?? "0";
                    string RequestedAmount = objPayoutRequest.RequestedAmount.ToString() ?? "0";
                    string gst = objPayoutRequest.GST.ToString() ?? "0";
                    string gst_Amount = objPayoutRequest.GSTAmount.ToString() ?? "0";
                    string tds_Amount = objPayoutRequest.TDSAmount.ToString() ?? "0";
                    string tds = objPayoutRequest.TDS.ToString() ?? "0";
                    string minReserved = objPayoutRequest.MinReserved.ToString() ?? "0";
                    string minReservedAmount = objPayoutRequest.MinReservedAmount.ToString() ?? "0";
                    string Amount = objPayoutRequest.TotalAmount.ToString() ?? "0";
                    string Penalty = objPayoutRequest.Penalty.ToString() ?? "0";
                    string PenaltyAmount = objPayoutRequest.PenaltyAmount.ToString() ?? "0";
                    string RequestedDate = objPayoutRequest.Create_Date.ToString();
                    string processingFees = objPayoutRequest.ProcessingFees.ToString();
                    string RequestStatus = "";
                    string RequestID = objPayoutRequest.RequestID.ToString();
                    if (objPayoutRequest.RequestStatus == 0)
                    {
                        RequestStatus = "Pending";
                    }



                    var UserLogin = db.PersonalDetails.Where(u => u.UserLoginID == userID)
                        .Join(db.MLMUsers, u => u.UserLoginID, m => m.UserID, (u, m) => new
                        {
                            RefferalCode = m.Ref_Id,
                            Name = u.FirstName
                        }).ToList();


                    //var last4 = Regex.Match(AccountNo, @"(.{4})\s*$");


                    //var last4Mobile = Regex.Match(mobile, @"(.{2x})\s*$");
                    //mobile = "xxxxxxxx" + last4Mobile;
                    //AccountNo = "xxxxxxxxx" + last4;

                    if (UserLogin != null && UserLogin.Count() > 0)
                    {
                        string Name = UserLogin.First().Name;
                        string RefferalCode = UserLogin.First().RefferalCode;
                        //string URL = "http://www.ezeelo.com/nagpur/2/login?Phone=" + mobile + "&ReferalCode=" + RefferalCode + "&Name=" + name + "&Email=" + Email;
                        //string URL = "" + (new URLsFromConfig()).GetURL("LEADERS") + "LeadersLogin/Login/?UserName=" + Email + "&Password=" + Password;

                        Dictionary<string, string> dictEmailValues = new Dictionary<string, string>();
                        dictEmailValues.Add("<!--Customer_Name-->", CustomerName);
                        dictEmailValues.Add("<!--Referral_ID-->", RefferalID);
                        dictEmailValues.Add("<!--Redeemable_Cash-->", RedeamableCash);
                        dictEmailValues.Add("<!--Requested_Amount-->", RequestedAmount);
                        dictEmailValues.Add("<!--GST-->", gst);
                        dictEmailValues.Add("<!--GST_Amount-->", gst_Amount);
                        dictEmailValues.Add("<!--TDS_Amount-->", tds_Amount);
                        dictEmailValues.Add("<!--TDS-->", tds);
                        dictEmailValues.Add("<!--Min_Reserved-->", minReserved);
                        dictEmailValues.Add("<!--Min_Reserved_Amount-->", minReservedAmount);
                        dictEmailValues.Add("<!--Total_Amount-->", Amount);
                        dictEmailValues.Add("<!--Penalty-->", Penalty);
                        dictEmailValues.Add("<!--Penalty_Amount-->", PenaltyAmount);
                        dictEmailValues.Add("<!--Request_Date-->", RequestedDate);
                        dictEmailValues.Add("<!--Request_Status-->", RequestStatus);
                        dictEmailValues.Add("<!--Processing_Fees-->", processingFees);
                        dictEmailValues.Add("<!--Request_ID-->", RequestID);
                        BusinessLogicLayer.GateWay gateWay = new BusinessLogicLayer.Email(System.Web.HttpContext.Current.Server);
                        // string EmailID = "tech@ezeelo.com";
                        gateWay.SendEmail(BusinessLogicLayer.GateWay.EmailGateWays.GANDHIBAGH, BusinessLogicLayer.GateWay.SenderMail.INFO, BusinessLogicLayer.GateWay.EMailTypes.ON_REQUEST_SEND_CRM, new string[] { "gaurav.shrote@gmail.com" }, dictEmailValues, true);
                        (new SendFCMNotification()).SendNotification("withdrawn_send", userID);
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


                SendEmail_BeforePayment(userID, TotalAmount, WalleteAmount);   //added on 11-2-19
                payoutRequestController.SendSMSToCustomer(userID, 0); // added on 25-3-19



            }


            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage);
                    }
                }
                throw;
            }

            return RedirectToAction("PayOutRequest");
        }

        public ActionResult CheckPassword(string redeamableAmount, string password, decimal requestAmount)
        {

            long userID = Convert.ToInt64(Session["ID"]);
            LeadersPayoutRequest objPayoutRequest = db.LeadersPayoutRequests.Where(x => x.UserLoginID == userID && x.IsActive == true).OrderByDescending(y => y.Create_Date).FirstOrDefault();


            if (requestAmount != null && password != "")
            {
                var redeamableAmnt = Convert.ToDecimal(redeamableAmount);
                var requestAmnt = Convert.ToDecimal(requestAmount);
                if (requestAmnt > 0)
                {
                    if (redeamableAmnt != 0)
                    {
                        if (requestAmnt <= redeamableAmnt)
                        {
                            if (objPayoutRequest == null || (objPayoutRequest.RequestStatus != 0 && objPayoutRequest.RequestStatus != 1))
                            // if (objPayoutRequest == null || objPayoutRequest.IsActive == false || objPayoutRequest.IsActive == null)
                            {

                                Password = password;
                                var existingPassword = db.UserLogins.Where(x => x.ID == userID).Select(y => y.Password).FirstOrDefault();
                                if (existingPassword == password)
                                {
                                    return Json(new { result = "matched" }, JsonRequestBehavior.AllowGet);
                                }
                                else
                                {
                                    return Json(new { result = "unmached" }, JsonRequestBehavior.AllowGet);
                                }
                            }
                            //    else
                            //    {
                            //        return Json(new { result = "NotEligible" }, JsonRequestBehavior.AllowGet);
                            //    }
                            //}
                            else
                            {

                                return Json(new { result = "SecondRequest", requestID = objPayoutRequest.RequestID }, JsonRequestBehavior.AllowGet);
                            }
                        }
                        else
                        {
                            return Json(new { result = "NotGreater" }, JsonRequestBehavior.AllowGet);
                        }
                    }
                    else
                    {

                        return Json(new { result = "NotEligible" }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    return Json(new { result = "ValidAmnt" }, JsonRequestBehavior.AllowGet);

                }
            }
            else
            {
                return Json(new { result = "FillDetails" }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult Summary(string amount)
        {
            LeadersPayoutMaster objPayout = db.LeadersPayoutMasters.FirstOrDefault();
            Amount = Convert.ToDecimal(amount);

            var gstAmnt = Amount * Convert.ToDecimal(0.01) * objPayout.GST;
            var TDSAmount = Amount * Convert.ToDecimal(0.01) * objPayout.TDS;
            var ProcessingFees = Amount * objPayout.Processing_Fees * Convert.ToDecimal(0.01);
            var Penalty = objPayout.Penalty;

            var TotalAmount = (Amount - gstAmnt - TDSAmount - ProcessingFees - Penalty);

            LeadersPayoutRequestViewModel objPayoutRequest = new LeadersPayoutRequestViewModel();
            objPayoutRequest.RequestedAmount = Amount;
            objPayoutRequest.GSTAmount = gstAmnt;
            objPayoutRequest.TDSAmount = TDSAmount;
            objPayoutRequest.ProcessingFees = ProcessingFees;
            objPayoutRequest.PenaltyAmount = Penalty;
            objPayoutRequest.TotalAmount = TotalAmount;
            return Json(objPayoutRequest, JsonRequestBehavior.AllowGet);
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
                Password = db.UserLogins.Where(x => x.ID == userLoginID).Select(y => y.Password).FirstOrDefault();

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

        [HttpPost]
        public PartialViewResult GetDetailslist()
        {
            long userID = Convert.ToInt64(Session["ID"]);
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
                    RequestID = lpay.RequestID ?? 0,
                    TransactionID = lpay.TransactionID

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
                else
                {
                }
                if (item.TransactionID == "NULL" || item.TransactionID == "xxxxxxxxxxx" || item.TransactionID == null)
                {
                    item.TransactionID = "-";
                }


            }
            return PartialView("_getpayoutdetails", payoutList.Where(x => x.UserLoginID == userID));
        }

    }
}