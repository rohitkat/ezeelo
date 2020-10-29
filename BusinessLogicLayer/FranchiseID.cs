using ModelLayer.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace BusinessLogicLayer
{
    public class FranchiseID
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
        public FranchiseID(System.Web.HttpServerUtility s)
        {
            server = s;
        }

        public static List<FranchisIdList> GetFranchiseID(int cityid, int pincode_areaid)
        {
            DataTable dt = new DataTable();
            ReadConfig config = new ReadConfig(server);
            DataAccessLayer.DbOperations dbOpr = new DataAccessLayer.GetData(config.DB_CONNECTION);
            List<object> paramValues = new List<object>();
            paramValues.Add(cityid);
            paramValues.Add(pincode_areaid);
            paramValues.Add("");
            paramValues.Add(null);
            paramValues.Add(5);// using 5 as mode for Getting FranchiseIDs List as per Pincode OR AreaId
            dt = dbOpr.GetRecords("GetData", paramValues);
            List<FranchisIdList> franchid = new List<FranchisIdList>();

            franchid = (from n in dt.AsEnumerable()
                        select new FranchisIdList
                             {
                                 FranchiseIDs = n.Field<int>("FranchiseIDs"),
                                 FranchiseOfficePincode = n.Field<string>("FranchiseOfficePincode"),
                                 CityName = n.Field<string>("CityName")
                             }).ToList();

            return franchid;
        }

    }
    public class FranchisIdList
    {
        public int FranchiseIDs { get; set; }
        public string FranchiseOfficePincode { get; set; }
        public string CityName { get; set; }
    }

}
