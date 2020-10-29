using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace ModelLayer.Models.Mapping
{
    public class BlockItemsListMap : EntityTypeConfiguration<BlockItemsList>
    {
        public BlockItemsListMap()
        {
            // Primary Key
            this.HasKey(t => t.ID);

            // Properties
            this.Property(t => t.ImageName)
                .IsRequired()
                .HasMaxLength(100);

            this.Property(t => t.LinkUrl)
                .IsRequired()
                .HasMaxLength(200);

            this.Property(t => t.Tooltip)
                .HasMaxLength(200);

            this.Property(t => t.DeviceType)
                .HasMaxLength(50);

            this.Property(t => t.DeviceID)
                .HasMaxLength(50);

            this.Property(t => t.NetworkIP)
                .HasMaxLength(15);

            this.Property(t => t.Remarks)
                .HasMaxLength(200);

            // Table & Column Mappings
            this.ToTable("BlockItemsList");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.FranchiseID).HasColumnName("FranchiseID");
            this.Property(t => t.DesignBlockTypeID).HasColumnName("DesignBlockTypeID");
            this.Property(t => t.StartDate).HasColumnName("StartDate");
            this.Property(t => t.EndDate).HasColumnName("EndDate");
            this.Property(t => t.SequenceOrder).HasColumnName("SequenceOrder");
            this.Property(t => t.ImageName).HasColumnName("ImageName");
            this.Property(t => t.LinkUrl).HasColumnName("LinkUrl");
            this.Property(t => t.Tooltip).HasColumnName("Tooltip");
            this.Property(t => t.ProductID).HasColumnName("ProductID");
            this.Property(t => t.IsActive).HasColumnName("IsActive");
            this.Property(t => t.CreateDate).HasColumnName("CreateDate");
            this.Property(t => t.CreatedBy).HasColumnName("CreatedBy");
            this.Property(t => t.ModifyDate).HasColumnName("ModifyDate");
            this.Property(t => t.ModifyBy).HasColumnName("ModifyBy");
            this.Property(t => t.DeviceType).HasColumnName("DeviceType");
            this.Property(t => t.DeviceID).HasColumnName("DeviceID");
            this.Property(t => t.NetworkIP).HasColumnName("NetworkIP");
            this.Property(t => t.Remarks).HasColumnName("Remarks");

            // Relationships
            this.HasRequired(t => t.DesignBlockType)
                .WithMany(t => t.BlockItemsLists)
                .HasForeignKey(d => d.DesignBlockTypeID);
            this.HasRequired(t => t.Franchise)
                .WithMany(t => t.BlockItemsLists)
                .HasForeignKey(d => d.FranchiseID);
            this.HasRequired(t => t.Product)
                .WithMany(t => t.BlockItemsLists)
                .HasForeignKey(d => d.ProductID);
        }
    }
}
