//-----------------------------------------------------------------------
// <copyright file="StockVarientController" company="Ezeelo Consumer Services Pvt. Ltd.">
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
    public class StockVarientController : ApiController
    {
        // GET api/stockvarient/4
        /// <summary>
        /// This method gives the details for Specific shopStockId, can be used while displaying Perticular product info at the time of purchasing.
        /// </summary>
        /// <param name="shopStockID">ShopStockID</param>
        /// <returns>object of type ShopProductVarientViewModel</returns>
        [ValidateModel]
        [ApiException]
        public object GetStockVarients(long shopStockID, long lCustLoginID)
        {
            object obj = new object();
            try
            {
                if (shopStockID == 0 || shopStockID == null || lCustLoginID == 0 || lCustLoginID == null)
                {
                    return obj = new { Success = 0, Message = "Enter valid data.", data = string.Empty };
                }
                ProductDetails prod = new ProductDetails(System.Web.HttpContext.Current.Server);
                var result = prod.GetShopStockVarients(shopStockID, lCustLoginID);
                if (result != null)
                {
                    obj = new { Success = 1, Message = "Product list are found.", data = result };
                }
                else
                {
                    obj = new { Success = 1, Message = "Product list are not found.", data = string.Empty };
                }
            }
            catch (Exception ex)
            {
                obj = new { Success = 0, Message = ex.Message, data = string.Empty };
            }
            return obj;

        }

        [ValidateModel]
        [ApiException]
        [HttpPost]
        public object GetStockVarients(List<ShopStockIDs> shopStockID)
        {
            object obj = new object();
            try
            {
                if (shopStockID == null || shopStockID.Count <= 0)
                {
                    return obj = new { Success = 0, Message = "Enter valid data.", data = string.Empty };
                }
                ProductDetails prod = new ProductDetails(System.Web.HttpContext.Current.Server);
                var carts = prod.GetShopStockVarients(shopStockID);
                if (carts != null && carts.Count > 0)
                {
                    obj = new { Success = 1, Message = "Cart list found", data = carts };
                }
                else
                {
                    obj = new { Success = 1, Message = "Cart list are not found.", data = string.Empty };
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
