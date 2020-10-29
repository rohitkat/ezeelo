using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer
{
    public class ReportManagement : MISReportRepository
    {       
        /// <summary>
        /// Developed  By  : Pradnyakar Badge
        /// to call store Procedure according to the parameter provided
        /// </summary>
        /// <param name="paramValues">franchiseID, FromDate,ToDate</param>
        /// <param name="server"></param>
        /// <returns></returns>
        public DataTable ShopOrderStatistic_MIS(List<object> paramValues,System.Web.HttpServerUtility server)
        {
            DataTable dt = new DataTable();
            ReadConfig config = new ReadConfig(server);
            DataAccessLayer.DbOperations dbOpr = new DataAccessLayer.GetData(config.DB_CONNECTION);
            dt = dbOpr.GetRecords("ShopOrderStatistic_MIS", paramValues);
            return dt;
        }
    }
    /// <summary>
    /// Developed  By  : Pradnyakar Badge
    /// interface
    /// </summary>
    public interface MISReportRepository
    {
        DataTable ShopOrderStatistic_MIS(List<object> paramValues, System.Web.HttpServerUtility server);
    }
    

}
