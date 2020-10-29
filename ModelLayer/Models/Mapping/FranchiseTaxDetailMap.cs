using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace ModelLayer.Models.Mapping
{
    public class FranchiseTaxDetailMap : EntityTypeConfiguration<FranchiseTaxDetail>
    {
        public FranchiseTaxDetailMap()
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
            this.ToTable("FranchiseTaxDetail");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.CityID).HasColumnName("CityID");
            this.Property(t => t.FranchiseID).HasColumnName("FranchiseID");
            this.Property(t => t.TaxationID).HasColumnName("TaxationID");           
            this.Property(t => t.InPercentage).HasColumnName("InPercentage");
            this.Property(t => t.InRupees).HasColumnName("InRupees");
            this.Property(t => t.IsDirect).HasColumnName("IsDirect");
            this.Property(t => t.IsCustomerSide).HasColumnName("IsCustomerSide");
            this.Property(t => t.IsOnTaxSum).HasColumnName("IsOnTaxSum");
            this.Property(t => t.IsMinusTaxs).HasColumnName("IsMinusTaxs");            
            this.Property(t => t.IsIncludeSaleRate).HasColumnName("IsIncludeSaleRate");
            this.Property(t => t.IsActive).HasColumnName("IsActive");
            this.Property(t => t.IsPercentage).HasColumnName("IsPercentage");
            this.Property(t => t.LowerLimit).HasColumnName("LowerLimit");
            this.Property(t => t.UpperLimit).HasColumnName("UpperLimit");
            this.Property(t => t.CreateDate).HasColumnName("CreateDate");
            this.Property(t => t.CreateBy).HasColumnName("CreateBy");
            this.Property(t => t.ModifyDate).HasColumnName("ModifyDate");
            this.Property(t => t.ModifyBy).HasColumnName("ModifyBy");
            this.Property(t => t.NetworkIP).HasColumnName("NetworkIP");
            this.Property(t => t.DeviceType).HasColumnName("DeviceType");
            this.Property(t => t.DeviceID).HasColumnName("DeviceID");

            // Relationships
            this.HasRequired(t => t.City)
                .WithMany(t => t.FranchiseTaxDetails)
                .HasForeignKey(d => d.CityID);
            this.HasRequired(t => t.Franchise)
                .WithMany(t => t.FranchiseTaxDetails)
                .HasForeignKey(d => d.FranchiseID);
            this.HasRequired(t => t.TaxationMaster)
                .WithMany(t => t.FranchiseTaxDetails)
                .HasForeignKey(d => d.TaxationID);

        }
    }
}
