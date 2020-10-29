using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity.ModelConfiguration;

namespace ModelLayer.Models.Mapping
{
    public class HelpDeskDetailsMap : EntityTypeConfiguration<HelpDeskDetails>
    {
        public HelpDeskDetailsMap()
        {
            // Primary Key
            this.HasKey(t => t.ID);

            // Properties
            this.Property(t => t.HelpLineNumber)
                .HasMaxLength(10);

            this.Property(t => t.ManagmentNumber)
                .HasMaxLength(50);

            this.Property(t => t.NetworkIP)
                .HasMaxLength(15);

            this.Property(t => t.DeviceType)
                .HasMaxLength(50);

            this.Property(t => t.DeviceID)
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("HelpDeskDetails");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.CityID).HasColumnName("CityID");
            this.Property(t => t.FranchiseID).HasColumnName("FranchiseID");
            this.Property(t => t.HelpLineNumber).HasColumnName("HelpLineNumber");
            this.Property(t => t.ManagmentNumber).HasColumnName("ManagmentNumber");
            this.Property(t => t.IsActive).HasColumnName("IsActive");
            this.Property(t => t.CreateDate).HasColumnName("CreateDate");
            this.Property(t => t.CreateBy).HasColumnName("CreateBy");
            this.Property(t => t.ModifyDate).HasColumnName("ModifyDate");
            this.Property(t => t.ModifyBy).HasColumnName("ModifyBy");
            this.Property(t => t.NetworkIP).HasColumnName("NetworkIP");
            this.Property(t => t.DeviceType).HasColumnName("DeviceType");
            this.Property(t => t.DeviceID).HasColumnName("DeviceID");

            // Relationships
            this.HasRequired(t => t.City)
                .WithMany(t => t.HelpDeskDetails)
                .HasForeignKey(d => d.CityID);
            this.HasRequired(t => t.Franchise)
                .WithMany(t => t.HelpDeskDetails)
                .HasForeignKey(d => d.FranchiseID);
        }
    }
}
