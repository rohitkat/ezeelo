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
   public class FranchisePageMessages
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
       public FranchisePageMessages(System.Web.HttpServerUtility s)
        {
            server = s;
        }

        public static List<WeeklySeasonalFestivalPageMessage> GetFranchisePageMessage(int franchiseid)
        {
            DataTable dt = new DataTable();
            ReadConfig config = new ReadConfig(server);
            DataAccessLayer.DbOperations dbOpr = new DataAccessLayer.GetData(config.DB_CONNECTION);
            List<object> paramValues = new List<object>();
            paramValues.Add(franchiseid);
            paramValues.Add("");
            paramValues.Add("");
            paramValues.Add(null);
            paramValues.Add(7);// using 7 as mode for Getting Franchise weekly Holiday Message
            dt = dbOpr.GetRecords("GetData", paramValues);

            List<WeeklySeasonalFestivalPageMessage> WSFMsg = new List<WeeklySeasonalFestivalPageMessage>();
            WSFMsg = (from n in dt.AsEnumerable()
                      select new WeeklySeasonalFestivalPageMessage
                             {
                                 MessageTypeID = n.Field<long>("MessageTypeID"),
                                  MessageType = n.Field<string>("MessageType"),
                                 Message = n.Field<string>("Message"),
                                 MinimumOrderInRupee = n.Field<int?>("MinimumOrderInRupee"),
                                 WeeklyHoliday = n.Field<string>("WeeklyHoliday") == null ? "" : n.Field<string>("WeeklyHoliday"),
                                 SeasonalMsgFrmMonth = n.Field<string>("SeasonalMsgFrmMonth") == null ? "" : n.Field<string>("SeasonalMsgFrmMonth"),
                                 SeasonalMsgToMonth = n.Field<string>("SeasonalMsgToMonth") == null ? "" : n.Field<string>("SeasonalMsgToMonth"),
                                 FestivalMsgFrmDate = n.Field<string>("FestivalMsgFrmDate") == null ? "" : n.Field<string>("FestivalMsgFrmDate"),
                                 FestivalMsgToDate = n.Field<string>("FestivalMsgToDate") == null ? "" : n.Field<string>("FestivalMsgToDate")
                             }).ToList();

            return WSFMsg;
        }
    }
}
