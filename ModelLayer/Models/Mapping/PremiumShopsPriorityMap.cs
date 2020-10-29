using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models.Mapping
{
    public class PremiumShopsPriorityMap : EntityTypeConfiguration<PremiumShopsPriority>
    {
        public PremiumShopsPriorityMap()
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
            this.ToTable("PremiumShopsPriority");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.FranchiseID).HasColumnName("FranchiseID");
            this.Property(t => t.CategoryID).HasColumnName("CategoryID");
            this.Property(t => t.ShopID).HasColumnName("ShopID");
            this.Property(t => t.PriorityLevel).HasColumnName("PriorityLevel");
            this.Property(t => t.IsActive).HasColumnName("IsActive");
            this.Property(t => t.CreateDate).HasColumnName("CreateDate");
            this.Property(t => t.ModifyDate).HasColumnName("ModifyDate");
            this.Property(t => t.CreateBy).HasColumnName("CreateBy");
            this.Property(t => t.ModifyBy).HasColumnName("ModifyBy");
            this.Property(t => t.NetworkIP).HasColumnName("NetworkIP");
            this.Property(t => t.DeviceType).HasColumnName("DeviceType");
            this.Property(t => t.DeviceID).HasColumnName("DeviceID");

            // Relationships
            this.HasRequired(t => t.Category)
                .WithMany(t => t.PremiumShopsPriorities)
                .HasForeignKey(d => d.CategoryID);
            //this.HasRequired(t => t.City)
            //    .WithMany(t => t.PremiumShopsPriorities)
            //    .HasForeignKey(d => d.CityID);
            this.HasRequired(t => t.Shop)
                .WithMany(t => t.PremiumShopsPriorities)
                .HasForeignKey(d => d.ShopID);

            this.HasRequired(t => t.Franchise)
                .WithMany(t => t.PremiumShopsPriorities)
                .HasForeignKey(d => d.FranchiseID);

        }
    }
}
