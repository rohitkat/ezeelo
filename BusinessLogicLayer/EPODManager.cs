using ModelLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer
{
    //base class for EPOD Related Details
    public class EPODManager:IEPOD
    {
         protected System.Web.HttpServerUtility server;
        protected EzeeloDBContext db = new EzeeloDBContext();
        public EPODManager(System.Web.HttpServerUtility server)
        {
            this.server = server;        
        }
    }
}
