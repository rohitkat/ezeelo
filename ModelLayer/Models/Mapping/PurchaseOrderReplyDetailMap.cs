using System.ComponentModel.DataAnnotations;
using System.Data.Entity.ModelConfiguration;

namespace ModelLayer.Models.Mapping
{
    public class PurchaseOrderReplyDetailMap : EntityTypeConfiguration<PurchaseOrderReplyDetail>
    {
        public PurchaseOrderReplyDetailMap()
        {
            this.HasKey(t => t.ID);
            // Properties
            this.Property(t => t.PurchaseOrderReplyID);
            this.Property(t => t.PurchaseOrderDetailID);
            this.Property(t => t.IsExtraItem);         
            this.Property(t => t.ProductID);
            this.Property(t => t.ProductNickname);          
            this.Property(t => t.ProductVarientID);          
            this.Property(t => t.Quantity);
            this.Property(t => t.BuyRatePerUnit);
            this.Property(t => t.MRP);
            this.Property(t => t.SaleRate);
            this.Property(t => t.CGSTAmount);
            this.Property(t => t.SGSTAmount);
            this.Property(t => t.IGSTAmount);
            this.Property(t => t.Amount);
            this.Property(t => t.Remark).HasMaxLength(500);

            // Table & Column Mappings
            this.ToTable("PurchaseOrderReplyDetail");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.PurchaseOrderReplyID).HasColumnName("PurchaseOrderReplyID");
            this.Property(t => t.PurchaseOrderDetailID).HasColumnName("PurchaseOrderDetailID");
            this.Property(t => t.IsExtraItem).HasColumnName("IsExtraItem");           
            this.Property(t => t.ProductID).HasColumnName("ProductID");
            this.Property(t => t.ProductNickname).HasColumnName("ProductNickname");
            this.Property(t => t.ProductVarientID).HasColumnName("ProductVarientID");           
            this.Property(t => t.Quantity).HasColumnName("Quantity");
            this.Property(t => t.BuyRatePerUnit).HasColumnName("BuyRatePerUnit");
            this.Property(t => t.MRP).HasColumnName("MRP");
            this.Property(t => t.SaleRate).HasColumnName("SaleRate");
            this.Property(t => t.CGSTAmount).HasColumnName("CGSTAmount");
            this.Property(t => t.SGSTAmount).HasColumnName("SGSTAmount");
            this.Property(t => t.IGSTAmount).HasColumnName("IGSTAmount");
            this.Property(t => t.Amount).HasColumnName("Amount");
            this.Property(t => t.Remark).HasColumnName("Remark");
       }
    }
}
