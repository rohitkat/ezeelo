using API.Models;
//-----------------------------------------------------------------------
// <copyright file="BestDealsController" company="Ezeelo Consumer Services Pvt. Ltd.">
//     Copyright (c) Ezeelo Consumer Services Pvt. Ltd. All rights reserved.
// </copyright>
// <author>Sujata Kullarkar</author>
//-----------------------------------------------------------------------
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
    public class BestDealsController : ApiController
    {
        // GET api/bestdeals
        /// <summary>
        /// Get the list of all types of offers like flat discount,free offer,Component offer applicable as on date.
        /// </summary>
        /// <returns></returns>
        [ApiException]
        public object Get()
        {
            object obj = new object();
            try
            {
                PriceAndOffers prc = new PriceAndOffers(System.Web.HttpContext.Current.Server);
                var todayDeals = prc.GetTodaysDeals();
                if (todayDeals != null)
                {
                    obj = new { Success = 1, Message = "Todays offer list.", data = todayDeals };
                }
                else
                {
                    obj = new { Success = 1, Message = "Todays no offer present.", data = string.Empty };
                }
            }
            catch (Exception ex)
            {
                obj = new { Success = 0, Message = ex.Message, data = string.Empty };
            }
            return obj;

        }


        /// <summary>
        /// Get the list of all products(with their different stocks) valid for selected offer
        /// </summary>
        /// <param name="searchDealProducts">Object with members OfferID, OfferType etc. send CategoryID = 0 if you want to search the products in selected offer, for all categories.</param>
        /// <returns>List of product stocks</returns>
        [ApiException]
        [ValidateModel]
        // POST api/bestdeals
        public object Post(BestDealProductSearchViewModel searchDealProducts)
        {
            object obj = new object();
            try
            {
                if (searchDealProducts == null)
                    return obj = new { Success = 0, Message = "Enter valid parameter.", data = string.Empty };
                PriceAndOffers prc = new PriceAndOffers(System.Web.HttpContext.Current.Server);
                var result = prc.GetProducts(searchDealProducts);
                if (result != null)
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
