using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace ModelLayer.Models.Mapping
{
    public class SchemeBudgetTransactionMap : EntityTypeConfiguration<SchemeBudgetTransaction>
    {
        public SchemeBudgetTransactionMap()
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
            this.ToTable("SchemeBudgetTransaction");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.ReferAndEarnSchemaID).HasColumnName("ReferAndEarnSchemaID");
            this.Property(t => t.TotalBudgetAmt).HasColumnName("TotalBudgetAmt");
            this.Property(t => t.RemainingAmt).HasColumnName("RemainingAmt");
            this.Property(t => t.ExpiryDate).HasColumnName("ExpiryDate");
            this.Property(t => t.CreateDate).HasColumnName("CreateDate");
            this.Property(t => t.CreateBy).HasColumnName("CreateBy");
            this.Property(t => t.ModifyDate).HasColumnName("ModifyDate");
            this.Property(t => t.ModifyBy).HasColumnName("ModifyBy");
            this.Property(t => t.NetworkIP).HasColumnName("NetworkIP");
            this.Property(t => t.DeviceType).HasColumnName("DeviceType");
            this.Property(t => t.DeviceID).HasColumnName("DeviceID");

            // Relationships
            this.HasRequired(t => t.ReferAndEarnSchema)
                .WithMany(t => t.SchemeBudgetTransactions)
                .HasForeignKey(d => d.ReferAndEarnSchemaID);

        }
    }
}
