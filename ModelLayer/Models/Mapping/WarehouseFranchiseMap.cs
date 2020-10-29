using System.ComponentModel.DataAnnotations;
using System.Data.Entity.ModelConfiguration;


namespace ModelLayer.Models.Mapping
{
   public class WarehouseFranchiseMap: EntityTypeConfiguration<WarehouseFranchise>
    {
       public WarehouseFranchiseMap()
        {
        // Primary Key
            this.HasKey(t => t.ID);

            // Properties
            this.Property(t => t.WarehouseID);

            this.Property(t => t.FranchiseID); 

            this.Property(t => t.IsActive);

        // Table & Column Mappings
            this.ToTable("WarehouseFranchise");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.WarehouseID).HasColumnName("WarehouseID");
            this.Property(t => t.FranchiseID).HasColumnName("FranchiseID");
            this.Property(t => t.IsActive).HasColumnName("IsActive");
       }
    }
}
