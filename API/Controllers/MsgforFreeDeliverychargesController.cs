using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace API.Controllers
{
    public class MsgforFreeDeliverychargesController : ApiController
    {
        public object Get(decimal GrandOrderAmount, long cityId, long FranchiseId)
        {
            object obj = new object();
            try
            {
                if (GrandOrderAmount == null || GrandOrderAmount <= 0 || cityId == null || cityId <= 0)
                {
                    obj = new { Success = 0, Message = "Please enter valid paramter.", data = string.Empty };
                }
                BusinessLogicLayer.DeliveryCharges dl = new BusinessLogicLayer.DeliveryCharges();
                string Msg = dl.CheckFreeDeliveryCharge(GrandOrderAmount, cityId, FranchiseId);
                if (!string.IsNullOrEmpty(Msg))
                {
                    obj = new { Success = 1, Message = "Success", data = Msg };
                }
                else
                {
                    obj = new { Success = 1, Message = "Success", data = string.Empty };
                }
            }
            catch (Exception ex)
            {
                obj = new { Success = 0, Message = ex.Message, data = string.Empty };
            }
            return obj;
        }
    }
}
