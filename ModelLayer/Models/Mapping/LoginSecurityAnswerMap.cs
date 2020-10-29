using System.ComponentModel.DataAnnotations;
using System.Data.Entity.ModelConfiguration;

namespace ModelLayer.Models.Mapping
{
    public class LoginSecurityAnswerMap : EntityTypeConfiguration<LoginSecurityAnswer>
    {
        public LoginSecurityAnswerMap()
        {
            // Primary Key
            this.HasKey(t => t.ID);

            // Properties
            this.Property(t => t.Answer)
                .HasMaxLength(150);

            this.Property(t => t.DeviceType)
                .HasMaxLength(50);

            this.Property(t => t.DeviceID)
                .HasMaxLength(50);

            this.Property(t => t.NetworkIP)
                .HasMaxLength(15);

            // Table & Column Mappings
            this.ToTable("LoginSecurityAnswer");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.UserLoginID).HasColumnName("UserLoginID");
            this.Property(t => t.SecurityQuestionID).HasColumnName("SecurityQuestionID");
            this.Property(t => t.Answer).HasColumnName("Answer");
            this.Property(t => t.DeviceType).HasColumnName("DeviceType");
            this.Property(t => t.DeviceID).HasColumnName("DeviceID");
            this.Property(t => t.CreateDate).HasColumnName("CreateDate");
            this.Property(t => t.CreateBy).HasColumnName("CreateBy");
            this.Property(t => t.ModifyDate).HasColumnName("ModifyDate");
            this.Property(t => t.ModifyBy).HasColumnName("ModifyBy");
            this.Property(t => t.NetworkIP).HasColumnName("NetworkIP");

            // Relationships
            this.HasRequired(t => t.PersonalDetail)
                .WithMany(t => t.LoginSecurityAnswers)
                .HasForeignKey(d => d.CreateBy);
            this.HasOptional(t => t.SecurityQuestion)
                .WithMany(t => t.LoginSecurityAnswers)
                .HasForeignKey(d => d.SecurityQuestionID);
            this.HasOptional(t => t.UserLogin)
                .WithMany(t => t.LoginSecurityAnswers)
                .HasForeignKey(d => d.UserLoginID);
            this.HasOptional(t => t.PersonalDetail1)
                .WithMany(t => t.LoginSecurityAnswers1)
                .HasForeignKey(d => d.ModifyBy);

        }
    }
}
