using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DeliveryPartner.Models.ViewModel
{
    public class PincodeViewModel
    {
        public long ID { get; set; }
        public long CityID { get; set; }
        public string Name { get; set; }
        public Boolean IsSelected { get; set; }
    }
}