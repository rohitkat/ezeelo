using System.ComponentModel.DataAnnotations;
using System.Data.Entity.ModelConfiguration;

namespace ModelLayer.Models.Mapping
{
    public class FranchiseCategoryMap : EntityTypeConfiguration<FranchiseCategory>
    {
        public FranchiseCategoryMap()
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
            this.ToTable("FranchiseCategory");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.FranchiseLocationID).HasColumnName("FranchiseLocationID");
            this.Property(t => t.CategoryID).HasColumnName("CategoryID");
            this.Property(t => t.ParentCategoryId).HasColumnName("ParentCategoryId");
            this.Property(t => t.Level).HasColumnName("Level");
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
                .WithMany(t => t.FranchiseCategories)
                .HasForeignKey(d => d.CategoryID);
            this.HasOptional(t => t.Category1)
                .WithMany(t => t.FranchiseCategories1)
                .HasForeignKey(d => d.ParentCategoryId);
            this.HasRequired(t => t.PersonalDetail)
                .WithMany(t => t.FranchiseCategories)
                .HasForeignKey(d => d.CreateBy);
            this.HasRequired(t => t.FranchiseLocation)
                .WithMany(t => t.FranchiseCategories)
                .HasForeignKey(d => d.FranchiseLocationID);
            this.HasOptional(t => t.PersonalDetail1)
                .WithMany(t => t.FranchiseCategories1)
                .HasForeignKey(d => d.ModifyBy);

        }
    }
}
