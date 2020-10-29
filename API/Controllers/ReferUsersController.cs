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
    public class ReferUsersController : ApiController
    {
        // GET api/referusers
        public object Get(long franchiseId)//Sonali_for_CityId_03-11-2018
        {
            object obj = new object();
            try
            {
                if (franchiseId == null || franchiseId <= 0)//Sonali_for_CityId_03-11-2018
                {
                    return obj = new { Success = 0, Message = "Enter valid cityid.", data = string.Empty };
                }
                ReferAndEarnSchemaName lRefereAndEarnSchema = new ReferAndEarnSchemaName();
                ReferAndEarn lReferAndEarn = new ReferAndEarn();
                lRefereAndEarnSchema = lReferAndEarn.GetReferSchemeName(franchiseId);//Sonali_for_CityId_03-11-2018
                if (lRefereAndEarnSchema != null)
                {
                    obj = new { Success = 1, Message = "Refer and Earn scheme are found for particular city.", data = lRefereAndEarnSchema };
                }
                else
                {
                    obj = new { Success = 1, Message = "Refer and Earn scheme are not found for particular city.", data = string.Empty };
                }

                //return Request.CreateResponse(HttpStatusCode.OK, lRefereAndEarnSchema);
            }
            catch (Exception ex)
            {
                obj = new { Success = 0, Message = ex.Message, data = string.Empty };
            }
            //List<ReferAndEarnSchemaName> lRefereAndEarnSchema = new List<ReferAndEarnSchemaName>();
            //ReferAndEarn lReferAndEarn = new ReferAndEarn();
            //lRefereAndEarnSchema = lReferAndEarn.GetReferSchemeName();
            return obj;
        }

        //// GET api/referusers/5
        //public string Get(int id)
        //{
        //    return "value";
        //}
        // [TokenVerification]
        [ApiException]
        // [LoginSuccess]
        //[ValidateModel]
        // POST api/referusers
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
                ReferAndEarn lReferAndEarn = new ReferAndEarn();
                referdetailviewmodel = lReferAndEarn.InsertReferDetail(referdetailviewmodel);
                if (referdetailviewmodel != null)
                {
                    obj = new { Success = 1, Message = "Referdetail successfully insert.", data = referdetailviewmodel };
                }
                else
                {
                    obj = new { Success = 1, Message = "Referdetail not insert.", data = string.Empty };
                }
                // return Request.CreateResponse(HttpStatusCode.OK, referdetailviewmodel);
            }
            catch (Exception ex)
            {
                obj = new { Success = 0, Message = ex.Message, data = string.Empty };
            }
            return obj;
        }


    }
}
