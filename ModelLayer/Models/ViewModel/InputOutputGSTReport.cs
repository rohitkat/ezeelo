using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models.ViewModel
{
    public class InputOutputGSTReport
    {        
        public DateTime? Date { get; set; }
        public string Invoice_No { get; set; }
        public string OrderCode { get; set; }
        public string GST_No  { get; set; }
        public string Items { get; set; }
        public int? GST { get; set; }
        public decimal? MRP { get; set; }
        public decimal Buy_rate_per_unit { get; set; }
        public decimal Sale_rate_per_unit { get; set; }      
        public decimal Base_rate_per_unit { get; set; }
        public decimal GST_per_unit { get; set; }
        public int Qty { get; set; }
        public decimal Total_GST_Amt { get; set; }
        public long WarehouseId { get; set; }
        public long WarehouseStockId { get; set; }
        public long CustomerOrderDetailID { get; set; }
        public bool IsSelect { get; set; }
        public decimal? GSTPart { get; set; }
        public List<InputOutputGSTReport> list { get; set; }
    }

   
    public class GSTReport
    {       
        public int? OutputGSTHeads { get; set; }
        public decimal OutputGSTAmount { get; set; }
        public int? InputGSTHeads { get; set; }
        public decimal InputGSTAmount { get; set; }
        public decimal Balance { get; set; }
    }
}
