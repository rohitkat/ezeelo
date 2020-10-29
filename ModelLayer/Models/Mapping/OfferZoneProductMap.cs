using System.ComponentModel.DataAnnotations;
using System.Data.Entity.ModelConfiguration;

namespace ModelLayer.Models.Mapping
{
    public class OfferZoneProductMap : EntityTypeConfiguration<OfferZoneProduct>
    {
        public OfferZoneProductMap()
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
            this.ToTable("OfferZoneProduct");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.OfferID).HasColumnName("OfferID");
            this.Property(t => t.ShopStockID).HasColumnName("ShopStockID");
            this.Property(t => t.FreeStockID).HasColumnName("FreeStockID");
            this.Property(t => t.IsActive).HasColumnName("IsActive");
            this.Property(t => t.CreateDate).HasColumnName("CreateDate");
            this.Property(t => t.CreateBy).HasColumnName("CreateBy");
            this.Property(t => t.ModifyDate).HasColumnName("ModifyDate");
            this.Property(t => t.ModifyBy).HasColumnName("ModifyBy");
            this.Property(t => t.NetworkIP).HasColumnName("NetworkIP");
            this.Property(t => t.DeviceType).HasColumnName("DeviceType");
            this.Property(t => t.DeviceID).HasColumnName("DeviceID");

            // Relationships
            this.HasRequired(t => t.Offer)
                .WithMany(t => t.OfferZoneProducts)
                .HasForeignKey(d => d.OfferID);
            this.HasRequired(t => t.PersonalDetail)
                .WithMany(t => t.OfferZoneProducts)
                .HasForeignKey(d => d.CreateBy);
            this.HasOptional(t => t.PersonalDetail1)
                .WithMany(t => t.OfferZoneProducts1)
                .HasForeignKey(d => d.ModifyBy);
            this.HasRequired(t => t.ShopStock)
                .WithMany(t => t.OfferZoneProducts)
                .HasForeignKey(d => d.FreeStockID);
            this.HasRequired(t => t.ShopStock1)
                .WithMany(t => t.OfferZoneProducts1)
                .HasForeignKey(d => d.ShopStockID);

        }
    }
}
