using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Franchise.Models.ViewModel
{
    public class CustomerCareSessionViewModel
    {
        //public long ID { get; set; }
        public long UserLoginID { get; set; }
        public int BusinessTypeId { get; set; }
        public string Username { get; set; }
        public long PersonalDetailID { get; set; }
        public long BusinessDetailID { get; set; }
        public int DeliveryPartnerID { get; set; }
        public string EmployeeCode { get; set; }
    }
}