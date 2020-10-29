using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer
{
    /// <summary>
    /// Role Menu List For Authentication
    /// </summary>
    public class RoleMenuList
    {
        /// <summary>
        /// Select All Menus according to the role and module
        /// </summary>
        /// <param name="MasterMenu">ModuleName</param>
        /// <param name="RoleList">RoleID</param>
        /// <param name="server">Server Connection</param>
        /// <returns>MenuDatatable</returns>
        public DataTable SelectRoleMenu(int MasterMenu, int RoleList, System.Web.HttpServerUtility server)
        {
            DataTable dt = new DataTable();
            ReadConfig config = new ReadConfig(server);
            DataAccessLayer.DbOperations dbOpr = new DataAccessLayer.GetData(config.DB_CONNECTION);
            List<object> paramValues = new List<object>();
            paramValues.Add(RoleList);
            paramValues.Add(MasterMenu);
            
            dt = dbOpr.GetRecords("RoleMenuList", paramValues);

            return dt;
        }


    }
}
