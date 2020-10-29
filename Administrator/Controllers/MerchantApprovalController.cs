//-----------------------------------------------------------------------
// <copyright file="MerchantApprovalController.cs" company="Ezeelo Consumer Services Pvt. Ltd.">
//     Copyright (c) Ezeelo Consumer Services Pvt. Ltd. All rights reserved.
// </copyright>
// <author>Snehal Shende</author>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ModelLayer.Models;
using ModelLayer.Models.ViewModel;
using BusinessLogicLayer;
using System.Data.Entity;
using System.Text;
using Administrator.Models;

namespace Administrator.Controllers
{
    public class MerchantApprovalController : Controller
    {
        private EzeeloDBContext db = new EzeeloDBContext();

        //
        // GET: /MerchantApproval/
        [SessionExpire]
        [CustomAuthorize(Roles = "MerchantApproval/CanRead")]
        public ActionResult Index(int franchiseID)
        {
            try
            {
                var lfav = (from ul in db.UserLogins
                            join bd in db.BusinessDetails on ul.ID equals bd.UserLoginID
                            join pd in db.PersonalDetails on ul.ID equals pd.UserLoginID
                            join s in db.Shops on bd.ID equals s.BusinessDetailID
                            join op in db.OwnerPlans on s.ID equals op.OwnerID
                            //join opc in db.OwnerPlanCategoryCharges on op.ID equals opc.OwnerPlanID
                            where bd.BusinessType.Prefix == "GBMR" && ul.IsLocked == true && s.FranchiseID == franchiseID && op.IsActive == true
                            select new MerchantPendingApprovalViewModel
                            {
                                UserLoginID = bd.UserLoginID,
                                BusinessTypePrefix = bd.BusinessType.Prefix,
                                Name = pd.Salutation.Name + " " + pd.FirstName + " " + pd.LastName + " (" + s.Name + ")",
                                OwnerId = s.ID,
                            }).Distinct().OrderBy(x => x.Name);

                return View(lfav);
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[MerchantApprovalController][GET:Index]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[MerchantApprovalController][GET:Index]",
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
            }
            return View();
        }

        [SessionExpire]
        [CustomAuthorize(Roles = "MerchantApproval/CanRead")]
        public ActionResult ShowFranchiseList()
        {
            try
            {
                //if (Session["FranchiseID"] == null)
                //{
                //var lfav = from ul in db.UserLogins
                //           join bd in db.BusinessDetails on ul.ID equals bd.UserLoginID
                //           join pd in db.PersonalDetails on ul.ID equals pd.UserLoginID
                //           join f in db.Franchises on bd.ID equals f.BusinessDetailID
                //           where bd.BusinessType.Prefix == "GBFR" && ul.IsLocked == false && f.ID != 1
                //           select new FranchisePendingApprovalViewModel
                //           {
                //               UserLoginID = bd.UserLoginID,
                //               BusinessTypePrefix = bd.BusinessType.Prefix,
                //               Name = pd.Salutation.Name + " " + pd.FirstName + " " + pd.LastName + " (" + bd.Name + ")",
                //               OwnerId = f.ID,
                //           };

                var result = db.UserLogins.Where(u => u.IsLocked == false).Join(db.BusinessDetails.Where(b => db.BusinessTypes.Where(bt => bt.Prefix == "GBFR").Select(bt => bt.ID).Contains(b.BusinessTypeID)), u => u.ID, b => b.UserLoginID, (u, b) => new { UserLoginId = u.ID, BdName = (b.Name == null) ? "" : b.Name, BdId = b.ID })
                      .Join(db.PersonalDetails, u => u.UserLoginId, p => p.UserLoginID, (u, p) => new
                      {
                          UserLoginId = u.UserLoginId,
                          FirstName = (p.FirstName == null) ? "" : p.FirstName,
                          LastName = (p.LastName == null) ? "" : p.LastName,
                          BDName = u.BdName,
                          BdId = u.BdId
                      })
                      .Join(db.Franchises, u => u.BdId, f => f.BusinessDetailID, (u, f) => new FranchisePendingApprovalViewModel
                      {
                          UserLoginID = u.UserLoginId,
                          BusinessTypePrefix = "GBFR",
                          Name = u.FirstName + " " + u.LastName + " (" + u.BDName + ")",
                          OwnerId = f.ID,
                      }).ToList();
                return View(result);
                //}
                //else
                //{
                //    return View("Index", new { franchiseID = Convert.ToInt32(Session["FranchiseID"]) });
                //}
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[MerchantApprovalController][GET:ShowFranchiseList]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[MerchantApprovalController][GET:ShowFranchiseList]",
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
            }
            return View();
        }

        //
        // GET: /MerchantApproval/Approve/5
        [HttpGet]
        [SessionExpire]
        [CustomAuthorize(Roles = "MerchantApproval/CanRead")]
        public ActionResult Approve(long userLoginId)
        {
            try
            {
                ApprovalViewModel lfav = GetOwnerDtails(userLoginId);
                GetAdminDetails(lfav);
                ViewBag.FranchiseID = GetFranchiseID(userLoginId);
                ViewBag.ChargeStageID = new SelectList(db.ChargeStages, "ID", "Name");
                return View(lfav);
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[MerchantApprovalController][GET:Approve]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[MerchantApprovalController][GET:Approve]",
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
            }
            return View();
        }

        private void GetAdminDetails(ApprovalViewModel pfav)
        {
            try
            {
                var adminDetail = (from bt in db.BusinessTypes
                                   join bd in db.BusinessDetails on bt.ID equals bd.BusinessTypeID
                                   join p in db.PersonalDetails on bd.UserLoginID equals p.UserLoginID
                                   where bt.Prefix == "GBSA"
                                   select new { personalID = p.ID, businesstypeID = bt.ID }).FirstOrDefault();
                if (adminDetail != null)
                {
                    pfav.ToPersonalDetailID = adminDetail.personalID;
                    pfav.ToBusinessTypeID = adminDetail.businesstypeID;
                }
            }
            catch (Exception ex)
            {
                throw new BusinessLogicLayer.MyException("[GetAdminDetails]", "Can't Get Admin Details! in Method !" + Environment.NewLine + ex.Message);
            }
        }

        [HttpPost]
        [SessionExpire]
        [CustomAuthorize(Roles = "MerchantApproval/CanWrite")]
        public ActionResult Approve([Bind(Include = "ChargeStageID,ChargeID,Fees,FromName,UserLoginID,FromBusinessTypeID,FromPersonalDetailId,ToBusinessTypeID,ToPersonalDetailID,TransactionAmount,IsApproved")] ApprovalViewModel approval)
        {
            using (var dbContextTransaction = db.Database.BeginTransaction())
            {
                try
                {
                    ViewBag.ChargeStageID = new SelectList(db.ChargeStages, "ID", "Name");
                    if (approval.IsApproved)
                    {

                        if (CheckProductLimitOfFranchise(GetFranchiseID(approval.UserLoginID)))
                        {
                            UserLogin ul = db.UserLogins.Find(approval.UserLoginID);
                            if (ul == null)
                            {
                                return View("Error");
                            }

                            WriteToLogTable(ul, ModelLayer.Models.Enum.COMMAND.UPDATE);

                            ul.IsLocked = (!approval.IsApproved);
                            ul.ModifyDate = DateTime.UtcNow;
                            ul.ModifyBy = CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(Session["ID"]));
                            TryUpdateModel(ul);

                            InsertRole(ul.ID);

                            db.Entry(ul).State = EntityState.Modified;
                            db.SaveChanges();
                            dbContextTransaction.Commit();
                            ViewBag.ChargeStageID = new SelectList(db.ChargeStages, "ID", "Name", approval.ChargeStageID);
                            sendSMS(ul.ID);
                            sendEmail(ul.ID);
                            return RedirectToAction("Index", new { franchiseID = GetFranchiseID(ul.ID) });
                        }
                        else
                        {
                            ModelState.AddModelError("CustomError", "Sorry!! You can't proceed, Merchant approve limit has been exceeded...");
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("CustomError", "Please! Check 'Is Approved' for Approval..");
                    }
                    ApprovalViewModel lfav = GetOwnerDtails(approval.UserLoginID);
                    GetAdminDetails(lfav);
                    return View(lfav);
                }
                catch (BusinessLogicLayer.MyException myEx)
                {
                    BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                        + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                        + "[MerchantApprovalController][POST:Approve]" + myEx.EXCEPTION_PATH,
                        BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
                }
                catch (Exception ex)
                {
                    BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                        + Environment.NewLine + ex.Message + Environment.NewLine
                        + "[MerchantApprovalController][POST:Approve]",
                        BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
                }
            }
            return View();
        }

        private void InsertRole(long userLoginID)
        {
            try
            {
                int RoleID = 0;
                int.TryParse(db.Roles.Where(x => x.Name == "MERCHANT").Select(x => x.ID).FirstOrDefault().ToString(), out RoleID);
                if (db.UserRoles.Where(x => x.UserLoginID == userLoginID && x.RoleID == RoleID).Select(x => x.ID).Count() == 0)
                {
                    UserRole uRole = new UserRole();
                    uRole.UserLoginID = userLoginID;
                    uRole.RoleID = RoleID;
                    uRole.IsActive = true;
                    uRole.CreateBy = CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(Session["ID"]));
                    uRole.CreateDate = DateTime.UtcNow;
                    uRole.NetworkIP = CommonFunctions.GetClientIP();
                    //if (ModelState.IsValid)
                    {
                        db.UserRoles.Add(uRole);
                    }
                }
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                throw new BusinessLogicLayer.MyException("[InsertRole]", "Can't assign Role..!" + Environment.NewLine + myEx.Message);
            }
        }

        private int GetFranchiseID(long UID)
        {
            long BusinessDetailID = 0;
            long ShopID = 0;
            int FranchiseID = 0;
            try
            {
                if (UID > 0)
                {
                    BusinessDetailID = Convert.ToInt32(db.BusinessDetails.Where(x => x.UserLoginID == UID).Select(x => x.ID).First());
                    ShopID = Convert.ToInt32(db.Shops.Where(x => x.BusinessDetailID == BusinessDetailID).Select(x => x.ID).First());
                    FranchiseID = Convert.ToInt32(db.Shops.Where(x => x.ID == ShopID).Select(x => x.FranchiseID).First());
                }
            }
            catch (Exception ex)
            {
                throw new BusinessLogicLayer.MyException("[GetFranchiseID]", "Can't Get Franchise Details! in Method !" + Environment.NewLine + ex.Message);
            }
            return FranchiseID;
        }
        protected ApprovalViewModel GetOwnerDtails(long userLoginId)
        {
            ApprovalViewModel lfav = null;
            try
            {
                lfav = (from p in db.PersonalDetails
                        join bd in db.BusinessDetails on p.UserLoginID equals bd.UserLoginID
                        join bt in db.BusinessTypes on bd.BusinessTypeID equals bt.ID
                        join s in db.Shops on bd.ID equals s.BusinessDetailID
                        join op in db.OwnerPlans on s.ID equals op.OwnerID
                        join pl in db.Plans on op.PlanID equals pl.ID
                        where p.UserLoginID == userLoginId && bt.Prefix == "GBMR" && pl.PlanCode.StartsWith("GBMR")
                        select new ApprovalViewModel
                        {
                            UserLoginID = bd.UserLoginID,
                            //NoOfEntitiesAllowed = pl.NoOfEntitiesAllowed,
                            FromName = p.Salutation.Name + " " + p.FirstName + " " + p.LastName,
                            FromPersonalDetailId = p.ID,
                            FromBusinessTypeID = bd.BusinessTypeID,
                            Fees = pl.Fees
                        }).FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw new Exception("Can't Get Owner Dtails! in Method [GetOwnerDtails]" + ex.Message);
            }
            return (lfav);
        }

        public JsonResult GetChargeByChargeStageId(int chargeStageID)
        {
            try
            {
                var lCharge = (from c in db.Charges
                               where c.ChargeStageID == chargeStageID
                               select new
                               {
                                   c.ID,
                                   c.Name
                               }).ToList();
                return Json(lCharge, JsonRequestBehavior.AllowGet);
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[MerchantApprovalController][GET:GetChargeByChargeStageId]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[MerchantApprovalController][GET:GetChargeByChargeStageId]",
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
            }
            return Json(null, JsonRequestBehavior.AllowGet);
        }


        public JsonResult GetReceivedAmount(ApprovalViewModel myData)
        {
            try
            {
                decimal? recamount = 0;
                if (myData != null)
                {
                    recamount = (from gbt in db.GandhibaghTransactions
                                 where gbt.FromPersonalDetailId == myData.FromPersonalDetailId &&
                                        gbt.FromBusinessTypeID == myData.FromBusinessTypeID &&
                                        gbt.ToPersonalDetailID == myData.ToPersonalDetailID &&
                                        gbt.ToBusinessTypeID == myData.ToPersonalDetailID &&
                                        gbt.ChargeID == myData.ChargeID && gbt.IsActive == true
                                 select gbt.TransactionAmount).FirstOrDefault();

                    if (recamount == null)
                        recamount = 0;
                }
                return Json(recamount, JsonRequestBehavior.AllowGet);
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[MerchantApprovalController][GET:GetReceivedAmount]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[MerchantApprovalController][GET:GetReceivedAmount]",
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
            }
            return Json(0, JsonRequestBehavior.AllowGet);
        }

        public void sendEmail(long uid)
        {
            try
            {
                PersonalDetail lPD = db.PersonalDetails.Find(CommonFunctions.GetPersonalDetailsID(uid));
                string email = db.UserLogins.Find(uid).Email;
                // var merchantDetail= db.UserLogins.Find(uid);

                long merchantId = db.BusinessDetails.Where(x => x.UserLoginID == uid).FirstOrDefault().ID;

                string shopName = db.Shops.Where(x => x.BusinessDetailID == merchantId).FirstOrDefault().Name;

                // Sending email to the user
                BusinessLogicLayer.GateWay gateWay = new BusinessLogicLayer.Email(System.Web.HttpContext.Current.Server);
                string city = "nagpur";
                int franchiseID = 2;////added
                if (System.Web.HttpContext.Current.Request.Cookies["CityCookie"] != null && System.Web.HttpContext.Current.Request.Cookies["CityCookie"].Value != "")
                {
                    city = System.Web.HttpContext.Current.Request.Cookies["CityCookie"].Value.Split('$')[1].ToLower();
                    franchiseID = Convert.ToInt32(System.Web.HttpContext.Current.Request.Cookies["CityCookie"].Value.Split('$')[2]); ////added
                }

                Dictionary<string, string> emailParaMetres = new Dictionary<string, string>();


                emailParaMetres.Add("<!--ACCOUNT_URL-->", "" + (new URLsFromConfig()).GetURL("MERCHANT") + "");
                emailParaMetres.Add("<!--ORDERS_URL-->", "" + (new URLsFromConfig()).GetURL("MERCHANT") + "placed");
                emailParaMetres.Add("<!--NAME-->", lPD.FirstName);
                emailParaMetres.Add("<!--SHOP_NAME-->", shopName);
                emailParaMetres.Add("<!--MERCHANT_ID-->", merchantId.ToString());
                emailParaMetres.Add("<!--URL_ADD_PRODUCT-->", "" + (new URLsFromConfig()).GetURL("MERCHANT") + "ProductUploadTemp/Create");
                //emailParaMetres.Add("<!--USER_GUIDE-->", "" + (new URLsFromConfig()).GetURL("CUSTOMER") + "" + city + "/merchant-guide"); ////hide
                emailParaMetres.Add("<!--USER_GUIDE-->", "" + (new URLsFromConfig()).GetURL("CUSTOMER") + "" + city + "/" + franchiseID + "/merchant-guide"); ////added

                BusinessLogicLayer.ReadConfig rcKey = new ReadConfig(System.Web.HttpContext.Current.Server);

                //---Added by mohit on 23-01-16 for help line number as per city---//
                Dictionary<string, string> dictionary = new Dictionary<string, string>();
                List<CityHelpLineNoViewModel> lCitlHelpLine = new List<CityHelpLineNoViewModel>();
                lCitlHelpLine = BusinessLogicLayer.CityHelpLineNo.GetCityHelpLineNo(merchantId).ToList();
                emailParaMetres.Add("<!--HELP_DESK-->", lCitlHelpLine[0].HelpLineNumber);
                //---End Of Code By Mohit---//
                gateWay.SendEmail(BusinessLogicLayer.GateWay.EmailGateWays.GANDHIBAGH, BusinessLogicLayer.GateWay.SenderMail.INFO, BusinessLogicLayer.GateWay.EMailTypes.MER_APPROVED, new string[] { email, rcKey.DEFAULT_ALL_EMAIL }, emailParaMetres, true);

            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Message", "Merchant Approved Succesfully, there might be problem sending email, please check your email or contact administrator!");
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[MerchantApprovalController][sendEmail]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[MerchantApprovalController][sendEmail]",
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
            }
        }

        public void sendSMS(long uid)
        {
            try
            {
                PersonalDetail lPD = db.PersonalDetails.Find(CommonFunctions.GetPersonalDetailsID(uid));
                string mbno = db.UserLogins.Find(uid).Mobile;

                // Sending sms to the user
                BusinessLogicLayer.GateWay gateWay = new BusinessLogicLayer.SMS(System.Web.HttpContext.Current.Server);
                BusinessLogicLayer.ReadConfig rcKey = new ReadConfig(System.Web.HttpContext.Current.Server);

                Dictionary<string, string> smsValues = new Dictionary<string, string>();
                smsValues.Add("#--NAME--#", lPD.FirstName);

                //gateWay.SendSMS(BusinessLogicLayer.GateWay.SMSGateWays.SUMIT, BusinessLogicLayer.GateWay.SMSOptions.MULTIPLE, BusinessLogicLayer.GateWay.SMSTypes.MER_APRVD, new string[] { mbno,rcKey.DEFAULT_ALL_SMS }, smsValues);
                gateWay.SendSMS(BusinessLogicLayer.GateWay.SMSGateWays.SUMIT, BusinessLogicLayer.GateWay.SMSOptions.MULTIPLE, BusinessLogicLayer.GateWay.SMSTypes.MER_APRVD, new string[] { mbno }, smsValues);

            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Message", "Merchant Approved Succesfully, there might be problem sending sms, please check your email or contact administrator!");
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[MerchantApprovalController][sendSMS]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[MerchantApprovalController][sendSMS]",
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
            }
        }
        public void WriteToLogTable(UserLogin obj, ModelLayer.Models.Enum.COMMAND mode)
        {
            try
            {
                //Log Table Insertion
                //LogTable logTable = new LogTable();
                //logTable.TableName = "UserLogin";//table Name(Model Name)
                //logTable.RecordXML = ModelLayer.Models.ObjectToXml.GetXMLFromObject(obj);
                //logTable.TableRowID = obj.ID;
                //logTable.Command = mode.ToString();
                //long? rowOwnerID = (obj.ModifyBy >= 0 ? obj.ModifyBy : obj.CreateBy);
                //logTable.RowOwnerID = (long)rowOwnerID;
                //logTable.CreateDate = DateTime.UtcNow;
                //logTable.CreateBy = CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(Session["ID"]));//Session ID
                //db.LogTables.Add(logTable);
                /**************************************/
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                     + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                     + "[MerchantApprovalController][WriteToLogTable]" + myEx.EXCEPTION_PATH,
                     BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[MerchantApprovalController][WriteToLogTable]",
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
            }
        }

        private bool CheckProductLimitOfFranchise(long FranchiseId)
        {
            try
            {
                int merchantLimit = (from s in db.Franchises
                                     join o in db.OwnerPlans on s.ID equals o.OwnerID
                                     join p in db.Plans on o.PlanID equals p.ID
                                     where p.PlanCode.StartsWith("GBFR") && s.ID == FranchiseId
                                     select p.NoOfEntitiesAllowed).FirstOrDefault();
                int merchantUploadedinFranchise = db.Shops.Where(x => x.FranchiseID == FranchiseId).Select(x => x.ID).Count();

                if (merchantLimit > merchantUploadedinFranchise)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                throw new BusinessLogicLayer.MyException("[ProductApproval][CheckProductLimitOfMerchant]", "Can't Validate Values!" + Environment.NewLine + ex.Message);
            }
            return false;
        }
    }
}