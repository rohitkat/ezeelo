using System.ComponentModel.DataAnnotations;
using System.Data.Entity.ModelConfiguration;

namespace ModelLayer.Models.Mapping
{
    public class UserChatMap : EntityTypeConfiguration<UserChat>
    {
        public UserChatMap()
        {
            // Primary Key
            this.HasKey(t => t.ID);

            // Properties
            this.Property(t => t.Text)
                .IsRequired()
                .HasMaxLength(500);

            this.Property(t => t.NetworkIP)
                .HasMaxLength(15);

            this.Property(t => t.DeviceType)
                .HasMaxLength(50);

            this.Property(t => t.DeviceID)
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("UserChat");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.ChatID).HasColumnName("ChatID");
            this.Property(t => t.PersonalDetailID).HasColumnName("PersonalDetailID");
            this.Property(t => t.Text).HasColumnName("Text");
            this.Property(t => t.IsActive).HasColumnName("IsActive");
            this.Property(t => t.CreateDate).HasColumnName("CreateDate");
            this.Property(t => t.CreateBy).HasColumnName("CreateBy");
            this.Property(t => t.ModifyDate).HasColumnName("ModifyDate");
            this.Property(t => t.ModifyBy).HasColumnName("ModifyBy");
            this.Property(t => t.NetworkIP).HasColumnName("NetworkIP");
            this.Property(t => t.DeviceType).HasColumnName("DeviceType");
            this.Property(t => t.DeviceID).HasColumnName("DeviceID");

            // Relationships
            this.HasRequired(t => t.Chat)
                .WithMany(t => t.UserChats)
                .HasForeignKey(d => d.ChatID);
            this.HasRequired(t => t.PersonalDetail)
                .WithMany(t => t.UserChats)
                .HasForeignKey(d => d.CreateBy);
            this.HasOptional(t => t.PersonalDetail1)
                .WithMany(t => t.UserChats1)
                .HasForeignKey(d => d.ModifyBy);
            this.HasRequired(t => t.PersonalDetail2)
                .WithMany(t => t.UserChats2)
                .HasForeignKey(d => d.PersonalDetailID);

        }
    }
}
