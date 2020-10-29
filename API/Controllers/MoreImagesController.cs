//-----------------------------------------------------------------------
// <copyright file="MoreImagesController" company="Ezeelo Consumer Services Pvt. Ltd.">
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
    public class MoreImagesController : ApiController
    {
        /// <summary>
        /// Get different large and small images of the selected product depending on given color stock variant
        /// </summary>
        /// <param name="pID">Product ID</param>
        /// <param name="clName">Color Name</param>
        /// <returns>List of small and large images</returns>
        [ApiException]
        [ValidateModel]
        // GET api/moreimages/4/Red
        public object Get(int pID, string clName)
        {
            object obj = new object();
            try
            {
                if (pID == null || pID <= 0)
                {
                    return obj = new { Success = 0, Message = "Enter valid productId.", data = string.Empty };
                }
                if (clName.ToString().ToUpper().Equals("N%2FA"))
                {
                    clName = "default";
                }
                else if (clName.ToString().ToUpper().Equals("N/A"))
                {
                    clName = "default";
                }
                else if (clName.ToString().ToUpper().Equals("N/A"))
                {
                    clName = "default";
                }
                var ImageList = ImageDisplay.GetStockImages(pID, string.IsNullOrEmpty(clName) || clName == "N/A" ? string.Empty : clName);
                //return ImageDisplay.GetStockImages(pID, string.IsNullOrEmpty(clName) || clName == "N/A" ? string.Empty : clName);
                if (ImageList != null && ImageList.Count > 0)
                {
                    obj = new { Success = 1, Message = "Product images are found.", data = new { Images = ImageList } };
                }
                else
                {
                    obj = new { Success = 1, Message = "Product images not found.", data = string.Empty };
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
