using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace ModelLayer.Models.Mapping
{
    public class MerchantKYCMap : EntityTypeConfiguration<MerchantKYC>
    {
        public MerchantKYCMap()
        {
            // Primary Key
            this.HasKey(t => t.ID);

            // Table & Column Mappings
            this.ToTable("MerchantKYC");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.MerchantID).HasColumnName("MerchantID");
            
            this.Property(t => t.ShopEstablishmentCertificateImageUrl).HasColumnName("ShopEstablishmentCertificateImageUrl").HasMaxLength(200);
            this.Property(t => t.PanImageUrl).HasColumnName("PanImageUrl").HasMaxLength(200);

            this.Property(t => t.GSTRegistrationImageUrl).HasColumnName("GSTRegistrationImageUrl").HasMaxLength(200);
            this.Property(t => t.AddressProofUrl).HasColumnName("AddressProofUrl").HasMaxLength(200);
            this.Property(t => t.VisingCardImageUrl).HasColumnName("VisingCardImageUrl").HasMaxLength(200);
            this.Property(t => t.CancelledblankChequeImageUrl).HasColumnName("CancelledblankChequeImageUrl").HasMaxLength(200);
            this.Property(t => t.PhotoImageUrl).HasColumnName("PhotoImageURL").HasMaxLength(200);

            this.Property(t => t.IsVerified).HasColumnName("IsVerified");
            this.Property(t => t.IsCompleted).HasColumnName("IsCompleted");
            this.Property(t => t.CreateDate).HasColumnName("CreateDate");
            this.Property(t => t.CreateBy).HasColumnName("CreateBy");
            this.Property(t => t.ModifyDate).HasColumnName("ModifyDate");
            this.Property(t => t.ModifyBy).HasColumnName("ModifyBy");
        }
    }
}