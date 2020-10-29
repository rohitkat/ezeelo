using System.ComponentModel.DataAnnotations;
using System.Data.Entity.ModelConfiguration;

namespace ModelLayer.Models.Mapping
{
    public class WarehouseStockMap : EntityTypeConfiguration<WarehouseStock>
    {

        public WarehouseStockMap()
        {
            this.HasKey(t => t.ID);

            // Properties
            this.Property(t => t.WarehouseID);

            this.Property(t => t.InvoiceID);

            this.Property(t => t.ProductID);

            this.Property(t => t.ProductVarientID);

            this.Property(t => t.MRP);

            this.Property(t => t.BuyRatePerUnit);

            this.Property(t => t.SaleRatePerUnit);

            this.Property(t => t.InitialQuantity);

            this.Property(t => t.AvailableQuantity);
         
            this.Property(t => t.StockStatus);

            this.Property(t => t.SizeID);

            this.Property(t => t.PackUnitID);
          

            // Table & Column Mappings
            this.ToTable("WarehouseStock");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.WarehouseID).HasColumnName("WarehouseID");
            this.Property(t => t.InvoiceID).HasColumnName("InvoiceID");
            this.Property(t => t.ProductID).HasColumnName("ProductID");
            this.Property(t => t.ProductVarientID).HasColumnName("ProductVarientID");
            this.Property(t => t.MRP).HasColumnName("MRP");
            this.Property(t => t.BuyRatePerUnit).HasColumnName("BuyRatePerUnit");
            this.Property(t => t.SaleRatePerUnit).HasColumnName("SaleRatePerUnit");
            this.Property(t => t.InitialQuantity).HasColumnName("InitialQuantity");
            this.Property(t => t.AvailableQuantity).HasColumnName("AvailableQuantity");           
            this.Property(t => t.StockStatus).HasColumnName("StockStatus");
            this.Property(t => t.SizeID).HasColumnName("SizeID");
            this.Property(t => t.PackUnitID).HasColumnName("PackUnitID");
        }
    }
}
