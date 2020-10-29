using System.ComponentModel.DataAnnotations;
using System.Data.Entity.ModelConfiguration;


namespace ModelLayer.Models.Mapping
{
   public class QuotationMap : EntityTypeConfiguration<Quotation>
    {
       public QuotationMap()
        {
            this.HasKey(t => t.ID);

            // Properties
            this.Property(t => t.RequestFromWarehouseID);

            this.Property(t => t.QuotationCode).HasMaxLength(50);          

            this.Property(t => t.QuotationRequestDate);                     

            this.Property(t => t.Remark).HasMaxLength(1000);

            this.Property(t => t.IsActive);

            this.Property(t => t.NetworkIP)
                .HasMaxLength(15);

            this.Property(t => t.DeviceType)
                .HasMaxLength(50);

            this.Property(t => t.DeviceID)
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("Quotation");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.RequestFromWarehouseID).HasColumnName("RequestFromWarehouseID");
            this.Property(t => t.QuotationCode).HasColumnName("QuotationCode");
            this.Property(t => t.QuotationRequestDate).HasColumnName("QuotationRequestDate");
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
