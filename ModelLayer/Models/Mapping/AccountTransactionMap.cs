using System.ComponentModel.DataAnnotations;
using System.Data.Entity.ModelConfiguration;

namespace ModelLayer.Models.Mapping
{
    public class AccountTransactionMap : EntityTypeConfiguration<AccountTransaction>
    {
        public AccountTransactionMap()
        {
            // Primary Key
            this.HasKey(t => t.ID);

            // Properties
            this.Property(t => t.Narration)
                .HasMaxLength(150);

            this.Property(t => t.TotalAmountType)
                .HasMaxLength(2);

            this.Property(t => t.NetworkIP)
                .HasMaxLength(15);

            this.Property(t => t.DeviceType)
                .HasMaxLength(50);

            this.Property(t => t.DeviceID)
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("AccountTransaction");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.UserLoginID).HasColumnName("UserLoginID");
            this.Property(t => t.GandhibaghTransactionID).HasColumnName("GandhibaghTransactionID");
            this.Property(t => t.LedgerHeadID).HasColumnName("LedgerHeadID");
            this.Property(t => t.Narration).HasColumnName("Narration");
            this.Property(t => t.DebitAmount).HasColumnName("DebitAmount");
            this.Property(t => t.CreditAmount).HasColumnName("CreditAmount");
            this.Property(t => t.TotalAmount).HasColumnName("TotalAmount");
            this.Property(t => t.TotalAmountType).HasColumnName("TotalAmountType");
            this.Property(t => t.IsActive).HasColumnName("IsActive");
            this.Property(t => t.CreateDate).HasColumnName("CreateDate");
            this.Property(t => t.CreateBy).HasColumnName("CreateBy");
            this.Property(t => t.ModifyDate).HasColumnName("ModifyDate");
            this.Property(t => t.ModifyBy).HasColumnName("ModifyBy");
            this.Property(t => t.NetworkIP).HasColumnName("NetworkIP");
            this.Property(t => t.DeviceType).HasColumnName("DeviceType");
            this.Property(t => t.DeviceID).HasColumnName("DeviceID");

            // Relationships
            this.HasRequired(t => t.GandhibaghTransaction)
                .WithMany(t => t.AccountTransactions)
                .HasForeignKey(d => d.GandhibaghTransactionID);
            this.HasRequired(t => t.LedgerHead)
                .WithMany(t => t.AccountTransactions)
                .HasForeignKey(d => d.LedgerHeadID);
            this.HasRequired(t => t.UserLogin)
                .WithMany(t => t.AccountTransactions)
                .HasForeignKey(d => d.UserLoginID);
            this.HasRequired(t => t.PersonalDetail)
                .WithMany(t => t.AccountTransactions)
                .HasForeignKey(d => d.CreateBy);
            this.HasOptional(t => t.PersonalDetail1)
                .WithMany(t => t.AccountTransactions1)
                .HasForeignKey(d => d.ModifyBy);

        }
    }
}
