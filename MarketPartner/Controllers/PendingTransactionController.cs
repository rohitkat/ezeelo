using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MarketPartner.Filter;
using ModelLayer.Models;
using MarketPartner.Controllers;


namespace MarketPartner.Controllers
{
    [SessionExpire]
    [Authorize]
    public class PendingTransactionController : Controller
    {
        EzeeloDBContext db = new EzeeloDBContext();

        public ActionResult Index()
        {
            long MerchantID = Convert.ToInt64(Session["MerchantID"]);
            List<MerchantTransactionRequest> list = db.merchantTransactionRequests.Where(p => p.MerchantId == MerchantID).OrderBy(p => p.CreateDate).OrderBy(p => p.Status).ToList();
            foreach (var item in list)
            {
                PersonalDetail p = db.PersonalDetails.FirstOrDefault(q => q.UserLoginID == item.UserLoginId);
                item.UserName = p.FirstName + " " + ((String.IsNullOrEmpty(p.LastName)) ? "" : p.LastName);
            }
            return View(list);
        }

        [HttpPost]
        public ActionResult AcceptTransRequest(long ID, string Remark)
        {
            long MerchantID = Convert.ToInt64(Session["MerchantID"]);
            string result = "";
            MerchantTransactionRequest transactionRequest = db.merchantTransactionRequests.FirstOrDefault(p => p.Id == ID);
            if (transactionRequest == null)
            {
                result = "Transaction Request not Found";
            }
            else
            {
                try
                {
                    string Service = db.Merchants.FirstOrDefault(p => p.Id == MerchantID).ServiceMasterDetail.Name;
                    PersonalDetail pp = db.PersonalDetails.FirstOrDefault(p => p.UserLoginID == transactionRequest.UserLoginId);
                    string name = pp.FirstName + " " + (String.IsNullOrEmpty(pp.LastName) ? "" : pp.LastName);
                    MerchantTransactionController transactionController = new MerchantTransactionController();
                    string NewTransactionId = transactionController.SaveTransaction(MerchantID, transactionRequest.BillAmount, transactionRequest.User.Mobile, Service, Remark, name, true);
                    transactionRequest.Status = 1;
                    transactionRequest.ModifyDate = DateTime.Now;
                    transactionRequest.RefTransactionId = NewTransactionId;
                    transactionRequest.Remark = Remark;
                    db.SaveChanges();
                    result = "Transaction Request Accepted Successfully!";
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
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult RejectTransRequest(long ID, string Remark)
        {
            long MerchantID = Convert.ToInt64(Session["MerchantID"]);
            string result = "";
            MerchantTransactionRequest transactionRequest = db.merchantTransactionRequests.FirstOrDefault(p => p.Id == ID);
            if (transactionRequest == null)
            {
                result = "Transaction Request not Found";
            }
            else
            {
                MerchantTransactionController transactionController = new MerchantTransactionController();
                transactionRequest.Status = 2;
                transactionRequest.ModifyDate = DateTime.Now;
                transactionRequest.Remark = Remark;
                db.SaveChanges();
                SendSMS_TransRequestReject(transactionRequest.User.Mobile, MerchantID, transactionRequest.UserLoginId, transactionRequest.TransactionId, transactionRequest.BillAmount.ToString());
                result = "Transaction Request Rejected Successfully!";
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public void SendSMS_TransRequestReject(string MobileNO, long MerchantId, long UserId, string Code, string BillAmt)
        {
            try
            {
                PersonalDetail pd = db.PersonalDetails.FirstOrDefault(p => p.UserLoginID == UserId);
                Merchant mer = db.Merchants.FirstOrDefault(p => p.Id == MerchantId);
                Dictionary<string, string> dictSMSValues = new Dictionary<string, string>();
                dictSMSValues.Add("#--CODE--#", Code);
                dictSMSValues.Add("#--NAME--#", pd.FirstName);
                dictSMSValues.Add("#--SHOP--#", mer.FranchiseName);
                dictSMSValues.Add("#--RS--#", BillAmt);

                BusinessLogicLayer.GateWay gateWay = new BusinessLogicLayer.SMS(System.Web.HttpContext.Current.Server);
                gateWay.SendSMS(BusinessLogicLayer.GateWay.SMSGateWays.SUMIT, BusinessLogicLayer.GateWay.SMSOptions.SINGLE, BusinessLogicLayer.GateWay.SMSTypes.MERCHANT_TRANSACTION_REQUEST_REJECT, new string[] { MobileNO }, dictSMSValues);
            }
            catch(Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                   + Environment.NewLine + ex.Message + Environment.NewLine
                   + "[PendingTransaction][SendSMS_TransRequestReject]",
                   BusinessLogicLayer.ErrorLog.Module.MarketPartner, System.Web.HttpContext.Current.Server);
            }
        }
    }
}