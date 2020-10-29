using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace ModelLayer.Models
{
    [Table("WarehouseZone")]
    public class WarehouseZone
    {
        [Key]
        public long ID { get; set; }
        public long WarehouseID { get; set; }
        public int ZoneID { get; set; }
        public string Name { get; set; }
        public DateTime CreateDate { get; set; }
        public long CreateBy { get; set; }
        public DateTime? ModifyDate { get; set; }
        public long? ModifyBy { get; set; }
        public string NetworkIP { get; set; }

        [NotMapped]
        public SelectList ZoneList { get; set; }
     
    }
}
