using System.ComponentModel.DataAnnotations;
using System.Data.Entity.ModelConfiguration;

namespace ModelLayer.Models.Mapping
{
    public class OwnerBankMap : EntityTypeConfiguration<OwnerBank>
    {
        public OwnerBankMap()
        {
            // Primary Key
            this.HasKey(t => t.ID);

            // Properties
            this.Property(t => t.BranchName)
                .IsRequired()
                .HasMaxLength(150);

            this.Property(t => t.IFSCCode)
                .IsRequired()
                .HasMaxLength(11);

            this.Property(t => t.AccountNumber)
               .IsRequired()
               .HasMaxLength(20);

            this.Property(t => t.MICRCode)
                .HasMaxLength(50);

            this.Property(t => t.NetworkIP)
                .HasMaxLength(15);

            this.Property(t => t.DeviceType)
                .HasMaxLength(50);

            this.Property(t => t.DeviceID)
                .HasMaxLength(50);
            this.Property(t => t.AccountName)
               .HasMaxLength(500);

            // Table & Column Mappings
            this.ToTable("OwnerBank");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.BusinessTypeID).HasColumnName("BusinessTypeID");
            this.Property(t => t.OwnerID).HasColumnName("OwnerID");
            this.Property(t => t.BankID).HasColumnName("BankID");
            this.Property(t => t.BranchName).HasColumnName("BranchName");
            this.Property(t => t.IFSCCode).HasColumnName("IFSCCode");
            this.Property(t => t.MICRCode).HasColumnName("MICRCode");
            this.Property(t => t.AccountNumber).HasColumnName("AccountNumber");
            this.Property(t => t.BankAccountTypeID).HasColumnName("BankAccountTypeID");
            this.Property(t => t.IsActive).HasColumnName("IsActive");
            this.Property(t => t.CreateDate).HasColumnName("CreateDate");
            this.Property(t => t.CreateBy).HasColumnName("CreateBy");
            this.Property(t => t.ModifyDate).HasColumnName("ModifyDate");
            this.Property(t => t.ModifyBy).HasColumnName("ModifyBy");
            this.Property(t => t.NetworkIP).HasColumnName("NetworkIP");
            this.Property(t => t.DeviceType).HasColumnName("DeviceType");
            this.Property(t => t.DeviceID).HasColumnName("DeviceID");
            this.Property(t => t.AccountName).HasColumnName("AccountName");

            // Relationships
            this.HasRequired(t => t.Bank)
                .WithMany(t => t.OwnerBanks)
                .HasForeignKey(d => d.BankID);
            this.HasRequired(t => t.BankAccountType)
                .WithMany(t => t.OwnerBanks)
                .HasForeignKey(d => d.BankAccountTypeID);
            this.HasRequired(t => t.BusinessType)
                .WithMany(t => t.OwnerBanks)
                .HasForeignKey(d => d.BusinessTypeID);
            this.HasRequired(t => t.PersonalDetail)
                .WithMany(t => t.OwnerBanks)
                .HasForeignKey(d => d.CreateBy);
            this.HasOptional(t => t.PersonalDetail1)
                .WithMany(t => t.OwnerBanks1)
                .HasForeignKey(d => d.ModifyBy);

        }
    }
}
