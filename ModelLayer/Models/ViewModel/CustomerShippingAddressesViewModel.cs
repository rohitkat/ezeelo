//-----------------------------------------------------------------------
// <copyright file="CustomerShippingAddressesViewModel" company="Ezeelo Consumer Services Pvt. Ltd.">
//     Copyright (c) Ezeelo Consumer Services Pvt. Ltd. All rights reserved.
// </copyright>
// <author>Sujata Kullarkar</author>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models.ViewModel
{
 public class CustomerShippingAddressesViewModel
    {
        public int AddressID { get; set; }
        public long UserLoginID { get; set; }
        public string PrimaryMobile { get; set; }
        public string SecondaryMobile { get; set; }
        public string ShippingAddress { get; set; }
        public int PincodeID { get; set; }
        public string PincodeName { get; set; }
        public long CityID { get; set; }
        public string CityName { get; set; }
        public long DistrictID { get; set; }
        public string DistrictName { get; set; }
        public long StateID { get; set; }
        public string StateName { get; set; }
        public Nullable<int> AreaID { get; set; }
        public string AreaName { get; set; }
        public bool IsSelected { get; set; }
        public bool IsDeliveryAddress { get; set; }//Sonali for Api_25/10/2018
        public string FirstName { get; set; }//Sonali for Api_25/10/2018
        public string LastName { get; set; }//Sonali for Api_25/10/2018
    }
}
