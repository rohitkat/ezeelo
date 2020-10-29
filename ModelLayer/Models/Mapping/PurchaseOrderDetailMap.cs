using System.ComponentModel.DataAnnotations;
using System.Data.Entity.ModelConfiguration;

namespace ModelLayer.Models.Mapping
{
    public class PurchaseOrderDetailMap : EntityTypeConfiguration<PurchaseOrderDetail>
    {
        public PurchaseOrderDetailMap()
        {
            this.HasKey(t => t.ID);

            // Properties
            this.Property(t => t.PurchaseOrderID);

            this.Property(t => t.ProductID);

            this.Property(t => t.ProductNickname);

          
            this.Property(t => t.ProductVarientID);

          
            this.Property(t => t.Quantity);

            this.Property(t => t.IsActive);           

            // Table & Column Mappings
            this.ToTable("PurchaseOrderDetail");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.PurchaseOrderID).HasColumnName("PurchaseOrderID");
            this.Property(t => t.ProductID).HasColumnName("ProductID");
            this.Property(t => t.ProductNickname).HasColumnName("ProductNickname");
            this.Property(t => t.ProductVarientID).HasColumnName("ProductVarientID");           
            this.Property(t => t.Quantity).HasColumnName("Quantity");
            this.Property(t => t.IsActive).HasColumnName("IsActive");
        }
    }
}
