using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ModelLayer.Models
{
    /// <summary>
    /// Added By Pradnyakar Badge
    /// 25-05-2016
    /// </summary>
    public partial class Career
    {
        public Career()
        {
            this.ApplicationDetails = new List<ApplicationDetail>();
        }

        public int ID { get; set; }
        [Required(ErrorMessage = "Jobtitle is Required")]
        [StringLength(200, MinimumLength = 1, ErrorMessage = "Jobtitle must be between 1 to 200 Chatacter")]
        public string Jobtitle { get; set; }
        [Required(ErrorMessage = "Education is Required")]
        [StringLength(300, MinimumLength = 1, ErrorMessage = "Education must be between 1 to 300 Chatacter")]
        public string Education { get; set; }
        [StringLength(150, MinimumLength = 1, ErrorMessage = "Experience Required must be between 1 to 150 Chatacter")]
        [Required(ErrorMessage = "Experience is Required")]
        public string ExperienceRequired { get; set; }
         [StringLength(300, MinimumLength = 1, ErrorMessage = "Skill Required must be between 1 to 300 Chatacter")]
         [Required(ErrorMessage = "Skill is Required")]
        public string SkillRequired { get; set; }
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "No of Opening is not valid")]
        public Nullable<int> NoOfOpening { get; set; }
        [StringLength(150, MinimumLength = 1, ErrorMessage = "Skill Required must be between 1 to 150 Chatacter")]
        [Required(ErrorMessage = "Location  is Required")]
        public string Location { get; set; }
        public string Domain { get; set; }      
        public Nullable<System.DateTime> PostDate { get; set; }      
        public Nullable<System.DateTime> ExpiryDate { get; set; }
        [Required(ErrorMessage = "Description is Required")]
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public System.DateTime CreateDate { get; set; }
        public long CreateBy { get; set; }
        public Nullable<System.DateTime> ModifyDate { get; set; }
        public Nullable<long> ModifyBy { get; set; }
        public string NetworkIP { get; set; }
        public string DeviceType { get; set; }
        public string DeviceID { get; set; }
        public virtual ICollection<ApplicationDetail> ApplicationDetails { get; set; }
    }
}
