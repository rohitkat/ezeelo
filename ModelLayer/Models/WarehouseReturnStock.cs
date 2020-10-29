
namespace ModelLayer.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    /// <summary>
    /// Yashaswi 17-3-2018
    /// </summary>
    [Table("WarehouseReturnStock")]
    public partial class WarehouseReturnStock
    {
        [Key]
        public long ID { get; set; }
        public long InvoiceId { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal? AdditionalCharge { get; set; }
        public decimal? TransportationCharge { get; set; }
        public string Remark { get; set; }
        public bool IsApproved { get; set; }
        public long? ApprovedBy { get; set; }
        public DateTime? ApprovedDate { get; set; } //Yashaswi Inventory Return 20-12-2018
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
