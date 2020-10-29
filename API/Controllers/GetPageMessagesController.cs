using API.Models;
using BusinessLogicLayer;
using ModelLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace API.Controllers
{
    public class GetPageMessagesController : ApiController
    {
        // GET api/pagemessages
        /* public IEnumerable<string> Get()
         {
             return new string[] { "value1", "value2" };
         }*/

        /// <summary>
        /// Get String Messages on Customer Page for Holidays
        /// </summary>
        /// <param name="FrnachiseId"></param>
        /// <returns></returns>
        [ApiException]
        // GET api/pagemessages/5
        public object Get(int FrnachiseId)
        {
            object obj = new object();
            try
            {
                if (FrnachiseId <= 0)
                {
                    return obj = new { Success = 0, Message = "Invalid parameter.", data = string.Empty };
                }
                List<WeeklySeasonalFestivalPageMessage> WSFMsg = new List<WeeklySeasonalFestivalPageMessage>();
                WSFMsg = FranchisePageMessages.GetFranchisePageMessage(FrnachiseId);
                if (WSFMsg.Count() > 0)
                {
                    obj = new { Success = 1, Message = "Successfull.", data = WSFMsg };
                    //obj = new { HTTPStatusCode = "200", UserMessage = "Successfull.", Data = WSFMsg };
                }
                else if (WSFMsg.Count() == 0)
                {
                    obj = new { Success = 0, Message = "No Record found.", data = string.Empty };
                    // obj = new { HTTPStatusCode = "204", UserMessage = "No Record found." };
                }
                else
                {
                    obj = new { Success = 0, Message = "Failed.", data = string.Empty };
                    // obj = new { HTTPStatusCode = "400", UserMessage = "Failed." };
                }
            }
            catch (Exception ex)
            {
                obj = new { Success = 0, Message = ex.Message, data = string.Empty };
            }
            return obj;
        }

        // POST api/pagemessages
        /* public void Post([FromBody]string value)
         {
         }

         // PUT api/pagemessages/5
         public void Put(int id, [FromBody]string value)
         {
         }

         // DELETE api/pagemessages/5
         public void Delete(int id)
         {
         }*/
    }
}
