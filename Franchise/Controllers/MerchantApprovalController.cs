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
using Franchise.Models;

namespace Franchise.Controllers
{
    [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
    public class MerchantApprovalController : Controller
    {
        private EzeeloDBContext db = new EzeeloDBContext();
        //
        // GET: /MerchantApproval/
        [SessionExpire]
        [CustomAuthorize(Roles = "MerchantApproval/CanRead")]
        public ActionResult Index()
        {
            try
            {
                long franchiseID = Convert.ToInt64(Session["FRANCHISE_ID"]);

                var lfav = (from ul in db.UserLogins
                            join bd in db.BusinessDetails on ul.ID equals bd.UserLoginID
                            join pd in db.PersonalDetails on ul.ID equals pd.UserLoginID
                            join s in db.Shops on bd.ID equals s.BusinessDetailID
                            join op in db.OwnerPlans on s.ID equals op.OwnerID
                            //join opc in db.OwnerPlanCategoryCharges on op.ID equals opc.OwnerPlanID
                            where bd.BusinessType.Prefix == "GBMR" && ul.IsLocked == true && s.FranchiseID == franchiseID &&
                            op.IsActive == true && op.Plan.PlanCode.StartsWith("GBMR")
                            select new MerchantPendingApprovalViewModel
                            {
                                UserLoginID = bd.UserLoginID,
                                BusinessTypePrefix = bd.BusinessType.Prefix,
                                Name = pd.Salutation.Name + " " + pd.FirstName + " " + pd.LastName,
                                OwnerId = s.ID,
                            }).Distinct().OrderBy(x => x.Name);

                return View(lfav);
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[MerchantApprovalController][GET:Index]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Franchise, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[MerchantApprovalController][GET:Index]",
                    BusinessLogicLayer.ErrorLog.Module.Franchise, System.Web.HttpContext.Current.Server);
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
                ViewBag.ChargeStageID = new SelectList(db.ChargeStages, "ID", "Name");
                return View(lfav);
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[MerchantApprovalController][GET:Approve]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Franchise, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[MerchantApprovalController][GET:Approve]",
                    BusinessLogicLayer.ErrorLog.Module.Franchise, System.Web.HttpContext.Current.Server);
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

        //private int GetFranchiseID()
        //{
        //    long BusinessDetailID = 0;
        //    long UID = Convert.ToInt64(Session["ID"]);
        //    int FranchiseID = 0;
        //    try
        //    {
        //        if (UID > 0)
        //        {
        //            BusinessDetailID = Convert.ToInt32(db.BusinessDetails.Where(x => x.UserLoginID == UID).Select(x => x.ID).First());
        //            FranchiseID = Convert.ToInt32(db.Franchises.Where(x => x.BusinessDetailID == BusinessDetailID).Select(x => x.ID).First());
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new BusinessLogicLayer.MyException("[GetFranchiseID]", "Can't Get Franchise Details! in Method !" + Environment.NewLine + ex.Message);
        //    }
        //    return FranchiseID;
        //}

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
                        UserLogin ul = db.UserLogins.Find(approval.UserLoginID);
                        if (ul == null)
                        {
                            return View("Error");
                        }

                        // WriteToLogTable(ul, ModelLayer.Models.Enum.COMMAND.UPDATE);

                        ul.IsLocked = (!approval.IsApproved);
                        ul.ModifyDate = DateTime.UtcNow;
                        ul.ModifyBy = CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(Session["ID"]));

                        //TryUpdateModel(ul);
                        if (ModelState.IsValid)
                        {
                            db.Entry(ul).State = EntityState.Modified;
                            db.SaveChanges();

                            InsertRole(ul.ID);

                            dbContextTransaction.Commit();
                            ViewBag.ChargeStageID = new SelectList(db.ChargeStages, "ID", "Name", approval.ChargeStageID);
                            sendSMS(ul.ID);
                            sendEmail(ul.ID);
                            return RedirectToAction("Edit", "MPersonalDetail", new { id = CommonFunctions.GetPersonalDetailsID(approval.UserLoginID) });
                        }
                        return RedirectToAction("Index");
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
                        BusinessLogicLayer.ErrorLog.Module.Franchise, System.Web.HttpContext.Current.Server);
                }
                catch (Exception ex)
                {
                    BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                        + Environment.NewLine + ex.Message + Environment.NewLine
                        + "[MerchantApprovalController][POST:Approve]",
                        BusinessLogicLayer.ErrorLog.Module.Franchise, System.Web.HttpContext.Current.Server);
                }
            }
            return View();
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
                    BusinessLogicLayer.ErrorLog.Module.Franchise, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[MerchantApprovalController][GET:GetChargeByChargeStageId]",
                    BusinessLogicLayer.ErrorLog.Module.Franchise, System.Web.HttpContext.Current.Server);
            }
            return Json(null, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetReceivedAmount(ApprovalViewModel myData)
        {
            decimal? recamount = 0;
            try
            {
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
                    BusinessLogicLayer.ErrorLog.Module.Franchise, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[MerchantApprovalController][GET:GetReceivedAmount]",
                    BusinessLogicLayer.ErrorLog.Module.Franchise, System.Web.HttpContext.Current.Server);
            }
            return Json(0, JsonRequestBehavior.AllowGet);
        }

        //public void WriteToLogTable(UserLogin obj, ModelLayer.Models.Enum.COMMAND mode)
        //{
        //    try
        //    {
        //        //Log Table Insertion
        //        LogTable logTable = new LogTable();
        //        logTable.TableName = "UserLogin";//table Name(Model Name)
        //        logTable.RecordXML = ModelLayer.Models.ObjectToXml.GetXMLFromObject(obj);
        //        logTable.TableRowID = obj.ID;
        //        logTable.Command = mode.ToString();
        //        long? rowOwnerID = (obj.ModifyBy >= 0 ? obj.ModifyBy : obj.CreateBy);
        //        logTable.RowOwnerID = (long)rowOwnerID;
        //        logTable.CreateDate = DateTime.UtcNow;
        //        logTable.CreateBy = CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(Session["ID"]));//Session ID
        //        db.LogTables.Add(logTable);
        //        /**************************************/
        //    }
        //    catch (BusinessLogicLayer.MyException myEx)
        //    {
        //        BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
        //             + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
        //             + "[MerchantApprovalController][WriteToLogTable]" + myEx.EXCEPTION_PATH,
        //             BusinessLogicLayer.ErrorLog.Module.Franchise, System.Web.HttpContext.Current.Server);
        //    }
        //    catch (Exception ex)
        //    {
        //        BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
        //            + Environment.NewLine + ex.Message + Environment.NewLine
        //            + "[MerchantApprovalController][WriteToLogTable]",
        //            BusinessLogicLayer.ErrorLog.Module.Franchise, System.Web.HttpContext.Current.Server);
        //    }
        //}

        private void InsertRole(long userLoginID)
        {
            try
            {
                int RoleID = 0;
                int.TryParse(db.Roles.Where(x => x.Name == "MERCHANT").Select(x => x.ID).FirstOrDefault().ToString(), out RoleID);
                if (db.UserRoles.Where(x => x.UserLoginID == userLoginID).Select(x => x.ID).Count() == 0)
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
                BusinessLogicLayer.ReadConfig rcKey = new BusinessLogicLayer.ReadConfig(System.Web.HttpContext.Current.Server);
                BusinessLogicLayer.GateWay gateWay = new BusinessLogicLayer.Email(System.Web.HttpContext.Current.Server);
                Dictionary<string, string> emailParaMetres = new Dictionary<string, string>();
                emailParaMetres.Add("<!--ACCOUNT_URL-->", "" + (new URLsFromConfig()).GetURL("MERCHANT") + "");
                emailParaMetres.Add("<!--ORDERS_URL-->", "" + (new URLsFromConfig()).GetURL("MERCHANT") + "");
                emailParaMetres.Add("<!--NAME-->", lPD.FirstName);
                emailParaMetres.Add("<!--SHOP_NAME-->", shopName);
                emailParaMetres.Add("<!--MERCHANT_ID-->", merchantId.ToString());
                emailParaMetres.Add("<!--URL_ADD_PRODUCT-->", "" + (new URLsFromConfig()).GetURL("MERCHANT") + "/ProductUploadTemp/Create");
                emailParaMetres.Add("<!--USER_GUIDE-->", "" + (new URLsFromConfig()).GetURL("CUSTOMER") + "home/Marchantguide");



                gateWay.SendEmail(BusinessLogicLayer.GateWay.EmailGateWays.GANDHIBAGH, BusinessLogicLayer.GateWay.SenderMail.INFO, BusinessLogicLayer.GateWay.EMailTypes.MER_APPROVED, new string[] { email,rcKey.DEFAULT_ALL_EMAIL }, emailParaMetres, true);

            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Message", "Merchant Approved Succesfully, there might be problem sending email, please check your email or contact administrator!");
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[MerchantApprovalController][sendEmail]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Franchise, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[MerchantApprovalController][sendEmail]",
                    BusinessLogicLayer.ErrorLog.Module.Franchise, System.Web.HttpContext.Current.Server);
            }
        }

        public void sendSMS(long uid)
        {
            try
            {
                PersonalDetail lPD = db.PersonalDetails.Find(CommonFunctions.GetPersonalDetailsID(uid));
                string mbno = db.UserLogins.Find(uid).Mobile;

                // Sending sms to the user
                BusinessLogicLayer.ReadConfig rcKey = new BusinessLogicLayer.ReadConfig(System.Web.HttpContext.Current.Server);
                BusinessLogicLayer.GateWay gateWay = new BusinessLogicLayer.SMS(System.Web.HttpContext.Current.Server);

                Dictionary<string, string> otp = BusinessLogicLayer.OTP.GenerateOTP("MRG");

                Dictionary<string, string> smsValues = new Dictionary<string, string>();
                smsValues.Add("#--NAME--#", lPD.FirstName);

                gateWay.SendSMS(BusinessLogicLayer.GateWay.SMSGateWays.SUMIT, BusinessLogicLayer.GateWay.SMSOptions.MULTIPLE, BusinessLogicLayer.GateWay.SMSTypes.MER_APRVD, new string[] { mbno, rcKey.DEFAULT_ALL_SMS }, smsValues);

            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Message", "Merchant Approved Succesfully, there might be problem sending sms, please check your mobile no. or contact administrator!");
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[MerchantApprovalController][sendSMS]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Franchise, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[MerchantApprovalController][sendSMS]",
                    BusinessLogicLayer.ErrorLog.Module.Franchise, System.Web.HttpContext.Current.Server);
            }
        }
    }
}