
//-----------------------------------------------------------------------
// <copyright file="LoginController" company="Ezeelo Consumer Services Pvt. Ltd.">
//     Copyright (c) Ezeelo Consumer Services Pvt. Ltd. All rights reserved.
// </copyright>
// <author>Gaurav Dixit</author>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ModelLayer.Models;
using System.Security.Claims;
using ModelLayer.Models.ViewModel;
using BusinessLogicLayer;
using System.Text.RegularExpressions;
using Microsoft.Owin.Security;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;
using System.Transactions;
using System.Data;
using System.Data.Entity;
using System.Web.Security;
using System.Text;
using System.Globalization;

namespace Gandhibagh.Controllers
{
    public class LoginController : Controller
    {
        Encryption _Encryption = new Encryption();//Added by Rumana on 13/04/2019
        private EzeeloDBContext db = new EzeeloDBContext();

        //[HttpGet]
        //public ActionResult Login()
        //{

        //    /*
        //       Indents:
        //     * Description: This is used to customer login , this will be call from payment process or normal login

        //     * Parameters: 

        //     * Precondition: 
        //     * Postcondition:
        //     * Logic: 
        //     */
        //    CarryData carryData = new CarryData();
        //    //Use for returnig to the previous page
        //    ViewBag.returnurl = Request.Cookies["UrlCookie"].Value;
        //     //   ViewBag.returnurl = Request.Cookies["UrlCookie"].Value.Split('?')[0];////added for testing
        //        carryData.returnURL = Request.Cookies["UrlCookie"].Value; 


        //   // carryData.returnURL = System.Web.HttpContext.Current.Request.UrlReferrer.ToString(); 
        //    carryData.isExpressBuy = false;

        //    if (Session["UID"] == null)
        //    {
        //        ViewBag.salutation = new SelectList(db.Salutations, "ID", "Name");

        //        if (Request.Cookies["UserName"] != null && Request.Cookies["UserName"].Value != string.Empty)
        //        {
        //            TempData["mobEmail"]=Request.Cookies["UserName"].Value.Split('$')[1];
        //        }
        //        if (Request.QueryString["callFrom"] != null)
        //        {
        //            //For outside city set layout page on viewstart page. because of this when layout page is not pass , login page load without layout
        //            //for this layout page is passed
        //            //Tejaswee (21/10/2015)
        //            //return View("_Login", "_Layout1Nagpur");

        //            carryData.callFrom = Request.QueryString["callFrom"].ToString();

        //            TempData["carryData"] = carryData;

        //            //if (Request.Cookies["CityCookie"] != null && Request.Cookies["CityCookie"].Value != string.Empty)
        //            //{
        //            //    string cityName = Request.Cookies["CityCookie"].Value.Split('$')[1];
        //            //    return View("_Login", "_Layout" + cityName);
        //            //}
        //            //else
        //            //{
        //            return View("_Login", "_Layout_Internal");
        //            //}

        //            //Gaurav Dixit
        //            //return PartialView("_Login");
        //        }
        //        else
        //        {
        //            TempData["carryData"] = carryData;
        //            //if (Request.Cookies["CityCookie"] != null && Request.Cookies["CityCookie"].Value != string.Empty)
        //            //{
        //            //    string cityName = Request.Cookies["CityCookie"].Value.Split('$')[1];
        //            //    return View("_Login", "_Layout" + cityName);
        //            //}
        //            //else
        //            //{
        //            return View("_Login", "_Layout_Internal");
        //           // }
        //            //                return View("_Login", "_Layout1Nagpur");
        //        }
        //    }
        //    else
        //    {
        //        return RedirectToAction("Index", "Home");
        //    }
        //}

        //[HttpPost]
        //public ActionResult Login(LoginViewModel model, string callFrom, bool isExpressBuy, string returnUrl)
        //{
        //    /*
        //       Indents:
        //     * Description: This is the post action of login method. Any User has to go through this action for login.
        //     *              The same action is used in normal login and payment process.
        //     * Parameters: 1) callFrom : this variable is used to check is the user coming from normal login or payment process.
        //     *             2) isExpressBuy : This variable is used to check is the buy process is normal or express. 
        //     *             3) returnUrl : is used to return the page to the calling page.
        //     * Precondition: 
        //     * Postcondition:
        //     * Logic: 1) check Email/Mobile(Regx) is incorrect then we redirect the user from current page, it may comming from payment process or normal login
        //              2) If true, then check for that Email/Mobile is exist in database or not
        //              2.1) If not exist, return error
        //              2.2) If exist, store UserLoginId/Email in session variable
        //              2.2.1)  If user comming from payment process page then redirect to payment process
        //              2.2.2) Return user to the calling page(return url)
        //     */

        //    try
        //    {

        //       // TempData["returnUrl"] = returnUrl;///for testing

        //        //- This variable is used in the view page to animate login button (cursor pointer....)
        //        ViewBag.IsSuccess = false;

        //        //- These varables are used to check is the user is Registered or not.
        //        bool IsEmailValid = false, IsMobileValid = false;

        //        IsEmailValid = CommonFunctions.IsValidEmailId(model.UserName);

        //        if (IsEmailValid == false)
        //            IsMobileValid = CommonFunctions.IsValidMobile(model.UserName);

        //        // 1.
        //        if (IsEmailValid == false && IsMobileValid == false)
        //        {
        //            ////ViewBag.Message = "Invalid UserName/Password!!";
        //            //TempData["Message"] = "Invalid UserName/Password!!";
        //            TempData["Message"] = " Please Enter Valid Email/Mobile. ";
        //            if (callFrom == "paymentProcess")
        //            {
        //                if (isExpressBuy == true)
        //                    return RedirectToAction("CustomerPaymentProcess", "PaymentProcess", new { IsExpressBuy = true });
        //                else
        //                    return RedirectToAction("CustomerPaymentProcess", "PaymentProcess");
        //            }
        //            return View("_Login", "_Layout_Internal");
        //        }
        //        else  //2.
        //        {
        //            string msg = string.Empty;
        //            Dictionary<string, string> lDictLoginDetails = this.CheckLogin(model.UserName, model.Password,out msg);


        //            if (lDictLoginDetails.Count() <= 0) // 2.1
        //            {
        //                //ViewBag.Message = "Invalid UserName/Password!!";
        //               // TempData["Message"] = "Invalid UserName/Password!!";
        //                TempData["Message"] = msg;
        //                if (callFrom == "paymentProcess")
        //                {
        //                    if (isExpressBuy == true)
        //                        return RedirectToAction("CustomerPaymentProcess", "PaymentProcess", new { IsExpressBuy = true });
        //                    else
        //                      //  return RedirectToAction("CustomerPaymentProcess", "PaymentProcess");
        //                        return RedirectToRoute("PaymentProcess", new { city = Request.Cookies["CityCookie"].Value.Split('$')[1].ToLower(), franchiseId = Request.Cookies["CityCookie"].Value.Split('$')[2].Trim() });////added franchiseId
        //                }

        //                return View("_Login", "_Layout_Internal");
        //            }
        //            else //2.2
        //            {
        //                Session["UID"] = lDictLoginDetails["ID"];
        //                Session["UserName"] = lDictLoginDetails["UserName"];




        //                long userLoginId = 0;
        //                long.TryParse(Convert.ToString(Session["UID"]), out userLoginId);

        //                Session["FirstName"] = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(db.PersonalDetails.Find(CommonFunctions.GetPersonalDetailsID(userLoginId)).FirstName.ToLower());

        //                /*============== Save user name in cookie =====================*/

        //                this.SetUserNameCookie(Session["FirstName"].ToString(), model.UserName);


        //                //===========================================================================
        //                if (callFrom == null || callFrom == string.Empty)
        //                {
        //                    CarryData obj = new CarryData();
        //                    obj = (CarryData)TempData["carryData"];
        //                    // ViewBag.returnurl = Request.Cookies["UrlCookie"].Value;
        //                    callFrom = obj.callFrom;
        //                }
        //                //============================================================================
        //                if (callFrom!=null && callFrom.ToLower().Contains("cart"))
        //                {
        //                    string[] val = callFrom.Split('/'); 
        //                    ShoppingCartInitialization objj = new ShoppingCartInitialization();
        //                    int status = objj.SetCookie(Convert.ToInt64(val[1]), Convert.ToInt64(val[2]), Convert.ToInt32(val[3]));
        //                    //==================== Track cart changes =======================
        //                    Nullable<long> lCartID = null;
        //                    if (ControllerContext.HttpContext.Request.Cookies["CartID"] != null)
        //                    {
        //                        lCartID = Convert.ToInt64(ControllerContext.HttpContext.Request.Cookies["CartID"].Value.ToString());
        //                    }
        //                    TrackCartBusiness.InsertCartDetails(lCartID, Convert.ToInt32(val[3]), userLoginId, Convert.ToInt64(val[1]), model.UserName, "SHOPPING_CART", "", "", "", "", Request.Cookies["CityCookie"].Value.Split('$')[1].ToLower(), "", Convert.ToInt32(Request.Cookies["CityCookie"].Value.Split('$')[2].Trim()));//--added by Ashish for multiple franchise in same city--//
        //                    //==================== Track cart changes =======================
        //                    return Redirect(returnUrl);
        //                }
        //                else if (callFrom == "paymentProcess")  // 2.2.1
        //                {
        //                    //change by harshada to redirect to subscription page
        //                    if (TempData["Subscription"] != null)
        //                    {
        //                        return RedirectToAction("Index", "SubscriptionPlan");
        //                    }
        //                    //-------------------------------------------------------
        //                    this.IsCouponUsedByCustomer(userLoginId, isExpressBuy);

        //                    if (isExpressBuy == true)
        //                        return RedirectToAction("CustomerPaymentProcess", "PaymentProcess", new { IsExpressBuy = true });
        //                    else
        //                        //return RedirectToAction("CustomerPaymentProcess", "PaymentProcess");
        //                        return RedirectToRoute("PaymentProcess", new { city = Request.Cookies["CityCookie"].Value.Split('$')[1].ToLower(), franchiseId = Request.Cookies["CityCookie"].Value.Split('$')[2] });////added franchiseId

        //                }
        //                else if (returnUrl != string.Empty && returnUrl != null) // 2.2.2
        //                {
        //                    if (returnUrl.Contains("CustomerOrder/OrderStatus"))
        //                    {
        //                        return RedirectToAction("MyOrders", "CustomerOrder");
        //                    }
        //                    else if (returnUrl.Contains("Customer/create"))
        //                    {
        //                        return RedirectToAction("Edit", "Customer", new { id = Session["UID"] });
        //                    }
        //                    else
        //                    {
        //                        return Redirect(returnUrl);
        //                       // return Redirect(TempData["returnUrl"].ToString());//for testing

        //                    }
        //                }

        //                else
        //                {
        //                    //change by harshada
        //                    string Url = "";
        //                    //return RedirectToAction("Index", "Home");
        //                    /*----------Get Url from cookie to redirect to its previous position-------*/
        //                    if (Request.Cookies["UrlCookie"] != null)
        //                    {
        //                        Url = Request.Cookies["UrlCookie"].Value.ToString();
        //                    }
        //                    //return Redirect(ControllerContext.HttpContext.Request.UrlReferrer.ToString());
        //                    return Redirect(Url);
        //                    /*----------End Get Url from cookie to redirect to its previous position-------*/
        //                }
        //            }
        //        }
        //        if (TempData["CurrentPageIndex"] != null)
        //        {
        //            TempData.Keep("CurrentPageIndex");
        //        }
        //    }
        //    catch (BusinessLogicLayer.MyException myEx)
        //    {
        //        ViewBag.IsSuccess = false;
        //        BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
        //            + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
        //            + "[LoginController][POST:Login]" + myEx.EXCEPTION_PATH,
        //            BusinessLogicLayer.ErrorLog.Module.Gandhibagh, System.Web.HttpContext.Current.Server);

        //        if (callFrom == "paymentProcess")
        //        {
        //            if (isExpressBuy == true)
        //                return RedirectToAction("CustomerPaymentProcess", "PaymentProcess", new { IsExpressBuy = true });
        //            else
        //                return RedirectToAction("CustomerPaymentProcess", "PaymentProcess");
        //        }
        //        return View("_Login", "_Layout_Internal");
        //    }
        //    catch (Exception ex)
        //    {
        //        ViewBag.IsSuccess = false;
        //        BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
        //            + Environment.NewLine + ex.Message + Environment.NewLine
        //            + "[LoginController][POST:Login]",
        //            BusinessLogicLayer.ErrorLog.Module.Gandhibagh, System.Web.HttpContext.Current.Server);

        //        if (callFrom == "paymentProcess")
        //        {
        //            if (isExpressBuy == true)
        //                return RedirectToAction("CustomerPaymentProcess", "PaymentProcess", new { IsExpressBuy = true });
        //            else
        //                //   return RedirectToAction("CustomerPaymentProcess", "PaymentProcess", new { });
        //                return RedirectToRoute("PaymentProcess", new { city = Request.Cookies["CityCookie"].Value.Split('$')[1].ToLower(), franchiseId = Request.Cookies["CityCookie"].Value.Split('$')[2] });////added franchiseId
        //        }
        //        return View("_Login", "_Layout_Internal");
        //    }
        //}

        [HttpGet]
        public ActionResult Login(string Phone, string ReferalCode, string Name, string Email)
        {
            if(ReferalCode != null)
            {
                TempData["URLReferalCode"] = ReferalCode;
            }
            string city = "";
            int FranchiseID = 0;////added by Ashish for multiple MCO in same city
            if (Request.Cookies["CityCookie"] != null && (Request.Cookies["CityCookie"].Value != null || Request.Cookies["CityCookie"].Value != string.Empty))
            {
                city = Request.Cookies["CityCookie"].Value.Split('$')[1].ToLower();
                FranchiseID = Convert.ToInt32(Request.Cookies["CityCookie"].Value.Split('$')[2]);////added by Ashish for multiple MCO in same city
                Session["SelectedCity"] = city;
                Session["SelectedFranchiseId"] = FranchiseID;
            }
            //Start Yashaswi For Mlm
            CustomerRegistrationViewModel SignUpDetail = new CustomerRegistrationViewModel();
            LoginViewModel logDetail = null;

            if (!String.IsNullOrEmpty(Phone) && !String.IsNullOrEmpty(ReferalCode) && !String.IsNullOrEmpty(Name) && !String.IsNullOrEmpty(Email))
            {
                //Check Fo existing User
                if (db.UserLogins.Any(p => p.Email == Email))
                {
                    logDetail = new LoginViewModel();
                    logDetail.ReferralId = ReferalCode;
                    logDetail.UserName = Email;
                    TempData["UserName"] = Email;
                }
                else if (db.UserLogins.Any(p => p.Mobile == Phone))
                {
                    logDetail = new LoginViewModel();
                    logDetail.ReferralId = ReferalCode;
                    logDetail.UserName = Phone;
                    TempData["UserName"] = Phone;
                }
                else
                {
                    SignUpDetail.ReferralId = ReferalCode;
                    SignUpDetail.FirstName = Name;
                    SignUpDetail.MobileNo = Phone;
                    SignUpDetail.EmailId = Email;
                    TempData["UserName"] = "";
                }
                ViewBag.CustomerRegistrationViewModel = SignUpDetail;
            }

            /*
               Indents:
             * Description: This is used to customer login , this will be call from payment process or normal login
             
             * Parameters: 
             
             * Precondition: 
             * Postcondition:
             * Logic: 
             */
            CarryData carryData = new CarryData();
            //Use for returnig to the previous page
            try
            {
                ViewBag.returnurl = Request.Cookies["UrlCookie"].Value;
                //   ViewBag.returnurl = Request.Cookies["UrlCookie"].Value.Split('?')[0];////added for testing
                carryData.returnURL = Request.Cookies["UrlCookie"].Value;
            }
            catch
            {
                //Yashaswi 01/12/2018 Default City Change 
                ViewBag.returnurl = "" + (new URLsFromConfig()).GetURL("CUSTOMER") + URLsFromConfig.GetDefaultData("CITY_NAME") + "/" + URLsFromConfig.GetDefaultData("FRANCHISE_ID") + "?URL=" + (new URLsFromConfig()).GetURL("CUSTOMER") + URLsFromConfig.GetDefaultData("CITY_NAME") + "/" + URLsFromConfig.GetDefaultData("FRANCHISE_ID");
                carryData.returnURL = "" + (new URLsFromConfig()).GetURL("CUSTOMER") + URLsFromConfig.GetDefaultData("CITY_NAME") + "/" + URLsFromConfig.GetDefaultData("FRANCHISE_ID") + "?URL=" + (new URLsFromConfig()).GetURL("CUSTOMER") + URLsFromConfig.GetDefaultData("CITY_NAME") + "/" + URLsFromConfig.GetDefaultData("FRANCHISE_ID");
            }

            //End Yashaswi For Mlm

            // carryData.returnURL = System.Web.HttpContext.Current.Request.UrlReferrer.ToString(); 
            carryData.isExpressBuy = false;

            if (Session["UID"] == null)
            {
                ViewBag.salutation = new SelectList(db.Salutations, "ID", "Name");

                if (Request.Cookies["UserName"] != null && Request.Cookies["UserName"].Value != string.Empty)
                {
                    TempData["mobEmail"] = Request.Cookies["UserName"].Value.Split('$')[1];
                }
                if (Request.QueryString["callFrom"] != null)
                {
                    //For outside city set layout page on viewstart page. because of this when layout page is not pass , login page load without layout
                    //for this layout page is passed
                    //Tejaswee (21/10/2015)
                    //return View("_Login", "_Layout1Nagpur");

                    carryData.callFrom = Request.QueryString["callFrom"].ToString();

                    TempData["carryData"] = carryData;

                    //if (Request.Cookies["CityCookie"] != null && Request.Cookies["CityCookie"].Value != string.Empty)
                    //{
                    //    string cityName = Request.Cookies["CityCookie"].Value.Split('$')[1];
                    //    return View("_Login", "_Layout" + cityName);
                    //}
                    //else
                    //{
                    //Start Yashaswi For Mlm
                    if (logDetail != null)
                    {
                        return View("_Login", "_Layout_Internal", logDetail);
                    }
                    else
                    {
                        return View("_Login", "_Layout_Internal");
                    }
                    //}

                    //Gaurav Dixit
                    //return PartialView("_Login");
                }
                else
                {
                    TempData["carryData"] = carryData;
                    //if (Request.Cookies["CityCookie"] != null && Request.Cookies["CityCookie"].Value != string.Empty)
                    //{
                    //    string cityName = Request.Cookies["CityCookie"].Value.Split('$')[1];
                    //    return View("_Login", "_Layout" + cityName);
                    //}
                    //else
                    //{
                    //Start Yashaswi For Mlm
                    if (logDetail != null)
                    {
                        return View("_Login", "_Layout_Internal", logDetail);
                    }
                    else
                    {
                        return View("_Login", "_Layout_Internal");
                    }
                    // }
                    //                return View("_Login", "_Layout1Nagpur");
                }
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        //Yashaswi for Reffereal code validation for Leaders SignUp
        public ActionResult ValidateReferralId(string ReferralId)
        {
            try
            {
                bool Result = db.MLMUsers.Any(p => p.Ref_Id == ReferralId && !db.UserLogins.Where(u => u.IsLocked == true).Select(u => u.ID).ToList().Contains(p.UserID));
                return Json(Result, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult Login(LoginViewModel model, string callFrom, bool? isExpressBuy, string returnUrl)
        {
            /*
               Indents:
             * Description: This is the post action of login method. Any User has to go through this action for login.
             *              The same action is used in normal login and payment process.
             * Parameters: 1) callFrom : this variable is used to check is the user coming from normal login or payment process.
             *             2) isExpressBuy : This variable is used to check is the buy process is normal or express. 
             *             3) returnUrl : is used to return the page to the calling page.
             * Precondition: 
             * Postcondition:
             * Logic: 1) check Email/Mobile(Regx) is incorrect then we redirect the user from current page, it may comming from payment process or normal login
                      2) If true, then check for that Email/Mobile is exist in database or not
                      2.1) If not exist, return error
                      2.2) If exist, store UserLoginId/Email in session variable
                      2.2.1)  If user comming from payment process page then redirect to payment process
                      2.2.2) Return user to the calling page(return url)
             */

            try
            {
                isExpressBuy = isExpressBuy == null ? false : isExpressBuy.Value;
                // TempData["returnUrl"] = returnUrl;///for testing

                //- This variable is used in the view page to animate login button (cursor pointer....)
                ViewBag.IsSuccess = false;

                //- These varables are used to check is the user is Registered or not.
                bool IsEmailValid = false, IsMobileValid = false;

                IsEmailValid = CommonFunctions.IsValidEmailId(model.UserName);

                if (IsEmailValid == false)
                    IsMobileValid = CommonFunctions.IsValidMobile(model.UserName);

                // 1.
                if (IsEmailValid == false && IsMobileValid == false)
                {
                    ////ViewBag.Message = "Invalid UserName/Password!!";
                    //TempData["Message"] = "Invalid UserName/Password!!";
                    TempData["Message"] = " Please Enter Valid Email/Mobile. ";
                    if (callFrom == "paymentProcess")
                    {
                        if (isExpressBuy == true)
                            return RedirectToAction("CustomerPaymentProcess", "PaymentProcess", new { IsExpressBuy = true });
                        else
                            return RedirectToAction("CustomerPaymentProcess", "PaymentProcess");
                    }
                    return View("_Login", "_Layout_Internal");
                }
                else  //2.
                {
                    string msg = string.Empty;
                    Dictionary<string, string> lDictLoginDetails = this.CheckLogin(model.UserName, model.Password, out msg);


                    if (lDictLoginDetails.Count() <= 0) // 2.1
                    {
                        //ViewBag.Message = "Invalid UserName/Password!!";
                        // TempData["Message"] = "Invalid UserName/Password!!";
                        TempData["Message"] = msg;
                        if (callFrom == "paymentProcess")
                        {
                            if (isExpressBuy == true)
                                return RedirectToAction("CustomerPaymentProcess", "PaymentProcess", new { IsExpressBuy = true });
                            else
                                //  return RedirectToAction("CustomerPaymentProcess", "PaymentProcess");
                                return RedirectToRoute("PaymentProcess", new { city = Request.Cookies["CityCookie"].Value.Split('$')[1].ToLower(), franchiseId = Request.Cookies["CityCookie"].Value.Split('$')[2].Trim() });////added franchiseId
                        }

                        return View("_Login", "_Layout_Internal");
                    }
                    else //2.2
                    {

                        //Start Yashaswi Leader Signup
                        try
                        {
                            long LoginUserId = Convert.ToInt64(lDictLoginDetails["ID"]);
                            UserLogin userLog = db.UserLogins.FirstOrDefault(p => p.ID == LoginUserId);
                            var EncryptedEmail = _Encryption.EncodePasswordToBase64(userLog.Email);//Added by Rumana on 13/04/2019
                            var EncryptedPassword = _Encryption.EncodePasswordToBase64(userLog.Password);//Added by Rumana on 13/04/2019
                            if (!String.IsNullOrEmpty(model.ReferralId))
                            {
                                MLMWalletPoints MLMWallet = new MLMWalletPoints();
                                string result = MLMWallet.LeadersSingUp(userLog.ID, userLog.Password, userLog.Email, model.ReferralId);
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
                                        UserName = "";
                                    }
                                    Session["LeaderSignUpLink"] = "" + (new URLsFromConfig()).GetURL("LEADERS") + "LeadersLogin/Login/?UserName=" + EncryptedEmail + "&Password=" + EncryptedPassword;//Changes by Rumana on 13/04/2019 EncryptedEmail and EncryptedPassword
                                    //Session["LeaderSignUpLink"] = "http://leaders.ezeelo.com/signin/?Email=" + userLog.Email + "&Password=" + userLog.Password;
                                    TempData["IsLeadersSignUp"] = "You Have Successfully Become Ezeelo Member" + UserName;
                                }
                                else if (result.Contains("ALREADY_R"))
                                {
                                    try
                                    {
                                        string RefId = db.MLMUsers.FirstOrDefault(p => p.UserID == userLog.ID).Refered_Id_ref;
                                        UserName = " under " + db.PersonalDetails.FirstOrDefault(p => p.UserLoginID == (db.MLMUsers.FirstOrDefault(m => m.Ref_Id == RefId).UserID)).FirstName;
                                    }
                                    catch
                                    {
                                        UserName = "";
                                    }
                                    Session["LeaderSignUpLink"] = "" + (new URLsFromConfig()).GetURL("LEADERS") + "LeadersLogin/Login/?UserName=" + EncryptedEmail + "&Password=" + EncryptedPassword;//Changes by Rumana on 13/04/2019 EncryptedEmail and EncryptedPassword
                                    //Session["LeaderSignUpLink"] = "http://leaders.ezeelo.com/signin/?Email=" + userLog.Email + "&Password=" + userLog.Password;
                                    TempData["IsLeadersSignUp"] = "You Are Already Registered As Ezeelo Member" + UserName;
                                }
                                else
                                {
                                    TempData["IsLeadersSignUp"] = "Facing Any Trouble , Happy To Help You, Simply Call 9172221910";
                                }
                            }
                            else
                            {
                                if (db.MLMUsers.Any(p => p.UserID == LoginUserId))
                                {
                                    Session["LeaderSignUpLink"] = "" + (new URLsFromConfig()).GetURL("LEADERS") + "LeadersLogin/Login/?UserName=" + EncryptedEmail + "&Password=" + EncryptedPassword;//Changes by Rumana on 13/04/2019 EncryptedEmail and EncryptedPassword
                                    //Session["LeaderSignUpLink"] = "http://leaders.ezeelo.com/signin/?Email=" + userLog.Email + "&Password=" + userLog.Password;
                                }
                                else
                                {
                                    Session["LeaderSignUpLink"] = null;
                                }
                            }
                        }
                        catch
                        {

                        }
                        //End

                        Session["UID"] = lDictLoginDetails["ID"];
                        Session["UserName"] = lDictLoginDetails["UserName"];




                        long userLoginId = 0;
                        long.TryParse(Convert.ToString(Session["UID"]), out userLoginId);

                        //Start Yashaswi 11-3-2019 To solve BUG 1429 Throwing error if details on present in PersonalDetails Table
                        long personalDetailId = CommonFunctions.GetPersonalDetailsID(userLoginId);
                        if (personalDetailId == 0)
                        {
                            Session["FirstName"] = "";
                        }
                        else
                        {
                            Session["FirstName"] = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(db.PersonalDetails.Find(personalDetailId).FirstName.ToLower());
                        }
                        //End Yashaswi 11-3-2019 To solve BUG 1429 Throwing error if details on present in PersonalDetails Table
                        /*============== Save user name in cookie =====================*/

                        this.SetUserNameCookie(Session["FirstName"].ToString(), model.UserName);

                        return RedirectToAction("Index", "Home");

                        //===========================================================================
                        if (callFrom == null || callFrom == string.Empty)
                        {
                            CarryData obj = new CarryData();
                            obj = (CarryData)TempData["carryData"];
                            // ViewBag.returnurl = Request.Cookies["UrlCookie"].Value;
                            callFrom = obj.callFrom;
                        }
                        //============================================================================
                        if (callFrom != null && callFrom.ToLower().Contains("cart"))
                        {
                            string[] val = callFrom.Split('/');
                            ShoppingCartInitialization objj = new ShoppingCartInitialization();
                            string status = objj.SetCookie(Convert.ToInt64(val[1]), Convert.ToInt64(val[2]), Convert.ToInt32(val[3]));
                            //==================== Track cart changes =======================
                            Nullable<long> lCartID = null;
                            if (ControllerContext.HttpContext.Request.Cookies["CartID"] != null)
                            {
                                lCartID = Convert.ToInt64(ControllerContext.HttpContext.Request.Cookies["CartID"].Value.ToString());
                            }
                            TrackCartBusiness.InsertCartDetails(lCartID, Convert.ToInt32(val[3]), userLoginId, Convert.ToInt64(val[1]), model.UserName, "SHOPPING_CART", "", "", "", "", Request.Cookies["CityCookie"].Value.Split('$')[1].ToLower(), "", Convert.ToInt32(Request.Cookies["CityCookie"].Value.Split('$')[2].Trim()));//--added by Ashish for multiple franchise in same city--//
                            //==================== Track cart changes =======================
                            return Redirect(returnUrl);
                        }
                        else if (callFrom == "paymentProcess")  // 2.2.1
                        {
                            //change by harshada to redirect to subscription page
                            if (TempData["Subscription"] != null)
                            {
                                return RedirectToAction("Index", "SubscriptionPlan");
                            }
                            //-------------------------------------------------------
                            this.IsCouponUsedByCustomer(userLoginId, isExpressBuy.Value);

                            if (isExpressBuy == true)
                                return RedirectToAction("CustomerPaymentProcess", "PaymentProcess", new { IsExpressBuy = true });
                            else
                                //return RedirectToAction("CustomerPaymentProcess", "PaymentProcess");
                                return RedirectToRoute("PaymentProcess", new { city = Request.Cookies["CityCookie"].Value.Split('$')[1].ToLower(), franchiseId = Request.Cookies["CityCookie"].Value.Split('$')[2] });////added franchiseId

                        }
                        else if (returnUrl != string.Empty && returnUrl != null) // 2.2.2
                        {
                            if (returnUrl.Contains("CustomerOrder/OrderStatus"))
                            {
                                return RedirectToAction("MyOrders", "CustomerOrder");
                            }
                            else if (returnUrl.Contains("Customer/create"))
                            {
                                return RedirectToAction("Edit", "Customer", new { id = Session["UID"] });
                            }
                            else
                            {
                                return Redirect(returnUrl);
                                // return Redirect(TempData["returnUrl"].ToString());//for testing

                            }
                        }

                        else
                        {
                            //change by harshada
                            string Url = "";
                            //return RedirectToAction("Index", "Home");
                            /*----------Get Url from cookie to redirect to its previous position-------*/
                            if (Request.Cookies["UrlCookie"] != null)
                            {
                                Url = Request.Cookies["UrlCookie"].Value.ToString();
                            }
                            //return Redirect(ControllerContext.HttpContext.Request.UrlReferrer.ToString());
                            return Redirect(Url);
                            /*----------End Get Url from cookie to redirect to its previous position-------*/
                        }
                    }
                }
                if (TempData["CurrentPageIndex"] != null)
                {
                    TempData.Keep("CurrentPageIndex");
                }
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                ViewBag.IsSuccess = false;
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[LoginController][POST:Login]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Gandhibagh, System.Web.HttpContext.Current.Server);

                if (callFrom == "paymentProcess")
                {
                    if (isExpressBuy == true)
                        return RedirectToAction("CustomerPaymentProcess", "PaymentProcess", new { IsExpressBuy = true });
                    else
                        return RedirectToAction("CustomerPaymentProcess", "PaymentProcess");
                }
                return View("_Login", "_Layout_Internal");
            }
            catch (Exception ex)
            {
                ViewBag.IsSuccess = false;
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[LoginController][POST:Login]",
                    BusinessLogicLayer.ErrorLog.Module.Gandhibagh, System.Web.HttpContext.Current.Server);

                if (callFrom == "paymentProcess")
                {
                    if (isExpressBuy == true)
                        return RedirectToAction("CustomerPaymentProcess", "PaymentProcess", new { IsExpressBuy = true });
                    else
                        //   return RedirectToAction("CustomerPaymentProcess", "PaymentProcess", new { });
                        return RedirectToRoute("PaymentProcess", new { city = Request.Cookies["CityCookie"].Value.Split('$')[1].ToLower(), franchiseId = Request.Cookies["CityCookie"].Value.Split('$')[2] });////added franchiseId
                }
                return View("_Login", "_Layout_Internal");
            }
        }


        //private Dictionary<string, string> CheckLogin(string pUserName, string pPassword)
        //{
        //    /*
        //       Indents: This method is used to check UserName/Password is exist in database or not
        //     * Description: 
        //     *              
        //     * Parameters:  pUserName: Contains UserName, pPassword: Contains Password

        //     * Precondition: 
        //     * Postcondition:
        //     * Logic: 
        //     */

        //    try
        //    {
        //        Dictionary<string, string> lDictUserDetails = new Dictionary<string, string>();

        //        var userExist = db.UserLogins.Select(x => new { x.Email, x.Mobile, x.Password, x.ID })
        //                                     .Where(x => (x.Email == pUserName || x.Mobile == pUserName) && x.Password == pPassword).ToList();
        //        if (userExist.Count() > 0)
        //        {
        //            foreach (var item in userExist)
        //            {
        //                lDictUserDetails.Add("ID", item.ID.ToString());
        //                /*to check whether Email Adderss is null stored 
        //                 * Pradnyakar Badge
        //                 * 05-01-2016
        //                 */
        //                if (item.Email != null)
        //                {
        //                    lDictUserDetails.Add("UserName", item.Email.ToString());
        //                }
        //                else
        //                {
        //                    lDictUserDetails.Add("UserName", item.Mobile.ToString());
        //                }
        //            }
        //        }

        //        return lDictUserDetails;
        //    }
        //    catch (MyException myEx)
        //    {
        //        throw new BusinessLogicLayer.MyException("[CustomerController][M:CheckLogin]", "Can't check login details!" + Environment.NewLine + myEx.Message);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new BusinessLogicLayer.MyException("[CustomerController][M:CheckLogin]", "Can't check login details!" + Environment.NewLine + ex.Message);
        //    }
        //}


        private Dictionary<string, string> CheckLogin(string pUserName, string pPassword, out string msg)
        {
            /*
               Indents: This method is used to check UserName/Password is exist in database or not
             * Description: 
             *              
             * Parameters:  pUserName: Contains UserName, pPassword: Contains Password
            
             * Precondition: 
             * Postcondition:
             * Logic: 
             */

            try
            {
                msg = string.Empty;
                Dictionary<string, string> lDictUserDetails = new Dictionary<string, string>();

                var userExist = db.UserLogins.Select(x => new { x.Email, x.Mobile, x.Password, x.ID, x.IsLocked })
                                             .Where(x => (x.Email == pUserName || x.Mobile == pUserName) && x.Password == pPassword && x.IsLocked == false).ToList();

                //- Start : Added by Avi Verma.
                //- Date : 06-Sep-2016. 
                //- Reason : CRM Person can login on behalf of customer for Abandoned Cart.
                if (userExist.Count() > 0)
                {
                    //- Customer Login.
                    //- Create an empty Abandoned Cart for him, So that it can be used as CartID for Track Cart module.
                    //- This cart is only created for Customer Login, not on CRM Login on behalf of Customer Login.
                    Cart lCart = CreateVirtualAbandonedCart(userExist.FirstOrDefault().ID);
                }
                if (userExist.Count() <= 0)
                {
                    //- Can CRM Login on behalf of Customer.
                    //- In this case we are not tracking Cart.
                    //- We will fill Existng Cart from Database.

                    //- Delete Existing CartID
                    if (ControllerContext.HttpContext.Response.Cookies["CartName"] != null)
                    {
                        ControllerContext.HttpContext.Response.Cookies["CartName"].Expires = DateTime.Now.AddDays(-1);
                    }
                    if (ControllerContext.HttpContext.Response.Cookies["CartID"] != null)
                    {
                        ControllerContext.HttpContext.Response.Cookies["CartID"].Expires = DateTime.Now.AddDays(-1);
                    }

                    Cart lCart = CanLoginForCart(pUserName, pPassword);
                    // FillExitingCart(lCart); ////hide
                    if (lCart != null)
                    {
                        FillExitingCart(lCart);////added 
                        userExist = db.UserLogins.Select(x => new { x.Email, x.Mobile, x.Password, x.ID, x.IsLocked })
                                                     .Where(x => (x.Email == pUserName || x.Mobile == pUserName) && x.IsLocked == false).ToList();
                    }


                    //Added by Zubair on 07-09-2017
                    //Partner/Franchise user can login with One time password and can order products, after taking permission from customer on mobile
                    UserLogin lUerLogin = db.UserLogins.FirstOrDefault(x => x.Mobile.Equals(pUserName) || x.Email.Equals(pUserName));
                    if (lUerLogin != null) //Yashaswi 27-7-2018
                    {
                        long ID = db.TempPasswords.Where(x => x.UserLoginID == lUerLogin.ID && x.TempPassword1 == pPassword && x.IsActive == true).Select(x => x.ID).FirstOrDefault();
                        if (ID != null && ID > 0)
                        {
                            userExist = db.UserLogins.Select(x => new { x.Email, x.Mobile, x.Password, x.ID, x.IsLocked })
                                                         .Where(x => (x.Email == pUserName || x.Mobile == pUserName) && x.IsLocked == false).ToList();

                            TempPassword lTempPassword2 = new TempPassword()
                            {
                                ID = ID,
                                LoginTime = DateTime.Now,
                                IsActive = false
                            };
                            db.TempPasswords.Attach(lTempPassword2);
                            db.Entry(lTempPassword2).Property(x => x.LoginTime).IsModified = true;
                            db.Entry(lTempPassword2).Property(x => x.IsActive).IsModified = true;
                            db.SaveChanges();
                        }
                    }
                    //End: Added by Zubair

                }
                //- End : Added by Avi Verma.



                if (userExist.Count() > 0)
                {
                    foreach (var item in userExist)
                    {
                        lDictUserDetails.Add("ID", item.ID.ToString());
                        /*to check whether Email Adderss is null stored 
                         * Pradnyakar Badge
                         * 05-01-2016
                         */
                        if (item.Email != null)
                        {
                            lDictUserDetails.Add("UserName", item.Email.ToString());
                        }
                        else
                        {
                            lDictUserDetails.Add("UserName", item.Mobile.ToString());
                        }
                    }
                }
                else
                {
                    var EmailMobExist = db.UserLogins.Where(x => (x.Email == pUserName || x.Mobile == pUserName)).ToList();
                    if (EmailMobExist.Count() <= 0)
                    {
                        msg = "This email/mobile is not registered with us.";
                    }
                    else
                    {
                        var pwdExist = db.UserLogins.Where(x => x.Password == pPassword && (x.Email == pUserName || x.Mobile == pUserName)).ToList();
                        if (pwdExist.Count() <= 0)
                        {
                            msg = "Incorrect password !please click on forgot password";
                        }
                        else
                        {
                            msg = "It seems, you are new to eZeelo! please register. ";
                        }
                    }
                }

                return lDictUserDetails;
            }
            catch (MyException myEx)
            {
                throw new BusinessLogicLayer.MyException("[CustomerController][M:CheckLogin]", "Can't check login details!" + Environment.NewLine + myEx.Message);
            }
            catch (Exception ex)
            {
                throw new BusinessLogicLayer.MyException("[CustomerController][M:CheckLogin]", "Can't check login details!" + Environment.NewLine + ex.Message);
            }
        }


        //- Start Change made by Avi Verma.
        //- Date : 07-Sep-2016.
        //- Reason : For Abandoned Cart.
        private Cart CreateVirtualAbandonedCart(long UserLoginID)
        {
            Cart lCart = new Cart();
            try
            {
                long cityID = 0;
                Franchise lFranchise = new Franchise();
                if (ControllerContext.HttpContext.Request.Cookies["CityCookie"] != null)
                {
                    string cookieValue = ControllerContext.HttpContext.Request.Cookies["CityCookie"].Value;
                    string[] arr = cookieValue.Split('$');
                    cityID = Convert.ToInt64(arr[0]);

                    lFranchise = db.Franchises.FirstOrDefault(x => x.BusinessDetail.Pincode.CityID == cityID);
                }

                TrackCartBusiness lTrackCartBusiness = new TrackCartBusiness();
                lCart = lTrackCartBusiness.CreateVirtualAbandonedCart(UserLoginID, cityID, lFranchise.ID, string.Empty, string.Empty);
                ControllerContext.HttpContext.Response.Cookies["CartName"].Value = lCart.Name;
                ControllerContext.HttpContext.Response.Cookies["CartID"].Value = lCart.ID.ToString();
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[LoginController][CreateCart]" + ex.Message,
                    BusinessLogicLayer.ErrorLog.Module.Gandhibagh, System.Web.HttpContext.Current.Server);
            }
            return lCart;
        }
        private Cart CanLoginForCart(string pUserName, string pPassword)
        {
            Cart lCart = null;
            try
            {
                UserLogin lUerLogin = db.UserLogins.FirstOrDefault(x => x.Mobile.Equals(pUserName) || x.Email.Equals(pUserName));
                if (lUerLogin == null)
                {
                    return null;
                }
                lCart = db.Carts.FirstOrDefault(x => x.UserLoginID == lUerLogin.ID && x.CartPassword == pPassword && x.IsActive == true);
            }
            catch (Exception)
            {
                return null;
            }
            return lCart;
        }
        private void FillExitingCart(Cart lCart)
        {
            ShoppingCartInitialization lShoppingCartInitialization = new ShoppingCartInitialization();
            lShoppingCartInitialization.DeleteShoppingCartCookie();
            List<TrackCart> lTrackCarts = db.TrackCarts.Where(x => x.CartID == lCart.ID && x.ShopStockID != null && x.Qty != null).ToList();
            foreach (TrackCart lTrackCart in lTrackCarts)
            {
                long lShopStockID = lTrackCart.ShopStockID != null ? (long)lTrackCart.ShopStockID : 1;
                int lQty = lTrackCart.Qty != null ? (int)lTrackCart.Qty : 1;
                lShoppingCartInitialization.SetCookie(lShopStockID, 1, lQty);
            }
        }
        //- End Change made by Avi Verma.
        [HttpGet]
        public ActionResult GuestCheckout()
        {
            /*
               Indents:
             * Description: This is used to guest checkout, this will only be call from payment process
             
             * Parameters: 
             
             * Precondition: 
             * Postcondition:
             * Logic: 
             */

            if (Request.QueryString["callFrom"] != null)
            {
                return PartialView("_GuestCheckOut");
            }
            else
            {
                return View("_GuestCheckOut", "_Layout_Internal");
            }
        }

        //[HttpPost]
        //public ActionResult GuestCheckout(GuestCheckoutViewModel model, string callFrom, bool isExpressBuy, string returnUrl)
        //{
        //    /*
        //       Indents:
        //     * Description: This is used to guest checkout, this will only be call from payment process

        //     * Parameters: 1) callFrom : this variable is used to check is the user coming from normal login or payment process.
        //     *             2) isExpressBuy : This variable is used to check is the buy process is normal or express. 
        //     *             3) returnUrl : is used to return the page to the calling page.

        //     * Precondition: 
        //     * Postcondition:
        //     * Logic:    1) If customer role is not exists then return
        //     *           2) Store the entire data of guest user in database
        //     *           2.1) Check user details exists or not, if yes then return
        //     *           2.2) Creating user login
        //     *           2.3) storing personal details
        //     *           2.4) Store role in UserRole table
        //     *           3) Insert OTP details and send OTP to customer
        //     *           4) Return to calling url
        //     */

        //    try
        //    {
        //        // Storing salutation in ViewBag
        //        ViewBag.salutation = new SelectList(db.Salutations, "ID", "Name");

        //        if (ModelState.IsValid)
        //        {
        //            var lRole = db.Roles.Where(x => x.Name == "Customer".ToUpper()).ToList();


        //            // 1. If customer role is not exists then return
        //            if (lRole.Count() <= 0)
        //            {
        //                TempData["MessageGuest"] = "Role not exist!!";

        //                if (isExpressBuy == true)
        //                    return RedirectToAction("CustomerPaymentProcess", "PaymentProcess", new { IsExpressBuy = true, checkout = "guest" });
        //                else
        //                    return RedirectToAction("CustomerPaymentProcess", "PaymentProcess", new { checkout = "guest" });
        //            }

        //            //  2. Store the entire data of guest user in database
        //            using (TransactionScope ts = new TransactionScope())
        //            {
        //                try
        //                {
        //                    string lMessage = string.Empty;
        //                    // 2.1 Check user details exists or not, if yes then return
        //                     lMessage = CommonFunctions.CheckUserDetails(model.EmailID, model.MobileNo);

        //                    if (lMessage != string.Empty)
        //                    {
        //                        TempData["MessageGuest"] = lMessage;

        //                        if (isExpressBuy == true)
        //                            return RedirectToAction("CustomerPaymentProcess", "PaymentProcess", new { IsExpressBuy = true, checkout = "guest" });
        //                        else
        //                            return RedirectToAction("CustomerPaymentProcess", "PaymentProcess", new { checkout = "guest" });
        //                    }

        //                    // 2.2 Creating user login
        //                    UserLogin userLogin = new UserLogin();

        //                    userLogin.ID = 0;
        //                    userLogin.Mobile = model.MobileNo;
        //                    userLogin.Email = model.EmailID;
        //                    userLogin.Password = "123456";
        //                    userLogin.IsLocked = false;
        //                    userLogin.CreateBy = 1;
        //                    userLogin.CreateDate = DateTime.UtcNow.AddHours(5.5);

        //                    db.UserLogins.Add(userLogin);
        //                    db.SaveChanges();

        //                    // Getting current user login id
        //                    Int64 id = userLogin.ID;

        //                    // 2.3 storing personal details
        //                    PersonalDetail personalDetail = new PersonalDetail();
        //                    personalDetail.ID = 0;
        //                    personalDetail.UserLoginID = id;
        //                    personalDetail.SalutationID = 1;
        //                    personalDetail.FirstName = "Guest"; // model.FirstName;
        //                    personalDetail.MiddleName = "";
        //                    personalDetail.LastName = "User";
        //                    personalDetail.IsActive = true;
        //                    personalDetail.CreateBy = 1;
        //                    personalDetail.CreateDate = DateTime.UtcNow.AddHours(5.5);

        //                    db.PersonalDetails.Add(personalDetail);
        //                    db.SaveChanges();

        //                    // 2.4 Store role in UserRole table
        //                    UserRole userRole = new UserRole();

        //                    userRole.ID = 0;
        //                    userRole.RoleID = lRole.FirstOrDefault().ID;
        //                    userRole.UserLoginID = id;
        //                    userRole.IsActive = true;
        //                    userRole.CreateDate = DateTime.Now;
        //                    userRole.CreateBy = CommonFunctions.GetPersonalDetailsID(id);

        //                    db.UserRoles.Add(userRole);
        //                    db.SaveChanges();

        //                                              // Clear model state
        //                    ModelState.Clear();

        //                    Session["UID"] = id;
        //                    Session["UserName"] = model.EmailID;

        //                    Dictionary<string, string> dictOTP = BusinessLogicLayer.OTP.GenerateOTP("USC");

        //                    this.InsertOTPDetails(dictOTP["USC"], dictOTP["OTP"]);
        //                    Session["OTPCode"] = dictOTP["USC"];
        //                    // Send OTP to customer
        //                    this.SendOTPToCustomer(model.MobileNo, dictOTP["OTP"]);

        //                    // Set model to NULL
        //                    model = null;
        //                    //Tejaswee 
        //                    //Add one more query string parameter for OTP verification
        //                    if (id > 0)
        //                    {
        //                        if (callFrom == "paymentProcess")
        //                        {
        //                            if (isExpressBuy == true)
        //                                return RedirectToAction("CustomerPaymentProcess", "PaymentProcess", new { IsExpressBuy = true, checkout = "guest", checkOTP = "True" });
        //                            else
        //                                return RedirectToAction("CustomerPaymentProcess", "PaymentProcess", new { checkout = "guest", checkOTP = "True" });
        //                        }
        //                    }

        //                    // Transaction complete
        //                    ts.Complete();
        //                }
        //                catch (Exception exception)
        //                {
        //                    TempData["MessageGuest"] = "Sorry! Problem in customer registration!!";

        //                    // Rollback transaction
        //                    ts.Dispose();

        //                    if (isExpressBuy == true)
        //                        return RedirectToAction("CustomerPaymentProcess", "PaymentProcess", new { IsExpressBuy = true, checkout = "guest", checkOTP = "True" });
        //                    else
        //                        return RedirectToAction("CustomerPaymentProcess", "PaymentProcess", new { checkout = "guest", checkOTP = "True" });
        //                }
        //            }
        //        }
        //        TempData["MessageGuest"] = "Please enter the details !!";

        //        if (isExpressBuy == true)
        //            return RedirectToAction("CustomerPaymentProcess", "PaymentProcess", new { IsExpressBuy = true, checkout = "guest" });
        //        else
        //            return RedirectToAction("CustomerPaymentProcess", "PaymentProcess", new { checkout = "guest" });
        //    }
        //    catch (BusinessLogicLayer.MyException myEx)
        //    {
        //        ViewBag.Message = "Sorry! Problem in customer registration!!";

        //        BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
        //            + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
        //            + "[CustomerController][POST:Create]" + myEx.EXCEPTION_PATH,
        //            BusinessLogicLayer.ErrorLog.Module.Gandhibagh, System.Web.HttpContext.Current.Server);

        //        return View();
        //    }
        //    catch (Exception ex)
        //    {
        //        ViewBag.Message = "Sorry! Problem in customer registration!!";

        //        BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
        //            + Environment.NewLine + ex.Message + Environment.NewLine
        //            + "[CustomerController][POST:Create]",
        //            BusinessLogicLayer.ErrorLog.Module.Gandhibagh, System.Web.HttpContext.Current.Server);

        //        return View();
        //    }
        //}


        [HttpPost]
        public ActionResult GuestCheckout(GuestCheckoutViewModel model, string callFrom, bool isExpressBuy, string returnUrl)
        {
            /*
               Indents:
             * Description: This is used to guest checkout, this will only be call from payment process
             
             * Parameters: 1) callFrom : this variable is used to check is the user coming from normal login or payment process.
             *             2) isExpressBuy : This variable is used to check is the buy process is normal or express. 
             *             3) returnUrl : is used to return the page to the calling page.
             
             * Precondition: 
             * Postcondition:
             * Logic:    1) If customer role is not exists then return
             *           2) Store the entire data of guest user in database
             *           2.1) Check user details exists or not, if yes then return
             *           2.2) Creating user login
             *           2.3) storing personal details
             *           2.4) Store role in UserRole table
             *           3) Insert OTP details and send OTP to customer
             *           4) Return to calling url
             */

            try
            {
                // Storing salutation in ViewBag
                ViewBag.salutation = new SelectList(db.Salutations, "ID", "Name");

                if (ModelState.IsValid)
                {
                    var lRole = db.Roles.Where(x => x.Name == "Customer".ToUpper()).ToList();


                    // 1. If customer role is not exists then return
                    if (lRole.Count() <= 0)
                    {
                        TempData["MessageGuest"] = "Role not exist!!";

                        if (isExpressBuy == true)
                            return RedirectToAction("CustomerPaymentProcess", "PaymentProcess", new { IsExpressBuy = true, checkout = "guest" });
                        else
                            return RedirectToAction("CustomerPaymentProcess", "PaymentProcess", new { checkout = "guest" });
                    }

                    //  2. Store the entire data of guest user in database
                    //using (TransactionScope ts = new TransactionScope())
                    {
                        try
                        {
                            string lMessage = string.Empty;
                            // 2.1 Check user details exists or not, if yes then return

                            lMessage = CommonFunctions.CheckUserDetails(model.EmailID, model.MobileNo);

                            if (lMessage != string.Empty)
                            {
                                //if (lMessage == "Email and Mobile No. already exist!!")
                                //{
                                //    if (isExpressBuy == true)
                                //        return RedirectToAction("CustomerPaymentProcess", "PaymentProcess", new { IsExpressBuy = true, checkout = "guest", checkOTP = "True" });
                                //    else
                                //        return RedirectToAction("CustomerPaymentProcess", "PaymentProcess", new { checkout = "guest", checkOTP = "True" });
                                //}
                                //else
                                //{
                                TempData["MessageGuest"] = lMessage;

                                if (isExpressBuy == true)
                                    return RedirectToAction("CustomerPaymentProcess", "PaymentProcess", new { IsExpressBuy = true, checkout = "guest" });
                                else
                                    return RedirectToAction("CustomerPaymentProcess", "PaymentProcess", new { checkout = "guest" });
                                //}
                            }

                            // 2.2 Creating user login
                            UserLogin userLogin = new UserLogin();

                            userLogin.ID = 0;
                            userLogin.Mobile = model.MobileNo;
                            TempData["ResendOTPMobileNo"] = model.MobileNo;
                            userLogin.Email = model.EmailID;
                            userLogin.Password = "123456";
                            userLogin.IsLocked = false;
                            userLogin.CreateBy = 1;
                            userLogin.CreateDate = DateTime.UtcNow.AddHours(5.5);

                            db.UserLogins.Add(userLogin);
                            db.SaveChanges();

                            // Getting current user login id
                            Int64 id = userLogin.ID;

                            // 2.3 storing personal details
                            PersonalDetail personalDetail = new PersonalDetail();
                            personalDetail.ID = 0;
                            personalDetail.UserLoginID = id;
                            personalDetail.SalutationID = 1;
                            personalDetail.FirstName = "Guest"; // model.FirstName;
                            personalDetail.MiddleName = "";
                            personalDetail.LastName = "User";
                            personalDetail.IsActive = true;
                            personalDetail.CreateBy = 1;
                            personalDetail.CreateDate = DateTime.UtcNow.AddHours(5.5);

                            db.PersonalDetails.Add(personalDetail);
                            db.SaveChanges();

                            // 2.4 Store role in UserRole table
                            UserRole userRole = new UserRole();

                            userRole.ID = 0;
                            userRole.RoleID = lRole.FirstOrDefault().ID;
                            userRole.UserLoginID = id;
                            userRole.IsActive = true;
                            userRole.CreateDate = DateTime.Now;
                            userRole.CreateBy = CommonFunctions.GetPersonalDetailsID(id);

                            db.UserRoles.Add(userRole);
                            db.SaveChanges();

                            // Clear model state
                            ModelState.Clear();

                            Session["UID"] = id;
                            /*For Nullable Email Address
                             * Pradnyakar Badge 
                             * 06-01-2016                             
                             */
                            if (model.EmailID != null)
                            {
                                Session["UserName"] = model.EmailID;
                            }
                            else
                            {
                                Session["UserName"] = model.MobileNo;
                            }

                            Dictionary<string, string> dictOTP = BusinessLogicLayer.OTP.GenerateOTP("USC");

                            this.InsertOTPDetails(dictOTP["USC"], dictOTP["OTP"]);
                            Session["OTPCode"] = dictOTP["USC"];
                            // Send OTP to customer
                            //OTP functionality is exluded now 
                            //this.SendOTPToCustomer(model.MobileNo, dictOTP["OTP"]);

                            // Set model to NULL
                            model = null;
                            //Tejaswee 
                            //Add CheckOTP parameter for OTP verification
                            //if (id > 0)
                            //{
                            //    if (callFrom == "paymentProcess")
                            //    {
                            //        if (isExpressBuy == true)
                            //            return RedirectToAction("CustomerPaymentProcess", "PaymentProcess", new { IsExpressBuy = true, checkout = "guest", checkOTP = "True" });
                            //        else
                            //            return RedirectToAction("CustomerPaymentProcess", "PaymentProcess", new { checkout = "guest", checkOTP = "True" });
                            //    }
                            //}

                            if (id > 0)
                            {
                                if (callFrom == "paymentProcess")
                                {
                                    if (isExpressBuy == true)
                                        return RedirectToAction("CustomerPaymentProcess", "PaymentProcess", new { IsExpressBuy = true, checkout = "guest" });
                                    else
                                        return RedirectToAction("CustomerPaymentProcess", "PaymentProcess", new { checkout = "guest" });
                                }
                            }

                            // Transaction complete
                            //ts.Complete();
                        }
                        catch (Exception exception)
                        {
                            TempData["MessageGuest"] = "Sorry! Problem in customer registration!!";

                            // Rollback transaction
                            //ts.Dispose();

                            if (isExpressBuy == true)
                                return RedirectToAction("CustomerPaymentProcess", "PaymentProcess", new { IsExpressBuy = true, checkout = "guest", checkOTP = "True" });
                            else
                                return RedirectToAction("CustomerPaymentProcess", "PaymentProcess", new { checkout = "guest", checkOTP = "True" });
                        }
                    }
                }
                TempData["MessageGuest"] = "Please enter the details !!";

                if (isExpressBuy == true)
                    return RedirectToAction("CustomerPaymentProcess", "PaymentProcess", new { IsExpressBuy = true, checkout = "guest" });
                else
                    return RedirectToAction("CustomerPaymentProcess", "PaymentProcess", new { checkout = "guest" });
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                ViewBag.Message = "Sorry! Problem in customer registration!!";

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[CustomerController][POST:Create]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Gandhibagh, System.Web.HttpContext.Current.Server);

                return View();
            }
            catch (Exception ex)
            {
                ViewBag.Message = "Sorry! Problem in customer registration!!";

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[CustomerController][POST:Create]",
                    BusinessLogicLayer.ErrorLog.Module.Gandhibagh, System.Web.HttpContext.Current.Server);

                return View();
            }
        }

        //public ActionResult CheckOTP(CheckOTP checkOTP, string callFrom, bool isExpressBuy)
        //{
        //    if (Session["OTPCode"] != null)
        //    {
        //        bool flag = this.IsOTPValid(Session["OTPCode"].ToString(), checkOTP.OTP);
        //        if (flag == true)
        //        {
        //            //PaymentProcess/CustomerPaymentProcess?checkout=guest
        //            if (isExpressBuy == true)
        //                return RedirectToAction("CustomerPaymentProcess", "PaymentProcess", new { IsExpressBuy = true });
        //            else
        //                return RedirectToAction("CustomerPaymentProcess", "PaymentProcess");
        //        }
        //        else
        //        {
        //            TempData["MessageGuest"] = "Please! Enter Correct OTP.";

        //            if (isExpressBuy == true)
        //                return RedirectToAction("CustomerPaymentProcess", "PaymentProcess", new { IsExpressBuy = true, checkout = "guest", checkOTP = "True" });
        //            else
        //                return RedirectToAction("CustomerPaymentProcess", "PaymentProcess", new { checkout = "guest", checkOTP = "True" });
        //        }
        //    }
        //    else
        //    {
        //        TempData["MessageGuest"] = "Your OTP Session Is Expired. Please! Regenerate OTP";
        //        if (isExpressBuy == true)
        //            return RedirectToAction("CustomerPaymentProcess", "PaymentProcess", new { IsExpressBuy = true, checkout = "guest", checkOTP = "True" });
        //        else
        //            return RedirectToAction("CustomerPaymentProcess", "PaymentProcess", new { checkout = "guest", checkOTP = "True" });
        //    }
        //    //return View();
        //}

        public ActionResult CheckOTP(CheckOTP checkOTP, string callFrom, bool isExpressBuy)
        {
            if (Session["OTPCode"] != null)
            {
                bool flag = this.IsOTPValid(Session["OTPCode"].ToString(), checkOTP.OTP);
                if (flag == true)
                {
                    //PaymentProcess/CustomerPaymentProcess?checkout=guest
                    if (isExpressBuy == true)
                        return RedirectToAction("CustomerPaymentProcess", "PaymentProcess", new { IsExpressBuy = true });
                    else
                        return RedirectToAction("CustomerPaymentProcess", "PaymentProcess");
                }
                else
                {
                    TempData["MessageGuest"] = "Please! Enter Correct OTP.";

                    if (isExpressBuy == true)
                        return RedirectToAction("CustomerPaymentProcess", "PaymentProcess", new { IsExpressBuy = true, checkout = "guest", checkOTP = "True" });
                    else
                        return RedirectToAction("CustomerPaymentProcess", "PaymentProcess", new { checkout = "guest", checkOTP = "True" });
                }
            }
            else
            {
                TempData["MessageGuest"] = "Your OTP Session Is Expired. Please! Regenerate OTP";
                if (isExpressBuy == true)
                    return RedirectToAction("CustomerPaymentProcess", "PaymentProcess", new { IsExpressBuy = true, checkout = "guest", checkOTP = "True" });
                else
                    return RedirectToAction("CustomerPaymentProcess", "PaymentProcess", new { checkout = "guest", checkOTP = "True" });
            }
            //return View();
        }

        //public ActionResult ResendOTP(GuestCheckoutViewModel model, bool isExpressBuy)
        //{
        //    try
        //    {

        //        Dictionary<string, string> dictOTP = BusinessLogicLayer.OTP.GenerateOTP("USC");

        //        this.InsertOTPDetails(dictOTP["USC"], dictOTP["OTP"]);
        //        Session["OTPCode"] = dictOTP["USC"];
        //        // Send OTP to customer
        //        this.SendOTPToCustomer(model.MobileNo, dictOTP["OTP"]);
        //        if (isExpressBuy == true)
        //            return RedirectToAction("CustomerPaymentProcess", "PaymentProcess", new { IsExpressBuy = true, checkout = "guest", checkOTP = "True" });
        //        else
        //            return RedirectToAction("CustomerPaymentProcess", "PaymentProcess", new { checkout = "guest", checkOTP = "True" });
        //    }
        //    catch (Exception)
        //    {

        //        throw;
        //    }
        //    //return View();
        //}

        public ActionResult ResendOTP(GuestCheckoutViewModel model, bool isExpressBuy)
        {
            try
            {

                if (TempData["ResendOTPMobileNo"] != null)
                {

                    Dictionary<string, string> dictOTP = BusinessLogicLayer.OTP.GenerateOTP("USC");

                    this.InsertOTPDetails(dictOTP["USC"], dictOTP["OTP"]);
                    Session["OTPCode"] = dictOTP["USC"];
                    // Send OTP to customer
                    this.SendOTPToCustomer(TempData["ResendOTPMobileNo"].ToString(), dictOTP["OTP"]);
                    TempData.Keep();
                    if (isExpressBuy == true)
                        return RedirectToAction("CustomerPaymentProcess", "PaymentProcess", new { IsExpressBuy = true, checkout = "guest", checkOTP = "True" });
                    else
                        return RedirectToAction("CustomerPaymentProcess", "PaymentProcess", new { checkout = "guest", checkOTP = "True" });
                }
            }
            catch (Exception)
            {

                throw;
            }
            return View();
        }

        //private void SendOTPToCustomer(string guestMobileNumber, string OTP)
        //{
        //    /*
        //      Indents:
        //    * Description: This method is used to send OTP to customer

        //    * Parameters: 

        //    * Precondition: 
        //    * Postcondition:
        //    * Logic: 
        //    */

        //    try
        //    {
        //        Dictionary<string, string> dictSMSValues = new Dictionary<string, string>();

        //        dictSMSValues.Add("#--NAME--#", "Customer");
        //        dictSMSValues.Add("#--ORD_NUM--#", OTP);

        //        BusinessLogicLayer.GateWay gateWay = new BusinessLogicLayer.SMS(System.Web.HttpContext.Current.Server);

        //        gateWay.SendSMS(BusinessLogicLayer.GateWay.SMSGateWays.SUMIT, BusinessLogicLayer.GateWay.SMSOptions.SINGLE, BusinessLogicLayer.GateWay.SMSTypes.OTP_CUST_REG, new string[] { guestMobileNumber }, dictSMSValues);
        //    }
        //    catch (BusinessLogicLayer.MyException myEx)
        //    {
        //        BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
        //            + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
        //            + "[PaymentProcessController][M:SendOTPToCustomer]" + myEx.EXCEPTION_PATH,
        //            BusinessLogicLayer.ErrorLog.Module.Gandhibagh, System.Web.HttpContext.Current.Server);
        //    }
        //    catch (Exception ex)
        //    {
        //        BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
        //            + Environment.NewLine + ex.Message + Environment.NewLine
        //            + "[PaymentProcessController][M:SendOTPToCustomer]",
        //            BusinessLogicLayer.ErrorLog.Module.Gandhibagh, System.Web.HttpContext.Current.Server);
        //    }
        //}

        private void SendOTPToCustomer(string guestMobileNumber, string OTP)
        {
            /*
              Indents:
            * Description: This method is used to send OTP to customer
             
            * Parameters: 
             
            * Precondition: 
            * Postcondition:
            * Logic: 
            */

            try
            {
                Dictionary<string, string> dictSMSValues = new Dictionary<string, string>();

                dictSMSValues.Add("#--NAME--#", "Customer");
                dictSMSValues.Add("#--OTP--#", OTP);

                BusinessLogicLayer.GateWay gateWay = new BusinessLogicLayer.SMS(System.Web.HttpContext.Current.Server);

                gateWay.SendSMS(BusinessLogicLayer.GateWay.SMSGateWays.SUMIT, BusinessLogicLayer.GateWay.SMSOptions.SINGLE, BusinessLogicLayer.GateWay.SMSTypes.OTP_CUST_REG, new string[] { guestMobileNumber }, dictSMSValues);
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[PaymentProcessController][M:SendOTPToCustomer]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Gandhibagh, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[PaymentProcessController][M:SendOTPToCustomer]",
                    BusinessLogicLayer.ErrorLog.Module.Gandhibagh, System.Web.HttpContext.Current.Server);
            }
        }

        //private void InsertOTPDetails(string sessionCode, string oneTimePassword)
        //{
        //    /*
        //      Indents:
        //    * Description: This method is used to insert OTP in OTP table

        //    * Parameters: 

        //    * Precondition: 
        //    * Postcondition:
        //    * Logic: 
        //    */

        //    try
        //    {
        //            ModelLayer.Models.OTP otp = new ModelLayer.Models.OTP();

        //            otp.ID = 0;
        //            otp.SessionCode = sessionCode;
        //            otp.OTP1 = oneTimePassword;
        //            otp.IsActive = true;
        //            otp.CreateDate = CommonFunctions.GetLocalTime();
        //            otp.ExpirationTime = CommonFunctions.GetLocalTime().AddMinutes(10);
        //            otp.CreateBy = 1;

        //            db.OTPs.Add(otp);
        //            db.SaveChanges();

        //    }
        //    catch (Exception)
        //    {

        //        throw;
        //    }
        //}

        private void InsertOTPDetails(string sessionCode, string oneTimePassword)
        {
            /*
              Indents:
            * Description: This method is used to insert OTP in OTP table
             
            * Parameters: 
             
            * Precondition: 
            * Postcondition:
            * Logic: 
            */

            try
            {
                ModelLayer.Models.OTP otp = new ModelLayer.Models.OTP();

                otp.ID = 0;
                otp.SessionCode = sessionCode;
                otp.OTP1 = oneTimePassword;
                otp.IsActive = true;
                otp.CreateDate = CommonFunctions.GetLocalTime();
                otp.ExpirationTime = CommonFunctions.GetLocalTime().AddMinutes(10);
                otp.CreateBy = 1;

                db.OTPs.Add(otp);
                db.SaveChanges();

            }
            catch (Exception)
            {

                throw;
            }
        }

        private void IsCouponUsedByCustomer(long userLoginId, bool IsExpressBuy)
        {
            /*
              Indents:
            * Description:
             
            * Parameters: 
             
            * Precondition: 
            * Postcondition:
            * Logic: 
            */


            ShopProductVarientViewModelCollection lShoppingCartCollection = new ShopProductVarientViewModelCollection();

            if (IsExpressBuy == true)
                lShoppingCartCollection = (ShopProductVarientViewModelCollection)TempData["ExpressBuyCollection"];
            else
                lShoppingCartCollection = (ShopProductVarientViewModelCollection)TempData["CartCollection"];

            if (lShoppingCartCollection != null)
            {
                if (ControllerContext.HttpContext.Request.Cookies["CouponManagementCookie"] != null && ControllerContext.HttpContext.Request.Cookies["CouponManagementCookie"].Value != string.Empty)
                {
                    string[] cookieDetails = ControllerContext.HttpContext.Request.Cookies["CouponManagementCookie"].Value.Split('$');

                    foreach (var item in lShoppingCartCollection.lShopProductVarientViewModel)
                    {
                        if (Convert.ToInt64(cookieDetails[0]) == item.ShopStockID)
                        {
                            BusinessLogicLayer.CouponManagement coupon = new BusinessLogicLayer.CouponManagement();
                            long cityId = Convert.ToInt64(ControllerContext.HttpContext.Request.Cookies["CityCookie"].Value.Split('$')[0]);
                            int franchiseId = Convert.ToInt32(ControllerContext.HttpContext.Request.Cookies["CityCookie"].Value.Split('$')[2]);//added
                            DataTable dt = coupon.CheckCouponCode(cookieDetails[1], item.ShopID, item.ProductID, CommonFunctions.GetPersonalDetailsID(userLoginId), cityId, franchiseId);//added cityId->franchiseId old
                            if (dt.Rows.Count > 0)
                            {
                                if (Convert.ToString(dt.Rows[0]["VALIDITY CODE"]) == "8")
                                {
                                    TempData["CouponMessage"] = "Coupon is already used. Please remove items from shopping cart";

                                    //HttpCookie CouponManagementCookie = new HttpCookie("CouponManagementCookie");
                                    ////Delete whole cookie
                                    //if (ControllerContext.HttpContext.Request.Cookies["CouponManagementCookie"] != null)
                                    //{
                                    //    CouponManagementCookie.Expires = DateTime.Now.AddDays(-1);
                                    //    ControllerContext.HttpContext.Response.Cookies.Add(CouponManagementCookie);
                                    //}
                                    //if (CouponManagementCookie.Expires < DateTime.Now)
                                    //{
                                    //    ControllerContext.HttpContext.Request.Cookies.Remove("CouponManagementCookie");
                                    //}

                                    //Response.Redirect("~/ShoppingCart/Index");
                                    RedirectToAction("Index", "ShoppingCart");
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Tejaswee 
        /// Check Entered OTP is valid or not
        /// </summary>
        /// <param name="OTPSessionCode"></param>
        /// <param name="OTP"></param>
        //private bool IsOTPValid(string OTPSessionCode,string OTP)
        //{
        //    try
        //    {
        //        var orgProducts = from a in db.OTPs
        //                          where a.SessionCode == OTPSessionCode && a.OTP1 == OTP && a.ExpirationTime <= CommonFunctions.GetLocalTime()
        //            select a;
        //        if (orgProducts != null)
        //            return true;
        //        else
        //            return false;
        //    }
        //    catch (Exception)
        //    {

        //        throw;
        //    }
        //}

        /// <summary>
        /// Tejaswee 
        /// Check Entered OTP is valid or not
        /// </summary>
        /// <param name="OTPSessionCode"></param>
        /// <param name="OTP"></param>
        private bool IsOTPValid(string OTPSessionCode, string OTP)
        {
            try
            {

                //var orgProducts = from a in db.OTPs
                //                  where a.SessionCode == OTPSessionCode && a.OTP1 == OTP && a.ExpirationTime <= CommonFunctions.GetLocalTime()
                //    select a;
                var orgProducts = db.OTPs.SingleOrDefault(x => x.SessionCode.ToUpper() == OTPSessionCode.ToUpper() && x.OTP1.ToUpper() == OTP.ToUpper());

                if (orgProducts != null)
                    return true;
                else
                    return false;
            }
            catch (Exception)
            {

                throw;
            }
        }


        /// <summary>
        ///This function is used when forgot password functionality is same as flipkart
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>

        //[HttpPost]
        //public JsonResult CheckEmailMobile(string userName)
        //{
        //    ViewBag.forgetUserName = "";
        //    var lUser = db.UserLogins.Where((x => x.Email == userName || x.Mobile==userName));
        //    if (lUser.Count() > 0)
        //    {
        //        ViewBag.forgetUserName = lUser;
        //        Dictionary<string, string> dictOTP = BusinessLogicLayer.OTP.GenerateOTP("USC");

        //        this.InsertOTPDetails(dictOTP["USC"], dictOTP["OTP"]);
        //        Session["OTPCode"] = dictOTP["USC"];
        //        //Send OTP to customer
        //        this.SendOTPToCustomerOnEmail(userName, dictOTP["OTP"]);
        //        return Json(new { ok = true, newurl = Url.Action("Login", "ForgotPassword" )});
        //        //return Json("1");

        //    }
        //    return Json(new { ok = false });
        //}

        //[HttpPost]
        public JsonResult SendForgotPassword(string pUsername)
        {
            //string lUsername = pUsername.Trim();
            string lPassword = string.Empty;
            if (string.IsNullOrEmpty(pUsername))
            {
                return Json("Please enter username.", JsonRequestBehavior.AllowGet);
            }
            UserLogin lUserLogin = db.UserLogins.SingleOrDefault(x => x.Mobile == pUsername || x.Email == pUsername);
            if (lUserLogin == null)
            {
                //return Json("New to eZeelo. Please register", JsonRequestBehavior.AllowGet);
                return Json("This email/mobile is not registered with us please check", JsonRequestBehavior.AllowGet);
            }

            //Send new password
            //lPassword = GenerateRandomString();
            //Boolean lStatus = UpdateNewPassword(pUsername, lPassword);
            //if (lStatus == false)
            //{
            //    return Json("We are getting some problem in resetting your Password.", JsonRequestBehavior.AllowGet);
            //}

            //Send existing password
            lPassword = UpdateNewPassword(pUsername);
            if (lPassword == string.Empty)
            {
                return Json("We are getting some problem in resetting your Password.", JsonRequestBehavior.AllowGet);
            }
            string FirstName = db.PersonalDetails.Where(x => x.UserLoginID == lUserLogin.ID).Select(x => x.FirstName).FirstOrDefault();//Fetch FirstName of User By Sonali on 12-02-2019
            SendPasswordToCustomerOnEmail(lUserLogin.Email, lPassword);  //This function send new password on customer's email address
            SendPasswordToCustomer(lUserLogin.Mobile, lPassword, FirstName);//Added FirstName of User By Sonali on 12-02-2019

            return Json("True", JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Generate new password and update entry in User login table and send new password to custmer
        /// </summary>
        /// <param name="pUsername"></param>
        /// <param name="pNewPassword"></param>
        /// <returns></returns>
        private Boolean UpdateNewPassword(string pUsername, string pNewPassword)
        {
            Boolean lStatus = false;
            try
            {
                pUsername = pUsername.Trim();
                pNewPassword = pNewPassword.Trim();
                UserLogin lUserLogin = db.UserLogins.SingleOrDefault(x => x.Mobile.Equals(pUsername) || x.Email.Equals(pUsername));
                lUserLogin.Password = pNewPassword;
                lUserLogin.ModifyBy = CommonFunctions.GetPersonalDetailsID(lUserLogin.ID);
                lUserLogin.ModifyDate = DateTime.Now;
                if (ModelState.IsValid)
                {
                    db.Entry(lUserLogin).State = EntityState.Modified;
                    db.SaveChanges();
                    lStatus = true;
                }
            }
            catch (Exception ex)
            {
                lStatus = false;
            }
            return lStatus;
        }

        /// <summary>
        /// Fetch existing password from database and send to customer
        /// </summary>
        /// <param name="pUsername"></param>
        /// <param name="pNewPassword"></param>
        /// <returns></returns>
        private string UpdateNewPassword(string pUsername)
        {
            string pwd = string.Empty;
            try
            {
                pUsername = pUsername.Trim();
                UserLogin lUserLogin = db.UserLogins.SingleOrDefault(x => x.Mobile.Equals(pUsername) || x.Email.Equals(pUsername));
                pwd = lUserLogin.Password;
            }
            catch (Exception ex)
            {
                pwd = string.Empty;
            }
            return pwd;
        }

        private string GenerateRandomString()
        {
            Random rnd = new Random();
            int month = rnd.Next(1, 13); // creates a number between 1 and 12
            int dice = rnd.Next(1, 7);   // creates a number between 1 and 6
            int card = rnd.Next(52);
            return month.ToString("0") + dice.ToString("0") + card.ToString("0");
        }


        public ActionResult ForgotPassword()
        {
            try
            {
                return View();
            }
            catch (Exception)
            {

                throw;
            }

        }

        private void SendPasswordToCustomerOnEmail(string email, string PWD)
        {
            /*
              Indents:
            * Description: This method is used to send Password to customer on mail
             
            * Parameters: 
             
            * Precondition: 
            * Postcondition:
            * Logic: 
            */

            try
            {
                EzeeloDBContext db = new EzeeloDBContext();

                Dictionary<string, string> dictEmailValues = new Dictionary<string, string>();

                dictEmailValues.Add("<!--PWD-->", PWD);

                BusinessLogicLayer.GateWay gateWay = new BusinessLogicLayer.Email(System.Web.HttpContext.Current.Server);

                gateWay.SendEmail(BusinessLogicLayer.GateWay.EmailGateWays.GANDHIBAGH, BusinessLogicLayer.GateWay.SenderMail.INFO, BusinessLogicLayer.GateWay.EMailTypes.FORGET_PASS, new string[] { email }, dictEmailValues, true);
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[PaymentProcessController][M:SendPasswordToCustomerOnEmail]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Gandhibagh, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[PaymentProcessController][M:SendPasswordToCustomerOnEmail]",
                    BusinessLogicLayer.ErrorLog.Module.Gandhibagh, System.Web.HttpContext.Current.Server);
            }
        }


        private void SendOTPToCustomerOnEmail(string email, string OTP)
        {
            /*
              Indents:
            * Description: This method is used to send OTP to customer on mail
             
            * Parameters: 
             
            * Precondition: 
            * Postcondition:
            * Logic: 
            */

            try
            {
                EzeeloDBContext db = new EzeeloDBContext();

                Dictionary<string, string> dictEmailValues = new Dictionary<string, string>();

                dictEmailValues.Add("<!--OTP-->", OTP);

                BusinessLogicLayer.GateWay gateWay = new BusinessLogicLayer.Email(System.Web.HttpContext.Current.Server);

                gateWay.SendEmail(BusinessLogicLayer.GateWay.EmailGateWays.GANDHIBAGH, BusinessLogicLayer.GateWay.SenderMail.INFO, BusinessLogicLayer.GateWay.EMailTypes.CUST_OTP, new string[] { email }, dictEmailValues, true);
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[PaymentProcessController][M:SendOTPToCustomer]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Gandhibagh, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[PaymentProcessController][M:SendOTPToCustomer]",
                    BusinessLogicLayer.ErrorLog.Module.Gandhibagh, System.Web.HttpContext.Current.Server);
            }
        }


        private void SendPasswordToCustomer(string guestMobileNumber, string password, string Name)//Name added by Sonali on 12-02-2019
        {
            /*
              Indents:
            * Description: This method is used to send OTP to customer

            * Parameters: 

            * Precondition: 
            * Postcondition:
            * Logic: 
            */

            try
            {
                Dictionary<string, string> dictSMSValues = new Dictionary<string, string>();
                dictSMSValues.Add("#--NAME--#", Name);//Name added by Sonali on 12-02-2019
                dictSMSValues.Add("#--PASSWORD--#", password);

                BusinessLogicLayer.GateWay gateWay = new BusinessLogicLayer.SMS(System.Web.HttpContext.Current.Server);

                gateWay.SendSMS(BusinessLogicLayer.GateWay.SMSGateWays.SUMIT, BusinessLogicLayer.GateWay.SMSOptions.SINGLE, BusinessLogicLayer.GateWay.SMSTypes.CUST_SEND_PWD, new string[] { guestMobileNumber }, dictSMSValues);
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[PaymentProcessController][M:SendOTPToCustomer]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Gandhibagh, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[PaymentProcessController][M:SendOTPToCustomer]",
                    BusinessLogicLayer.ErrorLog.Module.Gandhibagh, System.Web.HttpContext.Current.Server);
            }
        }

        public string CreatePassword(int length)
        {
            const string valid = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            StringBuilder res = new StringBuilder();
            Random rnd = new Random();
            while (0 < length--)
            {
                res.Append(valid[rnd.Next(valid.Length)]);
            }
            return res.ToString();
        }



        /*External Login Methods and Action 
         * Created By :- Pradnyakar Badge
         * Dated :-      23/01/2016
         * Purpose:      Allow user to login with FB and Google         
         */
        public JsonResult testExternalLogin(string email, string callFrom, string externalCallBackURL, string userName)
        {
            ExternalLoginviewModel externalLogin_detail = new ExternalLoginviewModel();
            if (Session["UID"] == null)
            {

                var externalLogin = db.UserLogins.Where(x => x.Email == email).FirstOrDefault();

                if (externalLogin != null)
                {
                    LoginViewModel model = new LoginViewModel();
                    model.UserName = email;
                    model.Password = externalLogin.Password;
                    this.Login(model, callFrom, false, externalCallBackURL);
                    externalLogin_detail.email = externalLogin.Email;
                    externalLogin_detail.password = externalLogin_detail.password;
                    externalLogin_detail.result = 1;
                }
                else
                {
                    RegisterNewUser regUser = new RegisterNewUser();
                    CustomerRegistrationViewModel model = new CustomerRegistrationViewModel();
                    model.EmailId = email;
                    model.Password = CreatePassword(10);
                    model.FirstName = userName;
                    regUser.CreateNew_Account(model);
                    externalLogin_detail.email = email;
                    externalLogin_detail.password = model.Password;
                    externalLogin_detail.result = 0;
                }
            }

            return Json(externalLogin_detail, JsonRequestBehavior.AllowGet);
        }



        public ActionResult ExternalLogin(string mediaType, string email, string userName, string callFrom)
        {
            CarryData obj = new CarryData();
            obj = (CarryData)TempData["carryData"];
            ViewBag.returnurl = Request.Cookies["UrlCookie"].Value;
            ViewBag.callFrom = callFrom;
            ViewBag.mediaType = mediaType;
            TempData.Keep();
            return View();
        }

        [HttpPost]
        public ActionResult ExternalLogin(string email, string callFrom, string userName)
        {
            string returnurl;

            if (Request.Cookies["UrlCookie"] != null && Request.Cookies["UrlCookie"].Value != string.Empty)
            {
                returnurl = Request.Cookies["UrlCookie"].Value;
            }
            else
            {
                string cityName = Request.Cookies["CityCookie"].Value.Split('$')[1].ToLower();
                int franchiseID = Convert.ToInt32(Request.Cookies["CityCookie"].Value.Split('$')[2]);////added
                returnurl = Url.Action("ExternalLogin", "Login", new { city = cityName, franchiseId = franchiseID });////added franchiseId
            }

            ExternalLoginviewModel externalLogin_detail = new ExternalLoginviewModel();
            if (Session["UID"] == null)
            {

                var externalLogin = db.UserLogins.Where(x => x.Email == email).FirstOrDefault();

                if (externalLogin != null)
                {
                    LoginViewModel model = new LoginViewModel();
                    model.UserName = email;
                    model.Password = externalLogin.Password;
                    string msg = string.Empty;
                    Dictionary<string, string> lDictLoginDetails = this.CheckLogin(model.UserName, model.Password, out msg);



                    if (lDictLoginDetails.Count() <= 0) // 2.1
                    {
                        //ViewBag.Message = "Invalid UserName/Password!!";
                        TempData["Message"] = "Invalid UserName/Password!!";
                        if (callFrom == "paymentProcess")
                        {
                            //if (isExpressBuy == true)
                            //    return RedirectToAction("CustomerPaymentProcess", "PaymentProcess", new { IsExpressBuy = true });
                            //else
                            // return RedirectToAction("CustomerPaymentProcess", "PaymentProcess");
                            return RedirectToRoute("PaymentProcess", new { city = Request.Cookies["CityCookie"].Value.Split('$')[1].ToLower(), franchiseId = Request.Cookies["CityCookie"].Value.Split('$')[2] });////added franchiseId

                        }

                        return View("_Login", "_Layout");
                    }
                    else //2.2
                    {
                        Session["UID"] = lDictLoginDetails["ID"];
                        Session["UserName"] = lDictLoginDetails["UserName"];

                        long userLoginId = 0;
                        long.TryParse(Convert.ToString(Session["UID"]), out userLoginId);

                        Session["FirstName"] = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(db.PersonalDetails.Find(CommonFunctions.GetPersonalDetailsID(userLoginId)).FirstName.ToLower());

                        if (callFrom == "paymentProcess")  // 2.2.1
                        {
                            //change by harshada to redirect to subscription page
                            if (TempData["Subscription"] != null)
                            {
                                return RedirectToAction("Index", "SubscriptionPlan");
                            }

                            //return RedirectToAction("CustomerPaymentProcess", "PaymentProcess");
                            return RedirectToRoute("PaymentProcess", new { city = Request.Cookies["CityCookie"].Value.Split('$')[1].ToLower(), franchiseId = Request.Cookies["CityCookie"].Value.Split('$')[2] });////added franchiseId

                        }
                        else if (returnurl != string.Empty && returnurl != null) // 2.2.2
                        {
                            if (returnurl.Contains("CustomerOrder/OrderStatus"))
                            {
                                return RedirectToAction("MyOrders", "CustomerOrder");
                            }
                            else if (returnurl.Contains("Customer/create"))
                            {
                                return RedirectToAction("Edit", "Customer", new { id = Session["UID"] });
                            }
                            else
                            {
                                return Redirect(returnurl);
                            }
                        }

                        else
                        {
                            //change by harshada
                            string Url = "";
                            //return RedirectToAction("Index", "Home");
                            /*----------Get Url from cookie to redirect to its previous position-------*/
                            if (Request.Cookies["UrlCookie"] != null)
                            {
                                Url = Request.Cookies["UrlCookie"].Value.ToString();
                            }
                            //return Redirect(ControllerContext.HttpContext.Request.UrlReferrer.ToString());
                            return Redirect(Url);
                            /*----------End Get Url from cookie to redirect to its previous position-------*/
                        }
                    }
                }
                else
                {
                    RegisterNewUser regUser = new RegisterNewUser();
                    CustomerRegistrationViewModel model = new CustomerRegistrationViewModel();
                    model.EmailId = email;
                    model.Password = CreatePassword(10);
                    model.FirstName = userName;
                    regUser.CreateNew_Account(model);
                    externalLogin_detail.email = email;
                    externalLogin_detail.password = model.Password;
                    Int64 UID = db.UserLogins.Where(x => x.Email == email).FirstOrDefault().ID;
                    Session["UID"] = UID;
                    Session["UserName"] = email;
                    Session["FirstName"] = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(db.PersonalDetails.Find(CommonFunctions.GetPersonalDetailsID(UID)).FirstName.ToLower());
                    return Redirect(returnurl);
                }
            }
            return RedirectToAction("Index", "Home");
        }

        private void SetUserNameCookie(string userName, string userEmailMob)
        {
            if (ControllerContext.HttpContext.Request.Cookies["UserName"] != null)
            {
                HttpCookie UserName = new HttpCookie("UserName");
                if (ControllerContext.HttpContext.Request.Cookies["UserName"] != null)
                {
                    UserName.Expires = DateTime.Now.AddDays(-1);
                    ControllerContext.HttpContext.Response.Cookies.Add(UserName);
                }
                if (UserName.Expires < DateTime.Now)
                {
                    ControllerContext.HttpContext.Request.Cookies.Remove("UserName");
                }
            }

            ControllerContext.HttpContext.Response.Cookies["UserName"].Value = userName + "$" + userEmailMob;
            ControllerContext.HttpContext.Response.Cookies["UserName"].Expires = DateTime.Now.AddDays(30);
            ControllerContext.HttpContext.Response.AppendCookie(ControllerContext.HttpContext.Response.Cookies["UserName"]);
        }

        //Yashaswi 15-03-2019
        public JsonResult GetParentName(string ReferralId, string UserName)
        {
            string Result = "";
            try
            {
                string IsExsit = "0";
                UserLogin UserLogin = db.UserLogins.FirstOrDefault(p => p.Mobile == UserName || p.Email == UserName);
                if (UserLogin != null)
                {
                    MLMUser objmlmUser = db.MLMUsers.FirstOrDefault(p => p.UserID == UserLogin.ID);
                    if (objmlmUser != null)
                    {
                        ReferralId = objmlmUser.Refered_Id_ref;
                        IsExsit = "1";
                    }
                }
                Result = db.PersonalDetails.FirstOrDefault(q => q.UserLoginID == db.MLMUsers.FirstOrDefault(p => p.Ref_Id == ReferralId).UserID).FirstName;
                Result = IsExsit + Result;
            }
            catch
            {

            }
            return Json(Result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetParentNameForRegistration(string ReferralId, string Email, string Mobile)
        {
            string Result = "";
            try
            {
                UserLogin UserLogin = db.UserLogins.FirstOrDefault(p => p.Mobile == Mobile || p.Email == Email);
                if (UserLogin != null)
                {
                    Result = "1";
                }
                else
                {
                    Result = db.PersonalDetails.FirstOrDefault(q => q.UserLoginID == db.MLMUsers.FirstOrDefault(p => p.Ref_Id == ReferralId).UserID).FirstName;
                }
            }
            catch
            {

            }
            return Json(Result, JsonRequestBehavior.AllowGet);
        }
    }
    public class ExternalLoginviewModel
    {
        public string email { get; set; }
        public string password { get; set; }
        public int result { get; set; }
    }

    public class CarryData
    {
        public string email { get; set; }
        public string callFrom { get; set; }
        public string externalCallBackURL { get; set; }
        public string userName { get; set; }
        public bool isExpressBuy { get; set; }
        public string returnURL { get; set; }
    }

}