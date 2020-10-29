using System.ComponentModel.DataAnnotations;
using System.Data.Entity.ModelConfiguration;

namespace ModelLayer.Models.Mapping
{
    public class WarehouseStockOrderDetailLogMap : EntityTypeConfiguration<WarehouseStockOrderDetailLog>
    {
        public WarehouseStockOrderDetailLogMap()
        {
            this.HasKey(t => t.ID);

            // Properties
            this.Property(t => t.WarehouseStockID);

            this.Property(t => t.Quantity);

            this.Property(t => t.CustomerOrderDetailID);
          

            // Table & Column Mappings
            this.ToTable("WarehouseStockOrderDetailLog");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.WarehouseStockID).HasColumnName("WarehouseStockID");
            this.Property(t => t.Quantity).HasColumnName("Quantity");
            this.Property(t => t.CustomerOrderDetailID).HasColumnName("CustomerOrderDetailID");            
        }
    }
}
