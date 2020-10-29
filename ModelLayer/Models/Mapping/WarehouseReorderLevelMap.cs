using System.ComponentModel.DataAnnotations;
using System.Data.Entity.ModelConfiguration;

namespace ModelLayer.Models.Mapping
{
    public class WarehouseReorderLevelMap : EntityTypeConfiguration<WarehouseReorderLevel>
    {
        public WarehouseReorderLevelMap()
        {
            this.HasKey(t => t.ID);

            // Properties
            this.Property(t => t.WarehouseID);          

            this.Property(t => t.ProductID);

            this.Property(t => t.ProductVarientID);           

            this.Property(t => t.AvailableQuantity);

            this.Property(t => t.ReorderLevel);            
          

            // Table & Column Mappings
            this.ToTable("WarehouseReorderLevel");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.WarehouseID).HasColumnName("WarehouseID");          
            this.Property(t => t.ProductID).HasColumnName("ProductID");
            this.Property(t => t.ProductVarientID).HasColumnName("ProductVarientID");           
            this.Property(t => t.AvailableQuantity).HasColumnName("AvailableQuantity");
            this.Property(t => t.ReorderLevel).HasColumnName("ReorderLevel");           
        }
    }
}
