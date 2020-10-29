using System.ComponentModel.DataAnnotations;
using System.Data.Entity.ModelConfiguration;

namespace ModelLayer.Models.Mapping
{
    public class OwnerAdvertisementMap : EntityTypeConfiguration<OwnerAdvertisement>
    {
        public OwnerAdvertisementMap()
        {
            // Primary Key
            this.HasKey(t => t.ID);

            // Properties
            this.Property(t => t.AdvertisementTitle)
                .IsRequired()
                .HasMaxLength(150);

            this.Property(t => t.NavigationUrl)
                .IsRequired()
                .HasMaxLength(300);

            this.Property(t => t.NetworkIP)
                .HasMaxLength(15);

            this.Property(t => t.DeviceType)
                .HasMaxLength(50);

            this.Property(t => t.DeviceID)
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("OwnerAdvertisement");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.AdvertisementID).HasColumnName("AdvertisementID");
            this.Property(t => t.AdvertisementTitle).HasColumnName("AdvertisementTitle");
            this.Property(t => t.NavigationUrl).HasColumnName("NavigationUrl");
            this.Property(t => t.NoOfDays).HasColumnName("NoOfDays");
            this.Property(t => t.NoOfHours).HasColumnName("NoOfHours");
            this.Property(t => t.BusinessTypeID).HasColumnName("BusinessTypeID");
            this.Property(t => t.OwnerID).HasColumnName("OwnerID");
            this.Property(t => t.FeesInRupee).HasColumnName("FeesInRupee");
            this.Property(t => t.PriorityLevel).HasColumnName("PriorityLevel");
            this.Property(t => t.IsLive).HasColumnName("IsLive");
            this.Property(t => t.IsActive).HasColumnName("IsActive");
            this.Property(t => t.CreateDate).HasColumnName("CreateDate");
            this.Property(t => t.CreateBy).HasColumnName("CreateBy");
            this.Property(t => t.ModifyDate).HasColumnName("ModifyDate");
            this.Property(t => t.ModifyBy).HasColumnName("ModifyBy");
            this.Property(t => t.NetworkIP).HasColumnName("NetworkIP");
            this.Property(t => t.DeviceType).HasColumnName("DeviceType");
            this.Property(t => t.DeviceID).HasColumnName("DeviceID");

            // Relationships
            this.HasRequired(t => t.Advertisement)
                .WithMany(t => t.OwnerAdvertisements)
                .HasForeignKey(d => d.AdvertisementID);
            this.HasRequired(t => t.BusinessType)
                .WithMany(t => t.OwnerAdvertisements)
                .HasForeignKey(d => d.BusinessTypeID);
            this.HasRequired(t => t.PersonalDetail)
                .WithMany(t => t.OwnerAdvertisements)
                .HasForeignKey(d => d.CreateBy);
            this.HasOptional(t => t.PersonalDetail1)
                .WithMany(t => t.OwnerAdvertisements1)
                .HasForeignKey(d => d.ModifyBy);

        }
    }
}
