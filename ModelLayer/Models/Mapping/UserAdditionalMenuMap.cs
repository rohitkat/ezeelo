using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace ModelLayer.Models.Mapping
{
    public class UserAdditionalMenuMap : EntityTypeConfiguration<UserAdditionalMenu>
    {
        public UserAdditionalMenuMap()
        {
            // Primary Key
            this.HasKey(t => new { t.ID, t.UserLoginID, t.IsActive, t.CreateDate, t.CreateBy });

            // Properties
            this.Property(t => t.ID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            this.Property(t => t.UserLoginID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.CreateBy)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.NetworkIP)
                .HasMaxLength(15);

            this.Property(t => t.DeviceType)
                .HasMaxLength(50);

            this.Property(t => t.DeviceID)
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("UserAdditionalMenu");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.UserLoginID).HasColumnName("UserLoginID");
            this.Property(t => t.MenuID).HasColumnName("MenuID");
            this.Property(t => t.CanRead).HasColumnName("CanRead");
            this.Property(t => t.CanWrite).HasColumnName("CanWrite");
            this.Property(t => t.CanDelete).HasColumnName("CanDelete");
            this.Property(t => t.CanPrint).HasColumnName("CanPrint");
            this.Property(t => t.CanExport).HasColumnName("CanExport");
            this.Property(t => t.CanImport).HasColumnName("CanImport");
            this.Property(t => t.IsActive).HasColumnName("IsActive");
            this.Property(t => t.CreateDate).HasColumnName("CreateDate");
            this.Property(t => t.CreateBy).HasColumnName("CreateBy");
            this.Property(t => t.ModifyDate).HasColumnName("ModifyDate");
            this.Property(t => t.ModifyBy).HasColumnName("ModifyBy");
            this.Property(t => t.NetworkIP).HasColumnName("NetworkIP");
            this.Property(t => t.DeviceType).HasColumnName("DeviceType");
            this.Property(t => t.DeviceID).HasColumnName("DeviceID");

            // Relationships
            this.HasOptional(t => t.Menu)
                .WithMany(t => t.UserAdditionalMenus)
                .HasForeignKey(d => d.MenuID);
            this.HasRequired(t => t.PersonalDetail)
                .WithMany(t => t.UserAdditionalMenus)
                .HasForeignKey(d => d.CreateBy);
            this.HasOptional(t => t.PersonalDetail1)
                .WithMany(t => t.UserAdditionalMenus1)
                .HasForeignKey(d => d.ModifyBy);
            this.HasRequired(t => t.UserLogin)
                .WithMany(t => t.UserAdditionalMenus)
                .HasForeignKey(d => d.UserLoginID);

        }
    }
}
