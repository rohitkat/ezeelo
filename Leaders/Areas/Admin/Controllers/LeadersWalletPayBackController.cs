using BusinessLogicLayer;
using ModelLayer.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Leaders.Areas.Admin.Controllers
{
    public class LeadersWalletPayBackController : Controller
    {
        private EzeeloDBContext db = new EzeeloDBContext();
        public ActionResult Index()
        {
            WalletPayBackViewModel obj = new WalletPayBackViewModel();

            List<LeadersWalletPayBackOrderDataViewModel> list = new List<LeadersWalletPayBackOrderDataViewModel>();
            int year = DateTime.Now.Year;
            int month = DateTime.Now.Month;
            year = (month == 1) ? (year - 1) : year;

            DateTime FromDate = new DateTime(year, DateTime.Now.Month, 1);
            DateTime ToDate = FromDate.AddMonths(1).AddDays(-1).AddHours(23).AddMinutes(59).AddSeconds(59);
            list = GetList(FromDate, ToDate, 0);

            obj.callType = 0;
            obj.FromDate = FromDate;
            obj.ToDate = ToDate;
            obj.list = list;
            return View(obj);
        }

        public List<LeadersWalletPayBackOrderDataViewModel> GetList(DateTime FromDate, DateTime ToDate, int Type_)
        {
            List<LeadersWalletPayBackOrderDataViewModel> list = new List<LeadersWalletPayBackOrderDataViewModel>();
            var FrmDate = new SqlParameter
            {
                ParameterName = "FromDate",
                Value = FromDate
            };
            var TDate = new SqlParameter
            {
                ParameterName = "ToDate",
                Value = ToDate
            };
            var Type = new SqlParameter
            {
                ParameterName = "Type",
                Value = Type_
            };
            list = db.Database.SqlQuery<LeadersWalletPayBackOrderDataViewModel>("EXEC SPLeadersWalletPayBack @FromDate,@ToDate,@Type", FrmDate, TDate, Type).ToList<LeadersWalletPayBackOrderDataViewModel>();
            return list;
        }


        [HttpPost]
        public ActionResult GetData(WalletPayBackViewModel obj)
        {
            List<LeadersWalletPayBackOrderDataViewModel> list = new List<LeadersWalletPayBackOrderDataViewModel>();
            DateTime FromDate = obj.FromDate;
            DateTime ToDate = obj.ToDate.AddHours(23).AddMinutes(59).AddSeconds(59);
            list = GetList(FromDate, ToDate, obj.callType);

            obj.FromDate = FromDate;
            obj.ToDate = ToDate;
            obj.list = list;
            ViewBag.StatusHeading = obj.callType;
            return PartialView("_LeadersWalletPayBack", obj);
        }

        [HttpPost]
        public ActionResult AcceptPayBack(WalletPayBackViewModel obj)
        {
            if (obj != null)
            {
                if (obj.list.Any(p => p.IsChecked == true))
                {
                    foreach (var item in obj.list.Where(p => p.IsChecked))
                    {
                        if (!db.LeadersWalletPayBacks.Any(p => p.MLMWalletTransactionId == item.MLMWalletTransactionId))
                        {
                            if (obj.callType == 0)
                            {
                                LeadersWalletPayBack objLeadersWalletPayBack = new LeadersWalletPayBack();
                                objLeadersWalletPayBack.MLMWalletTransactionId = item.MLMWalletTransactionId;
                                objLeadersWalletPayBack.Status = 1;//Accept
                                objLeadersWalletPayBack.ReturnAmount = item.WalletAmountUsed;
                                objLeadersWalletPayBack.CreateDate = DateTime.Now;
                                objLeadersWalletPayBack.CreateBy = Convert.ToInt64(Session["ID"]);
                                objLeadersWalletPayBack.NetworkIP = CommonFunctions.GetClientIP();
                                db.LeadersWalletPayBacks.Add(objLeadersWalletPayBack);
                                SendNotification(item.UserLoginID, item.MLMWalletTransactionId, obj.callType);
                                db.SaveChanges();
                                ViewBag.Message = "Request Accepted Succesfully!";
                            }
                        }
                        else
                        {
                            if (obj.callType == 1)
                            {
                                LeadersWalletPayBack objLeadersWalletPayBack = db.LeadersWalletPayBacks.FirstOrDefault(p => p.MLMWalletTransactionId == item.MLMWalletTransactionId);
                                objLeadersWalletPayBack.Status = 2;
                                objLeadersWalletPayBack.ModifyDate = DateTime.Now;
                                objLeadersWalletPayBack.ModifyBy = Convert.ToInt64(Session["ID"]);

                                MLMWalletTransaction objMLMWalletTransaction = db.MLMWalletTransactions.FirstOrDefault(p => p.ID == objLeadersWalletPayBack.MLMWalletTransactionId);
                                MLMCoinRate objMLMCoinRate = db.MLMCoinRates.FirstOrDefault(p => p.ID == objMLMWalletTransaction.MLMCoinRateID);

                                MLMWallet objMLMWallet = db.MLMWallets.FirstOrDefault(p => p.UserLoginID == item.UserLoginID);
                                objMLMWallet.Amount = objMLMWallet.Amount + objLeadersWalletPayBack.ReturnAmount;
                                objMLMWallet.Points = objMLMWallet.Points + (objLeadersWalletPayBack.ReturnAmount / (decimal)objMLMCoinRate.Rate);
                                db.SaveChanges();
                                SendNotification(item.UserLoginID, item.MLMWalletTransactionId, obj.callType);
                                ViewBag.Message = "Request Approved Succesfully!";
                            }
                        }
                    }

                }
            }

            List<LeadersWalletPayBackOrderDataViewModel> list = new List<LeadersWalletPayBackOrderDataViewModel>();
            list = GetList(obj.FromDate, obj.ToDate, obj.callType);
            obj.list = list;
            ViewBag.StatusHeading = obj.callType;
            return PartialView("_LeadersWalletPayBack", obj);
        }

        public ActionResult ExportToExcel(string fromdt, string todt, int Type)
        {
            var gv = new GridView();
            string FileName = "";

            if (Type == 0)
            {
                FileName = "Leaders Wallet Pay Back Pending Order Report";
            }
            else if (Type == 1)
            {
                FileName = "Leaders Wallet Pay Back Accepted Order Report";
            }
            else
            {
                FileName = "Leaders Wallet Pay Back Approved Order Report";
            }

            List<LeadersWalletPayBackOrderDataViewModel> list = new List<LeadersWalletPayBackOrderDataViewModel>();
            DateTime fdt = Convert.ToDateTime(fromdt);
            DateTime tdt = Convert.ToDateTime(todt);
            list = GetList(fdt, tdt, Type);

            gv.DataSource = list.Select(p => new { p.OrderCode, p.Name, p.MobilNo, p.OrderAmount, p.RetailPoints, p.WalletAmountUsed, p.Status, p.TranDate, p.ReturnAmount });
            gv.DataBind();

            ViewBag.StatusHeading = Type;
            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename=" + FileName + ".xls");
            Response.ContentType = "application/ms-excel";
            Response.Charset = "";
            StringWriter objStringWriter = new StringWriter();
            HtmlTextWriter objHtmlTextWriter = new HtmlTextWriter(objStringWriter);
            gv.RenderControl(objHtmlTextWriter);

            Response.Output.Write(objStringWriter.ToString());
            Response.Flush();
            Response.End();
            return RedirectToAction("Index");
        }



        public void SendSMS(string UserName, string OrderCode, decimal? WalAmt, string MobileNo, int Type)
        {
            try
            {
                Dictionary<string, string> dictSMSValues = new Dictionary<string, string>();
                dictSMSValues.Add("#--NAME--#", UserName);
                dictSMSValues.Add("#--OrderCode--#", OrderCode);
                dictSMSValues.Add("#--WalAmount--#", WalAmt.ToString());

                BusinessLogicLayer.GateWay gateWay = new BusinessLogicLayer.SMS(System.Web.HttpContext.Current.Server);
                if (Type == 0)
                {
                    gateWay.SendSMS(BusinessLogicLayer.GateWay.SMSGateWays.SUMIT, BusinessLogicLayer.GateWay.SMSOptions.MULTIPLE, BusinessLogicLayer.GateWay.SMSTypes.LEADERS_WALLET_PAYBACK_ACCEPT, new string[] { MobileNo }, dictSMSValues);
                }
                else
                {
                    gateWay.SendSMS(BusinessLogicLayer.GateWay.SMSGateWays.SUMIT, BusinessLogicLayer.GateWay.SMSOptions.MULTIPLE, BusinessLogicLayer.GateWay.SMSTypes.LEADERS_WALLET_PAYBACK_APPROVE, new string[] { MobileNo }, dictSMSValues);
                }
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[PaymentProcessController][M:SendSMSToCustomer]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Gandhibagh, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[PaymentProcessController][M:SendSMSToCustomer]",
                    BusinessLogicLayer.ErrorLog.Module.Gandhibagh, System.Web.HttpContext.Current.Server);
            }
        }
        public void SendEmail(string UserName, string OrderCode, decimal? WalAmt, string EmailId, int Type)
        {
            try
            {
                Dictionary<string, string> dictEmailValues = new Dictionary<string, string>();
                dictEmailValues.Add("<!--NAME-->", UserName);
                dictEmailValues.Add("<!--OrderCode-->", OrderCode);
                dictEmailValues.Add("<!--WalAmount-->", WalAmt.ToString());

                BusinessLogicLayer.GateWay gateWay = new BusinessLogicLayer.Email(System.Web.HttpContext.Current.Server);
                if (Type == 0)
                {
                    gateWay.SendEmail(BusinessLogicLayer.GateWay.EmailGateWays.GANDHIBAGH, BusinessLogicLayer.GateWay.SenderMail.INFO, BusinessLogicLayer.GateWay.EMailTypes.LEADERS_WALLET_PAYBACK_ACCEPT, new string[] { EmailId, "sales@ezeelo.com" }, dictEmailValues, true);
                }
                else
                {
                    gateWay.SendEmail(BusinessLogicLayer.GateWay.EmailGateWays.GANDHIBAGH, BusinessLogicLayer.GateWay.SenderMail.INFO, BusinessLogicLayer.GateWay.EMailTypes.LEADERS_WALLET_PAYBACK_APPROVE, new string[] { EmailId, "sales@ezeelo.com" }, dictEmailValues, true);
                }

            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[LeadersWalletPayBackController][M:SendEmail]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.BussinessLogicLayer, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[LeadersWalletPayBackController][M:SendEmail]",
                    BusinessLogicLayer.ErrorLog.Module.BussinessLogicLayer, System.Web.HttpContext.Current.Server);
            }
        }
        public void SendNotification(long UserLoginId, long TranId, int Type)
        {
            string UserName = "";
            string EmailId = "";
            string Mobile = "";
            string OrderCode = "";
            decimal? WalAmt = 0;

            var result = db.UserLogins.Where(p => p.ID == UserLoginId)
                .Join(db.PersonalDetails, U => U.ID, P => P.UserLoginID, (U, P) => new { U, P })
                .Join(db.MLMWalletTransactions.Where(M => M.ID == TranId), P => P.P.UserLoginID, M => M.UserLoginID, (P, M) => new { P, M })
                .Join(db.CustomerOrders, P => P.M.CustomerOrderID, C => C.ID, (P, C) => new { P, C })
                .Select(p => new
                {
                    Name = p.P.P.P.FirstName,
                    EmailId = p.P.P.U.Email,
                    Mobile = p.P.P.U.Mobile,
                    OrderCode = p.C.OrderCode,
                    WalAmt = p.P.M.WalletAmountUsed
                })
                .ToList();

            if (result != null)
            {
                UserName = result.First().Name;
                EmailId = result.First().EmailId;
                Mobile = result.First().Mobile;
                OrderCode = result.First().OrderCode;
                WalAmt = result.First().WalAmt;
            }
            SendEmail(UserName, OrderCode, WalAmt, EmailId, Type);
            SendSMS(UserName, OrderCode, WalAmt, Mobile, Type);
            AddNotification(UserName, OrderCode, WalAmt, Type, UserLoginId);
        }

        public void AddNotification(string UserName, string OrderCode, decimal? WalAmt, int type,long UserLoginId)
        {
            LeadersNotification obj = new LeadersNotification();
            if (type == 0)
            {
                obj.Body = "Refund of Rs. " + WalAmt + " will be credited in next 24 hrs in Ezee Money Wallet against Cancellation/Return of order no. " + OrderCode + ". Keep shopping www.ezeelo.com";
                obj.Title = "Refund Request Initiated";
            }
            else
            {
                obj.Body = "Refund of Rs. " + WalAmt + " credited in Ezee Money Wallet against Cancellation/Return of order no. " + OrderCode + ". Use it now, Click on Hotdeals for best offers www.ezeelo.com";
                obj.Title = "Ezee Money Amount Refunded";
            }
            obj.IsRead = false;
            obj.UserLoginId = UserLoginId;
            obj.CreateDate = DateTime.Now;
            db.LeadersNotifications.Add(obj);
            db.SaveChanges();
        }

    }
}