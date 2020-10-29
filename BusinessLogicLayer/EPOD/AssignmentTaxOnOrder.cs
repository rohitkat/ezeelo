using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer
{
    public class AssignmentTaxOnOrder
    {
        public long TaxOnOrderID { get; set; }
        public long CustomerOrderDetailID { get; set; }
        public long ProductTaxID { get; set; }
        public decimal TaxAmount { get; set; }
        public int TaxID { get; set; }
        public string TaxPrefix { get; set; }
        public string TaxName { get; set; }
    }
}
