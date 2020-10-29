using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace ModelLayer.Models.Mapping
{
    public class TrackSearchMap : EntityTypeConfiguration<TrackSearch>
    {
        public TrackSearchMap()
        {
            // Primary Key
            this.HasKey(t => t.ID);

            // Properties
            this.Property(t => t.ProductName)
                .HasMaxLength(300);

            this.Property(t => t.Lattitude)
                .HasMaxLength(15);

            this.Property(t => t.Longitude)
                .HasMaxLength(15);

            this.Property(t => t.NetworkIP)
                .HasMaxLength(15);

            this.Property(t => t.DeviceType)
                .HasMaxLength(50);

            this.Property(t => t.DeviceID)
                .HasMaxLength(50);

            this.Property(t => t.City)
              .HasMaxLength(50);

            this.Property(t => t.IMEI_NO)
               .HasMaxLength(20);

            // Table & Column Mappings
            this.ToTable("TrackSearch");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.UserLoginID).HasColumnName("UserLoginID");
            this.Property(t => t.CategoryID).HasColumnName("CategoryID");
            this.Property(t => t.ShopID).HasColumnName("ShopID");
            this.Property(t => t.ProductName).HasColumnName("ProductName");
            this.Property(t => t.Lattitude).HasColumnName("Lattitude");
            this.Property(t => t.Longitude).HasColumnName("Longitude");
            this.Property(t => t.NetworkIP).HasColumnName("NetworkIP");
            this.Property(t => t.DeviceType).HasColumnName("DeviceType");
            this.Property(t => t.DeviceID).HasColumnName("DeviceID");
            this.Property(t => t.CreateDate).HasColumnName("CreateDate");
            this.Property(t => t.CreateBy).HasColumnName("CreateBy");
            this.Property(t => t.City).HasColumnName("City");
            this.Property(t => t.FranchiseID).HasColumnName("FranchiseID");//added
            this.Property(t => t.IMEI_NO).HasColumnName("IMEI_NO");
        }
    }
}
