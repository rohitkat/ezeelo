using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models
{
    public partial class WarehouseBudget
    {
        public long ID { get; set; }
        public long WarehouseID { get; set; }
        public long SupplierID { get; set; }
        public decimal AmountPaid { get; set; }
        public decimal AmountBalance { get; set; }
        public decimal AmountRefund { get; set; }
        public decimal AmountAdvance { get; set; }
    }
}
