//-----------------------------------------------------------------------
// <copyright file="CustomerLogin.cs" company="Ezeelo Consumer Services Pvt. Ltd.">
//     Copyright (c) Ezeelo Consumer Services Pvt. Ltd. All rights reserved.
// </copyright>
// <author>Sujata Kullarkar</author>
//-----------------------------------------------------------------------
using ModelLayer;
using ModelLayer.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Data.Entity;
/*
 Handed over to Mohit, Tejaswee, Pradnyakar Sir
 */
namespace BusinessLogicLayer
{
    public class CustomerLogin : CustomerDetails
    {
        public CustomerLogin(System.Web.HttpServerUtility server): base(server)
        {

        }
        /// <summary>
        /// Validate user entered credentials and authenticate login request
        /// </summary>
        /// <param name="login">LoginViewModel</param>
        /// <returns>User Login ID and UserName</returns>
        public CustomerCredentials Login(LoginViewModel login)
        {
            CustomerCredentials lUserDetails = new CustomerCredentials();

            if (IsValidEmailId(login.UserName) || IsValidMobile(login.UserName))
            {
                if (!string.IsNullOrEmpty(login.ReferralId))
                {
                    if (IsValidReferralId(login.ReferralId))
                    {
                        lUserDetails = this.CheckLogin(login.UserName, login.Password);
                    }
                }
                else
                {
                    lUserDetails = this.CheckLogin(login.UserName, login.Password);
                }

            }
            return lUserDetails;
        }
        //---------------------------------Hide EPOD from Ashish for Live------------------------------------------------------

        ///// <summary>
        ///// Add by Ashish Nagrale
        ///// Validate user entered credentials and authenticate login request for Delivery partner and Employee
        ///// </summary>
        ///// <param name="login">LoginViewModel</param>
        ///// <returns>User Login ID and UserName</returns>
        //public CustomerCredentials UserLogin(LoginViewModel login, string ReqBy, string IMEI)
        //{
        //    CustomerCredentials lUserDetails = new CustomerCredentials();

        //    if (IsValidEmailId(login.UserName) || IsValidMobile(login.UserName))
        //    {
        //        lUserDetails = this.CheckUserLogin(login.UserName, login.Password, ReqBy, IMEI);

        //    }
        //    return lUserDetails;
        //}


        ///// <summary>
        ///// Add By Ashish Nagrale
        /////logout request for Delivery partner and Employee
        ///// </summary>
        ///// <param name="logout"></param>
        ///// <returns>bool</returns>
        ////public bool UserLogout(string userlogout)
        //public CustomerCredentials UserLogout(Logout userlogout, string ReqBy, string IMEI)
        //{
        //    CustomerCredentials lUserDetails = new CustomerCredentials();
        //    // bool check = UpdateDeliveryBoyLogoutDetail(userlogout.UserName,ReqBy,IMEI);

        //    if (IsValidEmailId(userlogout.UserName) || IsValidMobile(userlogout.UserName))
        //    {
        //        lUserDetails = UpdateDeliveryBoyLogoutDetail(userlogout.UserName, ReqBy, IMEI);

        //    }
        //    return lUserDetails;
        //}

        //-----------------------------------------------------------------------------------------


        /// <summary>
        /// Email Address validation
        /// </summary>
        /// <param name="pInputEmail">email address</param>
        /// <returns>Validation status</returns>
        public bool IsValidEmailId(string pInputEmail)
        {
            //Regex To validate Email Address
            Regex regex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
            Match match = regex.Match(pInputEmail);
            if (match.Success)
                return true;
            else
                return false;
        }
        /// <summary>
        /// Mobile No. validation
        /// </summary>
        /// <param name="pInputEmail">Mobile no</param>
        /// <returns>Validation status</returns>
        public bool IsValidMobile(string pInputMobile)
        {
            //Regex To validate Email Address
            Regex regex = new Regex(@"^[6-9]{1}[0-9]{9}$");
            Match match = regex.Match(pInputMobile);
            if (match.Success)
                return true;

            else
                return false;
        }

        //Sonali 27-08-2018
        public bool IsValidReferralId(string ReferralId)
        {
            bool Result = db.MLMUsers.Any(p => p.Ref_Id == ReferralId);
            return Result;
        }

        /// <summary>
        /// Get Customer's Login Id and User Name Depending on his\her entered User name and Password
        /// </summary>
        /// <param name="pUserName">UserName</param>
        /// <param name="pPassword">Password</param>
        /// <returns></returns>
        private CustomerCredentials CheckLogin(string pUserName, string pPassword)
        {
            CustomerCredentials lUserDetails = new CustomerCredentials();
            var userExist = db.UserLogins.Select(x => new { x.Email, x.Mobile, x.Password, x.ID, x.PersonalDetail.FirstName, x.PersonalDetail.LastName, x.IsLocked })//Sonali_29-10-2018_added last 2 parameter
                                 .Where(x => (x.Email == pUserName || x.Mobile == pUserName) && x.Password.Equals(pPassword, StringComparison.CurrentCulture) == true).ToList();
            if (userExist.Count() > 0)
            {

                foreach (var item in userExist)
                {
                    if (item.Password.Equals(pPassword, StringComparison.CurrentCulture) && (item.Email.Equals(pUserName, StringComparison.CurrentCulture) || item.Mobile.Equals(pUserName, StringComparison.CurrentCulture)))//Changes by Sonali for CaseSensitive UserName and Password on 11-02-2019
                    {
                        lUserDetails.UserLoginID = item.ID;
                        if (item.Email != null)
                        {
                            lUserDetails.UserName = item.Email.ToString();
                        }
                        lUserDetails.MobileNo = item.Mobile;
                        if (item.Email != null)
                        {
                            lUserDetails.EmailID = item.Email.ToString();
                        }
                        lUserDetails.FirstName = db.PersonalDetails.Where(x => x.UserLoginID == item.ID).Select(x => x.FirstName).FirstOrDefault();//Sonali_24-01-2019
                        lUserDetails.LastName = db.PersonalDetails.Where(x => x.UserLoginID == item.ID).Select(x => x.LastName).FirstOrDefault();//Sonali_24-01-2019
                        lUserDetails.IsLocked = item.IsLocked;
                    }
                    //lUserDetails.FirstName = item.FirstName;//Sonali_29-10-2018
                    //lUserDetails.LastName = item.LastName;//Sonali_29-10-2018
                }
            }

            return lUserDetails;
        }
        //---------------------------------Hide EPOD from Ashish for Live------------------------------------------------------
        ///// <summary>
        ///// Add By Ashish Nagrale
        ///// Get Delivered Partner's and Employee's Login Id and User Name Depending on his\her entered User name and Password
        ///// </summary>
        ///// <param name="pUserName">UserName</param>
        ///// <param name="pPassword">Password</param>
        ///// <returns></returns>

        //private CustomerCredentials CheckUserLogin(string pUserName, string pPassword, string ReqBy, string IMEI)
        //{
        //    try
        //    {
        //        CustomerCredentials lUserDetails = new CustomerCredentials();
        //        //lUserDetails = lUserDetailsOR;
        //        //var userExist = from ul in db.UserLogins select ul.ID;
        //        ///<summary>
        //        /// Checking for Delivery Partner Login
        //        /// </summary>
        //        var userExist = from ul in db.UserLogins
        //                        join bd in db.BusinessDetails on ul.ID equals bd.UserLoginID
        //                        join bt in db.BusinessTypes on bd.BusinessTypeID equals bt.ID
        //                        where ((ul.Email == pUserName || ul.Mobile == pUserName) && ul.Password == pPassword
        //                                && ul.IsLocked == false && bd.IsActive == true && bt.IsActive == true
        //                        )
        //                        select new
        //                        {
        //                            ID = ul.ID,
        //                            Email = ul.Email,
        //                            Mobile = ul.Mobile,
        //                            BusinessType = bt.Name
        //                        };
        //        if (userExist.Count() > 0)
        //        {
        //            foreach (var item in userExist)
        //            {
        //                lUserDetails.UserLoginID = item.ID;
        //                lUserDetails.UserName = item.Email.ToString();
        //                lUserDetails.MobileNo = item.Mobile;
        //                lUserDetails.EmailID = item.Email.ToString();
        //                lUserDetails.LoginType = item.BusinessType.ToString();
        //            }
        //        }
        //        else
        //        {
        //            ///<summary>
        //            /// Checking for Employee Login
        //            /// </summary>

        //            var IsLogin = (from dba in db.DeliveryBoyAttendances
        //                           join ul in db.UserLogins on dba.UserLoginID equals ul.ID
        //                           where (ul.Mobile == pUserName || ul.Email == pUserName) && dba.LogoutDateTime == null
        //                           orderby dba.LoginDateTime descending
        //                           select new
        //                           {
        //                               ID = dba.ID,
        //                               UserLoginID = dba.UserLoginID,
        //                               LoginDateTime = dba.LoginDateTime,
        //                               LogoutDateTime = dba.LogoutDateTime
        //                           }).Take(1); //FirstOrDefault();//<-For Max() //.Take(1);
        //            if (IsLogin.Count() == 0)
        //            {
        //                //--Insert Login Record --//
        //                var innerquery = from bt in db.BusinessTypes where bt.ID == 3 && bt.IsActive == true select bt.Prefix;
        //                var userExist2 = from ul in db.UserLogins
        //                                 join e in db.Employees on ul.ID equals e.UserLoginID
        //                                 where ((ul.Email == pUserName || ul.Mobile == pUserName) && ul.Password == pPassword && e.IsActive == true
        //                                         && ul.IsLocked == false && innerquery.Contains(e.EmployeeCode.Substring(0, 4))
        //                                 )
        //                                 select new
        //                                 {
        //                                     ID = ul.ID,
        //                                     Email = ul.Email,
        //                                     Mobile = ul.Mobile,
        //                                     BusinessType = "Delivery Partner-Employee"
        //                                 };
        //                if (userExist2.Count() > 0)
        //                {
        //                    foreach (var item in userExist2)
        //                    {
        //                        lUserDetails.UserLoginID = item.ID;
        //                        lUserDetails.UserName = item.Email.ToString();
        //                        lUserDetails.MobileNo = item.Mobile;
        //                        lUserDetails.EmailID = item.Email.ToString();
        //                        lUserDetails.LoginType = item.BusinessType.ToString();
        //                    }
        //                    InsertDeliveryBoyLoginDetail(lUserDetails.UserLoginID, ReqBy, IMEI);
        //                }

        //            }
        //            else
        //            {
        //                //--Already Loigged In.--//
        //                lUserDetails.LoginStatus = "Already Loigged In.";
        //            }
        //        }


        //        return lUserDetails;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        ///// <summary>
        ///// Add By Ashish Nagrale
        ///// Insert Attendance Record when Delivery Boy login
        ///// </summary>
        ///// <param name="userlogin"></param>
        //public void InsertDeliveryBoyLoginDetail(long userlogin, string ReqBy, string IMEI)
        //{

        //    try
        //    {
        //        ModelLayer.Models.DeliveryBoyAttendance dbatt = new ModelLayer.Models.DeliveryBoyAttendance();

        //        if (ReqBy == "m" && IMEI.Length > 5)
        //        {
        //            ReqBy = "Mobile";
        //        }
        //        else
        //        {
        //            ReqBy = "Desktop";
        //            IMEI = "";
        //        }
        //        var IstableEmpty = from edba in db.DeliveryBoyAttendances select edba.ID;
        //        if (IstableEmpty.Count() > 0)
        //        {
        //            var IsFirstTimeLogin=(from dba in db.DeliveryBoyAttendances
        //                            where dba.UserLoginID == userlogin
        //                                  select dba.UserLoginID);
        //            if (IsFirstTimeLogin.Count() == 0)
        //            {
        //                dbatt.ID = 0;
        //                dbatt.UserLoginID = userlogin;
        //                dbatt.LoginDateTime = CommonFunctions.GetLocalTime();
        //                dbatt.LoginNetworkIP = BusinessLogicLayer.CommonFunctions.GetClientIP();
        //                dbatt.LoginDeviceType = ReqBy;
        //                dbatt.LoginDeviceID = IMEI;
        //                db.DeliveryBoyAttendances.Add(dbatt);
        //                db.SaveChanges();
        //            }
        //            else
        //            {
        //                var IsLogout = (from dba in db.DeliveryBoyAttendances
        //                                where dba.UserLoginID == userlogin && dba.LogoutDateTime != null
        //                                orderby dba.LoginDateTime descending
        //                                select dba.UserLoginID).Take(1);

        //                if (IsLogout.Count() > 0)
        //                {
        //                    dbatt.ID = 0;
        //                    dbatt.UserLoginID = userlogin;
        //                    dbatt.LoginDateTime = CommonFunctions.GetLocalTime();
        //                    dbatt.LoginNetworkIP = BusinessLogicLayer.CommonFunctions.GetClientIP();
        //                    dbatt.LoginDeviceType = ReqBy;
        //                    dbatt.LoginDeviceID = IMEI;
        //                    db.DeliveryBoyAttendances.Add(dbatt);
        //                    db.SaveChanges();
        //                }
        //            }
        //        }
        //        else
        //        {
        //            dbatt.ID = 0;
        //            dbatt.UserLoginID = userlogin;
        //            dbatt.LoginDateTime = CommonFunctions.GetLocalTime();
        //            dbatt.LoginNetworkIP = BusinessLogicLayer.CommonFunctions.GetClientIP();
        //            dbatt.LoginDeviceType = ReqBy;
        //            dbatt.LoginDeviceID = IMEI;
        //            db.DeliveryBoyAttendances.Add(dbatt);
        //            db.SaveChanges();
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}
        ///// <summary>
        ///// Add By Ashish Nagrale
        ///// Logout for user -> Delivery Partner and Delivery Boy (Employee)
        ///// </summary>
        ///// <param name="userlogin"></param>
        ///// <returns></returns>
        ////public bool UpdateDeliveryBoyLogoutDetail(string userlogout)
        //public CustomerCredentials UpdateDeliveryBoyLogoutDetail(string userlogout, string ReqBy, string IMEI)
        //{
        //    try
        //    {
        //        CustomerCredentials lUserDetails = new CustomerCredentials();
        //        if (ReqBy == "m" && IMEI.Length > 5)
        //        {
        //            ReqBy = "Mobile";
        //        }
        //        else
        //        {
        //            ReqBy = "Desktop";
        //            IMEI = "";
        //        }
        //        var IsLogin = (from dba in db.DeliveryBoyAttendances
        //                       join ul in db.UserLogins on dba.UserLoginID equals ul.ID
        //                       where (ul.Mobile == userlogout || ul.Email == userlogout) && dba.LogoutDateTime == null
        //                       orderby dba.LoginDateTime descending
        //                       select new
        //                       {
        //                           ID = dba.ID,
        //                           UserLoginID = dba.UserLoginID,
        //                           LoginDateTime = dba.LoginDateTime,
        //                           LogoutDateTime = dba.LogoutDateTime,
        //                           LoginNetworkIP = dba.LoginNetworkIP,
        //                           LoginDeviceType = dba.LoginDeviceType,
        //                           LoginDeviceID = dba.LoginDeviceID
        //                       }).Take(1);

        //        if (IsLogin.Count() > 0)
        //        {
        //            //--Update Logout Record --//
        //            ModelLayer.Models.DeliveryBoyAttendance dbatt = new ModelLayer.Models.DeliveryBoyAttendance();
        //            foreach (var item in IsLogin)
        //            {
        //                dbatt.ID = item.ID;
        //                dbatt.UserLoginID = item.UserLoginID;
        //                dbatt.LoginDateTime = item.LoginDateTime;
        //                dbatt.LoginNetworkIP = item.LoginNetworkIP;
        //                dbatt.LoginDeviceType = item.LoginDeviceType;
        //                dbatt.LoginDeviceID = item.LoginDeviceID;
        //                dbatt.LogoutDateTime = CommonFunctions.GetLocalTime();
        //                dbatt.LogoutNetworkIP = BusinessLogicLayer.CommonFunctions.GetClientIP();
        //                dbatt.logoutDeviceType = ReqBy;
        //                dbatt.logoutDeviceID = IMEI;

        //                //db.Entry(area).State = EntityState.Modified;
        //                db.Entry(dbatt).State = EntityState.Modified;
        //                db.SaveChanges();

        //            }
        //            //  return true;
        //            lUserDetails.LoginStatus = "Logout Successfull.";
        //        }
        //        else
        //        {
        //            //--Already Loigged Out.--//
        //            lUserDetails.LoginStatus = "Already Loigged Out.";
        //        }
        //        //  return false;
        //        return lUserDetails;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }

        //}

        //------------------------------------------------------------------------------------------
     

    }
    /// <summary>
    /// This Class is to to send the response after successfull login.
    /// </summary>
    public class CustomerCredentials
    {
        public long UserLoginID { get; set; }
        public string UserName { get; set; }
        public string EmailID { get; set; }

        public string MobileNo { get; set; }
        public string FirstName { get; set; }//Sonali_29-10-2018 For APi return
        public string LastName { get; set; }//Sonali_29-10-2018 For APi return
        public bool IsMLMUser { get; set; }//Sonali_29-10-2018 For APi return
        public bool IsLocked { get; set; }
        //---------------------------------Hide EPOD from Ashish for Live------------------------------------------------------
        // Add by Ashish Nagtrale //
        /* public string LoginType { get; set; }
         public string LoginStatus { get; set; }*/

        //------------------------------------------------------------------------------------------------------------------------
    }
}
