using System.ComponentModel.DataAnnotations;
using System.Data.Entity.ModelConfiguration;

namespace ModelLayer.Models.Mapping
{
    public class QuotationItemDetailMap : EntityTypeConfiguration<QuotationItemDetail>
    {
        public QuotationItemDetailMap()
        {
            this.HasKey(t => t.ID);
            // Properties
            this.Property(t => t.QuotationID);

            this.Property(t => t.ProductID);

            this.Property(t => t.ProductNickname);
          
            this.Property(t => t.ProductVarientID);
          
            this.Property(t => t.Quantity); 

            // Table & Column Mappings
            this.ToTable("QuotationItemDetail");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.QuotationID).HasColumnName("QuotationID");
            this.Property(t => t.ProductID).HasColumnName("ProductID");
            this.Property(t => t.ProductNickname).HasColumnName("ProductNickname");
            this.Property(t => t.ProductVarientID).HasColumnName("ProductVarientID");           
            this.Property(t => t.Quantity).HasColumnName("Quantity");
        }
    }
}
