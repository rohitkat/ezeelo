using API.Models;
//-----------------------------------------------------------------------
// <copyright file="DeliveryChargeController" company="Ezeelo Consumer Services Pvt. Ltd.">
//     Copyright (c) Ezeelo Consumer Services Pvt. Ltd. All rights reserved.
// </copyright>
// <author>Sujata Kullarkar</author>
//-----------------------------------------------------------------------
using BusinessLogicLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace API.Controllers
{
    public class DeliveryChargeController : ApiController
    {
        /// <summary>
        /// Get Delivery Charges for total weight of product against each shop involved in order.
        /// </summary>
        /// <param name="pinCode">Shipping Pincode</param>
        /// <param name="totalWeight">Total Weight</param>
        /// <param name="isExpress">Delivery Type Express/Normal</param>
        /// <returns></returns>
        [ApiException] 
        [ValidateModel]
        // GET api/deliverycharge
        public object Get(string pinCode, decimal totalWeight,bool isExpress)
        {
            DeliveryCharges ldelCharges = new DeliveryCharges();
            return new { HTTPStatusCode = "200", DeliveryCharges = ldelCharges.GetDeliveryCharges(pinCode, totalWeight, isExpress) };
            
        }

        
    }
}
