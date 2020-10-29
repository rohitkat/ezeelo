using System.ComponentModel.DataAnnotations;
using System.Data.Entity.ModelConfiguration;

namespace ModelLayer.Models.Mapping
{
    public class CorporateOrderShippingFacilityDetailMap : EntityTypeConfiguration<CorporateOrderShippingFacilityDetail>
    {
        public CorporateOrderShippingFacilityDetailMap()
        {
            // Primary Key
            this.HasKey(t => t.ID);

            // Properties
            //this.Property(t => t.ID)
            //    .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.NetworkIP)
                .HasMaxLength(15);

            this.Property(t => t.DeviceType)
                .HasMaxLength(50);

            this.Property(t => t.DeviceID)
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("CorporateOrderShippingFacilityDetails");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.CustomerOrderDetailID).HasColumnName("CustomerOrderDetailID");
            this.Property(t => t.ShippingFacilityCharges).HasColumnName("ShippingFacilityCharges");
            this.Property(t => t.IsActive).HasColumnName("IsActive");
            this.Property(t => t.CreateDate).HasColumnName("CreateDate");
            this.Property(t => t.CreateBy).HasColumnName("CreateBy");
            this.Property(t => t.ModifyDate).HasColumnName("ModifyDate");
            this.Property(t => t.ModifyBy).HasColumnName("ModifyBy");
            this.Property(t => t.NetworkIP).HasColumnName("NetworkIP");
            this.Property(t => t.DeviceType).HasColumnName("DeviceType");
            this.Property(t => t.DeviceID).HasColumnName("DeviceID");
            this.Property(t => t.CorporateshippingFacilityID).HasColumnName("CorporateshippingFacilityID");

            // Relationships
            this.HasRequired(t => t.CorporateShippingFacility)
                .WithMany(t => t.CorporateOrderShippingFacilityDetails)
                .HasForeignKey(d => d.CorporateshippingFacilityID);
            this.HasRequired(t => t.CustomerOrderDetail)
                .WithMany(t => t.CorporateOrderShippingFacilityDetails)
                .HasForeignKey(d => d.CustomerOrderDetailID);

        }
    }
}
