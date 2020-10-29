using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace ModelLayer.Models.Mapping
{
    // Hide from Ashish for Live
    /*public class EPODMap : EntityTypeConfiguration<EPOD>
    {
        public EPODMap()
        {
            // Primary Key
            this.HasKey(t => t.ID);

            // Properties
            this.Property(t => t.OrderCode)
                .HasMaxLength(15);

            this.Property(t => t.ShopOrderCode)
                .HasMaxLength(15);

            this.Property(t => t.SignatureFromMerchant)
                .HasMaxLength(Int32.MaxValue);

            this.Property(t => t.SignatureFromCustomer)
                .HasMaxLength(Int32.MaxValue);

            this.Property(t => t.NetworkIP)
                .HasMaxLength(15);

            this.Property(t => t.DeviceType)
                .HasMaxLength(50);

            this.Property(t => t.DeviceID)
                .HasMaxLength(50);



            // Table & Column Mappings
            this.ToTable("EPOD");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.OrderCode).HasColumnName("OrderCode");
            this.Property(t => t.ShopOrderCode).HasColumnName("ShopOrderCode");
            this.Property(t => t.SignatureFromMerchant).HasColumnName("SignatureFromMerchant");
            this.Property(t => t.SignatureFromCustomer).HasColumnName("SignatureFromCustomer");
            this.Property(t => t.PayableAmount).HasColumnName("PayableAmount");
            this.Property(t => t.CreateDate).HasColumnName("CreateDate");
            this.Property(t => t.CreateBy).HasColumnName("CreateBy");
            this.Property(t => t.ModifyDate).HasColumnName("ModifyDate");
            this.Property(t => t.ModifyBy).HasColumnName("ModifyBy");
            this.Property(t => t.NetworkIP).HasColumnName("NetworkIP");
            this.Property(t => t.DeviceType).HasColumnName("DeviceType");
            this.Property(t => t.DeviceID).HasColumnName("DeviceID");

        }
    }
    */
}
