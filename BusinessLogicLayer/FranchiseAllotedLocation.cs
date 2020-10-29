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
    public class FranchiseAllotedLocation
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
        public FranchiseAllotedLocation(System.Web.HttpServerUtility s)
        {
            server = s;
        }
        /// <summary>
        /// For Getting Franchise allotted Area with Pincode
        /// </summary>
        /// <param name="cityid"></param>
        /// <returns></returns>
        public static List<FranchiseAreaPincode> GetFranchiseAreaPincode(int cityid)
        {
            DataTable dt = new DataTable();
            ReadConfig config = new ReadConfig(server);
            DataAccessLayer.DbOperations dbOpr = new DataAccessLayer.GetData(config.DB_CONNECTION);
            List<object> paramValues = new List<object>();
            paramValues.Add(cityid);
            paramValues.Add("");
            paramValues.Add("");
            paramValues.Add(null);
            paramValues.Add(4);// using 4 as mode for Getting Franchise allotted Area with Pincode
            dt = dbOpr.GetRecords("GetData", paramValues);

            List<FranchiseAreaPincode> franchAreaPin = new List<FranchiseAreaPincode>();

            franchAreaPin = (from n in dt.AsEnumerable()
                             select new FranchiseAreaPincode
                          {
                              Area = n.Field<string>("Area"),
                              AreaID = n.Field<int>("AreaID"),
                              Pincode = n.Field<string>("Pincode"),
                              PincodeID = n.Field<int>("PincodeID")
                          }).ToList();

            return franchAreaPin;
        }
    }

}
