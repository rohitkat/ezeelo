using System.ComponentModel.DataAnnotations;
using System.Data.Entity.ModelConfiguration;

namespace ModelLayer.Models.Mapping
{
    public class PlanCategoryChargeMap : EntityTypeConfiguration<PlanCategoryCharge>
    {
        public PlanCategoryChargeMap()
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
            this.ToTable("PlanCategoryCharge");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.PlanID).HasColumnName("PlanID");
            this.Property(t => t.CategoryID).HasColumnName("CategoryID");
            this.Property(t => t.ChargeID).HasColumnName("ChargeID");
            this.Property(t => t.ChargeInPercent).HasColumnName("ChargeInPercent");
            this.Property(t => t.ChargeInRupee).HasColumnName("ChargeInRupee");
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
                .WithMany(t => t.PlanCategoryCharges)
                .HasForeignKey(d => d.CategoryID);
            this.HasRequired(t => t.Charge)
                .WithMany(t => t.PlanCategoryCharges)
                .HasForeignKey(d => d.ChargeID);
            this.HasRequired(t => t.PersonalDetail)
                .WithMany(t => t.PlanCategoryCharges)
                .HasForeignKey(d => d.CreateBy);
            this.HasOptional(t => t.PersonalDetail1)
                .WithMany(t => t.PlanCategoryCharges1)
                .HasForeignKey(d => d.ModifyBy);
            this.HasRequired(t => t.Plan)
                .WithMany(t => t.PlanCategoryCharges)
                .HasForeignKey(d => d.PlanID);

        }
    }
}
