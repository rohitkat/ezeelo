using BusinessLogicLayer;
using ModelLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace API.Controllers
{
    public class SubmitTransViaQRCodeController : ApiController
    {
        private EzeeloDBContext db = new EzeeloDBContext();
        public object Get(long MerchantId, long UserLoginId, decimal BillAmount)
        {
            object obj = new object();
            try
            {
                UserLogin userLogin = db.UserLogins.FirstOrDefault(p => p.ID == UserLoginId);
                if (userLogin != null)
                {
                    Merchant merchant = db.Merchants.FirstOrDefault(p => p.Id == MerchantId);
                    if (merchant != null)
                    {
                        string TransId = GenerateCode();
                        MarketPartner merchantSMS = new MarketPartner();
                        MerchantTransactionRequest transactionRequest = new MerchantTransactionRequest();
                        transactionRequest.MerchantId = MerchantId;
                        transactionRequest.UserLoginId = UserLoginId;
                        transactionRequest.BillAmount = BillAmount;
                        transactionRequest.TransactionId = TransId;
                        transactionRequest.RefTransactionId = "";
                        transactionRequest.Status = 0;
                        transactionRequest.Remark = "";
                        transactionRequest.CreateDate = DateTime.Now;
                        db.merchantTransactionRequests.Add(transactionRequest);
                        db.SaveChanges();
                        //Send SMS to User
                        //send SMS to Merchant
                        merchantSMS.SendSMS_TransRequest(UserLoginId, TransId, BillAmount.ToString(), MerchantId);
                        obj = new { Success = 1, Message = "Transaction Request Send", data = TransId };
                    }
                    else
                    {
                        obj = new { Success = 0, Message = "Merchant Not Found", data = "" };
                    }
                }
                else
                {
                    obj = new { Success = 0, Message = "User Not Found", data = "" };
                }
            }
            catch (Exception ex)
            {
                obj = new { Success = 0, Message = ex.Message, data = string.Empty };
            }
            return obj;
        }

        public string GenerateCode()
        {
            BusinessLogicLayer.MarketPartner obj = new BusinessLogicLayer.MarketPartner();
            return obj.GetNextTransactionCodeThroughQRCode();
        }

    }
}
