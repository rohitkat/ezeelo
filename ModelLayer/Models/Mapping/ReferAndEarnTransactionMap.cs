using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace ModelLayer.Models.Mapping
{
    public class ReferAndEarnTransactionMap : EntityTypeConfiguration<ReferAndEarnTransaction>
    {
        public ReferAndEarnTransactionMap()
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
            this.ToTable("ReferAndEarnTransaction");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.CustomerOrderID).HasColumnName("CustomerOrderID");
            this.Property(t => t.ReferDetailID).HasColumnName("ReferDetailID");
            this.Property(t => t.TransactionID).HasColumnName("TransactionID");
            this.Property(t => t.CreateDate).HasColumnName("CreateDate");
            this.Property(t => t.CreateBy).HasColumnName("CreateBy");
            this.Property(t => t.ModifyDate).HasColumnName("ModifyDate");
            this.Property(t => t.ModifyBy).HasColumnName("ModifyBy");
            this.Property(t => t.NetworkIP).HasColumnName("NetworkIP");
            this.Property(t => t.DeviceType).HasColumnName("DeviceType");
            this.Property(t => t.DeviceID).HasColumnName("DeviceID");

            // Relationships
            this.HasRequired(t => t.CustomerOrder)
                .WithMany(t => t.ReferAndEarnTransactions)
                .HasForeignKey(d => d.CustomerOrderID);
            this.HasRequired(t => t.ReferDetail)
                .WithMany(t => t.ReferAndEarnTransactions)
                .HasForeignKey(d => d.ReferDetailID);

        }
    }
}
