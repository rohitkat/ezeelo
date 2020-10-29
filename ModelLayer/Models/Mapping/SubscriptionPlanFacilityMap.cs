using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace ModelLayer.Models.Mapping
{
    public class SubscriptionPlanFacilityMap : EntityTypeConfiguration<SubscriptionPlanFacility>
    {
        public SubscriptionPlanFacilityMap()
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
            this.ToTable("SubscriptionPlanFacility");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.SubscriptionPlanID).HasColumnName("SubscriptionPlanID");
            this.Property(t => t.SubscriptionFacilityID).HasColumnName("SubscriptionFacilityID");
            this.Property(t => t.FacilityValue).HasColumnName("FacilityValue");
            this.Property(t => t.IsActive).HasColumnName("IsActive");
            this.Property(t => t.CreateDate).HasColumnName("CreateDate");
            this.Property(t => t.CreateBy).HasColumnName("CreateBy");
            this.Property(t => t.ModifyDate).HasColumnName("ModifyDate");
            this.Property(t => t.ModifyBy).HasColumnName("ModifyBy");
            this.Property(t => t.NetworkIP).HasColumnName("NetworkIP");
            this.Property(t => t.DeviceType).HasColumnName("DeviceType");
            this.Property(t => t.DeviceID).HasColumnName("DeviceID");

            // Relationships
            this.HasRequired(t => t.SubscriptionFacility)
                .WithMany(t => t.SubscriptionPlanFacilities)
                .HasForeignKey(d => d.SubscriptionFacilityID);
            this.HasRequired(t => t.SubscriptionPlan)
                .WithMany(t => t.SubscriptionPlanFacilities)
                .HasForeignKey(d => d.SubscriptionPlanID);

        }
    }
}
