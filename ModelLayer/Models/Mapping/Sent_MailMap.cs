using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace ModelLayer.Models.Mapping
{
    public class Sent_MailMap : EntityTypeConfiguration<Sent_Mail>
    {
        public Sent_MailMap()
        {
            // Primary Key
            this.HasKey(t => t.Sent_Mail_ID);

            // Properties
            this.Property(t => t.Email_ID)
                .HasMaxLength(150);

            this.Property(t => t.Mobile)
                .HasMaxLength(10);

            this.Property(t => t.Remarks)
                .HasMaxLength(300);

            // Table & Column Mappings
            this.ToTable("Sent_Mail");
            this.Property(t => t.Sent_Mail_ID).HasColumnName("Sent_Mail_ID");
            this.Property(t => t.Mail_Type_ID).HasColumnName("Mail_Type_ID");
            this.Property(t => t.Login_ID).HasColumnName("Login_ID");
            this.Property(t => t.Email_ID).HasColumnName("Email_ID");
            this.Property(t => t.Mobile).HasColumnName("Mobile");
            this.Property(t => t.IsOffer).HasColumnName("IsOffer");
            this.Property(t => t.OfferID).HasColumnName("OfferID");
            this.Property(t => t.RemainingDays).HasColumnName("RemainingDays");
            this.Property(t => t.Sent_Date).HasColumnName("Sent_Date");
            this.Property(t => t.Remarks).HasColumnName("Remarks");

            // Relationships
            this.HasRequired(t => t.Email_Type)
                .WithMany(t => t.Sent_Mail)
                .HasForeignKey(d => d.Mail_Type_ID);

        }
    }
}
