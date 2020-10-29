using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ModelLayer.Models.ViewModel;
using ModelLayer.Models;
using System.Web.UI.WebControls;
using System.IO;
using System.Web.UI;
using Leaders.Filter;

namespace Leaders.Areas.Admin.Controllers
{
    [AdminSessionExpire]
    public class EzeeloUsersController : Controller
    {
        private EzeeloDBContext db = new EzeeloDBContext();


        public ActionResult Index()
        {

            return View();
        }
        public ActionResult GetList()
        {

            DateTime now = DateTime.Now;
            var startDate = new DateTime(now.Year, now.Month, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);   // For Current Month Records

            List<UserLogin> userList = GetEzeeloUsers().Where(x=>x.CreateDate >=startDate && x.CreateDate <=endDate).ToList();
            return View(userList);
        }

        public ActionResult EzeeloUserList()
        {
           // var ezeeloList = GetEzeeloUsers().ToList();

            return Json(new { data = GetEzeeloUsers().ToList() }, JsonRequestBehavior.AllowGet);
        }

        public IList<UserLogin> GetEzeeloUsers()
        {
           // List<UserViewModel> userList = new List<UserViewModel>();
            List<UserLogin> userList = db.UserLogins.ToList();


            return userList;
        }

        public ActionResult ExportToExcel(string startDate, string endDate)
        {
            //List<CustomerOrderViewModel> custList = GetEzeeloOrderList().ToList();
            List<UserLogin> userList = GetEzeeloUsers().ToList();

            List<ExcelEzeeloUser> excelCustList = new List<ExcelEzeeloUser>();
            if (startDate != "" && endDate != "")
            {
                DateTime toDate = Convert.ToDateTime(startDate);
                DateTime fromDate = Convert.ToDateTime(endDate);

                if (startDate == endDate)
                {
                    var SDate = new DateTime(fromDate.Year, fromDate.Month, fromDate.Day, fromDate.Hour, fromDate.Minute, 1);
                    var EDate = SDate.AddDays(1).AddMinutes(-1);
                    userList.Where(x => x.CreateDate >= SDate && x.CreateDate <= EDate).OrderByDescending(y => y.CreateDate).ToList();

                }
                userList.Where(x => x.CreateDate >= fromDate && x.CreateDate <= toDate).OrderByDescending(y => y.CreateDate).ToList();

            }


            foreach (var item in userList)
            {
                excelCustList.Add(new ExcelEzeeloUser
                {
                    EmailID = item.Email,
                    //FullName = item.FullName,
                    //OrderCode = item.OrderCode,
                    //OrderAmount = item.OrderAmount,


                    //PaymentMode = item.PaymentMode,
                    //PayableAmount = item.PayableAmount,
                    //PrimaryMobile = item.PrimaryMobile,
                    //SecondoryMobile = item.SecondoryMobile,
                    //ShippingAddress = item.ShippingAddress,

                    //Pincode = item.Pincode,
                    //AreaName = item.AreaName,
                    CreateDate = item.CreateDate,
                    Mobile = item.Mobile



                });
            }

            var gv = new GridView();
            gv.DataSource = excelCustList;
            gv.DataBind();
            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename=EzeeloCustomerUserExcel.xls");
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