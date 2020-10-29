using PagedList;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ModelLayer.Models
{
    public partial class FeedbackManagment
    {
        public int ID { get; set; }
        //[Required(ErrorMessage = "Email is required (we promise not to spam you!).")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        [RegularExpression(@"^(?:\d{10}|00\d{10}|\+\d{2}\d{8})$", ErrorMessage = "Please! enter proper Mobile No.")]
       // [Required(ErrorMessage = "Mobile No. is required")]
        public string Mobile { get; set; }
      [Required(ErrorMessage = "Please ! select feedback category.")]
        public int FeedbackCategaryID { get; set; }
        [Required(ErrorMessage = "Please ! enter message.")]
        public string Message { get; set; }
        public Nullable<int> FeedBackTypeID { get; set; }
        public string SearchKeywords { get; set; }    //By Amit
        public bool IsActive { get; set; }
        public System.DateTime CreateDate { get; set; }
        public Nullable<long> CreateBy { get; set; }
        public Nullable<System.DateTime> ModifyDate { get; set; }
        public Nullable<long> ModifyBy { get; set; }
        public string NetworkIP { get; set; }
        public string DeviceType { get; set; }
        public string DeviceID { get; set; }

        public Nullable<long> CityID { get; set; }
        public Nullable<int> FranchiseID { get; set; }////added
        public virtual FeedbackCategary FeedbackCategary { get; set; }
        public virtual FeedBackType FeedBackType { get; set; }
        public virtual PersonalDetail PersonalDetail { get; set; }
        public virtual PersonalDetail PersonalDetail1 { get; set; }

        public virtual City Cities { get; set; }

        public string CustOrderCode { get; set; }

       
        public string EmailSMSText { get; set; }
    }


    public class FeedbackGroupWise
    {
        public IPagedList<FeedbackManagment> AllFeedback { get; set; }
        public IPagedList<FeedbackManagment> OrderWiseFeedback { get; set; }
        public IPagedList<FeedbackManagment> ProductRequirementFeedback { get; set; }
    }
   
}
