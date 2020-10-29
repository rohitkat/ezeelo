using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models
{
    [Table("MarginDivision")]
    public class MarginDivision
    {
        [Key]
        public long ID { get; set; }
        public int Code { get; set; }
        public string MarginFor { get; set; }
        public int MarginInPercentage { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreateDate { get; set; }
        public long CreateBy { get; set; }
        public DateTime? ModifyDate { get; set; }
        public long? ModifyBy { get; set; }
        public string NetworkIP { get; set; }
        public string DeviceType { get; set; }
        public string DeviceID { get; set; }       
    }
}
