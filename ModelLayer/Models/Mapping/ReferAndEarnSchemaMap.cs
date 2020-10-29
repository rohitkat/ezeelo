using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace ModelLayer.Models.Mapping
{
    public class ReferAndEarnSchemaMap : EntityTypeConfiguration<ReferAndEarnSchema>
    {
        public ReferAndEarnSchemaMap()
        {
            // Primary Key
            this.HasKey(t => t.ID);

            // Properties
            this.Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.NetworkIP)
                .HasMaxLength(15);

            this.Property(t => t.DeviceType)
                .HasMaxLength(50);

            this.Property(t => t.DeviceID)
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("ReferAndEarnSchema");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.Name).HasColumnName("Name");
            this.Property(t => t.OrderwiseEarn).HasColumnName("OrderwiseEarn");
            this.Property(t => t.UserwiseEarn).HasColumnName("UserwiseEarn");
            this.Property(t => t.EarnInRS).HasColumnName("EarnInRS");
            this.Property(t => t.EarnInPercentage).HasColumnName("EarnInPercentage");
            this.Property(t => t.MaxNoOfOrders).HasColumnName("MaxNoOfOrders");
            this.Property(t => t.MaxPurchaseAmount).HasColumnName("MaxPurchaseAmount");
            this.Property(t => t.ExpirationDays).HasColumnName("ExpirationDays");
            this.Property(t => t.CityID).HasColumnName("CityID");
            this.Property(t => t.FranchiseID).HasColumnName("FranchiseID");
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
