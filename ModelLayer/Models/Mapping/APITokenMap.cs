using System.ComponentModel.DataAnnotations;
using System.Data.Entity.ModelConfiguration;

namespace ModelLayer.Models.Mapping
{
    public class APITokenMap : EntityTypeConfiguration<APIToken>
    {
        public APITokenMap()
        {
            // Primary Key
            this.HasKey(t => t.ID);

            // Properties
            this.Property(t => t.TokenCode)
                .HasMaxLength(15);

            this.Property(t => t.IMEI)
               .HasMaxLength(300);

            this.Property(t => t.NetworkIP)
                .HasMaxLength(15);

            this.Property(t => t.DeviceType)
                .HasMaxLength(50);

            this.Property(t => t.DeviceID)
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("APIToken");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.UserLoginID).HasColumnName("UserLoginID");
            this.Property(t => t.TokenCode).HasColumnName("TokenCode");
            this.Property(t => t.IMEI).HasColumnName("IMEI");
            this.Property(t => t.IsActive).HasColumnName("IsActive");
            this.Property(t => t.CreateDate).HasColumnName("CreateDate");
            this.Property(t => t.CreateBy).HasColumnName("CreateBy");
            this.Property(t => t.ModifyDate).HasColumnName("ModifyDate");
            this.Property(t => t.ModifyBy).HasColumnName("ModifyBy");
            this.Property(t => t.NetworkIP).HasColumnName("NetworkIP");
            this.Property(t => t.DeviceType).HasColumnName("DeviceType");
            this.Property(t => t.DeviceID).HasColumnName("DeviceID");

            // Relationships
            this.HasOptional(t => t.UserLogin)
                .WithMany(t => t.APITokens)
                .HasForeignKey(d => d.UserLoginID);
            this.HasRequired(t => t.PersonalDetail)
                .WithMany(t => t.APITokens)
                .HasForeignKey(d => d.CreateBy);
            this.HasOptional(t => t.PersonalDetail1)
                .WithMany(t => t.APITokens1)
                .HasForeignKey(d => d.ModifyBy);

        }
    }
}
