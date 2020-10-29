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

namespace Franchise.Controllers
{
    public class ForgotPasswordController : Controller
    {
        private EzeeloDBContext db = new EzeeloDBContext();
        public ActionResult Edit()
        {
            try
            {
                return View();
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[ForgotPasswordController][GET:Edit]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);

                return View();
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[ForgotPasswordController][GET:Edit]",
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);

                return View();
            }
        }

        [HttpPost]
        public ActionResult Edit(ForgotPasswordViewModel forgotPasswordViewModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var lUser = db.UserLogins.Where(x => x.Email == forgotPasswordViewModel.EmailId);

                    // Check email exists or not
                    if (lUser.Count() <= 0)
                    {
                        ViewBag.Message = "Email Id not exist!!";
                        return View();
                    }
                    else
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
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);

                return View();
            }
            catch (Exception ex)
            {
                ViewBag.Message = "Problem in sending mail!!";

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[ForgotPasswordController][POST:Edit]",
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);

                return View();
            }
            return View();
        }

        private void SendPassword(string pEmailId, long userLoginID)
        {
            try
            {
                EzeeloDBContext db = new EzeeloDBContext();

                var personalDetail = db.PersonalDetails.Where(x => x.UserLoginID == userLoginID);
                BusinessLogicLayer.GateWay gateWay = new BusinessLogicLayer.Email(System.Web.HttpContext.Current.Server);
                Dictionary<string, string> dictionary = new Dictionary<string, string>();


                dictionary.Add("<!--ACCOUNT_URL-->", "" + (new URLsFromConfig()).GetURL("PARTNER") + "");
                dictionary.Add("<!--ORDERS_URL-->", "" + (new URLsFromConfig()).GetURL("PARTNER") + "/CustomerOrder");
                dictionary.Add("<!--NAME-->", personalDetail.FirstOrDefault().FirstName);
                //dictionary.Add("<!--PWD_URL-->", "http://www.ezeelo.com/UserLogin/Edit?pUserLoginID=" + userLoginID);

             

                gateWay.SendEmail(BusinessLogicLayer.GateWay.EmailGateWays.GANDHIBAGH, BusinessLogicLayer.GateWay.SenderMail.INFO, BusinessLogicLayer.GateWay.EMailTypes.PWD_RCVRY, new string[] { pEmailId }, dictionary, true);
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[ForgotPasswordController][M:SendPassword]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[ForgotPasswordController][M:SendPassword]",
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
            }
        }
    }
}