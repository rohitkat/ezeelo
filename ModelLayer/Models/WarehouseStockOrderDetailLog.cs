using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models
{
    [Table("WarehouseStockOrderDetailLog")]
   public class WarehouseStockOrderDetailLog
    {
       [Key]
        public long ID { get; set; }
        public long WarehouseStockID { get; set; }       
        public int Quantity { get; set; }       
        public int OrderStatus { get; set; }
        public long CustomerOrderDetailID { get; set; }      
        public System.DateTime CreateDate { get; set; }
        public long CreateBy { get; set; }     
        public string NetworkIP { get; set; }
        public string DeviceType { get; set; }
        public string DeviceID { get; set; }   
    }
}
