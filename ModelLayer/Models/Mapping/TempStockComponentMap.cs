using System.ComponentModel.DataAnnotations;
using System.Data.Entity.ModelConfiguration;

namespace ModelLayer.Models.Mapping
{
    public class TempStockComponentMap : EntityTypeConfiguration<TempStockComponent>
    {
        public TempStockComponentMap()
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
            this.ToTable("TempStockComponent");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.ShopStockID).HasColumnName("ShopStockID");
            this.Property(t => t.ComponentID).HasColumnName("ComponentID");
            this.Property(t => t.ComponentWeight).HasColumnName("ComponentWeight");
            this.Property(t => t.ComponentUnitID).HasColumnName("ComponentUnitID");
            this.Property(t => t.CreateDate).HasColumnName("CreateDate");
            this.Property(t => t.IsActive).HasColumnName("IsActive");
            this.Property(t => t.CreateBy).HasColumnName("CreateBy");
            this.Property(t => t.ModifyDate).HasColumnName("ModifyDate");
            this.Property(t => t.ModifyBy).HasColumnName("ModifyBy");
            this.Property(t => t.NetworkIP).HasColumnName("NetworkIP");
            this.Property(t => t.DeviceType).HasColumnName("DeviceType");
            this.Property(t => t.DeviceID).HasColumnName("DeviceID");

            // Relationships
            this.HasRequired(t => t.Component)
                .WithMany(t => t.TempStockComponents)
                .HasForeignKey(d => d.ComponentID);
            this.HasRequired(t => t.PersonalDetail)
                .WithMany(t => t.TempStockComponents)
                .HasForeignKey(d => d.CreateBy);
            this.HasOptional(t => t.PersonalDetail1)
                .WithMany(t => t.TempStockComponents1)
                .HasForeignKey(d => d.ModifyBy);
            this.HasRequired(t => t.TempShopStock)
                .WithMany(t => t.TempStockComponents)
                .HasForeignKey(d => d.ShopStockID);
            this.HasRequired(t => t.Unit)
                .WithMany(t => t.TempStockComponents)
                .HasForeignKey(d => d.ComponentUnitID);

        }
    }
}
