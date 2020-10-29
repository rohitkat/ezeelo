using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using BusinessLogicLayer;
using MarketPartner.Filter;
using ModelLayer.Models;

namespace MarketPartner.Controllers
{

    [Authorize]
    public class RechargeAccountController : Controller
    {
        EzeeloDBContext db = new EzeeloDBContext();
        MerchantNotification merchantNotification = new MerchantNotification();

        public ActionResult Index(int? flag)
        {
            long MerchantID = Convert.ToInt64(Session["MerchantID"]);
            MerchantTopupRecharge recharge_ = db.merchantTopupRecharges.FirstOrDefault(p => p.MerchantID == MerchantID);
            if (recharge_ == null)
            {
                flag = 1;
            }
            List<MerchantTopupRechargeLog> log = db.merchantRechargeLog.Where(p => p.MerchantID == MerchantID && p.Status == 1).ToList();
            if(log.Count() == 0)
            {
                flag = 1;
            }
            MerchantTopupRechargeLog recharge = new MerchantTopupRechargeLog();
            MerchantCommonValues values = db.MerchantCommonValues.First();
            recharge.CompanyDetail = values.CompanyDetail;
            recharge.AccountNo = values.AccountNo;
            recharge.IFSCCode = values.IFSCCode;
            if (flag == 1)
            {
                recharge.TopupAmount = values.MerchantRegistrationFee;
                recharge.recharge = Convert.ToInt16(values.MerchantRegistrationFee).ToString();
            }
            string str = values.MerchantTopupMoney;

            string[] splitStr = str.Split(',');
            recharge.myList = new List<SelectListItem>();
            for (int i = 0; i < splitStr.Length; i++)
            {
                if (flag == 1)
                {
                    if (splitStr[i] == recharge.recharge)
                    {
                        MerchantDetails details = db.merchantDetails.FirstOrDefault(p => p.MerchantId == MerchantID);
                        if (details == null)
                        {
                            recharge.myList.Add(new SelectListItem { Value = splitStr[i], Text = splitStr[i] });
                            recharge.myList.Add(new SelectListItem { Value = "5000", Text = "5000" });
                            recharge.recharge = "";
                        }
                        else
                        {
                            recharge.myList.Add(new SelectListItem { Value = Convert.ToInt16(details.RegistrationFee).ToString(), Text = Convert.ToInt16(details.RegistrationFee).ToString() });
                            recharge.recharge = Convert.ToInt16(details.RegistrationFee).ToString();
                        }
                    }
                }
                else
                {
                    recharge.myList.Add(new SelectListItem { Value = splitStr[i], Text = splitStr[i] });
                }
            }

            recharge.list = db.merchantRechargeLog.Where(p => p.MerchantID == MerchantID).OrderByDescending(p => p.CreateDate).AsEnumerable()
                  .Select(p => new RechargeList
                  {
                      Date = p.CreateDate.ToString("dd-MM-yyyy"),
                      Amount = p.TopupAmount,
                      Status = p.Status.ToString()
                  }).Take(10).ToList();
            return View(recharge);
        }
        [HttpPost]
        public ActionResult Index(MerchantTopupRechargeLog modal)
        {
            long MerchantID = Convert.ToInt64(Session["MerchantID"]);
            MerchantTopupRecharge topupRecharge_ = db.merchantTopupRecharges.FirstOrDefault(p => p.MerchantID == MerchantID);
            if (topupRecharge_ == null)
            {
                MerchantTopupRecharge topup_Recharge = new MerchantTopupRecharge();
                topup_Recharge.MerchantID = MerchantID;
                topup_Recharge.TopupAmount = 0;
                topup_Recharge.Amount = 0;
                topup_Recharge.CreateDate = DateTime.Now;
                db.merchantTopupRecharges.Add(topup_Recharge);
            }
            else
            {
                //update
                topupRecharge_.ModifyDate = DateTime.Now;
            }
            // add entry in log table
            MerchantTopupRechargeLog topupRecharge = new MerchantTopupRechargeLog();
            topupRecharge.MerchantID = MerchantID;
            topupRecharge.TopupAmount = Convert.ToDecimal(modal.recharge);
            topupRecharge.Amount = Convert.ToDecimal(modal.recharge);
            topupRecharge.Mode = modal.Mode;
            topupRecharge.TransactionID_CheckNo = modal.TransactionID_CheckNo;
            topupRecharge.Status = 0;
            topupRecharge.IsActive = true;
            topupRecharge.CreateDate = DateTime.Now;
            db.merchantRechargeLog.Add(topupRecharge);

            db.SaveChanges();
            SendSMS(MerchantID);
            Merchant obj = db.Merchants.FirstOrDefault(p => p.Id == MerchantID);
            merchantNotification.SaveNotification(3, obj.FranchiseName);
            TempData["SuccessMsg"] = "Your recharge request is sent to Admin within 12 working hours your request will be accepted, for uninterrupted service recharge in advance. Thank You. Team eZeelo.";
            return RedirectToAction("Index", "Home");
        }

        public void SendSMS( long MerchantId)
        {
            try
            {
                Merchant mer = db.Merchants.FirstOrDefault(p => p.Id == MerchantId);
                Dictionary<string, string> dictSMSValues = new Dictionary<string, string>();

                BusinessLogicLayer.GateWay gateWay = new BusinessLogicLayer.SMS(System.Web.HttpContext.Current.Server);
                gateWay.SendSMS(BusinessLogicLayer.GateWay.SMSGateWays.SUMIT, BusinessLogicLayer.GateWay.SMSOptions.SINGLE, BusinessLogicLayer.GateWay.SMSTypes.MERCHANT_RECHARGE_REQUEST, new string[] { mer.ContactNumber }, dictSMSValues);
            }
            catch
            {

            }
        }
    }
}