using System.ComponentModel.DataAnnotations;
using System.Data.Entity.ModelConfiguration;


namespace ModelLayer.Models.Mapping
{
   public class SupplierMap : EntityTypeConfiguration<Supplier>
    {
      public SupplierMap()
       {
           this.HasKey(t => t.ID);

           // Properties
          this.Property(t=>t.Name).HasMaxLength(150);

           this.Property(t => t.SupplierCode)
               .HasMaxLength(25);

          this.Property(t => t.ContactPerson)
               .HasMaxLength(150);

          this.Property(t => t.Address)
               .HasMaxLength(500);

          this.Property(t => t.Mobile)
               .HasMaxLength(10);
          this.Property(t => t.Landline)
               .HasMaxLength(13);
          this.Property(t => t.Email)
               .HasMaxLength(150);
          this.Property(t => t.FAX)
               .HasMaxLength(15);

           this.Property(t => t.GSTNumber)
               .HasMaxLength(25);
           this.Property(t => t.PAN)
               .HasMaxLength(25);
           this.Property(t => t.Website)
               .HasMaxLength(150);

           this.Property(t => t.CC)
               .HasMaxLength(150);          

           this.Property(t => t.NetworkIP)
               .HasMaxLength(15);

           this.Property(t => t.DeviceType)
               .HasMaxLength(50);

           this.Property(t => t.DeviceID)
               .HasMaxLength(50);

           // Table & Column Mappings
           this.ToTable("Supplier");
           this.Property(t => t.ID).HasColumnName("ID");
           this.Property(t => t.Name).HasColumnName("Name");
           this.Property(t => t.SupplierCode).HasColumnName("SupplierCode");
           this.Property(t => t.ContactPerson).HasColumnName("ContactPerson");
           this.Property(t => t.Address).HasColumnName("Address");
           this.Property(t => t.Mobile).HasColumnName("Mobile");
           this.Property(t => t.Landline).HasColumnName("Landline");
           this.Property(t => t.Email).HasColumnName("Email");
           this.Property(t => t.FAX).HasColumnName("FAX");
           this.Property(t => t.GSTNumber).HasColumnName("GSTNumber");
           this.Property(t => t.PAN).HasColumnName("PAN");
           this.Property(t => t.Website).HasColumnName("Website");
           this.Property(t => t.CC).HasColumnName("CC");
           this.Property(t => t.StateID).HasColumnName("StateID");
           this.Property(t => t.CityID).HasColumnName("CityID");
           this.Property(t => t.PincodeID).HasColumnName("PincodeID");
           this.Property(t => t.IsActive).HasColumnName("IsActive");
           this.Property(t => t.CreateDate).HasColumnName("CreateDate");
           this.Property(t => t.CreateBy).HasColumnName("CreateBy");
           this.Property(t => t.ModifyDate).HasColumnName("ModifyDate");
           this.Property(t => t.ModifyBy).HasColumnName("ModifyBy");
           this.Property(t => t.NetworkIP).HasColumnName("NetworkIP");
           this.Property(t => t.DeviceType).HasColumnName("DeviceType");
           this.Property(t => t.DeviceID).HasColumnName("DeviceID");

           // Relationships
           this.HasRequired(t => t.PersonalDetails)
              .WithMany(t => t.Suppliers)
              .HasForeignKey(d => d.CreateBy);
           //this.HasOptional(t => t.States)
           //    .WithMany(t => t.supp)
           //    .HasForeignKey(d => d.ModifyBy);
           //this.HasRequired(t => t.ProductVarient)
           //    .WithMany(t => t.ShopStocks)
           //    .HasForeignKey(d => d.ProductVarientID);
           //this.HasRequired(t => t.ShopProduct)
           //    .WithMany(t => t.ShopStocks)
           //    .HasForeignKey(d => d.ShopProductID);
           //this.HasRequired(t => t.Unit)
           //    .WithMany(t => t.ShopStocks)
           //    .HasForeignKey(d => d.PackUnitID);
       }
    }
}
