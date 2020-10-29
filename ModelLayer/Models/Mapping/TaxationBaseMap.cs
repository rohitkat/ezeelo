using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace ModelLayer.Models.Mapping
{
    public class TaxationBaseMap : EntityTypeConfiguration<TaxationBase>
    {
        public TaxationBaseMap()
        {
            // Primary Key
            this.HasKey(t => t.ID);

            // Properties
            this.Property(t => t.Description)
                .HasMaxLength(150);

            this.Property(t => t.NetworkIP)
                .HasMaxLength(15);

            this.Property(t => t.DeviceType)
                .HasMaxLength(50);

            this.Property(t => t.DeviceID)
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("TaxationBase");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.TaxationID).HasColumnName("TaxationID");
            this.Property(t => t.FranchiseTaxDetailID).HasColumnName("FranchiseTaxDetailID");
            this.Property(t => t.Description).HasColumnName("Description");
            this.Property(t => t.IsActive).HasColumnName("IsActive");
            this.Property(t => t.CreateDate).HasColumnName("CreateDate");
            this.Property(t => t.CreateBy).HasColumnName("CreateBy");
            this.Property(t => t.ModifyDate).HasColumnName("ModifyDate");
            this.Property(t => t.ModifyBy).HasColumnName("ModifyBy");
            this.Property(t => t.NetworkIP).HasColumnName("NetworkIP");
            this.Property(t => t.DeviceType).HasColumnName("DeviceType");
            this.Property(t => t.DeviceID).HasColumnName("DeviceID");

            // Relationships
            this.HasRequired(t => t.FranchiseTaxDetail)
                .WithMany(t => t.TaxationBases)
                .HasForeignKey(d => d.FranchiseTaxDetailID);
            this.HasRequired(t => t.TaxationMaster)
                .WithMany(t => t.TaxationBases)
                .HasForeignKey(d => d.TaxationID);

        }
    }
}
