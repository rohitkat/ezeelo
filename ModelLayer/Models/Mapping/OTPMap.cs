using System.ComponentModel.DataAnnotations;
using System.Data.Entity.ModelConfiguration;

namespace ModelLayer.Models.Mapping
{
    public class OTPMap : EntityTypeConfiguration<OTP>
    {
        public OTPMap()
        {
            // Primary Key
            this.HasKey(t => t.ID);

            // Properties
            this.Property(t => t.SessionCode)
                .IsRequired()
                .HasMaxLength(10);

            this.Property(t => t.OTP1)
                .IsRequired()
                .HasMaxLength(6);

            this.Property(t => t.NetworkIP)
                .HasMaxLength(15);

            this.Property(t => t.DeviceType)
                .HasMaxLength(50);

            this.Property(t => t.DeviceID)
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("OTP");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.SessionCode).HasColumnName("SessionCode");
            this.Property(t => t.OTP1).HasColumnName("OTP");
            this.Property(t => t.ExpirationTime).HasColumnName("ExpirationTime");
            this.Property(t => t.IsActive).HasColumnName("IsActive");
            this.Property(t => t.CreateDate).HasColumnName("CreateDate");
            this.Property(t => t.CreateBy).HasColumnName("CreateBy");
            this.Property(t => t.ModifyDate).HasColumnName("ModifyDate");
            this.Property(t => t.ModifyBy).HasColumnName("ModifyBy");
            this.Property(t => t.NetworkIP).HasColumnName("NetworkIP");
            this.Property(t => t.DeviceType).HasColumnName("DeviceType");
            this.Property(t => t.DeviceID).HasColumnName("DeviceID");
            //-- Add By Ashish Nagrale --//
            // Hide from Ashish for Live
            /*this.Property(t => t.OrderCode).HasColumnName("OrderCode");
            this.Property(t => t.ShopOrderCode).HasColumnName("ShopOrderCode");
            this.Property(t => t.PayableAmount).HasColumnName("PayableAmount");
            */
            //-- End Add --//
            // Relationships
            this.HasRequired(t => t.PersonalDetail)
                .WithMany(t => t.OTPs)
                .HasForeignKey(d => d.CreateBy);
            this.HasOptional(t => t.PersonalDetail1)
                .WithMany(t => t.OTPs1)
                .HasForeignKey(d => d.ModifyBy);

        }
    }
}
