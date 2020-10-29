using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using ModelLayer.Models.Mapping;
using ModelLayer.Models;
using ModelLayer.Models.ViewModel;

namespace ModelLayer.Models
{
    public partial class EzeeloDBContext : DbContext
    {
        static EzeeloDBContext()
        {
            Database.SetInitializer<EzeeloDBContext>(null);
        }

        public EzeeloDBContext()
            //: base(@"Data Source=103.138.188.153;initial catalog=Ezeelo;user id=ezeelo-prod;password=EzeeloLive@2018!; TimeOut = 1000;MultipleActiveResultSets=true")
        : base(@"Data Source=180.179.213.83\sqlexpress;Initial Catalog=Ezeelo;user id=ezeelo-prod;Password=EzeeloLive@2018!;TimeOut=1000;MultipleActiveResultSets=True")
        {
            ((IObjectContextAdapter)this).ObjectContext.CommandTimeout = 250;
            //this.Configuration.ProxyCreationEnabled = false;
        }
        public DbSet<MerchantTransactionDistributionLevel16> MerchantTransactionDistributionLevel16 { get; set; }
        public DbSet<MarketPlaceEZProductGallery> marketPlaceEZProductGalleries { get; set; }
        public DbSet<MarketPlaceEZProductVarient> marketPlaceEZProductVarients { get; set; }
        public DbSet<MarketPlaceProductGallery> marketPlaceProductGalleries { get; set; }
        public DbSet<MarketPlaceProductVarient> marketPlaceProductVarients { get; set; }

        //Start by yashaswi 7-11-2019 Merchant
        public DbSet<MerchantApprovalLog> merchantApprovalLogs { get; set; }

        //By yashaswi 29-08-2019 for boosterPlan
        public DbSet<BoosterPlanMaster> BoosterPlanMaster { get; set; }
        public DbSet<BoosterPlanSubscriber> BoosterPlanSubscribers { get; set; }
        public DbSet<BoosterPlanSubscriberTranscations> planSubscriberTranscations { get; set; }
        public DbSet<BoosterPlanPayout> boosterPlanPayouts { get; set; }
        public DbSet<BoosterPlanPayoutDetails> boosterPayoutDetails { get; set; }
        public DbSet<CashbackWallet> cashbackWallets { get; set; }
        public DbSet<CashbackWalletLog> cashbackWallLog { get; set; }
        public DbSet<CashbackPointsPayout> cashbackPointsPayouts { get; set; }
        public DbSet<CashbackPointsPayoutDetail> cashbackPointsPayoutDetails { get; set; }
        //Start by yashaswi 01-07-2019 Merchant Master
        public DbSet<ServiceMaster> ServiceMasters { get; set; }
        public DbSet<CommissionMaster> CommissionMasters { get; set; }
        public DbSet<ShopTimingMaster> ShopTimingMasters { get; set; }
        public DbSet<HolidayMaster> HolidayMasters { get; set; }
        public DbSet<ServiceIncomeMaster> ServiceIncomeMasters { get; set; }
        public DbSet<ServiceIncomeMaster_Log> ServiceIncomeMasters_log { get; set; }
        public DbSet<MerchantBanner> merchantBanners { get; set; }
        public DbSet<MerchantBannerUpdateRequest> merchantBannerUpdateRequests { get; set; }
        public DbSet<MerchantsLogin> merchantLogins { get; set; }
        public DbSet<MerchantTransaction> merchantTransactions { get; set; }
        public DbSet<MerchantCommonValues> MerchantCommonValues { get; set; }
        public DbSet<MerchantTopupRecharge> merchantTopupRecharges { get; set; }
        public DbSet<MerchantTopupRechargeLog> merchantRechargeLog { get; set; }
        public DbSet<MerchantDetails> merchantDetails { get; set; }
        public DbSet<MerchantTransactionDistribution> merchantTransactionDistributions { get; set; }
        public DbSet<MerchantProfile> merchantProfiles { get; set; }
        public DbSet<MerchantPayout> merchantPayouts { get; set; }
        public DbSet<MerchantPayoutDetail> MerchantPayoutDetail { get; set; }
        public DbSet<MerchantSelfCBPoints> merchantSelfCBPoints { get; set; }
        public DbSet<MerchantNotifications> merchantNotifications { get; set; }
        public DbSet<MerchantTransactionRequest> merchantTransactionRequests { get; set; }
        
        //End

        //Start by Shaili 04-07-2019 Merchant Master
        public DbSet<Merchant> Merchants { get; set; }
        public DbSet<MerchantHoliday> MerchantHolidays { get; set; }
        public DbSet<MerchantHolidayUpdateRequest> merchantHolidayUpdates { get; set; }
        public DbSet<MerchantKYC> MerchantKYCs { get; set; }
        public DbSet<MerchantRating> MerchantRatings { get; set; }
        //End
        public DbSet<FCMUser> FCMUsers { get; set; }//Added by Yashaswi 21-6-2019 for APP notification
        public DbSet<EVWsSupplier> EVWsSuppliers { get; set; } //Added by Yashaswi 10-4-2019
        public DbSet<EVWsDV> EVWsDVs { get; set; } //Added by Yashaswi 10-4-2019
        public DbSet<RateMatrix> RateMatrix { get; set; } //Added by Yashaswi 10-4-2019
        public DbSet<RateMatrixExtension> RateMatrixExtension { get; set; } //Added by Yashaswi 10-4-2019
        public DbSet<CancelOrderReason> CancelOrderReason { get; set; }//Added by Sonali on 27-11-2018
        public DbSet<TrackSearch_Keywords> TrackSearch_Keywords { get; set; } //by Amit
        public DbSet<AccountingHead> AccountingHeads { get; set; }
        public DbSet<AccountTransaction> AccountTransactions { get; set; }
        public DbSet<Advertisement> Advertisements { get; set; }
        public DbSet<Advertiser> Advertisers { get; set; }
        public DbSet<AlgorithmList> AlgorithmLists { get; set; }
        public DbSet<APIToken> APITokens { get; set; }
        public DbSet<ApplicationAlgorithm> ApplicationAlgorithms { get; set; }
        /*Added By Pradnyakar Badge*/
        public DbSet<ApplicationDetail> ApplicationDetails { get; set; }
        /***********************************/
        public DbSet<ApplicationList> ApplicationLists { get; set; }
        public DbSet<Area> Areas { get; set; }
        public DbSet<Bank> Banks { get; set; }
        public DbSet<BankAccountType> BankAccountTypes { get; set; }
        public DbSet<Brand> Brands { get; set; }
        public DbSet<Manufacture> Manufactures { get; set; }     //Added by Priti on 18-2-2019


        public DbSet<BulkLog> BulkLogs { get; set; }
        public DbSet<BusinessDetail> BusinessDetails { get; set; }
        public DbSet<BusinessType> BusinessTypes { get; set; }

        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartLog> CartLogs { get; set; }

        /*Added By Pradnyakar Badge*/
        public DbSet<Career> Careers { get; set; }
        /***********************************/

        public DbSet<Category> Categories { get; set; }
        public DbSet<CategoryDimension> CategoryDimensions { get; set; }
        public DbSet<CategoryMaterial> CategoryMaterials { get; set; }
        public DbSet<CategorySize> CategorySizes { get; set; }
        public DbSet<CategorySpecification> CategorySpecifications { get; set; }
        public DbSet<Charge> Charges { get; set; }
        public DbSet<ChargeStage> ChargeStages { get; set; }
        public DbSet<Chat> Chats { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<Color> Colors { get; set; }
        public DbSet<Component> Components { get; set; }
        public DbSet<ComponentOffer> ComponentOffers { get; set; }
        public DbSet<CoupenList> CoupenLists { get; set; }
        public DbSet<CustomerCoupen> CustomerCoupens { get; set; }
        public DbSet<CustomerOrder> CustomerOrders { get; set; }
        public DbSet<CustomerOrderDetail> CustomerOrderDetails { get; set; }
        public DbSet<CustomerOrderDetailCall> CustomerOrderDetailCalls { get; set; }
        public DbSet<CustomerOrderDetailXML> CustomerOrderDetailXMLs { get; set; }
        public DbSet<CustomerOrderHistory> CustomerOrderHistories { get; set; }
        public DbSet<CustomerOrderOfferDetail> CustomerOrderOfferDetails { get; set; }
        public DbSet<CustomerOrderUserDefinedLog> CustomerOrderUserDefinedLogs { get; set; }
        public DbSet<CustomerRatingAndFeedback> CustomerRatingAndFeedbacks { get; set; }
        public DbSet<CustomerShippingAddress> CustomerShippingAddresses { get; set; }
        public DbSet<CustomerValletBalance> CustomerValletBalances { get; set; }
        public DbSet<DeliveryCashHandlingCharge> DeliveryCashHandlingCharges { get; set; }
        public DbSet<DeliveryDetailLog> DeliveryDetailLogs { get; set; }
        public DbSet<DeliveryOrderCashHandlingCharge> DeliveryOrderCashHandlingCharges { get; set; }
        public DbSet<DeliveryOrderDetail> DeliveryOrderDetails { get; set; }
        public DbSet<DeliveryPartner> DeliveryPartners { get; set; }
        public DbSet<DeliveryPincode> DeliveryPincodes { get; set; }
        public DbSet<DeliveryWeightSlab> DeliveryWeightSlabs { get; set; }
        public DbSet<DeviceMaster> DeviceMasters { get; set; }
        public DbSet<Dimension> Dimensions { get; set; }
        public DbSet<District> Districts { get; set; }
        public DbSet<Domain> Domains { get; set; }
        public DbSet<DomainCatgeory> DomainCatgeories { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<DeliveryBoy> DeliveryBoys { get; set; }
        public DbSet<Franchise> Franchises { get; set; }
        public DbSet<FranchiseCategory> FranchiseCategories { get; set; }
        public DbSet<FranchiseLocation> FranchiseLocations { get; set; }
        public DbSet<PaymentInTerms> PaymentInTerms { get; set; }////Added by Priti on 10/10/2018
        //public DbSet<Frenchise> Frenchises { get; set; }
        //public DbSet<FrenchiseCategory> FrenchiseCategories { get; set; }
        //public DbSet<FrenchiseLocation> FrenchiseLocations { get; set; }
        public DbSet<FrequentlyBuyTogetherProduct> FrequentlyBuyTogetherProducts { get; set; }
        public DbSet<GandhibaghTransaction> GandhibaghTransactions { get; set; }
        public DbSet<GbSetting> GbSettings { get; set; }

        public DbSet<GcmUser> GcmUsers { get; set; }
        public DbSet<GetwayPaymentTransaction> GetwayPaymentTransactions { get; set; }
        public DbSet<GoodwillOwnerPoint> GoodwillOwnerPoints { get; set; }
        public DbSet<LedgerHead> LedgerHeads { get; set; }
        public DbSet<LoginAttempt> LoginAttempts { get; set; }
        public DbSet<LoginSecurityAnswer> LoginSecurityAnswers { get; set; }
        public DbSet<LogTable> LogTables { get; set; }
        public DbSet<Material> Materials { get; set; }
        public DbSet<Menu> Menus { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<Offer> Offers { get; set; }
        public DbSet<OfferDuration> OfferDurations { get; set; }
        public DbSet<OfferZoneProduct> OfferZoneProducts { get; set; }
        public DbSet<OTP> OTPs { get; set; }
        public DbSet<OTPLog> OTPLogs { get; set; }
        public DbSet<OwnerAdvertisement> OwnerAdvertisements { get; set; }
        public DbSet<OwnerBank> OwnerBanks { get; set; }
        public DbSet<OwnerPlan> OwnerPlans { get; set; }
        public DbSet<OwnerPlanCategoryCharge> OwnerPlanCategoryCharges { get; set; }
        public DbSet<PaymentMode> PaymentModes { get; set; }
        public DbSet<PaymentData> PaymentDatas { get; set; }
        public DbSet<PersonalDetail> PersonalDetails { get; set; }
        public DbSet<Pincode> Pincodes { get; set; }
        public DbSet<Plan> Plans { get; set; }
        public DbSet<PlanBind> PlanBinds { get; set; }
        public DbSet<PlanBindCategory> PlanBindCategories { get; set; }
        public DbSet<PlanCategoryCharge> PlanCategoryCharges { get; set; }

        //--------Added By Pradnyakar --------------------
        public DbSet<PremiumShopsPriority> PremiumShopsPriorities { get; set; }
        //-----------------------End-----------------------

        public DbSet<Product> Products { get; set; }
        public DbSet<ProductBulkDetail> ProductBulkDetails { get; set; }
        public DbSet<ProductSpecification> ProductSpecifications { get; set; }
        public DbSet<ProductVarient> ProductVarients { get; set; }
        public DbSet<Rating> Ratings { get; set; }
        public DbSet<ReceiveOrderOnCall> ReceiveOrderOnCalls { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<RoleMenu> RoleMenus { get; set; }
        public DbSet<SalesRecord> SalesRecords { get; set; }
        public DbSet<Salutation> Salutations { get; set; }

        public DbSet<SchemeBudget> SchemeBudgets { get; set; }
        public DbSet<SchemeBudgetTransaction> SchemeBudgetTransactions { get; set; }

        public DbSet<SchemeType> SchemeTypes { get; set; }
        public DbSet<SecurityQuestion> SecurityQuestions { get; set; }
        public DbSet<SEO> SEOs { get; set; }
        public DbSet<Shop> Shops { get; set; }
        public DbSet<ShopComponentPrice> ShopComponentPrices { get; set; }
        public DbSet<ShopMarket> ShopMarkets { get; set; }

        //--------Added By Pradnyakar --------------------
        public DbSet<ShopMenuPriority> ShopMenuPriorities { get; set; }
        //-----------------------End-----------------------
        public DbSet<ShopPaymentMode> ShopPaymentModes { get; set; }
        public DbSet<ShopProduct> ShopProducts { get; set; }
        public DbSet<ShopProductCharge> ShopProductCharges { get; set; }
        public DbSet<ShopStock> ShopStocks { get; set; }
        public DbSet<ShopStockBulkLog> ShopStockBulkLogs { get; set; }
        public DbSet<Size> Sizes { get; set; }
        public DbSet<SourceOfInfo> SourceOfInfoes { get; set; }
        public DbSet<Specification> Specifications { get; set; }
        public DbSet<State> States { get; set; }
        public DbSet<StockComponent> StockComponents { get; set; }
        public DbSet<StockComponentOffer> StockComponentOffers { get; set; }
        public DbSet<StockComponentOfferDuration> StockComponentOfferDurations { get; set; }
        public DbSet<StockComponentPriceLog> StockComponentPriceLogs { get; set; }
        public DbSet<sysdiagram> sysdiagrams { get; set; }

        /*14-03-2016
          * Prdnyakar Badge 
          * For Taxation Work
         */
        public DbSet<TaxationBase> TaxationBases { get; set; }
        public DbSet<TaxationMaster> TaxationMasters { get; set; }
        public DbSet<TaxOnOrder> TaxOnOrders { get; set; }
        /**********************End************************************************************/
        public DbSet<TempProduct> TempProducts { get; set; }
        public DbSet<TempProductSpecification> TempProductSpecifications { get; set; }
        /*14-03-2016
          * Prdnyakar Badge 
          * For Taxation Work
         */
        public DbSet<ProductTax> ProductTaxes { get; set; }
        public DbSet<TempProductTax> TempProductTaxes { get; set; }// added by Manoj
        /**********************End************************************************************/
        public DbSet<TempProductVarient> TempProductVarients { get; set; }
        public DbSet<TempShopProduct> TempShopProducts { get; set; }
        public DbSet<TempShopStock> TempShopStocks { get; set; }
        public DbSet<TempStockComponent> TempStockComponents { get; set; }
        public DbSet<TodayScheme> TodaySchemes { get; set; }
        public DbSet<TransactionInput> TransactionInputs { get; set; }
        public DbSet<TransactionInputProcessAccount> TransactionInputProcessAccounts { get; set; }
        public DbSet<TransactionInputProcessMCO> TransactionInputProcessMCOes { get; set; }
        public DbSet<TransactionInputProcessShop> TransactionInputProcessShops { get; set; }

        public DbSet<Unit> Units { get; set; }
        public DbSet<UserAdditionalMenu> UserAdditionalMenus { get; set; }
        public DbSet<UserChat> UserChats { get; set; }
        public DbSet<UserLogin> UserLogins { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<UserTheme> UserThemes { get; set; }
        public DbSet<VehicleType> VehicleTypes { get; set; }
        public DbSet<WishList> WishLists { get; set; }
        public DbSet<ProprietoryProduct> ProprietoryProducts { get; set; }
        public DbSet<FeedbackCategary> FeedbackCategaries { get; set; }
        public DbSet<FeedbackManagment> FeedbackManagments { get; set; }
        public DbSet<FeedBackType> FeedBackTypes { get; set; }

        public DbSet<ModelLayer.Models.ViewModel.TrackOrderViewModel> TrackOrderViewModels { get; set; }
        public DbSet<ModelLayer.Models.ViewModel.ProductUploadTempViewModel> ProductUploadTempViewModels { get; set; }
        public DbSet<ModelLayer.Models.ViewModel.ChannelPartnerRegistration> ChannelPartnerRegistration { get; set; }
        //  public DbSet<ModelLayer.Models.ViewModel.PlanBindManagement> PlanBindManagements { get; set; }
        // public DbSet<ModelLayer.Models.ViewModel.PlanBindCatWiseViewModel> PlanBindCatWiseViewModels { get; set; }




        /*--------------------------------------- Start Changes made by Avi Verma 17-Sep-2015 for subscription Module  */
        public DbSet<SubscriptionEmployeeKey> SubscriptionEmployeeKeys { get; set; }
        public DbSet<SubscriptionFacility> SubscriptionFacilities { get; set; }
        public DbSet<SubscriptionPlan> SubscriptionPlans { get; set; }
        public DbSet<SubscriptionPlanDealWithCategory> SubscriptionPlanDealWithCategories { get; set; }
        public DbSet<SubscriptionPlanDealWithCategoryLog> SubscriptionPlanDealWithCategoryLogs { get; set; }
        public DbSet<SubscriptionPlanFacility> SubscriptionPlanFacilities { get; set; }
        public DbSet<SubscriptionPlanPurchasedBy> SubscriptionPlanPurchasedBies { get; set; }
        public DbSet<SubscriptionPlanUsedBy> SubscriptionPlanUsedBies { get; set; }
        /*------------------------------------------- End Changes made by Avi Verma 17-Sep-2015 for subscription Module--*/

        /*--------------- Start Changes made by Mohit Sinha 07-Oct-2015 for Corporate Module ------------------------- */
        public DbSet<CorporateCustomerShippingDeliveryDetail> CorporateCustomerShippingDeliveryDetails { get; set; }
        public DbSet<CorporateOrderShippingFacilityDetail> CorporateOrderShippingFacilityDetails { get; set; }
        public DbSet<CorporateShippingFacility> CorporateShippingFacilities { get; set; }
        /*--------------- End Changes made by Mohit Sinha 07-Oct-2015 for Corporate Module ------------------------- */

        public DbSet<ShopPriority> ShopPriorities { get; set; }


        public DbSet<DeliveryHoliday> DeliveryHolidays { get; set; }
        public DbSet<DeliverySchedule> DeliverySchedules { get; set; }
        public DbSet<OrderDeliveryScheduleDetail> OrderDeliveryScheduleDetails { get; set; }

        /*--------------- Start Changes made by Mohit Sinha 24-Dec-2015 for GB Tracks ------------------------- */
        public DbSet<GBTrack> GBTracks { get; set; }
        /*--------------- End Changes made by Mohit Sinha 24-Dec-2015 for GB Tracks ------------------------- */

        /*--------------- Start Changes made by Mohit Sinha 04-Jan-2016 for Franchise Menu ------------------------- */
        public DbSet<FranchiseMenu> FranchiseMenus { get; set; }
        /*--------------- End Changes made by Mohit Sinha 04-Jan-2016 for Franchise Menu ------------------------- */

        /*14-03-2016
          * Prdnyakar Badge 
          * For Taxation Work
         */
        public DbSet<FranchiseTaxDetail> FranchiseTaxDetails { get; set; }
        /*******************End Changes*******************************************************************************/

        /*--------------- Start Changes made by Mohit Sinha 01-Feb-2016 for Channel Partners ------------------------- */
        public DbSet<ChannelPartner> ChannelPartners { get; set; }
        /*--------------- End Changes made by Mohit Sinha 01-Feb-2016 for Channel Partners ------------------------- */

        /*--------------- Start Changes made by Snehal Shende 03-Feb-2016 for Dynamic Category Pages ------------------------- */
        public DbSet<DynamicCategoryProduct> DynamicCategoryProducts { get; set; }
        /*--------------- End Changes made by Snehal Shende 03-Feb-2016 for Dynamic Category Pages ------------------------- */

        /*--------------- Start Changes made by Snehal Shende 07-Mar-2016 for Dynamic Home Page ------------------------- */
        public DbSet<BlockItemsList> BlockItemsLists { get; set; }
        public DbSet<DesignBlockType> DesignBlockTypes { get; set; }
        /*--------------- End Changes made by Snehal Shende 07-Mar-2016 for Dynamic Home Page ------------------------- */
        public DbSet<TrackCart> TrackCarts { get; set; }
        public DbSet<TrackSearch> TrackSearches { get; set; }

        //---------- Added by Roshan Gomase---Start Full Dynamic Home Page  
        public DbSet<HomePageDynamicSectionsMaster> HomePageDynamicSectionsMasters { get; set; }

        public DbSet<HomePageDynamicSection> HomePageDynamicSection { get; set; }

        public DbSet<HomePageDynamicSectionBanner> HomePageDynamicSectionBanner { get; set; }

        public DbSet<HomePageDynamicSectionProduct> HomePageDynamicSectionProduct { get; set; }


        //---------- End by Roshan Gomase---end Full Dynamic Home Page
        //---------- Added by Roshan Gomase---Start EWalletRefund 
        public DbSet<EWalletRefund_Table> eWalletRefund_Table { get; set; }

        public DbSet<MlmWalletlog> mlmWalletlogs { get; set; }

        //---------- End by Roshan Gomase---end EWalletRefund

        //----------------- Added by Tejaswee ----------------
        public DbSet<PreviewFeatureDisplay> PreviewFeatureDisplays { get; set; }
        public DbSet<PreviewFeature> PreviewFeatures { get; set; }
        //----------------- Added by Tejaswee ----------------

        /*--------------- Start Changes made by Snehal Shende 12-May-2016 for Order Alignment  ------------------------- */
        public DbSet<CustomerOrderLog> CustomerOrderLogs { get; set; }
        public DbSet<CustomerOrderDetailLog> CustomerOrderDetailLogs { get; set; }

        /*--------------- End Changes made by Snehal Shende 12-May-2016 for ------------------------- */
        public DbSet<EarnDetail> EarnDetails { get; set; }
        public DbSet<ReferAndEarnSchema> ReferAndEarnSchemas { get; set; }
        public DbSet<ReferAndEarnTransaction> ReferAndEarnTransactions { get; set; }
        public DbSet<ReferDetail> ReferDetails { get; set; }

        //-- Add by Ashish Nagrale 22-April-2016 --//
        /*public DbSet<DeliveryBoyAttendance> DeliveryBoyAttendances { get; set; }
        public DbSet<EmployeeAssignment> EmployeeAssignment { get; set; }
        public DbSet<EmployeeAssignmentHistory> EmployeeAssignmentHistory { get; set; }
        public DbSet<EPOD> EPOD { get; set; }*/
        // Hide EPOD from Ashish for Live

        //-- Add by Ashish Nagrale For Admin Master--//
        public DbSet<HelpDeskDetails> HelpDeskDetails { get; set; }
        //-- End-- //
        //-- Add by Ashish Nagrale For Franchise wise monthly Order & GMV Target--//
        public DbSet<FranchiseOrderGMVTarget> FranchiseOrderGMVTargets { get; set; }
        //-- End-- //
        //-- Add by Ashish Nagrale For Franchise Page Message--//
        public DbSet<MessageType> MessageTypes { get; set; }
        public DbSet<Weekly_Seasona_Festival_Message> Weekly_Seasona_Festival_Message { get; set; }
        //--End-- //
        public DbSet<Sent_Mail> Sent_Mail { get; set; }
        public DbSet<Email_Type> Email_Type { get; set; }
        public DbSet<PromotionalERPPayout> PromotionalERPPayouts { get; set; } //Yashaswi 11/2/2018
        public DbSet<PromotionalERPPayoutDetails> PromotionalERPPayoutDetails { get; set; } //Yashaswi 11/2/2018
        // Added by Zubair for Inventory Management on 18-08-2017
        public DbSet<Warehouse> Warehouses { get; set; }
        public DbSet<WarehouseFranchise> WarehouseFranchises { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<PurchaseOrder> PurchaseOrders { get; set; }
        public DbSet<PurchaseOrderDetail> PurchaseOrderDetails { get; set; }
        public DbSet<WarehouseStock> WarehouseStocks { get; set; }
        public DbSet<WarehouseReorderLevel> WarehouseReorderLevels { get; set; }
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<InvoiceDetail> InvoiceDetails { get; set; }
        public DbSet<InvoiceExtraItem> InvoiceExtraItems { get; set; }
        public DbSet<TransactionType> TransactionTypes { get; set; }
        public DbSet<WarehouseFinancialTransaction> WarehouseFinancialTransactions { get; set; }
        public DbSet<WarehouseBudget> WarehouseBudgets { get; set; }
        public DbSet<WarehouseStockOrderDetailLog> WarehouseStockOrderDetailLogs { get; set; }
        public DbSet<Quotation> Quotations { get; set; }
        public DbSet<QuotationItemDetail> QuotationItemDetails { get; set; }
        public DbSet<QuotationSupplierList> QuotationSupplierLists { get; set; }
        public DbSet<QuotationReplyItem> QuotationReplyItems { get; set; }
        public DbSet<PurchaseOrderReply> PurchaseOrderReply { get; set; }
        public DbSet<PurchaseOrderReplyDetail> PurchaseOrderReplyDetails { get; set; }
        public DbSet<WarehouseReason> WarehouseReasons { get; set; } // Added by Yashaswi on 12/03/18
        public DbSet<WarehouseWastageStock> WarehouseWastageStock { get; set; } // Added by Yashaswi on 12/03/18
        public DbSet<WarehouseStockDeliveryDetail> WarehouseStockDeliveryDetails { get; set; }
        public DbSet<WarehouseReturnStock> WarehouseReturnStock { get; set; }// Added by Yashaswi on 20/03/18
        public DbSet<WarehouseReturnStockDetails> WarehouseReturnStockDetails { get; set; }// Added by Yashaswi on 20/03/18
        public DbSet<WarehouseStockLog> WarehouseStockLog { get; set; }// Added by Yashaswi on 20/03/18
        public DbSet<InvoiceAttachment> InvoiceAttachment { get; set; }// Added by Yashaswi on 26/03/18
        public DbSet<ShopStockOrderDetailLog> ShopStockOrderDetailLogs { get; set; }
        public DbSet<MarginDivision> MarginDivision { get; set; }// Added by Yashaswi on 12/4/18
        public DbSet<RateCalculation> RateCalculations { get; set; }
        public DbSet<Zone> Zones { get; set; }
        public DbSet<WarehouseZone> WarehouseZones { get; set; }
        public DbSet<WarehouseBlock> WarehouseBlocks { get; set; }
        public DbSet<WarehouseBlockLevel> WarehouseBlockLevels { get; set; }
        public DbSet<WarehouseBlockLocation> WarehouseBlockLocations { get; set; }
        public DbSet<ItemLocation> ItemLocations { get; set; }
        //public DbSet<ReservedItemLocationForFV> ReservedItemLocationForFV { get; set; }
        //public DbSet<WarehouseStockDeliveryDetailItemLocation> WarehouseStockDeliveryDetailItemLocations { get; set; }

        //End Inventory Management

        //Added by Zubair for MLM on 17-01-2018
        public DbSet<PartnerRequest> PartnerRequests { get; set; } //Yashaswi For PartnerRequestModule 9/5/2018
        public DbSet<MailReceiver> MailReceivers { get; set; }//Yashaswi For PartnerRequestModule 9/5/2018
        public DbSet<MLMWallet> MLMWallets { get; set; }
        public DbSet<MLMUser> MLMUsers { get; set; }
        public DbSet<MLMCoinRate> MLMCoinRates { get; set; }  // Added  by Amit 9/7/18
        public DbSet<MLMWallet_DirectIncome> MLMWallet_DirectIncomes { get; set; } //Yashaswi 6-8-2018
        public DbSet<LeadersPayoutMaster> LeadersPayoutMasters { get; set; }

        public DbSet<MLMAdminLogin> MLMAdminLogins { get; set; }  // Added by Amit on 5/7/18


        public DbSet<MLMWalletTransaction> MLMWalletTransactions { get; set; } // Added  by Amit 2/8/18

        public DbSet<LeadersPaymentHistory> LeadersPaymentHistorys { get; set; } // Added  by Amit 4/8/18
        public DbSet<Ezeelo_Payment_History> EzeeloPaymentHistorys { get; set; }  // Added  by Amit 4/8/18

        public DbSet<ReactivationMaster> ReactivationMasters { get; set; }  // Added  by Amit 6/8/18

        public DbSet<QRPMaster> QRPMasters { get; set; }    // Added  by Amit 6/8/18
        public DbSet<LogLeadersReactivationMaster> LogLeadersReactivationMasters { get; set; }   // Added  by Amit 6/8/18

        public DbSet<LogLeadersPayout> LogLeadersPayouts { get; set; }  // Added  by Amit 6/8/18
        public DbSet<LogQRPMaster> LogQRPMasters { get; set; }     // Added  by Amit 6/8/18
        public DbSet<Log_MLMCoinRate> Log_MLMCoinRates { get; set; } // Added  by Amit 6/8/18

        public DbSet<LeadersIncomeMaster> LeadersIncomeMasters { get; set; }  // Added  by Amit 7/8/18

        public DbSet<LogLeadersIncomeMaster> LogLeadersIncomeMasters { get; set; }   // Added  by Amit 7/8/18
        public DbSet<InActivePointsPayout> InActivePointsPayouts { get; set; }  // added by amit on 4-3-19
        public DbSet<LogMlmWalletPayout> LogMlmWalletPayouts { get; set; } // added by amit on 5-4-2019
        public DbSet<LeadersDesignationQualifierModel> LeadersDesignationQualifierModels { get; set; }

        public DbSet<LeadersMobileDisplay_Downline> LeadersMobileDisplay_Downlines { get; set; } //amit
        public DbSet<MLMUserInvites> MLMUserInvite { get; set; }
        public DbSet<KYCModel> KYCModels { get; set; }
        public DbSet<KYCApprovalLog> kYCApprovalLogs { get; set; }
        public DbSet<NetworkUserViewModel> NetworkUsersViewModel { get; set; }
        public DbSet<MLMWalletDetails> MLMWalletDetails { get; set; }
        public DbSet<EzeeMoneyPayout> EzeeMoneyPayouts { get; set; }
        public DbSet<Network_User_Extend> Network_User_Extends { get; set; }
        public DbSet<LeadersPayoutRequest> LeadersPayoutRequests { get; set; }
        public DbSet<Network_Table> NetworkTables { get; set; }
        public DbSet<EzeeMoneyPayoutDetails> EzeeMoneyPayoutDetail { get; set; }
        public DbSet<LeadersWalletPayBack> LeadersWalletPayBacks { get; set; }
        public DbSet<LeadersNotification> LeadersNotifications { get; set; }
        //End MLM

        public DbSet<TempPassword> TempPasswords { get; set; } // Added by Zubair on 11-09-2017

        public DbSet<Deal> Deals { get; set; } // Added By Sonali_27-12-2018
        public DbSet<DealProduct> DealProducts { get; set; }//Added By Sonali_27-12-2018
        public DbSet<PayubizTransaction> PayubizTransactions { get; set; }//Added for App PayubizTransaction save by Sonali on 20-12-2018
        public DbSet<DealBannerList> DealBannerLists { get; set; }//Added for App deal section bannerlist by Sonali on 07-01-2019
        public DbSet<DealCategoryList> DealCategoryLists { get; set; }//Added for App deal section CategoryList by Sonali on 10-01-2019
        public DbSet<DeliveryCharge> DeliveryCharges { get; set; }//Added for set delivery charges by Sonali on 15_01_2019
        public DbSet<Combo> Combos { get; set; }//Added for its not present by SOnali_17-01-2019
        public DbSet<ApplicationVersion> ApplicationVersion { get; set; }//Added for App by SOnali_08-02-2019
        public DbSet<LeaderVideo> LeaderVideos { get; set; }//Added for App by SOnali_23-02-2019
        public DbSet<RecentlyViewProduct> RecentlyViewProducts { get; set; }//Added for App by SOnali_07-03-2019
        public DbSet<BecomeLeader> BecomeLeaders { get; set; }//Added for App by Rumana on 07-03-2019
        public DbSet<HomePageDynamicSectionStyle> HomePageDynamicSectionStyle { get; set; }//Added for Admin by Roshan on 15-03-2019
        public DbSet<DesignationMaster> DesignationMaster { get; set; } // by lokesh for designation 
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new AccountingHeadMap());
            modelBuilder.Configurations.Add(new AccountTransactionMap());
            modelBuilder.Configurations.Add(new AdvertisementMap());
            modelBuilder.Configurations.Add(new AdvertiserMap());
            modelBuilder.Configurations.Add(new AlgorithmListMap());
            modelBuilder.Configurations.Add(new APITokenMap());
            modelBuilder.Configurations.Add(new ApplicationAlgorithmMap());
            modelBuilder.Configurations.Add(new ApplicationDetailMap());
            modelBuilder.Configurations.Add(new ApplicationListMap());
            modelBuilder.Configurations.Add(new AreaMap());
            modelBuilder.Configurations.Add(new BankMap());
            modelBuilder.Configurations.Add(new BankAccountTypeMap());
            modelBuilder.Configurations.Add(new BrandMap());
            modelBuilder.Configurations.Add(new BulkLogMap());
            modelBuilder.Configurations.Add(new BusinessDetailMap());
            modelBuilder.Configurations.Add(new BusinessTypeMap());
            modelBuilder.Configurations.Add(new CareerMap());
            modelBuilder.Configurations.Add(new CartMap());
            modelBuilder.Configurations.Add(new CartLogMap());
            modelBuilder.Configurations.Add(new CategoryMap());
            modelBuilder.Configurations.Add(new CategoryDimensionMap());
            modelBuilder.Configurations.Add(new CategoryMaterialMap());
            modelBuilder.Configurations.Add(new CategorySizeMap());
            modelBuilder.Configurations.Add(new CategorySpecificationMap());
            modelBuilder.Configurations.Add(new ChargeMap());
            modelBuilder.Configurations.Add(new ChargeStageMap());
            modelBuilder.Configurations.Add(new ChatMap());
            modelBuilder.Configurations.Add(new CityMap());
            modelBuilder.Configurations.Add(new ColorMap());
            modelBuilder.Configurations.Add(new ComponentMap());
            modelBuilder.Configurations.Add(new ComponentOfferMap());
            modelBuilder.Configurations.Add(new CoupenListMap());
            modelBuilder.Configurations.Add(new CustomerCoupenMap());
            modelBuilder.Configurations.Add(new CustomerOrderMap());
            modelBuilder.Configurations.Add(new CustomerOrderDetailMap());
            modelBuilder.Configurations.Add(new CustomerOrderDetailCallMap());
            modelBuilder.Configurations.Add(new CustomerOrderDetailXMLMap());
            modelBuilder.Configurations.Add(new CustomerOrderHistoryMap());
            modelBuilder.Configurations.Add(new CustomerOrderOfferDetailMap());
            modelBuilder.Configurations.Add(new CustomerOrderUserDefinedLogMap());
            modelBuilder.Configurations.Add(new CustomerRatingAndFeedbackMap());
            modelBuilder.Configurations.Add(new CustomerShippingAddressMap());
            modelBuilder.Configurations.Add(new CustomerValletBalanceMap());
            /*---------------------------PaperLess 22-April-2016-------------------------------------*/
            modelBuilder.Configurations.Add(new DeliveryBoyAttendanceMap());
            /*---------------------------PaperLess-------------------------------------*/
            modelBuilder.Configurations.Add(new DeliveryCashHandlingChargeMap());
            modelBuilder.Configurations.Add(new DeliveryDetailLogMap());
            modelBuilder.Configurations.Add(new DeliveryOrderCashHandlingChargeMap());
            modelBuilder.Configurations.Add(new DeliveryOrderDetailMap());
            modelBuilder.Configurations.Add(new DeliveryPartnerMap());
            modelBuilder.Configurations.Add(new DeliveryPincodeMap());
            modelBuilder.Configurations.Add(new DeliveryWeightSlabMap());
            modelBuilder.Configurations.Add(new DeviceMasterMap());
            modelBuilder.Configurations.Add(new DimensionMap());
            modelBuilder.Configurations.Add(new DistrictMap());
            modelBuilder.Configurations.Add(new DomainMap());
            modelBuilder.Configurations.Add(new DomainCatgeoryMap());
            modelBuilder.Configurations.Add(new EmployeeMap());
            /*---------------------------Task Assignment(PaperLess) By Ashish 22-April-2016 -------------------------------------*/
            /*modelBuilder.Configurations.Add(new EmployeeAssignmentMap());
            modelBuilder.Configurations.Add(new EmployeeAssignmentHistoryMap());
            modelBuilder.Configurations.Add(new EPODMap());*/
            // Hide EPOD from Ashish for Live
            /*---------------------------Task Assignment(PaperLess)-------------------------------------*/
            modelBuilder.Configurations.Add(new FeedbackCategaryMap());
            modelBuilder.Configurations.Add(new FeedbackManagmentMap());
            modelBuilder.Configurations.Add(new FeedBackTypeMap());
            modelBuilder.Configurations.Add(new FranchiseMap());
            modelBuilder.Configurations.Add(new FranchiseCategoryMap());
            modelBuilder.Configurations.Add(new FranchiseLocationMap());
            //modelBuilder.Configurations.Add(new FrenchiseMap());
            //modelBuilder.Configurations.Add(new FrenchiseCategoryMap());
            //modelBuilder.Configurations.Add(new FrenchiseLocationMap());

            /*14-03-2016  * Prdnyakar Badge  * For Taxation Work  */
            modelBuilder.Configurations.Add(new FranchiseTaxDetailMap());
            /*************************************************************/
            modelBuilder.Configurations.Add(new FrequentlyBuyTogetherProductMap());
            modelBuilder.Configurations.Add(new GandhibaghTransactionMap());
            modelBuilder.Configurations.Add(new GbSettingMap());
            modelBuilder.Configurations.Add(new GcmUserMap());
            modelBuilder.Configurations.Add(new GetwayPaymentTransactionMap());
            modelBuilder.Configurations.Add(new GoodwillOwnerPointMap());
            modelBuilder.Configurations.Add(new LedgerHeadMap());
            modelBuilder.Configurations.Add(new LoginAttemptMap());
            modelBuilder.Configurations.Add(new LoginSecurityAnswerMap());
            modelBuilder.Configurations.Add(new LogTableMap());
            modelBuilder.Configurations.Add(new MaterialMap());
            modelBuilder.Configurations.Add(new MenuMap());
            modelBuilder.Configurations.Add(new NotificationMap());
            modelBuilder.Configurations.Add(new OfferMap());
            modelBuilder.Configurations.Add(new OfferDurationMap());
            modelBuilder.Configurations.Add(new OfferZoneProductMap());
            modelBuilder.Configurations.Add(new OTPMap());
            modelBuilder.Configurations.Add(new OTPLogMap());
            modelBuilder.Configurations.Add(new OwnerAdvertisementMap());
            modelBuilder.Configurations.Add(new OwnerBankMap());
            modelBuilder.Configurations.Add(new OwnerPlanMap());
            modelBuilder.Configurations.Add(new OwnerPlanCategoryChargeMap());
            modelBuilder.Configurations.Add(new PaymentModeMap());
            modelBuilder.Configurations.Add(new PersonalDetailMap());
            modelBuilder.Configurations.Add(new PincodeMap());
            modelBuilder.Configurations.Add(new PlanMap());
            modelBuilder.Configurations.Add(new PlanBindMap());
            modelBuilder.Configurations.Add(new PlanBindCategoryMap());
            modelBuilder.Configurations.Add(new PlanCategoryChargeMap());
            //--------Added By Pradnyakar --------------------
            modelBuilder.Configurations.Add(new PremiumShopsPriorityMap());
            //----------End-----------------------------------
            modelBuilder.Configurations.Add(new ProductMap());
            modelBuilder.Configurations.Add(new ProductBulkDetailMap());
            modelBuilder.Configurations.Add(new ProductSpecificationMap());
            /*14-03-2016  * Prdnyakar Badge  * For Taxation Work  */
            modelBuilder.Configurations.Add(new ProductTaxMap());
            modelBuilder.Configurations.Add(new TempProductTaxMap());
            /*****************End*********************************/

            //--------------------- Added by Tejaswee ------------------------
            modelBuilder.Configurations.Add(new PreviewFeatureDisplayMap());
            modelBuilder.Configurations.Add(new PreviewFeatureMap());
            //--------------------- Added by Tejaswee ------------------------

            modelBuilder.Configurations.Add(new ProductVarientMap());
            modelBuilder.Configurations.Add(new RatingMap());
            modelBuilder.Configurations.Add(new ReceiveOrderOnCallMap());
            modelBuilder.Configurations.Add(new RoleMap());
            modelBuilder.Configurations.Add(new RoleMenuMap());
            modelBuilder.Configurations.Add(new SalesRecordMap());
            modelBuilder.Configurations.Add(new SalutationMap());

            modelBuilder.Configurations.Add(new SchemeBudgetMap());
            modelBuilder.Configurations.Add(new SchemeBudgetTransactionMap());

            modelBuilder.Configurations.Add(new SchemeTypeMap());
            modelBuilder.Configurations.Add(new SecurityQuestionMap());
            modelBuilder.Configurations.Add(new SEOMap());
            modelBuilder.Configurations.Add(new ShopMap());
            modelBuilder.Configurations.Add(new ShopComponentPriceMap());
            modelBuilder.Configurations.Add(new ShopMarketMap());
            modelBuilder.Configurations.Add(new ShopPaymentModeMap());
            modelBuilder.Configurations.Add(new ShopProductMap());
            modelBuilder.Configurations.Add(new ShopProductChargeMap());
            modelBuilder.Configurations.Add(new ShopStockMap());
            modelBuilder.Configurations.Add(new ShopStockBulkLogMap());
            modelBuilder.Configurations.Add(new SizeMap());
            modelBuilder.Configurations.Add(new SourceOfInfoMap());
            modelBuilder.Configurations.Add(new SpecificationMap());
            modelBuilder.Configurations.Add(new StateMap());
            modelBuilder.Configurations.Add(new StockComponentMap());
            modelBuilder.Configurations.Add(new StockComponentOfferMap());
            modelBuilder.Configurations.Add(new StockComponentOfferDurationMap());
            modelBuilder.Configurations.Add(new StockComponentPriceLogMap());
            modelBuilder.Configurations.Add(new sysdiagramMap());
            /*14-03-2016  * Prdnyakar Badge  * For Taxation Work  */
            modelBuilder.Configurations.Add(new TaxationBaseMap());
            modelBuilder.Configurations.Add(new TaxationMasterMap());
            modelBuilder.Configurations.Add(new TaxOnOrderMap());
            /*****************End*********************************/
            modelBuilder.Configurations.Add(new TempProductMap());
            modelBuilder.Configurations.Add(new TempProductSpecificationMap());
            modelBuilder.Configurations.Add(new TempProductVarientMap());
            modelBuilder.Configurations.Add(new TempShopProductMap());
            modelBuilder.Configurations.Add(new TempShopStockMap());
            modelBuilder.Configurations.Add(new TempStockComponentMap());
            modelBuilder.Configurations.Add(new TodaySchemeMap());


            modelBuilder.Configurations.Add(new TransactionInputMap());
            modelBuilder.Configurations.Add(new TransactionInputProcessAccountMap());
            modelBuilder.Configurations.Add(new TransactionInputProcessMCOMap());
            modelBuilder.Configurations.Add(new TransactionInputProcessShopMap());


            modelBuilder.Configurations.Add(new UnitMap());
            modelBuilder.Configurations.Add(new UserAdditionalMenuMap());
            modelBuilder.Configurations.Add(new UserChatMap());
            modelBuilder.Configurations.Add(new UserLoginMap());
            modelBuilder.Configurations.Add(new UserRoleMap());
            modelBuilder.Configurations.Add(new UserThemeMap());
            modelBuilder.Configurations.Add(new VehicleTypeMap());
            modelBuilder.Configurations.Add(new WishListMap());
            modelBuilder.Configurations.Add(new ProprietoryProductMap());



            /*--------------------------Subscription--------------------------------------*/
            modelBuilder.Configurations.Add(new SubscriptionEmployeeKeyMap());
            modelBuilder.Configurations.Add(new SubscriptionFacilityMap());
            modelBuilder.Configurations.Add(new SubscriptionPlanMap());
            modelBuilder.Configurations.Add(new SubscriptionPlanDealWithCategoryMap());
            modelBuilder.Configurations.Add(new SubscriptionPlanDealWithCategoryLogMap());
            modelBuilder.Configurations.Add(new SubscriptionPlanFacilityMap());
            modelBuilder.Configurations.Add(new SubscriptionPlanPurchasedByMap());
            modelBuilder.Configurations.Add(new SubscriptionPlanUsedByMap());
            /*----------------------------------------------------------------*/

            /*---------------------------Corporate-------------------------------------*/
            modelBuilder.Configurations.Add(new CorporateCustomerShippingDeliveryDetailMap());
            modelBuilder.Configurations.Add(new CorporateOrderShippingFacilityDetailMap());
            modelBuilder.Configurations.Add(new CorporateShippingFacilityMap());
            /*----------------------------------------------------------------*/

            modelBuilder.Configurations.Add(new ShopPriorityMap());

            /*---------------------------Delivery Schedule-------------------------------------*/
            modelBuilder.Configurations.Add(new DeliveryHolidayMap());
            modelBuilder.Configurations.Add(new DeliveryScheduleMap());
            modelBuilder.Configurations.Add(new OrderDeliveryScheduleDetailMap());
            /*----------------------------------------------------------------*/


            /*---------------------------GB Track-------------------------------------*/
            modelBuilder.Configurations.Add(new GBTrackMap());
            /*----------------------------------------------------------------*/
            /*---------------------------Franchise Menu-------------------------------------*/
            modelBuilder.Configurations.Add(new FranchiseMenuMap());
            /*----------------------------------------------------------------*/

            /*-----------------------------Channel Partner-------------------*/
            modelBuilder.Configurations.Add(new ChannelPartnerMap());
            /*----------------------------------------------------------------*/

            /*-----------------------------ShopMenuPriority-------------------*/
            modelBuilder.Configurations.Add(new ShopMenuPriorityMap());
            /*----------------------------------------------------------------*/

            /*-----------------------------Dynamic Category Pages-------------------*/
            modelBuilder.Configurations.Add(new DynamicCategoryProductMap());
            /*----------------------------------------------------------------*/
            /*-------------------------Added by Manoj-----For CartTrack---------------------*/
            modelBuilder.Configurations.Add(new TrackCartMap());
            modelBuilder.Configurations.Add(new TrackSearchMap());
            /*---------------------------End-----------------------------------------------*/

            /*-------------------------Added by Snehal-----For Dynamic home page---------------------*/
            modelBuilder.Configurations.Add(new BlockItemsListMap());
            modelBuilder.Configurations.Add(new DesignBlockTypeMap());
            /*---------------------------End-----------------------------------------------*/

            /*-------------------------Added by Snehal-----For Order Alignment---------------------*/
            modelBuilder.Configurations.Add(new CustomerOrderLogMap());
            modelBuilder.Configurations.Add(new CustomerOrderDetailLogMap());
            /*---------------------------End-----------------------------------------------*/
            modelBuilder.Configurations.Add(new EarnDetailMap());
            modelBuilder.Configurations.Add(new ReferAndEarnSchemaMap());
            modelBuilder.Configurations.Add(new ReferAndEarnTransactionMap());
            modelBuilder.Configurations.Add(new ReferDetailMap());

            /*-------------------------Added by Ashish-----For Admin Master---------------------*/
            modelBuilder.Configurations.Add(new HelpDeskDetailsMap());
            /*---------------------------End-----------------------------------------------*/
            /*-------------------------Added by Ashish-----For Franchise wise monthly Order & GMV Target---------------------*/
            modelBuilder.Configurations.Add(new FranchiseOrderGMVTargetMap());
            /*---------------------------End-----------------------------------------------*/
            /*-------------------------Added by Ashish-----For Franchise Page Message---------------------*/
            modelBuilder.Configurations.Add(new MessageTypeMap());
            modelBuilder.Configurations.Add(new Weekly_Seasona_Festival_MessageMap());
            /*---------------------------End-----------------------------------------------*/
            modelBuilder.Configurations.Add(new Sent_MailMap());
            modelBuilder.Configurations.Add(new Email_TypeMap());


            // Added by Zubair on 23-10-2017 for Inventory Management
            modelBuilder.Configurations.Add(new WarehouseMap());
            modelBuilder.Configurations.Add(new WarehouseFranchiseMap());
            modelBuilder.Configurations.Add(new SupplierMap());
            modelBuilder.Configurations.Add(new PurchaseOrderMap());
            modelBuilder.Configurations.Add(new PurchaseOrderDetailMap());
            modelBuilder.Configurations.Add(new WarehouseStockMap());
            modelBuilder.Configurations.Add(new WarehouseReorderLevelMap());
            modelBuilder.Configurations.Add(new InvoiceMap());
            modelBuilder.Configurations.Add(new InvoiceAttachmentMap());
            modelBuilder.Configurations.Add(new InvoiceDetailMap());
            modelBuilder.Configurations.Add(new InvoiceExtraItemMap());
            modelBuilder.Configurations.Add(new TransactionTypeMap());
            modelBuilder.Configurations.Add(new WarehouseFinancialTransactionMap());
            modelBuilder.Configurations.Add(new WarehouseBudgetMap());
            modelBuilder.Configurations.Add(new WarehouseStockOrderDetailLogMap());
            modelBuilder.Configurations.Add(new QuotationMap());
            modelBuilder.Configurations.Add(new QuotationItemDetailMap());
            modelBuilder.Configurations.Add(new QuotationSupplierListMap());
            modelBuilder.Configurations.Add(new QuotationReplyItemMap());
            modelBuilder.Configurations.Add(new PurchaseOrderReplyMap());
            modelBuilder.Configurations.Add(new PurchaseOrderReplyDetailMap());

            //End Inventory Management

            modelBuilder.Configurations.Add(new MLMWalletMap()); //Added by Zubair for MLM on 17-01-2018

            modelBuilder.Configurations.Add(new TempPasswordMap()); // Added by Zubair on 11-09-2017

            modelBuilder.Configurations.Add(new MerchantMap());
            modelBuilder.Configurations.Add(new MerchantHolidayMap());
            modelBuilder.Configurations.Add(new MerchantKYCMap());
            modelBuilder.Configurations.Add(new MerchantRatingMap());
        }

        public System.Data.Entity.DbSet<ModelLayer.Models.ViewModel.ProductApprovalViewModel> ProductApprovalViewModels { get; set; }

        public System.Data.Entity.DbSet<ModelLayer.Models.ViewModel.TrackCartReportViewModel> TrackCartReports { get; set; }

        public System.Data.Entity.DbSet<ModelLayer.Models.ViewModel.GBTrackReportViewModel> GBTrackReportViewModels { get; set; }

        public System.Data.Entity.DbSet<ModelLayer.Models.ViewModel.Report.Account.ReportTransactionInputProcessAccountViewModel> ReportTransactionInputProcessAccountViewModels { get; set; }

        public System.Data.Entity.DbSet<ModelLayer.Models.ViewModel.Scheme> Schemes { get; set; }

        public System.Data.Entity.DbSet<ModelLayer.Models.ViewModel.ReferDetailViewModel> ReferDetailViewModels { get; set; }

        public System.Data.Entity.DbSet<ModelLayer.Models.ViewModel.EarnAndReferDetailReportViewModel> EarnAndReferDetailReportViewModels { get; set; }

        //public System.Data.Entity.DbSet<ModelLayer.Models.DeliveryHolidayMessage> DeliveryHolidayMessages { get; set; }


    }
}
