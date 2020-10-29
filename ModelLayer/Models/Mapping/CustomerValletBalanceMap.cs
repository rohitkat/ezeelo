using System.ComponentModel.DataAnnotations;
using System.Data.Entity.ModelConfiguration;

namespace ModelLayer.Models.Mapping
{
    public class CustomerValletBalanceMap : EntityTypeConfiguration<CustomerValletBalance>
    {
        public CustomerValletBalanceMap()
        {
            // Primary Key
            this.HasKey(t => t.ID);

            // Properties
            this.Property(t => t.Reason)
                .HasMaxLength(150);

            this.Property(t => t.NetworkIP)
                .HasMaxLength(15);

            this.Property(t => t.DeviceType)
                .HasMaxLength(50);

            this.Property(t => t.DeviceID)
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("CustomerValletBalance");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.UserLoginID).HasColumnName("UserLoginID");
            this.Property(t => t.LastPoint).HasColumnName("LastPoint");
            this.Property(t => t.AddedPoint).HasColumnName("AddedPoint");
            this.Property(t => t.TotalPoint).HasColumnName("TotalPoint");
            this.Property(t => t.Reason).HasColumnName("Reason");
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
                .WithMany(t => t.CustomerValletBalances)
                .HasForeignKey(d => d.CreateBy);
            this.HasRequired(t => t.UserLogin)
                .WithMany(t => t.CustomerValletBalances)
                .HasForeignKey(d => d.UserLoginID);
            this.HasOptional(t => t.PersonalDetail1)
                .WithMany(t => t.CustomerValletBalances1)
                .HasForeignKey(d => d.ModifyBy);

        }
    }
}
