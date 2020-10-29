using System.ComponentModel.DataAnnotations;
using System.Data.Entity.ModelConfiguration;

namespace ModelLayer.Models.Mapping
{
    public class UserLoginMap : EntityTypeConfiguration<UserLogin>
    {
        public UserLoginMap()
        {
            // Primary Key
            this.HasKey(t => t.ID);

            // Properties
            this.Property(t => t.Mobile)
                //.IsRequired()
                .HasMaxLength(10);

            this.Property(t => t.Email)
               // .IsRequired()
                .HasMaxLength(150);

            this.Property(t => t.Password)
                .IsRequired()
                .HasMaxLength(100);

            this.Property(t => t.NetworkIP)
                .HasMaxLength(15);

            this.Property(t => t.DeviceType)
                .HasMaxLength(50);

            this.Property(t => t.DeviceID)
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("UserLogin");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.Mobile).HasColumnName("Mobile");
            this.Property(t => t.Email).HasColumnName("Email");
            this.Property(t => t.Password).HasColumnName("Password");
            this.Property(t => t.IsLocked).HasColumnName("IsLocked");
            this.Property(t => t.CreateDate).HasColumnName("CreateDate");
            this.Property(t => t.CreateBy).HasColumnName("CreateBy");
            this.Property(t => t.ModifyDate).HasColumnName("ModifyDate");
            this.Property(t => t.ModifyBy).HasColumnName("ModifyBy");
            this.Property(t => t.NetworkIP).HasColumnName("NetworkIP");
            this.Property(t => t.DeviceType).HasColumnName("DeviceType");
            this.Property(t => t.DeviceID).HasColumnName("DeviceID");

            // Relationships
            this.HasOptional(t => t.PersonalDetail)
                .WithMany(t => t.UserLogins)
                .HasForeignKey(d => d.CreateBy);
            this.HasOptional(t => t.PersonalDetail1)
                .WithMany(t => t.UserLogins1)
                .HasForeignKey(d => d.ModifyBy);
            this.HasOptional(t => t.PersonalDetail2)
                .WithMany(t => t.UserLogins2)
                .HasForeignKey(d => d.CreateBy);
            this.HasOptional(t => t.PersonalDetail3)
                .WithMany(t => t.UserLogins3)
                .HasForeignKey(d => d.ModifyBy);

        }
    }
}
