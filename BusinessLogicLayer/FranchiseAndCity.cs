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
    public class FranchiseAndCity
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
        public FranchiseAndCity(System.Web.HttpServerUtility s)
        {
            server = s;
        }

        public static List<FranchiseCity> GetFranchiseCities()
        {
            DataTable dt = new DataTable();
            ReadConfig config = new ReadConfig(server);
            DataAccessLayer.DbOperations dbOpr = new DataAccessLayer.GetData(config.DB_CONNECTION);
            List<object> paramValues = new List<object>();
            paramValues.Add(0);
            paramValues.Add("");
            paramValues.Add("");
            paramValues.Add(null);
            paramValues.Add(3);// using 3 as mode for Get City and its franchises
            dt = dbOpr.GetRecords("GetData", paramValues);

            List<FranchiseCity> franchCity =new List<FranchiseCity>();
            franchCity = (from n in dt.AsEnumerable()
                          select new FranchiseCity
                           {
                               CityName = n.Field<string>("CityName"),
                               CityID = n.Field<long>("CityID"),
                               FranchiseID = n.Field<string>("FranchiseIDs")
                           }).ToList();
            return franchCity;
        }
    }
}
