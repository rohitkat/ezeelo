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

namespace API.Controllers
{
    public class EPODController : ApiController
    {
       /* // GET api/epod
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/epod/5
        public string Get(int id)
        {
            return "value";
        }*/

        //--------------------------------Hide EPOD from Ashish for Live-------------------------------------------------------
        /*
        // POST api/epod
        /// <summary>
        /// Generate Electronic Proof Of Delivery for both merchant and customer 
        /// </summary>
        /// <param name="EpodMod"></param>
        /// <returns></returns>
        [LoginSuccess]
        [ApiException]
        [ValidateModel]
        public object Post(EPODModel EpodMod)
        {
            var re = Request;
            var Headers = re.Headers;
            string ReqBy = "";
            string IMEI = "";
            if (Headers.Contains("ReqBy") && Headers.Contains("IMEI"))
            {
                ReqBy = Headers.GetValues("ReqBy").First();
                IMEI = Headers.GetValues("IMEI").First();
            }
            EPODStaus lEPODStatus = new EPODStaus();
            EPODDetail lEPOD = new EPODDetail(System.Web.HttpContext.Current.Server);
            lEPODStatus = lEPOD.CreateElectronicSign(EpodMod, ReqBy, IMEI);
            object obj = new object();
            if (lEPODStatus.Status == "Successfull.")
                obj = new { HTTPStatusCode = "200", UserMessage = "Successfull." };
            else
                obj = new { HTTPStatusCode = "400", UserMessage = "Failed." };

            return obj;
        }*/ 

        //---------------------------------------------------------------------------------

       /* // PUT api/epod/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/epod/5
        public void Delete(int id)
        {
        }*/


    }
}
