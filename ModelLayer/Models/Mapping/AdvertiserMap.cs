using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace ModelLayer.Models.Mapping
{
    public class AdvertiserMap : EntityTypeConfiguration<Advertiser>
    {
        public AdvertiserMap()
        {
            // Primary Key
            this.Property(t => t.ID);

            // Properties
            /*this.Property(t => t.ID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);*/

            this.Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(150);

            this.Property(t => t.ContactPersone)
                .HasMaxLength(50);

            this.Property(t => t.BusinessDetailID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.PincodeID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.Address)
                .HasMaxLength(150);

            this.Property(t => t.Email)
                .HasMaxLength(50);

            this.Property(t => t.Mobile)
                .HasMaxLength(10);

            this.Property(t => t.NetworkIP)
                .HasMaxLength(15);

            this.Property(t => t.DeviceType)
                .HasMaxLength(50);

            this.Property(t => t.DeviceID)
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("Advertiser");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.Name).HasColumnName("Name");
            this.Property(t => t.ContactPersone).HasColumnName("ContactPersone");
            this.Property(t => t.BusinessDetailID).HasColumnName("BusinessDetailID");
            this.Property(t => t.PincodeID).HasColumnName("PincodeID");
            this.Property(t => t.Address).HasColumnName("Address");
            this.Property(t => t.Email).HasColumnName("Email");
            this.Property(t => t.Mobile).HasColumnName("Mobile");
            this.Property(t => t.IsLive).HasColumnName("IsLive");
            this.Property(t => t.IsActive).HasColumnName("IsActive");
            this.Property(t => t.CreateDate).HasColumnName("CreateDate");
            this.Property(t => t.ModifyDate).HasColumnName("ModifyDate");
            this.Property(t => t.CreateBy).HasColumnName("CreateBy");
            this.Property(t => t.ModifyBy).HasColumnName("ModifyBy");
            this.Property(t => t.NetworkIP).HasColumnName("NetworkIP");
            this.Property(t => t.DeviceType).HasColumnName("DeviceType");
            this.Property(t => t.DeviceID).HasColumnName("DeviceID");

            // Relationships
            this.HasRequired(t => t.BusinessDetail)
                .WithMany(t => t.Advertisers)
                .HasForeignKey(d => d.BusinessDetailID);
            this.HasOptional(t => t.PersonalDetail)
                .WithMany(t => t.Advertisers)
                .HasForeignKey(d => d.CreateBy);
            this.HasOptional(t => t.PersonalDetail1)
                .WithMany(t => t.Advertisers1)
                .HasForeignKey(d => d.ModifyBy);
            this.HasRequired(t => t.Pincode)
                .WithMany(t => t.Advertisers)
                .HasForeignKey(d => d.PincodeID);

        }
    }
}
