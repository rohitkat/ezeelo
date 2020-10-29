using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace ModelLayer.Models
{
    [Table("MarketPlaceEZProductGallery")]
    public class MarketPlaceEZProductGallery
    {
        [Key]
        public long ProductId { get; set; }
        public string ProductName { get; set; }
        public string ShortDescription { get; set; }
        public string GeneralDescription { get; set; }
        public string SearchKeyword { get; set; }
        public string HSNCode { get; set; }
        public string EANCode { get; set; }
        public long Category { get; set; }
        public bool IsActive { get; set; }
        public long CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public long ModifiedBy { get; set; }
        public DateTime ModifiedDate { get; set; }
        public  List<MarketPlaceEZProductVarient> VarientList { get; set; }
    }

    [Table("MarketPlaceEZProductVarient")]
    public class MarketPlaceEZProductVarient
    {
        [Key]
        public long Id { get; set; }
        public long ProductId { get; set; }
        public long SizeId { get; set; }
        public SelectList size { get; set; }
        public decimal MRP { get; set; }
        public decimal SaleRate { get; set; }
        public int GST { get; set; }
        public string  ImagePath { get; set; }
        public bool IsActive { get; set; }
        public long CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public long ModifiedBy { get; set; }
        public DateTime ModifiedDate { get; set; }
    }
}
