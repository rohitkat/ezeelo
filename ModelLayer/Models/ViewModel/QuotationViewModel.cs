using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models.ViewModel
{
    public class QuotationViewModel
    {
        public long QuotationID { get; set; }
        public long QuotationSupplierListID { get; set; }
        public string QuotationCode { get; set; }

        [Required(ErrorMessage = "Please select Warehouse name")]
        public long RequestFromWarehouseID { get; set; }
        public System.DateTime ExpectedReplyDate { get; set; }
        public bool IsSent { get; set; }
        public bool IsReplied { get; set; }
        public int ReplyItemCount { get; set; }

        [Required(ErrorMessage = "Please select Supplier name")]
        public long SupplierID { get; set; }
        public System.DateTime QuotationRequestDate { get; set; }
        public Nullable<System.DateTime> QuotationReplyDate { get; set; }
        public Nullable<decimal> Quantity { get; set; }
        public string Nickname { get; set; }
        public string Remark { get; set; }
        public Nullable<decimal> Amount { get; set; }
        public Nullable<decimal> GSTAmount { get; set; }
        public Nullable<decimal> UnitPrice { get; set; }
        public Nullable<decimal> ShippingCharge { get; set; }
        public Nullable<decimal> CustomDutyCharge { get; set; }
        public Nullable<decimal> OperatingCost { get; set; }
        public Nullable<decimal> AdditionalCost { get; set; }
        public Nullable<decimal> CGSTAmount { get; set; }
        public Nullable<decimal> SGSTAmount { get; set; }
        public Nullable<decimal> IGSTAmount { get; set; }
        public Nullable<decimal> TotalAmount { get; set; }
        public Boolean IsActive { get; set; }
        public System.DateTime CreateDate { get; set; }
        public long CreateBy { get; set; }
        public Nullable<System.DateTime> ModifyDate { get; set; }
        public Nullable<long> ModifyBy { get; set; }
        public string NetworkIP { get; set; }
        public string DeviceType { get; set; }
        public string DeviceID { get; set; }
        public Nullable<long> RepliedBy { get; set; }
        public string ProductName { get; set; }
        public string WarehouseName { get; set; }
        public string SupplierName { get; set; }
        public string Items { get; set; }
        public int TotalItems { get; set; }
        public decimal OrderAmount { get; set; }
        public string WarehouseContactPerson { get; set; }
        public string WarehouseAddress { get; set; }
        public string WarehouseMobile { get; set; }
        public string WarehousePincode { get; set; }
        public string WarehosueEmail { get; set; }
        public string WarehouseGSTNumber { get; set; }
        public string WarehouseFax { get; set; }
        public string SupplierCode { get; set; }
        public string SupplierContactPerson { get; set; }
        public string SupplierAddress { get; set; }
        public string SupplierMobile { get; set; }
        public string SupplierFax { get; set; }
        public string SupplierPincode { get; set; }
        public string SupplierEmail { get; set; }
        public string SupplierGSTNumber { get; set; }
        public Nullable<int> ddlParentCatetory { get; set; }
        public Quotation lQuotation { get; set; }
        public QuotationItemDetail lQuotationItemDetail { get; set; }
        public Category lCatetory { get; set; }
        public List<ParentCategory> ParentCategoryList { get; set; }
        public List<QuotationItemDetailViewModel> lQuotationItemDetailViewModels { get; set; }
        public List<SupplierModel> SupplierList { get; set; }
        public List<QuotationSupplierList> lQuotationSupplierList { get; set; }
        public List<QuotationSupplierListViewModel> lQuotationSupplierListViewModel { get; set; }
    }


    public class QuotationViewModelList
    {
        public List<QuotationViewModel> lQuotationViewModel = new List<QuotationViewModel>();
        public long WarehouseID { get; set; }
        public string WarehouseName { get; set; }
    }

}
