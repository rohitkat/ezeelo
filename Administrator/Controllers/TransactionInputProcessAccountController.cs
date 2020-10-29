using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ModelLayer.Models;
using BusinessLogicLayer.Account;
using ModelLayer.Models.ViewModel.Report.Account;
using Administrator.Models;


using PagedList;
using PagedList.Mvc;
using ClosedXML.Excel;
using System.IO;
using System.Data;
using System.Reflection;
using System.Globalization;
using System.Text;
using BusinessLogicLayer;

namespace Administrator.Controllers
{
    public class UserDetail
    {
        public long UserLoginID { get; set; }
        public long PersonalDetailID { get; set; }
        public string Email { get; set; }
        public string Mobile { get; set; }
        public string PersonName { get; set; }
    }
    public class TransactionInputProcessAccountController : Controller
    {
        private EzeeloDBContext db = new EzeeloDBContext();
        private int pageSize = 10;
        
        StringBuilder errStr = new StringBuilder("=====================================================================================" +
                                  Environment.NewLine
                                  + "ErrorLog Controller : BankController" + Environment.NewLine);

        // GET: /TransactionInputProcessAccount/
        [SessionExpire]
        [CustomAuthorize(Roles = "TransactionInputProcessAccount/CanRead")]
        public ActionResult Index(string SearchString, int? page, string FromDate, string ToDate, FormCollection form)
        {
            try
            {
                #region code
                pageSize = 300;
                int pageNumber = (page ?? 1);
                ViewBag.PageNumber = pageNumber;
                ViewBag.PageSize = pageSize;
                ViewBag.SearchString = SearchString;
                DateTime lFromDate = BusinessLogicLayer.CommonFunctions.GetProperDateTime(FromDate);
                DateTime lToDate = BusinessLogicLayer.CommonFunctions.GetProperDateTime(ToDate);

                ViewBag.FromDate = FromDate;
                ViewBag.ToDate = ToDate;

                ViewBag.lFromDate = lFromDate;
                ViewBag.lToDate = lToDate;

                TempData["FromDate"] = ViewBag.FromDate;
                TempData["ToDate"] = ViewBag.ToDate;
                TempData["Franchise"] = ViewBag.Franchise;

                Report lReport = new Report();

                int lFranchiseID = -1;
                if (form.AllKeys.Contains("Franchise"))
                {
                    if (int.TryParse(form["Franchise"].ToString(), out lFranchiseID)) { }
                    TempData["FranchiseID"] = lFranchiseID;
                }
                else if (TempData["FranchiseID"] != null)
                {
                    if (int.TryParse(TempData["FranchiseID"].ToString(), out lFranchiseID)) { }
                }
                List<Franchise> lFranchises = db.Franchises.Where(x => x.IsActive == true && x.ContactPerson.Length > 0).OrderBy(x => x.ContactPerson).ToList();
                ViewBag.Franchise = new SelectList(lFranchises, "ID", "ContactPerson", lFranchiseID);

                List<ReportTransactionInputProcessAccountViewModel> lReportTransactionInputProcessAccountViewModels = lReport.GetCashReceipts(lFranchiseID, lFromDate, lToDate);
                lReportTransactionInputProcessAccountViewModels = lReportTransactionInputProcessAccountViewModels.
                                                //Where(x => x.MCOShopID == lFranchiseID).
                                                Where(x => x.CreateDate.Date >= lFromDate.Date && x.CreateDate.Date <= lToDate.Date).OrderBy(x => x.OrderCode).ToList();
                TempData.Keep();
                return View(lReportTransactionInputProcessAccountViewModels.ToList().ToPagedList(pageNumber, pageSize));
                #endregion

            }
            catch (Exception ex)
            {
                errStr.Append("Method Name[ReportTransaction] :- Index[HttpGet]" + Environment.NewLine +
                               "ON Dated" + DateTime.UtcNow.AddHours(5.30).TimeOfDay + Environment.NewLine +
                                   ex.Message.ToString() + Environment.NewLine +
                         "====================================================================================="
                               );
                ModelState.AddModelError("Message", "Sorry! Problem in Generate Report!!");
                ErrorLog.ErrorLogFile("Sorry! Problem in Generate Report!! " + Environment.NewLine + errStr.ToString()
                    , ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
                return View();
            }
        }

        // GET: /TransactionInputProcessAccount/Details/5
        [SessionExpire]
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TransactionInputProcessAccount transactioninputprocessaccount = db.TransactionInputProcessAccounts.Find(id);
            if (transactioninputprocessaccount == null)
            {
                return HttpNotFound();
            }
            return View(transactioninputprocessaccount);
        }

        // GET: /TransactionInputProcessAccount/Create
        [SessionExpire]
        [CustomAuthorize(Roles = "TransactionInputProcessAccount/CanWrite")]
        public ActionResult Create()
        {
            ViewBag.LeadgerHeadID = new SelectList(db.LedgerHeads, "ID", "Name");
            ViewBag.ReceivedPaymentModeID = new SelectList(db.PaymentModes.Where(x => x.IsActive == true).ToList(), "ID", "Name");
            //ViewBag.TransactionInputID = new SelectList(db.TransactionInputs, "ID", "NetworkIP");
            return View();
        }

        // POST: /TransactionInputProcessAccount/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [SessionExpire]
        [CustomAuthorize(Roles = "TransactionInputProcessAccount/CanWrite")]
        public ActionResult Create([Bind(Include = "LeadgerHeadID,ReceivedPaymentModeID,CustomerOrderID,PODReceived,Narration")] TransactionInputProcessAccount transactioninputprocessaccount, string OrderCode, string ReceivedBy)
        {
            try
            {
                ModelLayer.Models.CustomerOrder lCustomerOrder = GetCustomerOrder1(OrderCode);

                TransactionInputProcessAccount lTransactionInputProcessAccount = db.TransactionInputProcessAccounts.FirstOrDefault(x => x.CustomerOrderID == lCustomerOrder.ID);
                if(lTransactionInputProcessAccount != null)
                {
                    TempData["Messaage"] = lCustomerOrder.OrderCode +  " Already Paid on " +  lCustomerOrder.CreateDate;
                    ViewBag.LeadgerHeadID = new SelectList(db.LedgerHeads, "ID", "Name");
                    ViewBag.ReceivedPaymentModeID = new SelectList(db.PaymentModes, "ID", "Name");
                    return View();
                }


                UserDetail lUserDetail = GetUserDetail1(ReceivedBy);
                transactioninputprocessaccount.CustomerOrderID = lCustomerOrder.ID;
                transactioninputprocessaccount.Amount = lCustomerOrder.PayableAmount;
                transactioninputprocessaccount.ReceivedFromUserLoginID = lUserDetail.UserLoginID;
                transactioninputprocessaccount.CreateDate = DateTime.UtcNow.AddHours(5.3);
                transactioninputprocessaccount.CreateBy = CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(Session["ID"]));
                transactioninputprocessaccount.NetworkIP = CommonFunctions.GetClientIP();
                if (ModelState.IsValid)
                {
                    db.TransactionInputProcessAccounts.Add(transactioninputprocessaccount);
                    db.SaveChanges();
                    //return RedirectToAction("Index");
                }
                ViewBag.LeadgerHeadID = new SelectList(db.LedgerHeads, "ID", "Name");
                ViewBag.ReceivedPaymentModeID = new SelectList(db.PaymentModes.Where(x => x.IsActive == true).ToList(), "ID", "Name");
                //ViewBag.TransactionInputID = new SelectList(db.TransactionInputs, "ID", "NetworkIP", transactioninputprocessaccount.TransactionInputID);
                TempData["Messaage"] = "Receipt Detail Created Successfully";
                return RedirectToAction("Create");
            }
            catch (Exception ex)
            {
                errStr.Append("Method Name[Http Request] :- Create[HttpPost]" + Environment.NewLine +
                               "ON Dated" + DateTime.UtcNow.AddHours(5.30).TimeOfDay + Environment.NewLine +
                                   ex.Message.ToString() + Environment.NewLine +
                         "====================================================================================="
                               );
                //ViewBag.Message = "Sorry! Problem in customer registration!!";
                ModelState.AddModelError("Message", "Sorry! Problem in Record Creation!!");
                ErrorLog.ErrorLogFile("Sorry! Problem in Record Creation!!" + Environment.NewLine + errStr.ToString()
                    , ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
                ViewBag.LeadgerHeadID = new SelectList(db.LedgerHeads, "ID", "Name");
                ViewBag.ReceivedPaymentModeID = new SelectList(db.PaymentModes.Where(x => x.IsActive == true).ToList(), "ID", "Name");
                return View();
            }


        }

        //GET: /TransactionInputProcessAccount/Edit/5
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TransactionInputProcessAccount transactioninputprocessaccount = db.TransactionInputProcessAccounts.Find(id);
            if (transactioninputprocessaccount == null)
            {
                return HttpNotFound();
            }
            ViewBag.TransactionInputID = new SelectList(db.TransactionInputs, "ID", "NetworkIP", transactioninputprocessaccount.TransactionInputID);
            return View(transactioninputprocessaccount);
        }

        // POST: /TransactionInputProcessAccount/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="ID,LeadgerHeadID,ReceivedPaymentModeID,TransactionInputID,Amount,ReceivedFromUserLoginID,PODReceived,Narration,CreateDate,CreateBy,ModifyDate,ModifyBy,NetworkIP,DeviceType,DeviceID")] TransactionInputProcessAccount transactioninputprocessaccount)
        {
            TransactionInputProcessAccount lTransactionInputProcessAccount = new TransactionInputProcessAccount();
            try
            {
                lTransactionInputProcessAccount = db.TransactionInputProcessAccounts.Find(transactioninputprocessaccount.ID);
                if(lTransactionInputProcessAccount == null)
                {
                    return View("Error");
                }
                lTransactionInputProcessAccount.PODReceived = transactioninputprocessaccount.PODReceived;
                lTransactionInputProcessAccount.Narration = transactioninputprocessaccount.Narration;
                lTransactionInputProcessAccount.ModifyBy = CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(Session["ID"]));
                lTransactionInputProcessAccount.ModifyDate = DateTime.UtcNow.AddHours(5.3);
            }
            catch (Exception ex)
            {
                errStr.Append("Method Name[Http Request] :- Create[HttpPost]" + Environment.NewLine +
                               "ON Dated" + DateTime.UtcNow.AddHours(5.30).TimeOfDay + Environment.NewLine +
                                   ex.Message.ToString() + Environment.NewLine +
                         "====================================================================================="
                               );
                //ViewBag.Message = "Sorry! Problem in customer registration!!";
                ModelState.AddModelError("Message", "Sorry! Problem in Record Creation!!");
                ErrorLog.ErrorLogFile("Sorry! Problem in Record Creation!!" + Environment.NewLine + errStr.ToString()
                    , ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
                //ViewBag.LeadgerHeadID = new SelectList(db.LedgerHeads, "ID", "Name");
                //ViewBag.ReceivedPaymentModeID = new SelectList(db.PaymentModes.Where(x => x.IsActive == true).ToList(), "ID", "Name");
                return View();
            }
            if (ModelState.IsValid)
            {
                db.Entry(lTransactionInputProcessAccount).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            //ViewBag.TransactionInputID = new SelectList(db.TransactionInputs, "ID", "NetworkIP", transactioninputprocessaccount.TransactionInputID);
            return View(transactioninputprocessaccount);
        }

        private ModelLayer.Models.CustomerOrder GetCustomerOrder1(string pGBOD)
        {
            ModelLayer.Models.CustomerOrder lCustomerOrder = new ModelLayer.Models.CustomerOrder();
            try
            {
                lCustomerOrder = db.CustomerOrders.FirstOrDefault(x => x.OrderCode.Trim() == pGBOD.Trim());
            }
            catch (Exception ex)
            {
                throw;
            }
            return lCustomerOrder;
        }

        private UserDetail GetUserDetail1(string pUsername)
        {
            /*This Action Responces to AJAX Call
             * */
            UserDetail lUserDetail = new UserDetail();
            try
            {
                lUserDetail = (from ul in db.UserLogins
                               join pd in db.PersonalDetails on ul.ID equals pd.UserLoginID
                               where (ul.Mobile.Trim() == pUsername.Trim() || ul.Email.Trim() == pUsername.Trim())
                               select new UserDetail
                               {
                                   UserLoginID = ul.ID,
                                   PersonalDetailID = pd.ID,
                                   Email = ul.Email,
                                   Mobile = ul.Mobile,
                                   PersonName = pd.FirstName
                               }).ToList().FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw;
            }

            return lUserDetail;
        }

        public ActionResult GetCustomerOrder(string GBOD)
        {
            /*This Action Responces to AJAX Call
             * */
            ModelLayer.Models.CustomerOrder lCustomerOrder = db.CustomerOrders.FirstOrDefault(x => x.OrderCode == GBOD.Trim());


            if (lCustomerOrder == null)
            {
                var errorMsg = "Ordercode Dose Not Exist.";
                return View(new { success = false, Error = errorMsg });
            }

            TransactionInputProcessAccount lTransactionInputProcessAccount = db.TransactionInputProcessAccounts.FirstOrDefault(x => x.CustomerOrderID == lCustomerOrder.ID);
            if(lTransactionInputProcessAccount != null)
            {
                ViewBag.TransactionInputProcessAccount = lTransactionInputProcessAccount;
            }

            ViewBag.CustomerOrder = lCustomerOrder;
            return PartialView("_CustomerOrder");
        }

        public ActionResult GetUserDetail(string Username)
        {
            /*This Action Responces to AJAX Call
             * */
            UserDetail lUserDetail = (from ul in db.UserLogins
                                      join pd in db.PersonalDetails on ul.ID equals pd.UserLoginID
                                      where (ul.Mobile.Trim() == Username.Trim() || ul.Email.Trim() == Username.Trim())
                                      select new UserDetail
                                      {
                                          UserLoginID = ul.ID,
                                          PersonalDetailID = pd.ID,
                                          Email = ul.Email,
                                          Mobile = ul.Mobile,
                                          PersonName = pd.FirstName
                                      }).ToList().FirstOrDefault();

            if (lUserDetail == null)
            {
                var errorMsg = "User Dose Not Exist.";
                return View(new { success = false, Error = errorMsg });
            }
            ViewBag.UserDetail = lUserDetail;
            return PartialView("_UserDetail");
        }

        public ActionResult GetAccountPendingReport()
        {
            List<ReportTransactionInputPendingAccountViewModel> lReportTransactionInputPendingAccountViewModels = new List<ReportTransactionInputPendingAccountViewModel>();
            try
            {
                #region code
                //pageSize = 300;
                //int pageNumber = (page ?? 1);
                //ViewBag.PageNumber = pageNumber;
                //ViewBag.PageSize = pageSize;
                //ViewBag.SearchString = SearchString;
                //DateTime lFromDate = BusinessLogicLayer.CommonFunctions.GetProperDateTime(FromDate);
                //DateTime lToDate = BusinessLogicLayer.CommonFunctions.GetProperDateTime(ToDate);

                //ViewBag.FromDate = FromDate;
                //ViewBag.ToDate = ToDate;

                //ViewBag.lFromDate = lFromDate;
                //ViewBag.lToDate = lToDate;

                //TempData["FromDate"] = ViewBag.FromDate;
                //TempData["ToDate"] = ViewBag.ToDate;
                //TempData["Franchise"] = ViewBag.Franchise;

                Report lReport = new Report();

                int lFranchiseID = -1;
                if(TempData["FranchiseID"]  != null)
                {
                    lFranchiseID = Convert.ToInt32(TempData["FranchiseID"]);
                }
                //if (form.AllKeys.Contains("Franchise"))
                //{
                //    if (int.TryParse(form["Franchise"].ToString(), out lFranchiseID)) { }
                //    TempData["FranchiseID"] = lFranchiseID;
                //}
                //else if (TempData["FranchiseID"] != null)
                //{
                //    if (int.TryParse(TempData["FranchiseID"].ToString(), out lFranchiseID)) { }
                //}
                List<Franchise> lFranchises = db.Franchises.Where(x => x.IsActive == true && x.ContactPerson.Length > 0).OrderBy(x => x.ContactPerson).ToList();
                ViewBag.Franchise = new SelectList(lFranchises, "ID", "ContactPerson", lFranchiseID);

                lReportTransactionInputPendingAccountViewModels = lReport.GetAccountPending(lFranchiseID);
                //TempData.Keep();
                return View("_AccountPending", lReportTransactionInputPendingAccountViewModels.ToList());
                #endregion

            }
            catch (Exception ex)
            {
                errStr.Append("Method Name[ReportTransaction] :- Index[HttpGet]" + Environment.NewLine +
                               "ON Dated" + DateTime.UtcNow.AddHours(5.30).TimeOfDay + Environment.NewLine +
                                   ex.Message.ToString() + Environment.NewLine +
                         "====================================================================================="
                               );
                ModelState.AddModelError("Message", "Sorry! Problem in Generate Report!!");
                ErrorLog.ErrorLogFile("Sorry! Problem in Generate Report!! " + Environment.NewLine + errStr.ToString()
                    , ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
                return View("_AccountPending", lReportTransactionInputPendingAccountViewModels.ToList());
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
