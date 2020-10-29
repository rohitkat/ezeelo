using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace ModelLayer.Models.Mapping
{
    public class ShopStockBulkLogMap : EntityTypeConfiguration<ShopStockBulkLog>
    {
        public ShopStockBulkLogMap()
        {
            // Primary Key
            this.HasKey(t => t.ID);

            // Properties
            this.Property(t => t.Result)
                .IsRequired()
                .HasMaxLength(10);

            this.Property(t => t.Description)
                .HasMaxLength(500);

            this.Property(t => t.NetworkIP)
                .HasMaxLength(15);

            this.Property(t => t.DeviceType)
                .HasMaxLength(50);

            this.Property(t => t.DeviceID)
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("ShopStockBulkLog");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.TempProductID).HasColumnName("TempProductID");
            this.Property(t => t.TempShopStockID).HasColumnName("TempShopStockID");
            this.Property(t => t.ExcelRowID).HasColumnName("ExcelRowID");
            this.Property(t => t.BulkLogID).HasColumnName("BulkLogID");
            this.Property(t => t.Result).HasColumnName("Result");
            this.Property(t => t.Description).HasColumnName("Description");
            this.Property(t => t.ImageCount).HasColumnName("ImageCount");
            this.Property(t => t.CreateDate).HasColumnName("CreateDate");
            this.Property(t => t.CreateBy).HasColumnName("CreateBy");
            this.Property(t => t.NetworkIP).HasColumnName("NetworkIP");
            this.Property(t => t.DeviceType).HasColumnName("DeviceType");
            this.Property(t => t.DeviceID).HasColumnName("DeviceID");

            // Relationships
            this.HasRequired(t => t.BulkLog)
                .WithMany(t => t.ShopStockBulkLogs)
                .HasForeignKey(d => d.BulkLogID);
            this.HasRequired(t => t.PersonalDetail)
                .WithMany(t => t.ShopStockBulkLogs)
                .HasForeignKey(d => d.CreateBy);
            this.HasRequired(t => t.TempProduct)
                .WithMany(t => t.ShopStockBulkLogs)
                .HasForeignKey(d => d.TempProductID);
            this.HasRequired(t => t.TempShopStock)
                .WithMany(t => t.ShopStockBulkLogs)
                .HasForeignKey(d => d.TempShopStockID);

        }
    }
}
