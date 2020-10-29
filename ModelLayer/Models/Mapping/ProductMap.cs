using System.ComponentModel.DataAnnotations;
using System.Data.Entity.ModelConfiguration;

namespace ModelLayer.Models.Mapping
{
    public class ProductMap : EntityTypeConfiguration<Product>
    {
        public ProductMap()
        {
            // Primary Key
            this.HasKey(t => t.ID);

            // Properties
            this.Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(300);

            this.Property(t => t.Description)
                .IsRequired()
                .HasMaxLength(5000);

            this.Property(t => t.SearchKeyword)
               .IsRequired();

            this.Property(t => t.NetworkIP)
                .HasMaxLength(15);

            this.Property(t => t.DeviceType)
                .HasMaxLength(50);

            this.Property(t => t.DeviceID)
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("Product");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.Name).HasColumnName("Name");
            this.Property(t => t.CategoryID).HasColumnName("CategoryID");
            this.Property(t => t.WeightInGram).HasColumnName("WeightInGram");
            this.Property(t => t.LengthInCm).HasColumnName("LengthInCm");
            this.Property(t => t.BreadthInCm).HasColumnName("BreadthInCm");
            this.Property(t => t.HeightInCm).HasColumnName("HeightInCm");
            this.Property(t => t.Description).HasColumnName("Description");
            this.Property(t => t.BrandID).HasColumnName("BrandID");
            this.Property(t => t.SearchKeyword).HasColumnName("SearchKeyword");
            this.Property(t => t.IsActive).HasColumnName("IsActive");
            this.Property(t => t.CreateDate).HasColumnName("CreateDate");
            this.Property(t => t.CreateBy).HasColumnName("CreateBy");
            this.Property(t => t.ModifyDate).HasColumnName("ModifyDate");
            this.Property(t => t.ModifyBy).HasColumnName("ModifyBy");
            this.Property(t => t.NetworkIP).HasColumnName("NetworkIP");
            this.Property(t => t.DeviceType).HasColumnName("DeviceType");
            this.Property(t => t.DeviceID).HasColumnName("DeviceID");

            // Relationships
            this.HasRequired(t => t.Brand)
                .WithMany(t => t.Products)
                .HasForeignKey(d => d.BrandID);
            this.HasRequired(t => t.Category)
                .WithMany(t => t.Products)
                .HasForeignKey(d => d.CategoryID);
            this.HasRequired(t => t.PersonalDetail)
                .WithMany(t => t.Products)
                .HasForeignKey(d => d.CreateBy);
            this.HasOptional(t => t.PersonalDetail1)
                .WithMany(t => t.Products1)
                .HasForeignKey(d => d.ModifyBy);

        }
    }
}
