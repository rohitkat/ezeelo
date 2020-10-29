using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace ModelLayer.Models.Mapping
{
    public class SubscriptionPlanDealWithCategoryLogMap : EntityTypeConfiguration<SubscriptionPlanDealWithCategoryLog>
    {
        public SubscriptionPlanDealWithCategoryLogMap()
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
            this.ToTable("SubscriptionPlanDealWithCategoryLog");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.UserLoginID).HasColumnName("UserLoginID");
            this.Property(t => t.SubscriptionPlanDealWithCategoryID).HasColumnName("SubscriptionPlanDealWithCategoryID");
            this.Property(t => t.PurchaseAmount).HasColumnName("PurchaseAmount");
            this.Property(t => t.IsActive).HasColumnName("IsActive");
            this.Property(t => t.CreateDate).HasColumnName("CreateDate");
            this.Property(t => t.CreateBy).HasColumnName("CreateBy");
            this.Property(t => t.ModifyDate).HasColumnName("ModifyDate");
            this.Property(t => t.ModifyBy).HasColumnName("ModifyBy");
            this.Property(t => t.NetworkIP).HasColumnName("NetworkIP");
            this.Property(t => t.DeviceType).HasColumnName("DeviceType");
            this.Property(t => t.DeviceID).HasColumnName("DeviceID");

            // Relationships
            this.HasRequired(t => t.SubscriptionPlanDealWithCategory)
                .WithMany(t => t.SubscriptionPlanDealWithCategoryLogs)
                .HasForeignKey(d => d.SubscriptionPlanDealWithCategoryID);

        }
    }
}
