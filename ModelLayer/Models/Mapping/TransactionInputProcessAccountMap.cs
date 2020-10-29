using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace ModelLayer.Models.Mapping
{
    public class TransactionInputProcessAccountMap : EntityTypeConfiguration<TransactionInputProcessAccount>
    {
        public TransactionInputProcessAccountMap()
        {
            // Primary Key
            this.HasKey(t => t.ID);

            // Properties
            this.Property(t => t.NetworkIP)
                .HasMaxLength(15);

            this.Property(t => t.DeviceType)
                .HasMaxLength(50);

            this.Property(t => t.DeviceID)
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("TransactionInputProcessAccount");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.LeadgerHeadID).HasColumnName("LeadgerHeadID");
            this.Property(t => t.ReceivedPaymentModeID).HasColumnName("ReceivedPaymentModeID");
            this.Property(t => t.TransactionInputID).HasColumnName("TransactionInputID");
            this.Property(t => t.CustomerOrderID).HasColumnName("CustomerOrderID");
            this.Property(t => t.Amount).HasColumnName("Amount");
            this.Property(t => t.ReceivedFromUserLoginID).HasColumnName("ReceivedFromUserLoginID");
            this.Property(t => t.PODReceived).HasColumnName("PODReceived");
            this.Property(t => t.Narration).HasColumnName("Narration");
            this.Property(t => t.CreateDate).HasColumnName("CreateDate");
            this.Property(t => t.CreateBy).HasColumnName("CreateBy");
            this.Property(t => t.ModifyDate).HasColumnName("ModifyDate");
            this.Property(t => t.ModifyBy).HasColumnName("ModifyBy");
            this.Property(t => t.NetworkIP).HasColumnName("NetworkIP");
            this.Property(t => t.DeviceType).HasColumnName("DeviceType");
            this.Property(t => t.DeviceID).HasColumnName("DeviceID");

            // Relationships
            this.HasRequired(t => t.LedgerHead)
                .WithMany(t => t.TransactionInputProcessAccounts)
                .HasForeignKey(d => d.LeadgerHeadID);
            this.HasRequired(t => t.PaymentMode)
                .WithMany(t => t.TransactionInputProcessAccounts)
                .HasForeignKey(d => d.ReceivedPaymentModeID);
            this.HasRequired(t => t.PersonalDetail)
                .WithMany(t => t.TransactionInputProcessAccounts)
                .HasForeignKey(d => d.CreateBy);
            this.HasOptional(t => t.PersonalDetail1)
                .WithMany(t => t.TransactionInputProcessAccounts1)
                .HasForeignKey(d => d.ModifyBy);
            this.HasOptional(t => t.TransactionInput)
                .WithMany(t => t.TransactionInputProcessAccounts)
                .HasForeignKey(d => d.TransactionInputID);
            this.HasRequired(t => t.UserLogin)
                .WithMany(t => t.TransactionInputProcessAccounts)
                .HasForeignKey(d => d.ReceivedFromUserLoginID);

        }
    }
}
