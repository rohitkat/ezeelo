using System.ComponentModel.DataAnnotations;
using System.Data.Entity.ModelConfiguration;

namespace ModelLayer.Models.Mapping
{
    public class TodaySchemeMap : EntityTypeConfiguration<TodayScheme>
    {
        public TodaySchemeMap()
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
            this.ToTable("TodayScheme");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.SchemeTypeID).HasColumnName("SchemeTypeID");
            this.Property(t => t.TodaysValueInRs).HasColumnName("TodaysValueInRs");
            this.Property(t => t.ApplicableOnPurchaseOfRs).HasColumnName("ApplicableOnPurchaseOfRs");
            this.Property(t => t.StartDatetime).HasColumnName("StartDatetime");
            this.Property(t => t.EndDatetime).HasColumnName("EndDatetime");
            this.Property(t => t.IsActive).HasColumnName("IsActive");
            this.Property(t => t.CreateDate).HasColumnName("CreateDate");
            this.Property(t => t.CreateBy).HasColumnName("CreateBy");
            this.Property(t => t.ModifyDate).HasColumnName("ModifyDate");
            this.Property(t => t.ModifyBy).HasColumnName("ModifyBy");
            this.Property(t => t.NetworkIP).HasColumnName("NetworkIP");
            this.Property(t => t.DeviceType).HasColumnName("DeviceType");
            this.Property(t => t.DeviceID).HasColumnName("DeviceID");

            // Relationships
            this.HasRequired(t => t.PersonalDetail)
                .WithMany(t => t.TodaySchemes)
                .HasForeignKey(d => d.CreateBy);
            this.HasOptional(t => t.PersonalDetail1)
                .WithMany(t => t.TodaySchemes1)
                .HasForeignKey(d => d.ModifyBy);
            this.HasOptional(t => t.SchemeType)
                .WithMany(t => t.TodaySchemes)
                .HasForeignKey(d => d.SchemeTypeID);

        }
    }
}
