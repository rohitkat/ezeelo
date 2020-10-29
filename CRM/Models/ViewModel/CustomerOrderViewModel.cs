using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ModelLayer.Models;

namespace CRM.Models.ViewModel
{
    public class CustomerOrderViewModel
    {
        public long ID { get; set; }
        public string OrderCode { get; set; }
        public long UserLoginID { get; set; }
        public Nullable<long> ReferenceCustomerOrderID { get; set; }
        public decimal OrderAmount { get; set; }
        public Nullable<int> NoOfPointUsed { get; set; }
        public Nullable<decimal> ValuePerPoint { get; set; }
        public string CoupenCode { get; set; }
        public Nullable<decimal> CoupenAmount { get; set; }
        public string PAN { get; set; }
        public string PaymentMode { get; set; }
        public decimal PayableAmount { get; set; }
        public string PrimaryMobile { get; set; }
        public string SecondoryMobile { get; set; }
        public string ShippingAddress { get; set; }
        public int PincodeID { get; set; }
        public Nullable<int> AreaID { get; set; }
        public System.DateTime CreateDate { get; set; }
        public long CreateBy { get; set; }
        public Nullable<System.DateTime> ModifyDate { get; set; }
        public Nullable<long> ModifyBy { get; set; }
        public string NetworkIP { get; set; }
        public string DeviceType { get; set; }
        public string DeviceID { get; set; }

        //Added by Harshda to add columns in order status report
        public string FranchiseName { get; set; }
        public string CityName { get; set; }
        //------------------------------------------------
        //----------------------------- extra added -//
        public int CustomerOrderDetailStatus { get; set; }
        public string Salutation { get; set; }
        public string PersonName { get; set; }

        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        //-------------------- Added by Tejaswee for delivery schedule (26-11-2015)
        public Nullable<DateTime> DeliveryDate { get; set; }
        public string DeliveryScheduleName { get; set; }
    }
}