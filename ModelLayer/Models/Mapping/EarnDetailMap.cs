using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace ModelLayer.Models.Mapping
{
    public class EarnDetailMap : EntityTypeConfiguration<EarnDetail>
    {
        public EarnDetailMap()
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
            this.ToTable("EarnDetail");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.ReferAndEarnSchemaID).HasColumnName("ReferAndEarnSchemaID");
            this.Property(t => t.EarnUID).HasColumnName("EarnUID");
            this.Property(t => t.ReferUID).HasColumnName("ReferUID");
            this.Property(t => t.EarnAmount).HasColumnName("EarnAmount");
            this.Property(t => t.UsedAmount).HasColumnName("UsedAmount");
            this.Property(t => t.RemainingAmount).HasColumnName("RemainingAmount");
            this.Property(t => t.CustomerOrderID).HasColumnName("CustomerOrderID");
            this.Property(t => t.CreateDate).HasColumnName("CreateDate");
            this.Property(t => t.CreateBy).HasColumnName("CreateBy");
            this.Property(t => t.ModifyDate).HasColumnName("ModifyDate");
            this.Property(t => t.ModifyBy).HasColumnName("ModifyBy");
            this.Property(t => t.NetworkIP).HasColumnName("NetworkIP");
            this.Property(t => t.DeviceType).HasColumnName("DeviceType");
            this.Property(t => t.DeviceID).HasColumnName("DeviceID");

            // Relationships
            this.HasOptional(t => t.CustomerOrder)
                .WithMany(t => t.EarnDetails)
                .HasForeignKey(d => d.CustomerOrderID);
            this.HasOptional(t => t.ReferAndEarnSchema)
                .WithMany(t => t.EarnDetails)
                .HasForeignKey(d => d.ReferAndEarnSchemaID);

        }
    }
}
