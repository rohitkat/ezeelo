using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models
{
    [Table("WarehouseStockLog")]
    public class WarehouseStockLog
    {
        [Key]
        public long ID { get; set; }
        public long WarehouseStockId { get; set; }
        public int Quantity { get; set; }
        public long CreateBy { get; set; }
        public DateTime CreateDate { get; set; }
        public int Status { get; set; }
        public string NetworkIp { get; set; }
    }
}
