using BusinessLogicLayer;
using ClosedXML.Excel;
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
    public class CashbackPointPayoutController : Controller
    {
        private EzeeloDBContext db = new EzeeloDBContext();
        public ActionResult Index()
        {
            CashbackPointsPayoutViewModel obj = new CashbackPointsPayoutViewModel();
            obj.PayoutDateFilter = new SelectList(db.EzeeMoneyPayouts.Where(p => p.Id >16 && !db.cashbackPointsPayouts.Select(m => m.EzeeMoneyPayoutID).Contains(p.Id)).
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
            obj.OnlyActiveUser = false;
            return View(obj);
        }
        public ActionResult GetPayoutDetails(long EzeeMoneyPayoutId, int ActiveUser)
        {
            CashbackPointsPayoutViewModel obj = new CashbackPointsPayoutViewModel();
            obj = getData(EzeeMoneyPayoutId, ActiveUser, 0);
            return PartialView("_GetAllData", obj);
        }
        public CashbackPointsPayoutViewModel getData(long EzeMoneyPayoutId, int ActiveUser, int Pay)
        {
            BoosterPlanData BD = new BoosterPlanData();
            List<SqlParameter> sp = new List<SqlParameter>()
            {
                new SqlParameter() {ParameterName = "@EzeeMoneyPayoutId", SqlDbType = SqlDbType.BigInt, Value= EzeMoneyPayoutId},
                 new SqlParameter() {ParameterName = "@ActiveUser", SqlDbType = SqlDbType.Int, Value= ActiveUser},
                new SqlParameter() {ParameterName = "@Pay", SqlDbType = SqlDbType.Int, Value= Pay}
            };
            DataSet ds = new DataSet();
            ds = BD.GetData("GetCahbackPointsForPayout", sp);
            CashbackPointsPayoutViewModel obj = new CashbackPointsPayoutViewModel();
            List<Orderwise> orderwise = new List<Orderwise>();
            List<Userwise> Userwise = new List<Userwise>();
            if (ds.Tables.Count > 0)
            {
                ds.Tables[0].TableName = "Orderwise CashbackPointsReport";
                orderwise = BusinessLogicLayer.Helper.CreateListFromTable<Orderwise>(ds.Tables[0]);
            }
            if (ds.Tables.Count > 1)
            {
                ds.Tables[1].TableName = "Userwise CashbackPointsReport";
                Userwise = BusinessLogicLayer.Helper.CreateListFromTable<Userwise>(ds.Tables[1]);
            }
           
            Session["CashbackPointPayoutData"] = ds;
            obj.orderwise = orderwise;
            obj.userwise = Userwise;
            return obj;
        }

        public ActionResult ExportToExcel()
        {
            string ReportName = "Cashback Points Payout Report";
            if (Session["CashbackPointPayoutData"] != null)
            {
                DataSet dataSet = (Session["CashbackPointPayoutData"]) as DataSet;
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

        public ActionResult Payout(long EzeeMoneyPayoutId,int ActiveUser)
        {
            if (db.cashbackPointsPayouts.Any(p => p.EzeeMoneyPayoutID == EzeeMoneyPayoutId))
            {
                return Json("Already Payout has been completed for selected Payout Cycle.", JsonRequestBehavior.AllowGet);
            }
            else
            {
                CashbackPointsPayoutViewModel obj = new CashbackPointsPayoutViewModel();
                obj = getData(EzeeMoneyPayoutId, ActiveUser, 1);
                return Json("Cashback Points Payout Done Successfuly!!!", JsonRequestBehavior.AllowGet);
            }
        }
    }
}