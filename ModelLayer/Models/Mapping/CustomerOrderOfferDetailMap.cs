using System.ComponentModel.DataAnnotations;
using System.Data.Entity.ModelConfiguration;

namespace ModelLayer.Models.Mapping
{
    public class CustomerOrderOfferDetailMap : EntityTypeConfiguration<CustomerOrderOfferDetail>
    {
        public CustomerOrderOfferDetailMap()
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
            this.ToTable("CustomerOrderOfferDetail");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.CustomerOrderDetailId).HasColumnName("CustomerOrderDetailId");
            this.Property(t => t.FreeShopStockId).HasColumnName("FreeShopStockId");
            this.Property(t => t.FreeQty).HasColumnName("FreeQty");
            this.Property(t => t.CreateDate).HasColumnName("CreateDate");
            this.Property(t => t.CreateBy).HasColumnName("CreateBy");
            this.Property(t => t.ModifyDate).HasColumnName("ModifyDate");
            this.Property(t => t.ModifyBy).HasColumnName("ModifyBy");
            this.Property(t => t.NetworkIP).HasColumnName("NetworkIP");
            this.Property(t => t.DeviceType).HasColumnName("DeviceType");
            this.Property(t => t.DeviceID).HasColumnName("DeviceID");

            // Relationships
            this.HasRequired(t => t.CustomerOrderDetail)
                .WithMany(t => t.CustomerOrderOfferDetails)
                .HasForeignKey(d => d.CustomerOrderDetailId);
            this.HasRequired(t => t.PersonalDetail)
                .WithMany(t => t.CustomerOrderOfferDetails)
                .HasForeignKey(d => d.CreateBy);
            this.HasRequired(t => t.ShopStock)
                .WithMany(t => t.CustomerOrderOfferDetails)
                .HasForeignKey(d => d.FreeShopStockId);
            this.HasOptional(t => t.PersonalDetail1)
                .WithMany(t => t.CustomerOrderOfferDetails1)
                .HasForeignKey(d => d.ModifyBy);

        }
    }
}
