using CRM.Models;
using CRM.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using PagedList.Mvc;
using CRM.SalesOrder;
using ModelLayer.Models;
using System.Net;
using ModelLayer.Models.ViewModel;
using BusinessLogicLayer;
using System.Data.Entity;

namespace CRM.Controllers
{
    [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
    public class CustomerDetailController : Controller
    {
        private EzeeloDBContext db = new EzeeloDBContext();
        private CustomerCareSessionViewModel customerCareSessionViewModel = new CustomerCareSessionViewModel();
        private int pageSize = 20;

        public void SessionDetails()
        {
            customerCareSessionViewModel.UserLoginID = Convert.ToInt64(Session["ID"]);
            customerCareSessionViewModel.Username = Session["UserName"].ToString();
            if (!Common.Common.GetAllLoginDetailFromSession(ref customerCareSessionViewModel))
            {
                if (Session["ID"] != null)
                {
                    Session["ID"] = null;
                }
                TempData["ServerMsg"] = "You are not CustomerCare Person";
                Response.Redirect(System.Web.Configuration.WebConfigurationManager.AppSettings["UrlForInvalidCustomerCare"]);
            }
        }

        //
        // GET: /CustomerDetail/
        [SessionExpire]
        //[CustomAuthorize(Roles = "CustomerDetail/CanRead")]
        public ActionResult Index(int? Franchises, string LastPurchaseDateMin, string LastPurchaseDateMax, string FromDate, string ToDate, int? page, string Customer, int? TotalOrderMin, int? TotalOrderMax, decimal? TotalAmountMin, decimal? TotalAmountMax, string Mobile, string Email, int? exportType, string submit)
        {
            try
            {
                SessionDetails();
                int lFranchiseID = (Franchises == null) ? 0 : (int)Franchises;
                TempData["FranchiseID"] = lFranchiseID;
                ViewBag.Franchises = new SelectList(db.Franchises.Where(x => x.IsActive == true && x.ContactPerson.Trim().Length > 0).ToList(), "ID", "ContactPerson");

                //int franchiseID = Convert.ToInt32(1);
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
                List<CustomerViewModel> lCustList = custDetail.GetCustomerDetail(null, lFranchiseID);

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
                ViewBag.NoOfCustomer = lCustList.Count();

                return View(lCustList.OrderByDescending(x => x.LastPurchasedDate).ToPagedList(pageNumber, pageSize));
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
        //[CustomAuthorize(Roles = "CustomerDetail/CanRead")]
        public ActionResult Details(int id)
        {
            try
            {
                SessionDetails();
                int franchiseID = -1;
                if (TempData["FranchiseID"] != null)
                {
                    franchiseID = Convert.ToInt32(TempData["FranchiseID"]);
                }

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

        [SessionExpire]
        public ActionResult NewUser(int? page, string Mobile, string Email)
        {
            int pageNumber = (page ?? 1);
            ViewBag.PageNumber = pageNumber;
            ViewBag.PageSize = pageSize;
            ViewBag.Mobile = Mobile;
            ViewBag.Email = Email;
            List<OTPLog> oTPLog = new List<OTPLog>();
            oTPLog = db.OTPLogs.OrderByDescending(p => p.CreateDate).ToList();
            if (!string.IsNullOrEmpty(Mobile) && !string.IsNullOrEmpty(Email))
            {
                oTPLog = db.OTPLogs.Where(p => p.Mobile == Mobile || p.Email == Email).OrderByDescending(p => p.CreateDate).ToList();
            }
            else
            {
                if (!string.IsNullOrEmpty(Mobile))
                {
                    oTPLog = db.OTPLogs.Where(p => p.Mobile == Mobile).OrderByDescending(p => p.CreateDate).ToList();
                }else  if (!string.IsNullOrEmpty(Email))
                {
                    oTPLog = db.OTPLogs.Where(p => p.Email == Email).OrderByDescending(p => p.CreateDate).ToList();
                }
            }
            return View(oTPLog.ToPagedList(pageNumber, pageSize));
        }

        //[SessionExpire]
        //[HttpPost]
        //public ActionResult NewUser(int? page,string Mobile, string Email)
        //{
        //    int pageNumber = (page ?? 1);
        //    ViewBag.PageNumber = pageNumber;
        //    ViewBag.PageSize = pageSize;
        //    ViewBag.Mobile = Mobile;
        //    ViewBag.Email = Email;
        //    List<OTPLog> oTPLog = db.OTPLogs.OrderByDescending(p => p.CreateDate).Take(500).ToList();
        //    return View(oTPLog.ToPagedList(pageNumber, pageSize));
        //}
    }
}