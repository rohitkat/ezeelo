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
    public class EzeeMoneyPayoutReportController : Controller
    {
        private EzeeloDBContext db = new EzeeloDBContext();
        // GET: Admin/EzeeMoneyPayoutReport
        public ActionResult Index()
        {
            EzeeMoneyPayoutReportViewModel obj = new EzeeMoneyPayoutReportViewModel();

            obj.PayoutDateFilter = new SelectList(db.EzeeMoneyPayouts.
                Select(p => new
                {
                    ID = p.Id,
                    FromDate = p.FromDate,
                    ToDate = p.ToDate,
                }).ToList()
                .Select(p => new
                {
                    ID = p.ID,
                    Name = p.ToDate.ToString("dd-MM-yyyy") + " To " + p.FromDate.ToString("dd-MM-yyyy")
                }), "ID", "Name").ToList();
            
            var list = db.EzeeMoneyPayoutDetail.ToList();
            obj.listEzeeMoneyPayoutDetails = list;
            return View(obj);
        }
        public PartialViewResult GetAllGroupData()
        {
            List<EzeeMoneyPayout_ReportViewModel> list = db.EzeeMoneyPayoutDetail.ToList().GroupBy(p => new { p.UserLoginId, p.Status })
                 .Select(g => new EzeeMoneyPayout_ReportViewModel { Status = g.Key.Status, UserloginId = g.Key.UserLoginId, listEzeeMoneyPayoutDetails = g.OrderBy(y => y.UserLoginId).ToList() }).ToList();
            Session["EzeeMoneyPayoutReport"] = list;
            Session["GoTo"] = "GetAllGroupData";
            return PartialView("_GetAllGroupData", list);

        }
        public PartialViewResult GetAllDataByStatus(int status)
        {
            EzeeMoneyPayoutReportViewModel obj = new EzeeMoneyPayoutReportViewModel();
            bool status_ = (status == 1) ? true : false;
            if (status_ == true || status_ == false)
            {
                obj.listEzeeMoneyPayoutDetails = db.EzeeMoneyPayoutDetail.Where(x => x.Status == status_).ToList();
                
            }
            else
            {
                obj.listEzeeMoneyPayoutDetails = db.EzeeMoneyPayoutDetail.ToList();
                
            }
            Session["GoTo"] = "GetAllDataByStatus";
            Session["EzeeMoneyPayoutReport"] = obj.listEzeeMoneyPayoutDetails;
            return PartialView("_GetAllData", obj);


        }
        public PartialViewResult GetAllDataByPayoutMonth(int Id)
        {

            EzeeMoneyPayoutReportViewModel obj = new EzeeMoneyPayoutReportViewModel();
            var id = Id;
            obj.listEzeeMoneyPayoutDetails = db.EzeeMoneyPayoutDetail.Where(x => x.EzeeMoneyPayoutID == id).ToList();
            Session["EzeeMoneyPayoutReport"] = obj.listEzeeMoneyPayoutDetails;
            Session["GoTo"] = "GetAllDataByPayoutMonth";
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
            FileName =" EzeeMoneyPayoutReport";
            List<ExportToExcel_List> list = new List<ExportToExcel_List>();
            if (Session["EzeeMoneyPayoutReport"] != null)
            {
                if(flag==0)
                { 
                List<EzeeMoneyPayoutDetails> objWRLVM = (List<EzeeMoneyPayoutDetails>)Session["EzeeMoneyPayoutReport"];
                    var i = 0;
                    if (objWRLVM.Count != 0)
                    {

                        foreach (var item in objWRLVM)
                        {
                            ExportToExcel_List o = new ExportToExcel_List();
                            i = i + 1;
                            o.SrNo = i;
                            o.UserloginId = item.UserLoginId;
                            o.Name = item.Name;
                            o.EmailId = item.EmailId;
                            o.PhoneNo = item.PhoneNo;
                            o.TotalOrdAmt = item.TotalOrdAmt;
                            o.TotalRetailPoints = item.TotalRetailPoints;
                            o.ERP = item.ERP;
                            if (item.Status == true)
                            {
                                o.Status = "Active";
                            }
                            else
                            {
                                o.Status = "InActive";
                            }
                            o.EzeeMoney = item.EzeeMoney;
                            o.QRP = item.QRP;
                            list.Add(o);
                        }
                    }
                }
                else
                {
                    List<EzeeMoneyPayout_ReportViewModel> obj = (List<EzeeMoneyPayout_ReportViewModel>)Session["EzeeMoneyPayoutReport"];
                    var i = 0;
                    if (obj.Count != 0)
                    {

                        foreach (var item in obj)
                        {
                            ExportToExcel_List o = new ExportToExcel_List();
                            i = i + 1;
                            o.SrNo = i;
                            o.UserloginId = item.listEzeeMoneyPayoutDetails.FirstOrDefault().UserLoginId;
                            o.Name = item.listEzeeMoneyPayoutDetails.FirstOrDefault().Name;
                            o.EmailId = item.listEzeeMoneyPayoutDetails.FirstOrDefault().EmailId;
                            o.PhoneNo = item.listEzeeMoneyPayoutDetails.FirstOrDefault().PhoneNo;
                            o.TotalOrdAmt = item.listEzeeMoneyPayoutDetails.Sum(x=>x.TotalOrdAmt);
                            o.TotalRetailPoints = item.listEzeeMoneyPayoutDetails.Sum(x => x.TotalRetailPoints);
                            o.ERP = item.listEzeeMoneyPayoutDetails.Sum(x => x.ERP);
                            if (item.Status == true)
                            {
                                o.Status = "Active";
                            }
                            else
                            {
                                o.Status = "InActive";
                            }
                            o.EzeeMoney = item.listEzeeMoneyPayoutDetails.Sum(x => x.EzeeMoney);
                            o.QRP = item.listEzeeMoneyPayoutDetails.Sum(x => x.QRP);
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
            Session["EzeeMoneyPayoutReport"] = null;
            return RedirectToAction("Index");
        }
    }
}