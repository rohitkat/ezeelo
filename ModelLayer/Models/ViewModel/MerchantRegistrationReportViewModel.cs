using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
//<copyright file="MerchantRegistrationReportViewModel.cs" company="Ezeelo Consumer Services Pvt. Ltd.">
//     Copyright (c) Ezeelo Consumer Services Pvt. Ltd. All rights reserved.
//    </copyright>
//    <author>Harshada Raghorte</author>
namespace ModelLayer.Models.ViewModel
{
    public class MerchantRegistrationReportViewModel
    {
        public long ID { get; set; }
        public string VendorName { get; set; }
        public long VendorRegistrationNo { get; set; }
        public string OrganisationName { get; set; }
        public string BuisnessNature { get; set; }
        public string MobileNo { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string Landmark { get; set; }
        public int PincodeID { get; set; }
        public Nullable<System.DateTime> YearOfEstablishment { get; set; }
        public System.DateTime RegistrationDate { get; set; }
        public Nullable<System.TimeSpan> ShopOpeningTime { get; set; }
        public Nullable<System.TimeSpan> ShopClosingTime { get; set; }
       
      
      
    }
}