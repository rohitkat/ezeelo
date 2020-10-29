using BusinessLogicLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace API.Controllers
{
    public class GetHelpLineNumberController : ApiController
    {
        // GET api/gethelplinenumber
        /* public IEnumerable<string> Get()
         {
             return new string[] { "value1", "value2" };
         }*/


        public object Get(int franchiseid)
        {
            List<HelpLine> helpLineNo = new List<HelpLine>();

            helpLineNo = HelpLineDesk.GetHelpLineNumber(franchiseid);

            object obj = new object();
            if (helpLineNo.Count() > 0)
            {
                obj = new { HTTPStatusCode = "200", UserMessage = "Successfull.", Data = helpLineNo };
            }
            else if (helpLineNo.Count() == 0)
            {
                obj = new { HTTPStatusCode = "204", UserMessage = "No Record found." };
            }
            else
            {
                obj = new { HTTPStatusCode = "400", UserMessage = "Failed." };
            }


            return obj;
        }

        // GET api/gethelplinenumber/5
        [HttpGet]
        [Route("api/GetHelpLineNumber/GetNumber")]
        public object GetNumber(int franchiseid)
        {
            object obj = new object();
            try
            {
                if (franchiseid == null || franchiseid <= 0)
                {
                    return obj = new { Success = 0, Message = "Enter valid parameter.", data = string.Empty };
                }
                List<HelpLine> helpLineNo = new List<HelpLine>();
                helpLineNo = HelpLineDesk.GetHelpLineNumber(franchiseid);
                if (helpLineNo.Count() > 0)
                {
                    obj = new { Success = 1, Message = "Successfull.", data = helpLineNo };
                }
                else if (helpLineNo.Count() == 0)
                {
                    obj = new { Success = 1, Message = "No Record found.", data = string.Empty };
                }
                else
                {
                    obj = new { Success = 0, Message = "Failed.", data = string.Empty };
                }
            }
            catch (Exception ex)
            {
                obj = new { Success = 0, Message = ex.Message, data = string.Empty };
            }
            return obj;
        }

        // POST api/gethelplinenumber
        /* public void Post([FromBody]string value)
         {
         }*/

        // PUT api/gethelplinenumber/5
        /* public void Put(int id, [FromBody]string value)
         {
         }*/

        // DELETE api/gethelplinenumber/5
        /*public void Delete(int id)
        {
        }*/
    }
}
