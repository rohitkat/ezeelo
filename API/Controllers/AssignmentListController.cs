
using API.Models;
using BusinessLogicLayer;
using ModelLayer.Models.ViewModel;
using ModelLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using System.Data.Entity;
using Newtonsoft.Json;

namespace API.Controllers
{
    public class AssignmentListController : ApiController
    {
        // GET api/assignmentlist
       /* public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }*/

        // GET api/assignmentlist/5

        //----------------------------Hide EPOD from Ashish for Live---------------------------------------------
        /*
        /// <summary>
        /// Provide List of Assigned Task as per User Login
        /// </summary>
        /// <param name="userloginId"></param>
        /// <returns></returns>
        [ApiException]
        public object Get(int userloginId)
        {
            List<EmployeeAssignmentList> empassign = new List<EmployeeAssignmentList>();
           empassign= Assignment.GetAssignList(userloginId);
           //var jsonData = JsonConvert.SerializeObject(empassign);
             object obj = new object();
             if (empassign.Count() > 0)
             {
                 obj = new { HTTPStatusCode = "200", UserMessage = "Successfull.", Data = empassign };
             }
             else if (empassign.Count() == 0)
             {
                 obj = new { HTTPStatusCode = "204", UserMessage = "No Record found." };
             }
             else
             {
                 obj = new { HTTPStatusCode = "400", UserMessage = "Failed." }; 
             }
          

            return obj;
        }*/  
        //------------------------------------------------------------------------

       /* // POST api/assignmentlist
        public void Post([FromBody]string value)
        {
        }

        // PUT api/assignmentlist/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/assignmentlist/5
        public void Delete(int id)
        {
        }*/
    }
}
