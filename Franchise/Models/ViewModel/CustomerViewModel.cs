using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Franchise.Models.ViewModel
{
    public class CustomerViewModel
    {
        public long UserLoginID { get; set; }
        public string RegMobile { get; set; }
        public string RegEmail { get; set; }
        public Boolean IsLocked { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime CreateDate { get; set; }
        public string Salutation { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public Nullable<DateTime> DOB { get; set; }
        public string Gender { get; set; }
        public long PincodeID { get; set; }
        public string Pincode { get; set; }
        public string Address { get; set; }
        public string AlternateMobile { get; set; }
        public string AlternateEmail { get; set; }
        public Boolean IsActive { get; set; }
        public int TotalOrder { get; set; }
        public decimal TotalAmount { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public Nullable<DateTime> LastPurchasedDate { get; set; }
        public int FedCount { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public Nullable<DateTime> LastFeedbackDate { get; set; }

        public string IsLeader { get; set; } //Yashaswi 30-7-2018
        public decimal RetailPoints { get; set; } //Yashaswi 30-7-2018
    }
}
