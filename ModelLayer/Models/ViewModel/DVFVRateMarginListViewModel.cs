using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models.ViewModel
{
    public class DVRateMarginListViewModel
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public bool IsFV { get; set; }
        public double Margin { get; set; }
        public double SalePrice { get; set; }
        public double PurchasePrice { get; set; }
        public List<FVRateMarginListViewModel> FVList { get; set; }

        public long RateMatrixId { get; set; }
        public double MRP { get; set; }
        public double GST { get; set; }
        public double EzPurchasePrice { get; set; }
        public double EzSalePrice { get; set; }
        public DateTime RateExpiry { get; set; }
        public double GrossMarginFlat { get; set; }
        public double ValuePostGST { get; set; }
        public double MarginPassedToCustomer { get; set; }
        public double MaxInwardMargin { get; set; }
        public double ActualFlatMargin { get; set; }

        public bool IsEditable { get; set; }
    }
    public class FVRateMarginListViewModel
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public bool IsFV { get; set; }

        public long DVId { get; set; }
        public double FVMargin { get; set; }
        public double FVSalePrice { get; set; }
        public double FVPurchasePrice { get; set; }
        public double FVMarginVAlueWithGST { get; set; }
        public double FVGST { get; set; }

        public double DVMargin { get; set; }
        public double DVSalePrice { get; set; }
        public double DVPurchasePrice { get; set; }
        public double DVMarginVAlueWithGST { get; set; }
        public double DVGST { get; set; }
        public double DVClaimAmt { get; set; }

        public double EzeeloMargin { get; set; }
        public double EzeeloGST { get; set; }
        public double PostGSTMargin { get; set; }
        public double ForLeadershipPer { get; set; }
        public double ForEzeeloPer { get; set; }
        public double ForLeadersRoyaltyPer { get; set; }
        public double ForLifestylePer { get; set; }
        public double ForLeadershipDevelopmentPer { get; set; }
        public double ForLeadership { get; set; }
        public double ForEzeelo { get; set; }
        public double ForLeadersRoyalty { get; set; }
        public double ForLifestyle { get; set; }
        public double ForLeadershipDevelopment { get; set; }
        public double BussinessPoints { get; set; }
        public double TotalGST { get; set; }
        public double TotalMargin { get; set; }
        public bool IsActive { get; set; }

        public double claimAmount { get; set; }
    }
    public class ProductRateMarginListViewModel
    {
        public long ProductId { get; set; }
        public string ProductName { get; set; }
        public string HSNCode { get; set; }
        public List<ProductVarientRateMarginListViewModel> PVarientList { get; set; }
    }

    public class ProductVarientRateMarginListViewModel
    {
        public long RateMatrixId { get; set; }
        public long ProductVarientId { get; set; }
        public string ProductVarientName { get; set; }
        public double MRP { get; set; }
        public double GST { get; set; }
        public double PurchasePrice { get; set; }
        public double SalePrice { get; set; }
        public DateTime RateExpiry { get; set; }
        public double GrossMarginFlat { get; set; }
        public double ValuePostGST { get; set; }
        public double MarginPassedToCustomer { get; set; }
        public double MaxInwardMargin { get; set; }
        public double ActualFlatMargin { get; set; }
        public bool IsActive { get; set; }
        public List<DVRateMarginListViewModel> DVList { get; set; }
    }



}
