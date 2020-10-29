using System.ComponentModel.DataAnnotations;
using System.Data.Entity.ModelConfiguration;

namespace ModelLayer.Models.Mapping
{
    public class DeliveryBoyMap : EntityTypeConfiguration<DeliveryBoy>
    {
        public DeliveryBoyMap()
        {
            // Primary Key
            this.HasKey(t => t.ID);

            // Properties
            this.Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(100);


            this.Property(t => t.Mobile)
               .IsRequired()
               .HasMaxLength(10);

          

            this.Property(t => t.Password)
               .IsRequired()
               .HasMaxLength(100);

            this.Property(t => t.Address)
               .IsRequired()
               .HasMaxLength(500);
            this.Property(t => t.AdhaarNo)
               .IsRequired()
               .HasMaxLength(100);

            this.Property(t => t.AdhaarImageUrl)
              .IsRequired()
              .HasMaxLength(200);

           

            this.Property(t => t.NetworkIP)
                .HasMaxLength(15);

            this.Property(t => t.DeviceType)
                .HasMaxLength(50);

            this.Property(t => t.DeviceID)
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("DeliveryBoy");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.FranchiseID).HasColumnName("FranchiseID");
            this.Property(t => t.Name).HasColumnName("Name");
            this.Property(t => t.Mobile).HasColumnName("Mobile");
            this.Property(t => t.Email).HasColumnName("Email");
            this.Property(t => t.Password).HasColumnName("Password");
            this.Property(t => t.Address).HasColumnName("Address");
            this.Property(t => t.AdhaarNo).HasColumnName("AdhaarNo");


            this.Property(t => t.Password).HasColumnName("AdhaarImageUrl");
            this.Property(t => t.Address).HasColumnName("DrivingLicenseUrl");
            this.Property(t => t.AdhaarNo).HasColumnName("IsVerified");

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
                .WithMany(t => t.DeliveryBoys)
                .HasForeignKey(d => d.CreateBy);

            this.HasRequired(t => t.FranchiseDetail)
               .WithMany(t => t.DeliveryBoys)
               .HasForeignKey(d => d.CreateBy);

            this.HasOptional(t => t.PersonalDetail1)
                .WithMany(t => t.DeliveryBoys)
                .HasForeignKey(d => d.ModifyBy);

        }
    }
}
