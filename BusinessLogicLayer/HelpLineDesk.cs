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
    public class HelpLineDesk
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
        public HelpLineDesk(System.Web.HttpServerUtility s)
        {
            server = s;
        }

        public static List<HelpLine> GetHelpLineNumber(int franchiseid)
        {
            DataTable dt = new DataTable();
            ReadConfig config = new ReadConfig(server);
            DataAccessLayer.DbOperations dbOpr = new DataAccessLayer.GetData(config.DB_CONNECTION);
            List<object> paramValues = new List<object>();
            paramValues.Add(franchiseid);
            paramValues.Add("");
            paramValues.Add("");
            paramValues.Add(null);
            paramValues.Add(6);// using 6 as mode for Getting Franchise allotted Area with Pincode
            dt = dbOpr.GetRecords("GetData", paramValues);

            List<HelpLine> helpLinNo = new List<HelpLine>();

            helpLinNo = (from n in dt.AsEnumerable()
                             select new HelpLine
                          {
                              HelpLineNumber = n.Field<string>("HelpLineNumber")
                          }).ToList();

            return helpLinNo;
        }
    }
    public class HelpLine
    {
        public string HelpLineNumber { get; set; } 
    }
}
