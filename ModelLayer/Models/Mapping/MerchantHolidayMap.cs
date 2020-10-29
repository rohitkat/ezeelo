using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace ModelLayer.Models.Mapping
{
    public class MerchantHolidayMap : EntityTypeConfiguration<MerchantHoliday>
    {
        public MerchantHolidayMap()
        {
            // Primary Key
            this.HasKey(t => t.ID);

            // Table & Column Mappings
            this.ToTable("MerchantHoliday");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.MerchantID).HasColumnName("MerchantID");
            this.Property(t => t.HolidayID).HasColumnName("HolidayID");
        }
    }
}