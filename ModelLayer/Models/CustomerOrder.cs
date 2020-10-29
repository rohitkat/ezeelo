using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ModelLayer.Models
{
    public partial class CustomerOrder
    {
        public CustomerOrder()
        {
            this.CustomerOrder1 = new List<CustomerOrder>();
            this.CustomerOrderDetails = new List<CustomerOrderDetail>();
            this.CustomerOrderDetailXMLs = new List<CustomerOrderDetailXML>();
            this.CustomerOrderHistories = new List<CustomerOrderHistory>();
            this.ReceiveOrderOnCalls = new List<ReceiveOrderOnCall>();
            //Added for delivery schedule (Added by Tejaswee 26-11-2015)
            this.OrderDeliveryScheduleDetails = new List<OrderDeliveryScheduleDetail>();

            this.EarnDetails = new List<EarnDetail>();
            this.ReferAndEarnTransactions = new List<ReferAndEarnTransaction>();


        }

        public long ID { get; set; }
        public string OrderCode { get; set; }
        public long UserLoginID { get; set; }
        public Nullable<long> ReferenceCustomerOrderID { get; set; }
        public decimal OrderAmount { get; set; }
        //public decimal? TotalOrderAmount { get; set; } //Yashaswi 18-9-2018
        public Nullable<int> NoOfPointUsed { get; set; }
        public Nullable<decimal> ValuePerPoint { get; set; }
        public string CoupenCode { get; set; }
        public Nullable<decimal> CoupenAmount { get; set; }
        public string PAN { get; set; }
        public string PaymentMode { get; set; }
        public decimal PayableAmount { get; set; }
        public string PrimaryMobile { get; set; }
        public string SecondoryMobile { get; set; }
        [MaxLength]
        public string ShippingAddress { get; set; }
        public int PincodeID { get; set; }
        public Nullable<int> AreaID { get; set; }

        //Added by Zubair for MLM on 05-04-2018
        public decimal BusinessPointsTotal { get; set; }
        public Nullable<decimal> MLMAmountUsed { get; set; }
        //End

        public decimal CashbackPointsTotal { get; set; }
        public System.DateTime CreateDate { get; set; }
        public long CreateBy { get; set; }
        public Nullable<System.DateTime> ModifyDate { get; set; }
        public Nullable<long> ModifyBy { get; set; }
        public string NetworkIP { get; set; }
        public string DeviceType { get; set; }
        public string DeviceID { get; set; }
        public virtual Area Area { get; set; }
        public virtual PersonalDetail PersonalDetail { get; set; }
        public virtual List<CustomerOrder> CustomerOrder1 { get; set; }
        public virtual CustomerOrder CustomerOrder2 { get; set; }
        public virtual Pincode Pincode { get; set; }
        public virtual UserLogin UserLogin { get; set; }
        public virtual List<CustomerOrderDetail> CustomerOrderDetails { get; set; }
        public virtual List<CustomerOrderDetailXML> CustomerOrderDetailXMLs { get; set; }
        public virtual List<CustomerOrderHistory> CustomerOrderHistories { get; set; }
        public virtual PersonalDetail PersonalDetail1 { get; set; }
        public virtual List<ReceiveOrderOnCall> ReceiveOrderOnCalls { get; set; }
        public virtual ICollection<OrderDeliveryScheduleDetail> OrderDeliveryScheduleDetails { get; set; }

        public virtual ICollection<EarnDetail> EarnDetails { get; set; }
        public virtual ICollection<ReferAndEarnTransaction> ReferAndEarnTransactions { get; set; }
    }
}
