using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models
{
    [Table("DeliveryCharge")]
    public class DeliveryCharge
    {
        [Key]
        public long ID { get; set; }
        public decimal Charges { get; set; }
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
        public virtual Franchise Franchise { get; set; }
        public Nullable<int> FranchiseID { get; set; }
        public decimal OrderAmount { get; set; }//OrderAmt added by Sonali_19-01-2019
        public decimal MinOrderAmount { get; set; }// added by Rumana_22-03-2019
        public String Message { get; set; }//added by Rumana_22-03-2019
    }
}
