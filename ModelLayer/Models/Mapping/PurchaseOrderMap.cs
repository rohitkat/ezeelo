using System.ComponentModel.DataAnnotations;
using System.Data.Entity.ModelConfiguration;


namespace ModelLayer.Models.Mapping
{
    public class PurchaseOrderMap : EntityTypeConfiguration<PurchaseOrder>
    {
        public PurchaseOrderMap()
        {
            this.HasKey(t => t.ID);

            // Properties
            this.Property(t => t.PurchaseOrderCode).HasMaxLength(15);

            this.Property(t => t.WarehouseID);

            this.Property(t => t.SupplierID);

            this.Property(t => t.OrderDate);

            this.Property(t => t.Remark).HasMaxLength(4000);

            this.Property(t => t.IsActive);

            this.Property(t => t.NetworkIP)
                .HasMaxLength(15);

            this.Property(t => t.DeviceType)
                .HasMaxLength(50);

            this.Property(t => t.DeviceID)
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("PurchaseOrder");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.PurchaseOrderCode).HasColumnName("PurchaseOrderCode");
            this.Property(t => t.WarehouseID).HasColumnName("WarehouseID");
            this.Property(t => t.SupplierID).HasColumnName("SupplierID");
            this.Property(t => t.OrderDate).HasColumnName("OrderDate");
            this.Property(t => t.Remark).HasColumnName("Remark");           
            this.Property(t => t.IsActive).HasColumnName("IsActive");
            this.Property(t => t.CreateDate).HasColumnName("CreateDate");
            this.Property(t => t.CreateBy).HasColumnName("CreateBy");
            this.Property(t => t.ModifyDate).HasColumnName("ModifyDate");
            this.Property(t => t.ModifyBy).HasColumnName("ModifyBy");
            this.Property(t => t.NetworkIP).HasColumnName("NetworkIP");
            this.Property(t => t.DeviceType).HasColumnName("DeviceType");
            this.Property(t => t.DeviceID).HasColumnName("DeviceID");

            // Relationships
            this.HasRequired(t => t.PersonalDetails)
               .WithMany(t => t.PurchaseOrders)
               .HasForeignKey(d => d.CreateBy);
        }
    } 
}
