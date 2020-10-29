using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ModelLayer.Models
{
    public partial class Advertisement
    {
        public Advertisement()
        {
            this.OwnerAdvertisements = new List<OwnerAdvertisement>();
        }

        public int ID { get; set; }

        [Required(ErrorMessage = "Page Name is Required")]
        [StringLength(50, MinimumLength = 5, ErrorMessage = "Page Name must be between 5 to 50 Chatacter")]
        public string PageName { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Width in pixel must be ve+ or integer")]
        public Nullable<int> WidthInPixel { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "HeightInPixel must be ve+ or integer")]
        public Nullable<int> HeightInPixel { get; set; }
        public string Alignment { get; set; }

         [Range(1, int.MaxValue, ErrorMessage = "NumberOfDays must be ve+ or integer")]
        public Nullable<int> NumberOfDays { get; set; }
        public Nullable<decimal> FeesInRs { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public System.DateTime CreateDate { get; set; }
        public long CreateBy { get; set; }
        public Nullable<System.DateTime> ModifyDate { get; set; }
        public Nullable<long> ModifyBy { get; set; }
        public string NetworkIP { get; set; }
        public string DeviceType { get; set; }
        public string DeviceID { get; set; }
        public virtual PersonalDetail PersonalDetail { get; set; }
        public virtual PersonalDetail PersonalDetail1 { get; set; }
        public virtual List<OwnerAdvertisement> OwnerAdvertisements { get; set; }
    }
}
