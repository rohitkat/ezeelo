using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models
{
  public class StockComponentPriceLog
    {
        public int ID { get; set; }
        public long ShopStockID { get; set; }
        public decimal OldSaleRate { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public long CreatedBy { get; set; }
        public string NetworkIP { get; set; }
        public string DeviceType { get; set; }
        public string DeviceID { get; set; }
        public virtual ShopStock ShopStock { get; set; }
    }
}
