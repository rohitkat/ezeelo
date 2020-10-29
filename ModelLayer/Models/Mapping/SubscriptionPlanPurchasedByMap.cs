using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace ModelLayer.Models.Mapping
{
    public class SubscriptionPlanPurchasedByMap : EntityTypeConfiguration<SubscriptionPlanPurchasedBy>
    {
        public SubscriptionPlanPurchasedByMap()
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
            this.ToTable("SubscriptionPlanPurchasedBy");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.UserLoginID).HasColumnName("UserLoginID");
            this.Property(t => t.SailedByEmployeeID).HasColumnName("SailedByEmployeeID");
            this.Property(t => t.SubscriptionPlanID).HasColumnName("SubscriptionPlanID");
            this.Property(t => t.StartDate).HasColumnName("StartDate");
            this.Property(t => t.EndDate).HasColumnName("EndDate");
            this.Property(t => t.IsActive).HasColumnName("IsActive");
            this.Property(t => t.CreateDate).HasColumnName("CreateDate");
            this.Property(t => t.CreateBy).HasColumnName("CreateBy");
            this.Property(t => t.ModifyDate).HasColumnName("ModifyDate");
            this.Property(t => t.ModifyBy).HasColumnName("ModifyBy");
            this.Property(t => t.NetworkIP).HasColumnName("NetworkIP");
            this.Property(t => t.DeviceType).HasColumnName("DeviceType");
            this.Property(t => t.DeviceID).HasColumnName("DeviceID");

            // Relationships
            this.HasRequired(t => t.SubscriptionPlan)
                .WithMany(t => t.SubscriptionPlanPurchasedBies)
                .HasForeignKey(d => d.SubscriptionPlanID);

        }
    }
}
