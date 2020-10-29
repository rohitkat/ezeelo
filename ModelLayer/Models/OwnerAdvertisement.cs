using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ModelLayer.Models
{
    public partial class OwnerAdvertisement
    {
        public int ID { get; set; }

        [Required(ErrorMessage = "Advertisement is Required")]
        [Range(1, int.MaxValue, ErrorMessage = "Invalid Advertisement")]
        public int AdvertisementID { get; set; }

        [Required(ErrorMessage = "Title is Required")]
        [StringLength(150, MinimumLength = 5, ErrorMessage = "Title must be between 5 to 150 Chatacter")]
        public string AdvertisementTitle { get; set; }

        [Required(ErrorMessage = "NavigationUrl is Required")]
        [StringLength(300, MinimumLength = 10, ErrorMessage = "Url must be between 10 to 150 Chatacter")]
        public string NavigationUrl { get; set; }

        [Required(ErrorMessage = "No Of Days is Required")]
        [Range(1, int.MaxValue, ErrorMessage = "Invalid No of Days")]
        public int NoOfDays { get; set; }


        [Required(ErrorMessage = "No Of Hours is Required")]
        [Range(double.MinValue, double.MaxValue, ErrorMessage = "Invalid No of Days")]
        public decimal NoOfHours { get; set; }

        [Required(ErrorMessage = "Business Type is Required")]
        [Range(1, int.MaxValue, ErrorMessage = "Invalid Business Type")]
        public int BusinessTypeID { get; set; }

        [Required(ErrorMessage = "Owner is Required")]
        //[Range(0, Int64.MaxValue, ErrorMessage = "Invalid Owner")]
        public long OwnerID { get; set; }
        public Nullable<decimal> FeesInRupee { get; set; }

        [Required(ErrorMessage = "Priority is Required")]
        [Range(1, int.MaxValue, ErrorMessage = "Invalid Priority Level")]
        public int PriorityLevel { get; set; }
        public bool IsLive { get; set; }
        public bool IsActive { get; set; }
        public System.DateTime CreateDate { get; set; }
        public long CreateBy { get; set; }
        public Nullable<System.DateTime> ModifyDate { get; set; }
        public Nullable<long> ModifyBy { get; set; }
        public string NetworkIP { get; set; }
        public string DeviceType { get; set; }
        public string DeviceID { get; set; }
        public virtual Advertisement Advertisement { get; set; }
        public virtual BusinessType BusinessType { get; set; }
        public virtual PersonalDetail PersonalDetail { get; set; }
        public virtual PersonalDetail PersonalDetail1 { get; set; }
    }
}
