using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace ModelLayer.Models.Mapping
{
    public class ReferDetailMap : EntityTypeConfiguration<ReferDetail>
    {
        public ReferDetailMap()
        {
            // Primary Key
            this.HasKey(t => t.ID);

            // Properties
            this.Property(t => t.Email)
                .HasMaxLength(150);

            this.Property(t => t.Mobile)
                .HasMaxLength(10);

            this.Property(t => t.NetworkIP)
                .HasMaxLength(15);

            this.Property(t => t.DeviceType)
                .HasMaxLength(50);

            this.Property(t => t.DeviceID)
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("ReferDetail");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.ReferAndEarnSchemaID).HasColumnName("ReferAndEarnSchemaID");
            this.Property(t => t.UserID).HasColumnName("UserID");
            this.Property(t => t.Email).HasColumnName("Email");
            this.Property(t => t.Mobile).HasColumnName("Mobile");
            this.Property(t => t.ReferenceID).HasColumnName("ReferenceID");
            this.Property(t => t.CreateDate).HasColumnName("CreateDate");
            this.Property(t => t.CreateBy).HasColumnName("CreateBy");
            this.Property(t => t.ModifyDate).HasColumnName("ModifyDate");
            this.Property(t => t.ModifyBy).HasColumnName("ModifyBy");
            this.Property(t => t.NetworkIP).HasColumnName("NetworkIP");
            this.Property(t => t.DeviceType).HasColumnName("DeviceType");
            this.Property(t => t.DeviceID).HasColumnName("DeviceID");

            // Relationships
            this.HasRequired(t => t.ReferAndEarnSchema)
                .WithMany(t => t.ReferDetails)
                .HasForeignKey(d => d.ReferAndEarnSchemaID);

        }
    }
}
