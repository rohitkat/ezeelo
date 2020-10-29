using System.ComponentModel.DataAnnotations;
using System.Data.Entity.ModelConfiguration;

namespace ModelLayer.Models.Mapping
{
    public class DeliveryCashHandlingChargeMap : EntityTypeConfiguration<DeliveryCashHandlingCharge>
    {
        public DeliveryCashHandlingChargeMap()
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
            this.ToTable("DeliveryCashHandlingCharge");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.DeliveryPartnerID).HasColumnName("DeliveryPartnerID");
            this.Property(t => t.MaxAmount).HasColumnName("MaxAmount");
            this.Property(t => t.PerHourCharge).HasColumnName("PerHourCharge");
            this.Property(t => t.IsApproved).HasColumnName("IsApproved");
            this.Property(t => t.ApprovedBy).HasColumnName("ApprovedBy");
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
                .WithMany(t => t.DeliveryCashHandlingCharges)
                .HasForeignKey(d => d.CreateBy);
            this.HasRequired(t => t.DeliveryPartner)
                .WithMany(t => t.DeliveryCashHandlingCharges)
                .HasForeignKey(d => d.DeliveryPartnerID);
            this.HasOptional(t => t.PersonalDetail1)
                .WithMany(t => t.DeliveryCashHandlingCharges1)
                .HasForeignKey(d => d.ApprovedBy);
            this.HasOptional(t => t.PersonalDetail2)
                .WithMany(t => t.DeliveryCashHandlingCharges2)
                .HasForeignKey(d => d.ModifyBy);

        }
    }
}
