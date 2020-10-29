//-----------------------------------------------------------------------
// <copyright file="ProductStockListController" company="Ezeelo Consumer Services Pvt. Ltd.">
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
using System.Web.Http.Description;
using API.Models;
using ModelLayer.Models;
using System.Linq.Expressions;

namespace API.Controllers
{

    public class ProductStockListController : ApiController
    {
        private EzeeloDBContext db = new EzeeloDBContext();
        // POST api/ProductStockList
        /// <summary>
        /// Get product list with all its stocks depending on search keyword and category
        /// </summary>
        /// <param name="productSearch">Product search object</param>
        /// <returns>List of relevant product Stocks</returns>
        [ApiException]
        //[Etag(300, 240, true)]
        public HttpResponseMessage Post(ProductSearchViewModel productSearch)
         {
             //-- For Differentiate Old and New APP --//
             if (productSearch.Version == null)
             { productSearch.FranchiseID = 0; }
 
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors);
                object obj = new object();
                obj = new { HTTPStatusCode = "400", UserMessage = "Invalid paramters", ValidationError = "One of the parameter is not valid." };
                return Request.CreateResponse(HttpStatusCode.BadRequest, obj);
            }
            if (productSearch.isListVarient)
            { 
                Mobile_ProductStockVarientViewModel ProductStockVarientViewModel = new Mobile_ProductStockVarientViewModel();
                ProductList productList = new ProductList(System.Web.HttpContext.Current.Server);
                ProductStockVarientViewModel = productList.Mobile_GetProductStockList(productSearch);

                return Request.CreateResponse(HttpStatusCode.OK, ProductStockVarientViewModel);
            }
            else
            {
                ProductStockVarientViewModel ProductStockVarientViewModel = new ProductStockVarientViewModel();
                ProductList productList = new ProductList(System.Web.HttpContext.Current.Server);

                ProductStockVarientViewModel = productList.GetProductStockList(productSearch);

                return Request.CreateResponse(HttpStatusCode.OK, ProductStockVarientViewModel);
            }

        }
                
    }
}
