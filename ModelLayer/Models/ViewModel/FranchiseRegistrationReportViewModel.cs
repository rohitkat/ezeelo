using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//<copyright file="FranchiseRegistrationReportViewModel.cs" company="Ezeelo Consumer Services Pvt. Ltd.">
//     Copyright (c) Ezeelo Consumer Services Pvt. Ltd. All rights reserved.
//    </copyright>
//    <author>Harshada Raghorte</author>
namespace ModelLayer.Models.ViewModel
{
    public class FranchiseRegistrationReportViewModel
    {
        public long ID { get; set; }
        public string FranchiseName { get; set; }
        public string OwnerName { get; set; }
        public long FranchiseRegistrationNo { get; set; }
        public string MobileNo { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string Landmark { get; set; }
        public string Pincode { get; set; }
        public string State { get; set; }
        public string City { get; set; }
        public string ContactPerson { get; set; }
        public string ContactPersonEmail { get; set; }
        public System.DateTime RegistrationDate { get; set; }


    }
}
