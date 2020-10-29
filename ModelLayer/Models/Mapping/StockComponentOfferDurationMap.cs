using System.ComponentModel.DataAnnotations;
using System.Data.Entity.ModelConfiguration;

namespace ModelLayer.Models.Mapping
{
    public class StockComponentOfferDurationMap : EntityTypeConfiguration<StockComponentOfferDuration>
    {
        public StockComponentOfferDurationMap()
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
            this.ToTable("StockComponentOfferDuration");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.ComponentOfferID).HasColumnName("ComponentOfferID");
            this.Property(t => t.StartDateTime).HasColumnName("StartDateTime");
            this.Property(t => t.EndDateTime).HasColumnName("EndDateTime");
            this.Property(t => t.IsActive).HasColumnName("IsActive");
            this.Property(t => t.CreateDate).HasColumnName("CreateDate");
            this.Property(t => t.CreateBy).HasColumnName("CreateBy");
            this.Property(t => t.ModifyDate).HasColumnName("ModifyDate");
            this.Property(t => t.ModifyBy).HasColumnName("ModifyBy");
            this.Property(t => t.NetworkIP).HasColumnName("NetworkIP");
            this.Property(t => t.DeviceType).HasColumnName("DeviceType");
            this.Property(t => t.DeviceID).HasColumnName("DeviceID");

            // Relationships
            this.HasRequired(t => t.ComponentOffer)
                .WithMany(t => t.StockComponentOfferDurations)
                .HasForeignKey(d => d.ComponentOfferID);
            this.HasRequired(t => t.PersonalDetail)
                .WithMany(t => t.StockComponentOfferDurations)
                .HasForeignKey(d => d.CreateBy);
            this.HasOptional(t => t.PersonalDetail1)
                .WithMany(t => t.StockComponentOfferDurations1)
                .HasForeignKey(d => d.ModifyBy);

        }
    }
}
