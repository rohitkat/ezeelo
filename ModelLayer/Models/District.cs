using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
namespace ModelLayer.Models
{
    public partial class District
    {
        public District()
        {
            this.Cities = new List<City>();
        }
        public long ID { get; set; }
        [Required(ErrorMessage = "District is Required")]
        [StringLength(150, MinimumLength = 3, ErrorMessage = "District must be between 3 to 150 characters ")] 
        public string Name { get; set; }

        [Required(ErrorMessage="State is Required")]
        [Range(1,Int64.MaxValue,ErrorMessage="State is not valid")]
        public long StateID { get; set; }
        public bool IsActive { get; set; }
        public System.DateTime CreateDate { get; set; }
        public long CreateBy { get; set; }
        public Nullable<System.DateTime> ModifyDate { get; set; }
        public Nullable<long> ModifyBy { get; set; }
        public string NetworkIP { get; set; }
        public string DeviceType { get; set; }
        public string DeviceID { get; set; }
        public virtual List<City> Cities { get; set; }
        public virtual PersonalDetail PersonalDetail { get; set; }
        public virtual State State { get; set; }
        public virtual PersonalDetail PersonalDetail1 { get; set; }
    }
}
