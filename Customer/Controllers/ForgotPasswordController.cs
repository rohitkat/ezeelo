
//-----------------------------------------------------------------------
// <copyright file="ForgotPasswordController" company="Ezeelo Consumer Services Pvt. Ltd.">
//     Copyright (c) Ezeelo Consumer Services Pvt. Ltd. All rights reserved.
// </copyright>
// <author>Gaurav Dixit</author>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ModelLayer.Models.ViewModel;
using ModelLayer.Models;
using System.Text;
using System.Data.Entity;
using BusinessLogicLayer;

namespace Gandhibagh.Controllers
{
    public class ForgotPasswordController : Controller
    {
        private EzeeloDBContext db = new EzeeloDBContext();
        public ActionResult Edit()
        {
            /*
               Indents:
             * Description: This action method is used to render GET request for forgot Password view.
             
             * Parameters: 
             
             * Precondition: 
             * Postcondition:
             * Logic: 
             */
            try
            {
                return View();
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[ForgotPasswordController][GET:Edit]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Gandhibagh, System.Web.HttpContext.Current.Server);

                return View();
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[ForgotPasswordController][GET:Edit]",
                    BusinessLogicLayer.ErrorLog.Module.Gandhibagh, System.Web.HttpContext.Current.Server);

                return View();
            }
        }

        [HttpPost]
        public ActionResult Edit(ForgotPasswordViewModel forgotPasswordViewModel)
        {
            /*
               Indents:
             * Description: This action method respond to POST request for forgot password
             
             * Parameters: ForgotPasswordViewModel forgotPasswordViewModel: Contains EmailId on which forgot password link will be sent.
             
             * Precondition: 
             * Postcondition:
             * Logic: 1) Check ModelState.IsValid is True
             *        2) Check email exist or not in our database
             *          2.1) If not, then return error
             *          2.1) Else send forgotPassword link to customer
             */
            try
            {
                // 1
                if (ModelState.IsValid)
                {
                    // 2
                    var lUser = db.UserLogins.Where(x => x.Email == forgotPasswordViewModel.EmailId);

                    // 2.1
                    if (lUser.Count() <= 0)
                    {
                        ViewBag.Message = "Email Id not exist!!";
                        return View();
                    }
                    else // 2.2
                    {
                        this.SendPassword(forgotPasswordViewModel.EmailId, lUser.FirstOrDefault().ID);
                        ViewBag.Message = "Information We have sent you email to reset your password. Please check your email.!!";
                    }
                }
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                ViewBag.Message = "Problem in sending mail!!";

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[ForgotPasswordController][POST:Edit]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Gandhibagh, System.Web.HttpContext.Current.Server);

                return View(forgotPasswordViewModel);
            }
            catch (Exception ex)
            {
                ViewBag.Message = "Problem in sending mail!!";

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[ForgotPasswordController][POST:Edit]",
                    BusinessLogicLayer.ErrorLog.Module.Gandhibagh, System.Web.HttpContext.Current.Server);

                return View(forgotPasswordViewModel);
            }
            return View(forgotPasswordViewModel);
        }

        private void SendPassword(string pEmailId, long userLoginID)
        {
            /*
               Indents:
             * Description: This method is used send forgot password link to registered email id.
             
             * Parameters: pEmailId: Customer Email Id
              *            userLoginID: Customer UserLoginID
             
             * Precondition: 
             * Postcondition:
             * Logic: 
             */
            try
            {
                EzeeloDBContext db = new EzeeloDBContext();
                string city = "";
                int lFranchiseID = 0;////added
                if (System.Web.HttpContext.Current.Request.Cookies["CityCookie"].Value != null)
                {
                    city = System.Web.HttpContext.Current.Request.Cookies["CityCookie"].Value.Split('$')[1].ToLower();
                    lFranchiseID =Convert.ToInt32(System.Web.HttpContext.Current.Request.Cookies["CityCookie"].Value.Split('$')[2]);////added
                }
                // Sending email to the customer
                BusinessLogicLayer.GateWay gateWay = new BusinessLogicLayer.Email(System.Web.HttpContext.Current.Server);
                Dictionary<string, string> dictionary = new Dictionary<string, string>();
                //dictionary.Add("<!--ACCOUNT_URL-->", "http://www.ezeelo.com/login/login");
                //dictionary.Add("<!--ORDERS_URL-->", "http://ezeelo.com/CustomerOrder/MyOrders");
                //dictionary.Add("<!--ACCOUNT_URL-->", "http://ezeelo.com/" + city + "/payment-process-login");////hide
                //dictionary.Add("<!--ORDERS_URL-->", "http://www.ezeelo.com/" + city + "/cust-o/my-order");////hide
                dictionary.Add("<!--ACCOUNT_URL-->", "" + (new URLsFromConfig()).GetURL("CUSTOMER") + "" + city + "/" + lFranchiseID + "/payment-process-login");////added  "/" + lFranchiseID +
                dictionary.Add("<!--ORDERS_URL-->", "" + (new URLsFromConfig()).GetURL("CUSTOMER") + "" + city + "/" + lFranchiseID + "/cust-o/my-order");////added  "/" + lFranchiseID +
                dictionary.Add("<!--NAME-->", pEmailId);
                //dictionary.Add("<!--PWD_URL-->", this.GetUrl() + "UserLogin/Edit?pUserLoginID=" + userLoginID);
                //dictionary.Add("<!--PWD_URL-->", this.GetUrl() + city + "/chn-pwd/" + userLoginID);////hide
                dictionary.Add("<!--PWD_URL-->", this.GetUrl() + city + "/" + lFranchiseID + "/chn-pwd/" + userLoginID);////added "/" + lFranchiseID +

                gateWay.SendEmail(BusinessLogicLayer.GateWay.EmailGateWays.GANDHIBAGH, BusinessLogicLayer.GateWay.SenderMail.INFO, BusinessLogicLayer.GateWay.EMailTypes.PWD_RCVRY, new string[] { pEmailId, "sales@ezeelo.com" }, dictionary, true);
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[ForgotPasswordController][M:SendPassword]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Gandhibagh, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[ForgotPasswordController][M:SendPassword]",
                    BusinessLogicLayer.ErrorLog.Module.Gandhibagh, System.Web.HttpContext.Current.Server);
            }
        }

        public string GetUrl()
        {
            /*
               Indents:
             * Description: This method get location protocol
             
             * Parameters: 
             
             * Precondition: 
             * Postcondition:
             * Logic: 
             */
            String strPathAndQuery =HttpContext.Request.Url.PathAndQuery;
            String strUrl = HttpContext.Request.Url.AbsoluteUri.Replace(strPathAndQuery, "/");

            return strUrl;
        }

        public ActionResult SetNewPassword(FormCollection frm)
        {
            try
            {
                if (Session["OTPCode"] != null && Session["OTPCode"].ToString() != string.Empty)
                {
                    string lSessionCode = Session["OTPCode"].ToString();

                    var OTPFromDB = db.OTPs.Count(x => x.SessionCode == lSessionCode && x.OTP1 == frm["OTP"]);

                    if (OTPFromDB == 0)
                    {
                        ViewBag.Message = "Invalid OTP!!";
                    }
                    else
                    {
                        ViewBag.Message = "Account verified!!";
                    }

                }
                else
                {
                    ViewBag.Message = "OTP Expired!!";
                }

                string val = frm["NewPassword"];
            }
            catch (Exception)
            {
                
                throw;
            }
            return RedirectToAction("Index", "Home");
        }

    }
}