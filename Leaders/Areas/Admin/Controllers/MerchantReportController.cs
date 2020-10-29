using BusinessLogicLayer;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ModelLayer.Models.ViewModel;
using ModelLayer.Models;
using ClosedXML.Excel;
using System.IO;
using Leaders.Filter;

namespace Leaders.Areas.Admin.Controllers
{
    [AdminSessionExpire]
    public class MerchantReportController : Controller
    {
        EzeeloDBContext db = new EzeeloDBContext();
        BoosterPlanData BD = new BoosterPlanData();
        public ActionResult TransactionReport()
        {
            MerchantTransactionReport obj = new MerchantTransactionReport();
            List<MerchantTransactionViewModel> list = new List<MerchantTransactionViewModel>();
            obj.FromDate = DateTime.Now.AddDays(-10);
            obj.ToDate = DateTime.Now;
            list = GetTransactionReport(obj.merchantId, obj.FromDate, obj.ToDate);
            obj.MerchantList = new SelectList(db.Merchants.Where(p => p.Status == "Approve")
                .Select(p => new
                {
                    ID = p.Id,
                    Name = p.FranchiseName + "/" + p.ServiceMasterDetail.Name
                }), "ID", "Name").ToList();
            obj.MerchantList = obj.MerchantList.OrderBy(p => p.Text).ToList();
            obj.list = list;
            return View(obj);
        }

        [HttpPost]
        public ActionResult TransactionReport(MerchantTransactionReport obj)
        {
            List<MerchantTransactionViewModel> list = new List<MerchantTransactionViewModel>();
            list = GetTransactionReport(obj.merchantId, obj.FromDate, obj.ToDate);
            obj.MerchantList = new SelectList(db.Merchants.Where(p => p.Status == "Approve")
                .Select(p => new
                {
                    ID = p.Id,
                    Name = p.FranchiseName + "/" + p.ServiceMasterDetail.Name
                }), "ID", "Name").ToList();
            obj.MerchantList = obj.MerchantList.OrderBy(p => p.Text).ToList();
            obj.list = list;
            return View(obj);
        }

        public List<MerchantTransactionViewModel> GetTransactionReport(long? MerchantId, DateTime fromDate, DateTime Todate)
        {
            List<MerchantTransactionViewModel> list = new List<MerchantTransactionViewModel>();
            List<SqlParameter> sp = new List<SqlParameter>()
            {
                new SqlParameter() {ParameterName = "@MerchantId", SqlDbType = SqlDbType.BigInt, Value= MerchantId},
                new SqlParameter() {ParameterName = "@FromDate", SqlDbType = SqlDbType.DateTime, Value= fromDate},
                new SqlParameter() {ParameterName = "@ToDate", SqlDbType = SqlDbType.DateTime, Value= Todate},
            };
            DataSet ds = new DataSet();
            ds = BD.GetData("MerchantTransactionReport", sp);
            Session["TransactionReport"] = ds;
            if (ds.Tables.Count > 0)
            {
                list = BusinessLogicLayer.Helper.CreateListFromTable<MerchantTransactionViewModel>(ds.Tables[0]);
            }
            return list;
        }
        public ActionResult ExportToExcel_TransactionReport()
        {
            string ReportName = "Merchant Transaction Report";
            if (Session["TransactionReport"] != null)
            {
                DataSet dataSet = (Session["TransactionReport"]) as DataSet;
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

        public ActionResult RatingReport()
        {
            MerchantReport obj = new MerchantReport();
            DateTime now = DateTime.Now;
            obj.FromDate = new DateTime(now.Year, now.Month, 1);
            obj.ToDate = obj.FromDate.AddMonths(1).AddDays(-1);
            obj.MerchantList = new SelectList(db.Merchants.Where(p => p.Status == "Approve")
                .Select(p => new
                {
                    ID = p.Id,
                    Name = p.FranchiseName + "/" + p.ServiceMasterDetail.Name
                }), "ID", "Name").ToList();
            obj.MerchantList = obj.MerchantList.OrderBy(p => p.Text).ToList();
            obj.ratingReviewLists = new List<MerchantRatingReviewList>();
            return View(obj);
        }

        [HttpPost]
        public ActionResult RatingReport(MerchantReport obj, string Go, string Save)
        {
            obj.MerchantList = new SelectList(db.Merchants.Where(p => p.Status == "Approve")
                .Select(p => new
                {
                    ID = p.Id,
                    Name = p.FranchiseName + "/" + p.ServiceMasterDetail.Name
                }), "ID", "Name").ToList();
            obj.MerchantList = obj.MerchantList.OrderBy(p => p.Text).ToList();
            if (Save == null)
            {
                obj.ratingReviewLists = GetRatingReport(obj.MerchantID, obj.FromDate, obj.ToDate);
            }
            else
            {
                TempData["Result"] = "No data found to Save";
                foreach (var item in obj.ratingReviewLists.Where(p => p.isdisplay == false))
                {
                    MerchantRating rating = db.MerchantRatings.FirstOrDefault(p => p.CustomerID == item.UserLoginID && p.MerchantID == item.MerchantID);
                    rating.IsDisplay = false;
                    db.SaveChanges();
                    TempData["Result"] = "Save Successfully";
                }
                foreach (var item in obj.ratingReviewLists.Where(p => p.isdisplay == true))
                {
                    MerchantRating rating = db.MerchantRatings.FirstOrDefault(p => p.CustomerID == item.UserLoginID && p.MerchantID == item.MerchantID);
                    rating.IsDisplay = true;
                    db.SaveChanges();
                    TempData["Result"] = "Save Successfully";
                }
            }
            return View(obj);
        }

        public ActionResult ExportToExcel_RatingReport()
        {
            string ReportName = "Merchant Rating-Review Report";
            if (Session["RatingReport"] != null)
            {
                DataSet dataSet = (Session["RatingReport"]) as DataSet;
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
        public List<MerchantRatingReviewList> GetRatingReport(long? MerchantId, DateTime fromDate, DateTime Todate)
        {
            List<MerchantRatingReviewList> list = new List<MerchantRatingReviewList>();
            List<SqlParameter> sp = new List<SqlParameter>()
            {
                new SqlParameter() {ParameterName = "@MerchantId", SqlDbType = SqlDbType.BigInt, Value= MerchantId},
                new SqlParameter() {ParameterName = "@FromDate", SqlDbType = SqlDbType.DateTime, Value= fromDate},
                new SqlParameter() {ParameterName = "@ToDate", SqlDbType = SqlDbType.DateTime, Value= Todate},
            };
            DataSet ds = new DataSet();
            ds = BD.GetData("MerchantRatingReviewList", sp);
            Session["RatingReport"] = ds;
            if (ds.Tables.Count > 0)
            {
                list = BusinessLogicLayer.Helper.CreateListFromTable<MerchantRatingReviewList>(ds.Tables[0]);
            }
            return list;
        }

        public List<MerchantPendingTransaction> GetPendingTransReport(long? MerchantId)
        {
            List<MerchantPendingTransaction> list = new List<MerchantPendingTransaction>();
            List<SqlParameter> sp = new List<SqlParameter>()
            {
                new SqlParameter() {ParameterName = "@MerchantId", SqlDbType = SqlDbType.BigInt, Value= MerchantId},
            };
            DataSet ds = new DataSet();
            ds = BD.GetData("MerchantPendingTransaction", sp);
            Session["PendingTransReport"] = ds;
            if (ds.Tables.Count > 0)
            {
                list = BusinessLogicLayer.Helper.CreateListFromTable<MerchantPendingTransaction>(ds.Tables[0]);
            }
            return list;
        }

        public ActionResult PendingTransactionReport()
        {
            MerchantReport obj = new MerchantReport();
            List<MerchantPendingTransaction> list = new List<MerchantPendingTransaction>();
            list = GetPendingTransReport(null);
            obj.MerchantList = new SelectList(db.Merchants.Where(p => p.Status == "Approve")
                .Select(p => new
                {
                    ID = p.Id,
                    Name = p.FranchiseName + "/" + p.ServiceMasterDetail.Name
                }), "ID", "Name").ToList();
            obj.MerchantList = obj.MerchantList.OrderBy(p => p.Text).ToList();
            obj.pendingTrans = list;
            return View(obj);
        }

        [HttpPost]
        public ActionResult PendingTransactionReport(MerchantReport obj)
        {
            List<MerchantPendingTransaction> list = new List<MerchantPendingTransaction>();
            list = GetPendingTransReport(obj.MerchantID == 0 ? null : obj.MerchantID);
            obj.MerchantList = new SelectList(db.Merchants.Where(p => p.Status == "Approve")
                .Select(p => new
                {
                    ID = p.Id,
                    Name = p.FranchiseName + "/" + p.ServiceMasterDetail.Name
                }), "ID", "Name").ToList();
            obj.MerchantList = obj.MerchantList.OrderBy(p => p.Text).ToList();
            obj.pendingTrans = list;
            return View(obj);
        }

        public ActionResult ExportToExcel_PendinTransactionReport()
        {
            string ReportName = "Merchant Pending Transaction Report";
            if (Session["PendingTransReport"] != null)
            {
                DataSet dataSet = (Session["PendingTransReport"]) as DataSet;
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

        public ActionResult GSTReport()
        {
            GSTReportViewModel GST = new GSTReportViewModel();
            GST.MerchantList = new SelectList(db.Merchants.Where(p => p.ApproveDate != null)
             .Select(p => new
             {
                 ID = p.Id,
                 Name = p.FranchiseName + "/" + p.ServiceMasterDetail.Name
             }), "ID", "Name").ToList();
            GST.MerchantList.Add(new SelectListItem()
            {
                Text = "All",
                Value = "0",
                Selected = true
            });
            GST.MerchantList = GST.MerchantList.OrderBy(p => p.Text).ToList();

            GST.StateList = new SelectList(db.States.Where(s => (db.Merchants.Where(p => p.ApproveDate != null).Select(p => p.State).Distinct()).Contains(s.ID)).OrderBy(s => s.Name).Select(s => new
            {
                ID = s.ID,
                Name = s.Name
            }), "ID", "Name").ToList();


            GST.CityList = new SelectList(db.Cities.Where(s => (db.Merchants.Where(p => p.ApproveDate != null).Select(p => p.City).Distinct()).Contains(s.ID)).OrderBy(s => s.Name).Select(s => new
            {
                ID = s.ID,
                Name = s.Name
            }), "ID", "Name").ToList();

            GST.Year = DateTime.Now.Year;
            GST.Month = DateTime.Now.Month;
            return View(GST);
        }

        [HttpPost]
        public ActionResult GSTReport(GSTReportViewModel GST)
        {
            GST = GetGSTReportData(GST.MerchantID, GST.Month, GST.Year, GST.StateId, GST.CityId);
            if (GST.StateId != null)
            {
                GST.MerchantList = new SelectList(db.Merchants.Where(p => p.ApproveDate != null && p.State == GST.StateId)
                .Select(p => new
                {
                    ID = p.Id,
                    Name = p.FranchiseName + "/" + p.ServiceMasterDetail.Name
                }), "ID", "Name").ToList();
            }
            if (GST.CityId != null && GST.CityId !=0)
            {
                GST.MerchantList = new SelectList(db.Merchants.Where(p => p.ApproveDate != null && p.City == GST.CityId)
                .Select(p => new
                {
                    ID = p.Id,
                    Name = p.FranchiseName + "/" + p.ServiceMasterDetail.Name
                }), "ID", "Name").ToList();
            }
            else
            {
                GST.MerchantList = new SelectList(db.Merchants.Where(p => p.ApproveDate != null)
                .Select(p => new
                {
                    ID = p.Id,
                    Name = p.FranchiseName + "/" + p.ServiceMasterDetail.Name
                }), "ID", "Name").ToList();
            }
          
            GST.MerchantList.Add(new SelectListItem()
            {
                Text = "All",
                Value = "0",
                Selected = true
            });
            GST.MerchantList = GST.MerchantList.OrderBy(p => p.Text).ToList();

            GST.StateList = new SelectList(db.States.Where(s => (db.Merchants.Where(p => p.ApproveDate != null).Select(p => p.State).Distinct()).Contains(s.ID)).OrderBy(s => s.Name).Select(s => new
            {
                ID = s.ID,
                Name = s.Name
            }), "ID", "Name").ToList();

            if (GST.StateId != null)
            {
                List<StateCityFranchiseMerchantViewModel> city = new List<StateCityFranchiseMerchantViewModel>();
                List<long> ldistrict = db.Districts.Where(x => x.StateID == GST.StateId).Select(x => x.ID).ToList();
                city = db.Cities.Where(c => ldistrict.Contains(c.DistrictID) && c.IsActive == true).Select(c => new StateCityFranchiseMerchantViewModel { CityID = c.ID, CityName = c.Name }).ToList();
                city = city.Where(s => (db.Merchants.Where(p => p.ApproveDate != null).Select(p => p.City).Distinct()).Contains(s.CityID)).ToList();

                GST.CityList = new SelectList(city.Select(s => new
                {
                    ID = s.CityID,
                    Name = s.CityName
                }), "ID", "Name").ToList();
            }
            else
            {
                GST.CityList = new SelectList(db.Cities.Where(s => (db.Merchants.Where(p => p.ApproveDate != null).Select(p => p.City).Distinct()).Contains(s.ID)).OrderBy(s => s.Name).Select(s => new
                {
                    ID = s.ID,
                    Name = s.Name
                }), "ID", "Name").ToList();
            }
            return View(GST);
        }
        public JsonResult GetCityList(int stateID)
        {
            List<StateCityFranchiseMerchantViewModel> city = new List<StateCityFranchiseMerchantViewModel>();
            List<long> ldistrict = db.Districts.Where(x => x.StateID == stateID).Select(x => x.ID).ToList();
            if (stateID == 0)
            {
                city = db.Cities.Where(s => (db.Merchants.Where(p => p.ApproveDate != null).Select(p => p.City).Distinct()).Contains(s.ID)).OrderBy(s => s.Name).Select(c => new StateCityFranchiseMerchantViewModel { CityID = c.ID, CityName = c.Name }).ToList();
            }
            else
            {
                city = db.Cities.Where(c => ldistrict.Contains(c.DistrictID) && c.IsActive == true).Select(c => new StateCityFranchiseMerchantViewModel { CityID = c.ID, CityName = c.Name }).ToList();
                city = city.Where(s => (db.Merchants.Where(p => p.ApproveDate != null).Select(p => p.City).Distinct()).Contains(s.CityID)).ToList();
            }
            return Json(city.Distinct().OrderBy(x => x.CityName).ToList(), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetMerchantListbyState(int StateId)
        {
            List<StateCityFranchiseMerchantViewModel> list = new List<StateCityFranchiseMerchantViewModel>();
            if (StateId == 0)
            {
                list = db.Merchants.Where(p => p.ApproveDate != null).Select(c => new StateCityFranchiseMerchantViewModel { CityID = c.Id, CityName = c.FranchiseName + "/" + c.ServiceMasterDetail.Name }).ToList();
            }
            else
            {
                list = db.Merchants.Where(p => p.ApproveDate != null && p.State == StateId).Select(c => new StateCityFranchiseMerchantViewModel { CityID = c.Id, CityName = c.FranchiseName + "/" + c.ServiceMasterDetail.Name }).ToList();
            }
            return Json(list.Distinct().OrderBy(x => x.CityName).ToList(), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetMerchantListbyCity(int CityID)
        {
            List<StateCityFranchiseMerchantViewModel> list = new List<StateCityFranchiseMerchantViewModel>();
            list = db.Merchants.Where(p => p.ApproveDate != null && p.City == CityID).Select(c => new StateCityFranchiseMerchantViewModel { CityID = c.Id, CityName = c.FranchiseName + "/" + c.ServiceMasterDetail.Name }).ToList();
            return Json(list.Distinct().OrderBy(x => x.CityName).ToList(), JsonRequestBehavior.AllowGet);
        }
        public GSTReportViewModel GetGSTReportData(long MerchantId, int Month, int Year, long? StateId, long? CityId)
        {
            GSTReportViewModel GST = new GSTReportViewModel();
            GST.MerchantID = MerchantId;
            GST.Month = Month;
            GST.Year = Year;
            GST.StateId = StateId;
            GST.CityId = CityId;
            List<MerchantGSTTransaction> list = new List<MerchantGSTTransaction>();
            List<SqlParameter> sp = new List<SqlParameter>()
            {
                new SqlParameter() {ParameterName = "@MerchantID", SqlDbType = SqlDbType.BigInt, Value= MerchantId},
                new SqlParameter() {ParameterName = "@Month", SqlDbType = SqlDbType.Int, Value= Month},
                new SqlParameter() {ParameterName = "@Year", SqlDbType = SqlDbType.Int, Value= Year},
                new SqlParameter() {ParameterName = "@State", SqlDbType = SqlDbType.BigInt, Value= StateId},
                new SqlParameter() {ParameterName = "@City", SqlDbType = SqlDbType.BigInt, Value= CityId},
            };
            DataSet ds = new DataSet();
            ds = BD.GetData("MerchantTransactionGSTReport", sp);
            if (ds.Tables.Count > 0)
            {
                list = BusinessLogicLayer.Helper.CreateListFromTable<MerchantGSTTransaction>(ds.Tables[0]);
            }
            if (MerchantId > 0)
            {
                GST.Merchant = db.Merchants.FirstOrDefault(p => p.Id == MerchantId);
            }
            GST.TransactionList = list;
            return GST;
        }
    }
}