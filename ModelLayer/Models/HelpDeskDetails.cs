using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models
{
    public partial class HelpDeskDetails
    {
        public int ID { get; set; }
        [Required]
        public long CityID { get; set; }
        [Required]
        public int FranchiseID { get; set; }
        [RegularExpression(@"^(?:\d{10}|00\d{10}|\+\d{2}\d{8})$", ErrorMessage = "Please enter proper Mobile No.")]
        [Required(ErrorMessage = "Mobile No. is required")]
        public string HelpLineNumber { get; set; }
        [RegularExpression(@"^(?:\d{10}|00\d{10}|\+\d{2}\d{8})$", ErrorMessage = "Please enter proper Mobile No.")]
        public string ManagmentNumber { get; set; }
        public bool IsActive { get; set; }
        public System.DateTime CreateDate { get; set; }
        public long CreateBy { get; set; }
        public Nullable<System.DateTime> ModifyDate { get; set; }
        public Nullable<long> ModifyBy { get; set; }
        public string NetworkIP { get; set; }
        public string DeviceType { get; set; }
        public string DeviceID { get; set; }
        public virtual City City { get; set; }
        public virtual Franchise Franchise { get; set; }
       
    }
}
