using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models.ViewModel
{
    public class DVFVRateExportListViewModel
    {
       // public long ProductID { get; set; }
       // public string ProductName { get; set; }
       // public string HSNCode { get; set; }
        public string VarientName { get; set; }
        public double MRP { get; set; }
        public double GST { get; set; }
        public double ValuePostGST { get; set; }
        public double GrossFlatMargin { get; set; }
        public double EzeeloPurchasePrice { get; set; }
        public DateTime RateExpiryDate { get; set; }
        public long DVId { get; set; }
        public string DVName { get; set; }
        public long FVId { get; set; }
        public string FVName { get; set; }
        public double FVMargin { get; set; }
        public double FVSalePrice { get; set; }
        public double FVPurchasePrice { get; set; }
        public double FVMarginValueGST { get; set; }
        public double FVGST { get; set; }
        public double DVMargin { get; set; }
        public double DVSalePrice { get; set; }
        public double DVPurchasePrice { get; set; }
        public double DVMarginValueGST { get; set; }
        public double DVGST { get; set; }
        public double EzeeloMargin { get; set; }
        public double EzeeloGST { get; set; }
        public double PostGSTMargin { get; set; }
        public double ForLeadership { get; set; }
        public double ForEzeelo { get; set; }
        public double ForLeadersRoyalty { get; set; }
        public double ForLifestyle { get; set; }
        public double ForLeadershipDevelopment { get; set; }
        public double BussinessPoints { get; set; }
        public double TotalGST { get; set; }

    }
}
