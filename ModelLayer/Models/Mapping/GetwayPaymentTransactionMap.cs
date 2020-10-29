using System.ComponentModel.DataAnnotations;
using System.Data.Entity.ModelConfiguration;

namespace ModelLayer.Models.Mapping
{
    public class GetwayPaymentTransactionMap : EntityTypeConfiguration<GetwayPaymentTransaction>
    {
        public GetwayPaymentTransactionMap()
        {
            // Primary Key
            this.HasKey(t => t.ID);

            // Properties
            this.Property(t => t.PaymentMode)
                .HasMaxLength(50);

            this.Property(t => t.PaymentGetWayTransactionId)
                .HasMaxLength(100);

            this.Property(t => t.Description)
                .HasMaxLength(500);

            this.Property(t => t.NetworkIP)
                .HasMaxLength(15);

            this.Property(t => t.DeviceType)
                .HasMaxLength(50);

            this.Property(t => t.DeviceID)
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("GetwayPaymentTransaction");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.PaymentMode).HasColumnName("PaymentMode");
            this.Property(t => t.FromUID).HasColumnName("FromUID");
            this.Property(t => t.ToUID).HasColumnName("ToUID");
            this.Property(t => t.AccountTransactionId).HasColumnName("AccountTransactionId");
            this.Property(t => t.PaymentGetWayTransactionId).HasColumnName("PaymentGetWayTransactionId");
            this.Property(t => t.Status).HasColumnName("Status");
            this.Property(t => t.Description).HasColumnName("Description");
            this.Property(t => t.TransactionDate).HasColumnName("TransactionDate");
            this.Property(t => t.IsActive).HasColumnName("IsActive");
            this.Property(t => t.CreateDate).HasColumnName("CreateDate");
            this.Property(t => t.CreateBy).HasColumnName("CreateBy");
            this.Property(t => t.ModifyDate).HasColumnName("ModifyDate");
            this.Property(t => t.ModifyBy).HasColumnName("ModifyBy");
            this.Property(t => t.NetworkIP).HasColumnName("NetworkIP");
            this.Property(t => t.DeviceType).HasColumnName("DeviceType");
            this.Property(t => t.DeviceID).HasColumnName("DeviceID");

            // Relationships
            this.HasRequired(t => t.PersonalDetail)
                .WithMany(t => t.GetwayPaymentTransactions)
                .HasForeignKey(d => d.CreateBy);
            this.HasOptional(t => t.PersonalDetail1)
                .WithMany(t => t.GetwayPaymentTransactions1)
                .HasForeignKey(d => d.ModifyBy);

        }
    }
}
