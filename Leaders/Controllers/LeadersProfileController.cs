using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ModelLayer.Models;
using ModelLayer.Models.ViewModel;
using System.Data.SqlClient;
using System.IO;
using BusinessLogicLayer;
using Leaders.Common;
using Leaders.Filter;
using System.Web.Configuration;
using System.Security.Cryptography;

namespace Leaders.Controllers
{
    public class LeadersProfileController : Controller
    {
        private EzeeloDBContext db = new EzeeloDBContext();
        private LeadersDashboard objLeaderDashboard = new LeadersDashboard();//Added By Sonali for commonFunction on 04-02-2019

        [SessionExpire]
        public ActionResult Index()
        {

            CommonController obj_CommonController = new CommonController();

            long userID = Convert.ToInt64(Session["ID"]);

            var emailID = db.UserLogins.Where(x => x.ID == userID).Select(y => y.Email).FirstOrDefault();
            LeadersProfileViewModel objProfile = new LeadersProfileViewModel();
            //Started By Sonali for commonFunction on 04-02-2019
            long LoginUserId = 0;
            if (Session["ID"] != null)
            {
                LoginUserId = Convert.ToInt64(Session["ID"]);
            }
            //Ended By Sonali for commonFunction on 04-02-2019
            //DashboardController objDashboard = new DashboardController();//Added By Sonali for commonFunction on 04-02-2019
            //objDashboard.InitializeController(this.Request.RequestContext);//Added By Sonali for commonFunction on 04-02-2019

            var idParam = new SqlParameter
            {
                ParameterName = "EmailID",
                Value = emailID

            };
            KYCModel objKYC = db.KYCModels.Where(y => y.UserLoginID == userID).FirstOrDefault();
            if (objKYC != null)
            {
                LeadersProfileViewModel profileObj = db.Database.SqlQuery<LeadersProfileViewModel>("EXEC Leaders_Profile_Select @EmailID", idParam).FirstOrDefault<LeadersProfileViewModel>();
                profileObj.AdhaarImageUrl = obj_CommonController.GetFileNameAadhar(db.KYCModels.FirstOrDefault(p => p.UserLoginID == userID).AdhaarImageUrl);
                profileObj.PANImage = obj_CommonController.GetFileNamePan(db.KYCModels.FirstOrDefault(p => p.UserLoginID == userID).PanImageUrl);
                profileObj.PassbookImage = obj_CommonController.GetFileNamePassbook(db.KYCModels.FirstOrDefault(p => p.UserLoginID == userID).PassbookImageUrl);
                profileObj.KYCForm = obj_CommonController.GetFileNameKYCForm(db.KYCModels.FirstOrDefault(x => x.UserLoginID == userID).KYCFormURL);

                profileObj.BenificiaryName = objKYC.BenificiaryName; // on 13-2-19
                profileObj.BenificiaryEmail = objKYC.BenificiaryEmail;  // on 13-2-19

                // profileObj.AdhaarImageUrl = ImageDisplay.SetProductThumbPath(userID, "Default", string.Empty, ProductUpload.IMAGE_TYPE.Approved);

                profileObj.ERP = Convert.ToDecimal(objLeaderDashboard.getERP(LoginUserId)); //Added By Sonali for call commonFunction on 04-02-2019
               /* profileObj.TotalMember = objLeaderDashboard.GetTotalMemberCount(LoginUserId);*/ //Added By Sonali for call commonFunction on 04-02-2019
                profileObj.IsVerified = objKYC.IsVerified;
                return View(profileObj);
            }
            else
            {
                LeadersProfileViewModel profileObj = db.Database.SqlQuery<LeadersProfileViewModel>("EXEC Leaders_Profile_Select @EmailID", idParam).FirstOrDefault<LeadersProfileViewModel>();

                profileObj.ERP = Convert.ToDecimal(objLeaderDashboard.getERP(LoginUserId));//Added By Sonali for call commonFunction on 04-02-2019
               /* profileObj.TotalMember = objLeaderDashboard.GetTotalMemberCount(LoginUserId);*///Added By Sonali for call commonFunction on 04-02-2019
                profileObj.IsVerified = false;
                return View(profileObj);
            }
        }

        [HttpPost]
        public JsonResult AgeValidate(DateTime DOB)
        {
            return Json(IsValidAge(DOB));
        }

        public bool IsValidAge(DateTime dob)
        {
            bool status;
            DateTime currentDate = DateTime.Now;
            TimeSpan result = (currentDate - dob);
            if (result.Days <= 6574)
            {
                return false;
            }
            else
            {
                return true;
            }
        }


        [SessionExpire]
        [HttpGet]
        public ActionResult Update()
        {
            long userID = Convert.ToInt64(Session["ID"]);

            var emailID = db.UserLogins.Where(x => x.ID == userID).Select(y => y.Email).FirstOrDefault();
            LeadersProfileViewModel objProfile = new LeadersProfileViewModel();

            DashboardController objDashboard = new DashboardController();
            objDashboard.InitializeController(this.Request.RequestContext); // for getting ERP and total Members

            var idParam = new SqlParameter
            {
                ParameterName = "EmailID",
                Value = emailID

            };
            long LoginUserId = 0;
            if (Session["ID"] != null)
            {
                LoginUserId = Convert.ToInt64(Session["ID"]);
            }
            LeadersProfileViewModel profileObj = db.Database.SqlQuery<LeadersProfileViewModel>("EXEC Leaders_Profile_Select @EmailID", idParam).FirstOrDefault<LeadersProfileViewModel>();

            profileObj.ERP = Convert.ToDecimal(objLeaderDashboard.getERP(userID));//Added By Sonali for call commonFunction on 04-02-2019
           // profileObj.TotalMember = objLeaderDashboard.GetTotalMemberCount(userID);//Added By Sonali for call commonFunction on 04-02-2019
            return View(profileObj);

        }
        [SessionExpire]
        [HttpPost]
        public ActionResult Update(LeadersProfileViewModel collection, string Gender)
        {
            long userID = Convert.ToInt64(Session["ID"]);

            PersonalDetail objPersonel = db.PersonalDetails.Where(x => x.UserLoginID == userID).FirstOrDefault();
            UserLogin objUserLogin = db.UserLogins.Where(y => y.ID == userID).FirstOrDefault();
            KYCModel objKyc = db.KYCModels.Where(x => x.UserLoginID == userID).FirstOrDefault();

            var PincodeID = objPersonel.PincodeID;
            var checkPincode = db.Pincodes.Where(x => x.Name == collection.Pincode).Select(y => y.Name).FirstOrDefault();

            if (collection.Pincode != null && checkPincode != null)
            {
                objPersonel.PincodeID = db.Pincodes.Where(x => x.Name == collection.Pincode).Select(y => y.ID).FirstOrDefault();

                db.SaveChanges();

            }
            else
            {
                TempData["message"] = "Please Select Pincode From Dropdown Itself!!";

                return RedirectToAction("Update");

            }

            objPersonel.FirstName = collection.FirstName;
            objPersonel.MiddleName = collection.MiddelName;
            objPersonel.LastName = collection.LastName;

            objPersonel.DOB = Convert.ToDateTime(collection.DOB);
            objPersonel.Gender = Gender;
            objPersonel.Address = collection.Address;

            db.SaveChanges();

            TempData["Alert"] = "Your Data is Updated Successfuly";
            return RedirectToAction("Index");
        }
        [SessionExpire]
        public ActionResult UpdateBankDetails()
        {
            CommonController obj_CommonController = new CommonController();

            long userID = Convert.ToInt64(Session["ID"]);

            var emailID = db.UserLogins.Where(x => x.ID == userID).Select(y => y.Email).FirstOrDefault();
            LeadersProfileViewModel objProfile = new LeadersProfileViewModel();

            DashboardController objDashboard = new DashboardController();
            objDashboard.InitializeController(this.Request.RequestContext);

            var idParam = new SqlParameter
            {
                ParameterName = "EmailID",
                Value = emailID

            };
            KYCModel objKYC = db.KYCModels.Where(y => y.UserLoginID == userID).FirstOrDefault();
            if (objKYC != null)
            {
                LeadersProfileViewModel profileObj = db.Database.SqlQuery<LeadersProfileViewModel>("EXEC Leaders_Profile_Select @EmailID", idParam).FirstOrDefault<LeadersProfileViewModel>();
                profileObj.AdhaarImageUrl = obj_CommonController.GetFileNameAadhar(db.KYCModels.FirstOrDefault(p => p.UserLoginID == userID).AdhaarImageUrl);
                profileObj.PANImage = obj_CommonController.GetFileNamePan(db.KYCModels.FirstOrDefault(p => p.UserLoginID == userID).PanImageUrl);
                profileObj.PassbookImage = obj_CommonController.GetFileNamePassbook(db.KYCModels.FirstOrDefault(p => p.UserLoginID == userID).PassbookImageUrl);
                profileObj.KYCForm = obj_CommonController.GetFileNameKYCForm(db.KYCModels.FirstOrDefault(p => p.UserLoginID == userID).KYCFormURL);

                ViewBag.BankName = db.Banks.ToList();

                ViewBag.TabValue = "Update";

                profileObj.ERP = Convert.ToDecimal(objLeaderDashboard.getERP(userID));//Added by Sonali for call common function on 04-02-2018
                //profileObj.TotalMember = objLeaderDashboard.GetTotalMemberCount(userID);//Added by Sonali for call common function on 04-02-2018


                profileObj.BenificiaryEmail = objKYC.BenificiaryEmail;  // on 13-2-19
                profileObj.BenificiaryName = objKYC.BenificiaryName;  // on 13-2-19


                return View(profileObj);
            }
            else
            {
                LeadersProfileViewModel profileObj = db.Database.SqlQuery<LeadersProfileViewModel>("EXEC Leaders_Profile_Select @EmailID", idParam).FirstOrDefault<LeadersProfileViewModel>();
                ViewBag.BankName = db.Banks.ToList();

                ViewBag.TabValue = "Update";
                profileObj.ERP = Convert.ToDecimal(objLeaderDashboard.getERP(userID));//Added By Sonali for call commonFunction on 04-02-2019
               // profileObj.TotalMember = objLeaderDashboard.GetTotalMemberCount(userID);//Added By Sonali for call commonFunction on 04-02-2019

                return View(profileObj);
            }

        }

        [HttpPost]
        [SessionExpire]
        public ActionResult UpdateBankDetails(LeadersProfileViewModel collection, string AccountType, HttpPostedFileBase file)
        {
            long userID = Convert.ToInt64(Session["ID"]);

            KYCModel objKYC = db.KYCModels.Where(x => x.UserLoginID == userID).FirstOrDefault();

            if (objKYC == null)
            {
                KYCModel addKYC = new KYCModel();
                addKYC.UserLoginID = userID;
                addKYC.AccountNo = collection.AccountNumber;
                addKYC.AccountType = AccountType;
                addKYC.AdhaarNo = collection.AdharCardNo;
                addKYC.BankIFSC = collection.IFSC;
                addKYC.BankID = Convert.ToInt32(collection.BankID);
                addKYC.CreateDate = System.DateTime.Now;
                addKYC.PanNo = collection.PAN;
                addKYC.BranchName = collection.BranchName;
                addKYC.BenificiaryEmail = collection.BenificiaryEmail;
                addKYC.BenificiaryName = collection.BenificiaryName;

                db.KYCModels.Add(addKYC);
                db.SaveChanges();
                TempData["Alert"] = "Your Data is Submitted Successfuly";
                return RedirectToAction("Index");

            }


            else
            {
                objKYC.BankIFSC = collection.IFSC;
                objKYC.AdhaarNo = collection.AdharCardNo;
                objKYC.AccountNo = collection.AccountNumber;
                //objKYC.AccountType = collection.AccountType;
                objKYC.AccountType = AccountType;
                objKYC.PanNo = collection.PAN;
                objKYC.BankID = Convert.ToInt32(collection.BankID);

                objKYC.ModifyDate = System.DateTime.Now;
                objKYC.BranchName = collection.BranchName;
                objKYC.BenificiaryName = collection.BenificiaryName;
                objKYC.BenificiaryEmail = collection.BenificiaryEmail;

                db.SaveChanges();

                TempData["Alert"] = "Your Data is Update Successfuly";
                return RedirectToAction("Index");

            }


        }
        //--added on 13-2-19--//
        public FileResult DownloadKYC()
        {

            string path = Request.PhysicalApplicationPath + "Content";
            Stream rtn = null;

            var fileName = "Ezeelo-KYC.docx";

            var filepath = System.IO.Path.Combine(path, fileName);
            return File(filepath, MimeMapping.GetMimeMapping(filepath), fileName);


        }
        //end //

        public ActionResult GetPincode(string Prefix)
        {
            var PincodeList = (from N in db.Pincodes
                               where N.Name.StartsWith(Prefix)
                               select new { N.Name });
            return Json(PincodeList, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetCityDistrictState(string pincode)
        {
            var cityID = db.Pincodes.Where(x => x.Name == pincode).Select(y => y.CityID).FirstOrDefault();
            var city = db.Cities.Where(x => x.ID == cityID).Select(y => y.Name).FirstOrDefault();
            var districtID = db.Cities.Where(x => x.ID == cityID).Select(y => y.DistrictID).FirstOrDefault();
            var district = db.Districts.Where(x => x.ID == districtID).Select(y => y.Name).FirstOrDefault();
            var stateID = db.Districts.Where(x => x.ID == districtID).Select(y => y.StateID).FirstOrDefault();
            var state = db.States.Where(x => x.ID == stateID).Select(y => y.Name).FirstOrDefault();

            var result = new { CityName = city, districtName = district, stateName = state };
            return Json(result, JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        [SessionExpire]
        public ActionResult ImageUpload(LeadersProfileViewModel collection, HttpPostedFileBase adhaar, HttpPostedFileBase pan, HttpPostedFileBase kyc, HttpPostedFileBase passbook)
        {
            CommonController obj_CommonController = new CommonController();
            long userID = Convert.ToInt64(Session["ID"]);
            int lMaxContent = obj_CommonController.GetMaxImageSize();
            KYCModel objKYC = db.KYCModels.Where(x => x.UserLoginID == userID).FirstOrDefault();
            string ImageExtensionMsg = "Please choose only Image file ex: png, jpg, jpeg, raw ";
            string ImageSizeMsg = "Image size can not be greater than " + (lMaxContent / 1024).ToString() + " KB ";
            if (adhaar == null && pan == null && kyc == null && passbook == null)
            {
                TempData["message"] = "Please select Image file";
                return RedirectToAction("UpdateBankDetails");
            }
            if (objKYC != null)
            {
                var allowedExtensions = new[] {
             ".png", ".jpg", ".jpeg", ".raw", ".tiff", ".bmp"
            };
                bool IsSaved = false;
                string Filename = "";
                string Ext = "";
                if (adhaar != null)
                {
                    var fileNameAdhar = Path.GetFileName(adhaar.FileName);
                    var ExtAdhar = Path.GetExtension(adhaar.FileName);
                    string nameAdhar = Path.GetFileNameWithoutExtension(fileNameAdhar);
                    

                    if (allowedExtensions.Contains(ExtAdhar.ToLower())) //check what type of extension  
                    {
                        if (adhaar.ContentLength <= lMaxContent)     
                        {
                            IsSaved = obj_CommonController.UploadImage((int)Constant.Inventory_Image_Type.ADHAR, adhaar, (long)userID, System.Web.HttpContext.Current.Server, out Filename, out Ext);
                            string aadharPath = nameAdhar+ "_" + userID + Ext;
                            objKYC.AdhaarImageUrl = aadharPath;
                        }
                        else
                        {
                            TempData["message"] = ImageSizeMsg + " for Adhaar Card Image/ID Proof";
                            return RedirectToAction("UpdateBankDetails");
                        }
                    }
                    else
                    {
                        TempData["message"] = ImageExtensionMsg + " for Adhaar Card Image/ID Proof";
                        return RedirectToAction("UpdateBankDetails");
                    }

                }

                if (pan != null)
                {
                    var fileNamePAN = Path.GetFileName(pan.FileName);
                    var ExtPan = Path.GetExtension(pan.FileName);
                    string namePan = Path.GetFileNameWithoutExtension(fileNamePAN);//getting file name without extension  
                    
                    if (allowedExtensions.Contains(ExtPan.ToLower())) //check what type of extension  
                    {
                        if (pan.ContentLength <= lMaxContent)
                        {
                            IsSaved = obj_CommonController.UploadImage((int)Constant.Inventory_Image_Type.PAN, pan, (long)userID, System.Web.HttpContext.Current.Server, out Filename, out Ext);
                            string PANPath = namePan + "_" + userID + Ext;
                            objKYC.PanImageUrl = PANPath;
                        }
                        else
                        {
                            TempData["message"] = ImageSizeMsg + " for PAN Card Image";
                            return RedirectToAction("UpdateBankDetails");
                        }
                    }
                    else
                    {
                        TempData["message"] = ImageExtensionMsg + " for PAN Card Image"; 
                        return RedirectToAction("UpdateBankDetails");
                    }

                }
                if (passbook != null)
                {
                    var fileNamePassbook = Path.GetFileName(passbook.FileName);
                    var ExtPassbook = Path.GetExtension(passbook.FileName);
                    string namePassbook = Path.GetFileNameWithoutExtension(fileNamePassbook);
                    

                    if (allowedExtensions.Contains(ExtPassbook.ToLower())) //check what type of extension  
                    {
                        if (passbook.ContentLength <= lMaxContent)
                        {
                            IsSaved = obj_CommonController.UploadImage((int)Constant.Inventory_Image_Type.PASSBOOK, passbook, (long)userID, System.Web.HttpContext.Current.Server, out Filename, out Ext);
                            string passbookPath = namePassbook + "_" + userID + Ext;
                            objKYC.PassbookImageUrl = passbookPath;
                        }
                        else
                        {
                            TempData["message"] = ImageSizeMsg + " for Passbook Image/Cancel Cheque";
                            return RedirectToAction("UpdateBankDetails");
                        }
                    }
                    else
                    {
                        TempData["message"] = ImageExtensionMsg + " for Passbook Image/Cancel Cheque"; 
                        return RedirectToAction("UpdateBankDetails");
                    }


                }
                if (kyc != null)
                {
                    var fileNamekyc = Path.GetFileName(kyc.FileName);
                    var Extkyc = Path.GetExtension(kyc.FileName);
                    string namePassbook = Path.GetFileNameWithoutExtension(fileNamekyc);

                    if (allowedExtensions.Contains(Extkyc.ToLower())) //check what type of extension  
                    {
                        if (kyc.ContentLength <= lMaxContent)
                        {
                            IsSaved = obj_CommonController.UploadImage((int)Constant.Inventory_Image_Type.KYC, kyc, (long)userID, System.Web.HttpContext.Current.Server, out Filename, out Ext);
                            string kycPath = namePassbook + "_" + userID + Ext;
                            objKYC.KYCFormURL = kycPath;
                        }
                        else
                        {
                            TempData["message"] = ImageSizeMsg + " for KYC Scan Image";
                            return RedirectToAction("UpdateBankDetails");
                        }
                    }
                    else
                    {
                        TempData["message"] = ImageExtensionMsg + " for KYC Scan Image";
                        return RedirectToAction("UpdateBankDetails");
                    }


                }



                db.SaveChanges();
                TempData["Alert"] = "Your File is Uploaded Successfuly";
                return RedirectToAction("UpdateBankDetails");
            }
            else
            {
                TempData["message"] = "Please Insert Bank Details First";
                return RedirectToAction("UpdateBankDetails");
            }
        }

        [HttpPost]
        public ActionResult UploadProfile(string FileName)
        {
            for (int i = 0; i < Request.Files.Count; i++)
            {
                var file = Request.Files[i];

                // var fileName = Path.GetFileName(file.FileName);

                //var path = Path.Combine(Server.MapPath("~/App_Data/"), fileName);


                var fileName = Path.GetFileName(file.FileName);
                var ExtPassbook = Path.GetExtension(file.FileName);
                string namePassbook = Path.GetFileNameWithoutExtension(fileName);
                // string passbookPath = namePassbook + "_" + userID + ExtPassbook;

                //if (allowedExtensions.Contains(ExtPassbook)) //check what type of extension  
                //{
                //    IsSaved = obj_CommonController.UploadImage((int)Constant.Inventory_Image_Type.PASSBOOK, passbook, (long)userID, System.Web.HttpContext.Current.Server, out Filename, out Ext);
                //    objKYC.PassbookImageUrl = passbookPath;
                //}
                //else
                //{
                //    TempData["message"] = "Please choose only Image file";
                //    return RedirectToAction("UpdateBankDetails");
                //}
                file.SaveAs(fileName);
            }
            return View();
        }


    }
}
