using BusinessLogicLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace API.Controllers
{
    public class CheckDeliveryPincodeController : ApiController
    {
        [Route("api/CheckDeliveryPincode")]
        public object Get(string Pincode)
        {
            object obj = new object();
            try
            {
                if (string.IsNullOrEmpty(Pincode))
                    return obj = new { Success = 0, Message = "Invalid Parameter.", data = string.Empty };
                PincodeVerification pincodeVerification = new PincodeVerification();
                ShoppingCartInitialization sci = new ShoppingCartInitialization();
                bool isDeliverable = pincodeVerification.IsDeliverablePincode(Pincode); // 1
                if (isDeliverable)
                    obj = new { Success = 1, Message = "This items can be delivered at your location!!!", data = new { isDeliverable = isDeliverable } };
                else
                    obj = new { Success = 1, Message = "Sorry! This items can't be delivered at your location!!!", data = new { isDeliverable = isDeliverable } };
            }
            catch (Exception ex)
            {
                obj = new { Success = 0, Message = ex.Message, data = string.Empty };
            }
            return obj;
        }
    }
}
