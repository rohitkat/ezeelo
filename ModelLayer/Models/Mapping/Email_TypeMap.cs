using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace ModelLayer.Models.Mapping
{
    public class Email_TypeMap : EntityTypeConfiguration<Email_Type>
    {
        public Email_TypeMap()
        {
            // Primary Key
            this.HasKey(t => t.Mail_Type_ID);

            // Properties
            this.Property(t => t.Mail_Name)
                .HasMaxLength(200);

            // Table & Column Mappings
            this.ToTable("Email_Type");
            this.Property(t => t.Mail_Type_ID).HasColumnName("Mail_Type_ID");
            this.Property(t => t.Mail_Name).HasColumnName("Mail_Name");
            this.Property(t => t.Mail_Containt).HasColumnName("Mail_Containt");
        }
    }
}
