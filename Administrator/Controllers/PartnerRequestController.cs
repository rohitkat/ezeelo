using Administrator.Models;
using ModelLayer.Models;
using ModelLayer.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Administrator.Controllers
{
    public class PartnerRequestController : Controller
    {
        //
        // GET: /PartnerRequest/
        private EzeeloDBContext db = new EzeeloDBContext();
        [SessionExpire]
        public ActionResult Index(string StartDate, string EndDate)
        {
            List<PartnerRequest> list = new List<PartnerRequest>();
            PartnerRequest obj = new PartnerRequest();
            list = obj.GetPartnerRequestList();
            DateTime StartDate_ = DateTime.Now.Date;
            DateTime EndDate_ = DateTime.Now.Date;
            if (StartDate != null && EndDate != null)
            {
                if (StartDate != "" && EndDate != "")
                {
                    StartDate_ = Convert.ToDateTime(StartDate);
                    EndDate_ = Convert.ToDateTime(EndDate);
                    list = list.Where(p => p.RegistrationDateTime <= EndDate_ && p.RegistrationDateTime >= StartDate_).ToList();
                    ViewBag.StartDate = StartDate;
                    ViewBag.EndDate = EndDate;
                }
            }
            return View(list);
        }

        [SessionExpire]
        public ActionResult Details(int Id)
        {
            PartnerRequest obj = db.PartnerRequests.FirstOrDefault(p => p.ID == Id);
            obj.State = db.States.FirstOrDefault(s => s.ID == obj.StateID).Name;
            obj.InvestmentCapacityList = new SelectList(
               new List<SelectListItem>
                {
                    new SelectListItem { Selected = false, Text = "5-10 Lacs", Value = ((int)1).ToString()},
                    new SelectListItem { Selected = false, Text = "10-15 Lacs", Value = ((int)2).ToString()},
                    new SelectListItem { Selected = false, Text = "15-20 Lacs", Value = ((int)3).ToString()},
                    new SelectListItem { Selected = false, Text = "20 Lac plus", Value = ((int)4).ToString()},
                }, "Value", "Text", 1);
            string CapcityId = obj.InvestmentCapacity.ToString();
            obj.Investment_Capacity = obj.InvestmentCapacityList.FirstOrDefault(p => p.Value == CapcityId).Text;
            obj.DisplayDate = obj.RegistrationDateTime.ToString("dd/MM/yyyy HH:mm");
            return View(obj);
        }

        public ActionResult ExportToExcel(string ext, string StartDate, string EndDate)
        {
            var gv = new GridView();
            string FileName = "ListOfPartner" + DateTime.Now.Date.ToString("ddMMyyyy");

            List<PartnerRequest> list = db.PartnerRequests.OrderByDescending(p => p.RegistrationDateTime).ToList();
            DateTime StartDate_ = DateTime.Now.Date;
            DateTime EndDate_ = DateTime.Now.Date;
            if (StartDate != null && EndDate != null)
            {
                if (StartDate != "" && EndDate != "")
                {
                    StartDate_ = Convert.ToDateTime(StartDate);
                    EndDate_ = Convert.ToDateTime(EndDate);
                    list = list.Where(p => p.RegistrationDateTime <= EndDate_ && p.RegistrationDateTime >= StartDate_).ToList();
                }
            }
            foreach (var item in list)
            {
                item.State = db.States.FirstOrDefault(s => s.ID == item.StateID).Name;

                item.InvestmentCapacityList = new SelectList(
                   new List<SelectListItem>
                {
                    new SelectListItem { Selected = false, Text = "5-10 Lacs", Value = ((int)1).ToString()},
                    new SelectListItem { Selected = false, Text = "10-15 Lacs", Value = ((int)2).ToString()},
                    new SelectListItem { Selected = false, Text = "15-20 Lacs", Value = ((int)3).ToString()},
                    new SelectListItem { Selected = false, Text = "20 Lac plus", Value = ((int)4).ToString()},
                }, "Value", "Text", 1);
                string CapcityId = item.InvestmentCapacity.ToString();
                item.Investment_Capacity = item.InvestmentCapacityList.FirstOrDefault(p => p.Value == CapcityId).Text;
                item.DisplayDate = item.RegistrationDateTime.ToString("dd/MM/yyyy HH:mm");
            }
            gv.DataSource = list;
            gv.DataBind();
            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename=" + FileName + "." + ext);
            if (ext == "xls")
            {
                Response.ContentType = "application/ms-excel";
            }
            else if (ext == "pdf")
            {
                Response.ContentType = "application/pdf";
            }
            else
            {
                Response.ContentType = "application/text";
            }
            Response.Charset = "";
            StringWriter objStringWriter = new StringWriter();
            HtmlTextWriter objHtmlTextWriter = new HtmlTextWriter(objStringWriter);
            gv.RenderControl(objHtmlTextWriter);
            Response.Output.Write(objStringWriter.ToString());
            Response.Flush();
            Response.End();
            return RedirectToAction("Index");
        }
    }
}