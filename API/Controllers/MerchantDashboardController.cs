using API.Models;
//-----------------------------------------------------------------------
// <copyright file="MerchantDashboardController" company="Ezeelo Consumer Services Pvt. Ltd.">
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
    public class MerchantDashboardController : ApiController
    {
        public HttpResponseMessage Get(long UserLoginId)
        {
            MerchantApp lMerchantApp = new MerchantApp(System.Web.HttpContext.Current.Server);
            var formatter = new JsonMediaTypeFormatter();
            var json = formatter.SerializerSettings;
            json.Converters.Add(new MyDateTimeConvertor());
            List<MerchantDashboardViewModel> lMerchantDashboardViewModel = new List<MerchantDashboardViewModel>();
            lMerchantDashboardViewModel = BusinessLogicLayer.MerchantApp.DashboardDetails(UserLoginId);
            return Request.CreateResponse(HttpStatusCode.OK, lMerchantDashboardViewModel);
        }

    }
}