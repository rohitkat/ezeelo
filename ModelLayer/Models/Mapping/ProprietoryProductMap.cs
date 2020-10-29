using System.ComponentModel.DataAnnotations;
using System.Data.Entity.ModelConfiguration;

namespace ModelLayer.Models.Mapping
{
    public class ProprietoryProductMap : EntityTypeConfiguration<ProprietoryProduct>
    {
        public ProprietoryProductMap()
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
            this.ToTable("ProprietoryProducts");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.ShopID).HasColumnName("ShopID");
            this.Property(t => t.ProductID).HasColumnName("ProductID");
            this.Property(t => t.CreateDate).HasColumnName("CreateDate");
            this.Property(t => t.CreateBy).HasColumnName("CreateBy");
            this.Property(t => t.NetworkIP).HasColumnName("NetworkIP");
            this.Property(t => t.DeviceType).HasColumnName("DeviceType");
            this.Property(t => t.DeviceID).HasColumnName("DeviceID");

            // Relationships
            this.HasRequired(t => t.PersonalDetail)
                .WithMany(t => t.ProprietoryProducts)
                .HasForeignKey(d => d.CreateBy);
            this.HasRequired(t => t.Product)
                .WithMany(t => t.ProprietoryProducts)
                .HasForeignKey(d => d.ProductID);
            this.HasRequired(t => t.Shop)
                .WithMany(t => t.ProprietoryProducts)
                .HasForeignKey(d => d.ShopID);

        }
    }
}
