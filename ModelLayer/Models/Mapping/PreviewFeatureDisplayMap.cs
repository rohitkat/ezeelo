using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace ModelLayer.Models.Mapping
{
    public class PreviewFeatureDisplayMap : EntityTypeConfiguration<PreviewFeatureDisplay>
    {
        public PreviewFeatureDisplayMap()
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
            this.ToTable("PreviewFeatureDisplay");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.ThirdLevelCatID).HasColumnName("ThirdLevelCatID");
            this.Property(t => t.PreviewFeatureID).HasColumnName("PreviewFeatureID");
            this.Property(t => t.IsActive).HasColumnName("IsActive");
            this.Property(t => t.CreateDate).HasColumnName("CreateDate");
            this.Property(t => t.CreateBy).HasColumnName("CreateBy");
            this.Property(t => t.ModifyDate).HasColumnName("ModifyDate");
            this.Property(t => t.ModifyBy).HasColumnName("ModifyBy");
            this.Property(t => t.NetworkIP).HasColumnName("NetworkIP");
            this.Property(t => t.DeviceType).HasColumnName("DeviceType");
            this.Property(t => t.DeviceID).HasColumnName("DeviceID");

            // Relationships
            this.HasRequired(t => t.Category)
                .WithMany(t => t.PreviewFeatureDisplays)
                .HasForeignKey(d => d.ThirdLevelCatID);
            this.HasRequired(t => t.PreviewFeature)
                .WithMany(t => t.PreviewFeatureDisplays)
                .HasForeignKey(d => d.PreviewFeatureID);

        }
    }
}
