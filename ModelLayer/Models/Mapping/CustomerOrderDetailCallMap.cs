using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace ModelLayer.Models.Mapping
{
    public class CustomerOrderDetailCallMap : EntityTypeConfiguration<CustomerOrderDetailCall>
    {
        public CustomerOrderDetailCallMap()
        {
            // Primary Key
            this.HasKey(t => t.ID);

            // Properties
            this.Property(t => t.ShopOrderCode)
                .IsRequired()
                .HasMaxLength(15);

            this.Property(t => t.Description)
                .IsRequired()
                .HasMaxLength(300);

            // Table & Column Mappings
            this.ToTable("CustomerOrderDetailCall");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.BusinessTypeID).HasColumnName("BusinessTypeID");
            this.Property(t => t.OwnerID).HasColumnName("OwnerID");
            this.Property(t => t.ShopOrderCode).HasColumnName("ShopOrderCode");
            this.Property(t => t.OrderStatus).HasColumnName("OrderStatus");
            this.Property(t => t.Description).HasColumnName("Description");
            this.Property(t => t.CreateDate).HasColumnName("CreateDate");
            this.Property(t => t.CreateBy).HasColumnName("CreateBy");
            ////this.Property(t => t.OTP).HasColumnName("OTP");//Added // Hide from Ashish for Live

            // Relationships
            //this.HasRequired(t => t.BusinessType)
            //    .WithMany(t => t.CustomerOrderDetailCalls)
            //    .HasForeignKey(d => d.BusinessTypeID);
            this.HasRequired(t => t.PersonalDetail)
                .WithMany(t => t.CustomerOrderDetailCalls)
                .HasForeignKey(d => d.CreateBy);

        }
    }
}
