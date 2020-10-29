using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Franchise.Models.ViewModel
{
    public class MyProfileViewModel
    {
        public long ID { get; set; }
        public int UserTypeID { get; set; }
        public Nullable<long> PersonalDetailID { get; set; }
        public Nullable<long> BusinessDetailID { get; set; }
        public Nullable<long> DeliveryPartnerID { get; set; }
    }
}