using System.ComponentModel.DataAnnotations;
using System.Data.Entity.ModelConfiguration;

namespace ModelLayer.Models.Mapping
{
    public class InvoiceDetailMap : EntityTypeConfiguration<InvoiceDetail>
    {
      public InvoiceDetailMap()
        {
            this.HasKey(t => t.ID);

            // Properties
            this.Property(t => t.InvoiceID);

            this.Property(t => t.PurchaseOrderDetailID);

            this.Property(t => t.ProductID);

            this.Property(t => t.ProductVarientID);

            this.Property(t => t.IsExtraItem);

            this.Property(t => t.BuyRatePerUnit);

            this.Property(t => t.ReceivedQuantity);

            this.Property(t => t.CGSTAmount);

            this.Property(t => t.SGSTAmount);

            this.Property(t => t.IGSTAmount); 

            this.Property(t => t.Amount);                  

            this.Property(t => t.Remark).HasMaxLength(4000);

            this.Property(t => t.IsActive);

            

            // Table & Column Mappings
            this.ToTable("InvoiceDetail");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.InvoiceID).HasColumnName("InvoiceID");
            this.Property(t => t.PurchaseOrderDetailID).HasColumnName("PurchaseOrderDetailID");
            this.Property(t => t.ProductID).HasColumnName("ProductID");
            this.Property(t => t.ProductVarientID).HasColumnName("ProductVarientID");
            this.Property(t => t.IsExtraItem).HasColumnName("IsExtraItem");
            this.Property(t => t.BuyRatePerUnit).HasColumnName("BuyRatePerUnit");
            this.Property(t => t.ReceivedQuantity).HasColumnName("ReceivedQuantity");
            this.Property(t => t.CGSTAmount).HasColumnName("CGSTAmount");
            this.Property(t => t.SGSTAmount).HasColumnName("SGSTAmount");
            this.Property(t => t.IGSTAmount).HasColumnName("IGSTAmount");
            this.Property(t => t.Amount).HasColumnName("Amount"); 
            this.Property(t => t.Remark).HasColumnName("Remark");
            this.Property(t => t.IsActive).HasColumnName("IsActive");
           
           
        }
    }
}
