using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models
{
    [Table("HomePageDynamicSectionBanner")]
    public partial class HomePageDynamicSectionBanner
    {
        [Key]
        public long ID { get; set; }

        public long HomePageDynamicSectionId { get; set; }

        public string ImageName { get; set; }
        [NotMapped]
        public string ImageName_ { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public int SequenceOrder { get; set; }

        public string ToolTip { get; set; }

        public string LinkURL { get; set; }

        public string Keyword { get; set; }

        public long? ShopId { get; set; }

        public long? BrandID { get; set; }

        public long? CategoryID { get; set; }

        public long? OfferId { get; set; }

        public string DisplayViewApp { get; set; }

        public bool IsActive { get; set; }

        public bool IsBanner { get; set; }

        public bool IsCategory { get; set; }//Added by Roshan on 30-03-2019

        public DateTime? CreateDate { get; set; }

        public long? CreatedBy { get; set; }

        public DateTime? ModifyDate { get; set; }

        public long? ModifyBy { get; set; }

        public string NetworkIp { get; set; }

        [NotMapped]
        public bool IsActive_ { get; set; }

        [NotMapped]
        public bool IsBanner_ { get; set; }

        //public virtual HomePageDynamicSectionsMaster HomePageDynamicSectionsMaster { get; set; }

        //public virtual Franchise Franchise { get; set; }

    }
}
