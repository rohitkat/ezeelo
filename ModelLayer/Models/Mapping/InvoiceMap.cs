using System.ComponentModel.DataAnnotations;
using System.Data.Entity.ModelConfiguration;

namespace ModelLayer.Models.Mapping
{
   
    public class InvoiceMap : EntityTypeConfiguration<Invoice>
    {
        public InvoiceMap()
        {
            this.HasKey(t => t.ID);

            // Properties
            this.Property(t => t.PurchaseOrderID);

            this.Property(t => t.InvoiceDate);

            this.Property(t => t.TotalDiscountAmount);           

            this.Property(t => t.OrderAmount);                  

            this.Property(t => t.Remark).HasMaxLength(4000);

            this.Property(t => t.IsActive);

            this.Property(t => t.NetworkIP)
                .HasMaxLength(15);

            this.Property(t => t.DeviceType)
                .HasMaxLength(50);

            this.Property(t => t.DeviceID)
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("Invoice");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.PurchaseOrderID).HasColumnName("PurchaseOrderID");
            this.Property(t => t.InvoiceDate).HasColumnName("InvoiceDate");
            this.Property(t => t.TotalDiscountAmount).HasColumnName("TotalDiscountAmount");
            this.Property(t => t.OrderAmount).HasColumnName("OrderAmount");           
            this.Property(t => t.Remark).HasColumnName("Remark");
            this.Property(t => t.IsActive).HasColumnName("IsActive");
            this.Property(t => t.CreateDate).HasColumnName("CreateDate");
            this.Property(t => t.CreateBy).HasColumnName("CreateBy");
            this.Property(t => t.ModifyDate).HasColumnName("ModifyDate");
            this.Property(t => t.ModifyBy).HasColumnName("ModifyBy");
            this.Property(t => t.NetworkIP).HasColumnName("NetworkIP");
            this.Property(t => t.DeviceType).HasColumnName("DeviceType");
            this.Property(t => t.DeviceID).HasColumnName("DeviceID");
           
        }
    }
}
