using BusinessLogicLayer;
using ClosedXML.Excel;
using Leaders.Filter;
using ModelLayer.Models;
using ModelLayer.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Leaders.Areas.Admin.Controllers
{
    [AdminSessionExpire]
    public class MerchantPayoutController : Controller
    {
        private EzeeloDBContext db = new EzeeloDBContext();
        public ActionResult Index()
        {
            MerchantPayoutViewModel obj = new MerchantPayoutViewModel();

            obj.PayoutDateFilter = new SelectList(db.EzeeMoneyPayouts.Where(p => p.Id > 19 && !db.merchantPayouts.Select(m => m.RefEzeeMoneyPayoutId).Contains(p.Id)).
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

            return View(obj);
        }
        public MerchantPayoutViewModel getData(long EzeMoneyPayoutId, int Pay)
        {
            BoosterPlanData BD = new BoosterPlanData();
            List<SqlParameter> sp = new List<SqlParameter>()
            {
                new SqlParameter() {ParameterName = "@EzeeMoneyPayoutId", SqlDbType = SqlDbType.BigInt, Value= EzeMoneyPayoutId},
                new SqlParameter() {ParameterName = "@Pay", SqlDbType = SqlDbType.Int, Value= Pay}
            };
            DataSet ds = new DataSet();
            ds = BD.GetData("MerchantPayoutProcedure", sp);
            MerchantPayoutViewModel obj = new MerchantPayoutViewModel();
            PayoutViewModel Payout = new PayoutViewModel();
            List<MerchantPaoutDetailsViewModel> PayoutDetails = new List<MerchantPaoutDetailsViewModel>();
            List<MerchantTransactionViewModel> Transactions = new List<MerchantTransactionViewModel>();
            if (ds.Tables.Count > 0)
            {
                if (ds.Tables[0].Rows.Count > 0)
                {
                    ds.Tables[0].TableName = "Payout";
                    Payout = BusinessLogicLayer.Helper.CreateItemFromRow<PayoutViewModel>(ds.Tables[0].Rows[0]);
                }
            }
            if (ds.Tables.Count > 1)
            {
                ds.Tables[1].TableName = "PayoutDetails";
                PayoutDetails = BusinessLogicLayer.Helper.CreateListFromTable<MerchantPaoutDetailsViewModel>(ds.Tables[1]);
            }
            if (ds.Tables.Count > 2)
            {
                ds.Tables[2].TableName = "TransactionList";
                Transactions = BusinessLogicLayer.Helper.CreateListFromTable<MerchantTransactionViewModel>(ds.Tables[2]);
            }
            Session["MerchantPayoutData"] = ds;
            obj.payout = Payout;
            obj.PayoutDetails = PayoutDetails;
            obj.TransactionList = Transactions;
            return obj;
        }

        public ActionResult GetPayoutDetails(long EzeeMoneyPayoutId)
        {
            MerchantPayoutViewModel obj = new MerchantPayoutViewModel();
            obj = getData(EzeeMoneyPayoutId, 0);
            return PartialView("_GetAllData", obj);
        }

        public ActionResult ExportToExcel()
        {
            string ReportName = "Merchant Network Payout Report";
            if (Session["MerchantPayoutData"] != null)
            {
                DataSet dataSet = (Session["MerchantPayoutData"]) as DataSet;
                using (XLWorkbook wb = new XLWorkbook())
                {
                    foreach (DataTable dt in dataSet.Tables)
                    {
                        wb.Worksheets.Add(dt);
                    }
                    Response.Clear();
                    Response.Buffer = true;
                    Response.Charset = "";
                    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    Response.AddHeader("content-disposition", "attachment;filename=" + ReportName + ".xlsx");
                    using (MemoryStream MyMemoryStream = new MemoryStream())
                    {
                        wb.SaveAs(MyMemoryStream);
                        MyMemoryStream.WriteTo(Response.OutputStream);
                        Response.Flush();
                        Response.End();
                    }
                }
            }
            return RedirectToAction("Index");
        }

        public ActionResult Payout(long EzeeMoneyPayoutId)
        {
            if (db.merchantPayouts.Any(p => p.RefEzeeMoneyPayoutId == EzeeMoneyPayoutId))
            {
                return Json("Already Payout has been completed for selected Payout Cycle.", JsonRequestBehavior.AllowGet);
            }
            else
            {
                MerchantPayoutViewModel obj = new MerchantPayoutViewModel();
                obj = getData(EzeeMoneyPayoutId, 1);
                return Json("Merchant Network Payout Done Successfuly!!!", JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult MerchantPayoutReport()
        {
            MerchantPayoutViewModel obj = new MerchantPayoutViewModel();

            obj.PayoutDateFilter = new SelectList(db.merchantPayouts.OrderByDescending(p=>p.ID).
               Select(p => new
               {
                   ID = p.ID,
                   FromDate = p.FromDate,
                   ToDate = p.ToDate,
               }).ToList()
               .Select(p => new
               {
                   ID = p.ID,
                   Name = p.FromDate.ToString("dd-MM-yyyy") + " To " + p.ToDate.ToString("dd-MM-yyyy")
               }), "ID", "Name").ToList();

            return View(obj);
        }

        public MerchantPayoutViewModel getPayoutReportData(long MerchantPayoutId)
        {
            BoosterPlanData BD = new BoosterPlanData();
            List<SqlParameter> sp = new List<SqlParameter>()
            {
                new SqlParameter() {ParameterName = "@MerchantPayoutId", SqlDbType = SqlDbType.BigInt, Value= MerchantPayoutId},
            };
            DataSet ds = new DataSet();
            ds = BD.GetData("MerchantPayoutReport", sp);
            MerchantPayoutViewModel obj = new MerchantPayoutViewModel();
            PayoutViewModel Payout = new PayoutViewModel();
            List<MerchantPaoutDetailsViewModel> PayoutDetails = new List<MerchantPaoutDetailsViewModel>();
            List<MerchantTransactionViewModel> Transactions = new List<MerchantTransactionViewModel>();
            if (ds.Tables.Count > 0)
            {
                if (ds.Tables[0].Rows.Count > 0)
                {
                    ds.Tables[0].TableName = "Payout";
                    Payout = BusinessLogicLayer.Helper.CreateItemFromRow<PayoutViewModel>(ds.Tables[0].Rows[0]);
                }
            }
            if (ds.Tables.Count > 1)
            {
                ds.Tables[1].TableName = "PayoutDetails";
                PayoutDetails = BusinessLogicLayer.Helper.CreateListFromTable<MerchantPaoutDetailsViewModel>(ds.Tables[1]);
            }
            if (ds.Tables.Count > 2)
            {
                ds.Tables[2].TableName = "TransactionList";
                Transactions = BusinessLogicLayer.Helper.CreateListFromTable<MerchantTransactionViewModel>(ds.Tables[2]);
            }
            Session["MerchantPayoutReportData"] = ds;
            obj.payout = Payout;
            obj.PayoutDetails = PayoutDetails;
            obj.TransactionList = Transactions;
            return obj;
        }

        public ActionResult GetPayoutReport(long MerchantPayoutId)
        {
            MerchantPayoutViewModel obj = new MerchantPayoutViewModel();
            obj = getPayoutReportData(MerchantPayoutId);
            return PartialView("_GetAllData", obj);
        }
        public ActionResult ExportToExcelReport()
        {
            string ReportName = "Merchant Network Payout Report";
            if (Session["MerchantPayoutReportData"] != null)
            {
                DataSet dataSet = (Session["MerchantPayoutReportData"]) as DataSet;
                using (XLWorkbook wb = new XLWorkbook())
                {
                    foreach (DataTable dt in dataSet.Tables)
                    {
                        wb.Worksheets.Add(dt);
                    }
                    Response.Clear();
                    Response.Buffer = true;
                    Response.Charset = "";
                    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    Response.AddHeader("content-disposition", "attachment;filename=" + ReportName + ".xlsx");
                    using (MemoryStream MyMemoryStream = new MemoryStream())
                    {
                        wb.SaveAs(MyMemoryStream);
                        MyMemoryStream.WriteTo(Response.OutputStream);
                        Response.Flush();
                        Response.End();
                    }
                }
            }
            return RedirectToAction("Index");
        }
    }
}