using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer
{
    /// <summary>
    /// Shop Priority Class
    /// </summary>
    public class ShopPriorityList
    {
        /// <summary>
        /// To Fill data Table as per parameter
        /// </summary> 
        /// <param name="FranchiseList">franchise id</param>
        /// <param name="CategoryID">category id</param>
        /// <param name="Level">level</param>
        /// <param name="server">serverpath</param>
        /// <returns>datatable</returns>
        public DataTable Call_Select_Procedure(Int64 FranchiseList, Int64 CategoryID, int Level, System.Web.HttpServerUtility server)
        {
            DataTable dt = new DataTable();
            ReadConfig config = new ReadConfig(server);
            DataAccessLayer.DbOperations dbOpr = new DataAccessLayer.GetData(config.DB_CONNECTION);
            List<object> paramValues = new List<object>();
            paramValues.Add(FranchiseList);
            paramValues.Add(CategoryID);
            paramValues.Add(Level);

            dt = dbOpr.GetRecords("ShopPriorityList", paramValues);

            return dt;
        }
    }
}
