using API.Models;
using BusinessLogicLayer;
using ModelLayer.Models;
using ModelLayer.Models.ViewModel;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Http;

namespace API.Controllers
{
    public class SocialRegistrationController : ApiController
    {
        private EzeeloDBContext db = new EzeeloDBContext();
        [LoginSuccess]
        [ApiException]
        public object Post(SocialRegistrationViewModel model)
        {
            object obj = new object();
            try
            {
                if (!ModelState.IsValid)
                {
                    Request.Properties["UserLoginId"] = null;//Added by Sonali for authorization filter on 16-04-2019
                    return obj = new { Success = 0, Message = "Sorry! Please enter valid details.", data = string.Empty };
                }
                if (!Regex.IsMatch(model.MobileNo, @"^([5-9]{1}[0-9]{9})$"))//Sonali_24/10/2018
                {
                    Request.Properties["UserLoginId"] = null;//Added by Sonali for authorization filter on 16-04-2019
                    return obj = new { Success = 0, Message = "Enter valid MobileNo.", data = string.Empty };
                }

                db.Configuration.ProxyCreationEnabled = false;
                if (model.EmailId != null)
                {
                    if (model.EmailId.Trim().ToString().Equals(""))
                    {
                        model.EmailId = null;
                    }
                }

                ModelState.Clear();
                model.LastName = "  ";
                if (ModelState.IsValid)
                {
                    /* Started ReferralId validation on 28-11-2018*/
                    if (!string.IsNullOrEmpty(model.ReferralId))
                    {
                        bool chkReferralId = db.MLMUsers.Any(p => p.Ref_Id == model.ReferralId);
                        if (chkReferralId != true)
                        {
                            Request.Properties["UserLoginId"] = null;//Added by Sonali for authorization filter on 16-04-2019
                            return obj = new { Success = 0, Message = "Invalid ReferralId.", data = string.Empty };
                        }
                    }
                    /* Ended ReferralId validation on 28-11-2018*/

                    CustomerCredentials lCustCredentials = new CustomerCredentials();
                    if (model.SocialAccType == "Google" && !string.IsNullOrEmpty(model.SocialId))
                    {
                        var userExist = (from user in db.UserLogins
                                         join ps in db.PersonalDetails on user.ID equals ps.UserLoginID
                                         where (user.Email == model.EmailId || user.Mobile == model.MobileNo) && ps.Insta == model.SocialId
                                         select new
                                         {
                                             UserLoginID = user.ID,
                                             UserName = user.Email,
                                             EmailID = user.Email,
                                             MobileNo = user.Mobile,
                                             FirstName = ps.FirstName,
                                             LastName = ps.LastName
                                         }).FirstOrDefault();
                        if (userExist != null)
                        {
                            //foreach (var item in userExist)
                            //{
                            lCustCredentials.UserLoginID = userExist.UserLoginID;
                            if (userExist.EmailID != null)
                            {
                                lCustCredentials.UserName = userExist.EmailID.ToString();
                            }
                            lCustCredentials.MobileNo = userExist.MobileNo;
                            if (userExist.EmailID != null)
                            {
                                lCustCredentials.EmailID = userExist.EmailID.ToString();
                            }
                            lCustCredentials.FirstName = userExist.FirstName;
                            lCustCredentials.LastName = userExist.LastName;

                            // }
                            if (!string.IsNullOrEmpty(model.ReferralId))
                            {
                                bool IsRefer = ReferUser(userExist.UserLoginID, model);
                                string ReferralId = db.MLMUsers.Where(x => x.UserID == lCustCredentials.UserLoginID).Select(x => x.Ref_Id).FirstOrDefault();
                                if (IsRefer)
                                {
                                    Request.Properties["UserLoginId"] = lCustCredentials.UserLoginID;//Added by Sonali for authorization filter on 16-04-2019
                                    obj = new { Success = 1, Message = "You Have Successfully Become Ezeelo Member!", data = new { UserLoginID = lCustCredentials.UserLoginID, UserName = lCustCredentials.UserName, EmailID = lCustCredentials.EmailID, MobileNo = lCustCredentials.MobileNo, FirstName = lCustCredentials.FirstName, LastName = lCustCredentials.LastName, IsMLMUser = true, ReferralId = ReferralId } };
                                }
                                else
                                {
                                    Request.Properties["UserLoginId"] = lCustCredentials.UserLoginID;//Added by Sonali for authorization filter on 16-04-2019
                                    obj = new { Success = 1, Message = "You Are Already Registered As Ezeelo Member!", data = new { UserLoginID = lCustCredentials.UserLoginID, UserName = lCustCredentials.UserName, EmailID = lCustCredentials.EmailID, MobileNo = lCustCredentials.MobileNo, FirstName = lCustCredentials.FirstName, LastName = lCustCredentials.LastName, IsMLMUser = true } };
                                }
                            }
                            else
                            {
                                lCustCredentials.IsMLMUser = db.MLMUsers.Where(x => x.UserID == lCustCredentials.UserLoginID).Any();
                                string ReferralId = db.MLMUsers.Where(x => x.UserID == lCustCredentials.UserLoginID).Select(x => x.Ref_Id).FirstOrDefault();
                                Request.Properties["UserLoginId"] = lCustCredentials.UserLoginID;//Added by Sonali for authorization filter on 16-04-2019
                                obj = new { Success = 1, Message = "Login Successfull.", data = new { UserLoginID = lCustCredentials.UserLoginID, UserName = lCustCredentials.UserName, EmailID = lCustCredentials.EmailID, MobileNo = lCustCredentials.MobileNo, FirstName = lCustCredentials.FirstName, LastName = lCustCredentials.LastName, IsMLMUser = lCustCredentials.IsMLMUser, ReferralId = ReferralId } };
                            }

                        }
                        else
                        {
                            var lRole = db.Roles.Where(x => x.Name == "Customer".ToUpper()).ToList();

                            //if (lRole.Count() <= 0)
                            //{
                            //    return obj = new { Success = 0, Message = "Role not exist!!", data = string.Empty };
                            //}

                            try
                            {
                                string lMessage = string.Empty;
                                string mobEmail = string.Empty;
                                // Check user details exists or not
                                lMessage = CommonFunctions.CheckUserDetails(model.EmailId, model.MobileNo, out mobEmail);

                                if (lMessage != string.Empty)
                                {
                                    Request.Properties["UserLoginId"] = null;//Added by Sonali for authorization filter on 16-04-2019
                                    return obj = new { Success = 0, Message = lMessage, data = string.Empty };
                                }
                                int SendOTPValue = CommonFunctions.SendOTP(model.EmailId, model.MobileNo, model.FirstName,true,true);
                                if (SendOTPValue == 1)
                                {
                                    Request.Properties["UserLoginId"] = null;
                                    return obj = new { Success = 1, Message = "OTP sent to email and mobile.", data = new { StatusCode = 1 } };

                                }
                                else if (SendOTPValue == 2)
                                {
                                    Request.Properties["UserLoginId"] = null;
                                    return obj = new { Success = 1, Message = "OTP regenerate limit exceeds.", data = new { StatusCode = -1 } };

                                }
                                BusinessLogicLayer.ReadConfig rcKey = new BusinessLogicLayer.ReadConfig(System.Web.HttpContext.Current.Server);
                                // Creating user login
                                UserLogin userLogin = new UserLogin();
                                userLogin.ID = 0;
                                userLogin.Mobile = model.MobileNo;
                                userLogin.Email = model.EmailId;
                                userLogin.Password = rcKey.SOCIAL_PASSWORD;
                                userLogin.IsLocked = false;
                                userLogin.CreateBy = 1;
                                userLogin.CreateDate = DateTime.UtcNow.AddHours(5.5);
                                db.UserLogins.Add(userLogin);
                                db.SaveChanges();

                                // Getting current user login id
                                Int64 id = userLogin.ID;

                                // storing personal details
                                PersonalDetail personalDetail = new PersonalDetail();
                                personalDetail.ID = 0;
                                personalDetail.UserLoginID = id;
                                //personalDetail.SalutationID = model.ID;
                                personalDetail.SalutationID = 1;
                                personalDetail.FirstName = model.FirstName;
                                personalDetail.MiddleName = model.MiddleName;
                                personalDetail.LastName = model.LastName;
                                personalDetail.IsActive = true;
                                personalDetail.CreateBy = 1;
                                personalDetail.CreateDate = DateTime.UtcNow.AddHours(5.5);
                                personalDetail.Insta = model.SocialId;
                                db.PersonalDetails.Add(personalDetail);
                                db.SaveChanges();

                                UserRole userRole = new UserRole();
                                userRole.ID = 0;
                                userRole.RoleID = lRole.FirstOrDefault().ID;
                                userRole.UserLoginID = id;
                                userRole.IsActive = true;
                                userRole.CreateDate = DateTime.Now;
                                userRole.CreateBy = CommonFunctions.GetPersonalDetailsID(id);
                                db.UserRoles.Add(userRole);
                                db.SaveChanges();

                                ModelState.Clear();

                                if (db.ReferDetails.Any(m => (m.Email == model.EmailId || m.Mobile == model.MobileNo) && m.ReferenceID == null))
                                {
                                    ReferDetail lReferDetail = db.ReferDetails.Where(m => m.Email == model.EmailId || m.Mobile == model.MobileNo).FirstOrDefault();
                                    if (lReferDetail != null)
                                    {
                                        //Update referance id in referDetail table
                                        //i.e. Refer user now registered with eZeelo
                                        lReferDetail.ReferenceID = id;
                                        lReferDetail.ModifyDate = DateTime.Now;
                                        lReferDetail.ModifyBy = CommonFunctions.GetPersonalDetailsID(id);
                                        lReferDetail.NetworkIP = BusinessLogicLayer.CommonFunctions.GetClientIP();
                                        db.SaveChanges();
                                    }
                                    //Check if their is user wise earn 
                                    var earn = (from RES in db.ReferAndEarnSchemas
                                                join RD in db.ReferDetails on RES.ID equals RD.ReferAndEarnSchemaID
                                                where RES.UserwiseEarn == true && RD.ID == lReferDetail.ID
                                                select new { EarnMoney = RES.EarnInRS }).FirstOrDefault();
                                    //If user wise earn then add earn money to refer by user account
                                    if (earn != null)
                                    {
                                        decimal EarnRs = Convert.ToDecimal(earn.EarnMoney);

                                        var PrevRemainingAmt = db.EarnDetails.OrderByDescending(u => u.ID).Where(x => x.EarnUID == lReferDetail.UserID).Select(x => x.RemainingAmount).FirstOrDefault();

                                        EarnDetail lEarnDetail = new EarnDetail();
                                        lEarnDetail.EarnUID = lReferDetail.UserID;
                                        lEarnDetail.ReferUID = lReferDetail.ReferenceID;
                                        //lEarnDetail.EarnAmount = earn.FirstOrDefault().EarnMoney;
                                        lEarnDetail.EarnAmount = EarnRs;
                                        lEarnDetail.UsedAmount = 0;
                                        if (PrevRemainingAmt != null)
                                        {
                                            lEarnDetail.RemainingAmount = PrevRemainingAmt + EarnRs;
                                        }
                                        else
                                        {
                                            lEarnDetail.RemainingAmount = EarnRs;
                                        }
                                        lEarnDetail.CustomerOrderID = null;
                                        lEarnDetail.CreateDate = DateTime.Now;
                                        lEarnDetail.CreateBy = CommonFunctions.GetPersonalDetailsID(id);
                                        lEarnDetail.NetworkIP = BusinessLogicLayer.CommonFunctions.GetClientIP();
                                        db.EarnDetails.Add(lEarnDetail);
                                        db.SaveChanges();
                                    }

                                }
                                if (!string.IsNullOrEmpty(model.ReferralId))
                                {
                                    bool IsRefer = ReferUser(id, model);
                                    string ReferralId = db.MLMUsers.Where(x => x.UserID == lCustCredentials.UserLoginID).Select(x => x.Ref_Id).FirstOrDefault();
                                    if (IsRefer)
                                    {
                                        Request.Properties["UserLoginId"] = lCustCredentials.UserLoginID;//Added by Sonali for authorization filter on 16-04-2019
                                        obj = new { Success = 1, Message = "You Have Successfully Become Ezeelo Member!", data = new { UserLoginID = userLogin.ID, UserName = personalDetail.FirstName, EmailID = userLogin.Email, MobileNo = userLogin.Mobile, FirstName = personalDetail.FirstName, LastName = personalDetail.LastName, IsMLMUser = true, ReferralId = ReferralId } };
                                    }
                                    else
                                    {
                                        Request.Properties["UserLoginId"] = lCustCredentials.UserLoginID;//Added by Sonali for authorization filter on 16-04-2019
                                        obj = new { Success = 1, Message = "You Are Already Registered As Ezeelo Member!", data = new { UserLoginID = userLogin.ID, UserName = personalDetail.FirstName, EmailID = userLogin.Email, MobileNo = userLogin.Mobile, FirstName = personalDetail.FirstName, LastName = personalDetail.LastName, IsMLMUser = true } };
                                    }
                                }
                                else
                                {
                                    lCustCredentials.IsMLMUser = db.MLMUsers.Where(x => x.UserID == userLogin.ID).Any();
                                    string ReferralId = db.MLMUsers.Where(x => x.UserID == lCustCredentials.UserLoginID).Select(x => x.Ref_Id).FirstOrDefault();
                                    Request.Properties["UserLoginId"] = lCustCredentials.UserLoginID;//Added by Sonali for authorization filter on 16-04-2019
                                    obj = new { Success = 1, Message = "Done! Registration Successfully Done!!", data = new { UserLoginID = userLogin.ID, UserName = personalDetail.FirstName, EmailID = userLogin.Email, MobileNo = userLogin.Mobile, FirstName = personalDetail.FirstName, LastName = personalDetail.LastName, IsMLMUser = lCustCredentials.IsMLMUser, ReferralId = ReferralId } };
                                }

                                //ViewBag.Message = "Done! Registration Successfully Done!!";
                                //obj = new { Success = 1, Message = "Done! Registration Successfully Done!!", data = new { UserLoginID = userLogin.ID, UserName = personalDetail.FirstName, EmailID = userLogin.Email, MobileNo = userLogin.Mobile } };
                                // new { HTTPStatusCode = "200 ", UserMessage = "Done! Registration Successfully Done!!", UserLoginID = id, UserName = personalDetail.FirstName, EmailID = userLogin.Email, MobileNo = userLogin.Mobile };
                                //Session["UID"] = id;
                                model = null;

                                //4) Return to user profile page

                            }
                            catch (Exception exception)
                            {
                                Request.Properties["UserLoginId"] = null;//Added by Sonali for authorization filter on 16-04-2019
                                obj = new { Success = 0, Message = "Sorry! Problem in customer registration!!", data = string.Empty };
                            }
                        }
                    }
                    else if (model.SocialAccType == "Facebook" && !string.IsNullOrEmpty(model.SocialId))
                    {
                        var userExist = (from user in db.UserLogins
                                         join ps in db.PersonalDetails on user.ID equals ps.UserLoginID
                                         where (user.Email == model.EmailId || user.Mobile == model.MobileNo) && ps.Facebook == model.SocialId
                                         select new
                                         {
                                             UserLoginID = user.ID,
                                             UserName = user.Email,
                                             EmailID = user.Email,
                                             MobileNo = user.Mobile,
                                             FirstName = ps.FirstName,
                                             LastName = ps.LastName
                                         }).FirstOrDefault();
                        //if(userExist != null)
                        //{

                        //}
                        //var userExist = (from user in db.UserLogins
                        //                 join ps in db.PersonalDetails on user.ID equals ps.UserLoginID
                        //                 where (user.Email == model.EmailId || user.Mobile == model.MobileNo) && ps.Facebook == model.SocialId
                        //                 select new
                        //                 {
                        //                     UserLoginID = user.ID,
                        //                     UserName = user.Email,
                        //                     EmailID = user.Email,
                        //                     MobileNo = user.Mobile,
                        //                     FirstName = ps.FirstName,
                        //                     LastName = ps.LastName
                        //                 }).ToList();
                        if (userExist != null)
                        {
                            //foreach (var item in userExist)
                            //{
                            lCustCredentials.UserLoginID = userExist.UserLoginID;
                            if (userExist.EmailID != null)
                            {
                                lCustCredentials.UserName = userExist.EmailID.ToString();
                            }
                            lCustCredentials.MobileNo = userExist.MobileNo;
                            if (userExist.EmailID != null)
                            {
                                lCustCredentials.EmailID = userExist.EmailID.ToString();
                            }
                            lCustCredentials.FirstName = userExist.FirstName;
                            lCustCredentials.LastName = userExist.LastName;
                            // }
                            if (!string.IsNullOrEmpty(model.ReferralId))
                            {
                                bool IsRefer = ReferUser(userExist.UserLoginID, model);
                                string ReferralId = db.MLMUsers.Where(x => x.UserID == lCustCredentials.UserLoginID).Select(x => x.Ref_Id).FirstOrDefault();
                                if (IsRefer)
                                {
                                    Request.Properties["UserLoginId"] = lCustCredentials.UserLoginID;//Added by Sonali for authorization filter on 16-04-2019
                                    obj = new { Success = 1, Message = "You Have Successfully Become Ezeelo Member!", data = new { UserLoginID = lCustCredentials.UserLoginID, UserName = lCustCredentials.UserName, EmailID = lCustCredentials.EmailID, MobileNo = lCustCredentials.MobileNo, FirstName = lCustCredentials.FirstName, LastName = lCustCredentials.LastName, IsMLMUser = true, ReferralId = ReferralId } };
                                }
                                else
                                {
                                    Request.Properties["UserLoginId"] = lCustCredentials.UserLoginID;//Added by Sonali for authorization filter on 16-04-2019
                                    obj = new { Success = 1, Message = "You Are Already Registered As Ezeelo Member!", data = new { UserLoginID = lCustCredentials.UserLoginID, UserName = lCustCredentials.UserName, EmailID = lCustCredentials.EmailID, MobileNo = lCustCredentials.MobileNo, FirstName = lCustCredentials.FirstName, LastName = lCustCredentials.LastName, IsMLMUser = true } };
                                }
                            }
                            else
                            {
                                lCustCredentials.IsMLMUser = db.MLMUsers.Where(x => x.UserID == lCustCredentials.UserLoginID).Any();
                                string ReferralId = db.MLMUsers.Where(x => x.UserID == lCustCredentials.UserLoginID).Select(x => x.Ref_Id).FirstOrDefault();
                                Request.Properties["UserLoginId"] = lCustCredentials.UserLoginID;//Added by Sonali for authorization filter on 16-04-2019
                                obj = new { Success = 1, Message = "Login Successfull.", data = new { UserLoginID = lCustCredentials.UserLoginID, UserName = lCustCredentials.UserName, EmailID = lCustCredentials.EmailID, MobileNo = lCustCredentials.MobileNo, FirstName = lCustCredentials.FirstName, LastName = lCustCredentials.LastName, IsMLMUser = lCustCredentials.IsMLMUser, ReferralId = ReferralId } };
                            }

                        }
                        else
                        {
                            var lRole = db.Roles.Where(x => x.Name == "Customer".ToUpper()).ToList();

                            //if (lRole.Count() <= 0)
                            //{
                            //    return obj = new { Success = 0, Message = "Role not exist!!", data = string.Empty };
                            //}

                            try
                            {
                                string lMessage = string.Empty;
                                string mobEmail = string.Empty;
                                // Check user details exists or not
                                lMessage = CommonFunctions.CheckUserDetails(model.EmailId, model.MobileNo, out mobEmail);

                                if (lMessage != string.Empty)
                                {
                                    Request.Properties["UserLoginId"] = null;//Added by Sonali for authorization filter on 16-04-2019
                                    return obj = new { Success = 0, Message = lMessage, data = string.Empty };
                                }
                                int SendOTPValue = CommonFunctions.SendOTP(model.EmailId, model.MobileNo, model.FirstName,true,true);
                                if (SendOTPValue == 1)
                                {
                                    Request.Properties["UserLoginId"] = null;
                                    return obj = new { Success = 1, Message = "OTP sent to email and mobile.", data = new { StatusCode = 1 } };

                                }
                                else if (SendOTPValue == 2 || SendOTPValue==-1)
                                {
                                    Request.Properties["UserLoginId"] = null;
                                    return obj = new { Success = 1, Message = "OTP regenerate limit exceeds.", data = new { StatusCode = -1 } };

                                }
                                BusinessLogicLayer.ReadConfig rcKey = new BusinessLogicLayer.ReadConfig(System.Web.HttpContext.Current.Server);
                                // Creating user login
                                UserLogin userLogin = new UserLogin();
                                userLogin.ID = 0;
                                userLogin.Mobile = model.MobileNo;
                                userLogin.Email = model.EmailId;
                                userLogin.Password = rcKey.SOCIAL_PASSWORD;
                                userLogin.IsLocked = false;
                                userLogin.CreateBy = 1;
                                userLogin.CreateDate = DateTime.UtcNow.AddHours(5.5);
                                db.UserLogins.Add(userLogin);
                                db.SaveChanges();

                                // Getting current user login id
                                Int64 id = userLogin.ID;

                                // storing personal details
                                PersonalDetail personalDetail = new PersonalDetail();
                                personalDetail.ID = 0;
                                personalDetail.UserLoginID = id;
                                //personalDetail.SalutationID = model.ID;
                                personalDetail.SalutationID = 1;
                                personalDetail.FirstName = model.FirstName;
                                personalDetail.MiddleName = model.MiddleName;
                                personalDetail.LastName = model.LastName;
                                personalDetail.IsActive = true;
                                personalDetail.CreateBy = 1;
                                personalDetail.CreateDate = DateTime.UtcNow.AddHours(5.5);
                                personalDetail.Facebook = model.SocialId;
                                db.PersonalDetails.Add(personalDetail);
                                db.SaveChanges();

                                UserRole userRole = new UserRole();
                                userRole.ID = 0;
                                userRole.RoleID = lRole.FirstOrDefault().ID;
                                userRole.UserLoginID = id;
                                userRole.IsActive = true;
                                userRole.CreateDate = DateTime.Now;
                                userRole.CreateBy = CommonFunctions.GetPersonalDetailsID(id);
                                db.UserRoles.Add(userRole);
                                db.SaveChanges();

                                ModelState.Clear();
                                if (db.ReferDetails.Any(m => (m.Email == model.EmailId || m.Mobile == model.MobileNo) && m.ReferenceID == null))
                                {
                                    ReferDetail lReferDetail = db.ReferDetails.Where(m => m.Email == model.EmailId || m.Mobile == model.MobileNo).FirstOrDefault();
                                    if (lReferDetail != null)
                                    {
                                        //Update referance id in referDetail table
                                        //i.e. Refer user now registered with eZeelo
                                        lReferDetail.ReferenceID = id;
                                        lReferDetail.ModifyDate = DateTime.Now;
                                        lReferDetail.ModifyBy = CommonFunctions.GetPersonalDetailsID(id);
                                        lReferDetail.NetworkIP = BusinessLogicLayer.CommonFunctions.GetClientIP();
                                        db.SaveChanges();
                                    }
                                    //Check if their is user wise earn 
                                    var earn = (from RES in db.ReferAndEarnSchemas
                                                join RD in db.ReferDetails on RES.ID equals RD.ReferAndEarnSchemaID
                                                where RES.UserwiseEarn == true && RD.ID == lReferDetail.ID
                                                select new { EarnMoney = RES.EarnInRS }).FirstOrDefault();
                                    //If user wise earn then add earn money to refer by user account
                                    if (earn != null)
                                    {
                                        decimal EarnRs = Convert.ToDecimal(earn.EarnMoney);

                                        var PrevRemainingAmt = db.EarnDetails.OrderByDescending(u => u.ID).Where(x => x.EarnUID == lReferDetail.UserID).Select(x => x.RemainingAmount).FirstOrDefault();

                                        EarnDetail lEarnDetail = new EarnDetail();
                                        lEarnDetail.EarnUID = lReferDetail.UserID;
                                        lEarnDetail.ReferUID = lReferDetail.ReferenceID;
                                        //lEarnDetail.EarnAmount = earn.FirstOrDefault().EarnMoney;
                                        lEarnDetail.EarnAmount = EarnRs;
                                        lEarnDetail.UsedAmount = 0;
                                        if (PrevRemainingAmt != null)
                                        {
                                            lEarnDetail.RemainingAmount = PrevRemainingAmt + EarnRs;
                                        }
                                        else
                                        {
                                            lEarnDetail.RemainingAmount = EarnRs;
                                        }
                                        lEarnDetail.CustomerOrderID = null;
                                        lEarnDetail.CreateDate = DateTime.Now;
                                        lEarnDetail.CreateBy = CommonFunctions.GetPersonalDetailsID(id);
                                        lEarnDetail.NetworkIP = BusinessLogicLayer.CommonFunctions.GetClientIP();
                                        db.EarnDetails.Add(lEarnDetail);
                                        db.SaveChanges();
                                    }

                                }
                                if (!string.IsNullOrEmpty(model.ReferralId))
                                {
                                    bool IsRefer = ReferUser(id, model);
                                    string ReferralId = db.MLMUsers.Where(x => x.UserID == lCustCredentials.UserLoginID).Select(x => x.Ref_Id).FirstOrDefault();
                                    if (IsRefer)
                                    {
                                        Request.Properties["UserLoginId"] = lCustCredentials.UserLoginID;//Added by Sonali for authorization filter on 16-04-2019
                                        obj = new { Success = 1, Message = "You Have Successfully Become Ezeelo Member!", data = new { UserLoginID = userLogin.ID, UserName = personalDetail.FirstName, EmailID = userLogin.Email, MobileNo = userLogin.Mobile, FirstName = personalDetail.FirstName, LastName = personalDetail.LastName, IsMLMUser = true, ReferralId = ReferralId } };
                                    }
                                    else
                                    {
                                        Request.Properties["UserLoginId"] = lCustCredentials.UserLoginID;//Added by Sonali for authorization filter on 16-04-2019
                                        obj = new { Success = 1, Message = "You Are Already Registered As Ezeelo Member!", data = new { UserLoginID = userLogin.ID, UserName = personalDetail.FirstName, EmailID = userLogin.Email, MobileNo = userLogin.Mobile, FirstName = personalDetail.FirstName, LastName = personalDetail.LastName, IsMLMUser = true } };
                                    }
                                }
                                else
                                {
                                    lCustCredentials.IsMLMUser = db.MLMUsers.Where(x => x.UserID == userLogin.ID).Any();
                                    string ReferralId = db.MLMUsers.Where(x => x.UserID == lCustCredentials.UserLoginID).Select(x => x.Ref_Id).FirstOrDefault();
                                    Request.Properties["UserLoginId"] = lCustCredentials.UserLoginID;//Added by Sonali for authorization filter on 16-04-2019
                                    obj = new { Success = 1, Message = "Done! Registration Successfully Done!!", data = new { UserLoginID = userLogin.ID, UserName = personalDetail.FirstName, EmailID = userLogin.Email, MobileNo = userLogin.Mobile, FirstName = personalDetail.FirstName, LastName = personalDetail.LastName, IsMLMUser = lCustCredentials.IsMLMUser, ReferralId = ReferralId } };
                                }

                                //ViewBag.Message = "Done! Registration Successfully Done!!";
                                // obj = new { Success = 1, Message = "Done! Registration Successfully Done!!", data = new { UserLoginID = userLogin.ID, UserName = personalDetail.FirstName, EmailID = userLogin.Email, MobileNo = userLogin.Mobile } };
                                // new { HTTPStatusCode = "200 ", UserMessage = "Done! Registration Successfully Done!!", UserLoginID = id, UserName = personalDetail.FirstName, EmailID = userLogin.Email, MobileNo = userLogin.Mobile };
                                //Session["UID"] = id;
                                model = null;

                                //4) Return to user profile page

                            }
                            catch (Exception exception)
                            {
                                Request.Properties["UserLoginId"] = null;//Added by Sonali for authorization filter on 16-04-2019
                                obj = new { Success = 0, Message = "Sorry! Problem in customer registration!!", data = string.Empty };
                            }
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                Request.Properties["UserLoginId"] = null;//Added by Sonali for authorization filter on 16-04-2019
                obj = new { Success = 0, Message = ex.Message, data = string.Empty };
            }
            return obj;
        }

        private bool ReferUser(long id, SocialRegistrationViewModel model)
        {
            bool IsRefer = false;
            try
            {
                MLMWalletPoints MLMWallet = new MLMWalletPoints();
                //long LoginUserId = id;
                //UserLogin userLog = db.UserLogins.FirstOrDefault(p => p.ID == LoginUserId);
                BusinessLogicLayer.ReadConfig rcKey = new BusinessLogicLayer.ReadConfig(System.Web.HttpContext.Current.Server);
                string result = MLMWallet.LeadersSingUp(id, rcKey.SOCIAL_PASSWORD, model.EmailId, model.ReferralId);
                string UserName = "";
                if (result.Contains("R_DONE"))
                {
                    try
                    {
                        string RefId = model.ReferralId;
                        UserName = " under " + db.PersonalDetails.FirstOrDefault(p => p.UserLoginID == (db.MLMUsers.FirstOrDefault(m => m.Ref_Id == RefId).UserID)).FirstName;
                    }
                    catch
                    {

                    }
                    IsRefer = true;
                }
                else if (result.Contains("ALREADY_R"))
                {
                    try
                    {
                        string RefId = db.MLMUsers.FirstOrDefault(p => p.UserID == id).Refered_Id_ref;
                        UserName = " under " + db.PersonalDetails.FirstOrDefault(p => p.UserLoginID == (db.MLMUsers.FirstOrDefault(m => m.Ref_Id == RefId).UserID)).FirstName;
                    }
                    catch
                    {

                    }
                    IsRefer = false;
                    //return obj = new { Success = 0, Message = "You Are Already Registered As Ezeelo Member" + UserName, data = new { UserLoginID = userLogin.ID, UserName = UserName } };
                    //  new { HTTPStatusCode = "400", UserMessage = "You Are Already Registered As Ezeelo Member" + UserName, UserLoginID = userLogin.ID, UserName = UserName };
                }
            }
            catch (Exception ex)
            {

            }
            return IsRefer;
        }


        [LoginSuccess]
        [ApiException]
        [Route("api/SocialRegistration/SocialRegistrationDetail")]
        [HttpGet]
        public object SocialRegistrationDetail(string SocialId, string SocialAccType)
        {
            object obj = new object();
            try
            {
                if (string.IsNullOrEmpty(SocialId) || string.IsNullOrEmpty(SocialAccType))
                {
                    Request.Properties["UserLoginId"] = null;//Added by Sonali for authorization filter on 16-04-2019
                    return obj = new { Success = 0, Message = "Sorry! Please enter valid details.", data = string.Empty };
                }
                if (SocialAccType == "Google")
                {
                    var userExist = (from user in db.UserLogins
                                     join ps in db.PersonalDetails on user.ID equals ps.UserLoginID
                                     where ps.Insta == SocialId
                                     select new
                                     {
                                         UserLoginID = user.ID,
                                         UserName = user.Email,
                                         EmailID = user.Email,
                                         MobileNo = user.Mobile,
                                         FirstName = ps.FirstName,
                                         LastName = ps.LastName
                                     }).ToList();
                    if (userExist != null && userExist.Count() > 0)
                    {
                        CustomerCredentials lCustCredentials = new CustomerCredentials();
                        foreach (var item in userExist)
                        {
                            lCustCredentials.UserLoginID = item.UserLoginID;
                            if (item.EmailID != null)
                            {
                                lCustCredentials.UserName = item.EmailID.ToString();
                            }
                            lCustCredentials.MobileNo = item.MobileNo;
                            if (item.EmailID != null)
                            {
                                lCustCredentials.EmailID = item.EmailID.ToString();
                            }
                            lCustCredentials.FirstName = item.FirstName;
                            lCustCredentials.LastName = item.LastName;

                        }
                        lCustCredentials.IsMLMUser = db.MLMUsers.Where(x => x.UserID == lCustCredentials.UserLoginID).Any();
                        string ReferralId = db.MLMUsers.Where(x => x.UserID == lCustCredentials.UserLoginID).Select(x => x.Ref_Id).FirstOrDefault();
                        Request.Properties["UserLoginId"] = lCustCredentials.UserLoginID;//Added by Sonali for authorization filter on 16-04-2019
                        obj = new { Success = 1, Message = "Login Successfull.", data = new { UserLoginID = lCustCredentials.UserLoginID, UserName = lCustCredentials.UserName, EmailID = lCustCredentials.EmailID, MobileNo = lCustCredentials.MobileNo, FirstName = lCustCredentials.FirstName, LastName = lCustCredentials.LastName, IsMLMUser = lCustCredentials.IsMLMUser, ReferralId = ReferralId } };
                    }
                    else
                    {
                        Request.Properties["UserLoginId"] = null;//Added by Sonali for authorization filter on 16-04-2019
                        obj = new { Success = 0, Message = "Data Not found", data = string.Empty };
                    }
                }
                else if (SocialAccType == "Facebook")
                {
                    var userExist = (from user in db.UserLogins
                                     join ps in db.PersonalDetails on user.ID equals ps.UserLoginID
                                     where ps.Facebook == SocialId
                                     select new
                                     {
                                         UserLoginID = user.ID,
                                         UserName = user.Email,
                                         EmailID = user.Email,
                                         MobileNo = user.Mobile,
                                         FirstName = ps.FirstName,
                                         LastName = ps.LastName
                                     }).ToList();
                    if (userExist != null && userExist.Count() > 0)
                    {
                        CustomerCredentials lCustCredentials = new CustomerCredentials();
                        foreach (var item in userExist)
                        {
                            lCustCredentials.UserLoginID = item.UserLoginID;
                            if (item.EmailID != null)
                            {
                                lCustCredentials.UserName = item.EmailID.ToString();
                            }
                            lCustCredentials.MobileNo = item.MobileNo;
                            if (item.EmailID != null)
                            {
                                lCustCredentials.EmailID = item.EmailID.ToString();
                            }
                            lCustCredentials.FirstName = item.FirstName;
                            lCustCredentials.LastName = item.LastName;
                        }

                        lCustCredentials.IsMLMUser = db.MLMUsers.Where(x => x.UserID == lCustCredentials.UserLoginID).Any();
                        string ReferralId = db.MLMUsers.Where(x => x.UserID == lCustCredentials.UserLoginID).Select(x => x.Ref_Id).FirstOrDefault();
                        Request.Properties["UserLoginId"] = lCustCredentials.UserLoginID;//Added by Sonali for authorization filter on 16-04-2019
                        obj = new { Success = 1, Message = "Login Successfull.", data = new { UserLoginID = lCustCredentials.UserLoginID, UserName = lCustCredentials.UserName, EmailID = lCustCredentials.EmailID, MobileNo = lCustCredentials.MobileNo, FirstName = lCustCredentials.FirstName, LastName = lCustCredentials.LastName, IsMLMUser = lCustCredentials.IsMLMUser, ReferralId = ReferralId } };
                    }
                    else
                    {
                        Request.Properties["UserLoginId"] = null;//Added by Sonali for authorization filter on 16-04-2019
                        obj = new { Success = 0, Message = "Data Not found", data = string.Empty };
                    }
                }
            }
            catch (Exception ex)
            {
                Request.Properties["UserLoginId"] = null;//Added by Sonali for authorization filter on 16-04-2019
                obj = new { Success = 0, Message = ex.Message, data = string.Empty };
            }
            return obj;
        }
        
    }
}
