using System.ComponentModel.DataAnnotations;
using System.Data.Entity.ModelConfiguration;

namespace ModelLayer.Models.Mapping
{
   public class WarehouseFinancialTransactionMap: EntityTypeConfiguration<WarehouseFinancialTransaction>
    {
       public WarehouseFinancialTransactionMap()
        {
            this.HasKey(t => t.ID);

            // Properties
            this.Property(t => t.WarehouseID);

            this.Property(t => t.SupplierID);

            this.Property(t => t.AccountNumber).HasMaxLength(128);

            this.Property(t => t.InvoiceID);

            this.Property(t => t.ReceiptNumber).HasMaxLength(128);

            this.Property(t => t.TransactionTypeID);

            this.Property(t => t.TransactionAmount);

            this.Property(t => t.TransactionDate);

            this.Property(t => t.PaymentMode);                  

            this.Property(t => t.Remark).HasMaxLength(4000);

            this.Property(t => t.IsActive);

            

            // Table & Column Mappings
            this.ToTable("WarehouseFinancialTransaction");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.WarehouseID).HasColumnName("WarehouseID");
            this.Property(t => t.SupplierID).HasColumnName("SupplierID");
            this.Property(t => t.AccountNumber).HasColumnName("AccountNumber");
            this.Property(t => t.InvoiceID).HasColumnName("InvoiceID");
            this.Property(t => t.ReceiptNumber).HasColumnName("ReceiptNumber");
            this.Property(t => t.TransactionTypeID).HasColumnName("TransactionTypeID");
            this.Property(t => t.TransactionAmount).HasColumnName("TransactionAmount");
            this.Property(t => t.TransactionDate).HasColumnName("TransactionDate");
            this.Property(t => t.PaymentMode).HasColumnName("PaymentMode");           
            this.Property(t => t.Remark).HasColumnName("Remark");
            this.Property(t => t.IsActive).HasColumnName("IsActive");
           
           
        }
    }
}
