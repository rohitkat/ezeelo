//-----------------------------------------------------------------------
// <copyright file="SimillarProductsController" company="Ezeelo Consumer Services Pvt. Ltd.">
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
using System.Web.Http.Description;
using API.Models;

namespace API.Controllers
{
    public class SimillarProductsController : ApiController
    {
        // Post api/SimillarProducts
        /// <summary>
        /// Get list of simillar products related to search product
        /// </summary>
        /// <param name="searchSimilarProducts">Simillar product object with page index and page size.</param>
        /// <returns></returns>
        [ApiException]
        [ValidateModel]

        public object Post(SearchSimilarProductViewModel searchSimilarProducts)
        {
            object obj = new object();
            try
            {
                if (!ModelState.IsValid)
                {
                    return obj = new { Success = 0, Message = "Invalid paramters.", data = string.Empty };
                }
                RelatedProductsViewModel relatedProducts = new RelatedProductsViewModel();
                ProductDetails productDetails = new ProductDetails(System.Web.HttpContext.Current.Server);
                relatedProducts = productDetails.GetSimillarProducts(searchSimilarProducts);
                if (relatedProducts != null)
                {
                    obj = new { Success = 1, Message = "Similar product list found.", data = relatedProducts };
                }
                else
                {
                    obj = new { Success = 0, Message = "Similar product list are not found.", data = string.Empty };
                }
            }
            catch (Exception ex)
            {
                obj = new { Success = 0, Message = ex.InnerException.InnerException, data = string.Empty };
            }
            return obj;
        }

        //public RelatedProductsViewModel GetSimillarProducts(SearchSimilarProductViewModel searchSimilarProducts)
        //{
        //    RelatedProductsViewModel relatedProducts = new RelatedProductsViewModel();
        //    ProductDetails productList = new ProductDetails(System.Web.HttpContext.Current.Server);
        //    relatedProducts = productList.GetSimillarProducts(searchSimilarProducts);
        //    return relatedProducts;
        //}

        /*Test Purpose */
        //public RelatedProductsViewModel GetSimillarProducts(int id)
        //{
        //    ProductDetails productList = new ProductDetails(System.Web.HttpContext.Current.Server);
        //    RelatedProductsViewModel rel = new RelatedProductsViewModel();
        //    SearchSimilarProductViewModel search = new SearchSimilarProductViewModel();
        //    search.CategoryID = 24;
        //    search.CityID = 1;
        //    search.PageIndex = 1;
        //    search.PageSize = 12;
        //    search.ProductID = 29;
        //    search.ShopID = 11;
        //    rel = productList.GetSimillarProducts(search);
        //    return rel;
        //}
    }
}
