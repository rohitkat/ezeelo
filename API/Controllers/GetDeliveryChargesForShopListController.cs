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
    public class GetDeliveryChargesForShopListController : ApiController
    {
        
        // POST api/getdeliverychargesforshoplist
        /// <summary>
        /// Update Delivery charges against total weight of products in ashop, in the ShopWiseDeliveryCharges list.
        /// </summary>
        /// <param name="ShopListAndPincode">List of ShopWiseDeliveryCharges and Delivery Pincode</param>
        /// <returns>Updated list of ShopWiseDeliveryCharges</returns>

        [ValidateModel]
        [ApiException]
        public HttpResponseMessage  Post(GetShopWiseDeliveryChargesViewModel  ShopListAndPincode)
        {
            if (ShopListAndPincode.ShopWiseDelivery != null && ShopListAndPincode.Pincode != null && !(ShopListAndPincode.Pincode.Trim().Equals(string.Empty)))
            {
                //Valid request continue;

            }
            else
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { HTTPStatusCode = "400", UserMessage = "Invalid paramters", ValidationError = "Please check pincode and Shoplist." });

            DeliveryCharges ldelCharges = new DeliveryCharges();

            return Request.CreateResponse(HttpStatusCode.OK, ldelCharges.GetDeliveryCharges(ShopListAndPincode)); 
        }

      
    }
}
