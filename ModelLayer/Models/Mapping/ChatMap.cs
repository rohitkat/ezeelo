using System.ComponentModel.DataAnnotations;
using System.Data.Entity.ModelConfiguration;

namespace ModelLayer.Models.Mapping
{
    public class ChatMap : EntityTypeConfiguration<Chat>
    {
        public ChatMap()
        {
            // Primary Key
            this.HasKey(t => t.ID);

            // Properties
            this.Property(t => t.Name)
                .HasMaxLength(50);

            this.Property(t => t.Email)
                .HasMaxLength(50);

            this.Property(t => t.Mobile)
                .HasMaxLength(10);

            this.Property(t => t.NetworkIP)
                .HasMaxLength(15);

            this.Property(t => t.DeviceType)
                .HasMaxLength(50);

            this.Property(t => t.DeviceID)
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("Chat");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.CRMPersonalDetailID).HasColumnName("CRMPersonalDetailID");
            this.Property(t => t.Name).HasColumnName("Name");
            this.Property(t => t.Email).HasColumnName("Email");
            this.Property(t => t.Mobile).HasColumnName("Mobile");
            this.Property(t => t.IsActive).HasColumnName("IsActive");
            this.Property(t => t.CreateDate).HasColumnName("CreateDate");
            this.Property(t => t.CreateBy).HasColumnName("CreateBy");
            this.Property(t => t.ModifyDate).HasColumnName("ModifyDate");
            this.Property(t => t.ModifyBy).HasColumnName("ModifyBy");
            this.Property(t => t.NetworkIP).HasColumnName("NetworkIP");
            this.Property(t => t.DeviceType).HasColumnName("DeviceType");
            this.Property(t => t.DeviceID).HasColumnName("DeviceID");

            // Relationships
            this.HasOptional(t => t.PersonalDetail)
                .WithMany(t => t.Chats)
                .HasForeignKey(d => d.CRMPersonalDetailID);
            this.HasRequired(t => t.PersonalDetail1)
                .WithMany(t => t.Chats1)
                .HasForeignKey(d => d.CreateBy);
            this.HasOptional(t => t.PersonalDetail2)
                .WithMany(t => t.Chats2)
                .HasForeignKey(d => d.ModifyBy);

        }
    }
}
