//-----------------------------------------------------------------------
// <copyright file="CustomerDetails.cs" company="Ezeelo Consumer Services Pvt. Ltd.">
//     Copyright (c) Ezeelo Consumer Services Pvt. Ltd. All rights reserved.
// </copyright>
// <author>Sujata Kullarkar</author>
//-----------------------------------------------------------------------
using ModelLayer.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModelLayer.Models;
using System.Transactions;
using System.Data.Entity.Validation;
using System.Data.SqlClient;
using System.Data;
/*
 Handed over to Mohit, Tejaswee, Pradnyakar Sir
 */
namespace BusinessLogicLayer
{
    public class CustomerDetails : CustomerManagement
    {

        /// <summary>
        /// calling Base class Constructor
        /// </summary>
        /// <param name="server">System.Web.HttpContext.Current.Server</param>
        public CustomerDetails(System.Web.HttpServerUtility server)
            : base(server)
        {

        }
        /// <summary>
        /// Get Customer's Personal and Login details
        /// </summary>
        /// <param name="custLoginID">Customer Login ID</param>
        /// <returns>CustomerDetailsViewModel</returns>
        //public CustomerDetailsViewModel GetCustomerDetails(long custLoginID)
        //{
        //    CustomerDetailsViewModel custDetails = new CustomerDetailsViewModel();
        //    custDetails = (from cust in db.UserLogins
        //                   join personal in db.PersonalDetails on cust.ID equals personal.UserLoginID
        //                   where cust.IsLocked == false && personal.IsActive == true && cust.ID == custLoginID
        //                   select new CustomerDetailsViewModel
        //                   {
        //                       UserLoginID = cust.ID,
        //                       Address = personal.Address,
        //                       AlternateEmail = personal.AlternateEmail,
        //                       AlternateMobile = personal.AlternateMobile,
        //                       DOB = personal.DOB,
        //                       EmailId = cust.Email,
        //                       FirstName = personal.FirstName,
        //                       MiddleName = personal.MiddleName,
        //                       LastName = personal.LastName,
        //                       MobileNo = cust.Mobile,
        //                       SalutationID = personal.SalutationID,
        //                       PincodeID = personal.PincodeID
        //                   }).FirstOrDefault();

        //    if (custDetails != null)
        //    {
        //        if (custDetails.PincodeID != null)
        //            custDetails.Pincode = db.Pincodes.Find(custDetails.PincodeID).Name;
        //        custDetails.SalutationName = db.Salutations.Find(custDetails.SalutationID).Name;
        //    }

        //    return custDetails;

        //}
        //Sonali 27-08-2018 
        public CustomerDetailsViewModel GetCustomerDetails(long custLoginID)
        {
            CustomerDetailsViewModel custDetails = new CustomerDetailsViewModel();
            db.Configuration.ProxyCreationEnabled = false;
            custDetails = (from cust in db.UserLogins
                           join personal in db.PersonalDetails on cust.ID equals personal.UserLoginID
                           //join securityAns in db.LoginSecurityAnswers on cust.ID equals securityAns.UserLoginID
                           where cust.IsLocked == false && personal.IsActive == true && cust.ID == custLoginID
                           select new CustomerDetailsViewModel
                           {
                               UserLoginID = cust.ID,
                               Address = personal.Address,
                               AlternateEmail = personal.AlternateEmail,
                               AlternateMobile = personal.AlternateMobile,
                               DOB = personal.DOB,
                               EmailId = cust.Email,
                               FirstName = personal.FirstName,
                               MiddleName = personal.MiddleName,
                               LastName = personal.LastName,
                               MobileNo = cust.Mobile,
                               SalutationID = personal.SalutationID,
                               PincodeID = personal.PincodeID,
                               Gender = personal.Gender,
                               Password = cust.Password,
                               // AnswerList = new List<LoginSecurityAnswer> { securityAns },
                               AvgIncome = 0
                           }).FirstOrDefault();

            if (custDetails != null)
            {
                var Answerlist = db.LoginSecurityAnswers.Where(x => x.UserLoginID == custDetails.UserLoginID).ToList();
                if (Answerlist != null)
                    custDetails.AnswerList = Answerlist;
                //join kyc in db.KYCModels on cust.ID equals kyc.UserLoginID
                //           join bank in db.Banks on kyc.BankID equals bank.ID
                //           join mlm in db.MLMUsers on cust.ID equals mlm.UserID
                var kyc = db.KYCModels.Where(x => x.UserLoginID == custDetails.UserLoginID).FirstOrDefault();
                if (kyc != null)
                {
                    custDetails.AdhaarNo = kyc.AdhaarNo;
                    custDetails.PanNo = kyc.PanNo;
                    custDetails.AccountNo = kyc.AccountNo;
                    custDetails.BankIFSC = kyc.BankIFSC;
                    custDetails.BranchName = kyc.BranchName;
                    custDetails.AccountType = kyc.AccountType;
                    custDetails.AdhaarImageUrl = kyc.AdhaarImageUrl;
                    custDetails.PanImageUrl = kyc.PanImageUrl;
                    custDetails.PassbookImageUrl = kyc.PassbookImageUrl;
                    var bank = db.Banks.Where(x => x.ID == kyc.BankID).FirstOrDefault();
                    if (bank != null)
                    {
                        custDetails.BankName = bank.Name;
                    }
                }
                var mlm = db.MLMUsers.Where(x => x.UserID == custDetails.UserLoginID).FirstOrDefault();
                if (mlm != null)
                {
                    custDetails.ReferralId = mlm.Ref_Id;
                }
                double Result = 0;
                DateTime FromDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                DateTime ToDate = FromDate.AddMonths(1).AddDays(-1).AddHours(11).AddMinutes(30);
                var idParam = new SqlParameter
                {
                    ParameterName = "UserID",
                    Value = custDetails.UserLoginID,
                };

                var IsERP = new SqlParameter
                {
                    ParameterName = "IsERP",
                    Value = 1,
                };
                var CurrentMonth = new SqlParameter
                {
                    ParameterName = "CurrentMonth",
                    Value = 1,
                };
                var Hour = new SqlParameter
                {
                    ParameterName = "Hour",
                    Value = 48,
                };
                var AllData = new SqlParameter
                {
                    ParameterName = "AllData",
                    Value = 0,
                };
                var DateFrom = new SqlParameter
                {
                    ParameterName = "DateFrom",
                    Value = FromDate
                };
                var DateTo = new SqlParameter
                {
                    ParameterName = "DateTo",
                    Value = ToDate
                };
                var TotalPoints = new SqlParameter
                {
                    ParameterName = "TotalPoints",
                    Direction = ParameterDirection.Output,
                    DbType = DbType.Decimal,
                    Precision = 18,
                    Scale = 4
                };
                db.Database.ExecuteSqlCommand("Leaders_SingleUser_ERP @UserID, @IsERP,@CurrentMonth,@Hour,@AllData,@DateFrom,@DateTo,@TotalPoints output", idParam, IsERP, CurrentMonth, Hour, AllData, DateFrom, DateTo, TotalPoints);
                if (TotalPoints != null)
                {
                    Result = Convert.ToDouble(TotalPoints.Value);
                }
                Math.Round(Result, 2);


                // var idParam = new SqlParameter
                // {
                //     ParameterName = "UserID",
                //     Value = custDetails.UserLoginID,
                // };

                // var IsERP = new SqlParameter
                // {
                //     ParameterName = "IsERP",
                //     Value = 1,
                // };
                // var CurrentMonth = new SqlParameter
                // {
                //     ParameterName = "CurrentMonth",
                //     Value = 1,
                // };
                // var Hour = new SqlParameter
                // {
                //     ParameterName = "Hour",
                //     Value = 48,
                // };

                // var AllData = new SqlParameter
                // {
                //     ParameterName = "AllData",
                //     Value = 0,
                // };
                // var TotalPoints = new SqlParameter
                // {
                //     ParameterName = "TotalPoints",
                //     Direction = ParameterDirection.Output,
                //     DbType = DbType.Decimal,
                //     Precision = 18,
                //     Scale = 4
                // };

                //// var objERPValue = db.Database.SqlQuery<double>("Leaders_SingleUser_ERP @UserID, @IsERP,@CurrentMonth,@Hour,@AllData,@TotalPoints OUTPUT", idParam, IsERP, CurrentMonth, Hour, AllData, TotalPoints).ToList<double>();
                // var objERPValue = db.Database.ExecuteSqlCommand("Leaders_SingleUser_ERP @UserID, @IsERP,@CurrentMonth,@Hour,@AllData,@TotalPoints output", idParam, IsERP, CurrentMonth, Hour, AllData, TotalPoints);
                // if (TotalPoints != null)
                // {
                //     Result = Convert.ToDouble(TotalPoints.Value);
                // }
                ////return Math.Round(Result, 2);
                //if (objERPValue != null)
                //{
                //    Result = objERPValue;
                //}
                custDetails.AvgErp = Convert.ToDecimal(Result);

                custDetails.QuetionsList = db.SecurityQuestions.Where(x => x.IsActive).ToList();
                if (custDetails.PincodeID != null)
                    custDetails.Pincode = db.Pincodes.Find(custDetails.PincodeID).Name;
                if (custDetails.SalutationID != null && custDetails.SalutationID > 0)
                    custDetails.SalutationName = db.Salutations.Find(custDetails.SalutationID).Name;
            }

            return custDetails;

        }
       
        /// <summary>
        /// Create customer profile
        /// </summary>
        /// <param name="lcust">Object of CustomerDetailsViewModel</param>
        /// <returns>0: No operation; 101:Saved Successfully; 102: Updated Successfully 103:Deleted Successfully; 104: Invalid Pincode; 105: Role 'Customer' not Exist</returns>
        public long CreateCustomer(CustomerDetailsViewModel lcust)
        {
            return SetCustomerDetails(lcust, "SAVE");

        }

        /// <summary>
        /// Edit Customer Profile
        /// </summary>
        /// <param name="lcust">Object of CustomerDetailsViewModel</param>
        /// <returns>0: No operation; 101:Saved Successfully; 102: Updated Successfully 103:Deleted Successfully; 104: Invalid Pincode; 105: Role 'Customer' not Exist</returns>
        public long EditCustomer(CustomerDetailsViewModel lcust)
        {
            long lOprStaus = 0;
            lOprStaus = SetCustomerDetails(lcust, "EDIT");
            return lOprStaus;

        }

        /// <summary>
        /// save/Update Customer Details 
        /// </summary>
        /// <param name="lcust">object of CustomerDetailsViewModel</param>
        /// <param name="Mode">Operation Mode</param>
        /// <returns>0: No operation; 101:Saved Successfully; 102: Updated Successfully 103:Deleted Successfully; 104: Invalid Pincode; 105: Role 'Customer' not Exist; 106: Customer Already Registered</returns>
        private long SetCustomerDetails(CustomerDetailsViewModel lcust, string Mode)
        {
            long lOprStatus = 0;


            using (TransactionScope ts = new TransactionScope())
            {
                try
                {
                    /*For Nullable Email Address
                      * Pradnyakar Badge
                      *  18-04-2016
                      */
                    if (lcust.EmailId != null)
                    {
                        if (lcust.EmailId.Trim().ToString().Equals(""))
                        {
                            lcust.EmailId = null;
                        }
                    }

                      /*For Nullable Mobile Address
                       * Pradnyakar Badge
                       * 18-04-2016
                       */
                    if (lcust.MobileNo != null)
                    {
                        if (lcust.MobileNo.Trim().ToString().Equals(""))
                        {
                            lcust.MobileNo = null;
                        }
                    }

                    if (lcust.MobileNo == null && lcust.EmailId == null)
                        return 107;

                    System.DateTime lCurrentDtTime = DateTime.UtcNow.AddHours(5.5);

                    /* Entry in User Login Table*/
                    UserLogin lUser = new UserLogin();
                    lUser.Mobile = lcust.MobileNo;
                    lUser.Email = lcust.EmailId;
                    if (lcust.Password == null || lcust.Password.ToString().Equals(""))
                    {
                        lUser.Password = CreatePassword(10);
                    }
                    else
                    {
                        lUser.Password = lcust.Password;
                    }
                    lUser.IsLocked = false;
                    lUser.CreateDate = lCurrentDtTime;
                    lUser.CreateBy = 1;

                    if (Mode.Equals("SAVE"))
                    {
                        // Check: Is user already exists
                        
                      //  var alreadyExistsUser = db.UserLogins.Where(x => x.Mobile == lcust.MobileNo || x.Email == lcust.EmailId).FirstOrDefault();
                        
                        /* For Nullable Mobile No or Email Address
                         * Pradnyakar Badge
                         * 18-04-2016
                         */
                        UserLogin alreadyExistsUser = new UserLogin();
                        if (lcust.MobileNo != null)
                        {
                            if (db.UserLogins.Where(x => x.Mobile == lcust.MobileNo).Count() > 0)
                            {
                                return 108;
                            }
                        }
                        if (lcust.EmailId != null)
                        {
                            if (db.UserLogins.Where(x => x.Email == lcust.EmailId).Count() > 0)
                            {
                                return 109;
                            }
                        }
                        /********************************************************************************************/

                        //if (alreadyExistsUser != null)
                        //    return 106;

                        db.UserLogins.Add(lUser);
                        db.SaveChanges();
                        //While insert, oprStatus contains UserLoginID
                        //if Userlogin ID is >0 means user Registered Successfully.

                    }
                    else
                    {
                        //Check UserLogin ID is provided or not 
                        if (lcust.UserLoginID == null)
                            return 107;

                        //UserLogin ID cannot be 0(zero)
                        if (lcust.UserLoginID <= 0)
                            return 100;

                        // Check: Is another user with provided email and mobile already exists.
                        var alreadyExistsUser = db.UserLogins.Where(x => x.ID != lcust.UserLoginID && (x.Mobile == lcust.MobileNo || x.Email == lcust.EmailId)).FirstOrDefault();
                        if (alreadyExistsUser != null)
                            return 106;

                        //Update                       
                        UserLogin userLoginDB = db.UserLogins.Where(x => x.ID == lcust.UserLoginID).FirstOrDefault();

                        if (userLoginDB == null)
                            return 108;
                        lUser.ID = lcust.UserLoginID;
                        lUser.CreateBy = userLoginDB.CreateBy;
                        lUser.CreateDate = userLoginDB.CreateDate;
                        lUser.ModifyDate = lCurrentDtTime;
                        lUser.Password = userLoginDB.Password;
                        lUser.ModifyBy = CommonFunctions.GetPersonalDetailsID(lcust.UserLoginID);


                        db.Entry(userLoginDB).CurrentValues.SetValues(lUser);
                        db.SaveChanges();

                    }
                   
                    /*Personal Detail table*/
                    PersonalDetail lPDetail = new PersonalDetail();
                    lPDetail.UserLoginID = lUser.ID;
                    lPDetail.SalutationID = lcust.SalutationID;
                    lPDetail.FirstName = lcust.FirstName;
                    lPDetail.LastName = lcust.LastName;

                    lPDetail.IsActive = true;
                    lPDetail.CreateDate = lCurrentDtTime;


                    if (Mode.Equals("SAVE"))
                    {
                        try
                        {

                            lPDetail.CreateDate = lCurrentDtTime;
                            db.PersonalDetails.Add(lPDetail);
                            db.SaveChanges();
                        }
                        catch (DbEntityValidationException ex)
                        {
                            // Retrieve the error messages as a list of strings.
                            var errorMessages = ex.EntityValidationErrors
                                    .SelectMany(x => x.ValidationErrors)
                                    .Select(x => new { x.ErrorMessage, x.PropertyName });

                            // Join the list to a single string.
                            var fullErrorMessage = string.Join("; ", errorMessages);

                            // Combine the original exception message with the new one.
                            var exceptionMessage = string.Concat(ex.Message, " The validation errors are: ", fullErrorMessage);

                            // Throw a new DbEntityValidationException with the improved exception message.
                            throw new DbEntityValidationException(exceptionMessage, ex.EntityValidationErrors);
                        }

                    }
                    else
                    {
                        lPDetail.MiddleName = lcust.MiddleName;
                        lPDetail.DOB = lcust.DOB;
                        lPDetail.Gender = lcust.Gender;
                        lPDetail.CreateBy = lUser.ID;

                        lPDetail.Address = lcust.Address;
                        lPDetail.AlternateEmail = lcust.EmailId;
                        lPDetail.AlternateMobile = lcust.MobileNo;
                        // Get pincode details from database
                        var lPincodeDB = from element in db.Pincodes
                                         where element.Name == lcust.Pincode || element.ID == lcust.PincodeID
                                         select element.ID;

                        // Check user entered pincode is valid or not
                        if (lPincodeDB.Count() == 0)
                        {
                            ts.Dispose();
                            return lOprStatus = 104;
                            //invalid Pincode
                        }
                        else
                        {

                            // Get pincode id           
                            foreach (var element in lPincodeDB)
                            {
                                lPDetail.PincodeID = element;
                            }
                        }

                        PersonalDetail lPDetailDB = db.PersonalDetails.Single(x => x.UserLoginID == lcust.UserLoginID);
                        lPDetail.ID = lPDetailDB.ID;
                        lPDetail.SalutationID = lcust.SalutationID;
                        lPDetail.LastName = lcust.LastName;
                        lPDetail.FirstName = lcust.FirstName;
                        lPDetail.CreateDate = lPDetailDB.CreateDate;
                        lPDetail.CreateBy = lPDetailDB.CreateBy;
                        lPDetail.ModifyDate = lCurrentDtTime;
                        lPDetail.ModifyBy = CommonFunctions.GetPersonalDetailsID(lcust.UserLoginID);

                        db.Entry(lPDetailDB).CurrentValues.SetValues(lPDetail);
                        db.SaveChanges();

                    }

                    /*User Role */
                    if (Mode.Equals("SAVE"))
                    {
                        Role role = new Role();
                        var lRole = from element in db.Roles
                                    where element.Name.ToUpper() == "CUSTOMER"
                                    select element.ID;

                        // Check Role :Customer exists or not
                        if (lRole.Count() == 0)
                        {
                            ts.Dispose();
                            return lOprStatus = 105;
                            //Role Not Exists
                        }
                        else
                        {
                            foreach (var element in lRole)
                            {
                                role.ID = element;

                            }

                        }
                        UserRole uRole = new UserRole();
                        uRole.UserLoginID = lUser.ID;
                        uRole.RoleID = role.ID;
                        uRole.IsActive = true;
                        uRole.CreateBy = 1;
                        uRole.CreateDate = lCurrentDtTime;
                        BusinessLogicLayer.ErrorLog.ErrorLogFile(" Not Saved " + role.ID, ErrorLog.Module.API, System.Web.HttpContext.Current.Server);

                        db.UserRoles.Add(uRole);
                        db.SaveChanges();

                        //====================================Update referenceID from referedetail table=============Manoj==//


                        if (db.ReferDetails.Any(m => m.Email == lcust.EmailId || m.Mobile == lcust.MobileNo))
                        {
                            ReferDetail lReferDetail = db.ReferDetails.Where(m => (m.Email == lcust.EmailId || m.Mobile == lcust.MobileNo) && m.ReferenceID == null).FirstOrDefault();
                            if (lReferDetail != null)
                            {
                                lReferDetail.ReferenceID = lUser.ID;
                                lReferDetail.ModifyDate = DateTime.Now;
                                //lReferDetail.ModifyBy = CommonFunctions.GetPersonalDetailsID(lUser.ID);
                                //lReferDetail.NetworkIP = BusinessLogicLayer.CommonFunctions.GetClientIP();
                                db.SaveChanges();
                                var earn = (from RES in db.ReferAndEarnSchemas
                                            join RD in db.ReferDetails on RES.ID equals RD.ReferAndEarnSchemaID
                                            where RES.UserwiseEarn == true && RD.ID == lReferDetail.ID
                                            select new { EarnMoney = RES.EarnInRS }).FirstOrDefault();

                                if (earn != null)
                                {
                                    decimal EarnRs = Convert.ToDecimal(earn.EarnMoney);

                                    var PrevRemainingAmt = db.EarnDetails.OrderByDescending(u => u.ID).Where(x => x.EarnUID == lReferDetail.UserID).Select(x => x.RemainingAmount).FirstOrDefault();

                                    EarnDetail lEarnDetail = new EarnDetail();
                                    lEarnDetail.EarnUID = lReferDetail.UserID;
                                    lEarnDetail.ReferUID = lReferDetail.ReferenceID;
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
                                    //lEarnDetail.CreateBy = CommonFunctions.GetPersonalDetailsID(lUser.ID);
                                    //lEarnDetail.NetworkIP = BusinessLogicLayer.CommonFunctions.GetClientIP();
                                    lEarnDetail.IsActive = true;
                                    lEarnDetail.ModifyDate = DateTime.Now; ;
                                    db.EarnDetails.Add(lEarnDetail);
                                    db.SaveChanges();
                                }
                            }

                        }
                        //==================================================================================================//
                    }

                    if (Mode.Equals("SAVE"))
                        lOprStatus = lUser.ID;//101
                    else
                        lOprStatus = 102;

                    lcust.UserLoginID = lUser.ID;// Changes By Pradnyakar on 16/04/2016
                    ts.Complete();
                }
                catch (Exception exception)
                {
                    lOprStatus = 500;

                    // Rollback transaction
                    ts.Dispose();
                    BusinessLogicLayer.ErrorLog.ErrorLogFile(" Not Saved " + exception.InnerException + exception.Message, ErrorLog.Module.API, System.Web.HttpContext.Current.Server);
                }

            }
            return lOprStatus;

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

    }
}
