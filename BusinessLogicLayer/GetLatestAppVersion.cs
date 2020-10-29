using ModelLayer.Models;
using ModelLayer.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace BusinessLogicLayer
{
    public class GetLatestAppVersion
    {
        private EzeeloDBContext db = new EzeeloDBContext();
        /// <summary>
        /// Data Server Connection 
        /// </summary>
        protected static System.Web.HttpServerUtility server;

        /// <summary>
        /// Server Connection Inisialization
        /// </summary>
        /// <param name="s"></param>
        public GetLatestAppVersion(System.Web.HttpServerUtility s)
        {
            server = s;
        }

        public static List<AppVersionViewModel> GetAppVersion()
        {
            DataTable dt = new DataTable();
            ReadConfig config = new ReadConfig(server);
            DataAccessLayer.DbOperations dbOpr = new DataAccessLayer.GetData(config.DB_CONNECTION);
            List<object> paramValues = new List<object>();
            paramValues.Add(0);
            paramValues.Add("");
            paramValues.Add("");
            paramValues.Add(null);
            paramValues.Add(8);// using 8 as mode for Geting latest version of App, published in App Stored in mobile
            dt = dbOpr.GetRecords("GetData", paramValues);

            List<AppVersionViewModel> VersionName = new List<AppVersionViewModel>();
            VersionName = (from n in dt.AsEnumerable()
                          select new AppVersionViewModel
                          {
                              VersionName = n.Field<string>("Name"),
                              VersionCode = n.Field<string>("Code")
                          }).ToList();
            return VersionName;
        }
    }
}
