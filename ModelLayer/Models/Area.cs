using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ModelLayer.Models
{
    public partial class Area
    {
        public Area()
        {
            this.CustomerOrders = new List<CustomerOrder>();
            this.CustomerShippingAddresses = new List<CustomerShippingAddress>();
            this.FranchiseLocations = new List<FranchiseLocation>();
            this.Shops = new List<Shop>();
        }

        public int ID { get; set; }
        [Required(ErrorMessage = "Area Name is Required")]
        [StringLength(150, MinimumLength=3 ,ErrorMessage="Area Name must be between 3 to 150 Chatacter")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Pincode is Required")]
        [Range(1,int.MaxValue,ErrorMessage="Pincode is not valid")]
        public int PincodeID { get; set; }
        public bool IsActive { get; set; }
        public System.DateTime CreateDate { get; set; }
        public long CreateBy { get; set; }
        public Nullable<System.DateTime> ModifyDate { get; set; }
        public Nullable<long> ModifyBy { get; set; }
        public string NetworkIP { get; set; }
        public string DeviceType { get; set; }
        public string DeviceID { get; set; }
        public virtual Pincode Pincode { get; set; }
        public virtual PersonalDetail PersonalDetail { get; set; }
        public virtual List<CustomerOrder> CustomerOrders { get; set; }
        public virtual List<CustomerShippingAddress> CustomerShippingAddresses { get; set; }
        public virtual List<FranchiseLocation> FranchiseLocations { get; set; }
        public virtual PersonalDetail PersonalDetail1 { get; set; }
        public virtual List<Shop> Shops { get; set; }
    }
}
