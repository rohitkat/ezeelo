using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ModelLayer.Models;
using System.Dynamic;
using BusinessLogicLayer;
using PagedList;
using PagedList.Mvc;

namespace Administrator.Controllers
{
    public class SentMailReportController : Controller
    {
        private EzeeloDBContext db = new EzeeloDBContext();
        private int pageSize = 50;
        // GET: /SentMailReport/
        public ActionResult Index(int? page, string StartDate, string EndDate, string SearchUserName, string SearchMobile, string SearchEmail, int? SearchRemainingDays, string SearchDomain, int? option)
        {
            DateTime frmd = new DateTime();
            DateTime tod = new DateTime();
            if (StartDate != "" && StartDate != null)
            {
                string from = StartDate.ToString();
                string[] f = from.Split('/');
                string[] ftime = f[2].Split(' ');
                frmd = CommonFunctions.GetLocalTime(Convert.ToInt32(f[0]), Convert.ToInt32(f[1]), Convert.ToInt32(ftime[0]), 0, 0, 0);
                frmd = Convert.ToDateTime(frmd.ToShortDateString());

            }
            if (EndDate != "" && EndDate != null)
            {
                string to = EndDate.ToString();
                string[] t = to.Split('/');
                string[] ttime = t[2].Split(' ');
                tod = CommonFunctions.GetLocalTime(Convert.ToInt32(t[0]), Convert.ToInt32(t[1]), Convert.ToInt32(ttime[0]), 0, 0, 0);
                tod = Convert.ToDateTime(tod.ToShortDateString());
                tod = tod.AddDays(1);

            }

            int TotalCount = 0;
            int pageNumber = (page ?? 1);
            ViewBag.PageNumber = pageNumber;
            ViewBag.PageSize = pageSize;
            ViewBag.StartDate = StartDate;
            ViewBag.EndDate = EndDate;
            ViewBag.SearchUserName = SearchUserName;
            ViewBag.SearchMobile = SearchMobile;
            ViewBag.SearchEmail = SearchEmail;
            ViewBag.SearchRemainingDays = SearchRemainingDays;
            ViewBag.SearchDomain = SearchDomain;

            List<Sent_Mail> lSent_Mail = new List<Sent_Mail>();
            var SentMailList =(from SM in db.Sent_Mail
                               join BD in db.BusinessDetails on SM.Login_ID equals BD.UserLoginID
                               where SM.RemainingDays != null
                               select new
                               {
                                   UserName = BD.Name,
                                   Email_ID = SM.Email_ID,
                                   Mobile = SM.Mobile,
                                   RemainingDays = SM.RemainingDays,
                                   Sent_Date = SM.Sent_Date,
                                   Remarks = SM.Remarks,
                                   Mail_Type_ID = SM.Mail_Type_ID
                               }).OrderByDescending(x => x.Sent_Date).ToList();

            if (StartDate != null && StartDate != "" && EndDate != null && EndDate != "")
            {
                SentMailList = SentMailList.Where(x => x.Sent_Date != null && x.Sent_Date >= frmd && x.Sent_Date <= tod).OrderByDescending(x => x.Sent_Date).ToList();
            }

            if (SearchMobile != null && SearchMobile != "")
            {
                SentMailList = SentMailList.Where(x => x.Mobile != null && x.Mobile.Trim().StartsWith(SearchMobile.Trim())).OrderByDescending(x => x.Sent_Date).ToList();
            }
            if (SearchEmail != null && SearchEmail != "")
            {
                SentMailList = SentMailList.Where(x => x.Email_ID != null && x.Email_ID.ToLower().Trim().Contains(SearchEmail.ToLower().Trim())).OrderByDescending(x => x.Sent_Date).ToList();
            }
            if (SearchUserName != null && SearchUserName != "")
            {
                SentMailList = SentMailList.Where(x => x.UserName != null && x.UserName.ToLower().Trim().Contains(SearchUserName.ToLower().Trim())).OrderByDescending(x => x.Sent_Date).ToList();
            }
            if (SearchRemainingDays != null)
            {
                SentMailList = SentMailList.Where(x => x.RemainingDays != null && x.RemainingDays == SearchRemainingDays).OrderByDescending(x => x.Sent_Date).ToList();
            }
            if (SearchDomain != null && SearchDomain != "")
            {
                if (SearchDomain=="SHOP")
                {
                    SentMailList = SentMailList.Where(x => x.Mail_Type_ID != null && x.Mail_Type_ID == 2).OrderByDescending(x => x.Sent_Date).ToList();
                }
                else
                {
                    SentMailList = SentMailList.Where(x => x.Mail_Type_ID != null && x.Mail_Type_ID == 3).OrderByDescending(x => x.Sent_Date).ToList();
                }
            }

            if(option !=null)
            {
                DataTable tblProduct = new DataTable();
                tblProduct.Columns.Add("Sr.No.", typeof(long));
                tblProduct.Columns.Add("User Name", typeof(string));
                tblProduct.Columns.Add("Email", typeof(string));
                tblProduct.Columns.Add("Mobil", typeof(string));
                tblProduct.Columns.Add("Remaining Days", typeof(Int32));
                tblProduct.Columns.Add("Sent Date", typeof(string));
                tblProduct.Columns.Add("Remarks", typeof(string));
              
                int i = 0;
                foreach (var row in SentMailList)
                {
                    i = i + 1;
                    tblProduct.LoadDataRow(new object[] { i, row.UserName, row.Email_ID, row.Mobile, row.RemainingDays, row.Sent_Date, row.Remarks }, false);
                }
                ExportExcelCsv ExportExcelCsv = new ExportExcelCsv(System.Web.HttpContext.Current.Server);
                if (option == 1)
                {
                    ExportExcelCsv.ExportToExcel(tblProduct, "Sent Mail Report");
                }
                else if (option == 2)
                {
                    ExportExcelCsv.ExportToCSV(tblProduct, "Sent Mail Report");
                }
                else if (option == 3)
                {
                    ExportExcelCsv.ExportToPDF(tblProduct, "Sent Mail Report");
                }
            }

             foreach (var item in SentMailList)
             {
                 Sent_Mail STM = new Sent_Mail();
                 STM.UserName = item.UserName;
                 STM.Email_ID = item.Email_ID;
                 STM.Mobile = item.Mobile;
                 STM.RemainingDays = item.RemainingDays;
                 STM.Sent_Date = item.Sent_Date;
                 STM.Remarks = item.Remarks;
                 STM.Mail_Type_ID = item.Mail_Type_ID;
                 lSent_Mail.Add(STM);
             }
             TotalCount = SentMailList.Count();
             ViewBag.TotalCount = TotalCount;

             return View(lSent_Mail.ToList().OrderByDescending(x => x.Sent_Date).ToPagedList(pageNumber, pageSize));
        }
    }
}
