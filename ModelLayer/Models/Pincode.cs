using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ModelLayer.Models
{
    public partial class Pincode
    {
        public Pincode()
        {
            this.Advertisers = new List<Advertiser>();
            this.Areas = new List<Area>();
            this.BusinessDetails = new List<BusinessDetail>();
            this.CustomerOrders = new List<CustomerOrder>();
            this.CustomerShippingAddresses = new List<CustomerShippingAddress>();
            this.DeliveryPartners = new List<DeliveryPartner>();
            this.DeliveryPincodes = new List<DeliveryPincode>();
            this.Franchises = new List<Franchise>();
            this.PersonalDetails = new List<PersonalDetail>();
            this.Shops = new List<Shop>();
        }

        public int ID { get; set; }

        [Required(ErrorMessage = "Pincode is Required")]
        [StringLength(6, MinimumLength = 6, ErrorMessage = "Pincode must be 6 characters ")] 
        public string Name { get; set; }

        [Required(ErrorMessage = "City is Required")]
        [Range(1, Int64.MaxValue,ErrorMessage="City is not Valid")]
        public long CityID { get; set; }
        public bool IsActive { get; set; }
        public System.DateTime CreateDate { get; set; }
        public long CreateBy { get; set; }
        public Nullable<System.DateTime> ModifyDate { get; set; }
        public Nullable<long> ModifyBy { get; set; }
        public string NetworkIP { get; set; }
        public string DeviceType { get; set; }
        public string DeviceID { get; set; }
        public virtual List<Advertiser> Advertisers { get; set; }
        public virtual List<Area> Areas { get; set; }
        public virtual List<BusinessDetail> BusinessDetails { get; set; }
        public virtual City City { get; set; }
        public virtual List<CustomerOrder> CustomerOrders { get; set; }
        public virtual List<CustomerShippingAddress> CustomerShippingAddresses { get; set; }
        public virtual List<DeliveryPartner> DeliveryPartners { get; set; }
        public virtual List<DeliveryPincode> DeliveryPincodes { get; set; }
        public virtual List<Franchise> Franchises { get; set; }
        public virtual PersonalDetail PersonalDetail { get; set; }
        public virtual PersonalDetail PersonalDetail1 { get; set; }
        public virtual List<PersonalDetail> PersonalDetails { get; set; }
        public virtual List<Shop> Shops { get; set; }
        [NotMapped]
        public bool IsDeliverablePincode { get; set; }

        //------Added by Shaili on 16-07-19----------------//
        //public virtual ICollection<Merchant> Merchant { get; set; }
        //------------End--------------------------------//
    }
}
