using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BusinessLogicLayer
{
    interface ISecurities : ICommonFuntionalities
    {
        string[] AuthorizedUserRight(System.Web.HttpServerUtility server, string ApplicationName, Int64 LoginID);
    }
}
