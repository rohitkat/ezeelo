using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer
{
    public class SitemapObjectList
    {
        public DataTable Select_FranchiseProductList(Int32 FranchiseID, System.Web.HttpServerUtility server) ////added Int64CityID-> Int32 FranchiseID
        {
            DataTable dt = new DataTable();
            try
            {
                ReadConfig config = new ReadConfig(server);
                DataAccessLayer.DbOperations dbOpr = new DataAccessLayer.GetData(config.DB_CONNECTION);
                List<object> paramValues = new List<object>();
                paramValues.Add(FranchiseID);////added CityID->FranchiseID
                dt = dbOpr.GetRecords("Select_FranchiseProductsList", paramValues);
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[SitemapObjectList][Select_FranchiseProductList]",
                    BusinessLogicLayer.ErrorLog.Module.BussinessLogicLayer, System.Web.HttpContext.Current.Server);
            }
            return dt;
        }

        public DataTable Select_FranchiseShopList(Int32 FranchiseID, System.Web.HttpServerUtility server)////added Int64CityID-> Int32 FranchiseID
        {
            DataTable dt = new DataTable();
            try
            {
                ReadConfig config = new ReadConfig(server);
                DataAccessLayer.DbOperations dbOpr = new DataAccessLayer.GetData(config.DB_CONNECTION);
                List<object> paramValues = new List<object>();
                paramValues.Add(FranchiseID);////added CityID->FranchiseID
                dt = dbOpr.GetRecords("Select_FranchiseShopsList", paramValues);
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[SitemapObjectList][Select_FranchiseShopList]",
                    BusinessLogicLayer.ErrorLog.Module.BussinessLogicLayer, System.Web.HttpContext.Current.Server);
            }
            return dt;
        }
    }
}
