using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ModelLayer.Models
{
    public partial class DeliveryPartner
    {
        public DeliveryPartner()
        {
            this.DeliveryCashHandlingCharges = new List<DeliveryCashHandlingCharge>();
            this.DeliveryOrderDetails = new List<DeliveryOrderDetail>();
            this.DeliveryPincodes = new List<DeliveryPincode>();
            this.DeliveryWeightSlabs = new List<DeliveryWeightSlab>();
            this.Shops = new List<Shop>();
        }

        public int ID { get; set; }
        public long BusinessDetailID { get; set; }
        //[Required(ErrorMessage = "Godown address is required")]
        [StringLength(50, ErrorMessage = "Address must be less then 150.")]
        public string GodownAddress { get; set; }
       // [Required(ErrorMessage = "Pincode is required")]
        public int PincodeID { get; set; }
        public string ServiceNumber { get; set; }
        //[Required(ErrorMessage = "Service level is required")]
        public int ServiceLevel { get; set; }
        //[Required(ErrorMessage = "Contact persion is required")]
        public string ContactPerson { get; set; }
        [RegularExpression(@"^(?:\d{10}|00\d{10}|\+\d{2}\d{8})$", ErrorMessage = "Please enter proper Mobile No.")]
        //[Required(ErrorMessage = "Mobile No. is required")]
        public string Mobile { get; set; }
        //[Required(ErrorMessage = "Email is required (we promise not to spam you!)")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        [RegularExpression(@"^(?:\d{10}|00\d{10}|\+\d{2}\d{8})$", ErrorMessage = "Please enter proper Landline No.")]
        public string Landline { get; set; }
        public string FAX { get; set; }
        [Required]
        public int VehicleTypeID { get; set; }
        //[Required(ErrorMessage = "Opening time is required")]
        public System.TimeSpan OpeningTime { get; set; }
        //[Required(ErrorMessage = "Closing time is required")]
        public System.TimeSpan ClosingTime { get; set; }
        public string WeeklyOff { get; set; }
        public bool IsLive { get; set; }
        public bool IsActive { get; set; }
        public System.DateTime CreateDate { get; set; }
        public long CreateBy { get; set; }
        public Nullable<System.DateTime> ModifyDate { get; set; }
        public Nullable<long> ModifyBy { get; set; }
        public string NetworkIP { get; set; }
        public string DeviceType { get; set; }
        public string DeviceID { get; set; }
        public virtual BusinessDetail BusinessDetail { get; set; }
        public virtual ICollection<DeliveryCashHandlingCharge> DeliveryCashHandlingCharges { get; set; }
        public virtual ICollection<DeliveryOrderDetail> DeliveryOrderDetails { get; set; }
        public virtual PersonalDetail PersonalDetail { get; set; }
        public virtual Pincode Pincode { get; set; }
        public virtual VehicleType VehicleType { get; set; }
        public virtual ICollection<DeliveryPincode> DeliveryPincodes { get; set; }
        public virtual ICollection<DeliveryWeightSlab> DeliveryWeightSlabs { get; set; }
        public virtual PersonalDetail PersonalDetail1 { get; set; }
        public virtual ICollection<Shop> Shops { get; set; }
    }
}
