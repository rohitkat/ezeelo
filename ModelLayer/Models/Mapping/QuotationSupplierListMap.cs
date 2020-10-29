using System.ComponentModel.DataAnnotations;
using System.Data.Entity.ModelConfiguration;

namespace ModelLayer.Models.Mapping
{
    public class QuotationSupplierListMap : EntityTypeConfiguration<QuotationSupplierList>
    {
        public QuotationSupplierListMap()
        {
            this.HasKey(t => t.ID);
            // Properties
            this.Property(t => t.QuotationID);

            this.Property(t => t.SupplierID);

            this.Property(t => t.IsReplied);
          
            this.Property(t => t.QuotationReplyDate);
          
            this.Property(t => t.Amount);

            this.Property(t => t.GSTAmount);

            this.Property(t => t.ShippingCharge);

            this.Property(t => t.AdditionalCost);

            this.Property(t => t.TotalAmount); 

            // Table & Column Mappings
            this.ToTable("QuotationSupplierList");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.QuotationID).HasColumnName("QuotationID");
            this.Property(t => t.SupplierID).HasColumnName("SupplierID");
            this.Property(t => t.IsReplied).HasColumnName("IsReplied");
            this.Property(t => t.QuotationReplyDate).HasColumnName("QuotationReplyDate");
            this.Property(t => t.Amount).HasColumnName("Amount");
            this.Property(t => t.GSTAmount).HasColumnName("GSTAmount");
            this.Property(t => t.ShippingCharge).HasColumnName("ShippingCharge");
            this.Property(t => t.AdditionalCost).HasColumnName("AdditionalCost");
            this.Property(t => t.TotalAmount).HasColumnName("TotalAmount");
        }
    }
}
