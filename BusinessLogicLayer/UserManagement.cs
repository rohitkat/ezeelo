using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BusinessLogicLayer
{
    public class UserManagement : ISecurities
    {
        /// <summary>
        /// Data Base connection string
        /// </summary>
        private string _dbConnection = string.Empty;
       

       public UserManagement()
        {           
            this._dbConnection = ""; //retrieve db connection string here from configurations setings file
        }

        public bool IsAuthenticate(string pLoginName, string pPassword) { return false; }

        public List<string> GetAuthorisation(string pLoginName) { return new List<string>(); }

        public virtual string[] AuthorizedUserRight(System.Web.HttpServerUtility server, string ApplicationName, Int64 LoginID) { return null; }

        public virtual System.Data.DataTable AuthenticateAdmin(System.Web.HttpServerUtility server,string loginID, string pwd) { return null; }
       
    }
}
