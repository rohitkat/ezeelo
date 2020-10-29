using API.Models;
//-----------------------------------------------------------------------
// <copyright file="SubscribedFacilityController" company="Ezeelo Consumer Services Pvt. Ltd.">
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
    public class SubscribedFacilityController : ApiController
    {
        public HttpResponseMessage Post(long UserLoginId)
        {
            SubscriptionCalculator lSubscriptionCalculator = new SubscriptionCalculator(System.Web.HttpContext.Current.Server);
            var formatter = new JsonMediaTypeFormatter();
            var json = formatter.SerializerSettings;
            json.Converters.Add(new MyDateTimeConvertor());
            List<SubscribedFacilityViewModel> lSubscribedFacilityViewModel = new List<SubscribedFacilityViewModel>();
            lSubscribedFacilityViewModel = BusinessLogicLayer.SubscriptionCalculator.SubscribedFacility(UserLoginId);
            return Request.CreateResponse(HttpStatusCode.OK, lSubscribedFacilityViewModel);
            //return new string[] { "value1", "value2" };
        }
    }
}
