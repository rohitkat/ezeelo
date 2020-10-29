using Administrator.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ModelLayer.Models;
using ModelLayer.Models.ViewModel;
using BusinessLogicLayer;
using System.Data.Entity;

namespace Administrator.Controllers
{
    public class DeliveryPartnerApprovalController : Controller
    {
        private EzeeloDBContext db = new EzeeloDBContext();

        //
        // GET: /DeliveryPartnerApproval/
        [SessionExpire]
        [CustomAuthorize(Roles = "DeliveryPartnerApproval/CanRead")]
        public ActionResult Index()
        {
            try
            {
                DeliveryPartnerApprovalViewModelList objDpList = new DeliveryPartnerApprovalViewModelList();
                var lDpav = (from ul in db.UserLogins
                             join bd in db.BusinessDetails on ul.ID equals bd.UserLoginID
                             join pd in db.PersonalDetails on ul.ID equals pd.UserLoginID
                             join dp in db.DeliveryPartners on bd.ID equals dp.BusinessDetailID
                             where bd.BusinessType.Prefix == "GBDP" && ul.IsLocked == true
                             select new DeliveryPartnerApprovalViewModel
                             {
                                 UserLoginID = bd.UserLoginID,
                                 BusinessTypePrefix = bd.BusinessType.Prefix,
                                 Name = pd.Salutation.Name + " " + pd.FirstName + " " + pd.LastName,
                                 OwnerId = dp.ID
                             }).ToList();

                objDpList.dpList = lDpav;
                return View(objDpList);
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[DeliveryPartnerApproval][GET:Index]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[DeliveryPartnerApproval][GET:Index]",
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
            }
            return View();
        }

        [HttpPost]
        [SessionExpire]
        [CustomAuthorize(Roles = "DeliveryPartnerApproval/CanWrite")]
        public ActionResult Approve(long? UID, long? ownerId)
        {
            using (var dbContextTransaction = db.Database.BeginTransaction())
            {
                try
                {
                    UserLogin ul = db.UserLogins.Find(UID);
                    WriteToLogTable(ul, ModelLayer.Models.Enum.COMMAND.UPDATE);
                    ul.IsLocked = false;
                    ul.ModifyDate = DateTime.UtcNow;
                    ul.ModifyBy = CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(Session["ID"]));
                    TryUpdateModel(ul);

                    DeliveryPartner dp = db.DeliveryPartners.Find(ownerId);
                    WriteToLogTable(dp, ModelLayer.Models.Enum.COMMAND.UPDATE);
                    dp.IsActive = true;
                    dp.IsLive = true;
                    dp.ModifyDate = DateTime.UtcNow;
                    dp.ModifyBy = CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(Session["ID"]));
                    TryUpdateModel(dp);

                    InsertRole(ul.ID);

                    if (ModelState.IsValid)
                    {
                        db.Entry(ul).State = EntityState.Modified;
                        db.SaveChanges();
                        dbContextTransaction.Commit();
                        sendSMS(ul.ID);
                        sendEmail(ul.ID);
                        return RedirectToAction("Index");
                    }
                }
                catch (BusinessLogicLayer.MyException myEx)
                {
                    BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                        + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                        + "[DeliveryPartnerApprovalController][POST:Index]" + myEx.EXCEPTION_PATH,
                        BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
                }
                catch (Exception ex)
                {
                    BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                        + Environment.NewLine + ex.Message + Environment.NewLine
                        + "[DeliveryPartnerApprovalController][POST:Index]",
                        BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
                }
            }
            return RedirectToAction("Index");
        }

        private void InsertRole(long userLoginID)
        {
            try
            {
                int RoleID = 0;
                int.TryParse(db.Roles.Where(x => x.Name == "DELIVERY").Select(x => x.ID).FirstOrDefault().ToString(), out RoleID);
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
                     + "[DeliveryPartnerApprovalController][WriteToLogTable:UserLogin]" + myEx.EXCEPTION_PATH,
                     BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[DeliveryPartnerApprovalController][WriteToLogTable:UserLogin]",
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
            }
        }

        public void WriteToLogTable(DeliveryPartner obj, ModelLayer.Models.Enum.COMMAND mode)
        {
            try
            {
                //Log Table Insertion
                //LogTable logTable = new LogTable();
                //logTable.TableName = "DeliveryPartner";//table Name(Model Name)
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
                     + "[DeliveryPartnerApprovalController][WriteToLogTable:DeliveryPartner]" + myEx.EXCEPTION_PATH,
                     BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[DeliveryPartnerApprovalController][WriteToLogTable:DeliveryPartner]",
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
            }
        }

        public void sendEmail(long uid)
        {
            try
            {
                PersonalDetail lPD = db.PersonalDetails.Find(CommonFunctions.GetPersonalDetailsID(uid));
                string email = db.UserLogins.Find(uid).Email;
                // var merchantDetail= db.UserLogins.Find(uid);

                // Sending email to the user
                BusinessLogicLayer.ReadConfig rcKey = new ReadConfig(System.Web.HttpContext.Current.Server);
                BusinessLogicLayer.GateWay gateWay = new BusinessLogicLayer.Email(System.Web.HttpContext.Current.Server);
                Dictionary<string, string> emailParaMetres = new Dictionary<string, string>();
                emailParaMetres.Add("<!--NAME-->", lPD.FirstName);
                //Start Yashaswi 14-11-2018
                URLsFromConfig objURLsFromConfig = new URLsFromConfig();
                string CustomerModuleUrl = objURLsFromConfig.GetURL("CUSTOMER");
                emailParaMetres.Add("<!--URL-->", CustomerModuleUrl);
                //End Yashaswi 14-11-2018

                gateWay.SendEmail(BusinessLogicLayer.GateWay.EmailGateWays.GANDHIBAGH, BusinessLogicLayer.GateWay.SenderMail.INFO, BusinessLogicLayer.GateWay.EMailTypes.DP_APPROVED, new string[] { email,rcKey.DEFAULT_ALL_EMAIL }, emailParaMetres, true);

            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Message", "Delivery Partner Approved Succesfully, there might be problem sending email, please check your email or contact administrator!");
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[DeliveryPartnerApprovalController][sendEmail]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[DeliveryPartnerApprovalController][sendEmail]",
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
            }
        }

        public void sendSMS(long uid)
        {
            try
            {
                PersonalDetail lPD = db.PersonalDetails.Find(CommonFunctions.GetPersonalDetailsID(uid));
                string mbno = db.UserLogins.Find(uid).Mobile;

                // Sending sms to the user
                BusinessLogicLayer.ReadConfig rcKey = new ReadConfig(System.Web.HttpContext.Current.Server);
                BusinessLogicLayer.GateWay gateWay = new BusinessLogicLayer.SMS(System.Web.HttpContext.Current.Server);

                Dictionary<string, string> otp = BusinessLogicLayer.OTP.GenerateOTP("MRG");

                Dictionary<string, string> smsValues = new Dictionary<string, string>();
                smsValues.Add("#--NAME--#", lPD.FirstName);

                //gateWay.SendSMS(BusinessLogicLayer.GateWay.SMSGateWays.SUMIT, BusinessLogicLayer.GateWay.SMSOptions.MULTIPLE, BusinessLogicLayer.GateWay.SMSTypes.DP_APRVD, new string[] { mbno,rcKey.DEFAULT_ALL_SMS }, smsValues);
                gateWay.SendSMS(BusinessLogicLayer.GateWay.SMSGateWays.SUMIT, BusinessLogicLayer.GateWay.SMSOptions.MULTIPLE, BusinessLogicLayer.GateWay.SMSTypes.DP_APRVD, new string[] { mbno }, smsValues);

            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Message", "Delivery Partner Approved Succesfully, there might be problem sending sms, please check your email or contact administrator!");
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[MerchantApprovalController][sendSMS]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[MerchantApprovalController][sendSMS]",
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
            }
        }
    }
}