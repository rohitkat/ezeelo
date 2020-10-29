using System.ComponentModel.DataAnnotations;
using System.Data.Entity.ModelConfiguration;

namespace ModelLayer.Models.Mapping
{
    public class WarehouseBudgetMap : EntityTypeConfiguration<WarehouseBudget>
    {
        public WarehouseBudgetMap()
        {
            this.HasKey(t => t.ID);

            // Properties
            this.Property(t => t.WarehouseID);

            this.Property(t => t.SupplierID);

            this.Property(t => t.AmountPaid);

            this.Property(t => t.AmountBalance);

            this.Property(t => t.AmountAdvance);

            this.Property(t => t.AmountRefund);           
            

            // Table & Column Mappings
            this.ToTable("WarehouseBudget");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.WarehouseID).HasColumnName("WarehouseID");
            this.Property(t => t.SupplierID).HasColumnName("SupplierID");
            this.Property(t => t.AmountPaid).HasColumnName("AmountPaid");
            this.Property(t => t.AmountBalance).HasColumnName("AmountBalance");
            this.Property(t => t.AmountAdvance).HasColumnName("AmountAdvance");
            this.Property(t => t.AmountRefund).HasColumnName("AmountRefund");                  
           
        }
    }
}
