//-----------------------------------------------------------------------
// <copyright file="VerifyPincodeController" company="Ezeelo Consumer Services Pvt. Ltd.">
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
using ModelLayer.Models;

namespace API.Controllers
{
    public class VerifyPincodeController : ApiController
    {

        // GET api/verifypincode/5
        /// <summary>
        /// Verify whether the product is deliverable on given pincode or not.
        /// </summary>
        /// <param name="pincode">Shipping Pincode</param>
        /// <returns>Flag True/False</returns>
        [ValidateModel]
        [ApiException]
        public object Get(long selectedArea, int AddressId)
        {
            object obj = new object();
            try
            {
                EzeeloDBContext db = new EzeeloDBContext();
                // PincodeVerification pinVerify = new PincodeVerification();
                if (selectedArea == null || selectedArea <= 0 || AddressId == null || AddressId <= 0)
                {
                    return obj = new { Success = 0, Message = "Invalid AreaId or AddressId.", data = string.Empty };
                }
                PincodeVerification pv = new PincodeVerification();
                int? NewArea = db.CustomerShippingAddresses.Where(x => x.ID == AddressId).FirstOrDefault().AreaID;
                long NewAreaId = 0;
                if (NewArea != null)
                {
                    NewAreaId = (long)NewArea;
                }
                bool isDeliverable = pv.IsDeliverableArea(NewAreaId, selectedArea);
                // bool isDeliverable = pinVerify.IsDeliverablePincode(pincode);
                if (isDeliverable)
                    return obj = new { Success = 1, Message = "Products can be shipped at" + selectedArea, data = new { status = "True" } };
                //  new { HTTPStatusCode = "200", UserMessage = "Products can be shipped at " + selectedArea, status = "True" };
                else
                    return obj = new { Success = 0, Message = "Home Delivery NOT AVAILABLE to the PINCODE of Selected Address! Please, Verify PINCODE again...!!!", data = new { status = "False" } };
                // new { HTTPStatusCode = "200", UserMessage = "Home Delivery NOT AVAILABLE to the PINCODE of Selected Address! Please, Verify PINCODE again...!!! ", status = "False" };
            }
            catch (Exception ex)
            {
                obj = new { Success = 0, Message = ex.Message, data = string.Empty };
            }
            return obj;
        }

    }
}
