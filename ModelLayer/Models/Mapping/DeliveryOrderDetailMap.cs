using System.ComponentModel.DataAnnotations;
using System.Data.Entity.ModelConfiguration;

namespace ModelLayer.Models.Mapping
{
    public class DeliveryOrderDetailMap : EntityTypeConfiguration<DeliveryOrderDetail>
    {
        public DeliveryOrderDetailMap()
        {
            // Primary Key
            this.HasKey(t => t.ID);

            // Properties
            this.Property(t => t.ShopOrderCode)
                .IsRequired()
                .HasMaxLength(15);

            this.Property(t => t.DeliveryType)
                .IsRequired()
                .HasMaxLength(10);

            this.Property(t => t.NetworkIP)
                .HasMaxLength(15);

            this.Property(t => t.DeviceType)
                .HasMaxLength(50);

            this.Property(t => t.DeviceID)
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("DeliveryOrderDetail");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.DeliveryPartnerID).HasColumnName("DeliveryPartnerID");
            this.Property(t => t.ShopOrderCode).HasColumnName("ShopOrderCode");
            this.Property(t => t.Weight).HasColumnName("Weight");
            this.Property(t => t.OrderAmount).HasColumnName("OrderAmount");
            this.Property(t => t.DeliveryCharge).HasColumnName("DeliveryCharge");
            this.Property(t => t.GandhibaghCharge).HasColumnName("GandhibaghCharge");
            this.Property(t => t.DeliveryType).HasColumnName("DeliveryType");
            this.Property(t => t.IsMyPincode).HasColumnName("IsMyPincode");
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
                .WithMany(t => t.DeliveryOrderDetails)
                .HasForeignKey(d => d.CreateBy);
            this.HasRequired(t => t.DeliveryPartner)
                .WithMany(t => t.DeliveryOrderDetails)
                .HasForeignKey(d => d.DeliveryPartnerID);
            this.HasOptional(t => t.PersonalDetail1)
                .WithMany(t => t.DeliveryOrderDetails1)
                .HasForeignKey(d => d.ModifyBy);

        }
    }
}
