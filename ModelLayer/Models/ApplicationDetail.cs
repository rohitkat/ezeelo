using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ModelLayer.Models
{
    /// <summary>
    /// Added By Pradnyakar Badge
    /// 25-05-2016
    /// </summary>
    public partial class ApplicationDetail
    {
        public long ID { get; set; }
        [Required(ErrorMessage="Invalid Opening Selection")]
        public int CareerID { get; set; }
        [Required(ErrorMessage = "Please Enter Your Name")]
        [RegularExpression(@"^[A-Za-z? ,_-]+$",
                       ErrorMessage = "Name is not valid")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Email is Required")]
        [RegularExpression(@"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}" +
                        @"\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\" +
                        @".)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$",
                        ErrorMessage = "Email is not valid")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Mobile No is Required")]
        [RegularExpression(@"^([6-9]{1}[0-9]{9})$", ErrorMessage = "Mobile is not valid")]
        [StringLength(10)]
        public string Mobile { get; set; }
        [Required(ErrorMessage = "Please Mention your experience in month/year")]
        public string TotalExpience { get; set; }
        [Required(ErrorMessage = "Your Current CTC is Required")]
        public string CurrentCTC { get; set; }
        [Required(ErrorMessage = "Enter Expected CTC")]
        public string ExpectedCTC { get; set; }
        public string ResumePath { get; set; }
        public string Remarks { get; set; }
        public DateTime AppliedDate { get; set; }
        public virtual Career Career { get; set; }
    }
}
