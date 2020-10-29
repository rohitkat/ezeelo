using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace ModelLayer.Models.Mapping
{
    public class TransactionInputProcessShopMap : EntityTypeConfiguration<TransactionInputProcessShop>
    {
        public TransactionInputProcessShopMap()
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
            this.ToTable("TransactionInputProcessShop");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.TransactionInputID).HasColumnName("TransactionInputID");
            this.Property(t => t.Qty).HasColumnName("Qty");
            this.Property(t => t.TotalMRP).HasColumnName("TotalMRP");
            this.Property(t => t.TotalSaleRate).HasColumnName("TotalSaleRate");
            this.Property(t => t.TotalOffer).HasColumnName("TotalOffer");
            this.Property(t => t.NewSaleRateAfterOffer).HasColumnName("NewSaleRateAfterOffer");
            this.Property(t => t.TotalShopFinalPrice).HasColumnName("TotalShopFinalPrice");
            this.Property(t => t.ShopReceivable).HasColumnName("ShopReceivable");
            this.Property(t => t.IsShopHandleOtherTAX).HasColumnName("IsShopHandleOtherTAX");
            this.Property(t => t.OtherTAXPayableReceivableFromMerchant).HasColumnName("OtherTAXPayableReceivableFromMerchant");
            this.Property(t => t.SumOfAmountShopReceivableAfterOtherTAX).HasColumnName("SumOfAmountShopReceivableAfterOtherTAX");
            this.Property(t => t.GBReceivableAmount).HasColumnName("GBReceivableAmount");
            this.Property(t => t.GBTransactionFee).HasColumnName("GBTransactionFee");
            this.Property(t => t.GBServiceTAXOnTransactionFee).HasColumnName("GBServiceTAXOnTransactionFee");
            this.Property(t => t.FinalShopReceivableAfterAllDone).HasColumnName("FinalShopReceivableAfterAllDone");
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
