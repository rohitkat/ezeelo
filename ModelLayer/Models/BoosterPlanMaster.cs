using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ModelLayer.Models
{
    [Table("BoosterPlanSubscriber")]
    public class BoosterPlanSubscriber
    {
        [Key]
        public long ID { get; set; }
        public int BoosterPlanMasterId { get; set; }
        public long CustomerOrderId { get; set; }
        public bool IsPaid { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreateDate { get; set; }
        public long CreateBy { get; set; }
        public DateTime? ModifyDate { get; set; }
        public long? ModifyBy { get; set; }
        public string NetworkIP { get; set; }
        public string DeviceType { get; set; }
        public bool IsDesignationChanged { get; set; }

        [ForeignKey("BoosterPlanMasterId")]
        public virtual BoosterPlanMaster BoosterPlanMaster { get; set; }
    }

    [Table("BoosterPlanMaster")]
    public class BoosterPlanMaster
    {
        [Key]
        public int ID { get; set; }
        public long BoosterCategoryId { get; set; }
        public int RetailPoints { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreateDate { get; set; }
    }
}
