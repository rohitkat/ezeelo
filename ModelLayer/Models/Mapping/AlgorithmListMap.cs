using System.ComponentModel.DataAnnotations;
using System.Data.Entity.ModelConfiguration;

namespace ModelLayer.Models.Mapping
{
    public class AlgorithmListMap : EntityTypeConfiguration<AlgorithmList>
    {
        public AlgorithmListMap()
        {
            // Primary Key
            this.HasKey(t => t.ID);

            // Properties
            this.Property(t => t.Name)
                .HasMaxLength(50);

            this.Property(t => t.NetworkIP)
                .HasMaxLength(15);

            this.Property(t => t.DeviceType)
                .HasMaxLength(50);

            this.Property(t => t.DeviceID)
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("AlgorithmList");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.Name).HasColumnName("Name");
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
                .WithMany(t => t.AlgorithmLists)
                .HasForeignKey(d => d.CreateBy);
            this.HasOptional(t => t.PersonalDetail1)
                .WithMany(t => t.AlgorithmLists1)
                .HasForeignKey(d => d.ModifyBy);

        }
    }
}
