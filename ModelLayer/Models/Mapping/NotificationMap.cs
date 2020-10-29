using System.ComponentModel.DataAnnotations;
using System.Data.Entity.ModelConfiguration;

namespace ModelLayer.Models.Mapping
{
    public class NotificationMap : EntityTypeConfiguration<Notification>
    {
        public NotificationMap()
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
            this.ToTable("Notification");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.TypeID).HasColumnName("TypeID");
            this.Property(t => t.FromBusinessTypeID).HasColumnName("FromBusinessTypeID");
            this.Property(t => t.FromPersonalDetailID).HasColumnName("FromPersonalDetailID");
            this.Property(t => t.ToBusinessTypeID).HasColumnName("ToBusinessTypeID");
            this.Property(t => t.ToPersonalDetailID).HasColumnName("ToPersonalDetailID");
            this.Property(t => t.TargetOwnerId).HasColumnName("TargetOwnerId");
            this.Property(t => t.Status).HasColumnName("Status");
            this.Property(t => t.CreateDate).HasColumnName("CreateDate");
            this.Property(t => t.CreateBy).HasColumnName("CreateBy");
            this.Property(t => t.ModifyDate).HasColumnName("ModifyDate");
            this.Property(t => t.ModifyBy).HasColumnName("ModifyBy");
            this.Property(t => t.NetworkIP).HasColumnName("NetworkIP");
            this.Property(t => t.DeviceType).HasColumnName("DeviceType");
            this.Property(t => t.DeviceID).HasColumnName("DeviceID");

            // Relationships
            this.HasRequired(t => t.BusinessType)
                .WithMany(t => t.Notifications)
                .HasForeignKey(d => d.FromBusinessTypeID);
            this.HasRequired(t => t.BusinessType1)
                .WithMany(t => t.Notifications1)
                .HasForeignKey(d => d.ToBusinessTypeID);
            this.HasRequired(t => t.PersonalDetail)
                .WithMany(t => t.Notifications)
                .HasForeignKey(d => d.CreateBy);
            this.HasOptional(t => t.PersonalDetail1)
                .WithMany(t => t.Notifications1)
                .HasForeignKey(d => d.ModifyBy);
            this.HasRequired(t => t.PersonalDetail2)
                .WithMany(t => t.Notifications2)
                .HasForeignKey(d => d.FromPersonalDetailID);
            this.HasRequired(t => t.PersonalDetail3)
                .WithMany(t => t.Notifications3)
                .HasForeignKey(d => d.ToPersonalDetailID);

        }
    }
}
