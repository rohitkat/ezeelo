using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace ModelLayer.Models.Mapping
{
    public class ApplicationDetailMap : EntityTypeConfiguration<ApplicationDetail>
    {
        public ApplicationDetailMap()
        {
            // Primary Key
            this.HasKey(t => t.ID);

            // Properties
            this.Property(t => t.Name)
                .HasMaxLength(150);

            this.Property(t => t.Email)
                .HasMaxLength(150);

            this.Property(t => t.Mobile)
                .HasMaxLength(12);

            this.Property(t => t.TotalExpience)
                .HasMaxLength(50);

            this.Property(t => t.CurrentCTC)
               .HasMaxLength(150);

            this.Property(t => t.ExpectedCTC)
               .HasMaxLength(150);

            this.Property(t => t.ResumePath)
                .HasMaxLength(150);

            this.Property(t => t.Remarks)
                .HasMaxLength(300);

            // Table & Column Mappings
            this.ToTable("ApplicationDetail");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.CareerID).HasColumnName("CareerID");
            this.Property(t => t.Name).HasColumnName("Name");
            this.Property(t => t.Email).HasColumnName("Email");
            this.Property(t => t.Mobile).HasColumnName("Mobile");
            this.Property(t => t.TotalExpience).HasColumnName("TotalExpience");
            this.Property(t => t.CurrentCTC).HasColumnName("CurrentCTC");
            this.Property(t => t.ExpectedCTC).HasColumnName("ExpectedCTC");
            this.Property(t => t.ResumePath).HasColumnName("ResumePath");
            this.Property(t => t.Remarks).HasColumnName("Remarks");
            this.Property(t => t.AppliedDate).HasColumnName("AppliedDate");
            // Relationships
            this.HasRequired(t => t.Career)
                .WithMany(t => t.ApplicationDetails)
                .HasForeignKey(d => d.CareerID);

        }
    }
}
