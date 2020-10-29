using System.ComponentModel.DataAnnotations;
using System.Data.Entity.ModelConfiguration;

namespace ModelLayer.Models.Mapping
{
    public class ShopStockMap : EntityTypeConfiguration<ShopStock>
    {
        public ShopStockMap()
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
            this.ToTable("ShopStock");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.ShopProductID).HasColumnName("ShopProductID");
            this.Property(t => t.ProductVarientID).HasColumnName("ProductVarientID");
            this.Property(t => t.Qty).HasColumnName("Qty");
            this.Property(t => t.ReorderLevel).HasColumnName("ReorderLevel");
            this.Property(t => t.StockStatus).HasColumnName("StockStatus");
            this.Property(t => t.PackSize).HasColumnName("PackSize");
            this.Property(t => t.PackUnitID).HasColumnName("PackUnitID");
            this.Property(t => t.MRP).HasColumnName("MRP");
            this.Property(t => t.WholeSaleRate).HasColumnName("WholeSaleRate");
            this.Property(t => t.RetailerRate).HasColumnName("RetailerRate");
            this.Property(t => t.IsInclusiveOfTax).HasColumnName("IsInclusiveOfTax");
            this.Property(t => t.IsActive).HasColumnName("IsActive");
            this.Property(t => t.CreateDate).HasColumnName("CreateDate");
            this.Property(t => t.CreateBy).HasColumnName("CreateBy");
            this.Property(t => t.ModifyDate).HasColumnName("ModifyDate");
            this.Property(t => t.ModifyBy).HasColumnName("ModifyBy");
            this.Property(t => t.NetworkIP).HasColumnName("NetworkIP");
            this.Property(t => t.DeviceType).HasColumnName("DeviceType");
            this.Property(t => t.DeviceID).HasColumnName("DeviceID");

            // Relationships
            this.HasRequired(t => t.PersonalDetail)
                .WithMany(t => t.ShopStocks)
                .HasForeignKey(d => d.CreateBy);
            this.HasOptional(t => t.PersonalDetail1)
                .WithMany(t => t.ShopStocks1)
                .HasForeignKey(d => d.ModifyBy);
            this.HasRequired(t => t.ProductVarient)
                .WithMany(t => t.ShopStocks)
                .HasForeignKey(d => d.ProductVarientID);
            this.HasRequired(t => t.ShopProduct)
                .WithMany(t => t.ShopStocks)
                .HasForeignKey(d => d.ShopProductID);
            this.HasRequired(t => t.Unit)
                .WithMany(t => t.ShopStocks)
                .HasForeignKey(d => d.PackUnitID);

        }
    }
}
