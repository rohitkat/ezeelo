//-----------------------------------------------------------------------
// <copyright file="StockOfferController" company="Ezeelo Consumer Services Pvt. Ltd.">
//     Copyright (c) Ezeelo Consumer Services Pvt. Ltd. All rights reserved.
// </copyright>
// <author>Sujata Kullarkar</author>
//-----------------------------------------------------------------------
using ModelLayer.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using BusinessLogicLayer;
using API.Models; 
namespace API.Controllers
{
    public class StockOfferController : ApiController
    {
        // GET api/stockoffer
        /// <summary>
        /// List of different offers (flat discount, free, component etc offers) and their details.
        /// </summary>
        /// <param name="ssID">Shop stock ID</param>
        /// <returns>List of offers and their details</returns>
        [ApiException] 
        [ValidateModel]
        public object Get(long ssID)
        {
            PriceAndOffers lprod = new PriceAndOffers(System.Web.HttpContext.Current.Server);
             ProductOffersViewModel productOffers = lprod.GetStockOffers(ssID);
             object obj = new object();
             if (productOffers != null)
                 obj = new { HTTPStatusCode = "200", productOffers };
             else
                 obj = new { HTTPStatusCode = "412"  };
             return obj;
        }

    }
}
