using System.ComponentModel.DataAnnotations;
using System.Data.Entity.ModelConfiguration;

namespace ModelLayer.Models.Mapping
{
    public class CustomerRatingAndFeedbackMap : EntityTypeConfiguration<CustomerRatingAndFeedback>
    {
        public CustomerRatingAndFeedbackMap()
        {
            // Primary Key
            this.HasKey(t => t.ID);

            // Properties
            this.Property(t => t.Feedback)
                .IsRequired()
                .HasMaxLength(150);

            this.Property(t => t.NetworkIP)
                .HasMaxLength(15);

            this.Property(t => t.DeviceType)
                .HasMaxLength(50);

            this.Property(t => t.DeviceID)
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("CustomerRatingAndFeedback");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.PersonalDetailID).HasColumnName("PersonalDetailID");
            this.Property(t => t.OwnerID).HasColumnName("OwnerID");
            this.Property(t => t.RatingID).HasColumnName("RatingID");
            this.Property(t => t.Point).HasColumnName("Point");
            this.Property(t => t.Feedback).HasColumnName("Feedback");
            this.Property(t => t.IsApproved).HasColumnName("IsApproved");
            this.Property(t => t.ApprovedBy).HasColumnName("ApprovedBy");
            this.Property(t => t.CreateDate).HasColumnName("CreateDate");
            this.Property(t => t.CreateBy).HasColumnName("CreateBy");
            this.Property(t => t.ModifyDate).HasColumnName("ModifyDate");
            this.Property(t => t.ModifyBy).HasColumnName("ModifyBy");
            this.Property(t => t.NetworkIP).HasColumnName("NetworkIP");
            this.Property(t => t.DeviceType).HasColumnName("DeviceType");
            this.Property(t => t.DeviceID).HasColumnName("DeviceID");

            // Relationships
            this.HasRequired(t => t.PersonalDetail)
                .WithMany(t => t.CustomerRatingAndFeedbacks)
                .HasForeignKey(d => d.CreateBy);
            this.HasRequired(t => t.Rating)
                .WithMany(t => t.CustomerRatingAndFeedbacks)
                .HasForeignKey(d => d.RatingID);
            this.HasOptional(t => t.PersonalDetail1)
                .WithMany(t => t.CustomerRatingAndFeedbacks1)
                .HasForeignKey(d => d.ModifyBy);

        }
    }
}
