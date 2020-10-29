using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models.Mapping
{
    public class ShopMenuPriorityMap : EntityTypeConfiguration<ShopMenuPriority>
    {
        public ShopMenuPriorityMap()
        {
            // Primary Key
            this.HasKey(t => t.ID);

            // Properties
            this.Property(t => t.ID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.CategoryName)
                .IsRequired()
                .HasMaxLength(150);

            this.Property(t => t.DeviceType)
                .HasMaxLength(50);

            this.Property(t => t.DeviceID)
                .HasMaxLength(50);

            this.Property(t => t.ImageName)
                .HasMaxLength(100);

            this.Property(t => t.Remarks)
                .HasMaxLength(200);

            // Table & Column Mappings
            this.ToTable("ShopMenuPriority");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.ShopID).HasColumnName("ShopID");
            this.Property(t => t.CategoryID).HasColumnName("CategoryID");
            this.Property(t => t.CategoryName).HasColumnName("CategoryName");
            this.Property(t => t.Level).HasColumnName("Level");
            this.Property(t => t.SequenceOrder).HasColumnName("SequenceOrder");
            this.Property(t => t.CreateDate).HasColumnName("CreateDate");
            this.Property(t => t.CreatedBy).HasColumnName("CreatedBy");
            this.Property(t => t.ModifyDate).HasColumnName("ModifyDate");
            this.Property(t => t.ModifyBy).HasColumnName("ModifyBy");
            this.Property(t => t.DeviceType).HasColumnName("DeviceType");
            this.Property(t => t.DeviceID).HasColumnName("DeviceID");
            this.Property(t => t.IsActive).HasColumnName("IsActive");
            this.Property(t => t.ImageName).HasColumnName("ImageName");
            this.Property(t => t.Remarks).HasColumnName("Remarks");

            // Relationships
            this.HasRequired(t => t.Category)
                .WithMany(t => t.ShopMenuPriorities)
                .HasForeignKey(d => d.CategoryID);
            this.HasRequired(t => t.Shop)
                .WithMany(t => t.ShopMenuPriorities)
                .HasForeignKey(d => d.ShopID);

        }
    }
}
