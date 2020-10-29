using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace ModelLayer.Models.Mapping
{
    public class MerchantMap : EntityTypeConfiguration<Merchant>
    {
        public MerchantMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Table & Column Mappings
            this.ToTable("Merchant");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.LeaderContactNo).HasColumnName("LeaderContactNo").HasMaxLength(10);
            this.Property(t => t.FranchiseName).HasColumnName("FranchiseName").HasMaxLength(200);
            this.Property(t => t.GSTINNo).HasColumnName("GSTINNo").HasMaxLength(50);
            this.Property(t => t.PANNo).HasColumnName("PANNo").HasMaxLength(50);
            this.Property(t => t.Address).HasColumnName("Address").HasMaxLength(150);
            this.Property(t => t.City).HasColumnName("City");
            this.Property(t => t.District).HasColumnName("District");
            this.Property(t => t.State).HasColumnName("State");
            this.Property(t => t.Country).HasColumnName("Country").HasMaxLength(150);
            this.Property(t => t.Pincode).HasColumnName("Pincode");
            this.Property(t => t.ShopTiming).HasColumnName("ShopTiming");
            this.Property(t => t.Status).HasColumnName("Status").HasMaxLength(50);
            this.Property(t => t.ContactPerson).HasColumnName("ContactPerson").HasMaxLength(150);
            this.Property(t => t.ContactNumber).HasColumnName("ContactNumber").HasMaxLength(10);
            this.Property(t => t.Email).HasColumnName("Email").HasMaxLength(150);
            this.Property(t => t.ValidityPeriod).HasColumnName("ValidityPeriod");
            this.Property(t => t.Category).HasColumnName("Category");
            this.Property(t => t.Comission).HasColumnName("Comission");
            this.Property(t => t.GoogleMapLink).HasColumnName("GoogleMapLink");
            this.Property(t => t.SpecialRemark).HasColumnName("SpecialRemark").HasMaxLength(15000);
            this.Property(t => t.TermCondition).HasColumnName("TermCondition");
            this.Property(t => t.CreateDate).HasColumnName("CreateDate");
            this.Property(t => t.ModifyDate).HasColumnName("ModifyDate");
            this.Property(t => t.NetworkIP).HasColumnName("NetworkIP").HasMaxLength(15);
            this.Property(t => t.DeviceType).HasColumnName("DeviceType").HasMaxLength(50);
            this.Property(t => t.DeviceID).HasColumnName("DeviceID").HasMaxLength(50);
            this.Property(t => t.ApproveDate).HasColumnName("ApproveDate");
            this.Property(t => t.AcceptDate).HasColumnName("AcceptDate");

            // Relationships
            this.HasRequired(t => t.CityDetail)
                .WithMany(t => t.Merchant)
                .HasForeignKey(d => d.City);
            this.HasRequired(t => t.StateDetail)
               .WithMany(t => t.Merchant)
               .HasForeignKey(d => d.State);           
            this.HasRequired(t => t.ShopTimingMasterDetail)
               .WithMany(t => t.Merchant)
               .HasForeignKey(d => d.ShopTiming);
            this.HasRequired(t => t.ServiceMasterDetail)
               .WithMany(t => t.Merchant)
               .HasForeignKey(d => d.Category);
            this.HasRequired(t => t.CommissionMasterDetail)
               .WithMany(t => t.Merchant)
               .HasForeignKey(d => d.Comission);
        }
    }
}