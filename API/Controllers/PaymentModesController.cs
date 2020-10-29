using API.Models;
using ModelLayer.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace API.Controllers
{
    public class PaymentModesController : ApiController
    {

        // GET api/paymentmodes
        /// <summary>
        /// Get list of payment modes available by shop while processing order.
        /// </summary>
        /// <param name="shopList">List of ShopID's</param>
        /// <returns>List of payment modes</returns>
        ///  -----------------changes by Sonali Warhade 07-09-2018 For api return format change--------------------//
        [ApiException]
        [ValidateModel]
        public object Post(ShopListViewModel shopList)
        {
            object obj = new object();
            try
            {
                if (shopList == null || shopList.shopList.Count() == 0)
                {
                    return obj = new { Success = 0, Message = "Please provide the list of shops.", data = string.Empty };
                    //ModelState.AddModelError("MissingRequiredQueryParameter", "Please provide the list of shops.");
                }
                AvailablePaymentModesViewModel apm = new AvailablePaymentModesViewModel();
                List<string> payModeList = new List<string>();
                BusinessLogicLayer.OrderPaymentModes orderPayment = new BusinessLogicLayer.OrderPaymentModes();
                apm = orderPayment.GetPaymentModes(shopList);
                if (apm.AvailablePaymentModeList != null && apm.AvailablePaymentModeList.Count > 0)
                {
                    obj = new { Success = 1, Message = "Paymentmode list found.", data = apm };
                }
                else
                {
                    obj = new { Success = 1, Message = "Paymentmode list not found.", data = string.Empty };
                }
            }
            catch (Exception ex)
            {
                obj = new { Success = 0, Message = ex.Message, data = string.Empty };
            }
            return obj;
        }
        //-----------------------------------------------------------------------------------------//

    }
}
