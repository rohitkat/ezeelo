using System.ComponentModel.DataAnnotations;
using System.Data.Entity.ModelConfiguration;

namespace ModelLayer.Models.Mapping
{
    public class ShopMarketMap : EntityTypeConfiguration<ShopMarket>
    {
        public ShopMarketMap()
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
            this.ToTable("ShopMarket");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.ShopID).HasColumnName("ShopID");
            this.Property(t => t.CategoryID).HasColumnName("CategoryID");
            this.Property(t => t.ParentCategoryID).HasColumnName("ParentCategoryID");
            this.Property(t => t.ShopCategoryLevel).HasColumnName("ShopCategoryLevel");
            this.Property(t => t.IsPrimary).HasColumnName("IsPrimary");
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
                .WithMany(t => t.ShopMarkets)
                .HasForeignKey(d => d.CategoryID);
            this.HasOptional(t => t.Category1)
                .WithMany(t => t.ShopMarkets1)
                .HasForeignKey(d => d.ParentCategoryID);
            this.HasRequired(t => t.PersonalDetail)
                .WithMany(t => t.ShopMarkets)
                .HasForeignKey(d => d.CreateBy);
            this.HasOptional(t => t.PersonalDetail1)
                .WithMany(t => t.ShopMarkets1)
                .HasForeignKey(d => d.ModifyBy);
            this.HasRequired(t => t.Shop)
                .WithMany(t => t.ShopMarkets)
                .HasForeignKey(d => d.ShopID);

        }
    }
}
