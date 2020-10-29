using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer
{
    public class FranchiseMenuList
    {
        /// <summary>
        /// To Fill data Table as per parameter
        /// </summary> 
        /// <param name="FranchiseList">franchise id</param>
        /// <param name="CategoryID">category id</param>
        /// <param name="Level">level</param>
        /// <param name="server">serverpath</param>
        /// <returns>datatable</returns>
        public DataTable Call_Select_Procedure(Int64 FranchiseID, Int64? CategoryID, System.Web.HttpServerUtility server)
        {
            DataTable dt = new DataTable();
            ReadConfig config = new ReadConfig(server);
            DataAccessLayer.DbOperations dbOpr = new DataAccessLayer.GetData(config.DB_CONNECTION);
            List<object> paramValues = new List<object>();
            paramValues.Add(FranchiseID);
            paramValues.Add(CategoryID);

            dt = dbOpr.GetRecords("FranchiseMenuPriorityList", paramValues);

            return dt;
        }

        public DataTable Select_FranchiseMenu(Int64 CityID, Int64 FranchiseID, System.Web.HttpServerUtility server)//added  FranchiseID
        {
            DataTable dt = new DataTable();
            try
            {
                ReadConfig config = new ReadConfig(server);
                DataAccessLayer.DbOperations dbOpr = new DataAccessLayer.GetData(config.DB_CONNECTION);
                List<object> paramValues = new List<object>();
                paramValues.Add(CityID);
                paramValues.Add(FranchiseID); //added FranchiseID
                dt = dbOpr.GetRecords("Select_FranchiseMenuList", paramValues);
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[FranchiseMenuList][Select_FranchiseMenu]",
                    BusinessLogicLayer.ErrorLog.Module.BussinessLogicLayer, System.Web.HttpContext.Current.Server);
            }
            return dt;
        }
    }
}
