using System.ComponentModel.DataAnnotations;
using System.Data.Entity.ModelConfiguration;

namespace ModelLayer.Models.Mapping
{
    public class GandhibaghTransactionMap : EntityTypeConfiguration<GandhibaghTransaction>
    {
        public GandhibaghTransactionMap()
        {
            // Primary Key
            this.HasKey(t => t.ID);

            // Properties
            this.Property(t => t.ChargeType)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.Particular)
                .HasMaxLength(100);

            this.Property(t => t.NetworkIP)
                .HasMaxLength(15);

            this.Property(t => t.DeviceType)
                .HasMaxLength(50);

            this.Property(t => t.DeviceID)
                .HasMaxLength(50);

            this.Property(t => t.Remark)
                .HasMaxLength(100);

            // Table & Column Mappings
            this.ToTable("GandhibaghTransaction");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.ChargeID).HasColumnName("ChargeID");
            this.Property(t => t.ChargeType).HasColumnName("ChargeType");
            this.Property(t => t.FromBusinessTypeID).HasColumnName("FromBusinessTypeID");
            this.Property(t => t.FromPersonalDetailId).HasColumnName("FromPersonalDetailId");
            this.Property(t => t.ToBusinessTypeID).HasColumnName("ToBusinessTypeID");
            this.Property(t => t.ToPersonalDetailID).HasColumnName("ToPersonalDetailID");
            this.Property(t => t.ApplicablePercent).HasColumnName("ApplicablePercent");
            this.Property(t => t.ApplicableRupee).HasColumnName("ApplicableRupee");
            this.Property(t => t.Particular).HasColumnName("Particular");
            this.Property(t => t.CustomerOrderDetailID).HasColumnName("CustomerOrderDetailID");
            this.Property(t => t.Amount).HasColumnName("Amount");
            this.Property(t => t.TransactionAmount).HasColumnName("TransactionAmount");
            this.Property(t => t.IsActive).HasColumnName("IsActive");
            this.Property(t => t.CreateDate).HasColumnName("CreateDate");
            this.Property(t => t.CreateBy).HasColumnName("CreateBy");
            this.Property(t => t.ModifyDate).HasColumnName("ModifyDate");
            this.Property(t => t.ModifyBy).HasColumnName("ModifyBy");
            this.Property(t => t.NetworkIP).HasColumnName("NetworkIP");
            this.Property(t => t.DeviceType).HasColumnName("DeviceType");
            this.Property(t => t.DeviceID).HasColumnName("DeviceID");
            this.Property(t => t.Remark).HasColumnName("Remark");


            // Relationships
            this.HasOptional(t => t.BusinessType)
                .WithMany(t => t.GandhibaghTransactions)
                .HasForeignKey(d => d.ToBusinessTypeID);
            this.HasOptional(t => t.BusinessType1)
                .WithMany(t => t.GandhibaghTransactions1)
                .HasForeignKey(d => d.FromBusinessTypeID);
            this.HasOptional(t => t.BusinessType2)
                .WithMany(t => t.GandhibaghTransactions2)
                .HasForeignKey(d => d.ToBusinessTypeID);
            this.HasRequired(t => t.Charge)
                .WithMany(t => t.GandhibaghTransactions)
                .HasForeignKey(d => d.ChargeID);
            this.HasOptional(t => t.CustomerOrderDetail)
                .WithMany(t => t.GandhibaghTransactions)
                .HasForeignKey(d => d.CustomerOrderDetailID);
            this.HasRequired(t => t.PersonalDetail)
                .WithMany(t => t.GandhibaghTransactions)
                .HasForeignKey(d => d.CreateBy);
            this.HasOptional(t => t.PersonalDetail1)
                .WithMany(t => t.GandhibaghTransactions1)
                .HasForeignKey(d => d.FromPersonalDetailId);
            this.HasOptional(t => t.PersonalDetail2)
                .WithMany(t => t.GandhibaghTransactions2)
                .HasForeignKey(d => d.ToPersonalDetailID);
            this.HasOptional(t => t.PersonalDetail3)
                .WithMany(t => t.GandhibaghTransactions3)
                .HasForeignKey(d => d.ModifyBy);

        }
    }
}
