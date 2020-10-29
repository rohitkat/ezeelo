using System.ComponentModel.DataAnnotations;
using System.Data.Entity.ModelConfiguration;

namespace ModelLayer.Models.Mapping
{
    public class CustomerOrderHistoryMap : EntityTypeConfiguration<CustomerOrderHistory>
    {
        public CustomerOrderHistoryMap()
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
            this.ToTable("CustomerOrderHistory");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.CustomerOrderID).HasColumnName("CustomerOrderID");
            this.Property(t => t.ShopStockID).HasColumnName("ShopStockID");
            this.Property(t => t.Status).HasColumnName("Status");
            this.Property(t => t.IsActive).HasColumnName("IsActive");
            this.Property(t => t.CreateDate).HasColumnName("CreateDate");
            this.Property(t => t.CreateBy).HasColumnName("CreateBy");
            this.Property(t => t.ModifyDate).HasColumnName("ModifyDate");
            this.Property(t => t.ModifyBy).HasColumnName("ModifyBy");
            this.Property(t => t.NetworkIP).HasColumnName("NetworkIP");
            this.Property(t => t.DeviceType).HasColumnName("DeviceType");
            this.Property(t => t.DeviceID).HasColumnName("DeviceID");

            // Relationships
            this.HasRequired(t => t.CustomerOrder)
                .WithMany(t => t.CustomerOrderHistories)
                .HasForeignKey(d => d.CustomerOrderID);
            this.HasRequired(t => t.PersonalDetail)
                .WithMany(t => t.CustomerOrderHistories)
                .HasForeignKey(d => d.CreateBy);
            this.HasRequired(t => t.ShopStock)
                .WithMany(t => t.CustomerOrderHistories)
                .HasForeignKey(d => d.ShopStockID);
            this.HasOptional(t => t.PersonalDetail1)
                .WithMany(t => t.CustomerOrderHistories1)
                .HasForeignKey(d => d.ModifyBy);

        }
    }
}
