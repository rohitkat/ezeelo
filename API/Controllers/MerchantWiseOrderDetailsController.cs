using API.Models;
//-----------------------------------------------------------------------
// <copyright file="MerchantWiseOrderDetailsController" company="Ezeelo Consumer Services Pvt. Ltd.">
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
    public class MerchantWiseOrderDetailsController : ApiController
    {
        public HttpResponseMessage Get(long UserLoginId, int OrderStatus, long OrderId)
        {
            MerchantApp lMerchantApp = new MerchantApp(System.Web.HttpContext.Current.Server);
            var formatter = new JsonMediaTypeFormatter();
            var json = formatter.SerializerSettings;
            json.Converters.Add(new MyDateTimeConvertor());
            List<MerchantAppOrderDetailsViewModel> lMerchantAppOrderDetailsViewModel = new List<MerchantAppOrderDetailsViewModel>();
            lMerchantAppOrderDetailsViewModel = BusinessLogicLayer.MerchantApp.OrderDetails(UserLoginId, OrderStatus, OrderId);
            return Request.CreateResponse(HttpStatusCode.OK, lMerchantAppOrderDetailsViewModel);
        }

    }
}