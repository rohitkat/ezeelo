using System;
using System.Collections.Generic;

using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace ModelLayer.Models.ViewModel
{
     public  class SampleofFVSaleMarginProduct_WiseReportViewModel
    {

        public int FranchiseID { get; set; }
        public long UserLoginID { get; set; }
        public long ID { get; set; }
        public long SKUID { get; set; }
        public long CustomerOrderID { get; set; }
        public string Customer { get; set; }


        public string Email { get; set; }
        public string Mobile { get; set; }
        public long ShopID { get; set; }

        public string Shop { get; set; }
        public string Address { get; set; }
        public long CityID { get; set; }
        public string City { get; set; }
        public string PincodeID { get; set; }
        public string Product { get; set; }

        public string Category3 { get; set; }
        public string Category2 { get; set; }
        public string Category1 { get; set; }
        public int Qty { get; set; }
        public string Size { get; set; }
        public decimal SaleRate { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime CreateDate { get; set; }


        public string HSNCode { get; set; }
        public string OrderCode { get; set; }
        public long CustomerOrderDetailID { get; set; }



        public string ShopOrderCode { get; set; }

        public string Status { get; set; }

        [Display(Name = "Joining DATE")]

        public DateTime JoiningDate { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd/mm/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime LastPurchaseDate { get; set; }
        public decimal TotalGstAmount { get; set; }
        public string PaymentMode { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd/mm/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime DeliveryDate { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/mm/yyyy}", ApplyFormatInEditMode = true)]

        public DateTime OrderPlacedDate { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd/mm/yyyy}", ApplyFormatInEditMode = true)]

        public string DeliveryType { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd/mm/yyyy}", ApplyFormatInEditMode = true)]

        public string SKUUnit { get; set; }
        public string ShippingAddress { get; set; }
        public string AddressFieldMobile { get; set; }
        public decimal GrossSaleAmount { get; set; }
        public decimal CGSTAmt { get; set; }
        public decimal SGSTAmt { get; set; }

        [Display(Name = "GST%")]
        public decimal GSTPercentage { get; set; }

        //[Display(Name = "GST%")]
        //public decimal CGST { get; set; }



        public string FromDate { get; set; }
        public string ToDate { get; set; }
        /// <summary>
        /// fv sale margin new column 
        /// </summary>
        public decimal? SaleRatePerUnit { get; set; }  ///add by priti 
        public decimal? DeliveryCharge { get; set; }
        public string BatchCode { get; set; }
        public int InitialQuantity { get; set; }
        public int AvailableQuantity { get; set; }
        public  decimal? MLMAmountUsed { get; set; }
        public string BrandName { get; set; }
        public decimal ? MRP { get; set; }
        public decimal ? Retailpoint { get; set; }
        public string ProductName { get; set; }
        public string SKUName { get; set; }

         public long productID { get; set; }
        public decimal? PurchaseAmount { get; set; }
        public  DateTime? ReceivedDate { get; set; }
     

        public string DVInvoiceCode { get; set; }
       
        public DateTime? DVInvoiceDate { get; set; }
      

        public DateTime? RecivedDate { get; set; }
    

        public string POCode { get; set; }
        public decimal? BuyRatePerUnit { get; set; }
        public decimal? MarginAmount { get; set; }
        public string SupplierName { get; set; }




    }
    public class SampleofFVSaleMarginProduct_WiseReportViewModelList
    {
        public List<SampleofFVSaleMarginProduct_WiseReportViewModel> lSampleofFVSaleMarginProduct_WiseReportViewModel = new List<SampleofFVSaleMarginProduct_WiseReportViewModel>();
        public int FrenchiseID { get; set; }
        public string FrenchiseName { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }



        //public Nullable<System.DateTime> FromDate_ { get; set; }  //ADDED FOR FITER
        //public Nullable<System.DateTime> ToDate_ { get; set; }  //ADDED FOR FITER
        //public SelectList WarehouseList { get; set; }
    }


}



