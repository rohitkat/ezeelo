using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models
{
    [Table("LeaderPromotionalPlan")]
    public class LeaderPromotionalPlan
    {
        public long Id { get; set; }
        public long FirstLevelCategoryId { get; set; }
        public DateTime CreateDate { get; set; }
        public bool IsActive { get; set; }
    }

    [Table("LeaderPromotionalPlanSubscriber")]
    public class LeaderPromotionalPlanSubscriber
    {
        public long Id { get; set; }
        public long UserLoginId { get; set; }
        public long CustomerOrderId { get; set; }
        public DateTime CreateDate { get; set; }
        public long CreateBy { get; set; }
        public DateTime? ModifyDate { get; set; }
        public long? ModifyBy { get; set; }
        public string NetworkIp { get; set; }
        public string DeviceTyoe { get; set; }
        public bool IsActive { get; set; }
    }
}
