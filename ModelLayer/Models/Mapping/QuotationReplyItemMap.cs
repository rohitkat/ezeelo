using System.ComponentModel.DataAnnotations;
using System.Data.Entity.ModelConfiguration;

namespace ModelLayer.Models.Mapping
{
  public class QuotationReplyItemMap: EntityTypeConfiguration<QuotationReplyItem>
    {
      public QuotationReplyItemMap()
        {
            this.HasKey(t => t.ID);
            // Properties
            this.Property(t => t.QuotationSupplierListID);

            this.Property(t => t.QuotationID);

            this.Property(t => t.ReplyFromSupplierID);

            this.Property(t => t.ReplyFromWarehouseID);

            this.Property(t => t.ProductID);

            this.Property(t => t.ProductNickname);
          
            this.Property(t => t.ProductVarientID);
          
            this.Property(t => t.Quantity);

            this.Property(t => t.UnitPrice);

            this.Property(t => t.CGSTAmount);

            this.Property(t => t.SGSTAmount);

            this.Property(t => t.IGSTAmount);

            this.Property(t => t.Amount);

            this.Property(t => t.Remark);

            // Table & Column Mappings
            this.ToTable("QuotationReplyItem");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.QuotationSupplierListID).HasColumnName("QuotationSupplierListID");
            this.Property(t => t.QuotationID).HasColumnName("QuotationID");
            this.Property(t => t.ReplyFromSupplierID).HasColumnName("ReplyFromSupplierID");
            this.Property(t => t.ReplyFromWarehouseID).HasColumnName("ReplyFromWarehouseID");
            this.Property(t => t.ProductID).HasColumnName("ProductID");
            this.Property(t => t.ProductNickname).HasColumnName("ProductNickname");
            this.Property(t => t.ProductVarientID).HasColumnName("ProductVarientID");           
            this.Property(t => t.Quantity).HasColumnName("Quantity");
            this.Property(t => t.UnitPrice).HasColumnName("UnitPrice");
            this.Property(t => t.CGSTAmount).HasColumnName("CGSTAmount");
            this.Property(t => t.SGSTAmount).HasColumnName("SGSTAmount");
            this.Property(t => t.IGSTAmount).HasColumnName("IGSTAmount");
            this.Property(t => t.Amount).HasColumnName("Amount");
            this.Property(t => t.Remark).HasColumnName("Remark");
       }
    }
}
