using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace ModelLayer.Models.Mapping
{
    public class SEOMap : EntityTypeConfiguration<SEO>
    {
        public SEOMap()
        {
            // Primary Key
            this.HasKey(t => t.ID);

            // Properties
            this.Property(t => t.H1)
                .HasMaxLength(150);

            this.Property(t => t.Metatag)
                .HasMaxLength(500);

            this.Property(t => t.URL)
                .HasMaxLength(150);

            this.Property(t => t.PageName)
                .HasMaxLength(50);

            this.Property(t => t.NetworkIP)
                .HasMaxLength(15);

            this.Property(t => t.DeviceType)
                .HasMaxLength(50);

            this.Property(t => t.DeviceID)
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("SEO");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.BusinessTypeID).HasColumnName("BusinessTypeID");
            this.Property(t => t.EntityID).HasColumnName("EntityID");
            this.Property(t => t.H1).HasColumnName("H1");
            this.Property(t => t.Metatag).HasColumnName("Metatag");
            this.Property(t => t.Description).HasColumnName("Description");
            this.Property(t => t.MetaKeyword).HasColumnName("MetaKeyword");
            this.Property(t => t.URL).HasColumnName("URL");
            this.Property(t => t.PageName).HasColumnName("PageName");
            this.Property(t => t.IsActive).HasColumnName("IsActive");
            this.Property(t => t.CreateDate).HasColumnName("CreateDate");
            this.Property(t => t.CreateBy).HasColumnName("CreateBy");
            this.Property(t => t.ModifyDate).HasColumnName("ModifyDate");
            this.Property(t => t.ModifyBy).HasColumnName("ModifyBy");
            this.Property(t => t.NetworkIP).HasColumnName("NetworkIP");
            this.Property(t => t.DeviceType).HasColumnName("DeviceType");
            this.Property(t => t.DeviceID).HasColumnName("DeviceID");

            // Relationships
            this.HasRequired(t => t.BusinessType)
                .WithMany(t => t.SEOs)
                .HasForeignKey(d => d.BusinessTypeID);
            this.HasRequired(t => t.PersonalDetail)
                .WithMany(t => t.SEOs)
                .HasForeignKey(d => d.CreateBy);
            this.HasOptional(t => t.PersonalDetail1)
                .WithMany(t => t.SEOs1)
                .HasForeignKey(d => d.ModifyBy);

        }
    }
}
