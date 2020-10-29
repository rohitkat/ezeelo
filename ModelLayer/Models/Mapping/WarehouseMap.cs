using System.ComponentModel.DataAnnotations;
using System.Data.Entity.ModelConfiguration;

namespace ModelLayer.Models.Mapping
{
    public class WarehouseMap : EntityTypeConfiguration<Warehouse>
    {
        public WarehouseMap()
        {
            // Primary Key
            this.HasKey(t => t.ID);

            // Properties
            this.Property(t => t.Name).HasMaxLength(150);

            this.Property(t => t.WarehouseCode)
                .HasMaxLength(50);

            this.Property(t => t.BusinessDetailID);

            this.Property(t => t.GSTNumber)
                 .HasMaxLength(25);

            this.Property(t => t.ServiceNumber)
                 .HasMaxLength(25);

            //this.Property(t => t.ServiceLevel);
            this.Property(t => t.PincodeID);

            this.Property(t => t.StateID);
            this.Property(t => t.CityID);

            this.Property(t => t.NearbyTransport)
                 .HasMaxLength(500);

            this.Property(t => t.Measurement)
                 .HasMaxLength(10);

            this.Property(t => t.FloorSpace)
                 .HasMaxLength(10);

            this.Property(t => t.Volume)
                 .HasMaxLength(10);

            this.Property(t => t.CustomEntry)
                .HasMaxLength(50);
            this.Property(t => t.CustomExit)
                .HasMaxLength(50);

            this.Property(t => t.IsActive);          

            this.Property(t => t.NetworkIP)
                .HasMaxLength(15);

            this.Property(t => t.DeviceType)
                .HasMaxLength(50);

            this.Property(t => t.DeviceID)
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("Warehouse");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.Name).HasColumnName("Name");
            this.Property(t => t.WarehouseCode).HasColumnName("WarehouseCode");
            this.Property(t => t.BusinessDetailID).HasColumnName("BusinessDetailID");
            this.Property(t => t.GSTNumber).HasColumnName("GSTNumber");
            this.Property(t => t.ServiceNumber).HasColumnName("ServiceNumber");
            //this.Property(t => t.ServiceLevel).HasColumnName("ServiceLevel");
            this.Property(t => t.PincodeID).HasColumnName("PincodeID");
            this.Property(t => t.StateID).HasColumnName("StateID");
            this.Property(t => t.CityID).HasColumnName("CityID");
            this.Property(t => t.NearbyTransport).HasColumnName("NearbyTransport");
            this.Property(t => t.Measurement).HasColumnName("Measurement");
            this.Property(t => t.FloorSpace).HasColumnName("FloorSpace");
            this.Property(t => t.Volume).HasColumnName("Volume");
            this.Property(t => t.CustomEntry).HasColumnName("CustomEntry");
            this.Property(t => t.CustomExit).HasColumnName("CustomExit");        
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
               .WithMany(t => t.Warehouses)
               .HasForeignKey(d => d.CreateBy);                      
           
        }
    }
}
