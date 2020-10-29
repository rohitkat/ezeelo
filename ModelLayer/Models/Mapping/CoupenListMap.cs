using System.ComponentModel.DataAnnotations;
using System.Data.Entity.ModelConfiguration;

namespace ModelLayer.Models.Mapping
{
    public class CoupenListMap : EntityTypeConfiguration<CoupenList>
    {
        public CoupenListMap()
        {
            // Primary Key
            this.HasKey(t => t.ID);

            // Properties
            this.Property(t => t.CoupenCode)
                .IsRequired()
                .HasMaxLength(15);

            this.Property(t => t.NetworkIP)
                .HasMaxLength(15);

            this.Property(t => t.DeviceType)
                .HasMaxLength(50);

            this.Property(t => t.DeviceID)
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("CoupenList");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.SchemeTypeID).HasColumnName("SchemeTypeID");
            this.Property(t => t.CoupenCode).HasColumnName("CoupenCode");
            this.Property(t => t.CoupenQty).HasColumnName("CoupenQty");
            this.Property(t => t.UsedQty).HasColumnName("UsedQty");
            this.Property(t => t.CityID).HasColumnName("CityID");
            this.Property(t => t.FranchiseID).HasColumnName("FranchiseID");//added
            this.Property(t => t.IsActive).HasColumnName("IsActive");
            this.Property(t => t.CreateDate).HasColumnName("CreateDate");
            this.Property(t => t.CreateBy).HasColumnName("CreateBy");
            this.Property(t => t.ModifyDate).HasColumnName("ModifyDate");
            this.Property(t => t.ModifyBy).HasColumnName("ModifyBy");
            this.Property(t => t.NetworkIP).HasColumnName("NetworkIP");
            this.Property(t => t.DeviceType).HasColumnName("DeviceType");
            this.Property(t => t.DeviceID).HasColumnName("DeviceID");

            // Relationships
            this.HasRequired(t => t.City)
                .WithMany(t => t.CoupenLists)
                .HasForeignKey(d => d.CityID);
            this.HasRequired(t => t.SchemeType)
                .WithMany(t => t.CoupenLists)
                .HasForeignKey(d => d.SchemeTypeID);
            this.HasRequired(t => t.PersonalDetail)
                .WithMany(t => t.CoupenLists)
                .HasForeignKey(d => d.CreateBy);
            this.HasOptional(t => t.PersonalDetail1)
                .WithMany(t => t.CoupenLists1)
                .HasForeignKey(d => d.ModifyBy);

        }
    }
}
