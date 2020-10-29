using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CRM.Models.ViewModel
{
    public class NewCustomerOrderViewModel
    {
        public long COID { get; set; }
        public string OrderCode { get; set; }
        public string OrderStatus { get; set; }
        public long UserLoginID { get; set; }
        public string Customer { get; set; }
        public string RegMobile { get; set; }
        public string RegEmail { get; set; }
        public long ReferenceCustomerOrderID { get; set; }
        public decimal OrderAmount { get; set; }
        public int NoOfPointUsed { get; set; }
        public decimal ValuePerPoint { get; set; }
        public string CoupenCode { get; set; }
        public decimal CoupenAmount { get; set; }
        public string PAN { get; set; }
        public string PaymentMode { get; set; }
        public decimal PayableAmount { get; set; }
        public string PrimaryMobile { get; set; }
        public string SecondoryMobile { get; set; }
        public string ShippingAddress { get; set; }
        public string Area { get; set; }
        public long AreaID { get; set; }
        public string Pincode { get; set; }
        public long PincodeID { get; set; }
        public string City { get; set; }
        public long CityID { get; set; }
        public DateTime CreateDate { get; set; }
        public long CreateBy { get; set; }
        public string CreatedByUser { get; set; }
        public Nullable<DateTime> ModifyDate { get; set; }
        public Nullable<long> ModifyBy { get; set; }
        public string ModifyByUser { get; set; }
        public string NetworkIP { get; set; }
        public string DeviceType { get; set; }
        public string DeviceID { get; set; }
        public Nullable<DateTime> DeliveryDate { get; set; }
        public string DeliveryTime { get; set; }
        public string DeliveryType { get; set; }
        //public int TotalRecord { get; set; }
    }
}