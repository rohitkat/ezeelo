//-----------------------------------------------------------------------
// <copyright file="ProductTechnicalSpecificationController" company="Ezeelo Consumer Services Pvt. Ltd.">
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
    public class ProductTechnicalSpecificationController : ApiController
    {
        /// <summary>
        /// Product's technical specification e.g Front Camera : 5 MP.  
        /// </summary>
        /// <param name="pID">Product ID</param>
        /// <returns>Product's Technical Specification List</returns>   
        [ApiException]
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
                ProductDetails lprod = new ProductDetails(System.Web.HttpContext.Current.Server);
                var ProductTechSpecification = lprod.GetTechnicalSpecifications(pID);
                if (ProductTechSpecification != null)
                {
                    obj = new { Success = 1, Message = "Product technical specification are found.", data = ProductTechSpecification };
                }
                else
                {
                    obj = new { Success = 1, Message = "Product technical specification are not found.", data = string.Empty };
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
