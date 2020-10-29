using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;

namespace ModelLayer.Models
{
    [Table("CancelOrderReason")]

    public class CancelOrderReason
    {
        public int Id { get; set; }
        
        public string Reason { get; set; }
    }
}
