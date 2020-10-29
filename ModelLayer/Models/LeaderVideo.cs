using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models
{
    [Table("LeaderVideo")]
    public class LeaderVideo
    {
        public long ID { get; set; }
        public string Link { get; set; }
        public long CreatedBy { get; set; }
        public DateTime CreateDate { get; set; }
        public long? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }
}
