using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models
{
    [Table("Network_Table")]
   public class Network_Table
    {
        [Key]
        public long UserID { get; set; }
        public string User_Name { get; set; }
        public string Referal_ID { get; set; }
        public double? Pts_Pending { get; set; }
        public string Email { get; set; }
        public DateTime? Joined_Date { get; set; }
        public DateTime? Last_Trx { get; set; }
        public string Network_Level { get; set; }
        public string Parent_Name { get; set; }
        public string Status { get; set; }
    }
}
