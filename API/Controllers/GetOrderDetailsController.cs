using API.Models;
//-----------------------------------------------------------------------
// <copyright file="GetOrderDetailsController" company="Ezeelo Consumer Services Pvt. Ltd.">
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
using System.Net.Http.Formatting;
using System.Web.Http;

namespace API.Controllers
{
    public class GetOrderDetailsController : ApiController
    {
        /// <summary>
        /// Get selected order details for customer
        /// </summary>
        /// <param name="lCustLoginID">Customer Login ID</param>
        /// <param name="lOrderID">Customer Order ID</param>
        /// <returns>List of Products in Order and Order Details for Given Order ID.</returns>
        [TokenVerification]
        [ApiException]
        [ValidateModel]
        // GET api/getorderdetails/5/5
        public object Get(long lCustLoginID, long lOrderID)
        {
            CustomerOrder lCustOrder = new CustomerOrder(System.Web.HttpContext.Current.Server);
            //var formatter = new JsonMediaTypeFormatter();
            //var json = formatter.SerializerSettings;
            //json.Converters.Add(new MyDateTimeConvertor());

            object obj = new object();
            if (lCustLoginID == null || lCustLoginID == 0 || lOrderID == null || lOrderID == 0)
            {
                return obj = new { Success = 0, Message = "Invalid custLoginID or orderId", data = string.Empty };
            }
            var orderdetails = lCustOrder.GetCustomerOrders(lCustLoginID, lOrderID);
            if (orderdetails != null)
            {
                List<CustomerOrderViewModel> CustomerOrderList = orderdetails.Orders;
                orderdetails.Orders = new List<CustomerOrderViewModel>();
                foreach (var item in CustomerOrderList)
                {
                    List<CustomerOrderDetailViewModel> ProductList = orderdetails.OrderProducts.Where(x => x.CustomerOrderID == item.CustomerOrderID).ToList();
                    if (ProductList != null && ProductList.Count > 0)
                    {
                        if (ProductList.Where(x => x.OrderStatus == 7).Any())
                        {
                            item.OrderStatus = 7;
                            item.OrderStatusName = Enum.GetName(typeof(ModelLayer.Models.Enum.ORDER_STATUS), item.OrderStatus);
                            //break;
                        }
                        else if (ProductList.Where(x => x.OrderStatus == 9).Any())
                        {
                            if (ProductList.Where(x => x.OrderStatus == 9).ToList().Count() == ProductList.Count())
                            {
                                item.OrderStatus = 9;
                                item.OrderStatusName = Enum.GetName(typeof(ModelLayer.Models.Enum.ORDER_STATUS), item.OrderStatus);
                                // break;
                            }
                            else
                            {
                                item.OrderStatus = ProductList.OrderByDescending(x => x.OrderStatus).Skip(1).FirstOrDefault().OrderStatus;
                                // item.OrderStatus = Convert.ToInt32(ProductList.OrderByDescending(x => x.OrderStatus).Take(2).ToList().OrderBy(x => x.OrderStatus).Take(1).Select(x => x.OrderStatus));
                                item.OrderStatusName = Enum.GetName(typeof(ModelLayer.Models.Enum.ORDER_STATUS), item.OrderStatus);
                                // break;
                            }
                        }
                        else
                        {
                            item.OrderStatus = ProductList.Select(x => x.OrderStatus).Max();
                            item.OrderStatusName = Enum.GetName(typeof(ModelLayer.Models.Enum.ORDER_STATUS), item.OrderStatus);
                            // break;
                        }
                        /* Started For Remove ABANDONED Order added by Sonali _04-12-2018*/
                        if (item.OrderStatus != 10)
                        {
                            item.TotalCGST = ProductList.Where(x => x.TaxesOnProduct != null && x.TaxesOnProduct.Count > 0).Sum(x => x.TaxesOnProduct.Where(y => y.TaxName.Contains("CGST")).Sum(y => y.TaxableAmount));
                            item.TotalSGST = ProductList.Where(x => x.TaxesOnProduct != null && x.TaxesOnProduct.Count > 0).Sum(x => x.TaxesOnProduct.Where(y => y.TaxName.Contains("SGST")).Sum(y => y.TaxableAmount));
                            item.TotalIGST = ProductList.Where(x => x.TaxesOnProduct != null && x.TaxesOnProduct.Count > 0).Sum(x => x.TaxesOnProduct.Where(y => y.TaxName.Contains("IGST")).Sum(y => y.TaxableAmount));
                            item.TotalDiscount = (ProductList.Sum(x => (x.MRP * x.Qty))) - (ProductList.Sum(x => (x.SaleRate * x.Qty)));
                            orderdetails.Orders.Add(item);
                        }
                    }
                    /*Ended For Remove ABANDONED Order added by Sonali _04-12-2018*/

                }
                obj = new { Success = 1, Message = "Details are found.", data = orderdetails };
            }
            else
            {
                obj = new { Success = 0, Message = "Details are not found.", data = string.Empty };
            }
            return obj;
        }
        /// <summary>
        /// Get order list for customer
        /// </summary>
        /// <param name="lCustLoginID">Customer Login ID</param>
        /// <returns>List of Products in Order and Order Details for All Orders for Selected Customer Login ID.</returns>
        [TokenVerification]
        [ApiException]
        // GET api/getorderdetails/5
        public object Get(long lCustLoginID)
        {
            CustomerOrder lCustOrder = new CustomerOrder(System.Web.HttpContext.Current.Server);
            object obj = new object();
            if (lCustLoginID == null || lCustLoginID == 0)
            {
                return obj = new { Success = 0, Message = "Invalid custLoginID", data = string.Empty };
            }
            //var formatter = new JsonMediaTypeFormatter();
            //var json = formatter.SerializerSettings;
            //json.Converters.Add(new MyDateTimeConvertor());
            var orderdetails = lCustOrder.GetCustomerOrders(lCustLoginID);
            if (orderdetails != null && orderdetails.Orders != null && orderdetails.Orders.Count > 0)
            {
                List<CustomerOrderViewModel> CustomerOrderList = orderdetails.Orders;
                orderdetails.Orders = new List<CustomerOrderViewModel>();
                foreach (var item in CustomerOrderList)
                {
                    List<CustomerOrderDetailViewModel> ProductList = orderdetails.OrderProducts.Where(x => x.CustomerOrderID == item.CustomerOrderID).ToList();
                    if (ProductList != null && ProductList.Count > 0)
                    {
                        if (ProductList.Where(x => x.OrderStatus == 7).Any())
                        {
                            item.OrderStatus = 7;
                            item.OrderStatusName = Enum.GetName(typeof(ModelLayer.Models.Enum.ORDER_STATUS), item.OrderStatus);
                            //break;
                        }
                        else if (ProductList.Where(x => x.OrderStatus == 9).Any())
                        {
                            if (ProductList.Where(x => x.OrderStatus == 9).ToList().Count() == ProductList.Count())
                            {
                                item.OrderStatus = 9;
                                item.OrderStatusName = Enum.GetName(typeof(ModelLayer.Models.Enum.ORDER_STATUS), item.OrderStatus);
                                // break;
                            }
                            else
                            {
                                item.OrderStatus = ProductList.OrderByDescending(x => x.OrderStatus).Skip(1).FirstOrDefault().OrderStatus;
                                // item.OrderStatus = Convert.ToInt32(ProductList.OrderByDescending(x => x.OrderStatus).Take(2).ToList().OrderBy(x => x.OrderStatus).Take(1).Select(x => x.OrderStatus));
                                item.OrderStatusName = Enum.GetName(typeof(ModelLayer.Models.Enum.ORDER_STATUS), item.OrderStatus);
                                // break;
                            }
                        }
                        else
                        {
                            item.OrderStatus = ProductList.Select(x => x.OrderStatus).Max();
                            item.OrderStatusName = Enum.GetName(typeof(ModelLayer.Models.Enum.ORDER_STATUS), item.OrderStatus);
                            // break;
                        }
                        /* Started For Remove ABANDONED Order added by Sonali _04-12-2018*/
                        if (item.OrderStatus != 10)
                        {
                            item.TotalCGST = ProductList.Where(x => x.TaxesOnProduct != null && x.TaxesOnProduct.Count > 0).Sum(x => x.TaxesOnProduct.Where(y => y.TaxName.Contains("CGST")).Sum(y => y.TaxableAmount));
                            item.TotalSGST = ProductList.Where(x => x.TaxesOnProduct != null && x.TaxesOnProduct.Count > 0).Sum(x => x.TaxesOnProduct.Where(y => y.TaxName.Contains("SGST")).Sum(y => y.TaxableAmount));
                            item.TotalIGST = ProductList.Where(x => x.TaxesOnProduct != null && x.TaxesOnProduct.Count > 0).Sum(x => x.TaxesOnProduct.Where(y => y.TaxName.Contains("IGST")).Sum(y => y.TaxableAmount));
                            item.TotalDiscount = (ProductList.Sum(x => (x.MRP * x.Qty))) - (ProductList.Sum(x => (x.SaleRate * x.Qty)));
                            orderdetails.Orders.Add(item);
                        }
                    }
                    /*Ended For Remove ABANDONED Order added by Sonali _04-12-2018*/

                }
                obj = new { Success = 1, Message = "Details are found.", data = orderdetails };
            }
            else
            {
                obj = new { Success = 0, Message = "Details are not found.", data = string.Empty };
            }
            return obj;
            // return Request.CreateResponse(HttpStatusCode.OK, lCustOrder.GetCustomerOrders(lCustLoginID), formatter);

        }
        /// <summary>
        /// Selecting first 10 records
        /// And Filtering by Date --- By Ashish not in use now
        /// </summary>
        /// <param name="lCustLoginID"></param>
        /// <param name="FrmDate"></param>
        /// <param name="ToDate"></param>
        /// <returns></returns>
        [TokenVerification]
        [ApiException]
        // GET api/getorderdetails/5
        public object Get(long lCustLoginID, string FrmDate, string ToDate)
        {

            CustomerOrder lCustOrder = new CustomerOrder(System.Web.HttpContext.Current.Server);
            object obj = new object();
            if (lCustLoginID == null || lCustLoginID == 0)
            {
                return obj = new { Success = 0, Message = "Invalid custLoginID", data = string.Empty };
            }
            //var formatter = new JsonMediaTypeFormatter();
            //var json = formatter.SerializerSettings;
            //json.Converters.Add(new MyDateTimeConvertor());
            var orderdetails = lCustOrder.GetCustomerOrders(lCustLoginID, FrmDate, ToDate);
            if (orderdetails != null)
            {
                obj = new { Success = 1, Message = "Details are found.", data = orderdetails };
            }
            else
            {
                obj = new { Success = 0, Message = "Details are not found.", data = string.Empty };
            }
            return obj;
            // return Request.CreateResponse(HttpStatusCode.OK, lCustOrder.GetCustomerOrders(lCustLoginID, FrmDate, ToDate), formatter);
        }

        /// <summary>
        /// Selecting first 10 records
        /// And Filtering by  Index --- By Ashish
        /// </summary>
        /// <param name="lCustLoginID"></param>
        /// <param name="FrmDate"></param>
        /// <param name="ToDate"></param>
        /// <returns></returns>
        [TokenVerification]
        [ApiException]
        // GET api/getorderdetails/5
        public HttpResponseMessage Get(long lCustLoginID, int Index, long? lCustomerOrderID = null)
        {
            CustomerOrder lCustOrder = new CustomerOrder(System.Web.HttpContext.Current.Server);
            var formatter = new JsonMediaTypeFormatter();
            var json = formatter.SerializerSettings;
            json.Converters.Add(new MyDateTimeConvertor());
            return Request.CreateResponse(HttpStatusCode.OK, lCustOrder.GetCustomerOrders(lCustLoginID, Index, lCustomerOrderID), formatter);

        }

        //public HttpResponseMessage Get(long lCustLoginID, int Index,long lCustomerOrderID)
        //{
        //    CustomerOrder lCustOrder = new CustomerOrder(System.Web.HttpContext.Current.Server);
        //    var formatter = new JsonMediaTypeFormatter();
        //    var json = formatter.SerializerSettings;
        //    json.Converters.Add(new MyDateTimeConvertor());
        //    return Request.CreateResponse(HttpStatusCode.OK, lCustOrder.GetCustomerOrders(lCustLoginID, Index,lCustomerOrderID), formatter);

        //}

    }
}
