using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace ModelLayer.Models.Mapping
{
    public class GcmUserMap : EntityTypeConfiguration<GcmUser>
    {
        public GcmUserMap()
        {
            // Primary Key
            this.HasKey(t => t.ID);

            // Properties
            this.Property(t => t.EmailID)
                .HasMaxLength(50);

            this.Property(t => t.Name)
                .HasMaxLength(100);

            this.Property(t => t.City)
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("GcmUser");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.GcmRegID).HasColumnName("GcmRegID");
            this.Property(t => t.EmailID).HasColumnName("EmailID");
            this.Property(t => t.IsActive).HasColumnName("IsActive");
            this.Property(t => t.CreateDate).HasColumnName("CreateDate");
            this.Property(t => t.Name).HasColumnName("Name");
            this.Property(t => t.City).HasColumnName("City");
        }
    }
}
