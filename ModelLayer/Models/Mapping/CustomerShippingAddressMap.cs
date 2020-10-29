using System.ComponentModel.DataAnnotations;
using System.Data.Entity.ModelConfiguration;

namespace ModelLayer.Models.Mapping
{
    public class CustomerShippingAddressMap : EntityTypeConfiguration<CustomerShippingAddress>
    {
        public CustomerShippingAddressMap()
        {
            // Primary Key
            this.HasKey(t => t.ID);

            // Properties
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
            this.ToTable("CustomerShippingAddress");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.UserLoginID).HasColumnName("UserLoginID");
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
            this.HasOptional(t => t.Area)
                .WithMany(t => t.CustomerShippingAddresses)
                .HasForeignKey(d => d.AreaID);
            this.HasRequired(t => t.PersonalDetail)
                .WithMany(t => t.CustomerShippingAddresses)
                .HasForeignKey(d => d.CreateBy);
            this.HasRequired(t => t.Pincode)
                .WithMany(t => t.CustomerShippingAddresses)
                .HasForeignKey(d => d.PincodeID);
            this.HasRequired(t => t.UserLogin)
                .WithMany(t => t.CustomerShippingAddresses)
                .HasForeignKey(d => d.UserLoginID);
            this.HasOptional(t => t.PersonalDetail1)
                .WithMany(t => t.CustomerShippingAddresses1)
                .HasForeignKey(d => d.ModifyBy);

        }
    }
}
