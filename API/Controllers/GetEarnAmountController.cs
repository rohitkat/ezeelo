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
    public class GetEarnAmountController : ApiController
    {
        EzeeloDBContext db = new EzeeloDBContext();
        // GET api/getearnamount  
        //public IEnumerable<string> Get()
        //{
        //    return new string[] { "value1", "value2" };
        //}

        // GET api/getearnamount/5
        public object Get(int userLoginID)
        {
            object obj = new object();
            try
            {
                if (userLoginID <= 0)
                {
                    return obj = new { Success = 0, Message = "User not login", data = string.Empty };
                    //obj = new { HTTPStatusCode = "400", UserMessage = "User not login", ValidationError = "Please login" };
                    //return Request.CreateResponse(HttpStatusCode.BadRequest, obj);
                }
                ReferAndEarn lReferAndEarn = new ReferAndEarn();
                decimal ReferandEarnAmt = lReferAndEarn.GetTotalEarnAmount(userLoginID);
                MLMWallet objWalletTransaction = db.MLMWallets.Where(x => x.UserLoginID == userLoginID).FirstOrDefault();
                LeadersPayoutMaster objPayoutMaster = db.LeadersPayoutMasters.FirstOrDefault();
                decimal EzeeloWalletCash = 0;
                decimal RedeamableCash = 0;
                decimal CasbackEzeeyMoney = 0;
                if (objWalletTransaction != null)
                {
                    EzeeloWalletCash = objWalletTransaction.Amount;
                    Decimal reservedAmount = Convert.ToDecimal(objWalletTransaction.Amount * objPayoutMaster.Min_Resereved * Convert.ToDecimal(0.01));
                    RedeamableCash = objWalletTransaction.Amount + ReferandEarnAmt - reservedAmount;
                    // objPayout.RedeamableCash = RedeamableCash;
                    RedeamableCash = Math.Round(RedeamableCash, 2);
                    CashbackWallet wallet = db.cashbackWallets.FirstOrDefault(p => p.UserLoginID == userLoginID);
                    if (wallet != null)
                    {
                        RedeamableCash = RedeamableCash + wallet.Amount;
                        CasbackEzeeyMoney =  wallet.Amount;
                    }
                    obj = new { Success = 1, Message = "Successfull.", data = new { ReferandEarnAmt = ReferandEarnAmt, EzeeMoney = EzeeloWalletCash, RedeamableCash = RedeamableCash, CasbackEzeeyMoney = CasbackEzeeyMoney } };
                }
                else
                {
                    obj = new { Success = 1, Message = "This is not mlm user.", data = new { ReferandEarnAmt = ReferandEarnAmt, EzeeMoney = EzeeloWalletCash, RedeamableCash = RedeamableCash, CasbackEzeeyMoney = CasbackEzeeyMoney } };
                }

                //obj = new { Success = 1, Message = "Successfull.", data = amt };
                //  return Request.CreateResponse(HttpStatusCode.OK, amt);
            }
            catch (Exception ex)
            {
                obj = new { Success = 0, Message = ex.Message, data = string.Empty };
            }
            return obj;
        }

        // POST api/getearnamount
        //public void Post([FromBody]string value)
        //{
        //}

        //// PUT api/getearnamount/5
        //public void Put(int id, [FromBody]string value)
        //{
        //}

        //// DELETE api/getearnamount/5
        //public void Delete(int id)
        //{
        //}


    }
}
