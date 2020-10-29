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
    public class GetFranchiseAllottedLocationController : ApiController
    {
        // GET api/getfranchiseallottedpincode
       /* public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }*/

        // GET api/getfranchiseallottedpincode/5
        /// <summary>
        /// For Getting Franchise allotted Area with Pincode
        /// </summary>
        /// <param name="cityid"></param>
        /// <returns></returns>
         [ApiException]
      
        public object Get(int cityid)
        {
            List<FranchiseAreaPincode> FranchAreaPin = new List<FranchiseAreaPincode>();

            FranchAreaPin = FranchiseAllotedLocation.GetFranchiseAreaPincode(cityid);

            object obj = new object();
            if (FranchAreaPin.Count() > 0)
            {
                obj = new { HTTPStatusCode = "200", UserMessage = "Successfull.", Data = FranchAreaPin };
            }
            else if (FranchAreaPin.Count() == 0)
            {
                obj = new { HTTPStatusCode = "204", UserMessage = "No Record found." };
            }
            else
            {
                obj = new { HTTPStatusCode = "400", UserMessage = "Failed." };
            }


            return obj;
        }

        // POST api/getfranchiseallottedpincode
        /*public void Post([FromBody]string value)
        {
        }*/

        // PUT api/getfranchiseallottedpincode/5
       /* public void Put(int id, [FromBody]string value)
        {
        }*/

        // DELETE api/getfranchiseallottedpincode/5
       /* public void Delete(int id)
        {
        }*/
    }
}
