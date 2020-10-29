using Leaders.Filter;
using ModelLayer.Models;
using ModelLayer.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Leaders.Areas.Admin.Controllers
{
    [AdminSessionExpire]
    public class PromotionalERPPayoutReportController : Controller
    {
        private EzeeloDBContext db = new EzeeloDBContext();
        public ActionResult Index()
        {
            PromotionalERPPayoutReportViewModel obj = new PromotionalERPPayoutReportViewModel();
            
            obj.PayoutDateFilter = new SelectList(db.PromotionalERPPayouts.
                Select(p => new
                {
                    Date = p.PaidDate
                }).ToList()
                .Select(p => p.Date).
                Select(p => new
                {
                    ID = p.Value.ToString("dd-MM-yyyy"),
                    Name = p.Value.ToString("dd-MM-yyyy")
                })
               .Distinct().OrderBy(p => p.ID), "ID", "Name").ToList();


            return View(obj);
        }
        public PartialViewResult GetAllGroupData()
        {
            
            PromotionalERPPayoutReportViewModel obj = new PromotionalERPPayoutReportViewModel();
            obj.List = (
                from pd in db.PromotionalERPPayoutDetails
                join p in db.PromotionalERPPayouts on pd.PromotionalERPPayoutId equals p.Id
                join u in db.UserLogins on pd.UserLoginId equals u.ID
                join ud in db.PersonalDetails on u.ID equals ud.UserLoginID
                select new PromotionalERPPayoutDetail_ReportViewModel
                {
                    Id = p.Id,
                    //PaidDate = p.PaidDate.Value.ToString("dd-MM-yyyy"),
                    UserLoginId = pd.UserLoginId,
                    Name = ud.FirstName + " " + ud.LastName,
                    Email = u.Email,
                    Phone = u.Mobile,
                    ERP = pd.ERP,
                    EzeeMoney = pd.EzeeMoney,
                    ReferenceText = p.ReferenceText
                }).ToList();
 

           
            foreach(var item in obj.List)
            {
                item.PaidDate = db.PromotionalERPPayouts.Where(x => x.Id == item.Id).Select(y => y.PaidDate).FirstOrDefault().Value.ToString("dd-MM-yyyy");
            }

            List<PromotionalERPPayout_ReportViewModel> reult = obj.List.GroupBy(p => new { p.UserLoginId }).Select(g => new PromotionalERPPayout_ReportViewModel { UserloginId =g.Key.UserLoginId , listPromotionalERPPayoutDetails = g.OrderBy(y => y.UserLoginId).ToList() }).ToList();
            Session["PromotionalERPPayoutReport"] = reult;
            return PartialView("_GetAllGroupData", reult);
           
        }

        public PartialViewResult GetAllDataByPaidDate(string date)
        {

            PromotionalERPPayoutReportViewModel obj = new PromotionalERPPayoutReportViewModel();
            List<long> ids =db.PromotionalERPPayouts.ToList().Where(a => a.PaidDate.Value.ToString("dd-MM-yyyy") == date).Select(a => a.Id).ToList();
            var abc = db.PromotionalERPPayouts.Where(p => ids.ToList().Contains(p.Id)).ToList();

            obj.List = (
                  from p in db.PromotionalERPPayouts.Where(p => ids.ToList().Contains(p.Id)).ToList()
                  join pd in db.PromotionalERPPayoutDetails on p.Id equals pd.PromotionalERPPayoutId
                  join u in db.UserLogins on pd.UserLoginId equals u.ID
                  join ud in db.PersonalDetails on u.ID equals ud.UserLoginID
                  select new PromotionalERPPayoutDetail_ReportViewModel
                  {
                      Id = p.Id,
                      PaidDate = p.PaidDate.Value.ToString("dd-MM-yyyy"),
                      UserLoginId = pd.UserLoginId,
                      Name = ud.FirstName + " " + ud.LastName,
                      Email = u.Email,
                      Phone = u.Mobile,
                      ERP = pd.ERP,
                      EzeeMoney = pd.EzeeMoney,
                      ReferenceText = p.ReferenceText
                  }).OrderBy(x => x.Id).ToList();
            Session["PromotionalERPPayoutReport"] = obj.List;
            return PartialView("_GetAllData", obj);
        }
        public ActionResult ExportToExcel(int flag)
        {
            long WarehouseID = 0;
            if (Session["WarehouseID"] != null)
            {
                WarehouseID = Convert.ToInt64(Session["WarehouseID"]);
            }
            string FileName = "";
            //Warehouse obj = db.Warehouses.FirstOrDefault(p => p.ID == WarehouseID);
            //if (obj != null)
            //{
            //    FileName = obj.Name;
            //}
            FileName = " PromotionalERPPayoutReport";
            List<PromotionalERPPayoutReportExportToExcel_List> list = new List<PromotionalERPPayoutReportExportToExcel_List>();
            if (Session["PromotionalERPPayoutReport"] != null)
            {
                if(flag==0)
                { 
                List<PromotionalERPPayoutDetail_ReportViewModel> objWRLVM = (List<PromotionalERPPayoutDetail_ReportViewModel>)Session["PromotionalERPPayoutReport"];
                var i = 0;
                if (objWRLVM.Count != 0)
                {

                    foreach (var item in objWRLVM)
                    {
                        PromotionalERPPayoutReportExportToExcel_List o = new PromotionalERPPayoutReportExportToExcel_List();
                        i = i + 1;
                        o.SrNo = i;
                        o.UserloginId = item.UserLoginId;
                        o.PaidDate = item.PaidDate;
                        o.Name = item.Name;
                        o.Email = item.Email;
                        o.Phone = item.Phone;
                        o.ERP = item.ERP;
                        o.EzeeMoney = item.EzeeMoney;
                       
                        o.ReferenceText = item.ReferenceText;
                        list.Add(o);
                    }
                }
                }
                else
                {
                    List<PromotionalERPPayout_ReportViewModel> obj = (List<PromotionalERPPayout_ReportViewModel>)Session["PromotionalERPPayoutReport"];
                    var i = 0;
                    if (obj.Count != 0)
                    {

                        foreach (var item in obj)
                        {
                            PromotionalERPPayoutReportExportToExcel_List o = new PromotionalERPPayoutReportExportToExcel_List();
                            i = i + 1;
                            o.SrNo = i;
                            o.UserloginId = item.listPromotionalERPPayoutDetails.FirstOrDefault().UserLoginId;
                            o.PaidDate = item.listPromotionalERPPayoutDetails.FirstOrDefault().PaidDate;
                            o.Name = item.listPromotionalERPPayoutDetails.FirstOrDefault().Name;
                            o.Email = item.listPromotionalERPPayoutDetails.FirstOrDefault().Email;
                            o.Phone = item.listPromotionalERPPayoutDetails.FirstOrDefault().Phone;
                            o.ERP = item.listPromotionalERPPayoutDetails.Sum(x => x.ERP);
                            o.EzeeMoney = item.listPromotionalERPPayoutDetails.Sum(x => x.EzeeMoney);
                            list.Add(o);
                        }
                    }
                }
            }


            var gv = new GridView();
            gv.DataSource = list;
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
            Session["PromotionalERPPayoutReport"] = null;
            return RedirectToAction("Index");
        }
    }
}