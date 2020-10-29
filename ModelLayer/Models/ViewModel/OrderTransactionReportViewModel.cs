using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
//<copyright file="OrderTransactionReportViewModel.cs" company="Ezeelo Consumer Services Pvt. Ltd.">
//     Copyright (c) Ezeelo Consumer Services Pvt. Ltd. All rights reserved.
//    </copyright>
//    <author>Harshada Raghorte</author>
namespace ModelLayer.Models.ViewModel
{
    public class OrderTransactionReportViewModel
    {
        public int ShopFranchiseID { get; set; }
        public string ShopFranchiseName { get; set; }////added 
        public long ShopID { get; set; }
        public string ShopName { get; set; }
        public long SKUID { get; set; }   //PRITI
        public long COID { get; set; }
        public string OrderCode { get; set; }
        public string OrderStatus { get; set; }
        public long UserLoginID { get; set; }
        public string Customer { get; set; }
        public string RegMobile { get; set; }
        public string RegEmail { get; set; }

        // Added by Zubair on 16-10-2017
        public string JoiningDate { get; set; }
        public DateTime LastPurchaseDate { get; set; }
        //End
        public long ReferenceCustomerOrderID { get; set; }
        public decimal OrderAmount { get; set; }
        public int NoOfPointUsed { get; set; }
        public decimal ValuePerPoint { get; set; }
        public string CoupenCode { get; set; }
        public decimal CoupenAmount { get; set; }
        public string PAN { get; set; }
        public string PaymentMode { get; set; }
        public decimal TotalPayableAmount { get; set; }
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
        public string ShopOrderCode { get; set; }
        public string ProductName { get; set; }
        public string Category3 { get; set; }
        public string Category2 { get; set; }
        public string Category1 { get; set; }
        public int Qty { get; set; }
        public string Size { get; set; }
        public decimal MRP { get; set; }
        public decimal SaleRate { get; set; }
        public decimal PayableAmount { get; set; }
        public decimal RetailPoints { get; set; } //Yashaswi 31-7-2018
        public string IsLeader { get; set; } //Yashaswi 31-7-2018
        public string BatchCode { get; set; }
        public decimal WalleteAmountUsed { get; set; } //Yashaswi 21-01-2019
                                                       //public decimal TotalAmount { get; set; }      

        public string IsBusinessBoosterPlanOrder { get; set; }
        public int GST { get; set; }
    }
}
