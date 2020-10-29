using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace ModelLayer.Models.Mapping
{
    public class TaxOnOrderMap : EntityTypeConfiguration<TaxOnOrder>
    {
        public TaxOnOrderMap()
        {
            // Primary Key
            this.HasKey(t => t.ID);

            // Properties
            this.Property(t => t.NetworkIP)
                .HasMaxLength(15);

            this.Property(t => t.DeviceType)
                .HasMaxLength(50);

            this.Property(t => t.DeviceID)
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("TaxOnOrder");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.CustomerOrderDetailID).HasColumnName("CustomerOrderDetailID");
            this.Property(t => t.ProductTaxID).HasColumnName("ProductTaxID");
            this.Property(t => t.Amount).HasColumnName("Amount");
            this.Property(t => t.CreateDate).HasColumnName("CreateDate");
            this.Property(t => t.CreateBy).HasColumnName("CreateBy");
            this.Property(t => t.ModifyDate).HasColumnName("ModifyDate");
            this.Property(t => t.ModifyBy).HasColumnName("ModifyBy");
            this.Property(t => t.NetworkIP).HasColumnName("NetworkIP");
            this.Property(t => t.DeviceType).HasColumnName("DeviceType");
            this.Property(t => t.DeviceID).HasColumnName("DeviceID");

            // Relationships
            this.HasRequired(t => t.CustomerOrderDetail)
                .WithMany(t => t.TaxOnOrders)
                .HasForeignKey(d => d.CustomerOrderDetailID);
            this.HasRequired(t => t.ProductTax)
                .WithMany(t => t.TaxOnOrders)
                .HasForeignKey(d => d.ProductTaxID);

        }
    }
}
