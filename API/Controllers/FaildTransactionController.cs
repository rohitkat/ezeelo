using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Mvc;

namespace API.Controllers
{
    public class FaildTransactionController : ApiController
    {
        public void POST(FormCollection frm)
        {
            object obj = new object();
            try
            {
                string Error = frm["error"].ToString();
                string ErrorMsg = frm["error_Message"].ToString();
                obj = new { Success = 0, Message = Error, data = ErrorMsg };
            }
            catch (Exception ex)
            {
                obj = new { Success = 0, Message = ex.Message, data = string.Empty };
            }
            // return obj;
            // return RedirectToRoute("PurchaseFailure", new { city = Request.Cookies["CityCookie"].Value.Split('$')[1].ToLower(), franchiseId = Convert.ToInt32(Request.Cookies["CityCookie"].Value.Split('$')[2]) });////added by Ashish for multiple MCO in same city

        }
    }
}
