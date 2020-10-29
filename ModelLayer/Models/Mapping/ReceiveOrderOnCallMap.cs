using System.ComponentModel.DataAnnotations;
using System.Data.Entity.ModelConfiguration;

namespace ModelLayer.Models.Mapping
{
    public class ReceiveOrderOnCallMap : EntityTypeConfiguration<ReceiveOrderOnCall>
    {
        public ReceiveOrderOnCallMap()
        {
            // Primary Key
            this.HasKey(t => t.ID);

            // Properties
            this.Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(150);

            this.Property(t => t.PrimaryMobile)
                .IsRequired()
                .HasMaxLength(10);

            this.Property(t => t.SecondaryMobile)
                .HasMaxLength(10);

            this.Property(t => t.Email)
                .IsRequired()
                .HasMaxLength(150);

            this.Property(t => t.ShippingAddress)
                .IsRequired()
                .HasMaxLength(150);

            this.Property(t => t.Description)
                .IsRequired()
                .HasMaxLength(300);

            this.Property(t => t.NetworkIP)
                .HasMaxLength(15);

            this.Property(t => t.DeviceType)
                .HasMaxLength(50);

            this.Property(t => t.DeviceID)
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("ReceiveOrderOnCall");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.OrderReceivedPersonalDetailID).HasColumnName("OrderReceivedPersonalDetailID");
            this.Property(t => t.Name).HasColumnName("Name");
            this.Property(t => t.PrimaryMobile).HasColumnName("PrimaryMobile");
            this.Property(t => t.SecondaryMobile).HasColumnName("SecondaryMobile");
            this.Property(t => t.Email).HasColumnName("Email");
            this.Property(t => t.ShippingAddress).HasColumnName("ShippingAddress");
            this.Property(t => t.Description).HasColumnName("Description");
            this.Property(t => t.CustomerOrderID).HasColumnName("CustomerOrderID");
            this.Property(t => t.CreateDate).HasColumnName("CreateDate");
            this.Property(t => t.CreateBy).HasColumnName("CreateBy");
            this.Property(t => t.ModifyDate).HasColumnName("ModifyDate");
            this.Property(t => t.ModifyBy).HasColumnName("ModifyBy");
            this.Property(t => t.NetworkIP).HasColumnName("NetworkIP");
            this.Property(t => t.DeviceType).HasColumnName("DeviceType");
            this.Property(t => t.DeviceID).HasColumnName("DeviceID");

            // Relationships
            this.HasOptional(t => t.CustomerOrder)
                .WithMany(t => t.ReceiveOrderOnCalls)
                .HasForeignKey(d => d.CustomerOrderID);
            this.HasRequired(t => t.PersonalDetail)
                .WithMany(t => t.ReceiveOrderOnCalls)
                .HasForeignKey(d => d.CreateBy);
            this.HasOptional(t => t.PersonalDetail1)
                .WithMany(t => t.ReceiveOrderOnCalls1)
                .HasForeignKey(d => d.ModifyBy);
            this.HasRequired(t => t.PersonalDetail2)
                .WithMany(t => t.ReceiveOrderOnCalls2)
                .HasForeignKey(d => d.OrderReceivedPersonalDetailID);

        }
    }
}
