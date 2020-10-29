using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CRM.Models.ViewModel
{
    public class ReviewIndexViewModel
    {
        public int ID { get; set; }
        public long PersonalDetailID { get; set; }
        public Nullable<long> OwnerID { get; set; }
        public int RatingID { get; set; }
        public Nullable<int> Point { get; set; }
        public string Feedback { get; set; }
        public bool IsApproved { get; set; }
        public Nullable<long> ApprovedBy { get; set; }
        public System.DateTime CreateDate { get; set; }
        public long CreateBy { get; set; }
        public Nullable<System.DateTime> ModifyDate { get; set; }
        public Nullable<long> ModifyBy { get; set; }
        public string NetworkIP { get; set; }
        public string DeviceType { get; set; }
        public string DeviceID { get; set; }

        //--------------------------------------------Extra added-------------------------------------//   
        public string ProductName { get; set; }

        public long ProductID { get; set; }

        public string AnalysedName { get; set; }

    }
}