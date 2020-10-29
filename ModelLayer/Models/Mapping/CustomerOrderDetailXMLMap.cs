using System.ComponentModel.DataAnnotations;
using System.Data.Entity.ModelConfiguration;

namespace ModelLayer.Models.Mapping
{
    public class CustomerOrderDetailXMLMap : EntityTypeConfiguration<CustomerOrderDetailXML>
    {
        public CustomerOrderDetailXMLMap()
        {
            // Primary Key
            this.HasKey(t => t.ID);

            // Properties
            this.Property(t => t.XML)
                .IsRequired();

            // Table & Column Mappings
            this.ToTable("CustomerOrderDetailXML");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.CustomerOrderID).HasColumnName("CustomerOrderID");
            this.Property(t => t.XML).HasColumnName("XML");
            this.Property(t => t.CreateDate).HasColumnName("CreateDate");
            this.Property(t => t.CreateBy).HasColumnName("CreateBy");
            this.Property(t => t.ModifyDate).HasColumnName("ModifyDate");
            this.Property(t => t.ModifyBy).HasColumnName("ModifyBy");

            // Relationships
            this.HasRequired(t => t.CustomerOrder)
                .WithMany(t => t.CustomerOrderDetailXMLs)
                .HasForeignKey(d => d.CustomerOrderID);
            this.HasRequired(t => t.PersonalDetail)
                .WithMany(t => t.CustomerOrderDetailXMLs)
                .HasForeignKey(d => d.CreateBy);
            this.HasOptional(t => t.PersonalDetail1)
                .WithMany(t => t.CustomerOrderDetailXMLs1)
                .HasForeignKey(d => d.ModifyBy);

        }
    }
}
