using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ModelLayer.Models
{
    public partial class DeliverySchedule
    {
        public DeliverySchedule()
        {
            this.OrderDeliveryScheduleDetails = new List<OrderDeliveryScheduleDetail>();
        }

        public long ID { get; set; }  
        
        public string DisplayName { get; set; }
        public System.TimeSpan ActualTimeFrom { get; set; }
        public System.TimeSpan ActualTimeTo { get; set; }
       
        [Range(1, 999999, ErrorMessage = "Please enter a valid number.")]
        public int NoOfDelivery { get; set; }
        public bool IsActive { get; set; }
        public System.DateTime CreateDate { get; set; }
        public long CreateBy { get; set; }
        public Nullable<System.DateTime> ModifyDate { get; set; }
        public Nullable<long> ModifyBy { get; set; }
        public string NetworkIP { get; set; }
        public string DeviceType { get; set; }
        public string DeviceID { get; set; }
        
        public Nullable<long> CityID { get; set; }
        public virtual City City { get; set; }
        public virtual Franchise Franchise { get; set; }////added
        public Nullable<int> FranchiseID { get; set; }//added
        public virtual ICollection<OrderDeliveryScheduleDetail> OrderDeliveryScheduleDetails { get; set; }
    }
}
