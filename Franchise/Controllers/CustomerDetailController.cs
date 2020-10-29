using Franchise.Models;
using Franchise.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using PagedList.Mvc;
using Franchise.SalesOrder;
using ModelLayer.Models;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Data.Entity;
using BusinessLogicLayer;
using System.Data.SqlClient;
using System.Data;
using System.Web.Configuration;
using ModelLayer.Models.ViewModel;

namespace Franchise.Controllers
{
    [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
    public class CustomerDetailController : Controller
    {
        private EzeeloDBContext db = new EzeeloDBContext();
        private CustomerCareSessionViewModel customerCareSessionViewModel = new CustomerCareSessionViewModel();
        private int pageSize = 20;

        public void SessionDetails()
        {
            try
            {
                customerCareSessionViewModel.UserLoginID = Convert.ToInt64(Session["ID"]);
                customerCareSessionViewModel.Username = Session["USER_NAME"].ToString();
                customerCareSessionViewModel.PersonalDetailID = Convert.ToInt64(Session["PERSONAL_ID"]);
                //Common.Common.GetAllLoginDetailFromSession(ref customerCareSessionViewModel);
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                throw new BusinessLogicLayer.MyException("[SessionDetails]", "Can't assign Session Details..!" + Environment.NewLine + myEx.Message);
            }
        }

        //
        // GET: /CustomerDetail/
        [SessionExpire]
        [CustomAuthorize(Roles = "CustomerDetail/CanRead")]
        public ActionResult Index(string LastPurchaseDateMin, string LastPurchaseDateMax,string FromDate,string ToDate, int? page, string Customer, int? TotalOrderMin, int? TotalOrderMax, decimal? TotalAmountMin, decimal? TotalAmountMax, string Mobile, string Email, int? exportType, string submit)
        {
            try
            {
                SessionDetails();
                int franchiseID = Convert.ToInt32(Session["FRANCHISE_ID"]);
                int pageNumber = (page ?? 1);
                ViewBag.PageNumber = pageNumber;
                ViewBag.PageSize = pageSize;
                ViewBag.Mobile = Mobile;
                ViewBag.Email = Email;
                ViewBag.Customer = Customer;
                ViewBag.TotalOrderMin = (TotalOrderMin == null) ? 0 : (int)TotalOrderMin;
                ViewBag.TotalOrderMax = (TotalOrderMax == null) ? 100 : (int)TotalOrderMax;
                ViewBag.TotalAmountMin = (TotalAmountMin == null) ? 0 : (decimal)TotalAmountMin;
                ViewBag.TotalAmountMax = (TotalAmountMax == null) ? 10000000 : (decimal)TotalAmountMax;
                
                CustomerDetailReport custDetail = new CustomerDetailReport();
                List<CustomerViewModel> lCustList = custDetail.GetCustomerDetail(null, franchiseID);
                
                if (Customer != null && Customer != "")
                {
                    lCustList = lCustList.Where(x => x.FirstName != null && x.FirstName.Trim().ToLower().Contains(Customer.Trim().ToLower())).ToList();
                }
                if ((TotalOrderMin != null && TotalOrderMax != null))
                {
                    lCustList = lCustList.Where(x => x.TotalOrder >= TotalOrderMin && x.TotalOrder <= TotalOrderMax).ToList();
                }
                if ((TotalAmountMin != null && TotalAmountMax != null))
                {
                    lCustList = lCustList.Where(x => x.TotalAmount >= TotalAmountMin && x.TotalAmount <= TotalAmountMax).ToList();
                }
                if ((LastPurchaseDateMin != null && LastPurchaseDateMin != "") && (LastPurchaseDateMax != null && LastPurchaseDateMax != ""))
                {
                    DateTime lLastPurchDateMin = BusinessLogicLayer.CommonFunctions.GetProperDateTime(LastPurchaseDateMin);
                    ViewBag.LastPurchaseDateMin = lLastPurchDateMin.ToString("dd/MM/yyyy");

                    DateTime lLastPurchDateMax = BusinessLogicLayer.CommonFunctions.GetProperDateTime(LastPurchaseDateMax);
                    ViewBag.LastPurchaseDateMax = lLastPurchDateMax.ToString("dd/MM/yyyy");

                    lCustList = lCustList.Where(x => x.LastPurchasedDate != null && ((DateTime)x.LastPurchasedDate).Date >= lLastPurchDateMin.Date && ((DateTime)x.LastPurchasedDate).Date <= lLastPurchDateMax.Date).ToList();
                }
                if (Mobile != null && Mobile != "")
                {
                    lCustList = lCustList.Where(x => x.RegMobile != null && x.RegMobile.Trim().StartsWith(Mobile.Trim())).ToList();
                }
                if (Email != null && Email != "")
                {
                    lCustList = lCustList.Where(x => x.RegEmail != null && x.RegEmail.Trim().Contains(Email.Trim())).ToList();
                }
                if ((FromDate != null && FromDate != "") || (ToDate != null && ToDate != ""))
                {
                    DateTime lFromDate = BusinessLogicLayer.CommonFunctions.GetProperDateTime(FromDate);
                    DateTime lToDate = BusinessLogicLayer.CommonFunctions.GetProperDateTime(ToDate);
                    ViewBag.FromDate = lFromDate.ToString("dd/MM/yyyy");
                    ViewBag.ToDate = lToDate.ToString("dd/MM/yyyy");

                    lCustList = lCustList.Where(x => x.CreateDate.Date >= lFromDate.Date && x.CreateDate.Date <= lToDate.Date).ToList();
                }
                if (exportType != null && submit.Trim().ToLower().Equals("export"))
                {
                    BusinessLogicLayer.ExportExcelCsv ExportExcelCsv = new BusinessLogicLayer.ExportExcelCsv(System.Web.HttpContext.Current.Server);
                    if (exportType == 1)
                    {
                        ExportExcelCsv.ExportToExcel(Common.Helper.ToDataTable(lCustList), "Customer Report");
                    }
                    else if (exportType == 2)
                    {
                        ExportExcelCsv.ExportToCSV(Common.Helper.ToDataTable(lCustList), "Customer Report");
                    }
                    else if (exportType == 3)
                    {
                        ExportExcelCsv.ExportToPDF(Common.Helper.ToDataTable(lCustList), "Customer Report");
                    }
                }

                //Added by Zubair on 13-09-2017 for showing Login Report
                if (exportType != null && submit.Trim().ToLower().Equals("login report"))
                {
                    List<TempPasswordViewModel> lTempPasswordViewModel = this.GetCustomerLoginDetail(null, franchiseID);

                    if (Customer != null && Customer != "")
                    {
                        lTempPasswordViewModel = lTempPasswordViewModel.Where(x => x.Customer != null && x.Customer.Trim().ToLower().Contains(Customer.Trim().ToLower())).ToList();
                    }

                    if ((LastPurchaseDateMin != null && LastPurchaseDateMin != "") && (LastPurchaseDateMax != null && LastPurchaseDateMax != ""))
                    {
                        DateTime lLastPurchDateMin = BusinessLogicLayer.CommonFunctions.GetProperDateTime(LastPurchaseDateMin);
                        DateTime lLastPurchDateMax = BusinessLogicLayer.CommonFunctions.GetProperDateTime(LastPurchaseDateMax);
                        lTempPasswordViewModel = lTempPasswordViewModel.Where(x => x.LoginTime != null && ((DateTime)x.LoginTime).Date >= lLastPurchDateMin.Date && ((DateTime)x.LoginTime).Date <= lLastPurchDateMax.Date).ToList();
                    }
                    if (Mobile != null && Mobile != "")
                    {
                        lTempPasswordViewModel = lTempPasswordViewModel.Where(x => x.Mobile != null && x.Mobile.Trim().StartsWith(Mobile.Trim())).ToList();
                    }
                    if (Email != null && Email != "")
                    {
                        lTempPasswordViewModel = lTempPasswordViewModel.Where(x => x.Email != null && x.Email.Trim().Contains(Email.Trim())).ToList();
                    }

                    BusinessLogicLayer.ExportExcelCsv ExportExcelCsv = new BusinessLogicLayer.ExportExcelCsv(System.Web.HttpContext.Current.Server);
                    if (exportType == 1)
                    {
                        ExportExcelCsv.ExportToExcel(Common.Helper.ToDataTable(lTempPasswordViewModel), "LoginReport");
                    }
                    else if (exportType == 2)
                    {
                        ExportExcelCsv.ExportToCSV(Common.Helper.ToDataTable(lTempPasswordViewModel), "LoginReport");
                    }
                    else if (exportType == 3)
                    {
                        ExportExcelCsv.ExportToPDF(Common.Helper.ToDataTable(lTempPasswordViewModel), "LoginReport");
                    }
                }
                //End By Zubair

                ViewBag.NoOfCustomer = lCustList.Count();

                return View(lCustList.OrderByDescending(x=>x.LastPurchasedDate).ToPagedList(pageNumber, pageSize));
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[CustomerDetail][GET:Index]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Franchise, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[CustomerDetail][GET:Index]",
                    BusinessLogicLayer.ErrorLog.Module.Franchise, System.Web.HttpContext.Current.Server);
            }
            return View();
        }

        // GET: /CustomerDetail/Details/5
        [SessionExpire]
        [CustomAuthorize(Roles = "CustomerDetail/CanRead")]
        public ActionResult Details(int id)
        {
            try
            {
                SessionDetails();
                int franchiseID = Convert.ToInt32(Session["FRANCHISE_ID"]);
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                CustomerDetailReport custDetail = new CustomerDetailReport();
                List<CustomerViewModel> lCust = custDetail.GetCustomerDetail(id, franchiseID);
                if (lCust == null || lCust.Count == 0)
                {
                    return HttpNotFound();
                }
                return View(lCust.FirstOrDefault());
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[CustomerDetail][GET:Details]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Franchise, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[CustomerDetail][GET:Details]",
                    BusinessLogicLayer.ErrorLog.Module.Franchise, System.Web.HttpContext.Current.Server);
            }
            return View();
        }


        //Added by Zubair on 11-09-2017 for temp password
        [SessionExpire]
        [CustomAuthorize(Roles = "CustomerDetail/CanRead")]
        public string GetTempPassword(string Username)
        {
            try
            {
                Username = Username.TrimStart().TrimEnd();

                var UserLoginID = db.UserLogins.Where(x => x.Mobile == Username || x.Email == Username).Select(x => x.ID).FirstOrDefault();
                if (UserLoginID == null || UserLoginID == 0)
                {
                    throw new Exception("Error: Invalid Username!");
                }

                long LoginID = Convert.ToInt64(UserLoginID);

                UserLogin lUserLogin = null;
                try
                {
                    lUserLogin = db.UserLogins.Find(UserLoginID);
                    if (lUserLogin == null)
                    {
                        throw new Exception("Error: Invalid Username!");
                    }
                }
                catch (Exception)
                {
                    throw new Exception("Error: Getting Username.");
                }

                var tempPassword = db.TempPasswords.Where(x => x.UserLoginID == LoginID && x.IsActive == true).Select(x => x.TempPassword1).FirstOrDefault();

                TempPassword objTemp = new TempPassword();

                if (tempPassword != null)
                {
                    long ID = db.TempPasswords.Where(x => x.TempPassword1 == tempPassword && x.UserLoginID == LoginID && x.IsActive == true).Select(x => x.ID).FirstOrDefault();
                    //Update TempPassword
                    var lTempPassword = new TempPassword()
                    {
                        ID = ID,
                        ModifyBy = Convert.ToInt64(Session["ID"]),
                        FranchiseID = Convert.ToInt64(Session["FRANCHISE_ID"]),
                        ModifyDate = DateTime.Now
                    };

                    if (ModelState.IsValid)
                    {
                        db.TempPasswords.Attach(lTempPassword);
                        db.Entry(lTempPassword).Property(x => x.FranchiseID).IsModified = true;
                        db.Entry(lTempPassword).Property(x => x.ModifyBy).IsModified = true;
                        db.Entry(lTempPassword).Property(x => x.ModifyDate).IsModified = true;
                        db.SaveChanges();

                        if (!string.IsNullOrEmpty(lUserLogin.Email))
                        {
                            return "Success: " + lUserLogin.Email + ":" + tempPassword;
                        }
                        else if (!string.IsNullOrEmpty(lUserLogin.Mobile))
                        {
                            return "Success: " + lUserLogin.Mobile + ":" + tempPassword;
                        }
                        else
                        {
                            throw new Exception("Error: Getting Username.");
                        }
                    }
                }
                else
                {
                    objTemp.UserLoginID = LoginID;
                    objTemp.TempPassword1 = CalculateMD5Hash(GenerateRandomString());
                    objTemp.FranchiseID = Convert.ToInt64(Session["FRANCHISE_ID"]);
                    objTemp.IsActive = true;
                    objTemp.CreateBy = Convert.ToInt64(Session["ID"]);
                    objTemp.CreateDate = DateTime.Now;
                    objTemp.ModifyBy = Convert.ToInt64(Session["ID"]);
                    objTemp.ModifyDate = DateTime.Now;
                    objTemp.NetworkIP = CommonFunctions.GetClientIP();
                    objTemp.DeviceID = "X";
                    objTemp.DeviceType = "X";
                    if (ModelState.IsValid)
                    {
                        db.TempPasswords.Add(objTemp);
                        db.SaveChanges();
                        if (!string.IsNullOrEmpty(lUserLogin.Email))
                        {
                            return "Success: " + lUserLogin.Email + ":" + objTemp.TempPassword1;
                        }
                        else if (!string.IsNullOrEmpty(lUserLogin.Mobile))
                        {
                            return "Success: " + lUserLogin.Mobile + ":" + objTemp.TempPassword1;
                        }
                        else
                        {
                            throw new Exception("Error: Getting Username.");
                        }
                    }
                }


            }
            catch (Exception ex)
            {
                return ex.Message.ToString();
            }
            return Username;
        }       

        private string CalculateMD5Hash(string input)
        {
            // step 1, calculate MD5 hash from input
            MD5 md5 = System.Security.Cryptography.MD5.Create();
            byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
            byte[] hash = md5.ComputeHash(inputBytes);
            // step 2, convert byte array to hex string
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("X2"));
            }
            return sb.ToString();
        }

        private string GenerateRandomString()
        {
            Random rnd = new Random();
            int month = rnd.Next(1, 13); // creates a number between 1 and 12
            int dice = rnd.Next(1, 7);   // creates a number between 1 and 6
            int card = rnd.Next(52);
            return month.ToString("0") + dice.ToString("0") + card.ToString("0");
        }

        // GET: /CustomerDetail/
        [SessionExpire]
        [CustomAuthorize(Roles = "CustomerDetail/CanRead")]
        public ActionResult TempPasswordLoginDetail(string LastPurchaseDateMin, string LastPurchaseDateMax, string Customer, string Mobile, string Email, int? exportType)
        {
            try
            {
                SessionDetails();
                int franchiseID = Convert.ToInt32(Session["FRANCHISE_ID"]);

                ViewBag.Mobile = Mobile;
                ViewBag.Email = Email;
                ViewBag.Customer = Customer;

                CustomerDetailReport custDetail = new CustomerDetailReport();
                List<TempPasswordViewModel> lCustList = this.GetCustomerLoginDetail(null, franchiseID);

                if (Customer != null && Customer != "")
                {
                    lCustList = lCustList.Where(x => x.Customer != null && x.Customer.Trim().ToLower().Contains(Customer.Trim().ToLower())).ToList();
                }

                if ((LastPurchaseDateMin != null && LastPurchaseDateMin != "") && (LastPurchaseDateMax != null && LastPurchaseDateMax != ""))
                {
                    DateTime lLastPurchDateMin = BusinessLogicLayer.CommonFunctions.GetProperDateTime(LastPurchaseDateMin);
                    ViewBag.LastPurchaseDateMin = lLastPurchDateMin.ToString("dd/MM/yyyy");

                    DateTime lLastPurchDateMax = BusinessLogicLayer.CommonFunctions.GetProperDateTime(LastPurchaseDateMax);
                    ViewBag.LastPurchaseDateMax = lLastPurchDateMax.ToString("dd/MM/yyyy");

                    lCustList = lCustList.Where(x => x.LoginTime != null && ((DateTime)x.LoginTime).Date >= lLastPurchDateMin.Date && ((DateTime)x.LoginTime).Date <= lLastPurchDateMax.Date).ToList();
                }
                if (Mobile != null && Mobile != "")
                {
                    lCustList = lCustList.Where(x => x.Mobile != null && x.Mobile.Trim().StartsWith(Mobile.Trim())).ToList();
                }
                if (Email != null && Email != "")
                {
                    lCustList = lCustList.Where(x => x.Email != null && x.Email.Trim().Contains(Email.Trim())).ToList();
                }

                if (exportType != null)
                {
                    BusinessLogicLayer.ExportExcelCsv ExportExcelCsv = new BusinessLogicLayer.ExportExcelCsv(System.Web.HttpContext.Current.Server);
                    if (exportType == 1)
                    {
                        ExportExcelCsv.ExportToExcel(Common.Helper.ToDataTable(lCustList), "Customer Report");
                    }
                    else if (exportType == 2)
                    {
                        ExportExcelCsv.ExportToCSV(Common.Helper.ToDataTable(lCustList), "Customer Report");
                    }
                    else if (exportType == 3)
                    {
                        ExportExcelCsv.ExportToPDF(Common.Helper.ToDataTable(lCustList), "Customer Report");
                    }
                }
                ViewBag.NoOfCustomer = lCustList.Count();

                return View();
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[CustomerDetail][GET:Index]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Franchise, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[CustomerDetail][GET:Index]",
                    BusinessLogicLayer.ErrorLog.Module.Franchise, System.Web.HttpContext.Current.Server);
            }
            return View();
        }


        public List<TempPasswordViewModel> GetCustomerLoginDetail(long? UserLoginID, int FranchiseID)
        {
            List<TempPasswordViewModel> lCust = new List<TempPasswordViewModel>();
            try
            {
                string fConnectionString = WebConfigurationManager.ConnectionStrings["EzeeloDBContext"].ToString();
                DataTable lDataTableCustomerOrder = new DataTable();
                SqlConnection con = new SqlConnection(fConnectionString);
                SqlCommand sqlComm = new SqlCommand("ReportCustomerTempPasswordLoginList", con);
                sqlComm.CommandType = CommandType.StoredProcedure;
                sqlComm.Parameters.AddWithValue("@pUserLoginID", SqlDbType.BigInt).Value = UserLoginID;
                sqlComm.Parameters.AddWithValue("@FranchiseID", SqlDbType.Int).Value = FranchiseID;
                con.Open();

                SqlDataAdapter da = new SqlDataAdapter(sqlComm);
                DataTable dt = new DataTable();
                da.Fill(dt);

                lCust = Common.Helper.CreateListFromTable<TempPasswordViewModel>(dt);
            }
            catch (Exception)
            {
                throw;
            }
            return lCust;
        }

        // End By Zubair

    }
}