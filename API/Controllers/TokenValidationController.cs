using ModelLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace API.Controllers
{
    public class TokenValidationController : ApiController
    {
        EzeeloDBContext db = new EzeeloDBContext();
        public object Get(string IMEI, string ApiToken)
        {
            object obj = new object();
            try
            {
                if (string.IsNullOrEmpty(IMEI) || string.IsNullOrEmpty(ApiToken))
                {
                    return obj = new { Success = 0, Message = "Enter valid input", data = string.Empty };
                }
                var row = db.APITokens.Where(x => x.IMEI == IMEI && x.TokenCode == ApiToken && x.IsActive == true).FirstOrDefault();
                if (row != null)
                {
                    obj = new { Success = 1, Message = "Success", data = string.Empty };
                }
                else
                {
                    obj = new { Success = 0, Message = "Please login again session is expired", data = string.Empty };
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
