using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;
using Leaders.Filter;
using ModelLayer.Models;
using ModelLayer.Models.ViewModel;

namespace Leaders.Areas.Admin.Controllers
{
    [AdminSessionExpire]
    public class EWalletRefundController : Controller
    {
        private EzeeloDBContext db = new EzeeloDBContext();
        ModelLayer.Models.ViewModel.EwalletRefundvm ls = new ModelLayer.Models.ViewModel.EwalletRefundvm();
        // GET: Admin/EWalletRefund
        public ActionResult Index()
        {
            //var Ewallet = GetEwallet().Where(x => x.Isactive == false).OrderByDescending(x => x.Date).ToList();

            ls.ewallets = (from n in db.eWalletRefund_Table
                           join m in db.UserLogins on n.UserLoginId equals m.ID
                           join p in db.PersonalDetails on n.UserLoginId equals p.UserLoginID
                           join q in db.CustomerOrders on n.CustomerOrderId equals q.ID
                           select new ModelLayer.Models.ViewModel.EwalletRefundviewmodel
                           {
                               Name = p.FirstName + " " + p.LastName,
                               Mobile = m.Mobile,
                               Email = m.Email,
                               OrderCode = q.OrderCode,
                               ID = n.ID,
                               RefundAmt = n.RefundAmt,
                               RequsetAmt = n.RequsetAmt,
                               UserLoginId = n.UserLoginId,
                               CustomerOrderId = n.CustomerOrderId,
                               Comment = n.Comment,
                               Isactive = n.Isactive,
                               Date = n.Date,
                               Status = n.Status
                           }).Where(x => x.Isactive == false).OrderByDescending(x => x.Date).ToList();

            foreach (var item in ls.ewallets)
            {
                if (item.Status == 0)
                {
                    item.ReturnStatus = "Pending";
                }
                if (item.Status == 1)
                {
                    item.ReturnStatus = "Accepted";
                }
                if (item.Status == 2)
                {
                    item.ReturnStatus = "Cancelled";
                }
            }

            return View(ls);
        }



       public ActionResult EWsearch()
        {
            return View();
        }

        public ActionResult ListBetweenDate(string startDate, string endDate)
        {
            DateTime sDate = Convert.ToDateTime(startDate);
            DateTime eDate = Convert.ToDateTime(endDate);
            var Wallet = GetEwallet().ToList();
            var Ewallet = GetEwallet().Where(x => EntityFunctions.TruncateTime(x.Date) >=sDate.Date && EntityFunctions.TruncateTime(x.Date) <= eDate.Date).AsEnumerable().ToList();
            return PartialView("_PartialEwallet", Ewallet);
        }

        public ActionResult partialUserList()
        {
            
            var Ewallet = GetEwallet().OrderByDescending(x => x.Date).ToList();

            return PartialView("_PartialEwallet", Ewallet);

        }

        


        [HttpPost]
        public PartialViewResult Ewalletpartial()
        {

            var EList = GetEwallet().ToList();
            return PartialView("_PartialEwallet", EList);
        }




        public ActionResult ExportToExcel(string fromDate, string toDate)
        {
            List<EwalletRefundviewmodel> Elist = GetEwallet().ToList();
            ExcelEwalletRefund objExcel = new ExcelEwalletRefund();
            List<ExcelEwalletRefund> leadersList = new List<ExcelEwalletRefund>();
            if (toDate != "" && fromDate != "")
            {
                //DateTime sDate = Convert.ToDateTime(fromDate);
                //DateTime eDate = Convert.ToDateTime(toDate);

                //if (fromDate == toDate)
                //{
                //    var SDate = new DateTime(sDate.Year, sDate.Month, sDate.Day, sDate.Hour, sDate.Minute, 1);
                //    var EDate = SDate.AddDays(1).AddMinutes(-1);
                //    Elist = ls.ewallets.Where(x => x.Date >= SDate && x.Date <= EDate).ToList();
                //}


                Elist = ls.ewallets.OrderByDescending(x => x.Date).ToList();
            }
            foreach (var item in Elist)
            {

                leadersList.Add(new ExcelEwalletRefund
                {
                    ID = item.ID,
                    Name = item.Name,
                    Email = item.Email,
                    Mobile = item.Mobile,
                    OrderCode = item.OrderCode,
                    RequsetAmt = item.RequsetAmt,
                    RefundAmt = item.RefundAmt,
                    Comment = item.Comment,
                    Status = item.ReturnStatus,
                    UserLoginId = item.UserLoginId,
                    CustomerOrderId = item.CustomerOrderId,
                    Date = item.Date

                });

            }

            leadersList.Add(objExcel);


            var gv = new GridView();
            gv.DataSource = leadersList;
            gv.DataBind();
            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename=EwalletListExcel.xls");
            Response.ContentType = "application/ms-excel";
            Response.Charset = "";
            StringWriter objStringWriter = new StringWriter();
            HtmlTextWriter objHtmlTextWriter = new HtmlTextWriter(objStringWriter);
            gv.RenderControl(objHtmlTextWriter);
            Response.Output.Write(objStringWriter.ToString());
            Response.Flush();
            Response.End();
            return View("Index");
        }

      

        public IList<EwalletRefundviewmodel> GetEwallet()
        {
            ls.ewallets = (from n in db.eWalletRefund_Table
                           join m in db.UserLogins on n.UserLoginId equals m.ID
                           join p in db.PersonalDetails on n.UserLoginId equals p.UserLoginID
                           join q in db.CustomerOrders on n.CustomerOrderId equals q.ID
                           select new ModelLayer.Models.ViewModel.EwalletRefundviewmodel
                           {
                               Name = p.FirstName + " " + p.LastName,
                               Mobile = m.Mobile,
                               Email = m.Email,
                               OrderCode = q.OrderCode,
                               ID = n.ID,
                               RefundAmt = n.RefundAmt,
                               RequsetAmt = n.RequsetAmt,
                               UserLoginId = n.UserLoginId,
                               CustomerOrderId = n.CustomerOrderId,
                               Comment = n.Comment,
                               Isactive = n.Isactive,
                               Date = n.Date,
                               Status = n.Status
                           }).ToList();

            foreach (var item in ls.ewallets)
            {
                if (item.Status == 0)
                {
                    item.ReturnStatus = "Pending";
                }
                if (item.Status == 1)
                {
                    item.ReturnStatus = "Accepted";
                }
                if (item.Status == 2)
                {
                    item.ReturnStatus = "Cancelled";
                }
            }

            return ls.ewallets;
        }



        public ActionResult Approval(int id)
        {
            ViewBag.id = id;

            return View();
        }

        [HttpPost]
        public ActionResult Approval(EWalletRefund_Table eWalletRefund_Table)
        {
            try
            {
                EWalletRefund_Table objEwallet = db.eWalletRefund_Table.FirstOrDefault(x => x.ID == eWalletRefund_Table.ID);
                if (objEwallet != null)
                {
                    objEwallet.RefundAmt = eWalletRefund_Table.RefundAmt;
                    objEwallet.Isactive = true;
                    objEwallet.Status = 1;
                    objEwallet.Comment = eWalletRefund_Table.Comment;
                    objEwallet.ModifiedDate = DateTime.Now;
                    objEwallet.NetworkID = BusinessLogicLayer.CommonFunctions.GetClientIP();
                    objEwallet.DeviceType = "Net Browser";
                    objEwallet.DeviceID = "x";
                    db.Entry(objEwallet).State = EntityState.Modified;

                    MLMWallet objmlmw = db.MLMWallets.FirstOrDefault(x => x.UserLoginID == objEwallet.UserLoginId);
                    decimal camt = objmlmw.Amount;
                    decimal amt = objEwallet.RefundAmt + objmlmw.Amount;
                    objmlmw.Amount = amt;
                    objmlmw.Points = amt * 10;
                    objmlmw.LastModifyDate = DateTime.Now;
                    db.Entry(objmlmw).State = EntityState.Modified;
                    MlmWalletlog objmlmlog = new MlmWalletlog();
                    objmlmlog.Amount = amt;
                    objmlmlog.IsCredit = true;
                    objmlmlog.CurrentAmt = camt;
                    objmlmlog.UserLoginID = objEwallet.UserLoginId;
                    objmlmlog.CustomerOrderId = objEwallet.CustomerOrderId;
                    objmlmlog.CreatedDate = DateTime.Now;
                    objmlmlog.NetworkID = BusinessLogicLayer.CommonFunctions.GetClientIP();
                    objmlmlog.DeviceType = "Net Browser";
                    objmlmlog.DeviceID = "x";
                    objmlmlog.EwalletRefund_TableID = eWalletRefund_Table.ID;
                    db.mlmWalletlogs.Add(objmlmlog);
                    SendSMSToCustomer(eWalletRefund_Table.ID, objEwallet.Status,eWalletRefund_Table.RefundAmt);
                    Send_EWalletRefund_Mail(eWalletRefund_Table.ID, objEwallet.Status, eWalletRefund_Table.RefundAmt);

                     db.SaveChanges();
                }

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                return View();
            }

        }


        public ActionResult Decline(int id)
        {
            return View();
        }

        [HttpPost]
        public ActionResult Decline(EWalletRefund_Table eWalletRefund_Table)
        {
            try
            {
                EWalletRefund_Table objEwallet = db.eWalletRefund_Table.FirstOrDefault(x => x.ID == eWalletRefund_Table.ID);
                if (objEwallet != null)
                {
                    objEwallet.RefundAmt = 0;
                    objEwallet.Isactive = true;
                    objEwallet.Status = 2;
                    objEwallet.Comment = eWalletRefund_Table.Comment;
                    objEwallet.ModifiedDate = DateTime.Now;
                    objEwallet.NetworkID = BusinessLogicLayer.CommonFunctions.GetClientIP();
                    objEwallet.DeviceType = "Net Browser";
                    objEwallet.DeviceID = "x";
                    db.Entry(objEwallet).State = EntityState.Modified;

                    MLMWallet objmlmw = db.MLMWallets.FirstOrDefault(x => x.UserLoginID == objEwallet.UserLoginId);
                    decimal camt = objmlmw.Amount;
                    decimal amt = objEwallet.RefundAmt + objmlmw.Amount;
                    objmlmw.Amount = amt;
                    objmlmw.Points = amt * 10;
                    objmlmw.LastModifyDate = DateTime.Now;
                    db.Entry(objmlmw).State = EntityState.Modified;
                    MlmWalletlog objmlmlog = new MlmWalletlog();
                    objmlmlog.Amount = amt;
                    objmlmlog.IsCredit = false;
                    objmlmlog.CurrentAmt = camt;
                    objmlmlog.UserLoginID = objEwallet.UserLoginId;
                    objmlmlog.CustomerOrderId = objEwallet.CustomerOrderId;
                    objmlmlog.CreatedDate = DateTime.Now;
                    objmlmlog.NetworkID = BusinessLogicLayer.CommonFunctions.GetClientIP();
                    objmlmlog.DeviceType = "Net Browser";
                    objmlmlog.DeviceID = "x";
                    objmlmlog.EwalletRefund_TableID = eWalletRefund_Table.ID;
                    db.mlmWalletlogs.Add(objmlmlog);

                    SendSMSToCustomer(eWalletRefund_Table.ID, objEwallet.Status, objEwallet.RefundAmt);
                    Send_EWalletRefund_Mail(eWalletRefund_Table.ID, objEwallet.Status, objEwallet.RefundAmt);

                    db.SaveChanges();
                }
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                return View();
            }
        }


        #region Method
        [HttpGet]
        public JsonResult getrefundamount(int id, decimal Refundamt)
        {
            string msg = null;
            if (db.eWalletRefund_Table.Any(p => p.ID == id && p.RequsetAmt < Refundamt || Refundamt <= 0))
            {
                decimal amt= db.eWalletRefund_Table.Where(p => p.ID == id).Select(p => p.RequsetAmt).FirstOrDefault();

                msg = " Refund Amount is greater than request amount. request amount is  " + amt +" or Refund Amount is not less than 0";
            }
            return Json(msg, JsonRequestBehavior.AllowGet);
        }


        public void SendSMSToCustomer(long ID, int? status, decimal RefundAmt)
        {
            string requestID = "";
            string requestedAmount = "";
            string OrderCode = "";
            //List<EWalletRefund_Table> Ewallet = new List<EWalletRefund_Table>();
            var Ewallet = db.eWalletRefund_Table.Where(x => x.ID == ID).FirstOrDefault();
            try
            {
                // var lOrder = db.CustomerOrders.Where(x => x.ID == orderId).ToList();


                if (status == 1)
                {
                    requestID = ID.ToString();
                    requestedAmount = RefundAmt.ToString();
                    //requestedAmount = db.eWalletRefund_Table.Where(z => z.ID == ID).Select(z => z.RefundAmt).FirstOrDefault().ToString();
                    OrderCode = db.CustomerOrders.Where(x => x.ID == Ewallet.CustomerOrderId).Select(x => x.OrderCode).FirstOrDefault();

                }
                if (status == 2)
                {
                    requestID = ID.ToString();
                    requestedAmount = RefundAmt.ToString();
                    //requestedAmount = db.eWalletRefund_Table.Where(z => z.ID == ID).Select(z => z.RefundAmt).FirstOrDefault().ToString();
                    OrderCode = db.CustomerOrders.Where(x => x.ID == Ewallet.CustomerOrderId).Select(x => x.OrderCode).FirstOrDefault();
                }

                var UserLoginid = db.eWalletRefund_Table.Where(x => x.ID == ID).Select(x => x.UserLoginId).FirstOrDefault();
                //string mobileNo = "8421118393";
                string mobileNo = db.UserLogins.Where(x => x.ID == UserLoginid).Select(y => y.Mobile).FirstOrDefault();
                var lPersonalDetails = db.PersonalDetails.Where(x => x.UserLoginID == UserLoginid).ToList();


                Dictionary<string, string> dictSMSValues = new Dictionary<string, string>();

                dictSMSValues.Add("#--NAME--#", Convert.ToString(lPersonalDetails.FirstOrDefault().FirstName));
                dictSMSValues.Add("#--REQUEST_ID--#", requestID);
                dictSMSValues.Add("#--REQUEST_AMOUNT--#", requestedAmount);
                dictSMSValues.Add("#--ORDER_CODE--#", OrderCode);

                BusinessLogicLayer.GateWay gateWay = new BusinessLogicLayer.SMS(System.Web.HttpContext.Current.Server);
                if (status == 2)
                {
                    var custid = db.eWalletRefund_Table.Where(x => x.ID == ID).Select(x => x.CustomerOrderId).FirstOrDefault();


                    if (db.CustomerOrderDetails.Any(x => x.CustomerOrderID == custid && x.OrderStatus == 7 || x.OrderStatus == 1))
                    {
                        gateWay.SendSMS(BusinessLogicLayer.GateWay.SMSGateWays.SUMIT, BusinessLogicLayer.GateWay.SMSOptions.SINGLE, BusinessLogicLayer.GateWay.SMSTypes.CANCEL_PARTIAL_EWALLET_REFUND_REQUEST, new string[] { mobileNo }, dictSMSValues);
                    }
                    else
                    {
                        gateWay.SendSMS(BusinessLogicLayer.GateWay.SMSGateWays.SUMIT, BusinessLogicLayer.GateWay.SMSOptions.SINGLE, BusinessLogicLayer.GateWay.SMSTypes.CANCEL_EWALLET_REFUND_REQUEST, new string[] { mobileNo }, dictSMSValues);

                    }

                }
                if (status == 1)
                {
                    var custid = db.eWalletRefund_Table.Where(x => x.ID == ID).Select(x => x.CustomerOrderId).FirstOrDefault();


                    if (db.CustomerOrderDetails.Any(x => x.CustomerOrderID == custid && x.OrderStatus == 7 || x.OrderStatus == 1))
                    {
                        gateWay.SendSMS(BusinessLogicLayer.GateWay.SMSGateWays.SUMIT, BusinessLogicLayer.GateWay.SMSOptions.SINGLE, BusinessLogicLayer.GateWay.SMSTypes.ACCEPT_EWALLET_REFUND_PARTIAL, new string[] { mobileNo }, dictSMSValues);

                    }
                    else
                    {
                        gateWay.SendSMS(BusinessLogicLayer.GateWay.SMSGateWays.SUMIT, BusinessLogicLayer.GateWay.SMSOptions.SINGLE, BusinessLogicLayer.GateWay.SMSTypes.ACCEPT_EWALLET_REFUND, new string[] { mobileNo }, dictSMSValues);
                    }

                }


            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[PaymentProcessController][M:SendOTPToCustomer]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Gandhibagh, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[PaymentProcessController][M:SendOTPToCustomer]",
                    BusinessLogicLayer.ErrorLog.Module.Gandhibagh, System.Web.HttpContext.Current.Server);
            }

        }



        public void Send_EWalletRefund_Mail(long ID, int? status, decimal RefundAmt)

        {

            bool flag = false;
            var UserLoginid = db.eWalletRefund_Table.Where(x => x.ID == ID).Select(x => x.UserLoginId).FirstOrDefault();
            string Emailid = db.UserLogins.Where(x => x.ID == UserLoginid).Select(x => x.Email).FirstOrDefault();
            try

            {
                string requestID = "";

                string OrderCode = "";
                var Ewallet = db.eWalletRefund_Table.Where(x => x.ID == ID).FirstOrDefault();
                string cancelledDate;
                string CancelledproductTotal_Amt;
                //var CustomerOrder = db.CustomerOrders.Where(x => x.ID == orderId).FirstOrDefault();
                var personalDetail = db.PersonalDetails.Where(x => x.UserLoginID == UserLoginid).FirstOrDefault();

                string Name = personalDetail.FirstName + " " + personalDetail.LastName;
                //string URL = "http://www.ezeelo.com/nagpur/2/login?Phone=" + mobile + "&ReferalCode=" + RefferalCode + "&Name=" + name + "&Email=" + Email;
                //string URL = "" + (new URLsFromConfig()).GetURL("LEADERS") + "LeadersLogin/Login/?UserName=" + Email + "&Password=" + Password;

                string Requestamount = Ewallet.RequsetAmt.ToString();
                string RefundAmount = RefundAmt.ToString();
                string RequestDate = Ewallet.Date.ToString();
                string CurrentDate = DateTime.Now.ToString();

                if (status == 1)
                {
                    requestID = ID.ToString();
                    //RefundAmount = RefundAmt.ToString();
                    //requestedAmount = db.eWalletRefund_Table.Where(z => z.ID == ID).Select(z => z.RefundAmt).FirstOrDefault().ToString();
                    OrderCode = db.CustomerOrders.Where(x => x.ID == Ewallet.CustomerOrderId).Select(x => x.OrderCode).FirstOrDefault();

                }
                if (status == 2)
                {
                    requestID = ID.ToString();
                    //RefundAmount = RefundAmt.ToString();
                    //requestedAmount = db.eWalletRefund_Table.Where(z => z.ID == ID).Select(z => z.RefundAmt).FirstOrDefault().ToString();
                    OrderCode = db.CustomerOrders.Where(x => x.ID == Ewallet.CustomerOrderId).Select(x => x.OrderCode).FirstOrDefault();
                }







                Dictionary<string, string> dictEmailValues = new Dictionary<string, string>();
                dictEmailValues.Add("<!--CUSTOMER_NAME-->", Name);
                dictEmailValues.Add("<!--REQUEST_AMOUNT-->", Requestamount);
                dictEmailValues.Add("<!--REQUEST_ID-->", requestID);
                dictEmailValues.Add("<!--REQUEST_DATE-->", RequestDate);
                dictEmailValues.Add("<!--RefundAmount-->", RefundAmount);
                //dictEmailValues.Add("<!--TRANSACTION_ID-->", TransactionID);
                //dictEmailValues.Add("<!--MOBILE-->", Mobile);
                dictEmailValues.Add("<!--ORDER_CODE-->", OrderCode);
                dictEmailValues.Add("<!--Date-->", CurrentDate);
                //dictEmailValues.Add("<!--CREDITED_DATE-->", TransactionDate);
                //dictEmailValues.Add("<!--URL-->", URL);

                BusinessLogicLayer.GateWay gateWay = new BusinessLogicLayer.Email(System.Web.HttpContext.Current.Server);
                // string EmailID = "tech@ezeelo.com";

                if (status == 2)
                {
                    var custid = db.eWalletRefund_Table.Where(x => x.ID == ID).Select(x => x.CustomerOrderId).FirstOrDefault();


                    if (db.CustomerOrderDetails.Any(x => x.CustomerOrderID == custid && x.OrderStatus == 7 || x.OrderStatus == 1))
                    {
                        gateWay.SendEmail(BusinessLogicLayer.GateWay.EmailGateWays.GANDHIBAGH, BusinessLogicLayer.GateWay.SenderMail.INFO, BusinessLogicLayer.GateWay.EMailTypes.EWALLET_REFUND_PCANCEL_LEADERS_ADMIN, new string[] { Emailid }, dictEmailValues, true);
                    }
                    else
                    {
                        gateWay.SendEmail(BusinessLogicLayer.GateWay.EmailGateWays.GANDHIBAGH, BusinessLogicLayer.GateWay.SenderMail.INFO, BusinessLogicLayer.GateWay.EMailTypes.EWALLET_REFUND_CANCEL_LEADER_ADMIN, new string[] { Emailid }, dictEmailValues, true);
                    }

                }
                if (status == 1)
                {
                    var custid = db.eWalletRefund_Table.Where(x => x.ID == ID).Select(x => x.CustomerOrderId).FirstOrDefault();


                    if (db.CustomerOrderDetails.Any(x => x.CustomerOrderID == custid && x.OrderStatus == 7 || x.OrderStatus == 1))
                    {
                        gateWay.SendEmail(BusinessLogicLayer.GateWay.EmailGateWays.GANDHIBAGH, BusinessLogicLayer.GateWay.SenderMail.INFO, BusinessLogicLayer.GateWay.EMailTypes.EWALLET_PACCEPTED_LEADERS_ADMIN, new string[] { Emailid }, dictEmailValues, true);
                    }
                    else
                    {
                        gateWay.SendEmail(BusinessLogicLayer.GateWay.EmailGateWays.GANDHIBAGH, BusinessLogicLayer.GateWay.SenderMail.INFO, BusinessLogicLayer.GateWay.EMailTypes.EWALLET_ACCEPTED_LEADERS_ADMIN, new string[] { Emailid }, dictEmailValues, true);
                    }

                }
                // gateWay.SendEmail(BusinessLogicLayer.GateWay.EmailGateWays.GANDHIBAGH, BusinessLogicLayer.GateWay.SenderMail.INFO, BusinessLogicLayer.GateWay.EMailTypes.EWALLET_REFUND, new string[] { "roshan.gomase@ezeelo.com" }, dictEmailValues, true);
                flag = true;

            }
            catch (Exception ex)

            {

            }

            //return flag;

        }


        #endregion


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
