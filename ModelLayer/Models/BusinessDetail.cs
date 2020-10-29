using ModelLayer.CustomAnnotation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ModelLayer.Models
{
    public partial class BusinessDetail
    {
        public BusinessDetail()
        {
            this.Advertisers = new List<Advertiser>();
            this.DeliveryPartners = new List<DeliveryPartner>();
            this.Franchises = new List<Franchise>();
            this.Shops = new List<Shop>();
            //--------------start of code 01-Feb-2016-------------//
            this.ChannelPartners = new List<ChannelPartner>();
            //------------End of comment-------------------------//
        }

        public long ID { get; set; }
        public long UserLoginID { get; set; }

        //[Required(ErrorMessage = "Business Name is required")]
        [Display(Name = "Business Name")]
        public string Name { get; set; }
        public int BusinessTypeID { get; set; }

        [Required(ErrorMessage = "Contact person is required")]
        [Display(Name = "Contact Person")]
        public string ContactPerson { get; set; }
        [RegularExpression(@"^(?:\d{10}|00\d{10}|\+\d{2}\d{8})$", ErrorMessage = "Please enter proper Mobile No.")]
        [Required(ErrorMessage = "Mobile No. is required")]
        public string Mobile { get; set; }
        [Required(ErrorMessage = "Email is required (we promise not to spam you!).")]
        [RegularExpression(@"^([\w-\.]+@([\w-]+\.)+[\w-]{2,4})?$", ErrorMessage = "Please enter proper email")]
        public string Email { get; set; }

        //[RegularExpression(@"^[\d]{3,4}[\-\s]*[\d]{6,7}$", ErrorMessage = "Please enter proper Landline1.")]
        [RegularExpression(@"^\d{3}([ -]\d\d|\d[ -]\d|\d\d[ -])\d{6}$", ErrorMessage = "Please enter proper Landline No. Like 0712-7585689")]
        public string Landline1 { get; set; }

       // [RegularExpression(@"^[\d]{3,4}[\-\s]*[\d]{6,7}$", ErrorMessage = "Please enter proper Landline2.")]
        [RegularExpression(@"^\d{3}([ -]\d\d|\d[ -]\d|\d\d[ -])\d{6}$", ErrorMessage = "Please enter proper Landline No. Like 0712-7585689")]
        public string Landline2 { get; set; }
        public string FAX { get; set; }
        public string Address { get; set; }

        [Required]
        [Display(Name = "Pincode")]
        public int PincodeID { get; set; }
        public string Website { get; set; }


        [Display(Name = "Year Of Establishment")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]        
        //[FutureDateValidation(ErrorMessage="Invalid Date"]
        public DateTime? YearOfEstablishment { get; set; }

        [Display(Name = "Source Of Info")]
        public Nullable<int> SourceOfInfoID { get; set; }

        [Display(Name = "Source Of Info Description")]
        public string SourcesInfoDescription { get; set; }
        public bool IsActive { get; set; }
        public System.DateTime CreateDate { get; set; }
        public long CreateBy { get; set; }
        public Nullable<System.DateTime> ModifyDate { get; set; }
        public Nullable<long> ModifyBy { get; set; }
        public string NetworkIP { get; set; }
        public string DeviceType { get; set; }
        public string DeviceID { get; set; }
        public virtual List<Advertiser> Advertisers { get; set; }
        public virtual BusinessType BusinessType { get; set; }
        public virtual Pincode Pincode { get; set; }
        public virtual SourceOfInfo SourceOfInfo { get; set; }
        public virtual UserLogin UserLogin { get; set; }
        public virtual PersonalDetail PersonalDetail { get; set; }
        public virtual List<DeliveryPartner> DeliveryPartners { get; set; }
        public virtual List<Franchise> Franchises { get; set; }
        public virtual PersonalDetail PersonalDetail1 { get; set; }
        public virtual List<Shop> Shops { get; set; }
        //-----------Added on 01-Feb-2016-----------------//
        public virtual ICollection<ChannelPartner> ChannelPartners { get; set; }
        //------------------End of Code-------------------//
    }
}
