using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models
{
    [Table("TrackSearch_Keywords")]

   public class TrackSearch_Keywords
    {

        [Key]
        public long ID { get; set; }
        public long UserloginID { get; set; }
        public string Keyword { get; set; }
        public int? City_ID { get; set; }
        public int? Franchise_ID { get; set; }
        public int? Category_ID { get; set; }
        public string Network_IP { get; set; }
        public string Device_ID { get; set; }
        public DateTime? Create_Date { get; set; }
        public bool? IsResult { get; set; }
        public string DeviceType { get; set; }
    }
}
