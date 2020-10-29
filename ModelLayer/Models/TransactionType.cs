using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models
{
    public partial class TransactionType
    {
        public TransactionType()
        {
            this.WarehouseFinancialTransactions = new HashSet<WarehouseFinancialTransaction>();
        }

        public int ID { get; set; }
        public string Name { get; set; }

        public virtual ICollection<WarehouseFinancialTransaction> WarehouseFinancialTransactions { get; set; }
    }
}
