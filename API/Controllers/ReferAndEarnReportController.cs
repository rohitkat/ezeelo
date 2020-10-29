using API.Models;
using BusinessLogicLayer;
using ModelLayer.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace API.Controllers
{
    public class ReferAndEarnReportController : ApiController
    {
        //// GET api/referandearnreport
        //public IEnumerable<string> Get()
        //{
        //    return new string[] { "value1", "value2" };
        //}


        //[TokenVerification] //commented by Ashwini Meshram 12-Dec-2016
        [ApiException]
        // [LoginSuccess]
        //[ValidateModel]
        // GET api/referandearnreport/5
        public object Get(long uid, long cityId)
        {
            object obj = new object();
            try
            {
                if (uid == null || uid <= 0)
                {
                    return obj = new { Success = 0, Message = "User not login.", data = string.Empty };
                    // obj = new { HTTPStatusCode = "400", UserMessage = "User not login", ValidationError = "Please login" };
                    //  return Request.CreateResponse(HttpStatusCode.BadRequest, obj);
                }
                if (cityId == null || cityId <= 0)
                {
                    return obj = new { Success = 0, Message = "Enter valid CityId.", data = string.Empty };
                }
                ReferAndEarn lReferAndEarn = new ReferAndEarn();
                EarnAndReferReportViewModelDetails lobj = new EarnAndReferReportViewModelDetails();
                lobj = lReferAndEarn.CustomerReferAndEarnReport(uid, cityId);
                if (lobj != null)
                {
                    obj = new { Success = 1, Message = "Records are found.", data = lobj };
                }
                else
                {
                    obj = new { Success = 1, Message = "Records are not found.", data = string.Empty };
                }
            }
            catch (Exception ex)
            {
                obj = new { Success = 0, Message = ex.Message, data = string.Empty };
            }
            return obj;
        }

        //// POST api/referandearnreport
        //public void Post([FromBody]string value)
        //{
        //}

        //// PUT api/referandearnreport/5
        //public void Put(int id, [FromBody]string value)
        //{
        //}

        //// DELETE api/referandearnreport/5
        //public void Delete(int id)
        //{
        //}
    }
}
