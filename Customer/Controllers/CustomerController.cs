//-----------------------------------------------------------------------
// <copyright file="CustomerController" company="Ezeelo Consumer Services Pvt. Ltd.">
//     Copyright (c) Ezeelo Consumer Services Pvt. Ltd. All rights reserved.
// </copyright>
// <author>Gaurav Dixit</author>
//-----------------------------------------------------------------------

using BusinessLogicLayer;
using Gandhibagh.Models;
using ModelLayer.Models;
using ModelLayer.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Validation;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Transactions;
using System.Web.Configuration;
using System.Web.Mvc;
using System.Data.Entity;
using System.Web;

namespace Gandhibagh.Controllers
{
    public class CustomerController : Controller
    {
        private EzeeloDBContext db = new EzeeloDBContext();

        public ActionResult Create()
        {
            //Set Cookie for Url saving & Use in Continue shopping
            URLCookie.SetCookies();

            /*
               Indents:
             * Description: This method is used  for get request for customer registration
             
             * Parameters: 
             
             * Precondition: 
             * Postcondition:
             * Logic: 
             */

            try
            {
                // Storing salutation in ViewBag   //Mantain Error Log
                ViewBag.salutation = new SelectList(db.Salutations, "ID", "Name");
                return View();
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Error", "There's Something wrong with the customer registration page!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[CustomerController][GET:Create]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Gandhibagh, System.Web.HttpContext.Current.Server);

                return View();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Error", "There's Something wrong with the customer registration page!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[CustomerController][GET:Create]",
                    BusinessLogicLayer.ErrorLog.Module.Gandhibagh, System.Web.HttpContext.Current.Server);

                return View();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CustomerRegistrationViewModel model)
        {

            /*
               Indents:
             * Description: This is used to create customer
             
             * Parameters: 
             
             * Precondition: 
             * Postcondition:
             * Logic:    1) If customer role is not exists then return
             *           2) Store the entire data of  user in database
             *           2.1) Check user details exists or not, if yes then return
             *           2.2) Creating user login
             *           2.3) storing personal details
             *           2.4) Store role in UserRole table
             *           3) Send email, message to customer
             *           4) Return to user profile page
             */

            try
            {




                /*For Nullable Email Address
                 * Pradnyakar Badge
                 * 06-01-2016
                 */

                if (model.EmailId != null)
                {
                    if (model.EmailId.Trim().ToString().Equals(""))
                    {
                        model.EmailId = null;
                    }
                }

                // Storing salutation in ViewBag
                ViewBag.salutation = new SelectList(db.Salutations.Where(x => x.IsActive == true), "ID", "Name");
                ModelState.Clear();
                //ModelState["LastName"].Value = string.Empty;
                model.LastName = "  ";

                //model.MiddleName = string.Empty;
                //UpdateModel(model);


                if (model.EmailId == null && model.MobileNo == null)
                {
                    ViewBag.Message = "Please Provide your Email or Mobile No...!!";

                    return View();
                }

                if (ModelState.IsValid)
                {
                    var lRole = db.Roles.Where(x => x.Name == "Customer".ToUpper()).ToList();

                    if (lRole.Count() <= 0)
                    {
                        ViewBag.Message = "Role not exist!!";
                        return View();
                    }


                    using (TransactionScope ts = new TransactionScope())
                    {
                        try
                        {
                            string lMessage = string.Empty;
                            string mobEmail = string.Empty;
                            // Check user details exists or not
                            lMessage = CommonFunctions.CheckUserDetails(model.EmailId, model.MobileNo, out mobEmail);

                            if (lMessage != string.Empty)
                            {
                                ViewBag.Message = lMessage;
                                TempData["mobEmail"] = mobEmail;
                                return View();
                            }

                            // Creating user login
                            UserLogin userLogin = new UserLogin();

                            userLogin.ID = 0;
                            userLogin.Mobile = model.MobileNo;
                            userLogin.Email = model.EmailId;
                            userLogin.Password = model.Password;
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

                            // Transaction complete
                            ts.Complete();

                            //Tejaswee 28/7/2015
                            //3) Send email, message to customer
                            SendEmailToCustomer(model.EmailId, model.FirstName);
                            SendMessageToCustomer(model.MobileNo, model.FirstName);

                            // Clear model state
                            ModelState.Clear();

                            ViewBag.Message = "Done! Registration Successfully Done!!";

                            Session["UID"] = id;

                            /*To allow null Email 
                             * if email is null then put Mobile no in the session
                             * Prandyakar Badge 05-01-2016                             
                             */
                            if (model.EmailId == null)
                            {
                                Session["UserName"] = model.MobileNo;
                            }
                            else
                            {
                                Session["UserName"] = model.EmailId;
                            }

                            Session["FirstName"] = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(personalDetail.FirstName.ToLower());

                            // Set model to NULL
                            model = null;

                            //4) Return to user profile page
                            if (id > 0)
                            {
                                return RedirectToAction("Edit", new { id = id });
                                //return Redirect(ControllerContext.HttpContext.Request.UrlReferrer.ToString());
                            }
                        }
                        catch (Exception exception)
                        {
                            ViewBag.Message = "Sorry! Problem in customer registration!!";

                            // Rollback transaction
                            ts.Dispose();

                            return View();
                        }
                    }
                }
                return View(model);
            }
            catch (DbEntityValidationException ex)
            {
                // Retrieve the error messages as a list of strings.
                var errorMessages = ex.EntityValidationErrors
                        .SelectMany(x => x.ValidationErrors)
                        .Select(x => new { x.ErrorMessage });

                var fullErrorMessage = string.Join("; ", errorMessages);
                var exceptionMessage = string.Concat(ex.Message, " The validation errors are: ", fullErrorMessage);
                ViewBag.Message = fullErrorMessage.Replace("ErrorMessage = ", " ");
                return View(model);
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


        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult CreateFromPartial(CustomerRegistrationViewModel model)
        //{

        //    /*
        //       Indents:
        //     * Description: This is used to create customer

        //     * Parameters: 

        //     * Precondition: 
        //     * Postcondition:
        //     * Logic:    1) If customer role is not exists then return
        //     *           2) Store the entire data of  user in database
        //     *           2.1) Check user details exists or not, if yes then return
        //     *           2.2) Creating user login
        //     *           2.3) storing personal details
        //     *           2.4) Store role in UserRole table
        //     *           3) Send email, message to customer
        //     *           4) Return to user profile page
        //     */

        //    try
        //    {




        //        /*For Nullable Email Address
        //         * Pradnyakar Badge
        //         * 06-01-2016
        //         */

        //        if (model.EmailId != null)
        //        {
        //            if (model.EmailId.Trim().ToString().Equals(""))
        //            {
        //                model.EmailId = null;
        //            }
        //        }

        //        // Storing salutation in ViewBag
        //        ViewBag.salutation = new SelectList(db.Salutations.Where(x => x.IsActive == true), "ID", "Name");
        //        ModelState.Clear();
        //        //ModelState["LastName"].Value = string.Empty;
        //        model.LastName = "  ";

        //        //model.MiddleName = string.Empty;
        //        //UpdateModel(model);


        //        if (model.EmailId == null && model.MobileNo == null)
        //        {
        //            ViewBag.Message = "Please Provide your Email or Mobile No...!!";

        //            return View("_Login", "_Layout_Internal");
        //        }

        //        if (ModelState.IsValid)
        //        {
        //            var lRole = db.Roles.Where(x => x.Name == "Customer".ToUpper()).ToList();

        //            if (lRole.Count() <= 0)
        //            {
        //                ViewBag.Message = "Role not exist!!";
        //                return View("_Login", "_Layout_Internal");
        //            }


        //            using (TransactionScope ts = new TransactionScope())
        //            {
        //                try
        //                {
        //                    string lMessage = string.Empty;
        //                    string mobEmail = string.Empty;
        //                    // Check user details exists or not
        //                    lMessage = CommonFunctions.CheckUserDetails(model.EmailId, model.MobileNo, out mobEmail);

        //                    if (lMessage != string.Empty)
        //                    {
        //                        ViewBag.Message = lMessage;
        //                        TempData["mobEmail"] = mobEmail;
        //                        return View("_Login", "_Layout_Internal");
        //                    }

        //                    // Creating user login
        //                    UserLogin userLogin = new UserLogin();

        //                    userLogin.ID = 0;
        //                    userLogin.Mobile = model.MobileNo;
        //                    userLogin.Email = model.EmailId;
        //                    userLogin.Password = model.Password;
        //                    userLogin.IsLocked = false;
        //                    userLogin.CreateBy = 1;
        //                    userLogin.CreateDate = DateTime.UtcNow.AddHours(5.5);

        //                    db.UserLogins.Add(userLogin);
        //                    db.SaveChanges();

        //                    // Getting current user login id
        //                    Int64 id = userLogin.ID;

        //                    // storing personal details
        //                    PersonalDetail personalDetail = new PersonalDetail();
        //                    personalDetail.ID = 0;
        //                    personalDetail.UserLoginID = id;
        //                    //personalDetail.SalutationID = model.ID;
        //                    personalDetail.SalutationID = 1;
        //                    personalDetail.FirstName = model.FirstName;
        //                    personalDetail.MiddleName = model.MiddleName;
        //                    personalDetail.LastName = model.LastName;
        //                    personalDetail.IsActive = true;
        //                    personalDetail.CreateBy = 1;
        //                    personalDetail.CreateDate = DateTime.UtcNow.AddHours(5.5);

        //                    db.PersonalDetails.Add(personalDetail);
        //                    db.SaveChanges();

        //                    UserRole userRole = new UserRole();

        //                    userRole.ID = 0;
        //                    userRole.RoleID = lRole.FirstOrDefault().ID;
        //                    userRole.UserLoginID = id;
        //                    userRole.IsActive = true;
        //                    userRole.CreateDate = DateTime.Now;
        //                    userRole.CreateBy = CommonFunctions.GetPersonalDetailsID(id);

        //                    db.UserRoles.Add(userRole);
        //                    db.SaveChanges();
        //                    //====================================Update referenceID from referedetail table=============Manoj==//
        //                    if (db.ReferDetails.Any(m => (m.Email == model.EmailId || m.Mobile == model.MobileNo) && m.ReferenceID == null))
        //                    {
        //                        ReferDetail lReferDetail = db.ReferDetails.Where(m => m.Email == model.EmailId || m.Mobile == model.MobileNo).FirstOrDefault();
        //                        if (lReferDetail != null)
        //                        {
        //                            //Update referance id in referDetail table
        //                            //i.e. Refer user now registered with eZeelo
        //                            lReferDetail.ReferenceID = id;
        //                            lReferDetail.ModifyDate = DateTime.Now;
        //                            lReferDetail.ModifyBy = CommonFunctions.GetPersonalDetailsID(id);
        //                            lReferDetail.NetworkIP = BusinessLogicLayer.CommonFunctions.GetClientIP();
        //                            db.SaveChanges();
        //                        }
        //                        //Check if their is user wise earn 
        //                        var earn = (from RES in db.ReferAndEarnSchemas
        //                                    join RD in db.ReferDetails on RES.ID equals RD.ReferAndEarnSchemaID
        //                                    where RES.UserwiseEarn == true && RD.ID == lReferDetail.ID
        //                                    select new { EarnMoney = RES.EarnInRS }).FirstOrDefault();
        //                        //If user wise earn then add earn money to refer by user account
        //                        if (earn != null)
        //                        {
        //                            decimal EarnRs = Convert.ToDecimal(earn.EarnMoney);

        //                            var PrevRemainingAmt = db.EarnDetails.OrderByDescending(u => u.ID).Where(x => x.EarnUID == lReferDetail.UserID).Select(x => x.RemainingAmount).FirstOrDefault();

        //                            EarnDetail lEarnDetail = new EarnDetail();
        //                            lEarnDetail.EarnUID = lReferDetail.UserID;
        //                            lEarnDetail.ReferUID = lReferDetail.ReferenceID;
        //                            //lEarnDetail.EarnAmount = earn.FirstOrDefault().EarnMoney;
        //                            lEarnDetail.EarnAmount = EarnRs;
        //                            lEarnDetail.UsedAmount = 0;
        //                            if (PrevRemainingAmt != null)
        //                            {
        //                                lEarnDetail.RemainingAmount = PrevRemainingAmt + EarnRs;
        //                            }
        //                            else
        //                            {
        //                                lEarnDetail.RemainingAmount = EarnRs;
        //                            }
        //                            lEarnDetail.CustomerOrderID = null;
        //                            lEarnDetail.CreateDate = DateTime.Now;
        //                            lEarnDetail.CreateBy = CommonFunctions.GetPersonalDetailsID(id);
        //                            lEarnDetail.NetworkIP = BusinessLogicLayer.CommonFunctions.GetClientIP();
        //                            db.EarnDetails.Add(lEarnDetail);
        //                            db.SaveChanges();
        //                        }

        //                    }
        //                    //==================================================================================================//
        //                    // Transaction complete
        //                    ts.Complete();

        //                    //Tejaswee 28/7/2015
        //                    //3) Send email, message to customer
        //                    SendEmailToCustomer(model.EmailId, model.FirstName);
        //                    SendMessageToCustomer(model.MobileNo, model.FirstName);

        //                    // Clear model state
        //                    ModelState.Clear();

        //                    ViewBag.Message = "Done! Registration Successfully Done!!";

        //                    Session["UID"] = id;

        //                    /*To allow null Email 
        //                     * if email is null then put Mobile no in the session
        //                     * Prandyakar Badge 05-01-2016                             
        //                     */
        //                    if (model.EmailId == null)
        //                    {
        //                        Session["UserName"] = model.MobileNo;
        //                    }
        //                    else
        //                    {
        //                        Session["UserName"] = model.EmailId;
        //                    }

        //                    Session["FirstName"] = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(personalDetail.FirstName.ToLower());

        //                    // Set model to NULL
        //                    model = null;

        //                    //4) Return to user profile page
        //                    if (id > 0)
        //                    {
        //                        return RedirectToAction("Edit", new { id = id });
        //                        //return Redirect(ControllerContext.HttpContext.Request.UrlReferrer.ToString());
        //                    }
        //                }
        //                catch (Exception exception)
        //                {
        //                    ViewBag.Message = "Sorry! Problem in customer registration!!";

        //                    // Rollback transaction
        //                    ts.Dispose();

        //                    return View("_Login", "_Layout_Internal");
        //                }
        //            }
        //        }
        //        return View(model);
        //    }
        //    catch (DbEntityValidationException ex)
        //    {
        //        // Retrieve the error messages as a list of strings.
        //        var errorMessages = ex.EntityValidationErrors
        //                .SelectMany(x => x.ValidationErrors)
        //                .Select(x => new { x.ErrorMessage });

        //        var fullErrorMessage = string.Join("; ", errorMessages);
        //        var exceptionMessage = string.Concat(ex.Message, " The validation errors are: ", fullErrorMessage);
        //        ViewBag.Message = fullErrorMessage.Replace("ErrorMessage = ", " ");
        //        return View(model);
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
        [ValidateAntiForgeryToken]
        public ActionResult CreateFromPartial(CustomerRegistrationViewModel model)
        {
            Encryption _Encryption = new Encryption();//Added by Rumana on 13/04/2019

            /*
               Indents:
             * Description: This is used to create customer
             
             * Parameters: 
             
             * Precondition: 
             * Postcondition:
             * Logic:    1) If customer role is not exists then return
             *           2) Store the entire data of  user in database
             *           2.1) Check user details exists or not, if yes then return
             *           2.2) Creating user login
             *           2.3) storing personal details
             *           2.4) Store role in UserRole table
             *           3) Send email, message to customer
             *           4) Return to user profile page
             */

            try
            {




                /*For Nullable Email Address
                 * Pradnyakar Badge
                 * 06-01-2016
                 */

                if (model.EmailId != null)
                {
                    if (model.EmailId.Trim().ToString().Equals(""))
                    {
                        model.EmailId = null;
                    }
                }

                // Storing salutation in ViewBag
                ViewBag.salutation = new SelectList(db.Salutations.Where(x => x.IsActive == true), "ID", "Name");
                ModelState.Clear();
                //ModelState["LastName"].Value = string.Empty;
                model.LastName = "  ";

                //model.MiddleName = string.Empty;
                //UpdateModel(model);


                if (model.EmailId == null && model.MobileNo == null)
                {
                    ViewBag.Message = "Please Provide your Email or Mobile No...!!";

                    return View("_Login", "_Layout_Internal");
                }

                if (ModelState.IsValid)
                {
                    var lRole = db.Roles.Where(x => x.Name == "Customer".ToUpper()).ToList();

                    if (lRole.Count() <= 0)
                    {
                        ViewBag.Message = "Role not exist!!";
                        return View("_Login", "_Layout_Internal");
                    }


                    //using (TransactionScope ts = new TransactionScope())
                    //{
                    try
                    {
                        string lMessage = string.Empty;
                        string mobEmail = string.Empty;
                        var EncryptedEmail = _Encryption.EncodePasswordToBase64(model.EmailId);//Added by Rumana on 13/04/2019
                        var EncryptedPassword = _Encryption.EncodePasswordToBase64(model.Password);//Added by Rumana on 13/04/2019
                        // Check user details exists or not
                        lMessage = CommonFunctions.CheckUserDetails(model.EmailId, model.MobileNo, out mobEmail);

                        if (lMessage != string.Empty)
                        {
                            ViewBag.Message = lMessage;
                            TempData["mobEmail"] = mobEmail;
                            return View("_Login", "_Layout_Internal");
                        }


                        // Creating user login
                        UserLogin userLogin = new UserLogin();

                        userLogin.ID = 0;
                        userLogin.Mobile = model.MobileNo;
                        userLogin.Email = model.EmailId;
                        userLogin.Password = model.Password;
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
                        //====================================Update referenceID from referedetail table=============Manoj==//
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

                        //==================================================================================================//
                        // Transaction complete
                        //ts.Complete();
                        //Start Yashaswi Leader Signup
                        try
                        {
                            if (!String.IsNullOrEmpty(model.ReferralId))
                            {
                                //using (TransactionScope ts1 = new TransactionScope())
                                //{
                                MLMWalletPoints MLMWallet = new MLMWalletPoints();
                                //long LoginUserId = id;
                                //UserLogin userLog = db.UserLogins.FirstOrDefault(p => p.ID == LoginUserId);
                                string result = MLMWallet.LeadersSingUp(id, model.Password, model.EmailId, model.ReferralId);
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
                                    //Yashaswi 1-9-2018
                                    //Session["LeaderSignUpLink"] = "" + (new URLsFromConfig()).GetURL("LEADERS") + "LeadersLogin/Login/?UserName=" + model.EmailId + "&Password=" + model.Password;
                                    Session["LeaderSignUpLink"] = "" + (new URLsFromConfig()).GetURL("LEADERS") + "LeadersLogin/Login/?UserName=" + EncryptedEmail + "&Password=" + EncryptedPassword;//Added by Rumana on 13/04/2019
                                    //Session["LeaderSignUpLink"] = "http://leaders.ezeelo.com/signin/?Email=" + model.EmailId + "&Password=" + model.Password;
                                    TempData["IsLeadersSignUp"] = "You Have Successfully Become Ezeelo Member" + UserName;
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
                                    //Yashaswi 1-9-2018
                                    Session["LeaderSignUpLink"] = "" + (new URLsFromConfig()).GetURL("LEADERS") + "LeadersLogin/Login/?UserName=" + EncryptedEmail + "&Password=" + EncryptedPassword;//Added by Rumana on 13/04/2019
                                    //Session["LeaderSignUpLink"] = "" + (new URLsFromConfig()).GetURL("LEADERS") + "LeadersLogin/Login/?UserName=" + model.EmailId + "&Password=" + model.Password;
                                    //Session["LeaderSignUpLink"] = "http://leaders.ezeelo.com/signin/?Email=" + model.EmailId + "&Password=" + model.Password;
                                    TempData["IsLeadersSignUp"] = "You Are Already Registered As Ezeelo Member" + UserName;
                                }
                                else
                                {
                                    TempData["IsLeadersSignUp"] = "Facing Any Trouble , Happy To Help You, Simply Call 9172221910";
                                }
                                //ts1.Complete();
                                //}

                            }

                        }
                        catch
                        {

                        }
                        //End



                        //Tejaswee 28/7/2015
                        //3) Send email, message to customer
                        SendEmailToCustomer(model.EmailId, model.FirstName);
                        SendMessageToCustomer(model.MobileNo, model.FirstName);

                        // Clear model state
                        ModelState.Clear();

                        ViewBag.Message = "Done! Registration Successfully Done!!";

                        Session["UID"] = id;

                        /*To allow null Email 
                         * if email is null then put Mobile no in the session
                         * Prandyakar Badge 05-01-2016                             
                         */
                        if (model.EmailId == null)
                        {
                            Session["UserName"] = model.MobileNo;
                        }
                        else
                        {
                            Session["UserName"] = model.EmailId;
                        }

                        Session["FirstName"] = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(personalDetail.FirstName.ToLower());




                        // Set model to NULL
                        model = null;

                        //4) Return to user profile page
                        if (id > 0)
                        {
                            return RedirectToAction("Edit", new { id = id });
                            //return Redirect(ControllerContext.HttpContext.Request.UrlReferrer.ToString());
                        }
                    }
                    catch (Exception exception)
                    {
                        ViewBag.Message = "Sorry! Problem in customer registration!!";

                        // Rollback transaction
                        //ts.Dispose();

                        return View("_Login", "_Layout_Internal");
                    }
                    //}
                }
                return View(model);
            }
            catch (DbEntityValidationException ex)
            {
                // Retrieve the error messages as a list of strings.
                var errorMessages = ex.EntityValidationErrors
                        .SelectMany(x => x.ValidationErrors)
                        .Select(x => new { x.ErrorMessage });

                var fullErrorMessage = string.Join("; ", errorMessages);
                var exceptionMessage = string.Concat(ex.Message, " The validation errors are: ", fullErrorMessage);
                ViewBag.Message = fullErrorMessage.Replace("ErrorMessage = ", " ");
                return View(model);
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

        [HttpPost]
        public ActionResult SendOtpToValidUser(string EmailId, string MobileNo, string Name, string ReferralId)
        {
            int IsValid = -1;
            string PartialString = string.Empty;
            try
            {
                if (string.IsNullOrEmpty(EmailId) || string.IsNullOrEmpty(MobileNo))
                {
                    PartialString = "Please Provide your Email or Mobile No...!!";
                    return Json(new { data = IsValid, PartialString = PartialString });
                }
                if(!db.MLMUsers.Any(p => p.Ref_Id == ReferralId && !db.UserLogins.Where(u => u.IsLocked == true).Select(u => u.ID).ToList().Contains(p.UserID)))
                {
                    PartialString = "Please Enter Valid Referral Id...!!";
                    return Json(new { data = IsValid, PartialString = PartialString });
                }

                var lRole = db.Roles.Where(x => x.Name == "Customer".ToUpper()).ToList();
                if (lRole.Count() <= 0)
                {
                    PartialString = "Role not exist!!";
                    return Json(new { data = IsValid, PartialString = PartialString });
                }

                string lMessage = string.Empty;
                string mobEmail = string.Empty;                                            // Check user details exists or not
                lMessage = CommonFunctions.CheckUserDetails(EmailId, MobileNo, out mobEmail);
                if (lMessage != string.Empty)
                {
                    PartialString = lMessage;
                    return Json(new { data = IsValid, PartialString = PartialString });
                }


                IsValid = CommonFunctions.SendOTP(EmailId, MobileNo, Name, true, true);
                if (IsValid == 1)
                {
                    PartialString = "OTP sent to email and mobile.";

                }
                else if (IsValid == -3)
                {
                    PartialString = "Please contact customer care for OTP on 9172221910";

                }
                else if (IsValid == 2 || IsValid==-1)
                {
                    IsValid = -1;
                    PartialString = "OTP regenerate limit exceeds.";
                }               

            }
            catch (Exception ex)
            {
                IsValid = -1;
            }
            return Json(new { data = IsValid, PartialString = PartialString });
        }

        /// <summary>
        /// Check if OTP is valid or not
        /// </summary>
        /// <param name="EmailOTP"></param>
        /// <param name="MobileOTP"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult IsOtpValid(string EmailId, string MobileNo, string EmailOTP, string MobileOTP)
        {
            int EmailOTPValid = 0;
            int MobileOTPValid = 0;
            OTPLog RegistrationOTPLog = db.OTPLogs.Where(x => x.Email.ToUpper() == EmailId.ToUpper() && x.Mobile == MobileNo && x.IsActive == true && DbFunctions.TruncateTime(x.CreateDate) == DbFunctions.TruncateTime(DateTime.Now)).FirstOrDefault();
            if (RegistrationOTPLog != null)
            {
                if (DateTime.Now < Convert.ToDateTime(RegistrationOTPLog.OTPExpire))
                {

                    if (RegistrationOTPLog.EmailOTP == EmailOTP)
                    {
                        EmailOTPValid = 1;
                    }
                    if (RegistrationOTPLog.MobileOTP == MobileOTP)
                    {
                        MobileOTPValid = 1;
                    }
                    if (EmailOTPValid == 1 && MobileOTPValid == 1)
                    {
                        RegistrationOTPLog.IsActive = false;
                        RegistrationOTPLog.IsValidated = true;
                        db.SaveChanges();
                    }
                    //RegistrationOTPLog.IsActive = false;


                }
                else
                {
                    EmailOTPValid = -1;
                    MobileOTPValid = -1;
                }
            }


            return Json(new { Edata = EmailOTPValid, Mdata = MobileOTPValid });
        }
        /// <summary>
        /// Send OTP to email and mobile
        /// </summary>
        /// <param name="EmailID"></param>
        /// <param name="PhoneNumber"></param>
        /// <param name="Name"></param>
        /// <param name="EmailOTP"></param>
        /// <param name="MobileOTP"></param>
        /// <returns></returns>
        bool SendOtpToEmailnMobileNumber(string EmailID, string PhoneNumber, string Name, string EmailOTP, string MobileOTP)
        {
            bool IsSend = false;

            try
            {


                try
                {

                    Dictionary<string, string> dictEmailValues = new Dictionary<string, string>();
                    dictEmailValues.Add("<!--Name-->", Name);
                    dictEmailValues.Add("<!--OTP-->", EmailOTP);
                    BusinessLogicLayer.GateWay gateWay = new BusinessLogicLayer.Email(System.Web.HttpContext.Current.Server);
                    gateWay.SendEmail(BusinessLogicLayer.GateWay.EmailGateWays.GANDHIBAGH,
                        BusinessLogicLayer.GateWay.SenderMail.INFO, BusinessLogicLayer.GateWay.EMailTypes.CUST_OTP_REG,
                        new string[] { EmailID }, dictEmailValues, true);
                }
                catch (Exception ex)
                {
                }

                try
                {


                    Dictionary<string, string> dictSMSValues = new Dictionary<string, string>();
                    BusinessLogicLayer.GateWay gateWay = new BusinessLogicLayer.SMS(System.Web.HttpContext.Current.Server);
                    dictSMSValues.Add("#--NAME--#", Name);
                    dictSMSValues.Add("#--OTP--#", MobileOTP);
                    gateWay.SendSMS(BusinessLogicLayer.GateWay.SMSGateWays.SUMIT,
                        BusinessLogicLayer.GateWay.SMSOptions.SINGLE, BusinessLogicLayer.GateWay.SMSTypes.OTP_CUST_REG,
                        new string[] { PhoneNumber }, dictSMSValues);

                }
                catch (Exception ex)
                {
                }

                IsSend = true;
            }
            catch (Exception ex)
            {
                IsSend = false;
            }
            return IsSend;
        }

        bool SendOtpToEmailnMobileNumberToAdmin(string EmailID, string PhoneNumber, string Name)
        {
            bool IsSend = false;

            try
            {


                try
                {
                    if (!string.IsNullOrEmpty(Convert.ToString(WebConfigurationManager.AppSettings["ADMIN_EMAIL"])))
                    {
                        Dictionary<string, string> dictEmailValues = new Dictionary<string, string>();

                        dictEmailValues.Add("<!--CUSTOMER_EMAIL-->", EmailID);
                        dictEmailValues.Add("<!--CUSTOMER_NAME-->", Name);
                        dictEmailValues.Add("<!--CUSTOMER_MOBILE-->", PhoneNumber);
                        BusinessLogicLayer.GateWay gateWay = new BusinessLogicLayer.Email(System.Web.HttpContext.Current.Server);
                        gateWay.SendEmail(BusinessLogicLayer.GateWay.EmailGateWays.GANDHIBAGH,
                            BusinessLogicLayer.GateWay.SenderMail.INFO, BusinessLogicLayer.GateWay.EMailTypes.ADM_CUST_REG_LIM_EXDS,
                            new string[] { Convert.ToString(WebConfigurationManager.AppSettings["ADMIN_EMAIL"]) }, dictEmailValues, true);
                    }
                }
                catch (Exception ex)
                {
                }

                try
                {

                    if (!string.IsNullOrEmpty(Convert.ToString(WebConfigurationManager.AppSettings["ADMIN_MOBILE_NUMBER"])))
                    {
                        Dictionary<string, string> dictSMSValues = new Dictionary<string, string>();
                        BusinessLogicLayer.GateWay gateWay = new BusinessLogicLayer.SMS(System.Web.HttpContext.Current.Server);
                        dictSMSValues.Add("#--CUSTOMER_NAME--#", Name);
                        dictSMSValues.Add("#--CUSTOMER_MOBILE--#", PhoneNumber);
                        dictSMSValues.Add("#--CUSTOMER_EMAIL--#", EmailID);
                        gateWay.SendSMS(BusinessLogicLayer.GateWay.SMSGateWays.SUMIT,
                            BusinessLogicLayer.GateWay.SMSOptions.SINGLE, BusinessLogicLayer.GateWay.SMSTypes.ADM_CUST_REG_LIM_EXDS,
                            new string[] { Convert.ToString(WebConfigurationManager.AppSettings["ADMIN_MOBILE_NUMBER"]) }, dictSMSValues);
                    }


                }
                catch (Exception ex)
                {
                }

                IsSend = true;
            }
            catch (Exception ex)
            {
                IsSend = false;
            }
            return IsSend;
        }
        /// <summary>
        /// Generates Random number for OTP
        /// </summary>
        /// <param name="Length">Number of characters to return</param>
        /// <returns></returns>
        string GetRandomNumberOTP(int Length)
        {
            //string alphabets = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            //string small_alphabets = "abcdefghijklmnopqrstuvwxyz";
            string numbers = "1234567890";

            //string characters = alphabets + small_alphabets + numbers;


            string otp = string.Empty;
            for (int i = 0; i < Length; i++)
            {
                string character = string.Empty;
                do
                {
                    int index = new Random().Next(0, numbers.Length);
                    character = numbers.ToCharArray()[index].ToString();
                } while (otp.IndexOf(character) != -1);
                otp += character;
            }
            return otp;
        }
        private string ConvertViewToString(string viewName)
        {
            using (StringWriter writer = new StringWriter())
            {
                ViewEngineResult vResult = ViewEngines.Engines.FindPartialView(ControllerContext, viewName);
                ViewContext vContext = new ViewContext(this.ControllerContext, vResult.View, ViewData, new TempDataDictionary(), writer);
                vResult.View.Render(vContext, writer);
                return writer.ToString();
            }
        }


        [SessionExpire]
        public ActionResult Edit(Int64 id)
        {
            //Set Cookie for Url saving & Use in Continue shopping
            URLCookie.SetCookies();
            /*
               Indents:
             * Description: This is used to edit customer detail(personal detail)
             
             * Parameters: Id: Contains UserLoginId
             
             * Precondition: 
             * Postcondition:
             * Logic:    1) If id != session["UID"] the return access denied
             *           2) if yes, then get all customer details from personal detail table and store in our view model to display
             *           3) Store security question in viewBag
            
             */

            try
            {
                long lUserLoginID = 0;
                long.TryParse(Convert.ToString(Session["UID"]), out lUserLoginID);

                if (lUserLoginID != id)
                {
                    return View("AccessDenied");
                }

                // Get user personal details
                PersonalDetail personalDetail = db.PersonalDetails.Single(x => x.UserLoginID == id);

                // Storing salutation in ViewBag
                ViewBag.SalutationID = new SelectList(db.Salutations.Where(x => x.IsActive == true).ToList(), "ID", "Name", personalDetail.SalutationID);

                UserLogin lUserLogin = db.UserLogins.Find(id);

                // Set values to CustomerPersonalDetailViewModel viewModel
                CustomerPersonalDetailViewModel customerPersonalDetailViewModel = new CustomerPersonalDetailViewModel();

                customerPersonalDetailViewModel.ID = personalDetail.ID;
                customerPersonalDetailViewModel.UserLoginID = personalDetail.UserLoginID;
                customerPersonalDetailViewModel.FirstName = personalDetail.FirstName;
                customerPersonalDetailViewModel.MiddleName = personalDetail.MiddleName;
                customerPersonalDetailViewModel.LastName = personalDetail.LastName;
                customerPersonalDetailViewModel.EmailId = lUserLogin.Email;
                customerPersonalDetailViewModel.Mobile = lUserLogin.Mobile;
                customerPersonalDetailViewModel.DOB = personalDetail.DOB;
                customerPersonalDetailViewModel.Gender = personalDetail.Gender;
                customerPersonalDetailViewModel.PincodeID = Convert.ToString(personalDetail.PincodeID);
                customerPersonalDetailViewModel.Address = personalDetail.Address;
                customerPersonalDetailViewModel.AlternateEmailID = personalDetail.AlternateEmail;
                customerPersonalDetailViewModel.AlternateMobileNo = personalDetail.AlternateMobile;


                // Get security question and id
                var loginSecurityAnswerFromDB = db.LoginSecurityAnswers.Where(x => x.UserLoginID == id);

                if (loginSecurityAnswerFromDB.Count() != 0)
                {
                    var lAnswer = from element in db.LoginSecurityAnswers
                                  where element.UserLoginID == id
                                  select element;
                    foreach (var answer in lAnswer)
                    {
                        //customerPersonalDetailViewModel.SecurityQuestionID = 3;
                        if (answer.SecurityQuestionID != null)
                        {
                            customerPersonalDetailViewModel.SecurityQuestionID = Convert.ToInt32(answer.SecurityQuestionID);
                        }
                        customerPersonalDetailViewModel.SecurityAnswer = answer.Answer;
                    }
                }

                // Storing security question in ViewBag
                ViewBag.SecurityQuestionID = new SelectList(db.SecurityQuestions.Where(x => x.IsActive == true).ToList(), "ID", "Question", customerPersonalDetailViewModel.SecurityQuestionID);

                // Get pincode name by using id
                if (Convert.ToString(personalDetail.PincodeID) != string.Empty)
                {
                    Pincode pincodeFromDB = db.Pincodes.Single(x => x.ID == personalDetail.PincodeID);
                    customerPersonalDetailViewModel.PincodeID = pincodeFromDB.Name;
                }
                return View(customerPersonalDetailViewModel);
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[CustomerController][GET:Edit]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Gandhibagh, System.Web.HttpContext.Current.Server);

                return View();
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[CustomerController][GET:Edit]",
                    BusinessLogicLayer.ErrorLog.Module.Gandhibagh, System.Web.HttpContext.Current.Server);

                return View();
            }
        }

        [SessionExpire]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Exclude = "ID")] CustomerPersonalDetailViewModel model, FormCollection form)
        {
            /*
               Indents:
             * Description: This is used to edit customer detail(personal detail)
             
             * Parameters: Id: Contains UserLoginId
             
             * Precondition: 
             * Postcondition:
             * Logic:    1) If id != session["UID"] the return access denied
             *           2) if yes, then get all customer details from personal detail table and store in our view model to display
             *           3) Store security question in viewBag
             *           4) save login details, personal details, security answer details in database
            
             */

            try
            {
                Int32 lSalutation = 0, lSecurityQuestion = 0;

                int.TryParse(Convert.ToString(form["SalutationID"]), out lSalutation);
                int.TryParse(Convert.ToString(form["SecurityQuestionID"]), out lSecurityQuestion);

                // Storing security question in ViewBag
                ViewBag.SecurityQuestionID = new SelectList(db.SecurityQuestions.Where(x => x.IsActive == true).ToList(), "ID", "Question", lSecurityQuestion);

                // Storing salutation in ViewBag
                ViewBag.SalutationID = new SelectList(db.Salutations.ToList(), "ID", "Name", lSalutation);
                ModelState.Clear();
                model.LastName = "  ";
                if (ModelState.IsValid)
                {
                    long lUserLoginID = 0;
                    long.TryParse(Convert.ToString(Session["UID"]), out lUserLoginID);

                    // Check email exist or not

                    /*allow null email Address
                     * Pradnyakar Badge
                     * 05-01-2016
                     */
                    if (model.EmailId != null)
                    {
                        var isEmailExist = db.UserLogins.Where(x => x.ID != lUserLoginID && x.Email == model.EmailId).ToList();

                        if (isEmailExist.Count() > 0)
                        {
                            ViewBag.Message = "Email already exist!!";
                            return View(model);
                        }
                    }

                    // Check mobile exist or not
                    if (model.Mobile != null)
                    {
                        var isMobileExist = db.UserLogins.Where(x => x.ID != lUserLoginID && x.Mobile == model.Mobile).ToList();
                        if (isMobileExist.Count() > 0)
                        {
                            ViewBag.Message = "Mobile already exist!!";
                            return View(model);
                        }
                    }
                    //Check validation
                    if (model.Address == null || model.Address == string.Empty)
                    {
                        ViewBag.Message = "Please! Enter Address!!";
                        return View(model);
                    }
                    //if (model.DOB == null)
                    //{
                    //    ViewBag.Message = "Please! Enter DOB!!"; 
                    //    return View(model);
                    //}
                    if (model.Gender == null)
                    {
                        ViewBag.Message = "Please! Select Gender!!";
                        return View(model);
                    }
                    using (TransactionScope ts = new TransactionScope())
                    {
                        // Get pincode details from database
                        var lPincodeDB = from element in db.Pincodes
                                         where element.Name == model.PincodeID
                                         select element.ID;

                        // Check user entered pincode is valid or not
                        if (lPincodeDB.Count() == 0)
                        {
                            ViewBag.Message = "Invalid Pincode!!";
                            return View(model);
                        }

                        // Get pincode id
                        int lPincodeID = 0;
                        foreach (var element in lPincodeDB)
                        {
                            lPincodeID = element;
                        }

                        // Changes in User login

                        UserLogin userLoginDB = db.UserLogins.Single(x => x.ID == lUserLoginID);

                        UserLogin userLogin = new UserLogin();

                        userLogin.ID = userLoginDB.ID;
                        userLogin.Mobile = model.Mobile;
                        userLogin.Email = model.EmailId;
                        userLogin.Password = userLoginDB.Password;
                        userLogin.IsLocked = false;
                        userLogin.CreateBy = CommonFunctions.GetPersonalDetailsID(lUserLoginID);
                        userLogin.CreateDate = userLoginDB.CreateDate;
                        userLogin.ModifyBy = CommonFunctions.GetPersonalDetailsID(lUserLoginID);
                        userLogin.ModifyDate = DateTime.Now;

                        db.Entry(userLoginDB).CurrentValues.SetValues(userLogin);
                        db.SaveChanges();


                        // Changes in personal detail
                        PersonalDetail personalDetailDB = db.PersonalDetails.Single(x => x.UserLoginID == model.UserLoginID);

                        PersonalDetail personalDetail = new PersonalDetail();

                        personalDetail.ID = personalDetailDB.ID;
                        personalDetail.UserLoginID = personalDetailDB.UserLoginID;
                        personalDetail.SalutationID = lSalutation;
                        personalDetail.FirstName = model.FirstName;
                        personalDetail.MiddleName = model.MiddleName;
                        personalDetail.LastName = model.LastName;
                        personalDetail.DOB = model.DOB;
                        personalDetail.Gender = model.Gender;
                        personalDetail.PincodeID = lPincodeID;
                        personalDetail.Address = model.Address;
                        personalDetail.AlternateMobile = model.AlternateMobileNo;
                        personalDetail.AlternateEmail = null; //model.AlternateEmailID;
                        personalDetail.IsActive = personalDetailDB.IsActive;
                        personalDetail.CreateBy = personalDetailDB.CreateBy;
                        personalDetail.CreateDate = personalDetailDB.CreateDate;
                        personalDetail.ModifyBy = 1;
                        personalDetail.ModifyDate = DateTime.UtcNow.AddHours(5.5);
                        personalDetail.NetworkIP = personalDetailDB.NetworkIP;
                        personalDetail.DeviceID = personalDetailDB.DeviceID;
                        personalDetail.DeviceType = personalDetailDB.DeviceType;



                        db.Entry(personalDetailDB).CurrentValues.SetValues(personalDetail);
                        db.SaveChanges();

                        //Tejaswee (07-11-2015)
                        //Changes in customer shipping address
                        CustomerShippingAddress customershippingaddress = new CustomerShippingAddress();
                        customershippingaddress.PincodeID = lPincodeID;
                        customershippingaddress.PrimaryMobile = model.Mobile;
                        customershippingaddress.SecondaryMobile = model.AlternateMobileNo;
                        customershippingaddress.ShippingAddress = model.Address;

                        customershippingaddress.CreateDate = DateTime.UtcNow;
                        customershippingaddress.CreateBy = 1;
                        customershippingaddress.ModifyDate = null;
                        customershippingaddress.ModifyBy = null;
                        customershippingaddress.NetworkIP = string.Empty;
                        customershippingaddress.DeviceType = string.Empty;
                        customershippingaddress.DeviceID = string.Empty;
                        customershippingaddress.UserLoginID = personalDetailDB.UserLoginID;
                        customershippingaddress.IsActive = true;

                        db.CustomerShippingAddresses.Add(customershippingaddress);
                        db.SaveChanges();

                        // Changes in login security anwser
                        LoginSecurityAnswer loginSecurityAnswer = new LoginSecurityAnswer();

                        loginSecurityAnswer.ID = 0;
                        loginSecurityAnswer.UserLoginID = model.UserLoginID;
                        loginSecurityAnswer.SecurityQuestionID = lSecurityQuestion;
                        loginSecurityAnswer.Answer = model.SecurityAnswer;
                        loginSecurityAnswer.CreateBy = 1;
                        loginSecurityAnswer.CreateDate = DateTime.UtcNow.AddHours(5.5);

                        var loginSecurityQuestionDB = db.LoginSecurityAnswers.Where(x => x.UserLoginID == model.UserLoginID);

                        // Execute if security question not exists!!
                        if (loginSecurityQuestionDB.Count() == 0)
                        {
                            db.LoginSecurityAnswers.Add(loginSecurityAnswer);
                            db.SaveChanges();
                        }
                        else
                        {
                            // Execute if security question exists!!
                            var lLoginSecurityAnswerDB = db.LoginSecurityAnswers.Single(x => x.UserLoginID == model.UserLoginID);

                            var lloginsecurityanswerdb = from element in db.LoginSecurityAnswers
                                                         where element.SecurityQuestionID == lSecurityQuestion
                                                         select element.ID;
                            Int64 ID = 0;
                            foreach (var element in lloginsecurityanswerdb)
                            {
                                ID = element;
                            }
                            loginSecurityAnswer.ID = lLoginSecurityAnswerDB.ID;

                            db.Entry(lLoginSecurityAnswerDB).CurrentValues.SetValues(loginSecurityAnswer);
                            db.SaveChanges();
                        }
                        ViewBag.Message = "Details Updated Successfully!!";

                        // Commit transaction!!
                        ts.Complete();
                        Session["FirstName"] = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(personalDetail.FirstName.ToLower());
                        TempData["IsLeadersSignUp"] = "User Profile Updated Successfully!!";
                        return RedirectToRoute("Home", new { city = Request.Cookies["CityCookie"].Value.Split('$')[1].ToLower(), franchiseId = Request.Cookies["CityCookie"].Value.Split('$')[2].Trim() });
                        //return View(model);
                    }
                }
                return View(model);
            }
            catch (DbEntityValidationException ex)
            {
                // Retrieve the error messages as a list of strings.
                var errorMessages = ex.EntityValidationErrors
                        .SelectMany(x => x.ValidationErrors)
                        .Select(x => new { x.ErrorMessage });

                // Join the list to a single string.
                var fullErrorMessage = string.Join("; ", errorMessages);

                // Combine the original exception message with the new one.
                var exceptionMessage = string.Concat(ex.Message, " The validation errors are: ", fullErrorMessage);
                ViewBag.Message = fullErrorMessage.Replace("ErrorMessage = ", " ");
                // Throw a new DbEntityValidationException with the improved exception message.
                //throw new DbEntityValidationException(exceptionMessage, ex.EntityValidationErrors);
                return View(model);
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                ViewBag.Message = "Sorry! Problem in updating customer details!!";

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[CustomerController][POST:Edit]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Gandhibagh, System.Web.HttpContext.Current.Server);

                return View(model);
            }
            catch (Exception ex)
            {
                ViewBag.Message = "Sorry! Problem in updating customer details!!";

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[CustomerController][POST:Edit]",
                    BusinessLogicLayer.ErrorLog.Module.Gandhibagh, System.Web.HttpContext.Current.Server);

                return View(model);
            }
        }

        public ActionResult Logout()
        {
            /*
               Indents:
             * Description: This method is used to logout customer
             
             * Parameters: 
             
             * Precondition: 
             * Postcondition:
             * Logic: 
             */
            try
            {
                Session["UID"] = null;
                Session["UserName"] = null;
                Session["FirstName"] = null;
                //ShoppingCartInitialization lShoppingCartInitialization = new ShoppingCartInitialization();
                //lShoppingCartInitialization.DeleteShoppingCartCookie();
                //HttpContext.Request.Cookies.Remove("FirstName");

                Request.Cookies.Clear();
                Session.Clear();
                Session.Abandon();
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[CustomerController][GET:Logout]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Gandhibagh, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[CustomerController][GET:Logout]",
                    BusinessLogicLayer.ErrorLog.Module.Gandhibagh, System.Web.HttpContext.Current.Server);
            }
            return RedirectToAction("Index", "Home");
        }

        //public ActionResult Test()
        //{
        //    return View();
        //}

        //private void SendEmailToCustomer(string emailID)
        //{
        //    /*
        //       Indents:
        //     * Description: This method is used to send email to customer

        //     * Parameters: 

        //     * Precondition: 
        //     * Postcondition:
        //     * Logic: 
        //     */
        //    try
        //    {
        //        Dictionary<string, string> dictEmailValues = new Dictionary<string, string>();
        //      BusinessLogicLayer.GateWay gateWay = new BusinessLogicLayer.Email(System.Web.HttpContext.Current.Server);
        //     gateWay.SendEmail(BusinessLogicLayer.GateWay.EmailGateWays.GANDHIBAGH,
        //         BusinessLogicLayer.GateWay.SenderMail.INFO, BusinessLogicLayer.GateWay.EMailTypes.CUST_WELCOME,
        //         new string[] { emailID }, dictEmailValues, true);
        //    }
        //    catch (Exception)
        //    {

        //       //Mantain Error Log
        //    }
        //}

        //private void SendMessageToCustomer(string mobileNo,string name)
        //{
        //    /*
        //       Indents:
        //     * Description: This method is used to send message to customer

        //     * Parameters: 

        //     * Precondition: 
        //     * Postcondition:
        //     * Logic: 
        //     */

        //    try
        //    {
        //        Dictionary<string, string> dictSMSValues = new Dictionary<string, string>();
        //        BusinessLogicLayer.GateWay gateWay = new BusinessLogicLayer.SMS(System.Web.HttpContext.Current.Server);
        //        dictSMSValues.Add("#--NAME--#", name);
        //        gateWay.SendSMS(BusinessLogicLayer.GateWay.SMSGateWays.SUMIT,
        //            BusinessLogicLayer.GateWay.SMSOptions.SINGLE, BusinessLogicLayer.GateWay.SMSTypes.CUST_WELCOME,
        //            new string[] { mobileNo     }, dictSMSValues);
        //    }
        //    catch (Exception)
        //    {

        //        //Mantain Error Log
        //    }
        //}

        private void SendEmailToCustomer(string emailID, string name)
        {
            /*
               Indents:
             * Description: This method is used to send email to customer
             
             * Parameters: 
             
             * Precondition: 
             * Postcondition:
             * Logic: 
             */
            try
            {
                string city = URLsFromConfig.GetDefaultData("CITY_NAME");//Yashaswi 01/12/2018 Default City Change 
                int FranchiseID = Convert.ToInt16(URLsFromConfig.GetDefaultData("FRANCHISE_ID"));//Yashaswi 01/12/2018 Default City Change 
                if (System.Web.HttpContext.Current.Request.Cookies["CityCookie"].Value != null)
                {
                    city = System.Web.HttpContext.Current.Request.Cookies["CityCookie"].Value.Split('$')[1].ToLower();
                    FranchiseID = Convert.ToInt32(Request.Cookies["CityCookie"].Value.Split('$')[2]);////added
                }
                Dictionary<string, string> dictEmailValues = new Dictionary<string, string>();
                //dictEmailValues.Add("<!--ACCOUNT_URL-->", "http://www.ezeelo.com/login/login");
                //dictEmailValues.Add("<!--ORDERS_URL-->", "http://ezeelo.com/CustomerOrder/MyOrders");
                // dictEmailValues.Add("<!--ACCOUNT_URL-->", "http://ezeelo.com/" + city + "/login");////hide
                // dictEmailValues.Add("<!--ORDERS_URL-->", "http://www.ezeelo.com/" + city + "/cust-o/my-order");////hide
                dictEmailValues.Add("<!--ACCOUNT_URL-->", "" + (new URLsFromConfig()).GetURL("CUSTOMER") + "" + city + "/" + FranchiseID + "/login");////added "/" + FranchiseID +
                dictEmailValues.Add("<!--ORDERS_URL-->", "" + (new URLsFromConfig()).GetURL("CUSTOMER") + "" + city + "/" + FranchiseID + "/cust-o/my-order");////added "/" + FranchiseID +
                dictEmailValues.Add("<!--NAME-->", name);
                BusinessLogicLayer.GateWay gateWay = new BusinessLogicLayer.Email(System.Web.HttpContext.Current.Server);
                gateWay.SendEmail(BusinessLogicLayer.GateWay.EmailGateWays.GANDHIBAGH,
                    BusinessLogicLayer.GateWay.SenderMail.INFO, BusinessLogicLayer.GateWay.EMailTypes.CUST_WELCOME,
                    new string[] { emailID, "sales@ezeelo.com" }, dictEmailValues, true);
            }
            catch (Exception)
            {

                //Mantain Error Log
            }
        }

        private void SendMessageToCustomer(string mobileNo, string name)
        {
            /*
               Indents:
             * Description: This method is used to send message to customer
             
             * Parameters: 
             
             * Precondition: 
             * Postcondition:
             * Logic: 
             */

            try
            {
                string RefId = "";
                UserLogin user = db.UserLogins.FirstOrDefault(p => p.Mobile == mobileNo);
                if (user != null)
                {
                    MLMUser mLMUser = db.MLMUsers.FirstOrDefault(p => p.UserID == user.ID);
                    if (mLMUser != null)
                    {
                        RefId = mLMUser.Ref_Id;
                    }
                }
                Dictionary<string, string> dictSMSValues = new Dictionary<string, string>();
                BusinessLogicLayer.GateWay gateWay = new BusinessLogicLayer.SMS(System.Web.HttpContext.Current.Server);
                dictSMSValues.Add("#--NAME--#", name);
                dictSMSValues.Add("#--REFID--#", RefId);
                gateWay.SendSMS(BusinessLogicLayer.GateWay.SMSGateWays.SUMIT,
                    BusinessLogicLayer.GateWay.SMSOptions.SINGLE, BusinessLogicLayer.GateWay.SMSTypes.CUST_WELCOME,
                    new string[] { mobileNo }, dictSMSValues);
            }
            catch (Exception)
            {

                //Mantain Error Log
            }
        }

    }
}