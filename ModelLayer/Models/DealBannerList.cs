using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models
{
    [Table("DealBannerList")]
    public class DealBannerList
    {
        [Key]
        public long Id { get; set; }
        public long DealId { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public System.DateTime StartDate { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public System.DateTime EndDate { get; set; }
        public int FranchiseId { get; set; }
        public int SequenceOrder { get; set; }
        public string ImageName { get; set; }
        public string Tooltip { get; set; }
        public Nullable<long> ProductID { get; set; }
        public bool IsActive { get; set; }
        public System.DateTime CreateDate { get; set; }
        public long CreatedBy { get; set; }
        public Nullable<System.DateTime> ModifyDate { get; set; }
        public Nullable<long> ModifyBy { get; set; }
        public string DeviceType { get; set; }
        public string DeviceID { get; set; }
        public string NetworkIP { get; set; }
        public string Remarks { get; set; }
        public Nullable<long> CategoryID { get; set; }//For Api Banner replication added by Sonali_05-12-2018
        public Nullable<long> BrandId { get; set; }//For Api Banner replication added by Sonali_05-12-2018
        public Nullable<long> ShopID { get; set; }//For Api Banner replication added by Sonali_05-12-2018
        public string Keyword { get; set; }//For Api Banner replication added by Sonali_05-12-2018
        public Nullable<long> OfferID { get; set; }//For Api Banner replication added by Sonali_05-12-2018
        public virtual Franchise Franchise { get; set; }
        public virtual Product Product { get; set; }
        public virtual Deal Deal { get; set; }
        public string DisplayViewApp { get; set; }//For Api Banner replication added by Sonali_05-12-2018
    }
}
