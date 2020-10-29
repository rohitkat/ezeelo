using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer
{
    public class GetDataFromDB
    {
        public DataTable Call_GetDataFromDB_Procedure(long ID, string Code, string Code2, DateTime Date, int Mode, System.Web.HttpServerUtility server)
        {
            DataTable dt = new DataTable();
            ReadConfig config = new ReadConfig(server);
            DataAccessLayer.DbOperations dbOpr = new DataAccessLayer.GetData(config.DB_CONNECTION);
            List<object> paramValues = new List<object>();
            paramValues.Add(ID);
            paramValues.Add(Code);
            paramValues.Add(Code2);
            paramValues.Add(Date);
            paramValues.Add(Mode);

            dt = dbOpr.GetRecords("GetData", paramValues);

            return dt;
        }
    }
}
