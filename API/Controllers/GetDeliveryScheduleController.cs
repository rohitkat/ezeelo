using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace API.Controllers
{

    public class GetDeliveryScheduleController : ApiController
    {
        // GET api/getdeliveryschedulecontroller
        /// <summary>
        /// Get Delivery Schedule 
        /// </summary>
        /// <returns></returns>
        public object Get(Int64 cityID, string pincode, int? franchiseID = null, int? Version = null)////Added Int? FranchiseID for Multiple MCO and int? Version=null for New App
        {
            object obj = new object();
            try
            {
                //-- For Differentiate Old and New APP --//
                if (Version == null)
                { franchiseID = null; }

                BusinessLogicLayer.DeliveryScheduleBLL objMethode = new BusinessLogicLayer.DeliveryScheduleBLL(System.Web.HttpContext.Current.Server);
                List<ModelLayer.Models.ViewModel.APIDeliveryScheduleViewModel> DeliveryScheduleList = new List<ModelLayer.Models.ViewModel.APIDeliveryScheduleViewModel>();
                DeliveryScheduleList = objMethode.Select_DeliverySchedule(cityID, pincode, franchiseID);////added franchiseID for Multiple MCO
                if (DeliveryScheduleList != null || DeliveryScheduleList.Count > 0)
                    obj = new { Success = 1, Message = "Delivery Schedule found.", data = DeliveryScheduleList };
                else
                    obj = new { Success = 1, Message = "Delivery Schedule not found.", data = string.Empty };

            }
            catch (Exception ex)
            {
                obj = new { Success = 0, Message = ex.Message, data = string.Empty };
            }
            return obj;
        }
    }
}
