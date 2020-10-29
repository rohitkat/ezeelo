using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace ModelLayer.Models.Mapping
{
    public class SubscriptionPlanDealWithCategoryMap : EntityTypeConfiguration<SubscriptionPlanDealWithCategory>
    {
        public SubscriptionPlanDealWithCategoryMap()
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
            this.ToTable("SubscriptionPlanDealWithCategory");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.SubscriptionPlanID).HasColumnName("SubscriptionPlanID");
            this.Property(t => t.CategoryID).HasColumnName("CategoryID");
            this.Property(t => t.MinimumAmount).HasColumnName("MinimumAmount");
            this.Property(t => t.DiscountInRs).HasColumnName("DiscountInRs");
            this.Property(t => t.DiscountInPer).HasColumnName("DiscountInPer");
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
                .WithMany(t => t.SubscriptionPlanDealWithCategories)
                .HasForeignKey(d => d.SubscriptionPlanID);

        }
    }
}
