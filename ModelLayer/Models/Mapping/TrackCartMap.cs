using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace ModelLayer.Models.Mapping
{
    public class TrackCartMap : EntityTypeConfiguration<TrackCart>
    {
        public TrackCartMap()
        {
            // Primary Key
            this.HasKey(t => t.ID);

            // Properties
            this.Property(t => t.Mobile)
                .HasMaxLength(10);

            this.Property(t => t.Stage)
                .HasMaxLength(30);

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
            this.ToTable("TrackCart");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.CartID).HasColumnName("CartID");
            this.Property(t => t.UserLoginID).HasColumnName("UserLoginID");
            this.Property(t => t.ShopStockID).HasColumnName("ShopStockID");
            this.Property(t => t.Qty).HasColumnName("Qty");
            this.Property(t => t.Mobile).HasColumnName("Mobile");
            this.Property(t => t.Stage).HasColumnName("Stage");
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

            // Relationships
            this.HasOptional(t => t.Cart)
                .WithMany(t => t.TrackCarts)
                .HasForeignKey(d => d.CartID);

        }
    }
}
