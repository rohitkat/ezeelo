using BusinessLogicLayer;
using ModelLayer.Models;
using ModelLayer.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Leaders.Areas.Admin.Controllers
{
    public class InActivePointsPayoutController : Controller
    {
        private EzeeloDBContext db = new EzeeloDBContext();
        //Yashaswi For Inactive Point payout 05-06-2019

        public ActionResult SelectPayout()
        {
            EzeeMoneyPayoutViewModel obj = new EzeeMoneyPayoutViewModel();

            obj.PayoutDateFilter = new SelectList(db.EzeeMoneyPayouts.Where(p=>p.IsInactivePaid !=true).
                Select(p => new
                {
                    ID = p.Id,
                    FromDate = p.FromDate,
                    ToDate = p.ToDate,
                }).ToList()
                .Select(p => new
                {
                    ID = p.ID,
                    Name = p.FromDate.ToString("dd-MM-yyyy") + " To " + p.ToDate.ToString("dd-MM-yyyy")
                }), "ID", "Name").ToList();          

            //For Last Payout
            EzeeMoneyPayout objEzeeMoneyPayoutDate = db.EzeeMoneyPayouts.Where(p => p.IsPaid == true && p.IsInactivePaid == true).OrderByDescending(p => p.InactivePaidDate).Take(1).FirstOrDefault();
            if (objEzeeMoneyPayoutDate != null)
            {
                obj.LastFromPayoutDate = objEzeeMoneyPayoutDate.FromDate;
                obj.LastToPayoutDate = objEzeeMoneyPayoutDate.ToDate;
            }


            return View(obj);
        }

        public ActionResult GetReport_InactivePoints(long EzeeMoneyPayoutId, int flag)
        {
            List<InactivePayoutFreezeDataReportViewModel> list = GetData(EzeeMoneyPayoutId, flag);
            return PartialView("_GetAllData", list);
        }

        public List<InactivePayoutFreezeDataReportViewModel> GetData(long EzeeMoneyPayoutId, int flag)
        {
            List<InactivePayoutFreezeDataReportViewModel> list = new List<InactivePayoutFreezeDataReportViewModel>();
            var EzeeMoneyPayoutId_ = new SqlParameter
            {
                ParameterName = "EzeeMoneyPayoutId",
                Value = EzeeMoneyPayoutId
            };
            var flag_ = new SqlParameter
            {
                ParameterName = "flag",
                Value = flag
            };
            list = db.Database.SqlQuery<InactivePayoutFreezeDataReportViewModel>("EXEC InactivePointsPayout @EzeeMoneyPayoutId,@flag", EzeeMoneyPayoutId_, flag_).ToList<InactivePayoutFreezeDataReportViewModel>();
            return list;
        }

        public ActionResult InactivePointsPayout(long EzeeMoneyPayoutId, int flag)
        {
            try
            {
                List<InactivePayoutFreezeDataReportViewModel> list = GetData(EzeeMoneyPayoutId, flag);
                if (list.Count() == 0)
                {
                    return Json("No Record found for Inactive Points Payout!!!", JsonRequestBehavior.AllowGet);
                }
                else
                {
                    foreach (var item in list)
                    {
                        (new SendFCMNotification()).SendNotification("inactive_points_payout", item.UserLoginId);
                    }
                    return Json("Inactive Points Payout Done Successfuly!!!", JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult ExportToExcel(long EzeeMoneyPayoutId)
        {
            List<InactivePayoutFreezeDataReportViewModel> list = GetData(EzeeMoneyPayoutId, 0);
            var gv = new GridView();
            string FileName = "Ezee Money Inactive Points Payout Report";
            EzeeMoneyPayout objEzeeMoneyPayout = db.EzeeMoneyPayouts.FirstOrDefault(p => p.Id == EzeeMoneyPayoutId);

            if (objEzeeMoneyPayout != null && list.Count != 0)
            {
                FileName = FileName + " From " + objEzeeMoneyPayout.FromDate.ToString("dd/MM/yyyy") + " " + objEzeeMoneyPayout.ToDate.ToString("dd/MM/yyyy");
            }

            gv.DataSource = list.Select(p => new { p.UserLoginId, p.Name, p.EmailId, p.PhoneNo, p.InactivePoint, p.EzeeMoney });
            gv.DataBind();


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
            return RedirectToAction("SelectPayout");
        }


        //public ActionResult Index()
        //{
        //    EzeeMoneyPayoutViewModel obj = new EzeeMoneyPayoutViewModel();
        //    obj.FromDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);   //amit
        //    obj.ToDate = obj.FromDate.AddMonths(1).AddDays(-1).AddHours(11).AddMinutes(30); //amit
        //    obj.isFrzDtDisable = true;
        //    obj.isGoDisable = false;
        //    obj.isPayEzMnyDisable = true;

        //    EzeeMoneyPayout objEzeeMoneyPayoutDate = db.EzeeMoneyPayouts.Where(p => p.IsPaid == true && p.IsInactivePaid == true).OrderByDescending(p => p.PaidDate).Take(1).FirstOrDefault();
        //    if (objEzeeMoneyPayoutDate != null)
        //    {
        //        obj.LastFromPayoutDate = objEzeeMoneyPayoutDate.FromDate;
        //        obj.LastToPayoutDate = objEzeeMoneyPayoutDate.ToDate;
        //    }
        //    else
        //    {

        //    }

        //    //Coin Rate
        //    MLMCoinRate objCoin = db.MLMCoinRates.FirstOrDefault(p => p.IsActive == true);
        //    if (objCoin != null)
        //    {
        //        obj.CoinRate = objCoin.Rate;
        //    }

        //    //Check For Freeze Data
        //    EzeeMoneyPayout objEzeeMoneyPayout = db.EzeeMoneyPayouts.FirstOrDefault(p => p.IsPaid == true && p.IsInactivePaid == false);
        //    if (objEzeeMoneyPayout != null)
        //    {
        //        obj.EzeeMoneyPayoutId = objEzeeMoneyPayout.Id;
        //        obj.FromDate = objEzeeMoneyPayout.FromDate;
        //        obj.ToDate = objEzeeMoneyPayout.ToDate;
        //        obj.CoinRate = objEzeeMoneyPayout.CoinRate;
        //        obj.TotalAmount = objEzeeMoneyPayout.TotalAmount;
        //        obj.TotalERP = objEzeeMoneyPayout.TotalERP;
        //        obj.PayableAmount = objEzeeMoneyPayout.PayableAmount;
        //        obj.PayableERP = objEzeeMoneyPayout.PayableERP;
        //        obj.DiffAmount = obj.TotalAmount - obj.PayableAmount;
        //        obj.DiffERP = obj.TotalERP - obj.PayableERP;

        //        obj.isFrzDtDisable = true;
        //        obj.isGoDisable = true;
        //        obj.isPayEzMnyDisable = false;
        //        obj.isGoClick = true;
        //        obj.UserCount = objEzeeMoneyPayout.TotalUserCount;
        //        obj.ActiveUserCount = objEzeeMoneyPayout.ActiveUserCount;
        //        obj.DelOrdCount = objEzeeMoneyPayout.DeliveredOrdCount;
        //    }

        //    Session["InActivePoints"] = "1";

        //    return View(obj);
        //    // return View();
        //}


        //public ActionResult Go(EzeeMoneyPayoutViewModel model)
        //{
        //    EzeeMoneyPayout objEzeeMoneyPayoutDate = db.EzeeMoneyPayouts.Where(p => p.IsPaid == true && p.IsInactivePaid == true).OrderByDescending(p => p.PaidDate).Take(1).FirstOrDefault();
        //    if (objEzeeMoneyPayoutDate != null)
        //    {
        //        if ((model.FromDate <= objEzeeMoneyPayoutDate.FromDate && model.FromDate <= objEzeeMoneyPayoutDate.ToDate) || (model.ToDate <= objEzeeMoneyPayoutDate.FromDate && model.ToDate <= objEzeeMoneyPayoutDate.ToDate))
        //        {
        //            TempData["ErrorMsg"] = "You Cant select this Date, Please Select date greater than Last Payout To date";
        //            return RedirectToAction("Index");
        //        }
        //    }
        //    List<InactivePayoutFreezeDataReportViewModel> list_rep = new List<InactivePayoutFreezeDataReportViewModel>();
        //    model = GetRecord(model.FromDate, model.ToDate, 0, out list_rep);
        //    model.isFrzDtDisable = false;
        //    model.isGoDisable = false;
        //    model.isPayEzMnyDisable = true;
        //    return View("Index", model);
        //}

        //public EzeeMoneyPayoutViewModel GetRecord(DateTime FromDate, DateTime ToDate, int ForPayout_, out List<InactivePayoutFreezeDataReportViewModel> list_rep)
        //{
        //    list_rep = PayoutFreezeData(FromDate, ToDate);
        //    EzeeMoneyPayoutViewModel model = new EzeeMoneyPayoutViewModel();
        //    model.FromDate = FromDate;
        //    //model.ToDate = ToDate.AddMonths(1).AddDays(-1).AddHours(23).AddMinutes(59).AddSeconds(59);
        //    //Count of Order Delivered

        //    model.ToDate = ToDate.AddHours(23).AddMinutes(59).AddSeconds(59); // added by amit

        //    // model.DelOrdCount = list_rep.Sum(p => p.DelOrdCount);
        //    //Coin Rate
        //    MLMCoinRate objCoin = db.MLMCoinRates.FirstOrDefault(p => p.IsActive == true);
        //    if (objCoin != null)
        //    {
        //        model.CoinRate = objCoin.Rate;
        //    }
        //    // model.UserCount = list_rep.Where(p => p.DelOrdCount != 0).Count();
        //    model.ActiveUserCount = list_rep.Where(p => p.Status == true).Count();
        //    model.TotalERP = Math.Round(list_rep.Sum(p => p.ERP), 2);
        //    model.TotalAmount = Math.Round(list_rep.Sum(p => p.EzeeMoney), 2);
        //    model.PayableERP = Math.Round(list_rep.Where(p => p.Status == true).Sum(p => p.ERP), 2);
        //    model.PayableAmount = Math.Round(list_rep.Where(p => p.Status == true).Sum(p => p.EzeeMoney), 2);
        //    model.DiffAmount = Math.Round(model.TotalAmount - model.PayableAmount, 2);
        //    model.DiffERP = Math.Round(model.TotalERP - model.PayableERP, 2);
        //    model.isGoClick = true;
        //    model.ListInactiveEzeeMoney = list_rep.Where(p => p.Status == true).ToList();
        //    model.TotalInActivePoints = Math.Round(list_rep.Sum(x => x.TotalInActivePoints), 2);

        //    //if (ForPayout_ == 1)
        //    //{
        //    //    foreach (var item in list_rep)
        //    //    {
        //    //        var fromDate = new SqlParameter
        //    //        {
        //    //            ParameterName = "DateFrom",
        //    //            Value = model.ToDate
        //    //        };
        //    //        var toDate = new SqlParameter
        //    //        {
        //    //            ParameterName = "DateTo",
        //    //            Value = model.FromDate
        //    //        };
        //    //        var Hour = new SqlParameter
        //    //        {
        //    //            ParameterName = "Hour",
        //    //            Value = WebConfigurationManager.AppSettings["Del_Hour"]
        //    //        };
        //    //        var UserId = new SqlParameter
        //    //        {
        //    //            ParameterName = "UserID",
        //    //            Value = item.UserLoginId
        //    //        };
        //    //        db.Database.ExecuteSqlCommand("EXEC EzeeMoneyPayoutFreezeData @UserID,@Hour,@DateTo,@DateFrom", UserId, Hour, toDate, fromDate);
        //    //    }
        //    //}
        //    return model;
        //}
        //public List<InactivePayoutFreezeDataReportViewModel> PayoutFreezeData(DateTime FromDate, DateTime ToDate)
        //{
        //    List<InactivePayoutFreezeDataReportViewModel> list = new List<InactivePayoutFreezeDataReportViewModel>();
        //    // ToDate = ToDate.AddHours(23).AddMinutes(59).AddSeconds(59);
        //    var EzeeMoneyPayout = db.EzeeMoneyPayouts.Where(x => EntityFunctions.TruncateTime(x.FromDate) == EntityFunctions.TruncateTime(FromDate) && EntityFunctions.TruncateTime(x.ToDate) == EntityFunctions.TruncateTime(ToDate) && x.IsPaid == true && x.IsInactivePaid == null).FirstOrDefault();
        //    List<EzeeMoneyPayoutDetails> PayoutMoneyDetailList = db.EzeeMoneyPayoutDetail.Where(x => x.EzeeMoneyPayoutID == EzeeMoneyPayout.Id).ToList();
        //    if (PayoutMoneyDetailList != null && PayoutMoneyDetailList.Count > 0)
        //    {
        //        var FromDate_1stMonth = FromDate.AddMonths(-1);
        //        var ToDate_1stMonth = ToDate.AddMonths(-1);
        //        var FromDate_2ndMonth = FromDate.AddMonths(-2);
        //        var ToDate_2ndMonth = ToDate.AddMonths(-2);
        //        var FirstMonth_Payout = db.EzeeMoneyPayouts.Where(x => EntityFunctions.TruncateTime(x.FromDate) == EntityFunctions.TruncateTime(FromDate_1stMonth) && EntityFunctions.TruncateTime(x.ToDate) == EntityFunctions.TruncateTime(ToDate_1stMonth) && x.IsPaid == true && x.IsInactivePaid == true).FirstOrDefault();
        //        var SecondMonth_Payout = db.EzeeMoneyPayouts.Where(x => EntityFunctions.TruncateTime(x.FromDate) == EntityFunctions.TruncateTime(FromDate_2ndMonth) && EntityFunctions.TruncateTime(x.ToDate) == EntityFunctions.TruncateTime(ToDate_2ndMonth) && x.IsPaid == true && x.IsInactivePaid == true).FirstOrDefault();
        //        if (ToDate_2ndMonth.Month == 1)
        //        {
        //            SecondMonth_Payout = db.EzeeMoneyPayouts.Where(x => x.Id == 7 && x.IsPaid == true && x.IsInactivePaid == true).FirstOrDefault();
        //        }
        //        List<long> UserLoginIds = PayoutMoneyDetailList.Select(x => x.UserLoginId).ToList();
        //        List<EzeeMoneyPayoutDetails> EzeeMoneyPayoutDetail_1stmonth = db.EzeeMoneyPayoutDetail.Where(x => UserLoginIds.Contains(x.UserLoginId) && x.EzeeMoneyPayoutID == FirstMonth_Payout.Id).ToList();
        //        List<EzeeMoneyPayoutDetails> EzeeMoneyPayoutDetail_2ndmonth = db.EzeeMoneyPayoutDetail.Where(x => UserLoginIds.Contains(x.UserLoginId) && x.EzeeMoneyPayoutID == SecondMonth_Payout.Id).ToList();
        //        //Coin Rate
        //        MLMCoinRate objCoin = db.MLMCoinRates.FirstOrDefault(p => p.IsActive == true);
        //        decimal CoinRate = 0;
        //        if (objCoin != null)
        //        {
        //            CoinRate = objCoin.Rate ?? 0;
        //        }
        //        foreach (var item in PayoutMoneyDetailList)
        //        {
        //            //  decimal inactivePt = EzeeMoneyPayoutDetail_1stmonth.Where(x => x.UserLoginId == item.UserLoginId && x.Status == false).FirstOrDefault(x => x.InActivePoints);
        //            InactivePayoutFreezeDataReportViewModel model = new InactivePayoutFreezeDataReportViewModel();
        //            model.UserLoginId = item.UserLoginId;
        //            model.TotalRetailPoints = item.TotalRetailPoints;
        //            model.Status = item.Status;
        //            model.QRP = item.QRP;
        //            model.PhoneNo = item.PhoneNo;
        //            model.Name = item.Name;
        //            model.EzeeMoney = item.EzeeMoney;
        //            model.ERP = item.ERP;
        //            model.EmailId = item.EmailId;
        //            model.Current_FromPayoutDate = EzeeMoneyPayout.FromDate;
        //            if (EzeeMoneyPayoutDetail_1stmonth.Where(x => x.UserLoginId == item.UserLoginId).ToList().Count > 0)
        //            {
        //                model.FirstMonth_ERP = EzeeMoneyPayoutDetail_1stmonth.Where(x => x.UserLoginId == item.UserLoginId).Select(x => x.ERP).FirstOrDefault();
        //                model.FirstMonth_Status = EzeeMoneyPayoutDetail_1stmonth.Where(x => x.UserLoginId == item.UserLoginId).Select(x => x.Status).FirstOrDefault();
        //                model.FirstMonth_InactiveStatus = EzeeMoneyPayoutDetail_1stmonth.Where(x => x.UserLoginId == item.UserLoginId).Select(x => x.IsInactivePaid).FirstOrDefault();
        //                model.FirstMonth_FromPayoutDate = FirstMonth_Payout.FreezeDate;
        //                model.FirstMonth_InactivePointDate = FirstMonth_Payout.InactivePaidDate;
        //                model.FirstMonth_QRP = EzeeMoneyPayoutDetail_1stmonth.Where(x => x.UserLoginId == item.UserLoginId).Select(x => x.QRP).FirstOrDefault();
        //            }
        //            if (EzeeMoneyPayoutDetail_2ndmonth.Where(x => x.UserLoginId == item.UserLoginId).ToList().Count > 0)
        //            {
        //                model.SecondMonth_ERP = EzeeMoneyPayoutDetail_2ndmonth.Where(x => x.UserLoginId == item.UserLoginId).Select(x => x.ERP).FirstOrDefault();
        //                model.SecondMonth_Status = EzeeMoneyPayoutDetail_2ndmonth.Where(x => x.UserLoginId == item.UserLoginId).Select(x => x.Status).FirstOrDefault();
        //                model.SecondMonth_FromPayoutDate = SecondMonth_Payout.FreezeDate;
        //                model.SecondMonth_InactivePayoutDate = SecondMonth_Payout.InactivePaidDate;
        //                model.SecondMonth_QRP = EzeeMoneyPayoutDetail_2ndmonth.Where(x => x.UserLoginId == item.UserLoginId).Select(x => x.QRP).FirstOrDefault();
        //            }
        //            decimal totalInactivePt = 0;
        //            if (model.Status)
        //            {
        //                if (EzeeMoneyPayout.IsInactivePaid == true && model.FirstMonth_Status == true)
        //                {
        //                    totalInactivePt = 0;
        //                }
        //                else
        //                {
        //                    if (model.FirstMonth_QRP > 0 && model.FirstMonth_Status)
        //                    {
        //                        model.FirstMonth_Status = false;
        //                    }
        //                    if (model.SecondMonth_QRP > 0 && model.SecondMonth_Status)
        //                    {
        //                        model.SecondMonth_Status = false;
        //                    }
        //                    if (model.FirstMonth_Status == false && model.SecondMonth_Status == false)
        //                    {
        //                        totalInactivePt = totalInactivePt + model.FirstMonth_ERP + model.SecondMonth_ERP;
        //                    }
        //                    else if (model.FirstMonth_Status == false && model.SecondMonth_Status == true)
        //                    {
        //                        totalInactivePt = totalInactivePt + model.FirstMonth_ERP;
        //                    }
        //                }
        //            }
        //            else
        //            {
        //                totalInactivePt = model.ERP;
        //                if (EzeeMoneyPayout.IsInactivePaid == true && model.FirstMonth_Status == true)
        //                {
        //                    totalInactivePt = totalInactivePt;
        //                }
        //                else
        //                {
        //                    if (model.FirstMonth_QRP > 0 && model.FirstMonth_Status)
        //                    {
        //                        model.FirstMonth_Status = false;
        //                    }
        //                    if (model.SecondMonth_QRP > 0 && model.SecondMonth_Status)
        //                    {
        //                        model.SecondMonth_Status = false;
        //                    }
        //                    if (model.FirstMonth_Status == false && model.SecondMonth_Status == false)
        //                    {
        //                        totalInactivePt = totalInactivePt + model.FirstMonth_ERP + model.SecondMonth_ERP;
        //                    }
        //                    else if (model.FirstMonth_Status == false && model.SecondMonth_Status == true)
        //                    {
        //                        totalInactivePt = totalInactivePt + model.FirstMonth_ERP;
        //                    }
        //                }

        //            }
        //            model.TotalInActivePoints = totalInactivePt;
        //            model.InActiveMoney = model.TotalInActivePoints * CoinRate;
        //            list.Add(model);
        //            //if(EzeeMoneyPayoutDetail_1stmonth.Where(x=>x.UserLoginId == item.UserLoginId).ToList())
        //        }
        //    }

        //    //DateTime endDate = ToDate.AddMonths(-1).AddHours(23).AddMinutes(59).AddSeconds(59);
        //    //var fromDate = new SqlParameter
        //    //{
        //    //    ParameterName = "DateFrom",
        //    //    Value = endDate
        //    //};
        //    //var toDate = new SqlParameter
        //    //{
        //    //    ParameterName = "DateTo",
        //    //    Value = FromDate
        //    //};
        //    //var Hour = new SqlParameter
        //    //{
        //    //    ParameterName = "Hour",
        //    //    Value = WebConfigurationManager.AppSettings["Del_Hour"]
        //    //};
        //    //list = db.Database.SqlQuery<PayoutFreezeDataReportViewModel>("exec PayoutFreezeDataReport @DateTo,@DateFrom,@Hour", toDate, fromDate, Hour).ToList<PayoutFreezeDataReportViewModel>();

        //    ////amit
        //    //foreach (var item in list)
        //    //{
        //    //    //if(item.Status==false)
        //    //    item.InActivePoints = Convert.ToDecimal(GetInactivePoints(item.UserLoginId, FromDate, ToDate));
        //    //    item.ActivePoints = item.ERP;
        //    //}
        //    //--end--//
        //    return list;
        //}

        //public ActionResult FreezeInActivePoints(DateTime FromDate, DateTime ToDate)
        //{

        //    EzeeMoneyPayout objEzeeMoneyPayout = db.EzeeMoneyPayouts.OrderByDescending(x => x.FreezeDate).FirstOrDefault(p => p.IsPaid == false);

        //    using (TransactionScope tscope = new TransactionScope(TransactionScopeOption.Required,
        //                            new System.TimeSpan(0, 15, 0)))
        //    {

        //        //MLMCoinRate objCoin = db.MLMCoinRates.FirstOrDefault(p => p.IsActive == true);

        //        //decimal CoinRate = Convert.ToDecimal(objCoin.Rate);
        //        int i = 0;
        //        try
        //        {
        //            List<InactivePayoutFreezeDataReportViewModel> list_rep = new List<InactivePayoutFreezeDataReportViewModel>();
        //            EzeeMoneyPayoutViewModel model = GetRecord(FromDate, ToDate, 1, out list_rep);

        //            if (model.ListInactiveEzeeMoney.Count() != 0)
        //            {

        //                //InActivePointsPayout obj = new InActivePointsPayout();
        //                //obj.FromDate = FromDate;
        //                //obj.ToDate = ToDate.AddMonths(1).AddDays(-1).AddHours(23).AddMinutes(59).AddSeconds(59);
        //                //obj.CoinRate = model.CoinRate;

        //                //obj.TotalInActivePoints = model.TotalInActivePoints ?? 0;

        //                //// obj.TotalAmount = model.TotalAmount;
        //                //// obj.TotalERP = model.TotalERP;
        //                //obj.PayableAmount = (model.TotalInActivePoints * CoinRate) ?? 0;
        //                //// obj.PayableERP = model.PayableERP;
        //                //obj.FreezeBy = Convert.ToInt64(Session["ID"]);
        //                //obj.FreezeDate = DateTime.Now;
        //                //obj.IsPaid = false;
        //                //obj.DeliveredOrdCount = model.DelOrdCount;
        //                //obj.ActiveUserCount = model.ActiveUserCount;
        //                //obj.TotalUserCount = model.UserCount;
        //                //obj.NetworkIP = CommonFunctions.GetClientIP();
        //                //db.InActivePointsPayouts.Add(obj);
        //                //db.SaveChanges();



        //                EzeeMoneyPayout obj = db.EzeeMoneyPayouts.Where(x => EntityFunctions.TruncateTime(x.FromDate) == EntityFunctions.TruncateTime(FromDate) && EntityFunctions.TruncateTime(x.ToDate) == EntityFunctions.TruncateTime(ToDate) && x.IsPaid == true && x.IsInactivePaid == null).FirstOrDefault();
        //                obj.TotalInActivePoints = model.TotalInActivePoints ?? 0;
        //                obj.InactiveFreezeDate = DateTime.Now;
        //                obj.IsInactivePaid = false;
        //                //db.EzeeMoneyPayouts.Add(objEzeeMoney);
        //                db.Entry(obj).State = EntityState.Modified;
        //                db.SaveChanges();

        //                List<MLMWalletDetails> MLMWalletDetailsList = db.MLMWalletDetails.Where(x => x.EzeeMoneyPayoutId == obj.Id).ToList();
        //                foreach (var item in model.ListInactiveEzeeMoney)
        //                {
        //                    MLMWalletDetails objMLMWalletDetails = MLMWalletDetailsList.Where(x => x.UserLoginId == item.UserLoginId).FirstOrDefault();
        //                    objMLMWalletDetails.InactivePoint = item.TotalInActivePoints;
        //                    objMLMWalletDetails.InactiveAmount = item.InActiveMoney;
        //                    db.Entry(objMLMWalletDetails).State = EntityState.Modified;
        //                    //db.MLMWalletDetails.Add(objMLMWalletDetails);
        //                    db.SaveChanges();
        //                }
        //                List<EzeeMoneyPayoutDetails> objEzeeMoneyPayoutDetailList = db.EzeeMoneyPayoutDetail.Where(p => p.EzeeMoneyPayoutID == obj.Id && p.IsInactivePaid == false).ToList();
        //                foreach (var item in list_rep)
        //                {
        //                    i = i + 1;

        //                    if (objEzeeMoneyPayoutDetailList.Any(p => p.UserLoginId == item.UserLoginId))
        //                    {
        //                        EzeeMoneyPayoutDetails objEMPD = objEzeeMoneyPayoutDetailList.Where(p => p.UserLoginId == item.UserLoginId).FirstOrDefault();
        //                        //Debug Pt1

        //                        if (objEMPD != null)
        //                        {
        //                            string msg = "debug pt1_" + i + "_" + item.UserLoginId + "_" + DateTime.Now.ToString();
        //                            System.Diagnostics.Debug.WriteLine(msg);
        //                            objEMPD.IsInactivePaid = true;
        //                            objEMPD.InActivePoints = item.TotalInActivePoints;
        //                            objEMPD.InactiveEzeeMoney = item.InActiveMoney;
        //                            db.Entry(objEMPD).State = EntityState.Modified;
        //                            db.SaveChanges();
        //                        }
        //                        //db1.Dispose();
        //                    }
        //                }
        //                // scope.Complete();

        //                //objEzeeMoneyPayout.TotalInActivePoints = list_rep.Sum(y => y.InActivePoints);
        //                //db.SaveChanges();

        //                tscope.Complete();
        //                return Json(obj.Id, JsonRequestBehavior.AllowGet);
        //            }

        //            //if (model.listEzeeMoney.Count() != 0)
        //            //{
        //            //    EzeeMoneyPayout obj = new EzeeMoneyPayout();
        //            //    obj.FromDate = FromDate;
        //            //    obj.ToDate = ToDate.AddMonths(1).AddDays(-1).AddHours(23).AddMinutes(59).AddSeconds(59);
        //            //    obj.CoinRate = model.CoinRate;
        //            //    obj.TotalAmount = model.TotalAmount;
        //            //    obj.TotalERP = model.TotalERP;
        //            //    obj.PayableAmount = model.PayableAmount;
        //            //    obj.PayableERP = model.PayableERP;
        //            //    obj.FreezeBy = Convert.ToInt64(Session["ID"]);
        //            //    obj.FreezeDate = DateTime.Now;
        //            //    obj.IsPaid = false;
        //            //    obj.DeliveredOrdCount = model.DelOrdCount;
        //            //    obj.ActiveUserCount = model.ActiveUserCount;
        //            //    obj.TotalUserCount = model.UserCount;
        //            //    obj.NetworkIP = CommonFunctions.GetClientIP();
        //            //    db.EzeeMoneyPayouts.Add(obj);
        //            //    db.SaveChanges();

        //            //    foreach (var item in model.listEzeeMoney)
        //            //    {
        //            //        MLMWalletDetails objMLMWalletDetails = new MLMWalletDetails();
        //            //        objMLMWalletDetails.EzeeMoneyPayoutId = obj.Id;
        //            //        objMLMWalletDetails.UserLoginId = item.UserLoginId;
        //            //        objMLMWalletDetails.ERP = item.ERP;
        //            //        objMLMWalletDetails.Amount = item.EzeeMoney;
        //            //        db.MLMWalletDetails.Add(objMLMWalletDetails);
        //            //        db.SaveChanges();
        //            //    }

        //            //    foreach (var item in list_rep)
        //            //    {
        //            //        EzeeMoneyPayoutDetails objEMPD = new EzeeMoneyPayoutDetails();
        //            //        if (!db.EzeeMoneyPayoutDetail.Any(p => p.UserLoginId == item.UserLoginId && p.EzeeMoneyPayoutID == obj.Id))
        //            //        {
        //            //            objEMPD.UserLoginId = item.UserLoginId;
        //            //            objEMPD.Name = item.Name;
        //            //            objEMPD.EmailId = item.EmailId;
        //            //            objEMPD.PhoneNo = item.PhoneNo;
        //            //            objEMPD.DelOrdCount = item.DelOrdCount;
        //            //            objEMPD.TotalOrdAmt = item.TotalOrdAmt;
        //            //            objEMPD.TotalRetailPoints = item.TotalRetailPoints;
        //            //            objEMPD.ERP = item.ERP;
        //            //            objEMPD.Status = item.Status;
        //            //            objEMPD.EzeeMoney = item.EzeeMoney;
        //            //            objEMPD.QRP = item.QRP;
        //            //            objEMPD.EzeeMoneyPayoutID = obj.Id;
        //            //            objEMPD.OrdCode = item.OrdCode; //yashaswi 22-1-19
        //            //            objEMPD.TransID = item.TransID; //yashaswi 22-1-19
        //            //            db.EzeeMoneyPayoutDetail.Add(objEMPD);
        //            //            db.SaveChanges();
        //            //        }
        //            //    }
        //            //    tscope.Complete();
        //            //    return Json(obj.Id, JsonRequestBehavior.AllowGet);
        //            //}
        //            else
        //            {
        //                return Json("No Record For Payout", JsonRequestBehavior.AllowGet);
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            tscope.Dispose();
        //            return Json(ex.Message, JsonRequestBehavior.AllowGet);
        //        }
        //    }
        //}

        //public double GetInactivePoints(long UserLoginID, DateTime FromDate, DateTime ToDate)
        //{
        //    try
        //    {
        //        long LoginUserId = 0;
        //        if (UserLoginID != 0)
        //        {
        //            LoginUserId = Convert.ToInt64(UserLoginID);
        //        }

        //        double Result = 0;
        //        try
        //        {
        //            if (FromDate == null || ToDate == null)
        //            {
        //                int year = DateTime.Now.Year;
        //                int month = DateTime.Now.Month;
        //                year = (month == 1) ? (year - 1) : year;
        //                month = (month == 1) ? (12) : month;
        //                // FromDate = new DateTime(year, month, 1);
        //                //ToDate = FromDate.AddMonths(1).AddDays(-1).AddHours(23).AddMinutes(59).AddSeconds(59);
        //            }
        //            var idParam = new SqlParameter
        //            {
        //                ParameterName = "LoginUserId",
        //                Value = LoginUserId
        //            };
        //            var Hour = new SqlParameter
        //            {
        //                ParameterName = "Hour",
        //                Value = WebConfigurationManager.AppSettings["Del_Hour"]
        //            };
        //            var DateFrom = new SqlParameter
        //            {
        //                ParameterName = "DateFrom",
        //                Value = ToDate
        //            };
        //            var DateTo = new SqlParameter
        //            {
        //                ParameterName = "DateTo",
        //                Value = FromDate
        //            };
        //            var TotalPoints = new SqlParameter
        //            {
        //                ParameterName = "TotalPoints",
        //                Direction = ParameterDirection.Output,
        //                DbType = DbType.Decimal,
        //                Precision = 18,
        //                Scale = 4
        //            };
        //            db.Database.ExecuteSqlCommand("Leaders_SingleUser_InactivePoints @LoginUserId ,@DateTo,@DateFrom,@Hour,@TotalPoints output", idParam, DateTo, DateFrom, Hour, TotalPoints);
        //            if (TotalPoints != null)
        //            {
        //                Result = Convert.ToDouble(TotalPoints.Value);
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //        }
        //        return Math.Round(Result, 2);
        //    }
        //    catch
        //    {
        //        return 0;
        //    }
        //}

        //public ActionResult InactivePayoutFreezeData_Report()
        //{
        //    //EzeeMoneyPayout objEzeeMoneyPayout = db.EzeeMoneyPayouts.FirstOrDefault(p => p.IsPaid == false);
        //    List<InactivePayoutFreezeDataReportViewModel> list = new List<InactivePayoutFreezeDataReportViewModel>();
        //    // ToDate = ToDate.AddHours(23).AddMinutes(59).AddSeconds(59);
        //    var EzeeMoneyPayout = db.EzeeMoneyPayouts.Where(x => x.IsPaid == true && x.IsInactivePaid == false).FirstOrDefault();
        //    List<EzeeMoneyPayoutDetails> PayoutMoneyDetailList = db.EzeeMoneyPayoutDetail.Where(x => x.EzeeMoneyPayoutID == EzeeMoneyPayout.Id).ToList();
        //    if (PayoutMoneyDetailList != null && PayoutMoneyDetailList.Count > 0)
        //    {
        //        var FromDate_1stMonth = EzeeMoneyPayout.FromDate.AddMonths(-1);
        //        var ToDate_1stMonth = EzeeMoneyPayout.ToDate.AddMonths(-1);
        //        var FromDate_2ndMonth = EzeeMoneyPayout.FromDate.AddMonths(-2);
        //        var ToDate_2ndMonth = EzeeMoneyPayout.ToDate.AddMonths(-2);
        //        var FirstMonth_Payout = db.EzeeMoneyPayouts.Where(x => EntityFunctions.TruncateTime(x.FromDate) == EntityFunctions.TruncateTime(FromDate_1stMonth) && EntityFunctions.TruncateTime(x.ToDate) == EntityFunctions.TruncateTime(ToDate_1stMonth) && x.IsPaid == true && x.IsInactivePaid == true).FirstOrDefault();
        //        var SecondMonth_Payout = db.EzeeMoneyPayouts.Where(x => EntityFunctions.TruncateTime(x.FromDate) == EntityFunctions.TruncateTime(FromDate_2ndMonth) && EntityFunctions.TruncateTime(x.ToDate) == EntityFunctions.TruncateTime(ToDate_2ndMonth) && x.IsPaid == true && x.IsInactivePaid == true).FirstOrDefault();
        //        if (ToDate_2ndMonth.Month == 1)
        //        {
        //            SecondMonth_Payout = db.EzeeMoneyPayouts.Where(x => x.Id == 7 && x.IsPaid == true && x.IsInactivePaid == true).FirstOrDefault();
        //        }
        //        List<long> UserLoginIds = PayoutMoneyDetailList.Select(x => x.UserLoginId).ToList();
        //        List<EzeeMoneyPayoutDetails> EzeeMoneyPayoutDetail_1stmonth = db.EzeeMoneyPayoutDetail.Where(x => UserLoginIds.Contains(x.UserLoginId) && x.EzeeMoneyPayoutID == FirstMonth_Payout.Id).ToList();
        //        List<EzeeMoneyPayoutDetails> EzeeMoneyPayoutDetail_2ndmonth = db.EzeeMoneyPayoutDetail.Where(x => UserLoginIds.Contains(x.UserLoginId) && x.EzeeMoneyPayoutID == SecondMonth_Payout.Id).ToList();
        //        //Coin Rate
        //        MLMCoinRate objCoin = db.MLMCoinRates.FirstOrDefault(p => p.IsActive == true);
        //        decimal CoinRate = 0;
        //        if (objCoin != null)
        //        {
        //            CoinRate = objCoin.Rate ?? 0;
        //        }
        //        foreach (var item in PayoutMoneyDetailList)
        //        {
        //            //  decimal inactivePt = EzeeMoneyPayoutDetail_1stmonth.Where(x => x.UserLoginId == item.UserLoginId && x.Status == false).FirstOrDefault(x => x.InActivePoints);
        //            InactivePayoutFreezeDataReportViewModel model = new InactivePayoutFreezeDataReportViewModel();
        //            model.UserLoginId = item.UserLoginId;
        //            model.TotalRetailPoints = item.TotalRetailPoints;
        //            model.Status = item.Status;
        //            model.QRP = item.QRP;
        //            model.PhoneNo = item.PhoneNo;
        //            model.Name = item.Name;
        //            model.EzeeMoney = item.EzeeMoney;
        //            model.ERP = item.ERP;
        //            model.EmailId = item.EmailId;
        //            model.Current_FromPayoutDate = EzeeMoneyPayout.FromDate;
        //            if (EzeeMoneyPayoutDetail_1stmonth.Where(x => x.UserLoginId == item.UserLoginId).ToList().Count > 0)
        //            {
        //                model.FirstMonth_ERP = EzeeMoneyPayoutDetail_1stmonth.Where(x => x.UserLoginId == item.UserLoginId).Select(x => x.ERP).FirstOrDefault();
        //                model.FirstMonth_Status = EzeeMoneyPayoutDetail_1stmonth.Where(x => x.UserLoginId == item.UserLoginId).Select(x => x.Status).FirstOrDefault();
        //                model.FirstMonth_InactiveStatus = EzeeMoneyPayoutDetail_1stmonth.Where(x => x.UserLoginId == item.UserLoginId).Select(x => x.IsInactivePaid).FirstOrDefault();
        //                model.FirstMonth_FromPayoutDate = FirstMonth_Payout.FreezeDate;
        //                model.FirstMonth_InactivePointDate = FirstMonth_Payout.InactivePaidDate;
        //                model.FirstMonth_QRP = EzeeMoneyPayoutDetail_1stmonth.Where(x => x.UserLoginId == item.UserLoginId).Select(x => x.QRP).FirstOrDefault();
        //            }
        //            if (EzeeMoneyPayoutDetail_2ndmonth.Where(x => x.UserLoginId == item.UserLoginId).ToList().Count > 0)
        //            {
        //                model.SecondMonth_ERP = EzeeMoneyPayoutDetail_2ndmonth.Where(x => x.UserLoginId == item.UserLoginId).Select(x => x.ERP).FirstOrDefault();
        //                model.SecondMonth_Status = EzeeMoneyPayoutDetail_2ndmonth.Where(x => x.UserLoginId == item.UserLoginId).Select(x => x.Status).FirstOrDefault();
        //                model.SecondMonth_FromPayoutDate = SecondMonth_Payout.FreezeDate;
        //                model.SecondMonth_InactivePayoutDate = SecondMonth_Payout.InactivePaidDate;
        //                model.SecondMonth_QRP = EzeeMoneyPayoutDetail_2ndmonth.Where(x => x.UserLoginId == item.UserLoginId).Select(x => x.QRP).FirstOrDefault();
        //            }
        //            decimal totalInactivePt = 0;
        //            if (model.Status)
        //            {
        //                if (EzeeMoneyPayout.IsInactivePaid == true && model.FirstMonth_Status == true)
        //                {
        //                    totalInactivePt = 0;
        //                }
        //                else
        //                {
        //                    if (model.FirstMonth_QRP > 0 && model.FirstMonth_Status)
        //                    {
        //                        model.FirstMonth_Status = false;
        //                    }
        //                    if (model.SecondMonth_QRP > 0 && model.SecondMonth_Status)
        //                    {
        //                        model.SecondMonth_Status = false;
        //                    }
        //                    if (model.FirstMonth_Status == false && model.SecondMonth_Status == false)
        //                    {
        //                        totalInactivePt = totalInactivePt + model.FirstMonth_ERP + model.SecondMonth_ERP;
        //                    }
        //                    else if (model.FirstMonth_Status == false && model.SecondMonth_Status == true)
        //                    {
        //                        totalInactivePt = totalInactivePt + model.FirstMonth_ERP;
        //                    }
        //                }
        //            }
        //            else
        //            {
        //                totalInactivePt = model.ERP;
        //                if (EzeeMoneyPayout.IsInactivePaid == true && model.FirstMonth_Status == true)
        //                {
        //                    totalInactivePt = totalInactivePt;
        //                }
        //                else
        //                {
        //                    if (model.FirstMonth_QRP > 0 && model.FirstMonth_Status)
        //                    {
        //                        model.FirstMonth_Status = false;
        //                    }
        //                    if (model.SecondMonth_QRP > 0 && model.SecondMonth_Status)
        //                    {
        //                        model.SecondMonth_Status = false;
        //                    }
        //                    if (model.FirstMonth_Status == false && model.SecondMonth_Status == false)
        //                    {
        //                        totalInactivePt = totalInactivePt + model.FirstMonth_ERP + model.SecondMonth_ERP;
        //                    }
        //                    else if (model.FirstMonth_Status == false && model.SecondMonth_Status == true)
        //                    {
        //                        totalInactivePt = totalInactivePt + model.FirstMonth_ERP;
        //                    }
        //                }

        //            }
        //            model.TotalInActivePoints = totalInactivePt;
        //            model.InActiveMoney = model.TotalInActivePoints * CoinRate;
        //            list.Add(model);
        //            //if(EzeeMoneyPayoutDetail_1stmonth.Where(x=>x.UserLoginId == item.UserLoginId).ToList())
        //        }
        //    }
        //    //List<EzeeMoneyPayoutDetails> list_rep = new List<EzeeMoneyPayoutDetails>();
        //    //if (objEzeeMoneyPayout != null)
        //    //{
        //    //    list_rep = db.EzeeMoneyPayoutDetail.Where(p => p.EzeeMoneyPayoutID == objEzeeMoneyPayout.Id).ToList();
        //    //}
        //    return View(list);
        //}

        //public ActionResult ExportToExcel()
        //{
        //    var gv = new GridView();
        //    string FileName = "";
        //    List<InactivePayoutFreezeDataReportViewModel> list = new List<InactivePayoutFreezeDataReportViewModel>();
        //    var EzeeMoneyPayout = db.EzeeMoneyPayouts.Where(x => x.IsPaid == true && x.IsInactivePaid == false).FirstOrDefault();
        //    List<EzeeMoneyPayoutDetails> PayoutMoneyDetailList = db.EzeeMoneyPayoutDetail.Where(x => x.EzeeMoneyPayoutID == EzeeMoneyPayout.Id).ToList();
        //    if (PayoutMoneyDetailList != null && PayoutMoneyDetailList.Count > 0)
        //    {
        //        var FromDate_1stMonth = EzeeMoneyPayout.FromDate.AddMonths(-1);
        //        var ToDate_1stMonth = EzeeMoneyPayout.ToDate.AddMonths(-1);
        //        var FromDate_2ndMonth = EzeeMoneyPayout.FromDate.AddMonths(-2);
        //        var ToDate_2ndMonth = EzeeMoneyPayout.ToDate.AddMonths(-2);
        //        var FirstMonth_Payout = db.EzeeMoneyPayouts.Where(x => EntityFunctions.TruncateTime(x.FromDate) == EntityFunctions.TruncateTime(FromDate_1stMonth) && EntityFunctions.TruncateTime(x.ToDate) == EntityFunctions.TruncateTime(ToDate_1stMonth) && x.IsPaid == true && x.IsInactivePaid == true).FirstOrDefault();
        //        var SecondMonth_Payout = db.EzeeMoneyPayouts.Where(x => EntityFunctions.TruncateTime(x.FromDate) == EntityFunctions.TruncateTime(FromDate_2ndMonth) && EntityFunctions.TruncateTime(x.ToDate) == EntityFunctions.TruncateTime(ToDate_2ndMonth) && x.IsPaid == true && x.IsInactivePaid == true).FirstOrDefault();
        //        if (ToDate_2ndMonth.Month == 1)
        //        {
        //            SecondMonth_Payout = db.EzeeMoneyPayouts.Where(x => x.Id == 7 && x.IsPaid == true && x.IsInactivePaid == true).FirstOrDefault();
        //        }
        //        List<long> UserLoginIds = PayoutMoneyDetailList.Select(x => x.UserLoginId).ToList();
        //        List<EzeeMoneyPayoutDetails> EzeeMoneyPayoutDetail_1stmonth = db.EzeeMoneyPayoutDetail.Where(x => UserLoginIds.Contains(x.UserLoginId) && x.EzeeMoneyPayoutID == FirstMonth_Payout.Id).ToList();
        //        List<EzeeMoneyPayoutDetails> EzeeMoneyPayoutDetail_2ndmonth = db.EzeeMoneyPayoutDetail.Where(x => UserLoginIds.Contains(x.UserLoginId) && x.EzeeMoneyPayoutID == SecondMonth_Payout.Id).ToList();
        //        //Coin Rate
        //        MLMCoinRate objCoin = db.MLMCoinRates.FirstOrDefault(p => p.IsActive == true);
        //        decimal CoinRate = 0;
        //        if (objCoin != null)
        //        {
        //            CoinRate = objCoin.Rate ?? 0;
        //        }
        //        foreach (var item in PayoutMoneyDetailList)
        //        {
        //            //  decimal inactivePt = EzeeMoneyPayoutDetail_1stmonth.Where(x => x.UserLoginId == item.UserLoginId && x.Status == false).FirstOrDefault(x => x.InActivePoints);
        //            InactivePayoutFreezeDataReportViewModel model = new InactivePayoutFreezeDataReportViewModel();
        //            model.UserLoginId = item.UserLoginId;
        //            model.TotalRetailPoints = item.TotalRetailPoints;
        //            model.Status = item.Status;
        //            model.QRP = item.QRP;
        //            model.PhoneNo = item.PhoneNo;
        //            model.Name = item.Name;
        //            model.EzeeMoney = item.EzeeMoney;
        //            model.ERP = item.ERP;
        //            model.EmailId = item.EmailId;
        //            model.Current_FromPayoutDate = EzeeMoneyPayout.FromDate;
        //            if (EzeeMoneyPayoutDetail_1stmonth.Where(x => x.UserLoginId == item.UserLoginId).ToList().Count > 0)
        //            {
        //                model.FirstMonth_ERP = EzeeMoneyPayoutDetail_1stmonth.Where(x => x.UserLoginId == item.UserLoginId).Select(x => x.ERP).FirstOrDefault();
        //                model.FirstMonth_Status = EzeeMoneyPayoutDetail_1stmonth.Where(x => x.UserLoginId == item.UserLoginId).Select(x => x.Status).FirstOrDefault();
        //                model.FirstMonth_InactiveStatus = EzeeMoneyPayoutDetail_1stmonth.Where(x => x.UserLoginId == item.UserLoginId).Select(x => x.IsInactivePaid).FirstOrDefault();
        //                model.FirstMonth_FromPayoutDate = FirstMonth_Payout.FreezeDate;
        //                model.FirstMonth_InactivePointDate = FirstMonth_Payout.InactivePaidDate;
        //                model.FirstMonth_QRP = EzeeMoneyPayoutDetail_1stmonth.Where(x => x.UserLoginId == item.UserLoginId).Select(x => x.QRP).FirstOrDefault();
        //            }
        //            if (EzeeMoneyPayoutDetail_2ndmonth.Where(x => x.UserLoginId == item.UserLoginId).ToList().Count > 0)
        //            {
        //                model.SecondMonth_ERP = EzeeMoneyPayoutDetail_2ndmonth.Where(x => x.UserLoginId == item.UserLoginId).Select(x => x.ERP).FirstOrDefault();
        //                model.SecondMonth_Status = EzeeMoneyPayoutDetail_2ndmonth.Where(x => x.UserLoginId == item.UserLoginId).Select(x => x.Status).FirstOrDefault();
        //                model.SecondMonth_FromPayoutDate = SecondMonth_Payout.FreezeDate;
        //                model.SecondMonth_InactivePayoutDate = SecondMonth_Payout.InactivePaidDate;
        //                model.SecondMonth_QRP = EzeeMoneyPayoutDetail_2ndmonth.Where(x => x.UserLoginId == item.UserLoginId).Select(x => x.QRP).FirstOrDefault();
        //            }
        //            decimal totalInactivePt = 0;
        //            if (model.Status)
        //            {
        //                if (EzeeMoneyPayout.IsInactivePaid == true && model.FirstMonth_Status == true)
        //                {
        //                    totalInactivePt = 0;
        //                }
        //                else
        //                {
        //                    if (model.FirstMonth_QRP > 0 && model.FirstMonth_Status)
        //                    {
        //                        model.FirstMonth_Status = false;
        //                    }
        //                    if (model.SecondMonth_QRP > 0 && model.SecondMonth_Status)
        //                    {
        //                        model.SecondMonth_Status = false;
        //                    }
        //                    if (model.FirstMonth_Status == false && model.SecondMonth_Status == false)
        //                    {
        //                        totalInactivePt = totalInactivePt + model.FirstMonth_ERP + model.SecondMonth_ERP;
        //                    }
        //                    else if (model.FirstMonth_Status == false && model.SecondMonth_Status == true)
        //                    {
        //                        totalInactivePt = totalInactivePt + model.FirstMonth_ERP;
        //                    }
        //                }
        //            }
        //            else
        //            {
        //                totalInactivePt = model.ERP;
        //                if (EzeeMoneyPayout.IsInactivePaid == true && model.FirstMonth_Status == true)
        //                {
        //                    totalInactivePt = totalInactivePt;
        //                }
        //                else
        //                {
        //                    if (model.FirstMonth_QRP > 0 && model.FirstMonth_Status)
        //                    {
        //                        model.FirstMonth_Status = false;
        //                    }
        //                    if (model.SecondMonth_QRP > 0 && model.SecondMonth_Status)
        //                    {
        //                        model.SecondMonth_Status = false;
        //                    }
        //                    if (model.FirstMonth_Status == false && model.SecondMonth_Status == false)
        //                    {
        //                        totalInactivePt = totalInactivePt + model.FirstMonth_ERP + model.SecondMonth_ERP;
        //                    }
        //                    else if (model.FirstMonth_Status == false && model.SecondMonth_Status == true)
        //                    {
        //                        totalInactivePt = totalInactivePt + model.FirstMonth_ERP;
        //                    }
        //                }

        //            }
        //            model.TotalInActivePoints = totalInactivePt;
        //            model.InActiveMoney = model.TotalInActivePoints * CoinRate;
        //            list.Add(model);
        //            //if(EzeeMoneyPayoutDetail_1stmonth.Where(x=>x.UserLoginId == item.UserLoginId).ToList())
        //        }

        //        FileName = "Ezee Money Inactive Point Payout Report From " + EzeeMoneyPayout.FromDate.ToString("dd/MM/yyyy") + " " + EzeeMoneyPayout.ToDate.ToString("dd/MM/yyyy");
        //    }
        //    // List<EzeeMoneyPayoutDetails> list_rep = new List<EzeeMoneyPayoutDetails>();
        //    //if (EzeeMoneyPayout != null)
        //    //{
        //    //    list_rep = db.EzeeMoneyPayoutDetail.Where(p => p.EzeeMoneyPayoutID == objEzeeMoneyPayout.Id).ToList();
        //    //    FileName = "Ezee Money Inactive Point Payout Report From " + EzeeMoneyPayout.FromDate.ToString("dd/MM/yyyy") + " " + EzeeMoneyPayout.ToDate.ToString("dd/MM/yyyy");
        //    //}

        //    gv.DataSource = list.Select(p => new { p.UserLoginId, p.Name, p.EmailId, p.PhoneNo, p.QRP, Status = p.Status.ToString(), TotalInactivePoint = p.TotalInActivePoints, InactivePtToEzeeMoney = p.InActiveMoney, p.ERP, p.EzeeMoney, CurrentPayoutDate = p.Current_FromPayoutDate.ToString(), p.FirstMonth_ERP, FirstMonthStatus = p.FirstMonth_Status.ToString(), FirstMonthPayoutDate = p.FirstMonth_FromPayoutDate.ToString(), FirstMonthInActiveDate = p.FirstMonth_InactivePointDate.ToString(), p.SecondMonth_ERP, SecondMonthStatus = p.SecondMonth_Status.ToString(), SecondMonthPayoutDate = p.SecondMonth_FromPayoutDate.ToString(), SecondMonthInActiveDate = p.SecondMonth_InactivePayoutDate.ToString() }); //Yashaswi 22-1-19 add ord code
        //    gv.DataBind();


        //    Response.ClearContent();
        //    Response.Buffer = true;
        //    Response.AddHeader("content-disposition", "attachment; filename=" + FileName + ".xls");
        //    Response.ContentType = "application/ms-excel";
        //    Response.Charset = "";
        //    StringWriter objStringWriter = new StringWriter();
        //    HtmlTextWriter objHtmlTextWriter = new HtmlTextWriter(objStringWriter);
        //    gv.RenderControl(objHtmlTextWriter);

        //    Response.Output.Write(objStringWriter.ToString());
        //    Response.Flush();
        //    Response.End();
        //    return RedirectToAction("Index");
        //}

        //public ActionResult PayOut_InactivePointEzeeMoney(long EzeeMoneyPayoutId)
        //{
        //    EzeeMoneyPayout objEzeeMoneyPayout = db.EzeeMoneyPayouts.FirstOrDefault(p => p.IsPaid == true && p.IsInactivePaid == false);
        //    if (objEzeeMoneyPayout != null)
        //    {
        //        if (objEzeeMoneyPayout.Id != EzeeMoneyPayoutId)
        //        {
        //            return Json("Ezee Money Inactive Point Payout Freeze Id Does not match with Requested Id", JsonRequestBehavior.AllowGet);
        //        }
        //        else
        //        {
        //            using (TransactionScope tscope = new TransactionScope())
        //            {
        //                try
        //                {
        //                    EzeeMoneyPayout objEzeeMoneyPayout_ = db.EzeeMoneyPayouts.FirstOrDefault(p => p.IsPaid == true && p.Id == EzeeMoneyPayoutId && p.IsInactivePaid == false);
        //                    objEzeeMoneyPayout_.IsInactivePaid = true;
        //                    objEzeeMoneyPayout_.InactivePaidDate = DateTime.Now;
        //                    db.SaveChanges();

        //                    List<MLMWalletDetails> listMLMWalletDetails = db.MLMWalletDetails.Where(p => p.EzeeMoneyPayoutId == EzeeMoneyPayoutId).ToList();
        //                    foreach (var item in listMLMWalletDetails)
        //                    {
        //                        MLMWallet objMLMWallet = db.MLMWallets.FirstOrDefault(p => p.UserLoginID == item.UserLoginId);
        //                        objMLMWallet.Amount = objMLMWallet.Amount + item.InactiveAmount ?? 0;
        //                        objMLMWallet.Points = objMLMWallet.Points + item.InactivePoint ?? 0;
        //                        objMLMWallet.IsActive = true;
        //                        objMLMWallet.LastModifyBy = Convert.ToInt64(Session["ID"]);
        //                        objMLMWallet.LastModifyDate = DateTime.Now;
        //                        db.SaveChanges();
        //                    }

        //                    ////Current Level
        //                    //List<MLMWallet_DirectIncome> listMLMWallet_DirectIncomeCurrent = db.MLMWallet_DirectIncomes.Where(p => p.CurrentLevel_IsPaid == false).ToList();
        //                    //foreach (var item in listMLMWallet_DirectIncomeCurrent)
        //                    //{
        //                    //    MLMWallet_DirectIncome obj_ = db.MLMWallet_DirectIncomes.FirstOrDefault(p => p.ID == item.ID);
        //                    //    obj_.CurrentLevel_IsPaid = true;
        //                    //    obj_.CurrentLevel_PayoutDate = DateTime.Now;
        //                    //    db.SaveChanges();
        //                    //}

        //                    ////UpLine 1
        //                    //List<MLMWallet_DirectIncome> listMLMWallet_DirectIncomeUpLine1 = db.MLMWallet_DirectIncomes.Where(p => p.UpLine1_IsPaid == false).ToList();
        //                    //foreach (var item in listMLMWallet_DirectIncomeUpLine1)
        //                    //{
        //                    //    MLMWallet_DirectIncome obj_ = db.MLMWallet_DirectIncomes.FirstOrDefault(p => p.ID == item.ID);
        //                    //    obj_.UpLine1_IsPaid = true;
        //                    //    obj_.UpLine1_PayoutDate = DateTime.Now;
        //                    //    db.SaveChanges();
        //                    //}

        //                    ////UpLine 2
        //                    //List<MLMWallet_DirectIncome> listMLMWallet_DirectIncomeUpLine2 = db.MLMWallet_DirectIncomes.Where(p => p.UpLine2_IsPaid == false).ToList();
        //                    //foreach (var item in listMLMWallet_DirectIncomeUpLine2)
        //                    //{
        //                    //    MLMWallet_DirectIncome obj_ = db.MLMWallet_DirectIncomes.FirstOrDefault(p => p.ID == item.ID);
        //                    //    obj_.UpLine2_IsPaid = true;
        //                    //    obj_.UpLine2_PayoutDate = DateTime.Now;
        //                    //    db.SaveChanges();
        //                    //}

        //                    ////UpLine 3
        //                    //List<MLMWallet_DirectIncome> listMLMWallet_DirectIncomeUpLine3 = db.MLMWallet_DirectIncomes.Where(p => p.UpLine3_IsPaid == false).ToList();
        //                    //foreach (var item in listMLMWallet_DirectIncomeUpLine3)
        //                    //{
        //                    //    MLMWallet_DirectIncome obj_ = db.MLMWallet_DirectIncomes.FirstOrDefault(p => p.ID == item.ID);
        //                    //    obj_.UpLine3_IsPaid = true;
        //                    //    obj_.UpLine3_PayoutDate = DateTime.Now;
        //                    //    db.SaveChanges();
        //                    //}

        //                    ////UpLine 4
        //                    //List<MLMWallet_DirectIncome> listMLMWallet_DirectIncomeUpLine4 = db.MLMWallet_DirectIncomes.Where(p => p.UpLine4_IsPaid == false).ToList();
        //                    //foreach (var item in listMLMWallet_DirectIncomeUpLine4)
        //                    //{
        //                    //    MLMWallet_DirectIncome obj_ = db.MLMWallet_DirectIncomes.FirstOrDefault(p => p.ID == item.ID);
        //                    //    obj_.UpLine4_IsPaid = true;
        //                    //    obj_.UpLine4_PayoutDate = DateTime.Now;
        //                    //    db.SaveChanges();
        //                    //}
        //                    tscope.Complete();
        //                    return Json(1, JsonRequestBehavior.AllowGet);
        //                }
        //                catch (Exception ex)
        //                {
        //                    tscope.Dispose();
        //                    return Json(ex.Message, JsonRequestBehavior.AllowGet);
        //                }
        //            }
        //        }
        //    }
        //    return View();
        //}

    }
}