using System.ComponentModel.DataAnnotations;
using System.Data.Entity.ModelConfiguration;

namespace ModelLayer.Models.Mapping
{
    public class CustomerCoupenMap : EntityTypeConfiguration<CustomerCoupen>
    {
        public CustomerCoupenMap()
        {
            // Primary Key
            this.HasKey(t => t.ID);

            // Properties
            this.Property(t => t.CoupenCode)
                .IsRequired()
                .HasMaxLength(15);

            this.Property(t => t.NetworkIP)
                .HasMaxLength(15);

            this.Property(t => t.DeviceType)
                .HasMaxLength(50);

            this.Property(t => t.DeviceID)
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("CustomerCoupen");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.UserLoginId).HasColumnName("UserLoginId");
            this.Property(t => t.CoupenCode).HasColumnName("CoupenCode");
            this.Property(t => t.CustomerOrderID).HasColumnName("CustomerOrderID");
            this.Property(t => t.CoupenValueInRs).HasColumnName("CoupenValueInRs");
            this.Property(t => t.IsRedeem).HasColumnName("IsRedeem");
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
                .WithMany(t => t.CustomerCoupens)
                .HasForeignKey(d => d.CreateBy);
            this.HasOptional(t => t.PersonalDetail1)
                .WithMany(t => t.CustomerCoupens1)
                .HasForeignKey(d => d.ModifyBy);

        }
    }
}
