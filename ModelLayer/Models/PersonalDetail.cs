using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ModelLayer.Models
{
    public partial class PersonalDetail
    {
        public PersonalDetail()
        {
            this.AccountingHeads = new List<AccountingHead>();
            this.AccountingHeads1 = new List<AccountingHead>();
            this.AccountTransactions = new List<AccountTransaction>();
            this.AccountTransactions1 = new List<AccountTransaction>();
            this.Advertisements = new List<Advertisement>();
            this.Advertisements1 = new List<Advertisement>();
            this.Advertisers = new List<Advertiser>();
            this.Advertisers1 = new List<Advertiser>();
            this.AlgorithmLists = new List<AlgorithmList>();
            this.AlgorithmLists1 = new List<AlgorithmList>();
            this.APITokens = new List<APIToken>();
            this.APITokens1 = new List<APIToken>();
            this.ApplicationAlgorithms = new List<ApplicationAlgorithm>();
            this.ApplicationAlgorithms1 = new List<ApplicationAlgorithm>();
            this.ApplicationLists = new List<ApplicationList>();
            this.ApplicationLists1 = new List<ApplicationList>();
            this.Areas = new List<Area>();
            this.Areas1 = new List<Area>();
            this.Banks = new List<Bank>();
            this.Banks1 = new List<Bank>();
            this.BankAccountTypes = new List<BankAccountType>();
            this.BankAccountTypes1 = new List<BankAccountType>();
            this.Brands = new List<Brand>();
            this.Brands1 = new List<Brand>();
            this.BulkLogs = new List<BulkLog>();
            this.BusinessDetails = new List<BusinessDetail>();
            this.BusinessDetails1 = new List<BusinessDetail>();
            this.BusinessTypes = new List<BusinessType>();
            this.BusinessTypes1 = new List<BusinessType>();
            this.Categories = new List<Category>();
            this.Categories1 = new List<Category>();
            this.CategoryDimensions = new List<CategoryDimension>();
            this.CategoryDimensions1 = new List<CategoryDimension>();
            this.CategoryMaterials = new List<CategoryMaterial>();
            this.CategoryMaterials1 = new List<CategoryMaterial>();
            this.CategorySizes = new List<CategorySize>();
            this.CategorySizes1 = new List<CategorySize>();
            this.CategorySpecifications = new List<CategorySpecification>();
            this.CategorySpecifications1 = new List<CategorySpecification>();
            this.Charges = new List<Charge>();
            this.Charges1 = new List<Charge>();
            this.ChargeStages = new List<ChargeStage>();
            this.ChargeStages1 = new List<ChargeStage>();
            this.Chats = new List<Chat>();
            this.Chats1 = new List<Chat>();
            this.Chats2 = new List<Chat>();
            this.Cities = new List<City>();
            this.Cities1 = new List<City>();
            this.Colors = new List<Color>();
            this.Colors1 = new List<Color>();
            this.Components = new List<Component>();
            this.Components1 = new List<Component>();
            this.ComponentOffers = new List<ComponentOffer>();
            this.ComponentOffers1 = new List<ComponentOffer>();
            this.CoupenLists = new List<CoupenList>();
            this.CoupenLists1 = new List<CoupenList>();
            this.CustomerCoupens = new List<CustomerCoupen>();
            this.CustomerCoupens1 = new List<CustomerCoupen>();
            this.CustomerOrders = new List<CustomerOrder>();
            this.CustomerOrders1 = new List<CustomerOrder>();
            this.CustomerOrderDetails = new List<CustomerOrderDetail>();
            this.CustomerOrderDetails1 = new List<CustomerOrderDetail>();
            this.CustomerOrderDetailCalls = new List<CustomerOrderDetailCall>();
            this.CustomerOrderDetailXMLs = new List<CustomerOrderDetailXML>();
            this.CustomerOrderDetailXMLs1 = new List<CustomerOrderDetailXML>();
            this.CustomerOrderHistories = new List<CustomerOrderHistory>();
            this.CustomerOrderHistories1 = new List<CustomerOrderHistory>();
            this.CustomerOrderOfferDetails = new List<CustomerOrderOfferDetail>();
            this.CustomerOrderOfferDetails1 = new List<CustomerOrderOfferDetail>();
            this.CustomerRatingAndFeedbacks = new List<CustomerRatingAndFeedback>();
            this.CustomerRatingAndFeedbacks1 = new List<CustomerRatingAndFeedback>();
            this.CustomerShippingAddresses = new List<CustomerShippingAddress>();
            this.CustomerShippingAddresses1 = new List<CustomerShippingAddress>();
            this.CustomerValletBalances = new List<CustomerValletBalance>();
            this.CustomerValletBalances1 = new List<CustomerValletBalance>();
            this.DeliveryCashHandlingCharges = new List<DeliveryCashHandlingCharge>();
            this.DeliveryCashHandlingCharges1 = new List<DeliveryCashHandlingCharge>();
            this.DeliveryCashHandlingCharges2 = new List<DeliveryCashHandlingCharge>();
            this.DeliveryDetailLogs = new List<DeliveryDetailLog>();
            this.DeliveryDetailLogs1 = new List<DeliveryDetailLog>();
            this.DeliveryOrderCashHandlingCharges = new List<DeliveryOrderCashHandlingCharge>();
            this.DeliveryOrderCashHandlingCharges1 = new List<DeliveryOrderCashHandlingCharge>();
            this.DeliveryOrderDetails = new List<DeliveryOrderDetail>();
            this.DeliveryOrderDetails1 = new List<DeliveryOrderDetail>();
            this.DeliveryPartners = new List<DeliveryPartner>();
            this.DeliveryPartners1 = new List<DeliveryPartner>();
            this.DeliveryPincodes = new List<DeliveryPincode>();
            this.DeliveryPincodes1 = new List<DeliveryPincode>();
            this.DeliveryWeightSlabs = new List<DeliveryWeightSlab>();
            this.DeliveryWeightSlabs1 = new List<DeliveryWeightSlab>();
            this.DeliveryWeightSlabs2 = new List<DeliveryWeightSlab>();
            this.DeviceMasters = new List<DeviceMaster>();
            this.DeviceMasters1 = new List<DeviceMaster>();
            this.Dimensions = new List<Dimension>();
            this.Dimensions1 = new List<Dimension>();
            this.Districts = new List<District>();
            this.Districts1 = new List<District>();
            this.Domains = new List<Domain>();
            this.Domains1 = new List<Domain>();
            this.DomainCatgeories = new List<DomainCatgeory>();
            this.DomainCatgeories1 = new List<DomainCatgeory>();
            this.Employees = new List<Employee>();
            this.Employees1 = new List<Employee>();
            this.FeedbackCategaries = new List<FeedbackCategary>();
            this.FeedbackCategaries1 = new List<FeedbackCategary>();
            this.FeedbackManagments = new List<FeedbackManagment>();
            this.FeedbackManagments1 = new List<FeedbackManagment>();
            this.FeedBackTypes = new List<FeedBackType>();
            this.FeedBackTypes1 = new List<FeedBackType>();
            this.Franchises = new List<Franchise>();
            this.Franchises1 = new List<Franchise>();
            this.FranchiseCategories = new List<FranchiseCategory>();
            this.FranchiseCategories1 = new List<FranchiseCategory>();
            this.FranchiseLocations = new List<FranchiseLocation>();
            this.FranchiseLocations1 = new List<FranchiseLocation>();
            //this.Frenchises = new List<Frenchise>();
            //this.Frenchises1 = new List<Frenchise>();
            this.FrequentlyBuyTogetherProducts = new List<FrequentlyBuyTogetherProduct>();
            this.FrequentlyBuyTogetherProducts1 = new List<FrequentlyBuyTogetherProduct>();
            this.GandhibaghTransactions = new List<GandhibaghTransaction>();
            this.GandhibaghTransactions1 = new List<GandhibaghTransaction>();
            this.GandhibaghTransactions2 = new List<GandhibaghTransaction>();
            this.GandhibaghTransactions3 = new List<GandhibaghTransaction>();
            this.GbSettings = new List<GbSetting>();
            this.GbSettings1 = new List<GbSetting>();
            this.GetwayPaymentTransactions = new List<GetwayPaymentTransaction>();
            this.GetwayPaymentTransactions1 = new List<GetwayPaymentTransaction>();
            this.GoodwillOwnerPoints = new List<GoodwillOwnerPoint>();
            this.GoodwillOwnerPoints1 = new List<GoodwillOwnerPoint>();
            this.LedgerHeads = new List<LedgerHead>();
            this.LedgerHeads1 = new List<LedgerHead>();
            this.LoginAttempts = new List<LoginAttempt>();
            this.LoginAttempts1 = new List<LoginAttempt>();
            this.LoginSecurityAnswers = new List<LoginSecurityAnswer>();
            this.LoginSecurityAnswers1 = new List<LoginSecurityAnswer>();
            this.LogTables = new List<LogTable>();
            this.Materials = new List<Material>();
            this.Materials1 = new List<Material>();
            this.Menus = new List<Menu>();
            this.Menus1 = new List<Menu>();
            this.Notifications = new List<Notification>();
            this.Notifications1 = new List<Notification>();
            this.Notifications2 = new List<Notification>();
            this.Notifications3 = new List<Notification>();
            this.OfferDurations = new List<OfferDuration>();
            this.OfferDurations1 = new List<OfferDuration>();
            this.OfferZoneProducts = new List<OfferZoneProduct>();
            this.OfferZoneProducts1 = new List<OfferZoneProduct>();
            this.OTPs = new List<OTP>();
            this.OTPs1 = new List<OTP>();
            this.OwnerAdvertisements = new List<OwnerAdvertisement>();
            this.OwnerAdvertisements1 = new List<OwnerAdvertisement>();
            this.OwnerBanks = new List<OwnerBank>();
            this.OwnerBanks1 = new List<OwnerBank>();
            this.OwnerPlans = new List<OwnerPlan>();
            this.OwnerPlans1 = new List<OwnerPlan>();
            this.OwnerPlanCategoryCharges = new List<OwnerPlanCategoryCharge>();
            this.OwnerPlanCategoryCharges1 = new List<OwnerPlanCategoryCharge>();
            this.PaymentModes = new List<PaymentMode>();
            this.PaymentModes1 = new List<PaymentMode>();
            this.PersonalDetail1 = new List<PersonalDetail>();
            this.Pincodes = new List<Pincode>();
            this.Plans = new List<Plan>();
            this.PlanBinds = new List<PlanBind>();
            this.PlanBindCategories = new List<PlanBindCategory>();
            this.PlanCategoryCharges = new List<PlanCategoryCharge>();
            this.Products = new List<Product>();
            this.ProductBulkDetails = new List<ProductBulkDetail>();
            this.ProductSpecifications = new List<ProductSpecification>();
            this.ProductVarients = new List<ProductVarient>();
            this.Ratings = new List<Rating>();
            this.ReceiveOrderOnCalls = new List<ReceiveOrderOnCall>();
            this.Roles = new List<Role>();
            this.RoleMenus = new List<RoleMenu>();
            this.SalesRecords = new List<SalesRecord>();
            this.Salutations = new List<Salutation>();
            this.SchemeTypes = new List<SchemeType>();
            this.SecurityQuestions = new List<SecurityQuestion>();
            this.Shops = new List<Shop>();
            this.ShopComponentPrices = new List<ShopComponentPrice>();
            this.ShopMarkets = new List<ShopMarket>();
            this.ShopPaymentModes = new List<ShopPaymentMode>();
            this.ShopProducts = new List<ShopProduct>();
            this.ShopProductCharges = new List<ShopProductCharge>();
            this.ShopStocks = new List<ShopStock>();
            this.ShopStockBulkLogs = new List<ShopStockBulkLog>();
            this.Sizes = new List<Size>();
            this.SourceOfInfoes = new List<SourceOfInfo>();
            this.Specifications = new List<Specification>();
            this.States = new List<State>();
            this.StockComponents = new List<StockComponent>();
            this.StockComponentOffers = new List<StockComponentOffer>();
            this.StockComponentOfferDurations = new List<StockComponentOfferDuration>();
            this.TempProducts = new List<TempProduct>();
            this.TempProductSpecifications = new List<TempProductSpecification>();
            this.TempProductVarients = new List<TempProductVarient>();
            this.TempShopProducts = new List<TempShopProduct>();
            this.TempShopStocks = new List<TempShopStock>();
            this.TempStockComponents = new List<TempStockComponent>();
            this.TodaySchemes = new List<TodayScheme>();
            this.Units = new List<Unit>();
            this.UserAdditionalMenus = new List<UserAdditionalMenu>();
            this.UserChats = new List<UserChat>();
            this.UserLogins = new List<UserLogin>();
            this.UserRoles = new List<UserRole>();
            this.VehicleTypes = new List<VehicleType>();
            this.WishLists = new List<WishList>();
            this.PersonalDetail11 = new List<PersonalDetail>();
            this.Pincodes1 = new List<Pincode>();
            this.Plans1 = new List<Plan>();
            this.PlanBinds1 = new List<PlanBind>();
            this.PlanBindCategories1 = new List<PlanBindCategory>();
            this.PlanCategoryCharges1 = new List<PlanCategoryCharge>();
            this.Products1 = new List<Product>();
            this.ProductSpecifications1 = new List<ProductSpecification>();
            this.ProductVarients1 = new List<ProductVarient>();
            this.Ratings1 = new List<Rating>();
            this.ReceiveOrderOnCalls1 = new List<ReceiveOrderOnCall>();
            this.Roles1 = new List<Role>();
            this.RoleMenus1 = new List<RoleMenu>();
            this.SalesRecords1 = new List<SalesRecord>();
            this.Salutations1 = new List<Salutation>();
            this.SchemeTypes1 = new List<SchemeType>();
            this.SecurityQuestions1 = new List<SecurityQuestion>();
            this.Shops1 = new List<Shop>();
            /*WelcomeLetter*/
            this.Shops2 = new List<Shop>();
            /**/
            this.ShopComponentPrices1 = new List<ShopComponentPrice>();
            this.ShopMarkets1 = new List<ShopMarket>();
            this.ShopPaymentModes1 = new List<ShopPaymentMode>();
            this.ShopProducts1 = new List<ShopProduct>();
            this.ShopProductCharges1 = new List<ShopProductCharge>();
            this.ShopStocks1 = new List<ShopStock>();
            this.Sizes1 = new List<Size>();
            this.SourceOfInfoes1 = new List<SourceOfInfo>();
            this.Specifications1 = new List<Specification>();
            this.States1 = new List<State>();
            this.StockComponents1 = new List<StockComponent>();
            this.StockComponentOffers1 = new List<StockComponentOffer>();
            this.StockComponentOfferDurations1 = new List<StockComponentOfferDuration>();
            this.TempProducts1 = new List<TempProduct>();
            this.TempProductSpecifications1 = new List<TempProductSpecification>();
            this.TempProductVarients1 = new List<TempProductVarient>();
            this.TempShopProducts1 = new List<TempShopProduct>();
            this.TempShopStocks1 = new List<TempShopStock>();
            this.TempStockComponents1 = new List<TempStockComponent>();
            this.TodaySchemes1 = new List<TodayScheme>();
            this.Units1 = new List<Unit>();
            this.UserAdditionalMenus1 = new List<UserAdditionalMenu>();
            this.UserChats1 = new List<UserChat>();
            this.UserLogins1 = new List<UserLogin>();
            this.UserRoles1 = new List<UserRole>();
            this.VehicleTypes1 = new List<VehicleType>();
            this.WishLists1 = new List<WishList>();
            this.ProprietoryProducts = new List<ProprietoryProduct>();
            this.ReceiveOrderOnCalls2 = new List<ReceiveOrderOnCall>();
            this.SEOs = new List<SEO>();
            this.SEOs1 = new List<SEO>();
            this.TransactionInputProcessAccounts = new List<TransactionInputProcessAccount>();
            this.TransactionInputProcessAccounts1 = new List<TransactionInputProcessAccount>();

            this.UserChats2 = new List<UserChat>();
            this.UserLogins2 = new List<UserLogin>();
            this.UserLogins3 = new List<UserLogin>();
            this.UserThemes = new List<UserTheme>();
            this.UserThemes1 = new List<UserTheme>();
            //Added by Zubair for Inventory
            this.Suppliers = new List<Supplier>();
            this.Warehouses = new List<Warehouse>();
            this.PurchaseOrders = new List<PurchaseOrder>();
            //End

        }

        public long ID { get; set; }
        public long UserLoginID { get; set; }

        [Required]
        public int SalutationID { get; set; }

        [Display(Name = "First Name")]
        [Required(ErrorMessage = "First Name is required")]
        public string FirstName { get; set; }

        [Display(Name = "Middle Name")]
        public string MiddleName { get; set; }

        //[Required(ErrorMessage = "Last Name is required")]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]

        public Nullable<System.DateTime> DOB { get; set; }
        // [Required(ErrorMessage = "Gender is required")]
        public string Gender { get; set; }

        [Display(Name = "Pincode")]
        public Nullable<int> PincodeID { get; set; }

        //[Required(ErrorMessage = "Address is required")]
        public string Address { get; set; }

        [RegularExpression(@"^(?:\d{10}|00\d{10}|\+\d{2}\d{8})$", ErrorMessage = "Please enter proper Mobile No.")]
        [Display(Name = "Alternate Mobile")]
        public string AlternateMobile { get; set; }
        //[Required(ErrorMessage = "Email is required (we promise not to spam you!)")]        

        //[DataType(DataType.EmailAddress)]
        [Display(Name = "Alternate Email")]
        [RegularExpression(@"^([\w-\.]+@([\w-]+\.)+[\w-]{2,4})?$", ErrorMessage = "Please enter proper email")]
        public string AlternateEmail { get; set; }
        public bool IsActive { get; set; }
        public System.DateTime CreateDate { get; set; }
        public Nullable<long> CreateBy { get; set; }
        public Nullable<System.DateTime> ModifyDate { get; set; }
        public Nullable<long> ModifyBy { get; set; }
        public string NetworkIP { get; set; }
        public string DeviceType { get; set; }
        public string DeviceID { get; set; }
        public virtual List<AccountingHead> AccountingHeads { get; set; }
        public virtual List<AccountingHead> AccountingHeads1 { get; set; }
        public virtual List<AccountTransaction> AccountTransactions { get; set; }
        public virtual List<AccountTransaction> AccountTransactions1 { get; set; }
        public virtual List<Advertisement> Advertisements { get; set; }
        public virtual List<Advertisement> Advertisements1 { get; set; }
        public virtual List<Advertiser> Advertisers { get; set; }
        public virtual List<Advertiser> Advertisers1 { get; set; }
        public virtual List<AlgorithmList> AlgorithmLists { get; set; }
        public virtual List<AlgorithmList> AlgorithmLists1 { get; set; }
        public virtual List<APIToken> APITokens { get; set; }
        public virtual List<APIToken> APITokens1 { get; set; }
        public virtual List<ApplicationAlgorithm> ApplicationAlgorithms { get; set; }
        public virtual List<ApplicationAlgorithm> ApplicationAlgorithms1 { get; set; }
        public virtual List<ApplicationList> ApplicationLists { get; set; }
        public virtual List<ApplicationList> ApplicationLists1 { get; set; }
        public virtual List<Area> Areas { get; set; }
        public virtual List<Area> Areas1 { get; set; }
        public virtual List<Bank> Banks { get; set; }
        public virtual List<Bank> Banks1 { get; set; }
        public virtual List<BankAccountType> BankAccountTypes { get; set; }
        public virtual List<BankAccountType> BankAccountTypes1 { get; set; }
        public virtual List<Brand> Brands { get; set; }
        public virtual List<Brand> Brands1 { get; set; }
        public virtual List<BulkLog> BulkLogs { get; set; }
        public virtual List<BusinessDetail> BusinessDetails { get; set; }
        public virtual List<BusinessDetail> BusinessDetails1 { get; set; }
        public virtual List<BusinessType> BusinessTypes { get; set; }
        public virtual List<BusinessType> BusinessTypes1 { get; set; }
        public virtual List<Category> Categories { get; set; }
        public virtual List<Category> Categories1 { get; set; }
        public virtual List<CategoryDimension> CategoryDimensions { get; set; }
        public virtual List<CategoryDimension> CategoryDimensions1 { get; set; }
        public virtual List<CategoryMaterial> CategoryMaterials { get; set; }
        public virtual List<CategoryMaterial> CategoryMaterials1 { get; set; }
        public virtual List<CategorySize> CategorySizes { get; set; }
        public virtual List<CategorySize> CategorySizes1 { get; set; }
        public virtual List<CategorySpecification> CategorySpecifications { get; set; }
        public virtual List<CategorySpecification> CategorySpecifications1 { get; set; }
        public virtual List<Charge> Charges { get; set; }
        public virtual List<Charge> Charges1 { get; set; }
        public virtual List<ChargeStage> ChargeStages { get; set; }
        public virtual List<ChargeStage> ChargeStages1 { get; set; }
        public virtual List<Chat> Chats { get; set; }
        public virtual List<Chat> Chats1 { get; set; }
        public virtual List<Chat> Chats2 { get; set; }
        public virtual List<City> Cities { get; set; }
        public virtual List<City> Cities1 { get; set; }
        public virtual List<Color> Colors { get; set; }
        public virtual List<Color> Colors1 { get; set; }
        public virtual List<Component> Components { get; set; }
        public virtual List<Component> Components1 { get; set; }
        public virtual List<ComponentOffer> ComponentOffers { get; set; }
        public virtual List<ComponentOffer> ComponentOffers1 { get; set; }
        public virtual List<CoupenList> CoupenLists { get; set; }
        public virtual List<CoupenList> CoupenLists1 { get; set; }
        public virtual List<CustomerCoupen> CustomerCoupens { get; set; }
        public virtual List<CustomerCoupen> CustomerCoupens1 { get; set; }
        public virtual List<CustomerOrder> CustomerOrders { get; set; }
        public virtual List<CustomerOrder> CustomerOrders1 { get; set; }
        public virtual List<CustomerOrderDetail> CustomerOrderDetails { get; set; }
        public virtual List<CustomerOrderDetail> CustomerOrderDetails1 { get; set; }
        public virtual List<CustomerOrderDetailXML> CustomerOrderDetailXMLs { get; set; }
        public virtual List<CustomerOrderDetailXML> CustomerOrderDetailXMLs1 { get; set; }
        public virtual List<CustomerOrderHistory> CustomerOrderHistories { get; set; }
        public virtual List<CustomerOrderHistory> CustomerOrderHistories1 { get; set; }
        public virtual List<CustomerOrderOfferDetail> CustomerOrderOfferDetails { get; set; }
        public virtual List<CustomerOrderOfferDetail> CustomerOrderOfferDetails1 { get; set; }
        public virtual List<CustomerOrderDetailCall> CustomerOrderDetailCalls { get; set; }
        public virtual List<CustomerRatingAndFeedback> CustomerRatingAndFeedbacks { get; set; }
        public virtual List<CustomerRatingAndFeedback> CustomerRatingAndFeedbacks1 { get; set; }
        public virtual List<CustomerShippingAddress> CustomerShippingAddresses { get; set; }
        public virtual List<CustomerShippingAddress> CustomerShippingAddresses1 { get; set; }
        public virtual List<CustomerValletBalance> CustomerValletBalances { get; set; }
        public virtual List<CustomerValletBalance> CustomerValletBalances1 { get; set; }
        public virtual List<DeliveryCashHandlingCharge> DeliveryCashHandlingCharges { get; set; }
        public virtual List<DeliveryCashHandlingCharge> DeliveryCashHandlingCharges1 { get; set; }
        public virtual List<DeliveryCashHandlingCharge> DeliveryCashHandlingCharges2 { get; set; }
        public virtual List<DeliveryDetailLog> DeliveryDetailLogs { get; set; }
        public virtual List<DeliveryDetailLog> DeliveryDetailLogs1 { get; set; }
        public virtual List<DeliveryOrderCashHandlingCharge> DeliveryOrderCashHandlingCharges { get; set; }
        public virtual List<DeliveryOrderCashHandlingCharge> DeliveryOrderCashHandlingCharges1 { get; set; }
        public virtual List<DeliveryOrderDetail> DeliveryOrderDetails { get; set; }
        public virtual List<DeliveryOrderDetail> DeliveryOrderDetails1 { get; set; }
        public virtual List<DeliveryPartner> DeliveryPartners { get; set; }
        public virtual List<DeliveryPartner> DeliveryPartners1 { get; set; }
        public virtual List<DeliveryPincode> DeliveryPincodes { get; set; }
        public virtual List<DeliveryPincode> DeliveryPincodes1 { get; set; }
        public virtual List<DeliveryWeightSlab> DeliveryWeightSlabs { get; set; }
        public virtual List<DeliveryWeightSlab> DeliveryWeightSlabs1 { get; set; }
        public virtual List<DeliveryWeightSlab> DeliveryWeightSlabs2 { get; set; }
        public virtual List<DeviceMaster> DeviceMasters { get; set; }
        public virtual List<DeviceMaster> DeviceMasters1 { get; set; }
        public virtual List<Dimension> Dimensions { get; set; }
        public virtual List<Dimension> Dimensions1 { get; set; }
        public virtual List<District> Districts { get; set; }
        public virtual List<District> Districts1 { get; set; }
        public virtual List<Domain> Domains { get; set; }
        public virtual List<Domain> Domains1 { get; set; }
        public virtual List<DomainCatgeory> DomainCatgeories { get; set; }
        public virtual List<DomainCatgeory> DomainCatgeories1 { get; set; }
        public virtual List<Employee> Employees { get; set; }
        public virtual List<Employee> Employees1 { get; set; }

        public virtual List<DeliveryBoy> DeliveryBoys { get; set; }
        public virtual List<DeliveryBoy> DeliveryBoys1 { get; set; }
        public virtual List<FeedbackCategary> FeedbackCategaries { get; set; }
        public virtual List<FeedbackCategary> FeedbackCategaries1 { get; set; }
        public virtual List<FeedbackManagment> FeedbackManagments { get; set; }
        public virtual List<FeedbackManagment> FeedbackManagments1 { get; set; }
        public virtual List<FeedBackType> FeedBackTypes { get; set; }
        public virtual List<FeedBackType> FeedBackTypes1 { get; set; }
        public virtual List<Franchise> Franchises { get; set; }
        public virtual List<Franchise> Franchises1 { get; set; }
        public virtual List<FranchiseCategory> FranchiseCategories { get; set; }
        public virtual List<FranchiseCategory> FranchiseCategories1 { get; set; }
        public virtual List<FranchiseLocation> FranchiseLocations { get; set; }
        public virtual List<FranchiseLocation> FranchiseLocations1 { get; set; }
        //public virtual List<Frenchise> Frenchises { get; set; }
        //public virtual List<Frenchise> Frenchises1 { get; set; }
        public virtual List<FrequentlyBuyTogetherProduct> FrequentlyBuyTogetherProducts { get; set; }
        public virtual List<FrequentlyBuyTogetherProduct> FrequentlyBuyTogetherProducts1 { get; set; }
        public virtual List<GandhibaghTransaction> GandhibaghTransactions { get; set; }
        public virtual List<GandhibaghTransaction> GandhibaghTransactions1 { get; set; }
        public virtual List<GandhibaghTransaction> GandhibaghTransactions2 { get; set; }
        public virtual List<GandhibaghTransaction> GandhibaghTransactions3 { get; set; }
        public virtual List<GbSetting> GbSettings { get; set; }
        public virtual List<GbSetting> GbSettings1 { get; set; }
        public virtual List<GetwayPaymentTransaction> GetwayPaymentTransactions { get; set; }
        public virtual List<GetwayPaymentTransaction> GetwayPaymentTransactions1 { get; set; }
        public virtual List<GoodwillOwnerPoint> GoodwillOwnerPoints { get; set; }
        public virtual List<GoodwillOwnerPoint> GoodwillOwnerPoints1 { get; set; }
        public virtual List<LedgerHead> LedgerHeads { get; set; }
        public virtual List<LedgerHead> LedgerHeads1 { get; set; }
        public virtual List<LoginAttempt> LoginAttempts { get; set; }
        public virtual List<LoginAttempt> LoginAttempts1 { get; set; }
        public virtual List<LoginSecurityAnswer> LoginSecurityAnswers { get; set; }
        public virtual List<LoginSecurityAnswer> LoginSecurityAnswers1 { get; set; }
        public virtual List<LogTable> LogTables { get; set; }
        public virtual List<Material> Materials { get; set; }
        public virtual List<Material> Materials1 { get; set; }
        public virtual List<Menu> Menus { get; set; }
        public virtual List<Menu> Menus1 { get; set; }
        public virtual List<Notification> Notifications { get; set; }
        public virtual List<Notification> Notifications1 { get; set; }
        public virtual List<Notification> Notifications2 { get; set; }
        public virtual List<Notification> Notifications3 { get; set; }
        public virtual List<OfferDuration> OfferDurations { get; set; }
        public virtual List<OfferDuration> OfferDurations1 { get; set; }
        public virtual List<OfferZoneProduct> OfferZoneProducts { get; set; }
        public virtual List<OfferZoneProduct> OfferZoneProducts1 { get; set; }
        public virtual List<OTP> OTPs { get; set; }
        public virtual List<OTP> OTPs1 { get; set; }
        public virtual List<OwnerAdvertisement> OwnerAdvertisements { get; set; }
        public virtual List<OwnerAdvertisement> OwnerAdvertisements1 { get; set; }
        public virtual List<OwnerBank> OwnerBanks { get; set; }
        public virtual List<OwnerBank> OwnerBanks1 { get; set; }
        public virtual List<OwnerPlan> OwnerPlans { get; set; }
        public virtual List<OwnerPlan> OwnerPlans1 { get; set; }
        public virtual List<OwnerPlanCategoryCharge> OwnerPlanCategoryCharges { get; set; }
        public virtual List<OwnerPlanCategoryCharge> OwnerPlanCategoryCharges1 { get; set; }
        public virtual List<PaymentMode> PaymentModes { get; set; }
        public virtual List<PaymentMode> PaymentModes1 { get; set; }
        public virtual List<PersonalDetail> PersonalDetail1 { get; set; }
        public virtual PersonalDetail PersonalDetail2 { get; set; }
        public virtual List<Pincode> Pincodes { get; set; }
        public virtual List<Plan> Plans { get; set; }
        public virtual List<PlanBind> PlanBinds { get; set; }
        public virtual List<PlanBindCategory> PlanBindCategories { get; set; }
        public virtual List<PlanCategoryCharge> PlanCategoryCharges { get; set; }
        public virtual List<Product> Products { get; set; }
        public virtual List<ProductBulkDetail> ProductBulkDetails { get; set; }
        public virtual List<ProductSpecification> ProductSpecifications { get; set; }
        public virtual List<ProductVarient> ProductVarients { get; set; }
        public virtual List<Rating> Ratings { get; set; }
        public virtual List<ProprietoryProduct> ProprietoryProducts { get; set; }
        public virtual List<ReceiveOrderOnCall> ReceiveOrderOnCalls { get; set; }
        public virtual List<Role> Roles { get; set; }
        public virtual List<RoleMenu> RoleMenus { get; set; }
        public virtual List<SalesRecord> SalesRecords { get; set; }
        public virtual List<Salutation> Salutations { get; set; }
        public virtual List<SchemeType> SchemeTypes { get; set; }
        public virtual List<SecurityQuestion> SecurityQuestions { get; set; }
        public virtual List<Shop> Shops { get; set; }
        public virtual List<ShopComponentPrice> ShopComponentPrices { get; set; }
        public virtual List<ShopMarket> ShopMarkets { get; set; }
        public virtual List<ShopPaymentMode> ShopPaymentModes { get; set; }
        public virtual List<ShopProduct> ShopProducts { get; set; }
        public virtual List<ShopProductCharge> ShopProductCharges { get; set; }
        public virtual List<ShopStock> ShopStocks { get; set; }
        public virtual List<ShopStockBulkLog> ShopStockBulkLogs { get; set; }
        public virtual List<Size> Sizes { get; set; }
        public virtual List<SourceOfInfo> SourceOfInfoes { get; set; }
        public virtual List<Specification> Specifications { get; set; }
        public virtual List<State> States { get; set; }
        public virtual List<StockComponent> StockComponents { get; set; }
        public virtual List<StockComponentOffer> StockComponentOffers { get; set; }
        public virtual List<StockComponentOfferDuration> StockComponentOfferDurations { get; set; }
        public virtual List<TempProduct> TempProducts { get; set; }
        public virtual List<TempProductSpecification> TempProductSpecifications { get; set; }
        public virtual List<TempProductVarient> TempProductVarients { get; set; }
        public virtual List<TempShopProduct> TempShopProducts { get; set; }
        public virtual List<TempShopStock> TempShopStocks { get; set; }
        public virtual List<TempStockComponent> TempStockComponents { get; set; }
        public virtual List<TodayScheme> TodaySchemes { get; set; }
        public virtual List<Unit> Units { get; set; }
        public virtual List<UserAdditionalMenu> UserAdditionalMenus { get; set; }
        public virtual List<UserChat> UserChats { get; set; }
        public virtual List<UserLogin> UserLogins { get; set; }
        public virtual List<UserRole> UserRoles { get; set; }
        public virtual List<VehicleType> VehicleTypes { get; set; }
        public virtual List<WishList> WishLists { get; set; }
        public virtual List<PersonalDetail> PersonalDetail11 { get; set; }
        public virtual PersonalDetail PersonalDetail3 { get; set; }
        public virtual List<Pincode> Pincodes1 { get; set; }
        public virtual List<Plan> Plans1 { get; set; }
        public virtual List<PlanBind> PlanBinds1 { get; set; }
        public virtual List<PlanBindCategory> PlanBindCategories1 { get; set; }
        public virtual List<PlanCategoryCharge> PlanCategoryCharges1 { get; set; }
        public virtual List<Product> Products1 { get; set; }
        public virtual List<ProductSpecification> ProductSpecifications1 { get; set; }
        public virtual List<ProductVarient> ProductVarients1 { get; set; }
        public virtual List<Rating> Ratings1 { get; set; }
        public virtual List<ReceiveOrderOnCall> ReceiveOrderOnCalls1 { get; set; }
        public virtual List<Role> Roles1 { get; set; }
        public virtual List<RoleMenu> RoleMenus1 { get; set; }
        public virtual List<SalesRecord> SalesRecords1 { get; set; }
        public virtual List<Salutation> Salutations1 { get; set; }
        public virtual List<SchemeType> SchemeTypes1 { get; set; }
        public virtual List<SecurityQuestion> SecurityQuestions1 { get; set; }
        public virtual List<Shop> Shops1 { get; set; }
        public virtual List<ShopComponentPrice> ShopComponentPrices1 { get; set; }
        public virtual List<ShopMarket> ShopMarkets1 { get; set; }
        public virtual List<ShopPaymentMode> ShopPaymentModes1 { get; set; }
        public virtual List<ShopProduct> ShopProducts1 { get; set; }
        public virtual List<ShopProductCharge> ShopProductCharges1 { get; set; }
        public virtual List<ShopStock> ShopStocks1 { get; set; }
        public virtual List<Size> Sizes1 { get; set; }
        public virtual List<SourceOfInfo> SourceOfInfoes1 { get; set; }
        public virtual List<Specification> Specifications1 { get; set; }
        public virtual List<State> States1 { get; set; }
        public virtual List<StockComponent> StockComponents1 { get; set; }
        public virtual List<StockComponentOffer> StockComponentOffers1 { get; set; }
        public virtual List<StockComponentOfferDuration> StockComponentOfferDurations1 { get; set; }
        public virtual List<TempProduct> TempProducts1 { get; set; }
        public virtual List<TempProductSpecification> TempProductSpecifications1 { get; set; }
        public virtual List<TempProductVarient> TempProductVarients1 { get; set; }
        public virtual List<TempShopProduct> TempShopProducts1 { get; set; }
        public virtual List<TempShopStock> TempShopStocks1 { get; set; }
        public virtual List<TempStockComponent> TempStockComponents1 { get; set; }
        public virtual List<TodayScheme> TodaySchemes1 { get; set; }
        public virtual List<Unit> Units1 { get; set; }
        public virtual List<UserAdditionalMenu> UserAdditionalMenus1 { get; set; }
        public virtual List<UserChat> UserChats1 { get; set; }
        public virtual List<UserLogin> UserLogins1 { get; set; }
        public virtual List<UserRole> UserRoles1 { get; set; }
        public virtual List<VehicleType> VehicleTypes1 { get; set; }
        public virtual List<WishList> WishLists1 { get; set; }
        public virtual Pincode Pincode { get; set; }
        public virtual Salutation Salutation { get; set; }
        public virtual UserLogin UserLogin { get; set; }
        public virtual List<ReceiveOrderOnCall> ReceiveOrderOnCalls2 { get; set; }
        public virtual List<SEO> SEOs { get; set; }
        public virtual List<SEO> SEOs1 { get; set; }
        public virtual ICollection<TransactionInputProcessAccount> TransactionInputProcessAccounts { get; set; }
        public virtual ICollection<TransactionInputProcessAccount> TransactionInputProcessAccounts1 { get; set; }
        public virtual List<UserChat> UserChats2 { get; set; }
        public virtual List<UserLogin> UserLogins2 { get; set; }
        public virtual List<UserLogin> UserLogins3 { get; set; }
        public virtual List<UserTheme> UserThemes { get; set; }
        public virtual List<UserTheme> UserThemes1 { get; set; }
        //Added by Zubair for Inventory
        public virtual List<Supplier> Suppliers { get; set; }
        public virtual List<Warehouse> Warehouses { get; set; }
        public virtual List<PurchaseOrder> PurchaseOrders { get; set; }
        //End

        public List<Offer> Offers { get; set; }

        public List<Offer> Offers1 { get; set; }

        /*WelcomeLetter*/
        public List<Shop> Shops2 { get; set; }
        /**/

        //----------------ChannelPartner-------------//
        public virtual ICollection<ChannelPartner> ChannelPartners { get; set; }
        //sonali
        public string Facebook { get; set; }
        public string Insta { get; set; }
        //-----------------End ------------------------//
    }
    
}
