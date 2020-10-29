using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models
{
    [Table("RateCalculationWithDV")]
   public class RateCalculationWithDVs
    {
        [Key]
        public long ID { get; set; }
        public long? ProductID { get; set; }
        public long? ProductVarientID { get; set; }

        public long WarehouseID { get; set; }
        public long? RateCalculationID { get; set; }
        public bool? IsActive { get; set; }

        public DateTime? CreateDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string NetworkIP { get; set; }

    }
}
