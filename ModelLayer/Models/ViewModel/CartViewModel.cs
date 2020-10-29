using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models.ViewModel
{
    public class CartViewModel
    {
        public long ID { get; set; }
        public string CartName { get; set; }
        public long UserLoginID { get; set; }
        public string CustomerName { get; set; }
        public string CustomerMobile { get; set; }
        public string CustomerEmail { get; set; }
        public decimal CartAmount { get; set; }
        public string CartStage { get; set; }
        public int Status { get; set; }
        public string StatusVal { get; set; }
        public string CartPassword { get; set; }

        public Nullable<long> CustomerOrderID { get; set; }
        public Nullable<long> CityID { get; set; }
        public Nullable<long> MCOID { get; set; }
        public Nullable<Boolean> IsPlacedByCustomer { get; set; }

        public bool IsActive { get; set; }
        public System.DateTime CreateDate { get; set; }
        public long CreateBy { get; set; }
        public Nullable<System.DateTime> ModifyDate { get; set; }
        public Nullable<long> ModifyBy { get; set; }
        public string NetworkIP { get; set; }
        public string DeviceType { get; set; }
        public string DeviceID { get; set; }

        //- Extra Added
        public string ProductName { get; set; }
        public string ProductSize { get; set; }
        public string ProductColor { get; set; }
        public string Adress { get; set; }
        public string Email { get; set; }
        public decimal Price { get; set; }
        public decimal SaleRate { get; set; }
        public Nullable<decimal> LandingPrice { get; set; }
        public string City { get; set; }



        public Nullable<long> CartID { get; set; }
        public Nullable<int> Qty { get; set; }
        public string OrderCode { get; set; }
    }
}
