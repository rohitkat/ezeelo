using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace ModelLayer.Models.Mapping
{
    public class UserThemeMap : EntityTypeConfiguration<UserTheme>
    {
        public UserThemeMap()
        {
            // Primary Key
            this.HasKey(t => new { t.ID, t.CreateDate, t.CreateBy });

            // Properties
            this.Property(t => t.ID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.CssName)
                .HasMaxLength(50);

            this.Property(t => t.CreateBy)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.NetworkIP)
                .HasMaxLength(15);

            this.Property(t => t.DeviceType)
                .HasMaxLength(50);

            this.Property(t => t.DeviceID)
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("UserTheme");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.UID).HasColumnName("UID");
            this.Property(t => t.CssName).HasColumnName("CssName");
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
                .WithMany(t => t.UserThemes)
                .HasForeignKey(d => d.CreateBy);
            this.HasOptional(t => t.PersonalDetail1)
                .WithMany(t => t.UserThemes1)
                .HasForeignKey(d => d.ModifyBy);
            this.HasOptional(t => t.UserLogin)
                .WithMany(t => t.UserThemes)
                .HasForeignKey(d => d.UID);

        }
    }
}
