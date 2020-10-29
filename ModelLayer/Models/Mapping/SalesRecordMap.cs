using System.ComponentModel.DataAnnotations;
using System.Data.Entity.ModelConfiguration;

namespace ModelLayer.Models.Mapping
{
    public class SalesRecordMap : EntityTypeConfiguration<SalesRecord>
    {
        public SalesRecordMap()
        {
            // Primary Key
            this.HasKey(t => t.ID);

            // Properties
            this.Property(t => t.FormSerialNumber)
                .IsRequired()
                .HasMaxLength(10);

            this.Property(t => t.SystemSerialNumber)
                .IsRequired()
                .HasMaxLength(10);

            this.Property(t => t.NetworkIP)
                .HasMaxLength(15);

            this.Property(t => t.DeviceType)
                .HasMaxLength(50);

            this.Property(t => t.DeviceID)
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("SalesRecord");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.FormSerialNumber).HasColumnName("FormSerialNumber");
            this.Property(t => t.SystemSerialNumber).HasColumnName("SystemSerialNumber");
            this.Property(t => t.FormSignDate).HasColumnName("FormSignDate");
            this.Property(t => t.EmployeeID).HasColumnName("EmployeeID");
            this.Property(t => t.CreateDate).HasColumnName("CreateDate");
            this.Property(t => t.CreateBy).HasColumnName("CreateBy");
            this.Property(t => t.ModifyDate).HasColumnName("ModifyDate");
            this.Property(t => t.ModifyBy).HasColumnName("ModifyBy");
            this.Property(t => t.NetworkIP).HasColumnName("NetworkIP");
            this.Property(t => t.DeviceType).HasColumnName("DeviceType");
            this.Property(t => t.DeviceID).HasColumnName("DeviceID");

            // Relationships
            this.HasRequired(t => t.Employee)
                .WithMany(t => t.SalesRecords)
                .HasForeignKey(d => d.EmployeeID);
            this.HasRequired(t => t.PersonalDetail)
                .WithMany(t => t.SalesRecords)
                .HasForeignKey(d => d.CreateBy);
            this.HasOptional(t => t.PersonalDetail1)
                .WithMany(t => t.SalesRecords1)
                .HasForeignKey(d => d.ModifyBy);

        }
    }
}
