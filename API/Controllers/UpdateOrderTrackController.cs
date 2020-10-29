using API.Models;
//-----------------------------------------------------------------------
// <copyright file="InsertSubscriptionPlanAmountUsedByController" company="Ezeelo Consumer Services Pvt. Ltd.">
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
    public class UpdateOrderTrackController : ApiController
    {
        public HttpResponseMessage Get(long UserLoginId, int OrderStatus, long OrderId)
        {
            MerchantApp lMerchantApp = new MerchantApp(System.Web.HttpContext.Current.Server);
            var formatter = new JsonMediaTypeFormatter();
            var json = formatter.SerializerSettings;
            json.Converters.Add(new MyDateTimeConvertor());
            int outResult = 0;
            BusinessLogicLayer.MerchantApp.UpdateOrderTrack(UserLoginId, OrderStatus, OrderId, ref outResult);
            return Request.CreateResponse(HttpStatusCode.OK, RetuenResult(outResult), formatter);
            //if (result == true)
            //    //return new { HTTPStatusCode = "200", UserMessage = "Record Update Successfully.",D };
            //    return Request.CreateResponse(HttpStatusCode.OK, RetuenResult(outResult), formatter);    
            //    //return Request.CreateResponse(HttpStatusCode.OK);
            //else
            //return Request.CreateResponse(HttpStatusCode.InternalServerError, result, formatter); 
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
    }
}
