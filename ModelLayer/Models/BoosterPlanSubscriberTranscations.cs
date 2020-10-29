using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ModelLayer.Models
{
    [Table("BoosterPlanSubscriberTranscations")]
    public class BoosterPlanSubscriberTranscations
    {
        [Key]
        public long ID { get; set; }
        public long UserLoginID { get; set; }
        public long CustomerOrderID { get; set; }
        public long BoosterPlanSubscriberID { get; set; }
        public int Status { get; set; }
        public decimal RetailPoints { get; set; }
        public decimal OrderAmount { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreateDate { get; set; }
        public long CreateBy { get; set; }
        public DateTime? ModifyDate { get; set; }
        public long? ModifyBy { get; set; }
        public string NetworkIP { get; set; }
    }
}
