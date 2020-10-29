using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace BusinessLogicLayer
{
    /// <summary>
    /// Check User Authentication 
    /// </summary>
    public class Authenticate : UserManagement
    {
        /// <summary>
        /// To Check User Authetication 
        /// </summary>
        /// <param name="server">DataConnection</param>
        /// <param name="loginID">Email/Mobile No</param>
        /// <param name="pwd">password</param>
        /// <returns></returns>
        public override System.Data.DataTable AuthenticateAdmin(System.Web.HttpServerUtility server,string loginID, string pwd)
        {
            DataTable dt = new DataTable();
            ReadConfig config = new ReadConfig(server);
            DataAccessLayer.DbOperations dbOpr = new DataAccessLayer.GetData(config.DB_CONNECTION);
            List<object> paramValues = new List<object>();
            paramValues.Add(loginID);
            paramValues.Add(pwd);
            dt = dbOpr.GetRecords("CheckLoginAdmin", paramValues);

            return dt;
        }
    }
}
