using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
namespace ModelLayer.Models.ViewModel
{
    public class DeliveryBoyManagement
    {
        public long ID { get; set; }
        public long FranchiseID { get; set; }

        [Required(ErrorMessage = "Name is Required")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Modile Number is Required")]
        [RegularExpression(@"^(?:\d{10}|00\d{10}|\+\d{2}\d{8})$", ErrorMessage = "Please enter proper Mobile No.")]
        public string Mobile { get; set; }

        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is Required")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Address is Required")]
        public string Address { get; set; }

        [Required(ErrorMessage = "Adhaar No is Required")]
        public string AdhaarNo { get; set; }

        [Required(ErrorMessage = "Adhaar image is Required")]
        public string AdhaarImageUrl { get; set; }

        [Required(ErrorMessage = "Driving License image is Required")]
        public string DrivingLicenseUrl { get; set; }

        public bool IsVerified { get; set; }
        public bool IsActive { get; set; }
    }
}
