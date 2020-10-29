using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using BusinessLogicLayer;
using Leaders.Filter;
using ModelLayer.Models;

//By Yashaswi 01-07-2019
namespace Leaders.Areas.Admin.Controllers
{
    [AdminSessionExpire]
    public class MerchantMasterController : Controller
    {
        EzeeloDBContext db = new EzeeloDBContext();
        //Service Master
        public ActionResult ServiceIndex()
        {
            List<ServiceMaster> list = db.ServiceMasters.OrderByDescending(p => p.Id).ToList();
            return View(list);
        }

        public ActionResult ServiceCreate()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ServiceCreate(ServiceMaster obj)
        {
            if (ModelState.IsValid)
            {
                if (db.ServiceMasters.Any(p => p.Name == obj.Name.Trim().Replace("\t", "")))
                {
                    ModelState.AddModelError("Name", "Service already present");
                    return View(obj);
                }
                ServiceMaster obj1 = new ServiceMaster();
                obj1.Name = obj.Name;
                obj1.IsActive = obj.IsActive;
                obj1.CreateBy = Convert.ToInt64(Session["ID"]);
                obj1.CreateDate = DateTime.Now;
                obj1.NetworkIP = CommonFunctions.GetClientIP();
                db.ServiceMasters.Add(obj1);
                db.SaveChanges();
                return RedirectToAction("ServiceIndex");
            }
            return View(obj);
        }

        public ActionResult ServiceUpdate(long Id)
        {
            ServiceMaster obj = db.ServiceMasters.Find(Id);
            ViewBag.IsInUse = db.Merchants.Any(p => p.Category == Id);
            return View(obj);
        }

        [HttpPost]
        public ActionResult ServiceUpdate(ServiceMaster obj)
        {
            if (ModelState.IsValid)
            {
                ServiceMaster obj1 = db.ServiceMasters.Find(obj.Id);
                if (db.ServiceMasters.Any(p => p.Name == obj.Name.Trim().Replace("\t", "") && p.Name != obj1.Name.Trim()))
                {
                    ModelState.AddModelError("Name", "Service already present");
                    return View(obj);
                }
                obj1.Name = obj.Name;
                obj1.IsActive = obj.IsActive;
                obj1.ModifyBy = Convert.ToInt64(Session["ID"]);
                obj1.ModifyDate = DateTime.Now;
                obj1.NetworkIP = CommonFunctions.GetClientIP();
                db.SaveChanges();
                return RedirectToAction("ServiceIndex");
            }
            return View(obj);
        }

        //Commission Master
        public ActionResult CommissionIndex()
        {
            List<CommissionMaster> list = db.CommissionMasters.OrderByDescending(p => p.Id).ToList();
            return View(list);
        }

        public ActionResult CommissionCreate()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CommissionCreate(CommissionMaster obj)
        {
            if (ModelState.IsValid)
            {
                if (db.CommissionMasters.Any(p => p.Commission == obj.Commission))
                {
                    ModelState.AddModelError("Commission", "Commission Value already present");
                    return View(obj);
                }
                CommissionMaster obj1 = new CommissionMaster();
                obj1.Commission = obj.Commission;
                obj1.IsActive = obj.IsActive;
                obj1.CreateBy = Convert.ToInt64(Session["ID"]);
                obj1.CreateDate = DateTime.Now;
                obj1.NetworkIP = CommonFunctions.GetClientIP();
                db.CommissionMasters.Add(obj1);
                db.SaveChanges();
                return RedirectToAction("CommissionIndex");
            }
            return View(obj);
        }

        public ActionResult CommissionUpdate(long Id)
        {
            CommissionMaster obj = db.CommissionMasters.Find(Id);
            ViewBag.IsInUse = db.Merchants.Any(p => p.Comission == Id);
            return View(obj);
        }

        [HttpPost]
        public ActionResult CommissionUpdate(CommissionMaster obj)
        {
            if (ModelState.IsValid)
            {
                CommissionMaster obj1 = db.CommissionMasters.Find(obj.Id);
                if (db.CommissionMasters.Any(p => p.Commission == obj.Commission && p.Commission != obj1.Commission))
                {
                    ModelState.AddModelError("Commission", "Commission Value already present");
                    return View(obj);
                }
                obj1.Commission = obj.Commission;
                obj1.IsActive = obj.IsActive;
                obj1.ModifyBy = Convert.ToInt64(Session["ID"]);
                obj1.ModifyDate = DateTime.Now;
                obj1.NetworkIP = CommonFunctions.GetClientIP();
                db.SaveChanges();
                return RedirectToAction("CommissionIndex");
            }
            return View(obj);
        }

        //Shop Timing Master
        public ActionResult ShopTimingIndex()
        {
            List<ShopTimingMaster> list = db.ShopTimingMasters.OrderByDescending(p => p.Id).ToList();
            return View(list);
        }

        public ActionResult ShopTimingCreate()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ShopTimingCreate(ShopTimingMaster obj)
        {
            if (ModelState.IsValid)
            {
                if (db.ShopTimingMasters.Any(p => p.FromTime == obj.FromTime && p.ToTime == obj.ToTime))
                {
                    ModelState.AddModelError("FromTime", "Same Shop Time entry already present");
                    return View(obj);
                }
                ShopTimingMaster obj1 = new ShopTimingMaster();
                obj1.FromTime = obj.FromTime;
                obj1.ToTime = obj.ToTime;
                obj1.IsActive = obj.IsActive;
                obj1.CreateBy = Convert.ToInt64(Session["ID"]);
                obj1.CreateDate = DateTime.Now;
                obj1.NetworkIP = CommonFunctions.GetClientIP();
                db.ShopTimingMasters.Add(obj1);
                db.SaveChanges();
                return RedirectToAction("ShopTimingIndex");
            }
            return View(obj);
        }

        public ActionResult ShopTimingUpdate(long Id)
        {
            ShopTimingMaster obj = db.ShopTimingMasters.Find(Id);
            //obj.FromTime = Convert.ToDateTime(obj.FromTime).ToString("hh:mm tt");
            //obj.ToTime = Convert.ToDateTime(obj.ToTime).ToString("hh:mm tt");

            ViewBag.IsInUse = db.Merchants.Any(p => p.ShopTiming == Id);
            return View(obj);
        }

        [HttpPost]
        public ActionResult ShopTimingUpdate(ShopTimingMaster obj)
        {
            if (ModelState.IsValid)
            {
                ShopTimingMaster obj1 = db.ShopTimingMasters.Find(obj.Id);
                ShopTimingMaster obj3 = db.ShopTimingMasters.FirstOrDefault(p => (p.FromTime == obj.FromTime && p.ToTime == obj.ToTime));
                if (obj3 != null)
                {
                    if (obj3.Id != obj1.Id)
                    {
                        ModelState.AddModelError("FromTime", "Same Shop Time entry already present");
                        return View(obj);
                    }
                }
                obj1.FromTime = obj.FromTime;
                obj1.ToTime = obj.ToTime;
                obj1.IsActive = obj.IsActive;
                obj1.ModifyBy = Convert.ToInt64(Session["ID"]);
                obj1.ModifyDate = DateTime.Now;
                obj1.NetworkIP = CommonFunctions.GetClientIP();
                db.SaveChanges();
                return RedirectToAction("ShopTimingIndex");
            }
            return View(obj);
        }

        //Holiday Timing Master
        public ActionResult HolidayIndex()
        {
            List<HolidayMaster> list = db.HolidayMasters.OrderByDescending(p => p.Id).ToList();
            return View(list);
        }

        public ActionResult HolidayCreate()
        {
            return View();
        }

        [HttpPost]
        public ActionResult HolidayCreate(HolidayMaster obj)
        {
            if (ModelState.IsValid)
            {
                if (db.HolidayMasters.Any(p => p.Name == obj.Name.Trim().Replace("\t", "") && p.HolidayDate == obj.HolidayDate))
                {
                    ModelState.AddModelError("Name", "Same Holiday entry already present");
                    return View(obj);
                }
                if (obj.HolidayDate != null)
                {
                    if (db.HolidayMasters.Any(p => p.HolidayDate == obj.HolidayDate))
                    {
                        ModelState.AddModelError("HolidayDate", "Same Holiday date entry already present");
                        return View(obj);
                    }
                }
                HolidayMaster obj1 = new HolidayMaster();
                obj1.Name = obj.Name;
                obj1.HolidayDate = obj.HolidayDate;
                obj1.IsActive = obj.IsActive;
                obj1.CreateBy = Convert.ToInt64(Session["ID"]);
                obj1.CreateDate = DateTime.Now;
                obj1.NetworkIP = CommonFunctions.GetClientIP();
                db.HolidayMasters.Add(obj1);
                db.SaveChanges();
                return RedirectToAction("HolidayIndex");
            }
            return View(obj);
        }

        public ActionResult HolidayUpdate(long Id)
        {
            HolidayMaster obj = db.HolidayMasters.Find(Id);
            ViewBag.IsInUse = db.MerchantHolidays.Any(p => p.HolidayID == Id);
            return View(obj);
        }

        [HttpPost]
        public ActionResult HolidayUpdate(HolidayMaster obj)
        {
            if (ModelState.IsValid)
            {
                HolidayMaster obj1 = db.HolidayMasters.Find(obj.Id);
                HolidayMaster obj3 = db.HolidayMasters.FirstOrDefault(p => p.Name == obj.Name.Trim().Replace("\t", "") && p.HolidayDate == obj.HolidayDate);
                if (obj3 != null)
                {
                    if (obj3.Id != obj1.Id)
                    {
                        ModelState.AddModelError("Name", "Same Holiday entry already present");
                        return View(obj);
                    }
                }
                if (obj.HolidayDate != null)
                {
                    HolidayMaster obj2 = db.HolidayMasters.FirstOrDefault(p => p.HolidayDate == obj.HolidayDate);
                    if (obj2 != null)
                    {
                        if (obj2.Id != obj1.Id)
                        {
                            ModelState.AddModelError("HolidayDate", "Same Holiday Date entry already present");
                            return View(obj);
                        }
                    }
                }
                obj1.Name = obj.Name;
                obj1.HolidayDate = obj.HolidayDate;
                obj1.IsActive = obj.IsActive;
                obj1.ModifyBy = Convert.ToInt64(Session["ID"]);
                obj1.ModifyDate = DateTime.Now;
                obj1.NetworkIP = CommonFunctions.GetClientIP();
                db.SaveChanges();
                return RedirectToAction("HolidayIndex");
            }
            return View(obj);
        }

        //Service Income Master
        public ActionResult ServiceIncomeIndex()
        {
            List<ServiceIncomeMaster> list = db.ServiceIncomeMasters.OrderByDescending(p => p.Id).ToList();
            return View(list);
        }

        public ActionResult ServiceIncomeCreate()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ServiceIncomeCreate(ServiceIncomeMaster obj)
        {
            if (db.ServiceIncomeMasters.Any())
            {
                ModelState.AddModelError("Company", "Master Entry is already created. You can not create new entry.");
            }
            else
            {
                if (ModelState.IsValid)
                {
                    ServiceIncomeMaster obj1 = new ServiceIncomeMaster();
                    obj1.Company = obj.Company;
                    obj1.RelationshipManager = obj.RelationshipManager;
                    obj1.UserLevel0 = obj.UserLevel0;
                    obj1.UptoLevel6 = obj.UptoLevel6;
                    obj1.Part5th = obj.Part5th;
                    obj1.IsActive = true;
                    obj1.CreateBy = Convert.ToInt64(Session["ID"]);
                    obj1.CreateDate = DateTime.Now;
                    obj1.NetworkIP = CommonFunctions.GetClientIP();
                    db.ServiceIncomeMasters.Add(obj1);
                    db.SaveChanges();
                    return RedirectToAction("ServiceIncomeIndex");
                }
            }
            return View(obj);
        }

        public ActionResult ServiceIncomeUpdate(long Id)
        {
            ServiceIncomeMaster obj = db.ServiceIncomeMasters.Find(Id);
            return View(obj);
        }

        [HttpPost]
        public ActionResult ServiceIncomeUpdate(ServiceIncomeMaster obj)
        {
            if (ModelState.IsValid)
            {
                if (obj.UserLevel0 + obj.UptoLevel6 + obj.Company + obj.RelationshipManager + obj.Part5th == 100)
                {
                    if (!db.ServiceIncomeMasters.Any(p => p.Company == obj.Company && p.UserLevel0 == obj.UserLevel0 && p.UptoLevel6 == obj.UptoLevel6 && p.RelationshipManager == obj.RelationshipManager))
                    {
                        ServiceIncomeMaster obj1 = db.ServiceIncomeMasters.Find(obj.Id);
                        obj1.Company = obj.Company;
                        obj1.RelationshipManager = obj.RelationshipManager;
                        obj1.UserLevel0 = obj.UserLevel0;
                        obj1.UptoLevel6 = obj.UptoLevel6;
                        obj1.Part5th = obj.Part5th;
                        obj1.IsActive = true;
                        obj1.ModifyBy = Convert.ToInt64(Session["ID"]);
                        obj1.ModifyDate = DateTime.Now;
                        obj1.NetworkIP = CommonFunctions.GetClientIP();

                        ServiceIncomeMaster_Log obj1Log = new ServiceIncomeMaster_Log();
                        obj1Log.Company = obj.Company;
                        obj1Log.RelationshipManager = obj.RelationshipManager;
                        obj1Log.UserLevel0 = obj.UserLevel0;
                        obj1Log.UptoLevel6 = obj.UptoLevel6;
                        obj1Log.Part5th = obj.Part5th;
                        obj1Log.IsActive = obj.IsActive;
                        obj1Log.CreateBy = Convert.ToInt64(Session["ID"]);
                        obj1Log.CreateDate = DateTime.Now;
                        obj1Log.NetworkIP = CommonFunctions.GetClientIP();
                        db.ServiceIncomeMasters_log.Add(obj1Log);
                        db.SaveChanges();
                    }
                }
                else
                {
                    ModelState.AddModelError("Company", "Sum of share value must be 100.");
                }
                return RedirectToAction("ServiceIncomeIndex");
            }
            return View(obj);
        }

        public ActionResult RechargeList()
        {
            List<MerchantTopupRechargeLog> list = db.merchantRechargeLog.OrderBy(p => p.CreateDate).OrderBy(p => p.Status).ToList()
                .Join(db.Merchants, l => l.MerchantID, m => m.Id, (l, m) => new MerchantTopupRechargeLog
                {
                    TopupAmount = l.TopupAmount,
                    Mode = l.Mode,
                    TransactionID_CheckNo = l.TransactionID_CheckNo,
                    CreateDate = l.CreateDate,
                    Status = l.Status,
                    recharge = m.FranchiseName,
                    ID = l.ID,
                    MerchantID = l.MerchantID
                }).ToList();
            foreach (var item in list)
            {
                List<MerchantTransactionDistribution> distribution = db.merchantTransactionDistributions.Where(p => p.MerchantId == item.MerchantID && p.IsPayable == false && p.CreateDate <= item.CreateDate).ToList();
                if (distribution != null)
                {
                    item.PendingTransaction = distribution.Sum(p => p.Commission);
                }
            }
            return View(list.OrderByDescending(p => p.CreateDate).ToList());
        }

        [HttpPost]
        public ActionResult RechargeReq_Accept(long Id, string Remark,decimal Amount)
        {
            string result = "";
            try
            {
                MerchantTopupRechargeLog objLog = db.merchantRechargeLog.FirstOrDefault(p => p.ID == Id);
                objLog.TopupAmount = Amount;
                objLog.Amount = Amount;
                MerchantTopupRecharge objMain = db.merchantTopupRecharges.FirstOrDefault(p => p.MerchantID == objLog.MerchantID);
                List<MerchantTopupRechargeLog> list = db.merchantRechargeLog.Where(p => p.MerchantID == objLog.MerchantID && p.Status == 1).ToList();
                if (list.Count() == 0)
                {
                    ///First Entry
                    ///deduct leader fee
                    decimal CoinRate = db.MLMCoinRates.FirstOrDefault(p => p.IsActive == true).Rate.Value;
                    decimal LeadersCommission = 0;
                    MerchantCommonValues values = db.MerchantCommonValues.FirstOrDefault();
                    Merchant merchant = db.Merchants.FirstOrDefault(p => p.Id == objLog.MerchantID);
                    UserLogin user = db.UserLogins.FirstOrDefault(p => p.Mobile == merchant.LeaderContactNo);
                    MerchantDetails details = db.merchantDetails.FirstOrDefault(p => p.MerchantId == objLog.MerchantID);
                    if (details == null)
                    {
                        LeadersCommission = values.LeaderCommission;
                    }
                    else
                    {
                        LeadersCommission = details.LeaderCommisiion;
                    }
                    if (objLog.TopupAmount <= LeadersCommission)
                    {
                        result = "Recharge Request Can't be Approve!!! Leaders Commission Is less than Topup Amount.";
                        return Json(result, JsonRequestBehavior.AllowGet);
                    }
                    MerchantTransactionDistribution distribution = new MerchantTransactionDistribution();
                    distribution.MerchantTransactionId = 0;
                    distribution.MerchantId = objLog.MerchantID;
                    distribution.RelationshipManagerPercentage = 0;
                    distribution.CompanyPercentage = 0;
                    distribution.Level0Percentage = 0;
                    distribution.UptoLevel6Percentage = 0;
                    distribution.Part5thPercentage = 0;
                    distribution.GST = 0;
                    distribution.IsApplied = false;
                    distribution.Commission = objLog.TopupAmount;
                    distribution.CalculatedCommission = LeadersCommission;
                    distribution.Company = 0;
                    distribution.Part5th = 0;
                    distribution.RelationshipManager_UserloginId = user.ID;
                    decimal GST = 1 + (18M / 100);
                    decimal calValue = Math.Round((LeadersCommission / GST), 2);
                    distribution.RelationshipManager = calValue / CoinRate;
                    distribution.Level0_UserLoginID = 0;
                    distribution.Level0 = 0;
                    distribution.Level1_UserLoginID = 0;
                    distribution.Level1 = 0;
                    distribution.Level2_UserLoginID = 0;
                    distribution.Level2 = 0;
                    distribution.Level3_UserLoginID = 0;
                    distribution.Level3 = 0;
                    distribution.Level4_UserLoginID = 0;
                    distribution.Level4 = 0;
                    distribution.Level5_UserLoginID = 0;
                    distribution.Level5 = 0;
                    distribution.Level6_UserLoginID = 0;
                    distribution.Level6 = 0;
                    distribution.IsPaid = false;
                    distribution.IsPayable = true;

                    distribution.CreateDate = DateTime.Now;
                    db.merchantTransactionDistributions.Add(distribution);

                    //calculateamount
                    objLog.Amount = objLog.TopupAmount - values.LeaderCommission;
                    objMain.TopupAmount = objLog.Amount;
                    objMain.Amount = objLog.Amount;
                    result = "Approved & Leaders commission deduct from recharge amount successfully!";
                }
                else
                {
                    objMain.TopupAmount = objMain.Amount + objLog.TopupAmount;
                    objMain.Amount = objMain.Amount + objLog.TopupAmount;
                    result = "Approved successfully!";
                }
                objLog.Status = 1;
                objLog.Remark = Remark;
                db.SaveChanges();
                SendSMS_RechargeAccept(objMain.MerchantID, objLog.Amount);
                CheckNegativeBalance(objMain.MerchantID);
            }
            catch (Exception ex)
            {
                result = "Error :" + ex.Message;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public void CheckNegativeBalance(long MerchantId)
        {
            List<MerchantTransactionDistribution> distributions = db.merchantTransactionDistributions.Where(p => p.MerchantId == MerchantId && p.IsPayable == false).ToList();
            foreach (var item in distributions)
            {
                MerchantTopupRecharge objMain = db.merchantTopupRecharges.FirstOrDefault(p => p.MerchantID == MerchantId);
                if (item.Commission <= objMain.Amount)
                {
                    objMain.Amount = objMain.Amount - item.Commission;
                    MerchantTransactionDistribution transactionDistribution = db.merchantTransactionDistributions.FirstOrDefault(p => p.ID == item.ID);
                    transactionDistribution.IsPayable = true;
                    db.SaveChanges();
                }
            }
        }

        public void SendSMS_RechargeAccept(long MerchantId, decimal amount)
        {
            try
            {
                Merchant obj = db.Merchants.FirstOrDefault(p => p.Id == MerchantId);

                string Mobile = obj.ContactNumber;
                Dictionary<string, string> dictSMSValues = new Dictionary<string, string>();
                dictSMSValues.Add("#--NAME--#", obj.ContactPerson);
                dictSMSValues.Add("#--TEXT--#", "Your amount " + amount + "Rs is added in your recharge. Thanks Team eZeelo");

                BusinessLogicLayer.GateWay gateWay = new BusinessLogicLayer.SMS(System.Web.HttpContext.Current.Server);
                gateWay.SendSMS(BusinessLogicLayer.GateWay.SMSGateWays.SUMIT, BusinessLogicLayer.GateWay.SMSOptions.SINGLE, BusinessLogicLayer.GateWay.SMSTypes.KYC_CMPT_REQUEST, new string[] { Mobile }, dictSMSValues);
            }

            catch
            {

            }
        }
        public void SendSMS_RechargeReject(long MerchantId)
        {
            try
            {
                Merchant obj = db.Merchants.FirstOrDefault(p => p.Id == MerchantId);

                string Mobile = obj.ContactNumber;
                Dictionary<string, string> dictSMSValues = new Dictionary<string, string>();
                dictSMSValues.Add("#--NAME--#", obj.ContactPerson);
                dictSMSValues.Add("#--TEXT--#", "Your amount is not reflected in our bank so this recharge is declined. Please process the same again. Thanks Team eZeelo");

                BusinessLogicLayer.GateWay gateWay = new BusinessLogicLayer.SMS(System.Web.HttpContext.Current.Server);
                gateWay.SendSMS(BusinessLogicLayer.GateWay.SMSGateWays.SUMIT, BusinessLogicLayer.GateWay.SMSOptions.SINGLE, BusinessLogicLayer.GateWay.SMSTypes.KYC_CMPT_REQUEST, new string[] { Mobile }, dictSMSValues);
            }
            catch
            {

            }
        }
        [HttpPost]
        public ActionResult RechargeReq_Reject(long Id, string Remark)
        {
            string result = "";
            try
            {
                MerchantTopupRechargeLog objLog = db.merchantRechargeLog.FirstOrDefault(p => p.ID == Id);
                MerchantTopupRecharge objMain = db.merchantTopupRecharges.FirstOrDefault(p => p.MerchantID == objLog.MerchantID);
                objLog.Status = 2;
                objLog.ModifyDate = DateTime.Now;
                objLog.Remark = Remark;
                db.SaveChanges();

                objMain.TopupAmount = 0;
                objMain.Amount = 0;
                db.SaveChanges();
                SendSMS_RechargeReject(objMain.MerchantID);
                result = "Recharge entry rejected successfully!";
            }
            catch (Exception ex)
            {
                result = "Error :" + ex.Message;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult MerchantSelfCBPointsList()
        {
            List<MerchantSelfCBPoints> list = db.merchantSelfCBPoints.ToList();
            list.ForEach(p => p.City = (db.Cities.FirstOrDefault(c => c.ID == p.CityId).Name));
            return View(list);
        }

        public ActionResult MerchantSelfCBPointsCreate()
        {
            MerchantSelfCBPoints merchantSelfCBPoints = new MerchantSelfCBPoints();
            return View(merchantSelfCBPoints);
        }

        [HttpPost]
        public ActionResult MerchantSelfCBPointsCreate(MerchantSelfCBPoints merchantSelfCBPoints)
        {
            merchantSelfCBPoints.CreateDate = DateTime.Now;
            db.merchantSelfCBPoints.Add(merchantSelfCBPoints);
            db.SaveChanges();
            return RedirectToAction("MerchantSelfCBPointsList");
        }

        public ActionResult MerchantSelfCBPointsEdit(long id)
        {
            MerchantSelfCBPoints merchantSelfCBPoints = db.merchantSelfCBPoints.FirstOrDefault(p => p.Id == id);
            merchantSelfCBPoints.City = db.Cities.FirstOrDefault(p => p.ID == merchantSelfCBPoints.CityId).Name;
            return View(merchantSelfCBPoints);
        }

        [HttpPost]
        public ActionResult MerchantSelfCBPointsEdit(MerchantSelfCBPoints merchantSelfCBPoints_)
        {
            MerchantSelfCBPoints merchantSelfCBPoints = db.merchantSelfCBPoints.FirstOrDefault(p => p.Id == merchantSelfCBPoints_.Id);
            merchantSelfCBPoints.CreateDate = DateTime.Now;
            merchantSelfCBPoints.IsActive = merchantSelfCBPoints_.IsActive;
            merchantSelfCBPoints.CBPoints = merchantSelfCBPoints_.CBPoints;
            db.SaveChanges();
            return RedirectToAction("MerchantSelfCBPointsList");
        }

        [HttpPost]
        public JsonResult CityList(string prefix)
        {
            var cities = db.Cities.Where(p => p.IsActive == true && !(db.merchantSelfCBPoints.Select(c => c.CityId).Contains(p.ID)) && p.Name.StartsWith(prefix))
                .Select(p => new
                {
                    label = p.Name,
                    val = p.ID
                }).ToList();

            return Json(cities);
        }

        [HttpPost]
        public JsonResult GetNotifications()
        {
            List<MerchantNotifications> merchantNotifications = (new MerchantNotification()).GetMerchantNotification();
            return Json(merchantNotifications, JsonRequestBehavior.AllowGet);
        }


        public ActionResult UpdateNotificationStatus(long id)
        {
            MerchantNotifications notifications = db.merchantNotifications.FirstOrDefault(p => p.Id == id);
            notifications.IsRead = true;
            db.SaveChanges();
            switch (notifications.Type)
            {
                case 1:
                    return RedirectToAction("Registered", "MerchantList");
                case 2:
                    return RedirectToAction("Registered", "MerchantList");
                case 3:
                    return RedirectToAction("RechargeList");
                case 4:
                    return RedirectToAction("ProfileUpdateList", "MerchantList");
                case 5:
                    return RedirectToAction("BannerImageList", "MerchantList");
                case 6:
                    return RedirectToAction("Index", "Home");
                case 7:
                    return RedirectToAction("Index", "Home");
                default:
                    return RedirectToAction("Index", "Home");
            }
        }
    }
}