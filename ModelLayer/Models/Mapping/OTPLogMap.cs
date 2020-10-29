using System.ComponentModel.DataAnnotations;
using System.Data.Entity.ModelConfiguration;

namespace ModelLayer.Models.Mapping
{

         public class OTPLogMap : EntityTypeConfiguration<OTPLog>
    {
        public OTPLogMap()
        {
            // Primary Key
            this.HasKey(t => t.ID);

            this.ToTable("OTPLog");
            

        }
    }
}
