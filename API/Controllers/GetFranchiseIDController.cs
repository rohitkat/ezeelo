using BusinessLogicLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace API.Controllers
{
    public class GetFranchiseIDController : ApiController
    {
        // GET api/getfranchiseid
        /* public IEnumerable<string> Get()
         {
             return new string[] { "value1", "value2" };
         }*/

        public object Get(int cityid, int pincide_areaid)
        {

            List<FranchisIdList> FranchId = new List<FranchisIdList>();

            FranchId = FranchiseID.GetFranchiseID(cityid, pincide_areaid);

            object obj = new object();
            if (FranchId.Count() > 0)
            {
                obj = new { HTTPStatusCode = "200", UserMessage = "Successfull.", Data = FranchId };
            }
            else if (FranchId.Count() == 0)
            {
                obj = new { HTTPStatusCode = "204", UserMessage = "No Record found." };
            }
            else
            {
                obj = new { HTTPStatusCode = "400", UserMessage = "Failed." };
            }


            return obj;
        }

        // GET api/getfranchiseid/5
        [HttpGet]
        [Route("api/GetFranchiseID/GetFranchise")]
        public object GetFranchise(int cityid, int pincide_areaid)
        {
            object obj = new object();
            try
            {
                if (cityid <= 0 || pincide_areaid <= 0)
                {
                    return obj = new { Success = 0, Message = "Enter valid data.", data = string.Empty };
                }
                List<FranchisIdList> FranchId = new List<FranchisIdList>();
                FranchId = FranchiseID.GetFranchiseID(cityid, pincide_areaid);
                if (FranchId.Count() > 0)
                {
                    obj = new { Success = 1, Message = "Successfull.", data = FranchId };
                    // obj = new { HTTPStatusCode = "200", UserMessage = "Successfull.", Data = FranchId };
                }
                else if (FranchId.Count() == 0)
                {
                    obj = new { Success = 1, Message = "No Record found.", data = string.Empty };
                    // obj = new { HTTPStatusCode = "204", UserMessage = "No Record found." };
                }
                else
                {
                    obj = new { Success = 0, Message = "Failed.", data = string.Empty };
                    //obj = new { HTTPStatusCode = "400", UserMessage = "Failed." };
                }
            }
            catch (Exception ex)
            {
                obj = new { Success = 0, Message = ex.Message, data = string.Empty };
            }
            return obj;
        }

        // POST api/getfranchiseid
        /* public void Post([FromBody]string value)
         {
         }*/

        // PUT api/getfranchiseid/5
        /* public void Put(int id, [FromBody]string value)
         {
         }*/

        // DELETE api/getfranchiseid/5
        /*public void Delete(int id)
        {
        }*/
    }
}
