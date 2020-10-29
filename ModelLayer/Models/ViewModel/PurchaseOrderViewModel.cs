using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models.ViewModel
{
   public class PurchaseOrderViewModel
    {
        
        public string ProductVarient { get; set; } //Added by Rumana on 17-05-2019
        public long PurchaseOrderID { get; set; }
        public string PurchaseOrderCode { get; set; }
        public long InvoiceID { get; set; }
        public string InvoiceCode { get; set; }

        [Required(ErrorMessage = "Please select Warehouse name")]
        public long WarehouseID { get; set; }

        [Required(ErrorMessage = "Please select Supplier name")]
        public long SupplierID { get; set; }
        public string SupplierEmail { get; set; }   ///Added by Priti on 9/10/2018

        public string SContactPerson { get; set; } ///Added by Priti on 9/10/2018
        public long? PaymentTermsID { get; set; } ///Added by Priti on 9/10/2018                                       ///
        public string PaymentInTermsname { get; set; } ///Added by Priti on 9/10/2018
                                                         
        [Required(ErrorMessage = "Please select Order Date")]
        public System.DateTime OrderDate { get; set; }
        public System.DateTime ReceivedDate { get; set; }       
        public Nullable<decimal> Quantity { get; set; }
        public string Nickname { get; set; }
        public decimal StockIn { get; set; }
        public decimal BuyRatePerUnit { get; set; }////aDDED BY Priti
        public int? TINNumber { get; set; }///aDDED BY Priti
        public long? SKUID { get; set; }////Added by Priti
        public Nullable<int> GSTInPer { get; set; }////Added by Priti
        public Nullable<decimal> GSTAmount { get; set; }////Added by Priti
        public Nullable<decimal> CGSTAmount { get; set; }////Added by Priti
        public Nullable<decimal> SGSTAmount { get; set; }////Added by Priti
        public string FSSILicenseNo { get; set; }  ///Added by Priti
        public string WarehouseGSTNumber { get; set; } ///Added by Priti
        public string SupplierGSTNumber { get; set; }///Added by Priti
        public string PANNumber { get; set; }///Added by Priti
        public string SupplierPanNumber { get; set; }///Added by Priti
        public string StateCode { get; set; }///Added by Priti
        public string StateCode1 { get; set; }///Added by Priti
        public long? StateID { get; set; }///Added by Priti

        public string Remark { get; set; }
     
        [Required(ErrorMessage = "Please select Expected Delivery Date")]
        public System.DateTime ExpetedDeliveryDate { get; set; }
        public Nullable<decimal> Amount { get; set; }
        public Boolean IsSent { get; set; }
        public int IsAcceptedBySupplier { get; set; }
        public Nullable<long> AcceptedByID { get; set; }      

        //[Required(ErrorMessage="Please select delivery date.")]
        public Nullable<DateTime> DeliveryDate { get; set; }
        public string ItemName { get; set; }
        public string HSNCode { get; set; }

        public Nullable<decimal> UnitPrice { get; set; }
        public Nullable<decimal> MRP { get; set; }
        public double EzeeloMRP { get; set; }
        public double ? DecidedSalePrice { get; set; }   //Added by Priti on 2-3-2019
        public double? GrossMarginFlat { get; set; }//Added by Priti on 2-3-2019

        public int InvoicedQuantity { get; set; }
        public int GRNQuantity { get; set; }
        public long RateCalculationID { get; set; }

        //Start Yashaswi 10-4-2019
        public long RateMatrixExtensionId { get; set; }
        public long FVId { get; set; }
        public long DVId { get; set; }
        //End by Yashaswi 10-4-2019

        public Nullable<decimal> ShippingCharge { get; set; }
        public Nullable<decimal> CustomDutyCharge { get; set; }
        public Nullable<decimal> OperatingCost { get; set; }
        public decimal TotalAmount { get; set; }
        public Boolean IsActive { get; set; }
        public System.DateTime CreateDate { get; set; }
        public long? CreateBy { get; set; }
        public string CreateByName { get; set; }  //Added by Priti
        public string ModifyByName { get; set; }   //Added by Priti
        public Nullable<System.DateTime> ModifyDate { get; set; }
        public Nullable<long> ModifyBy { get; set; }
        public string NetworkIP { get; set; }
        public string DeviceType { get; set; }
        public string DeviceID { get; set; }
        public string ProductName { get; set; }
        public string WarehouseName { get; set; }
        public string SupplierName { get; set; }
        public string Items { get; set; }
        public int TotalItems { get; set; }
        public decimal OrderAmount { get; set; }
        public int OrderedQuantity { get; set; }
        public int ReceivedQuantity { get; set; }
        public string WarehouseContactPerson { get; set; }
        public string WarehouseAddress { get; set; }
        public string WarehouseMobile { get; set; }
        public string WarehousePincode { get; set; }

        public string SupplierCode { get; set; }
        public string SupplierContactPerson { get; set; }      
        public string SupplierAddress { get; set; }
        public string SupplierMobile { get; set; }
        public string SupplierPincode { get; set; }
        public int InvoiceCount { get; set; }       
       public Nullable<int> ddlParentCatetory { get; set; }
       public PurchaseOrder lPurchaseOrder { get; set; }
       public PurchaseOrderDetail lPurchaseOrderDetail { get; set; }
       public UnitCalculation lUnitCalculation { get; set; }
       public WarehouseStock lWarehouseStock { get; set; }
       public Category lCatetory { get; set; }   

       public List<ParentCategory> ParentCategoryList { get; set; }

       public List<PurchaseOrderDetailsViewModel> lPurchaseOrderDetailsViewModels { get; set; }


        public string Entity { get; set; }  // by Priti
        public bool IsFulfillmentCenter { get; set; }// Added by Priti 
        public string Franchises { get; set; }// Added by Priti 
        public string City { get; set; }// Added by Priti 

    }

   public class ParentCategory
   {      
       public int ParentCategoryID { get; set; }
       public string ParentCategoryName { get; set; }
       public Boolean IsSelected { get; set; }
   }  

    public class PurchaseOrderViewModelList
    {
        public List<PurchaseOrderViewModel> lPurchaseOrderViewModel = new List<PurchaseOrderViewModel>();
        public long SupplierID { get; set; }
        public string SupplierName { get; set; }
        public long WarehouseID { get; set; }
        public string WarehouseName { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }

    }

}
