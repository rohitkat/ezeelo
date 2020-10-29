using BusinessLogicLayer;
using Leaders.Controllers;
using Leaders.Filter;
using ModelLayer.Models;
using ModelLayer.Models.ViewModel;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;

namespace Leaders.Areas.Admin.Controllers
{
    [AdminSessionExpire]
    public class KYCDocumentController : Controller
    {

        // GET: Admin/KYCDocument
        private EzeeloDBContext db = new EzeeloDBContext();

        private LeadersDashboard objLeaderDashboard = new LeadersDashboard();
        // GET: Admin/KYCDocument
        public ActionResult Index(int? pageNumber, string option, string search)
        {
            CommonController obj_CommonController = new CommonController();
            //Yashaswi
            string AdharImagePath = obj_CommonController.GetFileNameAadhar("");
            string PANImagePath = obj_CommonController.GetFileNamePan("");
            string PassbookImagePath = obj_CommonController.GetFileNamePassbook("");
            string KYCImagePath = obj_CommonController.GetFileNameKYCForm("");

            List<LeadersProfileViewModel> mlmUserList = db.KYCModels.Where(k => k.KYCFormURL != null || k.PassbookImageUrl != null || k.PanImageUrl != null || k.AdhaarImageUrl != null)
                .Join(db.UserLogins, k => k.UserLoginID, u => u.ID, (k, u) => new { k.UserLoginID, u.Email, u.Mobile, k.AdhaarImageUrl, k.KYCFormURL, k.PanImageUrl, k.PassbookImageUrl, k.CreateDate, k.ModifyDate,k.IsVerified })
                .Join(db.PersonalDetails, k => k.UserLoginID, p => p.UserLoginID, (k, p) =>
                        new LeadersProfileViewModel
                        {
                            UserID = k.UserLoginID,
                            FullName = p.FirstName + " " + ((p.LastName == null) ? "" : p.LastName),
                            FirstName = p.FirstName,
                            Email = k.Email,
                            Mobile = k.Mobile,
                            AdhaarImageUrl = (k.AdhaarImageUrl == null) ? null : AdharImagePath + k.AdhaarImageUrl,
                            PANImage = (k.PanImageUrl == null) ? null : PANImagePath + k.PanImageUrl,
                            PassbookImage = (k.PassbookImageUrl == null) ? null : PassbookImagePath + k.PassbookImageUrl,
                            KYCForm = (k.KYCFormURL == null) ? null : KYCImagePath + k.KYCFormURL,
                            UpdateDate = (k.ModifyDate == null) ? k.CreateDate : k.ModifyDate,
                            IsVerified = k.IsVerified
                        }).OrderByDescending(k => k.UpdateDate).ToList();

            //Commented by yashaswi
            //List<LeadersProfileViewModel> mlmUserList = db.UserLogins.Join(db.MLMUsers, x => x.ID, y => y.UserID, (x, y) => new
            //{
            //    UserLoginID = y.UserID,
            //    EmailID = x.Email,
            //    Mobile = x.Mobile


            //}).Join(db.PersonalDetails, m => m.UserLoginID, p => p.UserLoginID, (m, p) => new
            //{
            //    FullName = p.FirstName + " " + p.LastName,
            //    UserID = m.UserLoginID,
            //    Email = m.EmailID,
            //    Mobile = m.Mobile,

            //}).Join(db.KYCModels.Where(k => k.KYCFormURL != null || k.PassbookImageUrl != null || k.PanImageUrl != null || k.AdhaarImageUrl != null), t => t.UserID, k => k.UserLoginID, (t, k) => new LeadersProfileViewModel
            //{
            //    AdhaarImageUrl = k.AdhaarImageUrl,
            //    PANImage = k.PanImageUrl,
            //    PassbookImage = k.PassbookImageUrl,
            //    FullName = t.FullName,
            //    UserID = t.UserID,
            //    Email = t.Email,
            //    Mobile = t.Mobile

            //}).ToList();

            //foreach (var item in mlmUserList)
            //{
            //    var adharURL = db.KYCModels.FirstOrDefault(k => k.UserLoginID == item.UserID).AdhaarImageUrl;
            //    var panURL = db.KYCModels.FirstOrDefault(k => k.UserLoginID == item.UserID).PanImageUrl;
            //    var passbookURL = db.KYCModels.FirstOrDefault(k => k.UserLoginID == item.UserID).PassbookImageUrl;
            //    var kycURL = db.KYCModels.FirstOrDefault(k => k.UserLoginID == item.UserID).KYCFormURL;


            //    if (adharURL != null)
            //    {
            //        item.AdhaarImageUrl = obj_CommonController.GetFileNameAadhar(adharURL);
            //    }
            //    else
            //    {
            //        item.AdhaarImageUrl = null;
            //    }
            //    if (panURL != null)
            //    {
            //        item.PANImage = obj_CommonController.GetFileNamePan(panURL);
            //    }
            //    else
            //    {
            //        item.PANImage = null;
            //    }

            //    if (passbookURL != null)
            //    {
            //        item.PassbookImage = obj_CommonController.GetFileNamePassbook(passbookURL);
            //    }
            //    else
            //    {
            //        item.PassbookImage = null;
            //    }

            //    if (kycURL != null)
            //    {
            //        item.KYCForm = obj_CommonController.GetFileNameKYCForm(kycURL);
            //    }
            //    else
            //    {
            //        item.KYCForm = null;
            //    }


            //}
            if (search != null)
            {
                search = search.ToLower().Trim();
            }
            if (option == "Name")
            {
                return View(mlmUserList.Where(x => x.FullName.ToLower().Contains(search)).ToPagedList(pageNumber ?? 1, 5));
            }
            else if (option == "Email")
            {
                return View(mlmUserList.Where(x => x.Email.ToLower().Contains(search)).ToPagedList(pageNumber ?? 1, 5));
            }
            else if (option == "Mobile")
            {
                return View(mlmUserList.Where(x => x.Mobile == search).ToPagedList(pageNumber ?? 1, 5));
            }

            return View(mlmUserList.ToPagedList(pageNumber ?? 1, 5));
        }


        public FileStreamResult Download(string fileName)
        {
            string Path = WebConfigurationManager.AppSettings["INVENTORY_ROOT_IMAGE_HTTP"] + WebConfigurationManager.AppSettings["DIRECTORY"] + "/" + WebConfigurationManager.AppSettings["FOLDER_PASSBOOK"] + "/";


            //string aURL = Path + fileName;
            Stream rtn = null;

            HttpWebRequest aRequest = (HttpWebRequest)WebRequest.Create(fileName);
            HttpWebResponse aResponse = (HttpWebResponse)aRequest.GetResponse();

            rtn = aResponse.GetResponseStream();

            return File(rtn, "image/jpeg", fileName);

        }

        public ActionResult Details(long UserLoginId)
        {
            try
            {
                if (Session["RoleName"].ToString() == "superadmin" || Session["RoleName"].ToString() == "accounts")
                {
                    //Function code copy from LeadersProfile index method
                    CommonController obj_CommonController = new CommonController();
                    long userID = UserLoginId;
                    var emailID = db.UserLogins.Where(x => x.ID == userID).Select(y => y.Email).FirstOrDefault();
                    LeadersProfileViewModel objProfile = new LeadersProfileViewModel();
                    long LoginUserId = 0;
                    if (Session["ID"] != null)
                    {
                        LoginUserId = Convert.ToInt64(Session["ID"]);
                    }
                    var idParam = new SqlParameter
                    {
                        ParameterName = "EmailID",
                        Value = emailID
                    };
                    KYCModel objKYC = db.KYCModels.Where(y => y.UserLoginID == userID).FirstOrDefault();
                    LeadersProfileViewModel profileObj = db.Database.SqlQuery<LeadersProfileViewModel>("EXEC Leaders_Profile_Select @EmailID", idParam).FirstOrDefault<LeadersProfileViewModel>();

                    if (objKYC != null)
                    {
                        profileObj.AdhaarImageUrl = obj_CommonController.GetFileNameAadhar(db.KYCModels.FirstOrDefault(p => p.UserLoginID == userID).AdhaarImageUrl);
                        profileObj.PANImage = obj_CommonController.GetFileNamePan(db.KYCModels.FirstOrDefault(p => p.UserLoginID == userID).PanImageUrl);
                        profileObj.PassbookImage = obj_CommonController.GetFileNamePassbook(db.KYCModels.FirstOrDefault(p => p.UserLoginID == userID).PassbookImageUrl);
                        profileObj.KYCForm = obj_CommonController.GetFileNameKYCForm(db.KYCModels.FirstOrDefault(x => x.UserLoginID == userID).KYCFormURL);

                        profileObj.BenificiaryName = objKYC.BenificiaryName;
                        profileObj.BenificiaryEmail = objKYC.BenificiaryEmail;
                    }
                    profileObj.SMSText = "Please complete your KYC form. Visit leaders.ezeelo.com";
                    profileObj.UserID = UserLoginId;
                    profileObj.IsVerified = objKYC.IsVerified;
                    return View(profileObj);
                }
                else
                {
                    return RedirectToAction("Index", "Home", new { area = "Admin" });
                }
            }
            catch
            {

            }
            return View();
        }

        [HttpPost]
        public ActionResult ApproveKYC(long UserID)
        {
            try
            {
                KYCModel objKYC = db.KYCModels.Where(y => y.UserLoginID == UserID).FirstOrDefault();
                objKYC.IsVerified = true;


                KYCApprovalLog approvalLog = new KYCApprovalLog();
                approvalLog.IsApproved = true;
                approvalLog.KYCID = objKYC.ID;
                approvalLog.NetworkIP = CommonFunctions.GetClientIP();
                approvalLog.UserLoginId = objKYC.UserLoginID;
                approvalLog.CreateDate = DateTime.Now;
                approvalLog.CreateBy = Convert.ToInt64(Session["ID"]);
                db.kYCApprovalLogs.Add(approvalLog);

                db.SaveChanges();

                TempData["Message"] = "KYC Approved successfully!";
                 
            }
            catch(Exception ex)
            {
                TempData["Message"] = "Something went wrong! " + ex.Message + " " + " " + ex.InnerException.Message;
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult CancelApprovalKYC(long UserID, string Remark)
        {
            try
            {
                KYCModel objKYC = db.KYCModels.Where(y => y.UserLoginID == UserID).FirstOrDefault();
                objKYC.IsVerified = false;
                if (string.IsNullOrEmpty(Remark))
                {
                    Remark = "Approval Cancelled";
                }
                KYCApprovalLog approvalLog = new KYCApprovalLog();
                approvalLog.IsApproved = false;
                approvalLog.KYCID = objKYC.ID;
                approvalLog.NetworkIP = CommonFunctions.GetClientIP();
                approvalLog.UserLoginId = objKYC.UserLoginID;
                approvalLog.Remark = Remark;
                approvalLog.CreateDate = DateTime.Now;
                approvalLog.CreateBy = Convert.ToInt64(Session["ID"]);
                db.kYCApprovalLogs.Add(approvalLog);

                db.SaveChanges();

                TempData["Message"] = "KYC Approval cancel successfully!";
            }
            catch (Exception ex)
            {
                TempData["Message"] = "Something went wrong! " + ex.Message;
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult SendSMS(long UserID, string SMSText)
        {
            try
            {
                if (SMSText.Trim() == null)
                {
                    SMSText = "Hello, Please complete your KYC form. visit leaders.ezeelo.com";
                }
                string Mobile = db.UserLogins.FirstOrDefault(p => p.ID == UserID).Mobile;
                //Mobile = "9588664085";
                string UserName = db.PersonalDetails.FirstOrDefault(p => p.UserLoginID == UserID).FirstName;
                Dictionary<string, string> dictSMSValues = new Dictionary<string, string>();
                dictSMSValues.Add("#--NAME--#", UserName);
                dictSMSValues.Add("#--TEXT--#", SMSText);

                BusinessLogicLayer.GateWay gateWay = new BusinessLogicLayer.SMS(System.Web.HttpContext.Current.Server);
                gateWay.SendSMS(BusinessLogicLayer.GateWay.SMSGateWays.SUMIT, BusinessLogicLayer.GateWay.SMSOptions.SINGLE, BusinessLogicLayer.GateWay.SMSTypes.KYC_CMPT_REQUEST, new string[] { Mobile }, dictSMSValues);
                TempData["Message"] = "SMS send successfully!";
            }
            catch(Exception ex)
            {
                TempData["Message"] = "Problem in sending SMS! " + ex.Message;
            }
            return RedirectToAction("Index");
        }
    }
}