using API.Models;
//-----------------------------------------------------------------------
// <copyright file="GetIsUserSubscribedController" company="Ezeelo Consumer Services Pvt. Ltd.">
//     Copyright (c) Ezeelo Consumer Services Pvt. Ltd. All rights reserved.
// </copyright>
// <author>Mohit Sinha</author>
//-----------------------------------------------------------------------
using BusinessLogicLayer;
using ModelLayer.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Web.Http;

namespace API.Controllers
{
    public class GetIsUserSubscribedController : ApiController
    {

        public HttpResponseMessage Get(long UserLoginId)
        {
            //Calling BAL class
            SubscriptionCalculator lSubscriptionCalculator = new SubscriptionCalculator(System.Web.HttpContext.Current.Server);
            var formatter = new JsonMediaTypeFormatter();
            var json = formatter.SerializerSettings;
            json.Converters.Add(new MyDateTimeConvertor());
            //lSubscriptionCalculator.IsUserSubscribed(UserLoginId,0);
            int outResult=0;
            BusinessLogicLayer.SubscriptionCalculator.IsUserSubscribed(UserLoginId, ref outResult);


            return Request.CreateResponse(HttpStatusCode.OK, RetuenResult(outResult), formatter);

        }

        public static int RetuenResult(int outResult)
        {
            switch (outResult)
            {
                case 100: return 100;
                case 101: return 101;
                case 102: return 102;
                case 103: return 103;

            }

            return 0;

        }

        //// GET api/subscriptioncalculator
        //public IEnumerable<string> Get()
        //{
        //    return new string[] { "value1", "value2" };
        //}

        //// GET api/subscriptioncalculator/5
        //public string Get(int id)
        //{
        //    return "value";
        //}

        //// POST api/subscriptioncalculator
        //public void Post([FromBody]string value)
        //{
        //}

        //// PUT api/subscriptioncalculator/5
        //public void Put(int id, [FromBody]string value)
        //{
        //}

        //// DELETE api/subscriptioncalculator/5
        //public void Delete(int id)
        //{
        //}
    }
}
