using System.ComponentModel.DataAnnotations;
using System.Data.Entity.ModelConfiguration;

namespace ModelLayer.Models.Mapping
{
    public class ComponentOfferMap : EntityTypeConfiguration<ComponentOffer>
    {
        public ComponentOfferMap()
        {
            // Primary Key
            this.HasKey(t => t.ID);

            // Properties
            this.Property(t => t.ShortName)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.Description)
                .IsRequired()
                .HasMaxLength(150);

            this.Property(t => t.NetworkIP)
                .HasMaxLength(15);

            this.Property(t => t.DeviceType)
                .HasMaxLength(50);

            this.Property(t => t.DeviceID)
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("ComponentOffer");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.ShortName).HasColumnName("ShortName");
            this.Property(t => t.ShopID).HasColumnName("ShopID");
            this.Property(t => t.ComponentID).HasColumnName("ComponentID");
            this.Property(t => t.OfferInRs).HasColumnName("OfferInRs");
            this.Property(t => t.OfferInPercent).HasColumnName("OfferInPercent");
            this.Property(t => t.Description).HasColumnName("Description");
            this.Property(t => t.IsActive).HasColumnName("IsActive");
            this.Property(t => t.CreateDate).HasColumnName("CreateDate");
            this.Property(t => t.CreateBy).HasColumnName("CreateBy");
            this.Property(t => t.ModifyDate).HasColumnName("ModifyDate");
            this.Property(t => t.ModifyBy).HasColumnName("ModifyBy");
            this.Property(t => t.NetworkIP).HasColumnName("NetworkIP");
            this.Property(t => t.DeviceType).HasColumnName("DeviceType");
            this.Property(t => t.DeviceID).HasColumnName("DeviceID");

            // Relationships
            this.HasRequired(t => t.Component)
                .WithMany(t => t.ComponentOffers)
                .HasForeignKey(d => d.ComponentID);
            this.HasRequired(t => t.Shop)
                .WithMany(t => t.ComponentOffers)
                .HasForeignKey(d => d.ShopID);
            this.HasRequired(t => t.PersonalDetail)
                .WithMany(t => t.ComponentOffers)
                .HasForeignKey(d => d.CreateBy);
            this.HasOptional(t => t.PersonalDetail1)
                .WithMany(t => t.ComponentOffers1)
                .HasForeignKey(d => d.ModifyBy);

        }
    }
}
