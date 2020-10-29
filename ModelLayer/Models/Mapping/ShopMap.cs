using System.ComponentModel.DataAnnotations;
using System.Data.Entity.ModelConfiguration;

namespace ModelLayer.Models.Mapping
{
    public class ShopMap : EntityTypeConfiguration<Shop>
    {
        public ShopMap()
        {
            // Primary Key
            this.HasKey(t => t.ID);

            // Properties
            this.Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(150);

            this.Property(t => t.Website)
                .HasMaxLength(150);

            this.Property(t => t.Lattitude)
                .HasMaxLength(15);

            this.Property(t => t.Longitude)
                .HasMaxLength(15);

            this.Property(t => t.Address)
                .HasMaxLength(500);

            this.Property(t => t.NearestLandmark)
                .HasMaxLength(150);

            this.Property(t => t.ContactPerson)
                .IsRequired()
                .HasMaxLength(150);

            this.Property(t => t.Email)
                .IsRequired()
                .HasMaxLength(150);

            this.Property(t => t.Mobile)
                .IsRequired()
                .HasMaxLength(10);

            this.Property(t => t.Landline)
                .HasMaxLength(13);

            this.Property(t => t.FAX)
                .HasMaxLength(15);

            this.Property(t => t.VAT)
                .HasMaxLength(15);

            this.Property(t => t.TIN)
                .HasMaxLength(15);

            this.Property(t => t.PAN)
                .HasMaxLength(10);

            this.Property(t => t.WeeklyOff)
                .HasMaxLength(50);

            this.Property(t => t.SearchKeywords)
                .IsRequired()
                .HasMaxLength(500);
            
            this.Property(t => t.Description)
                .HasMaxLength(1000);

            this.Property(t => t.NetworkIP)
                .HasMaxLength(15);

            this.Property(t => t.DeviceType)
                .HasMaxLength(50);

            this.Property(t => t.DeviceID)
                .HasMaxLength(50);
       
            // Table & Column Mappings
            this.ToTable("Shop");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.BusinessDetailID).HasColumnName("BusinessDetailID");
            this.Property(t => t.Name).HasColumnName("Name");
            this.Property(t => t.Website).HasColumnName("Website");
            this.Property(t => t.Lattitude).HasColumnName("Lattitude");
            this.Property(t => t.Longitude).HasColumnName("Longitude");
            this.Property(t => t.Address).HasColumnName("Address");
            this.Property(t => t.NearestLandmark).HasColumnName("NearestLandmark");
            this.Property(t => t.PincodeID).HasColumnName("PincodeID");
            this.Property(t => t.AreaID).HasColumnName("AreaID");
            this.Property(t => t.OpeningTime).HasColumnName("OpeningTime");
            this.Property(t => t.ClosingTime).HasColumnName("ClosingTime");
            this.Property(t => t.ContactPerson).HasColumnName("ContactPerson");
            this.Property(t => t.Email).HasColumnName("Email");
            this.Property(t => t.Mobile).HasColumnName("Mobile");
            this.Property(t => t.Landline).HasColumnName("Landline");
            this.Property(t => t.FAX).HasColumnName("FAX");
            this.Property(t => t.VAT).HasColumnName("VAT");
            this.Property(t => t.TIN).HasColumnName("TIN");
            this.Property(t => t.PAN).HasColumnName("PAN");
            this.Property(t => t.WeeklyOff).HasColumnName("WeeklyOff");
            this.Property(t => t.CurrentItSetup).HasColumnName("CurrentItSetup");
            this.Property(t => t.InstitutionalMerchantPurchase).HasColumnName("InstitutionalMerchantPurchase");
            this.Property(t => t.InstitutionalMerchantSale).HasColumnName("InstitutionalMerchantSale");
            this.Property(t => t.NormalSale).HasColumnName("NormalSale");
            this.Property(t => t.IsDeliveryOutSource).HasColumnName("IsDeliveryOutSource");
            this.Property(t => t.IsFreeHomeDelivery).HasColumnName("IsFreeHomeDelivery");
            this.Property(t => t.MinimumAmountForFreeDelivery).HasColumnName("MinimumAmountForFreeDelivery");
            this.Property(t => t.DeliveryPartnerId).HasColumnName("DeliveryPartnerId");
            this.Property(t => t.FranchiseID).HasColumnName("FranchiseID");
            this.Property(t => t.IsLive).HasColumnName("IsLive");
            this.Property(t => t.IsManageInventory).HasColumnName("IsManageInventory");
            this.Property(t => t.SearchKeywords).HasColumnName("SearchKeywords");
            this.Property(t => t.IsAgreedOnReturnProduct).HasColumnName("IsAgreedOnReturnProduct");
            this.Property(t => t.ReturnDurationInDays).HasColumnName("ReturnDurationInDays");
            this.Property(t => t.Description).HasColumnName("Description");
            this.Property(t => t.IsActive).HasColumnName("IsActive");
            /*WelComeLetter*/
            this.Property(t => t.LetterDate).HasColumnName("LetterDate");
            this.Property(t => t.SendBy).HasColumnName("SendBy");
            /**/
            this.Property(t => t.CreateDate).HasColumnName("CreateDate");
            this.Property(t => t.CreateBy).HasColumnName("CreateBy");
            this.Property(t => t.ModifyDate).HasColumnName("ModifyDate");
            this.Property(t => t.ModifyBy).HasColumnName("ModifyBy");
            this.Property(t => t.NetworkIP).HasColumnName("NetworkIP");
            this.Property(t => t.DeviceType).HasColumnName("DeviceType");
            this.Property(t => t.DeviceID).HasColumnName("DeviceID");

            // Relationships
            this.HasRequired(t => t.Area)
                .WithMany(t => t.Shops)
                .HasForeignKey(d => d.AreaID);
            this.HasRequired(t => t.BusinessDetail)
                .WithMany(t => t.Shops)
                .HasForeignKey(d => d.BusinessDetailID);
            this.HasOptional(t => t.DeliveryPartner)
                .WithMany(t => t.Shops)
                .HasForeignKey(d => d.DeliveryPartnerId);
            this.HasOptional(t => t.Franchise)
                .WithMany(t => t.Shops)
                .HasForeignKey(d => d.FranchiseID);
            this.HasRequired(t => t.PersonalDetail)
                .WithMany(t => t.Shops)
                .HasForeignKey(d => d.CreateBy);
            this.HasOptional(t => t.PersonalDetail1)
                .WithMany(t => t.Shops1)
                .HasForeignKey(d => d.ModifyBy);
            this.HasRequired(t => t.Pincode)
                .WithMany(t => t.Shops)
                .HasForeignKey(d => d.PincodeID);
            /*WelComeLetter*/
            this.HasRequired(t => t.PersonalDetail2)
                .WithMany(t => t.Shops2)
                .HasForeignKey(d => d.SendBy);
            /**/
        }
    }
}
