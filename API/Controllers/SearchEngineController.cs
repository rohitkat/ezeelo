//-----------------------------------------------------------------------
// <copyright file="SearchEngineController" company="Ezeelo Consumer Services Pvt. Ltd.">
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
using System.Data.SqlClient;
using System.Data;
using ModelLayer.Models.ViewModel;
using BusinessLogicLayer;
using API.Models;
using System.IO;
using System.Text;
using ModelLayer.Models;

namespace API.Controllers
{
    public class SearchEngineController : ApiController
    {
        EzeeloDBContext db = new EzeeloDBContext();
        /// <summary>
        /// Search by keyword related to product/category/shop; Provide Search keyword in Pre Parameter and 0: All; 1:Shop; 2:Product in seachBy Parameter  
        /// </summary>
        /// <param name="pre">Search Term</param>
        /// <param name="searchby">Search For Shop/Product</param>
        /// <param name="cityID">City ID</param>
        /// <returns>List of objects related to search</returns>
        // GET api/searchengine/lap/PRODUCT
        [ApiException]
        [ValidateModel]
        [HttpGet]
        [Route("api/SearchEngine/GetSearchMetaData")]
        public object GetSearchMetaData(string pre, int searchBy, int cityID, int franchiseID, int? Version = null)////Added Int? FranchiseID for Multiple MCO and int? Version=null for New App
        {
            object obj = new object();
            try
            {
                if (string.IsNullOrEmpty(pre) && cityID <= 0 && franchiseID <= 0)
                {
                    return obj = new { Success = 0, Message = "Invalid parameter.", data = string.Empty };
                }
                //if (Version == null)
                //{ franchiseID = null; }


                ProductSearchViewModel productSearch = new ProductSearchViewModel();
                productSearch.PageIndex = 1;
                productSearch.Keyword = pre;
                if (!string.IsNullOrEmpty(productSearch.Keyword))
                {
                    int index = productSearch.Keyword.IndexOf("(");
                    if (index > 0)
                        productSearch.Keyword = productSearch.Keyword.Substring(0, index);
                    else
                    {
                        //For Category data fetch by Sonali on 09-04-2019
                        int categoryIndex = productSearch.Keyword.IndexOf("-");
                        if (categoryIndex > 0)
                        {
                            string nextChar = productSearch.Keyword.Substring((categoryIndex + 1), 1);
                            if (nextChar == " ")
                            {
                                int endIndex = productSearch.Keyword.Length - (categoryIndex + 2);
                                string categoryName = productSearch.Keyword.Substring(categoryIndex + 2, endIndex);
                                productSearch.CategoryID = db.Categories.Where(x => x.Name == categoryName).Select(x => x.ID).FirstOrDefault();
                                productSearch.Keyword = string.Empty;
                            }
                        }
                        //For Category data fetch by Sonali on 09-04-2019
                    }
                }
                productSearch.CityID = cityID;
                productSearch.FranchiseID = franchiseID;
                productSearch.PageSize = 50;//Sonali_03-11-2018_For display all product
                productSearch.Version = Version.HasValue ? Version.Value : 0;
                ProductWithRefinementViewModel productWithRefinementViewModel = new ProductWithRefinementViewModel();
                ProductList productList = new ProductList(System.Web.HttpContext.Current.Server);
                productWithRefinementViewModel = productList.GetProductList(productSearch, false);
                // var output = aSearch.GetSearchMetaData(pre, (AutoSearch.SEARCHBY)searchBy, cityID, franchiseID);
                if (productWithRefinementViewModel != null && productWithRefinementViewModel.productList.Count > 0 && productWithRefinementViewModel.productRefinements.Count > 0)
                    //{
                    obj = new { Success = 1, Message = "Record Found.", data = new { id = 1, productWithRefinementViewModel } };
                //}
                // AutoSearch aSearch = new AutoSearch();

                //if (result != null)
                //    obj = new { Success = 1, Message = "Record Found.", data = result };
                else
                    obj = new { Success = 0, Message = "Record not found.", data = string.Empty };
            }
            catch (Exception ex)
            {
                obj = new { Success = 0, Message = ex.Message, data = string.Empty };
            }
            return obj;
            //CityID 4968 for nagpur
            //Hide old code as not needed
            //if (cityID == 0 && franchiseID.Length == 0) //// added && franchiseID.Length==0
            //    cityID = 4968;

            //---------------------------
            ////if (franchiseID == 0) //Hide now for New App
            ////    franchiseID = 2;

            //-- For Differentiate Old and New APP --//
        }

        [Route("api/SearchEngine/GetSearchSuggestions")]
        public object GetSearchSuggestions(string pre, int cityId, int franchiseId, int Version)
        {
            object obj = new object();
            try
            {
                if (string.IsNullOrEmpty(pre) && cityId <= 0 && franchiseId <= 0)
                {
                    return obj = new { Success = 0, Message = "Invalid parameter.", data = string.Empty };
                }
                int searchBy = 0;
                AutoSearch aSearch = new AutoSearch();
                var result = aSearch.GetSearchMetaData(pre, (AutoSearch.SEARCHBY)searchBy, cityId, franchiseId);
                if (result != null)
                {
                    foreach (var item in result)
                    {
                        item.Name = item.Name.Replace("+", " ");
                    }
                    result = result.Where(x => x.Head == "Category" || x.Head == "Product").ToList();
                    obj = new { Success = 1, Message = "Successfull.", data = result };
                }
                else
                    obj = new { Success = 0, Message = "Record not found.", data = string.Empty };
            }
            catch (Exception ex)
            {
                obj = new { Success = 0, Message = ex.Message, data = string.Empty };
            }
            return obj;
        }

        [HttpPost]
        [Route("api/SearchEngine/GetSearchItem")]
        public object GetSearchItem(AutoSuggestViewModel viewModel)
        {
            object obj = new object();
            try
            {
                // viewModel.Name = viewModel.Name.ToLower().Replace("&", "and").Replace(/[\/\\#,+()$~%.'":*?<>{} ]/g, '-');
                if (viewModel.Head == "Product")
                {
                    string clName = "default";
                    ProductDetails p = new ProductDetails(System.Web.HttpContext.Current.Server);
                    ProductDetailview detailview = new ProductDetailview();
                    detailview.ProductbasicDetail = p.GetBasicDetails(Convert.ToInt64(viewModel.ID));
                    detailview.ProductVarient = p.GetStockVarients(Convert.ToInt64(viewModel.ID), 0, null, viewModel.FranchiseId);
                    detailview.Images = ImageDisplay.GetStockImages(Convert.ToInt64(viewModel.ID), string.IsNullOrEmpty(clName) || clName == "N/A" ? string.Empty : clName);
                    detailview.ProductGeneralSpecification = p.GetGeneralDescription(Convert.ToInt64(viewModel.ID));
                    if (!string.IsNullOrEmpty(detailview.ProductGeneralSpecification))
                    {
                        var httpWebRequest = (HttpWebRequest)WebRequest.Create(detailview.ProductGeneralSpecification);
                        //httpWebRequest.ContentType = "application/json";
                        httpWebRequest.Method = "GET";
                        var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                        using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                        {
                            var result = streamReader.ReadToEnd();
                            detailview.ProductGeneralSpecification = result;
                        };
                    }
                    if (detailview.ProductbasicDetail != null && detailview.ProductVarient != null)
                    {
                        obj = new { Success = 1, Message = "Product details are found.", data = detailview };
                    }
                    else
                    {
                        obj = new { Success = 0, Message = "Product details not found.", data = string.Empty };
                    }
                }
                else if (viewModel.Head == "Category")
                {
                    //if (viewModel.IsManagedItem)
                    //{

                    //}
                    //else
                    //{
                    int? shopID = 0;
                    viewModel.Name = string.Empty;
                    if (viewModel.CityId != null)
                    {
                        // string cookieValue = ControllerContext.HttpContext.Request.Cookies["CityCookie"].Value;
                        // TrackSearchBusiness.InsertSearchDetails(UserID, Convert.ToInt64(parentCategoryId), Convert.ToInt64(shopID), item, "", "", "", "", cookieValue.Split('$')[1], "");//hide
                        TrackSearchBusiness.InsertSearchDetails(viewModel.UserID, Convert.ToInt64(viewModel.ID), Convert.ToInt64(0), viewModel.Name, "", "", "", "", "", "", viewModel.CityId);//--added by Ashish for multiple franchise in same city--//
                    }
                    //================ END ===========================================================================

                    ProductSearchViewModel productSearch = new ProductSearchViewModel();
                    productSearch.PageIndex = 1;
                    productSearch.Keyword = viewModel.Name;
                    productSearch.CategoryID = Convert.ToInt64(viewModel.ID);
                    productSearch.FranchiseID = viewModel.FranchiseId;
                    productSearch.CityID = viewModel.CityId;
                    productSearch.PageSize = 12;
                    productSearch.Version = 1;
                    ProductWithRefinementViewModel productWithRefinementViewModel = new ProductWithRefinementViewModel();
                    ProductList productList = new ProductList(System.Web.HttpContext.Current.Server);
                    productWithRefinementViewModel = productList.GetProductList(productSearch, false);
                    if (productWithRefinementViewModel != null)
                    {
                        obj = new { Success = 1, Message = "Product list are found.", data = new { id = 1, productWithRefinementViewModel } };
                    }
                    //  }
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
