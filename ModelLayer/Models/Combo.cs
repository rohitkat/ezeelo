using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ModelLayer.Models
{
    [Table("Combo")]
    public class Combo
    {
        public Combo()
        {
            this.ComboProductList = new List<ComboProducts>();
        }

        [Key]
        public long ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public long FranchiseID { get; set; }
        public long ShopID { get; set; }
        public decimal TotalMRP { get; set; }
        public decimal TotalSaleRate { get; set; }
        public long TotalRetailPoints { get; set; }
        public bool IsApproved { get; set; }
        public long ApprovedBy { get; set; }
        public DateTime CreateDate { get; set; }
        public long CreatedBy { get; set; }
        public DateTime ModifyDate { get; set; }
        public long ModifiedBy { get; set; }
        public string NetworkIP { get; set; }
        public virtual ICollection<ComboProducts> ComboProductList { get; set; }
    }

    [Table("ComboProducts")]
    public class ComboProducts
    {
        [Key]
        public long ID { get; set; }
        public long ComboID { get; set; }
        public long ShopStockId { get; set; }
        public int Qty { get; set; }
        public decimal MRP { get; set; }
        public decimal SaleRate { get; set; }
        public long RetailPoints { get; set; }
        public long WarehouseStockID { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreateDate { get; set; }
        public long CreatedBy { get; set; }
        public DateTime ModifyDate { get; set; }
        public long ModifiedBy { get; set; }
        public string NetworkIP { get; set; }

        [ForeignKey("ComboID")]
        public virtual Combo objCombo { get; set; }
    }
}
