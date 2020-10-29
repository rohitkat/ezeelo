using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace ModelLayer.Models.Mapping
{
   public class MerchantRatingMap : EntityTypeConfiguration<MerchantRating>
    {
        public MerchantRatingMap()
        {
            // Primary Key
            this.HasKey(t => t.ID);

            // Table & Column Mappings
            this.ToTable("MerchantRating");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.MerchantID).HasColumnName("MerchantID");
            this.Property(t => t.CustomerID).HasColumnName("CutomerID");
            this.Property(t => t.IsDisplay).HasColumnName("IsDisplay");
            this.Property(t => t.Rating).HasColumnName("Rating");
            this.Property(t => t.Review).HasColumnName("Review").HasMaxLength(500);
            this.Property(t => t.CreateDate).HasColumnName("CreateDate");

            // Relationships
            this.HasRequired(t => t.UserLoginDetail)
                .WithMany(t => t.MerchantRating)
                .HasForeignKey(d => d.CustomerID);
        }
    }
}
