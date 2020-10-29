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
    public class LeadersCustomerOrderController : Controller
    {
        private EzeeloDBContext db = new EzeeloDBContext();
        public ActionResult Index()
        {
            //return PartialView("_partialLeadersOrder");
            return View();
        }

        [HttpGet]
        public ActionResult PartialLeadersOrder()
        {
            DateTime now = DateTime.Now;
            var startDate = new DateTime(now.Year, now.Month, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);

            var leadersList = GetLeadersOrderList().Where(x => x.CreateDate >= startDate && x.CreateDate <= endDate).ToList();
            return PartialView("PartialLeadersOrder", leadersList);

        }


        public ActionResult Select()
        {
            return View();
        }
        public ActionResult ListBetweenDate(string startDate, string endDate)
        {

            DateTime sDate = Convert.ToDateTime(startDate);
            DateTime eDate = Convert.ToDateTime(endDate);


            var SDate = new DateTime(sDate.Year, sDate.Month, sDate.Day, sDate.Hour, sDate.Minute, 1);
            var EDate = SDate.AddDays(1).AddMinutes(-1);

            if (startDate == endDate)
            {
                List<LeadersOrderViewModel> listOrder1 = GetLeadersOrderList().Where(x => x.CreateDate >= SDate && x.CreateDate <= EDate).ToList();
                return PartialView("PartialLeadersOrder", listOrder1);
            }

            List<LeadersOrderViewModel> listOrder = GetLeadersOrderList().Where(x => x.CreateDate >= sDate && x.CreateDate <= eDate).ToList();
            return PartialView("PartialLeadersOrder", listOrder);

        }


        public ActionResult LeadersOrderList()
        {

            return Json(new { data = GetLeadersOrderList() }, JsonRequestBehavior.AllowGet);
        }

        public IList<LeadersOrderViewModel> GetLeadersOrderList()
        {
            List<LeadersOrderViewModel> OrederList = new List<LeadersOrderViewModel>();

            OrederList = db.MLMUsers.Join(db.PersonalDetails, u => u.UserID, p => p.UserLoginID,
                (u, p) => new
                {
                    UserID = u.UserID,
                    FirstName = p.FirstName,
                    MiddelName = p.MiddleName,
                    LastName = (p.LastName == null ? "" : p.LastName),
                    EmailId = db.UserLogins.FirstOrDefault(x => x.ID == p.UserLoginID).Email,
                    // UserID = u.UserID,
                    Ref_Id = u.Ref_Id,
                    Join_date_ref = u.Join_date_ref,
                    Status_ref = u.Status_ref,
                    Activate_date_ref = u.Activate_date_ref,
                    Refered_Id_ref = u.Refered_Id_ref,
                    request = u.request,
                    request_active = u.request_active,

                    // Mobile=u.Mobile
                }
                )
                .Join(db.CustomerOrders, d => d.UserID, c => c.UserLoginID,
            (d, c) => new LeadersOrderViewModel
            {

                EmailID = d.EmailId,

                //CustomerOrderID = c.ID,

                FullName = d.FirstName + " " + d.LastName,

                UserID = d.UserID,
                Ref_Id = d.Ref_Id,
                Join_date_ref = d.Join_date_ref,
                Status_ref = d.Status_ref,
                Activate_date_ref = d.Activate_date_ref,
                Refered_Id_ref = d.Refered_Id_ref,
                request = d.request,
                request_active = d.request_active,

                OrderCode = c.OrderCode,
                OrderAmount = ((decimal?)c.OrderAmount) ?? 0,
                NoOfPointUsed = c.NoOfPointUsed,
                ValuePerPoint = ((decimal?)c.ValuePerPoint) ?? 0,
                CoupenCode = c.CoupenCode,
                CoupenAmount = ((decimal?)c.CoupenAmount) ?? 0,
                PAN = c.PAN,
                PaymentMode = c.PaymentMode,
                PayableAmount = ((decimal?)c.PayableAmount) ?? 0,
                PrimaryMobile = c.PrimaryMobile,
                SecondoryMobile = c.SecondoryMobile,
                ShippingAddress = c.ShippingAddress,
                BusinessPointsTotal = c.BusinessPointsTotal,
                MLMAmountUsed = ((decimal?)c.MLMAmountUsed) ?? 0,
                // PincodeID = c.PincodeID,
                // Pincode = c.Pincode.ToString(),
                Pincode = db.Pincodes.FirstOrDefault(p => p.ID == c.PincodeID).Name,
                AreaName = db.Areas.FirstOrDefault(p => p.ID == c.AreaID).Name,

                CreateDate = c.CreateDate,
                CreateBy = c.CreateBy
            }).ToList();
            return OrederList;

        }

        public ActionResult ExportToExcel(string toDate, string fromDate)
        {
            List<ExcelLeadersOrderViewModel> listLeadersOrder = new List<ExcelLeadersOrderViewModel>();
            List<LeadersOrderViewModel> listOrder = GetLeadersOrderList().ToList();
            if (toDate != "" && fromDate != "")
            {
                DateTime sDate = Convert.ToDateTime(fromDate);
                DateTime eDate = Convert.ToDateTime(toDate);

                if (fromDate == toDate)
                {
                    var SDate = new DateTime(sDate.Year, sDate.Month, sDate.Day, sDate.Hour, sDate.Minute, 1);
                    var EDate = SDate.AddDays(1).AddMinutes(-1);
                    listOrder = GetLeadersOrderList().Where(x => x.CreateDate >= SDate && x.CreateDate <= EDate).ToList();
                }


                listOrder = GetLeadersOrderList().Where(x => x.CreateDate >= sDate && x.CreateDate <= eDate).ToList();

            }

            foreach (var item in listOrder)
            {

                listLeadersOrder.Add(new ExcelLeadersOrderViewModel
                {

                    AreaName = item.AreaName,
                    BusinessPointsTotal = item.BusinessPointsTotal,

                    CreateDate = item.CreateDate,
                    FullName = item.FullName,
                    EmailID = item.EmailID,
                    Join_date_ref = item.Join_date_ref,
                    MLMAmountUsed = item.MLMAmountUsed,

                    OrderAmount = item.OrderAmount,
                    OrderCode = item.OrderCode,
                    PayableAmount = item.PayableAmount,
                    PaymentMode = item.PaymentMode,
                    Pincode = item.Pincode,
                    PrimaryMobile = item.PrimaryMobile,
                    Ref_Id = item.Ref_Id,
                    Refered_Id_ref = item.Refered_Id_ref,
                    SecondoryMobile = item.SecondoryMobile,
                    ShippingAddress = item.ShippingAddress


                });



            }

            var gv = new GridView();

            gv.DataSource = listLeadersOrder;
            gv.DataBind();
            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename=LeadersOrderExcel.xls");
            Response.ContentType = "application/ms-excel";
            Response.Charset = "";
            StringWriter objStringWriter = new StringWriter();
            HtmlTextWriter objHtmlTextWriter = new HtmlTextWriter(objStringWriter);
            gv.RenderControl(objHtmlTextWriter);
            Response.Output.Write(objStringWriter.ToString());
            Response.Flush();
            Response.End();
            return RedirectToAction("Select");
        }
    }
}