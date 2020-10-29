using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models
{
    [Table("MarketPlaceProductGallery")]
    public class MarketPlaceProductGallery
    {
        [Key]
        public long Id { get; set; }
        public long ProductId { get; set; }
        public string ProductName { get; set; }
        public string ShortDescription { get; set; }
        public string GeneralDescription { get; set; }
        public string SearchKeyword { get; set; }
        public string HSNCode { get; set; }
        public string EANCode { get; set; }
        public long Category { get; set; }
        public bool IsApproved { get; set; }
        public bool IsActive { get; set; }
        public long CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public long ModifiedBy { get; set; }
        public DateTime ModifiedDate { get; set; }
    }

    [Table("MarketPlaceProductVarient")]
    public class MarketPlaceProductVarient
    {
        [Key]
        public long Id { get; set; }
        public long MarketPlaceProductId { get; set; }
        public long SizeId { get; set; }
        public int GST { get; set; }
        public string ImagePath { get; set; }
        public bool IsApproved { get; set; }
        public bool IsActive { get; set; }
        public long CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public long ModifiedBy { get; set; }
        public DateTime ModifiedDate { get; set; }
    }
}
