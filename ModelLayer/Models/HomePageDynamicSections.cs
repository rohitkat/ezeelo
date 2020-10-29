using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models
{
    [Table("HomePageDynamicSection")]
    public partial class HomePageDynamicSection
    {
        [Key]
        public long ID { get; set; }

        public long SectionId { get; set; }

        public string SectionDisplayName { get; set; }

        //public string SectionHeader { get; set; }

        public string SectionStyle { get; set; }

        public int SequenceOrder { get; set; }

        public bool ShowInApp { get; set; }

        public bool IsActive { get; set; }

        public long FranchiseId { get; set; }

        public bool IsBanner { get; set; }

        public bool IsProduct { get; set; }

        public bool IsCategory { get; set; }

        public long? CreateBy { get; set; }

        public DateTime CreateDate { get; set; }

        public long? ModifyBy { get; set; }

        public DateTime? ModifyDate { get; set; }

        public string NetworkIP { get; set; }

        //public virtual HomePageDynamicSectionsMaster HomePageDynamicSectionsMaster { get; set; }

        // public virtual Franchise Franchise { get; set; }
    }
}
