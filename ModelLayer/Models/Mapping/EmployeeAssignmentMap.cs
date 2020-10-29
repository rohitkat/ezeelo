using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace ModelLayer.Models.Mapping
{
    public class EmployeeAssignmentMap : EntityTypeConfiguration<EmployeeAssignment>
    {
        // Hide from Ashish for Live
       /* public EmployeeAssignmentMap()
        {
            // Primary Key
            this.HasKey(t => t.ID);

            // Properties
            this.Property(t => t.OrderCode)
                .HasMaxLength(15);

            this.Property(t => t.ShopOrderCode)
                .HasMaxLength(15);

            this.Property(t => t.EmployeeCode)
                .HasMaxLength(15);

            this.Property(t => t.FromAddress)
                .HasMaxLength(150);

            this.Property(t => t.DeliveredType)
                .HasMaxLength(15);

            this.Property(t => t.DeliveredType)
                .HasMaxLength(15);

            this.Property(t => t.DeliverySchedule)
               .HasMaxLength(50);

            this.Property(t => t.ToAddress)
               .HasMaxLength(150);

            this.Property(t => t.NetworkIP)
                .HasMaxLength(15);

            this.Property(t => t.DeviceType)
                .HasMaxLength(50);

            this.Property(t => t.DeviceID)
                .HasMaxLength(50);

            this.Property(t => t.X)
                .HasMaxLength(50);

            this.Property(t => t.Y)
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("EmployeeAssignment");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.OrderCode).HasColumnName("OrderCode");
            this.Property(t => t.ShopOrderCode).HasColumnName("ShopOrderCode");
            this.Property(t => t.EmployeeCode).HasColumnName("GodownCode");
            this.Property(t => t.EmployeeCode).HasColumnName("EmployeeCode");
            this.Property(t => t.OrderStatus).HasColumnName("OrderStatus");
            this.Property(t => t.FromAddress).HasColumnName("FromAddress");
            this.Property(t => t.DeliveredType).HasColumnName("DeliveredType");
            this.Property(t => t.DeliveryType).HasColumnName("DeliveryType");
            this.Property(t => t.DeliveryDate).HasColumnName("DeliveryDate");
            this.Property(t => t.DeliverySchedule).HasColumnName("DeliverySchedule");
            this.Property(t => t.DeliveredTime).HasColumnName("DeliveredTime");
            this.Property(t => t.ToAddress).HasColumnName("ToAddress");
            this.Property(t => t.IsActive).HasColumnName("IsActive");
            this.Property(t => t.CreateDate).HasColumnName("CreateDate");
            this.Property(t => t.CreateBy).HasColumnName("CreateBy");
            this.Property(t => t.ModifyDate).HasColumnName("ModifyDate");
            this.Property(t => t.ModifyBy).HasColumnName("ModifyBy");
            this.Property(t => t.NetworkIP).HasColumnName("NetworkIP");
            this.Property(t => t.DeviceType).HasColumnName("DeviceType");
            this.Property(t => t.DeviceID).HasColumnName("DeviceID");
            this.Property(t => t.X).HasColumnName("X");
            this.Property(t => t.Y).HasColumnName("Y");
        }*/
    }
}
