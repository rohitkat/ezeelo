//-----------------------------------------------------------------------
// <copyright file="TrackShipmentController" company="Ezeelo Consumer Services Pvt. Ltd.">
//     Copyright (c) Ezeelo Consumer Services Pvt. Ltd. All rights reserved.
// </copyright>
// <author>Sujata Kullarkar</author>
//-----------------------------------------------------------------------
using ModelLayer.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using BusinessLogicLayer;
using API.Models;
using System.Net.Http.Formatting;

namespace API.Controllers
{
    public class TrackShipmentController : ApiController
    {
        // GET api/trackshipment
        /// <summary>
        /// Track customer’s order shipment. It displays the status(1: Placed; 2:Confirm; 3: Packed;
        /// 4:Dispatched from Shop; 7:Delivered; 8: Returned; 9: Cancelled) of Shipment at perticular time.
        /// </summary>
        /// <param name="orderID">Order ID</param>
        /// <param name="shopStockID">Shop Stock ID</param>
        /// <returns>Shipment History</returns>
        /*Order can be tracked, without logged-in.
         [TokenVerification]   */
        [ApiException]
        [ValidateModel]
        public object Get(long orderID, long shopStockID)
        {
            object obj = new object();
            try
            {
                if (orderID <= 0 || shopStockID <= 0)
                {
                    return obj = new { Success = 0, Message = "Enter valid parameter.", data = string.Empty };
                }
                TrackCustomerOrder lTrackOrder = new TrackCustomerOrder(System.Web.HttpContext.Current.Server);
                //var formatter = new JsonMediaTypeFormatter();
                //var json = formatter.SerializerSettings;
                //json.Converters.Add(new MyDateTimeConvertor());
                //return Request.CreateResponse(HttpStatusCode.OK, lTrackOrder.GetOrderProductHistory(orderID, shopStockID), formatter);
                var result = lTrackOrder.GetOrderProductHistory(orderID, shopStockID);
                if (result != null && result.Count > 0)
                    obj = new { Success = 1, Message = "Record found.", data = result };
                else
                    obj = new { Success = 1, Message = "No Record found.", data = string.Empty };
            }
            catch (Exception ex)
            {
                obj = new { Success = 0, Message = ex.Message, data = string.Empty };
            }
            return obj;
        }

    }
}
