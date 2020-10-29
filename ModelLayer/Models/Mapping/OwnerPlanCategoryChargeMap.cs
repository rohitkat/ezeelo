using System.ComponentModel.DataAnnotations;
using System.Data.Entity.ModelConfiguration;

namespace ModelLayer.Models.Mapping
{
    public class OwnerPlanCategoryChargeMap : EntityTypeConfiguration<OwnerPlanCategoryCharge>
    {
        public OwnerPlanCategoryChargeMap()
        {
            // Primary Key
            this.HasKey(t => t.ID);

            // Properties
            this.Property(t => t.NetworkIP)
                .HasMaxLength(15);

            this.Property(t => t.DeviceType)
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("OwnerPlanCategoryCharge");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.OwnerPlanID).HasColumnName("OwnerPlanID");
            this.Property(t => t.CategoryID).HasColumnName("CategoryID");
            this.Property(t => t.ChargeInPercent).HasColumnName("ChargeInPercent");
            this.Property(t => t.ChargeInRupee).HasColumnName("ChargeInRupee");
            this.Property(t => t.IsActive).HasColumnName("IsActive");
            this.Property(t => t.CreateDate).HasColumnName("CreateDate");
            this.Property(t => t.CreateBy).HasColumnName("CreateBy");
            this.Property(t => t.ModifyDate).HasColumnName("ModifyDate");
            this.Property(t => t.ModifyBy).HasColumnName("ModifyBy");
            this.Property(t => t.NetworkIP).HasColumnName("NetworkIP");
            this.Property(t => t.DeviceType).HasColumnName("DeviceType");

            // Relationships
            this.HasRequired(t => t.Category)
                .WithMany(t => t.OwnerPlanCategoryCharges)
                .HasForeignKey(d => d.CategoryID);
            this.HasRequired(t => t.OwnerPlan)
                .WithMany(t => t.OwnerPlanCategoryCharges)
                .HasForeignKey(d => d.OwnerPlanID);
            this.HasRequired(t => t.PersonalDetail)
                .WithMany(t => t.OwnerPlanCategoryCharges)
                .HasForeignKey(d => d.CreateBy);
            this.HasOptional(t => t.PersonalDetail1)
                .WithMany(t => t.OwnerPlanCategoryCharges1)
                .HasForeignKey(d => d.ModifyBy);

        }
    }
}
