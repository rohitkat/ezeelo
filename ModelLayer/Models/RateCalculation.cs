using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models
{
    [Table("RateCalculation")]
    public class RateCalculation
    {
        [Key]
        public long ID { get; set; }
        public long ProductId { get; set; }
        public long ProductVarientId { get; set; }
        public double MRP { get; set; }
        public int GSTInPer { get; set; }
        public double GrossMarginFlat { get; set; }
        public double ActualFlatMargin { get; set; }  //21/6/2018
        public double DecidedSalePrice { get; set; }
        public double ValuePostGST { get; set; }
        public double Dividend { get; set; }
        public double BaseInwardPriceEzeelo { get; set; }
        public double InwardMarginValue { get; set; }
        public double GSTOnPR { get; set; }
        public double MaxInwardMargin { get; set; } //6/6/2018
        public double MarginPassedToCustomer { get; set; } //6/6/2018
        public bool IsActive { get; set; }
        public DateTime RateExpiry { get; set; }
        public long CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public long? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string NetworkIP { get; set; }
        public string DeviceType { get; set; }
        public string DeviceID { get; set; }
    }

    //Added by Zubair
    public class CurrentRate 
    {
        public long RateCalculationID { get; set; }
        public double Rate { get; set; }
        public double MRP { get; set; }
        public decimal TotalGSTInPer { get; set; }
        public double TotalGSTAmount { get; set; }
        public decimal CGSTInPer { get; set; }
        public decimal CGSTAmount { get; set; }
        public decimal SGSTInPer { get; set; }
        public decimal SGSTAmount { get; set; }
        public decimal IGSTInPer { get; set; }
        public decimal IGSTAmount { get; set; }
        public double DecidedSalePrice { get; set; }   //by Priti 
        public double GrossMarginFlat { get; set; }             //by Priti 


    }


}
