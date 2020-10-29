using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace ModelLayer.Models.Mapping
{
    public class CustomerOrderLogMap : EntityTypeConfiguration<CustomerOrderLog>
    {
        public CustomerOrderLogMap()
        {
            // Primary Key
            this.HasKey(t => t.ID);

            // Properties
            this.Property(t => t.OrderCode)
                .IsRequired()
                .HasMaxLength(15);

            this.Property(t => t.CoupenCode)
                .HasMaxLength(15);

            this.Property(t => t.PAN)
                .HasMaxLength(10);

            this.Property(t => t.PaymentMode)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.PrimaryMobile)
                .IsRequired()
                .HasMaxLength(10);

            this.Property(t => t.SecondoryMobile)
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
            this.ToTable("CustomerOrderLog");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.OrderCode).HasColumnName("OrderCode");
            this.Property(t => t.UserLoginID).HasColumnName("UserLoginID");
            this.Property(t => t.ReferenceCustomerOrderID).HasColumnName("ReferenceCustomerOrderID");
            this.Property(t => t.OrderAmount).HasColumnName("OrderAmount");
            this.Property(t => t.NoOfPointUsed).HasColumnName("NoOfPointUsed");
            this.Property(t => t.ValuePerPoint).HasColumnName("ValuePerPoint");
            this.Property(t => t.CoupenCode).HasColumnName("CoupenCode");
            this.Property(t => t.CoupenAmount).HasColumnName("CoupenAmount");
            this.Property(t => t.PAN).HasColumnName("PAN");
            this.Property(t => t.PaymentMode).HasColumnName("PaymentMode");
            this.Property(t => t.PayableAmount).HasColumnName("PayableAmount");
            this.Property(t => t.PrimaryMobile).HasColumnName("PrimaryMobile");
            this.Property(t => t.SecondoryMobile).HasColumnName("SecondoryMobile");
            this.Property(t => t.ShippingAddress).HasColumnName("ShippingAddress");
            this.Property(t => t.PincodeID).HasColumnName("PincodeID");
            this.Property(t => t.AreaID).HasColumnName("AreaID");
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
