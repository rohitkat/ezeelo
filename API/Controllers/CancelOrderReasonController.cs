using ModelLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace API.Controllers
{
    public class CancelOrderReasonController : ApiController
    {
        protected EzeeloDBContext db = new EzeeloDBContext();
        public object Get()
        {
            object obj = new object();
            try
            {
                 List<CancelOrderReason> CancelOrderReasons = db.CancelOrderReason.ToList();
                // List<CancelOrderReason> CancelOrderReasons = new List<CancelOrderReason>();
                if (CancelOrderReasons != null && CancelOrderReasons.Count > 0)
                {
                    obj = new { Success = 1, Message = "Successfull", data = CancelOrderReasons };
                }
                else
                {
                    obj = new { Success = 0, Message = "No Reasons found", data = string.Empty };
                }
            }
            catch (Exception ex)
            {
                obj = new { Success = 0, Message = ex.Message, data = string.Empty };
            }
            return obj;
        }
    }
}
