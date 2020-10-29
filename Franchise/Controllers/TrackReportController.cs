using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ModelLayer.Models;
using System.Collections.ObjectModel;
using ModelLayer.Models.ViewModel;
using BusinessLogicLayer;
using Franchise.Models;
using System.Data;
using System.Data.SqlClient;
using PagedList;
using PagedList.Mvc;

namespace Franchise.Controllers
{
    public class TrackReportController : Controller
    {
        //
        // GET: /TrackReport/
        private EzeeloDBContext db = new EzeeloDBContext();
        private int pageSize = 25;

       // long FranchiseID = Convert.ToInt64(Session["FRANCHISE_ID"]);
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult _GetReport(int? page, string StartDate, string EndDate)
         {
             //List<TrackSearch> TrackSearchReportModel = new List<TrackSearch>();
             int pageNumber;
             int FranchiseID = Convert.ToInt32(Session["FRANCHISE_ID"]);

            // int FranchiseID = 2;
             try
             {
                 DateTime frmd = new DateTime();
                 DateTime tod = new DateTime();
                 if (StartDate != "" && StartDate != null)
                 {
                     string from = StartDate.ToString();
                     string[] f = from.Split('/');
                     string[] ftime = f[2].Split(' ');
                      frmd = CommonFunctions.GetLocalTime(Convert.ToInt32(f[0]), Convert.ToInt32(f[1]), Convert.ToInt32(ftime[0]), 0, 0, 0).Date;
                      frmd = CommonFunctions.GetProperDate(frmd.ToShortDateString());
                      //frmd = Convert.ToDateTime(from);

                 }
                 if (EndDate != "" && EndDate != null)
                 {
                     string to = EndDate.ToString();
                     string[] t = to.Split('/');
                     string[] ttime = t[2].Split(' ');
                     tod = CommonFunctions.GetLocalTime(Convert.ToInt32(t[0]), Convert.ToInt32(t[1]), Convert.ToInt32(ttime[0]), 0, 0, 0);
                     tod = CommonFunctions.GetProperDate(tod.ToShortDateString());
                     //tod = Convert.ToDateTime(tod.ToShortDateString());
                     tod = tod.AddDays(1);

                 }

                 int TotalCount = 0;
                  pageNumber = (page ?? 1);
                  ViewBag.PageNumber = pageNumber;
                  ViewBag.PageSize = pageSize;
                  ViewBag.StartDate = StartDate;
                  ViewBag.EndDate = EndDate;

                  List<TrackSearch> listTrackSearchReportViewModel = new List<TrackSearch>();

                 DataTable lDataTableCustomerOrder = new DataTable();
                 ReadConfig readCon = new ReadConfig(System.Web.HttpContext.Current.Server);
                 string conn = readCon.DB_CONNECTION;

                 SqlConnection con = new SqlConnection(conn);
                 SqlCommand sqlComm = new SqlCommand("GetTrackReport", con);

                 sqlComm.Parameters.AddWithValue("@FranchiseID", SqlDbType.Int).Value = FranchiseID;
                 sqlComm.Parameters.AddWithValue("@Fromdate", SqlDbType.DateTime).Value = frmd;
                 sqlComm.Parameters.AddWithValue("@Todate", SqlDbType.DateTime).Value = tod;
                 sqlComm.CommandType = CommandType.StoredProcedure;

                 con.Open();

                 //sqlComm.ExecuteNonQuery();

                 SqlDataAdapter da = new SqlDataAdapter(sqlComm);
                 DataTable dt = new DataTable();
                 da.Fill(dt);
                 

                 listTrackSearchReportViewModel = BusinessLogicLayer.Helper.CreateListFromTable<TrackSearch>(dt);

                 //listTrackSearchReportViewModel = listTrackSearchReportViewModel.Where(x => x.CreateDate >= frmd &&
                 //                            x.CreateDate <= tod).ToList();

                 TotalCount = listTrackSearchReportViewModel.Count();
                 ViewBag.TotalCount = TotalCount;
                 //return RedirectToAction("GetReport", TrackSearchReportModel.ToPagedList(pageNumber, pageSize));
                 return View("Index",listTrackSearchReportViewModel.ToPagedList(pageNumber, pageSize));

                 //return View(listTrackSearchReportViewModel.ToList().OrderByDescending(x => x.Date).ToPagedList(pageNumber, pageSize));

             }
             catch (BusinessLogicLayer.MyException myEx)
             {
                 ModelState.AddModelError("Error", "There's Something wrong in loading Shop Detail Report!!");

                 BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                     + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                     + "[TrackReportController][POST:GetReport]" + myEx.EXCEPTION_PATH,
                     BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
             }
             catch (Exception ex)
             {
                 ModelState.AddModelError("Error", "There's Something wrong in loading Shop Detail Report!!");

                 BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                     + Environment.NewLine + ex.Message + Environment.NewLine
                     + "[TrackReportController][POST:GetReport]",
                     BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
             }
             return View("Index");

        }

        //public ActionResult Export()
        //{
            
        //}

        public ActionResult Export(string StartDate, string EndDate, int option)
        {

            try
            {
                //int option=0;
                //int option = Convert.ToInt32(objfrm["ExportOption"].ToString());
                //int print=1;
               //int FranchiseID = Convert.ToInt32(Session["FRANCHISE_ID"]);
                List<TrackSearch> objTracksearch = new List<TrackSearch>();

                objTracksearch = this.GetData(StartDate, EndDate);

                //if (print == 1)
                //{
                //    return View("ForPrint", objTracksearch);
                //}
                DataTable dt = new DataTable();
                dt.Columns.Add("Sr.No.", typeof(long));
                dt.Columns.Add("City", typeof(string));
                dt.Columns.Add("Product Name", typeof(string));
                dt.Columns.Add("Device Type", typeof(string));
                dt.Columns.Add("Total Count", typeof(int));


                int i = 0;
                //ViewBag.stateID = new SelectList(db.States, "ID", "Name");
                foreach (var row in objTracksearch)
                {
                    i = i + 1;
                    //dt.LoadDataRow(new object[] { i, row.OrderCode, row.ShopName, row.OrderAmount, row.Customer,row.RegEmail,row.RegMobile,row.Pincode,row.ShippingAddress,
                    //row.City,row.CreateDate}, false);
                    dt.LoadDataRow(new object[] { i, row.City, row.ProductName, row.DeviceType, row.TotalCount }, false);



                }
                ExportExcelCsv ExportExcelCsv = new ExportExcelCsv(System.Web.HttpContext.Current.Server);
                if (option == 1)
                {
                    ExportExcelCsv.ExportToExcel(dt, "Track Search Report");
                }
                else if (option == 2)
                {
                    ExportExcelCsv.ExportToCSV(dt, "Track Search Report");
                }
                else if (option == 3)
                {
                    ExportExcelCsv.ExportToPDF(dt, "Track Search Report");
                }

            }
            catch (Exception)
            {
                throw;
            }
            return View();
        }

        public List<TrackSearch> GetData(string fromdate,string todate)
        {
            int FranchiseID = Convert.ToInt32(Session["FRANCHISE_ID"]);
            List<TrackSearch> TrackSearchReportModel = new List<TrackSearch>();

             //int FranchiseID = 2;
             try
             {
                 DateTime frmd = new DateTime();
                 DateTime tod = new DateTime();
                 if (fromdate != "" && fromdate != null)
                 {
                     string from = fromdate.ToString();
                     string[] f = from.Split('/');
                     string[] ftime = f[2].Split(' ');
                      frmd = CommonFunctions.GetLocalTime(Convert.ToInt32(f[0]), Convert.ToInt32(f[1]), Convert.ToInt32(ftime[0]), 0, 0, 0).Date;
                      frmd = CommonFunctions.GetProperDate(frmd.ToShortDateString());
                      //frmd = Convert.ToDateTime(from);

                 }
                 if (todate != "" && todate != null)
                 {
                     string to = todate.ToString();
                     string[] t = to.Split('/');
                     string[] ttime = t[2].Split(' ');
                     tod = CommonFunctions.GetLocalTime(Convert.ToInt32(t[0]), Convert.ToInt32(t[1]), Convert.ToInt32(ttime[0]), 0, 0, 0);
                     tod = CommonFunctions.GetProperDate(tod.ToShortDateString());
                     //tod = Convert.ToDateTime(tod.ToShortDateString());
                     tod = tod.AddDays(1);

                 }
          


                DataTable lDataTableCustomerOrder = new DataTable();
                ReadConfig readCon = new ReadConfig(System.Web.HttpContext.Current.Server);
                string conn = readCon.DB_CONNECTION;

                SqlConnection con = new SqlConnection(conn);
                SqlCommand sqlComm = new SqlCommand("GetTrackReport", con);
                 sqlComm.Parameters.AddWithValue("@FranchiseID", SqlDbType.Int).Value = FranchiseID;
                 sqlComm.Parameters.AddWithValue("@Fromdate", SqlDbType.DateTime).Value = frmd;
                 sqlComm.Parameters.AddWithValue("@Todate", SqlDbType.DateTime).Value = tod;

                sqlComm.CommandType = CommandType.StoredProcedure;

                con.Open();

                sqlComm.ExecuteNonQuery();

                SqlDataAdapter da = new SqlDataAdapter(sqlComm);
                DataTable dt = new DataTable();
                da.Fill(dt);

                TrackSearchReportModel = BusinessLogicLayer.Helper.CreateListFromTable<TrackSearch>(dt);
            }

                //return View(TrackSearchReportModel);

            catch (Exception)
            {
                throw;
            }
            return TrackSearchReportModel;


        }
    }
}