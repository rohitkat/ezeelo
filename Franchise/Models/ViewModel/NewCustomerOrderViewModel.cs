using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Franchise.Models.ViewModel
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
        //Yashaswi 30-7-2018
        public decimal RetailPoints { get; set; }
        public string IsLeader { get; set; }

        public decimal? MLMAmountUsed { get; set; }
        public string IsBusinessBoosterPlanOrder { get; set; }
        public decimal CashbackPoints { get; set; }
    }

    public class OrderListStatusCount
    {
        public OrderListStatusCount()
        {
            Pending = 0;
            Placed = 0;
            Confirm = 0;
            Packed = 0;
            DispFromShop = 0;
            InGodown = 0;
            DispFromGodown = 0;
            Delivered = 0;
            Returned = 0;
            Canceled = 0;
            Abondand = 0;
            Total = 0;
            PageCount = 0;
        }
        public int Pending { get; set; }
        public int Placed { get; set; }
        public int Confirm { get; set; }
        public int Packed { get; set; }
        public int DispFromShop { get; set; }
        public int InGodown { get; set; }
        public int DispFromGodown { get; set; }
        public int Delivered { get; set; }
        public int Returned { get; set; }
        public int Canceled { get; set; }
        public int Abondand { get; set; }
        public int Total { get; set; }
        public int PageCount { get; set; }
    }
}