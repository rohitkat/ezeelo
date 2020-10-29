//-----------------------------------------------------------------------
// <copyright file="ProductListController" company="Ezeelo Consumer Services Pvt. Ltd.">
//     Copyright (c) Ezeelo Consumer Services Pvt. Ltd. All rights reserved.
// </copyright>
// <author>Sujata Kullarkar</author>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ModelLayer.Models.ViewModel;
using System.Web.Http.Description;
using BusinessLogicLayer;
using API.Models;

namespace API.Controllers
{
    public class ProductListController : ApiController
    {
        [HttpGet]
        public object Get(Int64 ShopStockID)
        {
            object obj = new object();
            try
            {
                if (ShopStockID <= 0)
                {
                    return obj = new { Success = 0, Message = "Invalid parameter.", data = string.Empty };
                }
                List<PreviewProductForMobileViewModel> objList = new List<PreviewProductForMobileViewModel>();
                BusinessLogicLayer.ProductList productlist = new BusinessLogicLayer.ProductList(System.Web.HttpContext.Current.Server);
                objList = productlist.GetPreviewProductForMobile(ShopStockID);
                if (objList != null && objList.Count > 0)
                {
                    obj = new { Success = 1, Message = "Product List are found.", data = objList };
                }
                else
                {
                    obj = new { Success = 1, Message = "Product List are not found.", data = string.Empty };
                }
                // return objList;
            }
            catch (Exception ex)
            {
                obj = new { Success = 0, Message = ex.Message, data = string.Empty };
            }
            return obj;
        }


        // POST api/ProductList
        /// <summary>
        /// Get list of products having minimun sale rate, depending on search criteria
        /// </summary>
        /// <param name="productSearch">Product Search object</param>
        /// <returns></returns>
        [ApiException]
        [ResponseType(typeof(ProductWithRefinementViewModel))]
        public object post(ProductSearchViewModel productSearch)
        {
            object obj = new object();
            try
            {
                if (!ModelState.IsValid)
                {
                    return obj = new { Success = 0, Message = "Enter valid data.", data = string.Empty };
                    //return BadRequest(ModelState);
                }
                if (!string.IsNullOrEmpty(productSearch.Keyword))
                {
                    int index = productSearch.Keyword.IndexOf("(");
                    if (index > 0)
                        productSearch.Keyword = productSearch.Keyword.Substring(0, index);
                }
                productSearch.PageIndex = 1;
                ProductWithRefinementViewModel productWithRefinementViewModel = new ProductWithRefinementViewModel();
                ProductList productList = new ProductList(System.Web.HttpContext.Current.Server);
                productWithRefinementViewModel = productList.GetProductList(productSearch, false);
                
                if (productWithRefinementViewModel != null)
                {
                    obj = new { Success = 1, Message = "Product list are found.", data = new { id = 1, productWithRefinementViewModel } };
                }
                // return CreatedAtRoute("DefaultApi", new { id = 1 }, productWithRefinementViewModel);
            }
            catch (Exception ex)
            {
                obj = new { Success = 0, Message = ex.Message, data = string.Empty };
            }
            return obj;
        }
    }
}
