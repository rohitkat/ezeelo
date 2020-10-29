using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace ModelLayer.Models.Mapping
{
    public class FranchiseOrderGMVTargetMap : EntityTypeConfiguration<FranchiseOrderGMVTarget>
    {
        public FranchiseOrderGMVTargetMap()
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
            this.ToTable("FranchiseOrderGMVTarget");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.FranchiseID).HasColumnName("FranchiseID");
            this.Property(t => t.CityID).HasColumnName("CityID");
            this.Property(t => t.MonthlyOrderTarget).HasColumnName("MonthlyOrderTarget");
            this.Property(t => t.MonthlyGMVTarget).HasColumnName("MonthlyGMVTarget");
            this.Property(t => t.ForYear).HasColumnName("ForYear");
            this.Property(t => t.FromMonth).HasColumnName("FromMonth");
            this.Property(t => t.ToMonth).HasColumnName("ToMonth");
            this.Property(t => t.IsActive).HasColumnName("IsActive");
            this.Property(t => t.CreateDate).HasColumnName("CreateDate");
            this.Property(t => t.CreateBy).HasColumnName("CreateBy");
            this.Property(t => t.ModifyDate).HasColumnName("ModifyDate");
            this.Property(t => t.ModifyBy).HasColumnName("ModifyBy");
            this.Property(t => t.NetworkIP).HasColumnName("NetworkIP");
            this.Property(t => t.DeviceType).HasColumnName("DeviceType");
            this.Property(t => t.DeviceID).HasColumnName("DeviceID");

            // Relationships
            this.HasRequired(t => t.City)
                .WithMany(t => t.FranchiseOrderGMVTargets)
                .HasForeignKey(d => d.CityID);
            this.HasRequired(t => t.Franchise)
                .WithMany(t => t.FranchiseOrderGMVTargets)
                .HasForeignKey(d => d.FranchiseID);

        }
    }
}
