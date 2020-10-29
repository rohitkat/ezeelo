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
    public class EzeeloCustomerOrderController : Controller
    {
        private EzeeloDBContext db = new EzeeloDBContext();


        public ActionResult GetList(string startDate, string endDate)
        {

            DateTime startdate = Convert.ToDateTime(startDate);

            DateTime enddate = Convert.ToDateTime(endDate);

           
            if (startDate == endDate)
            {
                var SDate = new DateTime(startdate.Year, startdate.Month, startdate.Day, startdate.Hour, startdate.Minute, 1);
                var EDate = SDate.AddDays(1).AddMinutes(-1);

                var resultNew = GetEzeeloOrderList().Where(x => x.CreateDate >= SDate && x.CreateDate <= EDate).ToList();
                return PartialView("_partialOrderList", resultNew);
            }

            var result = GetEzeeloOrderList().Where(x => x.CreateDate >= startdate && x.CreateDate <= enddate).ToList();
             
            return PartialView("_partialOrderList", result);
         
        }
        public ActionResult Index()
        {

            return View();
        }

        public ActionResult Select()
        {
            return View();
        }

        public ActionResult partialEzeeloOrder()
        {
            DateTime now = DateTime.Now;
            var startDate = new DateTime(now.Year, now.Month, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);

            var ezeeloOrderList = GetEzeeloOrderList().Where(x => x.CreateDate >= startDate && x.CreateDate <= endDate).ToList();
            return PartialView("_partialOrderList", ezeeloOrderList);
        }

        public ActionResult CustomerOrderList()
        {

            var JsonResult = Json(new { data = GetEzeeloOrderList() }, JsonRequestBehavior.AllowGet);
            JsonResult.MaxJsonLength = int.MaxValue;
            return JsonResult;
           // return Json(new { data = GetEzeeloOrderList() }, JsonRequestBehavior.AllowGet);
        }
        public IList<CustomerOrderViewModel> GetEzeeloOrderList()
        {
            List<CustomerOrderViewModel> OrederList = new List<CustomerOrderViewModel>();

           
            OrederList = db.UserLogins.Join(db.PersonalDetails, u => u.ID, p => p.UserLoginID,
                (u, p) => new
                {
                    UserID = u.ID,
                    FirstName = p.FirstName,
                   // MiddelName = (p.MiddleName==null?"": p.MiddleName),
                    LastName = (p.LastName==null?"":p.LastName),
                    EmailId = u.Email,
                    // Mobile=u.Mobile
                }
                )
                .Join(db.CustomerOrders, d => d.UserID, c => c.UserLoginID,
            (d, c) => new CustomerOrderViewModel
            {

                EmailID = d.EmailId,

                //CustomerOrderID = c.ID,

                FullName = d.FirstName + " " + d.LastName,


                OrderCode = c.OrderCode,
                OrderAmount = (decimal?)c.OrderAmount ?? 0,
                NoOfPointUsed = c.NoOfPointUsed,
                ValuePerPoint = (decimal?)c.ValuePerPoint ?? 0,
                CoupenCode = c.CoupenCode,
                CoupenAmount = (decimal?)c.CoupenAmount ?? 0,
                PAN = c.PAN,
                PaymentMode = c.PaymentMode,
                PayableAmount = (decimal?)c.PayableAmount ?? 0,
                PrimaryMobile = c.PrimaryMobile,
                SecondoryMobile = c.SecondoryMobile,
                ShippingAddress = c.ShippingAddress,
                BusinessPointsTotal = c.BusinessPointsTotal,
                MLMAmountUsed = (decimal?)c.MLMAmountUsed ?? 0,
                // PincodeID = c.PincodeID,
                // Pincode = c.Pincode.ToString(),
                Pincode = db.Pincodes.FirstOrDefault(p => p.ID == c.PincodeID).Name,
                AreaName = db.Areas.FirstOrDefault(p => p.ID == c.AreaID).Name,
                CreateDate = c.CreateDate
            }).ToList();

          
            return OrederList;
                        
        }

        public ActionResult ExportToExcel(string startDate, string endDate)
        {
            List<CustomerOrderViewModel> custList = GetEzeeloOrderList().ToList();
            List<ExcelEzeeloCustomerOrder> excelCustList = new List<ExcelEzeeloCustomerOrder>();
            if (startDate != "" && endDate != "")
            {
                DateTime toDate = Convert.ToDateTime(startDate);
                DateTime fromDate = Convert.ToDateTime(endDate);

                if (startDate == endDate)
                {
                    var SDate = new DateTime(fromDate.Year, fromDate.Month, fromDate.Day, fromDate.Hour, fromDate.Minute, 1);
                    var EDate = SDate.AddDays(1).AddMinutes(-1);
                    custList.Where(x => x.CreateDate >= SDate && x.CreateDate <= EDate).OrderByDescending(y => y.CreateDate).ToList();

                }
                custList.Where(x => x.CreateDate >= fromDate && x.CreateDate <= toDate).OrderByDescending(y => y.CreateDate).ToList();

            }
           

            foreach (var item in custList)
            {
                excelCustList.Add(new ExcelEzeeloCustomerOrder
                {
                    EmailID=item.EmailID,
                    FullName = item.FullName,
                    OrderCode = item.OrderCode,
                    OrderAmount = item.OrderAmount,


                    PaymentMode = item.PaymentMode,
                    PayableAmount = item.PayableAmount,
                    PrimaryMobile = item.PrimaryMobile,
                    SecondoryMobile = item.SecondoryMobile,
                    ShippingAddress = item.ShippingAddress,

                    Pincode = item.Pincode,
                    AreaName = item.AreaName,
                    CreateDate = item.CreateDate


                });
            }

            var gv = new GridView();
            gv.DataSource = excelCustList;
            gv.DataBind();
            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename=EzeeloCustomerOrderExcel.xls");
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
	}
}