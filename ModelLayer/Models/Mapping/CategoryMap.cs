using System.ComponentModel.DataAnnotations;
using System.Data.Entity.ModelConfiguration;

namespace ModelLayer.Models.Mapping
{
    public class CategoryMap : EntityTypeConfiguration<Category>
    {
        public CategoryMap()
        {
            // Primary Key
            this.HasKey(t => t.ID);

            // Properties
            this.Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.Description)
                .IsRequired()
                .HasMaxLength(500);

            this.Property(t => t.SearchKeyword)
                .IsRequired();

            this.Property(t => t.NetworkIP)
                .HasMaxLength(15);

            this.Property(t => t.DeviceType)
                .HasMaxLength(50);

            this.Property(t => t.DeviceID)
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("Category");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.Name).HasColumnName("Name");
            this.Property(t => t.ParentCategoryID).HasColumnName("ParentCategoryID");
            this.Property(t => t.Level).HasColumnName("Level");
            this.Property(t => t.Description).HasColumnName("Description");
            this.Property(t => t.SearchKeyword).HasColumnName("SearchKeyword");
            this.Property(t => t.IsActive).HasColumnName("IsActive");
            this.Property(t => t.CreateDate).HasColumnName("CreateDate");
            this.Property(t => t.CreateBy).HasColumnName("CreateBy");
            this.Property(t => t.ModifyDate).HasColumnName("ModifyDate");
            this.Property(t => t.ModifyBy).HasColumnName("ModifyBy");
            this.Property(t => t.NetworkIP).HasColumnName("NetworkIP");
            this.Property(t => t.DeviceType).HasColumnName("DeviceType");
            this.Property(t => t.DeviceID).HasColumnName("DeviceID");
            /*======== Added by Tejaswee for Setting expiration date to some special Categories like Festival category ========*/
            this.Property(t => t.IsExpire).HasColumnName("IsExpire");
            this.Property(t => t.ExpiryDate).HasColumnName("ExpiryDate");
            /*======== Added by Tejaswee for Setting expiration date to some special Categories like Festival category ========*/
            
            // Relationships
            this.HasRequired(t => t.PersonalDetail)
                .WithMany(t => t.Categories)
                .HasForeignKey(d => d.CreateBy);
            this.HasOptional(t => t.PersonalDetail1)
                .WithMany(t => t.Categories1)
                .HasForeignKey(d => d.ModifyBy);
            this.HasOptional(t => t.Category2)
                .WithMany(t => t.Category1)
                .HasForeignKey(d => d.ParentCategoryID);

        }
    }
}
