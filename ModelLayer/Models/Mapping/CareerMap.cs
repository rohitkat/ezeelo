using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace ModelLayer.Models.Mapping
{
    public class CareerMap : EntityTypeConfiguration<Career>
    {
        public CareerMap()
        {
            // Primary Key
            this.HasKey(t => t.ID);

            // Properties
            this.Property(t => t.Jobtitle)
                .IsRequired()
                .HasMaxLength(200);

            this.Property(t => t.Education)
                .HasMaxLength(300);

            this.Property(t => t.SkillRequired)
                .HasMaxLength(300);

            this.Property(t => t.Location)
                .HasMaxLength(150);

            this.Property(t => t.Domain)
                .HasMaxLength(50);

            this.Property(t => t.NetworkIP)
                .HasMaxLength(15);

            this.Property(t => t.DeviceType)
                .HasMaxLength(50);

            this.Property(t => t.DeviceID)
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("Career");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.Jobtitle).HasColumnName("Jobtitle");
            this.Property(t => t.Education).HasColumnName("Education");
            this.Property(t => t.SkillRequired).HasColumnName("SkillRequired");
            this.Property(t => t.NoOfOpening).HasColumnName("NoOfOpening");
            this.Property(t => t.Location).HasColumnName("Location");
            this.Property(t => t.Domain).HasColumnName("Domain");
            this.Property(t => t.PostDate).HasColumnName("PostDate");
            this.Property(t => t.ExpiryDate).HasColumnName("ExpiryDate");
            this.Property(t => t.Description).HasColumnName("Description");
            this.Property(t => t.IsActive).HasColumnName("IsActive");
            this.Property(t => t.CreateDate).HasColumnName("CreateDate");
            this.Property(t => t.CreateBy).HasColumnName("CreateBy");
            this.Property(t => t.ModifyDate).HasColumnName("ModifyDate");
            this.Property(t => t.ModifyBy).HasColumnName("ModifyBy");
            this.Property(t => t.NetworkIP).HasColumnName("NetworkIP");
            this.Property(t => t.DeviceType).HasColumnName("DeviceType");
            this.Property(t => t.DeviceID).HasColumnName("DeviceID");
        }
    }
}
