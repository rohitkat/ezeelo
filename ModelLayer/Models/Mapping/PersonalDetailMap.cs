using System.ComponentModel.DataAnnotations;
using System.Data.Entity.ModelConfiguration;

namespace ModelLayer.Models.Mapping
{
    public class PersonalDetailMap : EntityTypeConfiguration<PersonalDetail>
    {
        public PersonalDetailMap()
        {
            // Primary Key
            this.HasKey(t => t.ID);

            // Properties
            this.Property(t => t.FirstName)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.MiddleName)
                .HasMaxLength(50);

            //this.Property(t => t.LastName)
            //    .IsRequired()
            //    .HasMaxLength(50);

            this.Property(t => t.Gender)
                .HasMaxLength(6);

            this.Property(t => t.Address)
                .HasMaxLength(500);

            this.Property(t => t.AlternateMobile)
                .HasMaxLength(10);

            this.Property(t => t.AlternateEmail)
                .HasMaxLength(150);

            this.Property(t => t.NetworkIP)
                .HasMaxLength(15);

            this.Property(t => t.DeviceType)
                .HasMaxLength(50);

            this.Property(t => t.DeviceID)
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("PersonalDetail");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.UserLoginID).HasColumnName("UserLoginID");
            this.Property(t => t.SalutationID).HasColumnName("SalutationID");
            this.Property(t => t.FirstName).HasColumnName("FirstName");
            this.Property(t => t.MiddleName).HasColumnName("MiddleName");
            this.Property(t => t.LastName).HasColumnName("LastName");
            this.Property(t => t.DOB).HasColumnName("DOB");
            this.Property(t => t.Gender).HasColumnName("Gender");
            this.Property(t => t.PincodeID).HasColumnName("PincodeID");
            this.Property(t => t.Address).HasColumnName("Address");
            this.Property(t => t.AlternateMobile).HasColumnName("AlternateMobile");
            this.Property(t => t.AlternateEmail).HasColumnName("AlternateEmail");
            this.Property(t => t.IsActive).HasColumnName("IsActive");
            this.Property(t => t.CreateDate).HasColumnName("CreateDate");
            this.Property(t => t.CreateBy).HasColumnName("CreateBy");
            this.Property(t => t.ModifyDate).HasColumnName("ModifyDate");
            this.Property(t => t.ModifyBy).HasColumnName("ModifyBy");
            this.Property(t => t.NetworkIP).HasColumnName("NetworkIP");
            this.Property(t => t.DeviceType).HasColumnName("DeviceType");
            this.Property(t => t.DeviceID).HasColumnName("DeviceID");

            // Relationships
            this.HasOptional(t => t.PersonalDetail2)
                .WithMany(t => t.PersonalDetail1)
                .HasForeignKey(d => d.CreateBy);
            this.HasOptional(t => t.PersonalDetail3)
                .WithMany(t => t.PersonalDetail11)
                .HasForeignKey(d => d.ModifyBy);
            this.HasOptional(t => t.Pincode)
                .WithMany(t => t.PersonalDetails)
                .HasForeignKey(d => d.PincodeID);
            this.HasRequired(t => t.Salutation)
                .WithMany(t => t.PersonalDetails)
                .HasForeignKey(d => d.SalutationID);
            this.HasRequired(t => t.UserLogin)
                .WithMany(t => t.PersonalDetails)
                .HasForeignKey(d => d.UserLoginID);

        }
    }
}
