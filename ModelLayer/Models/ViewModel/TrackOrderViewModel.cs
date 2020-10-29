//-----------------------------------------------------------------------
// <copyright file="TrackOrderViewModelList" company="Ezeelo Consumer Services Pvt. Ltd.">
//     Copyright (c) Ezeelo Consumer Services Pvt. Ltd. All rights reserved.
// </copyright>
// <author>Sujata Kullarkar</author>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace ModelLayer.Models.ViewModel
{
    public class TrackOrderViewModelList
    {
        public List<TrackOrderViewModel> LtrackOrderViewModelList { get; set; }
    }

    public class TrackOrderViewModel
    {


        public bool CheckBoxStatus { get; set; }
        public string ImageLocation { get; set; }
        //Customer Order 
        public long CustomerOrderID { get; set; }
        [DisplayName("Customer Order Number")]
        public string OrderCode { get; set; }
        public long UserLoginID { get; set; }
        [DisplayName("Order Amount")]
        public decimal OrderAmount { get; set; }
        [DisplayName("Payable Amount")]
        public decimal PayableAmount { get; set; }
        public string PrimaryMobile { get; set; }
        public string SecondoryMobile { get; set; }
        [DisplayName("Shipping Address")]
        public string ShippingAddress { get; set; }
        public int PincodeID { get; set; }
        [DisplayName("Date")]
        public System.DateTime CreateDate { get; set; }

        //Customer Order Detail-COD
        public long ID { get; set; }
        [DisplayName("Shop Order Number")]
        public string ShopOrderCode { get; set; }
        public long ShopID { get; set; }

        [DisplayName("Shop Name")]
        public string ShopName { get; set; }
        public long ShopStockID { get; set; }
        [DisplayName("Total Qty")]
        public int CODQty { get; set; }
        public int OrderStatus { get; set; }
        public decimal MRP { get; set; }
        public decimal SaleRate { get; set; }
        public decimal OfferPercent { get; set; }
        public decimal OfferRs { get; set; }
        public bool IsInclusivOfTax { get; set; }
        [DisplayName("Amount")]
        public decimal TotalAmount { get; set; }

        //CustomerOrderHistory-COH
        public long COHCustomerOrderID { get; set; }
        public long COHShopStockID { get; set; }
        public int Status { get; set; }
        public bool IsActive { get; set; }
        public System.DateTime COHCreateDate { get; set; }
        public long CreateBy { get; set; }
        public string NetworkIP { get; set; }
        public string DeviceType { get; set; }
        public string DeviceID { get; set; }

        //UserLogin
        public string Mobile { get; set; }
        public string Email { get; set; }

        //PersonalDetail-PD
        public long PDUserLoginID { get; set; }
        public string SalutationName { get; set; }
        [DisplayName("Customer Name")]
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }


        //ShopStock-SS
        public long SSID { get; set; }
        public long ShopProductID { get; set; }
        public int SSQty { get; set; }
        [DisplayName("Packet Size")]
        public decimal PackSize { get; set; }
        public int PackUnitID { get; set; }
        [DisplayName("Rate")]
        public decimal RetailerRate { get; set; }
        public Nullable<System.DateTime> ModifyDate { get; set; }
        public Nullable<long> ModifyBy { get; set; }
        public string SSNetworkIP { get; set; }
        public string SSDeviceType { get; set; }
        public string SSDeviceID { get; set; }

        //Unit-U
        public int UID { get; set; }
        [DisplayName("Unit Name")]
        public string Name { get; set; }

        //ShopProduct-SP
        public long SPID { get; set; }
        public long SPShopID { get; set; }
        public long ProductID { get; set; }

        //Product-P
        public long PID { get; set; }
        [DisplayName("Product Name")]
        public string PName { get; set; }


        //DeliveryOrderDetail-DOD
        public string DODShopOrderCode { get; set; }
        public decimal Weight { get; set; }
        //public decimal OrderAmount { get; set; }
        public decimal DeliveryCharge { get; set; }
        [DisplayName("Delivery Charge")]
        public decimal GandhibaghCharge { get; set; }
        public string DeliveryType { get; set; }

        //Shop
        public bool IsDeliveryOutSource { get; set; }

        //Address
        public string AreaName { get; set; }
        public string CityName { get; set; }
        public string PincodeName { get; set; }

        //Delivery Schedule
        public Nullable<DateTime> DeliveryDate { get; set; }
        public string DeliveryScheduleName { get; set; }

    }
}