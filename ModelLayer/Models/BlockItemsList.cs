using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ModelLayer.Models
{
    public partial class BlockItemsList
    {
        public long ID { get; set; }
        public int FranchiseID { get; set; }
        public long DesignBlockTypeID { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public System.DateTime StartDate { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public System.DateTime EndDate { get; set; }
        public int SequenceOrder { get; set; }
        public string ImageName { get; set; }
        public string LinkUrl { get; set; }
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
        public virtual DesignBlockType DesignBlockType { get; set; }
        public virtual Franchise Franchise { get; set; }
        public virtual Product Product { get; set; }
        public Nullable<int> CategoryID { get; set; }//For Api Banner replication added by Sonali_05-12-2018
        public Nullable<int> BrandId { get; set; }//For Api Banner replication added by Sonali_05-12-2018
        public Nullable<long> ShopID { get; set; }//For Api Banner replication added by Sonali_05-12-2018
        public string Keyword { get; set; }//For Api Banner replication added by Sonali_05-12-2018
        public Nullable<long> OfferID { get; set; }//For Api Banner replication added by Sonali_05-12-2018
        public string DisplayViewApp { get; set; }//For Api Banner replication added by Sonali_16-01-2019
    }
}
