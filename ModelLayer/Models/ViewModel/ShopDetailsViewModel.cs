//-----------------------------------------------------------------------
// <copyright file="ShopDetailsViewModel" company="Ezeelo Consumer Services Pvt. Ltd.">
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
   public class ShopDetailsViewModel
    {

       public string ShopLogoPath { get; set; }
        public long ShopID { get; set; }
        public long BusinessDetailID { get; set; }
        public string Name { get; set; }
        public string Website { get; set; }
        public string Lattitude { get; set; }
        public string Longitude { get; set; }
        public string Address { get; set; }
        public string NearestLandmark { get; set; }
        public int PincodeID { get; set; }
        public string ShopDescription { get; set; }
        public Nullable<int> AreaID { get; set; }
        public Nullable<System.TimeSpan> OpeningTime { get; set; }
        public Nullable<System.TimeSpan> ClosingTime { get; set; }
        public string ContactPerson { get; set; }
        public string Email { get; set; }
        public string Mobile { get; set; }
        public string Landline { get; set; }
        public string FAX { get; set; }
        public string VAT { get; set; }
        public string TIN { get; set; }
        public string PAN { get; set; }
        public string WeeklyOff { get; set; }
        public bool CurrentItSetup { get; set; }
        public bool InstitutionalMerchantPurchase { get; set; }
        public bool InstitutionalMerchantSale { get; set; }
        public bool NormalSale { get; set; }
        public bool IsDeliveryOutSource { get; set; }
        public bool IsFreeHomeDelivery { get; set; }
        public decimal MinimumAmountForFreeDelivery { get; set; }           
        public string SearchKeywords { get; set; }
        public bool IsAgreedOnReturnProduct { get; set; }
        public int ReturnDurationInDays { get; set; }
        public string Pincode { get; set; }
        public string AreaName { get; set; }

        //public Nullable<int> DeliveryPartnerId { get; set; }
        //public Nullable<int> FranchiseID { get; set; }

     

    }
}
