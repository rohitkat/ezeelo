using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models
{
    [Table("RecentlyViewProduct")]
    public class RecentlyViewProduct
    {
        public long Id { get; set; }
        public int FranchiseId { get; set; }
        public long UserLoginId { get; set; }
        public long ProductId { get; set; }
        public DateTime CreateDate { get; set; }
        public long CreateBy { get; set; }
        public DateTime? ModifyDate { get; set; }
        public long? ModifyBy { get; set; }
        public string DeviceType { get; set; }
        public string NetworkIP { get; set; }
        public string DeviceID { get; set; }
    }
}
