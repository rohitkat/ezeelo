using System.ComponentModel.DataAnnotations;
using System.Data.Entity.ModelConfiguration;


namespace ModelLayer.Models.Mapping
{
   public class InvoiceExtraItemMap: EntityTypeConfiguration<InvoiceExtraItem>
    {
       public InvoiceExtraItemMap()
        {
            this.HasKey(t => t.ID);
            // Properties
            this.Property(t => t.InvoiceID);          

            this.Property(t => t.ProductID);

            this.Property(t => t.ProductVarientID);
          
            this.Property(t => t.ProductNickname).HasMaxLength(500);

            this.Property(t => t.IsActive);
            

            // Table & Column Mappings
            this.ToTable("InvoiceExtraItem");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.InvoiceID).HasColumnName("InvoiceID");
            this.Property(t => t.ProductID).HasColumnName("ProductID");
            this.Property(t => t.ProductVarientID).HasColumnName("ProductVarientID");
            this.Property(t => t.ProductNickname).HasColumnName("ProductNickname");           
            this.Property(t => t.IsActive).HasColumnName("IsActive");           
           
        }
    }
}
