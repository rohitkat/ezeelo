using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models
{
    [Table("RateMatrix")]
    public class RateMatrix
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

    [Table("RateMatrixExtension")]
    public class RateMatrixExtension
    {
        [Key]
        public long ID { get; set; }
        public long EVWID { get; set; }
        public long RateMatrixId { get; set; }
        public long ProductVarientId { get; set; }

        public long FVID { get; set; }
        public double FVMargin { get; set; }
        public double FVSalePrice { get; set; }
        public double FVPurchasePrice { get; set; }
        public double FVMarginValueWithGST { get; set; }
        public double FVGST { get; set; }

        public long DVID { get; set; }
        public double DVMargin { get; set; }
        public double DVSalePrice { get; set; }
        public double DVPurchasePrice { get; set; }
        public double DVMarginValueWithGST { get; set; }
        public double DVGST { get; set; }

        public double MarginLeftForEzeeloBeforeLeadershipPayout { get; set; }
        public double GSTForEzeeloMargin { get; set; }
        public double PostGSTMargin { get; set; }
        public double ForLeadershipPercent { get; set; }
        public double ForLeadershipValue { get; set; }
        public double ForEzeeloPercent { get; set; }
        public double ForEzeeloValue { get; set; }
        public double ForLeadersRoyaltyPercent { get; set; }
        public double ForLeadersRoyaltyValue { get; set; }
        public double ForLifestyleFundPercent { get; set; }
        public double ForLifestyleFundValue { get; set; }
        public double ForLeadershipDevelopmentFundPercent { get; set; }
        public double ForLeadershipDevelopmentFundValue { get; set; }
        public double RetailPoint { get; set; }
        public double TotalGSTInSupplyChain { get; set; }
        public double TotalMargin { get; set; }
        public double OneBPInPaise { get; set; }

        public bool IsActive { get; set; }
        public DateTime CreateDate { get; set; }
        public long CreateBy { get; set; }
        public DateTime? ModifyDate { get; set; }
        public long? ModifyBy { get; set; }
        public string NetworkIP { get; set; }
    }

    public class Rate
    {
        public double PurchaseRate { get; set; }
        public double MRP { get; set; }
        public double FlatMargin { get; set; }
        public double DecidedSalePrice { get; set; }
        public long RateMatrixId { get; set; }
        public long RateMatrixExtensionId { get; set; }
        public double SaleRate { get; set; }
        public double GST { get; set; }
        public double TotalGST { get; set; }
    }
}
