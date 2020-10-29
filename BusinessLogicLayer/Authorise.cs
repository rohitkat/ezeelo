using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace BusinessLogicLayer
{
   public class Authorise : UserManagement
    {  
       public override string[] AuthorizedUserRight(System.Web.HttpServerUtility server, string ApplicationName, Int64 LoginID)
       {
           DataTable dt = new DataTable();
           ReadConfig config = new ReadConfig(server);
           DataAccessLayer.DbOperations dbOpr = new DataAccessLayer.GetData(config.DB_CONNECTION);
           List<object> paramValues = new List<object>();
           paramValues.Add(LoginID);
           paramValues.Add(ApplicationName);
           dt = dbOpr.GetRecords("AuthorizedUser", paramValues);

           int size = dt.Rows.Count * 6;

           string[] roleString = new string[size];
           int j = 0;

           for (int i = 0; i < dt.Rows.Count; i++)
           {
               roleString[j] = dt.Rows[i]["CanRead"].ToString();
               j++;
               roleString[j] = dt.Rows[i]["CanWrite"].ToString();
               j++;
               roleString[j] = dt.Rows[i]["CanDelete"].ToString();
               j++;
               roleString[j] = dt.Rows[i]["CanPrint"].ToString();
               j++;
               roleString[j] = dt.Rows[i]["CanExport"].ToString();
               j++;
               roleString[j] = dt.Rows[i]["CanImport"].ToString();
               j++;

           }

           return roleString;

       }
    }
}
