using System.ComponentModel.DataAnnotations;
using System.Data.Entity.ModelConfiguration;

namespace ModelLayer.Models.Mapping
{
    public class CorporateCustomerShippingDeliveryDetailMap : EntityTypeConfiguration<CorporateCustomerShippingDeliveryDetail>
    {
        public CorporateCustomerShippingDeliveryDetailMap()
        {
            // Primary Key
            this.HasKey(t => t.ID);

            // Properties
            this.Property(t => t.ToName)
                .IsRequired()
                .HasMaxLength(150);

            this.Property(t => t.PrimaryMobile)
                .IsRequired()
                .HasMaxLength(10);

            this.Property(t => t.SecondaryMobile)
                .HasMaxLength(10);

            this.Property(t => t.ShippingAddress)
                .IsRequired()
                .HasMaxLength(150);

            this.Property(t => t.NetworkIP)
                .HasMaxLength(15);

            this.Property(t => t.DeviceType)
                .HasMaxLength(50);

            this.Property(t => t.DeviceID)
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("CorporateCustomerShippingDeliveryDetails");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.CustomerOrderDetailID).HasColumnName("CustomerOrderDetailID");
            this.Property(t => t.FromUserLoginID).HasColumnName("FromUserLoginID");
            this.Property(t => t.ToName).HasColumnName("ToName");
            this.Property(t => t.DeliveryCharges).HasColumnName("DeliveryCharges");
            this.Property(t => t.ExpectedDeliveryDate).HasColumnName("ExpectedDeliveryDate");
            this.Property(t => t.Quantity).HasColumnName("Quantity");
            this.Property(t => t.PrimaryMobile).HasColumnName("PrimaryMobile");
            this.Property(t => t.SecondaryMobile).HasColumnName("SecondaryMobile");
            this.Property(t => t.ShippingAddress).HasColumnName("ShippingAddress");
            this.Property(t => t.PincodeID).HasColumnName("PincodeID");
            this.Property(t => t.AreaID).HasColumnName("AreaID");
            this.Property(t => t.IsActive).HasColumnName("IsActive");
            this.Property(t => t.CreateDate).HasColumnName("CreateDate");
            this.Property(t => t.CreateBy).HasColumnName("CreateBy");
            this.Property(t => t.ModifyDate).HasColumnName("ModifyDate");
            this.Property(t => t.ModifyBy).HasColumnName("ModifyBy");
            this.Property(t => t.NetworkIP).HasColumnName("NetworkIP");
            this.Property(t => t.DeviceType).HasColumnName("DeviceType");
            this.Property(t => t.DeviceID).HasColumnName("DeviceID");

            // Relationships
            this.HasRequired(t => t.CustomerOrderDetail)
                .WithMany(t => t.CorporateCustomerShippingDeliveryDetails)
                .HasForeignKey(d => d.CustomerOrderDetailID);

        }
    }
}
