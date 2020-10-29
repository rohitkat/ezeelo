using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models
{
    /// <summary>
    /// Yashaswi 17-3-2018
    /// </summary>
    [Table("WarehouseReturnStockDetails")]
    public class WarehouseReturnStockDetails
    {
        [Key]
        public long ID { get; set; }
        public long WarehouseReturnStockId { get; set; }
        public decimal ReturnRatePerUnit { get; set; }
        public long SubReasonId { get; set; }
        public long WarehouseStockId { get; set; }
        public int Quantity { get; set; }
        public string Remark { get; set; }
    }
}
