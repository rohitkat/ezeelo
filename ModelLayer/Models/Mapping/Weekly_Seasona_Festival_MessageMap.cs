using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace ModelLayer.Models.Mapping
{
    public class Weekly_Seasona_Festival_MessageMap : EntityTypeConfiguration<Weekly_Seasona_Festival_Message>
    {
        public Weekly_Seasona_Festival_MessageMap()
        {
            // Primary Key
            this.HasKey(t => t.ID);

            // Properties
            this.Property(t => t.WeeklyHoliday)
                .HasMaxLength(100);

            this.Property(t => t.NetworkIP)
                .HasMaxLength(15);

            this.Property(t => t.DeviceType)
                .HasMaxLength(50);

            this.Property(t => t.DeviceID)
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("Weekly_Seasona_Festival_Message");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.FranchiseID).HasColumnName("FranchiseID");
            this.Property(t => t.MessageTypeID).HasColumnName("MessageTypeID");
            this.Property(t => t.Message).HasColumnName("Message");
            this.Property(t => t.MinimumOrderInRupee).HasColumnName("MinimumOrderInRupee");
            this.Property(t => t.WeeklyHoliday).HasColumnName("WeeklyHoliday");
            this.Property(t => t.SeasonalMsgFrmMonth).HasColumnName("SeasonalMsgFrmMonth");
            this.Property(t => t.SeasonalMsgToMonth).HasColumnName("SeasonalMsgToMonth");
            this.Property(t => t.FestivalMsgFrmDate).HasColumnName("FestivalMsgFrmDate");
            this.Property(t => t.FestivalMsgToDate).HasColumnName("FestivalMsgToDate");
            this.Property(t => t.IsActive).HasColumnName("IsActive");
            this.Property(t => t.CreateDate).HasColumnName("CreateDate");
            this.Property(t => t.CreateBy).HasColumnName("CreateBy");
            this.Property(t => t.ModifyDate).HasColumnName("ModifyDate");
            this.Property(t => t.ModifyBy).HasColumnName("ModifyBy");
            this.Property(t => t.NetworkIP).HasColumnName("NetworkIP");
            this.Property(t => t.DeviceType).HasColumnName("DeviceType");
            this.Property(t => t.DeviceID).HasColumnName("DeviceID");

            // Relationships
            this.HasRequired(t => t.Franchise)
                .WithMany(t => t.Weekly_Seasona_Festival_Message)
                .HasForeignKey(d => d.FranchiseID);
            this.HasRequired(t => t.Weekly_Seasona_Festival_Message2)
                .WithMany(t => t.Weekly_Seasona_Festival_Message1)
                .HasForeignKey(d => d.MessageTypeID);

        }
    }
}
