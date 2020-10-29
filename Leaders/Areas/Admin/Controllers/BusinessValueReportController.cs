using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ModelLayer.Models;
using ModelLayer.Models.ViewModel;
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;
using Leaders.Filter;

namespace Leaders.Areas.Admin.Controllers
{
    [AdminSessionExpire]
    public class BusinessValueReportController : Controller
    {
        private EzeeloDBContext db = new EzeeloDBContext();

        public ActionResult Index()
        {
            return View();
        }


        public IList<CustomerOrderViewModel> businessValueList()
        {

           List<CustomerOrderViewModel> bList = new List<CustomerOrderViewModel>();

            Decimal objAmount = Convert.ToDecimal(0.066);


            bList = db.MLMUsers.Join(db.CustomerOrders, u => u.UserID, c => c.UserLoginID,
                 (u, c) => new CustomerOrderViewModel
                 {

                     EmailID = db.UserLogins.FirstOrDefault(p => p.ID == c.UserLoginID).Email,
                     CustomerOrderID = c.ID,
                     OrderCode = c.OrderCode,
                     OrderDate = c.CreateDate,
                     OrderAmount = c.OrderAmount,
                     BusinessValue = c.BusinessPointsTotal * objAmount,

                     BusinessPointsTotal=c.BusinessPointsTotal

                 }

                 ).ToList();

               
         return bList.ToList();

        }

        public ActionResult partialBusinessValueList()
        {

            return PartialView("partialBusinessValueList", businessValueList());
        }


        public ActionResult ListBetweenDate(string startDate, string endDate)
        {

            DateTime sDate = Convert.ToDateTime(startDate);
            DateTime eDate = Convert.ToDateTime(endDate);

            if (startDate == endDate)
            {
                var SDate = new DateTime(sDate.Year, sDate.Month, sDate.Day, sDate.Hour, sDate.Minute, 1);
                var EDate = SDate.AddDays(1).AddMinutes(-1);
                List<CustomerOrderViewModel> listOrderNew = businessValueList().Where(x => x.OrderDate >= SDate && x.OrderDate <= EDate).ToList();
                return PartialView("partialBusinessValueList", listOrderNew);
           
            }

            List<CustomerOrderViewModel> listOrder = businessValueList().Where(x => x.OrderDate >= sDate && x.OrderDate <= eDate).ToList();
            return PartialView("partialBusinessValueList", listOrder);
           
        }

        public ActionResult ExportToExcel(string toDate, string fromDate)
        {
           List <ExcelBusinessValueReportViewModel> bussValList = new List<ExcelBusinessValueReportViewModel>();
            var excelList = businessValueList();
            if (toDate != "" && fromDate != "")
            {


                DateTime sDate = Convert.ToDateTime(fromDate);
                DateTime eDate = Convert.ToDateTime(toDate);

                if (toDate == fromDate)
                {
                    var SDate = new DateTime(sDate.Year, sDate.Month, sDate.Day, sDate.Hour, sDate.Minute, 1);
                    var EDate = SDate.AddDays(1).AddMinutes(-1);
                    excelList = businessValueList().Where(x => x.OrderDate >= SDate && x.OrderDate <= EDate).OrderByDescending(y => y.OrderDate).ToList();
                }

                excelList = businessValueList().Where(x => x.OrderDate >= sDate && x.OrderDate <= eDate).OrderByDescending(y => y.OrderDate).ToList();
                
            
            }

            foreach (var item in excelList)
                {
                    bussValList.Add(new ExcelBusinessValueReportViewModel
                    {

                        EmailID = item.EmailID,
                        CustomerOrderID = item.CustomerOrderID,
                        OrderCode = item.OrderCode,
                        OrderDate = item.OrderDate,
                        OrderAmount = item.OrderAmount,
                        BusinessValue = item.BusinessValue,

                        BusinessPointsTotal = item.BusinessPointsTotal

                    });
                }

            var gv = new GridView();
            gv.DataSource = bussValList;
            gv.DataBind();
            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename=BusinessValueOrder.xls");
            Response.ContentType = "application/ms-excel";
            Response.Charset = "";
            StringWriter objStringWriter = new StringWriter();
            HtmlTextWriter objHtmlTextWriter = new HtmlTextWriter(objStringWriter);
            gv.RenderControl(objHtmlTextWriter);
            Response.Output.Write(objStringWriter.ToString());
            Response.Flush();
            Response.End();
           // return RedirectToAction("Select");
            return View();
        }
	}
}