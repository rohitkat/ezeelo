using System.ComponentModel.DataAnnotations;
using System.Data.Entity.ModelConfiguration;

namespace ModelLayer.Models.Mapping
{
    public class DomainCatgeoryMap : EntityTypeConfiguration<DomainCatgeory>
    {
        public DomainCatgeoryMap()
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
            this.ToTable("DomainCatgeory");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.DomainID).HasColumnName("DomainID");
            this.Property(t => t.CategoryID).HasColumnName("CategoryID");
            this.Property(t => t.IsPrimary).HasColumnName("IsPrimary");
            this.Property(t => t.IsActive).HasColumnName("IsActive");
            this.Property(t => t.CreateDate).HasColumnName("CreateDate");
            this.Property(t => t.CreateBy).HasColumnName("CreateBy");
            this.Property(t => t.ModifyDate).HasColumnName("ModifyDate");
            this.Property(t => t.ModifyBy).HasColumnName("ModifyBy");
            this.Property(t => t.NetworkIP).HasColumnName("NetworkIP");
            this.Property(t => t.DeviceType).HasColumnName("DeviceType");
            this.Property(t => t.DeviceID).HasColumnName("DeviceID");

            // Relationships
            this.HasOptional(t => t.Category)
                .WithMany(t => t.DomainCatgeories)
                .HasForeignKey(d => d.CategoryID);
            this.HasOptional(t => t.Domain)
                .WithMany(t => t.DomainCatgeories)
                .HasForeignKey(d => d.DomainID);
            this.HasRequired(t => t.PersonalDetail)
                .WithMany(t => t.DomainCatgeories)
                .HasForeignKey(d => d.CreateBy);
            this.HasOptional(t => t.PersonalDetail1)
                .WithMany(t => t.DomainCatgeories1)
                .HasForeignKey(d => d.ModifyBy);

        }
    }
}
