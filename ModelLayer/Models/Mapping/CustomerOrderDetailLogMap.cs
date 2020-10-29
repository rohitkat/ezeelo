using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace ModelLayer.Models.Mapping
{
    public class CustomerOrderDetailLogMap : EntityTypeConfiguration<CustomerOrderDetailLog>
    {
        public CustomerOrderDetailLogMap()
        {
            // Primary Key
            this.HasKey(t => t.ID);

            // Properties
            this.Property(t => t.ShopOrderCode)
                .IsRequired()
                .HasMaxLength(15);

            this.Property(t => t.ReferenceShopOrderCode)
                .HasMaxLength(15);

            this.Property(t => t.NetworkIP)
                .HasMaxLength(15);

            this.Property(t => t.DeviceType)
                .HasMaxLength(50);

            this.Property(t => t.DeviceID)
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("CustomerOrderDetailLog");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.ShopOrderCode).HasColumnName("ShopOrderCode");
            this.Property(t => t.ReferenceShopOrderCode).HasColumnName("ReferenceShopOrderCode");
            this.Property(t => t.CustomerOrderID).HasColumnName("CustomerOrderID");
            this.Property(t => t.ShopStockID).HasColumnName("ShopStockID");
            this.Property(t => t.ShopID).HasColumnName("ShopID");
            this.Property(t => t.Qty).HasColumnName("Qty");
            this.Property(t => t.OrderStatus).HasColumnName("OrderStatus");
            this.Property(t => t.MRP).HasColumnName("MRP");
            this.Property(t => t.SaleRate).HasColumnName("SaleRate");
            this.Property(t => t.OfferPercent).HasColumnName("OfferPercent");
            this.Property(t => t.OfferRs).HasColumnName("OfferRs");
            this.Property(t => t.IsInclusivOfTax).HasColumnName("IsInclusivOfTax");
            this.Property(t => t.TotalAmount).HasColumnName("TotalAmount");
            this.Property(t => t.IsActive).HasColumnName("IsActive");
            this.Property(t => t.CreateDate).HasColumnName("CreateDate");
            this.Property(t => t.CreateBy).HasColumnName("CreateBy");
            this.Property(t => t.ModifyDate).HasColumnName("ModifyDate");
            this.Property(t => t.ModifyBy).HasColumnName("ModifyBy");
            this.Property(t => t.NetworkIP).HasColumnName("NetworkIP");
            this.Property(t => t.DeviceType).HasColumnName("DeviceType");
            this.Property(t => t.DeviceID).HasColumnName("DeviceID");
        }
    }
}
