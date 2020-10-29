using BusinessLogicLayer;
using ModelLayer.Models;
using ModelLayer.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace API.Controllers
{
    public class ReferSmsAndEmailController : ApiController
    {
        private EzeeloDBContext db = new EzeeloDBContext();
        // GET api/refersmsandemail
        //public IEnumerable<string> Get()
        //{
        //    return new string[] { "value1", "value2" };
        //}

        //// GET api/refersmsandemail/5
        //public string Get(int id)
        //{
        //    return "value";
        //}

        // POST api/refersmsandemail
        public object Post(ReferCustomerViewModel referdetailviewmodel)
        {
            object obj = new object();
            try
            {
                if (referdetailviewmodel.UID <= 0)
                {
                    return obj = new { Success = 0, Message = "User not login.", data = string.Empty };
                    // return obj = new { HTTPStatusCode = "400", UserMessage = "User not login", ValidationError = "Please login" };
                }
                if (referdetailviewmodel.lSubReferDetail == null || referdetailviewmodel.lSubReferDetail.Count <= 0 || referdetailviewmodel.ReferAndEarnSchemaID <= 0)
                {
                    return obj = new { Success = 0, Message = "Please insert valid details.", data = string.Empty };
                }
                foreach (var i in referdetailviewmodel.lSubReferDetail)
                {
                    string name = db.PersonalDetails.Where(x => x.UserLoginID == referdetailviewmodel.UID).Select(x => x.FirstName).FirstOrDefault();
                    ReferAndEarn lReferAndEarn = new ReferAndEarn();
                    lReferAndEarn.SendMailToCustomer(name, i.Email, i.Email);
                    lReferAndEarn.SendSMSToCustomer(name, i.Email, i.Mobile);
                }
                obj = new { Success = 1, Message = "SMS and Email send Successfully.", data = string.Empty };
                // return new { HTTPStatusCode = "200", UserMessage = "SMS and Email send Successfully." };
            }
            catch (Exception ex)
            {
                obj = new { Success = 0, Message = ex.Message, data = string.Empty };
            }
            return obj;
        }

        // PUT api/refersmsandemail/5
        //public void Put(int id, [FromBody]string value)
        //{
        //}

        //// DELETE api/refersmsandemail/5
        //public void Delete(int id)
        //{
        //}
    }
}
