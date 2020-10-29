using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace ModelLayer.Models.Mapping
{
    public class DeliveryScheduleMap : EntityTypeConfiguration<DeliverySchedule>
    {
        public DeliveryScheduleMap()
        {
            // Primary Key
            this.HasKey(t => t.ID);

            // Properties
            this.Property(t => t.DisplayName)
                .IsRequired()
                .HasMaxLength(25);

            this.Property(t => t.NetworkIP)
                .HasMaxLength(15);

            this.Property(t => t.DeviceType)
                .HasMaxLength(50);

            this.Property(t => t.DeviceID)
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("DeliverySchedule");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.DisplayName).HasColumnName("DisplayName");
            this.Property(t => t.ActualTimeFrom).HasColumnName("ActualTimeFrom");
            this.Property(t => t.ActualTimeTo).HasColumnName("ActualTimeTo");
            this.Property(t => t.NoOfDelivery).HasColumnName("NoOfDelivery");
            this.Property(t => t.IsActive).HasColumnName("IsActive");
            this.Property(t => t.CreateDate).HasColumnName("CreateDate");
            this.Property(t => t.CreateBy).HasColumnName("CreateBy");
            this.Property(t => t.ModifyDate).HasColumnName("ModifyDate");
            this.Property(t => t.ModifyBy).HasColumnName("ModifyBy");
            this.Property(t => t.NetworkIP).HasColumnName("NetworkIP");
            this.Property(t => t.DeviceType).HasColumnName("DeviceType");
            this.Property(t => t.DeviceID).HasColumnName("DeviceID");
            this.Property(t => t.CityID).HasColumnName("CityID");
            this.Property(t => t.FranchiseID).HasColumnName("FranchiseID");//added

            // Relationships
            this.HasOptional(t => t.City)
                .WithMany(t => t.DeliverySchedules)
                .HasForeignKey(d => d.CityID);

        }
    }
}
