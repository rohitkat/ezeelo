using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer
{
    /// <summary>
    /// 
    /// </summary>
    public class CompareProduct_detail
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ProductID"></param>
        /// <param name="server"></param>
        /// <returns></returns>
        public DataTable Select_CompareProduct(Int64 ProductID,  System.Web.HttpServerUtility server)
        {
            DataTable dt = new DataTable();
            ReadConfig config = new ReadConfig(server);
            DataAccessLayer.DbOperations dbOpr = new DataAccessLayer.GetData(config.DB_CONNECTION);
            List<object> paramValues = new List<object>();           
            paramValues.Add(ProductID);

            dt = dbOpr.GetRecords("Select_CompareProduct", paramValues);

            return dt;
        }
    }
}
