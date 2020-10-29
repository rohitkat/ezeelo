using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace ModelLayer.Models.Mapping
{
    public class FeedbackManagmentMap : EntityTypeConfiguration<FeedbackManagment>
    {
        public FeedbackManagmentMap()
        {
            // Primary Key
            this.HasKey(t => t.ID);

            // Properties
            this.Property(t => t.Email)
                .IsRequired()
                .HasMaxLength(150);

            this.Property(t => t.Mobile)
                .HasMaxLength(10);

            this.Property(t => t.Message)
                .IsRequired()
                .HasMaxLength(150);

            this.Property(t => t.NetworkIP)
                .HasMaxLength(50);

            this.Property(t => t.DeviceType)
                .HasMaxLength(50);

            this.Property(t => t.DeviceID)
                .HasMaxLength(50);

            this.Property(t => t.CustOrderCode)
              .HasMaxLength(20);

            this.Property(t => t.EmailSMSText)
              .HasMaxLength(150);

            // Table & Column Mappings
            this.ToTable("FeedbackManagment");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.Email).HasColumnName("Email");
            this.Property(t => t.Mobile).HasColumnName("Mobile");
            this.Property(t => t.FeedbackCategaryID).HasColumnName("FeedbackCategaryID");
            this.Property(t => t.Message).HasColumnName("Message");
            this.Property(t => t.FeedBackTypeID).HasColumnName("FeedBackTypeID");
            this.Property(t => t.IsActive).HasColumnName("IsActive");
            this.Property(t => t.CreateDate).HasColumnName("CreateDate");
            this.Property(t => t.CreateBy).HasColumnName("CreateBy");
            this.Property(t => t.ModifyDate).HasColumnName("ModifyDate");
            this.Property(t => t.ModifyBy).HasColumnName("ModifyBy");
            this.Property(t => t.NetworkIP).HasColumnName("NetworkIP");
            this.Property(t => t.DeviceType).HasColumnName("DeviceType");
            this.Property(t => t.DeviceID).HasColumnName("DeviceID");

            this.Property(t => t.CustOrderCode).HasColumnName("CustOrderCode");

            this.Property(t => t.CityID).HasColumnName("CityID");//added for missing
            this.Property(t => t.FranchiseID).HasColumnName("FranchiseID");//added


            this.Property(t => t.EmailSMSText).HasColumnName("EmailSMSText");//added

            // Relationships
            this.HasRequired(t => t.FeedbackCategary)
                .WithMany(t => t.FeedbackManagments)
                .HasForeignKey(d => d.FeedbackCategaryID);
            this.HasOptional(t => t.FeedBackType)
                .WithMany(t => t.FeedbackManagments)
                .HasForeignKey(d => d.FeedBackTypeID);
            this.HasOptional(t => t.PersonalDetail)
                .WithMany(t => t.FeedbackManagments)
                .HasForeignKey(d => d.CreateBy);
            this.HasOptional(t => t.PersonalDetail1)
                .WithMany(t => t.FeedbackManagments1)
                .HasForeignKey(d => d.ModifyBy);

        }
    }
}
