namespace ModelLayer.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("WarehouseWastageStock")]
    public partial class WarehouseWastageStock
    {
        [Key]
        public long ID { get; set; }
        public long WarehouseStockID { get; set; }
        public int WastageQuantity { get; set; }
        public long SubReasonID { get; set; }
        public string Remark { get; set; }
        public long LocationId { get; set; }
        public string Img_Path { get; set; }
        public DateTime CreateDate { get; set; }
        public long CreateBy { get; set; }
        public DateTime? ModifyDate { get; set; }
        public long? ModifyBy { get; set; }
        public string NetworkIP { get; set; }
        public string DeviceType { get; set; }
        public string DeviceID { get; set; }
    
    }
}
