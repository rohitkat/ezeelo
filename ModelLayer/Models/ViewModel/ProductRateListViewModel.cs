using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models.ViewModel
{
    public class ProductRateListViewModel
    {
        public long ProductId { get; set; }
        public string ProductName { get; set; }
        public string ProductImgPath { get; set; }
        public string HSNCode { get; set; }
        public int VarientCount { get; set; }
        public bool IsActive { get; set; }
        public DateTime QuotationDate { get; set; }
        public long QuotationID { get; set; }
        public DateTime RateInsertDate { get; set; }
        public long RateCalcultionId { get; set; }
        public List<ProductVarientRateListViewModel> VarientList { get; set; }
    }

    public class ProductVarientRateListViewModel
    {
        public long ID { get; set; }
        public bool IsEditable { get; set; }
        /// .................Display Field...................//        
        public long ProductId { get; set; }
        public long ProductVarientId { get; set; }
        public string VarientName { get; set; }
        public bool IsFromQuotation { get; set; }
        public double MRP_ { get; set; }
        public double PurchaseRate { get; set; }
        public bool checkbox { get; set; }
        public double BaseInwardPriceEzeelopreGSt { get; set; }
        public double MaxInwardMargin { get; set; } //Yashaswi 6/6/2018
        public double MarginPassedToCustomer { get; set; } //Yashaswi 6/6/2018
        public double ActualFlatMargin { get; set; } //Yashaswi 21/6/2018

        /// .................Input Field...................//        
        public double MRP { get; set; }
        public int GSTInPer { get; set; }
        public double GrossMarginFlat { get; set; }
        public double DecidedSalePrice { get; set; }
        public DateTime RateExpiry { get; set; }
        public bool IsActive { get; set; } //Yashaswi 27/4/2018

        /// .................Calculated Field...................//
        public double ValuePostGST { get; set; }
        public double Dividend { get; set; }
        public double BaseInwardPriceEzeelo { get; set; }
        public double InwardMarginValue { get; set; }
        public double GSTOnPR { get; set; }


        public long DVId { get; set; }
        public string DVName { get; set; }
        public double Margin1 { get; set; }
        public double Margin2 { get; set; }
        public double Margin3 { get; set; }
        public double Margin4 { get; set; }
        public double Margin5 { get; set; }

        public bool RateMatrixExtensionIsActive { get; set; }
        public bool RateMatrixExtensionIsActiveModel { get; set; }
    }

    public class ProductVarientIdList
    {
        public long ProductVarientId { get; set; }
        public bool IsInsert { get; set; }
    }

    public class ProductVarientListDV
    {
        public long ProductId { get; set; }
        public string ProductName { get; set; }
        public long ProductVarientId { get; set; }
        public string ProductVarientName { get; set; }
        public long DVId { get; set; }
        public string DVName { get; set; }
        public long RateCalculationId { get; set; }
        public double MRP { get; set; }
        public double GST { get; set; }
        public double BaseInwardPriceEzeelo { get; set; }
        public string RateExpiryDate { get; set; }
        public double DecidedSalePrice { get; set; }
    }
}
