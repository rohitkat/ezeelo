//-----------------------------------------------------------------------
// <copyright file="CustomerShippingAddressController" company="Ezeelo Consumer Services Pvt. Ltd.">
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
using ModelLayer.Models;
using API.Models;
using System.Data.Entity.Infrastructure;
using System.Web.Routing;


namespace API.Controllers
{
    public class CustomerShippingAddressController : ApiController
    {
        private EzeeloDBContext db = new EzeeloDBContext();
        // GET api/customershippingaddress/5
        /// <summary>
        /// Get the list all shipping addresses maintained by customer
        /// </summary>
        /// <param name="custLoginID">Customer Login ID</param>
        /// <returns>List all shipping addresses</returns>
        //[TokenVerification]
        // [ApiException] 
        //[ValidateModel]

        public object Get(long custLoginID)
        {
            Object obj = new object();
            if (custLoginID == null || custLoginID == 0)
            {
                return obj = new { Success = 0, Message = "Invalid custLoginID", data = string.Empty };
            }
            CustomerShippingDetails sAddr = new CustomerShippingDetails(System.Web.HttpContext.Current.Server);
            List<CustomerShippingAddressesViewModel> ls = new List<CustomerShippingAddressesViewModel>();
            ls = sAddr.GetCustomerShippingAddresses(custLoginID);
            //if (sAddr.GetCustomerShippingAddresses(custLoginID).Count == 0)
            //    return new List<CustomerShippingAddressesViewModel>();
            //return sAddr.GetCustomerShippingAddresses(custLoginID);
            if (ls != null && ls.Count > 0)
            {
                obj = new { Success = 1, Message = "Address list are found.", data = ls };
            }
            else
            {
                obj = new { Success = 0, Message = "Address list are not found.", data = string.Empty };
            }
            return obj;
        }
        /// <summary>
        /// Save shipping address
        /// </summary>
        /// <param name="Addr">CustomerShippingAddress memberes:UserLoginID,PrimaryMobile,ShippingAddress,PincodeID,AreaID </param>
        /// <returns>Response contains save operation status</returns>
        [TokenVerification]
        [ApiException]
        // [ValidateModel]
        // POST api/customershippingaddress
        public object Post(CustomerShippingAddress Addr)
        {
            object obj = new object();
            try
            {
                if (!ModelState.IsValid)
                {
                    return obj = new { Success = 0, Message = "Please enter valid details!!", data = string.Empty };
                }
                CustomerShippingDetails sAddr = new CustomerShippingDetails(System.Web.HttpContext.Current.Server);
                Addr.DeviceType = "Mobile";
                int oprStatus = sAddr.CreateCustomerShippingAddress(Addr);
                if (oprStatus == 101)
                    obj = new { Success = 1, Message = "Shipping Address Saved Successfully.", data = new { Id = Addr.ID } };
                else
                    obj = new { Success = 0, Message = "Problem in Saving Shipping Address.", data = string.Empty };
            }
            catch (Exception ex)
            {
                obj = new { Success = 0, Message = ex.Message, data = string.Empty };
            }
            return obj;
        }
        /// <summary>
        /// Edit customer Shipping address
        /// </summary>
        ///<param name="id">Shipping Address ID</param>
        /// <param name="Addr">customershippingaddress members: ID, PrimaryMobile,ShippingAddress,PincodeID</param>
        /// <returns>Response contains edit opration status</returns>
        [TokenVerification]
        [ApiException]
        [ValidateModel]
        // PUT api/customershippingaddress/5
        public object Put(int id, CustomerShippingAddress Addr)
        {
            object obj = new object();
            try
            {
                if (!ModelState.IsValid)
                {
                    return obj = new { Success = 0, Message = "Invalid parameter.", data = string.Empty };
                }
                if (id != Addr.ID)
                {
                    return obj = new { Success = 0, Message = "Invalid Request. id parameter sent in url, is not matched the ID provided in posted parameters.", data = string.Empty };
                }
                ModelLayer.Models.Pincode lPincode = db.Pincodes.Where(x => x.ID == Addr.PincodeID).FirstOrDefault();
                if (lPincode == null)
                {
                    ModelState.AddModelError("Error", "Invalid Pincode");
                    return obj = new { Success = 0, Message = "Pincode dose not exist. Please try another pincode", data = string.Empty };
                }
                else
                {
                    CustomerShippingDetails sAddr = new CustomerShippingDetails(System.Web.HttpContext.Current.Server);
                    Addr.DeviceType = "Mobile";
                    int oprStatus = sAddr.EditCustomerShippingAddress(Addr);

                    if (oprStatus == 102)
                        obj = new { Success = 1, Message = "Shipping Address Updated Successfully.", data = string.Empty };
                    else if (oprStatus == 500)
                        obj = new { Success = 0, Message = "Internal server error.", data = string.Empty };
                    else if (oprStatus == 104)
                        obj = new { Success = 0, Message = "The record you want to edit, does not exists.", data = string.Empty };
                }
            }
            catch (Exception ex)
            {
                obj = new { Success = 0, Message = ex.Message, data = string.Empty };
            }
            return obj;
        }
        /// <summary>
        /// Delete Customer Shipping address
        /// </summary>
        /// <param name="id">Shipping address ID</param>
        /// <returns>Response delete operation status</returns>
        [TokenVerification]
        [ApiException]
        [ValidateModel]
        // DELETE api/customershippingaddress/5
        public object Delete(int id)
        {
            object obj = new object();
            try
            {
                if (id == null || id <= 0)
                {
                    return obj = new { Success = 0, Message = "Invalid parameter.", data = string.Empty };
                }
                CustomerShippingDetails sAddr = new CustomerShippingDetails(System.Web.HttpContext.Current.Server);
                int oprStatus = sAddr.DeleteCustomerShippingAddress(id);
                if (oprStatus == 500)
                    obj = new { Success = 0, Message = "Internal Server Error.", data = string.Empty };
                else if (oprStatus == 103)
                    obj = new { Success = 0, Message = "The record you want to delete, does not exists.", data = string.Empty };
                else if (oprStatus == 106)
                    obj = new { Success = 0, Message = "ConditionNotMet.", data = string.Empty };
                else
                    obj = new { Success = 1, Message = "Shipping address deleted successfully.", data = string.Empty };
            }
            catch (Exception ex)
            {
                obj = new { Success = 0, Message = ex.Message, data = string.Empty };
            }
            return obj;
        }

        //  [Route("api/customershippingaddress/SetDefaultAddress")]
        [HttpGet]
        public object SetDefaultAddress(int id, long UserLoginId)
        {
            object obj = new object();
            try
            {
                if (id <= 0 || UserLoginId <= 0)
                {
                    return obj = new { Success = 0, Message = "Invalid parameter.", data = string.Empty };
                }
                List<CustomerShippingAddress> ShippingAddresslist = db.CustomerShippingAddresses.Where(x => x.UserLoginID == UserLoginId && x.IsActive).ToList();
                if (ShippingAddresslist != null && ShippingAddresslist.Count > 0)
                {
                    var ModifiedAddressList = ShippingAddresslist.Where(x => x.ID != id && x.IsDeliveryAddress).ToList();
                    if (ModifiedAddressList != null && ModifiedAddressList.Count > 0)
                    {
                        foreach (var item in ModifiedAddressList)
                        {
                            item.IsDeliveryAddress = false;
                            item.NetworkIP = BusinessLogicLayer.CommonFunctions.GetClientIP();
                            item.ModifyBy = CommonFunctions.GetPersonalDetailsID(UserLoginId);
                            item.ModifyDate = CommonFunctions.GetLocalTime();
                            db.Entry(item).CurrentValues.SetValues(item);
                            db.SaveChanges();
                        }
                        var addres = ShippingAddresslist.Where(x => x.ID == id).FirstOrDefault();
                        addres.IsDeliveryAddress = true;
                        addres.NetworkIP = BusinessLogicLayer.CommonFunctions.GetClientIP();
                        addres.ModifyBy = CommonFunctions.GetPersonalDetailsID(UserLoginId);
                        addres.ModifyDate = CommonFunctions.GetLocalTime();
                        db.Entry(addres).CurrentValues.SetValues(addres);
                        db.SaveChanges();
                    }
                    else
                    {
                        var addres = ShippingAddresslist.Where(x => x.ID == id).FirstOrDefault();
                        if (addres != null)
                        {
                            addres.IsDeliveryAddress = true;
                            addres.NetworkIP = BusinessLogicLayer.CommonFunctions.GetClientIP();
                            addres.ModifyBy = CommonFunctions.GetPersonalDetailsID(UserLoginId);
                            addres.ModifyDate = CommonFunctions.GetLocalTime();
                            db.Entry(addres).CurrentValues.SetValues(addres);
                            db.SaveChanges();
                        }
                    }
                    obj = new { Success = 1, Message = "Successfull.", data = string.Empty };
                }
                else
                    obj = new { Success = 0, Message = "Not Found Address.", data = string.Empty };
            }
            catch (Exception ex)
            {
                obj = new { Success = 0, Message = ex.Message, data = string.Empty };
            }
            return obj;
        }
    }
}
