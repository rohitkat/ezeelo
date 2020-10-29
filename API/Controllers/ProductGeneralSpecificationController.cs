//-----------------------------------------------------------------------
// <copyright file="ProductGeneralSpecificationController" company="Ezeelo Consumer Services Pvt. Ltd.">
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
using API.Models;


namespace API.Controllers
{
    public class ProductGeneralSpecificationController : ApiController
    {
        [ApiException]
        // GET api/productgeneralspecification/5
        /// <summary>
        /// This API returns the path of .htm file, where the product details are mentioned.
        /// </summary>
        /// <param name="pID">Product ID</param>
        /// <returns>File Path</returns>
        [ValidateModel]
        public object Get(long pID)
        {
            object obj = new object();
            try
            {
                if (pID == null || pID <= 0)
                {
                    return obj = new { Success = 0, Message = "Enter valid productId.", data = string.Empty };
                }
                ProductDetails pd = new ProductDetails(System.Web.HttpContext.Current.Server);
                string filePath = pd.GetGeneralDescription(pID);
                if (filePath.Trim().Equals(string.Empty))
                {
                    obj = new { Success = 0, Message = "Description file not available for provided product.", data = string.Empty };
                    //obj = new { HTTPStatusCode = "200", UserMessage = "Description file not available for provided product", FilePath = "" };
                }
                else
                    obj = new { Success = 1, Message = "Description file is available.", data = new { FilePath = filePath } };
                // obj = new { HTTPStatusCode = "200", UserMessage = "Description file is available.", FilePath = filePath };
            }
            catch (Exception ex)
            {
                obj = new { Success = 0, Message = ex.Message, data = string.Empty };
            }
            return obj;
        }

        [ApiException]
        // GET api/productgeneralspecification/5
        /// <summary>
        /// This API returns the path of .htm file, where the product details are mentioned.
        /// </summary>
        /// <param name="pID">Product ID</param>
        /// <param name="pID">IsWeb true for  HTML Contain OR False for File PATH</param>
        /// <returns>File Path</returns>
        [ValidateModel]
        public object Get(long pID, bool IsWeb)
        {
            object obj = new object();
            try
            {
                if (pID == null || pID <= 0)
                {
                    return obj = new { Success = 0, Message = "Enter valid productId.", data = string.Empty };
                }
                ProductDetails pd = new ProductDetails(System.Web.HttpContext.Current.Server);

                string filePath = string.Empty;
                if (IsWeb)
                    filePath = pd.GetGeneralDescription(pID, true);
                else
                    filePath = pd.GetGeneralDescription(pID);

                if (filePath.Trim().Equals(string.Empty))
                {
                    obj = new { Success = 1, Message = "Description file not available for provided product.", data = string.Empty };
                    //obj = new { HTTPStatusCode = "200", UserMessage = "Description file not available for provided product", FilePath = "" };
                }
                else
                    obj = new { Success = 1, Message = "Description file is available.", data = new { FilePath = filePath } };
                // obj = new { HTTPStatusCode = "200", UserMessage = "Description file is available.", FilePath = filePath };
            }
            catch (Exception ex)
            {
                obj = new { Success = 0, Message = ex.Message, data = string.Empty };
            }
            return obj;
        }
    }
}
