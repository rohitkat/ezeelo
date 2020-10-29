using BusinessLogicLayer;
using MarketPartner.Filter;
using ModelLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace MarketPartner.Controllers
{
    [SessionExpire]
    [Authorize]
    public class MerchantTransactionController : Controller
    {
        EzeeloDBContext db = new EzeeloDBContext();
        MerchantNotification merchantNotification = new MerchantNotification();
        public ActionResult AddNew()
        {
            long MerchantID = Convert.ToInt64(Session["MerchantID"]);
            MerchantTransaction transaction = new MerchantTransaction();
            transaction.Service = db.Merchants.FirstOrDefault(p => p.Id == MerchantID).ServiceMasterDetail.Name;
            CheckRechargeStatus(MerchantID, out string Type, out string Msg);
            ViewBag.Type = Type;
            ViewBag.Msg = Msg;
            MerchantTopupRecharge merchantTopupRecharges = db.merchantTopupRecharges.FirstOrDefault(p => p.MerchantID == MerchantID);
            if (merchantTopupRecharges != null)
            {
                ViewBag.RemainingAmt = merchantTopupRecharges.Amount;
            }
            return View(transaction);
        }

        public void CheckRechargeStatus(long MerchantID, out string Type, out string Msg)
        {
            Type = "2";
            Msg = "";
            MerchantTopupRecharge merchantTopupRecharges = db.merchantTopupRecharges.FirstOrDefault(p => p.MerchantID == MerchantID);
            if (merchantTopupRecharges == null)
            {
                //no Recharge found
                Type = "0";
                Msg = "Kindly Topup your Cashback Deposit account for uninterrupted services. Team Ezeelo";
            }
            else
            {
                decimal TotalAmount = merchantTopupRecharges.TopupAmount;

                if (TotalAmount == 0)
                {
                    Type = "0";
                    Msg = "Sorry !!! You can not add new transaction. Your Topup request is in process.";
                }
                else
                {
                    MerchantCommonValues values = db.MerchantCommonValues.FirstOrDefault();
                    decimal RemainingAMount = merchantTopupRecharges.Amount;
                    decimal WarningPer = values.TopupMax;
                    decimal BlockedPer = values.TopupMin;
                    decimal BlockedValue = TotalAmount * (BlockedPer / 100);
                    decimal WarningValue = TotalAmount * (WarningPer / 100);
                    if (RemainingAMount == 0)
                    {
                        Type = "0";
                        Msg = "Cashback Deposit is 0 , Kindly Topup your account earliest. Team Ezeelo";
                    }
                    else if (BlockedValue >= RemainingAMount)
                    {
                        //display blocked meassage
                        Type = "1";
                        Msg = "Cashback Deposit has gone below " + Math.Round(BlockedPer) + "% , Kindly Topup for uninterrupted services. Team Ezeelo";
                    }
                    else if (WarningValue >= RemainingAMount)
                    {
                        //display warning message
                        Type = "1";
                        Msg = "Cashback Deposit has gone below " + Math.Round(WarningPer) + "% , Kindly Topup your account earliest. Team Ezeelo";
                    }
                }
            }
        }

        [HttpPost]
        public ActionResult AddNew(MerchantTransaction transaction)
        {
            JsonResult obj = ValidateContactNo(transaction.MobileNo);
            if (obj.Data.ToString().Contains("data = 1"))
            {
                long MerchantId = Convert.ToInt64(Session["MerchantID"]);
                string Type;
                string Msg;
                CheckRechargeStatus(MerchantId, out Type, out Msg);

                if (Type == "0")
                {
                    ModelState.AddModelError("BillAmount", Msg);
                    ViewBag.Type = Type;
                    ViewBag.Msg = Msg;
                    return View(transaction);
                }
                try
                {
                    SaveTransaction(MerchantId, transaction.BillAmount, transaction.MobileNo, transaction.Service, transaction.Remark, transaction.Name, false);
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("MobileNo", "Problem in saving transaction " + ex.Message);
                    return View(transaction);
                }
                //bool IsLessCommission = false;
                //decimal Commission = GetEzeeloCommission(transaction.BillAmount);
                //decimal RemainingDistribuatedAmount = 0;
                //decimal RemainingRechargeAmount = 0;
                //MerchantTopupRecharge merchantTopupRecharges = db.merchantTopupRecharges.FirstOrDefault(p => p.MerchantID == MerchantId);
                //if (merchantTopupRecharges != null)
                //{
                //    RemainingRechargeAmount = merchantTopupRecharges.Amount;
                //    if (merchantTopupRecharges.Amount < Commission)
                //    {
                //        RemainingDistribuatedAmount = Commission - merchantTopupRecharges.Amount;
                //        IsLessCommission = true;
                //    }
                //}
                //using (TransactionScope ts = new TransactionScope(TransactionScopeOption.Required,
                //                   new System.TimeSpan(0, 15, 0)))
                //{
                //    try
                //    {
                //        UserLogin user = db.UserLogins.FirstOrDefault(p => p.Mobile == transaction.MobileNo);
                //        MerchantTransaction transaction_ = new MerchantTransaction();
                //        transaction_.UserLoginID = user.ID;
                //        transaction_.MerchantID = MerchantId;
                //        transaction_.Service = transaction.Service;
                //        transaction_.Remark = transaction.Remark;
                //        transaction_.BillAmount = transaction.BillAmount;
                //        transaction_.Comission = Commission;
                //        transaction_.CreateDate = DateTime.Now;
                //        transaction_.NetworkIP = CommonFunctions.GetClientIP();
                //        transaction_.TransactionCode = GenerateCode();
                //        transaction_.MobileNo = transaction.MobileNo;
                //        transaction_.Name = transaction.Name;
                //        db.merchantTransactions.Add(transaction_);
                //        db.SaveChanges();

                //        if (IsLessCommission)
                //        {
                //            DistributeCommission(MerchantId, merchantTopupRecharges.Amount, transaction_.ID, transaction_.UserLoginID, true);
                //            DistributeCommission(MerchantId, RemainingDistribuatedAmount, transaction_.ID, transaction_.UserLoginID, false);
                //            DeductCommissionFromRecharge(MerchantId, merchantTopupRecharges.Amount);
                //        }
                //        else
                //        {
                //            DistributeCommission(MerchantId, transaction_.Comission, transaction_.ID, transaction_.UserLoginID, true);
                //            DeductCommissionFromRecharge(MerchantId, transaction_.Comission);
                //        }
                //        SendSMS(transaction.MobileNo, transaction_.MerchantID, user.ID, transaction_.TransactionCode, transaction.BillAmount.ToString());
                //        ts.Complete();
                //        if (IsLessCommission)
                //        {
                //            TempData["SuccessMsg"] = "Transaction Saved Successfully, Only " + RemainingRechargeAmount + " Rs commission value is distributed & Transaction code is " + transaction_.TransactionCode;
                //        }
                //        else
                //        {
                //            TempData["SuccessMsg"] = "Transaction Saved Successfully & Transaction code is " + transaction_.TransactionCode;
                //        }
                //    }
                //    catch (Exception ex)
                //    {
                //        Transaction.Current.Rollback();
                //        ts.Dispose();
                //        ModelState.AddModelError("MobileNo", "Problem in saving transaction " + ex.Message);
                //        return View(transaction);
                //    }
                //}
                return RedirectToAction("AddNew");
            }
            else
            {
                ModelState.AddModelError("MobileNo", "Invalid Contact Number");
                return View(transaction);
            }
        }
        public string SaveTransaction(long MerchantId,decimal BillAmount,string MobileNo,string Service,string Remark, string Name,bool isRequest)
        {
            UserLogin user = db.UserLogins.FirstOrDefault(p => p.Mobile == MobileNo);
            MerchantTransaction transaction_ = new MerchantTransaction();
            bool IsLessCommission = false;
            decimal Commission = GetEzeeloCommission(BillAmount,MerchantId);
            decimal RemainingDistribuatedAmount = 0;
            decimal RemainingRechargeAmount = 0;
            MerchantTopupRecharge merchantTopupRecharges = db.merchantTopupRecharges.FirstOrDefault(p => p.MerchantID == MerchantId);
            if (merchantTopupRecharges != null)
            {
                RemainingRechargeAmount = merchantTopupRecharges.Amount;
                if (merchantTopupRecharges.Amount < Commission)
                {
                    RemainingDistribuatedAmount = Commission - merchantTopupRecharges.Amount;
                    IsLessCommission = true;
                }
            }
            using (TransactionScope ts = new TransactionScope(TransactionScopeOption.Required,
                               new System.TimeSpan(0, 15, 0)))
            {
                try
                {
                    transaction_.UserLoginID = user.ID;
                    transaction_.MerchantID = MerchantId;
                    transaction_.Service = Service;
                    transaction_.Remark =Remark;
                    transaction_.BillAmount = BillAmount;
                    transaction_.Comission = Commission;
                    transaction_.CreateDate = DateTime.Now;
                    transaction_.NetworkIP = CommonFunctions.GetClientIP();
                    transaction_.TransactionCode = GenerateCode();
                    transaction_.MobileNo = MobileNo;
                    transaction_.Name = Name;
                    db.merchantTransactions.Add(transaction_);
                    db.SaveChanges();

                    if (IsLessCommission)
                    {
                        DistributeCommission(MerchantId, merchantTopupRecharges.Amount, transaction_.ID, transaction_.UserLoginID, true);
                        DistributeCommission(MerchantId, RemainingDistribuatedAmount, transaction_.ID, transaction_.UserLoginID, false);
                        DeductCommissionFromRecharge(MerchantId, merchantTopupRecharges.Amount);
                    }
                    else
                    {
                        DistributeCommission(MerchantId, transaction_.Comission, transaction_.ID, transaction_.UserLoginID, true);
                        DeductCommissionFromRecharge(MerchantId, transaction_.Comission);
                    }                    
                    ts.Complete();                    
                    if (IsLessCommission)
                    {
                        TempData["SuccessMsg"] = "Transaction Saved Successfully, Only " + RemainingRechargeAmount + " Rs commission value is distributed & Transaction code is " + transaction_.TransactionCode;
                    }
                    else
                    {
                        TempData["SuccessMsg"] = "Transaction Saved Successfully & Transaction code is " + transaction_.TransactionCode;
                    }
                   
                }
                catch (Exception ex)
                {
                    Transaction.Current.Rollback();
                    ts.Dispose();
                    throw ex;
                }
            }
            if (isRequest)
            {
                SendSMS_TransRequestAccept(MobileNo, transaction_.MerchantID, user.ID, transaction_.TransactionCode, BillAmount.ToString(), transaction_.TransactionCode);
            }
            else
            {
                SendSMS(MobileNo, transaction_.MerchantID, user.ID, transaction_.TransactionCode, BillAmount.ToString());
            }
            return transaction_.TransactionCode;
        }

        public void DeductCommissionFromRecharge(long MerchantID, decimal Commission)
        {
            MerchantTopupRecharge merchantTopupRecharges = db.merchantTopupRecharges.FirstOrDefault(p => p.MerchantID == MerchantID);
            Merchant obj = db.Merchants.FirstOrDefault(p => p.Id == MerchantID);
            if (merchantTopupRecharges != null)
            {
                if (merchantTopupRecharges.Amount >= Commission)
                {
                    merchantTopupRecharges.Amount = merchantTopupRecharges.Amount - Commission;
                    db.SaveChanges();
                }
                //check for condition
                MerchantCommonValues values = db.MerchantCommonValues.FirstOrDefault();
                decimal RemainingAMount = merchantTopupRecharges.Amount;
                decimal WarningPer = values.TopupMax;
                decimal BlockedPer = values.TopupMin;
                decimal BlockedValue = merchantTopupRecharges.TopupAmount * (BlockedPer / 100);
                decimal WarningValue = merchantTopupRecharges.TopupAmount * (WarningPer / 100);
                if (BlockedValue >= RemainingAMount)
                {
                    //Send blocked meassage
                    SendSMS_RechargeBlocked(MerchantID);
                    merchantNotification.SaveNotification(6, obj.FranchiseName);
                }
                else if (WarningValue >= RemainingAMount)
                {
                    //Send warning message
                    SendSMS_RechargeWarning(MerchantID);
                    merchantNotification.SaveNotification(7, obj.FranchiseName);
                }
            }
        }

        public void DistributeCommission(long MerchantId, decimal Commission, long TransactionId, long UserLoginId, bool IsPayable)
        {
            Merchant merchant = db.Merchants.FirstOrDefault(p => p.Id == MerchantId);
            UserLogin user = db.UserLogins.FirstOrDefault(p => p.Mobile == merchant.LeaderContactNo);
            MerchantDetails obj = db.merchantDetails.FirstOrDefault(p => p.MerchantId == MerchantId);
            ServiceIncomeMaster incomeMaster = db.ServiceIncomeMasters.FirstOrDefault();
            if (obj == null)
            {
                obj = new MerchantDetails();
                obj.Company = incomeMaster.Company;
                obj.RMCommission = incomeMaster.RelationshipManager;
                obj.Level0 = incomeMaster.UserLevel0;
                obj.UptoLevel6 = incomeMaster.UptoLevel6;
                obj.Part5th = incomeMaster.Part5th;
                obj.IsGSTApply = true;
                obj.GST = 18;
            }
            decimal CoinRate = db.MLMCoinRates.FirstOrDefault(p => p.IsActive == true).Rate.Value;
            MerchantTransactionDistribution distribution = new MerchantTransactionDistribution();
            distribution.MerchantTransactionId = TransactionId;
            distribution.MerchantId = MerchantId;
            distribution.RelationshipManagerPercentage = obj.RMCommission;
            distribution.CompanyPercentage = obj.Company;
            distribution.Level0Percentage = obj.Level0;
            distribution.UptoLevel6Percentage = obj.UptoLevel6;
            distribution.Part5thPercentage = obj.Part5th;
            distribution.GST = obj.GST;
            distribution.IsApplied = obj.IsGSTApply;
            distribution.Commission = Commission;
            if (obj.IsGSTApply)
            {
                decimal GST = 1 + (obj.GST / 100);
                decimal calValue = Commission / GST;
                distribution.CalculatedCommission = Math.Round(calValue, 2);
            }
            else
            {
                distribution.CalculatedCommission = Commission;
            }
            distribution.Company = Math.Round(distribution.CalculatedCommission * (distribution.CompanyPercentage / 100), 2);
            distribution.Company = distribution.Company / CoinRate; //Convert into ERP
            distribution.Part5th = Math.Round(distribution.CalculatedCommission * (distribution.Part5thPercentage / 100), 2);
            distribution.Part5th = distribution.Part5th / CoinRate; //Convert into ERP
            distribution.RelationshipManager_UserloginId = user.ID;
            distribution.RelationshipManager = Math.Round(distribution.CalculatedCommission * (distribution.RelationshipManagerPercentage / 100), 2);
            distribution.RelationshipManager = distribution.RelationshipManager / CoinRate; //Convert into ERP
            distribution.Level0_UserLoginID = UserLoginId;
            distribution.Level0 = Math.Round(distribution.CalculatedCommission * (distribution.Level0Percentage / 100), 2);
            distribution.Level0 = distribution.Level0 / CoinRate; // Convert into ERP
            //Calculation 
            decimal UptoLevel6Value = Math.Round(distribution.CalculatedCommission * (distribution.UptoLevel6Percentage / 100), 2);
            UptoLevel6Value = UptoLevel6Value / CoinRate; // Convert into ERP
            MLMWalletPoints points = new MLMWalletPoints();
            decimal Up1 = Convert.ToDecimal(points.GetUpLineValue(1));
            decimal Up2 = Convert.ToDecimal(points.GetUpLineValue(2));
            decimal Up3 = Convert.ToDecimal(points.GetUpLineValue(3));
            decimal Up4 = Convert.ToDecimal(points.GetUpLineValue(4));
            decimal Up5 = Convert.ToDecimal(points.GetUpLineValue(5));
            decimal Up6 = Convert.ToDecimal(points.GetUpLineValue(6));

            long UpLine1_UserLoginId = points.GetUpLine(UserLoginId);
            decimal UpLine1 = Math.Round(UptoLevel6Value * Up1, 2);

            long UpLine2_UserLoginId = points.GetUpLine(UpLine1_UserLoginId);
            decimal UpLine2 = Math.Round(UptoLevel6Value * Up2, 2);

            long UpLine3_UserLoginId = points.GetUpLine(UpLine2_UserLoginId);
            decimal UpLine3 = Math.Round(UptoLevel6Value * Up3, 2);

            long UpLine4_UserLoginId = points.GetUpLine(UpLine3_UserLoginId);
            decimal UpLine4 = Math.Round(UptoLevel6Value * Up4, 2);

            long UpLine5_UserLoginId = points.GetUpLine(UpLine4_UserLoginId);
            decimal UpLine5 = Math.Round(UptoLevel6Value * Up5, 2);

            long UpLine6_UserLoginId = points.GetUpLine(UpLine5_UserLoginId);
            decimal UpLine6 = Math.Round(UptoLevel6Value * Up6, 2);

            distribution.UptoLevel6 = UptoLevel6Value;
            distribution.Level1_UserLoginID = UpLine1_UserLoginId;
            distribution.Level1 = UpLine1;
            distribution.Level2_UserLoginID = UpLine2_UserLoginId;
            distribution.Level2 = UpLine2;
            distribution.Level3_UserLoginID = UpLine3_UserLoginId;
            distribution.Level3 = UpLine3;
            distribution.Level4_UserLoginID = UpLine4_UserLoginId;
            distribution.Level4 = UpLine4;
            distribution.Level5_UserLoginID = UpLine5_UserLoginId;
            distribution.Level5 = UpLine5;
            distribution.Level6_UserLoginID = UpLine6_UserLoginId;
            distribution.Level6 = UpLine6;
            distribution.IsPaid = false;
            distribution.IsPayable = IsPayable;
            distribution.CreateDate = DateTime.Now;
            db.merchantTransactionDistributions.Add(distribution);
            db.SaveChanges();
        }
        public void SendSMS(string MobileNO, long MerchantId, long UserId, string Code, string BillAmt)
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
                gateWay.SendSMS(BusinessLogicLayer.GateWay.SMSGateWays.SUMIT, BusinessLogicLayer.GateWay.SMSOptions.SINGLE, BusinessLogicLayer.GateWay.SMSTypes.MERCHANT_TRANSACTION_USER, new string[] { MobileNO }, dictSMSValues);
            }
            catch
            {

            }
        }
        public void SendSMS_TransRequestAccept(string MobileNO, long MerchantId, long UserId, string Code, string BillAmt,string NewCode)
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
                dictSMSValues.Add("#--NEWCODE--#", NewCode);
                BusinessLogicLayer.GateWay gateWay = new BusinessLogicLayer.SMS(System.Web.HttpContext.Current.Server);
                gateWay.SendSMS(BusinessLogicLayer.GateWay.SMSGateWays.SUMIT, BusinessLogicLayer.GateWay.SMSOptions.SINGLE, BusinessLogicLayer.GateWay.SMSTypes.MERCHANT_TRANSACTION_REQUEST_ACCEPT, new string[] { MobileNO }, dictSMSValues);
            }
            catch(Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                  + Environment.NewLine + ex.Message + Environment.NewLine
                  + "[MerchantTransaction][SendSMS_TransRequestAccept]",
                  BusinessLogicLayer.ErrorLog.Module.MarketPartner, System.Web.HttpContext.Current.Server);
            }
        }
        public void SendSMS_RechargeWarning(long MerchantId)
        {
            try
            {
                Merchant mer = db.Merchants.FirstOrDefault(p => p.Id == MerchantId);
                Dictionary<string, string> dictSMSValues = new Dictionary<string, string>();
                dictSMSValues.Add("#--NAME--#", mer.ContactPerson);

                BusinessLogicLayer.GateWay gateWay = new BusinessLogicLayer.SMS(System.Web.HttpContext.Current.Server);
                gateWay.SendSMS(BusinessLogicLayer.GateWay.SMSGateWays.SUMIT, BusinessLogicLayer.GateWay.SMSOptions.SINGLE, BusinessLogicLayer.GateWay.SMSTypes.MERCHANT_RECHARGE_WARNING, new string[] { mer.ContactNumber }, dictSMSValues);
                gateWay.SendSMS(BusinessLogicLayer.GateWay.SMSGateWays.SUMIT, BusinessLogicLayer.GateWay.SMSOptions.SINGLE, BusinessLogicLayer.GateWay.SMSTypes.LEADER_RECHARGE_WARNING, new string[] { mer.LeaderContactNo }, dictSMSValues);
            }
            catch
            {

            }
        }
        public void SendSMS_RechargeBlocked(long MerchantId)
        {
            try
            {
                Merchant mer = db.Merchants.FirstOrDefault(p => p.Id == MerchantId);
                Dictionary<string, string> dictSMSValues = new Dictionary<string, string>();
                dictSMSValues.Add("#--NAME--#", mer.ContactPerson);

                BusinessLogicLayer.GateWay gateWay = new BusinessLogicLayer.SMS(System.Web.HttpContext.Current.Server);
                gateWay.SendSMS(BusinessLogicLayer.GateWay.SMSGateWays.SUMIT, BusinessLogicLayer.GateWay.SMSOptions.SINGLE, BusinessLogicLayer.GateWay.SMSTypes.MERCHANT_TRANSACTION_2WARNING, new string[] { mer.ContactNumber }, dictSMSValues);
                gateWay.SendSMS(BusinessLogicLayer.GateWay.SMSGateWays.SUMIT, BusinessLogicLayer.GateWay.SMSOptions.SINGLE, BusinessLogicLayer.GateWay.SMSTypes.LEADER_TRANSACTION_2WARNING, new string[] { mer.LeaderContactNo }, dictSMSValues);
            }
            catch
            {

            }
        }
        public string GenerateCode()
        {
            BusinessLogicLayer.MarketPartner obj = new BusinessLogicLayer.MarketPartner();
            return obj.GetNextTransactionCode();
        }
        public JsonResult ValidateContactNo(string contactNo)
        {
            object obj = new object();
            try
            {
                UserLogin user = db.UserLogins.FirstOrDefault(p => p.Mobile == contactNo);
                if (user != null)
                {
                    MLMUser user_ = db.MLMUsers.FirstOrDefault(p => p.UserID == user.ID);
                    PersonalDetail personalDetail = db.PersonalDetails.FirstOrDefault(p => p.UserLoginID == user.ID);
                    if (user_ != null)
                    {
                        string Name = personalDetail.FirstName + " " + ((personalDetail.LastName == null) ? "" : personalDetail.LastName);
                        obj = new { data = 1, result = "Mobile number is verified!", Name = Name.Trim() };
                    }
                    else
                    {
                        obj = new { data = 2, result = "Mobile number is verified! But user is not register as Leader.", Name = "" };
                    }
                }
                else
                {
                    obj = new { data = 0, result = "Mobile Number is not registered with Ezeelo!", Name = "" };
                }
            }
            catch (Exception ex)
            {
            }
            return Json(obj, JsonRequestBehavior.AllowGet);
        }
        public JsonResult CalculateCommission(decimal BillAMount)
        {
            long MerchantID = Convert.ToInt64(Session["MerchantID"]);
            return Json(GetEzeeloCommission(BillAMount, MerchantID), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetUserTransaction(string MobileNo)
        {
            long MerchantID = Convert.ToInt64(Session["MerchantID"]);
            UserLogin user = db.UserLogins.FirstOrDefault(p => p.Mobile == MobileNo);
            if (user != null)
            {
                var list = db.merchantTransactions.Where(p => p.UserLoginID == user.ID && p.MerchantID == MerchantID)
                    .OrderByDescending(p => p.CreateDate)
                    .AsEnumerable()
                    .Select(p => new
                    {
                        code = p.TransactionCode,
                        Date = p.CreateDate.ToString("dd/MM/yyyy"),
                        Amount = p.BillAmount
                    }).ToList().Take(10);

                return Json(list, JsonRequestBehavior.AllowGet);
            }
            return Json("", JsonRequestBehavior.AllowGet);
        }
        public decimal GetEzeeloCommission(decimal BillAmount,long MerchantId)
        {
            decimal CommisionPer = db.Merchants.FirstOrDefault(p => p.Id == MerchantId).CommissionMasterDetail.Commission;
            decimal commission = (CommisionPer / 100) * BillAmount;
            return Math.Round(commission, 2);
        }
    }
}