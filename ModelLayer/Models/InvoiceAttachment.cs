using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models
{
    [Table("InvoiceAttachment")]
    public class InvoiceAttachment
    {
        [Key]
        public long ID { get; set; }
        public long InvoiceID { get; set; }
        public string FileName { get; set; }
        public string Extention { get; set; }
        public bool IsActive { get; set; }
    }
}
