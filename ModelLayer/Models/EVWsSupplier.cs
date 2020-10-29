using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ModelLayer.Models
{
    [Table("EVWsSupplier")]
    public class EVWsSupplier
    {
        public long ID { get; set; }
        public long WarehouseId { get; set; }
        public long SupplierId { get; set; }
        public bool IsActive { get; set; }
        public long CreateBy { get; set; }
        public DateTime CreateDate { get; set; }
        public long? ModifyBy { get; set; }
        public DateTime? ModifyDate { get; set;}
        public string NetworkIP { get; set; }
    }

    [Table("EVWsDV")]
    public class EVWsDV
    {
        public long ID { get; set; }
        public long WarehouseId_EVW { get; set; }
        public long WarehouseId { get; set; }
        public bool IsActive { get; set; }
        public long CreateBy { get; set; }
        public DateTime CreateDate { get; set; }
        public long? ModifyBy { get; set; }
        public DateTime? ModifyDate { get; set; }
        public string NetworkIP { get; set; }
    }
}
