using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
//<copyright file="CustomerRegistrationReportViewModel.cs" company="Ezeelo Consumer Services Pvt. Ltd.">
//     Copyright (c) Ezeelo Consumer Services Pvt. Ltd. All rights reserved.
//    </copyright>
//    <author>Harshada Raghorte</author>
namespace ModelLayer.Models.ViewModel
{
    public class CustomerRegistrationReportViewModel
    {
        //public List< UserLogin> userLogin { get; set; }
        //public List< PersonalDetail> personalDetail { get; set; }
            public Int64 ID { get; set; }
            public Int64 CustomerID { get; set; }
            public string FullName { get; set; }
            public string Email { get; set; }
            public string MobileNo { get; set; }
            public string Address { get; set; }
            public string CityName { get; set; }
            public System.DateTime RegistrationDate { get; set; }
            public string NetworkIP { get; set; }
            public int FranchiseID { get; set; }////added
            public string Franchises { get; set; }////added
            public virtual Franchise Franchise { get; set; }////added


        }
    
}