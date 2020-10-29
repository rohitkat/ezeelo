using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ModelLayer.Models;
using ModelLayer.Models.ViewModel;
using Merchant.Models;
using BusinessLogicLayer;

namespace Merchant.Controllers
{
    public class UserLoginController : Controller
    {
        private EzeeloDBContext db = new EzeeloDBContext();

        long fUserLoginID = 0;

        // GET: /UserLogin/Edit/5
        public ActionResult Edit(long? pUserLoginID)
        {
            /*
               Indents:
             * Description: This method is used to change password of customer.
             *              This method may be called from two postions :
             *              1) click on change password link from customer dashboard.
             *              2) Request for forgot password, in this case an email will be send
             *              to customer registered email id. in which we will send a link to reset your password.
             *              which contains pUserLoginID as userLoginId.
             
             * Parameters: pUserLoginID: This parameter is used for storing customer userLoginId.
             
             * Precondition: 
             * Postcondition:
             * Logic: 1) If pUserLoginID is NULL, that means user is comming from customer dashboard change password.
             *        2) Else User is comming from by click on link send in email on registered email address.
             *        3) Check user is exist or not, if not then return
             *        4) If all is well then return change password view
             */



            long lUserLoginID = 0;
            long.TryParse(Convert.ToString(pUserLoginID), out lUserLoginID);

            if (lUserLoginID <= 0) // 2
            {
                long.TryParse(Convert.ToString(Session["UID"]), out fUserLoginID);
            }
            else // 1
            {
                fUserLoginID = lUserLoginID;
            }

            UserLogin userlogin = db.UserLogins.Find(fUserLoginID);
            if (userlogin == null)  // 3
            {
                return HttpNotFound();
            }

            ChangePasswordViewModel lChangePasswordViewModel = new ChangePasswordViewModel();
            lChangePasswordViewModel.ID = userlogin.ID;  // 4

            return View(lChangePasswordViewModel);
        }

        // POST: /UserLogin/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ChangePasswordViewModel changePasswordViewModel, long? pUserLoginID)
        {
            /*
              Indents:
            * Description: This method(POST) is used to change password of customer.
            *              This method may be called from two postions :
            *              1) click on change password link from customer dashboard, then conatins old, new and retype password
            *              2) Request for forgot password, in this case an email will be send
            *              to customer registered email id. in which we will send a link to reset your password.
            *              which contains pUserLoginID as userLoginId, contains new and retype password
             
            * Parameters: pUserLoginID: This parameter is used for storing customer userLoginId.
             
            * Precondition: 
            * Postcondition:
            * Logic: 1) If NewPassword==NULL || ReTypeNewPassword==NULL then return
             *       2) If NewPassword!=ReTypeNewPassword Then Return
            *        3) If lUserLoginID<=0 then means user is comming from by click change password from dashboard
            *        3.1) In this case, we must ask old password
             *       4) Else User is comming from by ciick on link send in email address
             *       4.1) In this case, we will not ask old password 
            *        5) Update UserLogin Table
             *       
            */

            UserLogin lUserLogin = new UserLogin();
            try
            {
                EzeeloDBContext db1 = new EzeeloDBContext();
                lUserLogin = db1.UserLogins.Find(changePasswordViewModel.ID);

                //------------------------- new passord and retype password equals or not -//
                if (changePasswordViewModel.NewPassword == null || changePasswordViewModel.ReTypeNewPassword == null) // 1
                {
                    TempData["Title"] = "Please Enter Password and Retype Password";
                    return View(changePasswordViewModel);
                }
                if (changePasswordViewModel.NewPassword.ToUpper() != changePasswordViewModel.ReTypeNewPassword.ToUpper()) // 2
                {
                    TempData["Title"] = "Passwords does not match";
                    return View(changePasswordViewModel);
                }

                long lUserLoginID = 0;
                long.TryParse(Convert.ToString(pUserLoginID), out lUserLoginID);

                if (lUserLoginID <= 0)  // 3
                {
                    //------------------------- check old password == new password -//
                    if (lUserLogin.Password.ToUpper() != changePasswordViewModel.OldPassword.ToUpper())  // 3.1
                    {
                        TempData["Title"] = "Old Password is In Correct";
                        return View(changePasswordViewModel);
                    }
                    long.TryParse(Convert.ToString(Session["UID"]), out fUserLoginID);
                }
                else   // 4
                {
                    fUserLoginID = lUserLoginID;  // 4.1
                }

                // 5
                lUserLogin.Password = changePasswordViewModel.NewPassword;
                lUserLogin.CreateBy = lUserLogin.CreateBy;
                lUserLogin.CreateDate = lUserLogin.CreateDate;
                lUserLogin.ModifyBy = CommonFunctions.GetPersonalDetailsID(fUserLoginID);
                lUserLogin.ModifyDate = DateTime.Now;
                db1.Dispose();
                if (ModelState.IsValid)
                {
                    db.Entry(lUserLogin).State = EntityState.Modified;
                    db.SaveChanges();
                    TempData["Title"] = "Password changes Successfully";
                    return View(changePasswordViewModel);
                }

                return View(changePasswordViewModel);
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                TempData["Title"] = "Sorry! Problem in changing password!!";

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[UserLoginController][POST:Edit]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Gandhibagh, System.Web.HttpContext.Current.Server);

                return View(changePasswordViewModel);
            }
            catch (Exception ex)
            {
                TempData["Title"] = "Sorry! Problem in changing password!!";

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[UserLoginController][POST:Edit]",
                    BusinessLogicLayer.ErrorLog.Module.Gandhibagh, System.Web.HttpContext.Current.Server);

                return View(changePasswordViewModel);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
