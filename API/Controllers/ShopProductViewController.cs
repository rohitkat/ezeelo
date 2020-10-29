//-----------------------------------------------------------------------
// <copyright file="ShopProductViewController" company="Ezeelo Consumer Services Pvt. Ltd.">
//     Copyright (c) Ezeelo Consumer Services Pvt. Ltd. All rights reserved.
// </copyright>
// <author>Sujata Kullarkar</author>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using BusinessLogicLayer;
using ModelLayer.Models.ViewModel;
using API.Models;
namespace API.Controllers
{
    public class ShopProductViewController : ApiController
    {
        // GET api/stockvarient/4/5
        /// <summary>
        /// Get Shop Product details
        /// </summary>
        /// <param name="ProductID">Product ID</param>
        /// <param name="shopID">Shop ID</param>
        /// <returns>product details with varients available for shop.</returns>
         [ApiException] 
        [ValidateModel]
        public IEnumerable<ShopProductVarientViewModel> ShopStockVarients(long ProductID, long shopID)
        {
            ProductDetails prod = new ProductDetails(System.Web.HttpContext.Current.Server);
            return prod.GetStockVarients(ProductID, shopID);
        }
    }
}
