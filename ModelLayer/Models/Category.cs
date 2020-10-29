using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ModelLayer.Models
{
    public partial class Category
    {
        public Category()
        {
            this.CategoryDimensions = new List<CategoryDimension>();
            this.CategoryMaterials = new List<CategoryMaterial>();
            this.CategorySizes = new List<CategorySize>();
            this.CategorySpecifications = new List<CategorySpecification>();
            this.DomainCatgeories = new List<DomainCatgeory>();
            this.FranchiseCategories = new List<FranchiseCategory>();
            this.FranchiseCategories1 = new List<FranchiseCategory>();
            this.OwnerPlanCategoryCharges = new List<OwnerPlanCategoryCharge>();
            this.Category1 = new List<Category>();
            this.PlanBindCategories = new List<PlanBindCategory>();
            this.PlanCategoryCharges = new List<PlanCategoryCharge>();
            this.Products = new List<Product>();
            this.ShopMarkets = new List<ShopMarket>();
            this.ShopMarkets1 = new List<ShopMarket>();
            this.TempProducts = new List<TempProduct>();

            //------Added by mohit on 14-01-16----------------//
            this.FranchiseMenus = new List<FranchiseMenu>();
            //------------End  ----------------------------//

            //------Added by Pradnyakar on 04-02-16----------------//
            this.ShopMenuPriorities = new List<ShopMenuPriority>();
            //------------End  ----------------------------//
            //------Added by Pradnyakar on 11-02-16----------------//
            this.PremiumShopsPriorities = new List<PremiumShopsPriority>();
            //------------End  ----------------------------//
        }

        public int ID { get; set; }
        [Required(ErrorMessage = "Category Name is Required")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Category Name must be between 3 - 150 characters ")] 
        public string Name { get; set; }
        public Nullable<int> ParentCategoryID { get; set; }
        public int Level { get; set; }

        [DataType(DataType.MultilineText)]
        [Required(ErrorMessage = "Description is Required")]
        [StringLength(450, MinimumLength = 3, ErrorMessage = "Description must be between 3 - 450 characters ")] 
        public string Description { get; set; }

        [DataType(DataType.MultilineText)]
        [Required(ErrorMessage = "Keyword is Required")]
        //[StringLength(500, MinimumLength = 3, ErrorMessage = "Keyword must be between 3 - 500 characters ")] 
        public string SearchKeyword { get; set; }
        public bool IsActive { get; set; }

        /*======== Added by Tejaswee for Setting expiration date to some special Categories like Festival category ========*/
        public bool IsExpire { get; set; }
        public Nullable<System.DateTime> ExpiryDate { get; set; }

        /*======== Added by Tejaswee for Setting expiration date to some special Categories like Festival category ========*/

        public System.DateTime CreateDate { get; set; }
        public long CreateBy { get; set; }
        public Nullable<System.DateTime> ModifyDate { get; set; }
        public Nullable<long> ModifyBy { get; set; }
        public string NetworkIP { get; set; }
        public string DeviceType { get; set; }
        public string DeviceID { get; set; }
        public virtual List<CategoryDimension> CategoryDimensions { get; set; }
        public virtual List<CategoryMaterial> CategoryMaterials { get; set; }
        public virtual List<CategorySize> CategorySizes { get; set; }
        public virtual List<CategorySpecification> CategorySpecifications { get; set; }
        public virtual PersonalDetail PersonalDetail { get; set; }
        public virtual List<DomainCatgeory> DomainCatgeories { get; set; }
        public virtual List<FranchiseCategory> FranchiseCategories { get; set; }
        public virtual List<FranchiseCategory> FranchiseCategories1 { get; set; }
        public virtual PersonalDetail PersonalDetail1 { get; set; }
        public virtual List<OwnerPlanCategoryCharge> OwnerPlanCategoryCharges { get; set; }
        public virtual List<Category> Category1 { get; set; }
        public virtual Category Category2 { get; set; }
        public virtual List<PlanBindCategory> PlanBindCategories { get; set; }
        public virtual List<PlanCategoryCharge> PlanCategoryCharges { get; set; }
        public virtual List<Product> Products { get; set; }
        public virtual List<ShopMarket> ShopMarkets { get; set; }
        public virtual List<ShopMarket> ShopMarkets1 { get; set; }
        public virtual List<TempProduct> TempProducts { get; set; }
        //------Added by mohit on 14-01-16----------------//
        public virtual ICollection<FranchiseMenu> FranchiseMenus { get; set; }
        //------------End  ----------------------------//

        //------Added by Pradnyakar on 03-02-16----------------//
        public virtual ICollection<ShopMenuPriority> ShopMenuPriorities { get; set; }
        //------------End  ----------------------------//

        //------Added by Pradnyakar on 11-02-16----------------//
        public virtual ICollection<PremiumShopsPriority> PremiumShopsPriorities { get; set; }
        //------------End  ----------------------------//
        public virtual ICollection<PreviewFeatureDisplay> PreviewFeatureDisplays { get; set; }
    }
}
