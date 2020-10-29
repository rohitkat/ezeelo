//-----------------------------------------------------------------------
// <copyright file="CustomerOrderDetailViewModel" company="Ezeelo Consumer Services Pvt. Ltd.">
//     Copyright (c) Ezeelo Consumer Services Pvt. Ltd. All rights reserved.
// </copyright>
// <author>Sujata Kullarkar</author>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models.ViewModel
{
    public class CustomerOrderDetailViewModel
    {
        public long ID { get; set; }
        public string StockThumbPath { get; set; }
        public long ProductID { get; set; }
        public string ProductName { get; set; }
        public string HSNCode { get; set; }
        public string ColorCode { get; set; }
        public string ColorName { get; set; }
        public string SizeName { get; set; }
        public string DimensionName { get; set; }
        public string MaterialName { get; set; }
        public long CustomerOrderDetailID { get; set; }
        public string ShopOrderCode { get; set; }
        public string OrderCode { get; set; }
        public long CustomerOrderID { get; set; }
        public long ShopStockID { get; set; }
        public long ShopID { get; set; }
        public string ShopName { get; set; }
        public decimal PackSize { get; set; }
        public string PackUnitName { get; set; }
        public int Qty { get; set; }
        public int OrderStatus { get; set; }
        public string OrderStatusName { get; set; }
        public decimal MRP { get; set; }
        public decimal SaleRate { get; set; }
        public decimal OfferPercent { get; set; }
        public decimal OfferRs { get; set; }
        public bool IsInclusivOfTax { get; set; }
        public decimal TotalAmount { get; set; }
        //Added by Tejaswee
        public int StockQty { get; set; }
        public bool StockStatus { get; set; }
        public bool IsShopLive { get; set; }
        public bool IsProductLive { get; set; }
        public Nullable<decimal> BusinessPointPerUnit { get; set; } //Added by Zubair for MLM on 06-01-2018
        public Nullable<decimal> BusinessPoints { get; set; } //Added by Zubair for MLM on 06-01-2018
        public decimal CashbackPointPerUnit { get; set; }
        public decimal CashbackPoint { get; set; }
        //public bool Is { get; set; }

        /*Added By Pradnyakar Badge
         * 31-03-2016
         * For Taxtion Detail On Product Purchase
         */
        public List<CalulatedTaxesRecord> TaxesOnProduct { get; set; }

        //Added by Zubair on 11-07-2018 for picklist
        public long LocationID { get; set; }
        public string Location { get; set; }
        public string BatchCode { get; set; }
        public long WarehouseStockID { get; set; }
        //End


    }
}
