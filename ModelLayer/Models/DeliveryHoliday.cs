using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ModelLayer.Models
{
    public partial class DeliveryHoliday
    {
        public int ID { get; set; }
        
        public System.DateTime Date { get; set; }
        public bool IsActive { get; set; }
        public System.DateTime CreateDate { get; set; }
        public long CreateBy { get; set; }
        public Nullable<System.DateTime> ModifyDate { get; set; }
        public Nullable<long> ModifyBy { get; set; }
        public string NetworkIP { get; set; }
        public string DeviceType { get; set; }
        public string DeviceID { get; set; }
        
        public Nullable<long> CityID { get; set; }
        public Nullable<int> FranchiseID { get; set; }//added
        public virtual City City { get; set; }
        public virtual Franchise Franchise { get; set; }////added
    }
}
