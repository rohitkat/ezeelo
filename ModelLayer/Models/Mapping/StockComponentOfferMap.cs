using System.ComponentModel.DataAnnotations;
using System.Data.Entity.ModelConfiguration;

namespace ModelLayer.Models.Mapping
{
    public class StockComponentOfferMap : EntityTypeConfiguration<StockComponentOffer>
    {
        public StockComponentOfferMap()
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
            this.ToTable("StockComponentOffer");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.ComponentOfferID).HasColumnName("ComponentOfferID");
            this.Property(t => t.ShopStockID).HasColumnName("ShopStockID");
            this.Property(t => t.IsActive).HasColumnName("IsActive");
            this.Property(t => t.CreateDate).HasColumnName("CreateDate");
            this.Property(t => t.CreateBy).HasColumnName("CreateBy");
            this.Property(t => t.ModifyDate).HasColumnName("ModifyDate");
            this.Property(t => t.ModifyBy).HasColumnName("ModifyBy");
            this.Property(t => t.NetworkIP).HasColumnName("NetworkIP");
            this.Property(t => t.DeviceType).HasColumnName("DeviceType");
            this.Property(t => t.DeviceID).HasColumnName("DeviceID");

            // Relationships
            this.HasRequired(t => t.ComponentOffer)
                .WithMany(t => t.StockComponentOffers)
                .HasForeignKey(d => d.ComponentOfferID);
            this.HasRequired(t => t.PersonalDetail)
                .WithMany(t => t.StockComponentOffers)
                .HasForeignKey(d => d.CreateBy);
            this.HasOptional(t => t.PersonalDetail1)
                .WithMany(t => t.StockComponentOffers1)
                .HasForeignKey(d => d.ModifyBy);
            this.HasRequired(t => t.ShopStock)
                .WithMany(t => t.StockComponentOffers)
                .HasForeignKey(d => d.ShopStockID);

        }
    }
}
