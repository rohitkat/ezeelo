using System.ComponentModel.DataAnnotations;
using System.Data.Entity.ModelConfiguration;

namespace ModelLayer.Models.Mapping
{
    public class RoleMenuMap : EntityTypeConfiguration<RoleMenu>
    {
        public RoleMenuMap()
        {
            // Primary Key
            this.HasKey(t => t.ID);

            // Properties
            this.Property(t => t.NetworkIP)
                .HasMaxLength(15);

            this.Property(t => t.DeviceType)
                .HasMaxLength(50);

            this.Property(t => t.DeviceID)
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("RoleMenu");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.RoleID).HasColumnName("RoleID");
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
                .WithMany(t => t.RoleMenus)
                .HasForeignKey(d => d.MenuID);
            this.HasRequired(t => t.PersonalDetail)
                .WithMany(t => t.RoleMenus)
                .HasForeignKey(d => d.CreateBy);
            this.HasOptional(t => t.PersonalDetail1)
                .WithMany(t => t.RoleMenus1)
                .HasForeignKey(d => d.ModifyBy);
            this.HasRequired(t => t.Role)
                .WithMany(t => t.RoleMenus)
                .HasForeignKey(d => d.RoleID);

        }
    }
}
