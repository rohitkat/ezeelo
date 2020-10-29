using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ModelLayer.Models;
using ModelLayer.Models.ViewModel;

namespace Leaders.Controllers
{
    public class NetworkExplorerController : Controller
    {
        private EzeeloDBContext db = new EzeeloDBContext();
        public ActionResult TabularView()
        {
            List<Network_Table> reportList = GetNetworkReportList();
            if (reportList != null)
            {
                return View(reportList);
            }
            return View();

        }

        public List<Network_Table> GetNetworkReportList()
        {
            long userID = Convert.ToInt64(Session["ID"]);
            var idParam = new SqlParameter
            {
                ParameterName = "UserID",
                Value = userID

            };

            db.Database.ExecuteSqlCommand("Leaders_NetworkUsers @UserID", idParam);
            List<NetworkUserViewModel> networkUserList = db.NetworkUsersViewModel.ToList();
            if (networkUserList.Count() > 0)
            {
                db.Database.ExecuteSqlCommand("Leaders_NetworkTable");

                // db.SaveChanges();
                List<Network_Table> networkList = db.NetworkTables.OrderByDescending(x => x.Joined_Date).ToList();

                return networkList;
            }
            else
            {
                return null;
            }
           
           // db.Database.ExecuteSqlCommand("Leaders_NetworkTable");

           //// db.SaveChanges();
           // List<Network_Table> networkList = db.NetworkTables.OrderByDescending(x => x.Joined_Date).ToList();

           // return networkList;

            
        }

        public ActionResult ListView()
        {
            //List<NetworkReportViewModel> Level1 = GetNetworkReportList().Where(y => y.Downline == 1).ToList();
            //List<NetworkReportViewModel> Level2 = GetNetworkReportList().Where(y => y.Downline == 2).ToList();
            //List<NetworkReportViewModel> Level3 = GetNetworkReportList().Where(y => y.Downline == 3).ToList();
            //List<NetworkReportViewModel> Level4 = GetNetworkReportList().Where(y => y.Downline == 4).ToList();
            return View(GetNetworkReportList());
        }
	}
}