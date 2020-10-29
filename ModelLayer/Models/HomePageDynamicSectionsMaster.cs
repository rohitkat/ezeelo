using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models
{
    [Table("HomePageDynamicSectionsMaster")]
    public partial class HomePageDynamicSectionsMaster
    {
        //public HomePageDynamicSectionsMaster()
        //{
        //    this.HomePageDynamicSection = new List<HomePageDynamicSection>();
        //    this.HomePageDynamicSectionBanner = new List<HomePageDynamicSectionBanner>();
        //    this.HomePageDynamicSectionProduct = new List<HomePageDynamicSectionProduct>();
        //}

        [Key]
        public long ID { get; set; }

        public int SectionID { get; set; }

        [Required(ErrorMessage = "Section is Required")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Name must be between 3 - 50 characters ")]
        public string Section { get; set; }


        [StringLength(50, MinimumLength = 3, ErrorMessage = "Name must be between 3 - 50 characters ")]
        public string SectionHeader { get; set; }

        public string Description { get; set; }

        //public bool? IsBannerUpload { get; set; }

        //public bool? IsCategoryImgUpload { get; set; }

        //public bool? IsProductImgUpload { get; set; }

        public bool? IsActive { get; set; }

        public decimal? MobileImgWidth { get; set; }

        public decimal? MobileImgHeight { get; set; }

        public string MobileImgSize { get; set; }

        public decimal? PortalImgWidth { get; set; }

        public decimal? PortalImgHeight { get; set; }

        public string PortalImgSize { get; set; }

        public DateTime? CreateDate { get; set; }

        public long? CreatedBy { get; set; }

        public DateTime? ModifyDate { get; set; }

        public long? ModifyBy { get; set; }

        public string NetworkIp { get; set; }

        //public virtual ICollection<HomePageDynamicSection> HomePageDynamicSection { get; set; }

        //public virtual ICollection<HomePageDynamicSectionBanner> HomePageDynamicSectionBanner { get; set; }

        //public virtual ICollection<HomePageDynamicSectionProduct> HomePageDynamicSectionProduct { get; set; }

    }
}
