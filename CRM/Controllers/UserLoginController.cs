using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ModelLayer.Models;
using PagedList;
using PagedList.Mvc;
using CRM.Models.ViewModel;
using CRM.Models;
using System.Collections;

namespace CRM.Controllers
{
    public class UserLoginController : Controller
    {
        private EzeeloDBContext db = new EzeeloDBContext();
        private CustomerCareSessionViewModel customerCareSessionViewModel = new CustomerCareSessionViewModel();
        private int pageSize = 10;

        public void SessionDetails()
        {
            customerCareSessionViewModel.UserLoginID = Convert.ToInt64(Session["ID"]);
            customerCareSessionViewModel.Username = Session["UserName"].ToString();
            Common.Common.GetAllLoginDetailFromSession(ref customerCareSessionViewModel);
        }

        [SessionExpire]
        // GET: /UserLogin/Edit/5
        public ActionResult Edit()
        {
            SessionDetails();
            UserLogin userlogin = db.UserLogins.Find(customerCareSessionViewModel.UserLoginID);
            if (userlogin == null)
            {
                return HttpNotFound();
            }

            ChangePasswordViewModel lChangePasswordViewModel = new ChangePasswordViewModel();
            lChangePasswordViewModel.ID = userlogin.ID;
            ViewBag.CreateBy = new SelectList(db.PersonalDetails, "ID", "FirstName", userlogin.CreateBy);
            ViewBag.ModifyBy = new SelectList(db.PersonalDetails, "ID", "FirstName", userlogin.ModifyBy);
            return View(lChangePasswordViewModel);
        }

        // POST: /UserLogin/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [SessionExpire]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ChangePasswordViewModel changePasswordViewModel)
        {
            SessionDetails();
            UserLogin lUserLogin = new UserLogin();
            try
            {
                EzeeloDBContext db1 = new EzeeloDBContext();
                lUserLogin = db1.UserLogins.Find(changePasswordViewModel.ID);
                
                //------------------------- new passord and retype password equals or not -//
                if (changePasswordViewModel.NewPassword == null || changePasswordViewModel.ReTypeNewPassword == null)
                {
                    throw new Exception("Please Enter Password and Retype Password");
                }
                if (changePasswordViewModel.NewPassword.ToUpper() != changePasswordViewModel.ReTypeNewPassword.ToUpper())
                {
                    throw new Exception("Passwords does not match");
                }

                //------------------------- check old password == new password -//
                if (lUserLogin.Password.ToUpper() != changePasswordViewModel.OldPassword.ToUpper())
                {
                    throw new Exception("Old Password is In Correct");
                }
                lUserLogin.Password = changePasswordViewModel.NewPassword;
                lUserLogin.CreateBy = lUserLogin.CreateBy;
                lUserLogin.CreateDate = lUserLogin.CreateDate;
                lUserLogin.ModifyBy = customerCareSessionViewModel.PersonalDetailID;
                lUserLogin.ModifyDate = DateTime.Now;
                db1.Dispose();
                if (ModelState.IsValid)
                {
                    db.Entry(lUserLogin).State = EntityState.Modified;
                    db.SaveChanges();
                    TempData["Title"] = "Password changes Successfully";
                    TempData["Description"] = "Please relogin to verify !...";
                    return View("ServerMsg");
                }
                ViewBag.CreateBy = new SelectList(db.PersonalDetails, "ID", "FirstName", lUserLogin.CreateBy);
                ViewBag.ModifyBy = new SelectList(db.PersonalDetails, "ID", "FirstName", lUserLogin.ModifyBy);
                ViewBag.CreateBy = new SelectList(db.PersonalDetails, "ID", "FirstName", lUserLogin.CreateBy);
                ViewBag.ModifyBy = new SelectList(db.PersonalDetails, "ID", "FirstName", lUserLogin.ModifyBy);
                return View(changePasswordViewModel);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Error", ex.Message);

                //Code to write error log
                //BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                //    + Environment.NewLine + ex.Message + Environment.NewLine
                //    + "[UserLogin][POST:Edit]",
                //    BusinessLogicLayer.ErrorLog.Module.CRM, System.Web.HttpContext.Current.Server);

                ViewBag.CreateBy = new SelectList(db.PersonalDetails, "ID", "FirstName", lUserLogin.CreateBy);
                ViewBag.ModifyBy = new SelectList(db.PersonalDetails, "ID", "FirstName", lUserLogin.ModifyBy);
                ViewBag.CreateBy = new SelectList(db.PersonalDetails, "ID", "FirstName", lUserLogin.CreateBy);
                ViewBag.ModifyBy = new SelectList(db.PersonalDetails, "ID", "FirstName", lUserLogin.ModifyBy);
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
