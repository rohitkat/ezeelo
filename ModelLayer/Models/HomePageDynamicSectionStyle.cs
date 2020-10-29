using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models
{

    [Table("HomePageDynamicSectionStyle")]
    public class HomePageDynamicSectionStyle
    {
        [Key]
        public long ID { get; set; }

        public string SectionStyle { get; set; }

        

        public decimal? Mobile_Banner_Height { get; set; }

        public decimal? Mobile_Banner_Width { get; set; }

        public string Mobile_Banner_Size { get; set; }

        public decimal? Mobile_Category_Height { get; set; }

        public decimal? Mobile_Category_Width { get; set; }

        public string Mobile_Category_Size { get; set; }

        public decimal? Portal_Banner_Height { get; set; }

        public decimal? Portal_Banner_Width { get; set; }

        public string Portal_Banner_Size { get; set; }

        public decimal? Portal_Category_Height { get; set; }

        public decimal? Portal_Category_Width { get; set; }

        public string Portal_Category_Size { get; set; }

    }
}
