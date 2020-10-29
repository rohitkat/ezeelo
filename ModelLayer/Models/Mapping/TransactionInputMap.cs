using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace ModelLayer.Models.Mapping
{
    public class TransactionInputMap : EntityTypeConfiguration<TransactionInput>
    {
        public TransactionInputMap()
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
            this.ToTable("TransactionInput");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.CustomerOrderDetailID).HasColumnName("CustomerOrderDetailID");
            this.Property(t => t.ShopStockID).HasColumnName("ShopStockID");
            this.Property(t => t.ShopID).HasColumnName("ShopID");
            this.Property(t => t.ChannelPartnerID).HasColumnName("ChannelPartnerID");
            this.Property(t => t.MCOCustomerID).HasColumnName("MCOCustomerID");
            this.Property(t => t.MCOShopID).HasColumnName("MCOShopID");
            this.Property(t => t.MCODeliveryID).HasColumnName("MCODeliveryID");
            this.Property(t => t.GandhibaghID).HasColumnName("GandhibaghID");
            this.Property(t => t.Qty).HasColumnName("Qty");
            this.Property(t => t.MRPPerUnit).HasColumnName("MRPPerUnit");
            this.Property(t => t.SaleRatePerUnit).HasColumnName("SaleRatePerUnit");
            this.Property(t => t.OfferInPercentByShopPerUnit).HasColumnName("OfferInPercentByShopPerUnit");
            this.Property(t => t.OfferInRsByShopPerUnit).HasColumnName("OfferInRsByShopPerUnit");
            this.Property(t => t.IsInclusiveOfTAX).HasColumnName("IsInclusiveOfTAX");
            this.Property(t => t.TAXID).HasColumnName("TAXID");
            this.Property(t => t.ServiceTAX).HasColumnName("ServiceTAX");
            this.Property(t => t.IsShopHandleOtherTAX).HasColumnName("IsShopHandleOtherTAX");
            this.Property(t => t.SumOfOtherTAX).HasColumnName("SumOfOtherTAX");
            this.Property(t => t.LandingPriceByShopPerUnit).HasColumnName("LandingPriceByShopPerUnit");
            this.Property(t => t.ChargeInRsByGBPerUnit).HasColumnName("ChargeInRsByGBPerUnit");
            this.Property(t => t.ChargeINPercentByGBPerUnit).HasColumnName("ChargeINPercentByGBPerUnit");
            this.Property(t => t.CommisionInRsMCOCustomer).HasColumnName("CommisionInRsMCOCustomer");
            this.Property(t => t.CommisionInPercentMCOCustomer).HasColumnName("CommisionInPercentMCOCustomer");
            this.Property(t => t.CommisionInRsMCOShop).HasColumnName("CommisionInRsMCOShop");
            this.Property(t => t.CommisionInPercentMCOShop).HasColumnName("CommisionInPercentMCOShop");
            this.Property(t => t.CommisionInRsMCODelivery).HasColumnName("CommisionInRsMCODelivery");
            this.Property(t => t.CommisionInPercentMCODelivery).HasColumnName("CommisionInPercentMCODelivery");
            this.Property(t => t.CommisionInRsGB).HasColumnName("CommisionInRsGB");
            this.Property(t => t.CommisionInPercentGB).HasColumnName("CommisionInPercentGB");
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
