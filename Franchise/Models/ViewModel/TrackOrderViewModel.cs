using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Franchise.Models.ViewModel
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
        public string OrderCode { get; set; }
        public long UserLoginID { get; set; }
        public decimal OrderAmount { get; set; }
        public decimal PayableAmount { get; set; }
        public string PrimaryMobile { get; set; }
        public string SecondoryMobile { get; set; }
        public string ShippingAddress { get; set; }
        public int PincodeID { get; set; }
        public System.DateTime CreateDate { get; set; }
       
        //Customer Order Detail-COD
        public long ID { get; set; }
        public string ShopOrderCode { get; set; }
        public long ShopID { get; set; }
        public long ShopStockID { get; set; }
        public int CODQty { get; set; }
        public int OrderStatus { get; set; }
        public decimal MRP { get; set; }
        public decimal SaleRate { get; set; }
        public decimal OfferPercent { get; set; }
        public decimal OfferRs { get; set; }
        public bool IsInclusivOfTax { get; set; }
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
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }


        //ShopStock-SS
        public long SSID { get; set; }
        public long ShopProductID { get; set; }
        public int SSQty { get; set; }
        public decimal PackSize { get; set; }
        public string PackUnitName { get; set; }
        public decimal RetailerRate { get; set; }
        public Nullable<System.DateTime> ModifyDate { get; set; }
        public Nullable<long> ModifyBy { get; set; }
        public string SSNetworkIP { get; set; }
        public string SSDeviceType { get; set; }
        public string SSDeviceID { get; set; }

        //Unit-U
        public int UID { get; set; }
        public string Name { get; set; }

        //ShopProduct-SP
        public long SPID { get; set; }
        public long SPShopID { get; set; }
        public long ProductID { get; set; }

        //Product-P
        public long PID { get; set; }
        public string PName { get; set; }


        //DeliveryOrderDetail-DOD
        public string DODShopOrderCode { get; set; }
        public decimal Weight { get; set; }
        //public decimal OrderAmount { get; set; }
        public decimal DeliveryCharge { get; set; }
        public decimal GandhibaghCharge { get; set; }
        public string DeliveryType { get; set; }

        //Shop
        public bool IsDeliveryOutSource { get; set; }
        
        //Address
        public string AreaName { get; set; }
        public string CityName { get; set; }
        public string PincodeName { get; set; }

    }
}