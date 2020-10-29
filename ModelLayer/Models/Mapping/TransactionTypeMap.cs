using System.ComponentModel.DataAnnotations;
using System.Data.Entity.ModelConfiguration;


namespace ModelLayer.Models.Mapping
{
   public class TransactionTypeMap: EntityTypeConfiguration<TransactionType>
    {
       public TransactionTypeMap()
        {
            this.HasKey(t => t.ID);           
          
            this.Property(t => t.Name).HasMaxLength(500);            

            // Table & Column Mappings
            this.ToTable("TransactionType");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.Name).HasColumnName("Name");            
        }
    }
}
