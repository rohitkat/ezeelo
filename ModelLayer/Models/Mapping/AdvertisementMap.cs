using System.ComponentModel.DataAnnotations;
using System.Data.Entity.ModelConfiguration;

namespace ModelLayer.Models.Mapping
{
    public class AdvertisementMap : EntityTypeConfiguration<Advertisement>
    {
        public AdvertisementMap()
        {
            // Primary Key
            this.HasKey(t => t.ID);

            // Properties
            this.Property(t => t.PageName)
                .HasMaxLength(50);

            this.Property(t => t.Alignment)
                .HasMaxLength(10);

            this.Property(t => t.Description)
                .HasMaxLength(150);

            this.Property(t => t.NetworkIP)
                .HasMaxLength(15);

            this.Property(t => t.DeviceType)
                .HasMaxLength(50);

            this.Property(t => t.DeviceID)
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("Advertisement");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.PageName).HasColumnName("PageName");
            this.Property(t => t.WidthInPixel).HasColumnName("WidthInPixel");
            this.Property(t => t.HeightInPixel).HasColumnName("HeightInPixel");
            this.Property(t => t.Alignment).HasColumnName("Alignment");
            this.Property(t => t.NumberOfDays).HasColumnName("NumberOfDays");
            this.Property(t => t.FeesInRs).HasColumnName("FeesInRs");
            this.Property(t => t.Description).HasColumnName("Description");
            this.Property(t => t.IsActive).HasColumnName("IsActive");
            this.Property(t => t.CreateDate).HasColumnName("CreateDate");
            this.Property(t => t.CreateBy).HasColumnName("CreateBy");
            this.Property(t => t.ModifyDate).HasColumnName("ModifyDate");
            this.Property(t => t.ModifyBy).HasColumnName("ModifyBy");
            this.Property(t => t.NetworkIP).HasColumnName("NetworkIP");
            this.Property(t => t.DeviceType).HasColumnName("DeviceType");
            this.Property(t => t.DeviceID).HasColumnName("DeviceID");

            // Relationships
            this.HasRequired(t => t.PersonalDetail)
                .WithMany(t => t.Advertisements)
                .HasForeignKey(d => d.CreateBy);
            this.HasOptional(t => t.PersonalDetail1)
                .WithMany(t => t.Advertisements1)
                .HasForeignKey(d => d.ModifyBy);

        }
    }
}
