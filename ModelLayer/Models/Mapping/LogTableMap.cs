using System.ComponentModel.DataAnnotations;
using System.Data.Entity.ModelConfiguration;

namespace ModelLayer.Models.Mapping
{
    public class LogTableMap : EntityTypeConfiguration<LogTable>
    {
        public LogTableMap()
        {
            // Primary Key
            this.HasKey(t => t.ID);

            // Properties
            this.Property(t => t.TableName)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.RecordXML)
                .IsRequired();

            this.Property(t => t.Command)
                .IsRequired()
                .HasMaxLength(10);

            // Table & Column Mappings
            this.ToTable("LogTable");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.TableName).HasColumnName("TableName");
            this.Property(t => t.RecordXML).HasColumnName("RecordXML");
            this.Property(t => t.TableRowID).HasColumnName("TableRowID");
            this.Property(t => t.Command).HasColumnName("Command");
            this.Property(t => t.RowOwnerID).HasColumnName("RowOwnerID");
            this.Property(t => t.CreateDate).HasColumnName("CreateDate");
            this.Property(t => t.CreateBy).HasColumnName("CreateBy");

            // Relationships
            this.HasRequired(t => t.PersonalDetail)
                .WithMany(t => t.LogTables)
                .HasForeignKey(d => d.CreateBy);

        }
    }
}
