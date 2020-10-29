using API.Models;
//-----------------------------------------------------------------------
// <copyright file="SubscriberDiscountOnCategoryController" company="Ezeelo Consumer Services Pvt. Ltd.">
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
    public class SubscriberDiscountOnCategoryController : ApiController
    {

        //public IEnumerable<SubscribedDiscountOnCategoryViewModel> Post(long UserLoginId, List<OrderDetailsCartShopStock> CartShopStockID)
       /// <summary>
       /// 
       /// </summary>
       /// <param name="UserLoginId"></param>
       /// <param name="CartShopStockID"></param>
       /// <returns></returns>
        public HttpResponseMessage Post(SubscriberDiscountOnShopStock subscriberDiscountOnShopStock)
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
           lOrderDetailsCartShopStock = (from n in subscriberDiscountOnShopStock.SubscriberDiscountOnShopStockList
                                         select new OrderDetailsCartShopStock
                                         {
                                             Quantity = n.Quantity,
                                             shopStockId = n.shopStockId
                                         }).ToList();


            return Request.CreateResponse(HttpStatusCode.OK, BusinessLogicLayer.SubscriptionCalculator.SubscriberDiscountOnCategory(subscriberDiscountOnShopStock.UserLoginId, lOrderDetailsCartShopStock));
            //return new string[] { "value1", "value2" };
        }
        //// GET api/subscriberdiscountoncategory
        //public IEnumerable<string> Get()
        //{
        //    return new string[] { "value1", "value2" };
        //}

        //// GET api/subscriberdiscountoncategory/5
        //public string Get(int id)
        //{
        //    return "value";
        //}

        //// POST api/subscriberdiscountoncategory
        //public void Post([FromBody]string value)
        //{
        //}

        //// PUT api/subscriberdiscountoncategory/5
        //public void Put(int id, [FromBody]string value)
        //{
        //}

        //// DELETE api/subscriberdiscountoncategory/5
        //public void Delete(int id)
        //{
        //}
    }
}
