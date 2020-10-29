using System.ComponentModel.DataAnnotations;
using System.Data.Entity.ModelConfiguration;

namespace ModelLayer.Models.Mapping
{
    public class FranchiseMenuMap : EntityTypeConfiguration<FranchiseMenu>
    {
        public FranchiseMenuMap()
        {
            // Primary Key
            this.HasKey(t => t.ID);

            // Properties
            this.Property(t => t.CategoryName)
                .HasMaxLength(150);

            this.Property(t => t.DeviceType)
                .HasMaxLength(50);

            this.Property(t => t.DeviceID)
                .HasMaxLength(50);

            this.Property(t => t.ImageName)
                .HasMaxLength(100);

            this.Property(t => t.Remarks)
                .HasMaxLength(200);

            // Table & Column Mappings
            this.ToTable("FranchiseMenu");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.FranchiseID).HasColumnName("FranchiseID");
            this.Property(t => t.CategoryName).HasColumnName("CategoryName");
            this.Property(t => t.CategoryID).HasColumnName("CategoryID");
            this.Property(t => t.Level).HasColumnName("Level");
            this.Property(t => t.SequenceOrder).HasColumnName("SequenceOrder");
            this.Property(t => t.CreateDate).HasColumnName("CreateDate");
            this.Property(t => t.CreatedBy).HasColumnName("CreatedBy");
            this.Property(t => t.ModifyDate).HasColumnName("ModifyDate");
            this.Property(t => t.ModifyBy).HasColumnName("ModifyBy");
            this.Property(t => t.DeviceType).HasColumnName("DeviceType");
            this.Property(t => t.DeviceID).HasColumnName("DeviceID");
            this.Property(t => t.IsActive).HasColumnName("IsActive");
            this.Property(t => t.ImageName).HasColumnName("ImageName");
            this.Property(t => t.Remarks).HasColumnName("Remarks");
            /*======== Added by Tejaswee for Setting expiration date to some special Categories like Festival category ========*/
            this.Property(t => t.IsExpire).HasColumnName("IsExpire");
            this.Property(t => t.ExpiryDate).HasColumnName("ExpiryDate");
            /*======== Added by Tejaswee for Setting expiration date to some special Categories like Festival category ========*/

            // Relationships
            this.HasRequired(t => t.Category)
                .WithMany(t => t.FranchiseMenus)
                .HasForeignKey(d => d.CategoryID);
            this.HasRequired(t => t.Franchise)
                .WithMany(t => t.FranchiseMenus)
                .HasForeignKey(d => d.FranchiseID);

        }
    }
}
