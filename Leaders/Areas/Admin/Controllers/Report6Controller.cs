using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ModelLayer.Models;
using ModelLayer.Models.ViewModel;
using System.Web.UI.WebControls;
using System.IO;
using System.Web.UI;
using Leaders.Filter;

namespace Leaders.Areas.Admin.Controllers
{
    [AdminSessionExpire]
    public class Report6Controller : Controller
    {
        private EzeeloDBContext db = new EzeeloDBContext();

        public ActionResult partialReport6()
        {

            DateTime now = DateTime.Now;
            var startDate = new DateTime(now.Year, now.Month, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);   // for select data from current Month

            List<Report6ViewModel> reportList = db.Database.SqlQuery<Report6ViewModel>("EXEC Leaders_Report6Select").ToList<Report6ViewModel>().ToList();

            var count = reportList.Count();

            foreach (var item in reportList)
            {
                long UserLoginID = db.CustomerOrders.Where(x => x.OrderCode == item.OrderCode).Select(y => y.UserLoginID).FirstOrDefault();

                decimal? WalletAmountUsed = db.CustomerOrders.Where(x => x.MLMAmountUsed >= 0 && x.OrderCode == item.OrderCode).Select(y => y.MLMAmountUsed).FirstOrDefault();

                // decimal? WalletAmntUsed =db.CustomerOrders.GroupBy(x=>x.UserLoginID).Select{ y => y.Key, value }

                item.MlmAmountUsed = WalletAmountUsed;
            }

            return PartialView("_partialReport6", reportList);

            

        }

        public ActionResult GetList()
        {
            return View();
        }
        public ActionResult FilterDateList(string startDate, string endDate)
        {
            DateTime sDate = Convert.ToDateTime(startDate);
            DateTime eDate = Convert.ToDateTime(endDate);

            if (startDate == endDate)
            {
                var SDate = new DateTime(sDate.Year, sDate.Month, sDate.Day, sDate.Hour, sDate.Minute, 1);
                var EDate = SDate.AddDays(1).AddMinutes(-1);

                var reportListNew = db.Database.SqlQuery<Report6ViewModel>("EXEC Leaders_Report6Select").ToList<Report6ViewModel>().Where(x => x.OrderDate >= SDate && x.OrderDate <= EDate).ToList();
                return PartialView("_partialReport6", reportListNew);
            }


            var reportList = db.Database.SqlQuery<Report6ViewModel>("EXEC Leaders_Report6Select").ToList<Report6ViewModel>().Where(x => x.OrderDate >= sDate && x.OrderDate <= eDate).ToList();

            foreach (var item in reportList)
            {
                long UserLoginID = db.CustomerOrders.Where(x => x.OrderCode == item.OrderCode).Select(y => y.UserLoginID).FirstOrDefault();

                decimal? WalletAmountUsed = db.CustomerOrders.Where(x => x.MLMAmountUsed >= 0 && x.OrderCode == item.OrderCode).Select(y => y.MLMAmountUsed).FirstOrDefault();

                // decimal? WalletAmntUsed =db.CustomerOrders.GroupBy(x=>x.UserLoginID).Select{ y => y.Key, value }
                item.MlmAmountUsed = WalletAmountUsed;
            }

            return PartialView("_partialReport6", reportList);

        }

        public ActionResult ExportToExcel(string toDate, string fromDate)
        {
           // List<ExcelReport6ViewModel> excelReportList = new List<ExcelReport6ViewModel>();
            List<Report6ViewModel> reportList = db.Database.SqlQuery<Report6ViewModel>("EXEC Leaders_Report6Select").ToList<Report6ViewModel>();
            if (toDate != "" && fromDate != "")
            {
                DateTime sDate = Convert.ToDateTime(fromDate);

                DateTime eDate = Convert.ToDateTime(toDate);

                if (toDate == fromDate)
                {
                    var SDate = new DateTime(sDate.Year, sDate.Month, sDate.Day, sDate.Hour, sDate.Minute, 1);
                    var EDate = SDate.AddDays(1).AddMinutes(-1);

                    reportList = reportList.Where(x => x.OrderDate >= SDate && x.OrderDate <= EDate).ToList();

                }

                reportList = reportList.Where(x => x.OrderDate >= sDate && x.OrderDate <= eDate).ToList();
            }
            //foreach (var item in reportList)
            //{
            //    excelReportList.Add(

            //        new ExcelReport6ViewModel
            //        {

            //            City = item.City,
            //            Current_Level_User = item.Current_Level_User,
            //            Customer = item.Customer,
            //            DeliveryDate = item.DeliveryDate,
            //            DeviceType = item.DeviceType,
            //            EzeeMoney = item.EzeeMoney,
            //            //IsActive = item.IsActive,
            //            // IsMLMUser = item.IsMLMUser,
            //            Level1_User = item.Level1_User,
            //            Level2_User = item.Level2_User,
            //            Level3_User = item.Level3_User,
            //            Level4_User = item.Level4_User,
            //            Mobile = item.Mobile,
            //            OrderCode = item.OrderCode,
            //            OrderDate = item.OrderDate,
            //            Parent = item.Parent,
            //            PayableAmount = item.PayableAmount,
            //            PaymentMode = item.PaymentMode,
            //            PincodeID = item.PincodeID,
            //            ReferBy = item.ReferBy,
            //            ReferralID = item.ReferralID,
            //            RP_Distribution_Level0 = item.RP_Distribution_Level0,
            //            RP_Distribution_Level1 = item.RP_Distribution_Level1,
            //            RP_Distribution_Level2 = item.RP_Distribution_Level2,
            //            RP_Distribution_Level3 = item.RP_Distribution_Level3,
            //            RP_Distribution_Level4 = item.RP_Distribution_Level4,
            //            RPEarned_Order = item.RPEarned_Order,
            //            ShippingAddress = item.ShippingAddress,
            //            Status = item.Status
            //        }


            //);
            //    if (item.IsMLMUser == true)
            //    {
            //        foreach (var list in excelReportList)
            //        {
            //            list.CheckMLMUser="TRUE";

            //        }
            //    }
            //    else
            //    {
            //        foreach (var list in excelReportList)
            //        {
            //            list.CheckMLMUser = "FALSE";

            //        }
            //    }
            //    if (item.IsActive == true)
            //    {
            //        foreach (var list in excelReportList)
            //        {
            //            list.CheckActiveUser = "TRUE";

            //        }
            //    }
            //    else
            //    {
            //        foreach (var list in excelReportList)
            //        {
            //            list.CheckActiveUser = "FALSE";

            //        }
            //    }

            foreach (var item in reportList)
            {
                if (item.IsMLMUser == true)
                {
                    item.CheckMLMUser = "TRUE";
                }
                else
                {
                    item.CheckMLMUser = "FALSE";
                }
                if (item.IsActive == false)
                {
                    item.CheckActiveUser = "FALSE";
                }
                else
                {
                    item.CheckActiveUser = "TRUE";
                }
            }

            var gv = new GridView();
            gv.DataSource = reportList;
            gv.DataBind();
            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename=Report6Excel.xls");
            Response.ContentType = "application/ms-excel";
            Response.Charset = "";
            StringWriter objStringWriter = new StringWriter();
            HtmlTextWriter objHtmlTextWriter = new HtmlTextWriter(objStringWriter);
            gv.RenderControl(objHtmlTextWriter);
            Response.Output.Write(objStringWriter.ToString());
            Response.Flush();
            Response.End();
            return View("GetList");
        }
    }
}
