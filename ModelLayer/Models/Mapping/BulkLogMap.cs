using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace ModelLayer.Models.Mapping
{
    public class BulkLogMap : EntityTypeConfiguration<BulkLog>
    {
        public BulkLogMap()
        {
            // Primary Key
            this.HasKey(t => t.ID);

            // Properties
            this.Property(t => t.ExcelSheetName)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.NetworkIP)
                .HasMaxLength(15);

            this.Property(t => t.DeviceType)
                .HasMaxLength(50);

            this.Property(t => t.DeviceID)
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("BulkLog");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.ShopID).HasColumnName("ShopID");
            this.Property(t => t.ExcelSheetName).HasColumnName("ExcelSheetName");
            this.Property(t => t.BulkType).HasColumnName("BulkType");
            this.Property(t => t.TotalSuccess).HasColumnName("TotalSuccess");
            this.Property(t => t.TotalFail).HasColumnName("TotalFail");
            this.Property(t => t.CreateDate).HasColumnName("CreateDate");
            this.Property(t => t.CreateBy).HasColumnName("CreateBy");
            this.Property(t => t.NetworkIP).HasColumnName("NetworkIP");
            this.Property(t => t.DeviceType).HasColumnName("DeviceType");
            this.Property(t => t.DeviceID).HasColumnName("DeviceID");

            // Relationships
            this.HasRequired(t => t.Shop)
                .WithMany(t => t.BulkLogs)
                .HasForeignKey(d => d.ShopID);
            this.HasRequired(t => t.PersonalDetail)
                .WithMany(t => t.BulkLogs)
                .HasForeignKey(d => d.CreateBy);

        }
    }
}
