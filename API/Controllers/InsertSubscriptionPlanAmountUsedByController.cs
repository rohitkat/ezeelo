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
    public class InsertSubscriptionPlanAmountUsedByController : ApiController
    {
        public HttpResponseMessage Post(PlanAmountUsedBy planAmountUsedBy)
        {
            SubscriptionCalculator lSubscriptionCalculator = new SubscriptionCalculator(System.Web.HttpContext.Current.Server);
            var formatter = new JsonMediaTypeFormatter();
            var json = formatter.SerializerSettings;
            json.Converters.Add(new MyDateTimeConvertor());
            //lSubscriptionCalculator.IsUserSubscribed(UserLoginId,0);
            //int outResult = 0;
            //SubscribedDiscountOnCategoryViewModel lSubscribedDiscountOnCategory = new SubscribedDiscountOnCategoryViewModel();
            //ProductList productList = new ProductList(System.Web.HttpContext.Current.Server);
            // lSubscribedDiscountOnCategory = BusinessLogicLayer.SubscriptionCalculator.SubscriberDiscountOnCategory(UserLoginId, CartShopStockID);
            List<OrderDetailsCartShopStock> lOrderDetailsCartShopStock = new List<OrderDetailsCartShopStock>();
            lOrderDetailsCartShopStock = (from n in planAmountUsedBy.OrderDetailsCartShopStockList
                                          select new OrderDetailsCartShopStock
                                          {
                                              Quantity = n.Quantity,
                                              shopStockId = n.shopStockId
                                          }).ToList();
            List<SubscribedFacilityViewModel> lSubscribedFacilityViewModel = new List<SubscribedFacilityViewModel>();
            lSubscribedFacilityViewModel = (from n in planAmountUsedBy.SubscribedFacilityList
                                            select new SubscribedFacilityViewModel
                                            {
                                                ID = n.ID,
                                                Name = n.Name,
                                                BehaviorType = n.BehaviorType,
                                                FacilityValue = n.FacilityValue,

                                            }).ToList();
            bool result;
            result = (BusinessLogicLayer.SubscriptionCalculator.InsertSubscriptionPlanAmountUsedByAPI(planAmountUsedBy.UserLoginId, lOrderDetailsCartShopStock, planAmountUsedBy.CustOrderId, lSubscribedFacilityViewModel));
            if (result == true)
                return Request.CreateResponse(HttpStatusCode.OK, 1);
            else
                return Request.CreateResponse(HttpStatusCode.OK, 1);
            //return new string[] { "value1", "value2" };
        }
    }
}
