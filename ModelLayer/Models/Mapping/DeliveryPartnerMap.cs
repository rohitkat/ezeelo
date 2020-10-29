using System.ComponentModel.DataAnnotations;
using System.Data.Entity.ModelConfiguration;

namespace ModelLayer.Models.Mapping
{
    public class DeliveryPartnerMap : EntityTypeConfiguration<DeliveryPartner>
    {
        public DeliveryPartnerMap()
        {
            // Primary Key
            this.HasKey(t => t.ID);

            // Properties
            this.Property(t => t.GodownAddress)
                .IsRequired()
                .HasMaxLength(150);

            this.Property(t => t.ServiceNumber)
                .HasMaxLength(15);

            this.Property(t => t.ContactPerson)
                .IsRequired()
                .HasMaxLength(150);

            this.Property(t => t.Mobile)
                .IsRequired()
                .HasMaxLength(10);

            this.Property(t => t.Email)
                .IsRequired()
                .HasMaxLength(150);

            this.Property(t => t.Landline)
                .HasMaxLength(13);

            this.Property(t => t.FAX)
                .HasMaxLength(13);

            this.Property(t => t.WeeklyOff)
                .HasMaxLength(10);

            this.Property(t => t.NetworkIP)
                .HasMaxLength(15);

            this.Property(t => t.DeviceType)
                .HasMaxLength(50);

            this.Property(t => t.DeviceID)
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("DeliveryPartner");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.BusinessDetailID).HasColumnName("BusinessDetailID");
            this.Property(t => t.GodownAddress).HasColumnName("GodownAddress");
            this.Property(t => t.PincodeID).HasColumnName("PincodeID");
            this.Property(t => t.ServiceNumber).HasColumnName("ServiceNumber");
            this.Property(t => t.ServiceLevel).HasColumnName("ServiceLevel");
            this.Property(t => t.ContactPerson).HasColumnName("ContactPerson");
            this.Property(t => t.Mobile).HasColumnName("Mobile");
            this.Property(t => t.Email).HasColumnName("Email");
            this.Property(t => t.Landline).HasColumnName("Landline");
            this.Property(t => t.FAX).HasColumnName("FAX");
            this.Property(t => t.VehicleTypeID).HasColumnName("VehicleTypeID");
            this.Property(t => t.OpeningTime).HasColumnName("OpeningTime");
            this.Property(t => t.ClosingTime).HasColumnName("ClosingTime");
            this.Property(t => t.WeeklyOff).HasColumnName("WeeklyOff");
            this.Property(t => t.IsLive).HasColumnName("IsLive");
            this.Property(t => t.IsActive).HasColumnName("IsActive");
            this.Property(t => t.CreateDate).HasColumnName("CreateDate");
            this.Property(t => t.CreateBy).HasColumnName("CreateBy");
            this.Property(t => t.ModifyDate).HasColumnName("ModifyDate");
            this.Property(t => t.ModifyBy).HasColumnName("ModifyBy");
            this.Property(t => t.NetworkIP).HasColumnName("NetworkIP");
            this.Property(t => t.DeviceType).HasColumnName("DeviceType");
            this.Property(t => t.DeviceID).HasColumnName("DeviceID");

            // Relationships
            this.HasRequired(t => t.BusinessDetail)
                .WithMany(t => t.DeliveryPartners)
                .HasForeignKey(d => d.BusinessDetailID);
            this.HasRequired(t => t.PersonalDetail)
                .WithMany(t => t.DeliveryPartners)
                .HasForeignKey(d => d.CreateBy);
            this.HasRequired(t => t.Pincode)
                .WithMany(t => t.DeliveryPartners)
                .HasForeignKey(d => d.PincodeID);
            this.HasRequired(t => t.VehicleType)
                .WithMany(t => t.DeliveryPartners)
                .HasForeignKey(d => d.VehicleTypeID);
            this.HasOptional(t => t.PersonalDetail1)
                .WithMany(t => t.DeliveryPartners1)
                .HasForeignKey(d => d.ModifyBy);

        }
    }
}
