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
    public class GetCoupenDetailsForShopListController : ApiController
    {
       

        // POST api/getcoupendetailsforshoplist
        /// <summary>
        /// Get Coupen details, and ID of shop for which coupen is applied if IsFreeDelivery flag in CouponDetails is true, then 
        /// Delivery Charges for that shop will be zero(free delivery)
        /// </summary>
        /// <param name="ShopListAndCoupen">Shop list included in Cart and Coupen code applied against cart.</param>
        /// <returns>Coupen </returns>
      [ValidateModel]
        [ApiException]
        public HttpResponseMessage Post(CoupenCodeOnShopListViewModel ShopListAndCoupen)
        {
            
            double orderTot = 0.0;
            if (ShopListAndCoupen.ShopWiseDelivery != null && ShopListAndCoupen.CoupenCode != null && !(ShopListAndCoupen.CoupenCode.Trim().Equals(string.Empty)))
            {
                //Valid request continue;

            }
            else
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { HTTPStatusCode = "400", UserMessage = "Invalid paramters", ValidationError = "Please Enter valid coupenCode and provide valid shoplist." });
         
            List<ShopWiseDeliveryCharges> shopList = new List<ShopWiseDeliveryCharges>();
            shopList = ShopListAndCoupen.ShopWiseDelivery;

                CouponManagement couponManagement = new CouponManagement();
                CouponDetailsViewModel objCouponDetails = new CouponDetailsViewModel();
                GetShopWiseVoucherAmountViewModel GetCoupenDetails = new GetShopWiseVoucherAmountViewModel();
            foreach (var item in shopList)
            {
                orderTot = orderTot + Convert.ToDouble(item.OrderAmount);
            }

            //-- For Differentiate Old and New APP --//
            if (ShopListAndCoupen.Version == null)
            { ShopListAndCoupen.FranchiseID = null; }

                if (ShopListAndCoupen.CustomerLoginID  <= 0)
                    objCouponDetails = couponManagement.CheckCouponCodeOnBillAmount(ShopListAndCoupen.CoupenCode, ShopListAndCoupen.ShopWiseDelivery.FirstOrDefault().ShopID, orderTot);
                else
                    objCouponDetails = couponManagement.CheckCouponCodeOnBillAmount(ShopListAndCoupen.CoupenCode, ShopListAndCoupen.ShopWiseDelivery.FirstOrDefault().ShopID, orderTot, ShopListAndCoupen.CustomerLoginID, ShopListAndCoupen.CityID, ShopListAndCoupen.FranchiseID);////added ShopListAndCoupen.FranchiseID for Multiple MCO

                if (objCouponDetails.Result == 1)
                {

                    GetCoupenDetails.CoupenDetails = objCouponDetails;
                    GetCoupenDetails.ShopIDForFreeDelivery = ShopListAndCoupen.ShopWiseDelivery.FirstOrDefault().ShopID;
                }
                //Added by Tejaswee (25-2-2016)
                //For returning respective msg if voucher not valid
                else
                {
                    GetCoupenDetails.CoupenDetails = objCouponDetails;
                    GetCoupenDetails.ShopIDForFreeDelivery = 0;
                }
           

            return Request.CreateResponse(HttpStatusCode.OK, GetCoupenDetails); 
        }

    }
}
