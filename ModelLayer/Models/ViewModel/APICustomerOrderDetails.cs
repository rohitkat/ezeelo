using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models.ViewModel
{
    public class APICustomerOrderDetails
    {
        [Required(ErrorMessage = "Please provide Shop Stock ID")]
        public long ShopStockID { get; set; }
        [Required(ErrorMessage = "Please provide Shop ID")]
        public long ShopID { get; set; }
        //[Required(ErrorMessage = "Please provide Qty")]
        public int Qty { get; set; }
        //public int OrderStatus { get; set; }
        //[Required(ErrorMessage = "Please provide product MRP")]
        //public decimal MRP { get; set; }
        //[Required(ErrorMessage = "Please provide product SaleRate")]
        //public decimal SaleRate { get; set; }
        //public decimal OfferPercent { get; set; }
        //public decimal OfferRs { get; set; }
        //[Required]
        //public bool IsInclusivOfTax { get; set; }
        //[Required(ErrorMessage = "Please provide total Amount")]
        //public decimal TotalAmount { get; set; }

        /*************************
         * Taxes Apply on Product
         * Pradnyakar Badge
         * 29-03-2015
         *************************/
        //public List<CalulatedTaxesRecord> TaxesOnProduct { get; set; }
    }
}
