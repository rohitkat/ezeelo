
//-----------------------------------------------------------------------
// <copyright file="SendOrderSmsAndEmailController" company="Ezeelo Consumer Services Pvt. Ltd.">
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
using ModelLayer;
using BusinessLogicLayer;
using API.Models;

namespace API.Controllers
{
    public class SendOrderSmsAndEmailController : ApiController
    {


        // GET api/sendordersmsandemail/5
        /// <summary>
        /// Send SMS and Emails  to merchant and customer after placing or cancellation of order by customer.
        /// </summary>
        /// <param name="custLoginID">Customer Login ID</param>
        /// <param name="orderID">Order ID</param>
        /// <param name="status">Status of order 1: Placed; 2: Confirmed....</param>
        /// <returns></returns>
        [TokenVerification]
        [ApiException]
        public object Get(long custLoginID, long orderID, int status, int? Version = null)
        {
            object obj = new object();
            try
            {
                if (custLoginID <= 0 || orderID <= 0 || status <= 0)
                {
                    return obj = new { Success = 0, Message = "Invalid parameter.", data = string.Empty };
                }
                if ((ModelLayer.Models.Enum.ORDER_STATUS)status == ModelLayer.Models.Enum.ORDER_STATUS.PLACED)
                {
                    OrderPlacedSmsAndEmail orderPlaced = new OrderPlacedSmsAndEmail(System.Web.HttpContext.Current.Server);
                    //-- For Differentiate Old and New APP --//
                    //if (Version == null)
                    //{ 
                    orderPlaced.SendSMSToCustomer(custLoginID, orderID);
                    //}
                    orderPlaced.SendSMSToMerchant(custLoginID, orderID);
                    orderPlaced.SendMailToCustomer(custLoginID, orderID);
                    orderPlaced.SendMailToMerchant(custLoginID, orderID);
                    obj = new { Success = 1, Message = "SMS Sent Successfully.", data = string.Empty };
                }
                else if ((ModelLayer.Models.Enum.ORDER_STATUS)status == ModelLayer.Models.Enum.ORDER_STATUS.CANCELLED)
                {
                    OrderCancelSmsAndEmail orderCancel = new OrderCancelSmsAndEmail(System.Web.HttpContext.Current.Server);
                    //-- For Differentiate Old and New APP --//
                    //if (Version == null)
                    //{
                    orderCancel.SendSMSToCustomer(custLoginID, orderID);
                    //}
                    orderCancel.SendSMSToMerchant(custLoginID, orderID);
                    orderCancel.SendMailToCustomer(custLoginID, orderID);
                    orderCancel.SendMailToMerchant(custLoginID, orderID);
                    obj = new { Success = 1, Message = "SMS Sent Successfully.", data = string.Empty };
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
