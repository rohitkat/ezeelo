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
using System.Globalization;
using BusinessLogicLayer;


namespace Administrator.Controllers
{
    public class SearchKeywordReportController : Controller
    {
        EzeeloDBContext db = new EzeeloDBContext();

        public ActionResult Index()
        {
            return View();

        }
        public List<Save_SiteKeywordSearchViewModel> GetSearchList()
        {
            List<TrackSearch_Keywords> keywordSearchList = db.TrackSearch_Keywords.ToList();
            List<Save_SiteKeywordSearchViewModel> searchList = new List<Save_SiteKeywordSearchViewModel>();
            
            foreach (var item in keywordSearchList)
            {
                if (item.UserloginID != 0)
                {
                    searchList.Add(new Save_SiteKeywordSearchViewModel
                    {
                        FullName = db.PersonalDetails.Where(x => x.UserLoginID == item.UserloginID).Select(y => y.FirstName + " " + y.MiddleName + " " + y.LastName).FirstOrDefault(),
                        Email = db.UserLogins.Where(x => x.ID == item.UserloginID).Select(y => y.Email).FirstOrDefault(),
                        Mobile = db.UserLogins.Where(x => x.ID == item.UserloginID).Select(y => y.Mobile).FirstOrDefault(),
                        Keyword = item.Keyword,
                        CityName=db.Cities.Where(x=>x.ID==item.City_ID).Select(y=>y.Name).FirstOrDefault(),
                        FranchiseName=db.Franchises.Where(x=>x.ID==item.Franchise_ID).Select(y=>y.ContactPerson).FirstOrDefault(),
                        CategoryName=db.Categories.Where(x=>x.ID==item.Category_ID).Select(y=>y.Name).FirstOrDefault(),
                        Create_Date = item.Create_Date,
                        IsResult=item.IsResult,
                        DeviceType = item.DeviceType,
                        Device_ID = item.Device_ID,
                        Network_IP = item.Network_IP

                    });
                }
                else
                {
                    searchList.Add(new Save_SiteKeywordSearchViewModel
                    {

                        FullName = "Guest",
                        Email = "",
                        Mobile = "",
                        Keyword = item.Keyword,
                        Create_Date = item.Create_Date,
                        CityName = db.Cities.Where(x => x.ID == item.City_ID).Select(y => y.Name).FirstOrDefault(),
                        FranchiseName = db.Franchises.Where(x => x.ID == item.Franchise_ID).Select(y => y.ContactPerson).FirstOrDefault(),
                        CategoryName = db.Categories.Where(x => x.ID == item.Category_ID).Select(y => y.Name).FirstOrDefault(),
                        IsResult=item.IsResult,
                        Device_ID = item.Device_ID,
                        DeviceType = item.DeviceType,
                        Network_IP = item.Network_IP

                    });
                }
            }

            foreach (var item in searchList)
            {
                if (item.IsResult == false)
                {
                    item.SearchResult = "false";
                }
                else
                {
                    item.SearchResult = "true";
                }
            }
            return searchList;
        }

        public ActionResult partialSearchKeywordReport()
        {
            var searchList = GetSearchList();
            return PartialView("partialSearchKeywordReport", searchList);
        }


        public ActionResult FilterDateList(string startDate, string endDate)
        {
           // DateTime sDate = DateTime.Parse(startDate);
           // DateTime sDate = DateTime.ParseExact(startDate, "MM/dd/yyyy", CultureInfo.InvariantCulture);
          //  DateTime eDate = DateTime.ParseExact(endDate, "MM/dd/yyyy", CultureInfo.InvariantCulture);
           // DateTime eDate = DateTime.Parse(endDate);

            DateTime frmd = new DateTime();
            DateTime tod = new DateTime();
            if (startDate != "" && startDate != null)
            {
                string from = startDate.ToString();
                string[] f = from.Split('/');
                string[] ftime = f[2].Split(' ');
                frmd = CommonFunctions.GetLocalTime(Convert.ToInt32(f[0]), Convert.ToInt32(f[1]), Convert.ToInt32(ftime[0]), 0, 0, 0);
                frmd = Convert.ToDateTime(frmd.ToShortDateString());

            }
            if (endDate != "" && endDate != null)
            {
                string to = endDate.ToString();
                string[] t = to.Split('/');
                string[] ttime = t[2].Split(' ');
                tod = CommonFunctions.GetLocalTime(Convert.ToInt32(t[0]), Convert.ToInt32(t[1]), Convert.ToInt32(ttime[0]), 0, 0, 0);
                tod = Convert.ToDateTime(tod.ToShortDateString());
                tod = tod.AddDays(1);

            }

            if (startDate == endDate)
            {
                var SDate = new DateTime(frmd.Year, frmd.Month, frmd.Day, frmd.Hour, frmd.Minute, 1);
                var EDate = SDate.AddDays(1).AddMinutes(-1);

                var reportListNew = GetSearchList().ToList<Save_SiteKeywordSearchViewModel>().Where(x => x.Create_Date >= SDate && x.Create_Date <= EDate).ToList();
                return PartialView("partialSearchKeywordReport", reportListNew);
            }
            else
            {

                var reportList = GetSearchList().ToList<Save_SiteKeywordSearchViewModel>().Where(x => x.Create_Date >= frmd && x.Create_Date <= tod).ToList();

                return PartialView("partialSearchKeywordReport", reportList);
            }

        }
        public ActionResult ExportToExcel(string toDate, string fromDate)
        {
            List<Save_SiteKeywordSearchViewModel> reportList = GetSearchList();

            DateTime frmd = new DateTime();
            DateTime tod = new DateTime();
            if (fromDate != "" && fromDate != null)
            {
                string from = fromDate.ToString();
                string[] f = from.Split('/');
                string[] ftime = f[2].Split(' ');
                frmd = CommonFunctions.GetLocalTime(Convert.ToInt32(f[0]), Convert.ToInt32(f[1]), Convert.ToInt32(ftime[0]), 0, 0, 0);
                frmd = Convert.ToDateTime(frmd.ToShortDateString());

            }
            if (toDate != "" && toDate != null)
            {
                string to = toDate.ToString();
                string[] t = to.Split('/');
                string[] ttime = t[2].Split(' ');
                tod = CommonFunctions.GetLocalTime(Convert.ToInt32(t[0]), Convert.ToInt32(t[1]), Convert.ToInt32(ttime[0]), 0, 0, 0);
                tod = Convert.ToDateTime(tod.ToShortDateString());
                tod = tod.AddDays(1);

            }
            if (toDate != "" && fromDate != "")
            {
               // DateTime sDate = Convert.ToDateTime(fromDate);
               // DateTime eDate = Convert.ToDateTime(toDate);

                if (toDate == fromDate)
                {
                    var SDate = new DateTime(frmd.Year, frmd.Month, frmd.Day, frmd.Hour, frmd.Minute, 1);
                    var EDate = SDate.AddDays(1).AddMinutes(-1);

                    reportList = reportList.Where(x => x.Create_Date >= SDate && x.Create_Date <= EDate).ToList();

                }
                else
                {
                    reportList = reportList.Where(x => x.Create_Date >= frmd && x.Create_Date <= tod).ToList();
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
            return View("Index");
        }
	}
}