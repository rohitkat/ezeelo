using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace ModelLayer.Models.ViewModel
{
    public class PurchaseOrderReplyViewModel
    {
        public long PurchaseOrderReplyID { get; set; }
        public long PurchaseOrderID { get; set; }
        public string PurchaseOrderCode { get; set; }
        public string InvoiceCode { get; set; }
        public int IsAcceptedBySupplier { get; set; }
        public Nullable<long> AcceptedByID { get; set; }      

        //[Required(ErrorMessage = "Please select invoice date")]
        public Nullable<System.DateTime> ReplyDate { get; set; }
        public string SupplierRemark { get; set; }
        public Nullable<decimal> TotalDiscountAmount { get; set; }

        public System.DateTime ExpetedDeliveryDate { get; set; }

        [Required(ErrorMessage = "Please select delivery date.")]
        public Nullable<System.DateTime> DeliveryDateTime { get; set; }

        [Required(ErrorMessage = "Please enter order amount")]
        public decimal OrderAmount { get; set; }
        public Nullable<decimal> ShippingCharge { get; set; }
        public Nullable<decimal> CustomDutyCharge { get; set; }
        public Nullable<decimal> OperatingCost { get; set; }
        public Nullable<decimal> AdditionalCost { get; set; }
        public decimal GSTAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public int TotalItems { get; set; }
        public string Remark { get; set; }
        public bool IsReplied { get; set; }
        public long RepliedBy { get; set; }

        [Required(ErrorMessage = "Please select Dispatch date!")]
        public Nullable<System.DateTime> DispatchDate { get; set; }

        [RegularExpression(@"^[a-zA-Z'.\s]{1,50}$", ErrorMessage = "Use Characters Only!")]
        public string DriverName { get; set; }

        [RegularExpression(@"^([0-9]{10})$", ErrorMessage = "Invalid Mobile Number!")]
        public string DriverMobileNumber { get; set; }

        [RegularExpression(@"^[a-zA-Z0-9.\s]{0,25}$", ErrorMessage = "Invalid Licence Number!")]
        public string DriverLicenceNumber { get; set; }

        [RegularExpression(@"^[a-zA-Z0-9\s]{0,25}$", ErrorMessage = "Invalid Vehicle Number!")]
        public string VehicleNumber { get; set; }

        [RegularExpression(@"^[a-zA-Z0-9\s]{0,50}$", ErrorMessage = "Invalid Vehicle Type!")]
        public string VehicleType { get; set; }

        [RegularExpression(@"^[a-zA-Z'.\s]{1,100}$", ErrorMessage = "Invalid Name!")]
        public string LogisticCompanyName { get; set; }
        public string LogisticCompanyAddress { get; set; }

        [RegularExpression(@"^[a-zA-Z'.\s]{1,50}$", ErrorMessage = "Use Characters Only!")]
        public string LogisticContactPerson { get; set; }

        [RegularExpression(@"^([0-9]{10})$", ErrorMessage = "Invalid Contact Number!")]
        public string LogisticContactNumber { get; set; }
        public string EWayBillNumber { get; set; }
        public string TrackingNumber { get; set; }
        public bool IsActive { get; set; }
        public System.DateTime CreateDate { get; set; }
        public long CreateBy { get; set; }
        public Nullable<System.DateTime> ModifyDate { get; set; }
        public Nullable<long> ModifyBy { get; set; }
        public string NetworkIP { get; set; }
        public string DeviceType { get; set; }
        public string DeviceID { get; set; }
        public long WarehouseID { get; set; }
        public string WarehouseName { get; set; }
        public long SupplierID { get; set; }
        public string SupplierName { get; set; }
        public string SupplierGSTNumber { get; set; }
        public string SupplierFax { get; set; }
        public string SupplierEmail { get; set; }
        public Nullable<long> ShopID { get; set; }
        public string ShopName { get; set; }
        public string FSSILicenseNo { get; set; }  ///Added by Priti
        public string InsecticidesLicenseNo { get; set; }    ///Added by Priti
        public string ShopAddress { get; set; }

        public string WarehouseContactPerson { get; set; }
        public string WarehouseAddress { get; set; }
        public string WarehouseMobile { get; set; }
        public string WarehosueEmail { get; set; }
        public string WarehousePincode { get; set; }

        public string SupplierCode { get; set; }
        public string SupplierContactPerson { get; set; }
        public string SupplierAddress { get; set; }
        public string SupplierMobile { get; set; }
        public string SupplierPincode { get; set; }
        public virtual ICollection<PurchaseOrderReplyAttachment> PurchaseOrderReplyAttachments { get; set; }
        public virtual ICollection<PurchaseOrderReplyDetail> PurchaseOrderReplyDetails { get; set; }
        //public virtual ICollection<InvoiceExtraItem> InvoiceExtraItems { get; set; }
        public List<PurchaseOrderReplyDetailViewModel> lPurchaseOrderReplyDetailViewModels { get; set; }
        public List<WarehouseReorderLevel> lWarehouseReorderLevels { get; set; }
        //yashaswi 2/8/2018 For GST Amount
        public List<GSTClass> lGst { get; set; }


        public string WarehouseGSTNumber { get; set; }   ///Added by Priti on 15/10/2018
        public string StateCode { get; set; }///Added by Priti on 16/10/2018
        public string StateCode1 { get; set; }///Added by Priti on 16/10/2018
        public string DVFSSILicenseNo { get; set; }  ///Added by Priti on 16/10/2018
        public double MRP { get; set; }   ///Added by Priti  on 3-11-2018
        public long BrandID { get; set; }  ///Added by Priti   on 3-11-2018
        public string BrandName { get; set; }///Added by Priti   on 3-11-2018
        public decimal POValue { get; set; }////Added by Priti    on 3-11-2018
        public string CreateByName { get; set; }    ///Added by Priti   on 9-11-2018
        public string ModifyByName { get; set; }    ///Added by Priti    on 9-11-2018
        public string SupplierPanNo { get; set; }         ///Added by Priti    on 9-11-2018



        public int? TINNumber { get; set; }   ///Added by Priti    on 9-11-2018

        public string Entity { get; set; }///Added by Priti
        public string WarehousePanNo { get; set; }         ///Added by Priti on 11-3-2019
        public long DVId { get; set; }
        public long FVId { get; set; }

    }

    //yashaswi 2/8/2018 For GST Amount
    public class GSTClass
    {
        public decimal? TaxableAmt { get; set; }
        public decimal? GST { get; set; }
        public double? CGST { get; set; }
        [DisplayFormat(DataFormatString = "{0:0.0000}", ApplyFormatInEditMode = true)]  // added by priti
        public decimal? CGSTAmt { get; set; }
        public double? SGST { get; set; }
        public decimal? SGSTAmt { get; set; }
        public decimal? TaxAmt { get; set; }
        public decimal TotalAmt { get; set; }
        public decimal totalGst { get; set; }   //added by Priti on 16-10-2018
        //public double? CGST { get; set; }
        public Nullable<int> GSTInPer { get; set; }//added by Priti on 16-10-2018
    }
}
