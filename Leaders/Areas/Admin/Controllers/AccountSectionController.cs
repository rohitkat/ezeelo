using Leaders.Filter;
using ModelLayer.Models;
using ModelLayer.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Leaders.Areas.Admin.Controllers
{
    [AdminSessionExpire]
    public class AccountSectionController : Controller
    {
        private EzeeloDBContext db = new EzeeloDBContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult partialReport6()
        {

            DateTime now = DateTime.Now;
            var startDate = new DateTime(now.Year, now.Month, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);   // for select data from current Month

            var reportList = db.Database.SqlQuery<Report6ViewModel>("EXEC Leaders_Report6Select").ToList<Report6ViewModel>().Where(x => x.OrderDate >= startDate && x.OrderDate <= endDate);

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

            return PartialView("_partialReport6", reportList);

        }

        public ActionResult ExportToExcel(string toDate, string fromDate)
        {

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