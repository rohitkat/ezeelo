using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace ModelLayer.Models.Mapping
{
    public class DeliveryBoyAttendanceMap : EntityTypeConfiguration<DeliveryBoyAttendance>
    {
        // Hide from Ashish for Live
        /*public DeliveryBoyAttendanceMap()
        {
            // Primary Key
            this.HasKey(t => t.ID);

            // Properties
            // Table & Column Mappings
            this.ToTable("DeliveryBoyAttendance");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.UserLoginID).HasColumnName("UserLoginID");
            this.Property(t => t.LoginDateTime).HasColumnName("LoginDateTime");
            this.Property(t => t.LogoutDateTime).HasColumnName("LogoutDateTime");
            this.Property(t => t.LoginNetworkIP).HasColumnName("LoginNetworkIP");
            this.Property(t => t.LoginDeviceType).HasColumnName("LoginDeviceType");
            this.Property(t => t.LoginDeviceID).HasColumnName("LoginDeviceID");
            this.Property(t => t.LogoutNetworkIP).HasColumnName("LogoutNetworkIP");
            this.Property(t => t.logoutDeviceType).HasColumnName("logoutDeviceType");
            this.Property(t => t.logoutDeviceID).HasColumnName("logoutDeviceID");
            // Relationships
            this.HasOptional(t => t.UserLogin)
                .WithMany(t => t.DeliveryBoyAttendances)
                .HasForeignKey(d => d.UserLoginID);

        }
    */
    }
}
