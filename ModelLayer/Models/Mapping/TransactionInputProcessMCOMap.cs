using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace ModelLayer.Models.Mapping
{
    public class TransactionInputProcessMCOMap : EntityTypeConfiguration<TransactionInputProcessMCO>
    {
        public TransactionInputProcessMCOMap()
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
            this.ToTable("TransactionInputProcessMCO");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.TransactionInputProcessShopID).HasColumnName("TransactionInputProcessShopID");
            this.Property(t => t.Qty).HasColumnName("Qty");
            this.Property(t => t.TotalMRP).HasColumnName("TotalMRP");
            this.Property(t => t.TotalSaleRate).HasColumnName("TotalSaleRate");
            this.Property(t => t.MCOCustomerReceivable).HasColumnName("MCOCustomerReceivable");
            this.Property(t => t.MCOShopReceivable).HasColumnName("MCOShopReceivable");
            this.Property(t => t.MCODeliveryReceivable).HasColumnName("MCODeliveryReceivable");
            this.Property(t => t.GBReceivable).HasColumnName("GBReceivable");
            this.Property(t => t.AmountRemaining).HasColumnName("AmountRemaining");
            this.Property(t => t.IsActive).HasColumnName("IsActive");
            this.Property(t => t.CreateDate).HasColumnName("CreateDate");
            this.Property(t => t.CreateBy).HasColumnName("CreateBy");
            this.Property(t => t.ModifyDate).HasColumnName("ModifyDate");
            this.Property(t => t.ModifyBy).HasColumnName("ModifyBy");
            this.Property(t => t.NetworkIP).HasColumnName("NetworkIP");
            this.Property(t => t.DeviceType).HasColumnName("DeviceType");
            this.Property(t => t.DeviceID).HasColumnName("DeviceID");
        }
    }
}
