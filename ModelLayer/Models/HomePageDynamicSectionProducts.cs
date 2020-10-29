using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models
{
    [Table("HomePageDynamicSectionProduct")]
    public  class HomePageDynamicSectionProduct
    {
        [Key]
        public long ID { get; set; }

        public long HomePageDynamicSectionId { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public long ShopStockId { get; set; }

        public int SequenceOrder { get; set; }

        public bool IsActive { get; set; }

        public DateTime CreateDate { get; set; }

        public long CreatedBy { get; set; }

        public long ModifyBy { get; set; }

        public DateTime ModifyDate { get; set; }

        public string NetworkIp { get; set; }

        //public virtual HomePageDynamicSectionsMaster HomePageDynamicSectionsMaster { get; set; }

        //public virtual Franchise Franchise { get; set; }
    }
}
