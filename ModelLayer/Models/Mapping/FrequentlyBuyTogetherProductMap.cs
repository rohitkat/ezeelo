using System.ComponentModel.DataAnnotations;
using System.Data.Entity.ModelConfiguration;

namespace ModelLayer.Models.Mapping
{
    public class FrequentlyBuyTogetherProductMap : EntityTypeConfiguration<FrequentlyBuyTogetherProduct>
    {
        public FrequentlyBuyTogetherProductMap()
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
            this.ToTable("FrequentlyBuyTogetherProducts");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.WithProductID).HasColumnName("WithProductID");
            this.Property(t => t.ThisProductID).HasColumnName("ThisProductID");
            this.Property(t => t.IsActive).HasColumnName("IsActive");
            this.Property(t => t.CreateDate).HasColumnName("CreateDate");
            this.Property(t => t.CreateBy).HasColumnName("CreateBy");
            this.Property(t => t.ModifyDate).HasColumnName("ModifyDate");
            this.Property(t => t.ModifyBy).HasColumnName("ModifyBy");
            this.Property(t => t.NetworkIP).HasColumnName("NetworkIP");
            this.Property(t => t.DeviceType).HasColumnName("DeviceType");
            this.Property(t => t.DeviceID).HasColumnName("DeviceID");

            // Relationships
            this.HasRequired(t => t.PersonalDetail)
                .WithMany(t => t.FrequentlyBuyTogetherProducts)
                .HasForeignKey(d => d.CreateBy);
            this.HasRequired(t => t.Product)
                .WithMany(t => t.FrequentlyBuyTogetherProducts)
                .HasForeignKey(d => d.ThisProductID);
            this.HasRequired(t => t.Product1)
                .WithMany(t => t.FrequentlyBuyTogetherProducts1)
                .HasForeignKey(d => d.WithProductID);
            this.HasOptional(t => t.PersonalDetail1)
                .WithMany(t => t.FrequentlyBuyTogetherProducts1)
                .HasForeignKey(d => d.ModifyBy);

        }
    }
}
