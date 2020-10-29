using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer
{
   public class CategorySpecificationListClass
    {
       public static DataTable Select_ProductSpecification(Int64? productID, Int64 categoryID,System.Web.HttpServerUtility server)
       {
           DataTable dt = new DataTable();
           ReadConfig config = new ReadConfig(server);
           DataAccessLayer.DbOperations dbOpr = new DataAccessLayer.GetData(config.DB_CONNECTION);
           List<object> paramValues = new List<object>();

           paramValues.Add(productID);
           paramValues.Add(categoryID);
           dt = dbOpr.GetRecords("Select_ProductSpecification", paramValues);

           return dt;
       
       }
       public static DataTable Select_ProductSpecificationAfterApproved(Int64? productID, Int64 categoryID, System.Web.HttpServerUtility server)
       {
           DataTable dt = new DataTable();
           ReadConfig config = new ReadConfig(server);
           DataAccessLayer.DbOperations dbOpr = new DataAccessLayer.GetData(config.DB_CONNECTION);
           List<object> paramValues = new List<object>();

           paramValues.Add(productID);
           paramValues.Add(categoryID);
           dt = dbOpr.GetRecords("Select_ProductSpecificationAfterApproved", paramValues);

           return dt;

       }
    }
}
