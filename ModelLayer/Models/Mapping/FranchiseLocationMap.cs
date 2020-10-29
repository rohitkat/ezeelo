using System.ComponentModel.DataAnnotations;
using System.Data.Entity.ModelConfiguration;

namespace ModelLayer.Models.Mapping
{
    public class FranchiseLocationMap : EntityTypeConfiguration<FranchiseLocation>
    {
        public FranchiseLocationMap()
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
            this.ToTable("FranchiseLocation");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.FranchiseID).HasColumnName("FranchiseID");
            this.Property(t => t.AreaID).HasColumnName("AreaID");
            this.Property(t => t.IsActive).HasColumnName("IsActive");
            this.Property(t => t.CreateDate).HasColumnName("CreateDate");
            this.Property(t => t.CreateBy).HasColumnName("CreateBy");
            this.Property(t => t.ModifyDate).HasColumnName("ModifyDate");
            this.Property(t => t.ModifyBy).HasColumnName("ModifyBy");
            this.Property(t => t.NetworkIP).HasColumnName("NetworkIP");
            this.Property(t => t.DeviceType).HasColumnName("DeviceType");
            this.Property(t => t.DeviceID).HasColumnName("DeviceID");

            // Relationships
            this.HasOptional(t => t.Area)
                .WithMany(t => t.FranchiseLocations)
                .HasForeignKey(d => d.AreaID);
            this.HasOptional(t => t.Franchise)
                .WithMany(t => t.FranchiseLocations)
                .HasForeignKey(d => d.FranchiseID);
            this.HasRequired(t => t.PersonalDetail)
                .WithMany(t => t.FranchiseLocations)
                .HasForeignKey(d => d.CreateBy);
            this.HasOptional(t => t.PersonalDetail1)
                .WithMany(t => t.FranchiseLocations1)
                .HasForeignKey(d => d.ModifyBy);

        }
    }
}
