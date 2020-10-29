using System.ComponentModel.DataAnnotations;
using System.Data.Entity.ModelConfiguration;

namespace ModelLayer.Models.Mapping
{
    public class GBTrackMap : EntityTypeConfiguration<GBTrack>
    {
        public GBTrackMap()
        {
            // Primary Key
            this.HasKey(t => t.ID);

            // Properties
            this.Property(t => t.PageURL)
                .HasMaxLength(256);

            this.Property(t => t.NetworkIP)
                .HasMaxLength(50);

            this.Property(t => t.DeviceType)
                .HasMaxLength(50);

            this.Property(t => t.DeviceID)
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("GBTrack");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.PageURL).HasColumnName("PageURL");
            this.Property(t => t.UserLoginId).HasColumnName("UserLoginId");
            this.Property(t => t.InTime).HasColumnName("InTime");
            this.Property(t => t.OutTime).HasColumnName("OutTime");
            this.Property(t => t.NetworkIP).HasColumnName("NetworkIP");
            this.Property(t => t.DeviceType).HasColumnName("DeviceType");
            this.Property(t => t.DeviceID).HasColumnName("DeviceID");
        }
    }
}
