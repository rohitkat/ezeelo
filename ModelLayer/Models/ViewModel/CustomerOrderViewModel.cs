//-----------------------------------------------------------------------
// <copyright file="CustomerOrderViewModel" company="Ezeelo Consumer Services Pvt. Ltd.">
//     Copyright (c) Ezeelo Consumer Services Pvt. Ltd. All rights reserved.
// </copyright>
// <author>Sujata Kullarkar</author>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models.ViewModel
{
    public class CustomerOrderViewModel
    {
        public long CustomerOrderID { get; set; }
        public string OrderCode { get; set; }
        public decimal OrderAmount { get; set; }
        public Nullable<int> NoOfPointUsed { get; set; }
        public Nullable<decimal> ValuePerPoint { get; set; }
        public string CoupenCode { get; set; }
        public Nullable<decimal> CoupenAmount { get; set; }
        public string PAN { get; set; }
        public string PaymentMode { get; set; }
        public decimal TotallDeliveryCharge { get; set; }
        public decimal PayableAmount { get; set; }
        public bool IsCancellable { get; set; }
        public string PrimaryMobile { get; set; }
        public string SecondoryMobile { get; set; }
        public string ShippingAddress { get; set; }
        public int PincodeID { get; set; }
        public string Pincode { get; set; }
        public string CityName { get; set; }
        public string AreaName { get; set; }
        public Nullable<int> AreaID { get; set; }
        public System.DateTime OrderDate { get; set; }
        public string EmailID { get; set; }
        public string FullName { get; set; }
        public decimal? UsedWalletAnount { get; set; }
        public string ShopName { get; set; }
        public List<CustomerOrderDetailViewModel> OrderProducts { get; set; }//// Added by Ashwani for GetOrderDetails API
        public decimal BusinessPointsTotal { get; set; }  //Added by Zubair for MLM on 06-01-2018
        public Nullable<decimal> MLMAmountUsed { get; set; }  //Added by Zubair for MLM on 06-01-2018
        public decimal CashbackPoints { get; set; }

        public string FirstName { get; set; }  // added by amit on 11/7/2018 for MLM Customer Orders
        public string MiddelName { get; set; } // added by amit on 11/7/2018 for MLM Customer Orders
        public string LastName { get; set; } // added by amit on 11/7/2018 for MLM Customer Orders

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime? CreateDate { get; set; }  // added by amit on 11/7/2018 for MLM Customer Orders
        public long? CreateBy { get; set; }  // added by amit on 11/7/2018 for MLM Customer Orders


        public decimal BusinessValue { get; set; } // added by amit on 30/7/2018 for BusinessValue Report
        public long UserLoginID { get; set; }
        public long TransactionTypeID { get; set; }
        public decimal TransactionPoints { get; set; }
        public long MLMCoinRateID { get; set; }
        public string OrderType { get; set; }

        public string Reference { get; set; }
        // public long? CustomerOrderID { get; set; }
        public bool? IsAdded { get; set; }
        public decimal CurrentWalletAmount { get; set; }
        public long? AddOrSub { get; set; }
        public decimal? WalletAmountUsed { get; set; }

        public string IsMlm { get; set; }  // added by amit 
        public string GSTNo { get; set; }//Sonali_ForApi_30-10-2018
        public int OrderStatus { get; set; }//Sonali_ForApi_03-12-2018
        public string OrderStatusName { get; set; }//Sonali_ForApi_03-12-2018
        public decimal TotalSGST { get; set; }//Sonali_ForApi_22-12-2018
        public decimal TotalCGST { get; set; }//Sonali_ForApi_22-12-2018
        public decimal TotalIGST { get; set; }//Sonali_ForApi_22-12-2018
        public decimal TotalDiscount { get; set; }//Sonali_ForApi_22-12-2018
    }


}


