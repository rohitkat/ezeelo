using System.ComponentModel.DataAnnotations;
using System.Data.Entity.ModelConfiguration;

namespace ModelLayer.Models.Mapping
{
   
    public class InvoiceAttachmentMap : EntityTypeConfiguration<InvoiceAttachment>
    {
        public InvoiceAttachmentMap()
        {
            this.HasKey(t => t.ID);

            // Properties
            this.Property(t => t.InvoiceID);

            this.Property(t => t.FileName).HasMaxLength(4000);

            this.Property(t => t.Extention).HasMaxLength(10);           

            this.Property(t => t.IsActive);
            
            // Table & Column Mappings
            this.ToTable("InvoiceAttachment");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.InvoiceID).HasColumnName("InvoiceID");
            this.Property(t => t.FileName).HasColumnName("FileName");
            this.Property(t => t.Extention).HasColumnName("Extention");        
            this.Property(t => t.IsActive).HasColumnName("IsActive");            

        }
    }
}
