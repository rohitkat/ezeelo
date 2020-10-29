using ModelLayer.Models;
using ModelLayer.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer
{
    public class ReferAndEarn
    {
        private EzeeloDBContext db = new EzeeloDBContext();
        public void GetReferAndEarnDetails(long userLoginId, decimal OrderAmnt, long customerOrderID)
        {
            try
            {
                //long chaiwLI=this.GetTransactionCode();
                ReadConfig readCon = new ReadConfig(System.Web.HttpContext.Current.Server);
                SqlConnection con = new SqlConnection(readCon.DB_CONNECTION);
                con.Open();
                SqlCommand sqlComm = new SqlCommand("GetReferByUserDetails", con);
                sqlComm.CommandType = CommandType.StoredProcedure;
                sqlComm.Parameters.AddWithValue("@userLoginId", userLoginId);
                sqlComm.Parameters.AddWithValue("@OrderAmnt", OrderAmnt);
                sqlComm.Parameters.AddWithValue("@customerOrderID", customerOrderID);
                sqlComm.Parameters.AddWithValue("@transactionID", this.GetTransactionCode());
                sqlComm.ExecuteNonQuery();
                con.Close();

            }
            catch (Exception ex)
            {

               // throw;
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[ReferAndEarn][M:GetReferAndEarnDetails]",
                    BusinessLogicLayer.ErrorLog.Module.Gandhibagh, System.Web.HttpContext.Current.Server);
            }

        }

        private long GetTransactionCode()
        {
            long tranCode = 0;
            //Guid randomId = Guid.NewGuid();
            //tranCode = randomId.ToString().Substring(0, 15).ToUpper();

            Random random = new Random();
            //tranCode = "REF"+ random.Next(100000, 999999).ToString();
            tranCode = random.Next(100000, 999999);
            return tranCode;
        }

        public ReferDetailViewModel InsertReferDetail(ReferDetailViewModel referdetailviewmodel, long userLoginID)
        {
            //Dictionary<string, string> dic = new Dictionary<string, string>();
            try
            {
                using (EzeeloDBContext db = new EzeeloDBContext())
                {
                    foreach (var i in referdetailviewmodel.lSubReferDetail)
                    {
                        if (!db.UserLogins.Any(m => m.Email == i.Email || m.Mobile == i.Mobile))
                        {
                            if (!db.ReferDetails.Any(m => m.Email == i.Email || m.Mobile == i.Mobile))
                            {
                                ReferDetail lReferDetail = new ReferDetail();
                                if (!CommonFunctions.IsValidEmailId(i.Email) && !CommonFunctions.IsValidMobile(i.Mobile))
                                {
                                    i.Message = "Please check Email and Mobile";
                                }
                                else if (!CommonFunctions.IsValidEmailId(i.Email))
                                {
                                    i.Message = "Please check Email";

                                }
                                else if (!CommonFunctions.IsValidMobile(i.Mobile))
                                {
                                    i.Message = "Please check Mobile";
                                }
                                else
                                {
                                    lReferDetail.ReferAndEarnSchemaID = referdetailviewmodel.ReferAndEarnSchemaID;
                                    lReferDetail.UserID = Convert.ToInt64(userLoginID);
                                    lReferDetail.Email = i.Email;
                                    lReferDetail.Mobile = i.Mobile;
                                    lReferDetail.ReferenceID = null;
                                    lReferDetail.CreateDate = DateTime.UtcNow.AddHours(5.5); 
                                    lReferDetail.CreateBy = CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(userLoginID));
                                    lReferDetail.NetworkIP = BusinessLogicLayer.CommonFunctions.GetClientIP();
                                    lReferDetail.DeviceType = "x";
                                    lReferDetail.DeviceID = "x";
                                    db.ReferDetails.Add(lReferDetail);
                                    db.SaveChanges();
                                    string name = db.PersonalDetails.Where(x => x.UserLoginID == userLoginID).Select(x => x.FirstName).FirstOrDefault();
                                    this.SendMailToCustomer(name, i.Email, i.Email);
                                    this.SendSMSToCustomer(name, i.Email, i.Mobile);
                                    i.Message = "Refer Successfully";
                                }
                                //ReferDetail lReferDetail = new ReferDetail();
                                //lReferDetail.ReferAndEarnSchemaID = referdetailviewmodel.ReferAndEarnSchemaID;
                                //lReferDetail.UserID = Convert.ToInt64(userLoginID);
                                //lReferDetail.Email = i.Email;
                                //lReferDetail.Mobile = i.Mobile;
                                //lReferDetail.ReferenceID = null;
                                //lReferDetail.CreateDate = DateTime.UtcNow.AddHours(5.5); ;
                                //lReferDetail.CreateBy = CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(userLoginID));
                                //lReferDetail.NetworkIP = BusinessLogicLayer.CommonFunctions.GetClientIP();
                                //lReferDetail.DeviceType = "x";
                                //lReferDetail.DeviceID = "x";
                                //db.ReferDetails.Add(lReferDetail);
                                //db.SaveChanges();
                                //string name = db.PersonalDetails.Where(x => x.UserLoginID == userLoginID).Select(x => x.FirstName).FirstOrDefault();
                                //this.SendMailToCustomer(name, i.Email, i.Email);
                                //this.SendSMSToCustomer(name, i.Email, i.Mobile);
                                //i.Message = "Refer Successfully";
                            }
                            else
                            {
                                i.Message = "User already Referred";
                            }
                        }
                        else
                        {
                            i.Message = "User already Exist";
                        }
                    }
                }
                return referdetailviewmodel;
            }
            catch (Exception)
            {

                throw;
            }
        }

        /// <summary>
        /// Created by Tejaswee for API
        /// Insert Refer customer details
        /// </summary>
        /// <param name="lReferCustomerViewModel"></param>
        /// <returns></returns>
        public ReferCustomerViewModel InsertReferDetail(ReferCustomerViewModel lReferCustomerViewModel)
        {
            //Dictionary<string, string> dic = new Dictionary<string, string>();
            try
            {
                using (EzeeloDBContext db = new EzeeloDBContext())
                {
                    foreach (var i in lReferCustomerViewModel.lSubReferDetail)
                    {
                        if (!db.UserLogins.Any(m => m.Email == i.Email || m.Mobile == i.Mobile))
                        {
                            if (!db.ReferDetails.Any(m => m.Email == i.Email || m.Mobile == i.Mobile))
                            {
                                ReferDetail lReferDetail = new ReferDetail();
                                lReferDetail.ReferAndEarnSchemaID = lReferCustomerViewModel.ReferAndEarnSchemaID;
                                lReferDetail.UserID = lReferCustomerViewModel.UID;
                                lReferDetail.Email = i.Email;
                                lReferDetail.Mobile = i.Mobile;
                                lReferDetail.ReferenceID = null;
                                lReferDetail.CreateDate = DateTime.UtcNow.AddHours(5.5); 
                                lReferDetail.CreateBy = CommonFunctions.GetPersonalDetailsID(lReferCustomerViewModel.UID);
                                lReferDetail.NetworkIP = BusinessLogicLayer.CommonFunctions.GetClientIP();
                                lReferDetail.DeviceType = "x";
                                lReferDetail.DeviceID = "x";
                                db.ReferDetails.Add(lReferDetail);
                                db.SaveChanges();
                                //string name = db.PersonalDetails.Where(x => x.UserLoginID == lReferCustomerViewModel.UID).Select(x => x.FirstName).FirstOrDefault();
                                //this.SendMailToCustomer(name, i.Email, i.Email);
                                //this.SendSMSToCustomer(name, i.Email, i.Mobile);
                                i.Message = "Refer Successfully";

                            }
                            else
                            {
                                i.Message = "User already Referred";
                            }
                        }
                        else
                        {
                            i.Message = "User already Exist";
                        }
                    }
                }
                return lReferCustomerViewModel;
            }
            catch (Exception)
            {
                return null;
                //throw; By sonali
            }
        }


        //public decimal GetTotalEarnAmount(long userLoginID)
        //{
        //    decimal amt = 0;
        //    try
        //    {
        //        //var PrevRemainingAmt = db.EarnDetails.OrderByDescending(u => u.ID).Where(x => x.UserID == userLoginID).Select(x => x.RemainingAmount).FirstOrDefault();
        //        var amt1 = db.EarnDetails.OrderByDescending(u => u.ModifyDate).Where(x => x.EarnUID == userLoginID && x.IsActive==true).Select(x => x.RemainingAmount).FirstOrDefault();
        //        if (amt1 != null)
        //        {
        //            amt = (decimal)amt1;
        //        }
        //    }
        //    catch (Exception)
        //    {

        //        throw;
        //    }
        //    return amt;
        //}

        public decimal GetTotalEarnAmount(long userLoginID)
        {
            decimal amt = 0;
            try
            {
                //var PrevRemainingAmt = db.EarnDetails.OrderByDescending(u => u.ID).Where(x => x.UserID == userLoginID).Select(x => x.RemainingAmount).FirstOrDefault();
                var amt1 = db.EarnDetails.OrderByDescending(u => u.ModifyDate).Where(x => x.EarnUID == userLoginID && x.IsActive == true).Select(x => x.RemainingAmount).FirstOrDefault();
                if (amt1 != null)
                {
                    amt = (decimal)amt1;
                }
            }
            catch (Exception)
            {

                throw;
            }
            return amt;
        }


        public void SendMailToCustomer(string referByName, string referToName, string emailID)
        {
            try
            {
                EzeeloDBContext db = new EzeeloDBContext();
                Dictionary<string, string> dictEmailValues = new Dictionary<string, string>();
                string city = "";
                int franchisrID = 0; ////added
                if (System.Web.HttpContext.Current.Request.Cookies["CityCookie"] != null && System.Web.HttpContext.Current.Request.Cookies["CityCookie"].Value != "")
                {
                    city = System.Web.HttpContext.Current.Request.Cookies["CityCookie"].Value.Split('$')[1].ToLower();
                    franchisrID =Convert.ToInt32( System.Web.HttpContext.Current.Request.Cookies["CityCookie"].Value.Split('$')[2]); ////added
                }
                dictEmailValues.Add("#--SUBJECT--#", referByName + " has invited you to try eZeelo ");
                //dictEmailValues.Add("<!--REFER_TO_NAME-->", referToName);
                dictEmailValues.Add("<!--REFER_TO_NAME-->", "");

                dictEmailValues.Add("<!--URL-->", "" + (new URLsFromConfig()).GetURL("CUSTOMER") + "" + city + "/" + franchisrID + "/login");////added /" + franchisrID +
                BusinessLogicLayer.GateWay gateWay = new BusinessLogicLayer.Email(System.Web.HttpContext.Current.Server);

                gateWay.SendEmail(BusinessLogicLayer.GateWay.EmailGateWays.GANDHIBAGH, BusinessLogicLayer.GateWay.SenderMail.INFO, BusinessLogicLayer.GateWay.EMailTypes.CUST_REFER_TO, new string[] { emailID }, dictEmailValues, true);
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[PaymentProcessController][M:SendMailToCustomer]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Gandhibagh, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[PaymentProcessController][M:SendMailToCustomer]",
                    BusinessLogicLayer.ErrorLog.Module.Gandhibagh, System.Web.HttpContext.Current.Server);
            }
        }


        public void SendSMSToCustomer(string referByName, string referToName, string mobileNo)
        {
            try
            {
                Dictionary<string, string> dictSMSValues = new Dictionary<string, string>();
                //dictSMSValues.Add("#--NAME--#", referToName);

                dictSMSValues.Add("#--ReferByName--#", referByName);

                BusinessLogicLayer.GateWay gateWay = new BusinessLogicLayer.SMS(System.Web.HttpContext.Current.Server);
                ReadConfig readConfig = new ReadConfig(System.Web.HttpContext.Current.Server);
                gateWay.SendSMS(BusinessLogicLayer.GateWay.SMSGateWays.SUMIT, BusinessLogicLayer.GateWay.SMSOptions.MULTIPLE, BusinessLogicLayer.GateWay.SMSTypes.CUST_REFER_TO, new string[] { mobileNo }, dictSMSValues);

            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[PaymentProcessController][M:SendSMSToMerchant]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Gandhibagh, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[PaymentProcessController][M:SendSMSToMerchant]",
                    BusinessLogicLayer.ErrorLog.Module.Gandhibagh, System.Web.HttpContext.Current.Server);
            }
        }

        public List<ReferAndEarnSchemaName> GetReferSchemeName()
        {
            List<ReferAndEarnSchemaName> lReferAndEarnSchemaName = new List<ReferAndEarnSchemaName>();
            lReferAndEarnSchemaName = (
                from r in db.ReferAndEarnSchemas
                where r.IsActive == true
                select new ReferAndEarnSchemaName
                {
                    ID = r.ID,
                    Name = r.Name
                }).ToList();

            return lReferAndEarnSchemaName;
        }
        //public ReferAndEarnSchemaName GetReferSchemeName(long cityID, long? FranchiseID)
        //{
        //    ReferAndEarnSchemaName lReferAndEarnSchemaName = new ReferAndEarnSchemaName();
        //    lReferAndEarnSchemaName = (
        //        from r in db.ReferAndEarnSchemas
        //        where r.IsActive == true && r.CityID == cityID
        //        select new ReferAndEarnSchemaName
        //        {
        //            ID = r.ID,
        //            Name = r.Name
        //        }).FirstOrDefault();

        //    return lReferAndEarnSchemaName;
        //}
        //Sonali for api 05/09/2018
        public ReferAndEarnSchemaName GetReferSchemeName(long franchiseId)//Sonali_03-11-2018
        {
            ReferAndEarnSchemaName lReferAndEarnSchemaName = new ReferAndEarnSchemaName();
            lReferAndEarnSchemaName = (
                from r in db.ReferAndEarnSchemas
                where r.IsActive == true && r.FranchiseID == franchiseId//Sonali_03-11-2018 
                select new ReferAndEarnSchemaName
                {
                    ID = r.ID,
                    Name = r.Name
                }).FirstOrDefault();

            return lReferAndEarnSchemaName;
        }
        //public EarnAndReferReportViewModelDetails CustomerReferAndEarnReport(long uid, long cityId)
        //{
        //    EarnAndReferReportViewModelDetails lobj = new EarnAndReferReportViewModelDetails();
        //    List<EarnAndReferDetailReportViewModel> lEarnAndReferDetailReportViewModel = new List<EarnAndReferDetailReportViewModel>();
        //    var RemainingAmount = db.EarnDetails.OrderByDescending(u => u.ID).Where(x => x.EarnUID == uid).Select(x => x.RemainingAmount).FirstOrDefault();
        //    lobj.RemainingAmount = Convert.ToInt64(RemainingAmount);
        //    lobj.totalEarnAmount = db.EarnDetails.Where(x => x.EarnUID == uid).Sum(x => x.EarnAmount);
        //    lobj.totalUsedAmount = db.EarnDetails.Where(x => x.EarnUID == uid).Sum(x => x.UsedAmount);
        //    DataTable dt = new DataTable();
        //    ReadConfig config = new ReadConfig(System.Web.HttpContext.Current.Server);
        //    DataAccessLayer.DbOperations dbOpr = new DataAccessLayer.GetData(config.DB_CONNECTION);
        //    List<object> paramValues = new List<object>();
        //    paramValues.Add(uid);
        //    dt = dbOpr.GetRecords("Select_ReferAndEarnReport", paramValues);

        //    lobj.lEarnAndReferReportViewModelDet = (from n in dt.AsEnumerable()
        //                                            select new EarnAndReferDetailReportViewModel
        //                                            {

        //                                                UserName = n.Field<string>("UserName"),
        //                                                Email = n.Field<string>("Email"),
        //                                                Mobile = n.Field<string>("Mobile"),
        //                                                ReferenceID = n.Field<long?>("ReferenceID"),
        //                                                EarnAmount = n.Field<decimal?>("EarnAmount"),
        //                                                SchemeName = n.Field<string>("SchemeName"),

        //                                            }).ToList();
        //    =========== Tejaswee Change in new Project also ==================//

        //    long schemeID = db.ReferAndEarnSchemas.Where(x => x.IsActive == true && x.CityID == cityId).Select(x => x.ID).FirstOrDefault();

        //    lobj.SchemeRemainAmt = db.SchemeBudgetTransactions.Where(x => x.ReferAndEarnSchemaID == schemeID).Select(x => x.RemainingAmt).FirstOrDefault();
        //    DateTime date = db.SchemeBudgetTransactions.Where(x => x.ReferAndEarnSchemaID == schemeID).Select(x => x.ExpiryDate).FirstOrDefault();
        //    lobj.ExpiryDays = "0";
        //    if (date > DateTime.Now)
        //    {
        //        lobj.ExpiryDays = date.Subtract(DateTime.Now).Days.ToString();
        //    }

        //    return lobj;

        //}
        //Sonali for api 05/09/2018
        public EarnAndReferReportViewModelDetails CustomerReferAndEarnReport(long uid, long cityId)
        {
            EarnAndReferReportViewModelDetails lobj = new EarnAndReferReportViewModelDetails();
            //List<EarnAndReferDetailReportViewModel> lEarnAndReferDetailReportViewModel = new List<EarnAndReferDetailReportViewModel>();
            var RemainingAmount = db.EarnDetails.OrderByDescending(u => u.ID).Where(x => x.EarnUID == uid).Select(x => x.RemainingAmount).FirstOrDefault();
            lobj.RemainingAmount = Convert.ToInt64(RemainingAmount);
            lobj.totalEarnAmount = db.EarnDetails.Where(x => x.EarnUID == uid).Sum(x => x.EarnAmount);
            lobj.totalUsedAmount = db.EarnDetails.Where(x => x.EarnUID == uid).Sum(x => x.UsedAmount);
            DataTable dt = new DataTable();
            ReadConfig config = new ReadConfig(System.Web.HttpContext.Current.Server);
            DataAccessLayer.DbOperations dbOpr = new DataAccessLayer.GetData(config.DB_CONNECTION);
            List<object> paramValues = new List<object>();
            paramValues.Add(uid);
            dt = dbOpr.GetRecords("Select_ReferAndEarnReport", paramValues);

            lobj.lEarnAndReferReportViewModelDet = (from n in dt.AsEnumerable()
                                                    select new EarnAndReferDetailReportViewModel
                                                    {

                                                        UserName = n.Field<string>("UserName"),
                                                        Email = n.Field<string>("Email"),
                                                        Mobile = n.Field<string>("Mobile"),
                                                        ReferenceID = n.Field<long?>("ReferenceID"),
                                                        EarnAmount = n.Field<decimal?>("EarnAmount"),
                                                        SchemeName = n.Field<string>("SchemeName"),

                                                    }).ToList();
            //=========== Tejaswee Change in new Project also ==================//

            long schemeID = db.ReferAndEarnSchemas.Where(x => x.IsActive == true && x.CityID == cityId).Select(x => x.ID).FirstOrDefault();

            lobj.SchemeRemainAmt = db.SchemeBudgetTransactions.Where(x => x.ReferAndEarnSchemaID == schemeID).Select(x => x.RemainingAmt).FirstOrDefault();
            DateTime date = db.SchemeBudgetTransactions.Where(x => x.ReferAndEarnSchemaID == schemeID).Select(x => x.ExpiryDate).FirstOrDefault();
            lobj.ExpiryDays = "0";
            if (date > DateTime.Now)
            {
                lobj.ExpiryDays = date.Subtract(DateTime.Now).Days.ToString();
            }

            return lobj;

        }
   
    }
}
