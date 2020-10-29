using System.ComponentModel.DataAnnotations;
using System.Data.Entity.ModelConfiguration;

namespace ModelLayer.Models.Mapping
{
    public class ShopProductMap : EntityTypeConfiguration<ShopProduct>
    {
        public ShopProductMap()
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
            this.ToTable("ShopProduct");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.ShopID).HasColumnName("ShopID");
            this.Property(t => t.ProductID).HasColumnName("ProductID");
            this.Property(t => t.IsActive).HasColumnName("IsActive");
            this.Property(t => t.DisplayProductFromDate).HasColumnName("DisplayProductFromDate");
            this.Property(t => t.CreateDate).HasColumnName("CreateDate");
            this.Property(t => t.CreateBy).HasColumnName("CreateBy");
            this.Property(t => t.ModifyDate).HasColumnName("ModifyDate");
            this.Property(t => t.ModifyBy).HasColumnName("ModifyBy");
            this.Property(t => t.NetworkIP).HasColumnName("NetworkIP");
            this.Property(t => t.DeviceType).HasColumnName("DeviceType");
            this.Property(t => t.DeviceID).HasColumnName("DeviceID");

            // Relationships
            this.HasRequired(t => t.PersonalDetail)
                .WithMany(t => t.ShopProducts)
                .HasForeignKey(d => d.CreateBy);
            this.HasOptional(t => t.PersonalDetail1)
                .WithMany(t => t.ShopProducts1)
                .HasForeignKey(d => d.ModifyBy);
            this.HasRequired(t => t.Product)
                .WithMany(t => t.ShopProducts)
                .HasForeignKey(d => d.ProductID);
            this.HasRequired(t => t.Shop)
                .WithMany(t => t.ShopProducts)
                .HasForeignKey(d => d.ShopID);

        }
    }
}
