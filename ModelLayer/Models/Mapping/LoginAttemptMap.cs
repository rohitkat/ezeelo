using System.ComponentModel.DataAnnotations;
using System.Data.Entity.ModelConfiguration;

namespace ModelLayer.Models.Mapping
{
    public class LoginAttemptMap : EntityTypeConfiguration<LoginAttempt>
    {
        public LoginAttemptMap()
        {
            // Primary Key
            this.HasKey(t => t.ID);

            // Properties
            this.Property(t => t.NetworkIP)
                .HasMaxLength(15);

            this.Property(t => t.DeviceType)
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("LoginAttempt");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.UserLoginID).HasColumnName("UserLoginID");
            this.Property(t => t.AttemptCount).HasColumnName("AttemptCount");
            this.Property(t => t.ExpirationTime).HasColumnName("ExpirationTime");
            this.Property(t => t.IsLocked).HasColumnName("IsLocked");
            this.Property(t => t.CreateDate).HasColumnName("CreateDate");
            this.Property(t => t.CreateBy).HasColumnName("CreateBy");
            this.Property(t => t.ModifyDate).HasColumnName("ModifyDate");
            this.Property(t => t.ModifyBy).HasColumnName("ModifyBy");
            this.Property(t => t.NetworkIP).HasColumnName("NetworkIP");
            this.Property(t => t.DeviceType).HasColumnName("DeviceType");

            // Relationships
            this.HasRequired(t => t.PersonalDetail)
                .WithMany(t => t.LoginAttempts)
                .HasForeignKey(d => d.CreateBy);
            this.HasRequired(t => t.UserLogin)
                .WithMany(t => t.LoginAttempts)
                .HasForeignKey(d => d.UserLoginID);
            this.HasOptional(t => t.PersonalDetail1)
                .WithMany(t => t.LoginAttempts1)
                .HasForeignKey(d => d.ModifyBy);

        }
    }
}
