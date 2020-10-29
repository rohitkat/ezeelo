using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models.ViewModel
{
   public class CustomerClassificationViewModel    {
      
       public string InitialFromDate { get; set; }
       public string FromDate { get; set; }
       public string ToDate { get; set; }
       public int CityID { get; set; }
       public int PincodeID { get; set; }
       public virtual List<CustomerClassificationFirstLevelCategory> lCustomerClassificationFirstLevelCategory { get; set; }
       public virtual List<CustomerClassificationFruitsAndVegitablesCategory> lCustomerClassificationFruitsAndVegitablesCategory { get; set; }
       public virtual List<CustomerClassificationGroceryCategory> lCustomerClassificationGroceryCategory { get; set; }
       public virtual List<CustomerClassificationMeatAndPoultry> lCustomerClassificationMeatAndPoultry { get; set; }
       public virtual List<CustomerClassificationOtherCategory> lCustomerClassificationOtherCategory { get; set; }
       public virtual List<CustomerClassificationPetCareProducts> lCustomerClassificationPetCareProducts { get; set; }
       public virtual List<CustomerClassificationOfferProducts> lCustomerClassificationOfferProducts { get; set; }
       public virtual List<CustomerClassificationFMCGProducts> lCustomerClassificationFMCGProducts { get; set; }
       public virtual List<CustomerClassificationMixedCategoryProducts> lCustomerClassificationMixedCategoryProducts { get; set; }
       public virtual List<CustomerClassificationDetail> lCustomerClassificationDetails { get; set; }
    }



    public class CustomerClassificationFirstLevelCategory
    {
        public int LevelID { get; set; }
        public int ImageCount { get; set; }
        public string ImageType { get; set; }
        public string Color { get; set; }
        public int TotalCount { get; set; }
        public string Description { get; set; }
        public int Condition1 { get; set; }
        public int Condition2{get;set;}
        public int Condition3{get;set;}
        public int Condition4{get;set;}      
    }

    public class CustomerClassificationFruitsAndVegitablesCategory
    {
        public int LevelID { get; set; }
        public int ImageCount { get; set; }
        public string ImageType { get; set; }
        public string Color { get; set; }
        public int TotalVegCount { get; set; }
        public int Condition1 { get; set; }
        public int Condition2 { get; set; }
        public int Condition3 { get; set; }
        public int Condition4 { get; set; }
    }

    public class CustomerClassificationGroceryCategory
    {
        public int LevelID { get; set; }
        public int ImageCount { get; set; }
        public string ImageType { get; set; }
        public string Color { get; set; }
        public int TotalGroceryCount { get; set; }
        public int Condition1 { get; set; }
        public int Condition2 { get; set; }
        public int Condition3 { get; set; }
        public int Condition4 { get; set; }
    }

    public class CustomerClassificationMeatAndPoultry
    {
        public int LevelID { get; set; }
        public int ImageCount { get; set; }
        public string ImageType { get; set; }
        public string Color { get; set; }
        public int TotalMeatCount { get; set; }
        public int Condition1 { get; set; }
        public int Condition2 { get; set; }
        public int Condition3 { get; set; }
        public int Condition4 { get; set; }
    }

    public class CustomerClassificationOtherCategory
    {
        public int LevelID { get; set; }
        public int ImageCount { get; set; }
        public string ImageType { get; set; }
        public string Color { get; set; }
        public int TotalOtherCount { get; set; }
        public int Condition1 { get; set; }
        public int Condition2 { get; set; }
        public int Condition3 { get; set; }
        public int Condition4 { get; set; }
    }

    public class CustomerClassificationPetCareProducts
    {
        public int LevelID { get; set; }
        public int ImageCount { get; set; }
        public string ImageType { get; set; }
        public string Color { get; set; }
        public int TotalPetCareCount { get; set; }
        public int Condition1 { get; set; }
        public int Condition2 { get; set; }
        public int Condition3 { get; set; }
        public int Condition4 { get; set; }
    }

    public class CustomerClassificationOfferProducts
    {
        public int LevelID { get; set; }
        public int ImageCount { get; set; }
        public string ImageType { get; set; }
        public string Color { get; set; }
        public int TotalOfferCount { get; set; }
        public int Condition1 { get; set; }
        public int Condition2 { get; set; }
        public int Condition3 { get; set; }
        public int Condition4 { get; set; }
    }

    public class CustomerClassificationFMCGProducts
    {
        public int LevelID { get; set; }
        public int ImageCount { get; set; }
        public string ImageType { get; set; }
        public string Color { get; set; }
        public int TotalFMCGCount { get; set; }
        public int Condition1 { get; set; }
        public int Condition2 { get; set; }
        public int Condition3 { get; set; }
        public int Condition4 { get; set; }
    }

    public class CustomerClassificationMixedCategoryProducts
    {
        public int LevelID { get; set; }
        public int ImageCount { get; set; }
        public string ImageType { get; set; }
        public string Color { get; set; }
        public int TotalMixedCount { get; set; }
        public int Condition1 { get; set; }
        public int Condition2 { get; set; }
        public int Condition3 { get; set; }
        public int Condition4 { get; set; }
    }

    public class CustomerClassificationDetail
    {
        public long UserLoginID { get; set; }
        public string Name{get;set;}
        public int TotalOrder { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime LastPurchaseDate { get; set; }
        public DateTime JoiningDate { get; set; }
        public string PrimaryMobile{get;set;}
        public string RegisteredMobile{get;set;}
        public string Email { get; set; }
        public string Pincode { get; set; }
        public string City { get; set; }
        public string ShippingAddress1{get;set;}
        public string ShippingAddress2 { get; set; }
        public string ShippingAddress3 { get; set; }
    }
}
