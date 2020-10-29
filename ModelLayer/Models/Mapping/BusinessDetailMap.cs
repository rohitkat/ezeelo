using System.ComponentModel.DataAnnotations;
using System.Data.Entity.ModelConfiguration;

namespace ModelLayer.Models.Mapping
{
    public class BusinessDetailMap : EntityTypeConfiguration<BusinessDetail>
    {
        public BusinessDetailMap()
        {
            // Primary Key
            this.HasKey(t => t.ID);

            // Properties
            this.Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(150);

            this.Property(t => t.ContactPerson)
                .IsRequired()
                .HasMaxLength(150);

            this.Property(t => t.Mobile)
                .IsRequired()
                .HasMaxLength(10);

            this.Property(t => t.Email)
                .IsRequired()
                .HasMaxLength(150);

            this.Property(t => t.Landline1)
                .HasMaxLength(13);

            this.Property(t => t.Landline2)
                .HasMaxLength(13);

            this.Property(t => t.FAX)
                .HasMaxLength(13);

            this.Property(t => t.Address)
                .HasMaxLength(500);

            this.Property(t => t.Website)
                .HasMaxLength(150);

            this.Property(t => t.SourcesInfoDescription)
                .HasMaxLength(150);

            this.Property(t => t.NetworkIP)
                .HasMaxLength(15);

            this.Property(t => t.DeviceType)
                .HasMaxLength(50);

            this.Property(t => t.DeviceID)
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("BusinessDetail");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.UserLoginID).HasColumnName("UserLoginID");
            this.Property(t => t.Name).HasColumnName("Name");
            this.Property(t => t.BusinessTypeID).HasColumnName("BusinessTypeID");
            this.Property(t => t.ContactPerson).HasColumnName("ContactPerson");
            this.Property(t => t.Mobile).HasColumnName("Mobile");
            this.Property(t => t.Email).HasColumnName("Email");
            this.Property(t => t.Landline1).HasColumnName("Landline1");
            this.Property(t => t.Landline2).HasColumnName("Landline2");
            this.Property(t => t.FAX).HasColumnName("FAX");
            this.Property(t => t.Address).HasColumnName("Address");
            this.Property(t => t.PincodeID).HasColumnName("PincodeID");
            this.Property(t => t.Website).HasColumnName("Website");
            this.Property(t => t.YearOfEstablishment).HasColumnName("YearOfEstablishment");
            this.Property(t => t.SourceOfInfoID).HasColumnName("SourceOfInfoID");
            this.Property(t => t.SourcesInfoDescription).HasColumnName("SourcesInfoDescription");
            this.Property(t => t.IsActive).HasColumnName("IsActive");
            this.Property(t => t.CreateDate).HasColumnName("CreateDate");
            this.Property(t => t.CreateBy).HasColumnName("CreateBy");
            this.Property(t => t.ModifyDate).HasColumnName("ModifyDate");
            this.Property(t => t.ModifyBy).HasColumnName("ModifyBy");
            this.Property(t => t.NetworkIP).HasColumnName("NetworkIP");
            this.Property(t => t.DeviceType).HasColumnName("DeviceType");
            this.Property(t => t.DeviceID).HasColumnName("DeviceID");

            // Relationships
            this.HasRequired(t => t.BusinessType)
                .WithMany(t => t.BusinessDetails)
                .HasForeignKey(d => d.BusinessTypeID);
            this.HasRequired(t => t.Pincode)
                .WithMany(t => t.BusinessDetails)
                .HasForeignKey(d => d.PincodeID);
            this.HasOptional(t => t.SourceOfInfo)
                .WithMany(t => t.BusinessDetails)
                .HasForeignKey(d => d.SourceOfInfoID);
            this.HasRequired(t => t.UserLogin)
                .WithMany(t => t.BusinessDetails)
                .HasForeignKey(d => d.UserLoginID);
            this.HasRequired(t => t.PersonalDetail)
                .WithMany(t => t.BusinessDetails)
                .HasForeignKey(d => d.CreateBy);
            this.HasOptional(t => t.PersonalDetail1)
                .WithMany(t => t.BusinessDetails1)
                .HasForeignKey(d => d.ModifyBy);

        }
    }
}
