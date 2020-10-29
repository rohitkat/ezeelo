using System.ComponentModel.DataAnnotations;
using System.Data.Entity.ModelConfiguration;

namespace ModelLayer.Models.Mapping
{
    public class ProductVarientMap : EntityTypeConfiguration<ProductVarient>
    {
        public ProductVarientMap()
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
            this.ToTable("ProductVarient");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.ProductID).HasColumnName("ProductID");
            this.Property(t => t.ColorID).HasColumnName("ColorID");
            this.Property(t => t.SizeID).HasColumnName("SizeID");
            this.Property(t => t.DimensionID).HasColumnName("DimensionID");
            this.Property(t => t.MaterialID).HasColumnName("MaterialID");
            this.Property(t => t.IsActive).HasColumnName("IsActive");
            this.Property(t => t.CreateDate).HasColumnName("CreateDate");
            this.Property(t => t.CreateBy).HasColumnName("CreateBy");
            this.Property(t => t.ModifyDate).HasColumnName("ModifyDate");
            this.Property(t => t.ModifyBy).HasColumnName("ModifyBy");
            this.Property(t => t.NetworkIP).HasColumnName("NetworkIP");
            this.Property(t => t.DeviceType).HasColumnName("DeviceType");
            this.Property(t => t.DeviceID).HasColumnName("DeviceID");

            // Relationships
            this.HasRequired(t => t.Color)
                .WithMany(t => t.ProductVarients)
                .HasForeignKey(d => d.ColorID);
            this.HasRequired(t => t.Dimension)
                .WithMany(t => t.ProductVarients)
                .HasForeignKey(d => d.DimensionID);
            this.HasRequired(t => t.Material)
                .WithMany(t => t.ProductVarients)
                .HasForeignKey(d => d.MaterialID);
            this.HasRequired(t => t.PersonalDetail)
                .WithMany(t => t.ProductVarients)
                .HasForeignKey(d => d.CreateBy);
            this.HasOptional(t => t.PersonalDetail1)
                .WithMany(t => t.ProductVarients1)
                .HasForeignKey(d => d.ModifyBy);
            this.HasRequired(t => t.Size)
                .WithMany(t => t.ProductVarients)
                .HasForeignKey(d => d.SizeID);

        }
    }
}
