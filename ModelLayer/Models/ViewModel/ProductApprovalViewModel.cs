//-----------------------------------------------------------------------
// <copyright file="ProductApprovalViewModel.cs" company="Ezeelo Consumer Services Pvt. Ltd.">
//     Copyright (c) Ezeelo Consumer Services Pvt. Ltd. All rights reserved.
// </copyright>
// <author>Snehal Shende</author>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models.ViewModel
{
    public class ProductApprovalViewModelList
    {
        public List<ProductApprovalViewModel> ProductUploadTempViewModelLIst { get; set; }
        public List<ProductVarient> TempProductVarientList { get; set; }
        public List<ShopStock> TempShopStockList { get; set; }
    }
    public class ProductApprovalViewModel
    {
        // Product
        public long ID { get; set; }
        public string Name { get; set; }
        public int CategoryID { get; set; }
        public int WeightInGram { get; set; }
        public int LengthInCm { get; set; }
        public int BreadthInCm { get; set; }
        public int HeightInCm { get; set; }
        public string Description { get; set; }
        public int BrandID { get; set; }
        public string SearchKeyword { get; set; }
        public bool IsActive { get; set; }
        public System.DateTime CreateDate { get; set; }
        public long CreateBy { get; set; }
        public Nullable<System.DateTime> ModifyDate { get; set; }
        public Nullable<long> ModifyBy { get; set; }
        public string NetworkIP { get; set; }
        public string DeviceType { get; set; }
        public string DeviceID { get; set; }
        public virtual Brand Brand { get; set; }
        public virtual Category Category { get; set; }

        // Product Variant
        public long ProductVarientID { get; set; }
        public int ColorID { get; set; }
        public int SizeID { get; set; }
        public int DimensionID { get; set; }
        public int MaterialID { get; set; }

        // Shop Product
        public long ShopProductID { get; set; }
        public long ShopID { get; set; }
        public System.DateTime DisplayProductFromDate { get; set; }

        // Shop Stock
        public long ShopStockID { get; set; }
        //public long ShopProductID { get; set; }
        public int Qty { get; set; }
        public int ReorderLevel { get; set; }
        public bool StockStatus { get; set; }
        public decimal PackSize { get; set; }
        public int PackUnitID { get; set; }
        public decimal MRP { get; set; }
        public decimal WholeSaleRate { get; set; }
        public decimal RetailerRate { get; set; }
        public bool IsInclusiveOfTax { get; set; }
        //Category
        public string CategoryName { get; set; }
        //Brand
        public string BrandName { get; set; }
        //StockComponent
        public long StockComponentID { get; set; }
        public int ComponentID { get; set; }
        public decimal ComponentWeight { get; set; }
        public int ComponentUnitID { get; set; }
    }
}
