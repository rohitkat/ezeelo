using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models
{
    [Table("FCMUser")]
    public class FCMUser
    {
        [Key]
        public long ID { get; set; }
        public long UserLoginId { get; set; }
        public string FCMRegistartionId { get; set; }
        public string DeviceId { get; set; }
        public string DeviceType { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string NetworkIP { get; set; }
    }
}
