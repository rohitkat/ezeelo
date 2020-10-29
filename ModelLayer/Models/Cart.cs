using System;
using System.Collections.Generic;

namespace ModelLayer.Models
{
    public partial class Cart
    {
        public Cart()
        {
            this.CartLogs = new List<CartLog>();
            this.TrackCarts = new List<TrackCart>();
        }

        public long ID { get; set; }
        public string Name { get; set; }
        public long UserLoginID { get; set; }
        public int Status { get; set; }
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
        public virtual ICollection<CartLog> CartLogs { get; set; }
        public virtual ICollection<TrackCart> TrackCarts { get; set; }
        public bool IsCouponApply { get; set; }//Added by Sonali for set IsCoupon applied on 19-02-2019
        public string CouponCode { get; set; }//Added by Sonali for set coupon code applied on 19-02-2019
        public decimal CouponAmount { get; set; }//Added by Sonali for set coupon amount applied on 19-02-2019
        public decimal WalletUsed { get; set; }//Added by Sonali for set wallet amount used applied on 19-02-2019
        public bool IsWalletApply { get; set; }//Added by Sonali for set IsWallet applied on 19-02-2019
    }
}
