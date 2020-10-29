using System.ComponentModel.DataAnnotations;
using System.Data.Entity.ModelConfiguration;

namespace ModelLayer.Models.Mapping
{
    public class FranchiseMap : EntityTypeConfiguration<Franchise>
    {
        public FranchiseMap()
        {
            // Primary Key
            this.HasKey(t => t.ID);

            // Properties
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
                .HasMaxLength(15);

            this.Property(t => t.Address)
                .HasMaxLength(150);

            this.Property(t => t.NetworkIP)
                .HasMaxLength(15);

            this.Property(t => t.DeviceType)
                .HasMaxLength(50);

            this.Property(t => t.DeviceID)
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("Franchise");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.BusinessDetailID).HasColumnName("BusinessDetailID");
            this.Property(t => t.ServiceNumber).HasColumnName("ServiceNumber");
            this.Property(t => t.ContactPerson).HasColumnName("ContactPerson");
            this.Property(t => t.Mobile).HasColumnName("Mobile");
            this.Property(t => t.Email).HasColumnName("Email");
            this.Property(t => t.Landline).HasColumnName("Landline");
            this.Property(t => t.FAX).HasColumnName("FAX");
            this.Property(t => t.Address).HasColumnName("Address");
            this.Property(t => t.PincodeID).HasColumnName("PincodeID");
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
                .WithMany(t => t.Franchises)
                .HasForeignKey(d => d.BusinessDetailID);
            this.HasRequired(t => t.PersonalDetail)
                .WithMany(t => t.Franchises)
                .HasForeignKey(d => d.CreateBy);
            this.HasOptional(t => t.Pincode)
                .WithMany(t => t.Franchises)
                .HasForeignKey(d => d.PincodeID);
            this.HasOptional(t => t.PersonalDetail1)
                .WithMany(t => t.Franchises1)
                .HasForeignKey(d => d.ModifyBy);

        }
    }
}
