using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace ModelLayer.Models.Mapping
{
    public class DesignBlockTypeMap : EntityTypeConfiguration<DesignBlockType>
    {
        public DesignBlockTypeMap()
        {
            // Primary Key
            this.HasKey(t => t.ID);

            // Properties
            this.Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.ImageExtension)
                .HasMaxLength(5);

            this.Property(t => t.DeviceType)
                .HasMaxLength(50);

            this.Property(t => t.DeviceID)
                .HasMaxLength(50);

            this.Property(t => t.NetworkIP)
                .HasMaxLength(15);

            this.Property(t => t.Remarks)
                .HasMaxLength(200);

            // Table & Column Mappings
            this.ToTable("DesignBlockType");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.Name).HasColumnName("Name");
            this.Property(t => t.ImageExtension).HasColumnName("ImageExtension");
            this.Property(t => t.ImageWidth).HasColumnName("ImageWidth");
            this.Property(t => t.ImageHeight).HasColumnName("ImageHeight");
            this.Property(t => t.MinLimit).HasColumnName("MinLimit");
            this.Property(t => t.MaxLimit).HasColumnName("MaxLimit");
            this.Property(t => t.IsActive).HasColumnName("IsActive");
            this.Property(t => t.CreateDate).HasColumnName("CreateDate");
            this.Property(t => t.CreatedBy).HasColumnName("CreatedBy");
            this.Property(t => t.ModifyDate).HasColumnName("ModifyDate");
            this.Property(t => t.ModifyBy).HasColumnName("ModifyBy");
            this.Property(t => t.DeviceType).HasColumnName("DeviceType");
            this.Property(t => t.DeviceID).HasColumnName("DeviceID");
            this.Property(t => t.NetworkIP).HasColumnName("NetworkIP");
            this.Property(t => t.Remarks).HasColumnName("Remarks");
        }
    }
}
