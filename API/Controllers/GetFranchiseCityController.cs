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
    public class GetFranchiseCityController : ApiController
    {
        // GET api/getfranchisecity
        //public IEnumerable<string> Get()
        //{
        //    return new string[] { "value1", "value2" };
        //}

        // GET api/getfranchisecity/5
        /// <summary>
        /// Provide list of cities and its franchise on load page in mobile
        /// </summary>
        /// <returns></returns>
         [ApiException]
        public object Get()
        {
            List<FranchiseCity> franchcity = new List<FranchiseCity>();
            
             franchcity = FranchiseAndCity.GetFranchiseCities();

            object obj = new object();
            if (franchcity.Count() > 0)
            {
                obj = new { HTTPStatusCode = "200", UserMessage = "Successfull.", Data = franchcity };
            }
            else if (franchcity.Count() == 0)
            {
                obj = new { HTTPStatusCode = "204", UserMessage = "No Record found." };
            }
            else
            {
                obj = new { HTTPStatusCode = "400", UserMessage = "Failed." };
            }


            return obj;
           // return "value";
        }

        // POST api/getfranchisecity
        //public void Post([FromBody]string value)
        //{
        //}

        // PUT api/getfranchisecity/5
        //public void Put(int id, [FromBody]string value)
        //{
        //}

        // DELETE api/getfranchisecity/5
        //public void Delete(int id)
        //{
        //}
    }
}
