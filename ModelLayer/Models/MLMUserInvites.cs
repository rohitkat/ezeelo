using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models
{
    [Table("MLMUserInvites")]
    public class MLMUserInvites
    {
        [Key]
        public long Id { get; set; }
        public long UserLoginID { get; set; }
        public string InviteID { get; set; }
        public string Name { get; set; }
        public string Mobile { get; set; }
        public string Message { get; set; }
        public string Email { get; set; }
        public string InviteOffer { get; set; }
        public bool IsAccepted { get; set; }
        public DateTime CreateDate { get; set; }
        public long CreateBy { get; set; }

    }
}
